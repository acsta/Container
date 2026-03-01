# CheckHitLayerType.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | CheckHitLayerType.cs |
| **路径** | Assets/Scripts/Mono/Module/Entity/Hit/CheckHitLayerType.cs |
| **所属模块** | 框架层 → Mono/Module/Entity/Hit |
| **文件职责** | 定义击中检测的层级检查类型枚举 |

---

## 枚举说明

### CheckHitLayerType

**用途**: 指定射线检测/击中检测时应该检测哪些层级的对象

| 值 | 名称 | 说明 |
|----|------|------|
| `0` | `OnlyHitBox` | 仅检测 HitBox 层级（角色/物体的击中盒） |
| `1` | `OnlyHitScene` | 仅检测 Scene 层级（场景几何体、障碍物） |
| `2` | `Both` | 同时检测 HitBox 和 Scene 层级 |

**使用场景**:
```csharp
// 攻击检测 - 只检测角色 HitBox
HitInfo hit = HitSystem.Raycast(targetPos, CheckHitLayerType.OnlyHitBox);

// 射击检测 - 检测所有（角色 + 场景）
HitInfo hit = HitSystem.Raycast(targetPos, CheckHitLayerType.Both);

// 场景交互 - 只检测场景
HitInfo hit = HitSystem.Raycast(targetPos, CheckHitLayerType.OnlyHitScene);
```

---

## 层级说明

### HitBox 层级

包含：
- 角色的 HitBoxComponent 标记的碰撞体
- 可被击中的游戏对象
- 敌人、玩家、NPC 等实体

### Scene 层级

包含：
- 场景几何体（墙壁、地面、天花板）
- 静态障碍物
- 不可穿透的环境物体

---

## 使用示例

### 示例 1: 攻击判定 - 仅检测角色

```csharp
// 近战攻击 - 只检测敌人 HitBox
public void PerformMeleeAttack(Vector3 direction)
{
    HitInfo hit = HitSystem.Raycast(
        origin: transform.position,
        direction: direction,
        maxDistance: 3f,
        layerType: CheckHitLayerType.OnlyHitBox  // 只检测角色
    );
    
    if (hit.EntityId != 0)
    {
        // 击中敌人
        DamageSystem.ApplyDamage(hit.EntityId, attackDamage);
        SpawnHitEffect(hit.HitPos, hit.HitBoxType);
    }
}
```

### 示例 2: 射击判定 - 检测所有

```csharp
// 射击 - 检测角色和场景（子弹会打在墙上）
public void PerformShoot(Vector3 direction)
{
    HitInfo hit = HitSystem.Raycast(
        origin: gunMuzzle.position,
        direction: direction,
        maxDistance: 100f,
        layerType: CheckHitLayerType.Both  // 检测所有
    );
    
    if (hit.EntityId != 0)
    {
        // 击中角色
        DamageSystem.ApplyDamage(hit.EntityId, bulletDamage);
        SpawnBloodEffect(hit.HitPos);
    }
    else if (hit.Distance > 0)
    {
        // 击中场景
        SpawnBulletHoleEffect(hit.HitPos, hit.HitDir);
    }
}
```

### 示例 3: 场景交互 - 仅检测场景

```csharp
// 点击地面移动 - 只检测场景
public void OnClickMove(Vector2 screenPos)
{
    Ray ray = Camera.main.ScreenPointToRay(screenPos);
    HitInfo hit = HitSystem.Raycast(
        origin: ray.origin,
        direction: ray.direction,
        maxDistance: 100f,
        layerType: CheckHitLayerType.OnlyHitScene  // 只检测场景
    );
    
    if (hit.Distance > 0)
    {
        // 移动到点击位置
        MoveTo(hit.HitPos);
    }
}
```

### 示例 4: 头部射击判定

```csharp
// 爆头判定 - 检测 HitBox 并检查类型
public void CheckHeadshot(HitInfo hit)
{
    if (hit.HitBoxType == HitBoxType.Head)
    {
        // 爆头！造成额外伤害
        float damage = baseDamage * headshotMultiplier;
        DamageSystem.ApplyDamage(hit.EntityId, damage);
        PlayHeadshotEffect();
    }
    else
    {
        // 普通击中
        DamageSystem.ApplyDamage(hit.EntityId, baseDamage);
    }
}
```

### 示例 5: 范围检测

```csharp
// 检测范围内是否有敌人
public bool HasEnemyInRange(float radius)
{
    Collider[] colliders = Physics.OverlapSphere(
        transform.position,
        radius,
        LayerMask.GetMask("HitBox")
    );
    
    return colliders.Length > 0;
}

// 或者使用 HitSystem
public bool HasEnemyInRange(float radius)
{
    List<HitInfo> hits = HitSystem.SphereCast(
        center: transform.position,
        radius: radius,
        layerType: CheckHitLayerType.OnlyHitBox
    );
    
    return hits.Count > 0;
}
```

---

## 与 Unity LayerMask 的对应

### 建议的 Layer 配置

```
Layers 配置：
├── Layer 8: HitBox    (角色、敌人、NPC)
├── Layer 9: Scene     (场景几何体)
└── Layer 10: Effect   (特效，不检测)

LayerMask 计算：
├── OnlyHitBox  → 1 << 8
├── OnlyHitScene → 1 << 9
└── Both        → (1 << 8) | (1 << 9)
```

### 实现参考

```csharp
public static class HitLayerMask
{
    public const int HitBox = 8;
    public const int Scene = 9;
    
    public static int GetMask(CheckHitLayerType type)
    {
        switch (type)
        {
            case CheckHitLayerType.OnlyHitBox:
                return 1 << HitBox;
            case CheckHitLayerType.OnlyHitScene:
                return 1 << Scene;
            case CheckHitLayerType.Both:
                return (1 << HitBox) | (1 << Scene);
            default:
                return -1;  // 所有层级
        }
    }
}
```

---

## 性能优化

### 1. 层级过滤

使用层级过滤减少不必要的碰撞检测：
```csharp
// 好的做法 - 明确指定层级
HitInfo hit = HitSystem.Raycast(origin, dir, maxDist, CheckHitLayerType.OnlyHitBox);

// 避免的做法 - 检测所有层级
HitInfo hit = HitSystem.Raycast(origin, dir, maxDist);  // 可能检测到不需要的物体
```

### 2. 距离限制

合理设置最大检测距离：
```csharp
// 近战攻击 - 短距离
float meleeRange = 3f;

// 射击 - 长距离
float shootRange = 100f;

// 狙击 - 超长距离
float sniperRange = 500f;
```

---

## 相关文档

- **HitBoxComponent**: [HitBoxComponent.cs.md](./HitBoxComponent.cs.md) - HitBox 组件
- **HitBoxType**: [HitBoxType.cs.md](./HitBoxType.cs.md) - HitBox 类型（普通/头部）
- **HitInfo**: [HitInfo.cs.md](./HitInfo.cs.md) - 击中信息结构
- **ColliderBoxComponent**: [ColliderBoxComponent.cs.md](./ColliderBoxComponent.cs.md) - 碰撞触发器

---

## 注意事项

### ⚠️ Layer 配置

确保 Unity 项目的 Layer 设置与代码一致：
```
Edit → Project Settings → Tags and Layers
```

### ⚠️ 性能影响

`Both` 模式检测最多物体，性能开销最大。如无必要，优先使用特定层级。

### ⚠️ 射线检测频率

高频射线检测（如每帧）会影响性能，建议：
- 使用协程降低检测频率
- 仅在需要时检测
- 使用物理层过滤

---

*文档生成时间：2026-03-01 | OpenClaw AI 助手*
