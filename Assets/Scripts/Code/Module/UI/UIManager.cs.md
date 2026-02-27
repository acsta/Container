# UIManager.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | UIManager.cs |
| **路径** | Assets/Scripts/Code/Module/UI/UIManager.cs |
| **所属模块** | 框架层 → Code/Module/UI |
| **文件职责** | UI 框架核心管理类，负责所有 UI 窗口的创建、销毁、层级管理和生命周期控制 |

---

## 类/结构体说明

### UIManager

| 属性 | 说明 |
|------|------|
| **职责** | UI 系统核心管理器，提供窗口管理、层级管理、消息盒子、屏幕适配等功能 |
| **泛型参数** | 无 |
| **继承关系** | 无继承（partial 类，另有 UIManager.Layers.cs） |
| **实现的接口** | `IManager` |

**设计模式**: 单例模式 + 对象池 + 状态机

```csharp
// 单例实现
public static UIManager Instance { get; private set; }

// 通过 ManagerProvider 注册
ManagerProvider.RegisterManager<UIManager>();
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `Instance` | `UIManager` | `public static` | 单例实例，全局访问点 |
| `windows` | `Dictionary<string, UIWindow>` | `private` | 所有存活窗口的字典 {uiName: window} |
| `windowStack` | `Dictionary<UILayerNames, LinkedList<string>>` | `private` | 按层级组织的窗口栈，维护显示顺序 |
| `boxes` | `Dictionary<UIBaseView, UIWindow>` | `private` | 所有存活的消息盒子 {instance: window} |
| `ScreenSizeFlag` | `float` | `public` | 屏幕适配缩放系数 |
| `WidthPadding` | `float` | `public` | 安全区域边缘宽度（刘海屏适配） |
| `UICamera` | `Camera` | `private` | UI 专用相机（来自 Layers.cs） |

---

## 方法说明（按重要程度排序）

### Init()

**签名**:
```csharp
public void Init()
```

**职责**: 初始化 UI 系统，创建层级结构，注册事件监听

**核心逻辑**:
```
1. 调用 ResetSafeArea() 初始化安全区域
2. 初始化 windows, windowStack, boxes 字典
3. 调用 InitLayer() 创建 UI 层级（见 UIManager.Layers.cs）
4. 注册 Back 键事件监听 MessageId.OnKeyInput
```

**调用者**: ManagerProvider.RegisterManager<UIManager>()

**被调用者**: `InitLayer()`, `ResetSafeArea()`, `Messager.Instance.AddListener`

---

### Destroy()

**签名**:
```csharp
public void Destroy()
```

**职责**: 销毁 UI 系统，清理所有窗口和资源

**核心逻辑**:
```
1. 移除 Back 键事件监听
2. 设置 Instance = null
3. 异步调用 OnDestroyAsync()
   - 销毁所有窗口 DestroyAllWindow()
   - 清空字典
   - 销毁层级 DestroyLayer()
```

**调用者**: ManagerProvider.RemoveManager<UIManager>()

---

### OpenWindow<T>(string path, UILayerNames layerName)

**签名**:
```csharp
public async ETTask<T> OpenWindow<T>(string path, UILayerNames layerName = UILayerNames.NormalLayer) 
    where T : UIBaseView, IOnCreate
```

**职责**: 打开一个 UI 窗口（无参数版本）

**核心逻辑**:
```
1. 获取类型名称 uiName = TypeInfo<T>.TypeName
2. 检查窗口是否已存在 GetWindow(uiName)
3. 如果不存在，创建新窗口 InitWindow<T>(path, layerName)
4. 设置窗口层级 target.Layer = layerName
5. 调用 InnerOpenWindow<T>(target) 执行实际加载
6. 返回窗口实例
```

**调用者**: 任何需要打开 UI 的代码

**被调用者**: `InitWindow<T>()`, `InnerOpenWindow<T>()`, `GetWindow()`

**使用示例**:
```csharp
// 打开加载界面
await UIManager.Instance.OpenWindow<UILoadingView>(UILoadingView.PrefabPath, UILayerNames.TipLayer);

// 打开主界面
await UIManager.Instance.OpenWindow<UILobbyView>(UILobbyView.PrefabPath);
```

---

### OpenWindow<T, P1>(string path, P1 p1, UILayerNames layerName)

**签名**:
```csharp
public async ETTask<T> OpenWindow<T, P1>(string path, P1 p1, UILayerNames layerName = UILayerNames.NormalLayer)
    where T : UIBaseView, IOnCreate, IOnEnable<P1>
```

**职责**: 打开一个带单个参数的 UI 窗口

**核心逻辑**: 与无参版本类似，但调用 `InnerOpenWindow<T, P1>(target, p1)` 传递参数

**使用示例**:
```csharp
// 打开拍卖界面，传入关卡 ID
await UIManager.Instance.OpenWindow<UIAuctionView, int>(UIAuctionView.PrefabPath, levelId);
```

---

### OpenBox<T>(string path, UILayerNames layerName, int during)

**签名**:
```csharp
public async ETTask<T> OpenBox<T>(string path, UILayerNames layerName = UILayerNames.TipLayer, int during = -1)
    where T : UIBaseView, IOnCreate
```

**职责**: 打开一个消息盒子（支持多例，关闭后销毁）

**与 OpenWindow 的区别**:
| 特性 | OpenWindow | OpenBox |
|------|------------|---------|
| 实例 | 单例 | 多例 |
| 缓存 | 关闭窗口后缓存 | 关闭后立即销毁 |
| 用途 | 主界面、系统窗口 | 提示框、对话框 |
| 生命周期 | 持久 | 临时 |

**核心逻辑**:
```
1. 创建新窗口 InitWindow<T>(path, layerName)
2. 标记为盒子 target.IsBox = true
3. 打开窗口 InnerOpenWindow<T>(target)
4. 添加到 boxes 字典
5. 如果 during > 0，设置定时关闭
```

**使用示例**:
```csharp
// 打开提示框，5 秒后自动关闭
await UIManager.Instance.OpenBox<UIToastView>(UIToast.PrefabPath, during: 5000);

// 打开消息框，手动关闭
var msgBox = await UIManager.Instance.OpenBox<UIMsgBoxWin, MsgBoxPara>(
    UIMsgBoxWin.PrefabPath, 
    new MsgBoxPara { Content = "确定吗？" }
);
```

---

### CloseWindow<T>()

**签名**:
```csharp
public async ETTask CloseWindow<T>()
```

**职责**: 关闭指定类型的窗口

**核心逻辑**:
```
1. 获取类型名称 uiName = TypeInfo<T>.TypeName
2. 调用 CloseWindow(uiName)
3. 等待窗口加载完成（如果正在加载）
4. 调用 InnerBeforeCloseWindow() 执行关闭前回调
5. 从栈中移除 RemoveFromStack()
6. 调用 InnerCloseWindow() 隐藏窗口
```

**注意**: 关闭窗口后不销毁，缓存以便下次快速打开

---

### DestroyWindow<T>(int clear)

**签名**:
```csharp
public async ETTask DestroyWindow<T>(int clear = -1) where T : UIBaseView
```

**职责**: 销毁指定窗口，释放资源

**核心逻辑**:
```
1. 先关闭窗口 CloseWindow(uiName)
2. 调用 InnerDestroyWindow(target, clear) 销毁资源
3. 从 windows 字典移除
4. 调用 target.Dispose()
```

**参数 clear**: 对象池缓存阈值，-1 表示无限缓存

---

### GetView<T>(int active)

**签名**:
```csharp
public T GetView<T>(int active = 0) where T : UIBaseView
```

**职责**: 获取已打开的窗口实例

**参数 active**:
- `0` = 不做限制
- `1` = 只返回打开的窗口
- `-1` = 只返回关闭的窗口
- `2` = 只返回打开且加载完成的窗口

**使用示例**:
```csharp
// 获取加载界面实例
var loadingView = UIManager.Instance.GetView<UILoadingView>();
loadingView?.SetProgress(0.5f);

// 获取已打开且加载完成的窗口
var view = UIManager.Instance.GetView<UIAuctionView>(active: 2);
```

---

### GetTopWindow(UILayerNames layer)

**签名**:
```csharp
public UIWindow GetTopWindow(UILayerNames layer)
```

**职责**: 获取指定层级的最顶层窗口

**核心逻辑**:
```
1. 获取该层级的窗口栈 windowStack[layer]
2. 从栈顶开始遍历（First → Last）
3. 返回第一个存在的窗口
```

**使用示例**:
```csharp
// 获取最顶层窗口（用于 Back 键处理）
var topWin = UIManager.Instance.GetTopWindow();
if (topWin != null && topWin.View.CanBack)
{
    await topWin.View.OnInputKeyBack();
}
```

---

### IsWindowActive<T>(int active)

**签名**:
```csharp
public bool IsWindowActive<T>(int active = 0) where T : UIBaseView
```

**职责**: 判断窗口是否处于指定状态

**使用示例**:
```csharp
// 检查加载界面是否已打开且加载完成
if (UIManager.Instance.IsWindowActive<UILoadingView>(active: 2))
{
    // 可以安全操作
}
```

---

### InnerOpenWindow<T>(UIWindow target)

**签名**:
```csharp
async ETTask<T> InnerOpenWindow<T>(UIWindow target)
```

**职责**: 内部窗口加载流程（核心方法）

**核心逻辑**:
```
1. 获取协程锁 CoroutineLockManager.Instance.Wait()
2. 设置 target.Active = true
3. 检查是否需要加载（LoadingState == NotStart）
4. 如果需要：
   - 从对象池获取预制体 InnerOpenWindowGetGameObject()
   - 设置父节点到对应层级
   - 调用 view.OnCreate()
5. 重置窗口层级 InnerResetWindowLayer()
6. 添加到窗口栈 AddWindowToStack()
7. 设置 LoadingState = LoadOver
8. 返回窗口实例
```

**协程锁作用**: 防止同一窗口被重复打开

---

### InnerOpenWindowGetGameObject(string path, UIWindow target)

**签名**:
```csharp
async ETTask InnerOpenWindowGetGameObject(string path, UIWindow target)
```

**职责**: 从对象池加载预制体并初始化

**核心逻辑**:
```
1. 从对象池获取 GameObject
   GameObjectPoolManager.GetInstance().GetGameObjectAsync(path)
2. 设置父节点到对应层级的 RectTransform
3. 设置窗口名称 trans.name = target.Name
4. 绑定 Transform 到 view
5. 调用 view.OnCreate() 生命周期
6. 注册国际化 II18N 接口
```

---

### AddWindowToStack(UIWindow target)

**签名**:
```csharp
async ETTask AddWindowToStack(UIWindow target)
```

**职责**: 将窗口添加到层级栈，并处理特殊层级逻辑

**核心逻辑**:
```
1. 检查是否已存在栈中，存在则先移除
2. 添加到栈顶 windowStack[layerName].AddFirst(uiName)
3. 调用 InnerAddWindowToStack(target) 设置 Z 序
4. 激活窗口 view.SetActive(true)
5. 如果是背景层级：
   - 关闭所有 NormalLayer
   - 关闭所有 GameLayer
   - 关闭其他 BackgroundLayer
```

**背景层级特殊处理**: 打开背景层时自动关闭下层 UI，避免冲突

---

### CloseWindowByLayer(UILayerNames layer, params string[] exceptUINames)

**签名**:
```csharp
public async ETTask CloseWindowByLayer(UILayerNames layer, params string[] exceptUINames)
```

**职责**: 关闭指定层级的所有窗口（可排除指定窗口）

**核心逻辑**:
```
1. 创建排除集合 HashSetComponent<string>
2. 遍历所有窗口 windows
3. 如果窗口层级匹配且不在排除列表中
4. 并行关闭所有符合条件的窗口 ETTaskHelper.WaitAll()
```

**使用示例**:
```csharp
// 关闭所有普通层窗口，但保留主界面
await UIManager.Instance.CloseWindowByLayer(UILayerNames.NormalLayer, "UILobbyView");
```

---

### DestroyAllWindow()

**签名**:
```csharp
public async ETTask DestroyAllWindow()
```

**职责**: 销毁所有窗口，用于场景切换或游戏重启

**核心逻辑**:
```
1. 获取所有窗口名称 keys
2. 倒序遍历（从后往前）
3. 并行销毁所有窗口 ETTaskHelper.WaitAll()
```

**调用者**: `UIManager.Destroy()`, `SceneManager.SwitchScene()`

---

### SetWidthPadding(float value)

**签名**:
```csharp
public void SetWidthPadding(float value)
```

**职责**: 设置安全区域边缘宽度（刘海屏/挖孔屏适配）

**核心逻辑**:
```
1. 更新 WidthPadding 值
2. 遍历所有层级的所有窗口
3. 对实现 IOnWidthPaddingChange 的窗口调用 OnWidthPaddingChange()
```

**使用示例**:
```csharp
// 适配刘海屏
float safePadding = SystemInfoHelper.safeArea.xMin;
UIManager.Instance.SetWidthPadding(safePadding);
```

---

### ScreenPointToUILocalPoint(RectTransform parentRT, Vector2 screenPoint)

**签名**:
```csharp
public Vector2 ScreenPointToUILocalPoint(RectTransform parentRT, Vector2 screenPoint)
```

**职责**: 将屏幕坐标转换为 UI 局部坐标

**核心逻辑**:
```
1. 获取 UICamera
2. WebGL 平台特殊处理（camera = null）
3. 使用 RectTransformUtility.ScreenPointToLocalPointInRectangle()
4. 返回转换后的坐标，失败返回 (9999, 9999)
```

**使用示例**:
```csharp
// 点击位置转换
Vector2 screenPos = Input.mousePosition;
Vector2 uiPos = UIManager.Instance.ScreenPointToUILocalPoint(rectTransform, screenPos);
```

---

### OnKeyInput(int key, int state)

**签名**:
```csharp
private void OnKeyInput(int key, int state)
```

**职责**: 处理 Back 键输入，实现返回功能

**核心逻辑**:
```
1. 检查是否是 Back 键且按下状态
2. 获取最顶层窗口 GetTopWindow()
3. 如果窗口允许返回（win.View.CanBack）
4. 调用窗口的 OnInputKeyBack() 方法
```

**调用者**: Messager.Instance（事件系统）

---

## Unity 生命周期集成

### UIManager 不是 MonoBehaviour

UIManager 是普通 C# 类，通过 ManagerProvider 集成到 Unity 生命周期：

```csharp
// Init 在注册时自动调用
ManagerProvider.RegisterManager<UIManager>();  // → Init()

// Destroy 在移除时自动调用
ManagerProvider.RemoveManager<UIManager>();    // → Destroy()
```

### UI 窗口的生命周期

每个 UI 窗口（UIBaseView 子类）遵循以下生命周期：

```
1. OnCreate()      - 创建时调用（只调用一次）
2. OnEnable()      - 每次打开时调用
3. OnDisable()     - 每次关闭时调用
4. OnBeforeDisable() - 关闭前回调（可选）
5. OnDestroy()     - 销毁时调用（只调用一次）
```

**生命周期接口**:
- `IOnCreate` - OnCreate()
- `IOnEnable<P>` - OnEnable(P p)
- `IOnDisable` - OnDisable()
- `IOnDestroy` - OnDestroy()
- `IOnBeforeCloseWin` - OnBeforeDisable()

---

## 阅读指引

### 建议的阅读顺序

1. **先看字段定义** - 了解 windows, windowStack, boxes 的作用
2. **再看 Init/Destroy** - 理解 UI 系统生命周期
3. **重点看 OpenWindow** - 理解窗口打开流程
4. **然后看 InnerOpenWindow** - 理解内部加载机制
5. **最后看 Close/Destroy** - 理解窗口关闭和销毁

### 这个文件中最值得学习的技术点

1. **泛型方法重载**: 支持 0-4 个参数的 OpenWindow 重载
2. **协程锁机制**: 防止同一窗口被重复打开
3. **对象池集成**: UI 预制体通过 GameObjectPoolManager 管理
4. **层级栈管理**: 使用 LinkedList 维护窗口显示顺序
5. **生命周期系统**: 完整的 OnCreate/OnEnable/OnDisable/OnDestroy
6. **屏幕适配**: 安全区域、刘海屏适配方案
7. **消息盒子**: 支持多例、定时关闭的临时窗口

---

## UI 层级系统

UIManager 定义了多个 UI 层级（详见 UIManager.Layers.cs）：

| 层级 | 说明 | 典型用途 |
|------|------|----------|
| `BackgroundLayer` | 背景层 | 主界面背景 |
| `GameBackgroundLayer` | 游戏背景层 | 游戏场景背景 |
| `NormalLayer` | 普通层 | 一般 UI 窗口 |
| `GameLayer` | 游戏层 | 游戏内 UI |
| `TipLayer` | 提示层 | 消息框、提示 |
| `TopLayer` | 顶层 | 全屏遮罩、引导 |

**层级优先级**: TopLayer > TipLayer > GameLayer > NormalLayer > GameBackgroundLayer > BackgroundLayer

---

## 窗口状态机

```
UIWindowLoadingState:
  NotStart → Loading → LoadOver
                ↑
                └── 加载失败时可能回退
```

| 状态 | 说明 |
|------|------|
| `NotStart` | 未开始加载 |
| `Loading` | 正在加载 |
| `LoadOver` | 加载完成 |

---

## 使用示例

### 示例 1: 打开窗口

```csharp
// 无参数窗口
await UIManager.Instance.OpenWindow<UILoadingView>(UILoadingView.PrefabPath);

// 带参数窗口
await UIManager.Instance.OpenWindow<UIAuctionView, int>(
    UIAuctionView.PrefabPath, 
    levelId
);

// 带多个参数
await UIManager.Instance.OpenWindow<UIAuctionView, int, string, bool>(
    UIAuctionView.PrefabPath,
    levelId,
    playerName,
    isGuide
);
```

### 示例 2: 打开消息盒子

```csharp
// 简单提示框
await UIManager.Instance.OpenBox<UIToastView>(
    UIToast.PrefabPath,
    during: 3000  // 3 秒后自动关闭
);

// 带参数的消息框
var result = await UIManager.Instance.OpenBox<UIMsgBoxWin, MsgBoxPara>(
    UIMsgBoxWin.PrefabPath,
    new MsgBoxPara
    {
        Content = "确定要退出吗？",
        ConfirmText = "确定",
        CancelText = "取消",
        ConfirmCallback = (win) => { /* 确定逻辑 */ },
        CancelCallback = (win) => { /* 取消逻辑 */ }
    }
);
```

### 示例 3: 关闭窗口

```csharp
// 关闭指定窗口
await UIManager.Instance.CloseWindow<UILoadingView>();

// 销毁窗口（释放资源）
await UIManager.Instance.DestroyWindow<UILoadingView>();

// 关闭整个层级
await UIManager.Instance.CloseWindowByLayer(UILayerNames.TipLayer);

// 销毁所有窗口
await UIManager.Instance.DestroyAllWindow();
```

### 示例 4: 获取窗口

```csharp
// 获取窗口实例
var loadingView = UIManager.Instance.GetView<UILoadingView>();
loadingView?.SetProgress(0.5f);

// 检查窗口状态
if (UIManager.Instance.IsWindowActive<UIAuctionView>(active: 2))
{
    // 窗口已打开且加载完成
}

// 获取最顶层窗口
var topWin = UIManager.Instance.GetTopWindow();
```

---

## 相关文档

- [UIManager.Layers.cs.md](./UIManager.Layers.cs.md) - UI 层级系统
- [UIWindow.cs.md](./UIWindow.cs.md) - 窗口数据结构
- [UIBaseView.cs.md](./UIBaseView.cs.md) - UI 基类
- [IOnCreate.cs.md](./IOnCreate.cs.md) - 生命周期接口
- [GameObjectPoolManager.cs.md](../Resource/GameObjectPoolManager.cs.md) - 对象池管理
- [PROJECT_DOCUMENTATION.md](../../../PROJECT_DOCUMENTATION.md) - 项目全景文档

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
