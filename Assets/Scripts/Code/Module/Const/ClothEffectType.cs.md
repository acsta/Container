# ClothEffectType.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | ClothEffectType.cs |
| **路径** | Assets/Scripts/Code/Module/Const/ClothEffectType.cs |
| **所属模块** | 框架层 → Code/Module/Const |
| **文件职责** | 服装效果类型枚举定义 |

---

## 枚举说明

### ClothEffectType

| 属性 | 说明 |
|------|------|
| **职责** | 定义服装的效果类型 |
| **类型** | enum（int） |

```csharp
public enum ClothEffectType
{
    None = 0,
    FinalMoneyAddon = 1,
    TaskItemAppearAddon = 2,
    JudgePriceReduce = 3,
}
```

---

## 效果类型

| 值 | 枚举 | 说明 |
|------|------|------|
| 0 | `None` | 无效果 |
| 1 | `FinalMoneyAddon` | 最终金额加成 |
| 2 | `TaskItemAppearAddon` | 任务物品出现加成 |
| 3 | `JudgePriceReduce` | 估价降低 |

---

## 效果详解

### FinalMoneyAddon（最终金额加成）

**说明**: 增加最终出售金额

**用途**: 
- 提高物品售价
- 增加玩家收益

**示例**:
```csharp
if (effectType == ClothEffectType.FinalMoneyAddon)
{
    // 最终金额增加 10%
    finalPrice = basePrice * 1.1m;
}
```

### TaskItemAppearAddon（任务物品出现加成）

**说明**: 增加任务物品出现的概率

**用途**:
- 提高任务物品掉落率
- 帮助玩家完成任务

**示例**:
```csharp
if (effectType == ClothEffectType.TaskItemAppearAddon)
{
    // 任务物品出现概率提高 20%
    taskItemChance = baseChance * 1.2m;
}
```

### JudgePriceReduce（估价降低）

**说明**: 降低鉴定价格

**用途**:
- 降低起拍价
- 让玩家更容易低价拍下

**示例**:
```csharp
if (effectType == ClothEffectType.JudgePriceReduce)
{
    // 估价降低 15%
    judgePrice = basePrice * 0.85m;
}
```

---

## 使用示例

### 示例 1: 检查服装效果

```csharp
// 获取服装效果类型
ClothEffectType effectType = (ClothEffectType)clothConfig.EffectType;

// 检查是否有加成效果
if (effectType != ClothEffectType.None)
{
    Log.Info($"服装有效果：{effectType}");
}
```

### 示例 2: 应用服装效果

```csharp
void ApplyClothEffect(ClothEffectType effectType)
{
    switch (effectType)
    {
        case ClothEffectType.FinalMoneyAddon:
            // 最终金额加成
            playerData.MoneyMultiplier += 0.1m;
            break;
            
        case ClothEffectType.TaskItemAppearAddon:
            // 任务物品出现加成
            taskItemAppearRate += 0.2m;
            break;
            
        case ClothEffectType.JudgePriceReduce:
            // 估价降低
            judgePriceMultiplier -= 0.15m;
            break;
            
        case ClothEffectType.None:
            // 无效果
            break;
    }
}
```

### 示例 3: 计算最终收益

```csharp
BigNumber CalculateFinalPrice(BigNumber basePrice, List<ClothEffectType> effects)
{
    decimal multiplier = 1.0m;
    
    foreach (var effect in effects)
    {
        if (effect == ClothEffectType.FinalMoneyAddon)
        {
            multiplier += 0.1m;  // +10%
        }
    }
    
    return basePrice * multiplier;
}
```

---

## 相关文档

- [GameConst.cs.md](./GameConst.cs.md) - 游戏常量
- [PlayerData.cs.md](../Player/PlayerData.cs.md) - 玩家数据

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
