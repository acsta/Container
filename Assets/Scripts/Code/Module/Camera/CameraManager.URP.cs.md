# CameraManager.URP.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | CameraManager.URP.cs |
| **路径** | Assets/Scripts/Code/Module/Camera/CameraManager.URP.cs |
| **所属模块** | 框架层 → Code/Module/Camera |
| **文件职责** | URP 渲染管线下的相机栈管理 |

---

## 类说明

### CameraManager (URP 分部)

| 属性 | 说明 |
|------|------|
| **职责** | 管理 URP 渲染管线的相机栈配置 |
| **渲染管线** | Universal Render Pipeline (URP) |
| **继承关系** | CameraManager 的分部类 |

```csharp
public partial class CameraManager
{
    #region URPCamera
    // URP 特定的相机管理逻辑
    #endregion
}
```

---

## 字段说明

### sceneMainCameraGo

```csharp
private GameObject sceneMainCameraGo;
```

**说明**: 场景主相机 GameObject 引用

**用途**: 用于销毁时清理

---

### sceneMainCamera

```csharp
private Camera sceneMainCamera;
```

**说明**: 场景主相机 Camera 组件引用

**用途**: 用于相机栈配置和获取

---

## 方法说明

### SetCameraStackAtLoadingStart()

**签名**:
```csharp
public void SetCameraStackAtLoadingStart()
```

**职责**: 在场景 Loading 开始时设置相机栈

**核心逻辑**:
```
1. 获取 UI 相机
2. WebGL 1.0 平台：激活 UI 相机
3. 其他平台：设置 UI 相机为 Base 渲染类型
4. 重置场景相机
```

**用途**: Loading 期间场景被销毁，需要将 UI 相机从 Overlay 切换到 Base 类型

**使用示例**:
```csharp
// 在 Loading 开始时调用
void OnLoadingStart()
{
    CameraManager.Instance.SetCameraStackAtLoadingStart();
}
```

---

### ResetSceneCamera()

**签名**:
```csharp
public void ResetSceneCamera()
```

**职责**: 重置场景相机引用

**核心逻辑**:
```
1. 清空 sceneMainCameraGo 引用
2. 清空 sceneMainCamera 引用
```

**用途**: 场景切换时清理旧相机引用

---

### SetCameraStackAtLoadingDone()

**签名**:
```csharp
public void SetCameraStackAtLoadingDone()
```

**职责**: 在场景 Loading 完成后设置相机栈

**核心逻辑**:
```
1. 获取场景主相机 (Camera.main)
2. 如果已有旧相机引用，销毁旧相机
3. 设置新的场景主相机
4. 配置 URP 渲染数据：
   - 启用后处理 (renderPostProcessing = true)
   - 设置渲染类型为 Base
   - 设置渲染器索引 (SetRenderer(1))
5. 获取 UI 相机
6. 标记 UI 相机为 UI 相机 (isUICamera = true)
7. 将 UI 相机添加为 Overlay 相机
```

**使用示例**:
```csharp
// 在 Loading 完成后调用
void OnLoadingDone()
{
    CameraManager.Instance.SetCameraStackAtLoadingDone();
}
```

---

### AddOverlayCamera()

**签名**:
```csharp
void AddOverlayCamera(Camera baseCamera, Camera overlayCamera)
```

**职责**: 将 UI 相机添加为 Base 相机的 Overlay

**参数**:
| 参数 | 类型 | 说明 |
|------|------|------|
| `baseCamera` | Camera | 基础相机（场景相机） |
| `overlayCamera` | Camera | 叠加相机（UI 相机） |

**核心逻辑**:
```
1. WebGL 1.0 平台：禁用 UI 相机（不支持 Overlay）
2. 其他平台：
   - 设置 UI 相机为 Overlay 渲染类型
   - 将 UI 相机添加到 Base 相机的 cameraStack
```

**URP 相机栈**:
```
Base Camera (场景)
    └── Overlay Camera (UI)
            └── Overlay Camera (其他 UI)
```

---

### MainCamera()

**签名**:
```csharp
public Camera MainCamera()
```

**职责**: 获取场景主相机

**返回**: 场景主相机实例

**实现**:
```csharp
public Camera MainCamera()
{
    return this.sceneMainCamera;
}
```

**使用示例**:
```csharp
var mainCam = CameraManager.Instance.MainCamera();
Vector3 camPos = mainCam.transform.position;
```

---

## URP 相机栈机制

### 相机渲染类型

| 类型 | 说明 | 用途 |
|------|------|------|
| Base | 基础渲染层 | 场景、3D 物体 |
| Overlay | 叠加渲染层 | UI、特效 |

### 相机栈配置

```
Loading 前:
    Scene Camera (Base)
        └── UI Camera (Overlay)

Loading 中（场景销毁）:
    UI Camera (Base)  ← 临时切换

Loading 后:
    Scene Camera (Base)
        └── UI Camera (Overlay)
```

---

## 使用场景

### 场景 1: 场景切换

```csharp
// 场景切换流程
async Task SwitchScene(string sceneName)
{
    // 1. Loading 开始
    CameraManager.Instance.SetCameraStackAtLoadingStart();
    
    // 2. 加载场景
    await LoadSceneAsync(sceneName);
    
    // 3. Loading 完成
    CameraManager.Instance.SetCameraStackAtLoadingDone();
}
```

### 场景 2: 动态 UI 相机

```csharp
// 获取 UI 相机并配置
var uiCamera = UIManager.Instance.GetUICamera();
var urpData = uiCamera.GetUniversalAdditionalCameraData();

// 配置为 UI 相机
urpData.isUICamera = true;
urpData.renderType = CameraRenderType.Overlay;
```

### 场景 3: 多相机渲染

```csharp
// 添加多个 Overlay 相机
var baseCamera = CameraManager.Instance.MainCamera();
var uiCamera = UIManager.Instance.GetUICamera();
var effectCamera = GetEffectCamera();

// UI 相机作为 Overlay
CameraManager.Instance.AddOverlayCamera(baseCamera, uiCamera);

// 特效相机也作为 Overlay
CameraManager.Instance.AddOverlayCamera(baseCamera, effectCamera);
```

---

## 平台差异处理

### WebGL 1.0

WebGL 1.0 不支持 URP 相机栈，需要特殊处理：

```csharp
if (PlatformUtil.IsWebGl1())
{
    // 禁用 UI 相机，使用其他方式渲染 UI
    uiCamera.gameObject.SetActive(false);
}
else
{
    // 正常使用相机栈
    uiCamera.GetUniversalAdditionalCameraData().renderType = CameraRenderType.Overlay;
}
```

---

## URP 配置项

### UniversalAdditionalCameraData

| 属性 | 说明 | 常用值 |
|------|------|--------|
| `renderType` | 渲染类型 | Base/Overlay |
| `renderPostProcessing` | 后处理 | true/false |
| `isUICamera` | 是否 UI 相机 | true/false |
| `cameraStack` | 相机栈列表 | List<Camera> |

### 配置示例

```csharp
// 场景相机配置
var sceneData = sceneMainCamera.GetUniversalAdditionalCameraData();
sceneData.renderPostProcessing = true;
sceneData.renderType = CameraRenderType.Base;
sceneData.SetRenderer(1);  // 使用第二个渲染器

// UI 相机配置
var uiData = uiCamera.GetUniversalAdditionalCameraData();
uiData.isUICamera = true;
uiData.renderType = CameraRenderType.Overlay;
```

---

## 注意事项

### 1. 相机栈顺序

Overlay 相机的渲染顺序由添加顺序决定：

```csharp
// 先添加的在下层
AddOverlayCamera(base, uiCamera);      // UI 在下层
AddOverlayCamera(base, effectCamera);  // 特效在上层
```

### 2. 内存管理

场景切换时需要清理旧相机：

```csharp
if (sceneMainCamera != null)
{
    sceneMainCamera = null;
    Object.Destroy(sceneMainCameraGo);  // 销毁旧相机
}
```

### 3. WebGL 兼容性

WebGL 1.0 需要特殊处理，不能依赖相机栈

---

## 相关文档

- [CameraManager.cs.md](./CameraManager.cs.md) - 相机管理器主类
- [CameraManager.TaoTieRP.cs.md](./CameraManager.TaoTieRP.cs.md) - TaoTieRP 实现
- [UIManager.cs.md](../UI/UIManager.cs.md) - UI 管理器

---

*文档生成时间：2026-03-01 | OpenClaw AI 助手*
