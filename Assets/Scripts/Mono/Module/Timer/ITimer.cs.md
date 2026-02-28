# ITimer.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|------|
| **文件名** | ITimer.cs |
| **路径** | Assets/Scripts/Mono/Module/Timer/ITimer.cs |
| **所属模块** | 框架层 → Mono/Module/Timer |
| **文件职责** | 定义定时器接口和抽象基类，规范定时器回调行为 |

---

## 类/结构体说明

### ITimer 接口

| 属性 | 说明 |
|------|------|
| **职责** | 定义定时器的标准接口，规范 Handle 方法 |
| **泛型参数** | 无 |
| **继承关系** | 无 |
| **实现的接口** | 无 |

**设计模式**: 接口模式

```csharp
// 实现接口
public class MyTimer : ITimer
{
    public void Handle(object args)
    {
        // 定时器回调逻辑
    }
}
```

---

### ATimer<T> 抽象类

| 属性 | 说明 |
|------|------|
| **职责** | 提供泛型定时器基类，简化 ITimer 实现 |
| **泛型参数** | `T` - 定时器参数类型 |
| **继承关系** | 实现 `ITimer` |
| **实现的接口** | `ITimer` |

**设计模式**: 模板方法模式 + 泛型模式

```csharp
// 继承抽象类
public class ResetTimeScaleTimer : ATimer<GameTimerManager>
{
    public override void Run(GameTimerManager t)
    {
        t.SetTimeScale(1);
    }
}
```

---

## 方法说明

### ITimer.Handle

**签名**:
```csharp
void Handle(object args)
```

**职责**: 定时器触发时的回调方法

**参数**:
| 参数名 | 类型 | 说明 |
|--------|------|------|
| `args` | `object` | 回调参数（类型由具体实现决定） |

**调用者**: TimerManager 定时器系统

---

### ATimer<T>.Handle

**签名**:
```csharp
public void Handle(object args)
```

**职责**: 实现 ITimer 接口，调用泛型 Run 方法

**核心逻辑**:
```
1. 将 object args 转换为 T 类型
2. 调用 Run(T t)
```

**调用者**: TimerManager 定时器系统

**被调用者**: `Run(T t)`（子类实现）

---

### ATimer<T>.Run

**签名**:
```csharp
public abstract void Run(T t)
```

**职责**: 定时器回调逻辑（由子类实现）

**参数**:
| 参数名 | 类型 | 说明 |
|--------|------|------|
| `t` | `T` | 泛型参数（具体类型由子类指定） |

**调用者**: `Handle(object args)`

**被调用者**: 子类实现

---

## 继承关系

```
ITimer (接口)
    ↑
ATimer<T> (抽象类)
    ↑
具体定时器类（如 ResetTimeScale）
```

---

## 使用示例

### 示例 1: 直接实现接口

```csharp
public class SimpleTimer : ITimer
{
    public void Handle(object args)
    {
        Debug.Log("定时器触发！");
        
        if (args is string message)
        {
            Debug.Log($"参数：{message}");
        }
    }
}

// 使用
ITimer timer = new SimpleTimer();
timer.Handle("Hello Timer");
```

### 示例 2: 继承抽象类（推荐）

```csharp
[Timer(TimerType.ResetTimeScale)]
public class ResetTimeScale : ATimer<GameTimerManager>
{
    public override void Run(GameTimerManager t)
    {
        try
        {
            t.SetTimeScale(1);  // 恢复正常速度
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
    }
}
```

**优势**:
- 类型安全（无需手动转换）
- 代码简洁
- 泛型支持

### 示例 3: 带参数的定时器

```csharp
public class DamageOverTimeTimer : ATimer<Entity>
{
    public override void Run(Entity entity)
    {
        // 对实体造成持续伤害
        entity.TakeDamage(10);
        
        Debug.Log($"{entity.Name} 受到 10 点伤害");
    }
}

// 使用
ATimer<Entity> timer = new DamageOverTimeTimer();
timer.Handle(targetEntity);  // 自动转换为 Entity 类型
```

### 示例 4: 多个定时器

```csharp
// 刷新定时器
[Timer(TimerType.RefreshData)]
public class RefreshDataTimer : ATimer<UIManager>
{
    public override void Run(UIManager ui)
    {
        ui.RefreshAllWindows();
    }
}

// 心跳定时器
[Timer(TimerType.Heartbeat)]
public class HeartbeatTimer : ATimer<NetworkManager>
{
    public override void Run(NetworkManager network)
    {
        network.SendHeartbeat();
    }
}
```

---

## 设计要点

### 为什么需要 ITimer 接口？

1. **统一规范**: 所有定时器都实现相同的接口
2. **多态支持**: 可以用 `ITimer` 类型存储任何定时器
3. **解耦**: TimerManager 不依赖具体实现

### 为什么需要 ATimer<T> 抽象类？

```csharp
// ❌ 直接实现接口（需要手动转换）
public class MyTimer : ITimer
{
    public void Handle(object args)
    {
        GameTimerManager t = args as GameTimerManager;
        if (t == null) return;
        // ...
    }
}

// ✅ 继承抽象类（类型安全）
public class MyTimer : ATimer<GameTimerManager>
{
    public override void Run(GameTimerManager t)
    {
        // 直接使用，无需转换
    }
}
```

**优势**:
- 类型安全（编译时检查）
- 无需手动转换
- 代码更简洁
- IDE 智能提示

### 泛型的意义

```csharp
ATimer<GameTimerManager>  // T = GameTimerManager
ATimer<Entity>            // T = Entity
ATimer<UIManager>         // T = UIManager
```

**好处**:
- 每个定时器可以有不同类型的参数
- 避免 object 转换的性能开销
- 编译时类型检查

---

## 与 TimerAttribute 配合

```csharp
[Timer(TimerType.SomeType)]  // 标记定时器类型
public class SomeTimer : ATimer<GameTimerManager>  // 实现定时器
{
    public override void Run(GameTimerManager t)
    {
        // 定时器逻辑
    }
}
```

**关系**:
- `[Timer]`: 标记类为定时器，指定触发类型
- `ATimer<T>`: 提供泛型回调接口
- `TimerManager`: 扫描并触发定时器

---

## 相关文档

- [TimerAttribute.cs.md](./TimerAttribute.cs.md) - 定时器特性
- [TimerManager.cs.md](./TimerManager.cs.md) - 定时器管理器
- [GameTimerManager.cs.md](./GameTimerManager.cs.md) - 游戏时间管理器

---

*文档生成时间：2026-02-28 | OpenClaw AI 助手*
