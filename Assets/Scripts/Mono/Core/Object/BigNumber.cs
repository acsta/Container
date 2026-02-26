using System.Diagnostics;
using System.Text.RegularExpressions;
using System;
using System.Globalization;
using System.Text;
using TaoTie.LitJson.Extensions;

namespace TaoTie
{
    /// <summary>
    /// 一个表示大值数字的类型。支持整数和浮点数。
    /// </summary>
    public sealed class BigNumber
    {
        private static readonly Regex BigNumberRegex = new Regex(@"^[-]?(\d*[\.]?\d+)$", RegexOptions.Compiled);
        [JsonIgnore]
        public static ushort PRECISION = 32;
        [JsonIgnore]
        public static readonly BigNumber Zero = new();
        [JsonIgnore]
        public static readonly BigNumber FloatMax = float.MaxValue;
        [JsonIgnore]
        public static readonly BigNumber FloatMin = float.MinValue;
        [JsonIgnore]
        public static readonly BigNumber DoubleMax = double.MaxValue;
        [JsonIgnore]
        public static readonly BigNumber DoubleMin = double.MinValue;
        [JsonIgnore]
        public static readonly BigNumber LongMax = long.MaxValue;
        [JsonIgnore]
        public static readonly BigNumber LongMin = long.MinValue;
        [JsonIgnore]
        public static readonly BigNumber DecimalMax = decimal.MaxValue;
        [JsonIgnore]
        public static readonly BigNumber DecimalMin = decimal.MinValue;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string strValue;
        /// <summary>
        /// 字符串表示的数值
        /// </summary>
        [JsonIgnore]
        public string Value => this.strValue;

        [JsonIgnore]
        public bool IsInteger => strValue == null || !strValue.Contains('.');
        [JsonIgnore]
        public int IntegerLength
        {
            get
            {
                if (string.IsNullOrEmpty(strValue)) return 0;
                var index = strValue.IndexOf('.');
                return index < 0 ? strValue.Length : index;
            }
        }
        [JsonIgnore]
        public int NumberOfDecimalPlaces
        {
            get
            {
                if (IsInteger) return 0;
                var index = strValue.IndexOf('.');
                return strValue.Length - index - 1;
            }
        }
        /// <summary>
        /// 构造一个表示 0 的大数类型
        /// </summary>
        public BigNumber() => this.strValue = "0";

        /// <summary>
        /// 用字符串构造一个大数类型
        /// </summary>
        /// <param name="value">数值字符串</param>
        public BigNumber(string value)
        {
            if (!BigNumberPreProcess(value, out this.strValue)) this.strValue = "0";
        }

        public BigNumber(BigNumber other)
        {
            this.strValue = other.strValue;
        }

        #region 重载 Object 三大接口

        public override string ToString() => this.strValue;

        /// <summary>
        /// 保留小数
        /// </summary>
        /// <param name="count">保留小数位数</param>
        /// <returns></returns>
        public string ToString(int count)
        {
            int dotIndex = strValue.IndexOf('.');
            if (dotIndex <0)
            {
                return strValue;
            }

            if (count == 0)
            {
                return strValue.Substring(0, dotIndex);
            }

            var val = strValue.Substring(0, Math.Min(dotIndex + count + 1, strValue.Length));
            while (val.EndsWith("0"))
            {
                val = val.TrimEnd('0');
                if (val.EndsWith("."))
                {
                    val = val.TrimEnd('.');
                    break;
                }
            }
            return val;
        }

        /// <summary>
        /// 保留精确数字位数
        /// </summary>
        /// <param name="count">保留有效数字位数</param>
        /// <param name="ceil">向上取整？</param>
        /// <returns></returns>
        public string ToLString(int count, bool ceil = false)
        {
            if (count <= 0 || strValue == null || strValue.Length <= 0) return strValue;
            bool isNegative = strValue[0] == '-';
            if (isNegative) ceil = !ceil;
            StringBuilder sb = new StringBuilder();
            BigNumber flag = null;
            int cc = 0;
            int pointIndex = strValue.IndexOf('.');
            bool isCeil = false;
            //是整数补0
            if (pointIndex < 0 || pointIndex - (isNegative ? 1 : 0) >= count)
            {
                for (int i = isNegative ? 1 : 0; i < (pointIndex<0?strValue.Length:pointIndex); i++)
                {
                    if (cc < count)
                    {
                        sb.Append(strValue[i]);
                    }
                    else
                    {
                        if (ceil && !isCeil && flag != null && strValue[i] != '0')
                        {
                            isCeil = true;
                            flag += 1;
                        }
                        sb.Append("0");
                    }

                    cc++;
                    if (cc == count && ceil && flag == null)
                    {
                        flag = sb.ToString();
                        sb = new StringBuilder();
                        if (pointIndex >= 0)
                        {
                            isCeil = true;
                            flag += 1;
                        }
                    }
                }

                string res;
                if (flag != null)
                {
                    res = flag.ToString() + sb.ToString();
                }
                else
                {
                    res = sb.ToString();
                }
                if (res == "0")
                {
                    return res;
                }
                return (isNegative ? "-" : "") + res;
            }
            else
            {
                for (int i = isNegative ? 1 : 0; i <= count + (isNegative ? 1 : 0); i++)
                {
                    sb.Append(strValue[i]);
                }
                flag = sb.ToString();
                if (ceil && count + (isNegative ? 1 : 0) + 1 < strValue.Length)
                {
                    sb = new StringBuilder();
                    for (int i = 0; i < flag.strValue.Length; i++)
                    {
                        if (flag.strValue[i] != '.')
                        {
                            sb.Append(i == flag.strValue.Length - 1 ? 1 : 0);
                        }
                        else
                        {
                            sb.Append('.');
                        }
                    }

                    flag += sb.ToString();
                }

                var res = flag.ToString();
                if (res == "0")
                {
                    return res;
                }
                return (isNegative ? "-" : "") + res;
            }
        }
        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj is string) return this.strValue.Equals(obj);
            if (obj is BigNumber) return this.strValue == ((BigNumber)obj).strValue;
            return false;
        }

        public override int GetHashCode() => this.strValue.GetHashCode();

        #endregion

        #region 类型转换 加减乘除 逻辑运算

        public static implicit operator BigNumber(string value) => new(value);
        public static implicit operator BigNumber(sbyte value) => new(value.ToString());
        public static implicit operator BigNumber(byte value) => new(value.ToString());
        public static implicit operator BigNumber(short value) => new(value.ToString());
        public static implicit operator BigNumber(ushort value) => new(value.ToString());
        public static implicit operator BigNumber(int value) => new(value.ToString());
        public static implicit operator BigNumber(uint value) => new(value.ToString());
        public static implicit operator BigNumber(long value) => new(value.ToString());
        public static implicit operator BigNumber(ulong value) => new(value.ToString());
        public static implicit operator BigNumber(float value) => ConvertFromFloat(value);
        public static implicit operator BigNumber(double value) => ConvertFromFloat(value);
        public static implicit operator BigNumber(decimal value) => ConvertFromFloat(value);

        public static implicit operator string(BigNumber value) => value.strValue;
        public static implicit operator decimal(BigNumber value)
        {
            if (value > DecimalMax || value < DecimalMin) throw new ArgumentOutOfRangeException();
            return decimal.Parse(value.strValue);
        }
        public static implicit operator float(BigNumber value)
        {
            if (value > FloatMax || value < FloatMin) throw new ArgumentOutOfRangeException();
            return float.Parse(value.strValue);
        }
        public static implicit operator double(BigNumber value)
        {
            if (value > DoubleMax || value < DoubleMin) throw new ArgumentOutOfRangeException();
            return double.Parse(value.strValue);
        }
        public static implicit operator long(BigNumber value)
        {
            if (value > LongMax || value < LongMin) throw new ArgumentOutOfRangeException();
            return long.Parse(value.strValue);
        }
        public static implicit operator int(BigNumber value) => (int)(long)value;


        public static BigNumber operator +(BigNumber l, BigNumber r) => BigNumberAdd(l, r);
        public static BigNumber operator -(BigNumber l, BigNumber r) => BigNumberSubtract(l, r);
        public static BigNumber operator *(int l, BigNumber r) => BigNumberMultiply(r, l);
        public static BigNumber operator *(BigNumber l, int r) => BigNumberMultiply(l, r);
        public static BigNumber operator *(BigNumber l, BigNumber r) => BigNumberMultiply(l, r);
        public static BigNumber operator /(BigNumber l, int r) => BigNumberDivide(l, (long)r);
        public static BigNumber operator /(BigNumber l, long r) => BigNumberDivide(l, r);
        public static BigNumber operator /(BigNumber l, BigNumber r) => BigNumberDivide(l, r);
        public static BigNumber operator %(BigNumber l, BigNumber r)
        {
            if (r.strValue == "0") throw new DivideByZeroException();
            if (l.strValue == "0") return new BigNumber();

            BigNumber quotient = l / r;
            if (quotient.IsInteger)
            {
                return l - (quotient * r);
            }

            // 取整数部分
            BigNumber integerQuotient = new BigNumber(quotient.strValue.Split('.')[0]);
            return l - (integerQuotient * r);
        }

       
        public static bool operator == (BigNumber l, int r) => Compare(l, r) == 0;
        public static bool operator == (int l, BigNumber r) => Compare(r, l) == 0;
        public static bool operator == (BigNumber l, long r) => Compare(l, r) == 0;
        public static bool operator == (long l, BigNumber r) => Compare(r, l) == 0;
        public static bool operator ==(BigNumber l, BigNumber r)
        {
            if (ReferenceEquals(l, r)) return true;
            if (ReferenceEquals(l, null) || ReferenceEquals(r, null)) return false;
            return l.strValue == r.strValue;
        }
        public static bool operator != (BigNumber l, int r) => Compare(r, l) != 0;
        public static bool operator != (int l, BigNumber r) => Compare(r, l) != 0;
        public static bool operator != (BigNumber l, long r) => Compare(r, l) != 0;
        public static bool operator != (long l, BigNumber r) => Compare(r, l) != 0;
        public static bool operator !=(BigNumber l, BigNumber r)
        {
            return !(l == r);
        }

        public static bool operator <(int l, BigNumber r) => Compare(r, l) > 0;
        public static bool operator <(BigNumber l, int r) => Compare(l, r) < 0;
        public static bool operator <(long l, BigNumber r) => Compare(r, l) > 0;
        public static bool operator <(BigNumber l, long r) => Compare(l, r) < 0;
        public static bool operator <(BigNumber l, BigNumber r)
        {
            var ret = LessThen(l, r, out string result);
            if (ret.HasValue) return ret.Value;
            return result[0] == '-';
        }
        public static bool operator >(int l, BigNumber r) => Compare(r, l) < 0;
        public static bool operator >(BigNumber l, int r) => Compare(l, r) > 0;
        public static bool operator >(long l, BigNumber r) => Compare(r, l) < 0;
        public static bool operator >(BigNumber l, long r) => Compare(l, r) > 0;
        public static bool operator >(BigNumber l, BigNumber r)
        {
            var ret = GreaterThen(l, r, out string result);
            if (ret.HasValue) return ret.Value;
            return result[0] != '-' && result != "0";
        }
        public static bool operator <=(int l, BigNumber r) => Compare(r, l) >= 0;
        public static bool operator <=(BigNumber l, int r) => Compare(l, r) <= 0;
        public static bool operator <=(long l, BigNumber r) => Compare(r, l) >= 0;
        public static bool operator <=(BigNumber l, long r) => Compare(l, r) <= 0;
        public static bool operator <=(BigNumber l, BigNumber r)
        {
            var ret = LessThen(l, r, out string result);
            if (ret.HasValue) return ret.Value;
            return result[0] == '-' || result == "0";
        }
        public static bool operator >=(int l, BigNumber r) => Compare(r, l) <= 0;
        public static bool operator >=(BigNumber l, int r) => Compare(l, r) >= 0;
        public static bool operator >=(long l, BigNumber r) => Compare(r, l) <= 0;
        public static bool operator >=(BigNumber l, long r) => Compare(l, r) >= 0;
        public static bool operator >=(BigNumber l, BigNumber r)
        {
            var ret = GreaterThen(l, r, out string result);
            if (ret.HasValue) return ret.Value;
            return result[0] != '-';
        }

        public static bool operator ==(BigNumber l, float r) => EqualsFloat(l, r);
        public static bool operator !=(BigNumber l, float r) => !EqualsFloat(l, r);
        public static bool operator ==(BigNumber l, double r) => EqualsFloat(l, r);
        public static bool operator !=(BigNumber l, double r) => !EqualsFloat(l, r);
        public static bool operator ==(float l, BigNumber r) => EqualsFloat(r, l);
        public static bool operator !=(float l, BigNumber r) => !EqualsFloat(r, l);
        public static bool operator ==(double l, BigNumber r) => EqualsFloat(r, l);
        public static bool operator !=(double l, BigNumber r) => !EqualsFloat(r, l);

        public static void Cell2Integer(BigNumber val)
        {
            if(val.IsInteger) return;
            val.strValue = val.strValue.Substring(0, val.IntegerLength);
            val.strValue = (val + 1).strValue;
        }
        
        public static void Floor2Integer(BigNumber val)
        {
            if(val.IsInteger) return;
            val.strValue = val.strValue.Substring(0, val.IntegerLength);
        }
        
        public static void Round2Integer(BigNumber val)
        {
            if(val.IsInteger) return;
            var number = val.strValue[val.IntegerLength + 1] - 48;
            val.strValue = val.strValue.Substring(0, val.IntegerLength);
            if (number >= 5)
            {
                val.strValue = (val + 1).strValue;
            }
        }
        #endregion

        #region 加减乘除

        // 加法 +
        static string BigNumberAdd(BigNumber num1, BigNumber num2)
        {
            if (num1.Value == "-") return "-" + num2.Value;
            if (num1 == Zero) return num2;
            if (num2 == Zero) return num1;
            if (num1.IntegerLength < 25 && num2.IntegerLength < 25 && decimal.TryParse(num1,out var d1) && decimal.TryParse(num2,out var d2))
            {
                return ConvertFromFloat(d1 + d2);
            }
            int idx_dot = AlignDot(num1, num2, out string value1, out string value2);
            string result, prefix = "";
            if (value1[0] == '-')
            {
                value1 = value1.Substring(1);
                if (value2[0] == '-')
                {
                    value2 = value2.Substring(1);
                    prefix = "-";
                    result = Execute(BigIntegerAdd, value1, value2, idx_dot);
                }
                else result = Execute(BigIntegerSubtract, value2, value1, idx_dot);
            }
            else
            {
                if (value2[0] == '-')
                {
                    value2 = value2.Substring(1);
                    result = Execute(BigIntegerSubtract, value1, value2, idx_dot);
                }
                else result = Execute(BigIntegerAdd, value1, value2, idx_dot);
            }

            return PostResult(prefix, result, idx_dot);
        }

        // 减法 -
        static string BigNumberSubtract(BigNumber num1, BigNumber num2)
        {
            if (num2 == Zero) return num1;
            if (num1 == Zero)
            {
                if (num2 == Zero) return Zero;
                if (num2.Value[0] == '-') return num2.Value.Substring(1);
                return "-" + num2.Value;
            }
            if (num1.IntegerLength < 25 && num2.IntegerLength < 25 && decimal.TryParse(num1,out var d1) && decimal.TryParse(num2,out var d2))
            {
                return ConvertFromFloat(d1 - d2);
            }
            int idx_dot = AlignDot(num1, num2, out string value1, out string value2);
            string result, prefix = "";
            if (value1[0] == '-')
            {
                value1 = value1.Substring(1);
                if (value2[0] == '-')
                {
                    value2 = value2.Substring(1);
                    result = Execute(BigIntegerSubtract, value2, value1, idx_dot);
                }
                else
                {
                    prefix = "-";
                    result = Execute(BigIntegerAdd, value1, value2, idx_dot);
                }
            }
            else
            {
                if (value2[0] == '-')
                {
                    value2 = value2.Substring(1);
                    result = Execute(BigIntegerAdd, value1, value2, idx_dot);
                }
                else result = Execute(BigIntegerSubtract, value1, value2, idx_dot);
            }
            return PostResult(prefix, result, idx_dot);
        }

        // 乘法 *
        static string BigNumberMultiply(BigNumber num1, int num2)
        {
            if (num2 == 0 || num1 == Zero)
            {
                return "0";
            }
            if (num1.IntegerLength < 12 && decimal.TryParse(num1,out var d1))
            {
                return ConvertFromFloat(d1 * num2);
            }
            // 处理符号
            bool isNegative = false;
            if (num1.Value.StartsWith("-"))
            {
                isNegative = !isNegative;
                num1 = num1.Value.Substring(1);
            }

            if (num2 < 0)
            {
                isNegative = !isNegative;
                num2 = -num2;
            }

            // 检查是否为小数
            bool hasDecimal = !num1.IsInteger;
            string integerPart = num1;
            string decimalPart = "";
            int decimalPlaces = 0;

            if (hasDecimal)
            {
                string[] parts = num1.Value.Split('.');
                integerPart = parts[0];
                decimalPart = parts[1];
                decimalPlaces = decimalPart.Length;
            }

            // 合并整数和小数部分
            string fullNumber = integerPart + decimalPart;
            if (string.IsNullOrEmpty(fullNumber))
            {
                return "0";
            }

            // 执行乘法
            StringBuilder result = new StringBuilder();
            int carry = 0;

            // 从最低位开始处理
            for (int i = fullNumber.Length - 1; i >= 0; i--)
            {
                int digit = fullNumber[i] - '0';
                int product = digit * num2 + carry;
                carry = product / 10;
                result.Insert(0, (product % 10).ToString());
            }

            // 处理最后的进位
            if (carry > 0)
            {
                result.Insert(0, carry.ToString());
            }

            // 处理全零结果
            if (result.Length == 0)
            {
                return "0";
            }

            string finalResult = result.ToString();

            // 处理小数部分
            if (hasDecimal)
            {
                // 计算小数点位置
                int decimalPosition = finalResult.Length - decimalPlaces;

                // 处理小数点位置在开头的情况
                if (decimalPosition <= 0)
                {
                    // 补零
                    finalResult = finalResult.PadLeft(decimalPlaces + 1, '0');
                    decimalPosition = 1;
                }

                // 插入小数点
                finalResult = finalResult.Insert(decimalPosition, ".");

                // 移除尾部多余的零
                finalResult = finalResult.TrimEnd('0');
                if (finalResult.EndsWith("."))
                {
                    finalResult = finalResult.Substring(0, finalResult.Length - 1);
                }
            }

            // 添加负号
            if (isNegative && finalResult != "0")
            {
                finalResult = "-" + finalResult;
            }

            return finalResult;
        }

        static string BigNumberMultiply(BigNumber num1, BigNumber num2)
        {
            if (num1.IntegerLength < 12 && num2.IntegerLength < 12 && decimal.TryParse(num1,out var d1) && decimal.TryParse(num2,out var d2))
            {
                return ConvertFromFloat(d1 * d2);
            }
            string prefix = GetPrefix(num1, num2, out string value1, out string value2);
            // 小数点
            int idx, idx_dot = 0;
            idx = value1.IndexOf('.');
            if (idx > 0)
            {
                idx_dot += value1.Length - idx - 1;
                value1 = value1[0] == '0' ? value1.Substring(2) : value1.Remove(idx, 1);
                BigNumberPreProcess(value1, out value1);
            }
            idx = value2.IndexOf(".");
            if (idx > 0)
            {
                idx_dot += value2.Length - idx - 1;
                value2 = value2[0] == '0' ? value2.Substring(2) : value2.Remove(idx, 1);
                BigNumberPreProcess(value2, out value2);
            }
            // 计算
            using ListComponent<string> listEachResult = ListComponent<string>.Create();
            using ListComponent<char> listResult = ListComponent<char>.Create();
            for (int i = value2.Length - 1; i > -1; i--)
            {
                listResult.Clear();
                for (int j = i; j < value2.Length - 1; j++)
                    listResult.Add('0');
                int v = value2[i] - '0';
                if (v == 0) continue;
                int left = 0;
                for (int j = value1.Length - 1; j > -1; j--)
                {
                    int t = value1[j] - '0';
                    t *= v;
                    t += left;
                    left = t / 10;
                    t %= 10;
                    listResult.Add((char)('0' + t));
                }
                if (left > 0) listResult.Add((char)('0' + left));
                listResult.Reverse();
                listEachResult.Add(new string(listResult.ToArray()));
            }
            if (listEachResult.Count == 0) return "0";

            string result = listEachResult[0];
            listEachResult.RemoveAt(0);
            while (listEachResult.Count > 0)
            {
                result = BigIntegerAdd(result, listEachResult[0]);
                listEachResult.RemoveAt(0);
            }

            if (idx_dot > 0)
            {
                if (idx_dot >= result.Length) result = result.PadLeft(idx_dot + 1, '0');
                result = string.Concat(result.Substring(0, result.Length - idx_dot), ".", result.Substring(result.Length - idx_dot));
            }
            return prefix + result;
        }

        // 除法 ÷
        static string BigNumberDivide(BigNumber num1, long num2)
        {
            if (num2 == 0) throw new DivideByZeroException();
            if (decimal.TryParse(num1, out var d1))
            {
                return ConvertFromFloat(d1 / num2);
            }
            // 处理符号
            bool isNegative = false;
            if (num2 < 0)
            {
                isNegative = true;
                num2 = -num2; // 转为正数处理
            }

             // 检查是否为小数
            bool hasDecimal = !num1.IsInteger;
            string integerPart = num1;
            string decimalPart = "";
            int decimalPlaces = 0;

            if (hasDecimal)
            {
                string[] parts = num1.Value.Split('.');
                integerPart = parts[0];
                decimalPart = parts[1];
                decimalPlaces = decimalPart.Length;
            }

            // 合并整数和小数部分
            string fullNumber = integerPart + decimalPart;
            if (string.IsNullOrEmpty(fullNumber) || fullNumber == "0")
            {
                return "0";
            }

            // 执行除法
            StringBuilder result = new StringBuilder();
            long remainder = 0;

            // 处理整数部分
            for (int i = 0; i < fullNumber.Length; i++)
            {
                long currentDigit = fullNumber[i] - '0';
                long currentNumber = remainder * 10 + currentDigit;
                
                long quotientDigit = currentNumber / num2;
                remainder = currentNumber % num2;

                // 跳过前导零
                if (quotientDigit > 0 || result.Length > 0)
                {
                    result.Append(quotientDigit);
                }
            }

            // 处理全零结果
            if (result.Length == 0)
            {
                return "0";
            }

            string finalResult = result.ToString();

            // 处理小数部分
            if (hasDecimal)
            {
                // 计算小数点位置
                int decimalPosition = finalResult.Length - decimalPlaces;
                
                // 处理小数点位置在开头的情况
                if (decimalPosition <= 0)
                {
                    // 补零
                    finalResult = finalResult.PadLeft(decimalPlaces + 1, '0');
                    decimalPosition = 1;
                }

                // 插入小数点
                finalResult = finalResult.Insert(decimalPosition, ".");
                
                // 移除尾部多余的零
                finalResult = finalResult.TrimEnd('0');
                if (finalResult.EndsWith("."))
                {
                    finalResult = finalResult.Substring(0, finalResult.Length - 1);
                }
            }

            // 添加负号
            if (isNegative && finalResult != "0")
            {
                finalResult = "-" + finalResult;
            }

            return finalResult;
        }
        static string BigNumberDivide(BigNumber num1, BigNumber num2)
        {
            if (num2 == Zero) throw new DivideByZeroException();
            if (num1.IntegerLength < 25 && num2.IntegerLength < 25 && decimal.TryParse(num1,out var d1) && decimal.TryParse(num2,out var d2))
            {
                return ConvertFromFloat(d1 / d2);
            }
            string prefix = GetPrefix(num1, num2, out string value1, out string value2);
            // 小数点
            int idx, idx_dot = 0;
            idx = value1.IndexOf('.');
            if (idx > 0)
            {
                idx_dot += value1.Length - idx - 1;
                value1 = value1[0] == '0' ? value1.Substring(2) : value1.Remove(idx, 1);
                BigNumberPreProcess(value1, out value1);
            }
            idx = value2.IndexOf('.');
            if (idx > 0)
            {
                idx_dot -= value2.Length - idx - 1;
                value2 = value2[0] == '0' ? value2.Substring(2) : value2.Remove(idx, 1);
                BigNumberPreProcess(value2, out value2);
            }
            // 计算
            using ListComponent<char> list = ListComponent<char>.Create();
            if (value1.Length < value2.Length)
            {
                idx_dot += value2.Length - value1.Length;
                value1 = value1.PadRight(value2.Length, '0');
            }

            BigNumber tmp = value1.Substring(0, value2.Length), x;
            value1 = value1.Substring(value2.Length);
            do
            {
                int v = 0;
                do
                {
                    x = tmp;
                    tmp -= value2;
                    v++;
                } while (tmp > Zero);
                if (tmp != Zero)
                {
                    v--;
                    tmp = x;
                }
                list.Add((char)('0' + v));
                if (value1.Length == 0) break;
                tmp = tmp.strValue + value1[0];
                value1 = value1.Substring(1);
            } while (true);

            tmp = new BigNumber(tmp);
            int dot_cnt = 0;
            if (tmp != Zero)
            {
                list.Add('.');
                while (tmp != Zero)
                {
                    tmp.strValue += "0";
                    int v = 0;
                    do
                    {
                        x = tmp;
                        tmp -= value2;
                        v++;
                    } while (tmp > Zero);
                    if (tmp != Zero)
                    {
                        v--;
                        tmp = x;
                    }
                    list.Add((char)('0' + v));
                    dot_cnt++;

                    if (dot_cnt > PRECISION) break;
                }
            }

            if (idx_dot != 0)
            {
                idx = list.IndexOf('.');
                if (idx > 0)
                {
                    list.RemoveAt(idx);
                    if (idx > idx_dot)
                    {
                        idx -= idx_dot;
                        if (idx > list.Count)
                        {
                            while (idx > list.Count) list.Add('0');
                        }
                        else list.Insert(idx, '.');
                    }
                    else
                    {
                        idx_dot -= idx;
                        for (int i = 0; i < idx_dot; i++) list.Insert(0, '0');
                        list.Insert(0, '.');
                        list.Insert(0, '0');
                    }
                }
                else
                {
                    if (idx_dot < 0)
                    {
                        idx_dot = -idx_dot;
                        for (int i = 0; i < idx_dot; i++) list.Insert(0, '0');
                    }
                    else
                    {
                        while (list.Count <= idx_dot) list.Insert(0, '0');
                        list.Insert(list.Count - idx_dot, '.');
                    }
                }
            }
            
            return prefix + new string(list.ToArray());
        }

        #endregion

        // 大数字符串格式检查
        static bool CheckBigNumber(string num) => BigNumberRegex.IsMatch(num);

        // 大数字符串预处理：掐头去尾
        static bool BigNumberPreProcess(string num, out string value)
        {
            if (string.IsNullOrWhiteSpace(num) || !CheckBigNumber(num))
            {
                value = "0";
                return false;
            }
            string prefix = "";
            if (num[0] == '-')
            {
                prefix = "-";
                num = num.Substring(1);
            }
            // 移除前导零
            int start = 0;
            while (start < num.Length && num[start] == '0' && (start == num.Length - 1 || num[start + 1] != '.'))
            {
                start++;
            }
            
            // 移除小数部分尾随零
            int end = num.Length - 1;
            int dotIndex = num.IndexOf('.');
            if (dotIndex >= 0)
            {
                while (end > dotIndex && num[end] == '0')
                {
                    end--;
                }
                if (end == dotIndex) end--; // 移除多余的小数点
            }
            
            if (start > end)
            {
                value = "0";
            }
            else
            {
                value = num.Substring(start, end - start + 1);
                if (value == "0") return true;
                value = prefix + value;
            }
            return true;
        }

        // 正整数 +
        static string BigIntegerAdd(string num1, string num2)
        {
            string value1, value2;
            BigNumberPreProcess(num1, out value1);
            BigNumberPreProcess(num2, out value2);
            if (value1.Length > value2.Length)
            {
                if (value2.Length == 1 && value2[0] == '0') return value1;
                value2 = value2.PadLeft(value1.Length, '0');
            }
            else
            {
                if (value1.Length == 1 && value1[0] == '0') return value2;
                value1 = value1.PadLeft(value2.Length, '0');
            }
            using ListComponent<char> list = ListComponent<char>.Create();
            int left = 0;
            for (int i = value1.Length - 1; i > -1; i--)
            {
                int v = value1[i] - '0';
                v += value2[i] - '0';
                v += left;
                left = v / 10;
                v %= 10;
                list.Add((char)('0' + v));
            }
            if (left > 0) list.Add((char)('0' + left));
            list.Reverse();
            return new string(list.ToArray());
        }

        // 正整数 -
        static string BigIntegerSubtract(string num1, string num2)
        {
            string value1, value2;
            BigNumberPreProcess(num1, out num1);
            BigNumberPreProcess(num2, out num2);
            string prefix = "";
            if (num1.Length > num2.Length)
            {
                if (num2.Length == 1 && num2[0] == '0') return num1;
                value1 = num1;
                value2 = num2.PadLeft(num1.Length, '0');
            }
            else if (num1.Length < num2.Length)
            {
                if (num1.Length == 1 && num1[0] == '0') return "-" + num2;
                value1 = num2;
                value2 = num1.PadLeft(num2.Length, '0');
                prefix = "-";
            }
            else
            {
                for (int i = 0; i < num2.Length; i++)
                {
                    if (num1[i] != num2[i])
                    {
                        if (num1[i] < num2[i])
                        {
                            value1 = num2;
                            value2 = num1;
                            prefix = "-";
                        }
                        else
                        {
                            value1 = num1;
                            value2 = num2;
                        }
                        goto calc;
                    }
                }
                return "0";
            }
        calc:
            using ListComponent<char> list = ListComponent<char>.Create();
            bool borrowed = false;
            for (int i = value1.Length - 1; i > -1; i--)
            {
                int v = value1[i];
                if (borrowed) v--;
                borrowed = false;
                if (v < value2[i])
                {
                    borrowed = true;
                    v += 10;
                }
                v -= value2[i];
                list.Add((char)('0' + v));
            }
            list.Reverse();
            return prefix + new string(list.ToArray());
        }

        // 加减法预处理：小数点对其
        static int AlignDot(string num1, string num2, out string value1, out string value2)
        {
            value1 = num1;
            value2 = num2;
            int idx1 = value1.IndexOf('.');
            int idx2 = value2.IndexOf(".");
            int sum1 = idx1 > 0 ? num1.Length - 1 - idx1 : idx1;
            int sum2 = idx2 > 0 ? num2.Length - 1 - idx2 : idx2;
            if (sum1 > sum2)
            {
                if (idx2 > 0) value2 = num2.PadRight(num2.Length + sum1 - sum2, '0');
                else value2 = num2 + "." + new string('0', sum1);
            }
            else if (sum2 > sum1)
            {
                if (idx1 > 0) value1 = num1.PadRight(num1.Length + sum2 - sum1, '0');
                else value1 = num1 + "." + new string('0', sum2);
                return sum2;
            }
            return sum1;
        }

        // 加减法中相同的处理过程
        static string Execute(Func<string, string, string> func, string num1, string num2, int idx)
        {
            if (idx < 0) return func(num1, num2);
            return func(num1.Remove(num1.Length - 1 - idx, 1), num2.Remove(num2.Length - 1 - idx, 1));
        }

        // float、double、decimal 转 BigNumber
        static BigNumber ConvertFromFloat<T>(T value) where T : struct
        {
            string val = value.ToString();
            int eIndex = val.IndexOf('E');
            
            if (eIndex < 0)
            {
                return new BigNumber(val);
            }
            
            // 处理科学计数法
            string basePart = val.Substring(0, eIndex);
            bool negativeExponent = val[eIndex + 1] == '-';
            int exponent = int.Parse(val.Substring(eIndex + 2));
            
            if (negativeExponent)
            {
                return new BigNumber(basePart) / new BigNumber("1" + new string('0', exponent));
            }
            
            return new BigNumber(basePart) * new BigNumber("1" + new string('0', exponent));
        }

        static bool EqualsFloat<T>(BigNumber l, T r) where T : struct
        {
            BigNumber rNum = ConvertFromFloat(r);
            return l == rNum;
        }

        // 加减结果处理
        static string PostResult(string prefix, string result, int idx_dot)
        {
            if (result == "0") return "0";
            if (result[0] == '-')
            {
                prefix = "-";
                result = result.Substring(1);
            }
            if (result.Length <= idx_dot) result = result.PadLeft(idx_dot + 1, '0');
            if (idx_dot > 0) result = string.Concat(result.Substring(0, result.Length - idx_dot), ".", result.Substring(result.Length - idx_dot));
            return prefix + result;
        }

        // 乘除结果符号
        static string GetPrefix(string num1, string num2, out string value1, out string value2)
        {
            value1 = num1;
            value2 = num2;
            string prefix = "";
            if (value1[0] == '-')
            {
                prefix = "-";
                value1 = value1.Substring(1);
            }
            if (value2[0] == '-')
            {
                prefix = prefix.Length == 1 ? "" : "-";
                value2 = value2.Substring(1);
            }
            return prefix;
        }
        
        // 乘除结果符号
        static string GetPrefix(string num1, int num2, out string value1)
        {
            value1 = num1;
            string prefix = "";
            if (value1[0] == '-')
            {
                prefix = "-";
                value1 = value1.Substring(1);
            }
            if (num2 < 0)
            {
                prefix = prefix.Length == 1 ? "" : "-";
            }
            return prefix;
        }

        static bool? LessThen(BigNumber l, BigNumber r, out string result)
        {
            result = "";
            if (l.strValue[0] == '-')
            {
                if (r.strValue[0] != '-') return true;
            }
            else
            {
                if (r.strValue[0] == '-') return false;
            }
            result = BigNumberSubtract(l, r);

            return null;
        }

        static bool? GreaterThen(BigNumber l, BigNumber r, out string result)
        {
            result = "";
            if (l.strValue[0] == '-')
            {
                if (r.strValue[0] != '-') return false;
            }
            else
            {
                if (r.strValue[0] == '-') return true;
            }
            result = BigNumberSubtract(l, r);

            return null;
        }

        static int Compare(BigNumber num1, long num2)
        {
            // 处理符号
            bool isNum1Negative = num1.Value.StartsWith("-");
            bool isNum2Negative = num2 < 0;
            
            // 情况1: 符号不同
            if (isNum1Negative && !isNum2Negative)
            {
                return -1; // 负 < 正
            }
            if (!isNum1Negative && isNum2Negative)
            {
                return 1; // 正 > 负
            }
            
            // 移除符号
            string absNum1 = isNum1Negative ? num1.Value.Substring(1) : num1;
            var absNum2 = Math.Abs(num2);
            
            // 检查是否为小数
            bool hasDecimal = absNum1.Contains(".");
            string integerPart = absNum1;
            string decimalPart = "";
            
            if (hasDecimal)
            {
                string[] parts = absNum1.Split('.');
                integerPart = parts[0];
                decimalPart = parts[1];
            }
            
            // 情况2: 整数部分比较
            long intPartValue;
            if (string.IsNullOrWhiteSpace(integerPart) || integerPart.Trim() == "0")
            {
                intPartValue = 0;
            }
            else
            {
                // 仅比较整数部分的前19位（long的最大位数）
                string intPartForCompare = integerPart.Length > 19 ? 
                    integerPart.Substring(0, 19) : integerPart;
                if (!long.TryParse(intPartForCompare, out intPartValue))
                {
                    // 处理超大整数
                    if (integerPart.Length > absNum2.ToString().Length)
                    {
                        return isNum1Negative?-1:1; // 正数越大越好，负数越小越好
                    }
                    intPartValue = 0;
                }
            }
            
            // 整数部分比较
            if (intPartValue > absNum2)
            {
                return isNum1Negative?-1:1; // 正数越大越好，负数越小越好
            }
            if (intPartValue < absNum2)
            {
                return isNum1Negative?1:-1; // 正数越小越差，负数越大越好
            }
            
            // 情况3: 整数部分相等，比较小数部分
            if (hasDecimal && !string.IsNullOrWhiteSpace(decimalPart))
            {
                // 移除小数部分的尾部零
                decimalPart = decimalPart.TrimEnd('0');
                
                if (!string.IsNullOrEmpty(decimalPart))
                {
                    // 有小数部分表示大于（对正数而言）
                    return isNum1Negative?-1:1;
                }
            }
            
            // 情况4: 完全相等
            return 0;
        }
    }
}
