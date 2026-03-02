# UICommonMiniGameView.cs - 小游戏通用视图基类

## 📄 文件信息

| 属性 | 值 |
|------|------|
| 文件路径 | `Assets/Scripts/Code/Game/UIGame/UIMiniGame/UICommonMiniGameView.cs` |
| 命名空间 | `TaoTie` |
| 基类 | `UIBaseView` |
| 实现接口 | `IOnCreate`, `IOnEnable<int>`, `IOnDisable` |

---

## 🎯 类说明

`UICommonMiniGameView` 是小游戏通用视图基类，为所有小游戏提供通用的功能，包括物品展示、价格显示、关闭逻辑等。

### 核心职责

- **物品展示**: 显示当前小游戏关联的物品信息
- **价格显示**: 显示物品的基础价格和当前价格
- **通用逻辑**: 提供小游戏共用的打开/关闭逻辑
- **派生基础**: 为具体小游戏提供扩展基础

---

## 📋 字段说明

### UI 组件字段

| 字段名 | 类型 | 说明 |
|--------|------|------|
| `ItemIcon` | `UIImage` | 物品图标 |
| `ItemName` | `UITextmesh` | 物品名称 |
| `ItemPrice` | `UITextmesh` | 物品价格 |
| `CloseBtn` | `UIButton` | 关闭按钮 |

### 数据字段

| 字段名 | 类型 | 说明 |
|--------|------|------|
| `itemId` | `int` | 物品配置 ID |
| `ItemConfig` | `ItemConfig` | 物品配置数据（只读属性） |

---

## 🔧 方法说明

### 生命周期方法

#### `OnCreate()`
视图创建时初始化通用 UI 组件。

#### `OnEnable(int id)`
视图启用时设置物品数据。

**参数说明:**
- `id`: 物品配置 ID

**主要功能:**
1. 保存物品 ID
2. 获取物品配置
3. 显示物品图标、名称、价格

#### `OnDisable()`
视图禁用时的清理。

---

## 💡 使用示例

### 派生类示例

```csharp
public class UIAppraisalView : UICommonMiniGameView, IOnDisable
{
    public override void OnCreate()
    {
        base.OnCreate();  // 调用基类初始化
        // 添加鉴定特有的 UI 组件
    }
    
    public override void OnEnable(int id)
    {
        base.OnEnable(id);  // 调用基类设置物品数据
        // 添加鉴定特有的逻辑
    }
}
```

---

## 🔗 相关文档

- [UIAppraisalView.cs.md](./UIAppraisalView.cs.md) - 鉴定小游戏
- [UIRepairView.cs.md](./UIRepairView.cs.md) - 修复小游戏
- [UIQuarantineView.cs.md](./UIQuarantineView.cs.md) - 检疫小游戏

---

*最后更新：2026-03-02*
