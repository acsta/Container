# CoroutineLockQueue.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | CoroutineLockQueue.cs |
| **路径** | Assets/Scripts/Code/Module/CoroutineLock/CoroutineLockQueue.cs |
| **所属模块** | 框架层 → Code/Module/CoroutineLock |
| **文件职责** | 协程锁队列数据结构 |

---

## 类/结构体说明

### CoroutineLockInfo

| 属性 | 说明 |
|------|------|
| **职责** | 存储队列中单个等待协程的信息 |
| **类型** | struct |

```csharp
public struct CoroutineLockInfo
{
    public ETTask<CoroutineLock> Tcs;
    public int Time;
}
```

### CoroutineLockQueue

| 属性 | 说明 |
|------|------|
| **职责** | 管理等待同一把锁的协程队列 |
| **类型** | class |
| **实现的接口** | `IDisposable` |

```csharp
public class CoroutineLockQueue : IDisposable
{
    // 协程锁队列
}
```

---

## 字段说明

### CoroutineLockInfo 字段

| 字段 | 类型 | 说明 |
|------|------|------|
| `Tcs` | `ETTask<CoroutineLock>` | 等待锁的任务完成源 |
| `Time` | `int` | 超时时间（毫秒） |

### CoroutineLockQueue 字段

| 字段 | 类型 | 说明 |
|------|------|------|
| `queue` | `Queue<CoroutineLockInfo>` | 等待队列 |
| `Count` | `int` | 队列长度（只读属性） |

---

## 方法说明

### Create()

**签名**:
```csharp
public static CoroutineLockQueue Create()
```

**职责**: 从对象池获取队列

**返回**: 新的 CoroutineLockQueue 实例

**核心逻辑**:
```
1. 从 ObjectPool 获取
2. 返回复用的队列
```

**调用者**: CoroutineLockManager

---

### Dispose()

**签名**:
```csharp
public void Dispose()
```

**职责**: 释放队列（回收到对象池）

**核心逻辑**:
```
1. 清空队列
2. 回收到 ObjectPool
```

**调用者**: CoroutineLockManager（队列为空时）

---

### Add(ETTask<CoroutineLock> tcs, int time)

**签名**:
```csharp
public void Add(ETTask<CoroutineLock> tcs, int time)
```

**职责**: 添加等待协程到队列

**参数**:
- `tcs`: 等待锁的任务
- `time`: 超时时间（毫秒）

**核心逻辑**:
```
1. 创建 CoroutineLockInfo
2. 入队
```

**调用者**: CoroutineLock.WaitAsync()

---

### Dequeue()

**签名**:
```csharp
public CoroutineLockInfo Dequeue()
```

**职责**: 出队第一个等待协程

**返回**: 队列头部的协程信息

**核心逻辑**:
```
1. 出队
2. 返回
```

**调用者**: CoroutineLockManager.RunNextCoroutine()

---

## 使用示例

### 示例 1: 创建队列

```csharp
// 创建协程锁队列
CoroutineLockQueue queue = CoroutineLockQueue.Create();

// 添加到管理器
coroutineLockQueueType.Add(key, queue);
```

### 示例 2: 添加等待协程

```csharp
// 协程等待锁
ETTask<CoroutineLock> tcs = ETTask<CoroutineLock>.Create(true);

// 添加到队列
queue.Add(tcs, timeout: 5000);

// 等待锁
CoroutineLock coroutineLock = await tcs;
```

### 示例 3: 执行下一个协程

```csharp
// 当前协程释放锁
if (queue.Count > 0)
{
    // 出队下一个等待者
    CoroutineLockInfo info = queue.Dequeue();
    
    // 完成等待任务
    info.Tcs.SetResult(coroutineLock);
    
    // 如果队列为空，回收队列
    if (queue.Count == 0)
    {
        queue.Dispose();
        coroutineLockQueueType.Remove(key);
    }
}
```

---

## 队列生命周期

```
创建队列 (Create)
    ↓
添加等待协程 (Add)
    ↓
    ├── 协程 1 等待
    ├── 协程 2 等待
    └── 协程 3 等待
    ↓
执行下一个 (Dequeue)
    ↓
    ├── 协程 1 获得锁
    ├── 协程 2 等待
    └── 协程 3 等待
    ↓
队列空？
├─ 是 → Dispose() 回收
└─ 否 → 继续等待
```

---

## 与相关类的关系

```
CoroutineLockManager
    ├── CoroutineLockQueueType (类型映射)
    │   └── MultiMap<long, CoroutineLockQueue>
    │       └── CoroutineLockQueue (队列)
    │           └── Queue<CoroutineLockInfo>
    │               └── CoroutineLockInfo
    │                   ├── Tcs: ETTask<CoroutineLock>
    │                   └── Time: int
```

---

## 相关文档

- [CoroutineLockManager.cs.md](./CoroutineLockManager.cs.md) - 协程锁管理器
- [CoroutineLock.cs.md](./CoroutineLock.cs.md) - 协程锁
- [CoroutineLockQueueType.cs.md](./CoroutineLockQueueType.cs.md) - 协程锁队列类型

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
