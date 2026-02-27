# UIInput.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | UIInput.cs |
| **路径** | Assets/Scripts/Code/Module/UIComponent/UIInput.cs |
| **所属模块** | 框架层 → Code/Module/UIComponent |
| **文件职责** | UI 输入框组件，封装 InputField |

---

## 类/结构体说明

### UIInput

| 属性 | 说明 |
|------|------|
| **职责** | 封装 Unity InputField，提供文本输入和事件监听 |
| **泛型参数** | 无 |
| **继承关系** | 继承自 `UIBaseContainer` |
| **实现的接口** | `IOnDestroy` |

```csharp
public class UIInput : UIBaseContainer, IOnDestroy
{
    // UI 输入框组件
}
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `input` | `InputField` | `private` | Unity InputField 组件 |
| `onValueChange` | `UnityAction<string>` | `private` | 值变化事件回调 |
| `onEndEdit` | `UnityAction<string>` | `private` | 编辑结束事件回调 |

---

## 方法说明（按重要程度排序）

### OnDestroy()

**签名**:
```csharp
public void OnDestroy()
```

**职责**: 销毁时清理资源

**核心逻辑**:
```
1. 移除值变化事件监听 RemoveOnValueChanged()
2. 移除编辑结束事件监听 RemoveOnEndEdit()
```

**调用者**: `UIBaseContainer.BeforeOnDestroy()`

---

### GetText() / SetText(string text)

**签名**:
```csharp
public string GetText()
public void SetText(string text)
```

**职责**: 获取/设置输入框文本

**核心逻辑**:
```
1. 激活 InputField 组件
2. 获取/设置 input.text
```

**调用者**: 需要读取/设置输入内容的代码

**使用示例**:
```csharp
// 获取输入内容
string playerName = nameInput.GetText();

// 设置默认值
nameInput.SetText("Player1");
```

---

### SetOnValueChanged(Action func)

**签名**:
```csharp
public void SetOnValueChanged(Action func)
```

**职责**: 设置值变化事件回调（每次输入触发）

**参数**:
- `func`: 值变化回调函数

**核心逻辑**:
```
1. 激活 InputField 组件
2. 移除旧的监听
3. 包装回调（转换为 UnityAction<string>）
4. 保存 onValueChange
5. 添加新的监听
```

**调用者**: 需要监听输入变化的代码

**使用示例**:
```csharp
// 监听输入变化
nameInput.SetOnValueChanged(() =>
{
    string text = nameInput.GetText();
    Log.Info($"输入内容：{text}");
    ValidateName(text);
});
```

---

### SetOnEndEdit(Action func)

**签名**:
```csharp
public void SetOnEndEdit(Action func)
```

**职责**: 设置编辑结束事件回调（失去焦点或按 Enter 触发）

**参数**:
- `func`: 编辑结束回调函数

**核心逻辑**:
```
1. 激活 InputField 组件
2. 移除旧的监听
3. 包装回调（转换为 UnityAction<string>）
4. 保存 onEndEdit
5. 添加新的监听
```

**调用者**: 需要在输入完成后处理的代码

**使用示例**:
```csharp
// 监听编辑结束
searchInput.SetOnEndEdit(() =>
{
    string query = searchInput.GetText();
    Log.Info($"搜索：{query}");
    Search(query);
});
```

---

### RemoveOnValueChanged() / RemoveOnEndEdit()

**签名**:
```csharp
public void RemoveOnValueChanged()
public void RemoveOnEndEdit()
```

**职责**: 移除事件监听

**核心逻辑**:
```
1. 如果回调不为 null
2. 从 input 移除监听
3. 清空回调
```

**调用者**: `SetOnValueChanged()`, `SetOnEndEdit()`, `OnDestroy()`

---

## 阅读指引

### 建议的阅读顺序

1. **理解输入框组件作用** - 为什么需要 UIInput
2. **看 GetText/SetText** - 理解文本读写
3. **看 SetOnValueChanged** - 理解实时监听
4. **看 SetOnEndEdit** - 理解编辑完成监听

### 最值得学习的技术点

1. **双重事件**: onValueChange（实时）+ onEndEdit（完成）
2. **事件管理**: 自动清理事件监听
3. **简单封装**: 直接封装 Unity InputField

---

## 使用示例

### 示例 1: 玩家昵称输入

```csharp
public class UIRegisterView : UIBaseView, IOnCreate
{
    private UIInput nameInput;
    private UIButton submitButton;
    
    public void OnCreate()
    {
        nameInput = AddComponent<UIInput>("NameInput");
        submitButton = AddComponent<UIButton>("SubmitButton");
        
        // 监听输入变化（实时验证）
        nameInput.SetOnValueChanged(() =>
        {
            string name = nameInput.GetText();
            ValidateName(name);
        });
        
        // 监听编辑结束
        nameInput.SetOnEndEdit(() =>
        {
            Log.Info($"昵称输入完成：{nameInput.GetText()}");
        });
        
        submitButton.SetOnClick(OnSubmit);
    }
    
    private void OnSubmit()
    {
        string name = nameInput.GetText();
        if (string.IsNullOrEmpty(name))
        {
            Log.Warning("昵称为空");
            return;
        }
        
        SubmitRegister(name);
    }
    
    private void ValidateName(string name)
    {
        // 实时验证昵称
        if (name.Length > 10)
        {
            Log.Warning("昵称过长");
        }
    }
}
```

### 示例 2: 搜索框

```csharp
public class UISearchView : UIBaseView, IOnCreate
{
    private UIInput searchInput;
    
    public void OnCreate()
    {
        searchInput = AddComponent<UIInput>("SearchInput");
        
        // 编辑结束时搜索
        searchInput.SetOnEndEdit(() =>
        {
            string query = searchInput.GetText();
            SearchItems(query);
        });
    }
}
```

---

## 相关文档

- [UIBaseContainer.cs.md](./UIBaseContainer.cs.md) - UI 容器基类
- [UIInputTextmesh.cs.md](./UIInputTextmesh.cs.md) - TextMesh 输入框
- [UIButton.cs.md](./UIButton.cs.md) - UI 按钮组件

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
