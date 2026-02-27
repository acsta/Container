# AIDecision.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | AIDecision.cs |
| **路径** | Assets/Scripts/Code/Game/Component/AI/Decision/AIDecision.cs |
| **所属模块** | 玩法层 → Game/Component/AI/Decision |
| **文件职责** | AI 决策结果数据结构 |

---

## 类/结构体说明

### AIDecision

| 属性 | 说明 |
|------|------|
| **职责** | 存储 AI 决策结果（动作、策略、延迟、表情） |
| **泛型参数** | 无 |
| **继承关系** | 无 |
| **实现的接口** | 无 |

```csharp
public class AIDecision
{
    // AI 决策结果
}
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `Act` | `ActDecision` | `public` | 动作决策（如叫价、离场等） |
| `Tactic` | `AITactic` | `public` | 叫价策略（如低价、中价、高价等） |
| `Emoji` | `string` | `public` | 表情（用于气泡对话） |
| `Delay` | `int` | `public` | 出价延迟时间（毫秒） |

---

## 字段详解

### Act (动作决策)

**类型**: `ActDecision`

**说明**: AI 要执行的动作

**常见值**:
- `NoActDecision`: 无动作
- `Action_Bid`: 叫价
- `Action_Walk`: 走路
- `Action_Run`: 跑步
- `Action_Sit`: 坐下
- `Action_Stand`: 站立

**使用示例**:
```csharp
AIDecision decision = ai.Think();
if (decision.Act == ActDecision.Action_Bid)
{
    // AI 要叫价
}
```

---

### Tactic (叫价策略)

**类型**: `AITactic`

**说明**: AI 的叫价策略

**常见值**:
- `Sidelines`: 观望（不叫价）
- `LowWeight`: 低价叫价
- `MediumWeight`: 中价叫价
- `HighWeight`: 高价叫价
- `Random`: 随机策略
- `RandomLow`: 随机低价策略
- `LeaveWalk`: 离开（走路）
- `LeaveRun`: 离开（跑步）

**使用示例**:
```csharp
AIDecision decision = ai.Think();
switch (decision.Tactic)
{
    case AITactic.LowWeight:
        UserAuction(AITactic.Low);
        break;
    case AITactic.MediumWeight:
        UserAuction(AITactic.Medium);
        break;
    case AITactic.HighWeight:
        UserAuction(AITactic.High);
        break;
}
```

---

### Emoji (表情)

**类型**: `string`

**说明**: AI 的表情（用于气泡对话）

**使用示例**:
```csharp
AIDecision decision = ai.Think();
if (!string.IsNullOrEmpty(decision.Emoji))
{
    // 显示表情气泡
    ShowEmojiBubble(decision.Emoji);
}
```

---

### Delay (出价延迟)

**类型**: `int`

**说明**: 出价延迟时间（毫秒）

**用途**: 模拟人类思考时间，增加真实感

**使用示例**:
```csharp
AIDecision decision = ai.Think();
if (decision.Delay > 0)
{
    // 等待延迟时间后叫价
    await TimerManager.Instance.WaitAsync(decision.Delay);
    UserAuction(decision.Tactic);
}
```

---

## 阅读指引

### 建议的阅读顺序

1. **理解数据结构** - AIDecision 存储什么信息
2. **看 Tactic** - 理解叫价策略
3. **看 Act** - 理解动作决策
4. **了解 Delay/Emoji** - 理解细节设计

### 最值得学习的技术点

1. **决策封装**: 将决策结果封装为单一数据结构
2. **延迟模拟**: Delay 模拟人类思考时间
3. **表情系统**: Emoji 增加 AI 拟人化
4. **策略分离**: Tactic 和 Act 分离，灵活组合

---

## 使用示例

### 示例 1: AI 决策流程

```csharp
// 在 AuctionManager.AIThink() 中
foreach (var bidder in bidders)
{
    AIComponent ai = bidder.GetComponent<AIComponent>();
    
    // 获取决策结果
    AIDecision decision = ai.Think();
    
    // 根据策略叫价
    if (decision.Tactic != AITactic.Sidelines)
    {
        // 等待延迟
        if (decision.Delay > 0)
        {
            await TimerManager.Instance.WaitAsync(decision.Delay);
        }
        
        // 叫价
        UserAuction(decision.Tactic);
        
        // 显示表情
        if (!string.IsNullOrEmpty(decision.Emoji))
        {
            ShowEmojiBubble(bidder.Id, decision.Emoji);
        }
    }
}
```

### 示例 2: 决策结果处理

```csharp
public void ProcessAIDecision(AIDecision decision)
{
    // 检查是否需要叫价
    if (decision.Tactic == AITactic.Sidelines)
    {
        Log.Info("AI 选择观望");
        return;
    }
    
    // 检查动作
    switch (decision.Act)
    {
        case ActDecision.Action_Bid:
            Log.Info("AI 要叫价");
            break;
        case ActDecision.Action_Walk:
            Log.Info("AI 要走路离开");
            break;
        case ActDecision.Action_Run:
            Log.Info("AI 要跑步离开");
            break;
    }
    
    // 显示表情
    if (!string.IsNullOrEmpty(decision.Emoji))
    {
        Log.Info($"AI 表情：{decision.Emoji}");
    }
}
```

---

## 相关文档

- [AIComponent.cs.md](../AIComponent.cs.md) - AI 决策组件
- [AIDecisionTree.cs.md](./AIDecisionTree.cs.md) - AI 决策树
- [AIKnowledge.cs.md](../Knowledge/AIKnowledge.cs.md) - AI 知识库

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
