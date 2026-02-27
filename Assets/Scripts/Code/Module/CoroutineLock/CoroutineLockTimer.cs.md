# CoroutineLockTimer.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | CoroutineLockTimer.cs |
| **路径** | Assets/Scripts/Code/Module/CoroutineLock/CoroutineLockTimer.cs |
| **所属模块** | 框架层 → Code/Module/CoroutineLock |
| **文件职责** | 协程锁定时器数据结构 |

---

## 类说明

### CoroutineLockTimer

| 属性 | 说明 |
|------|------|
| **职责** | 存储协程锁的超时定时器信息 |
| **类型** | class |

```csharp
public class CoroutineLockTimer
{
    public CoroutineLock CoroutineLock { get; }
    public long CoroutineLockInstanceId { get; }
}
```

---

## 字段说明

| 字段 | 类型 | 说明 |
|------|------|------|
| `CoroutineLock` | `CoroutineLock` | 关联的协程锁实例 |
| `CoroutineLockInstanceId` | `long` | 协程锁实例 ID（用于验证） |

---

## 设计说明

### 为什么需要 InstanceId？

在超时检查时，协程锁可能已经被释放并回收到对象池。InstanceId 用于验证定时器关联的协程锁是否仍然有效。

```csharp
// 超时检查
CoroutineLockTimer timer = timerOutTimer.Dequeue();

// 验证 InstanceId 是否匹配
if (timer.CoroutineLockInstanceId != timer.CoroutineLock.InstanceId)
{
    // 协程锁已被释放，跳过
    continue;
}

// InstanceId 匹配，执行超时处理
CoroutineLock coroutineLock = timer.CoroutineLock;
CoroutineLockManager.Instance.RunNextCoroutine(...);
```

---

## 使用示例

### 示例 1: 创建定时器

```csharp
// 创建协程锁
CoroutineLock coroutineLock = CoroutineLock.Create(type, key, level);

// 创建定时器
CoroutineLockTimer timer = new CoroutineLockTimer(coroutineLock);

// 添加到超时映射
long timeoutTime = TimeInfo.Instance.ClientFrameTime() + timeout;
coroutineLockManager.timers.Add(timeoutTime, timer);
```

### 示例 2: 超时检查

```csharp
// 获取当前时间
long timeNow = TimeInfo.Instance.ClientFrameTime();

// 遍历超时定时器
foreach (var item in coroutineLockManager.timers)
{
    if (item.Key > timeNow)
    {
        break;  // 还未超时
    }
    
    // 超时，添加到处理队列
    coroutineLockManager.timeOutIds.Enqueue(item.Key);
}

// 处理超时的
while (coroutineLockManager.timeOutIds.Count > 0)
{
    long time = coroutineLockManager.timeOutIds.Dequeue();
    var list = coroutineLockManager.timers[time];
    
    foreach (var timer in list)
    {
        // 验证 InstanceId
        if (timer.CoroutineLockInstanceId != timer.CoroutineLock.InstanceId)
        {
            continue;  // 已失效
        }
        
        // 执行下一个协程
        CoroutineLockManager.Instance.RunNextCoroutine(
            timer.CoroutineLock.coroutineLockType,
            timer.CoroutineLock.key,
            timer.CoroutineLock.level + 1
        );
    }
}
```

---

## 生命周期

```
创建 CoroutineLock
    ↓
创建 CoroutineLockTimer
    ↓
添加到 timers 映射
    ↓
等待超时
    ↓
超时检查
    ↓
验证 InstanceId
    ↓
执行下一个协程
    ↓
定时器失效
```

---

## 相关文档

- [CoroutineLockManager.cs.md](./CoroutineLockManager.cs.md) - 协程锁管理器
- [CoroutineLock.cs.md](./CoroutineLock.cs.md) - 协程锁

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
