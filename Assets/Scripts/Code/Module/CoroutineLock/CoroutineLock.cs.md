# CoroutineLock.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | CoroutineLock.cs |
| **路径** | Assets/Scripts/Code/Module/CoroutineLock/CoroutineLock.cs |
| **所属模块** | 框架层 → Code/Module/CoroutineLock |
| **文件职责** | 协程锁数据结构，表示一个锁实例 |

---

## 类/结构体说明

### CoroutineLock

| 属性 | 说明 |
|------|------|
| **职责** | 表示一个协程锁实例，使用 using 自动释放 |
| **泛型参数** | 无 |
| **继承关系** | 无 |
| **实现的接口** | `IDisposable` |

```csharp
public class CoroutineLock : IDisposable
{
    // 协程锁
}
```

---

## 字段与属性

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `coroutineLockType` | `int` | `public` | 协程锁类型 |
| `key` | `long` | `public` | 锁键值 |
| `level` | `int` | `public` | 递归层级 |
| `InstanceId` | `long` | `public` | 实例 ID |

---

## 方法说明

### Create(int type, long k, int count)

**签名**:
```csharp
public static CoroutineLock Create(int type, long k, int count)
```

**职责**: 创建协程锁

**参数**:
- `type`: 协程锁类型
- `k`: 锁键值
- `count`: 层级

**返回**: 新的 CoroutineLock 实例

**核心逻辑**:
```
1. 从对象池获取实例
2. 设置 coroutineLockType = type
3. 设置 key = k
4. 设置 level = count
5. 生成 InstanceId
6. 返回实例
```

**调用者**: `CoroutineLockManager.Wait()`, `CoroutineLockManager.Notify()`

---

### Dispose()

**签名**:
```csharp
public void Dispose()
```

**职责**: 释放协程锁（执行下一个）

**核心逻辑**:
```
1. 如果 coroutineLockType != None：
   - 调用 RunNextCoroutine 执行下一个
2. 否则：
   - 记录错误（超时）
3. 清空字段
4. 回收到对象池
```

**超时处理**:
```csharp
if (this.coroutineLockType != CoroutineLockType.None)
{
    CoroutineLockManager.Instance.RunNextCoroutine(...);
}
else
{
    // 超时了
    Log.Error($"coroutine lock timeout: {this.coroutineLockType} {this.key} {this.level}");
}
```

**调用者**: using 语句块结束时

---

## 阅读指引

### 建议的阅读顺序

1. **理解锁作用** - CoroutineLock 有什么用
2. **看 Create** - 理解如何创建
3. **看 Dispose** - 理解如何释放
4. **了解 using 模式** - 理解自动释放

### 最值得学习的技术点

1. **对象池**: 从对象池获取/回收
2. **using 模式**: 自动释放锁
3. **超时检测**: coroutineLockType=None 表示超时
4. **链式执行**: Dispose 触发下一个协程

---

## 使用示例

### 示例 1: 基础用法

```csharp
// 等待获取锁
using (await CoroutineLockManager.Instance.Wait(CoroutineLockType.Player, playerId))
{
    // 临界区代码
    await UpdatePlayerData();
}
// using 结束自动调用 Dispose()，执行下一个协程
```

### 示例 2: 嵌套锁

```csharp
// 外层锁
using (await CoroutineLockManager.Instance.Wait(CoroutineLockType.Auction, auctionId))
{
    // 内层锁
    using (await CoroutineLockManager.Instance.Wait(CoroutineLockType.Player, playerId))
    {
        // 同时持有两个锁
        await ProcessAuction();
    }
    // 内层锁释放
}
// 外层锁释放
```

### 示例 3: 超时处理

```csharp
// 设置超时时间（5 秒）
using (await CoroutineLockManager.Instance.Wait(
    CoroutineLockType.Resource, 
    resourceId, 
    time: 5000
))
{
    // 如果 5 秒内未获取到锁，会超时
    await LoadResource();
}
```

### 示例 4: 手动释放（不推荐）

```csharp
var lock = await CoroutineLockManager.Instance.Wait(CoroutineLockType.Player, playerId);
try
{
    await UpdatePlayerData();
}
finally
{
    lock.Dispose();  // 手动释放（推荐用 using）
}
```

---

## 生命周期

```
创建 → Create()
  ↓
使用 → using 语句块
  ↓
释放 → Dispose()
  ↓
回收 → 对象池
```

---

## 相关文档

- [CoroutineLockManager.cs.md](./CoroutineLockManager.cs.md) - 协程锁管理器
- [CoroutineLockQueue.cs.md](./CoroutineLockQueue.cs.md) - 协程锁队列

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
