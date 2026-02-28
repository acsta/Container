# TimeSinceLastBid.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|------|
| **文件名** | TimeSinceLastBid.cs |
| **路径** | Assets/Scripts/Code/Module/Config/Value/TimeSinceLastBid.cs |
| **所属模块** | 框架层 → Code/Module/Config/Value |
| **文件职责** | 定义距上次出价时间值类型，计算从上次出价到当前的时间间隔 |

---

## 类/结构体说明

### TimeSinceLastBid

| 属性 | 说明 |
|------|------|
| **职责** | BaseValue 的时间计算实现，返回从上次出价到当前的时间间隔（毫秒） |
| **泛型参数** | 无 |
| **继承关系** | 继承 `BaseValue` |
| **实现的接口** | 无 |

**设计模式**: 状态依赖模式

```csharp
// 创建距上次出价时间
var timeSinceBid = new TimeSinceLastBid();
float elapsed = timeSinceBid.Resolve(knowledge);  // 返回经过的毫秒数
```

---

## 字段与属性

TimeSinceLastBid **没有字段**，完全依赖 AI 知识库的状态数据。

**状态依赖**: `knowledge.LastBidTime`

---

## Odin Inspector 集成

### LabelText 特性

```csharp
[LabelText("距上次出价时间（ms）")]
public class TimeSinceLastBid: BaseValue
```

**效果**: Inspector 中显示友好的中文标签 "距上次出价时间（ms）"

**用途**: 策划在配置编辑器中快速识别此类型

---

## 方法说明

### Resolve

**签名**:
```csharp
public override float Resolve(AIKnowledge knowledge)
```

**职责**: 计算从上次出价到当前的时间间隔

**核心逻辑**:
```
1. 获取当前时间：GameTimerManager.Instance.GetTimeNow()
2. 获取上次出价时间：knowledge.LastBidTime
3. 返回时间差：current - lastBid
```

**调用者**: DecisionCompareNode, DecisionActionNode

**参数**:
| 参数名 | 类型 | 说明 |
|--------|------|------|
| `knowledge` | `AIKnowledge` | AI 知识库，包含 LastBidTime |

**返回值**: `float` - 距上次出价的时间间隔（毫秒）

---

## 依赖

### GameTimerManager

```csharp
GameTimerManager.Instance.GetTimeNow()
```

**说明**: 游戏定时器管理器，提供当前时间戳

**返回值**: 当前时间（毫秒或秒，取决于实现）

---

### AIKnowledge.LastBidTime

```csharp
knowledge.LastBidTime
```

**说明**: AI 上次出价的时间戳

**初始化**: 在 AI 首次出价时设置

**更新**: 每次出价后更新为当前时间

---

## 使用示例

### 示例 1: 冷却时间检查

```csharp
// 检查是否过了冷却时间（5 秒）
var cooldownCheck = new DecisionCompareNode
{
    LeftValue = new TimeSinceLastBid(),
    CompareMode = CompareMode.Greater,
    RightValue = new SingleValue(5000f),  // 5000ms = 5 秒
    True = new DecisionActionNode
    {
        Act = ActDecision.Emoji_Nice,
        Tactic = AITactic.LowWeight,
        Remarks = "冷却时间已过，可以出价"
    },
    False = new DecisionActionNode
    {
        Act = ActDecision.Stand_Idle1,
        Tactic = AITactic.Sidelines,
        Remarks = "冷却中，等待"
    }
};
```

### 示例 2: 犹豫时间判断

```csharp
// 如果犹豫超过 10 秒，强制出价
var hesitationCheck = new DecisionCompareNode
{
    LeftValue = new TimeSinceLastBid(),
    CompareMode = CompareMode.Greater,
    RightValue = new SingleValue(10000f),  // 10 秒
    True = new DecisionActionNode
    {
        Act = ActDecision.Emoji_Sigh,
        Tactic = AITactic.MediumWeight,
        Remarks = "犹豫太久，随便出一个价"
    },
    False = new DecisionActionNode
    {
        Act = ActDecision.Stand_Idle1,
        Tactic = AITactic.Sidelines,
        Remarks = "继续思考"
    }
};
```

### 示例 3: 快速连击检测

```csharp
// 检查是否在快速连续出价（<1 秒）
var rapidBidCheck = new DecisionCompareNode
{
    LeftValue = new TimeSinceLastBid(),
    CompareMode = CompareMode.Less,
    RightValue = new SingleValue(1000f),  // 1 秒
    True = new DecisionActionNode
    {
        Act = ActDecision.Emoji_Cheer,
        Tactic = AITactic.HighWeight,
        Remarks = "快速连击，强势出价"
    },
    False = new DecisionActionNode
    {
        Act = ActDecision.Emoji_Smile1,
        Tactic = AITactic.LowWeight,
        Remarks = "正常节奏出价"
    }
};
```

### 示例 4: 动态延迟计算

```csharp
// 延迟 = 距上次出价时间 * 0.5（越久没出价，延迟越短）
var dynamicDelay = new OperatorValue
{
    Left = new TimeSinceLastBid(),
    Op = LogicMode.Mul,
    Right = new SingleValue(0.5f)
};

var actionNode = new DecisionActionNode
{
    Act = ActDecision.Emoji_Nice,
    Tactic = AITactic.LowWeight,
    Delay = dynamicDelay,
    Remarks = "动态延迟"
};
```

---

## 典型使用场景

### 1. 出价冷却

```csharp
// 防止 AI 过于频繁出价
if (TimeSinceLastBid < CooldownTime)
{
    // 冷却中，等待
}
else
{
    // 可以出价
}
```

### 2. 超时强制行动

```csharp
// 防止 AI 犹豫太久
if (TimeSinceLastBid > MaxThinkTime)
{
    // 超时，随机出价
}
```

### 3. 行为模式分析

```csharp
// 分析 AI 出价模式
if (TimeSinceLastBid < QuickBidThreshold)
{
    // 快速出价模式
}
else if (TimeSinceLastBid > SlowBidThreshold)
{
    // 慢速出价模式
}
```

---

## 时间单位

### 毫秒（ms）

**说明**: 返回值为毫秒

**常见阈值**:
| 时间 | 毫秒 | 说明 |
|------|------|------|
| 0.5 秒 | 500 ms | 快速反应 |
| 1 秒 | 1000 ms | 正常反应 |
| 3 秒 | 3000 ms | 犹豫 |
| 5 秒 | 5000 ms | 冷却时间 |
| 10 秒 | 10000 ms | 超时 |

### 转换

```csharp
// 毫秒转秒
float seconds = milliseconds / 1000f;

// 秒转毫秒
float milliseconds = seconds * 1000f;
```

---

## 设计要点

### 为什么需要 TimeSinceLastBid？

1. **状态感知**: 让 AI 决策基于历史行为
2. **冷却控制**: 防止过于频繁的行动
3. **超时处理**: 避免 AI 卡住不动
4. **行为分析**: 支持复杂的决策逻辑

### 与 RandomAuctionTime 对比

| 特性 | TimeSinceLastBid | RandomAuctionTime |
|------|------------------|-------------------|
| **返回值** | 动态计算（状态依赖） | 随机值（配置依赖） |
| **可预测性** | ✅ 可预测 | ❌ 不可预测 |
| **用途** | 冷却检查、超时判断 | 延迟执行 |
| **状态依赖** | ✅ LastBidTime | ❌ 无 |

### 首次出价的处理

```csharp
// 如果 LastBidTime 为 0（首次出价）
if (knowledge.LastBidTime == 0)
{
    return 0;  // 或返回一个很大的值表示"很久"
}
```

**建议**: 在文档或代码中明确说明首次出价时的行为

---

## 相关文档

- [BaseValue.cs.md](./BaseValue.cs.md) - 值类型基类
- [RandomAuctionTime.cs.md](./RandomAuctionTime.cs.md) - 随机拍卖时间
- [MinAuctionTime.cs.md](./MinAuctionTime.cs.md) - 最小拍卖时间
- [DecisionCompareNode.cs.md](../DecisionTree/DecisionCompareNode.cs.md) - 使用 Value 的比较节点
- [GameTimerManager.cs.md](../../Mono/Module/Timer/GameTimerManager.cs.md) - 游戏定时器管理器

---

*文档生成时间：2026-02-28 | OpenClaw AI 助手*
