# MinAuctionTime.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|------|
| **文件名** | MinAuctionTime.cs |
| **路径** | Assets/Scripts/Code/Module/Config/Value/MinAuctionTime.cs |
| **所属模块** | 框架层 → Code/Module/Config/Value |
| **文件职责** | 定义最小拍卖时间值类型，从配置中读取拍卖时间范围的最小值 |

---

## 类/结构体说明

### MinAuctionTime

| 属性 | 说明 |
|------|------|
| **职责** | BaseValue 的拍卖专用实现，从配置表读取拍卖时间范围的最小值 |
| **泛型参数** | 无 |
| **继承关系** | 继承 `BaseValue` |
| **实现的接口** | 无 |

**设计模式**: 配置驱动模式 + 常量模式

```csharp
// 创建最小拍卖时间
var minTime = new MinAuctionTime();
float delay = minTime.Resolve(knowledge);  // 返回配置的最小值
```

---

## 字段与属性

MinAuctionTime **没有字段**，完全依赖配置表数据。

**配置依赖**: `knowledge.Config.AuctionTime[0]`

---

## Odin Inspector 集成

### LabelText 特性

```csharp
[NinoType(false)][LabelText("配置表最低出价时间")]
public class MinAuctionTime: BaseValue
```

**效果**: Inspector 中显示友好的中文标签 "配置表最低出价时间"

**用途**: 策划在配置编辑器中快速识别此类型

---

## 方法说明

### Resolve

**签名**:
```csharp
public override float Resolve(AIKnowledge knowledge)
```

**职责**: 从配置表读取拍卖时间范围的最小值

**核心逻辑**:
```
1. 从 knowledge.Config.AuctionTime 读取时间范围数组
2. 返回 AuctionTime[0]（最小值）
```

**调用者**: DecisionActionNode（用于 Delay 字段）

**参数**:
| 参数名 | 类型 | 说明 |
|--------|------|------|
| `knowledge` | `AIKnowledge` | AI 知识库，包含 Config 引用 |

**返回值**: `float` - 拍卖时间最小值（毫秒）

---

## 配置表依赖

### AuctionTime 配置

```csharp
// knowledge.Config.AuctionTime 是一个数组
AuctionTime = [minTime, maxTime]

// MinAuctionTime 返回 AuctionTime[0]
```

**示例配置**:
```json
{
  "AuctionTime": [500, 1500]
}
```

**MinAuctionTime 返回**: `500` (ms)

---

## 使用示例

### 示例 1: 固定最小延迟

```csharp
// 拍卖 AI：使用最小延迟（快速出价）
var quickBidAction = new DecisionActionNode
{
    Act = ActDecision.Emoji_Nice,
    Tactic = AITactic.HighWeight,
    Delay = new MinAuctionTime(),  // 固定使用最小延迟 500ms
    Remarks = "快速出价策略"
};
```

### 示例 2: 与 RandomAuctionTime 对比使用

```csharp
// 激进 AI：总是快速出价
var aggressiveAI = new DecisionActionNode
{
    Act = ActDecision.Emoji_Cheer,
    Tactic = AITactic.HighWeight,
    Delay = new MinAuctionTime(),  // 最小延迟
    Remarks = "激进策略，快速出价"
};

// 保守 AI：随机延迟
var conservativeAI = new DecisionActionNode
{
    Act = ActDecision.Emoji_Smile1,
    Tactic = AITactic.LowWeight,
    Delay = new RandomAuctionTime(),  // 随机延迟
    Remarks = "保守策略，随机延迟"
};
```

### 示例 3: 条件延迟

```csharp
// 根据情况选择延迟
var conditionalDelay = new DecisionConditionNode
{
    Condition = "IsLastItem",
    True = new DecisionActionNode
    {
        Act = ActDecision.Emoji_Nice,
        Tactic = AITactic.AllIn,
        Delay = new MinAuctionTime(),  // 最后物品，快速出价
        Remarks = "最后物品，快速梭哈"
    },
    False = new DecisionActionNode
    {
        Act = ActDecision.Emoji_Smile1,
        Tactic = AITactic.LowWeight,
        Delay = new RandomAuctionTime(),  // 普通物品，随机延迟
        Remarks = "普通物品，正常出价"
    }
};
```

---

## 与 RandomAuctionTime 对比

| 特性 | MinAuctionTime | RandomAuctionTime |
|------|----------------|-------------------|
| **返回值** | 固定值 min | 随机值 [min, max] |
| **可预测性** | ✅ 完全可预测 | ❌ 不可预测 |
| **使用场景** | 快速反应、紧急情况 | 自然模拟、增加变数 |
| **自然度** | 低（机械感） | 高（拟人化） |

---

## 设计要点

### 为什么需要 MinAuctionTime？

1. **策略表达**: 明确表达"总是最快"的策略意图
2. **性能**: 无需随机数生成
3. **可预测**: 便于调试和测试
4. **配置驱动**: 策划可以调整最小延迟

### 配置驱动的优势

```csharp
// 代码中无需硬编码数值
return knowledge.Config.AuctionTime[0];

// 策划可以在配置表中调整
// [500, 1500] → [300, 1000] 加快节奏
// [500, 1500] → [1000, 3000] 放慢节奏
```

### 毫秒单位

**说明**: 返回值为毫秒（ms）

**转换**:
```csharp
float seconds = minTime / 1000f;  // 转换为秒
yield return new WaitForSeconds(seconds);
```

---

## 典型使用场景

### 1. 激进 AI 策略

```csharp
// 激进 AI：总是最快出价
var aggressiveBid = new DecisionActionNode
{
    Delay = new MinAuctionTime(),
    Tactic = AITactic.HighWeight,
    Remarks = "快速高价竞标"
};
```

### 2. 倒计时场景

```csharp
// 拍卖即将结束，快速出价
var lastChanceBid = new DecisionActionNode
{
    Delay = new MinAuctionTime(),
    Tactic = AITactic.AllIn,
    Remarks = "最后机会，梭哈"
};
```

### 3. 低价抢购

```csharp
// 低价物品，快速抢购
var quickSteal = new DecisionActionNode
{
    Delay = new MinAuctionTime(),
    Tactic = AITactic.LowWeight,
    Remarks = "低价快抢"
};
```

---

## 配置表设计建议

### 推荐配置结构

```json
{
  "AIConfig": {
    "AuctionTime": [500, 1500],
    "ThinkTime": [200, 800],
    "ReactionTime": [100, 300]
  }
}
```

### 多难度配置

```json
{
  "DifficultyConfig": {
    "Easy": {
      "AuctionTime": [1000, 3000]  // 简单：AI 慢
    },
    "Normal": {
      "AuctionTime": [500, 1500]   // 普通：AI 正常
    },
    "Hard": {
      "AuctionTime": [200, 800]    // 困难：AI 快
    }
  }
}
```

---

## 相关文档

- [BaseValue.cs.md](./BaseValue.cs.md) - 值类型基类
- [RandomAuctionTime.cs.md](./RandomAuctionTime.cs.md) - 随机拍卖时间
- [TimeSinceLastBid.cs.md](./TimeSinceLastBid.cs.md) - 距上次出价时间
- [DecisionActionNode.cs.md](../DecisionTree/DecisionActionNode.cs.md) - 使用 Delay 的动作节点

---

*文档生成时间：2026-02-28 | OpenClaw AI 助手*
