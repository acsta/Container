# ZeroValue.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|------|
| **文件名** | ZeroValue.cs |
| **路径** | Assets/Scripts/Code/Module/Config/Value/ZeroValue.cs |
| **所属模块** | 框架层 → Code/Module/Config/Value |
| **文件职责** | 定义零值类型，始终返回 0 |

---

## 类/结构体说明

### ZeroValue

| 属性 | 说明 |
|------|------|
| **职责** | BaseValue 的特殊实现，始终返回 0，无需存储任何字段 |
| **泛型参数** | 无 |
| **继承关系** | 继承 `BaseValue` |
| **实现的接口** | 无 |

**设计模式**: 单例模式（隐式）+ 常量模式

```csharp
// 创建零值
var zero = new ZeroValue();
float result = zero.Resolve(knowledge);  // 始终返回 0
```

---

## 字段与属性

ZeroValue **没有字段**，因为它的值始终为 0。

**优势**:
- 序列化后体积最小
- 无需配置任何参数
- 语义清晰（明确表示"零"）

---

## 方法说明

### Resolve

**签名**:
```csharp
public override float Resolve(AIKnowledge knowledge)
```

**职责**: 始终返回 0

**核心逻辑**:
```
1. 直接返回 0
2. 不依赖任何字段或外部状态
```

**调用者**: DecisionCompareNode, DecisionActionNode, 等

**参数**:
| 参数名 | 类型 | 说明 |
|--------|------|------|
| `knowledge` | `AIKnowledge` | AI 知识库（未使用） |

**返回值**: `float` - 始终为 0

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

### 示例 1: 作为占位值

```csharp
// 作为默认值或占位符
var compareNode = new DecisionCompareNode
{
    LeftValue = new SingleValue { Value = 50f },
    CompareMode = CompareMode.Greater,
    RightValue = new ZeroValue(),  // 与 0 比较
    True = new DecisionActionNode { Act = ActDecision.Action_Punch },
    False = new DecisionActionNode { Act = ActDecision.Stand_Idle1 }
};
```

### 示例 2: 重置操作

```csharp
// 将某个值重置为 0
var resetOperation = new OperatorValue
{
    Left = new CurrentValue(),
    Op = LogicMode.Add,
    Right = new ZeroValue()  // 加 0，保持不变（或用于重置）
};
```

### 示例 3: 条件判断

```csharp
// 检查值是否大于 0
var positiveCheck = new DecisionCompareNode
{
    LeftValue = new SomeValue(),
    CompareMode = CompareMode.Greater,
    RightValue = new ZeroValue(),  // 是否大于 0
    True = new DecisionActionNode { Act = ActDecision.Emoji_Nice },
    False = new DecisionActionNode { Act = ActDecision.Emoji_Sigh }
};
```

---

## 与 SingleValue(0) 对比

| 特性 | ZeroValue | SingleValue(0) |
|------|-----------|----------------|
| **序列化大小** | 最小（无字段） | 4 字节（float） |
| **语义清晰度** | 清晰（就是 0） | 较清晰 |
| **配置复杂度** | 最简单 | 需配置 Value=0 |
| **性能** | 相同 | 相同 |
| **推荐使用** | ✅ 优先 | 可用 |

**建议**: 当需要表示"零"时，优先使用 `ZeroValue`。

---

## 设计要点

### 为什么需要专门的 ZeroValue？

1. **语义清晰**: 明确表示"零"的意图
2. **节省空间**: 无需存储 Value 字段
3. **配置简单**: 策划无需填写任何值
4. **性能优化**: 虽然差异微小，但确实更快

### 不实现构造函数

```csharp
// 没有构造函数，使用默认即可
public partial class ZeroValue: BaseValue
{
    // 无构造函数
}
```

**原因**: 无需初始化任何字段

---

## 相关文档

- [BaseValue.cs.md](./BaseValue.cs.md) - 值类型基类
- [SingleValue.cs.md](./SingleValue.cs.md) - 固定值实现
- [OperatorValue.cs.md](./OperatorValue.cs.md) - 运算符值（可使用 ZeroValue 作为操作数）

---

*文档生成时间：2026-02-28 | OpenClaw AI 助手*
