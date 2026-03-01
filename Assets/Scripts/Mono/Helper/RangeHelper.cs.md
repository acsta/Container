# RangeHelper.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | RangeHelper.cs |
| **路径** | Assets/Scripts/Mono/Helper/RangeHelper.cs |
| **所属模块** | Mono/Helper |
| **文件职责** | 范围工具类，提供列表随机打乱功能 |

---

## 类说明

### RangeHelper

| 属性 | 说明 |
|------|------|
| **职责** | 提供列表/数组随机打乱扩展方法 |
| **类型** | `static class` |

---

## 方法说明

### RandomSort<T>(List<T>)

```csharp
public static void RandomSort<T>(this List<T> list)
```

**职责**: 随机打乱 List (Fisher-Yates 洗牌算法)

---

### RandomSort<T>(T[])

```csharp
public static void RandomSort<T>(this T[] list)
```

**职责**: 随机打乱数组

---

## 使用示例

```csharp
// 打乱列表
List<int> list = new List<int> { 1, 2, 3, 4, 5 };
list.RandomSort();

// 打乱数组
int[] array = { 1, 2, 3, 4, 5 };
array.RandomSort();
```

---

## 算法说明

使用 Fisher-Yates 洗牌算法:
```
for i from n-1 down to 1:
    j = random(0, i)
    swap(list[i], list[j])
```

时间复杂度：O(n)
空间复杂度：O(1)

---

## 相关文档

- [Random 类文档](https://docs.microsoft.com/en-us/dotnet/api/system.random)

---

*文档生成时间：2026-03-02 | OpenClaw AI 助手*
