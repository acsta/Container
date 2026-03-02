# UIReportItem.cs - 结算报告列表项

## 📄 文件信息

| 属性 | 值 |
|------|------|
| 文件路径 | `Assets/Scripts/Code/Game/UIGame/UIAuction/UIReportItem.cs` |
| 命名空间 | `TaoTie` |
| 基类 | `UIBaseContainer` |
| 实现接口 | `IOnCreate`, `IOnEnable` |

---

## 🎯 类说明

`UIReportItem` 是结算报告列表项组件，用于在 `UIReportWin` 中展示单个阶段的竞拍结果，包括阶段编号、宝盒信息、收益/损失等。

### 核心职责

- **阶段展示**: 显示阶段编号和宝盒信息
- **结果呈现**: 根据报告类型显示不同的结果文本和颜色
- **数据绑定**: 从 `AuctionReport` 数据结构绑定显示内容

---

## 📋 字段说明

### UI 组件字段

| 字段名 | 类型 | 说明 |
|--------|------|------|
| `Title` | `UITextmesh` | 阶段标题（如"第 1 阶段"） |
| `Icon` | `UIImage` | 宝盒/容器图标 |
| `Name` | `UITextmesh` | 宝盒/容器名称 |
| `Result` | `UIImage` | 结果背景（颜色表示胜负） |
| `Text` | `UITextmesh` | 结果文本（收益/损失金额） |

---

## 🔧 方法说明

### 生命周期方法

#### `OnCreate()`
组件创建时初始化所有 UI 子组件。

**主要功能:**
1. 初始化所有 UI 组件引用
2. 设置阶段标题的国际化键

#### `OnEnable()`
组件启用时的初始化（当前为空实现）。

---

### 业务方法

#### `SetData(AuctionReport data)`
设置报告数据并刷新显示。

**参数说明:**
- `data`: 竞拍报告数据

**主要功能:**
1. **阶段编号**: 设置标题为"第 X 阶段"
2. **容器信息**:
   - 从 `ContainerConfigCategory` 获取容器配置
   - 设置图标和名称
   - 如果容器不存在：显示默认图标和"空"文本
3. **结果展示**（根据报告类型）:

**报告类型处理:**

| 类型 | 条件 | 显示内容 | 颜色 |
|------|------|----------|------|
| `Self` | 收益 >= 0 | "赢得 X" | 绿色 |
| `Self` | 收益 < 0 | "损失 X" | 红色 |
| `Others` | 竞拍成功 > 0 且收益 > 0 | "赢得 X" | 绿色 |
| `Others` | 竞拍成功 > 0 且收益 <= 0 | "放弃" | 灰色 |
| `Others` | 竞拍成功 = 0 | "放弃" | 灰色 |
| `Pass` | - | "竞拍失败" | 灰色 |
| `NoResult` | - | "放弃" | 灰色 |

---

## 🔄 流程图

```mermaid
flowchart TD
    A[SetData 调用] --> B[设置阶段编号]
    B --> C{容器配置存在？}
    C -->|是 | D[设置容器图标和名称]
    C -->|否 | E[设置默认图标和"空"]
    
    D --> F{报告类型？}
    E --> F
    
    F -->|Self | G{收益 >= 0?}
    G -->|是 | H[显示"赢得 X" 绿色]
    G -->|否 | I[显示"损失 X" 红色]
    
    F -->|Others | J{竞拍成功 > 0?}
    J -->|是 | K{收益 > 0?}
    K -->|是 | H
    K -->|否 | L[显示"放弃" 灰色]
    J -->|否 | L
    
    F -->|Pass | M[显示"竞拍失败" 灰色]
    F -->|NoResult | L
```

---

## 💡 使用示例

### 在滚动列表中使用

```csharp
// 在 UIReportWin 的 GetScrollViewItemByIndex 中
public LoopListViewItem2 GetScrollViewItemByIndex(LoopListView2 listView, int index)
{
    var item = listView.NewListViewItem("UIReportItem", index);
    
    UIReportItem reportItem;
    if (!item.IsInitHandlerCalled)
    {
        reportItem = ScrollView.AddItemViewComponent<UIReportItem>(item);
        item.IsInitHandlerCalled = true;
    }
    else
    {
        reportItem = ScrollView.GetUIItemView<UIReportItem>(item);
    }
    
    // 设置数据
    reportItem.SetData(list[index]);
    return item;
}
```

### 报告数据结构

```csharp
// AuctionReport 结构
{
    Index: 0,                      // 阶段索引
    ContainerId: 101,              // 容器 ID
    Type: ReportType.Self,         // 报告类型
    FinalUserWin: 1500,            // 最终收益
    RaiseSuccessCount: 1           // 竞拍成功次数
}
```

---

## 🔗 相关文档

- [UIReportWin.cs.md](./UIReportWin.cs.md) - 结算报告窗口
- [AuctionReport.cs.md](../../../Data/AuctionReport.cs.md) - 竞拍报告数据
- [ContainerConfig.cs.md](../../../../Module/Generate/Config/ContainerConfig.cs.md) - 容器配置表

---

*最后更新：2026-03-02*
