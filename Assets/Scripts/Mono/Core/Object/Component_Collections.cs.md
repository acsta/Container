# 集合组件 (Component) 综合文档

> **覆盖文件**: ListComponent, DictionaryComponent, HashSetComponent, LinkedListComponent  
> **生成时间**: 2026-02-28  
> **命名空间**: `TaoTie`

---

## 📑 目录

1. [概述](#1-概述)
2. [ListComponent](#2-listcomponent)
3. [DictionaryComponent](#3-dictionarycomponent)
4. [HashSetComponent](#4-hashsetcomponent)
5. [LinkedListComponent](#5-linkedlistcomponent)
6. [使用示例](#6-使用示例)

---

## 1. 概述

### 设计理念

所有 `Component` 类都是对 .NET 标准集合类的包装，提供**对象池支持**，通过 `IDisposable` 接口实现自动回收。

**继承关系**:
```
ListComponent<T>      : List<T>,      IDisposable
DictionaryComponent<T,V> : Dictionary<T,V>, IDisposable
HashSetComponent<T>   : HashSet<T>,   IDisposable
LinkedListComponent<T>: LinkedList<T>, IDisposable
```

**核心优势**:
- ✅ 无缝继承标准集合类的所有方法
- ✅ 通过 `Create()` 从对象池获取
- ✅ 通过 `Dispose()` 自动回收
- ✅ 支持 `using` 语句自动管理生命周期

---

## 2. ListComponent

### 类说明

| 属性 | 说明 |
|------|------|
| **定义** | `public class ListComponent<T> : List<T>, IDisposable` |
| **职责** | 池化的 List，自动管理内存 |
| **泛型参数** | `T` - 列表元素类型 |

### API

```csharp
// 创建（从对象池获取）
public static ListComponent<T> Create()

// 回收（清理并返回对象池）
public void Dispose()
```

### 使用示例

```csharp
// ✅ 推荐：使用 using 自动回收
using (var list = ListComponent<int>.Create())
{
    list.Add(1);
    list.Add(2);
    list.Add(3);
    
    foreach (var item in list)
    {
        Log.Info(item);
    }
} // 自动调用 Dispose()

// ✅ 手动管理
var list = ListComponent<string>.Create();
try
{
    list.Add("Hello");
    list.Add("World");
}
finally
{
    list.Dispose();
}

// ❌ 错误：忘记回收（内存泄漏）
var list = ListComponent<int>.Create();
list.Add(1);
// 没有调用 Dispose()，对象未回收
```

### 继承的常用方法

```csharp
var list = ListComponent<int>.Create();

// List 标准方法
list.Add(1);
list.AddRange(new[] { 2, 3, 4 });
list.Remove(1);
list.RemoveAt(0);
list.Contains(2);
list.IndexOf(3);
list.Insert(0, 100);
list.Sort();
list.Reverse();
list.Clear();

// 属性
int count = list.Count;
int capacity = list.Capacity;
var item = list[0];
```

---

## 3. DictionaryComponent

### 类说明

| 属性 | 说明 |
|------|------|
| **定义** | `public class DictionaryComponent<T,V> : Dictionary<T,V>, IDisposable` |
| **职责** | 池化的 Dictionary，自动管理内存 |
| **泛型参数** | `T` - 键类型, `V` - 值类型 |

### API

```csharp
// 创建（从对象池获取）
public static DictionaryComponent<T, V> Create()

// 回收（清理并返回对象池）
public void Dispose()
```

### 使用示例

```csharp
using (var dict = DictionaryComponent<string, int>.Create())
{
    dict["key1"] = 100;
    dict["key2"] = 200;
    
    if (dict.TryGetValue("key1", out var value))
    {
        Log.Info($"Value: {value}");
    }
} // 自动回收
```

### 继承的常用方法

```csharp
var dict = DictionaryComponent<string, int>.Create();

// Dictionary 标准方法
dict.Add("key", 100);
dict["key"] = 200; // 设置/修改
dict.Remove("key");
dict.Clear();
dict.ContainsKey("key");
dict.ContainsValue(100);
dict.TryGetValue("key", out var value);

// 属性
int count = dict.Count;
var keys = dict.Keys;
var values = dict.Values;
```

---

## 4. HashSetComponent

### 类说明

| 属性 | 说明 |
|------|------|
| **定义** | `public class HashSetComponent<T> : HashSet<T>, IDisposable` |
| **职责** | 池化的 HashSet，自动管理内存 |
| **泛型参数** | `T` - 集合元素类型 |

### API

```csharp
// 创建（从对象池获取）
public static HashSetComponent<T> Create()

// 回收（清理并返回对象池）
public void Dispose()
```

### 使用示例

```csharp
using (var set = HashSetComponent<int>.Create())
{
    set.Add(1);
    set.Add(2);
    set.Add(2); // 重复，不会添加
    
    Log.Info($"Count: {set.Count}"); // 输出：2
} // 自动回收
```

### 继承的常用方法

```csharp
var set = HashSetComponent<string>.Create();

// HashSet 标准方法
set.Add("item");
set.Remove("item");
set.Clear();
set.Contains("item");

// 集合操作
var set2 = HashSetComponent<int>.Create();
set.UnionWith(set2);        // 并集
set.IntersectWith(set2);    // 交集
set.ExceptWith(set2);       // 差集
set.SymmetricExceptWith(set2); // 对称差集
set.IsSubsetOf(set2);       // 是否子集
set.IsSupersetOf(set2);     // 是否超集
```

---

## 5. LinkedListComponent

### 类说明

| 属性 | 说明 |
|------|------|
| **定义** | `public class LinkedListComponent<T> : LinkedList<T>, IDisposable` |
| **职责** | 池化的 LinkedList，自动管理内存 |
| **泛型参数** | `T` - 节点元素类型 |

### API

```csharp
// 创建（从对象池获取）
public static LinkedListComponent<T> Create()

// 回收（清理并返回对象池）
public void Dispose()
```

### 使用示例

```csharp
using (var list = LinkedListComponent<int>.Create())
{
    list.AddFirst(1);
    list.AddLast(2);
    list.AddLast(3);
    
    // 遍历
    foreach (var item in list)
    {
        Log.Info(item);
    }
} // 自动回收
```

### 继承的常用方法

```csharp
var list = LinkedListComponent<string>.Create();

// 添加节点
list.AddFirst("first");
list.AddLast("last");
list.AddAfter(list.First, "second");
list.AddBefore(list.Last, "third");

// 移除节点
list.Remove("first");
list.RemoveFirst();
list.RemoveLast();
list.Clear();

// 访问
var first = list.First;
var last = list.Last;
int count = list.Count;

// 遍历
foreach (var item in list) { }
for (var node = list.First; node != null; node = node.Next) { }
```

---

## 6. 使用示例

### 示例 1: 战斗伤害计算

```csharp
public class BattleSystem
{
    // 计算多个目标的伤害
    public void CalculateDamages(List<Enemy> enemies, int baseDamage)
    {
        using (var damages = ListComponent<int>.Create())
        {
            // 计算每个敌人的伤害
            foreach (var enemy in enemies)
            {
                int damage = Mathf.FloorToInt(baseDamage * enemy.DefenseMultiplier);
                damages.Add(damage);
            }
            
            // 应用伤害
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].TakeDamage(damages[i]);
            }
        } // damages 自动回收
    }
}
```

### 示例 2: 数据分组

```csharp
public class DataGrouping
{
    // 按类型分组数据
    public void GroupItems(List<Item> items)
    {
        using (var groups = DictionaryComponent<int, ListComponent<Item>>.Create())
        {
            // 分组
            foreach (var item in items)
            {
                if (!groups.ContainsKey(item.TypeId))
                {
                    groups[item.TypeId] = ListComponent<Item>.Create();
                }
                groups[item.TypeId].Add(item);
            }
            
            // 处理每个组
            foreach (var kvp in groups)
            {
                Log.Info($"Type {kvp.Key}: {kvp.Value.Count} items");
                ProcessGroup(kvp.Value);
            }
        } // groups 和所有内部 ListComponent 都会回收
    }
    
    private void ProcessGroup(ListComponent<Item> group)
    {
        // 处理逻辑...
    }
}
```

### 示例 3: 去重处理

```csharp
public class UniqueProcessor
{
    // 处理唯一 ID 列表
    public void ProcessUniqueIds(List<int> allIds)
    {
        using (var uniqueIds = HashSetComponent<int>.Create())
        {
            // 去重
            foreach (var id in allIds)
            {
                uniqueIds.Add(id);
            }
            
            Log.Info($"Unique count: {uniqueIds.Count}");
            
            // 处理唯一 ID
            foreach (var id in uniqueIds)
            {
                ProcessId(id);
            }
        }
    }
    
    private void ProcessId(int id)
    {
        // 处理逻辑...
    }
}
```

### 示例 4: 任务队列

```csharp
public class TaskQueue
{
    private LinkedListComponent<Task> taskQueue;
    
    public TaskQueue()
    {
        taskQueue = LinkedListComponent<Task>.Create();
    }
    
    public void AddTask(Task task)
    {
        taskQueue.AddLast(task);
    }
    
    public void ProcessTasks()
    {
        while (taskQueue.Count > 0)
        {
            var task = taskQueue.First.Value;
            taskQueue.RemoveFirst();
            
            task.Execute();
        }
    }
    
    public void Clear()
    {
        taskQueue.Clear();
    }
    
    public void Dispose()
    {
        taskQueue?.Dispose();
        taskQueue = null;
    }
}
```

### 示例 5: 嵌套使用（注意回收顺序）

```csharp
public class NestedExample
{
    public void ProcessData()
    {
        // 外层字典
        using (var outerDict = DictionaryComponent<string, DictionaryComponent<int, string>>.Create())
        {
            // 内层字典
            var innerDict1 = DictionaryComponent<int, string>.Create();
            innerDict1[1] = "One";
            innerDict1[2] = "Two";
            
            var innerDict2 = DictionaryComponent<int, string>.Create();
            innerDict2[3] = "Three";
            innerDict2[4] = "Four";
            
            outerDict["Group1"] = innerDict1;
            outerDict["Group2"] = innerDict2;
            
            // 使用...
            foreach (var kvp in outerDict)
            {
                Log.Info($"Group: {kvp.Key}");
                foreach (var inner in kvp.Value)
                {
                    Log.Info($"  {inner.Key}: {inner.Value}");
                }
            }
            
            // ⚠️ 注意：需要手动回收内层对象
            foreach (var innerDict in outerDict.Values)
            {
                innerDict.Dispose();
            }
        } // 外层字典自动回收
    }
}
```

---

## ⚠️ 注意事项

### 1. 必须回收

```csharp
// ❌ 错误：忘记回收
var list = ListComponent<int>.Create();
list.Add(1);
// 没有 Dispose，内存泄漏！

// ✅ 正确：使用 using
using (var list = ListComponent<int>.Create())
{
    list.Add(1);
} // 自动回收

// ✅ 正确：手动回收
var list = ListComponent<int>.Create();
try
{
    list.Add(1);
}
finally
{
    list.Dispose();
}
```

### 2. 嵌套对象回收

```csharp
// ❌ 错误：只回收外层
using (var dict = DictionaryComponent<string, ListComponent<int>>.Create())
{
    dict["list1"] = ListComponent<int>.Create();
    dict["list2"] = ListComponent<int>.Create();
    // 使用...
} // 只回收了 dict，内部的 ListComponent 未回收！

// ✅ 正确：先回收内层
using (var dict = DictionaryComponent<string, ListComponent<int>>.Create())
{
    dict["list1"] = ListComponent<int>.Create();
    dict["list2"] = ListComponent<int>.Create();
    
    try
    {
        // 使用...
    }
    finally
    {
        // 先回收内层
        foreach (var list in dict.Values)
        {
            list.Dispose();
        }
    }
}
```

### 3. 不要在回收后继续使用

```csharp
var list = ListComponent<int>.Create();
list.Add(1);
list.Dispose();

// ❌ 错误：回收后继续使用
list.Add(2); // 未定义行为，可能导致 bug
```

---

## 性能对比

### 不使用 Component

```csharp
void Update()
{
    // 每帧创建新 List，产生 GC
    var list = new List<int>();
    list.Add(1);
    // ...
} // list 被 GC 回收
```

**GC 压力**: 每帧产生垃圾

### 使用 Component

```csharp
void Update()
{
    using (var list = ListComponent<int>.Create())
    {
        list.Add(1);
        // ...
    } // 回收到对象池，无 GC
}
```

**GC 压力**: 0（首次创建后复用）

---

## 相关文档

- [ObjectPool.cs.md](../ObjectPool.cs.md) - 对象池核心
- [MultiMap.cs.md](./MultiMap.cs.md) - 多重映射
- [UnOrderMultiMap.cs.md](./UnOrderMultiMap.cs.md) - 无序多重映射

---

*文档由 OpenClaw AI 助手自动生成 | 基于静态代码分析*
