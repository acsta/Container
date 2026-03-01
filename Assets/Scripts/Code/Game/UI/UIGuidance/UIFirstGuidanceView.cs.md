# UIFirstGuidanceView.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | UIFirstGuidanceView.cs |
| **路径** | Assets/Scripts/Code/Game/UI/UIGuidance/UIFirstGuidanceView.cs |
| **所属模块** | 游戏层 → Code/Game/UI/UIGuidance |
| **文件职责** | 首次引导 UI 视图，显示新手引导的初始遮罩和动画 |

---

## 类/结构体说明

### UIFirstGuidanceView

| 属性 | 说明 |
|------|------|
| **职责** | 显示新手引导的初始界面，包含遮罩和关闭动画，点击遮罩可关闭 |
| **泛型参数** | 无 |
| **继承关系** | 继承 `UIBaseView` 类 |
| **实现的接口** | `IOnCreate`, `IOnEnable`, `IOnWidthPaddingChange` |

**设计模式**: 组件模式

```csharp
// 使用方式
// 通过 UIManager 打开
var firstGuidance = await UIManager.Instance.OpenWindow<UIFirstGuidanceView>(UIFirstGuidanceView.PrefabPath, UILayerNames.GuideLayer);
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `PrefabPath` | `string` | `public static` | 预制体路径："UI/UIGuidance/Prefabs/UIFirstGuidanceView.prefab" |
| `Mask` | `UIPointerClick` | `public` | 遮罩点击组件 |
| `Animator` | `UIAnimator` | `public` | 动画组件 |

---

## 方法说明（按重要程度排序）

### OnCreate()

**签名**:
```csharp
public void OnCreate()
```

**职责**: 创建 UI 组件

**核心逻辑**:
```
1. 添加 UIAnimator 组件
2. 添加 UIPointerClick 组件（路径："Mask"）
```

**调用者**: `UIManager`（窗口创建时）

---

### OnEnable()

**签名**:
```csharp
public void OnEnable()
```

**职责**: UI 启用时的初始化

**核心逻辑**:
```
1. 设置 Mask 的点击回调为 OnClickMask
```

**调用者**: `UIManager`（窗口启用时）

**被调用者**: `Mask.SetOnClick()`

---

### CloseSelf()

**签名**:
```csharp
public override async ETTask CloseSelf()
```

**职责**: 关闭自身，播放关闭动画并通知引导管理器

**核心逻辑**:
```
1. 通知 GuidanceManager 关闭事件（"Close_UIFirstGuidanceView"）
2. 播放 "First_Guidance_Close" 动画
3. 调用 base.CloseSelf()
```

**调用者**: `OnClickMask()`, UIManager

**被调用者**: `GuidanceManager.Instance.NoticeEvent()`, `Animator.Play()`

---

### OnClickMask()

**签名**:
```csharp
public void OnClickMask()
```

**职责**: 遮罩点击事件处理

**核心逻辑**:
```
1. 关闭自身
```

**调用者**: `Mask`（点击时）

**被调用者**: `CloseSelf()`

---

## 使用示例

### 打开首次引导

```csharp
// 打开首次引导界面
var firstGuidance = await UIManager.Instance.OpenWindow<UIFirstGuidanceView>(
    UIFirstGuidanceView.PrefabPath, 
    UILayerNames.GuideLayer
);

// 点击遮罩会自动关闭
```

---

## 相关文档链接

- [UIBaseView.cs.md](../../../Module/UI/UIBaseView.cs.md) - UI 基类视图
- [UIAnimator.cs.md](../../../Module/UIComponent/UIAnimator.cs.md) - UI 动画组件
- [UIPointerClick.cs.md](../../../Module/UIComponent/UIPointerClick.cs.md) - UI 点击组件
- [GuidanceManager.cs.md](../../../Module/Guidance/GuidanceManager.cs.md) - 引导管理器

---

*文档生成时间：2026-03-02*
