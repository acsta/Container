# UnOrderDoubleKeyMapSet.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | UnOrderDoubleKeyMapSet.cs |
| **路径** | Assets/Scripts/Mono/Core/Object/UnOrderDoubleKeyMapSet.cs |
| **所属模块** | 框架层 → Mono/Core/Object |
| **命名空间** | `TaoTie` |
| **文件职责** | 提供双键无序多重映射（T → M → HashSet<N>，去重） |

---

## 类说明

### UnOrderDoubleKeyMapSet<T, M, N>

| 属性 | 说明 |
|------|------|
| **职责** | 继承 `Dictionary<T, UnOrderMultiMapSet<M, N>>`，支持双键索引的去重多重映射 |
| **泛型参数** | `T` - 第一层键类型<br>`M` - 第二层键类型<br>`N` - 值类型（HashSet 存储，自动去重） |
| **继承关系** | `Dictionary<T, UnOrderMultiMapSet<M, N>>` |

**数据结构**:
```
T (第一层键)
  └─ UnOrderMultiMapSet<M, N> (第二层映射，HashSet 去重)
       └─ M (第二层键)
            └─ HashSet<N> (值集合，去重)
```

---

## 方法说明

### Add(T t, M m, N k)

添加键值对（自动去重）

### Remove(T t, M m, N k)

移除指定的键值对

### Contains(T t, M m, N k)

检查是否包含指定的键值对

### this[T t, M m]

双键索引器，返回 HashSet<N>

### Count

获取所有值的总数

---

## 与 UnOrderDoubleKeyMap 的区别

| 特性 | UnOrderDoubleKeyMapSet | UnOrderDoubleKeyMap |
|------|------------------------|---------------------|
| **内层容器** | `HashSet<N>` | `List<N>` |
| **值是否去重** | ✅ 自动去重 | ❌ 允许重复 |

---

## 使用示例

```csharp
var map = new UnOrderDoubleKeyMapSet<string, int, float>();

// 添加（自动去重）
map.Add("category1", 1, 100f);
map.Add("category1", 1, 100f); // 重复值被忽略
map.Add("category1", 1, 200f);

// 获取 HashSet
HashSet<float> values = map["category1", 1];

// 检查
bool has = map.Contains("category1", 1, 100f); // true

// 移除
map.Remove("category1", 1, 100f);
```

---

## 相关文档

- [UnOrderDoubleKeyMap.cs.md](./UnOrderDoubleKeyMap.cs.md) - List 版本的双键映射
- [UnOrderMultiMapSet.cs.md](./UnOrderMultiMapSet.cs.md) - 内层使用的多重映射
- [MultiMapSet.cs.md](./MultiMapSet.cs.md) - 单键去重版本

---

*文档生成时间：2026-03-02 | OpenClaw AI 助手*
