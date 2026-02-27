# UnOrderDoubleKeyDictionary.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | UnOrderDoubleKeyDictionary.cs |
| **路径** | Assets/Scripts/Mono/Core/Object/UnOrderDoubleKeyDictionary.cs |
| **所属模块** | 框架层 → Mono/Core/Object |
| **文件职责** | 实现双键字典，支持通过两个键（T 和 M）索引值 N |

---

## 类/结构体说明

### UnOrderDoubleKeyDictionary<T, M, N>

| 属性 | 说明 |
|------|------|
| **职责** | 嵌套字典结构，支持两级键索引 |
| **泛型参数** | T - 第一级键类型<br/>M - 第二级键类型<br/>N - 值类型 |
| **继承关系** | 继承自 `Dictionary<T, Dictionary<M, N>>` |
| **实现的接口** | 继承自 Dictionary 的所有接口 |

**数据结构**:
```
UnOrderDoubleKeyDictionary<T, M, N>
    ↓
Dictionary<T, Dictionary<M, N>>
    ├── T1 → Dictionary<M, N>
    │       ├── M1 → N1
    │       └── M2 → N2
    └── T2 → Dictionary<M, N>
            └── M3 → N3
```

**典型用途**: ManagerProvider 中用于 `Dictionary<Type, string, object>`，通过类型 + 名称索引 Manager

---

## 字段与属性

本类继承自 Dictionary，无额外字段。

**继承的字段**:
- 内部维护 `Dictionary<T, Dictionary<M, N>>` 结构

---

## 方法说明（按重要程度排序）

### Add(T t, M m, N n)

**签名**:
```csharp
public void Add(T t, M m, N n)
```

**职责**: 添加一个键值对，自动创建中间字典

**核心逻辑**:
```
1. 尝试获取第一级键 t 对应的子字典 kSet
2. 如果 kSet 为 null（不存在）:
   - 创建新的 Dictionary<M, N>
   - 添加到 this[t]
3. 将 (m, n) 添加到 kSet
```

**调用者**: ManagerProvider.RegisterManager

**使用示例**:
```csharp
var dict = new UnOrderDoubleKeyDictionary<Type, string, object>();
dict.Add(typeof(UIManager), "", uiManagerInstance);
dict.Add(typeof(ConfigManager), "", configManagerInstance);
```

---

### TryGetValue(T t, M m, out N n)

**签名**:
```csharp
public bool TryGetValue(T t, M m, out N n)
```

**职责**: 通过两级键获取值

**核心逻辑**:
```
1. 初始化 n 为 default
2. 尝试通过 t 获取子字典 dic
3. 如果 dic 不存在，返回 false
4. 否则在 dic 中查找 m，返回结果
```

**调用者**: ManagerProvider.GetManager

**使用示例**:
```csharp
if (dict.TryGetValue(typeof(UIManager), "", out var manager))
{
    var ui = manager as UIManager;
}
```

---

### Remove(T t, M m)

**签名**:
```csharp
public bool Remove(T t, M m)
```

**职责**: 移除指定键值对，自动清理空子字典

**核心逻辑**:
```
1. 获取第一级键 t 对应的子字典 dic
2. 如果 dic 为 null 或移除失败，返回 false
3. 如果 dic 移除后 Count == 0，从父字典移除 t
4. 返回 true
```

**调用者**: ManagerProvider.RemoveManager

**使用示例**:
```csharp
dict.Remove(typeof(UIManager), "");
// 如果该类型下没有其他名称的 Manager，会自动清理整个子字典
```

---

### TryGetDic(T t, out Dictionary<M, N> k)

**签名**:
```csharp
public bool TryGetDic(T t, out Dictionary<M, N> k)
```

**职责**: 获取第一级键对应的子字典

**核心逻辑**: 直接调用基类的 TryGetValue

**使用示例**:
```csharp
if (dict.TryGetDic(typeof(UIManager), out var subDict))
{
    foreach (var kv in subDict)
    {
        // 遍历该类型下所有名称的 Manager
    }
}
```

---

### ContainSubKey(T t, M m)

**签名**:
```csharp
public bool ContainSubKey(T t, M m)
```

**职责**: 检查是否包含指定的两级键

**核心逻辑**:
```
1. 获取子字典 dic
2. 如果 dic 为 null，返回 false
3. 否则检查 dic 是否包含键 m
```

---

### ContainValue(T t, M m, N n)

**签名**:
```csharp
public bool ContainValue(T t, M m, N n)
```

**职责**: 检查指定位置是否包含特定值

**核心逻辑**:
```
1. 获取子字典 dic
2. 如果 dic 为 null，返回 false
3. 检查 dic 是否包含键 m
4. 检查 dic 是否包含值 n
```

---

## 与 ManagerProvider 的协作

### ManagerProvider 中的使用

```csharp
// ManagerProvider.cs
UnOrderDoubleKeyDictionary<Type, string, object> managersDictionary;

// 注册 Manager
public static T RegisterManager<T>(string name = "") where T : class, IManager
{
    var type = TypeInfo<T>.Type;
    if (!Instance.managersDictionary.TryGetValue(type, name, out var res))
    {
        res = Activator.CreateInstance(type) as T;
        // ...
        Instance.managersDictionary.Add(type, name, res);  // ← 使用 Add
    }
    return res as T;
}

// 获取 Manager
public static T GetManager<T>(string name = "") where T : class, IManagerDestroy
{
    var type = TypeInfo<T>.Type;
    if (!Instance.managersDictionary.TryGetValue(type, name, out var res))
    {
        return null;  // ← 使用 TryGetValue
    }
    return res as T;
}

// 移除 Manager
public static void RemoveManager<T>(string name = "")
{
    var type = TypeInfo<T>.Type;
    if (Instance.managersDictionary.TryGetValue(type, name, out var res))
    {
        Instance.managersDictionary.Remove(type, name);  // ← 使用 Remove
        // ...
    }
}
```

---

## 阅读指引

### 建议的阅读顺序

1. **理解数据结构** - 嵌套字典的概念
2. **看 Add 方法** - 理解自动创建子字典的逻辑
3. **看 TryGetValue** - 理解两级查找
4. **看 Remove** - 理解自动清理空字典

### 最值得学习的技术点

1. **继承扩展**: 继承 Dictionary 并添加便捷方法
2. **自动创建**: Add 时自动创建中间层字典
3. **自动清理**: Remove 后自动清理空子字典
4. **泛型设计**: 三个泛型参数支持任意类型组合

---

## 使用示例

### 示例 1: Manager 注册表

```csharp
// 类型 + 名称 → Manager 实例
var managers = new UnOrderDoubleKeyDictionary<Type, string, object>();

// 注册
managers.Add(typeof(UIManager), "", new UIManager());
managers.Add(typeof(UIManager), "Popup", new PopupManager());
managers.Add(typeof(ConfigManager), "", new ConfigManager());

// 获取
managers.TryGetValue(typeof(UIManager), "", out var ui);
managers.TryGetValue(typeof(UIManager), "Popup", out var popup);

// 移除
managers.Remove(typeof(UIManager), "Popup");
```

### 示例 2: 玩家数据索引

```csharp
// 玩家 ID + 数据类型 → 数据对象
var playerData = new UnOrderDoubleKeyDictionary<long, DataType, object>();

playerData.Add(1001, DataType.Gold, 1000);
playerData.Add(1001, DataType.Diamond, 100);
playerData.Add(1002, DataType.Gold, 2000);

// 获取玩家 1001 的金币
playerData.TryGetValue(1001, DataType.Gold, out var gold);
```

---

## 性能特点

| 操作 | 时间复杂度 | 说明 |
|------|-----------|------|
| Add | O(1) | 字典插入 |
| TryGetValue | O(1) | 两次字典查找 |
| Remove | O(1) | 字典删除 + 可能的清理 |
| ContainSubKey | O(1) | 字典 ContainsKey |

---

## 相关文档

- [ManagerProvider.cs.md](../Manager/ManagerProvider.cs.md) - 主要使用者
- [DoubleMap.cs.md](./DoubleMap.cs.md) - 另一种双键映射实现
- [UnOrderMultiMap.cs.md](./UnOrderMultiMap.cs.md) - 多值映射

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
