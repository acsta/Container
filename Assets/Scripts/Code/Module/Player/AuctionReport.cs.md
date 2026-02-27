# AuctionReport.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | AuctionReport.cs |
| **路径** | Assets/Scripts/Code/Module/Player/AuctionReport.cs |
| **所属模块** | 框架层 → Code/Module/Player |
| **文件职责** | 拍卖报告数据结构，记录单场拍卖结果 |

---

## 类/结构体说明

### ReportType（枚举）

| 值 | 说明 |
|------|------|
| `NoResult` | 无结果 |
| `Self` | 我拍下 |
| `Others` | 他人拍下 |
| `Pass` | 流拍 |

---

### AuctionReport

| 属性 | 说明 |
|------|------|
| **职责** | 记录单场拍卖的完整数据（物品、价格、结果等） |
| **泛型参数** | 无 |
| **继承关系** | 无 |
| **实现的接口** | 无 |

```csharp
public class AuctionReport
{
    // 拍卖报告数据结构
}
```

---

## 字段详解（按功能分类）

### 基础信息

| 字段 | 类型 | 说明 |
|------|------|------|
| `Index` | `int` | 拍卖序号 |
| `ContainerId` | `int` | 集装箱类型 |

### 物品信息

| 字段 | 类型 | 说明 |
|------|------|------|
| `Items` | `ItemConfig[]` | 物品列表 |
| `BoxTypes` | `BoxType[]` | 物品类型（箱子类型） |
| `PlayData` | `object[]` | 小玩法数据 |
| `TaskItemIndex` | `int` | 任务物品序号（-1 无） |

### 拍卖结果

| 字段 | 类型 | 说明 |
|------|------|------|
| `Type` | `ReportType` | 拍卖结果（我拍下/他人拍下/流拍） |
| `LastAuctionPriceStr` | `string` | 拍得价格（字符串） |
| `LastAuctionPrice` | `BigNumber` | 拍得价格（计算属性） |
| `RaiseSuccessCount` | `int` | 抬价成功次数 |
| `RaisePriceStr` | `string` | 抬价金额（字符串） |
| `RaisePrice` | `BigNumber` | 抬价金额（计算属性） |
| `SaleType` | `int` | 出售类型（0=普通出售） |
| `GameInfoId` | `int` | 情报 ID |

### 最终收益

| 字段 | 类型 | 说明 |
|------|------|------|
| `AllPriceStr` | `string` | 最终售价（字符串） |
| `AllPrice` | `BigNumber` | 最终售价（计算属性） |
| `FinalUserWin` | `BigNumber` | 玩家最终盈利（计算属性） |

---

## 计算属性

### FinalUserWin

**签名**:
```csharp
public BigNumber FinalUserWin { get; }
```

**职责**: 计算玩家最终盈利

**核心逻辑**:
```
1. 如果是玩家拍下（Type == Self）：
   - 如果是普通出售（SaleType == 0）
   - 返回 AllPrice - LastAuctionPrice
   - 否则记录错误

2. 如果有抬价成功（RaiseSuccessCount > 0）：
   - 计算抬价奖励 = RaisePrice * 抬价倍数
   - 如果有情报，应用情报加成
   - 返回抬价奖励

3. 否则返回 0
```

**抬价奖励计算**:
```csharp
if (RaiseSuccessCount > 0)
{
    var playerRaiseRewardsMoney = RaisePrice * AuctionHelper.GetRaiseMul(RaiseSuccessCount);
    
    if (GameInfoId > 0)
    {
        var gameInfo = GameInfoConfigCategory.Instance.Get(GameInfoId);
        playerRaiseRewardsMoney = gameInfo.GetRaiseRewards(playerRaiseRewardsMoney);
    }
    
    return playerRaiseRewardsMoney;
}
```

**调用者**: 结算界面显示收益

---

## 阅读指引

### 建议的阅读顺序

1. **理解数据结构** - AuctionReport 记录什么
2. **看基础信息** - 拍卖基本信息
3. **看拍卖结果** - 理解 Type 枚举
4. **看 FinalUserWin** - 理解盈利计算

### 最值得学习的技术点

1. **BigNumber 存储**: 使用字符串存储大数值
2. **枚举状态**: ReportType 表示拍卖结果
3. **计算属性**: FinalUserWin 动态计算盈利
4. **灵活数据**: PlayData 支持多种小玩法数据

---

## 使用示例

### 示例 1: 创建拍卖报告

```csharp
AuctionReport report = new AuctionReport();
report.Index = 1;
report.ContainerId = 101;
report.Items = new[] { itemConfig1, itemConfig2 };
report.Type = ReportType.Self;
report.LastAuctionPriceStr = "1000";
report.AllPriceStr = "1500";
```

### 示例 2: 访问拍卖结果

```csharp
// 检查是否拍下
if (report.Type == ReportType.Self)
{
    Log.Info("玩家拍下集装箱");
}
else if (report.Type == ReportType.Others)
{
    Log.Info("他人拍下集装箱");
}
else if (report.Type == ReportType.Pass)
{
    Log.Info("流拍");
}
```

### 示例 3: 计算盈利

```csharp
// 获取最终盈利
BigNumber win = report.FinalUserWin;
Log.Info($"本场盈利：{win}");

// 如果是抬价盈利
if (report.RaiseSuccessCount > 0)
{
    Log.Info($"抬价{report.RaiseSuccessCount}次，获得奖励{report.RaisePrice}");
}
```

### 示例 4: 访问物品信息

```csharp
// 遍历物品
foreach (var item in report.Items)
{
    Log.Info($"物品：{item.Name}");
}

// 检查任务物品
if (report.TaskItemIndex >= 0)
{
    Log.Info($"任务物品索引：{report.TaskItemIndex}");
}
```

---

## 拍卖结果类型

| Type | 说明 | 盈利计算 |
|------|------|---------|
| `NoResult` | 无结果 | 0 |
| `Self` | 我拍下 | AllPrice - LastAuctionPrice |
| `Others` | 他人拍下 | RaisePrice（如果有抬价） |
| `Pass` | 流拍 | 0 |

---

## 相关文档

- [AuctionHelper.cs.md](../../Game/System/Auction/AuctionHelper.cs.md) - 拍卖辅助工具
- [PlayerData.cs.md](./PlayerData.cs.md) - 玩家数据

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
