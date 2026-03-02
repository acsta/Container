# EntityType.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | EntityType.cs |
| **路径** | Assets/Scripts/Mono/Module/Entity/EntityType.cs |
| **所属模块** | 框架层 → Mono/Module/Entity |
| **文件职责** | 定义实体类型枚举，用于区分游戏中不同类型的实体对象 |

---

## 枚举说明

### EntityType

| 属性 | 说明 |
|------|------|
| **职责** | 定义游戏中所有实体的类型分类 |
| **底层类型** | `byte`（占用 1 字节，取值范围 0-255） |
| **用途** | 实体过滤、碰撞检测、AI 行为判断等 |

```csharp
public enum EntityType : byte
{
    Bidder = 1,
    Host = 2,
    Npc = 3,
    Player = 4,
    Box = 5,
    Animal = 6,
    MAX,
    ALL,
}
```

---

## 枚举值详解

| 枚举值 | 数值 | 说明 | 使用场景 |
|--------|------|------|----------|
| `Bidder` | 1 | 竞拍者 | 参与拍卖的玩家实体 |
| `Host` | 2 | 主持人 | 游戏主持人/NPC 主持人 |
| `Npc` | 3 | NPC | 非玩家控制角色 |
| `Player` | 4 | 玩家 | 玩家控制的实体 |
| `Box` | 5 | 箱子/容器 | 可交互的容器对象 |
| `Animal` | 6 | 动物 | 动物类实体 |
| `MAX` | 7 | 最大值 | 用于数组大小、边界检查 |
| `ALL` | 8 | 全部 | 表示所有类型，用于全选过滤 |

---

## 设计原理

### 1. byte 类型选择

使用 `byte` 作为底层类型的原因：

**内存效率**:
```csharp
// byte 占用 1 字节
EntityType[] types = new EntityType[1000];  // 仅占用 1000 字节

// 如果使用 int（默认）
// EntityType[] types = new EntityType[1000];  // 占用 4000 字节
```

**网络传输**:
```csharp
// 网络消息中传输实体类型
// byte: 1 字节
// int: 4 字节
// 节省 75% 带宽
```

**性能考虑**:
- byte 足够容纳所有类型（当前仅 6 种）
- 现代 CPU 对 byte 操作有良好优化
- 减少缓存占用

### 2. 特殊值设计

**MAX 值**:
```csharp
// 用于数组大小
EntityType[] entityArrays = new EntityType[(int)EntityType.MAX];

// 用于边界检查
if (type < EntityType.MAX) { /* 有效类型 */ }
```

**ALL 值**:
```csharp
// 用于物理检测过滤
PhysicsHelper.OverlapSphereNonAllocEntity(
    center, radius, 
    new[] { EntityType.ALL },  // 检测所有类型
    out var entities);
```

---

## 使用示例

### 示例 1: 实体过滤

```csharp
// 检测玩家周围的 NPC
int count = PhysicsHelper.OverlapSphereNonAllocEntity(
    playerPos, 
    10f, 
    new[] { EntityType.Npc }, 
    out var entities);

// 检测所有实体
int count = PhysicsHelper.OverlapSphereNonAllocEntity(
    center, 
    radius, 
    new[] { EntityType.ALL }, 
    out var entities);
```

### 示例 2: 实体组件

```csharp
public class EntityComponent : MonoBehaviour
{
    public long Id { get; set; }
    public EntityType EntityType { get; set; }
    
    public void Initialize(EntityType type, long id)
    {
        EntityType = type;
        Id = id;
    }
}

// 使用
var entity = gameObject.AddComponent<EntityComponent>();
entity.Initialize(EntityType.Player, 12345);
```

### 示例 3: AI 行为判断

```csharp
public class AIController : MonoBehaviour
{
    [SerializeField] private EntityType targetType;
    
    public void Update()
    {
        // 根据目标类型执行不同行为
        switch (targetType)
        {
            case EntityType.Player:
                ChasePlayer();
                break;
            case EntityType.Npc:
                TalkToNpc();
                break;
            case EntityType.Box:
                OpenBox();
                break;
        }
    }
}
```

### 示例 4: 碰撞过滤

```csharp
public class HitBoxComponent : MonoBehaviour
{
    public EntityType ownerType;
    
    public void TakeDamage(EntityType attackerType, float damage)
    {
        // 友军免疫
        if (attackerType == ownerType)
        {
            return;
        }
        
        // 处理伤害
        ApplyDamage(damage);
    }
}
```

### 示例 5: 实体类型扩展

```csharp
// 添加新实体类型时，在 MAX 之前插入
public enum EntityType : byte
{
    Bidder = 1,
    Host = 2,
    Npc = 3,
    Player = 4,
    Box = 5,
    Animal = 6,
    Monster = 7,    // 新增
    MAX,            // 自动变为 8
    ALL,
}
```

---

## 与相关组件配合

### EntityComponent

```csharp
// EntityComponent 使用 EntityType 标识实体类型
public class EntityComponent : MonoBehaviour
{
    public EntityType EntityType { get; set; }
    public long Id { get; set; }
}

// 获取实体类型
var entity = gameObject.GetComponent<EntityComponent>();
if (entity.EntityType == EntityType.Player)
{
    // 玩家特定逻辑
}
```

### PhysicsHelper

```csharp
// PhysicsHelper 使用 EntityType 进行过滤
public static int OverlapSphereNonAllocEntity(
    Vector3 center, 
    float radius, 
    EntityType[] filter,
    out long[] res)
{
    // ... 实现
}

// 使用
int count = PhysicsHelper.OverlapSphereNonAllocEntity(
    center, radius, 
    new[] { EntityType.Player, EntityType.Npc }, 
    out var entities);
```

### HitInfo

```csharp
// HitInfo 包含实体 ID，可通过 EntityType 进一步过滤
public struct HitInfo
{
    public long EntityId;
    public Vector3 HitPos;
    public Vector3 HitDir;
    public float Distance;
    public HitBoxType HitBoxType;
}

// 获取击中实体后检查类型
var hitInfo = hitInfos[i];
var entity = GetEntityById(hitInfo.EntityId);
if (entity.EntityType == EntityType.Player)
{
    // 玩家被击中
}
```

---

## 注意事项

### 1. 类型值固定

枚举值一旦确定，不应随意修改，特别是已用于网络协议或存档的情况：

```csharp
// ❌ 错误：修改已有枚举值
public enum EntityType : byte
{
    Bidder = 2,  // 原来是 1，修改会导致兼容性问题
    Host = 1,    // 原来是 2
}

// ✅ 正确：只添加新值
public enum EntityType : byte
{
    Bidder = 1,
    Host = 2,
    // ... 原有值不变
    Monster = 7,  // 新增
}
```

### 2. MAX 值的使用

MAX 值会自动跟随枚举值数量变化：

```csharp
// 当前枚举
// Bidder=1, Host=2, Npc=3, Player=4, Box=5, Animal=6, MAX=7, ALL=8

// 添加新类型后
// Bidder=1, Host=2, Npc=3, Player=4, Box=5, Animal=6, Monster=7, MAX=8, ALL=9
```

### 3. ALL 值的特殊处理

ALL 值不是真实的实体类型，而是用于过滤的特殊标记：

```csharp
// ✅ 正确：用于过滤
new[] { EntityType.ALL }  // 表示所有类型

// ❌ 错误：作为实体类型
entity.EntityType = EntityType.ALL;  // 不应该这样用
```

### 4. 数组索引转换

如果需要将 EntityType 用作数组索引，需要减 1（因为从 1 开始）：

```csharp
// 实体类型数组
EntityData[] entityDataArray = new EntityData[(int)EntityType.MAX];

// 访问时
int index = (int)entityType - 1;  // 减 1 转换为 0 基索引
entityDataArray[index] = data;
```

---

## 扩展建议

### 添加新实体类型

```csharp
public enum EntityType : byte
{
    // 原有类型
    Bidder = 1,
    Host = 2,
    Npc = 3,
    Player = 4,
    Box = 5,
    Animal = 6,
    
    // 新增类型（在 MAX 之前）
    Monster = 7,
    Boss = 8,
    Pet = 9,
    
    // 特殊值
    MAX,    // 自动变为 10
    ALL,    // 自动变为 11
}
```

### 位掩码扩展（如果需要）

如果未来需要更复杂的类型过滤，可以考虑位掩码：

```csharp
[Flags]
public enum EntityTypeMask : ushort
{
    None = 0,
    Bidder = 1 << 0,    // 1
    Host = 1 << 1,      // 2
    Npc = 1 << 2,       // 4
    Player = 1 << 3,    // 8
    Box = 1 << 4,       // 16
    Animal = 1 << 5,    // 32
    All = 0xFFFF,
}

// 使用
EntityTypeMask mask = EntityTypeMask.Player | EntityTypeMask.Npc;
if ((entityMask & mask) != 0) { /* 匹配 */ }
```

---

## 相关文档

- [EntityComponent.cs.md](./EntityComponent.cs.md) - 实体组件
- [HitInfo.cs.md](./Hit/HitInfo.cs.md) - 击中信息
- [PhysicsHelper.cs.md](../../Helper/PhysicsHelper.cs.md) - 物理辅助工具

---

## 技术要点总结

| 要点 | 说明 |
|------|------|
| **byte 类型** | 节省内存和带宽 |
| **从 1 开始** | 0 保留给无效值 |
| **MAX 值** | 用于数组大小和边界检查 |
| **ALL 值** | 特殊标记，表示所有类型 |
| **固定值** | 枚举值不应随意修改 |

---

*文档生成时间：2026-03-01 | OpenClaw AI 助手*
