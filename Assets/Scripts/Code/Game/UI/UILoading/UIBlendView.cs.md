# UIBlendView.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | UIBlendView.cs |
| **路径** | Assets/Scripts/Code/Game/UI/UILoading/UIBlendView.cs |
| **所属模块** | 玩法层 → UI 加载界面 |
| **文件职责** | 屏幕截图淡出过渡效果，用于场景切换时的平滑过渡 |

---

## 类/结构体说明

### UIBlendView

| 属性 | 说明 |
|------|------|
| **职责** | 屏幕截图淡出过渡视图类 |
| **泛型参数** | 无 |
| **继承关系** | 继承 `UIBaseView` |
| **实现的接口** | `IOnCreate`, `IOnEnable`, `IOnDisable` |

**设计模式**: 屏幕后处理 + 渐变动画

```csharp
// 使用示例
var blendView = await UIManager.Instance.OpenWindow<UIBlendView>(
    UIBlendView.PrefabPath
);

// 捕获背景（隐藏 UI）
await blendView.CaptureBg(ignoreUI: true);

// 执行淡出过渡
await blendView.DoFade(during: 1000);
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `PrefabPath` | `string` | `public static` | 预制体路径：`"UI/UILoading/Prefabs/UIBlendView.prefab"` |
| `RawImage` | `UIRawImage` | `public` | 原始图像组件（显示截图） |
| `Material` | `Material` | `public` | 材质实例（控制渐变） |
| `Loading` | `UIImage` | `public` | 加载指示器图片 |
| `Progress` | `int` | `private` | Shader 进度参数 ID |
| `screenShotTemp` | `Texture2D` | `private static` | 截图纹理缓存（静态单例） |

---

## 生命周期方法

### OnCreate()

**签名**:
```csharp
public void OnCreate()
```

**职责**: 初始化组件引用和 Shader 参数

**核心逻辑**:
```
1. 获取 RawImage 原始图像组件（"RawImage"）
2. 获取 Loading 加载图片组件（"Loading"）
3. 获取材质实例 Material = RawImage.GetMaterial()
4. 获取 Shader 进度参数 ID Shader.PropertyToID("_Progress")
```

**调用者**: UIManager（窗口创建时自动调用）

---

### OnEnable()

**签名**:
```csharp
public void OnEnable()
```

**职责**: 启用时重置状态

**核心逻辑**:
```
1. 隐藏 Loading 图片
2. 禁用 RawImage 显示
3. 设置 Shader 进度为 0
```

**调用者**: UIManager（窗口启用时自动调用）

---

### OnDisable()

**签名**:
```csharp
public void OnDisable()
```

**职责**: 禁用时清理截图纹理

**核心逻辑**:
```
1. 如果 screenShotTemp 不为空:
   - 设置 RawImage 纹理为 null
   - 销毁 screenShotTemp
   - 清空 screenShotTemp 引用
```

**调用者**: UIManager（窗口禁用时自动调用）

**资源管理**: 防止纹理泄漏

---

## 公共方法

### CaptureBg(bool ignoreUI = false)

**签名**:
```csharp
public async ETTask CaptureBg(bool ignoreUI = false)
```

**职责**: 捕获当前屏幕截图作为背景

**参数说明**:
- `ignoreUI`: 是否隐藏 UI 层（默认 false）

**核心逻辑**:
```
1. 关闭引导窗口 UIGuidanceView
2. 清理旧截图纹理（如果有）
3. 如果需要隐藏 UI:
   a. 获取主相机
   b. 如果是 WebGL1:
      - 从 cullingMask 移除 UI 层
   c. 否则（URP）:
      - 遍历相机栈
      - 找到 isUICamera 的相机
      - 隐藏该相机
4. 等待帧完成 UnityLifeTimeHelper.WaitFrameFinish()
5. 创建临时纹理 screenShotTemp:
   - 大小：Screen.width x Screen.height
   - 格式：TextureFormat.RGB24
6. 读取屏幕像素 screenShotTemp.ReadPixels()
7. 应用纹理 screenShotTemp.Apply()
8. 设置 RawImage 纹理
9. 启用 RawImage 显示
10. 恢复 UI 层显示:
    - WebGL1: 恢复 cullingMask
    - URP: 恢复 UI 相机
11. 显示 Loading 图片
12. 重新打开引导窗口
```

**调用者**: 场景切换代码

**异步行为**: 等待帧完成和纹理操作

**平台适配**:
- **WebGL1**: 使用 cullingMask 控制 UI 层
- **URP**: 使用相机栈控制 UI 相机

---

### DoFade(float during = 1000)

**签名**:
```csharp
public async ETTask DoFade(float during = 1000)
```

**职责**: 执行淡出过渡动画

**参数说明**:
- `during`: 动画时长（毫秒，默认 1000ms）

**核心逻辑**:
```
1. 隐藏 Loading 图片
2. 记录开始时间 startTime
3. 淡出循环:
   - 每帧等待 1ms
   - 计算当前时间 timeNow
   - 计算进度 progress = Clamp01((timeNow - startTime) / during)
   - 如果 progress >= 1 → 跳出循环
   - 应用曲线 progress = -0.1 + 2.1 * progress
   - 设置 Shader 进度 Material.SetFloat(Progress, progress)
4. 设置最终进度 Material.SetFloat(Progress, 2.1f)
```

**进度曲线**:
```
输入 progress: 0.0 → 1.0
输出 progress: -0.1 → 2.0

曲线公式：output = -0.1 + 2.1 * input

效果：从 -0.1 开始，到 2.0 结束，覆盖完整的 Shader 渐变范围
```

**调用者**: 场景切换代码

**异步行为**: 逐帧执行淡出动画

---

## 使用场景

### 1. 场景切换淡出
```csharp
// 在场景切换前
var blendView = await UIManager.Instance.OpenWindow<UIBlendView>(
    UIBlendView.PrefabPath,
    UILayerNames.EffectLayer
);

// 捕获当前场景（不含 UI）
await blendView.CaptureBg(ignoreUI: true);

// 执行 1 秒淡出
await blendView.DoFade(during: 1000);

// 切换场景
await SceneManager.SwitchScene<HomeScene>();

// 关闭窗口
await blendView.CloseSelf();
```

### 2. 转场特效
```csharp
public async ETTask TransitionWithBlend()
{
    var blendView = await UIManager.Instance.OpenWindow<UIBlendView>(
        UIBlendView.PrefabPath
    );
    
    try
    {
        await blendView.CaptureBg(ignoreUI: false);
        await blendView.DoFade(during: 1500);
        
        // 在淡出过程中执行场景加载
        await LoadNextScene();
    }
    finally
    {
        await blendView.CloseSelf();
    }
}
```

---

## UI 结构

```
UIBlendView (UIBaseView)
├── RawImage (UIRawImage) - 屏幕截图显示
│   └── Material (Material) - 渐变材质
│       └── _Progress (float) - 渐变进度参数
└── Loading (UIImage) - 加载指示器
```

---

## Shader 进度范围

| 进度值 | 效果 |
|--------|------|
| `0` | 完全透明（可见背景） |
| `0.5` | 半透明过渡 |
| `1.0` | 接近不透明 |
| `2.0` | 完全不透明（黑色） |
| `-0.1` | 起始值（确保完全可见） |
| `2.1` | 结束值（确保完全黑色） |

**DoFade 曲线**:
```
时间：0ms          500ms         1000ms
      │             │             │
      ▼             ▼             ▼
进度：-0.1 ─────→ 1.0 ─────→ 2.1
      (可见)    (半透明)    (黑色)
```

---

## 平台适配

### WebGL1
```csharp
// 使用 cullingMask 控制 UI 层
mainCamera.cullingMask -= LayerMask.GetMask("UI"); // 隐藏 UI
// ... 截图 ...
mainCamera.cullingMask += LayerMask.GetMask("UI"); // 恢复 UI
```

### URP（通用渲染管线）
```csharp
// 使用相机栈控制 UI 相机
var cd = mainCamera.GetUniversalAdditionalCameraData();
for (int i = 0; i < cd.cameraStack.Count; i++)
{
    if (cd.cameraStack[i].GetUniversalAdditionalCameraData().isUICamera)
    {
        cd.cameraStack[i].gameObject.SetActive(false); // 隐藏 UI 相机
        break;
    }
}
// ... 截图 ...
uiCamera.SetActive(true); // 恢复 UI 相机
```

---

## 资源管理

### 静态纹理缓存
```csharp
private static Texture2D screenShotTemp;
```

**设计意图**:
- 单例模式，避免重复创建纹理
- 在 OnDisable 时清理，防止内存泄漏
- 在 CaptureBg 时复用，减少 GC

**生命周期**:
```
创建 → CaptureBg (首次) → 复用 → OnDisable (销毁)
```

---

## 性能优化

### 1. 纹理复用
```csharp
if (screenShotTemp == null)
{
    // 首次创建
    screenShotTemp = new Texture2D(...);
}
// 后续调用直接复用
```

### 2. 帧同步
```csharp
await UnityLifeTimeHelper.WaitFrameFinish();
```
确保在帧完成后读取屏幕，避免撕裂

### 3. 条件隐藏 UI
```csharp
if (ignoreUI)
{
    // 只在需要时隐藏 UI
}
```

---

## 注意事项

### 1. 纹理内存
截图纹理大小为 `Screen.width × Screen.height × 3 bytes`
- 1920×1080 = ~6MB
- 在 OnDisable 时务必清理

### 2. WebGL 兼容性
WebGL1 不支持相机栈，使用 cullingMask 方式

### 3. 引导窗口
CaptureBg 会临时关闭引导窗口，完成后自动恢复

---

## 相关文档

- [UIManager.cs.md](../../Module/UI/UIManager.cs.md) - UI 管理器
- [UIBaseView.cs.md](../../Module/UI/UIBaseView.cs.md) - UI 视图基类
- [UIRawImage.cs.md](../../Module/UIComponent/UIRawImage.cs.md) - 原始图像组件
- [UIImage.cs.md](../../Module/UIComponent/UIImage.cs.md) - 图片组件
- [CameraManager.cs.md](../../Module/Camera/CameraManager.cs.md) - 相机管理器
- [UIGuidanceView.cs.md](../UIGuidance/UIGuidanceView.cs.md) - 引导窗口
- [SceneManager.cs.md](../../Module/Scene/SceneManager.cs.md) - 场景管理器
- [UnityLifeTimeHelper.cs](#) - Unity 生命周期工具

---

*文档生成时间：2026-03-01 | OpenClaw AI 助手*
