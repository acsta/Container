# Entity.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | Entity.cs |
| **路径** | Assets/Scripts/Code/Game/Entity/Entity.cs |
| **所属模块** | 玩法层 → Code/Game/Entity |
| **文件职责** | 实体基类，提供组件系统、生命周期管理、对象池复用 |

---

## 类/结构体说明

### Entity (抽象类)

| 属性 | 说明 |
|------|------|
| **职责** | 所有游戏实体的基类，提供组件系统和生命周期 |
| **泛型参数** | 无 |
| **继承关系** | 无继承（抽象类） |
| **实现的接口** | `IDisposable` |

**设计模式**: 组件模式 + 对象池 + ECS 模式

```csharp
// 实体示例
public class Player : Entity
{
    public override EntityType Type => EntityType.Player;
    
    public void Init(int playerId)
    {
        // 初始化玩家
        AddComponent<NumericComponent>();
        AddComponent<AIComponent>();
    }
}

// 创建实体
var player = EntityManager.Instance.CreateEntity<Player, int>(playerId);
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `Id` | `long` | `public` | 实体唯一 ID（自动生成） |
| `IsDispose` | `bool` | `public` | 是否已销毁 |
| `Parent` | `EntityManager` | `public` | 所属实体管理器 |
| `Type` | `EntityType` | `public abstract` | 实体类型（子类实现） |
| `CreateTime` | `long` | `public` | 创建时间戳 |
| `Components` | `DictionaryComponent<Type, Component>` | `protected` | 自身组件字典 |
| `OtherComponents` | `DictionaryComponent<Type, Component>` | `protected` | 共享组件字典 |
| `BaseType` | `DictionaryComponent<Component, Type>` | `protected` | 基类组件映射 |

---

## 方法说明（按重要程度排序）

### Dispose()

**签名**:
```csharp
public void Dispose()
```

**职责**: 销毁实体，清理组件，回收到对象池

**核心逻辑**:
```
1. 检查是否已销毁（IsDispose）
2. 设置 IsDispose = true
3. 移除延迟销毁定时器
4. 遍历所有组件：
   - 调用 BeforeDestroy()
   - 调用 Destroy()（如果实现 IComponentDestroy）
   - 调用 AfterDestroy()
5. 释放 Components/OtherComponents/BaseType
6. 调用 IEntityDestroy.Destroy()（如果实现）
7. 从 EntityManager 移除
8. 回收到对象池 ObjectPool.Instance.Recycle(this)
```

**调用者**: EntityManager.Remove(), DelayDestroyEntityTimer

---

### BeforeInit(EntityManager um)

**签名**:
```csharp
public void BeforeInit(EntityManager um)
```

**职责**: 初始化前准备（由 EntityManager 调用）

**核心逻辑**:
```
1. 设置 Parent = um
2. 生成唯一 ID Id = IdGenerater.Instance.GenerateInstanceId()
3. 设置 IsDispose = false
4. 创建 Components/OtherComponents/BaseType 字典
5. 记录创建时间 CreateTime
```

**调用者**: EntityManager.CreateEntity<T>()

---

### AddComponent<T>(Type baseType)

**签名** (多个重载):
```csharp
public T AddComponent<T>(Type baseType = null) where T : Component
public T AddComponent<T, P1>(P1 p1, Type baseType = null) where T : Component, IComponent<P1>
public T AddComponent<T, P1, P2>(P1 p1, P2 p2) where T : Component, IComponent<P1, P2>
public T AddComponent<T, P1, P2, P3>(P1 p1, P2 p2, P3 p3) where T : Component, IComponent<P1, P2, P3>
```

**职责**: 添加组件到实体

**参数**:
- `baseType`: 基类类型（用于多态查询）

**核心逻辑**:
```
1. 检查是否已销毁
2. 检查是否重复添加
3. 从对象池获取组件 ObjectPool.Instance.Fetch()
4. 调用 BeforeInit(this)
5. 添加到 Components 字典
6. 如果有 baseType，同时添加基类映射
7. 调用 Init() 或 Init(p1, p2...)
8. 调用 AfterInit()
9. 返回组件
```

**调用者**: 实体子类 Init() 方法

**使用示例**:
```csharp
// 添加无参数组件
var numericComponent = AddComponent<NumericComponent>();

// 添加带参数组件
var aiComponent = AddComponent<AIComponent, DecisionTree>(decisionTree);

// 添加带基类的组件（支持多态查询）
AddComponent<WarriorComponent, UnitComponent>(typeof(UnitComponent));
```

---

### GetComponent<T>(bool includeOther)

**签名**:
```csharp
public T GetComponent<T>(bool includeOther = true) where T : Component, IComponentDestroy
```

**职责**: 获取组件（支持查找共享组件）

**参数**:
- `includeOther`: 是否包含共享组件

**返回**: 组件对象（不存在返回 null）

**核心逻辑**:
```
1. 检查是否已销毁
2. 如果 includeOther，先查找 OtherComponents
3. 查找 Components
4. 返回组件或 null
```

**调用者**: 需要访问组件的逻辑

**使用示例**:
```csharp
// 获取组件
var numericComponent = GetComponent<NumericComponent>();
int hp = numericComponent.GetAttribute(NumericType.Hp);

// 不查找共享组件
var aiComponent = GetComponent<AIComponent>(includeOther: false);
```

---

### GetOrAddComponent<T>()

**签名**:
```csharp
public T GetOrAddComponent<T>() where T : Component, IComponent
```

**职责**: 获取组件，如果不存在则添加

**返回**: 组件对象

**核心逻辑**:
```
1. 尝试 GetComponent<T>()
2. 如果不存在，调用 AddComponent<T>()
3. 返回组件
```

**使用示例**:
```csharp
// 获取或添加组件
var component = GetOrAddComponent<NumericComponent>();
```

---

### RemoveComponent<T>()

**签名**:
```csharp
public void RemoveComponent<T>() where T : Component
public void RemoveComponent(Type type)
```

**职责**: 移除组件

**核心逻辑**:
```
1. 从 Components 查找组件
2. 如果有基类映射，同时移除基类
3. 从 Components 移除
4. 调用组件 Dispose()
```

**调用者**: 需要移除组件的逻辑

---

### AddOtherComponent<T>(T t)

**签名**:
```csharp
public void AddOtherComponent<T>(T t) where T : Component
```

**职责**: 添加共享组件（不管理生命周期）

**参数**:
- `t`: 组件对象

**核心逻辑**:
```
1. 添加到 OtherComponents 字典
2. 不调用生命周期方法
```

**调用者**: 需要共享组件的场景

**使用示例**:
```csharp
// 共享其他实体的组件
var otherNumeric = otherEntity.GetComponent<NumericComponent>();
AddOtherComponent(otherNumeric);
```

---

### DelayDispose(long delay)

**签名**:
```csharp
public void DelayDispose(long delay)
```

**职责**: 延迟销毁实体

**参数**:
- `delay`: 延迟时间（毫秒）

**核心逻辑**:
```
1. 移除旧的延迟定时器（如果有）
2. 创建新的延迟定时器
3. 定时器到期后调用 Dispose()
```

**调用者**: 需要延迟销毁的场景

**使用示例**:
```csharp
// 1 秒后销毁
entity.DelayDispose(1000);

// 3 秒后销毁
entity.DelayDispose(3000);
```

---

## 组件系统

### 组件生命周期

```
创建 → BeforeInit → Init → AfterInit → [Update] → BeforeDestroy → Destroy → AfterDestroy → Dispose
```

### 组件接口

```csharp
// 基础组件接口
public interface IComponent { void Init(); }
public interface IComponent<P1> { void Init(P1 p1); }
// ... 最多 3 个参数

// 生命周期接口
public interface IComponentDestroy { void Destroy(); }
public interface IEntityDestroy { void Destroy(); }
```

### 组件示例

```csharp
// 数值组件
public class NumericComponent : Component, IComponent
{
    private Dictionary<NumericType, BigNumber> attributes;
    
    public override void Init()
    {
        attributes = new Dictionary<NumericType, BigNumber>();
    }
    
    public BigNumber GetAttribute(NumericType type)
    {
        return attributes[type];
    }
    
    public void SetAttribute(NumericType type, BigNumber value)
    {
        attributes[type] = value;
    }
}

// AI 组件
public class AIComponent : Component, IComponent<DecisionTree>
{
    private DecisionTree decisionTree;
    
    public override void Init(DecisionTree p1)
    {
        decisionTree = p1;
    }
    
    public void MakeDecision()
    {
        decisionTree.Evaluate();
    }
}
```

---

## 阅读指引

### 建议的阅读顺序

1. **理解实体基类作用** - 为什么需要 Entity
2. **看 Dispose** - 理解销毁流程
3. **看 AddComponent** - 理解组件添加
4. **看 GetComponent** - 理解组件获取

### 最值得学习的技术点

1. **组件模式**: 通过组件扩展实体功能
2. **对象池**: Fetch/Recycle 复用组件
3. **生命周期管理**: BeforeInit/Init/AfterInit/Destroy
4. **共享组件**: OtherComponents 支持组件共享
5. **基类映射**: BaseType 支持多态查询
6. **延迟销毁**: DelayDispose 延迟清理

---

## 使用示例

### 示例 1: 创建实体

```csharp
// 玩家实体
public class Player : Entity
{
    public override EntityType Type => EntityType.Player;
    
    public override void Init(int playerId)
    {
        // 添加组件
        AddComponent<NumericComponent>();
        AddComponent<AIComponent>();
        AddComponent<PlayerDataComponent, int>(playerId);
    }
}

// 创建
var player = EntityManager.Instance.CreateEntity<Player, int>(playerId);
```

### 示例 2: 使用组件

```csharp
// 获取组件
var numericComponent = player.GetComponent<NumericComponent>();
var aiComponent = player.GetComponent<AIComponent>();

// 使用组件
numericComponent.SetAttribute(NumericType.Hp, 100);
aiComponent.MakeDecision();
```

### 示例 3: 移除组件

```csharp
// 移除指定组件
player.RemoveComponent<AIComponent>();

// 延迟销毁实体
player.DelayDispose(5000);  // 5 秒后销毁
```

---

## 实体类型枚举

```csharp
public enum EntityType
{
    None,
    Player,        // 玩家
    Bidder,        // 竞拍者
    Box,           // 箱子
    Host,          // 拍卖师
    NPC,           // NPC
    // ... 更多类型
}
```

---

## 相关文档

- [EntityManager.cs.md](../System/Entity/EntityManager.cs.md) - 实体管理器
- [Component.cs.md](./Component.cs.md) - 组件基类
- [ObjectPool.cs.md](../../../ThirdParty/ObjectPool/ObjectPool.cs.md) - 对象池

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
