# CoroutineLockQueueType.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | CoroutineLockQueueType.cs |
| **路径** | Assets/Scripts/Code/Module/CoroutineLock/CoroutineLockQueueType.cs |
| **所属模块** | 框架层 → Code/Module/CoroutineLock |
| **文件职责** | 协程锁队列类型映射 |

---

## 类说明

### CoroutineLockQueueType

| 属性 | 说明 |
|------|------|
| **职责** | 管理同一类型下所有 Key 的队列映射 |
| **类型** | class |
| **继承关系** | 继承自 `MultiMap<long, CoroutineLockQueue>` |

```csharp
public class CoroutineLockQueueType : MultiMap<long, CoroutineLockQueue>
{
    // 协程锁队列类型
}
```

---

## 设计说明

### 继承 MultiMap

`CoroutineLockQueueType` 继承自 `MultiMap<long, CoroutineLockQueue>`，提供：

- **Key**: `long` - 锁键值
- **Value**: `CoroutineLockQueue` - 协程锁队列

### 用途

一个 `CoroutineLockQueueType` 对应一个 `CoroutineLockType`，管理该类型下所有 Key 的队列。

```
CoroutineLockType.Resources (类型 1)
    ↓
CoroutineLockQueueType (管理所有资源锁)
    ├── Key: "texture_001" → CoroutineLockQueue
    ├── Key: "texture_002" → CoroutineLockQueue
    └── Key: "material_001" → CoroutineLockQueue
```

---

## 使用示例

### 示例 1: 创建队列类型

```csharp
// 创建资源锁队列类型
CoroutineLockQueueType resourceQueueType = CoroutineLockQueueType.Create();

// 添加到管理器列表
coroutineLockManager.list.Add(resourceQueueType);
```

### 示例 2: 添加队列

```csharp
// 为某个 Key 创建队列
long key = "texture_001".GetHashCode();
CoroutineLockQueue queue = CoroutineLockQueue.Create();
resourceQueueType.Add(key, queue);
```

### 示例 3: 获取队列

```csharp
// 获取指定 Key 的队列
if (resourceQueueType.TryGetValue(key, out var queue))
{
    // 添加到队列等待
    queue.Add(tcs, timeout);
}
```

### 示例 4: 移除队列

```csharp
// 队列为空时移除
if (queue.Count == 0)
{
    resourceQueueType.Remove(key);
    queue.Dispose();
}
```

---

## 与相关类的关系

```
CoroutineLockManager
    ├── list: List<CoroutineLockQueueType>
    │   ├── [0] CoroutineLockQueueType (None)
    │   ├── [1] CoroutineLockQueueType (Resources)
    │   ├── [2] CoroutineLockQueueType (UIManager)
    │   └── ...
    │
    └── 每个 CoroutineLockQueueType
        └── MultiMap<long, CoroutineLockQueue>
            ├── Key1 → CoroutineLockQueue
            ├── Key2 → CoroutineLockQueue
            └── ...
```

---

## 相关文档

- [CoroutineLockManager.cs.md](./CoroutineLockManager.cs.md) - 协程锁管理器
- [CoroutineLockQueue.cs.md](./CoroutineLockQueue.cs.md) - 协程锁队列
- [CoroutineLockType.cs.md](./CoroutineLockType.cs.md) - 协程锁类型

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
