# TurntableItem.cs - 转盘奖励项组件

## 📄 文件信息

| 属性 | 值 |
|------|------|
| **文件路径** | `Assets/Scripts/Code/Game/UIGame/UIAuction/TurntableItem.cs` |
| **命名空间** | `TaoTie` |
| **基类** | `UIBaseContainer` |
| **实现接口** | `IOnCreate` |

---

## 🎯 类说明

`TurntableItem` 是转盘小游戏中的奖励项显示组件，用于在转盘面板上显示单个奖励的图标和金额。

> **注意**: 此文件与 `UIMiniGame/TurntableItem.cs` 功能相同，位于不同目录可能是为了不同的使用场景或历史原因。

### 核心职责

- **奖励展示**: 显示奖励的图标和金币数量
- **数据绑定**: 从配置数据设置显示内容

---

## 📋 字段说明

### UI 组件字段

| 字段名 | 类型 | 说明 |
|--------|------|------|
| `Image` | `UIImage` | 奖励图标显示组件 |
| `Text` | `UITextmesh` | 奖励金额文本 |

---

## 🔧 方法说明

### 生命周期方法

#### `OnCreate()`
组件创建时初始化 UI 组件。

```csharp
public void OnCreate()
{
    Image = AddComponent<UIImage>("Icon");
    Text = AddComponent<UITextmesh>("Text");
}
```

---

### 业务方法

#### `SetData(TurntableRewardsConfig config)`
设置奖励数据并更新显示。

**参数说明:**
- `config`: 转盘奖励配置数据

**核心逻辑:**
```
1. 格式化金币数量：TranslateMoneyToStr(config.RewardCount)
2. 设置奖励图标：config.Icon 路径
```

**使用示例:**
```csharp
item.SetData(config);
// Text 显示："1,500"
// Image 显示：[金币图标]
```

---

## 🔗 相关文档

- [UIBaseContainer.cs.md](../UI/UIBaseContainer.cs.md) - UI 容器基类
- [TurntableRewardsConfig.cs.md](../../../Module/Generate/Config/TurntableRewardsConfig.cs.md) - 转盘奖励配置
- [UITurntableView.cs.md](../UIMiniGame/UITurntableView.cs.md) - 大厅转盘视图

---

*文档由 OpenClaw AI 助手自动生成 | 基于静态代码分析*
