# HitBoxType.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | HitBoxType.cs |
| **路径** | Assets/Scripts/Mono/Module/Entity/Hit/HitBoxType.cs |
| **所属模块** | 框架层 → Mono/Module/Entity/Hit |
| **文件职责** | 定义 HitBox（击中盒）的类型枚举 |

---

## 枚举说明

### HitBoxType

**用途**: 区分不同部位的击中盒，用于伤害倍率计算和特效播放

| 值 | 名称 | 说明 |
|----|------|------|
| `0` | `Normal` | 普通 HitBox（身体、四肢等常规部位） |
| `1` | `Head` | 头部 HitBox（用于爆头判定） |

**使用场景**:
```csharp
// 根据 HitBox 类型计算伤害
float damage = baseDamage;
if (hitInfo.HitBoxType == HitBoxType.Head)
{
    damage *= headshotMultiplier;  // 爆头伤害倍率
}

// 根据 HitBox 类型播放特效
if (hitInfo.HitBoxType == HitBoxType.Head)
{
    SpawnHeadshotEffect(hitInfo.HitPos);
}
else
{
    SpawnNormalHitEffect(hitInfo.HitPos);
}
```

---

## 伤害倍率参考

### 典型配置

| HitBox 类型 | 基础倍率 | 说明 |
|------------|---------|------|
| `Normal` | 1.0x | 标准伤害 |
| `Head` | 2.0x - 3.0x | 爆头额外伤害 |

### 武器差异化配置

```csharp
// 不同武器的爆头倍率
public class WeaponConfig
{
    // 步枪 - 2 倍爆头
    public float RifleHeadshotMultiplier = 2.0f;
    
    // 狙击枪 - 3 倍爆头
    public float SniperHeadshotMultiplier = 3.0f;
    
    // 手枪 - 1.5 倍爆头
    public float PistolHeadshotMultiplier = 1.5f;
    
    // 霰弹枪 - 2.5 倍爆头（近距离）
    public float ShotgunHeadshotMultiplier = 2.5f;
}
```

---

## 使用示例

### 示例 1: 基础伤害计算

```csharp
public float CalculateDamage(HitInfo hit, WeaponConfig weapon)
{
    float damage = weapon.BaseDamage;
    
    // 根据 HitBox 类型应用倍率
    switch (hit.HitBoxType)
    {
        case HitBoxType.Head:
            damage *= weapon.HeadshotMultiplier;
            break;
        case HitBoxType.Normal:
        default:
            damage *= 1.0f;
            break;
    }
    
    return damage;
}
```

### 示例 2: 爆头判定与反馈

```csharp
public void OnHit(HitInfo hit)
{
    if (hit.HitBoxType == HitBoxType.Head)
    {
        // 爆头！
        Debug.Log("Headshot!");
        
        // 播放爆头音效
        AudioManager.Instance.Play("Headshot");
        
        // 生成爆头特效
        SpawnEffect("BloodExplosion", hit.HitPos);
        
        // UI 提示
        UIManager.Instance.ShowHitMarker("HEADSHOT", Color.red);
        
        // 统计
        StatsManager.Instance.RecordHeadshot();
    }
    else
    {
        // 普通击中
        AudioManager.Instance.Play("HitFlesh");
        SpawnEffect("BloodSplatter", hit.HitPos);
        UIManager.Instance.ShowHitMarker("HIT", Color.white);
    }
}
```

### 示例 3: 成就系统

```csharp
public class AchievementManager : MonoBehaviour
{
    private int headshotCount = 0;
    
    public void OnEnemyKilled(HitInfo lastHit)
    {
        if (lastHit.HitBoxType == HitBoxType.Head)
        {
            headshotCount++;
            
            // 检查成就
            if (headshotCount >= 10)
                UnlockAchievement("Headshot Beginner");
            if (headshotCount >= 50)
                UnlockAchievement("Headshot Expert");
            if (headshotCount >= 100)
                UnlockAchievement("Headshot Master");
            if (headshotCount >= 500)
                UnlockAchievement("Headshot Legend");
        }
    }
}
```

### 示例 4: 技能系统

```csharp
public class SkillSystem : MonoBehaviour
{
    // 技能：爆头伤害提升
    public class HeadshotBoostSkill : Skill
    {
        public float bonusMultiplier = 0.5f;  // 额外 50% 爆头伤害
        
        public override float ModifyDamage(HitInfo hit, float baseDamage)
        {
            if (hit.HitBoxType == HitBoxType.Head)
            {
                return baseDamage * (1.0f + bonusMultiplier);
            }
            return baseDamage;
        }
    }
    
    // 技能：弱点识破（普通击中也有额外伤害）
    public class WeaknessSpotSkill : Skill
    {
        public float normalHitBonus = 0.2f;  // 普通击中 +20%
        
        public override float ModifyDamage(HitInfo hit, float baseDamage)
        {
            if (hit.HitBoxType == HitBoxType.Normal)
            {
                return baseDamage * (1.0f + normalHitBonus);
            }
            return baseDamage;
        }
    }
}
```

### 示例 5: 敌人 AI 反应

```csharp
public class EnemyAI : MonoBehaviour
{
    public void TakeDamage(HitInfo hit, float damage)
    {
        health -= damage;
        
        // 根据 HitBox 类型做出不同反应
        switch (hit.HitBoxType)
        {
            case HitBoxType.Head:
                // 头部受击 - 硬直更大
                PlayAnimation("Hit_Head");
                ApplyStun(1.5f);  // 眩晕 1.5 秒
                break;
                
            case HitBoxType.Normal:
                // 身体受击 - 普通硬直
                PlayAnimation("Hit_Body");
                ApplyStun(0.5f);  // 眩晕 0.5 秒
                break;
        }
        
        // 应用击退
        ApplyKnockback(hit.HitDir, damage * knockbackMultiplier);
        
        if (health <= 0)
        {
            Die(hit);
        }
    }
    
    void Die(HitInfo lastHit)
    {
        // 根据最后击中部位播放死亡动画
        if (lastHit.HitBoxType == HitBoxType.Head)
        {
            PlayAnimation("Death_Headshot");
        }
        else
        {
            PlayAnimation("Death_Normal");
        }
    }
}
```

---

## 扩展 HitBox 类型

如需更多 HitBox 类型，可扩展枚举：

```csharp
public enum HitBoxType
{
    Normal = 0,      // 普通（身体、四肢）
    Head = 1,        // 头部
    Chest = 2,       // 胸部（额外伤害）
    Limb = 3,        // 四肢（减少伤害）
    Critical = 4,    // 要害（极高伤害）
}
```

**伤害倍率建议**:
```csharp
Dictionary<HitBoxType, float> damageMultipliers = new Dictionary<HitBoxType, float>
{
    { HitBoxType.Normal, 1.0f },
    { HitBoxType.Head, 2.5f },
    { HitBoxType.Chest, 1.5f },
    { HitBoxType.Limb, 0.7f },
    { HitBoxType.Critical, 3.0f }
};
```

---

## 配置表示例

### GameInfo 配置

```csharp
// 配置表：HitBoxDamageConfig
public class HitBoxDamageConfig
{
    public int Id;
    public HitBoxType Type;
    public float DamageMultiplier;
    public string HitEffect;
    public string HitSound;
}

// 配置数据
[Config]
public static class HitBoxDamageConfigCategory
{
    // 普通击中
    {1, new HitBoxDamageConfig {
        Id = 1,
        Type = HitBoxType.Normal,
        DamageMultiplier = 1.0f,
        HitEffect = "BloodSplatter",
        HitSound = "HitFlesh"
    }},
    
    // 爆头
    {2, new HitBoxDamageConfig {
        Id = 2,
        Type = HitBoxType.Head,
        DamageMultiplier = 2.5f,
        HitEffect = "BloodExplosion",
        HitSound = "Headshot"
    }}
}
```

---

## 相关文档

- **HitInfo**: [HitInfo.cs.md](./HitInfo.cs.md) - 击中信息结构
- **HitBoxComponent**: [HitBoxComponent.cs.md](./HitBoxComponent.cs.md) - HitBox 组件
- **CheckHitLayerType**: [CheckHitLayerType.cs.md](./CheckHitLayerType.cs.md) - 检测层级类型

---

## 注意事项

### ⚠️ 默认值

枚举默认值为 `Normal = 0`，未设置 HitBoxType 的 HitBox 会被视为普通类型。

### ⚠️ 性能

使用 switch 或字典查找时注意性能：
```csharp
// 好的做法 - switch（编译时优化）
switch (hitType)
{
    case HitBoxType.Head: return 2.5f;
    case HitBoxType.Normal: return 1.0f;
}

// 可接受的做法 - 字典（运行时查找）
return damageMultipliers[hitType];
```

### ⚠️ 网络同步

在多人游戏中，HitBoxType 需要同步到服务器进行验证：
```csharp
// 客户端发送
network.Send(new HitMessage
{
    TargetId = targetId,
    HitPos = hitPos,
    HitBoxType = hitType  // 服务器验证
});

// 服务器验证
if (!ValidateHitBoxType(hitType, target))
{
    // 可能是作弊，拒绝伤害
    return;
}
```

---

*文档生成时间：2026-03-01 | OpenClaw AI 助手*
