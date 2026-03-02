# Init.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | Init.cs |
| **路径** | Assets/Scripts/Mono/Init.cs |
| **所属模块** | 框架层 → Mono |
| **文件职责** | 游戏入口初始化，负责 Unity 生命周期管理、SDK 初始化、热更新启动 |

---

## 类/结构体说明

### CodeMode (枚举)

| 值 | 名称 | 说明 |
|----|------|------|
| 1 | `LoadDll` | 加载外部 DLL 热更新 |
| 2 | `BuildIn` | 代码内置到整包 |
| 3 | `Wolong` | 卧龙模式（HybridCLR） |
| 4 | `LoadFromUrl` | 从 URL 加载 DLL（调试用） |

```csharp
public enum CodeMode
{
    LoadDll = 1,      // 加载 dll
    BuildIn = 2,      // 直接打进整包
    Wolong = 3,       // HybridCLR 模式
    LoadFromUrl = 4,  // 从 URL 加载
}
```

---

### Init (类)

| 属性 | 说明 |
|------|------|
| **职责** | 游戏入口 MonoBehaviour，负责初始化流程 |
| **继承** | `MonoBehaviour` |
| **核心功能** | Unity 设置、SDK 初始化、资源管理、代码加载、生命周期转发 |

```csharp
public class Init : MonoBehaviour
{
    public CodeMode CodeMode = CodeMode.LoadDll;
    public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;
    
    // 初始化流程
    private async ETTask AwakeAsync();
    async ETTask InitSDK();
    
    // Unity 生命周期
    private void Start();
    private void Update();
    private void LateUpdate();
    private void FixedUpdate();
    
    // 其他方法
    public async ETTask ReStart();
    private void RegisterManager();
    void InitUnitySetting();
}
```

---

## 字段与属性

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `CodeMode` | `CodeMode` | `public` | 代码加载模式 |
| `PlayMode` | `EPlayMode` | `public` | 资源播放模式 |
| `IsInit` | `bool` | `private` | 是否已初始化 |
| `waitForEndOfFrame` | `WaitForEndOfFrame` | `private` | 帧结束等待对象 |

---

## 方法说明（按重要程度排序）

### AwakeAsync()

**签名**:
```csharp
private async ETTask AwakeAsync()
```

**职责**: 异步初始化流程

**核心逻辑**:
```
1. 平台检查：校正 PlayMode（WebGL 必须用 WebPlayMode）
2. 初始化 Unity 设置 InitUnitySetting()
3. 设置时区 TimeInfo.Instance.TimeZone
4. 注册全局异常处理
5. 设置 DontDestroyOnLoad
6. 注册 ETTask 异常处理
7. 设置日志系统 Log.ILog = new UnityLogger()
8. 初始化 SDK InitSDK()
9. 初始化资源管理器 PackageManager.Instance.Init()
10. 校正 CodeMode（AOT 检查）
11. 注册管理器 RegisterManager()
12. 设置 CodeLoader 模式
13. 启动代码加载 CodeLoader.Instance.Start()
```

**平台校正**:
```csharp
#if UNITY_WEBGL
    if (PlayMode != EPlayMode.WebPlayMode)
    {
        PlayMode = EPlayMode.WebPlayMode;
        Debug.LogError("Error PlayMode! " + PlayMode);
    }
#else
    if (PlayMode == EPlayMode.EditorSimulateMode || PlayMode == EPlayMode.WebPlayMode)
    {
        PlayMode = EPlayMode.HostPlayMode;
        Debug.LogError("Error PlayMode! " + PlayMode);
    }
#endif
```

**调用者**: `Start()`

---

### InitSDK()

**签名**:
```csharp
async ETTask InitSDK()
```

**职责**: 初始化各平台 SDK

**核心逻辑**:
```
根据预编译宏初始化对应平台 SDK：
- UNITY_WEBGL_TT: 字节跳动小游戏 SDK
- UNITY_WEBGL_TAPTAP: TapTap 小游戏 SDK
- UNITY_WEBGL_WeChat: 微信小游戏 SDK
- UNITY_WEBGL_BILIGAME: B 站小游戏 SDK
- UNITY_WEBGL_ALIPAY: 支付宝小程序 SDK
- UNITY_WEBGL_KS: 快手小游戏 SDK
- UNITY_WEBGL_FACEBOOK: Facebook Instant Games
- UNITY_WEBGL_MINIHOST: 迷你主机 SDK
- UNITY_WEBGL_4399: 4399 小游戏 SDK
- 其他：直接返回
```

**字节跳动 SDK 初始化示例**:
```csharp
TTSDK.TT.InitSDK((code, env) =>
{
    // 注册 OnShow 事件
    TTSDK.TT.GetAppLifeCycle().OnShow += (dic) =>
    {
        // 解析 scene 参数，判断进入方式
        string scene = dic.TryGetValue("scene", out var res) ? (string)res : null;
        int enterWay = 0;
        
        if (scene.EndsWith("1001") || scene.EndsWith("1036") || scene.EndsWith("1042"))
            enterWay = 1;  // 侧边栏
        else if (scene.EndsWith("1020"))
            enterWay = 2;  // 快捷方式
        
        if (Define.EnterWay != enterWay)
        {
            Define.EnterWay = enterWay;
            Messager.Instance.Broadcast(0, MessageId.EnterWayChange);
        }
    };
    task.SetResult();
});
```

**调用者**: `AwakeAsync()`

---

### Start()

**签名**:
```csharp
private void Start()
```

**职责**: Unity Start 生命周期，启动异步初始化

**核心逻辑**:
```
1. 调整 CanvasScaler 的 matchWidthOrHeight（适配不同屏幕比例）
2. 启动 AwakeAsync 协程
```

**屏幕适配**:
```csharp
var canvasScaler = GameObject.Find("Canvas").GetComponent<CanvasScaler>();
if (canvasScaler != null)
{
    if ((float)Screen.width / Screen.height > Define.DesignScreenWidth / Define.DesignScreenHeight)
        canvasScaler.matchWidthOrHeight = 1;  // 匹配高度
    else
        canvasScaler.matchWidthOrHeight = 0;  // 匹配宽度
}
AwakeAsync().Coroutine();
```

**调用者**: Unity 自动调用

---

### Update()

**签名**:
```csharp
private void Update()
```

**职责**: Unity Update 生命周期，转发时间更新和代码逻辑

**核心逻辑**:
```
1. 如果未初始化，直接返回
2. 更新时间信息 TimeInfo.Instance.Update()
3. 调用 CodeLoader.Update
4. 调用 ManagerProvider.Update()
5. 检查是否需要重启 ReStart()
6. 处理帧结束等待任务
```

**帧结束任务处理**:
```csharp
int count = UnityLifeTimeHelper.FrameFinishTask.Count;
if (count > 0)
{
    StartCoroutine(WaitFrameFinish());
}
```

**调用者**: Unity 自动调用

---

### WaitFrameFinish()

**签名**:
```csharp
private IEnumerator WaitFrameFinish()
```

**职责**: 协程等待帧结束，触发等待任务

**核心逻辑**:
```
1. 等待帧结束 yield return waitForEndOfFrame
2. 触发所有 FrameFinishTask 队列中的任务
```

**实现**:
```csharp
yield return waitForEndOfFrame;
int count = UnityLifeTimeHelper.FrameFinishTask.Count;
while (count-- > 0)
{
    ETTask task = UnityLifeTimeHelper.FrameFinishTask.Dequeue();
    task.SetResult();
}
```

**调用者**: `Update()`

---

### ReStart()

**签名**:
```csharp
public async ETTask ReStart()
```

**职责**: 热重启游戏（不退出应用）

**核心逻辑**:
```
1. 重置 isReStart 标志
2. 卸载未使用资源 Resources.UnloadUnusedAssets()
3. 强制卸载所有资源 PackageManager.Instance.ForceUnloadAllAssets()
4. 再次卸载未使用资源
5. 清空管理器 ManagerProvider.Clear()
6. 更新配置 PackageManager.Instance.UpdateConfig()
7. 强制 GC（两次）
8. 重新注册管理器 RegisterManager()
9. 调用 OnApplicationQuit 回调
10. 重新启动代码加载 CodeLoader.Instance.Start()
```

**调用者**: `Update()`（当 isReStart 为 true 时）

**使用示例**:
```csharp
// 触发热重启
CodeLoader.Instance.ReStart();
```

---

### RegisterManager()

**签名**:
```csharp
private void RegisterManager()
```

**职责**: 注册核心管理器

**核心逻辑**:
```
1. 注册 PerformanceManager（性能管理）
2. 注册 AssemblyManager（程序集管理）
```

**实现**:
```csharp
ManagerProvider.RegisterManager<PerformanceManager>();
ManagerProvider.RegisterManager<AssemblyManager>();
```

**调用者**: `AwakeAsync()`, `ReStart()`

---

### LateUpdate()

**签名**:
```csharp
private void LateUpdate()
```

**职责**: Unity LateUpdate 生命周期转发

**核心逻辑**:
```
1. 调用 CodeLoader.LateUpdate
2. 调用 ManagerProvider.LateUpdate()
```

**调用者**: Unity 自动调用

---

### FixedUpdate()

**签名**:
```csharp
private void FixedUpdate()
```

**职责**: Unity FixedUpdate 生命周期转发

**核心逻辑**:
```
1. 调用 CodeLoader.FixedUpdate
2. 调用 ManagerProvider.FixedUpdate()
```

**调用者**: Unity 自动调用

---

### OnApplicationQuit()

**签名**:
```csharp
private void OnApplicationQuit()
```

**职责**: 应用退出回调转发

**核心逻辑**:
```
1. 调用 CodeLoader.OnApplicationQuit
```

**调用者**: Unity 自动调用

---

### OnApplicationFocus(bool hasFocus) / OnApplicationPause(bool pauseStatus)

**签名**:
```csharp
void OnApplicationFocus(bool hasFocus)
void OnApplicationPause(bool pauseStatus)
```

**职责**: 应用焦点/暂停回调转发

**核心逻辑**:
```
1. 调用 CodeLoader.OnApplicationFocus(hasFocus)
```

**调用者**: Unity 自动调用

---

### InitUnitySetting()

**签名**:
```csharp
void InitUnitySetting()
```

**职责**: 初始化 Unity 设置

**核心逻辑**:
```
1. 禁用多点触控 Input.multiTouchEnabled = false
2. 禁用垂直同步 QualitySettings.vSyncCount = 0
3. 允许后台运行 Application.runInBackground = true
4. 初始化随机数种子
5. 设置屏幕常亮（根据平台）
```

**实现**:
```csharp
Input.multiTouchEnabled = false;
QualitySettings.vSyncCount = 0;
Application.runInBackground = true;
UnityEngine.Random.InitState((int) TimeInfo.Instance.ServerNow());

#if UNITY_WEBGL_TT
    TTSDK.TT.SetKeepScreenOn(true);
#else 
    Screen.sleepTimeout = SleepTimeout.NeverSleep;
#endif
```

**调用者**: `AwakeAsync()`

---

## 阅读指引

### 建议的阅读顺序

1. **理解 Init 的作用** - 游戏入口点
2. **看 AwakeAsync** - 主初始化流程
3. **看 InitSDK** - 平台 SDK 初始化
4. **看生命周期转发** - Update/LateUpdate/FixedUpdate

### 最值得学习的技术点

1. **异步初始化**: 使用 ETTask 进行异步流程控制
2. **平台适配**: 多平台 SDK 初始化
3. **热重启机制**: ReStart 实现不退出应用的重启
4. **生命周期转发**: 将 Unity 生命周期转发给框架
5. **屏幕适配**: CanvasScaler 动态调整

---

## 使用示例

### 示例 1: 配置 Init 组件

```csharp
// 在 Unity 编辑器中配置 Init 组件
// Init.cs 挂载到场景中的 GameObject 上

// 设置代码模式
init.CodeMode = CodeMode.LoadDll;  // 热更新模式
init.PlayMode = EPlayMode.HostPlayMode;  // 本地服务器模式
```

### 示例 2: 触发热重启

```csharp
// 在游戏内触发热重启（如版本更新后）
if (needRestart)
{
    CodeLoader.Instance.ReStart();
}
```

### 示例 3: 监听应用焦点变化

```csharp
// 在代码中监听应用焦点
CodeLoader.Instance.OnApplicationFocus = (hasFocus) =>
{
    if (hasFocus)
    {
        Log.Info("应用获得焦点");
        ResumeGame();
    }
    else
    {
        Log.Info("应用失去焦点");
        PauseGame();
    }
};
```

---

## 注意事项

1. **单例模式**: Init 组件应该只有一个实例
2. **DontDestroyOnLoad**: Init GameObject 不会被场景切换销毁
3. **平台校正**: PlayMode 和 CodeMode 会根据平台自动校正
4. **异常处理**: 全局异常会记录到日志系统
5. **热重启**: ReStart 会清空所有管理器状态

---

## 相关文档

- [CodeLoader.cs.md](Module/CodeLoader/CodeLoader.cs.md) - 代码加载器
- [PackageManager.cs.md](Module/YooAssets/PackageManager.cs.md) - 资源管理器
- [ManagerProvider.cs.md](Core/Manager/ManagerProvider.cs.md) - 管理器提供者
- [TimeInfo.cs.md](Module/Timer/TimeInfo.cs.md) - 时间信息
- [UnityLifeTimeHelper.cs.md](Helper/UnityLifeTimeHelper.cs.md) - 生命周期辅助

---

*文档生成时间：2026-03-01 | OpenClaw AI 助手*
