# TurntableItem.cs - 转盘奖励项组件

## 📄 文件信息

| 属性 | 值 |
|------|------|
| **文件路径** | `Assets/Scripts/Code/Game/UIGame/UIMiniGame/TurntableItem.cs` |
| **命名空间** | `TaoTie` |
| **基类** | `UIBaseContainer` |
| **实现接口** | `IOnCreate` |

---

## 🎯 类说明

`TurntableItem` 是转盘小游戏中的奖励项显示组件，用于在转盘面板上显示单个奖励的图标和金额。

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
   - 自动添加千分位分隔符
   - 大数字可能使用 K/M 等单位
2. 设置奖励图标：config.Icon 路径
```

**使用示例:**
```csharp
// 设置奖励项数据
item.SetData(config);

// config 示例：
// {
//     "RewardCount": 1500,
//     "Icon": "UIGame/UIMiniGame/Atlas/reward_gold"
// }

// 显示结果：
// Text: "1,500"
// Image: [金币图标]
```

---

## 📊 组件结构图

```mermaid
graph TD
    subgraph TurntableItem["TurntableItem"]
        Image[UIImage "Icon"]
        Text[UITextmesh "Text"]
    end
    
    subgraph Parent["转盘面板 (Panel)"]
        Item1[TurntableItem 0°]
        Item2[TurntableItem 45°]
        Item3[TurntableItem 90°]
        Item4[TurntableItem 135°]
        Item5[TurntableItem 180°]
        Item6[TurntableItem 225°]
        Item7[TurntableItem 270°]
        Item8[TurntableItem 315°]
    end
    
    Item1 --> Parent
    Item2 --> Parent
    Item3 --> Parent
    Item4 --> Parent
    Item5 --> Parent
    Item6 --> Parent
    Item7 --> Parent
    Item8 --> Parent
    
    note right of Parent "8 个奖励项<br/>每项旋转 45 度"
```

---

## 💡 使用场景

### 大厅转盘（UITurntableView）

```csharp
// 在 UITurntableView.GetPanelItemByIndex 中调用
public void GetPanelItemByIndex(int index, GameObject obj)
{
    TurntableItem item = Panel.GetUIItemView<TurntableItem>(obj);
    if (item == null)
    {
        item = Panel.AddItemViewComponent<TurntableItem>(obj);
    }
    item.SetData(data[index]);
    item.GetTransform().localEulerAngles = new Vector3(0, 0, -45 * index);
}
```

### 奖励配置示例

```csharp
// TurntableRewardsConfig
{
    "Lv": 1,
    "RestaurantLv": 1,
    "Weight": 50,           // 权重 50（影响概率）
    "RewardCount": 500,     // 奖励 500 金币
    "Icon": "UIGame/UIMiniGame/Atlas/gold_500"
}

// 显示效果：
// [金币图标] 
//    500
```

---

## 🔗 相关文档

- [UIBaseContainer.cs.md](../../UI/UIBaseContainer.cs.md) - UI 容器基类
- [TurntableRewardsConfig.cs.md](../../../Module/Generate/Config/TurntableRewardsConfig.cs.md) - 转盘奖励配置
- [UITurntableView.cs.md](./UITurntableView.cs.md) - 大厅转盘视图
- [UIImage.cs.md](../../UIComponent/UIImage.cs.md) - UI 图片组件
- [UITextmesh.cs.md](../../UIComponent/UITextmesh.cs.md) - UI 文本组件

---

*文档由 OpenClaw AI 助手自动生成 | 基于静态代码分析*
