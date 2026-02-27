# CoroutineLockManager.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | CoroutineLockManager.cs |
| **路径** | Assets/Scripts/Code/Module/CoroutineLock/CoroutineLockManager.cs |
| **所属模块** | 框架层 → Code/Module/CoroutineLock |
| **文件职责** | 协程锁管理器，管理并发协程的排队执行 |

---

## 类/结构体说明

### CoroutineLockManager

| 属性 | 说明 |
|------|------|
| **职责** | 管理协程锁队列，确保同 Key 的协程顺序执行，支持超时机制 |
| **泛型参数** | 无 |
| **继承关系** | 无 |
| **实现的接口** | `IManager`, `IUpdate` |

```csharp
public class CoroutineLockManager : IManager, IUpdate
{
    // 协程锁管理器
}
```

---

## 字段与属性

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `Instance` | `CoroutineLockManager` | `public static` | 单例实例 |
| `list` | `List<CoroutineLockQueueType>` | `private` | 协程锁队列类型列表 |
| `nextFrameRun` | `Queue<(int, long, int)>` | `private` | 下一帧执行队列 |
| `timers` | `MultiMap<long, CoroutineLockTimer>` | `private` | 超时定时器映射 |
| `timeOutIds` | `Queue<long>` | `private` | 超时 ID 队列 |
| `timerOutTimer` | `Queue<CoroutineLockTimer>` | `private` | 超时定时器队列 |
| `minTime` | `long` | `private` | 最小超时时间 |
| `timeNow` | `long` | `private` | 当前时间 |

---

## 方法说明（按重要程度排序）

### Init() / Destroy()

**签名**:
```csharp
public void Init()
public void Destroy()
```

**职责**: 初始化和销毁

**核心逻辑**:
```
// Init:
1. 设置 Instance = this
2. 创建 CoroutineLockQueueType 列表（按 CoroutineLockType.Max）

// Destroy:
1. 清空 Instance
2. 释放所有队列
3. 清空所有缓存
```

**调用者**: `ManagerProvider.Init()`

---

### Update()

**签名**:
```csharp
public void Update()
```

**职责**: 每帧更新协程锁状态

**核心逻辑**:
```
1. 检查超时的协程锁 TimeoutCheck()
2. 处理下一帧执行队列：
   - 遍历 nextFrameRun
   - 调用 Notify 执行下一个协程
```

**调用者**: `ManagerProvider.Update()`

---

### Wait(int coroutineLockType, long key, int time)

**签名**:
```csharp
public async ETTask<CoroutineLock> Wait(int coroutineLockType, long key, int time = 60000)
```

**职责**: 等待获取协程锁

**参数**:
- `coroutineLockType`: 协程锁类型
- `key`: 锁键值
- `time`: 超时时间（毫秒，默认 60000）

**返回**: CoroutineLock 实例

**核心逻辑**:
```
1. 获取对应类型的队列
2. 如果 key 不存在：
   - 创建新队列
   - 创建协程锁（level=1）
   - 如果 time>0，添加超时定时器
   - 返回协程锁
3. 如果 key 存在：
   - 创建 ETTask 等待
   - 添加到队列
   - 等待 task 完成
   - 返回协程锁
```

**调用者**: 需要并发控制的代码

**使用示例**:
```csharp
// 等待获取锁
using (await CoroutineLockManager.Instance.Wait(CoroutineLockType.Player, playerId))
{
    // 临界区代码（同一 playerId 的协程会排队）
    await UpdatePlayerData();
}
// 释放锁（自动执行下一个）
```

---

### Notify(int coroutineLockType, long key, int level)

**签名**:
```csharp
public void Notify(int coroutineLockType, long key, int level)
```

**职责**: 通知队列执行下一个协程

**核心逻辑**:
```
1. 获取对应类型的队列
2. 如果 key 不存在，返回
3. 如果队列为空：
   - 移除 key
   - 返回
4. 从队列取出下一个
5. 创建新的协程锁
6. 设置 task 结果
```

**调用者**: `Update()`, `RunNextCoroutine()`

---

### RunNextCoroutine(int coroutineLockType, long key, int level)

**签名**:
```csharp
public void RunNextCoroutine(int coroutineLockType, long key, int level)
```

**职责**: 执行下一个协程

**参数**:
- `coroutineLockType`: 协程锁类型
- `key`: 锁键值
- `level`: 递归层级

**核心逻辑**:
```
1. 如果 level == 100，记录警告（防止死循环）
2. 添加到 nextFrameRun 队列（下一帧执行）
```

**调用者**: `CoroutineLock.Dispose()`, `TimeoutCheck()`

---

### TimeoutCheck()

**签名**:
```csharp
private void TimeoutCheck()
```

**职责**: 检查超时的协程锁

**核心逻辑**:
```
1. 如果 timers 为空，返回
2. 获取当前时间
3. 如果当前时间 < minTime，返回
4. 遍历 timers，找出超时的：
   - 添加到 timeOutIds
5. 处理超时的：
   - 从 timers 移除
   - 添加到 timerOutTimer
6. 遍历 timerOutTimer：
   - 检查 InstanceId 是否匹配
   - 调用 RunNextCoroutine 执行下一个
```

**调用者**: `Update()`

---

## 阅读指引

### 建议的阅读顺序

1. **理解管理器作用** - 协程锁解决什么问题
2. **看 Wait** - 理解如何获取锁
3. **看 Notify/RunNextCoroutine** - 理解队列执行
4. **看 TimeoutCheck** - 理解超时机制

### 最值得学习的技术点

1. **并发控制**: 同 Key 协程排队执行
2. **超时机制**: 防止死锁
3. **对象池**: CoroutineLock 使用对象池
4. **多队列**: 按类型和 Key 分组管理
5. **异步等待**: ETTask 等待锁释放

---

## 使用场景

### 场景 1: 玩家数据更新

```csharp
// 多个协程可能同时修改玩家数据
async ETTask AddItem(int itemId, int count)
{
    using (await CoroutineLockManager.Instance.Wait(CoroutineLockType.Player, playerId))
    {
        // 临界区：确保同一玩家的操作顺序执行
        var player = GetPlayer(playerId);
        player.AddItem(itemId, count);
        await SavePlayerData();
    }
}
```

### 场景 2: 拍卖系统叫价

```csharp
async ETTask UserAuction(AITactic tactic)
{
    using (await CoroutineLockManager.Instance.Wait(CoroutineLockType.Auction, auctionId))
    {
        // 确保叫价顺序执行
        if (CanAuction())
        {
            await ProcessAuction(tactic);
        }
    }
}
```

### 场景 3: 资源加载

```csharp
async ETTask<T> LoadResource<T>(string path)
{
    using (await CoroutineLockManager.Instance.Wait(CoroutineLockType.Resource, path.GetHashCode()))
    {
        // 确保同一资源只加载一次
        if (!IsLoaded(path))
        {
            return await ResourcesManager.Instance.LoadAsync<T>(path);
        }
        return GetLoaded<T>(path);
    }
}
```

---

## 协程锁类型（CoroutineLockType）

| 类型 | 用途 |
|------|------|
| `None` | 无（超时） |
| `Player` | 玩家数据 |
| `Auction` | 拍卖系统 |
| `Resource` | 资源加载 |
| `UI` | UI 操作 |
| `Max` | 最大数量 |

---

## 相关文档

- [CoroutineLock.cs.md](./CoroutineLock.cs.md) - 协程锁
- [CoroutineLockQueue.cs.md](./CoroutineLockQueue.cs.md) - 协程锁队列
- [CoroutineLockTimer.cs.md](./CoroutineLockTimer.cs.md) - 协程锁定时器

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
