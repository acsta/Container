# LogicMode.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|------|
| **文件名** | LogicMode.cs |
| **路径** | Assets/Scripts/Code/Module/Config/Value/LogicMode.cs |
| **所属模块** | 框架层 → Code/Module/Config/Value |
| **文件职责** | 定义逻辑运算模式枚举，用于 OperatorValue |

---

## 类/结构体说明

### LogicMode 枚举

| 属性 | 说明 |
|------|------|
| **职责** | 定义 OperatorValue 支持的所有运算类型 |
| **泛型参数** | 无 |
| **继承关系** | 继承 `System.Enum` |
| **实现的接口** | 无 |

**用途**:
- 在 `OperatorValue.Op` 字段中使用
- 策划通过下拉菜单选择运算类型
- 运行时决定执行哪种运算

---

## 枚举值详解

| 值 | 枚举名 | 中文标签 | 运算符 | 说明 |
|----|--------|----------|--------|------|
| 0 | `Default` | 无 | - | 无运算，直接返回 Left |
| 1 | `Add` | 加 | `+` | 加法运算 |
| 2 | `Red` | 减 | `-` | 减法运算 |
| 3 | `Mul` | 乘 | `*` | 乘法运算 |
| 4 | `Div` | 除 | `/` | 除法运算 |
| 5 | `Rem` | 取余 | `%` | 取余运算 |
| 6 | `Pow` | 次方 | `^` | 次方运算 |

---

## Odin Inspector 集成

### LabelText 特性

```csharp
public enum LogicMode
{
    [LabelText("无")]
    Default,
    
    [LabelText("加")]
    Add,
    
    [LabelText("减")]
    Red,
    
    [LabelText("乘")]
    Mul,
    
    [LabelText("除")]
    Div,
    
    [LabelText("取余")]
    Rem,
    
    [LabelText("次方")]
    Pow,
}
```

**效果**: Inspector 中显示中文标签，策划更易理解

**Inspector 显示**:
```
运算类型：[加 ▼]
```

---

## 运算真值表

### 示例输入
- Left = 10
- Right = 3

### 运算结果

| LogicMode | 计算 | 结果 |
|-----------|------|------|
| `Default` | Left | 10 |
| `Add` | 10 + 3 | 13 |
| `Red` | 10 - 3 | 7 |
| `Mul` | 10 * 3 | 30 |
| `Div` | 10 / 3 | 3.333... |
| `Rem` | 10 % 3 | 1 |
| `Pow` | 10 ^ 3 | 1000 |

---

## 使用示例

### 示例 1: 加法

```csharp
// 总血量 = 基础血量 + 装备加成
var totalHP = new OperatorValue
{
    Left = new BaseHPValue(),
    Op = LogicMode.Add,
    Right = new EquipmentHPBonusValue()
};
```

### 示例 2: 减法

```csharp
// 剩余生命 = 当前生命 - 伤害
var remainingHP = new OperatorValue
{
    Left = new CurrentHPValue(),
    Op = LogicMode.Red,
    Right = new DamageValue()
};
```

### 示例 3: 乘法

```csharp
// 最终伤害 = 基础伤害 * 暴击系数
var finalDamage = new OperatorValue
{
    Left = new BaseDamageValue(),
    Op = LogicMode.Mul,
    Right = new CritMultiplierValue()
};
```

### 示例 4: 除法

```csharp
// 伤害减免比例 = 防御力 / (防御力 + 100)
var damageReduction = new OperatorValue
{
    Left = new DefenseValue(),
    Op = LogicMode.Div,
    Right = new OperatorValue
    {
        Left = new DefenseValue(),
        Op = LogicMode.Add,
        Right = new SingleValue(100f)
    }
};
```

### 示例 5: 取余

```csharp
// 循环索引 = 当前索引 % 总数
var loopIndex = new OperatorValue
{
    Left = new CurrentIndexValue(),
    Op = LogicMode.Rem,
    Right = new TotalCountValue()
};
```

### 示例 6: 次方

```csharp
// 升级经验 = 等级 ^ 2 * 100
var levelUpExp = new OperatorValue
{
    Left = new OperatorValue
    {
        Left = new LevelValue(),
        Op = LogicMode.Pow,
        Right = new SingleValue(2f)
    },
    Op = LogicMode.Mul,
    Right = new SingleValue(100f)
};
```

---

## 命名说明

### Red vs Sub

```csharp
[LabelText("减")]
Red,  // 而非 Sub
```

**说明**: 枚举名为 `Red` 而非更常见的 `Sub` (Subtract)

**可能原因**:
- 开发团队命名习惯
- 避免与某些关键字冲突
- 无特殊原因，历史遗留

**建议**: 保持命名一致性，如果重构可考虑改为 `Sub`

---

## 扩展建议

### 可能的扩展运算符

```csharp
public enum LogicMode
{
    // ... 现有值
    
    [LabelText("绝对值")]
    Abs,       // 绝对值（只需要 Left）
    
    [LabelText("向下取整")]
    Floor,     // 向下取整
    
    [LabelText("向上取整")]
    Ceil,      // 向上取整
    
    [LabelText("四舍五入")]
    Round,     // 四舍五入
    
    [LabelText("最小值")]
    Min,       // Min(Left, Right)
    
    [LabelText("最大值")]
    Max,       // Max(Left, Right)
    
    [LabelText("平均值")]
    Average,   // (Left + Right) / 2
}
```

### 实现示例

```csharp
public override float Resolve(AIKnowledge knowledge)
{
    switch (Op)
    {
        // ... 现有 case
        
        case LogicMode.Abs:
            return Mathf.Abs(Left.Resolve(knowledge));
        
        case LogicMode.Floor:
            return Mathf.Floor(Left.Resolve(knowledge));
        
        case LogicMode.Ceil:
            return Mathf.Ceil(Left.Resolve(knowledge));
        
        case LogicMode.Min:
            return Mathf.Min(Left.Resolve(knowledge), Right.Resolve(knowledge));
        
        case LogicMode.Max:
            return Mathf.Max(Left.Resolve(knowledge), Right.Resolve(knowledge));
        
        case LogicMode.Average:
            return (Left.Resolve(knowledge) + Right.Resolve(knowledge)) / 2f;
    }
}
```

---

## 设计要点

### 为什么使用枚举？

1. **类型安全**: 避免字符串拼写错误
2. **性能**: 整数 switch 比字符串解析快
3. **编辑器友好**: 下拉菜单选择
4. **完整性**: 集中管理所有运算符
5. **可扩展**: 新增运算符只需添加枚举值

### Default 的特殊性

```csharp
case LogicMode.Default:
    return Left.Resolve(knowledge);
```

**说明**: Default 不使用 Right 操作数

**用途**:
- 包装单个值（统一接口）
- 作为默认/初始状态
- 条件禁用右侧操作数

### 除零处理

**现状**:
- `Rem` 有除零保护（返回 Left）
- `Div` 没有除零保护

**建议**: 为 `Div` 添加保护
```csharp
case LogicMode.Div:
    float right = Right.Resolve(knowledge);
    if (right == 0)
    {
        Log.Warning("LogicMode.Div: 除零错误");
        return 0;  // 或 float.MaxValue
    }
    return Left.Resolve(knowledge) / right;
```

---

## 与 CompareMode 对比

| 特性 | LogicMode | CompareMode |
|------|-----------|-------------|
| **用途** | 数值运算 | 数值比较 |
| **返回值** | float | bool（通过 DecisionCompareNode） |
| **使用位置** | OperatorValue | DecisionCompareNode |
| **操作数** | 2 个（Left + Right） | 2 个（Left + Right） |
| **数量** | 7 种 | 6 种 |

---

## 相关文档

- [OperatorValue.cs.md](./OperatorValue.cs.md) - 使用 LogicMode 的运算值
- [BaseValue.cs.md](./BaseValue.cs.md) - 值类型基类
- [DecisionCompareNode.cs.md](../DecisionTree/DecisionCompareNode.cs.md) - 使用 CompareMode 的比较节点
- [CompareMode.cs.md](../DecisionTree/CompareMode.cs.md) - 比较模式枚举

---

*文档生成时间：2026-02-28 | OpenClaw AI 助手*
