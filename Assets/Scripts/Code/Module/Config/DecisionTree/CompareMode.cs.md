# CompareMode.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|------|
| **文件名** | CompareMode.cs |
| **路径** | Assets/Scripts/Code/Module/Config/DecisionTree/CompareMode.cs |
| **所属模块** | 框架层 → Code/Module/Config/DecisionTree |
| **文件职责** | 定义数值比较模式枚举，用于 DecisionCompareNode |

---

## 类/结构体说明

### CompareMode 枚举

| 属性 | 说明 |
|------|------|
| **职责** | 定义所有可用的数值比较操作符，用于 DecisionCompareNode.CompareMode |
| **泛型参数** | 无 |
| **继承关系** | 继承 `System.Enum` |
| **实现的接口** | 无 |

**用途**:
- 在 `DecisionCompareNode.CompareMode` 字段中使用
- 策划通过下拉菜单选择比较操作符
- 运行时决定如何比较两个数值

---

## 枚举值详解

| 值 | 枚举名 | 中文标签 | 操作符 | 说明 |
|----|--------|----------|--------|------|
| 0 | `Equal` | == | `==` | 等于 |
| 1 | `NotEqual` | != | `!=` | 不等于 |
| 2 | `Greater` | > | `>` | 大于 |
| 3 | `Less` | < | `<` | 小于 |
| 4 | `LEqual` | <= | `<=` | 小于等于 |
| 5 | `GEqual` | >= | `>=` | 大于等于 |

---

## Odin Inspector 集成

### LabelText 特性

```csharp
public enum CompareMode
{
    [LabelText("==")]
    Equal,
    
    [LabelText("!=")]
    NotEqual,
    
    [LabelText(">")]
    Greater,
    
    [LabelText("<")]
    Less,
    
    [LabelText("<=")]
    LEqual,
    
    [LabelText(">=")]
    GEqual,
}
```

**效果**: Inspector 中显示数学符号，直观易懂

**Inspector 显示**:
```
比较模式：[== ▼]
```

---

## 比较逻辑实现

### 代码示例

```csharp
public class DecisionCompareNode : DecisionNode
{
    public BaseValue LeftValue;
    public CompareMode CompareMode;
    public BaseValue RightValue;
    
    public bool Evaluate(AIKnowledge knowledge)
    {
        float left = LeftValue.Resolve(knowledge);
        float right = RightValue.Resolve(knowledge);
        
        return this.CompareMode switch
        {
            TaoTie.CompareMode.Equal => left == right,
            TaoTie.CompareMode.NotEqual => left != right,
            TaoTie.CompareMode.Greater => left > right,
            TaoTie.CompareMode.Less => left < right,
            TaoTie.CompareMode.LEqual => left <= right,
            TaoTie.CompareMode.GEqual => left >= right,
            _ => false
        };
    }
}
```

### 浮点数比较注意事项

```csharp
// 浮点数相等比较应使用容差
public static bool ApproximatelyEqual(float a, float b, float epsilon = 0.0001f)
{
    return Mathf.Abs(a - b) < epsilon;
}

// 在 CompareMode.Equal 中使用
case CompareMode.Equal:
    return ApproximatelyEqual(left, right);
```

---

## 使用示例

### 示例 1: 等于判断

```csharp
// 检查当前血量是否等于 50
var equalCheck = new DecisionCompareNode
{
    LeftValue = new CurrentHealthValue(),
    CompareMode = CompareMode.Equal,
    RightValue = new SingleValue { Value = 50f },
    True = new DecisionActionNode { Act = ActDecision.Emoji_Nice },
    False = new DecisionActionNode { Act = ActDecision.Emoji_Sigh }
};
```

### 示例 2: 不等于判断

```csharp
// 检查目标 ID 是否不等于 0（有目标）
var hasTargetCheck = new DecisionCompareNode
{
    LeftValue = new TargetIdValue(),
    CompareMode = CompareMode.NotEqual,
    RightValue = new SingleValue { Value = 0f },
    True = new DecisionActionNode { Act = ActDecision.Action_Punch },
    False = new DecisionActionNode { Act = ActDecision.Stand_Idle1 }
};
```

### 示例 3: 大于判断

```csharp
// 检查血量是否大于 50%
var healthCheck = new DecisionCompareNode
{
    LeftValue = new HealthPercentValue(),
    CompareMode = CompareMode.Greater,
    RightValue = new SingleValue { Value = 50f },
    True = new DecisionActionNode 
    { 
        Act = ActDecision.Action_Punch,
        Tactic = AITactic.HighWeight,
        Remarks = "血量健康，进攻"
    },
    False = new DecisionActionNode 
    { 
        Act = ActDecision.Action_Run,
        Tactic = AITactic.LeaveRun,
        Remarks = "血量低，逃跑"
    }
};
```

### 示例 4: 小于判断

```csharp
// 检查距离是否小于 5 米（近身）
var distanceCheck = new DecisionCompareNode
{
    LeftValue = new DistanceToTargetValue(),
    CompareMode = CompareMode.Less,
    RightValue = new SingleValue { Value = 5f },
    True = new DecisionActionNode { Act = ActDecision.Action_Punch },
    False = new DecisionActionNode { Act = ActDecision.Action_Walk }
};
```

### 示例 5: 小于等于判断

```csharp
// 检查价格是否小于等于预算
var budgetCheck = new DecisionCompareNode
{
    LeftValue = new CurrentPriceValue(),
    CompareMode = CompareMode.LEqual,
    RightValue = new BudgetValue(),
    True = new DecisionActionNode 
    { 
        Act = ActDecision.Emoji_Nice,
        Tactic = AITactic.LowWeight,
        Remarks = "价格在预算内，竞标"
    },
    False = new DecisionActionNode 
    { 
        Act = ActDecision.Emoji_Sigh,
        Tactic = AITactic.LeaveWalk,
        Remarks = "超出预算，放弃"
    }
};
```

### 示例 6: 大于等于判断

```csharp
// 检查等级是否大于等于 10
var levelCheck = new DecisionCompareNode
{
    LeftValue = new PlayerLevelValue(),
    CompareMode = CompareMode.GEqual,
    RightValue = new SingleValue { Value = 10f },
    True = new DecisionActionNode { Act = ActDecision.Dance_1 },
    False = new DecisionActionNode { Act = ActDecision.Stand_Idle1 }
};
```

---

## 真值表

| CompareMode | Left | Right | 结果 |
|-------------|------|-------|------|
| `Equal` (==) | 50 | 50 | ✅ True |
| `Equal` (==) | 50 | 30 | ❌ False |
| `NotEqual` (!=) | 50 | 50 | ❌ False |
| `NotEqual` (!=) | 50 | 30 | ✅ True |
| `Greater` (>) | 50 | 30 | ✅ True |
| `Greater` (>) | 50 | 50 | ❌ False |
| `Greater` (>) | 30 | 50 | ❌ False |
| `Less` (<) | 30 | 50 | ✅ True |
| `Less` (<) | 50 | 50 | ❌ False |
| `Less` (<) | 50 | 30 | ❌ False |
| `LEqual` (<=) | 30 | 50 | ✅ True |
| `LEqual` (<=) | 50 | 50 | ✅ True |
| `LEqual` (<=) | 50 | 30 | ❌ False |
| `GEqual` (>=) | 50 | 30 | ✅ True |
| `GEqual` (>=) | 50 | 50 | ✅ True |
| `GEqual` (>=) | 30 | 50 | ❌ False |

---

## 组合使用示例

### 范围检查（大于且小于）

```csharp
// 检查血量是否在 30-70 之间
var rangeCheck = new DecisionConditionNode
{
    Condition = "HasTarget",
    True = new DecisionCompareNode
    {
        LeftValue = new HealthPercentValue(),
        CompareMode = CompareMode.Greater,
        RightValue = new SingleValue { Value = 30f },
        True = new DecisionCompareNode
        {
            LeftValue = new HealthPercentValue(),
            CompareMode = CompareMode.Less,
            RightValue = new SingleValue { Value = 70f },
            True = new DecisionActionNode 
            { 
                Act = ActDecision.Action_Punch,
                Remarks = "血量 30-70%，正常战斗"
            },
            False = new DecisionActionNode 
            { 
                Act = ActDecision.Emoji_Cheer,
                Remarks = "血量>70%，状态很好"
            }
        },
        False = new DecisionActionNode 
        { 
            Act = ActDecision.Action_Run,
            Remarks = "血量<30%，危险！"
        }
    },
    False = new DecisionActionNode { Act = ActDecision.Stand_Idle1 }
};
```

### 边界检查（等于或大于等于）

```csharp
// 检查血量是否等于或低于 30%（危险线）
var dangerCheck = new DecisionCompareNode
{
    LeftValue = new HealthPercentValue(),
    CompareMode = CompareMode.LEqual,
    RightValue = new SingleValue { Value = 30f },
    True = new DecisionActionNode 
    { 
        Act = ActDecision.Emoji_Cry,
        Tactic = AITactic.LeaveRun,
        Remarks = "血量≤30%，危险！逃跑！"
    },
    False = new DecisionActionNode 
    { 
        Act = ActDecision.Action_Punch,
        Remarks = "血量>30%，继续战斗"
    }
};
```

---

## 设计要点

### 为什么使用枚举？

1. **类型安全**: 避免字符串拼写错误
2. **性能**: 整数比较比字符串快
3. **编辑器友好**: 下拉菜单选择
4. **完整性**: 包含所有常用比较操作符
5. **可扩展**: 可以添加新的比较模式（如近似等于）

### 命名规范

- `Equal`: 等于（==）
- `NotEqual`: 不等于（!=）
- `Greater`: 大于（>）
- `Less`: 小于（<）
- `LEqual`: Less Equal，小于等于（<=）
- `GEqual`: Greater Equal，大于等于（>=）

### 浮点数比较

对于浮点数比较，建议：
- `Equal` 使用容差比较（ApproximatelyEqual）
- 避免直接 `==` 比较浮点数
- 考虑添加 `ApproximatelyEqual` 模式

---

## 扩展建议

### 可能的扩展模式

```csharp
public enum CompareMode
{
    // ... 现有值
    
    [LabelText("≈")]
    ApproximatelyEqual,  // 近似等于（浮点数容差）
    
    [LabelText(">>")]
    MuchGreater,  // 远大于（差值超过阈值）
    
    [LabelText("<<")]
    MuchLess,  // 远小于
}
```

### 实现示例

```csharp
case CompareMode.ApproximatelyEqual:
    return Mathf.Abs(left - right) < 0.0001f;

case CompareMode.MuchGreater:
    return left - right > 10f;  // 差值大于 10

case CompareMode.MuchLess:
    return right - left > 10f;
```

---

## 相关文档

- [DecisionCompareNode.cs.md](./DecisionCompareNode.cs.md) - 使用 CompareMode 的节点
- [DecisionConditionNode.cs.md](./DecisionConditionNode.cs.md) - 条件判断节点
- [DecisionActionNode.cs.md](./DecisionActionNode.cs.md) - 执行动作节点

---

*文档生成时间：2026-02-28 | OpenClaw AI 助手*
