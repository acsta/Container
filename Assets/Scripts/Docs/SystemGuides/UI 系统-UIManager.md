# UI 系统理解指南 - UIManager

> **文档类型**: 系统理解指南  
> **适用范围**: Code/Module/UI + Code/Game/UI  
> **生成时间**: 2026-03-03  
> **前置知识**: Unity UGUI、异步编程、对象池

---

## 📑 概述

UI 系统负责管理游戏的所有界面窗口，包括打开/关闭窗口、层级管理、资源加载等。

**核心职责**:
- 管理 UI 窗口生命周期（创建/打开/关闭/销毁）
- 管理 UI 层级（弹出栈、普通层、顶层等）
- 处理 UI 资源加载和缓存
- 提供返回键处理
- 支持安全区域适配

**关键文件**:
| 文件 | 职责 |
|------|------|
| `UIManager.cs` | UI 管理核心 |
| `UIWindow.cs` | 窗口封装 |
| `UIBaseView.cs` | UI 基类 |
| `UILayer.cs` | 层级管理 |

---

## 🎯 系统职责

### 解决的核心问题

1. **UI 管理混乱**: 大量 UI 窗口需要统一管理
2. **层级问题**: 弹窗应该在普通窗口之上
3. **资源泄漏**: UI 预制体需要缓存和释放
4. **返回键处理**: Android/返回键需要关闭最上层窗口

### 设计思路

```
UI 系统设计:
1. UIWindow: 封装窗口的加载/激活/隐藏
2. UILayer: 管理不同层级的窗口栈
3. 对象池：缓存 UI 预制体，减少加载时间
4. 生命周期：IOnCreate/IOnEnable/IOnDisable/IOnDestroy

层级结构:
┌─────────────────────┐
│    TopLayer (顶层)   │ ← 系统提示、Toast
├─────────────────────┤
│    PopUpLayer (弹出) │ ← 对话框、确认框
├─────────────────────┤
│  NormalLayer (普通)  │ ← 主要界面
├─────────────────────┤
│   BottomLayer (底层) │ ← 背景、遮罩
└─────────────────────┘
```

---

## 🏗️ 架构设计

### 核心类图

```
┌─────────────────────────────────────────────────────────┐
│                     UIManager                            │
│  ┌─────────────────────────────────────────────────┐   │
│  │  windows: Dictionary<string, UIWindow>          │   │
│  │  所有存活的窗口                                  │   │
│  └─────────────────────────────────────────────────┘   │
│  ┌─────────────────────────────────────────────────┐   │
│  │  windowStack: Dictionary<Layer, LinkedList>     │   │
│  │  各层级的窗口栈                                  │   │
│  └─────────────────────────────────────────────────┘   │
│  ┌─────────────────────────────────────────────────┐   │
│  │  boxes: Dictionary<UIBaseView, UIWindow>        │   │
│  │  消息盒子（提示框）                              │   │
│  └─────────────────────────────────────────────────┘   │
│                                                          │
│  +OpenWindow<T>()                                       │
│  +CloseWindow(window)                                   │
│  +GetWindow<T>()                                        │
│  +DestroyAllWindow()                                    │
└─────────────────────────────────────────────────────────┘
                            │
                            │ 管理
                            ▼
┌─────────────────────────────────────────────────────────┐
│                      UIWindow                            │
│─────────────────────────────────────────────────────────│
│ +Name: string                                          │
│ +View: UIBaseView                                      │
│ +Layer: UILayerNames                                   │
│ +Active: bool                                          │
│ +LoadingState: enum                                    │
│─────────────────────────────────────────────────────────│
│ +LoadAsync(path)                                       │
│ +Show()                                                │
│ +Hide()                                                │
│ +Destroy()                                             │
└─────────────────────────────────────────────────────────┘
                            ▲
                            │ 组合
                            │
┌─────────────────────────────────────────────────────────┐
│                    UIBaseView                            │
│─────────────────────────────────────────────────────────│
│ +CanBack: bool                                         │
│ +IsLoading: bool                                       │
│─────────────────────────────────────────────────────────│
│ +OnCreate()                                            │
│ +OnEnable()                                            │
│ +OnDisable()                                           │
│ +OnDestroy()                                           │
│ +OnInputKeyBack()                                      │
└─────────────────────────────────────────────────────────┘
```

---

## 🔄 核心流程

### 打开窗口流程

```csharp
// 1. 调用打开窗口
await UIManager.Instance.OpenWindow<UILobbyView>("UI/UILobbyView");

// 2. 内部流程
public async ETTask<T> OpenWindow<T>(string path, UILayerNames layerName = NormalLayer) 
    where T : UIBaseView, IOnCreate
{
    string uiName = TypeInfo<T>.TypeName;
    
    // 检查窗口是否已存在
    var target = GetWindow(uiName);
    if (target == null)
    {
        // 创建新窗口
        target = InitWindow<T>(path, layerName);
        windows[uiName] = target;
    }
    
    target.Layer = layerName;
    
    // 打开窗口
    return await InnerOpenWindow<T>(target);
}

// 3. InnerOpenWindow 流程
async ETTask<T> InnerOpenWindow<T>(UIWindow window)
{
    // 加载预制体（如果未加载）
    if (window.LoadingState != LoadOver)
    {
        await window.LoadAsync();
    }
    
    // 加入层级栈
    AddToStack(window.Layer, window.Name);
    
    // 显示窗口
    window.Show();
    
    // 调用 OnCreate（仅第一次）
    if (window.View is IOnCreate onCreate)
    {
        await onCreate.OnCreate();
    }
    
    // 调用 OnEnable
    if (window.View is IOnEnable onEnable)
    {
        await onEnable.OnEnable();
    }
    
    return window.View as T;
}
```

---

### 关闭窗口流程

```csharp
// 1. 关闭窗口
await UIManager.Instance.CloseWindow(window);

// 2. 内部流程
public async ETTask CloseWindow(UIWindow window)
{
    if (window == null) return;
    
    // 调用 OnDisable
    if (window.View is IOnDisable onDisable)
    {
        await onDisable.OnDisable();
    }
    
    // 从层级栈移除
    RemoveFromStack(window.Layer, window.Name);
    
    // 隐藏窗口
    window.Hide();
    
    // 调用 OnBeforeClose
    if (window.View is IOnBeforeCloseWin beforeClose)
    {
        await beforeClose.OnBeforeCloseWin();
    }
}

// 3. 销毁窗口
public async ETTask DestroyWindow(UIWindow window)
{
    await CloseWindow(window);
    
    // 调用 OnDestroy
    if (window.View is IOnDestroy onDestroy)
    {
        await onDestroy.OnDestroy();
    }
    
    // 销毁预制体
    await window.Destroy();
    
    // 从字典移除
    windows.Remove(window.Name);
}
```

---

### 返回键处理流程

```csharp
// 1. Init 中注册事件
public void Init()
{
    Messager.Instance.AddListener<int, int>(0, MessageId.OnKeyInput, OnKeyInput);
}

// 2. 返回键处理
private void OnKeyInput(int key, int state)
{
    if (key == (int)GameKeyCode.Back && (state & InputManager.KeyDown) != 0)
    {
        // 获取最上层窗口
        var win = GetTopWindow();
        
        // 检查是否可以返回
        if (win != null && win.View != null && win.View.CanBack)
        {
            // 调用窗口的返回处理
            win.View.OnInputKeyBack().Coroutine();
        }
    }
}

// 3. 窗口实现返回逻辑
public class UILobbyView : UIBaseView
{
    public override bool CanBack => false;  // 大厅不允许返回
    
    public override async ETTask OnInputKeyBack()
    {
        // 显示确认对话框
        var result = await UIManager.Instance.ShowMessageBox("确定要退出吗？");
        if (result)
        {
            Application.Quit();
        }
    }
}
```

---

## 💡 使用示例

### 基础窗口

```csharp
// 1. 定义窗口
public class UILobbyView : UIBaseView, IOnCreate, IOnEnable, IOnDisable
{
    private Button btnStart;
    private Button btnSetting;
    
    public override async ETTask OnCreate()
    {
        // 初始化（仅调用一次）
        btnStart = GetChild<Button>("BtnStart");
        btnSetting = GetChild<Button>("BtnSetting");
        
        btnStart.onClick.AddListener(OnStartClick);
        btnSetting.onClick.AddListener(OnSettingClick);
    }
    
    public override async ETTask OnEnable()
    {
        // 每次显示时调用
        UpdateUI();
    }
    
    public override async ETTask OnDisable()
    {
        // 每次隐藏时调用
    }
    
    private void OnStartClick()
    {
        // 打开游戏场景
        UIManager.Instance.OpenWindow<UIGameView>("UI/UIGameView");
    }
    
    private void OnSettingClick()
    {
        // 打开设置窗口（弹出层）
        UIManager.Instance.OpenWindow<UISettingView>(
            "UI/UISettingView", 
            layerName: UILayerNames.PopUpLayer
        );
    }
}
```

### 打开窗口

```csharp
// 打开无参数窗口
var lobby = await UIManager.Instance.OpenWindow<UILobbyView>("UI/UILobbyView");

// 打开带参数窗口
var game = await UIManager.Instance.OpenWindow<UIGameView, GameData>(
    "UI/UIGameView", 
    gameData
);

// 打开带多个参数的窗口
var shop = await UIManager.Instance.OpenWindow<UIShopView, int, string>(
    "UI/UIShopView",
    shopId,
    shopType
);
```

### 关闭窗口

```csharp
// 获取窗口
var setting = UIManager.Instance.GetWindow<UISettingView>();

// 关闭窗口
await UIManager.Instance.CloseWindow(setting);

// 销毁窗口（释放资源）
await UIManager.Instance.DestroyWindow(setting);

// 关闭所有窗口
await UIManager.Instance.DestroyAllWindow();
```

### 检查窗口状态

```csharp
// 检查窗口是否打开
bool isLobbyOpen = UIManager.Instance.IsWindowActive<UILobbyView>();

// 检查窗口是否打开且加载完成
bool isLobbyReady = UIManager.Instance.IsWindowActive<UILobbyView>(active: 2);

// 获取窗口
var lobby = UIManager.Instance.GetView<UILobbyView>();

// 获取最上层窗口
var topWin = UIManager.Instance.GetTopWindow();

// 获取指定层级的最上层窗口
var topPopUp = UIManager.Instance.GetTopWindow(UILayerNames.PopUpLayer);
```

### 消息框

```csharp
// 显示确认框
bool result = await UIManager.Instance.ShowMessageBox("确定要删除吗？");
if (result)
{
    DeleteItem();
}

// 显示提示框
await UIManager.Instance.ShowToast("操作成功");

// 显示输入框
string input = await UIManager.Instance.ShowInputDialog("请输入名称：");
```

### 安全区域适配

```csharp
// 自动适配刘海屏/挖孔屏
public class UIBaseView : MonoBehaviour
{
    protected virtual void Awake()
    {
        // 获取安全区域
        var safeArea = SystemInfoHelper.safeArea;
        
        // 设置边距
        float leftPadding = safeArea.xMin;
        float rightPadding = Screen.width - safeArea.xMax;
        
        // 调整 UI 布局
        AdjustLayout(leftPadding, rightPadding);
    }
}
```

---

## 🔗 依赖关系

```
依赖:
├─→ ResourcesManager (资源加载)
├─→ GameObjectPoolManager (对象池)
├─→ Messager (返回键事件)
├─→ SystemInfoHelper (安全区域)
└─→ TimerManager (异步等待)

被依赖:
├─→ 所有 UI 窗口（UILobby/UIGame/UIShop 等）
├─→ 游戏流程（需要打开 UI 的地方）
└─→ 场景系统（场景切换时清理 UI）
```

---

## ⚠️ 注意事项

| 问题 | 说明 | 解决方案 |
|------|------|----------|
| 重复打开 | 同一窗口多次打开 | 检查 `IsWindowActive()` |
| 资源泄漏 | 关闭窗口未销毁 | 使用 `DestroyWindow()` |
| 层级错误 | 窗口出现在错误层级 | 明确指定 `layerName` |
| 返回键失效 | `CanBack` 设置错误 | 正确实现 `CanBack` 属性 |
| 安全区域 | 刘海屏显示异常 | 使用 `ResetSafeArea()` |

---

## 🔍 性能优化

### UI 缓存策略

```csharp
// 频繁打开的窗口：缓存
var lobby = await UIManager.Instance.OpenWindow<UILobbyView>("UI/UILobbyView");
// 关闭时不销毁，仅隐藏
await UIManager.Instance.CloseWindow(lobby);
// 再次打开时秒开（从缓存获取）

// 不常打开的窗口：不缓存
var rare = await UIManager.Instance.OpenWindow<UIRareView>("UI/UIRareView");
// 关闭时销毁
await UIManager.Instance.DestroyWindow(rare);
```

### 分层加载

```
场景启动时:
1. 立即加载：UILoading（加载界面）
2. 预加载：UILobby（大厅界面）
3. 按需加载：其他窗口

优势:
- 启动时间短
- 内存占用低
- 切换流畅
```

---

## 📚 相关文档

| 文档 | 说明 |
|------|------|
| [UIWindow.cs.md](./UI/UIWindow.cs.md) | UIWindow 详细文档 |
| [UIBaseView.cs.md](./UI/UIBaseView.cs.md) | UIBaseView 详细文档 |
| [ResourcesManager.cs.md](../../Mono/Module/Resource/ResourcesManager.cs.md) | 资源管理器 |
| [场景系统](./场景系统-SceneManager.md) | 场景切换时清理 UI |

---

*文档由 OpenClaw AI 助手自动生成 | UI 系统理解指南*
