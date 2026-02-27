# DoubleMap.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | DoubleMap.cs |
| **路径** | Assets/Scripts/Mono/Core/Object/DoubleMap.cs |
| **所属模块** | 框架层 → Mono/Core/Object |
| **文件职责** | 实现双向映射，支持 K→V 和 V→K 两个方向的查找 |

---

## 类/结构体说明

### DoubleMap<K, V>

| 属性 | 说明 |
|------|------|
| **职责** | 维护两个字典，实现键值双向查找 |
| **泛型参数** | K - 正向键类型<br/>V - 反向键类型（也是值） |
| **继承关系** | 无继承 |
| **实现的接口** | 无 |

**数据结构**:
```
DoubleMap<K, V>
    ├── kv: Dictionary<K, V>  // 正向映射 K → V
    └── vk: Dictionary<V, K>  // 反向映射 V → K
```

**典型用途**: ID ↔ 对象、枚举 ↔ 字符串等双向查找场景

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `kv` | `Dictionary<K, V>` | `private readonly` | 正向映射字典 |
| `vk` | `Dictionary<V, K>` | `private readonly` | 反向映射字典 |
| `Keys` | `List<K>` | `public` | 返回所有键的列表（每次创建新列表） |
| `Values` | `List<V>` | `public` | 返回所有值的列表（每次创建新列表） |

---

## 方法说明（按重要程度排序）

### Add(K key, V value)

**签名**:
```csharp
public void Add(K key, V value)
```

**职责**: 添加键值对，同时维护正向和反向映射

**核心逻辑**:
```
1. 检查 key 或 value 是否为 null → 是则返回
2. 检查 key 是否已存在于 kv → 是则返回
3. 检查 value 是否已存在于 vk → 是则返回
4. 同时添加到 kv 和 vk
```

**调用者**: 需要建立双向映射的场景

**被调用者**: `kv.Add`, `vk.Add`

**使用示例**:
```csharp
var map = new DoubleMap<int, string>();
map.Add(1, "One");
map.Add(2, "Two");
```

---

### GetValueByKey(K key)

**签名**:
```csharp
public V GetValueByKey(K key)
```

**职责**: 通过键获取值（正向查找）

**核心逻辑**:
```
1. 检查 key 是否为 null
2. 检查 kv 是否包含 key
3. 返回 kv[key] 或 default(V)
```

**调用者**: 需要正向查找的场景

---

### GetKeyByValue(V value)

**签名**:
```csharp
public K GetKeyByValue(V value)
```

**职责**: 通过值获取键（反向查找）

**核心逻辑**:
```
1. 检查 value 是否为 null
2. 检查 vk 是否包含 value
3. 返回 vk[value] 或 default(K)
```

**调用者**: 需要反向查找的场景

**使用示例**:
```csharp
var map = new DoubleMap<int, string>();
map.Add(1, "One");

var value = map.GetValueByKey(1);      // "One"
var key = map.GetKeyByValue("One");    // 1
```

---

### RemoveByKey(K key)

**签名**:
```csharp
public void RemoveByKey(K key)
```

**职责**: 通过键删除映射

**核心逻辑**:
```
1. 检查 key 是否为 null
2. 通过 key 获取对应的 value
3. 如果不存在，返回
4. 同时从 kv 和 vk 中删除
```

**被调用者**: `kv.TryGetValue`, `kv.Remove`, `vk.Remove`

---

### RemoveByValue(V value)

**签名**:
```csharp
public void RemoveByValue(V value)
```

**职责**: 通过值删除映射

**核心逻辑**:
```
1. 检查 value 是否为 null
2. 通过 value 获取对应的 key
3. 如果不存在，返回
4. 同时从 kv 和 vk 中删除
```

---

### ForEach(Action<K, V> action)

**签名**:
```csharp
public void ForEach(Action<K, V> action)
```

**职责**: 遍历所有键值对

**核心逻辑**:
```
1. 检查 action 是否为 null
2. 遍历 kv.Keys
3. 对每个 key 调用 action(key, kv[key])
```

**使用示例**:
```csharp
map.ForEach((key, value) => 
{
    Debug.Log($"{key} → {value}");
});
```

---

### Clear()

**签名**:
```csharp
public void Clear()
```

**职责**: 清空所有映射

**核心逻辑**: 同时清空 kv 和 vk

---

### ContainsKey(K key)

**签名**:
```csharp
public bool ContainsKey(K key)
```

**职责**: 检查是否包含指定键

---

### ContainsValue(V value)

**签名**:
```csharp
public bool ContainsValue(V value)
```

**职责**: 检查是否包含指定值

---

### Contains(K key, V value)

**签名**:
```csharp
public bool Contains(K key, V value)
```

**职责**: 检查是否包含指定的键值对

**核心逻辑**: 同时检查 kv.ContainsKey(key) 和 vk.ContainsKey(value)

---

## 构造函数

### DoubleMap()

**签名**:
```csharp
public DoubleMap()
```

**职责**: 创建默认容量的双向映射

---

### DoubleMap(int capacity)

**签名**:
```csharp
public DoubleMap(int capacity)
```

**职责**: 创建指定容量的双向映射

**使用示例**:
```csharp
var map = new DoubleMap<int, string>(100);  // 预分配 100 个元素容量
```

---

## 阅读指引

### 建议的阅读顺序

1. **理解数据结构** - 两个字典维护双向映射
2. **看 Add 方法** - 理解如何同时维护两个字典
3. **看 GetValueByKey/GetKeyByValue** - 理解双向查找
4. **看 Remove 方法** - 理解如何同步删除

### 最值得学习的技术点

1. **双向一致性**: 添加/删除时同时维护两个字典
2. **null 安全**: 所有方法都检查 null
3. **唯一性约束**: 键和值都必须唯一
4. **只读字段**: `readonly` 确保字典引用不被替换

---

## 使用示例

### 示例 1: ID ↔ 对象映射

```csharp
public class Player
{
    public int Id { get; set; }
    public string Name { get; set; }
}

var playerMap = new DoubleMap<int, Player>();

// 添加
playerMap.Add(1, new Player { Id = 1, Name = "Alice" });
playerMap.Add(2, new Player { Id = 2, Name = "Bob" });

// 通过 ID 查找玩家
var player = playerMap.GetValueByKey(1);  // Alice

// 通过玩家查找 ID
var id = playerMap.GetKeyByValue(player); // 1
```

### 示例 2: 枚举 ↔ 字符串映射

```csharp
var stateMap = new DoubleMap<GameState, string>();

stateMap.Add(GameState.Playing, "游戏中");
stateMap.Add(GameState.Paused, "已暂停");
stateMap.Add(GameState.GameOver, "游戏结束");

// 枚举转字符串
string text = stateMap.GetValueByKey(GameState.Paused);  // "已暂停"

// 字符串转枚举
GameState state = stateMap.GetKeyByValue("游戏结束");  // GameState.GameOver
```

### 示例 3: 实体 ID 映射

```csharp
// Entity 系统中，EntityId ↔ Entity 对象
var entityMap = new DoubleMap<long, Entity>();

// 创建实体
var entity = new Entity { Id = 1001 };
entityMap.Add(entity.Id, entity);

// 通过 ID 获取实体
var e = entityMap.GetValueByKey(1001);

// 通过实体获取 ID
var id = entityMap.GetKeyByValue(e);  // 1001

// 移除实体
entityMap.RemoveByKey(1001);
// 或
entityMap.RemoveByValue(e);
```

---

## 与 UnOrderDoubleKeyDictionary 的对比

| 特性 | DoubleMap | UnOrderDoubleKeyDictionary |
|------|-----------|---------------------------|
| **映射方向** | 双向 K↔V | 两级键 T+M→N |
| **数据结构** | 两个独立字典 | 嵌套字典 |
| **典型用途** | ID↔对象 | 类型 + 名称→Manager |
| **键唯一性** | K 和 V 都必须唯一 | T+M 组合唯一 |

---

## 性能特点

| 操作 | 时间复杂度 | 说明 |
|------|-----------|------|
| Add | O(1) | 两次字典插入 |
| GetValueByKey | O(1) | 字典查找 |
| GetKeyByValue | O(1) | 字典查找 |
| RemoveByKey | O(1) | 两次字典删除 |
| RemoveByValue | O(1) | 两次字典删除 |
| Contains | O(1) | 两次字典查找 |

**空间复杂度**: O(n) - 存储两份数据（正向 + 反向）

---

## 注意事项

### 值必须唯一

```csharp
var map = new DoubleMap<int, string>();
map.Add(1, "One");
map.Add(2, "One");  // ❌ 不会添加，因为 "One" 已存在
```

### null 值处理

```csharp
map.Add(null, "value");  // ❌ 不会添加，key 为 null
map.Add(1, null);        // ❌ 不会添加，value 为 null
```

### Keys/Values 返回新列表

```csharp
var keys = map.Keys;  // 每次调用创建新 List
keys.Add(999);        // 不影响原 map
```

---

## 相关文档

- [UnOrderDoubleKeyDictionary.cs.md](./UnOrderDoubleKeyDictionary.cs.md) - 双键字典
- [UnOrderMultiMap.cs.md](./UnOrderMultiMap.cs.md) - 多值映射
- [EntityManager.cs.md](../../Code/Game/System/Entity/EntityManager.cs.md) - 可能的使用者

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
