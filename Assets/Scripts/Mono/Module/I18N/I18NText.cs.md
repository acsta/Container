# I18NText.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|------|
| **文件名** | I18NText.cs |
| **路径** | Assets/Scripts/Mono/Module/I18N/I18NText.cs |
| **所属模块** | 框架层 → Mono/Module/I18N |
| **文件职责** | 提供多语言文本组件，自动根据语言设置更新 UI 文本 |

---

## 类/结构体说明

### I18NText

| 属性 | 说明 |
|------|------|
| **职责** | MonoBehaviour 组件，挂载到 Text 对象上，自动获取并更新多语言文本 |
| **泛型参数** | 无 |
| **继承关系** | 继承 `MonoBehaviour` |
| **实现的接口** | 无 |

**设计模式**: 观察者模式 + 组件模式

```csharp
// 使用方式
// 1. 在 Unity Inspector 中设置 Key
// 2. 挂载到 Text 或 TextMeshPro 对象上
// 3. 自动根据语言设置更新文本
```

---

## 字段与属性

### key

| 属性 | 值 |
|------|------|
| **类型** | `string` |
| **访问级别** | `public` |
| **说明** | 多语言文本的键值 |

**用途**: 在 I18NBridge 中查找对应语言的文本

**Inspector 设置**:
```
I18NText 组件
└── Key: "UI_StartGame"
```

---

### m_Text

| 属性 | 值 |
|------|------|
| **类型** | `Text` |
| **访问级别** | `private` |
| **说明** | Unity UI Text 组件引用 |

---

### m_MeshText

| 属性 | 值 |
|------|------|
| **类型** | `TMPro.TMP_Text` |
| **访问级别** | `private` |
| **说明** | TextMeshPro Text 组件引用 |

---

## 方法说明

### Awake

**签名**:
```csharp
void Awake()
```

**职责**: 初始化，获取 Text 组件引用

**核心逻辑**:
```
1. 获取 Text 组件（如果存在）
2. 获取 TextMeshPro 组件（如果存在）
```

**调用者**: Unity 生命周期

---

### OnEnable

**签名**:
```csharp
private void OnEnable()
```

**职责**: 启用时初始化文本并订阅语言切换事件

**核心逻辑**:
```
1. 调用 OnSwitchLanguage() 更新文本
2. 订阅 I18NBridge.OnLanguageChangeEvt 事件
```

**调用者**: Unity 生命周期

---

### OnDisable

**签名**:
```csharp
private void OnDisable()
```

**职责**: 禁用时取消订阅语言切换事件

**核心逻辑**:
```
1. 取消订阅 I18NBridge.OnLanguageChangeEvt 事件
```

**调用者**: Unity 生命周期

---

### OnSwitchLanguage

**签名**:
```csharp
private void OnSwitchLanguage()
```

**职责**: 语言切换时更新文本

**核心逻辑**:
```
1. 调用 I18NBridge.Instance.GetText(key) 获取文本
2. 如果 m_Text 存在，设置 m_Text.text
3. 如果 m_MeshText 存在，设置 m_MeshText.text
```

**调用者**: OnEnable, I18NBridge.OnLanguageChangeEvt 事件

---

## 使用示例

### 示例 1: 基础使用

1. 在 Unity 中创建 UI Text 对象
2. 挂载 `I18NText` 组件
3. 在 Inspector 中设置 Key（如 "UI_StartGame"）
4. 运行时自动显示对应语言的文本

### 示例 2: 切换语言

```csharp
// 切换语言
I18NBridge.Instance.GetValueByKey = GetChineseText;  // 设置为中文
// I18NText 组件会自动收到 OnLanguageChangeEvt 事件并更新文本
```

### 示例 3: 多语言配置

```csharp
// 初始化多语言
Dictionary<string, string> chineseTexts = new Dictionary<string, string>
{
    { "UI_StartGame", "开始游戏" },
    { "UI_Settings", "设置" },
    { "UI_Exit", "退出" }
};

I18NBridge.Instance.GetValueByKey = (key) =>
{
    return chineseTexts.TryGetValue(key, out var text) ? text : key;
};
```

---

## 设计要点

### 为什么支持两种 Text 组件？

```csharp
m_Text = GetComponent<Text>();         // Unity UI
m_MeshText = GetComponent<TMPro.TMP_Text>();  // TextMeshPro
```

**原因**:
- Unity UI Text: 传统 UI 系统
- TextMeshPro: 现代 UI 系统，效果更好
- 同时支持，兼容不同项目

### 为什么在 OnEnable 订阅事件？

```csharp
private void OnEnable()
{
    OnSwitchLanguage();  // 立即更新
    I18NBridge.Instance.OnLanguageChangeEvt += OnSwitchLanguage;  // 订阅
}

private void OnDisable()
{
    I18NBridge.Instance.OnLanguageChangeEvt -= OnSwitchLanguage;  // 取消订阅
}
```

**原因**:
- 对象启用时立即显示正确文本
- 监听语言切换事件
- 对象禁用时取消订阅，避免内存泄漏

---

## 相关文档

- [I18NBridge.cs.md](./I18NBridge.cs.md) - 多语言桥接
- [I18NKey.cs.md](../../Const/I18NKey.cs.md) - 多语言键值定义

---

*文档生成时间：2026-02-28 | OpenClaw AI 助手*
