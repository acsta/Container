# Range01Value.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|------|
| **文件名** | Range01Value.cs |
| **路径** | Assets/Scripts/Code/Module/Config/Value/Range01Value.cs |
| **所属模块** | 框架层 → Code/Module/Config/Value |
| **文件职责** | 定义随机 0-1 值类型，每次解析返回随机数 |

---

## 类/结构体说明

### Range01Value

| 属性 | 说明 |
|------|------|
| **职责** | BaseValue 的随机实现，每次 Resolve 调用返回 0-1 之间的随机数 |
| **泛型参数** | 无 |
| **继承关系** | 继承 `BaseValue` |
| **实现的接口** | 无 |

**设计模式**: 随机模式

```csharp
// 创建随机值
var randomValue = new Range01Value();
float result1 = randomValue.Resolve(knowledge);  // 如 0.347
float result2 = randomValue.Resolve(knowledge);  // 如 0.812（不同）
```

---

## 字段与属性

Range01Value **没有字段**，因为它是纯随机生成。

---

## 方法说明

### Resolve

**签名**:
```csharp
public override float Resolve(AIKnowledge knowledge)
```

**职责**: 返回 0-1 之间的随机浮点数

**核心逻辑**:
```
1. 调用 UnityEngine.Random.Range(0f, 1f)
2. 返回随机数（包含 0，不包含 1）
```

**调用者**: DecisionCompareNode, DecisionActionNode, 等

**参数**:
| 参数名 | 类型 | 说明 |
|--------|------|------|
| `knowledge` | `AIKnowledge` | AI 知识库（未使用） |

**返回值**: `float` - 0-1 之间的随机数 [0, 1)

**分布特性**:
- 最小值：0.0（可能）
- 最大值：接近 1.0（如 0.9999）
- 平均值：0.5
- 分布：均匀分布

---

## 依赖

### UnityEngine.Random

```csharp
using UnityEngine;

// 使用 Unity 的随机数生成器
Random.Range(0f, 1f)
```

**说明**: 使用 Unity 内置的随机数生成器，确保跨平台一致性。

---

## Nino 序列化特性

### NinoType

```csharp
[NinoType(false)]
```

**说明**: 标记为 Nino 可序列化类型。

**序列化结果**: 由于没有字段，序列化后几乎不占空间。

---

## 使用示例

### 示例 1: 概率判断

```csharp
// 50% 概率执行某个动作
var probabilityCheck = new DecisionCompareNode
{
    LeftValue = new Range01Value(),  // 随机 0-1
    CompareMode = CompareMode.Less,
    RightValue = new SingleValue { Value = 0.5f },  // 阈值 0.5
    True = new DecisionActionNode 
    { 
        Act = ActDecision.Action_Punch,
        Remarks = "50% 概率攻击"
    },
    False = new DecisionActionNode 
    { 
        Act = ActDecision.Stand_Idle1,
        Remarks = "50% 概率待机"
    }
};
```

### 示例 2: 随机延迟

```csharp
// 随机延迟（0-1 秒）
var actionNode = new DecisionActionNode
{
    Act = ActDecision.Action_Punch,
    Delay = new Range01Value()  // 随机 0-1 秒
};
```

### 示例 3: 随机因子

```csharp
// 攻击力 = 基础值 * 随机因子 (0.8-1.2)
var damageFormula = new OperatorValue
{
    Left = new BaseDamageValue(),
    Op = LogicMode.Mul,
    Right = new OperatorValue
    {
        Left = new SingleValue { Value = 0.4f },  // 0.4
        Op = LogicMode.Add,
        Right = new OperatorValue
        {
            Left = new Range01Value(),  // 0-1
            Op = LogicMode.Mul,
            Right = new SingleValue { Value = 0.8f }  // *0.8 = 0-0.8
        }  // 0.4 + 0-0.8 = 0.4-1.2
    }
};
```

### 示例 4: 多重随机判断

```csharp
// 三重随机判断
var randomDecision = new ConfigAIDecisionTree
{
    Type = "RandomTest",
    Node = new DecisionCompareNode
    {
        LeftValue = new Range01Value(),
        CompareMode = CompareMode.Less,
        RightValue = new SingleValue { Value = 0.33f },
        True = new DecisionActionNode 
        { 
            Act = ActDecision.Dance_1,
            Remarks = "33% 概率跳舞 1"
        },
        False = new DecisionCompareNode
        {
            LeftValue = new Range01Value(),
            CompareMode = CompareMode.Less,
            RightValue = new SingleValue { Value = 0.5f },  // 在剩余 67% 中的 50%
            True = new DecisionActionNode 
            { 
                Act = ActDecision.Dance_2,
                Remarks = "约 33% 概率跳舞 2"
            },
            False = new DecisionActionNode 
            { 
                Act = ActDecision.Dance_3,
                Remarks = "约 34% 概率跳舞 3"
            }
        }
    }
};
```

---

## 随机数特性

### 均匀分布

```
概率密度
  ^
  |
1 |─────────────────
  |                 │
  |                 │
  |                 │
0 +─────────────────┴──> 值
  0                 1
```

**说明**: 每个值出现的概率相等

### 多次调用结果示例

```csharp
var random = new Range01Value();

random.Resolve(knowledge);  // 0.347821
random.Resolve(knowledge);  // 0.812453
random.Resolve(knowledge);  // 0.156789
random.Resolve(knowledge);  // 0.923456
random.Resolve(knowledge);  // 0.501234
```

---

## 与 Unity Random.value 对比

| 特性 | Range01Value | Random.value |
|------|--------------|--------------|
| **功能** | 相同 | 相同 |
| **使用场景** | 配置系统 | 代码中 |
| **可序列化** | ✅ 是 | ❌ 否 |
| **可配置** | ✅ 是 | ❌ 否 |
| **推荐** | 配置中使用 | 代码中使用 |

---

## 设计要点

### 为什么需要 Range01Value？

1. **配置友好**: 策划可以在配置表中使用随机值
2. **可序列化**: 可以保存到配置文件
3. **语义清晰**: 明确表示"随机 0-1"
4. **可组合**: 可以与其他 Value 类型组合使用

### 不存储种子

```csharp
// 没有种子字段
public partial class Range01Value: BaseValue
{
    // 每次调用都使用 Unity 的全局随机数生成器
}
```

**原因**:
- 简化配置
- 依赖 Unity 的随机数系统
- 如需可重复随机，应使用带种子的实现

---

## 扩展建议

### 带范围的随机值

```csharp
[NinoType(false)]
public partial class RangeValue: BaseValue
{
    [NinoMember(1)]
    public float Min;
    
    [NinoMember(2)]
    public float Max;
    
    public override float Resolve(AIKnowledge knowledge)
    {
        return Random.Range(Min, Max);
    }
}

// 使用
var damage = new RangeValue { Min = 10f, Max = 20f };
```

### 带种子的随机值

```csharp
[NinoType(false)]
public partial class SeededRandomValue: BaseValue
{
    [NinoMember(1)]
    public int Seed;
    
    private System.Random random;
    
    public override float Resolve(AIKnowledge knowledge)
    {
        if (random == null)
            random = new System.Random(Seed);
        
        return (float)random.NextDouble();  // 0-1
    }
}
```

---

## 相关文档

- [BaseValue.cs.md](./BaseValue.cs.md) - 值类型基类
- [SingleValue.cs.md](./SingleValue.cs.md) - 固定值实现
- [ZeroValue.cs.md](./ZeroValue.cs.md) - 零值实现
- [OperatorValue.cs.md](./OperatorValue.cs.md) - 运算符值
- [Unity Random 文档](https://docs.unity3d.com/ScriptReference/Random.html)

---

*文档生成时间：2026-02-28 | OpenClaw AI 助手*
