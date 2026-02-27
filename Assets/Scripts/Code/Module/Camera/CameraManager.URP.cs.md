# CameraManager.URP.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | CameraManager.URP.cs |
| **路径** | Assets/Scripts/Code/Module/Camera/CameraManager.URP.cs |
| **所属模块** | 框架层 → Code/Module/Camera |
| **文件职责** | URP 渲染管线相机管理（Partial Class） |

---

## 文件说明

本文件是 `CameraManager` 的 **Partial Class** 扩展，专注于 **URP（Universal Render Pipeline）** 渲染管线的相机管理。

---

## 字段与属性

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `sceneMainCameraGo` | `GameObject` | `private` | 场景主相机 GameObject |
| `sceneMainCamera` | `Camera` | `private` | 场景主相机 Camera |

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
2. 如果是 WebGL 1.0，激活 UI 相机
3. 否则，设置 UI 相机为 Base 渲染类型
4. 重置场景相机
```

**URP 相机栈**:
```csharp
var uiCamera = UIManager.Instance.GetUICamera();
if (PlatformUtil.IsWebGl1())
    uiCamera.gameObject.SetActive(true);
else
    uiCamera.GetUniversalAdditionalCameraData().renderType = CameraRenderType.Base;
```

**调用者**: LoadingScene 开始加载时

---

### ResetSceneCamera()

**签名**:
```csharp
public void ResetSceneCamera()
```

**职责**: 重置场景相机引用

**核心逻辑**:
```
1. 清空 sceneMainCameraGo
2. 清空 sceneMainCamera
```

**调用者**: `SetCameraStackAtLoadingStart()`, 场景切换时

---

### SetCameraStackAtLoadingDone()

**签名**:
```csharp
public void SetCameraStackAtLoadingDone()
```

**职责**: 在场景 Loading 完成后设置相机栈

**核心逻辑**:
```
1. 获取场景主相机（Camera.main）
2. 如果场景已有主相机：
   - 销毁旧的 sceneMainCamera
   - 设置新的 sceneMainCamera
3. 配置 URP 渲染设置：
   - 启用后处理
   - 设置渲染类型为 Base
   - 设置渲染器索引
4. 配置 UI 相机为 Overlay 类型
5. 将 UI 相机添加到场景相机的相机栈
```

**URP 配置**:
```csharp
var render = this.sceneMainCamera.GetUniversalAdditionalCameraData();
render.renderPostProcessing = true;
render.renderType = CameraRenderType.Base;
render.SetRenderer(1);

var uiCamera = UIManager.Instance.GetUICamera();
uiCamera.GetUniversalAdditionalCameraData().isUICamera = true;
AddOverlayCamera(this.sceneMainCamera, uiCamera);
```

**调用者**: LoadingScene 加载完成时

---

### AddOverlayCamera(Camera baseCamera, Camera overlayCamera)

**签名**:
```csharp
void AddOverlayCamera(Camera baseCamera, Camera overlayCamera)
```

**职责**: 将 UI 相机添加为 Overlay 相机

**参数**:
- `baseCamera`: 基础相机（场景相机）
- `overlayCamera`: 覆盖相机（UI 相机）

**核心逻辑**:
```
1. 如果是 WebGL 1.0，禁用 UI 相机（不支持相机栈）
2. 否则：
   - 设置 UI 相机为 Overlay 渲染类型
   - 将 UI 相机添加到基础相机的相机栈
```

**WebGL 1.0 兼容**:
```csharp
if (PlatformUtil.IsWebGl1())
{
    overlayCamera.gameObject.SetActive(false);
}
else
{
    overlayCamera.GetUniversalAdditionalCameraData().renderType = CameraRenderType.Overlay;
    baseCamera.GetUniversalAdditionalCameraData().cameraStack.Add(overlayCamera);
}
```

**调用者**: `SetCameraStackAtLoadingDone()`

---

### MainCamera()

**签名**:
```csharp
public Camera MainCamera()
```

**职责**: 获取场景主相机

**返回**: 场景主相机 Camera

**核心逻辑**:
```
1. 返回 sceneMainCamera
```

**调用者**: 需要访问主相机的代码

---

## 阅读指引

### 建议的阅读顺序

1. **理解 URP 相机栈** - URP 如何管理多相机
2. **看 Loading 流程** - 理解相机切换时机
3. **看 AddOverlayCamera** - 理解 UI 相机配置
4. **了解 WebGL 兼容** - 理解平台差异

### 最值得学习的技术点

1. **URP 相机栈**: CameraStack 支持多相机渲染
2. **渲染类型**: Base/Overlay 区分主相机和 UI 相机
3. **Loading 切换**: Loading 时切换相机配置
4. **WebGL 兼容**: WebGL 1.0 不支持相机栈的处理

---

## URP 相机栈

### 渲染类型

| 类型 | 说明 | 用途 |
|------|------|------|
| `Base` | 基础相机 | 渲染场景主体 |
| `Overlay` | 覆盖相机 | 渲染 UI 层 |

### 相机栈结构

```
Scene Camera (Base)
    ├── 渲染场景
    ├── 后处理效果
    └── CameraStack
        └── UI Camera (Overlay)
            └── 渲染 UI
```

---

## Loading 流程中的相机切换

### Loading 开始

```
UI Camera: Overlay → Base（临时作为主相机）
Scene Camera: 重置为空
```

### Loading 完成

```
Scene Camera: 设置为 Base，启用后处理
UI Camera: Base → Overlay，添加到相机栈
```

---

## 使用示例

### 示例 1: 获取主相机

```csharp
// 获取场景主相机
Camera mainCamera = CameraManager.Instance.MainCamera();

// 设置相机位置
mainCamera.transform.position = new Vector3(0, 10, -10);
```

### 示例 2: Loading 流程

```csharp
// Loading 开始
void OnLoadingStart()
{
    CameraManager.Instance.SetCameraStackAtLoadingStart();
}

// Loading 完成
void OnLoadingDone()
{
    CameraManager.Instance.SetCameraStackAtLoadingDone();
}
```

---

## 相关文档

- [CameraManager.cs.md](./CameraManager.cs.md) - 相机管理器（主文件）
- [CameraManager.TaoTieRP.cs.md](./CameraManager.TaoTieRP.cs.md) - TaoTieRP 渲染管线
- [UIManager.cs.md](../UI/UIManager.cs.md) - UI 管理器

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
