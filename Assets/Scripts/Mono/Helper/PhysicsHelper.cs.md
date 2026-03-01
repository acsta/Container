# PhysicsHelper.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | PhysicsHelper.cs |
| **路径** | Assets/Scripts/Mono/Helper/PhysicsHelper.cs |
| **所属模块** | Mono/Helper |
| **文件职责** | 物理检测工具类，提供高效的射线/碰撞检测，支持实体过滤 |

---

## 类说明

### PhysicsHelper

| 属性 | 说明 |
|------|------|
| **职责** | 封装 Unity 物理检测 API，提供实体过滤、HitInfo 处理等功能 |
| **类型** | `static class` |

---

## 核心方法

### RaycastNonAlloc

```csharp
public static bool RaycastNonAlloc(Vector3 origin, Vector3 direction, 
    out RaycastHit result, float maxDistance, int layerMask, 
    QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
```

**职责**: 射线检测 (返回最近命中)

---

### OverlapSphereNonAllocEntity

```csharp
public static int OverlapSphereNonAllocEntity(Vector3 center, float radius, 
    EntityType[] filter, out long[] res)
```

**职责**: 球形范围检测实体 ID

---

### OverlapBoxNonAllocHitInfo

```csharp
public static int OverlapBoxNonAllocHitInfo(Vector3 center, Vector3 halfExtents, 
    Quaternion orientation, EntityType[] filter, CheckHitLayerType type, out HitInfo[] res)
```

**职责**: 盒形范围检测 HitInfo

---

### RaycastNonAllocHitInfo

```csharp
public static int RaycastNonAllocHitInfo(Vector3 origin, Vector3 direction, 
    float maxDistance, EntityType[] filter, CheckHitLayerType type, out HitInfo[] res)
```

**职责**: 射线检测 HitInfo (含距离、方向、受击点)

---

## 使用示例

```csharp
// 射线检测实体
if (PhysicsHelper.RaycastNonAllocEntity(origin, direction, 10f, 
    new[] { EntityType.Player }, out long[] entities))
{
    foreach (var entityId in entities)
    {
        // 处理命中实体
    }
}

// 球形范围检测 HitInfo
int count = PhysicsHelper.OverlapSphereNonAllocHitInfo(
    center, radius, 
    new[] { EntityType.Enemy }, 
    CheckHitLayerType.OnlyHitBox, 
    out HitInfo[] hitInfos);
```

---

## 相关文档

- [HitInfo.cs.md](../Module/Entity/Hit/HitInfo.cs.md) - 击中信息
- [EntityType.cs.md](../Module/Entity/EntityType.cs.md) - 实体类型
- [CheckHitLayerType.cs.md](../Module/Entity/Hit/CheckHitLayerType.cs.md) - 检测层级类型

---

*文档生成时间：2026-03-02 | OpenClaw AI 助手*
