# HitInfo.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | HitInfo.cs |
| **路径** | Assets/Scripts/Mono/Module/Entity/Hit/HitInfo.cs |
| **所属模块** | 框架层 → Mono/Module/Entity/Hit |
| **文件职责** | 定义击中检测的结果数据结构 |

---

## 结构体说明

### HitInfo

| 属性 | 说明 |
|------|------|
| **职责** | 存储射线检测/击中检测的完整结果信息 |
| **类型** | `struct`（值类型） |
| **命名空间** | `TaoTie` |

**设计模式**: 数据传输对象（DTO）

```csharp
// 使用方式
// 1. HitSystem.Raycast() 返回 HitInfo
// 2. 包含击中位置、方向、距离、目标实体 ID、HitBox 类型
// 3. 用于后续伤害计算、特效生成等
```

---

## 字段说明

| 名称 | 类型 | 说明 |
|------|------|------|
| `EntityId` | `long` | 被击中的实体 ID（0 表示未击中实体） |
| `HitPos` | `Vector3` | 击中点的世界坐标 |
| `HitDir` | `Vector3` | 击中方向（归一化向量） |
| `Distance` | `float` | 从射线原点到击中点的距离 |
| `HitBoxType` | `HitBoxType` | 击中的 HitBox 类型（Normal/Head） |

---

## 字段详解

### EntityId

**类型**: `long`

**说明**: 被击中实体的唯一标识符

**值含义**:
- `0`: 未击中任何实体（可能击中场景或空气）
- `>0`: 击中的实体 ID

**使用示例**:
```csharp
HitInfo hit = HitSystem.Raycast(origin, direction, maxDistance);

if (hit.EntityId != 0)
{
    // 击中了实体
    Entity entity = EntityManager.Instance.Get(hit.EntityId);
    entity.TakeDamage(damage);
}
else
{
    // 未击中实体（可能击中场景）
    SpawnBulletHole(hit.HitPos);
}
```

---

### HitPos

**类型**: `Vector3`

**说明**: 击中点的世界空间坐标

**使用示例**:
```csharp
// 在击中点生成特效
SpawnEffect("BloodSplatter", hit.HitPos);

// 在击中点生成弹孔
SpawnDecal("BulletHole", hit.HitPos, hit.HitDir);

// 计算击中点到目标的距离
float distToTarget = Vector3.Distance(hit.HitPos, target.position);
```

---

### HitDir

**类型**: `Vector3`

**说明**: 击中时的方向向量（通常为射线方向，归一化）

**使用示例**:
```csharp
// 生成击中特效（方向影响粒子喷射方向）
SpawnEffect("BloodSplatter", hit.HitPos, hit.HitDir);

// 计算反射方向
Vector3 reflectDir = Vector3.Reflect(hit.HitDir, surfaceNormal);

// 生成弹孔（方向决定弹孔朝向）
SpawnDecal("BulletHole", hit.HitPos, Quaternion.LookRotation(hit.HitDir));
```

---

### Distance

**类型**: `float`

**说明**: 从射线原点到击中点的距离（单位：米）

**使用示例**:
```csharp
// 检查是否在有效射程内
if (hit.Distance <= weaponRange)
{
    ApplyDamage(hit.EntityId);
}

// 计算伤害衰减
float damageMultiplier = 1.0f - (hit.Distance / maxEffectiveRange);
damageMultiplier = Mathf.Clamp(damageMultiplier, 0.5f, 1.0f);
ApplyDamage(hit.EntityId, baseDamage * damageMultiplier);

// 检查是否击中
bool hasHit = hit.Distance > 0;
```

---

### HitBoxType

**类型**: `HitBoxType`

**说明**: 击中的 HitBox 类型

**值**:
- `Normal` (0): 普通击中（身体、四肢）
- `Head` (1): 头部击中（爆头）

**使用示例**:
```csharp
// 根据 HitBox 类型计算伤害
float damage = baseDamage;
if (hit.HitBoxType == HitBoxType.Head)
{
    damage *= headshotMultiplier;  // 爆头伤害倍率
    PlayHeadshotSound();
}

ApplyDamage(hit.EntityId, damage);
```

**详细说明**: 参见 [HitBoxType.cs.md](./HitBoxType.cs.md)

---

## 使用示例

### 示例 1: 基础射击检测

```csharp
public void Shoot(Vector3 origin, Vector3 direction, float maxDistance)
{
    HitInfo hit = HitSystem.Raycast(origin, direction, maxDistance);
    
    if (hit.EntityId != 0)
    {
        // 击中敌人
        Entity target = EntityManager.Instance.Get(hit.EntityId);
        target.TakeDamage(calculateDamage(hit));
        
        // 生成击中特效
        SpawnHitEffect(hit.HitPos, hit.HitDir);
    }
    else if (hit.Distance > 0)
    {
        // 击中场景
        SpawnBulletHole(hit.HitPos, hit.HitDir);
    }
}

float calculateDamage(HitInfo hit)
{
    float damage = baseDamage;
    
    // 爆头加成
    if (hit.HitBoxType == HitBoxType.Head)
    {
        damage *= 2.0f;
    }
    
    // 距离衰减
    damage *= 1.0f - (hit.Distance / maxEffectiveRange);
    
    return damage;
}
```

### 示例 2: 近战攻击检测

```csharp
public void PerformMeleeAttack()
{
    Vector3 origin = transform.position + Vector3.up * 1.5f;
    Vector3 direction = transform.forward;
    float range = 3f;
    
    HitInfo hit = HitSystem.Raycast(origin, direction, range, CheckHitLayerType.OnlyHitBox);
    
    if (hit.EntityId != 0)
    {
        // 击中敌人
        Entity target = EntityManager.Instance.Get(hit.EntityId);
        target.TakeDamage(meleeDamage);
        target.ApplyKnockback(direction, knockbackForce);
        
        // 播放击中动画
        PlayHitAnimation(hit.HitPos);
        
        // 生成血液特效
        SpawnBloodEffect(hit.HitPos);
    }
}
```

### 示例 3: 狙击检测

```csharp
public void SniperShoot()
{
    Vector3 origin = sniperMuzzle.position;
    Vector3 direction = sniperSight.forward;
    float maxDistance = 500f;
    
    HitInfo hit = HitSystem.Raycast(origin, direction, maxDistance, CheckHitLayerType.Both);
    
    if (hit.EntityId != 0)
    {
        // 击中目标
        Entity target = EntityManager.Instance.Get(hit.EntityId);
        
        // 狙击伤害（高伤害，无视部分护甲）
        float damage = sniperDamage;
        if (hit.HitBoxType == HitBoxType.Head)
        {
            damage *= 3.0f;  // 狙击爆头三倍伤害
        }
        
        target.TakeDamage(damage);
        
        // 狙击特效
        SpawnSniperHitEffect(hit.HitPos);
        PlaySniperSound();
    }
    
    // 狙击弹道轨迹
    SpawnBulletTrail(origin, hit.HitPos);
}
```

### 示例 4: 范围爆炸检测

```csharp
public void Explosion(Vector3 center, float radius, float damage)
{
    Collider[] colliders = Physics.OverlapSphere(center, radius);
    
    foreach (var collider in colliders)
    {
        var hitBox = collider.GetComponent<HitBoxComponent>();
        if (hitBox != null)
        {
            // 构建 HitInfo
            HitInfo hit = new HitInfo
            {
                EntityId = GetEntityId(collider),
                HitPos = collider.ClosestPoint(center),
                HitDir = (collider.transform.position - center).normalized,
                Distance = Vector3.Distance(center, collider.transform.position),
                HitBoxType = hitBox.HitBoxType
            };
            
            // 根据距离计算伤害衰减
            float distanceFactor = 1.0f - (hit.Distance / radius);
            float finalDamage = damage * distanceFactor;
            
            Entity entity = EntityManager.Instance.Get(hit.EntityId);
            entity.TakeDamage(finalDamage);
            entity.ApplyExplosionForce(hit.HitDir, explosionForce * distanceFactor);
        }
    }
}
```

### 示例 5: 击中信息日志

```csharp
public void LogHitInfo(HitInfo hit)
{
    if (hit.EntityId != 0)
    {
        Debug.Log($"击中实体 ID: {hit.EntityId}");
        Debug.Log($"击中位置：{hit.HitPos}");
        Debug.Log($"击中方向：{hit.HitDir}");
        Debug.Log($"距离：{hit.Distance:F2}m");
        Debug.Log($"HitBox 类型：{hit.HitBoxType}");
    }
    else
    {
        Debug.Log($"未击中实体，距离：{hit.Distance:F2}m");
    }
}
```

---

## 数据结构关系

```
HitInfo (击中信息)
├── EntityId: long         → EntityManager → Entity (实体)
├── HitPos: Vector3        → 世界坐标
├── HitDir: Vector3        → 方向向量
├── Distance: float        → 距离（米）
└── HitBoxType: HitBoxType → 击中部位类型
    ├── Normal (0)         → 普通伤害
    └── Head (1)           → 爆头伤害
```

---

## 与 Unity RaycastHit 的对比

| 属性 | HitInfo | Unity RaycastHit | 说明 |
|------|---------|------------------|------|
| 实体 ID | `EntityId` | 无 | 游戏实体系统 ID |
| 击中点 | `HitPos` | `point` | 世界坐标 |
| 方向 | `HitDir` | 需自行计算 | 归一化方向 |
| 距离 | `Distance` | `distance` | 相同 |
| HitBox 类型 | `HitBoxType` | 无 | 自定义类型 |
| Collider | 无 | `collider` | HitInfo 不直接存储 |

**转换示例**:
```csharp
// Unity RaycastHit → HitInfo
RaycastHit unityHit;
if (Physics.Raycast(ray, out unityHit))
{
    var hitBox = unityHit.collider.GetComponent<HitBoxComponent>();
    
    HitInfo hit = new HitInfo
    {
        EntityId = GetEntityId(unityHit.collider),
        HitPos = unityHit.point,
        HitDir = ray.direction,
        Distance = unityHit.distance,
        HitBoxType = hitBox != null ? hitBox.HitBoxType : HitBoxType.Normal
    };
}
```

---

## 相关文档

- **HitBoxType**: [HitBoxType.cs.md](./HitBoxType.cs.md) - HitBox 类型枚举
- **HitBoxComponent**: [HitBoxComponent.cs.md](./HitBoxComponent.cs.md) - HitBox 组件
- **CheckHitLayerType**: [CheckHitLayerType.cs.md](./CheckHitLayerType.cs.md) - 检测层级类型
- **ColliderBoxComponent**: [ColliderBoxComponent.cs.md](./ColliderBoxComponent.cs.md) - 碰撞触发器

---

## 注意事项

### ⚠️ 值类型

HitInfo 是 struct（值类型），复制时会创建新实例：
```csharp
HitInfo hit1 = GetHit();
HitInfo hit2 = hit1;  // 复制，不是引用

hit2.HitPos = Vector3.zero;  // 不影响 hit1
```

### ⚠️ EntityId = 0

`EntityId = 0` 表示未击中有效实体，但不代表没有击中任何东西（可能击中了场景）。

### ⚠️ Distance = 0

`Distance = 0` 通常表示射线未击中任何物体。

### ⚠️ 方向归一化

确保 `HitDir` 是归一化向量：
```csharp
hit.HitDir = direction.normalized;
```

---

*文档生成时间：2026-03-01 | OpenClaw AI 助手*
