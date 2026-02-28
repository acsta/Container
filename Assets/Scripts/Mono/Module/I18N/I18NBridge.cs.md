# I18NBridge.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|------|
| **文件名** | I18NBridge.cs |
| **路径** | Assets/Scripts/Mono/Module/I18N/I18NBridge.cs |
| **所属模块** | 框架层 → Mono/Module/I18N |
| **文件职责** | 提供多语言桥接，统一管理语言切换和文本获取 |

---

## 类/结构体说明

### I18NBridge

| 属性 | 说明 |
|------|------|
| **职责** | 单例类，提供多语言文本获取和语言切换事件 |
| **泛型参数** | 无 |
| **继承关系** | 无 |
| **实现的接口** | 无 |

**设计模式**: 单例模式 + 桥接模式

```csharp
// 获取文本
string text = I18NBridge.Instance.GetText("UI_StartGame");

// 切换语言
I18NBridge.Instance.GetValueByKey = GetEnglishText;
```

---

## 字段与属性

### Instance

| 属性 | 值 |
|------|------|
| **类型** | `I18NBridge` |
| **访问级别** | `public static` |
| **说明** | 单例实例，全局访问点 |

---

### OnLanguageChangeEvt

| 属性 | 值 |
|------|------|
| **类型** | `Action` |
| **访问级别** | `public` |
| **说明** | 语言切换事件 |

**调用者**: 切换语言时手动触发

**订阅者**: I18NText 等需要响应语言切换的组件

---

### GetValueByKey

| 属性 | 值 |
|------|------|
| **类型** | `Func<string, string>` |
| **访问级别** | `public` |
| **说明** | 通过键值获取文本的委托 |

**用途**: 设置多语言文本查找逻辑

**设置示例**:
```csharp
// 中文
I18NBridge.Instance.GetValueByKey = (key) =>
{
    return chineseTexts.TryGetValue(key, out var text) ? text : key;
};

// 英文
I18NBridge.Instance.GetValueByKey = (key) =>
{
    return englishTexts.TryGetValue(key, out var text) ? text : key;
};
```

---

## 方法说明

### GetText

**签名**:
```csharp
public string GetText(string i18NKey)
```

**职责**: 通过键值获取多语言文本

**核心逻辑**:
```
1. 调用 GetValueByKey?.Invoke(i18NKey)
2. 返回文本（如果委托未设置，返回 null）
```

**参数**:
| 参数名 | 类型 | 说明 |
|--------|------|------|
| `i18NKey` | `string` | 多语言键值 |

**返回值**: `string` - 对应语言的文本

**调用者**: I18NText, 代码中需要多语言文本的地方

**使用示例**:
```csharp
// 在代码中获取多语言文本
string startGameText = I18NBridge.Instance.GetText("UI_StartGame");
button.GetComponent<Text>().text = startGameText;
```

---

## 使用示例

### 示例 1: 初始化多语言

```csharp
// 游戏启动时初始化
Dictionary<string, string> chineseTexts = new Dictionary<string, string>
{
    { "UI_StartGame", "开始游戏" },
    { "UI_Settings", "设置" },
    { "UI_Exit", "退出" },
    { "MSG_Welcome", "欢迎来到游戏！" }
};

// 设置为中文
I18NBridge.Instance.GetValueByKey = (key) =>
{
    return chineseTexts.TryGetValue(key, out var text) ? text : key;
};
```

### 示例 2: 切换语言

```csharp
public void SwitchToEnglish()
{
    Dictionary<string, string> englishTexts = new Dictionary<string, string>
    {
        { "UI_StartGame", "Start Game" },
        { "UI_Settings", "Settings" },
        { "UI_Exit", "Exit" },
        { "MSG_Welcome", "Welcome to the game!" }
    };
    
    I18NBridge.Instance.GetValueByKey = (key) =>
    {
        return englishTexts.TryGetValue(key, out var text) ? text : key;
    };
    
    // 触发语言切换事件
    I18NBridge.Instance.OnLanguageChangeEvt?.Invoke();
    // 所有 I18NText 组件会自动更新文本
}
```

### 示例 3: 动态加载语言包

```csharp
public async ETTask LoadLanguage(string languageCode)
{
    // 从服务器或本地加载语言包
    string json = await LoadLanguageFile(languageCode);
    Dictionary<string, string> texts = JsonHelper.FromJson<Dictionary<string, string>>(json);
    
    I18NBridge.Instance.GetValueByKey = (key) =>
    {
        return texts.TryGetValue(key, out var text) ? text : key;
    };
    
    I18NBridge.Instance.OnLanguageChangeEvt?.Invoke();
}
```

### 示例 4: 带默认值的多语言

```csharp
I18NBridge.Instance.GetValueByKey = (key) =>
{
    if (texts.TryGetValue(key, out var text))
        return text;
    
    // 找不到时返回键值本身（便于调试）
    Log.Warning($"未找到多语言文本：{key}");
    return key;
};
```

---

## 设计要点

### 为什么使用委托？

```csharp
public Func<string, string> GetValueByKey;
```

**优势**:
1. **灵活性**: 可以设置任意查找逻辑
2. **解耦**: I18NBridge 不关心文本存储方式
3. **可扩展**: 支持字典、配置文件、服务器等多种数据源

### 为什么需要事件？

```csharp
public Action OnLanguageChangeEvt;
```

**用途**:
- 通知所有订阅者语言已切换
- I18NText 等组件自动更新文本
- 无需手动刷新每个 UI 元素

### 单例模式

```csharp
public static I18NBridge Instance { get; private set; } = new I18NBridge();
```

**原因**:
- 全局唯一实例
- 任何地方都可以访问
- 便于管理语言状态

---

## 相关文档

- [I18NText.cs.md](./I18NText.cs.md) - 多语言文本组件
- [I18NKey.cs.md](../../Const/I18NKey.cs.md) - 多语言键值定义

---

*文档生成时间：2026-02-28 | OpenClaw AI 助手*
