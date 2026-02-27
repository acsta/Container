# UIInputTextmesh.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | UIInputTextmesh.cs |
| **路径** | Assets/Scripts/Code/Module/UIComponent/UIInputTextmesh.cs |
| **所属模块** | 框架层 → Code/Module/UIComponent |
| **文件职责** | TextMesh Pro 输入框组件，支持 WebGL 平台键盘 |

---

## 类/结构体说明

### UIInputTextmesh

| 属性 | 说明 |
|------|------|
| **职责** | 封装 TMPro.TMP_InputField，支持 WebGL 平台原生键盘 |
| **泛型参数** | 无 |
| **继承关系** | 继承自 `UIBaseContainer` |
| **实现的接口** | `IOnDestroy`, `IOnCreate` |

```csharp
public class UIInputTextmesh : UIBaseContainer, IOnDestroy, IOnCreate
{
    // TextMesh Pro 输入框组件
}
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `input` | `TMPro.TMP_InputField` | `private` | TextMesh Pro InputField 组件 |
| `onValueChange` | `UnityAction<string>` | `private` | 值变化事件回调 |
| `onEndEdit` | `UnityAction<string>` | `private` | 编辑结束事件回调 |
| `isShowKeyboard` | `bool` | `private` | 是否显示键盘（WebGL） |

---

## 方法说明（按重要程度排序）

### OnCreate()

**签名**:
```csharp
public void OnCreate()
```

**职责**: 创建时初始化（WebGL 平台键盘支持）

**核心逻辑**:
```
1. 如果是 WebGL 平台：
   - 激活组件
   - 监听 onSelect 事件（选择输入框时显示键盘）
   - 监听 onDeselect 事件（失去焦点时隐藏键盘）
```

**调用者**: `UIManager.InnerOpenWindowGetGameObject()`

---

### OnDestroy()

**签名**:
```csharp
public void OnDestroy()
```

**职责**: 销毁时清理资源

**核心逻辑**:
```
1. 移除值变化事件监听
2. 移除编辑结束事件监听
3. 如果是 WebGL 平台：
   - 移除 onSelect/onDeselect 监听
   - 隐藏键盘
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

---

### SetOnValueChanged(Action func) / SetOnEndEdit(Action func)

**签名**:
```csharp
public void SetOnValueChanged(Action func)
public void SetOnEndEdit(Action func)
```

**职责**: 设置值变化/编辑结束事件回调

**与 UIInput 的区别**: 功能相同，但使用 TMPro.TMP_InputField

---

### SetInteractable(bool flag)

**签名**:
```csharp
public void SetInteractable(bool flag)
```

**职责**: 设置是否可交互

**参数**:
- `flag`: true=可交互，false=不可交互

**核心逻辑**:
```
1. 激活 InputField 组件
2. 设置 input.interactable = flag
```

**调用者**: 需要控制输入框状态的代码

---

## WebGL 平台支持

### OnSelect(string text)

**签名**:
```csharp
private void OnSelect(string text)
```

**职责**: 选择输入框时显示原生键盘

**核心逻辑**:
```
1. 检查是否已显示键盘
2. 设置 isShowKeyboard = true
3. 根据平台显示键盘：
   - 微信小游戏：WeChatWASM.WX.ShowKeyboard()
   - 快手小游戏：KSWASM.KS.ShowKeyboard()
   - 抖音小游戏：TTSDK.TT.ShowKeyboard()
   - TapTap：WeChatWASM.WX.ShowKeyboard()
   - 其他：BridgeHelper.SetUpOverlayDialog()
4. 注册键盘事件（OnKeyboardConfirm/OnKeyboardComplete/OnKeyboardInput）
```

**平台适配**:
```csharp
#if UNITY_WEBGL_WeChat || UNITY_WEBGL_TAPTAP
    WeChatWASM.WX.ShowKeyboard(new ()
    {
        defaultValue = input.text,
        maxLength = input.characterLimit <= 0 ? 9999 : input.characterLimit,
        confirmType = "done",
    });
#elif UNITY_WEBGL_KS
    KSWASM.KS.ShowKeyboard(...);
#elif UNITY_WEBGL_TT
    TTSDK.TT.ShowKeyboard(...);
#else
    BridgeHelper.SetUpOverlayDialog(...);
#endif
```

---

### HideKeyboard()

**签名**:
```csharp
private void HideKeyboard()
```

**职责**: 隐藏原生键盘

**核心逻辑**:
```
1. 检查是否已显示键盘
2. 设置 isShowKeyboard = false
3. 根据平台隐藏键盘
4. 注销键盘事件
```

---

### OnInput / OnConfirm / OnComplete

**签名**:
```csharp
private void OnInput(WeChatWASM.OnKeyboardInputListenerResult v)
private void OnConfirm(WeChatWASM.OnKeyboardInputListenerResult v)
private void OnComplete(WeChatWASM.OnKeyboardInputListenerResult v)
```

**职责**: 处理原生键盘事件

**核心逻辑**:
```
// OnInput:
1. 如果输入框已聚焦
2. 更新 input.text = v.value

// OnConfirm/OnComplete:
1. 隐藏键盘
```

---

## 阅读指引

### 建议的阅读顺序

1. **理解组件作用** - 为什么需要 UIInputTextmesh
2. **看 GetText/SetText** - 理解文本读写
3. **看 OnSelect/HideKeyboard** - 理解 WebGL 键盘支持
4. **了解平台适配** - 不同小游戏平台的键盘 API

### 最值得学习的技术点

1. **WebGL 键盘适配**: 多平台小游戏原生键盘支持
2. **条件编译**: #if UNITY_WEBGL_* 区分平台
3. **事件管理**: 自动注册/注销键盘事件
4. **防重复显示**: isShowKeyboard 标志

---

## 使用示例

### 示例 1: 聊天输入框

```csharp
public class UIChatView : UIBaseView, IOnCreate
{
    private UIInputTextmesh chatInput;
    private UIButton sendButton;
    
    public void OnCreate()
    {
        chatInput = AddComponent<UIInputTextmesh>("ChatInput");
        sendButton = AddComponent<UIButton>("SendButton");
        
        chatInput.SetOnEndEdit(() =>
        {
            string message = chatInput.GetText();
            if (!string.IsNullOrEmpty(message))
            {
                SendMessage(message);
                chatInput.SetText("");
            }
        });
        
        sendButton.SetOnClick(() =>
        {
            string message = chatInput.GetText();
            if (!string.IsNullOrEmpty(message))
            {
                SendMessage(message);
                chatInput.SetText("");
            }
        });
    }
}
```

### 示例 2: 搜索框

```csharp
public class UISearchView : UIBaseView, IOnCreate
{
    private UIInputTextmesh searchInput;
    
    public void OnCreate()
    {
        searchInput = AddComponent<UIInputTextmesh>("SearchInput");
        
        // 实时搜索
        searchInput.SetOnValueChanged(() =>
        {
            string query = searchInput.GetText();
            Search(query);
        });
    }
}
```

---

## 平台差异

| 平台 | 键盘 API | 说明 |
|------|---------|------|
| 微信小游戏 | WeChatWASM.WX.ShowKeyboard | 原生键盘 |
| 快手小游戏 | KSWASM.KS.ShowKeyboard | 原生键盘 |
| 抖音小游戏 | TTSDK.TT.ShowKeyboard | 原生键盘 |
| TapTap | WeChatWASM.WX.ShowKeyboard | 原生键盘 |
| 其他 H5 | BridgeHelper.SetUpOverlayDialog | 自定义对话框 |

---

## 相关文档

- [UIBaseContainer.cs.md](./UIBaseContainer.cs.md) - UI 容器基类
- [UIInput.cs.md](./UIInput.cs.md) - 普通输入框
- [UITextmesh.cs.md](./UITextmesh.cs.md) - TextMesh 文本

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
