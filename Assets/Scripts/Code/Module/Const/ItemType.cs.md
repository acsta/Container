# ItemType.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | ItemType.cs |
| **路径** | Assets/Scripts/Code/Module/Const/ItemType.cs |
| **所属模块** | 框架层 → Code/Module/Const |
| **文件职责** | 物品类型枚举定义 |

---

## 枚举说明

### ItemType

| 属性 | 说明 |
|------|------|
| **职责** | 定义物品的类型，用于区分不同玩法 |
| **类型** | enum（int） |

```csharp
public enum ItemType
{
    Container = -2,
    Const = -1,
    None = 0,
    Story = 1,
    Appraisal = 10,
    AppraisalResult = 11,
    Repair = 20,
    GoodsCheck = 30,
    Quarantine = 40,
    BombDisposal = 50,
    MAX = 10000,
    GodOfWealth = 10010,
}
```

---

## 物品类型

### 特殊类型（负数）

| 值 | 枚举 | 说明 |
|------|------|------|
| -2 | `Container` | 集装箱（容器本身） |
| -1 | `Const` | 常量物品 |
| 0 | `None` | 无类型 |

### 剧情类（1-9）

| 值 | 枚举 | 说明 |
|------|------|------|
| 1 | `Story` | 剧情物品 |

### 鉴定类（10-19）

| 值 | 枚举 | 说明 |
|------|------|------|
| 10 | `Appraisal` | 鉴定物品（转盘玩法） |
| 11 | `AppraisalResult` | 鉴定结果 |

### 修复类（20-29）

| 值 | 枚举 | 说明 |
|------|------|------|
| 20 | `Repair` | 修复物品（拼图验证码玩法） |

### 验货类（30-39）

| 值 | 枚举 | 说明 |
|------|------|------|
| 30 | `GoodsCheck` | 验货物品（答题玩法） |

### 检疫类（40-49）

| 值 | 枚举 | 说明 |
|------|------|------|
| 40 | `Quarantine` | 检疫物品（刮刮乐玩法） |

### 拆弹类（50-99）

| 值 | 枚举 | 说明 |
|------|------|------|
| 50 | `BombDisposal` | 拆弹物品（线迷宫玩法） |

### 全局类（≥10000）

| 值 | 枚举 | 说明 |
|------|------|------|
| 10000 | `MAX` | 最大值标记 |
| 10010 | `GodOfWealth` | 财神爷（全局玩法） |

---

## 类型分组

### 按玩法分类

| 分组 | 类型范围 | 说明 |
|------|---------|------|
| 剧情 | 1 | 剧情相关物品 |
| 鉴定 | 10-11 | 转盘鉴定玩法 |
| 修复 | 20 | 拼图验证码玩法 |
| 验货 | 30 | 答题玩法 |
| 检疫 | 40 | 刮刮乐玩法 |
| 拆弹 | 50 | 线迷宫玩法 |
| 全局 | ≥10000 | 财神爷等全局事件 |

### 按数值范围判断

```csharp
// 判断是否是全局玩法
if (itemType >= ItemType.MAX)
{
    // 全局玩法（如财神爷）
}
else
{
    // 普通玩法
}
```

---

## 使用示例

### 示例 1: 检查物品类型

```csharp
// 获取物品类型
ItemType itemType = (ItemType)itemConfig.Type;

// 检查是否是鉴定物品
if (itemType == ItemType.Appraisal)
{
    Log.Info("这是鉴定物品，需要玩转盘");
}

// 检查是否是任务物品
if (itemType == ItemType.Story)
{
    Log.Info("这是剧情物品");
}
```

### 示例 2: 根据类型启动小游戏

```csharp
async ETTask StartMiniGame(ItemConfig item)
{
    ItemType itemType = (ItemType)item.Type;
    
    switch (itemType)
    {
        case ItemType.Appraisal:
            // 鉴定玩法（转盘）
            await UIManager.Instance.OpenBox<UIAppraisalView>(...);
            break;
            
        case ItemType.Repair:
            // 修复玩法（拼图）
            await UIManager.Instance.OpenBox<UIRepairView>(...);
            break;
            
        case ItemType.GoodsCheck:
            // 验货玩法（答题）
            await UIManager.Instance.OpenBox<UIGoodsCheckView>(...);
            break;
            
        case ItemType.Quarantine:
            // 检疫玩法（刮刮乐）
            await UIManager.Instance.OpenBox<UIQuarantineView>(...);
            break;
            
        case ItemType.BombDisposal:
            // 拆弹玩法（线迷宫）
            await UIManager.Instance.OpenBox<UIBombDisposalView>(...);
            break;
            
        case ItemType.GodOfWealth:
            // 财神爷
            ShowGodOfWealth();
            break;
    }
}
```

### 示例 3: 判断是否是小玩法物品

```csharp
bool IsMiniGameItem(ItemType itemType)
{
    // 排除特殊类型和全局类型
    return itemType > 0 && itemType < ItemType.MAX;
}

// 使用
if (IsMiniGameItem(itemType))
{
    // 需要玩小游戏
    await StartMiniGame(item);
}
```

### 示例 4: 获取类型范围

```csharp
// 检查是否是鉴定类物品
bool IsAppraisalType(ItemType type)
{
    return type >= ItemType.Appraisal && type < ItemType.Repair;
}

// 检查是否是修复类物品
bool IsRepairType(ItemType type)
{
    return type >= ItemType.Repair && type < ItemType.GoodsCheck;
}
```

---

## 相关文档

- [BoxType.cs.md](./BoxType.cs.md) - 箱子类型
- [GameConst.cs.md](./GameConst.cs.md) - 游戏常量
- [AuctionHelper.cs.md](../../Game/System/Auction/AuctionHelper.cs.md) - 拍卖辅助

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
