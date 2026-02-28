# DictionaryComponent.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|------|
| **文件名** | DictionaryComponent.cs |
| **路径** | Assets/Scripts/Mono/Core/Object/DictionaryComponent.cs |
| **所属模块** | 框架层 → Mono/Core/Object |
| **文件职责** | 提供可对象池复用的 Dictionary 组件，减少 GC 压力 |

---

## 类/结构体说明

### DictionaryComponent<T, V>

| 属性 | 说明 |
|------|------|
| **职责** | 继承自 `Dictionary<T, V>`，支持对象池复用，自动清理和回收 |
| **泛型参数** | `T` - 键类型，`V` - 值类型 |
| **继承关系** | 继承 `Dictionary<T, V>`，实现 `IDisposable` |
| **实现的接口** | `IDisposable` |

**设计模式**: 对象池模式 + 工厂模式

```csharp
// 创建字典
using (var dict = DictionaryComponent<string, int>.Create())
{
    dict.Add("key1", 1);
    dict.Add("key2", 2);
    
    // 使用字典
    if (dict.TryGetValue("key1", out int value))
    {
        Debug.Log($"Value: {value}");
    }
    
    // using 结束时自动 Clear 并回收到对象池
}
```

---

## 方法说明

### Create

**签名**:
```csharp
public static DictionaryComponent<T, V> Create()
```

**职责**: 从对象池获取 DictionaryComponent 实例

**核心逻辑**:
```
1. 调用 ObjectPool.Instance.Fetch<DictionaryComponent<T, V>>()
2. 返回复用的实例
```

**调用者**: 任何需要临时字典的代码

**使用示例**:
```csharp
var dict = DictionaryComponent<string, int>.Create();
dict.Add("a", 1);
// ... 使用
dict.Dispose();  // 手动回收
```

---

### Dispose

**签名**:
```csharp
public void Dispose()
```

**职责**: 清理字典并回收到对象池

**核心逻辑**:
```
1. 调用 Clear() 清空字典
2. 调用 ObjectPool.Instance.Recycle(this) 回收到对象池
```

**调用者**: using 语句块结束、手动调用

**使用示例**:
```csharp
// 方式 1: using 语句（推荐）
using (var dict = DictionaryComponent<string, int>.Create())
{
    // 使用字典
}  // 自动调用 Dispose()

// 方式 2: 手动调用
var dict = DictionaryComponent<string, int>.Create();
// ... 使用
dict.Dispose();
```

---

## 继承自 Dictionary<T, V> 的方法

DictionaryComponent<T, V> 继承 `Dictionary<T, V>` 的所有方法：

| 方法 | 说明 |
|------|------|
| `Add(TKey key, TValue value)` | 添加键值对 |
| `Remove(TKey key)` | 移除键值对 |
| `Clear()` | 清空字典 |
| `ContainsKey(TKey key)` | 检查是否包含键 |
| `TryGetValue(TKey key, out TValue value)` | 尝试获取值 |
| `Count` | 键值对数量 |
| `this[TKey key]` | 索引器 |

---

## 使用示例

### 示例 1: 临时数据收集

```csharp
// 收集玩家统计数据
using (var stats = DictionaryComponent<string, long>.Create())
{
    stats["kill_count"] = player.KillCount;
    stats["death_count"] = player.DeathCount;
    stats["score"] = player.Score;
    
    // 发送到服务器
    await NetworkManager.Instance.Send("UpdateStats", stats);
}  // 自动清理
```

### 示例 2: 配置数据传递

```csharp
public void ApplyConfig(DictionaryComponent<string, object> config)
{
    if (config.TryGetValue("volume", out var volume))
    {
        SetVolume((float)volume);
    }
    
    if (config.TryGetValue("fullscreen", out var fullscreen))
    {
        SetFullscreen((bool)fullscreen);
    }
}

// 使用
using (var config = DictionaryComponent<string, object>.Create())
{
    config["volume"] = 0.8f;
    config["fullscreen"] = true;
    ApplyConfig(config);
}
```

### 示例 3: 替代 LINQ 减少 GC

```csharp
// ❌ 会产生 GC
var dict = entities.ToDictionary(e => e.Id, e => e.Name);

// ✅ 使用 DictionaryComponent 减少 GC
using (var dict = DictionaryComponent<long, string>.Create())
{
    foreach (var entity in entities)
    {
        dict.Add(entity.Id, entity.Name);
    }
    
    // 使用 dict
}
```

---

## 性能优势

### GC 对比

| 方式 | GC 分配 | 性能 |
|------|--------|------|
| `new Dictionary<T, V>()` | 每次分配 | ⚠️ 高 GC |
| `DictionaryComponent<T, V>.Create()` | 首次分配 | ✅ 零 GC |

### 对象池复用

```csharp
// 循环中创建字典
for (int i = 0; i < 1000; i++)
{
    // ❌ 每次创建新对象，GC 压力大
    var dict1 = new Dictionary<string, int>();
    
    // ✅ 复用对象池，几乎无 GC
    using (var dict2 = DictionaryComponent<string, int>.Create())
    {
        // 使用
    }
}
```

---

## 设计要点

### 为什么需要 DictionaryComponent？

1. **减少 GC**: 频繁创建/销毁 Dictionary 会产生大量垃圾
2. **性能优化**: 对象池复用避免频繁内存分配
3. **自动清理**: Dispose 时自动 Clear，避免内存泄漏
4. **类型安全**: 泛型支持，编译时检查

### 与 ListComponent 对比

| 特性 | ListComponent<T> | DictionaryComponent<T, V> |
|------|------------------|---------------------------|
| **继承** | `List<T>` | `Dictionary<T, V>` |
| **泛型** | 1 个 | 2 个 |
| **用途** | 列表收集 | 键值对映射 |
| **对象池** | ✅ 支持 | ✅ 支持 |

---

## 注意事项

### 1. 必须调用 Dispose

```csharp
// ❌ 错误：忘记 Dispose
var dict = DictionaryComponent<string, int>.Create();
// 使用...
// 字典没有回收，造成对象池泄漏

// ✅ 正确：使用 using
using (var dict = DictionaryComponent<string, int>.Create())
{
    // 使用...
}  // 自动 Dispose
```

### 2. 不要保留引用

```csharp
// ❌ 错误：保留对象池中的字典引用
DictionaryComponent<string, int> cachedDict;

void Wrong()
{
    cachedDict = DictionaryComponent<string, int>.Create();
    cachedDict.Add("key", 1);
    cachedDict.Dispose();  // 回收到池中
    
    // 后续使用会出错
    cachedDict.Add("key2", 2);  // ⚠️ 可能影响其他使用者
}

// ✅ 正确：用后即弃
using (var dict = DictionaryComponent<string, int>.Create())
{
    dict.Add("key", 1);
    // 使用...
}  // 不再使用
```

---

## 相关文档

- [ListComponent.cs.md](./ListComponent.cs.md) - 列表组件（类似设计）
- [ObjectPool.cs.md](./ObjectPool.cs.md) - 对象池实现

---

*文档生成时间：2026-02-28 | OpenClaw AI 助手*
