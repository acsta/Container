# TimerManager.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | TimerManager.cs |
| **路径** | Assets/Scripts/Mono/Module/Timer/TimerManager.cs |
| **所属模块** | 框架层 → Mono/Module/Timer |
| **文件职责** | 定时器管理器，提供一次性定时器、重复定时器、等待异步等功能 |

---

## 类/结构体说明

### TimerManager

| 属性 | 说明 |
|------|------|
| **职责** | 管理所有定时器，支持 OnceTimer、RepeatedTimer、WaitTimer |
| **泛型参数** | 无 |
| **继承关系** | 无继承 |
| **实现的接口** | `IManager`, `IUpdate` |

**设计模式**: 单例模式 + 时间轮算法

```csharp
// 单例实现
public static TimerManager Instance { get; private set; }

// 通过 ManagerProvider 注册
ManagerProvider.RegisterManager<TimerManager>();
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `Instance` | `TimerManager` | `public static` | 单例实例，全局访问点 |
| `childs` | `Dictionary<long, TimerAction>` | `protected` | 所有定时器实例 {timerId: TimerAction} |
| `TimeId` | `MultiMap<long, long>` | `protected` | 时间 → 定时器 ID 列表（时间轮） |
| `minTime` | `long` | `protected` | 最小到期时间（优化，避免每次遍历） |
| `timerActions` | `ITimer[]` | `protected` | 定时器动作数组（通过反射加载） |
| `everyFrameTimer` | `Queue<long>` | `protected` | 每帧执行的定时器队列 |

---

## TimerClass 枚举

```csharp
public enum TimerClass : byte
{
    None,           // 无
    OnceTimer,      // 一次性定时器
    OnceWaitTimer,  // 一次性等待定时器（返回 Task）
    RepeatedTimer,  // 重复定时器
}
```

---

## 方法说明（按重要程度排序）

### Update()

**签名**:
```csharp
public virtual void Update()
```

**职责**: 每帧更新，检查并执行到期的定时器

**核心逻辑**:
```
1. 处理每帧定时器（everyFrameTimer）
   - 出队并执行
   - 重新入队（如果是重复定时器）
2. 检查 TimeId 中是否有到期定时器
3. 将到期定时器加入 timeOutTimerIds 队列
4. 执行所有到期定时器
```

**调用者**: ManagerProvider.Update()（每帧）

**被调用者**: `Run()`, `GetTimeNow()`

---

### WaitAsync(long time, ETCancellationToken cancellationToken)

**签名**:
```csharp
public async ETTask<bool> WaitAsync(long time, ETCancellationToken cancellationToken = null)
```

**职责**: 异步等待指定时间

**参数**:
- `time`: 等待时间（毫秒）
- `cancellationToken`: 取消令牌（可选）

**返回**: `true` = 等待完成，`false` = 被取消

**核心逻辑**:
```
1. 计算到期时间 tillTime = GetTimeNow() + time
2. 创建 ETTask<bool> tcs
3. 创建 OnceWaitTimer 定时器
4. 注册取消回调
5. await tcs
6. 返回结果
```

**调用者**: 需要异步等待的代码

**使用示例**:
```csharp
// 等待 1 秒
await TimerManager.Instance.WaitAsync(1000);

// 等待 500ms，支持取消
var cts = new ETCancellationToken();
await TimerManager.Instance.WaitAsync(500, cts);

// 取消等待
cts.Cancel();
```

---

### WaitTillAsync(long tillTime, ETCancellationToken cancellationToken)

**签名**:
```csharp
public async ETTask<bool> WaitTillAsync(long tillTime, ETCancellationToken cancellationToken = null)
```

**职责**: 异步等待到指定时间点

**参数**:
- `tillTime`: 绝对时间戳
- `cancellationToken`: 取消令牌

**与 WaitAsync 的区别**:
- `WaitAsync`: 相对时间（等待多久）
- `WaitTillAsync`: 绝对时间（等到某个时间点）

---

### NewOnceTimer(long tillTime, int type, object args)

**签名**:
```csharp
public long NewOnceTimer(long tillTime, int type, object args)
```

**职责**: 创建一次性定时器（回调式）

**参数**:
- `tillTime`: 到期时间戳
- `type`: 定时器类型（对应 TimerAttribute）
- `args`: 回调参数

**返回**: 定时器 ID（可用于取消）

**核心逻辑**:
```
1. 创建 TimerAction（OnceTimer）
2. 添加到 TimeId 时间轮
3. 返回定时器 ID
```

**调用者**: 需要定时回调的代码

**使用示例**:
```csharp
// 1 秒后执行
long timerId = TimerManager.Instance.NewOnceTimer(
    TimerManager.Instance.GetTimeNow() + 1000,
    TimerType.MyTimer,
    null
);

// 取消定时器
TimerManager.Instance.Remove(ref timerId);
```

---

### NewFrameTimer(int type, object args)

**签名**:
```csharp
public long NewFrameTimer(int type, object args)
```

**职责**: 创建每帧执行的定时器

**参数**:
- `type`: 定时器类型
- `args`: 回调参数

**返回**: 定时器 ID

**使用示例**:
```csharp
// 每帧执行
long timerId = TimerManager.Instance.NewFrameTimer(
    TimerType.Update,
    this
);

// 取消
TimerManager.Instance.Remove(ref timerId);
```

---

### NewRepeatedTimer(long time, int type, object args)

**签名**:
```csharp
public long NewRepeatedTimer(long time, int type, object args)
```

**职责**: 创建重复定时器

**参数**:
- `time`: 间隔时间（毫秒）
- `type`: 定时器类型
- `args`: 回调参数

**返回**: 定时器 ID

**使用示例**:
```csharp
// 每 2 秒执行一次
long timerId = TimerManager.Instance.NewRepeatedTimer(
    2000,
    TimerType.CheckStatus,
    this
);

// 取消
TimerManager.Instance.Remove(ref timerId);
```

---

### Remove(ref long id)

**签名**:
```csharp
public bool Remove(ref long id)
```

**职责**: 移除/取消定时器

**参数**:
- `id`: 定时器 ID（引用传递，成功后置为 0）

**返回**: `true` = 移除成功，`false` = 定时器不存在

**使用示例**:
```csharp
long timerId = TimerManager.Instance.NewOnceTimer(...);

// 取消定时器
if (TimerManager.Instance.Remove(ref timerId))
{
    Log.Info("定时器已取消");
}
// timerId 现在为 0
```

---

## 定时器类型系统

### TimerAttribute

通过反射加载定时器处理器：

```csharp
[Timer(TimerType.CheckStatus)]
public class CheckStatusTimer : ITimer
{
    public void Handle(object obj)
    {
        // 定时器逻辑
    }
}
```

### 注册流程

```csharp
protected void InitAction()
{
    // 获取所有带 [Timer] 特性的类
    List<Type> types = AttributeManager.Instance.GetTypes(
        TypeInfo<TimerAttribute>.Type
    );
    
    foreach (Type type in types)
    {
        ITimer timer = Activator.CreateInstance(type) as ITimer;
        TimerAttribute attr = type.GetCustomAttribute<TimerAttribute>();
        timerActions[attr.Type] = timer;
    }
}
```

---

## 阅读指引

### 建议的阅读顺序

1. **理解定时器作用** - 为什么需要 TimerManager
2. **看 Update 方法** - 理解定时器轮询机制
3. **看 WaitAsync** - 理解异步等待
4. **看 NewOnceTimer** - 理解回调式定时器

### 最值得学习的技术点

1. **时间轮算法**: MultiMap<long, long> 高效管理定时器
2. **minTime 优化**: 避免每次遍历所有定时器
3. **异步等待**: ETTask 实现 awaitable 定时器
4. **反射注册**: 通过 [Timer] 特性自动注册处理器
5. **每帧定时器**: everyFrameTimer 特殊处理

---

## 使用示例

### 示例 1: 异步等待

```csharp
// 等待 1 秒
await TimerManager.Instance.WaitAsync(1000);
Log.Info("1 秒后");

// 等待 500ms 后执行
await TimerManager.Instance.WaitAsync(500);
DoSomething();
```

### 示例 2: 一次性定时器

```csharp
// 定义定时器处理器
[Timer(TimerType.MyTimer)]
public class MyTimer : ITimer
{
    public void Handle(object obj)
    {
        Log.Info("定时器触发");
    }
}

// 创建定时器
long timerId = TimerManager.Instance.NewOnceTimer(
    TimerManager.Instance.GetTimeNow() + 5000,  // 5 秒后
    TimerType.MyTimer,
    null
);
```

### 示例 3: 重复定时器

```csharp
// 每 2 秒检查一次状态
[Timer(TimerType.CheckStatus)]
public class CheckStatusTimer : ITimer
{
    public void Handle(object obj)
    {
        var manager = obj as GameManager;
        manager.CheckStatus();
    }
}

// 启动重复定时器
long timerId = TimerManager.Instance.NewRepeatedTimer(
    2000,  // 2 秒间隔
    TimerType.CheckStatus,
    this
);

// 停止定时器
TimerManager.Instance.Remove(ref timerId);
```

### 示例 4: 带取消的等待

```csharp
var cts = new ETCancellationToken();

try
{
    // 等待 10 秒，支持取消
    await TimerManager.Instance.WaitAsync(10000, cts);
    Log.Info("等待完成");
}
catch (OperationCanceledException)
{
    Log.Info("等待被取消");
}

// 取消等待
cts.Cancel();
```

---

## 相关文档

- [TimerAction.cs.md](./TimerAction.cs.md) - 定时器动作类
- [ITimer.cs.md](./ITimer.cs.md) - 定时器接口
- [ManagerProvider.cs.md](../../Core/Manager/ManagerProvider.cs.md) - Manager 注册

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
