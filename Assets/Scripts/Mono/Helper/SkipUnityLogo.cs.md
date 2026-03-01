# SkipUnityLogo.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | SkipUnityLogo.cs |
| **路径** | Assets/Scripts/Mono/Helper/SkipUnityLogo.cs |
| **所属模块** | 框架层 → Mono/Helper |
| **文件职责** | 跳过 Unity 启动 Logo，优化游戏启动体验 |

---

## 类/结构体说明

### SkipUnityLogo

| 属性 | 说明 |
|------|------|
| **职责** | 在适当时机跳过 Unity 启动 Logo 显示 |
| **类型** | `class` |
| **特性** | `[Preserve]` - 防止代码被裁剪 |
| **生命周期** | 自动在 `BeforeSplashScreen` 阶段初始化 |

```csharp
[Preserve]
public class SkipUnityLogo
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    private static void BeforeSplashScreen()
    {
        // 跳过 Logo 逻辑
    }
}
```

---

## 方法说明（按重要程度排序）

### BeforeSplashScreen()

**签名**:
```csharp
[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
private static void BeforeSplashScreen()
```

**职责**: 在 Unity 启动 Logo 显示前执行，决定是否跳过 Logo

**触发时机**: Unity 运行时初始化，`BeforeSplashScreen` 阶段

**核心逻辑**:
```
1. WebGL 平台：
   - 如果是 Debug 构建，直接返回（保留 Logo）
   - 如果是华为渠道 + URP 渲染管线，直接返回（避免黑屏问题）
   - 否则：订阅 Application.focusChanged 事件，在获得焦点时停止 Logo
2. 其他平台：
   - 异步执行 AsyncSkip 方法
```

**平台差异处理**:
```csharp
#if UNITY_WEBGL
    if(Debug.isDebugBuild) return;
    
    // URP + 华为渠道特殊处理（避免黑屏）
    if (PlatformUtil.IsHuaWeiGroup() &&
        GraphicsSettings.currentRenderPipeline?.GetType()?.Name?.Contains("UniversalRenderPipelineAsset") == true)
    {
        return;
    }
    
    Application.focusChanged += Application_focusChanged;
#else
    System.Threading.Tasks.Task.Run(AsyncSkip);
#endif
```

**调用者**: Unity 运行时自动调用

---

### Application_focusChanged(bool obj)

**签名**:
```csharp
private static void Application_focusChanged(bool obj)
```

**职责**: WebGL 平台下，当应用获得焦点时停止 Logo 显示

**参数**:
- `obj`: `true` = 获得焦点，`false` = 失去焦点

**核心逻辑**:
```
1. 取消订阅 focusChanged 事件（只触发一次）
2. 立即停止 Splash Screen
```

**实现**:
```csharp
Application.focusChanged -= Application_focusChanged;
SplashScreen.Stop(SplashScreen.StopBehavior.StopImmediate);
```

**调用者**: Unity `Application.focusChanged` 事件

---

### AsyncSkip()

**签名**:
```csharp
private static void AsyncSkip()
```

**职责**: 非 WebGL 平台下异步停止 Logo 显示

**核心逻辑**:
```
1. 立即停止 Splash Screen
```

**实现**:
```csharp
SplashScreen.Stop(SplashScreen.StopBehavior.StopImmediate);
```

**调用者**: `BeforeSplashScreen()` 中的异步任务

---

## 阅读指引

### 建议的阅读顺序

1. **理解跳过 Logo 的目的** - 为什么需要跳过
2. **看 BeforeSplashScreen** - 主入口方法
3. **看平台差异处理** - WebGL vs 其他平台

### 最值得学习的技术点

1. **运行时初始化**: `[RuntimeInitializeOnLoadMethod]` 特性
2. **代码保留**: `[Preserve]` 防止 IL2CPP 裁剪
3. **平台条件编译**: `#if UNITY_WEBGL` 区分平台
4. **事件订阅模式**: `Application.focusChanged` 事件处理
5. **特殊渠道处理**: 华为 + URP 黑屏问题规避

---

## 使用示例

### 示例 1: 自动生效（无需手动调用）

```csharp
// 该类会在 Unity 启动时自动执行
// 无需在任何地方手动调用

// 启动流程：
// 1. Unity 启动
// 2. 执行 [RuntimeInitializeOnLoadMethod] 标记的方法
// 3. SkipUnityLogo.BeforeSplashScreen() 被调用
// 4. 根据平台执行相应逻辑
// 5. Logo 被跳过或保留
```

### 示例 2: Debug 构建保留 Logo

```csharp
// Debug 构建时，SkipUnityLogo 会自动跳过
if (Debug.isDebugBuild)
{
    // Logo 会正常显示
    // 方便调试启动流程
}
```

### 示例 3: 华为渠道 URP 特殊处理

```csharp
// 华为渠道 + URP 渲染管线
if (PlatformUtil.IsHuaWeiGroup() && 
    GraphicsSettings.currentRenderPipeline is UniversalRenderPipelineAsset)
{
    // 跳过 Logo 会导致黑屏
    // SkipUnityLogo 会自动保留 Logo
}
```

---

## 技术细节

### RuntimeInitializeOnLoadMethod

**作用**: 在 Unity 运行时初始化的特定阶段自动执行方法

**可用阶段**:
```csharp
// BeforeSplashScreen - 在启动 Logo 显示前（最早）
[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]

// AfterSceneLoad - 在场景加载后
[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]

// BeforeSceneLoad - 在场景加载前
[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
```

### Preserve 特性

**作用**: 防止 IL2CPP 代码裁剪移除该类

**使用场景**:
- 通过反射调用的类
- 需要自动初始化的类
- 平台特定代码

```csharp
[Preserve]  // 告诉 IL2CPP 保留此类
public class SkipUnityLogo
{
    // ...
}
```

### SplashScreen.StopBehavior

**枚举值**:
```csharp
enum StopBehavior
{
    StopImmediate,      // 立即停止
    StopAfterFadeOut,   // 淡出后停止
    WaitForCurrentFrame // 等待当前帧结束
}
```

---

## 注意事项

1. **Debug 构建**: Debug 模式下会保留 Logo，方便调试
2. **华为渠道**: 华为设备 + URP 渲染管线会保留 Logo，避免黑屏
3. **WebGL 平台**: 使用焦点事件触发，其他平台使用异步任务
4. **自动执行**: 无需手动调用，Unity 启动时自动执行

---

## 相关文档

- [PlatformUtil.cs.md](./PlatformUtil.cs.md) - 平台工具类
- [Define.cs.md](../Define.cs.md) - 全局定义
- [Init.cs.md](../Init.cs.md) - 游戏初始化

---

*文档生成时间：2026-03-01 | OpenClaw AI 助手*
