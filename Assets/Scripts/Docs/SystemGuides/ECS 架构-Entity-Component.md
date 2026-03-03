# ECS 架构理解指南 - Entity + Component

> **文档类型**: 系统理解指南  
> **适用范围**: Code/Game/Entity + Code/Game/Component  
> **生成时间**: 2026-03-03  
> **前置知识**: OOP 基础、设计模式、对象池

---

## 📑 概述

Container 使用 **ECS（Entity-Component-System）架构**管理游戏对象。

**核心概念**:
| 概念 | 说明 | 类比 |
|------|------|------|
| **Entity** | 实体，游戏对象容器 | "空盒子" |
| **Component** | 组件，数据和行为 | "盒子里的工具" |
| **System** | 系统，处理逻辑 | "使用工具的工人" |

**关键文件**:
| 文件 | 职责 |
|------|------|
| `Entity.cs` | 实体基类 |
| `Component.cs` | 组件基类 |
| `EntityManager.cs` | 实体管理器 |
| `IEntity.cs` / `IComponent.cs` | 接口定义 |

---

## 🎯 系统职责

### 解决的核心问题

1. **代码复用**: 传统继承导致类爆炸，组件化可以组合功能
2. **灵活扩展**: 动态添加/移除功能，无需修改基类
3. **内存优化**: 组件按需添加，不占用多余内存
4. **逻辑分离**: 数据（Component）与逻辑（System）分离

### 设计思路

```
传统 OOP:
Player : Character : Unit : Entity
NPC    : Character : Unit : Entity
Enemy  : Character : Unit : Entity
→ 继承层次深，难以维护

ECS 架构:
Entity (空对象)
├─→ PlayerComponent (玩家数据)
├─→ CharacterComponent (角色数据)
├─→ AIComponent (AI 逻辑)
└─→ ViewComponent (视图表现)
→ 灵活组合，易于扩展
```

---

## 🏗️ 架构设计

### 核心类图

```
┌─────────────────────────────────────────────────────────┐
│                      Entity                              │
│  ┌─────────────────────────────────────────────────┐   │
│  │  Components: Dictionary<Type, Component>        │   │
│  │  自身组件                                        │   │
│  └─────────────────────────────────────────────────┘   │
│  ┌─────────────────────────────────────────────────┐   │
│  │  OtherComponents: Dictionary<Type, Component>   │   │
│  │  共享其他实体的组件                              │   │
│  └─────────────────────────────────────────────────┘   │
│  ┌─────────────────────────────────────────────────┐   │
│  │  BaseType: Dictionary<Component, Type>          │   │
│  │  基类映射（支持继承）                            │   │
│  └─────────────────────────────────────────────────┘   │
│                                                          │
│  +AddComponent<T>()                                     │
│  +GetComponent<T>()                                     │
│  +RemoveComponent<T>()                                  │
│  +Dispose()                                             │
└─────────────────────────────────────────────────────────┘
                            ▲
                            │ 组合
                            │
┌─────────────────────────────────────────────────────────┐
│                     Component                            │
│─────────────────────────────────────────────────────────│
│ +parent: Entity                                         │
│ +Id: long                                               │
│ +IsDispose: bool                                        │
│─────────────────────────────────────────────────────────│
│ +BeforeInit() / AfterInit()                            │
│ +BeforeDestroy() / AfterDestroy()                      │
│ +Dispose()                                              │
└─────────────────────────────────────────────────────────┘
```

---

## 🔄 核心流程

### Entity 生命周期

```
┌─────────┐     ┌─────────┐     ┌─────────┐     ┌─────────┐
│  创建   │────►│  初始化 │────►│  使用   │────►│  销毁   │
│  Fetch  │     │ BeforeInit│    │  访问   │     │  Dispose │
│  对象池 │     │ AfterInit │    │  组件   │     │  回收到池│
└─────────┘     └─────────┘     └─────────┘     └─────────┘
                     │                                   │
                     ▼                                   ▼
              ┌─────────────┐                   ┌─────────────┐
              │ 添加组件     │                   │ 移除组件     │
              │ 注册 Update  │                   │ 注销 Update  │
              └─────────────┘                   └─────────────┘
```

### 组件生命周期

```csharp
// Component 生命周期方法
public abstract class Component : IDisposable
{
    // 1. 从对象池获取后调用
    public void BeforeInit(Entity entity)
    {
        IsDispose = false;
        parent = entity;
    }
    
    // 2. 添加组件后调用
    public void AfterInit()
    {
        // 注册 Update 循环
        if(this is IUpdate updater)
            timerId = GameTimerManager.Instance.NewFrameTimer(
                TimerType.ComponentUpdate, 
                updater
            );
        
        // 注册全局事件
        if (this is IFixedUpdate fixedUpdate)
            CodeLoader.Instance.FixedUpdate += fixedUpdate.FixedUpdate;
        if (this is ILateUpdate lateUpdate)
            CodeLoader.Instance.LateUpdate += lateUpdate.LateUpdate;
    }
    
    // 3. 销毁前调用
    public void BeforeDestroy()
    {
        IsDispose = true;
        
        // 注销事件
        if (this is IFixedUpdate fixedUpdate)
            CodeLoader.Instance.FixedUpdate -= fixedUpdate.FixedUpdate;
        if (this is ILateUpdate lateUpdate)
            CodeLoader.Instance.LateUpdate -= lateUpdate.LateUpdate;
        
        // 移除定时器
        GameTimerManager.Instance?.Remove(ref timerId);
    }
    
    // 4. 销毁后调用
    public void AfterDestroy()
    {
        parent = null;
        ObjectPool.Instance.Recycle(this);  // 回收到对象池
    }
}
```

---

### 添加组件流程

```csharp
// Entity.AddComponent<T>()
public T AddComponent<T>(Type baseType = null) where T : Component
{
    if (IsDispose) return null;
    
    // 检查是否重复添加
    Type type = TypeInfo<T>.Type;
    if (Components.ContainsKey(type))
    {
        Log.Error($"重复添加{type.Name}");
        return default;
    }

    // 从对象池获取组件
    T data = ObjectPool.Instance.Fetch(type) as T;
    
    // 初始化
    data.BeforeInit(this);
    Components.Add(type, data);
    
    // 支持基类映射
    if (baseType != null)
    {
        Components.Add(baseType, data);
        BaseType.Add(data, baseType);
    }
    
    // 调用组件 Init
    if (data is IComponent comp)
        comp.Init();
    
    // 生命周期回调
    data.AfterInit();
    
    return data;
}
```

---

### 获取组件流程

```csharp
// Entity.GetComponent<T>()
public T GetComponent<T>(bool includeOther = true) where T : Component
{
    if (IsDispose) return null;
    
    Type type = TypeInfo<T>.Type;
    
    // 先查 OtherComponents（共享组件）
    if (includeOther && OtherComponents.TryGetValue(type, out var res))
    {
        if (res.IsDispose)
        {
            OtherComponents.Remove(type);  // 清理已销毁的
        }
        else
        {
            return (T) res;
        }
    }
    
    // 再查 Components（自身组件）
    if (Components.TryGetValue(type, out res))
    {
        return (T) res;
    }

    return default;
}
```

---

### 销毁 Entity 流程

```csharp
// Entity.Dispose()
public void Dispose()
{
    if (IsDispose) return;
    IsDispose = true;
    
    // 移除延时销毁定时器
    if (delayDestroyTimerId != 0) 
        GameTimerManager.Instance.Remove(ref delayDestroyTimerId);
    
    // 销毁所有组件
    foreach (var item in Components)
    { 
        if(item.Key != item.Value.GetType()) continue;
        
        // 组件生命周期回调
        item.Value?.BeforeDestroy();
        (item.Value as IComponentDestroy)?.Destroy();
        item.Value?.AfterDestroy();
    }

    // 清理容器
    Components.Dispose();
    Components = null;
    OtherComponents.Dispose();
    OtherComponents = null;
    BaseType.Dispose();
    BaseType = null;
    
    // 子类清理
    (this as IEntityDestroy)?.Destroy();
    
    // 从父管理器移除
    Parent?.Remove(this);
    Parent = null;
    
    // 回收到对象池
    ObjectPool.Instance.Recycle(this);
}
```

---

## 💡 使用示例

### 创建 Entity

```csharp
// 通过 EntityManager 创建
var player = EntityManager.Instance.Create<Player>();

// 或使用对象池
var entity = ObjectPool.Instance.Fetch<Entity>();
entity.BeforeInit(entityManager);
```

### 添加组件

```csharp
// 创建玩家实体
var player = EntityManager.Instance.Create<Player>();

// 添加组件
var numeric = player.AddComponent<NumericComponent>();
var ai = player.AddComponent<AIComponent>();
var view = player.AddComponent<ViewComponent>();

// 带参数的组件
var state = player.AddComponent<StateComponent, int>(initialState);

// 带基类映射（支持继承查询）
var character = player.AddComponent<CharacterComponent>(
    baseType: typeof(UnitComponent)
);

// 查询时可以用基类
var unit = player.GetComponent<UnitComponent>();  // 能查到
```

### 访问组件

```csharp
// 获取组件
var numeric = player.GetComponent<NumericComponent>();
var ai = player.GetComponent<AIComponent>();

// 获取或添加（不存在则创建）
var view = player.GetOrAddComponent<ViewComponent>();

// 获取父实体
var parent = view.GetParent<Player>();

// 通过组件访问其他组件
var ai = view.GetComponent<AIComponent>();  // 等价于 player.GetComponent<AIComponent>()
```

### 移除组件

```csharp
// 移除特定组件
player.RemoveComponent<AIComponent>();

// 或移除组件实例
var ai = player.GetComponent<AIComponent>();
player.RemoveComponent(ai);

// 组件会自动调用 BeforeDestroy/AfterDestroy
```

### 延时销毁

```csharp
// 5 秒后销毁实体
entity.DelayDispose(5000);

// 取消延时销毁
if (delayDestroyTimerId != 0)
    GameTimerManager.Instance.Remove(ref delayDestroyTimerId);
```

### 共享组件（OtherComponents）

```csharp
// 实体 A 添加组件
var sharedComp = entityA.AddComponent<SharedComponent>();

// 实体 B 共享该组件
entityB.AddOtherComponent(sharedComp);

// 实体 B 可以访问
var comp = entityB.GetComponent<SharedComponent>();

// 移除共享（不影响原组件）
entityB.RemoveOtherComponent(sharedComp);
```

---

### 完整示例：玩家实体

```csharp
// 1. 定义玩家实体
public class Player : Entity
{
    public override EntityType Type => EntityType.Player;
    
    // 快捷访问组件
    public NumericComponent Numeric => GetComponent<NumericComponent>();
    public AIComponent AI => GetComponent<AIComponent>();
    public ViewComponent View => GetComponent<ViewComponent>();
}

// 2. 定义组件
public class NumericComponent : Component, IUpdate
{
    public int HP { get; set; }
    public int Attack { get; set; }
    
    public void Init()
    {
        HP = 100;
        Attack = 10;
    }
    
    public void Update()
    {
        // 每帧更新数值
        CheckHP();
    }
    
    public void Destroy() { }
}

public class AIComponent : Component
{
    public AIState State { get; set; }
    
    public void Init()
    {
        State = AIState.Patrol;
    }
    
    public void UpdateAI()
    {
        // AI 逻辑
        switch (State)
        {
            case AIState.Patrol:
                Patrol();
                break;
            case AIState.Chase:
                ChasePlayer();
                break;
        }
    }
}

// 3. 使用
var player = EntityManager.Instance.Create<Player>();
player.AddComponent<NumericComponent>();
player.AddComponent<AIComponent>();
player.AddComponent<ViewComponent>();

// 访问
player.Numeric.HP -= 10;
player.AI.UpdateAI();
```

---

## 🔗 依赖关系

```
依赖:
├─→ ObjectPool (对象池)
├─→ GameTimerManager (定时器)
├─→ CodeLoader (Update 注册)
└─→ IdGenerater (ID 生成)

被依赖:
├─→ 所有游戏实体（Player/NPC/Enemy 等）
├─→ 所有游戏组件（Numeric/AI/View 等）
└─→ System 层（NumericSystem/AuctionSystem 等）
```

---

## ⚠️ 注意事项

| 问题 | 说明 | 解决方案 |
|------|------|----------|
| 重复添加组件 | 同一类型组件添加多次 | 先 `GetComponent<T>()` 检查 |
| 空引用 | 实体销毁后访问组件 | 检查 `entity.IsDispose` |
| 内存泄漏 | 组件持有资源未释放 | 在 `Destroy()` 中清理 |
| 循环引用 | 组件间互相引用 | 通过 Entity 中转 |
| Update 注销 | 组件销毁时未注销 Update | `BeforeDestroy` 中自动处理 |

---

## 🔍 设计原理

### 为什么使用 ECS？

```
传统继承方案:
Entity
└─→ Unit
    └─→ Character
        ├─→ Player
        ├─→ NPC
        └─→ Enemy

问题:
1. 类爆炸：每种种族/职业都需要新类
2. 难以组合：Player 想要 AI 功能？多重继承？
3. 代码复用：共享逻辑难以提取

ECS 方案:
Entity (空对象)
├─→ UnitComponent (单位数据)
├─→ CharacterComponent (角色数据)
├─→ AIComponent (AI 逻辑，可选)
├─→ PlayerComponent (玩家数据，可选)
└─→ ViewComponent (视图表现)

优势:
1. 灵活组合：需要 AI 就加 AIComponent
2. 代码复用：AIComponent 可用于 NPC/Enemy
3. 易于扩展：新功能加新组件，不影响现有代码
```

### 对象池优化

```
场景：创建/销毁 1000 个实体

不使用对象池:
- 创建：1000 次 new 操作
- 销毁：1000 次 GC
- 性能：差

使用对象池:
- 创建：从池中 Fetch（无 new）
- 销毁：回收到池（无 GC）
- 性能：10-100 倍提升
```

---

## 📚 相关文档

| 文档 | 说明 |
|------|------|
| [Entity.cs.md](../Entity/Entity.cs.md) | Entity 详细文档 |
| [Component.cs.md](../Component/Component.cs.md) | Component 详细文档 |
| [EntityManager.cs.md](../System/Entity/EntityManager.cs.md) | 实体管理器 |
| [对象池文档](../../Mono/Core/ObjectPool.cs.md) | 对象池系统 |

---

*文档由 OpenClaw AI 助手自动生成 | ECS 架构理解指南*
