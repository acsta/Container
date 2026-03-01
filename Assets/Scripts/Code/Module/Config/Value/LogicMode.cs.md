# LogicMode.cs 文档

## 📄 文件信息表

| 属性 | 值 |
|------|------|
| 文件路径 | `Assets/Scripts/Code/Module/Config/Value/LogicMode.cs` |
| 命名空间 | `TaoTie` |
| 类类型 | 枚举 |
| 依赖模块 | Sirenix.OdinInspector |

---

## 🏗️ 类说明

**LogicMode** 枚举定义了 `OperatorValue` 支持的数学运算类型。

### 核心职责

- 定义二元运算的操作类型
- 提供 Odin Inspector 的友好显示标签
- 用于配置表中的运算选择

---

## 📊 枚举值

| 值 | 名称 | 标签 | 运算 | 示例 |
|----|------|------|------|------|
| 0 | `Default` | 无 | 返回左值 | Left |
| 1 | `Add` | 加 | Left + Right | 5 + 3 = 8 |
| 2 | `Red` | 减 | Left - Right | 5 - 3 = 2 |
| 3 | `Mul` | 乘 | Left * Right | 5 * 3 = 15 |
| 4 | `Div` | 除 | Left / Right | 6 / 3 = 2 |
| 5 | `Rem` | 取余 | Left % Right | 7 % 3 = 1 |
| 6 | `Pow` | 次方 | Left ^ Right | 2 ^ 3 = 8 |

---

## 🔄 Mermaid 流程图

### 运算选择流程

```mermaid
flowchart TD
    A[OperatorValue.Resolve] --> B{Op 值}
    B -->|Default| C[返回 Left]
    B -->|Add| D[Left + Right]
    B -->|Red| E[Left - Right]
    B -->|Mul| F[Left * Right]
    B -->|Div| G[Left / Right]
    B -->|Rem| H[Left % Right]
    B -->|Pow| I[(int)pow Left, Right]
```

---

## 💡 使用示例

### 在 OperatorValue 中使用

```csharp
// 加法
var addOp = new OperatorValue
{
    Left = new SingleValue(5),
    Op = LogicMode.Add,
    Right = new SingleValue(3)
};
float result = addOp.Resolve(knowledge);  // 8

// 乘法
var mulOp = new OperatorValue
{
    Left = new SingleValue(5),
    Op = LogicMode.Mul,
    Right = new SingleValue(3)
};
float result = mulOp.Resolve(knowledge);  // 15

// 次方
var powOp = new OperatorValue
{
    Left = new SingleValue(2),
    Op = LogicMode.Pow,
    Right = new SingleValue(3)
};
float result = powOp.Resolve(knowledge);  // 8 (int)
```

### 在配置表中使用

```yaml
# ConfigAIDecisionTree 配置示例
Type: "CalculatorAI"
Node:
  Type: DecisionActionNode
  Tactic: Sidelines
  Delay:
    Type: OperatorValue
    Op: Mul  # 乘法
    Left:
      Type: FormulaValue
      Formula: "BaseDelay"
    Right:
      Type: SingleValue
      Value: 1.5  # 1.5 倍延迟
```

### 遍历所有运算模式

```csharp
// 测试所有运算
float left = 10;
float right = 3;

foreach (LogicMode mode in Enum.GetValues(typeof(LogicMode)))
{
    var op = new OperatorValue
    {
        Left = new SingleValue(left),
        Op = mode,
        Right = new SingleValue(right)
    };
    
    float result = op.Resolve(knowledge);
    Log.Info($"{mode}: {left} op {right} = {result}");
}

// 输出:
// Default: 10 op 3 = 10
// Add: 10 op 3 = 13
// Red: 10 op 3 = 7
// Mul: 10 op 3 = 30
// Div: 10 op 3 = 3.333...
// Rem: 10 op 3 = 1
// Pow: 10 op 3 = 1000 (int)
```

---

## 📝 Odin Inspector 显示

在 Unity 编辑器中，枚举值会显示为友好的中文标签：

```
[下拉选择]
├─ 无 (Default)
├─ 加 (Add)
├─ 减 (Red)
├─ 乘 (Mul)
├─ 除 (Div)
├─ 取余 (Rem)
└─ 次方 (Pow)
```

---

## ⚠️ 注意事项

### 除法精度

```csharp
// 除法返回 float，保留小数
var divOp = new OperatorValue
{
    Left = new SingleValue(10),
    Op = LogicMode.Div,
    Right = new SingleValue(3)
};
float result = divOp.Resolve(knowledge);  // 3.333333...
```

### 次方取整

```csharp
// Pow 运算结果转换为 int
var powOp = new OperatorValue
{
    Left = new SingleValue(2.5f),
    Op = LogicMode.Pow,
    Right = new SingleValue(2)
};
float result = powOp.Resolve(knowledge);  // 6 (2.5^2 = 6.25 → 6)
```

### 取余除零保护

```csharp
// Rem 运算在除数为 0 时返回被除数
var remOp = new OperatorValue
{
    Left = new SingleValue(10),
    Op = LogicMode.Rem,
    Right = new ZeroValue()
};
float result = remOp.Resolve(knowledge);  // 10 (不抛出异常)
```

---

## 🔗 相关文档链接

- [OperatorValue.cs.md](./OperatorValue.cs.md) - 运算值类
- [BaseValue.cs.md](./BaseValue.cs.md) - 值基类
- [SingleValue.cs.md](./SingleValue.cs.md) - 固定值

---

*最后更新：2026-03-02*
