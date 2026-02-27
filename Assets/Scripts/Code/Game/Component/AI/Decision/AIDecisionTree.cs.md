# AIDecisionTree.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | AIDecisionTree.cs |
| **路径** | Assets/Scripts/Code/Game/Component/AI/Decision/AIDecisionTree.cs |
| **所属模块** | 玩法层 → Game/Component/AI/Decision |
| **文件职责** | AI 行为树决策系统 |

---

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **类/结构体** | AIDecisionTree（静态类） |
| **职责** | 执行 AI 行为树决策 |
| **依赖** | AIKnowledge, AIDecision, DecisionNode |

---

## 核心方法

### Think(AIKnowledge knowledge, AIDecision decision)

**签名**:
```csharp
public static void Think(AIKnowledge knowledge, AIDecision decision)
```

**职责**: 执行行为树决策

**参数**:
- `knowledge`: AI 知识库（当前状态信息）
- `decision`: 决策结果（输出）

**核心逻辑**:
```
1. 检查是否开启 AI 拍卖（GameSetting.OpenAIAuction）
2. 获取决策原型配置（ConfigAIDecisionTreeCategory）
3. 如果配置有节点，调用 Handler() 递归执行行为树
4. 在编辑器模式下，记录决策日志
```

**调用者**: `AIComponent.Think()`

---

### Handler(AIKnowledge knowledge, AIDecision decision, DecisionNode tree, StringBuilder sb)

**签名**:
```csharp
private static void Handler(AIKnowledge knowledge, AIDecision decision, DecisionNode tree, StringBuilder sb = null)
```

**职责**: 递归处理行为树节点

**参数**:
- `knowledge`: AI 知识库
- `decision`: 决策结果
- `tree`: 当前节点
- `sb`: 日志字符串构建器（编辑器模式）

**核心逻辑**:

#### 1. 动作节点（DecisionActionNode）

```csharp
if (tree is DecisionActionNode actionNode)
{
    decision.Act = actionNode.Act;
    decision.Tactic = actionNode.Tactic;
    decision.Delay = (int)actionNode.Delay.Resolve(knowledge);
    decision.Emoji = actionNode.Emoji;
    
    // 记录日志
    Log.Info(sb.ToString());
}
```

#### 2. 条件节点（DecisionConditionNode）

```csharp
else if (tree is DecisionConditionNode conditionNode)
{
    // 查找条件方法
    if (AIDecisionInterface.Methods.TryGetValue(conditionNode.Condition, out var func))
    {
        // 执行条件判断
        if (func(knowledge))
        {
            // 条件为真，处理 True 分支
            Handler(knowledge, decision, conditionNode.True, sb);
        }
        else
        {
            // 条件为假，处理 False 分支
            Handler(knowledge, decision, conditionNode.False, sb);
        }
    }
}
```

#### 3. 比较节点（DecisionCompareNode）

```csharp
else if (tree is DecisionCompareNode compareNode)
{
    // 解析左右值
    var left = compareNode.LeftValue?.Resolve(knowledge) ?? 0;
    var right = compareNode.RightValue?.Resolve(knowledge) ?? 0;
    
    // 执行比较
    if (IsMatch(left, right, compareNode.CompareMode))
    {
        // 条件为真，处理 True 分支
        Handler(knowledge, decision, compareNode.True, sb);
    }
    else
    {
        // 条件为假，处理 False 分支
        Handler(knowledge, decision, compareNode.False, sb);
    }
}
```

**调用者**: `Think()`

---

### IsMatch(float l, float r, CompareMode mode)

**签名**:
```csharp
private static bool IsMatch(float l, float r, CompareMode mode)
```

**职责**: 执行数值比较

**参数**:
- `l`: 左值
- `r`: 右值
- `mode`: 比较模式

**返回**: 比较结果

**核心逻辑**:
```csharp
switch (mode)
{
    case CompareMode.Equal:
        return l == r;
    case CompareMode.NotEqual:
        return l != r;
    case CompareMode.Greater:
        return l > r;
    case CompareMode.Less:
        return l < r;
    case CompareMode.LEqual:
        return l <= r;
    case CompareMode.GEqual:
        return l >= r;
}
```

**调用者**: `Handler()`（处理 DecisionCompareNode）

---

## 阅读指引

### 建议的阅读顺序

1. **理解行为树概念** - 什么是行为树
2. **看 Think** - 理解决策入口
3. **看 Handler** - 理解节点处理
4. **了解节点类型** - 三种节点的作用

### 最值得学习的技术点

1. **行为树决策**: 树形结构的 AI 决策系统
2. **递归处理**: Handler 递归遍历行为树
3. **条件反射**: AIDecisionInterface.Methods 动态调用条件方法
4. **编辑器日志**: StringBuilder 记录决策路径

---

## 行为树节点类型

| 节点类型 | 说明 | 作用 |
|---------|------|------|
| `DecisionActionNode` | 动作节点 | 输出最终决策结果 |
| `DecisionConditionNode` | 条件节点 | 根据条件判断选择分支 |
| `DecisionCompareNode` | 比较节点 | 比较两个数值选择分支 |

---

## 行为树执行流程示例

```
决策原型："AggressiveBidder"
│
├─ 条件：资金是否充足？(IsMoneyEnoughHigh)
│  ├─ True → 动作：高价叫价 (HighWeight)
│  └─ False → 条件：资金是否中等？(IsMoneyEnoughMedium)
│     ├─ True → 动作：中价叫价 (MediumWeight)
│     └─ False → 条件：资金是否足够低价？(IsMoneyEnoughLow)
│        ├─ True → 动作：低价叫价 (LowWeight)
│        └─ False → 动作：观望 (Sidelines)
```

**执行过程**:
1. 从根节点开始
2. 检查条件 `IsMoneyEnoughHigh(knowledge)`
3. 如果为真，执行动作节点（高价叫价）
4. 如果为假，进入 False 分支，检查下一个条件
5. 直到遇到动作节点，输出决策结果

---

## 使用示例

### 示例 1: 执行行为树决策

```csharp
// 在 AIComponent.Think() 中
public AIDecision Think()
{
    // 保存上一次决策
    decisionOld.Act = decision.Act;
    decisionOld.Tactic = decision.Tactic;
    
    // 执行行为树决策
    AIDecisionTree.Think(knowledge, decision);
    
    // 处理特殊策略
    if (decision.Tactic == AITactic.Random)
    {
        RandomTactic();
    }
    
    return decision;
}
```

### 示例 2: 编辑器日志输出

```csharp
// 在编辑器模式下，记录决策路径
#if UNITY_EDITOR
StringBuilder sb = new StringBuilder();
sb.AppendLine(knowledge.DecisionArchetype);
AIDecisionTree.Think(knowledge, decision, sb);
Log.Info(sb.ToString());
#endif

// 输出示例:
// AggressiveBidder
// IsMoneyEnoughHigh true
// Action: HighWeight, Delay: 500ms
```

---

## 配置示例（ConfigAIDecisionTree）

```json
{
  "Id": 1,
  "Name": "AggressiveBidder",
  "Node": {
    "$type": "DecisionConditionNode",
    "Condition": "IsMoneyEnoughHigh",
    "True": {
      "$type": "DecisionActionNode",
      "Act": "Action_Bid",
      "Tactic": "HighWeight",
      "Delay": 500,
      "Emoji": "😎"
    },
    "False": {
      "$type": "DecisionConditionNode",
      "Condition": "IsMoneyEnoughMedium",
      "True": {
        "$type": "DecisionActionNode",
        "Act": "Action_Bid",
        "Tactic": "MediumWeight",
        "Delay": 1000,
        "Emoji": "🙂"
      },
      "False": {
        "$type": "DecisionActionNode",
        "Act": "NoAct",
        "Tactic": "Sidelines",
        "Emoji": "😐"
      }
    }
  }
}
```

---

## 相关文档

- [AIComponent.cs.md](../AIComponent.cs.md) - AI 决策组件
- [AIDecision.cs.md](./AIDecision.cs.md) - AI 决策结果
- [AIDecisionInterface.cs.md](./AIDecisionInterface.cs.md) - AI 决策条件方法
- [AIKnowledge.cs.md](../Knowledge/AIKnowledge.cs.md) - AI 知识库

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
