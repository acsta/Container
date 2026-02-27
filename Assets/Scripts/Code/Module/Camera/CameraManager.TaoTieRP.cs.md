# CameraManager.TaoTieRP.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | CameraManager.TaoTieRP.cs |
| **路径** | Assets/Scripts/Code/Module/Camera/CameraManager.TaoTieRP.cs |
| **所属模块** | 框架层 → Code/Module/Camera |
| **文件职责** | TaoTieRP 渲染管线相机管理（Partial Class） |

---

## 文件说明

本文件是 `CameraManager` 的 **Partial Class** 扩展，专注于 **TaoTieRP（自定义渲染管线）** 的相机管理。

**注意**: 当前文件内容已被注释，表示 TaoTieRP 功能暂未启用或使用 URP 替代。

---

## 当前状态

### 已注释的代码

```csharp
// using UnityEngine;
//
// namespace TaoTie
// {
//     public partial class CameraManager
//     {
//         #region URPCamera
//         
//         private GameObject sceneMainCameraGo;
//         private Camera sceneMainCamera;
//         
//         // 在场景 loading 开始时设置 camera statck
//         // loading 时场景被销毁，这个时候需要将 UI 摄像机从 overlay->base
//         public void SetCameraStackAtLoadingStart() { ... }
//         
//         public void ResetSceneCamera() { ... }
//         
//         public void SetCameraStackAtLoadingDone() { ... }
//         
//         void AddOverlayCamera(Camera baseCamera, Camera overlayCamera) { ... }
//         
//         public Camera MainCamera() { ... }
//         
//         #endregion
//     }
// }
```

---

## 设计意图

### TaoTieRP vs URP

| 特性 | URP | TaoTieRP |
|------|-----|----------|
| 来源 | Unity 官方 | 项目自定义 |
| 成熟度 | 高 | 开发中 |
| 当前使用 | ✅ 启用 | ⏸️ 注释 |

### 未来启用

当 TaoTieRP 开发完成后，可以取消注释并使用自定义渲染管线。

---

## 与 URP 的关系

当前项目使用 URP 渲染管线，相关功能在 `CameraManager.URP.cs` 中实现。

TaoTieRP.cs 保留作为：
1. 未来自定义渲染管线的扩展点
2. URP 的备选方案
3. 特殊平台的定制需求

---

## 使用示例

### 当前（使用 URP）

```csharp
// 当前项目使用 URP
// CameraManager.URP.cs 中的方法生效

CameraManager.Instance.SetCameraStackAtLoadingStart();
CameraManager.Instance.SetCameraStackAtLoadingDone();
```

### 未来（启用 TaoTieRP）

```csharp
// 取消注释 TaoTieRP.cs
// 可能需要条件编译区分

#if USE_TAOTIERP
    // TaoTieRP 逻辑
#else
    // URP 逻辑
#endif
```

---

## 相关文档

- [CameraManager.cs.md](./CameraManager.cs.md) - 相机管理器（主文件）
- [CameraManager.URP.cs.md](./CameraManager.URP.cs.md) - URP 渲染管线

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
