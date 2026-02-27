# AIComponent.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | AIComponent.cs |
| **路径** | Assets/Scripts/Code/Game/Component/AI/AIComponent.cs |
| **所属模块** | 玩法层 → Game/Component/AI |
| **文件职责** | AI 决策组件，处理 AI 竞拍者的决策逻辑 |

---

## 类/结构体说明

### AIComponent

| 属性 | 说明 |
|------|------|
| **职责** | AI 竞拍者决策组件，管理知识收集和决策输出 |
| **泛型参数** | 无 |
| **继承关系** | 继承自 `Component` |
| **实现的接口** | `IComponent<string>` |

```csharp
public class AIComponent : Component, IComponent<string>
{
    // AI 决策组件
}
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `knowledge` | `AIKnowledge` | `protected` | 收集的信息（知识库） |
| `decision` | `AIDecision` | `protected` | 当前决策结果 |
| `decisionOld` | `AIDecision` | `protected` | 上一次决策结果 |

---

## 方法说明（按重要程度排序）

### Init(string decisionArchetype)

**签名**:
```csharp
public void Init(string decisionArchetype)
```

**职责**: 初始化 AI 组件

**参数**:
- `decisionArchetype`: 决策原型配置名

**核心逻辑**:
```
1. 从对象池获取 AIKnowledge
2. 调用 knowledge.Init(parent, decisionArchetype)
```

**调用者**: 实体创建时

---

### Destroy()

**签名**:
```csharp
public void Destroy()
```

**职责**: 销毁时清理资源

**核心逻辑**:
```
1. 调用 knowledge.Dispose()
2. 设置 knowledge = null
```

**调用者**: 实体销毁时

---

### Think()

**签名**:
```csharp
public AIDecision Think()
```

**职责**: 执行决策

**返回**: 决策结果 AIDecision

**核心逻辑**:
```
1. 保存上一次决策结果到 decisionOld
2. 调用 AIDecisionTree.Think(knowledge, decision) 执行行为树
3. 根据决策类型处理：
   - Random → 调用 RandomTactic()
   - RandomLow → 调用 RandomLowTactic()
   - LeaveWalk → 调用 IAuctionManager.Instance.Leave()，设置为观望
   - LeaveRun → 调用 IAuctionManager.Instance.Leave()，设置为观望
4. 如果有动作决策，播放动画
5. 返回 decision
```

**决策类型处理**:
```csharp
if (decision.Tactic == AITactic.Random)
{
    RandomTactic();
}
else if (decision.Tactic == AITactic.RandomLow)
{
    RandomLowTactic();
}
else if (decision.Tactic == AITactic.LeaveWalk)
{
    IAuctionManager.Instance.Leave(Id, 0);
    decision.Tactic = AITactic.Sidelines;
    decision.Act = ActDecision.Action_Walk;
}
else if (decision.Tactic == AITactic.LeaveRun)
{
    IAuctionManager.Instance.Leave(Id, 1);
    decision.Tactic = AITactic.Sidelines;
    decision.Act = ActDecision.Action_Run;
}
```

**调用者**: `AuctionManager.AIThink()`

---

### GetKnowledge()

**签名**:
```csharp
public AIKnowledge GetKnowledge()
```

**职责**: 获取知识库

**返回**: AIKnowledge 对象

**核心逻辑**:
```
1. 返回 knowledge
```

**调用者**: 需要访问 AI 知识的代码

---

### RandomTactic()

**签名**:
```csharp
private void RandomTactic()
```

**职责**: 配置表随机决策

**核心逻辑**:
```
1. 根据资金充足程度确定等级 lv：
   - High → lv=3
   - Medium → lv=2
   - Low → lv=1
   - 不足 → 设置为观望 (Sidelines)
2. 随机生成 ran = Random.Range(0, knowledge.Width[lv])
3. 遍历等级，找到匹配的权值区间
4. 设置 decision.Tactic
```

**资金判断**:
```csharp
if (AIDecisionInterface.IsMoneyEnoughHigh(knowledge))
{
    lv = 3;
}
else if (AIDecisionInterface.IsMoneyEnoughMedium(knowledge))
{
    lv = 2;
}
else if (AIDecisionInterface.IsMoneyEnoughLow(knowledge))
{
    lv = 1;
}
else
{
    decision.Tactic = AITactic.Sidelines;
    return;
}
```

**调用者**: `Think()`

---

### RandomLowTactic()

**签名**:
```csharp
private void RandomLowTactic()
```

**职责**: 配置表随机决策（仅低价，其他观望）

**核心逻辑**:
```
1. 检查资金是否足够低价
2. 如果足够：
   - 随机生成 ran
   - 遍历权值找到匹配区间
   - 如果不是低价，设置为观望
3. 如果不足，设置为观望
```

**与 RandomTactic 的区别**:
- `RandomTactic`: 根据资金选择任意等级
- `RandomLowTactic`: 只有低价才叫价，其他情况观望

**调用者**: `Think()`

---

## 阅读指引

### 建议的阅读顺序

1. **理解组件作用** - AI 竞拍者如何决策
2. **看 Think** - 理解决策流程
3. **看 RandomTactic/RandomLowTactic** - 理解随机决策
4. **了解知识库** - 理解 AIKnowledge

### 最值得学习的技术点

1. **行为树决策**: AIDecisionTree.Think() 执行行为树
2. **知识库管理**: AIKnowledge 收集拍卖信息
3. **权值随机**: 根据权值区间随机选择策略
4. **对象池**: AIKnowledge 使用对象池管理

---

## 决策类型（AITactic）

| 类型 | 说明 | 行为 |
|------|------|------|
| `Sidelines` | 观望 | 不叫价 |
| `LowWeight` | 低价 | 低价叫价 |
| `MediumWeight` | 中价 | 中价叫价 |
| `HighWeight` | 高价 | 高价叫价 |
| `Random` | 随机 | 根据权值随机 |
| `RandomLow` | 随机低价 | 只有低价才叫价 |
| `LeaveWalk` | 离开（走路） | 离场，走路动画 |
| `LeaveRun` | 离开（跑步） | 离场，跑步动画 |

---

## 使用示例

### 示例 1: AI 决策流程

```csharp
// 在 AuctionManager.AIThink() 中
foreach (var bidder in bidders)
{
    AIComponent ai = bidder.GetComponent<AIComponent>();
    
    // 执行决策
    AIDecision decision = ai.Think();
    
    // 根据决策叫价
    if (decision.Tactic != AITactic.Sidelines)
    {
        UserAuction(decision.Tactic);
    }
}
```

### 示例 2: 初始化 AI

```csharp
// 创建 AI 竞拍者实体
Entity aiEntity = EntityFactory.Create("Bidder");

// 添加 AI 组件
AIComponent ai = aiEntity.AddComponent<AIComponent>();

// 初始化（传入决策原型配置）
ai.Init("AggressiveBidder");
```

### 示例 3: 获取知识库

```csharp
AIComponent ai = entity.GetComponent<AIComponent>();
AIKnowledge knowledge = ai.GetKnowledge();

// 访问知识
BigNumber money = knowledge.Money;
long bidCount = knowledge.BidCount;
bool isAnger = knowledge.IsAnger;
```

---

## 相关文档

- [AIKnowledge.cs.md](./Knowledge/AIKnowledge.cs.md) - AI 知识库
- [AIDecisionTree.cs.md](./Decision/AIDecisionTree.cs.md) - AI 决策树
- [AIDecisionInterface.cs.md](./Decision/AIDecisionInterface.cs.md) - AI 决策条件

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
