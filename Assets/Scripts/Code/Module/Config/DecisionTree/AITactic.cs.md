# AITactic.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|------|
| **文件名** | AITactic.cs |
| **路径** | Assets/Scripts/Code/Module/Config/DecisionTree/AITactic.cs |
| **所属模块** | 框架层 → Code/Module/Config/DecisionTree |
| **文件职责** | 定义 AI 策略枚举，指定 AI 的决策逻辑和行为模式 |

---

## 类/结构体说明

### AITactic 枚举

| 属性 | 说明 |
|------|------|
| **职责** | 定义 AI 可采用的所有策略类型，用于 DecisionActionNode |
| **泛型参数** | 无 |
| **继承关系** | 继承 `System.Enum` |
| **实现的接口** | 无 |

**用途**:
- 在 `DecisionActionNode.Tactic` 字段中使用
- 策划通过下拉菜单选择 AI 策略
- 运行时驱动 AI 逻辑系统执行对应策略

---

## 枚举值详解

### 观望类（0）

| 值 | 枚举名 | 中文标签 | 说明 |
|----|--------|----------|------|
| 0 | `Sidelines` | 观望 | 不参与行动，观察局势 |

**使用场景**:
- AI 暂时不采取行动
- 等待更好的时机
- 观察其他 AI 或玩家行为

**示例**:
```csharp
var waitNode = new DecisionActionNode
{
    Act = ActDecision.Stand_Idle1,
    Tactic = AITactic.Sidelines,
    Remarks = "观望状态，暂不行动"
};
```

---

### 竞价类（1-4）

| 值 | 枚举名 | 中文标签 | 说明 |
|----|--------|----------|------|
| 1 | `LowWeight` | 喊低价 | 出低价，保守策略 |
| 2 | `MediumWeight` | 喊中价 | 出中等价格，平衡策略 |
| 3 | `HighWeight` | 喊高价 | 出高价，激进策略 |
| 4 | `AllIn` | 梭哈 | 全押，孤注一掷 |

**使用场景**: 拍卖系统、竞价场景

**策略对比**:

| 策略 | 风险 | 收益 | 适用场景 |
|------|------|------|----------|
| `LowWeight` | 低 | 低 | 资金有限，试探市场 |
| `MediumWeight` | 中 | 中 | 平衡风险与收益 |
| `HighWeight` | 高 | 高 | 志在必得，资金充足 |
| `AllIn` | 极高 | 极高 | 最后机会，背水一战 |

**示例**:
```csharp
// 保守竞价
var conservativeBid = new DecisionActionNode
{
    Act = ActDecision.Emoji_Nice,
    Tactic = AITactic.LowWeight,
    Delay = new RandomAuctionTime { Min = 1000, Max = 2000 },
    Remarks = "保守策略，喊低价"
};

// 激进竞价
var aggressiveBid = new DecisionActionNode
{
    Act = ActDecision.Emoji_Cheer,
    Tactic = AITactic.HighWeight,
    Delay = new SingleValue { Value = 300 },  // 快速反应
    Remarks = "激进策略，喊高价"
};

// 梭哈
var allInBid = new DecisionActionNode
{
    Act = ActDecision.Dance_1,
    Tactic = AITactic.AllIn,
    Remarks = "梭哈！全押！"
};
```

---

### 随机类（5-6）

| 值 | 枚举名 | 中文标签 | 说明 |
|----|--------|----------|------|
| 5 | `Random` | 钱够则随机 | 资金足够时随机选择策略 |
| 6 | `RandomLow` | 钱够则只随机低价 | 资金足够时只随机选择低价 |

**使用场景**:
- 增加 AI 行为的不可预测性
- 模拟人类犹豫不决的心理
- 防止 AI 行为模式过于固定

**示例**:
```csharp
// 随机策略
var randomNode = new DecisionActionNode
{
    Act = ActDecision.Emoji_Smile1,
    Tactic = AITactic.Random,
    Remarks = "随机策略，增加不确定性"
};

// 随机低价
var randomLowNode = new DecisionActionNode
{
    Act = ActDecision.Emoji_Putter_Around,
    Tactic = AITactic.RandomLow,
    Remarks = "随机低价策略，保守但不可预测"
};
```

---

### 离场类（7-8）

| 值 | 枚举名 | 中文标签 | 说明 |
|----|--------|----------|------|
| 7 | `LeaveWalk` | 离场 | 走路离开，优雅退场 |
| 8 | `LeaveRun` | 跑路 | 跑步离开，紧急逃跑 |

**使用场景**:
- 资金不足时离场
- 价格过高放弃竞拍
- 危险情况逃跑

**对比**:

| 策略 | 速度 | 情绪 | 使用场景 |
|------|------|------|----------|
| `LeaveWalk` | 慢 | 平静/无奈 | 正常离场，优雅退场 |
| `LeaveRun` | 快 | 紧急/恐慌 | 紧急逃跑，危险情况 |

**示例**:
```csharp
// 优雅离场
var gracefulLeave = new DecisionActionNode
{
    Act = ActDecision.Emoji_Sigh,
    Tactic = AITactic.LeaveWalk,
    Remarks = "价格太高，无奈离场"
};

// 紧急逃跑
var panicRun = new DecisionActionNode
{
    Act = ActDecision.Action_Run,
    Tactic = AITactic.LeaveRun,
    Emoji = "3",  // 震惊表情
    Remarks = "危险！快跑！"
};
```

---

## Odin Inspector 集成

### LabelText 特性

```csharp
public enum AITactic
{
    [LabelText("观望")]
    Sidelines = 0,
    
    [LabelText("喊低价")]
    LowWeight = 1,
    
    [LabelText("喊中价")]
    MediumWeight = 2,
    
    [LabelText("喊高价")]
    HighWeight = 3,
    
    [LabelText("梭哈")]
    AllIn = 4,
    
    [LabelText("钱够则随机")]
    Random = 5,
    
    [LabelText("钱够则只随机低价")]
    RandomLow = 6,
    
    [LabelText("离场")]
    LeaveWalk = 7,
    
    [LabelText("跑路")]
    LeaveRun = 8
}
```

**效果**: Inspector 中显示中文标签，策划更易理解

---

## 策略执行逻辑

### 拍卖系统示例

```csharp
public class AuctionAI : MonoBehaviour
{
    public void ExecuteTactic(AITactic tactic)
    {
        switch (tactic)
        {
            case AITactic.Sidelines:
                // 观望，不行动
                break;
                
            case AITactic.LowWeight:
                // 喊低价
                Bid(CalculateLowPrice());
                break;
                
            case AITactic.MediumWeight:
                // 喊中价
                Bid(CalculateMediumPrice());
                break;
                
            case AITactic.HighWeight:
                // 喊高价
                Bid(CalculateHighPrice());
                break;
                
            case AITactic.AllIn:
                // 梭哈
                Bid(AllMoney());
                break;
                
            case AITactic.Random:
                // 随机选择
                var tactics = new[] { AITactic.LowWeight, AITactic.MediumWeight, AITactic.HighWeight };
                ExecuteTactic(tactics[Random.Range(0, tactics.Length)]);
                break;
                
            case AITactic.RandomLow:
                // 随机低价
                Bid(CalculateLowPrice() * Random.Range(0.8f, 1.2f));
                break;
                
            case AITactic.LeaveWalk:
                // 走路离开
                StartWalkingAway();
                break;
                
            case AITactic.LeaveRun:
                // 跑步离开
                StartRunningAway();
                break;
        }
    }
}
```

---

## 使用示例

### 示例 1: 完整拍卖 AI 决策树

```csharp
var auctionAI = new ConfigAIDecisionTree
{
    Type = "Auctioneer",
    Node = new DecisionConditionNode
    {
        Condition = "HasEnoughMoney",
        Remarks = "检查是否有足够资金",
        
        // 资金足够
        True = new DecisionCompareNode
        {
            LeftValue = new CurrentMoneyValue(),
            CompareMode = CompareMode.Greater,
            RightValue = new FormulaValue("CurrentPrice * 2"),
            Remarks = "资金是否充足（> 2 倍当前价）",
            
            // 资金充足 - 激进策略
            True = new DecisionActionNode
            {
                Act = ActDecision.Emoji_Cheer,
                Tactic = AITactic.HighWeight,
                Delay = new SingleValue { Value = 500 },
                Remarks = "资金充足，高价竞标"
            },
            
            // 资金一般 - 保守策略
            False = new DecisionActionNode
            {
                Act = ActDecision.Emoji_Smile1,
                Tactic = AITactic.RandomLow,
                Delay = new RandomAuctionTime { Min = 800, Max = 1500 },
                Remarks = "资金一般，随机低价"
            }
        },
        
        // 资金不足 - 离场
        False = new DecisionActionNode
        {
            Act = ActDecision.Emoji_Aghast,
            Tactic = AITactic.LeaveWalk,
            Remarks = "资金不足，无奈离场"
        }
    }
};
```

### 示例 2: 战斗 AI 策略

```csharp
var combatAI = new ConfigAIDecisionTree
{
    Type = "Combat",
    Node = new DecisionCompareNode
    {
        LeftValue = new HealthPercentValue(),
        CompareMode = CompareMode.Greater,
        RightValue = new SingleValue { Value = 50f },
        Remarks = "血量是否高于 50%",
        
        // 血量健康 - 进攻
        True = new DecisionActionNode
        {
            Act = ActDecision.Action_Punch,
            Tactic = AITactic.HighWeight,  // 激进进攻
            Remarks = "血量健康，全力进攻"
        },
        
        // 血量低 - 逃跑
        False = new DecisionActionNode
        {
            Act = ActDecision.Action_Run,
            Tactic = AITactic.LeaveRun,  // 紧急逃跑
            Remarks = "血量低，快跑！"
        }
    }
};
```

### 示例 3: 社交 AI 策略

```csharp
var socialAI = new ConfigAIDecisionTree
{
    Type = "Social",
    Node = new DecisionActionNode
    {
        Act = ActDecision.Emoji_Hi,
        Tactic = AITactic.Sidelines,  // 只是打招呼，不参与
        Remarks = "打招呼，观望状态"
    }
};
```

---

## 策略统计

| 分类 | 枚举值 | 数量 |
|------|--------|------|
| 观望类 | Sidelines | 1 |
| 竞价类 | LowWeight, MediumWeight, HighWeight, AllIn | 4 |
| 随机类 | Random, RandomLow | 2 |
| 离场类 | LeaveWalk, LeaveRun | 2 |
| **总计** | - | **9** |

---

## 设计要点

### 为什么分离 Act 和 Tactic？

1. **关注点分离**:
   - `Act`: 视觉表现（做什么动作）
   - `Tactic`: 逻辑决策（用什么策略）

2. **灵活组合**:
   ```csharp
   // 同样的动作，不同策略
   new DecisionActionNode { Act = Emoji_Nice, Tactic = LowWeight };
   new DecisionActionNode { Act = Emoji_Nice, Tactic = HighWeight };
   
   // 同样的策略，不同动作
   new DecisionActionNode { Act = Emoji_Nice, Tactic = LowWeight };
   new DecisionActionNode { Act = Action_Punch, Tactic = LowWeight };
   ```

3. **独立扩展**: 新增策略不影响动作系统

### 策略命名规范

- `Sidelines`: 观望，不参与
- `*Weight`: 竞价策略（Low/Medium/High/All）
- `Random*`: 随机策略
- `Leave*`: 离场策略（Walk/Run）

### 策略扩展建议

新增策略时：
1. 在枚举末尾添加新值
2. 在 AI 逻辑系统中实现对应策略
3. 添加 `LabelText` 特性
4. 更新相关文档

---

## 相关文档

- [DecisionActionNode.cs.md](./DecisionActionNode.cs.md) - 使用 AITactic 的节点
- [ActDecision.cs.md](./ActDecision.cs.md) - AI 动作枚举
- [DecisionCompareNode.cs.md](./DecisionCompareNode.cs.md) - 数值比较节点（用于条件判断）

---

*文档生成时间：2026-02-28 | OpenClaw AI 助手*
