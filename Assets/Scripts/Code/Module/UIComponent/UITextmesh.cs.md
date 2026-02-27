# UITextmesh.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | UITextmesh.cs |
| **路径** | Assets/Scripts/Code/Module/UIComponent/UITextmesh.cs |
| **所属模块** | 框架层 → Code/Module/UIComponent |
| **文件职责** | TextMesh Pro 文本组件，支持跳字动画、国际化 |

---

## 类/结构体说明

### UITextmesh

| 属性 | 说明 |
|------|------|
| **职责** | 封装 TMPro.TMP_Text，支持国际化、跳字动画、颜色控制 |
| **泛型参数** | 无 |
| **继承关系** | 继承自 `UIBaseContainer` |
| **实现的接口** | `II18N` |

```csharp
public class UITextmesh : UIBaseContainer, II18N
{
    // TextMesh Pro 文本组件
}
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `text` | `TMPro.TMP_Text` | `private` | TextMesh Pro 文本组件 |
| `i18nCompTouched` | `I18NText` | `private` | Unity 侧 I18N 组件 |
| `textKey` | `I18NKey` | `private` | 国际化键 |
| `keyParams` | `object[]` | `private` | 国际化参数 |
| `lastNum` | `BigNumber` | `public` | 上次显示的数字（用于动画） |
| `cancel` | `ETCancellationToken` | `private` | 动画取消令牌 |

---

## 方法说明（按重要程度排序）

### OnLanguageChange()

**签名**:
```csharp
public void OnLanguageChange()
```

**职责**: 语言变化时更新文本

**核心逻辑**:
```
1. 激活 Text 组件
2. 如果 textKey 不为 default
3. 从 I18NManager 获取翻译文本
4. 如果有参数，格式化文本
5. 设置 text.text
```

**调用者**: `I18NManager`（语言切换时广播）

---

### GetText() / SetText(string text)

**签名**:
```csharp
public string GetText()
public void SetText(string text)
```

**职责**: 获取/设置文本内容

**核心逻辑**:
```
// SetText:
1. 禁用 I18N 组件
2. 设置 textKey = default
3. 设置 text.text = text
4. 重置 lastNum = 0
```

**调用者**: 需要显示动态文本的代码

---

### SetI18NKey(I18NKey key) / SetI18NKey(I18NKey key, params object[] paras)

**签名**:
```csharp
public void SetI18NKey(I18NKey key)
public void SetI18NKey(I18NKey key, params object[] paras)
```

**职责**: 设置国际化键

**核心逻辑**:
```
1. 如果 key 为 default，设置空文本
2. 禁用 I18N 组件
3. 设置 textKey = key
4. 取消当前动画
5. 调用 SetI18NText(paras)
6. 重置 lastNum = 0
```

**调用者**: 需要显示国际化文本的代码

---

### SetTextColor(Color color) / SetTextColor(string colorstr)

**签名**:
```csharp
public void SetTextColor(Color color)
public void SetTextColor(string colorstr)
```

**职责**: 设置文本颜色

**核心逻辑**:
```
1. 激活 Text 组件
2. 解析颜色（如果传入字符串）
3. 设置 text.color = color
```

**调用者**: 需要改变文本颜色的代码

---

### SetTextWithColor(string text, string colorstr)

**签名**:
```csharp
public void SetTextWithColor(string text, string colorstr)
```

**职责**: 设置带颜色的文本（使用 HTML 标签）

**核心逻辑**:
```
1. 如果 colorstr 为空，调用 SetText(text)
2. 否则，使用 HTML 颜色标签
   text.text = $"<color={colorstr}>{text}</color>"
```

**调用者**: 需要显示带颜色文本的代码

---

### DoI18NNum(BigNumber number, bool showFlag)

**签名**:
```csharp
public async ETTask DoI18NNum(BigNumber number, bool showFlag = true)
```

**职责**: 国际化数字跳字动画

**参数**:
- `number`: 目标数字
- `showFlag`: 是否显示负号

**核心逻辑**:
```
1. 检查 textKey 是否为 default
2. 禁用 I18N 组件
3. 从 I18NManager 获取翻译文本
4. 计算差值 dis = number - lastNum
5. 取消当前动画
6. 如果差值为 0，返回
7. 创建取消令牌
8. 记录开始时间
9. 循环动画（500ms）：
   - 计算进度 progress = (timeNow - startTime) / 500f
   - 计算当前数字 this.lastNum = lastN + progress * dis
   - 翻译数字为字符串
   - 设置 text.text = string.Format(text, num)
   - 如果超过 500ms，设置 lastNum = number，跳出循环
   - 等待 35ms（30 帧）
   - 检查是否取消
```

**调用者**: 需要显示数字变化动画的场景（如金币变化）

**使用示例**:
```csharp
// 金币从 100 变化到 500
await goldText.DoI18NNum(new BigNumber(500));
```

---

### DoNum(BigNumber number, bool showFlag, Action<bool> onFlagChange)

**签名**:
```csharp
public async ETTask DoNum(BigNumber number, bool showFlag = true, Action<bool> onFlagChange = null)
```

**职责**: 数字跳字动画（非国际化）

**参数**:
- `number`: 目标数字
- `showFlag`: 是否显示负号
- `onFlagChange`: 符号变化回调

**核心逻辑**:
```
1. 禁用 I18N 组件
2. 计算差值 dis = number - lastNum
3. 取消当前动画
4. 如果差值为 0 且 number 不为 0，返回
5. 创建取消令牌
6. 记录开始时间
7. 调用 onFlagChange(lastNum > 0)
8. 循环动画（500ms）：
   - 计算进度 progress
   - 计算当前数字 num = lastN + progress * dis
   - 如果符号变化，调用 onFlagChange(num > 0)
   - 更新 this.lastNum = num
   - 翻译数字为字符串
   - 设置 text.text
   - 如果超过 500ms，设置 lastNum = number，跳出循环
   - 等待 35ms
   - 检查是否取消
9. 调用 onFlagChange(number > 0)
```

**与 DoI18NNum 的区别**: 不使用国际化，直接显示数字

---

### SetNum(BigNumber number, bool showFlag)

**签名**:
```csharp
public void SetNum(BigNumber number, bool showFlag = true)
```

**职责**: 直接设置数字（无动画）

**核心逻辑**:
```
1. 禁用 I18N 组件
2. 取消当前动画
3. 翻译数字为字符串
4. 设置 text.text
5. 更新 lastNum = number
```

**调用者**: 需要直接显示数字的场景

---

### SetTextGray(bool isGray)

**签名**:
```csharp
public void SetTextGray(bool isGray)
```

**职责**: 设置文本灰化效果

**核心逻辑**:
```
1. 获取 TextColorCtrl 组件
2. 如果 isGray=true，设置灰色
3. 否则，清除颜色
```

**调用者**: 需要禁用文本但保持可见的场景

---

### GetCharacterCount() / GetLineCount()

**签名**:
```csharp
public int GetCharacterCount()
public int GetLineCount()
```

**职责**: 获取字符数/行数

**核心逻辑**:
```
1. 激活 Text 组件
2. 返回 text.CharacterCount / text.m_textInfo.lineCount
```

**调用者**: 需要统计文本信息的代码

---

### GetLastCharacterLocalPosition() / GetCharacterLocalPosition(int index)

**签名**:
```csharp
public Vector3 GetLastCharacterLocalPosition()
public Vector3 GetCharacterLocalPosition(int index)
```

**职责**: 获取字符位置（用于特效定位）

**核心逻辑**:
```
1. 激活 Text 组件
2. 获取字符信息 text.m_textInfo.characterInfo
3. 返回指定字符的右下角坐标
```

**调用者**: 需要在字符位置生成特效的代码

---

## 阅读指引

### 建议的阅读顺序

1. **理解组件作用** - 为什么需要 UITextmesh
2. **看 SetText/SetI18NKey** - 理解文本设置
3. **看 DoNum/DoI18NNum** - 理解跳字动画
4. **了解字符位置** - 理解特效定位

### 最值得学习的技术点

1. **跳字动画**: 500ms 平滑过渡数字变化
2. **取消令牌**: ETCancellationToken 支持中断动画
3. **国际化支持**: II18N 接口自动响应语言切换
4. **字符定位**: 获取字符坐标用于特效

---

## 使用示例

### 示例 1: 金币显示（带动画）

```csharp
public class UIGoldView : UIBaseView, IOnCreate
{
    private UITextmesh goldText;
    
    public void OnCreate()
    {
        goldText = AddComponent<UITextmesh>("GoldText");
        goldText.SetI18NKey(I18NKey.UI_Gold_Format);
    }
    
    public void SetGold(BigNumber gold)
    {
        // 带跳字动画
        goldText.DoI18NNum(gold).Coroutine();
    }
}
```

### 示例 2: 伤害数字（特效定位）

```csharp
public void ShowDamage(int damage)
{
    damageText.SetText(damage.ToString());
    
    // 获取最后一个字符位置
    Vector3 pos = damageText.GetLastCharacterLocalPosition();
    
    // 在字符位置生成特效
    EffectManager.Instance.PlayEffect("Effects/Damage", pos);
}
```

### 示例 3: 文本灰化

```csharp
// 灰化文本（禁用状态）
titleText.SetTextGray(isGray: true);

// 恢复
titleText.SetTextGray(isGray: false);
```

---

## 相关文档

- [UIBaseContainer.cs.md](./UIBaseContainer.cs.md) - UI 容器基类
- [UIText.cs.md](./UIText.cs.md) - 普通文本组件
- [I18NManager.cs.md](../I18N/I18NManager.cs.md) - 国际化管理器

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
