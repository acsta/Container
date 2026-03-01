# HashSetComponent.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | HashSetComponent.cs |
| **路径** | Assets/Scripts/Mono/Core/Object/HashSetComponent.cs |
| **所属模块** | Mono 层 → Core/Object |
| **文件职责** | 可回收的 HashSet 组件，基于对象池 |

---

## 类说明

### HashSetComponent\<T\>

| 属性 | 说明 |
|------|------|
| **职责** | 继承自 HashSet 的可回收集合组件 |
| **继承关系** | HashSet<T>, IDisposable |
| **设计模式** | 对象池模式 |

```csharp
public class HashSetComponent<T> : HashSet<T>, IDisposable
{
    // 从对象池获取实例
    public static HashSetComponent<T> Create()
    
    // 回收到对象池
    public void Dispose()
}
```

---

## 方法说明

### Create()

**签名**:
```csharp
public static HashSetComponent<T> Create()
```

**职责**: 从对象池获取 HashSet 实例

**返回**: 新的或复用的 HashSet 组件实例

**核心逻辑**:
```
1. 调用 ObjectPool.Instance.Fetch<HashSetComponent<T>>()
2. 返回实例
```

**使用示例**:
```csharp
// 创建 HashSet 组件
var set = HashSetComponent<int>.Create();
set.Add(1);
set.Add(2);
set.Add(3);
```

---

### Dispose()

**签名**:
```csharp
public void Dispose()
```

**职责**: 清理并回收到对象池

**核心逻辑**:
```
1. 调用 Clear() 清空集合
2. 调用 ObjectPool.Instance.Recycle(this) 回收
```

**使用示例**:
```csharp
using (var set = HashSetComponent<string>.Create())
{
    set.Add("item1");
    set.Add("item2");
    // 使用集合
} // 自动调用 Dispose() 回收
```

---

## 使用场景

### 场景 1: 临时集合（using 语句）

```csharp
// 自动回收
using (var uniqueIds = HashSetComponent<long>.Create())
{
    uniqueIds.Add(1001);
    uniqueIds.Add(1002);
    
    if (uniqueIds.Contains(1001))
    {
        // 处理
    }
} // 自动清理和回收
```

### 场景 2: 去重检查

```csharp
// 检查重复 ID
using (var idSet = HashSetComponent<int>.Create())
{
    foreach (var item in items)
    {
        if (!idSet.Add(item.Id))
        {
            // 发现重复
            Debug.LogWarning($"Duplicate ID: {item.Id}");
        }
    }
}
```

### 场景 3: 频繁创建销毁的场景

```csharp
// 每帧处理临时数据
void Update()
{
    using (var visibleEnemies = HashSetComponent<Enemy>.Create())
    {
        // 收集可见敌人
        CollectVisibleEnemies(visibleEnemies);
        
        // 处理
        ProcessEnemies(visibleEnemies);
    } // 自动回收，避免 GC
}
```

### 场景 4: 集合运算

```csharp
using (var setA = HashSetComponent<int>.Create())
using (var setB = HashSetComponent<int>.Create())
{
    setA.Add(1);
    setA.Add(2);
    setA.Add(3);
    
    setB.Add(2);
    setB.Add(3);
    setB.Add(4);
    
    // 交集
    setA.IntersectWith(setB);  // {2, 3}
    
    // 并集
    setA.UnionWith(setB);      // {1, 2, 3, 4}
    
    // 差集
    setA.ExceptWith(setB);     // {1}
}
```

---

## 优势

### 1. 减少 GC 压力

```csharp
// 传统方式（每帧产生 GC）
void Update()
{
    var set = new HashSet<int>();
    // ...
}

// 使用 HashSetComponent（无 GC）
void Update()
{
    using (var set = HashSetComponent<int>.Create())
    {
        // ...
    }
}
```

### 2. 自动清理

Dispose() 自动调用 Clear()，避免数据污染

### 3. 类型安全

泛型设计，编译时类型检查

### 4. 继承 HashSet 全部功能

```csharp
// 所有 HashSet 方法都可用
set.Add(item);
set.Remove(item);
set.Contains(item);
set.IntersectWith(other);
set.UnionWith(other);
set.ExceptWith(other);
set.SymmetricExceptWith(other);
set.IsSubsetOf(other);
set.IsSupersetOf(other);
```

---

## 注意事项

### 1. 必须回收

```csharp
// ❌ 错误：不回收导致内存泄漏
var set = HashSetComponent<int>.Create();
// 忘记 Dispose

// ✅ 正确：使用 using 语句
using (var set = HashSetComponent<int>.Create())
{
    // 使用
}
```

### 2. 回收后勿用

```csharp
var set = HashSetComponent<int>.Create();
set.Dispose();
set.Add(100); // ❌ 错误：已回收的实例不应再使用
```

### 3. 不适合长期持有

对象池适合短期临时使用，长期持有的数据应使用普通 HashSet

---

## 与对象池的配合

```
ObjectPool
    ├── HashSetComponent<int> (池化)
    ├── HashSetComponent<string> (池化)
    ├── HashSetComponent<Player> (池化)
    └── HashSetComponent<...> (池化)
```

每个泛型组合有独立的对象池。

---

## 相关文档

- [ObjectPool.cs.md](../ObjectPool.cs.md) - 对象池
- [DictionaryComponent.cs.md](./DictionaryComponent.cs.md) - 字典组件
- [ListComponent.cs.md](./ListComponent.cs.md) - 列表组件

---

*文档生成时间：2026-03-01 | OpenClaw AI 助手*
