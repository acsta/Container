# CameraManager.TaoTieRP.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | CameraManager.TaoTieRP.cs |
| **路径** | Assets/Scripts/Code/Module/Camera/CameraManager.TaoTieRP.cs |
| **所属模块** | 框架层 → Code/Module/Camera |
| **文件职责** | TaoTie 自研渲染管线的相机管理（预留） |

---

## 文件状态

> ⚠️ **注意**: 此文件当前为注释状态，代码未启用

**原因**: TaoTieRP 渲染管线尚未实现或正在开发中

---

## 类说明

### CameraManager (TaoTieRP 分部)

| 属性 | 说明 |
|------|------|
| **职责** | 预留 TaoTie 自研渲染管线的相机管理逻辑 |
| **渲染管线** | TaoTie Render Pipeline (自研) |
| **状态** | 代码已注释，未启用 |
| **继承关系** | CameraManager 的分部类 |

```csharp
// 当前状态：全部代码已注释
// using UnityEngine;
//
// namespace TaoTie
// {
//     public partial class CameraManager
//     {
//         #region URPCamera
//         // ... 注释的代码 ...
//         #endregion
//     }
// }
```

---

## 预留功能

根据注释代码，TaoTieRP 计划实现以下功能：

### 1. 相机栈管理

```csharp
// 预留字段
private GameObject sceneMainCameraGo;
private Camera sceneMainCamera;
```

### 2. Loading 流程

```csharp
// 预留方法
public void SetCameraStackAtLoadingStart()
public void SetCameraStackAtLoadingDone()
public void ResetSceneCamera()
```

### 3. Overlay 相机

```csharp
// 预留方法
void AddOverlayCamera(Camera baseCamera, Camera overlayCamera)
```

### 4. 主相机获取

```csharp
// 预留方法
public Camera MainCamera()
```

---

## 与 URP 实现的对比

根据注释代码，TaoTieRP 的实现与 URP 类似，但有以下差异：

| 功能 | URP 实现 | TaoTieRP 预留 |
|------|---------|--------------|
| UI 相机激活 | `gameObject.SetActive(true)` | 注释掉 |
| 渲染类型设置 | `renderType = Base/Overlay` | 注释掉 |
| 后处理 | `renderPostProcessing = true` | 注释掉 |
| 渲染器设置 | `SetRenderer(1)` | 注释掉 |
| 相机栈 | `cameraStack.Add()` | 注释掉 |

---

## 启用方法

要启用 TaoTieRP 支持，需要：

1. **取消注释代码**
2. **实现 TaoTieRP 特定的 API**
3. **替换 URP 相关的调用**

### 示例修改

```csharp
// 当前（注释状态）
// uiCamera.GetUniversalAdditionalCameraData().renderType = CameraRenderType.Base;

// 启用后（示例）
uiCamera.GetTaoTieAdditionalCameraData().renderType = CameraRenderType.Base;
```

---

## 使用场景

### 场景 1: 切换到 TaoTieRP

当项目需要从 URP 切换到 TaoTieRP 时：

```csharp
// 1. 取消注释 CameraManager.TaoTieRP.cs
// 2. 注释或移除 CameraManager.URP.cs
// 3. 更新平台检测逻辑
```

### 场景 2: 双管线支持

支持运行时切换渲染管线：

```csharp
public partial class CameraManager
{
    public void SetupCameraStack()
    {
        if (UseTaoTieRP)
        {
            SetupTaoTieRPCamera();
        }
        else
        {
            SetupURPCamera();
        }
    }
}
```

---

## 开发建议

### 1. 保持接口一致

TaoTieRP 实现应保持与 URP 相同的公共接口：

```csharp
// 两个分部类都应实现
public Camera MainCamera()
public void SetCameraStackAtLoadingStart()
public void SetCameraStackAtLoadingDone()
```

### 2. 条件编译

使用条件编译支持多管线：

```csharp
#if TAOTIE_RP
    // TaoTieRP 实现
#elif URP
    // URP 实现
#endif
```

### 3. 平台检测

添加渲染管线检测：

```csharp
public bool IsTaoTieRP()
{
    #if TAOTIE_RP
    return true;
    #else
    return false;
    #endif
}
```

---

## 相关文件

| 文件 | 状态 | 说明 |
|------|------|------|
| CameraManager.cs | ✅ 启用 | 主类，包含 Shake 等方法 |
| CameraManager.URP.cs | ✅ 启用 | URP 渲染管线实现 |
| CameraManager.TaoTieRP.cs | ⏸️ 预留 | TaoTieRP 渲染管线（待实现） |

---

## 注意事项

### 1. 不要同时启用

URP 和 TaoTieRP 实现不应同时启用，会导致冲突

### 2. 测试覆盖

启用 TaoTieRP 后需要充分测试：
- Loading 流程
- 相机切换
- UI 渲染
- 后处理效果

### 3. 文档更新

启用后需要更新此文档，记录实际实现

---

## 相关文档

- [CameraManager.cs.md](./CameraManager.cs.md) - 相机管理器主类
- [CameraManager.URP.cs.md](./CameraManager.URP.cs.md) - URP 实现

---

*文档生成时间：2026-03-01 | OpenClaw AI 助手*
