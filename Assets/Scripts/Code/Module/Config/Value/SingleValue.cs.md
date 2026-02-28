# SingleValue.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | SingleValue.cs |
| **路径** | Assets/Scripts/Code/Module/Config/Value/SingleValue.cs |
| **所属模块** | 框架层 → Code/Module/Config/Value |
| **文件职责** | 定义固定值类型，返回预设的浮点数值 |

---

## 类/结构体说明

### SingleValue

| 属性 | 说明 |
|------|------|
| **职责** | BaseValue 的最简单实现，始终返回固定的浮点数值 |
| **泛型参数** | 无 |
| **继承关系** | 继承 `BaseValue` |
| **实现的接口** | 无 |

**设计模式**: 常量模式

```csharp
// 创建固定值
var value = new SingleValue { Value = 50f };
float result = value.Resolve(knowledge);  // 始终返回 50
```

---

## 字段与属性

### Value

| 属性 | 值 |
|------|------|
| **类型** | `float` |
| **访问级别** | `public` |
| **默认值** | `0` |
| **说明** | 固定的浮点数值 |

**Nino 序列化**: `[NinoMember(1)]`

---

## 构造函数

### 无参构造函数

```csharp
public SingleValue()
{
    Value = 0;
}
```

**用途**: 创建默认值为 0 的固定值

---

### 有参构造函数

```csharp
public SingleValue(float val)
{
    Value = val;
}
```

**用途**: 创建指定值的固定值

**使用示例**:
```csharp
var value = new SingleValue(50f);  // 创建值为 50 的固定值
```

---

## 方法说明

### Resolve

**签名**:
```csharp
public override float Resolve(AIKnowledge knowledge)
```

**职责**: 返回固定的 Value 值（忽略 knowledge 参数）

**核心逻辑**:
```
1. 直接返回 Value 字段
2. 不依赖任何外部状态
```

**调用者**: DecisionCompareNode, DecisionActionNode, 等

**参数**:
| 参数名 | 类型 | 说明 |
|--------|------|------|
| `knowledge` | `AIKnowledge` | AI 知识库（未使用） |

**返回值**: `float` - 固定的 Value 值

---

## Nino 序列化特性

### NinoType

```csharp
[NinoType(false)]
```

**说明**: 标记为 Nino 可序列化类型。

### NinoMember

```csharp
[NinoMember(1)]  // Value
```

**说明**: 显式指定成员序列化顺序。

---

## 使用示例

### 示例 1: 基础使用

```csharp
// 创建固定值
var fixedValue = new SingleValue { Value = 100f };

// 在比较节点中使用
var compareNode = new DecisionCompareNode
{
    LeftValue = new SingleValue { Value = 50f },
    CompareMode = CompareMode.Greater,
    RightValue = new SingleValue { Value = 30f },
    True = new DecisionActionNode { Act = ActDecision.Action_Punch },
    False = new DecisionActionNode { Act = ActDecision.Action_Run }
};
```

### 示例 2: 使用构造函数

```csharp
// 使用构造函数创建
var healthThreshold = new SingleValue(30f);
var distanceThreshold = new SingleValue(5f);

// 在配置中使用
var config = new DecisionCompareNode
{
    LeftValue = healthThreshold,
    CompareMode = CompareMode.Greater,
    RightValue = new SingleValue(30f)
};
```

### 示例 3: 在 Action 节点中使用

```csharp
// 延迟时间使用固定值
var actionNode = new DecisionActionNode
{
    Act = ActDecision.Action_Punch,
    Tactic = AITactic.HighWeight,
    Delay = new SingleValue(1000f)  // 固定延迟 1 秒
};
```

---

## 与其他 Value 类型对比

| 类型 | 返回值 | 使用场景 |
|------|--------|----------|
| `SingleValue` | 固定值 | 阈值、常量、固定延迟 |
| `ZeroValue` | 始终 0 | 占位、重置 |
| `Range01Value` | 随机 0-1 | 概率、随机因子 |
| `FormulaValue` | 公式计算 | 复杂数值关系 |
| `OperatorValue` | 运算结果 | 加减乘除等运算 |
| `RandomAuctionTime` | 配置随机时间 | 拍卖出价延迟 |

---

## 设计要点

### 为什么需要 SingleValue？

1. **最简单**: BaseValue 的最基础实现
2. **默认选择**: 大多数情况下的首选
3. **性能最优**: 无需计算，直接返回
4. **配置友好**: 策划容易理解和使用

### 忽略 knowledge 参数

```csharp
public override float Resolve(AIKnowledge knowledge)
{
    return Value;  // 完全忽略 knowledge
}
```

**原因**:
- 固定值不依赖任何外部状态
- 保持接口一致性（所有 BaseValue 子类都有此方法）
- 未来可以扩展为依赖 knowledge

---

## 相关文档

- [BaseValue.cs.md](./BaseValue.cs.md) - 值类型基类
- [ZeroValue.cs.md](./ZeroValue.cs.md) - 零值实现
- [Range01Value.cs.md](./Range01Value.cs.md) - 随机 0-1 值
- [FormulaValue.cs.md](./FormulaValue.cs.md) - 公式值
- [DecisionCompareNode.cs.md](../DecisionTree/DecisionCompareNode.cs.md) - 使用 Value 的比较节点

---

*文档生成时间：2026-02-28 | OpenClaw AI 助手*
