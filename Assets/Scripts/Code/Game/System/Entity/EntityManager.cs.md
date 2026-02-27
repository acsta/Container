# EntityManager.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | EntityManager.cs |
| **路径** | Assets/Scripts/Code/Game/System/Entity/EntityManager.cs |
| **所属模块** | 玩法层 → Code/Game/System/Entity |
| **文件职责** | 实体管理系统，负责实体的创建、销毁、查找、类型管理 |

---

## 类/结构体说明

### EntityManager

| 属性 | 说明 |
|------|------|
| **职责** | 管理所有游戏实体（Entity），提供对象池复用、类型索引 |
| **泛型参数** | 无 |
| **继承关系** | 无继承 |
| **实现的接口** | `IManager` |

**设计模式**: 单例模式 + 对象池 + ECS 模式

```csharp
// 单例实现
public static EntityManager Instance { get; private set; }

// 通过 ManagerProvider 注册
ManagerProvider.RegisterManager<EntityManager>();
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `Instance` | `EntityManager` | `public static` | 单例实例 |
| `root` | `GameObject` | `private` | 实体根节点 "EntityRoot" |
| `entities` | `ListComponent<Entity>` | `private` | 所有实体列表 |
| `idEntityMap` | `DictionaryComponent<long, Entity>` | `private` | ID → 实体映射 |
| `typeEntities` | `DictionaryComponent<Type, IList>` | `private` | 类型 → 实体列表映射 |
| `GameObjectRoot` | `Transform` | `public` | 实体根节点 Transform |

---

## 方法说明（按重要程度排序）

### Init()

**签名**:
```csharp
public void Init()
```

**职责**: 初始化实体管理器

**核心逻辑**:
```
1. 设置单例 Instance = this
2. 创建 entities 列表（所有实体）
3. 创建 idEntityMap 字典（ID 索引）
4. 创建 typeEntities 字典（类型索引）
5. 创建 "EntityRoot" GameObject
6. 设置 DontDestroyOnLoad（跨场景保留）
```

**调用者**: ManagerProvider.RegisterManager<EntityManager>()

---

### Destroy()

**签名**:
```csharp
public void Destroy()
```

**职责**: 销毁实体管理器，清理所有实体

**核心逻辑**:
```
1. 设置 Instance = null
2. 倒序遍历所有实体，逐个调用 Dispose()
3. 释放 entities/idEntityMap/typeEntities
4. 销毁 EntityRoot GameObject
```

**调用者**: ManagerProvider.RemoveManager<EntityManager>()

---

### CreateEntity<T>(...)

**签名** (多个重载):
```csharp
public T CreateEntity<T>() where T : Entity, IEntity
public T CreateEntity<T, P1>(P1 p1) where T : Entity, IEntity<P1>
public T CreateEntity<T, P1, P2>(P1 p1, P2 p2) where T : Entity, IEntity<P1, P2>
// ... 最多 5 个参数
```

**职责**: 创建实体（从对象池获取）

**核心逻辑**:
```
1. 从对象池获取实体 ObjectPool.Instance.Fetch<T>()
2. 调用 BeforeInit(this) 设置 EntityManager
3. 调用 Init() 或 Init(p1, p2...) 初始化
4. 添加到管理器 Add(res)
5. 返回实体
```

**调用者**: 需要创建实体的代码

**使用示例**:
```csharp
// 创建无参数实体
var entity = EntityManager.Instance.CreateEntity<Entity>();

// 创建带参数实体
var player = EntityManager.Instance.CreateEntity<Player, int>(playerId);

// 创建带多个参数实体
var bidder = EntityManager.Instance.CreateEntity<Bidder, int, bool>(aiId, isBlack);
```

---

### Get(long id) / Get<T>(long id)

**签名**:
```csharp
public Entity Get(long id)
public T Get<T>(long id) where T : Entity
```

**职责**: 根据 ID 获取实体

**参数**:
- `id`: 实体 ID

**返回**: 实体对象（如果不存在或已销毁则返回 null）

**核心逻辑**:
```
1. 从 idEntityMap 查找
2. 检查 IsDispose 标志
3. 返回实体或 null
```

**调用者**: 需要通过 ID 访问实体的代码

**使用示例**:
```csharp
// 获取实体
Entity entity = EntityManager.Instance.Get(entityId);

// 获取指定类型实体
Player player = EntityManager.Instance.Get<Player>(playerId);
```

---

### TryGet(long id, out Entity res)

**签名**:
```csharp
public bool TryGet(long id, out Entity res)
```

**职责**: 尝试获取实体（安全版本）

**参数**:
- `id`: 实体 ID
- `res`: 输出参数

**返回**: `true` = 获取成功，`false` = 不存在

**使用示例**:
```csharp
if (EntityManager.Instance.TryGet(entityId, out var entity))
{
    // 使用 entity
}
else
{
    // 实体不存在
}
```

---

### GetAll<T>()

**签名**:
```csharp
public List<T> GetAll<T>() where T : Entity
```

**职责**: 获取指定类型的所有实体

**返回**: 该类型实体列表

**核心逻辑**:
```
1. 从 typeEntities 查找类型对应的列表
2. 如果不存在，创建新列表并缓存
3. 返回列表
```

**调用者**: 需要遍历某类实体的代码

**使用示例**:
```csharp
// 获取所有玩家
List<Player> players = EntityManager.Instance.GetAll<Player>();

// 获取所有竞拍者
List<Bidder> bidders = EntityManager.Instance.GetAll<Bidder>();

// 遍历
foreach (var bidder in bidders)
{
    bidder.DoSomething();
}
```

---

### GetAllDict()

**签名**:
```csharp
public Dictionary<long, Entity> GetAllDict()
```

**职责**: 获取所有实体的 ID 字典

**返回**: idEntityMap

**调用者**: 需要遍历所有实体的代码

---

### Remove<T>(T entity) / Remove(long id)

**签名**:
```csharp
public void Remove<T>(T entity) where T : Entity
public void Remove(long id)
```

**职责**: 移除/销毁实体

**核心逻辑**:
```
1. 从 idEntityMap 移除
2. 从 entities 列表移除
3. 从 typeEntities 列表移除
4. 调用 entity.Dispose()
```

**调用者**: 需要销毁实体的代码

**使用示例**:
```csharp
// 通过实体移除
EntityManager.Instance.Remove(entity);

// 通过 ID 移除
EntityManager.Instance.Remove(entityId);
```

---

### GetTotal()

**签名**:
```csharp
public int GetTotal()
```

**职责**: 获取实体总数

**返回**: 实体数量

**调用者**: 性能监控、调试

---

## 实体系统设计

### 实体基类 (Entity)

```csharp
// Entity 基类位于 Code/Game/Entity/Entity.cs
public class Entity : IDisposable
{
    public long Id { get; set; }
    public bool IsDispose { get; private set; }
    
    // 组件系统
    private DictionaryComponent<Type, IComponent> components;
    
    public T AddComponent<T>() where T : IComponent, new()
    public T GetComponent<T>() where T : IComponent
    public void RemoveComponent<T>()
    
    // 生命周期
    public virtual void BeforeInit(EntityManager manager)
    public virtual void Init()
    public virtual void Dispose()
}
```

### 实体接口

```csharp
// 支持不同参数数量的初始化
public interface IEntity { void Init(); }
public interface IEntity<P1> { void Init(P1 p1); }
public interface IEntity<P1, P2> { void Init(P1 p1, P2 p2); }
// ... 最多 5 个参数
```

---

## 阅读指引

### 建议的阅读顺序

1. **理解实体系统作用** - 为什么需要 EntityManager
2. **看 CreateEntity** - 理解实体创建
3. **看 Get/GetAll** - 理解实体查找
4. **看 Remove** - 理解实体销毁

### 最值得学习的技术点

1. **对象池**: Fetch/Recycle 复用实体，减少 GC
2. **类型索引**: typeEntities 支持按类型快速查找
3. **泛型重载**: 支持 0-5 个参数的创建方法
4. **ECS 模式**: Entity + Component 设计
5. **ID 生成**: 每个实体有唯一 ID

---

## 使用示例

### 示例 1: 创建实体

```csharp
// 创建玩家实体
var player = EntityManager.Instance.CreateEntity<Player, int>(playerId);

// 创建竞拍者实体
var bidder = EntityManager.Instance.CreateEntity<Bidder, int, bool>(
    aiConfigId,
    isBlackMerchant
);

// 创建箱子实体
var box = EntityManager.Instance.CreateEntity<Box, int>(itemId);
```

### 示例 2: 查找实体

```csharp
// 通过 ID 查找
Player player = EntityManager.Instance.Get<Player>(playerId);

// 获取所有同类实体
List<Bidder> bidders = EntityManager.Instance.GetAll<Bidder>();

// 遍历处理
foreach (var bidder in bidders)
{
    if (bidder.IsThinking)
    {
        bidder.MakeDecision();
    }
}
```

### 示例 3: 移除实体

```csharp
// 竞拍者离场
EntityManager.Instance.Remove(bidder.Id);

// 箱子打开后销毁
EntityManager.Instance.Remove(box);
```

### 示例 4: 实体组件

```csharp
// 添加组件
var aiComponent = player.AddComponent<AIComponent>();
aiComponent.SetDecisionTree(tree);

// 获取组件
var numericComponent = player.GetComponent<NumericComponent>();
int hp = numericComponent.GetAttribute(NumericType.Hp);

// 移除组件
player.RemoveComponent<AIComponent>();
```

---

## 实体生命周期

```
创建 → BeforeInit → Init → [Update] → Dispose → 回收到对象池
  ↓                    ↓                          ↓
Fetch 对象池       初始化完成                  Recycle 对象池
```

---

## 相关文档

- [Entity.cs.md](../../Entity/Entity.cs.md) - 实体基类
- [ObjectPool.cs.md](../../../ThirdParty/ObjectPool/ObjectPool.cs.md) - 对象池
- [ManagerProvider.cs.md](../../../Mono/Core/Manager/ManagerProvider.cs.md) - Manager 注册

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
