# RandomAuctionTime.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|------|
| **文件名** | RandomAuctionTime.cs |
| **路径** | Assets/Scripts/Code/Module/Config/Value/RandomAuctionTime.cs |
| **所属模块** | 框架层 → Code/Module/Config/Value |
| **文件职责** | 定义随机拍卖时间值类型，从配置中读取时间范围并返回随机值 |

---

## 类/结构体说明

### RandomAuctionTime

| 属性 | 说明 |
|------|------|
| **职责** | BaseValue 的拍卖专用实现，从配置表读取拍卖时间范围并返回随机值 |
| **泛型参数** | 无 |
| **继承关系** | 继承 `BaseValue` |
| **实现的接口** | 无 |

**设计模式**: 配置驱动模式

```csharp
// 创建随机拍卖时间
var randomTime = new RandomAuctionTime();
float delay = randomTime.Resolve(knowledge);  // 从配置读取范围并随机
```

---

## 字段与属性

RandomAuctionTime **没有字段**，完全依赖配置表数据。

**配置依赖**: `knowledge.Config.AuctionTime`

---

## Odin Inspector 集成

### LabelText 特性

```csharp
[NinoType(false)][LabelText("配置表随机出价时间")]
public class RandomAuctionTime: BaseValue
```

**效果**: Inspector 中显示友好的中文标签 "配置表随机出价时间"

**用途**: 策划在配置编辑器中快速识别此类型

---

## 方法说明

### Resolve

**签名**:
```csharp
public override float Resolve(AIKnowledge knowledge)
```

**职责**: 从配置表读取拍卖时间范围，返回随机值

**核心逻辑**:
```
1. 从 knowledge.Config.AuctionTime 读取时间范围数组
2. 使用 AuctionTime[0] 作为最小值
3. 使用 AuctionTime[1] 作为最大值
4. 调用 Random.Range(min, max) 返回随机值
```

**调用者**: DecisionActionNode（用于 Delay 字段）

**参数**:
| 参数名 | 类型 | 说明 |
|--------|------|------|
| `knowledge` | `AIKnowledge` | AI 知识库，包含 Config 引用 |

**返回值**: `float` - 拍卖时间范围内的随机值（毫秒）

---

## 配置表依赖

### AuctionTime 配置

```csharp
// knowledge.Config.AuctionTime 是一个数组
AuctionTime = [minTime, maxTime]

// 示例配置
AuctionTime = [500, 1500]  // 500ms - 1500ms
```

**配置位置**: 可能在 AI 配置表或全局配置表中

**配置示例** (JSON):
```json
{
  "AuctionTime": [500, 1500]
}
```

---

## 使用示例

### 示例 1: 拍卖 AI 出价延迟

```csharp
// 拍卖 AI：随机延迟出价
var bidAction = new DecisionActionNode
{
    Act = ActDecision.Emoji_Nice,
    Tactic = AITactic.LowWeight,
    Delay = new RandomAuctionTime(),  // 从配置读取 500-1500ms
    Remarks = "随机延迟出价"
};
```

### 示例 2: 完整拍卖决策树

```csharp
var auctionAI = new ConfigAIDecisionTree
{
    Type = "Bidder",
    Node = new DecisionConditionNode
    {
        Condition = "HasEnoughMoney",
        True = new DecisionActionNode
        {
            Act = ActDecision.Emoji_Nice,
            Tactic = AITactic.LowWeight,
            Delay = new RandomAuctionTime(),  // 配置驱动延迟
            Remarks = "资金充足，随机延迟后出价"
        },
        False = new DecisionActionNode
        {
            Act = ActDecision.Emoji_Sigh,
            Tactic = AITactic.LeaveWalk,
            Remarks = "资金不足，离场"
        }
    }
};
```

### 示例 3: 不同 AI 使用不同时间范围

```csharp
// 配置表可以为不同 AI 类型定义不同时间范围
// AI_001 (保守型): AuctionTime = [1000, 3000]  // 思考时间长
// AI_002 (激进型): AuctionTime = [200, 800]    // 快速反应
// AI_003 (普通型): AuctionTime = [500, 1500]   // 正常速度

// 代码中通过 knowledge.Config 访问对应配置
var delay = new RandomAuctionTime().Resolve(knowledge);
```

---

## 与 MinAuctionTime 对比

| 特性 | RandomAuctionTime | MinAuctionTime |
|------|-------------------|----------------|
| **返回值** | 随机值 [min, max] | 固定值 min |
| **使用场景** | 增加不确定性 | 固定最小延迟 |
| **自然度** | 高（模拟人类） | 中（机械感） |
| **配置依赖** | AuctionTime[0] 和 [1] | AuctionTime[0] |

---

## 设计要点

### 为什么从配置读取？

1. **策划友好**: 无需修改代码即可调整时间范围
2. **可配置**: 不同 AI/场景可以使用不同配置
3. **热更新**: 修改配置表即可生效（如果支持热更）
4. **平衡调整**: 便于游戏平衡性调整

### 随机性的意义

1. **自然度**: 模拟人类出价的犹豫时间
2. **防识别**: 避免 AI 行为模式被玩家识别
3. **趣味性**: 增加游戏变数
4. **公平性**: 多个 AI 不会同时出价

### 毫秒单位

**说明**: 返回值为毫秒（ms）

**转换**:
```csharp
float seconds = delay / 1000f;  // 转换为秒
yield return new WaitForSeconds(seconds);
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

### 多 AI 类型配置

```json
{
  "AIConfigs": {
    "Conservative": {
      "AuctionTime": [1000, 3000],
      "ThinkTime": [500, 1500]
    },
    "Aggressive": {
      "AuctionTime": [200, 800],
      "ThinkTime": [100, 300]
    },
    "Normal": {
      "AuctionTime": [500, 1500],
      "ThinkTime": [200, 800]
    }
  }
}
```

---

## 相关文档

- [BaseValue.cs.md](./BaseValue.cs.md) - 值类型基类
- [MinAuctionTime.cs.md](./MinAuctionTime.cs.md) - 最小拍卖时间
- [TimeSinceLastBid.cs.md](./TimeSinceLastBid.cs.md) - 距上次出价时间
- [DecisionActionNode.cs.md](../DecisionTree/DecisionActionNode.cs.md) - 使用 Delay 的动作节点

---

*文档生成时间：2026-02-28 | OpenClaw AI 助手*
