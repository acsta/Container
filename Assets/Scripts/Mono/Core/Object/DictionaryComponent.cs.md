# DictionaryComponent.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | DictionaryComponent.cs |
| **路径** | Assets/Scripts/Mono/Core/Object/DictionaryComponent.cs |
| **所属模块** | Mono 层 → Core/Object |
| **文件职责** | 可回收的字典组件，基于对象池 |

---

## 类说明

### DictionaryComponent\<T, V\>

| 属性 | 说明 |
|------|------|
| **职责** | 继承自 Dictionary 的可回收字典组件 |
| **继承关系** | Dictionary<T, V>, IDisposable |
| **设计模式** | 对象池模式 |

```csharp
public class DictionaryComponent<T,V> : Dictionary<T,V>, IDisposable
{
    // 从对象池获取实例
    public static DictionaryComponent<T,V> Create()
    
    // 回收到对象池
    public void Dispose()
}
```

---

## 方法说明

### Create()

**签名**:
```csharp
public static DictionaryComponent<T,V> Create()
```

**职责**: 从对象池获取字典实例

**返回**: 新的或复用的字典组件实例

**核心逻辑**:
```
1. 调用 ObjectPool.Instance.Fetch<DictionaryComponent<T,V>>()
2. 返回实例
```

**使用示例**:
```csharp
// 创建字典组件
var dict = DictionaryComponent<string, int>.Create();
dict.Add("key1", 100);
dict.Add("key2", 200);
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
1. 调用 Clear() 清空字典
2. 调用 ObjectPool.Instance.Recycle(this) 回收
```

**使用示例**:
```csharp
using (var dict = DictionaryComponent<string, int>.Create())
{
    dict.Add("key", 100);
    // 使用字典
} // 自动调用 Dispose() 回收
```

---

## 使用场景

### 场景 1: 临时字典（using 语句）

```csharp
// 自动回收
using (var tempDict = DictionaryComponent<string, int>.Create())
{
    tempDict["score"] = 100;
    tempDict["level"] = 5;
    
    ProcessData(tempDict);
} // 自动清理和回收
```

### 场景 2: 手动管理

```csharp
// 手动创建和回收
var dict = DictionaryComponent<int, string>.Create();
try
{
    dict[1] = "player1";
    dict[2] = "player2";
    
    // 使用字典
}
finally
{
    dict.Dispose(); // 手动回收
}
```

### 场景 3: 频繁创建销毁的场景

```csharp
// 每帧处理临时数据
void Update()
{
    using (var tempData = DictionaryComponent<string, float>.Create())
    {
        // 收集临时数据
        CollectTempData(tempData);
        
        // 处理
        ProcessTempData(tempData);
    } // 自动回收，避免 GC
}
```

---

## 优势

### 1. 减少 GC 压力

```csharp
// 传统方式（每帧产生 GC）
void Update()
{
    var dict = new Dictionary<string, int>();
    // ...
}

// 使用 DictionaryComponent（无 GC）
void Update()
{
    using (var dict = DictionaryComponent<string, int>.Create())
    {
        // ...
    }
}
```

### 2. 自动清理

Dispose() 自动调用 Clear()，避免数据污染

### 3. 类型安全

泛型设计，编译时类型检查

---

## 注意事项

### 1. 必须回收

```csharp
// ❌ 错误：不回收导致内存泄漏
var dict = DictionaryComponent<string, int>.Create();
// 忘记 Dispose

// ✅ 正确：使用 using 语句
using (var dict = DictionaryComponent<string, int>.Create())
{
    // 使用
}
```

### 2. 回收后勿用

```csharp
var dict = DictionaryComponent<string, int>.Create();
dict.Dispose();
dict.Add("key", 100); // ❌ 错误：已回收的实例不应再使用
```

### 3. 不适合长期持有

对象池适合短期临时使用，长期持有的数据应使用普通 Dictionary

---

## 与对象池的配合

```
ObjectPool
    ├── DictionaryComponent<string, int> (池化)
    ├── DictionaryComponent<int, string> (池化)
    └── DictionaryComponent<...> (池化)
```

每个泛型组合有独立的对象池。

---

## 相关文档

- [ObjectPool.cs.md](../ObjectPool.cs.md) - 对象池
- [HashSetComponent.cs.md](./HashSetComponent.cs.md) - HashSet 组件
- [ListComponent.cs.md](./ListComponent.cs.md) - 列表组件

---

*文档生成时间：2026-03-01 | OpenClaw AI 助手*
