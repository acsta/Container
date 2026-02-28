# FormulaValue.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|------|
| **文件名** | FormulaValue.cs |
| **路径** | Assets/Scripts/Code/Module/Config/Value/FormulaValue.cs |
| **所属模块** | 框架层 → Code/Module/Config/Value |
| **文件职责** | 定义公式运算值类型，通过公式字符串计算复杂数值 |

---

## 类/结构体说明

### FormulaValue

| 属性 | 说明 |
|------|------|
| **职责** | BaseValue 的公式实现，根据 Formula 字符串和 NumericComponent 计算数值 |
| **泛型参数** | 无 |
| **继承关系** | 继承 `BaseValue` |
| **实现的接口** | 无 |

**设计模式**: 解释器模式

```csharp
// 创建公式值
var formula = new FormulaValue { Formula = "HP * 0.3 + STR * 2" };
float result = formula.Resolve(knowledge);  // 根据 NumericComponent 计算
```

---

## 字段与属性

### Formula

| 属性 | 值 |
|------|------|
| **类型** | `string` |
| **访问级别** | `public` |
| **说明** | 公式字符串，定义数值计算逻辑 |

**Nino 序列化**: `[NinoMember(1)]`

**公式语法**: 由 `FormulaStringFx` 类解析和支持

**示例公式**:
- `"HP"` - 直接返回 HP 值
- `"HP * 0.3"` - HP 的 30%
- `"STR * 2 + INT"` - STR 的 2 倍加 INT
- `"Level * 10 + EXP / 100"` - 等级相关计算

---

## 方法说明

### Resolve

**签名**:
```csharp
public override float Resolve(AIKnowledge knowledge)
```

**职责**: 根据 Formula 字符串和 NumericComponent 计算数值

**核心逻辑**:
```
1. 从 knowledge.Entity 获取 NumericComponent
2. 如果组件存在:
   - 调用 FormulaStringFx.Get(Formula) 获取公式解析器
   - 调用 GetData(numc) 计算结果
   - 返回计算结果
3. 如果组件不存在:
   - 记录错误日志
   - 返回 0
```

**调用者**: DecisionCompareNode, DecisionActionNode, 等

**参数**:
| 参数名 | 类型 | 说明 |
|--------|------|------|
| `knowledge` | `AIKnowledge` | AI 知识库，包含 Entity 引用 |

**返回值**: `float` - 公式计算结果

**异常处理**: 如果 NumericComponent 不存在，记录错误并返回 0。

---

## 依赖

### NumericComponent

```csharp
var numc = knowledge.Entity.GetComponent<NumericComponent>();
```

**说明**: 数值组件，存储实体的所有数值属性（HP、STR、INT 等）

### FormulaStringFx

```csharp
return FormulaStringFx.Get(Formula).GetData(numc);
```

**说明**: 公式字符串解析器，负责解析和计算公式

---

## Nino 序列化特性

### NinoType

```csharp
[NinoType(false)]
```

**说明**: 标记为 Nino 可序列化类型。

### NinoMember

```csharp
[NinoMember(1)]  // Formula
```

**说明**: 显式指定成员序列化顺序。

---

## 使用示例

### 示例 1: 基础公式

```csharp
// 血量阈值 = 最大血量的 30%
var healthThreshold = new FormulaValue
{
    Formula = "MaxHP * 0.3"
};

var compareNode = new DecisionCompareNode
{
    LeftValue = new CurrentHPValue(),
    CompareMode = CompareMode.Less,
    RightValue = healthThreshold,
    True = new DecisionActionNode 
    { 
        Act = ActDecision.Action_Run,
        Remarks = "血量低于 30%，逃跑"
    },
    False = new DecisionActionNode { Act = ActDecision.Action_Punch }
};
```

### 示例 2: 复合公式

```csharp
// 攻击力 = 力量 * 2 + 等级 * 5
var damageFormula = new FormulaValue
{
    Formula = "STR * 2 + Level * 5"
};

var actionNode = new DecisionActionNode
{
    Act = ActDecision.Action_Punch,
    Delay = new SingleValue(500f),
    // 伤害计算使用公式
    // (实际伤害计算可能在其他地方使用此公式)
};
```

### 示例 3: 复杂公式

```csharp
// 拍卖出价 = 当前价 * (1 + VIP 等级 * 0.1) + 金币 / 1000
var bidFormula = new FormulaValue
{
    Formula = "CurrentPrice * (1 + VIP * 0.1) + Gold / 1000"
};

var bidDecision = new DecisionCompareNode
{
    LeftValue = new CurrentMoneyValue(),
    CompareMode = CompareMode.Greater,
    RightValue = bidFormula,
    True = new DecisionActionNode 
    { 
        Act = ActDecision.Emoji_Nice,
        Tactic = AITactic.LowWeight,
        Remarks = "资金充足，可以出价"
    },
    False = new DecisionActionNode 
    { 
        Act = ActDecision.Emoji_Sigh,
        Tactic = AITactic.Sidelines,
        Remarks = "资金不足，观望"
    }
};
```

---

## 公式语法

### 支持的运算符

根据 `FormulaStringFx` 的实现，可能支持：

| 运算符 | 说明 | 示例 |
|--------|------|------|
| `+` | 加法 | `A + B` |
| `-` | 减法 | `A - B` |
| `*` | 乘法 | `A * B` |
| `/` | 除法 | `A / B` |
| `%` | 取余 | `A % B` |
| `^` 或 `Pow` | 次方 | `A ^ 2` |
| `()` | 括号 | `(A + B) * C` |

### 支持的数值类型

取决于 `NumericComponent` 中定义的数值：

- `HP` - 当前血量
- `MaxHP` - 最大血量
- `STR` - 力量
- `INT` - 智力
- `Level` - 等级
- `EXP` - 经验值
- `Gold` - 金币
- `VIP` - VIP 等级
- 等等...

---

## 错误处理

### NumericComponent 不存在

```csharp
if (numc != null)
{
    return FormulaStringFx.Get(Formula).GetData(numc);
}
Log.Error($"获取{Formula}时，未找到 NumericComponent 组件");
return 0;
```

**说明**: 如果 Entity 没有 NumericComponent，记录错误并返回 0。

**调试建议**:
1. 检查 Entity 是否正确添加了 NumericComponent
2. 检查 knowledge.Entity 是否正确设置
3. 在编辑器中验证配置

---

## 设计要点

### 为什么使用字符串公式？

1. **配置友好**: 策划可以直接编辑公式
2. **灵活性**: 支持任意复杂计算
3. **可扩展**: 新增数值类型无需修改代码
4. **可维护**: 公式集中管理，易于调试

### 与 OperatorValue 对比

| 特性 | FormulaValue | OperatorValue |
|------|--------------|---------------|
| **配置方式** | 字符串公式 | 树形结构 |
| **复杂度** | 适合复杂公式 | 适合简单运算 |
| **可读性** | 高（类似数学） | 中（嵌套结构） |
| **性能** | 需解析字符串 | 直接计算 |
| **推荐场景** | 复杂公式 | 简单运算 |

---

## 相关文档

- [BaseValue.cs.md](./BaseValue.cs.md) - 值类型基类
- [OperatorValue.cs.md](./OperatorValue.cs.md) - 运算符值
- [NumericComponent.cs.md](../../Game/Numeric/NumericComponent.cs.md) - 数值组件
- [FormulaStringFx.cs.md](../../Helper/FormulaStringFx.cs.md) - 公式解析器

---

*文档生成时间：2026-02-28 | OpenClaw AI 助手*
