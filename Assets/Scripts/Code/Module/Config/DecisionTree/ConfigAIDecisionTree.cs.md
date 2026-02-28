# ConfigAIDecisionTree.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | ConfigAIDecisionTree.cs |
| **路径** | Assets/Scripts/Code/Module/Config/DecisionTree/ConfigAIDecisionTree.cs |
| **所属模块** | 框架层 → Code/Module/Config/DecisionTree |
| **文件职责** | 定义 AI 决策树配置数据结构，包含 AI 类型和决策树根节点 |

---

## 类/结构体说明

### ConfigAIDecisionTree

| 属性 | 说明 |
|------|------|
| **职责** | 存储单个 AI 决策树的配置数据，作为配置表的基本单元 |
| **泛型参数** | 无 |
| **继承关系** | 无 |
| **实现的接口** | 无 |

**设计模式**: 数据传输对象（DTO）+ 组合模式（决策树节点）

```csharp
// 配置示例
var aiTree = new ConfigAIDecisionTree
{
    Type = "Attack",
    Node = new DecisionConditionNode
    {
        Condition = "DistanceCheck",
        True = new DecisionActionNode { Act = ActDecision.Action_Punch },
        False = new DecisionActionNode { Act = ActDecision.Action_Run }
    }
};
```

---

## 字段与属性

### Type

| 属性 | 值 |
|------|------|
| **类型** | `string` |
| **访问级别** | `public` |
| **说明** | AI 决策树的类型标识符，用于唯一识别和查询 |

**Nino 序列化**: `[NinoMember(1)]`

**Odin Inspector**: `[LabelText("AI 类型")]`

**用途**:
- 作为 `ConfigAIDecisionTreeCategory.dict` 的 key
- 用于 `Get(string type)` 方法查询
- 在配置文件中作为唯一标识

**示例值**:
- `"Attack"` - 攻击决策树
- `"Patrol"` - 巡逻决策树
- `"Flee"` - 逃跑决策树
- `"Auction"` - 拍卖 AI 决策树

---

### Node

| 属性 | 值 |
|------|------|
| **类型** | `DecisionNode` |
| **访问级别** | `public` |
| **说明** | 决策树的根节点，定义 AI 的决策逻辑 |

**Nino 序列化**: `[NinoMember(2)]`

**多态支持**: 可以是以下任意子类：
- `DecisionConditionNode` - 条件判断节点
- `DecisionCompareNode` - 数值比较节点
- `DecisionActionNode` - 执行动作节点

---

## Nino 序列化特性

### NinoType

```csharp
[NinoType(false)]
```

**说明**: 标记为 Nino 可序列化类型，参数 `false` 表示不使用隐式成员模式。

### NinoMember

```csharp
[NinoMember(1)]  // Type
[NinoMember(2)]  // Node
```

**说明**: 显式指定成员序列化顺序和标识，确保版本兼容性。

---

## 决策树结构

### 树形结构示例

```mermaid
graph TD
    Root[ConfigAIDecisionTree] --> Type[Type: "Attack"]
    Root --> Node[Node: DecisionNode]
    
    Node --> Cond[DecisionConditionNode<br/>距离检查]
    Cond -->|True| Compare[DecisionCompareNode<br/>血量比较]
    Cond -->|False| Action1[DecisionActionNode<br/>逃跑]
    
    Compare -->|True| Action2[DecisionActionNode<br/>强力攻击]
    Compare -->|False| Action3[DecisionActionNode<br/>普通攻击]
```

### JSON 配置示例

```json
{
  "Type": "AuctionAI",
  "Node": {
    "$type": "DecisionConditionNode",
    "Condition": "HasEnoughMoney",
    "True": {
      "$type": "DecisionCompareNode",
      "LeftValue": {
        "$type": "SingleValue",
        "Value": 100
      },
      "CompareMode": "Greater",
      "RightValue": {
        "$type": "FormulaValue",
        "Formula": "CurrentPrice * 1.5"
      },
      "True": {
        "$type": "DecisionActionNode",
        "Act": "LowWeight",
        "Tactic": "Bid",
        "Delay": {
          "$type": "RandomAuctionTime",
          "Min": 500,
          "Max": 1500
        }
      },
      "False": {
        "$type": "DecisionActionNode",
        "Act": "NoActDecision",
        "Tactic": "Wait"
      }
    },
    "False": {
      "$type": "DecisionActionNode",
      "Act": "Emoji_Sigh",
      "Tactic": "LeaveWalk"
    }
  }
}
```

---

## 使用示例

### 示例 1: 创建简单决策树

```csharp
// 创建攻击决策树
var attackTree = new ConfigAIDecisionTree
{
    Type = "Attack",
    Node = new DecisionActionNode
    {
        Act = ActDecision.Action_Punch,
        Tactic = AITactic.Sidelines
    }
};

// 序列化保存
byte[] bytes = Serializer.Serialize(attackTree);
```

### 示例 2: 创建复杂决策树

```csharp
// 创建条件判断决策树
var complexTree = new ConfigAIDecisionTree
{
    Type = "Combat",
    Node = new DecisionConditionNode
    {
        Condition = "IsEnemyVisible",
        True = new DecisionCompareNode
        {
            LeftValue = new SingleValue { Value = 50f },  // 当前血量
            CompareMode = CompareMode.Greater,
            RightValue = new SingleValue { Value = 30f }, // 阈值
            True = new DecisionActionNode
            {
                Act = ActDecision.Action_Punch,
                Tactic = AITactic.HighWeight
            },
            False = new DecisionActionNode
            {
                Act = ActDecision.Action_Run,
                Tactic = AITactic.LeaveRun
            }
        },
        False = new DecisionActionNode
        {
            Act = ActDecision.Stand_Idle1,
            Tactic = AITactic.Sidelines
        }
    }
};
```

### 示例 3: 从配置加载

```csharp
// 通过 ConfigAIDecisionTreeCategory 加载
await ConfigAIDecisionTreeCategory.Instance.LoadAsync();

// 获取特定 AI 决策树
var auctionAI = ConfigAIDecisionTreeCategory.Instance.Get("Auction");

// 使用决策树
var aiController = GetComponent<AIController>();
aiController.LoadDecisionTree(auctionAI.Node);
```

---

## Odin Inspector 支持

### 编辑器显示

```csharp
// 在 Unity Inspector 中显示友好的标签
[NinoMember(1)][LabelText("AI 类型")]
public string Type;
```

**效果**: Inspector 中显示为 "AI 类型" 而非 "Type"

### 配置编辑

策划可以在 Unity Inspector 中直接编辑 AI 决策树配置，支持：
- 下拉选择 AI 类型
- 可视化编辑决策树节点
- 实时预览决策逻辑

---

## 相关文档

- [DecisionNode.cs.md](./DecisionNode.cs.md) - 决策节点基类
- [DecisionConditionNode.cs.md](./DecisionConditionNode.cs.md) - 条件判断节点
- [DecisionCompareNode.cs.md](./DecisionCompareNode.cs.md) - 数值比较节点
- [DecisionActionNode.cs.md](./DecisionActionNode.cs.md) - 执行动作节点
- [ConfigAIDecisionTreeCategory.cs.md](../ConfigAIDecisionTreeCategory.cs.md) - AI 决策树配置管理器

---

*文档生成时间：2026-02-28 | OpenClaw AI 助手*
