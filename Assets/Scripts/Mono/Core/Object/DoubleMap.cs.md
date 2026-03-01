# DoubleMap.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | DoubleMap.cs |
| **路径** | Assets/Scripts/Mono/Core/Object/DoubleMap.cs |
| **所属模块** | Mono 层 → Core/Object |
| **文件职责** | 双向映射字典，支持 K→V 和 V→K 双向查找 |

---

## 类说明

### DoubleMap\<K, V\>

| 属性 | 说明 |
|------|------|
| **职责** | 维护键值对的双向映射关系 |
| **特点** | K→V 和 V→K 都是 O(1) 查找 |
| **约束** | key 和 value 都不能为 null，且必须唯一 |

```csharp
public class DoubleMap<K, V>
{
    // K → V 映射
    private readonly Dictionary<K, V> kv;
    
    // V → K 映射
    private readonly Dictionary<V, K> vk;
}
```

---

## 构造方法

### 无参构造

```csharp
public DoubleMap()
```

**说明**: 创建默认容量的双向映射

---

### 带容量构造

```csharp
public DoubleMap(int capacity)
```

**说明**: 创建指定初始容量的双向映射

**用途**: 预知数据量时优化性能

---

## 方法说明

### ForEach()

**签名**:
```csharp
public void ForEach(Action<K, V> action)
```

**职责**: 遍历所有键值对

**参数**:
- `action`: 对每个键值对执行的操作

**使用示例**:
```csharp
var map = new DoubleMap<string, int>();
map.Add("a", 1);
map.Add("b", 2);

map.ForEach((key, value) => {
    Console.WriteLine($"{key} = {value}");
});
```

---

### Keys / Values

**签名**:
```csharp
public List<K> Keys { get; }
public List<V> Values { get; }
```

**说明**: 获取所有键或所有值的列表（返回新列表）

**注意**: 每次访问都创建新列表，避免外部修改影响内部数据

---

### Add()

**签名**:
```csharp
public void Add(K key, V value)
```

**职责**: 添加键值对

**约束**:
- key 不能为 null
- value 不能为 null
- key 不能已存在
- value 不能已存在

**核心逻辑**:
```
1. 检查 key 和 value 是否为 null
2. 检查 kv 字典是否包含 key
3. 检查 vk 字典是否包含 value
4. 同时添加到 kv 和 vk 字典
```

**使用示例**:
```csharp
var map = new DoubleMap<string, int>();
map.Add("one", 1);
map.Add("two", 2);
```

---

### GetValueByKey()

**签名**:
```csharp
public V GetValueByKey(K key)
```

**职责**: 通过键获取值

**返回**: 对应的值，不存在则返回 default(V)

**时间复杂度**: O(1)

**使用示例**:
```csharp
var map = new DoubleMap<string, int>();
map.Add("score", 100);

int score = map.GetValueByKey("score");  // 100
int level = map.GetValueByKey("level");  // 0 (不存在)
```

---

### GetKeyByValue()

**签名**:
```csharp
public K GetKeyByValue(V value)
```

**职责**: 通过值获取键

**返回**: 对应的键，不存在则返回 default(K)

**时间复杂度**: O(1)

**使用示例**:
```csharp
var map = new DoubleMap<string, int>();
map.Add("player1", 100);
map.Add("player2", 200);

string name = map.GetKeyByValue(100);  // "player1"
```

---

### RemoveByKey()

**签名**:
```csharp
public void RemoveByKey(K key)
```

**职责**: 通过键移除键值对

**核心逻辑**:
```
1. 通过 key 查找对应的 value
2. 从 kv 字典移除 key
3. 从 vk 字典移除 value
```

**使用示例**:
```csharp
var map = new DoubleMap<string, int>();
map.Add("a", 1);

map.RemoveByKey("a");  // 移除 "a" → 1
```

---

### RemoveByValue()

**签名**:
```csharp
public void RemoveByValue(V value)
```

**职责**: 通过值移除键值对

**核心逻辑**:
```
1. 通过 value 查找对应的 key
2. 从 vk 字典移除 value
3. 从 kv 字典移除 key
```

**使用示例**:
```csharp
var map = new DoubleMap<string, int>();
map.Add("a", 1);

map.RemoveByValue(1);  // 移除 "a" → 1
```

---

## 使用场景

### 场景 1: ID ↔ 对象映射

```csharp
// 玩家 ID 和玩家对象双向查找
var playerMap = new DoubleMap<long, Player>();
playerMap.Add(1001, player1);
playerMap.Add(1002, player2);

// 通过 ID 找玩家
Player p = playerMap.GetValueByKey(1001);

// 通过玩家找 ID
long id = playerMap.GetKeyByValue(player1);
```

### 场景 2: 名称 ↔ ID 映射

```csharp
// 道具名称和 ID 双向映射
var itemMap = new DoubleMap<string, int>();
itemMap.Add("sword", 101);
itemMap.Add("shield", 102);

// 通过名称查 ID
int swordId = itemMap.GetValueByKey("sword");  // 101

// 通过 ID 查名称
string itemName = itemMap.GetKeyByValue(102);  // "shield"
```

### 场景 3: 双向关联数据

```csharp
// 用户 ↔ 邮箱双向查找
var userMailMap = new DoubleMap<string, string>();
userMailMap.Add("user1", "user1@example.com");
userMailMap.Add("user2", "user2@example.com");

// 通过用户查邮箱
string mail = userMailMap.GetValueByKey("user1");

// 通过邮箱查用户
string user = userMailMap.GetKeyByValue("user2@example.com");
```

---

## 数据结构

```
DoubleMap<K, V>
    ├── kv: Dictionary<K, V>  (K → V)
    └── vk: Dictionary<V, K>  (V → K)
    
添加 "a" → 1:
    kv["a"] = 1
    vk[1] = "a"
    
通过 "a" 查值: kv["a"] → 1
通过 1 查键：vk[1] → "a"
```

---

## 性能特性

| 操作 | 时间复杂度 | 说明 |
|------|-----------|------|
| Add | O(1) | 字典添加 |
| GetValueByKey | O(1) | 字典查找 |
| GetKeyByValue | O(1) | 字典查找 |
| RemoveByKey | O(1) | 字典删除 |
| RemoveByValue | O(1) | 字典删除 |
| ForEach | O(n) | 遍历所有 |
| Keys/Values | O(n) | 创建新列表 |

---

## 注意事项

### 1. null 值不支持

```csharp
map.Add(null, 100);  // ❌ 不会添加，直接返回
map.Add("key", null); // ❌ 不会添加，直接返回
```

### 2. 唯一性约束

```csharp
map.Add("a", 1);
map.Add("a", 2);  // ❌ 不会添加，key 已存在
map.Add("b", 1);  // ❌ 不会添加，value 已存在
```

### 3. 修改需先删除

DoubleMap 不支持直接修改，需先删除再添加：

```csharp
// ❌ 错误：没有直接修改的方法
// map["a"] = 2;

// ✅ 正确：先删除再添加
map.RemoveByKey("a");
map.Add("a", 2);
```

---

## 相关文档

- [DictionaryComponent.cs.md](./DictionaryComponent.cs.md) - 字典组件
- [UnOrderDoubleKeyDictionary.cs.md](./UnOrderDoubleKeyDictionary.cs.md) - 双键字典

---

*文档生成时间：2026-03-01 | OpenClaw AI 助手*
