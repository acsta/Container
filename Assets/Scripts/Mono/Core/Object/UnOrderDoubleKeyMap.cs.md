# UnOrderDoubleKeyMap.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | UnOrderDoubleKeyMap.cs |
| **路径** | Assets/Scripts/Mono/Core/Object/UnOrderDoubleKeyMap.cs |
| **所属模块** | 框架层 → Mono/Core/Object |
| **命名空间** | `TaoTie` |
| **文件职责** | 提供双键无序多重映射（T → M → List<N>） |

---

## 类说明

### UnOrderDoubleKeyMap<T, M, N>

| 属性 | 说明 |
|------|------|
| **职责** | 继承 `Dictionary<T, UnOrderMultiMap<M, N>>`，支持双键索引的多重映射 |
| **泛型参数** | `T` - 第一层键类型<br>`M` - 第二层键类型<br>`N` - 值类型（List 存储，允许重复） |
| **继承关系** | `Dictionary<T, UnOrderMultiMap<M, N>>` |
| **实现的接口** | 无额外接口 |

**数据结构**:
```
T (第一层键)
  └─ UnOrderMultiMap<M, N> (第二层映射)
       └─ M (第二层键)
            └─ List<N> (值列表)
```

---

## 字段与属性

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `this[T t, M m]` | `List<N>` | `public new` | 双键索引器，返回第二层键对应的 List |

---

## 方法说明

### Add(T t, M m, N k)

**签名**:
```csharp
public void Add(T t, M m, N k)
```

**职责**: 添加三层结构的键值对

**核心逻辑**:
```
1. 尝试获取第一层键对应的 UnOrderMultiMap
2. 如果不存在，创建新的 UnOrderMultiMap
3. 调用 UnOrderMultiMap.Add(m, k)
```

---

### Remove(T t, M m, N k)

**签名**:
```csharp
public bool Remove(T t, M m, N k)
```

**职责**: 移除指定的键值对

**返回值**: `true` - 成功移除；`false` - 键不存在

---

### Contains(T t, M m, N k)

**签名**:
```csharp
public bool Contains(T t, M m, N k)
```

**职责**: 检查是否包含指定的键值对

---

### TryGetList(T t, M m, out List<N> list)

**签名**:
```csharp
public bool TryGetList(T t, M m, out List<N> list)
```

**职责**: 尝试获取双键对应的 List

**返回值**: `true` - 找到；`false` - 未找到

---

### Count

**签名**:
```csharp
public new int Count { get; }
```

**职责**: 获取所有值的总数（遍历所有内层 Count 求和）

---

## 使用示例

```csharp
// 创建双键映射
var map = new UnOrderDoubleKeyMap<string, int, float>();

// 添加数据
map.Add("category1", 1, 100f);
map.Add("category1", 1, 200f); // 允许重复
map.Add("category1", 2, 300f);
map.Add("category2", 1, 400f);

// 获取列表
if (map.TryGetList("category1", 1, out var list))
{
    foreach (var value in list)
    {
        Log.Info(value.ToString()); // 100, 200
    }
}

// 检查包含
bool has = map.Contains("category1", 1, 100f); // true

// 移除
map.Remove("category1", 1, 100f);

// 获取总数
int total = map.Count; // 所有值的数量
```

---

## 相关文档

- [UnOrderMultiMap.cs.md](./UnOrderMultiMap.cs.md) - 内层使用的多重映射
- [UnOrderDoubleKeyMapSet.cs.md](./UnOrderDoubleKeyMapSet.cs.md) - HashSet 版本的双键映射
- [MultiMap.cs.md](./MultiMap.cs.md) - 单键多重映射

---

*文档生成时间：2026-03-02 | OpenClaw AI 助手*
