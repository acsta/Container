# BoxType.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | BoxType.cs |
| **路径** | Assets/Scripts/Code/Module/Const/BoxType.cs |
| **所属模块** | 框架层 → Code/Module/Const |
| **文件职责** | 箱子类型枚举定义 |

---

## 枚举说明

### BoxType

| 属性 | 说明 |
|------|------|
| **职责** | 定义拍卖中箱子的类型 |
| **类型** | enum（int） |

```csharp
public enum BoxType
{
    Normal = 0,
    Task = 1,
    Empty = 2,
    RandOpenEvt = 3,
    GodOfWealthEvt = 4,
}
```

---

## 箱子类型

| 值 | 枚举 | 说明 | 是否算钱 |
|------|------|------|---------|
| 0 | `Normal` | 正常物品 | ✅ 是 |
| 1 | `Task` | 任务物品 | ❌ 否 |
| 2 | `Empty` | 空箱子 | ❌ 否 |
| 3 | `RandOpenEvt` | 随机开箱事件 | ❌ 否 |
| 4 | `GodOfWealthEvt` | 财神爷事件 | ❌ 否 |

---

## 类型详解

### Normal（正常物品）

**说明**: 普通物品，参与最终金额计算

**用途**: 
- 大部分箱子都是 Normal 类型
- 只有 Normal 类型的物品参与算钱

**示例**:
```csharp
if (boxType == BoxType.Normal)
{
    // 计算物品价值
    totalValue += item.Price;
}
```

### Task（任务物品）

**说明**: 任务所需的特殊物品

**用途**:
- 完成特定任务
- 不参与金额计算

**示例**:
```csharp
if (boxType == BoxType.Task)
{
    // 完成任务进度
    taskManager.CompleteTaskItem(itemId);
}
```

### Empty（空箱子）

**说明**: 空箱子，没有任何物品

**用途**:
- 增加游戏随机性
- 不影响游戏进程

**示例**:
```csharp
if (boxType == BoxType.Empty)
{
    // 显示空箱子动画
    PlayEmptyAnimation();
}
```

### RandOpenEvt（随机开箱事件）

**说明**: 触发随机开箱事件

**用途**:
- 特殊事件触发
- 可能获得额外奖励

**示例**:
```csharp
if (boxType == BoxType.RandOpenEvt)
{
    // 触发随机事件
    TriggerRandomEvent();
}
```

### GodOfWealthEvt（财神爷事件）

**说明**: 触发财神爷事件

**用途**:
- 特殊奖励事件
- 通常给予大量金钱

**示例**:
```csharp
if (boxType == BoxType.GodOfWealthEvt)
{
    // 财神爷奖励
    playerData.Money += bonusAmount;
}
```

---

## 使用示例

### 示例 1: 检查箱子类型

```csharp
// 获取箱子类型
BoxType boxType = box.GetComponent<BoxComponent>().Type;

// 检查是否是任务物品
if (boxType == BoxType.Task)
{
    Log.Info("这是任务物品");
}

// 检查是否是正常物品
if (boxType == BoxType.Normal)
{
    Log.Info($"物品价值：{item.Price}");
}
```

### 示例 2: 计算总价值

```csharp
BigNumber totalValue = BigNumber.Zero;

foreach (var box in boxes)
{
    if (box.Type == BoxType.Normal)
    {
        // 只有 Normal 类型参与算钱
        totalValue += box.Item.Price;
    }
}

Log.Info($"集装箱总价值：{totalValue}");
```

### 示例 3: 任务进度检查

```csharp
int taskItemCount = 0;

foreach (var box in boxes)
{
    if (box.Type == BoxType.Task)
    {
        taskItemCount++;
    }
}

if (taskItemCount >= requiredCount)
{
    // 完成任务
    CompleteTask();
}
```

### 示例 4: 事件触发

```csharp
switch (boxType)
{
    case BoxType.Normal:
        // 正常物品
        ShowItemInfo(item);
        break;
        
    case BoxType.Task:
        // 任务物品
        UpdateTaskProgress();
        break;
        
    case BoxType.Empty:
        // 空箱子
        PlayEmptySound();
        break;
        
    case BoxType.RandOpenEvt:
        // 随机事件
        TriggerRandomEvent();
        break;
        
    case BoxType.GodOfWealthEvt:
        // 财神爷
        ShowGodOfWealthAnimation();
        GiveBonus();
        break;
}
```

---

## 相关文档

- [ItemType.cs.md](./ItemType.cs.md) - 物品类型
- [GameConst.cs.md](./GameConst.cs.md) - 游戏常量
- [AuctionHelper.cs.md](../../Game/System/Auction/AuctionHelper.cs.md) - 拍卖辅助

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
