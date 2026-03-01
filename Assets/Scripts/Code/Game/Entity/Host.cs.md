# Host.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | Host.cs |
| **路径** | Assets/Scripts/Code/Game/Entity/Host.cs |
| **所属模块** | 游戏层 → Code/Game/Entity |
| **文件职责** | 主机实体类，继承 Unit，实现 IEntity 接口，用于表示场景中的主机单位（带有休闲动作组件） |

---

## 类/结构体说明

### Host

| 属性 | 说明 |
|------|------|
| **职责** | 表示场景中的主机实体，支持通过配置 ID 初始化，自动启用休闲动作组件 |
| **泛型参数** | 无 |
| **继承关系** | 继承 `Unit` 类 |
| **实现的接口** | `IEntity<int>` |

**设计模式**: 实体组件模式

```csharp
// 使用方式
// 通过 EntityManager 创建主机实体
var host = entityManager.CreateEntity<Host, int>(configId);
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `Type` | `EntityType` | `public override` | 实体类型，固定为 `EntityType.Host` |
| `ConfigId` | `int` | `protected` | 配置表 ID（继承自 Unit） |

---

## 方法说明（按重要程度排序）

### Init(int id)

**签名**:
```csharp
public void Init(int id)
```

**职责**: 初始化主机实体

**核心逻辑**:
```
1. 设置 ConfigId = id
2. 添加 GameObjectHolderComponent 组件（用于管理 GameObject 加载）
3. 添加 CasualActionComponent 组件
4. 启用休闲动作组件（SetEnable(true)）
```

**调用者**: `EntityManager.CreateEntity<Host, int>()`

**被调用者**: `AddComponent<GameObjectHolderComponent>()`, `AddComponent<CasualActionComponent>()`

---

### Destroy()

**签名**:
```csharp
public void Destroy()
```

**职责**: 销毁主机实体（当前为空实现）

**调用者**: `EntityManager.RemoveEntity()`

---

## Mermaid 流程图

### Host 类继承关系

```mermaid
classDiagram
    class Entity {
        +Id: long
        +IsDispose: bool
    }
    
    class SceneEntity {
        +Position: Vector3
        +Rotation: Quaternion
        +LocalScale: Vector3
    }
    
    class Unit {
        +ConfigId: int
        +Config: UnitConfig
    }
    
    class Host {
        +Type: EntityType
        +Init(int)
        +Destroy()
    }
    
    class IEntity {
        <<interface>>
        +Init()
        +Destroy()
    }
    
    Entity <|-- SceneEntity
    SceneEntity <|-- Unit
    Unit <|-- Host
    IEntity <|.. Host
```

### 初始化流程

```mermaid
sequenceDiagram
    participant EM as EntityManager
    participant H as Host
    participant GHC as GameObjectHolderComponent
    participant CAC as CasualActionComponent

    EM->>H: CreateEntity<Host, int>(id)
    EM->>H: Init(id)
    H->>H: ConfigId = id
    H->>GHC: AddComponent
    H->>CAC: AddComponent
    H->>CAC: SetEnable(true)
    H-->>EM: 返回主机实体
```

---

## 使用示例

### 创建主机实体

```csharp
// 通过 EntityManager 创建
var entityManager = ManagerProvider.GetManager<EntityManager>();
var host = entityManager.CreateEntity<Host, int>(configId);

// 主机会自动加载对应的 GameObject 并启用休闲动作
var ghc = host.GetComponent<GameObjectHolderComponent>();
await ghc.WaitLoadGameObjectOver();

// 获取休闲动作组件
var cac = host.GetComponent<CasualActionComponent>();
Debug.Log($"休闲动作已启用：{cac.IsEnabled}");
```

### 获取主机配置

```csharp
// 通过 ConfigId 获取配置
var config = host.Config;
Debug.Log($"主机名称：{config.Name}");
Debug.Log($"预制体路径：{config.Perfab}");
```

---

## 相关文档链接

- [Unit.cs.md](Unit.cs.md) - 场景单位基类
- [SceneEntity.cs.md](SceneEntity.cs.md) - 场景实体基类
- [Entity.cs.md](Entity.cs.md) - 实体基类
- [EntityManager.cs.md](../../System/Entity/EntityManager.cs.md) - 实体管理器
- [CasualActionComponent.cs.md](../../Component/AI/CasualActionComponent.cs.md) - 休闲动作组件
- [EntityType.cs.md](../../../Mono/Module/Entity/EntityType.cs.md) - 实体类型枚举

---

*文档生成时间：2026-03-02*
