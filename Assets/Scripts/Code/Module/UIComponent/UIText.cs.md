# UIText.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | UIText.cs |
| **路径** | Assets/Scripts/Code/Module/UIComponent/UIText.cs |
| **所属模块** | 框架层 → Code/Module/UIComponent |
| **文件职责** | UI 文本组件，封装 Text，支持国际化 |

---

## 类/结构体说明

### UIText

| 属性 | 说明 |
|------|------|
| **职责** | 封装 Unity Text，支持国际化（I18N） |
| **泛型参数** | 无 |
| **继承关系** | 继承自 `UIBaseContainer` |
| **实现的接口** | `II18N` |

```csharp
public class UIText : UIBaseContainer, II18N
{
    // UI 文本组件
}
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `text` | `Text` | `private` | Unity Text 组件 |
| `i18nCompTouched` | `I18NText` | `private` | Unity 侧 I18N 组件 |
| `textKey` | `I18NKey` | `private` | 国际化键 |
| `keyParams` | `object[]` | `private` | 国际化参数 |

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

### GetText()

**签名**:
```csharp
public string GetText()
```

**职责**: 获取当前文本

**返回**: 当前显示的文本

**核心逻辑**:
```
1. 激活 Text 组件
2. 返回 text.text
```

**调用者**: 需要读取文本内容的代码

---

### SetText(string text)

**签名**:
```csharp
public void SetText(string text)
```

**职责**: 设置文本内容（非国际化）

**参数**:
- `text`: 要显示的文本

**核心逻辑**:
```
1. 禁用 I18N 组件（防止被覆盖）
2. 设置 textKey = default
3. 设置 text.text = text
```

**调用者**: 需要显示动态文本的代码

**使用示例**:
```csharp
// 显示玩家昵称
nicknameText.SetText(playerName);

// 显示数字
countText.SetText(itemCount.ToString());
```

---

### SetI18NKey(I18NKey key)

**签名**:
```csharp
public void SetI18NKey(I18NKey key)
```

**职责**: 设置国际化键（无参数）

**参数**:
- `key`: 国际化键

**核心逻辑**:
```
1. 如果 key 为 default，设置空文本
2. 禁用 I18N 组件
3. 设置 textKey = key
4. 调用 SetI18NText()
```

**调用者**: 需要显示国际化文本的代码

**使用示例**:
```csharp
// 显示"确定"
confirmText.SetI18NKey(I18NKey.Global_Btn_Confirm);

// 显示"取消"
cancelText.SetI18NKey(I18NKey.Global_Btn_Cancel);
```

---

### SetI18NKey(I18NKey key, params object[] paras)

**签名**:
```csharp
public void SetI18NKey(I18NKey key, params object[] paras)
```

**职责**: 设置国际化键（带参数）

**参数**:
- `key`: 国际化键
- `paras`: 格式化参数

**核心逻辑**:
```
1. 如果 key 为 default，设置空文本
2. 禁用 I18N 组件
3. 设置 textKey = key
4. 保存 keyParams = paras
5. 调用 SetI18NText(paras)
```

**调用者**: 需要显示带参数国际化文本的代码

**使用示例**:
```csharp
// 显示"欢迎来到 {0}"
welcomeText.SetI18NKey(I18NKey.Welcome_Message, playerName);

// 显示"获得 {0} 金币"
rewardText.SetI18NKey(I18NKey.Reward_Gold, goldCount);
```

---

### SetI18NText(params object[] paras)

**签名**:
```csharp
public void SetI18NText(params object[] paras)
```

**职责**: 设置国际化文本（使用已保存的 key）

**参数**:
- `paras`: 格式化参数

**核心逻辑**:
```
1. 检查 textKey 是否为 default
2. 禁用 I18N 组件
3. 保存 keyParams = paras
4. 从 I18NManager 获取翻译文本
5. 如果有参数，格式化文本
6. 设置 text.text
```

**调用者**: `SetI18NKey()`, `OnLanguageChange()`

---

### SetTextColor(Color color)

**签名**:
```csharp
public void SetTextColor(Color color)
```

**职责**: 设置文本颜色

**参数**:
- `color`: 颜色

**核心逻辑**:
```
1. 激活 Text 组件
2. 设置 text.color = color
```

**调用者**: 需要改变文本颜色的代码

**使用示例**:
```csharp
// 设置红色
titleText.SetTextColor(Color.red);

// 设置十六进制颜色
titleText.SetTextColor(new Color(1, 0, 0));  // 红色
```

---

### SetTextWithColor(string text, string colorstr)

**签名**:
```csharp
public void SetTextWithColor(string text, string colorstr)
```

**职责**: 设置带颜色的文本（使用 HTML 标签）

**参数**:
- `text`: 文本内容
- `colorstr`: 颜色字符串

**核心逻辑**:
```
1. 如果 colorstr 为空，调用 SetText(text)
2. 否则，使用 HTML 颜色标签
   text.text = $"<color={colorstr}>{text}</color>"
```

**调用者**: 需要显示带颜色文本的代码

**使用示例**:
```csharp
// 显示红色文本
tipText.SetTextWithColor("重要提示", "red");

// 显示十六进制颜色
tipText.SetTextWithColor("重要提示", "#FF0000");
```

---

## 阅读指引

### 建议的阅读顺序

1. **理解文本组件作用** - 为什么需要 UIText
2. **看 SetText** - 理解普通文本设置
3. **看 SetI18NKey** - 理解国际化文本
4. **看 OnLanguageChange** - 理解语言切换

### 最值得学习的技术点

1. **国际化支持**: II18N 接口自动响应语言切换
2. **参数格式化**: 支持 string.Format 参数
3. **I18N 组件管理**: 自动禁用 Unity 侧 I18NText 组件
4. **HTML 颜色标签**: 支持 Unity Rich Text

---

## 使用示例

### 示例 1: 普通文本

```csharp
public class UIItemView : UIBaseView, IOnCreate
{
    private UIText itemName;
    private UIText itemCount;
    
    public void OnCreate()
    {
        itemName = AddComponent<UIText>("Name");
        itemCount = AddComponent<UIText>("Count");
    }
    
    public void SetItem(ItemConfig config, int count)
    {
        // 显示物品名称（国际化）
        itemName.SetI18NKey(config.NameKey);
        
        // 显示数量（动态文本）
        itemCount.SetText(count.ToString());
    }
}
```

### 示例 2: 带参数的国际化文本

```csharp
// 显示"欢迎来到 {0}"
welcomeText.SetI18NKey(I18NKey.Welcome_Message, playerName);

// 显示"获得 {0} 金币"
rewardText.SetI18NKey(I18NKey.Reward_Gold, goldCount);

// 显示"第 {0} 关，难度 {1}"
levelText.SetI18NKey(I18NKey.Level_Info, levelId, difficulty);
```

### 示例 3: 文本颜色

```csharp
// 设置颜色
titleText.SetTextColor(Color.red);

// 设置带颜色的文本
tipText.SetTextWithColor("重要提示", "red");

// 根据条件设置颜色
if (itemCount > 100)
{
    countText.SetTextColor(new Color(1, 0.8f, 0));  // 金色
}
else
{
    countText.SetTextColor(Color.white);
}
```

### 示例 4: 语言切换响应

```csharp
// 当用户切换语言时，I18NManager 会广播
// 所有实现了 II18N 接口的组件会自动更新

// 例如：从中文切换到英文
// "确定" → "Confirm"
// "取消" → "Cancel"
// "欢迎来到 {0}" → "Welcome, {0}"
```

---

## 相关文档

- [UIBaseContainer.cs.md](./UIBaseContainer.cs.md) - UI 容器基类
- [UIImage.cs.md](./UIImage.cs.md) - UI 图片组件
- [I18NManager.cs.md](../I18N/I18NManager.cs.md) - 国际化管理器

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
