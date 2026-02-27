# I18NManager.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | I18NManager.cs |
| **路径** | Assets/Scripts/Code/Module/I18N/I18NManager.cs |
| **所属模块** | 框架层 → Code/Module/I18N |
| **文件职责** | 国际化管理器，支持多语言切换和数值缩写 |

---

## 类/结构体说明

### I18NManager

| 属性 | 说明 |
|------|------|
| **职责** | 管理多语言切换、文本翻译、数值缩写 |
| **泛型参数** | 无 |
| **继承关系** | 无 |
| **实现的接口** | `IManager` |

```csharp
public class I18NManager : IManager
{
    // 国际化管理器
}
```

---

## 字段与属性

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `Instance` | `I18NManager` | `public static` | 单例实例 |
| `OnLanguageChangeEvt` | `event Action` | `public` | 语言切换事件 |
| `CurLangType` | `LangType` | `public` | 当前语言类型 |
| `i18nTextKeyDic` | `Dictionary<int, string>` | `private` | 多语言文本字典 |
| `numTemp` | `Dictionary<int, I18NKey>` | `private` | 数值单位缓存 |

---

## 方法说明（按重要程度排序）

### Init() / Destroy()

**签名**:
```csharp
public void Init()
public void Destroy()
```

**职责**: 初始化和销毁

**核心逻辑**:
```
// Init:
1. 设置 Instance = this
2. 创建 numTemp 缓存
3. 设置 I18NBridge.GetValueByKey 回调
4. 从缓存读取语言类型
5. 调用 InitAsync() 加载配置

// InitAsync:
1. 加载对应语言的 I18NConfigCategory
2. 遍历配置，填充 i18nTextKeyDic

// Destroy:
1. 清空缓存和事件
2. 清空字典
```

**调用者**: `ManagerProvider.Init()`

---

### I18NGetText(I18NKey key) / I18NGetText(string key)

**签名**:
```csharp
public string I18NGetText(I18NKey key)
public string I18NGetText(string key)
```

**职责**: 获取翻译文本

**参数**:
- `key`: 国际化键（枚举或字符串）

**返回**: 翻译后的文本（找不到返回 key）

**核心逻辑**:
```
1. 从 i18nTextKeyDic 查找
2. 如果找到，返回翻译文本
3. 如果未找到，记录错误，返回 key 字符串
```

**调用者**: 需要显示翻译文本的代码

**使用示例**:
```csharp
string text = I18NManager.Instance.I18NGetText(I18NKey.Global_Btn_Confirm);
// 中文 → "确定"
// 英文 → "Confirm"
```

---

### I18NGetParamText(I18NKey key, params object[] paras)

**签名**:
```csharp
public string I18NGetParamText(I18NKey key, params object[] paras)
```

**职责**: 获取带参数的翻译文本

**参数**:
- `key`: 国际化键
- `paras`: 格式化参数

**返回**: 格式化后的翻译文本

**核心逻辑**:
```
1. 从字典获取翻译文本
2. 如果有参数，调用 string.Format
3. 返回结果
```

**调用者**: 需要带参数翻译的代码

**使用示例**:
```csharp
// 配置："获得{0}金币"
string text = I18NManager.Instance.I18NGetParamText(
    I18NKey.Reward_Gold, 
    1000
);
// 中文 → "获得 1000 金币"
// 英文 → "Get 1000 Gold"
```

---

### I18NTryGetText(I18NKey key, out string result)

**签名**:
```csharp
public bool I18NTryGetText(I18NKey key, out string result)
```

**职责**: 尝试获取翻译文本（不报错）

**返回**: true=成功，false=失败

**与 I18NGetText 的区别**: 未找到时不记录错误

---

### SwitchLanguage(int langType)

**签名**:
```csharp
public async ETTask SwitchLanguage(int langType)
```

**职责**: 切换语言

**参数**:
- `langType`: 目标语言类型（LangType 枚举值）

**核心逻辑**:
```
1. 清空 numTemp 缓存
2. 保存语言类型到缓存
3. 更新 CurLangType
4. 重新加载对应语言的配置
5. 清空并重建 i18nTextKeyDic
6. 触发语言切换事件
```

**事件触发**:
```csharp
I18NBridge.Instance.OnLanguageChangeEvt?.Invoke();
OnLanguageChangeEvt?.Invoke();
```

**调用者**: 语言设置界面

**使用示例**:
```csharp
// 切换到英文
await I18NManager.Instance.SwitchLanguage((int)LangType.English);

// 切换到中文
await I18NManager.Instance.SwitchLanguage((int)LangType.Chinese);
```

---

### RegisterI18NEntity(II18N entity) / RemoveI18NEntity(II18N entity)

**签名**:
```csharp
public void RegisterI18NEntity(II18N entity)
public void RemoveI18NEntity(II18N entity)
```

**职责**: 注册/移除国际化实体

**核心逻辑**:
```
// Register:
1. 订阅 OnLanguageChange 事件

// Remove:
1. 取消订阅 OnLanguageChange 事件
```

**调用者**: 实现了 II18N 接口的 UI 组件

---

### TranslateMoneyToStr(BigNumber num, minUseInteger, onePoint)

**签名**:
```csharp
public string TranslateMoneyToStr(BigNumber num, bool minUseInteger = false, PointType onePoint = PointType.Two)
```

**职责**: 转换金钱为缩写字符串

**参数**:
- `num`: 金额
- `minUseInteger`: 未缩写时是否保留整数
- `onePoint`: 保留小数位数

**返回**: 缩写后的字符串

**核心逻辑**:
```
1. 根据语言选择进制：
   - 中文：10000 进制（万、亿）
   - 英文：1000 进制（K、M、B）
2. 调用 GetTransNum 处理
3. 返回结果
```

**缩写规则**:
```
中文:
- 10000 → 1 万
- 100000000 → 1 亿

英文:
- 1000 → 1K
- 1000000 → 1M
- 1000000000 → 1B
```

**调用者**: 显示金钱的 UI

**使用示例**:
```csharp
BigNumber money = new BigNumber(15000000);
string text = I18NManager.Instance.TranslateMoneyToStr(money);
// 中文 → "1500.00 万"
// 英文 → "15.00M"
```

---

### ApproximateMoneyToStr(BigNumber num, ApproximateType approximateType)

**签名**:
```csharp
public string ApproximateMoneyToStr(BigNumber num, ApproximateType approximateType)
```

**职责**: 转换金钱为缩写并取整

**参数**:
- `num`: 金额
- `approximateType`: 取整类型（Cell=四舍五入，Floor=向下取整）

**返回**: 缩写并取整后的字符串

**与 TranslateMoneyToStr 的区别**: 自动取整

---

## 枚举类型

### PointType

| 值 | 说明 |
|------|------|
| `None` | 不保留小数 |
| `One` | 保留 1 位小数 |
| `Two` | 保留 2 位小数 |

### ApproximateType

| 值 | 说明 |
|------|------|
| `Cell` | 四舍五入 |
| `Floor` | 向下取整 |

---

## 阅读指引

### 建议的阅读顺序

1. **理解管理器作用** - I18NManager 管理什么
2. **看 I18NGetText** - 理解文本翻译
3. **看 SwitchLanguage** - 理解语言切换
4. **看 TranslateMoneyToStr** - 理解数值缩写

### 最值得学习的技术点

1. **多语言支持**: 动态加载语言配置
2. **事件机制**: OnLanguageChangeEvt 通知 UI 更新
3. **数值缩写**: 中英文不同进制（万/K、亿/M）
4. **缓存优化**: numTemp 缓存单位 Key

---

## 使用示例

### 示例 1: 获取翻译文本

```csharp
// 简单翻译
string confirmText = I18NManager.Instance.I18NGetText(I18NKey.Global_Btn_Confirm);

// 带参数翻译
string rewardText = I18NManager.Instance.I18NGetParamText(
    I18NKey.Reward_Gold, 
    1000
);
```

### 示例 2: 切换语言

```csharp
// 切换到英文
await I18NManager.Instance.SwitchLanguage((int)LangType.English);

// 监听语言切换
I18NManager.Instance.OnLanguageChangeEvt += () =>
{
    Log.Info("语言已切换");
    UpdateUIText();
};
```

### 示例 3: 显示金钱

```csharp
BigNumber money = new BigNumber(15000000);

// 标准缩写（保留 2 位小数）
string text1 = I18NManager.Instance.TranslateMoneyToStr(money);
// 中文 → "1500.00 万"

// 不保留小数
string text2 = I18NManager.Instance.TranslateMoneyToStr(money, onePoint: PointType.None);
// 中文 → "1500 万"

// 取整缩写
string text3 = I18NManager.Instance.ApproximateMoneyToStr(money, ApproximateType.Floor);
// 中文 → "1500 万"
```

### 示例 4: 实现 II18N 接口

```csharp
public class UIText : UIBaseContainer, II18N
{
    public void OnLanguageChange()
    {
        // 语言切换时更新文本
        UpdateText();
    }
}

// 注册到 I18NManager
I18NManager.Instance.RegisterI18NEntity(uiText);
```

---

## 相关文档

- [I18NConfig.cs.md](./I18NConfig.cs.md) - 国际化配置
- [II18N.cs.md](./II18N.cs.md) - 国际化接口
- [I18NKey.cs.md](../Const/I18NKey.cs.md) - 国际化键枚举

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
