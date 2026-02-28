# BigNumber.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | BigNumber.cs |
| **路径** | Assets/Scripts/Mono/Core/Object/BigNumber.cs |
| **所属模块** | Mono/Core/Object |
| **文件职责** | 高精度大数运算，支持超大整数和浮点数，用于游戏数值系统 |
| **代码行数** | ~1227 行 |

---

## 类/结构体说明

### BigNumber

| 属性 | 说明 |
|------|------|
| **职责** | 表示任意精度的数字，支持整数和浮点数，用于突破标准数值类型的范围限制 |
| **泛型参数** | 无 |
| **继承关系** | `sealed class`（不可继承） |
| **实现的接口** | 无（但实现了完整的运算符重载） |

**设计模式**: 值对象模式 + 字符串存储

```csharp
// 创建大数
var num = new BigNumber("123456789012345678901234567890");
var num2 = BigNumber.Parse("999999999999999999999999");

// 运算
var sum = num + num2;
var product = num * 2;

// 比较
if (num > num2) { }

// 格式化
string str = num.ToString();
string fixed2 = num.ToString(2); // 保留 2 位小数
```

---

## 常量与静态字段

| 名称 | 类型 | 说明 |
|------|------|------|
| `PRECISION` | `ushort` | 精度设置，默认 32 |
| `Zero` | `BigNumber` | 零值常量 |
| `FloatMax` | `BigNumber` | float 最大值 |
| `FloatMin` | `BigNumber` | float 最小值 |
| `DoubleMax` | `BigNumber` | double 最大值 |
| `DoubleMin` | `BigNumber` | double 最小值 |
| `LongMax` | `BigNumber` | long 最大值 |
| `LongMin` | `BigNumber` | long 最小值 |
| `DecimalMax` | `BigNumber` | decimal 最大值 |
| `DecimalMin` | `BigNumber` | decimal 最小值 |

---

## 核心属性

| 名称 | 类型 | 说明 |
|------|------|------|
| `Value` | `string` | 字符串表示的数值 |
| `IsInteger` | `bool` | 是否为整数 |
| `IntegerLength` | `int` | 整数部分长度 |
| `NumberOfDecimalPlaces` | `int` | 小数位数 |

---

## 构造函数

```csharp
// 默认构造（值为 0）
public BigNumber()

// 字符串构造
public BigNumber(string value)

// 复制构造
public BigNumber(BigNumber other)

// 数值类型构造（重载）
public BigNumber(int value)
public BigNumber(long value)
public BigNumber(float value)
public BigNumber(double value)
public BigNumber(decimal value)
```

---

## 运算符重载

### 算术运算符

```csharp
// 加法
public static BigNumber operator +(BigNumber a, BigNumber b)
public static BigNumber operator +(BigNumber a, int b)
public static BigNumber operator +(int a, BigNumber b)
// ... 其他数值类型重载

// 减法
public static BigNumber operator -(BigNumber a, BigNumber b)
public static BigNumber operator -(BigNumber a) // 取反

// 乘法
public static BigNumber operator *(BigNumber a, BigNumber b)
public static BigNumber operator *(BigNumber a, int b)
// ... 其他数值类型重载

// 除法
public static BigNumber operator /(BigNumber a, BigNumber b)
public static BigNumber operator /(BigNumber a, int b)
// ... 其他数值类型重载

// 取模
public static BigNumber operator %(BigNumber a, BigNumber b)
```

### 比较运算符

```csharp
public static bool operator ==(BigNumber a, BigNumber b)
public static bool operator !=(BigNumber a, BigNumber b)
public static bool operator >(BigNumber a, BigNumber b)
public static bool operator <(BigNumber a, BigNumber b)
public static bool operator >=(BigNumber a, BigNumber b)
public static bool operator <=(BigNumber a, BigNumber b)
```

### 类型转换

```csharp
// 隐式转换
public static implicit operator BigNumber(int value)
public static implicit operator BigNumber(long value)
public static implicit operator BigNumber(float value)
public static implicit operator BigNumber(double value)
public static implicit operator BigNumber(decimal value)

// 显式转换
public static explicit operator int(BigNumber value)
public static explicit operator long(BigNumber value)
public static explicit operator float(BigNumber value)
public static explicit operator double(BigNumber value)
public static explicit operator decimal(BigNumber value)
public static explicit operator uint(BigNumber value)
public static explicit operator ulong(BigNumber value)
```

---

## 核心方法

### Parse

**签名**:
```csharp
public static BigNumber Parse(string value)
```

**职责**: 解析字符串为大数

**使用示例**:
```csharp
var num = BigNumber.Parse("123456789012345678901234567890");
var invalid = BigNumber.Parse("abc"); // 返回 Zero
```

---

### ToString

**签名**:
```csharp
public override string ToString()
public string ToString(int count) // 保留小数位数
public string ToLString(int count, bool ceil = false) // 保留有效数字
```

**职责**: 格式化为字符串

**使用示例**:
```csharp
var num = new BigNumber("123.456789");

num.ToString();      // "123.456789"
num.ToString(2);     // "123.45"
num.ToLString(3);    // "123" (3 位有效数字)
num.ToLString(5);    // "123.45" (5 位有效数字)
```

---

### ToFloat / ToDouble / ToLong 等

**签名**:
```csharp
public float ToFloat()
public double ToDouble()
public long ToLong()
public int ToInt()
public decimal ToDecimal()
```

**职责**: 转换为标准数值类型

**注意**: 可能丢失精度或溢出

**使用示例**:
```csharp
var num = new BigNumber("123.456");
float f = num.ToFloat();
long l = num.ToLong();
```

---

### Abs

**签名**:
```csharp
public BigNumber Abs()
```

**职责**: 返回绝对值

**使用示例**:
```csharp
var num = new BigNumber("-123.45");
var abs = num.Abs(); // "123.45"
```

---

### Pow

**签名**:
```csharp
public BigNumber Pow(int exponent)
```

**职责**: 计算幂

**使用示例**:
```csharp
var num = new BigNumber("2");
var squared = num.Pow(2);    // "4"
var cubed = num.Pow(3);      // "8"
var huge = num.Pow(100);     // "1267650600228229401496703205376"
```

---

### CompareTo

**签名**:
```csharp
public int CompareTo(BigNumber other)
```

**职责**: 比较大小

**返回值**:
- `-1`: 小于
- `0`: 等于
- `1`: 大于

**使用示例**:
```csharp
var a = new BigNumber("100");
var b = new BigNumber("200");

int result = a.CompareTo(b); // -1 (小于)
```

---

### Min / Max

**签名**:
```csharp
public static BigNumber Min(BigNumber a, BigNumber b)
public static BigNumber Max(BigNumber a, BigNumber b)
```

**职责**: 返回较小/较大值

**使用示例**:
```csharp
var min = BigNumber.Min(num1, num2);
var max = BigNumber.Max(num1, num2);
```

---

### Clamp

**签名**:
```csharp
public BigNumber Clamp(BigNumber min, BigNumber max)
```

**职责**: 限制数值范围

**使用示例**:
```csharp
var num = new BigNumber("150");
var clamped = num.Clamp(0, 100); // "100"
```

---

## 使用示例

### 游戏金币系统

```csharp
public class PlayerCurrency
{
    private BigNumber gold;
    private BigNumber diamond;
    
    public PlayerCurrency()
    {
        gold = BigNumber.Zero;
        diamond = BigNumber.Zero;
    }
    
    public void AddGold(BigNumber amount)
    {
        gold += amount;
    }
    
    public bool TrySpendGold(BigNumber amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            return true;
        }
        return false;
    }
    
    public void AddInterest(float rate)
    {
        // 计算利息
        BigNumber interest = gold * rate;
        gold += interest;
    }
    
    public string GetGoldString()
    {
        // 格式化显示
        if (gold > 1000000)
        {
            return gold.ToLString(3) + "M"; // 1.23M
        }
        return gold.ToString();
    }
}
```

### 伤害计算（高精度）

```csharp
public class DamageCalculator
{
    public BigNumber CalculateDamage(BigNumber baseDamage, BigNumber multiplier, BigNumber bonus)
    {
        // 基础伤害 × 倍率 + 加成
        BigNumber damage = baseDamage * multiplier + bonus;
        
        // 向下取整
        return new BigNumber(damage.ToLong());
    }
    
    public BigNumber CalculateCritDamage(BigNumber damage, float critMultiplier)
    {
        return damage * critMultiplier;
    }
    
    public BigNumber CalculateDefenseReduction(BigNumber damage, BigNumber defense)
    {
        // 伤害减免公式：damage * (1000 / (1000 + defense))
        BigNumber reduction = new BigNumber("1000") / (new BigNumber("1000") + defense);
        return damage * reduction;
    }
}
```

### 数值显示格式化

```csharp
public static class NumberFormatter
{
    private static readonly BigNumber THOUSAND = new BigNumber("1000");
    private static readonly BigNumber MILLION = new BigNumber("1000000");
    private static readonly BigNumber BILLION = new BigNumber("1000000000");
    private static readonly BigNumber TRILLION = new BigNumber("1000000000000");
    
    public static string FormatBigNumber(BigNumber num)
    {
        if (num >= TRILLION)
        {
            return (num / TRILLION).ToString(2) + "T";
        }
        if (num >= BILLION)
        {
            return (num / BILLION).ToString(2) + "B";
        }
        if (num >= MILLION)
        {
            return (num / MILLION).ToString(2) + "M";
        }
        if (num >= THOUSAND)
        {
            return (num / THOUSAND).ToString(2) + "K";
        }
        return num.ToString();
    }
    
    // 使用示例
    // 1234 → "1.23K"
    // 1234567 → "1.23M"
    // 1234567890 → "1.23B"
}
```

---

## 性能优化建议

### 1. 避免频繁创建

```csharp
// ❌ 低效：每次创建新对象
BigNumber result = 0;
for (int i = 0; i < 1000; i++)
{
    result = result + i; // 创建 1000 个临时对象
}

// ✅ 高效：复用对象
BigNumber result = BigNumber.Zero;
for (int i = 0; i < 1000; i++)
{
    result += i;
}
```

### 2. 使用合适的类型

```csharp
// 小数值用标准类型
int smallNum = 100;

// 大数值用 BigNumber
BigNumber hugeNum = BigNumber.Parse("12345678901234567890");
```

### 3. 缓存常用常量

```csharp
// ✅ 缓存
private static readonly BigNumber HUNDRED = new BigNumber("100");
private static readonly BigNumber THOUSAND = new BigNumber("1000");

// 使用缓存
var result = value * HUNDRED;
```

---

## ⚠️ 注意事项

| 问题 | 说明 | 解决方案 |
|------|------|----------|
| **性能开销** | BigNumber 运算比标准类型慢 | 仅在需要时使用 |
| **精度问题** | 字符串存储可能有精度损失 | 设置合适的 PRECISION |
| **空值处理** | 无效字符串会返回 Zero | 检查输入有效性 |
| **类型转换** | 转换为标准类型可能溢出 | 使用 TryParse 模式 |
| **内存占用** | 每个对象都有字符串开销 | 避免大量临时对象 |

---

## 相关文档

- [ObjectPool.cs.md](../ObjectPool.cs.md) - 对象池
- [IdGenerater.cs.md](./IdGenerater.cs.md) - ID 生成器

---

*文档由 OpenClaw AI 助手自动生成 | 基于静态代码分析*
