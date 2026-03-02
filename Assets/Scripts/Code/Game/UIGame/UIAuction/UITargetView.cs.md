# UITargetView.cs - 目标瞄准视图

## 📄 文件信息

| 属性 | 值 |
|------|------|
| 文件路径 | `Assets/Scripts/Code/Game/UIGame/UIAuction/UITargetView.cs` |
| 命名空间 | `TaoTie` |
| 基类 | `UIBaseView` |
| 实现接口 | `IOnCreate`, `IOnEnable` |

---

## 🎯 类说明

`UITargetView` 是目标瞄准视图，用于在 UI 上显示一个瞄准框，指向 3D 场景中的特定目标物体。常用于引导玩家注意某个建筑、物品或角色。

### 核心职责

- **目标追踪**: 将 3D 世界坐标转换为 UI 坐标
- **瞄准框显示**: 显示动态大小的瞄准框
- **动画效果**: 播放瞄准框出现动画
- **自动关闭**: 动画播放完成后自动关闭视图

---

## 📋 字段说明

### UI 组件字段

| 字段名 | 类型 | 说明 |
|--------|------|------|
| `Aim` | `UIAnimator` | 瞄准框动画控制器 |

---

## 🔧 方法说明

### 生命周期方法

#### `OnCreate()`
视图创建时初始化瞄准框动画组件。

#### `OnEnable()`
视图启用时隐藏瞄准框（默认状态）。

---

### 业务方法

#### `EnterTarget(GameObject target)`
进入目标瞄准模式，显示瞄准框指向指定目标。

**参数说明:**
- `target`: 要瞄准的目标游戏对象

**主要功能:**
1. **坐标转换**:
   - 如果目标是 3D 物体：将世界坐标转换为 UI 坐标
   - 如果目标是 UI 物体：直接使用 UI 坐标
   
2. **大小适配**:
   - 3D 物体：固定大小 200x200
   - UI 物体：根据目标大小的 2 倍设置
   
3. **动画播放**:
   - 显示瞄准框
   - 播放 "MaskOpen" 动画
   - 动画完成后自动关闭视图

**坐标转换逻辑:**
```csharp
// 3D 物体转 UI 坐标
Vector2 pt = UIManager.Instance.ScreenPointToUILocalPoint(
    GetRectTransform(),
    mainCamera.WorldToScreenPoint(target.transform.position)
);

// UI 物体直接使用
Aim.GetRectTransform().position = target.transform.position;
```

---

## 🔄 流程图

```mermaid
flowchart TD
    A[EnterTarget 调用] --> B{目标是否为空？}
    B -->|是 | C[设置坐标为 (0,0)]
    B -->|否 | D{目标是 3D 物体？}
    
    D -->|是 | E[获取主相机]
    E --> F[WorldToScreenPoint 转换]
    F --> G[ScreenPointToUILocalPoint 转换]
    G --> H[设置固定大小 200x200]
    
    D -->|否 | I[获取 RectTransform]
    I --> J[使用目标位置]
    J --> K[设置大小为目标 2 倍]
    
    C --> L[显示瞄准框]
    H --> L
    K --> L
    
    L --> M[播放 MaskOpen 动画]
    M --> N[动画完成]
    N --> O[关闭视图]
```

---

## 💡 使用示例

### 瞄准 3D 场景中的建筑

```csharp
// 获取场景中的建筑物体
GameObject building = SceneManager.Instance.GetCurrentScene<MapScene>()
    .collector.GetGameObject("Collection");

// 打开瞄准视图并指向建筑
var targetView = await UIManager.Instance.OpenWindow<UITargetView>(UITargetView.PrefabPath);
await targetView.EnterTarget(building);

// 视图会在动画播放完成后自动关闭
```

### 瞄准 UI 元素

```csharp
// 获取 UI 按钮
UIButton targetBtn = UIManager.Instance.GetView<UIGameView>(1).DiceBtn;

// 打开瞄准视图并指向按钮
var targetView = await UIManager.Instance.OpenWindow<UITargetView>(UITargetView.PrefabPath);
await targetView.EnterTarget(targetBtn.GetGameObject());
```

---

## 🔗 相关文档

- [UIManager.cs.md](../../../UI/UIManager.cs.md) - UI 管理器
- [CameraManager.cs.md](../../../Manager/CameraManager.cs.md) - 相机管理器
- [UIBaseView.cs.md](../../../UI/UIBaseView.cs.md) - UI 视图基类

---

*最后更新：2026-03-02*
