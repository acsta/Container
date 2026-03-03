# 框架三件套理解指南 - Core + Timer + Messager

> **文档类型**: 系统理解指南  
> **适用范围**: 框架层核心模块  
> **生成时间**: 2026-03-03  
> **前置知识**: C# 基础、Unity 生命周期、async/await

---

## 📑 概述

Container 框架的**核心三件套**是整个项目的基石，所有其他系统都建立在这三个模块之上：

| 系统 | 职责 | 类比 |
|------|------|------|
| **ManagerProvider** | 依赖注入容器、管理器生命周期 | 公司的"人力资源部" |
| **TimerManager** | 定时器、延时任务、协程等待 | 公司的"日程管理员" |
| **Messager** | 事件总线、松耦合通信 | 公司的"内部邮件系统" |

---

## 🎯 一、ManagerProvider - 依赖注入容器

### 1.1 系统职责

**核心问题**: 如何统一管理所有 Manager 的创建、获取和销毁？

**解决方案**:
- 全局单例，所有 Manager 通过它注册和获取
- 自动管理 `Update`/`LateUpdate`/`FixedUpdate` 生命周期
- 支持类型 + 名称双重索引（同一类型的多个实例）

**如果没有它**:
- 每个 Manager 需要手动管理单例
- Update 调用分散在各处，难以统一调度
- 模块间耦合严重，难以测试

---

### 1.2 核心架构

```
┌─────────────────────────────────────────────────────────┐
│                   ManagerProvider                        │
│  ┌─────────────────────────────────────────────────┐   │
│  │            managersDictionary (核心)             │   │
│  │  Type + string → object  双重索引               │   │
│  └─────────────────────────────────────────────────┘   │
│                                                          │
│  ┌──────────────┐ ┌──────────────┐ ┌──────────────┐   │
│  │updateManagers│ │lateManagers  │ │fixedManagers │   │
│  │ LinkedList   │ │ LinkedList   │ │ LinkedList   │   │
│  └──────────────┘ └──────────────┘ └──────────────┘   │
└─────────────────────────────────────────────────────────┘
                            │
        ┌───────────────────┼───────────────────┐
        ▼                   ▼                   ▼
   ┌─────────┐        ┌─────────┐        ┌─────────┐
   │UIManager│        │TimerMgr │        │ConfigMgr│
   │IUpdate  │        │IUpdate  │        │         │
   └─────────┘        └─────────┘        └─────────┘
```

---

### 1.3 核心流程

#### 注册 Manager 流程

```csharp
// 1. 游戏启动时 (Entry.StartAsync)
ManagerProvider.RegisterManager<UIManager>();
ManagerProvider.RegisterManager<TimerManager>();
ManagerProvider.RegisterManager<ConfigManager>();

// 2. RegisterManager 内部逻辑
public static T RegisterManager<T>(string name = "") where T :class,IManager
{
    var type = TypeInfo<T>.Type;
    
    // 检查是否已注册
    if (!Instance.managersDictionary.TryGetValue(type, name, out var res))
    {
        // 创建实例
        res = Activator.CreateInstance(type) as T;
        
        // 根据接口自动加入 Update 队列
        if (res is IUpdate u) Instance.updateManagers.AddLast(u);
        if (res is ILateUpdate lu) Instance.lateUpdateManagers.AddLast(lu);
        if (res is IFixedUpdate fu) Instance.fixedUpdateManagers.AddLast(fu);
        
        // 调用 Init 初始化
        (res as T).Init();
        
        // 加入字典和列表
        Instance.managersDictionary.Add(type, name, res);
        Instance.allManagers.AddLast(res);
    }
    return res as T;
}
```

#### Update 调用流程

```csharp
// Unity MonoBehavior.Update → ManagerProvider.Update
public static void Update()
{
    // 遍历所有实现 IUpdate 的 Manager
    for (var node = Instance.updateManagers.First; node != null; node = node.Next)
    {
        node.Value.Update();  // UIManager.Update → TimerManager.Update → ...
    }
    
    // 处理完成的任务
    while (count-- > 0)
    {
        ETTask task = UnityLifeTimeHelper.UpdateFinishTask.Dequeue();
        task.SetResult();
    }
}
```

---

### 1.4 使用示例

#### 自定义 Manager

```csharp
public class MyManager : IManager, IUpdate
{
    public static MyManager Instance { get; private set; }
    
    public void Init()
    {
        Instance = this;
        Log.Info("MyManager 初始化完成");
    }
    
    public void Destroy()
    {
        Instance = null;
        Log.Info("MyManager 已销毁");
    }
    
    public void Update()
    {
        // 每帧执行的逻辑
        CheckSomething();
    }
}

// 注册
void Start()
{
    ManagerProvider.RegisterManager<MyManager>();
}

// 获取
var myMgr = ManagerProvider.GetManager<MyManager>();
// 或
myMgr = MyManager.Instance;
```

#### 带参数的 Manager

```csharp
public class PoolManager : IManager<string>
{
    public void Init(string poolName)
    {
        Log.Info($"初始化对象池：{poolName}");
    }
    
    public void Destroy() { }
}

// 注册带参数的 Manager
ManagerProvider.RegisterManager<PoolManager, string>("DefaultPool", "myPool");
```

---

### 1.5 依赖关系

```
被依赖：所有 Manager 都依赖 ManagerProvider
依赖：不依赖具体 Manager，只依赖接口 (IManager, IUpdate 等)
```

---

### 1.6 注意事项

| 问题 | 说明 | 解决方案 |
|------|------|----------|
| 重复注册 | 同一类型多次注册返回同一实例 | 使用 `GetManager<T>()` 先检查 |
| 循环依赖 | A Manager 依赖 B，B 又依赖 A | 调整注册顺序，或延迟获取 |
| 忘记销毁 | Manager 持有资源未释放 | 实现 `Destroy()` 清理资源 |
| 线程安全 | Update 中修改 Manager 列表 | 使用链表遍历，避免并发修改 |

---

## 🎯 二、TimerManager - 定时器系统

### 2.1 系统职责

**核心问题**: 如何高效管理延时任务和定时任务？

**解决方案**:
- 时间轮算法 + 最小堆优化
- 支持一次性、重复、每帧定时器
- 异步等待 (`WaitAsync`) + 回调式 (`NewOnceTimer`)

**如果没有它**:
- 使用 Unity `Invoke` 无法热更新
- 协程性能开销大
- 定时器管理分散，难以维护

---

### 2.2 核心架构

```
┌─────────────────────────────────────────────────────────┐
│                   TimerManager                           │
│  ┌─────────────────────────────────────────────────┐   │
│  │         TimeId: MultiMap<time, timerId>         │   │
│  │  按时间排序的定时器 ID                           │   │
│  └─────────────────────────────────────────────────┘   │
│                                                          │
│  ┌──────────────┐ ┌──────────────┐ ┌──────────────┐   │
│  │  childs      │ │everyFrameTimer│ │ timeOutQueue │   │
│  │  TimerAction │ │  每帧定时器   │ │  超时队列    │   │
│  └──────────────┘ └──────────────┘ └──────────────┘   │
└─────────────────────────────────────────────────────────┘
```

---

### 2.3 定时器类型

| 类型 | TimerClass | 说明 | 使用场景 |
|------|-----------|------|---------|
| `OnceTimer` | 一次性定时器 | 指定时间执行一次 | 延时任务 |
| `OnceWaitTimer` | 一次性等待定时器 | 配合 `WaitAsync` | 异步等待 |
| `RepeatedTimer` | 重复定时器 | 每隔时间执行 | 心跳、轮询 |
| `FrameTimer` | 每帧定时器 | 每帧执行 | 持续检测 |

---

### 2.4 核心流程

#### WaitAsync 流程

```csharp
// 1. 调用 WaitAsync
await TimerManager.Instance.WaitAsync(1000);  // 等待 1 秒

// 2. 内部实现
public async ETTask<bool> WaitAsync(long time, ETCancellationToken cancellationToken = null)
{
    long tillTime = GetTimeNow() + time;
    
    // 创建任务完成源
    ETTask<bool> tcs = ETTask<bool>.Create(true);
    
    // 创建定时器
    TimerAction timer = AddChild(TimerClass.OnceWaitTimer, time, 0, tcs);
    AddTimer(tillTime, timer);
    
    // 等待完成
    bool ret = await tcs;
    return ret;
}

// 3. Update 中检查超时
public void Update()
{
    var timeNow = GetTimeNow();
    
    // 检查是否有超时的定时器
    foreach (var item in TimeId)
    {
        if (item.Key > timeNow) break;  // 还没到时间
        
        // 执行超时的定时器
        timeOutTimerIds.Enqueue(timerId);
    }
    
    // 执行回调
    while (timeOutTimerIds.Count > 0)
    {
        long timerId = timeOutTimerIds.Dequeue();
        TimerAction timerAction = GetChild(timerId);
        Run(timerAction);  // tcs.SetResult(true)
    }
}
```

#### 重复定时器流程

```csharp
// 创建重复定时器
long timerId = TimerManager.Instance.NewRepeatedTimer(5000, timerType, args);

// Update 中执行
case TimerClass.RepeatedTimer:
    // 1. 计算下次执行时间
    long tillTime = GetTimeNow() + timerAction.Time;
    AddTimer(tillTime, timerAction);  // 重新加入时间轮
    
    // 2. 执行回调
    ITimer timer = timerActions[type];
    timer.Handle(timerAction.Object);
    break;
```

---

### 2.5 使用示例

#### 异步等待

```csharp
// 等待 1 秒
await TimerManager.Instance.WaitAsync(1000);
Log.Info("1 秒后执行");

// 带取消令牌
var cts = new ETCancellationToken();
try
{
    await TimerManager.Instance.WaitAsync(5000, cts);
    // 逻辑...
}
finally
{
    cts.Dispose();
}
```

#### 重复定时器

```csharp
// 方式 1: 使用 ITimer (支持热更新)
[Timer(Type = 1001)]
public class HeartbeatTimer : ITimer
{
    public void Handle(object obj)
    {
        SendHeartbeat();
    }
}

long timerId = TimerManager.Instance.NewRepeatedTimer(5000, 1001, null);

// 方式 2: 使用 async 循环
public async ETVoid HeartbeatLoop()
{
    while (true)
    {
        await TimerManager.Instance.WaitAsync(5000);
        SendHeartbeat();
    }
}
```

#### 每帧定时器

```csharp
[Timer(Type = 1002)]
public class FrameTimer : ITimer
{
    public void Handle(object obj)
    {
        // 每帧执行的逻辑
        CheckPlayerInput();
    }
}

long timerId = TimerManager.Instance.NewFrameTimer(1002, null);
```

---

### 2.6 依赖关系

```
依赖：TimeInfo (时间获取)
被依赖：Messager (BroadcastNextFrame)、所有需要延时的模块
```

---

### 2.7 注意事项

| 问题 | 说明 | 解决方案 |
|------|------|----------|
| 定时器泄漏 | 创建后未移除 | 使用 `Remove(ref id)` 或 `ETCancellationToken` |
| 时间精度 | 不是高精度定时器 | 不适用于毫秒级精度场景 |
| 每帧定时器性能 | 性能开销大 | 仅在必要时使用 |
| 热更新 | `WaitAsync` 不能热更 | 需要热更时使用 `NewOnceTimer` + `ITimer` |

---

## 🎯 三、Messager - 消息事件系统

### 3.1 系统职责

**核心问题**: 如何实现模块间的松耦合通信？

**解决方案**:
- 观察者模式实现
- 双键索引 (id + name) 定位事件
- 支持 0-5 个参数的泛型委托

**如果没有它**:
- 模块间直接调用导致高耦合
- 跨系统通信复杂
- 难以扩展和维护

---

### 3.2 核心架构

```
┌─────────────────────────────────────────────────────────┐
│                      Messager                            │
│  ┌─────────────────────────────────────────────────┐   │
│  │    evtGroup: Dictionary<id, MultiMapSet>        │   │
│  │                                                  │   │
│  │  id=0 (UI 模块)                                  │   │
│  │  ├─ name=1001 → [callback1, callback2, ...]     │   │
│  │  ├─ name=1002 → [callback3, callback4, ...]     │   │
│  │                                                  │   │
│  │  id=1 (网络模块)                                 │   │
│  │  ├─ name=2001 → [callback5, ...]                │   │
│  └─────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────┘
```

---

### 3.3 核心流程

#### 订阅与发布流程

```csharp
// 1. 订阅事件
Messager.Instance.AddListener(0, MessageId.OnKeyInput, OnKeyInput);

void OnKeyInput(int key, int state)
{
    Log.Info($"按键：{key}, 状态：{state}");
}

// 2. 发布事件
Messager.Instance.Broadcast(0, MessageId.OnKeyInput, KeyCode.A, 1);

// 3. 内部实现
public void Broadcast<P1, P2>(long id, int name, P1 p1, P2 p2)
{
    if (evtGroup.TryGetValue(id, out var evts))
    {
        if (evts.TryGetValue(name, out var evt))
        {
            using var list = ToList(evt);  // 复制到临时列表，避免遍历中修改
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                if (item is Action<P1, P2> action)
                {
                    action.Invoke(p1, p2);
                }
                else // 多态支持
                {
                    // 反射调用，支持子类参数
                    item.DynamicInvoke(p1, p2);
                }
            }
        }
    }
}
```

#### 延迟广播流程

```csharp
// 下一帧广播
await Messager.Instance.BroadcastNextFrame(0, MessageId.OnGameStart);

// 内部实现
public async ETTask BroadcastNextFrame<P1, P2>(long id, int name, P1 p1, P2 p2)
{
    if (evtGroup.TryGetValue(id, out var evts))
    {
        if (evts.TryGetValue(name, out var evt))
        {
            // 等待一帧
            await TimerManager.Instance.WaitAsync(1);
            
            // 执行回调
            using var list = ToList(evt);
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                if (item is Action<P1, P2> action)
                {
                    action.Invoke(p1, p2);
                }
            }
        }
    }
}
```

---

### 3.4 使用示例

#### 基础事件

```csharp
// 定义事件 ID
public enum MessageId
{
    OnGameStart = 1001,
    OnKeyInput = 1002,
    OnLoginSuccess = 1003,
}

// 订阅
void Start()
{
    Messager.Instance.AddListener(0, MessageId.OnGameStart, OnGameStart);
}

void OnGameStart()
{
    Log.Info("游戏开始");
}

// 发布
Messager.Instance.Broadcast(0, MessageId.OnGameStart);
```

#### 带参数的事件

```csharp
// 订阅
Messager.Instance.AddListener(0, MessageId.OnKeyInput, OnKeyInput);

void OnKeyInput(int key, int state)
{
    if (key == KeyCode.A && state == 1)
    {
        Player.MoveLeft();
    }
}

// 发布
Messager.Instance.Broadcast(0, MessageId.OnKeyInput, KeyCode.A, 1);
```

#### 取消订阅

```csharp
// 重要：在 Destroy 中取消订阅，避免内存泄漏
public void Destroy()
{
    Messager.Instance.RemoveListener(0, MessageId.OnKeyInput, OnKeyInput);
}
```

---

### 3.5 依赖关系

```
依赖：TimerManager (BroadcastNextFrame)
被依赖：所有需要跨模块通信的系统
```

---

### 3.6 注意事项

| 问题 | 说明 | 解决方案 |
|------|------|----------|
| 内存泄漏 | 订阅后未取消 | 在 `Destroy()` 中移除监听 |
| 类型不匹配 | 参数类型不一致 | 使用泛型版本，编译器检查 |
| 执行顺序 | 多个订阅者执行顺序不确定 | 不要依赖执行顺序 |
| 异常传播 | 某个订阅者异常影响其他 | Messager 内部已做异常隔离 |

---

## 🔗 三件套协作关系

```
┌──────────────────────────────────────────────────────────┐
│                     游戏启动流程                          │
└──────────────────────────────────────────────────────────┘

1. Entry.StartAsync()
   ↓
2. ManagerProvider.RegisterManager<TimerManager>()
   ↓
3. ManagerProvider.RegisterManager<Messager>()
   ↓
4. ManagerProvider.RegisterManager<UIManager>()
   ↓
5. ManagerProvider.RegisterManager<ConfigManager>()
   ↓
6. 游戏主循环 (Unity Update)
   ↓
   ├─→ ManagerProvider.Update()
   │   ├─→ TimerManager.Update()  检查定时器
   │   ├─→ UIManager.Update()     UI 逻辑
   │   └─→ ...其他 Manager
   │
   ├─→ ManagerProvider.LateUpdate()
   │   └─→ ...延迟更新
   │
   └─→ ManagerProvider.FixedUpdate()
       └─→ ...物理更新
```

---

## 💡 最佳实践

### 1. Manager 注册时机

```csharp
// ✅ 推荐：在 Entry.StartAsync 统一注册
public static async Task StartAsync()
{
    ManagerProvider.RegisterManager<TimerManager>();
    ManagerProvider.RegisterManager<Messager>();
    ManagerProvider.RegisterManager<UIManager>();
    ManagerProvider.RegisterManager<ConfigManager>();
    
    // 等待配置加载完成
    await ConfigManager.Instance.LoadAsync();
}
```

### 2. 定时器使用

```csharp
// ✅ 推荐：优先使用 WaitAsync（逻辑连贯）
public async ETTask DoSomethingAsync()
{
    await TimerManager.Instance.WaitAsync(1000);
    Log.Info("1 秒后执行");
}

// ✅ 热更新场景：使用 ITimer
[Timer(Type = 1001)]
public class MyTimer : ITimer
{
    public void Handle(object obj)
    {
        // 热更新逻辑
    }
}
```

### 3. 事件订阅

```csharp
// ✅ 推荐：在 Init 订阅，在 Destroy 取消
public class UIManager : IManager
{
    public void Init()
    {
        Messager.Instance.AddListener(0, MessageId.OnKeyInput, OnKeyInput);
    }
    
    public void Destroy()
    {
        Messager.Instance.RemoveListener(0, MessageId.OnKeyInput, OnKeyInput);
    }
}
```

---

## 📚 相关文档

| 文档 | 说明 |
|------|------|
| [ManagerProvider.cs.md](../Mono/Core/Manager/ManagerProvider.cs.md) | ManagerProvider 详细文档 |
| [TimerManager.cs.md](../Mono/Module/Timer/TimerManager.cs.md) | TimerManager 详细文档 |
| [Messager.cs.md](../Mono/Module/Messager/Messager.cs.md) | Messager 详细文档 |
| [ETTask 总览](../ThirdParty/ETTask/README.md) | 异步任务系统 |

---

*文档由 OpenClaw AI 助手自动生成 | 框架层核心系统理解指南*
