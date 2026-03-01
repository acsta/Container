# UnOrderMultiMap.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | UnOrderMultiMap.cs |
| **路径** | Assets/Scripts/Mono/Core/Object/UnOrderMultiMap.cs |
| **所属模块** | 框架层 → Mono/Core/Object |
| **命名空间** | `TaoTie` |
| **文件职责** | 提供一键多值的无序多重映射集合（内层为 List） |

---

## 类说明

### UnOrderMultiMap<T, K>

| 属性 | 说明 |
|------|------|
| **职责** | 继承 `Dictionary<T, List<K>>`，支持一个键对应多个值（允许重复），无序 |
| **泛型参数** | `T` - 键类型（无序）<br>`K` - 值类型（允许重复） |
| **继承关系** | `Dictionary<T, List<K>>` |
| **实现的接口** | 无额外接口 |

---

## 方法说明

### Add(T t, K k)

添加键值对，自动创建内层 List

### Remove(T t, K k)

移除指定的键值对，如果 List 为空则移除整个键

### GetAll(T t)

获取键对应的所有值（返回副本数组）

### GetOne(T t)

获取键对应的第一个值

### Contains(T t, K k)

检查是否包含指定的键值对

---

## 与 MultiMap 的区别

| 特性 | UnOrderMultiMap | MultiMap |
|------|-----------------|----------|
| **基类** | `Dictionary<T, List<K>>` | `SortedDictionary<T, List<K>>` |
| **键是否有序** | ❌ 无序 | ✅ 有序 |
| **性能** | 更快（O(1)） | 稍慢（O(log n)） |

---

## 使用示例

```csharp
var map = new UnOrderMultiMap<string, int>();

// 添加（允许重复）
map.Add("cat", 1);
map.Add("cat", 2);
map.Add("cat", 1); // 重复也添加

// 获取所有
int[] values = map.GetAll("cat"); // [1, 2, 1]

// 获取第一个
int first = map.GetOne("cat"); // 1

// 检查
bool has = map.Contains("cat", 2); // true

// 移除
map.Remove("cat", 1); // 只移除一个 1
```

---

## 相关文档

- [MultiMap.cs.md](./MultiMap.cs.md) - 有序版本
- [UnOrderMultiMapSet.cs.md](./UnOrderMultiMapSet.cs.md) - HashSet 去重版本
- [UnOrderDoubleKeyMap.cs.md](./UnOrderDoubleKeyMap.cs.md) - 双键版本

---

*文档生成时间：2026-03-02 | OpenClaw AI 助手*
