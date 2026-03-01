# BigNumber.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | BigNumber.cs |
| **路径** | Assets/Scripts/Mono/Core/Object/BigNumber.cs |
| **所属模块** | Mono 层 → Core/Object |
| **文件职责** | 高精度大数运算，支持任意精度整数和浮点数 |

---

## 类说明

### BigNumber

| 属性 | 说明 |
|------|------|
| **职责** | 表示任意精度的大数值，支持整数和浮点数运算 |
| **精度** | 默认 32 位小数精度 (可配置) |
| **存储方式** | 字符串存储，避免浮点精度丢失 |
| **线程安全** | 非线程安全 |

```csharp
public sealed class BigNumber
{
    // 内部字符串存储
    private string strValue;
    
    // 可配置精度
    public static ushort PRECISION = 32;
}
```

---

## 常量定义

### 预定义常量

| 常量 | 值 | 说明 |
|------|-----|------|
| `Zero` | "0" | 零值 |
| `FloatMax` | float.MaxValue | float 最大值 |
| `FloatMin` | float.MinValue | float 最小值 |
| `DoubleMax` | double.MaxValue | double 最大值 |
| `DoubleMin` | double.MinValue | double 最小值 |
| `LongMax` | long.MaxValue | long 最大值 |
| `LongMin` | long.MinValue | long 最小值 |
| `DecimalMax` | decimal.MaxValue | decimal 最大值 |
| `DecimalMin` | decimal.MinValue | decimal 最小值 |

---

## 属性说明

### Value

```csharp
public string Value => this.strValue;
```

**说明**: 获取数值的字符串表示

---

### IsInteger

```csharp
public bool IsInteger => strValue == null || !strValue.Contains('.');
```

**说明**: 判断是否为整数（不含小数点）

---

### IntegerLength

```csharp
public int IntegerLength
{
    get
    {
        // 返回整数部分的位数
    }
}
```

**说明**: 获取整数部分的长度

---

### NumberOfDecimalPlaces

```csharp
public int NumberOfDecimalPlaces
{
    get
    {
        // 返回小数部分的位数
    }
}
```

**说明**: 获取小数部分的位数

---

## 构造方法

### 无参构造

```csharp
public BigNumber() => this.strValue = "0";
```

**说明**: 构造值为 0 的大数

---

### 字符串构造

```csharp
public BigNumber(string value)
{
    if (!BigNumberPreProcess(value, out this.strValue)) this.strValue = "0";
}
```

**说明**: 从字符串构造大数，自动格式化和验证

---

### 拷贝构造

```csharp
public BigNumber(BigNumber other)
{
    this.strValue = other.strValue;
}
```

**说明**: 从另一个 BigNumber 拷贝

---

## 类型转换

### 隐式转换（输入）

```csharp
// 从数值类型转换
public static implicit operator BigNumber(sbyte/byte/short/ushort/int/uint/long/ulong value)
public static implicit operator BigNumber(float/double/decimal value)
public static implicit operator BigNumber(string value)
```

**使用示例**:
```csharp
BigNumber a = 100;           // int
BigNumber b = 3.14f;         // float
BigNumber c = "123456789";   // string
```

---

### 隐式转换（输出）

```csharp
// 转换为数值类型
public static implicit operator string(BigNumber value)
public static implicit operator float/double/decimal/long/int(BigNumber value)
```

**注意**: 转换可能抛出 `ArgumentOutOfRangeException` 如果超出目标类型范围

**使用示例**:
```csharp
BigNumber big = "123456789";
string s = big;              // "123456789"
float f = big;               // 123456789.0f
```

---

## 运算符重载

### 算术运算符

| 运算符 | 签名 | 说明 |
|--------|------|------|
| `+` | `BigNumber + BigNumber` | 加法 |
| `-` | `BigNumber - BigNumber` | 减法 |
| `*` | `BigNumber * int/BigNumber` | 乘法 |
| `/` | `BigNumber / int/long/BigNumber` | 除法 |
| `%` | `BigNumber % BigNumber` | 取模 |

**使用示例**:
```csharp
BigNumber a = "12345678901234567890";
BigNumber b = "98765432109876543210";

BigNumber sum = a + b;        // 加法
BigNumber diff = a - b;       // 减法
BigNumber product = a * 100;  // 乘法
BigNumber quotient = a / b;   // 除法
BigNumber remainder = a % b;  // 取模
```

---

### 比较运算符

| 运算符 | 签名 | 说明 |
|--------|------|------|
| `==` | `BigNumber == int/long/BigNumber` | 等于 |
| `!=` | `BigNumber != int/long/BigNumber` | 不等于 |
| `<` | `BigNumber < int/long/BigNumber` | 小于 |
| `>` | `BigNumber > int/long/BigNumber` | 大于 |
| `<=` | `BigNumber <= int/long/BigNumber` | 小于等于 |
| `>=` | `BigNumber >= int/long/BigNumber` | 大于等于 |

**使用示例**:
```csharp
BigNumber a = "100";
BigNumber b = "200";

if (a < b) { /* ... */ }
if (a == 100) { /* ... */ }
```

---

## 核心方法

### ToString()

```csharp
public override string ToString()
public string ToString(int count)  // 保留指定小数位
```

**说明**: 转换为字符串表示

**使用示例**:
```csharp
BigNumber num = "3.1415926535";
num.ToString();      // "3.1415926535"
num.ToString(2);     // "3.14"
```

---

### ToLString()

```csharp
public string ToLString(int count, bool ceil = false)
```

**说明**: 保留精确数字位数（有效数字）

**参数**:
- `count`: 有效数字位数
- `ceil`: 是否向上取整

**使用示例**:
```csharp
BigNumber num = "12345.6789";
num.ToLString(3);        // "12300"
num.ToLString(3, true);  // "12400" (向上取整)
```

---

### 取整方法

```csharp
// 向上取整
public static void Cell2Integer(BigNumber val)

// 向下取整
public static void Floor2Integer(BigNumber val)

// 四舍五入
public static void Round2Integer(BigNumber val)
```

**使用示例**:
```csharp
BigNumber num = "3.7";
BigNumber.Cell2Integer(num);   // num = "4"
BigNumber.Floor2Integer(num);  // num = "3"
BigNumber.Round2Integer(num);  // num = "4"
```

---

## 内部算法

### 大数加法 (BigNumberAdd)

**算法流程**:
```
1. 处理符号（正负数情况）
2. 对齐小数点
3. 执行整数加法/减法
4. 恢复小数点位置
5. 处理结果符号
```

**优化**: 对于小数值（整数部分 < 25 位），使用 decimal 直接计算

---

### 大数减法 (BigNumberSubtract)

**算法流程**:
```
1. 处理符号
2. 对齐小数点
3. 执行整数减法
4. 处理借位
5. 恢复小数点位置
```

---

### 大数乘法 (BigNumberMultiply)

**算法流程**:
```
1. 处理符号
2. 分离整数和小数部分
3. 从低位到高位逐位相乘
4. 处理进位
5. 恢复小数点位置
```

**优化**: 对于小数值（整数部分 < 12 位），使用 decimal 直接计算

---

### 大数除法 (BigNumberDivide)

**算法流程**:
```
1. 处理符号
2. 分离整数和小数部分
3. 执行长除法
4. 计算小数部分（达到 PRECISION 精度）
5. 恢复小数点位置
```

**精度控制**: 小数部分计算达到 `PRECISION` (默认 32) 位后停止

---

## 使用场景

### 场景 1: 游戏货币计算

```csharp
// 避免浮点精度问题
BigNumber gold = "999999999.99";
BigNumber tax = gold * 0.05m;  // 5% 税率
BigNumber final = gold - tax;
```

### 场景 2: 高精度数值显示

```csharp
// 显示伤害数值
BigNumber damage = "123456789012345678901234567890";
uiText.text = damage.ToLString(4);  // "123500000000000000000000000000"
```

### 场景 3: 数值比较

```csharp
// 排行榜分数比较
BigNumber score1 = player1.Score;
BigNumber score2 = player2.Score;

if (score1 > score2)
{
    // player1 排名更高
}
```

---

## 性能优化

### 小数值优化

对于整数部分 < 25 位的数值，内部使用 `decimal` 直接计算，避免字符串操作开销：

```csharp
if (num1.IntegerLength < 25 && num2.IntegerLength < 25 && 
    decimal.TryParse(num1, out var d1) && decimal.TryParse(num2, out var d2))
{
    return ConvertFromFloat(d1 + d2);  // 使用 decimal 计算
}
```

### 对象池友好

`BigNumber` 是不可变类型（sealed class），适合与对象池配合使用。

---

## 注意事项

### 精度限制

- 默认小数精度为 32 位
- 可通过 `BigNumber.PRECISION` 调整
- 除法运算达到精度限制后截断

### 类型转换风险

```csharp
BigNumber big = "1e100";
float f = big;  // 可能抛出 ArgumentOutOfRangeException
```

**建议**: 转换前检查范围

### 科学计数法支持

支持科学计数法字符串输入：

```csharp
BigNumber a = "1.23E+10";   // 12300000000
BigNumber b = "1.23E-5";    // 0.0000123
```

---

## 相关文档

- [ObjectPool.cs.md](../ObjectPool.cs.md) - 对象池
- [ListComponent.cs.md](./ListComponent.cs.md) - 列表组件

---

*文档生成时间：2026-03-01 | OpenClaw AI 助手*
