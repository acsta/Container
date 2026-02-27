# UILayerNames.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | UILayerNames.cs |
| **路径** | Assets/Scripts/Code/Module/UI/UILayerNames.cs |
| **所属模块** | 框架层 → Code/Module/UI |
| **文件职责** | 定义 UI 层级枚举，管理 UI 窗口的显示顺序 |

---

## 类/结构体说明

### UILayerNames (枚举)

| 属性 | 说明 |
|------|------|
| **职责** | 定义 UI 层级的枚举类型 |
| **泛型参数** | 无 |
| **继承关系** | 继承自 `byte` |
| **实现的接口** | 无 |

```csharp
public enum UILayerNames : byte
{
    GameBackgroundLayer,   // 游戏背景层
    BackgroundLayer,       // 主界面背景层
    GameLayer,             // 游戏内 UI 层
    SceneLayer,            // 场景 UI 层
    NormalLayer,           // 普通 UI 层
    TipLayer,              // 提示 UI 层
    TopLayer,              // 顶层 UI
}
```

**存储优化**: 使用 `byte` 而非默认 `int`，节省内存（0-255 范围足够）

---

## 层级详细说明

### 层级顺序（从底到顶）

| 枚举值 | 内部值 | 说明 | 典型用途 |
|--------|--------|------|----------|
| `GameBackgroundLayer` | 0 | 游戏背景层 | 游戏场景背景 UI |
| `BackgroundLayer` | 1 | 主界面背景层 | 大厅、主界面背景 |
| `GameLayer` | 2 | 游戏内 UI 层 | 游戏内功能 UI |
| `SceneLayer` | 3 | 场景 UI 层 | 场景交互 UI（点击建筑信息等） |
| `NormalLayer` | 4 | 普通 UI 层 | 一级、二级、三级窗口 |
| `TipLayer` | 5 | 提示 UI 层 | 错误弹窗、网络提示 |
| `TopLayer` | 6 | 顶层 UI | 场景加载、全屏遮罩 |

---

## 层级配置（来自 UIManager.Layers.cs）

### 完整配置表

| 层级 | PlaneDistance | OrderInLayer | 说明 |
|------|---------------|--------------|------|
| `GameBackgroundLayer` | 1000 | 0 | 最底层，游戏场景背景 |
| `BackgroundLayer` | 900 | 1000 | 主界面背景 |
| `GameLayer` | 800 | 1800 | 游戏内功能 UI |
| `SceneLayer` | 700 | 2000 | 场景交互 UI |
| `NormalLayer` | 600 | 3000 | 普通窗口 |
| `TipLayer` | 500 | 4000 | 提示弹窗 |
| `TopLayer` | 400 | 5000 | 最顶层，加载界面 |

**PlaneDistance**: Canvas 平面距离摄像机的距离（值越大越靠后）

**OrderInLayer**: Unity Sorting Order（值越大越靠前）

---

## 层级使用示例

### 示例 1: 打开加载界面（顶层）

```csharp
// 加载界面放在 TipLayer
await UIManager.Instance.OpenWindow<UILoadingView>(
    UILoadingView.PrefabPath,
    UILayerNames.TipLayer
);
```

### 示例 2: 打开主界面（背景层）

```csharp
// 主界面放在 BackgroundLayer
await UIManager.Instance.OpenWindow<UILobbyView>(
    UILobbyView.PrefabPath,
    UILayerNames.BackgroundLayer
);
```

### 示例 3: 打开普通窗口

```csharp
// 默认使用 NormalLayer
await UIManager.Instance.OpenWindow<UIAuctionView>(
    UIAuctionView.PrefabPath,
    UILayerNames.NormalLayer
);
```

### 示例 4: 打开提示框

```csharp
// 提示框放在 TipLayer
await UIManager.Instance.OpenBox<UIToastView>(
    UIToast.PrefabPath,
    UILayerNames.TipLayer,
    during: 3000
);
```

---

## 层级特殊行为

### BackgroundLayer 自动清理

当打开 BackgroundLayer 或 GameBackgroundLayer 的窗口时，会自动关闭下层窗口：

```csharp
// UIManager.AddWindowToStack 中的逻辑
if (!target.IsBox && isFirst && 
    (layerName == UILayerNames.BackgroundLayer || 
     layerName == UILayerNames.GameBackgroundLayer))
{
    // 如果是背景层，则销毁所有的 NormalLayer/GameLayer/BackgroundLayer
    await CloseWindowByLayer(UILayerNames.NormalLayer);
    await CloseWindowByLayer(UILayerNames.GameLayer);
    await CloseWindowByLayer(UILayerNames.BackgroundLayer, uiName);
    await CloseWindowByLayer(UILayerNames.GameBackgroundLayer, uiName);
}
```

**设计意图**: 打开新主界面时，自动清理旧界面的子窗口

---

## 层级栈管理

UIManager 为每个层级维护一个窗口栈：

```csharp
// 窗口栈：按打开顺序记录窗口
private Dictionary<UILayerNames, LinkedList<string>> windowStack;

// 添加窗口到栈顶
windowStack[layerName].AddFirst(uiName);

// 获取最顶层窗口
UIWindow GetTopWindow(UILayerNames layer)
{
    var wins = windowStack[layer];
    for (var node = wins.First; node != null; node = node.Next)
    {
        var name = node.Value;
        var win = GetWindow(name, 1);
        if (win != null)
            return win;
    }
    return null;
}
```

---

## 阅读指引

### 建议的阅读顺序

1. **理解层级概念** - 为什么需要 UI 层级
2. **记住层级顺序** - 从底到顶 7 个层级
3. **了解配置参数** - PlaneDistance 和 OrderInLayer
4. **掌握特殊行为** - BackgroundLayer 的自动清理

### 最值得学习的技术点

1. **byte 枚举**: 节省内存（7 个值用 byte 足够）
2. **层级分离**: 不同用途的 UI 放在不同层级
3. **自动管理**: BackgroundLayer 自动清理下层窗口
4. **栈结构**: LinkedList 维护窗口打开顺序

---

## 层级可视化

```
┌─────────────────────────────────────┐
│         TopLayer (5000)             │ ← 场景加载、全屏遮罩
├─────────────────────────────────────┤
│       TipLayer (4000)               │ ← 提示框、错误弹窗
├─────────────────────────────────────┤
│      NormalLayer (3000)             │ ← 普通窗口、多级 UI
├─────────────────────────────────────┤
│      SceneLayer (2000)              │ ← 场景交互 UI
├─────────────────────────────────────┤
│       GameLayer (1800)              │ ← 游戏内功能 UI
├─────────────────────────────────────┤
│    BackgroundLayer (1000)           │ ← 主界面背景
├─────────────────────────────────────┤
│ GameBackgroundLayer (0)             │ ← 游戏场景背景
└─────────────────────────────────────┘
```

---

## 相关文档

- [UIManager.cs.md](./UIManager.cs.md) - UI 管理器
- [UIManager.Layers.cs.md](./UIManager.Layers.cs.md) - 层级系统实现
- [UILayer.cs.md](./UILayer.cs.md) - 层级类

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
