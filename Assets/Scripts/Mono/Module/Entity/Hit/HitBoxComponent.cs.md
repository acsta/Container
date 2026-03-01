# HitBoxComponent.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | HitBoxComponent.cs |
| **路径** | Assets/Scripts/Mono/Module/Entity/Hit/HitBoxComponent.cs |
| **所属模块** | 框架层 → Mono/Module/Entity/Hit |
| **文件职责** | 标记 GameObject 为击中盒（HitBox），指定 HitBox 类型 |

---

## 类说明

### HitBoxComponent

| 属性 | 说明 |
|------|------|
| **职责** | 附加到 GameObject 上，标记该物体为可被击中的 HitBox，并指定类型（普通/头部） |
| **继承关系** | `MonoBehaviour` |
| **依赖组件** | `Collider`（必需） |

**设计模式**: 标记组件

```csharp
// 使用方式
// 1. 在角色的身体部位添加 Collider
// 2. 添加 HitBoxComponent
// 3. 设置 HitBoxType（Normal 或 Head）
// 4. 自动设置 Collider 为 Trigger 模式
```

**Required Components**:
```csharp
[RequireComponent(typeof(Collider))]
public class HitBoxComponent : MonoBehaviour
```

---

## 字段与属性

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `HitBoxType` | `HitBoxType` | `public` | HitBox 类型（Normal=普通，Head=头部） |

---

## 方法说明

### Awake()

**签名**:
```csharp
public void Awake()
```

**职责**: 初始化时将 Collider 设置为 Trigger 模式

**核心逻辑**:
```
1. 获取 Collider 组件
2. 如果存在，设置 isTrigger = true
```

**调用者**: Unity 生命周期（自动调用）

---

## HitBoxType 枚举

| 值 | 名称 | 说明 |
|----|------|------|
| `0` | `Normal` | 普通 HitBox（身体、四肢等） |
| `1` | `Head` | 头部 HitBox（爆头判定） |

**详细说明**: 参见 [HitBoxType.cs.md](./HitBoxType.cs.md)

---

## 使用示例

### 示例 1: 基础配置 - 角色 HitBox

```
角色模型层级结构：
Character (GameObject)
├── Model (SkinnedMeshRenderer)
├── HitBox_Body (GameObject)
│   ├── BoxCollider
│   └── HitBoxComponent (HitBoxType = Normal)
├── HitBox_Head (GameObject)
│   ├── SphereCollider
│   └── HitBoxComponent (HitBoxType = Head)
├── HitBox_LeftArm (GameObject)
│   ├── BoxCollider
│   └── HitBoxComponent (HitBoxType = Normal)
└── HitBox_RightArm (GameObject)
    ├── BoxCollider
    └── HitBoxComponent (HitBoxType = Normal)
```

### 示例 2: 运行时获取 HitBox 类型

```csharp
// 当击中发生时
void OnHit(Collider collider)
{
    var hitBox = collider.GetComponent<HitBoxComponent>();
    if (hitBox != null)
    {
        if (hitBox.HitBoxType == HitBoxType.Head)
        {
            // 爆头！造成额外伤害
            ApplyDamage(100f);
            PlayHeadshotEffect();
        }
        else
        {
            // 普通伤害
            ApplyDamage(50f);
        }
    }
}
```

### 示例 3: 射线检测获取 HitBox 信息

```csharp
// 射击检测
RaycastHit hit;
if (Physics.Raycast(ray, out hit))
{
    var hitBox = hit.collider.GetComponent<HitBoxComponent>();
    if (hitBox != null)
    {
        // 构建击中信息
        HitInfo hitInfo = new HitInfo
        {
            EntityId = GetEntityId(hit.collider),
            HitPos = hit.point,
            HitDir = ray.direction,
            Distance = hit.distance,
            HitBoxType = hitBox.HitBoxType
        };
        
        // 处理击中
        HandleHit(hitInfo);
    }
}
```

### 示例 4: 配置多个 HitBox

```csharp
// 在角色身上配置多个 HitBox
public class CharacterHitBoxes : MonoBehaviour
{
    [Header("HitBox 配置")]
    public HitBoxComponent bodyHitBox;
    public HitBoxComponent headHitBox;
    public HitBoxComponent leftArmHitBox;
    public HitBoxComponent rightArmHitBox;
    
    // 获取所有 HitBox
    public List<HitBoxComponent> GetAllHitBoxes()
    {
        return new List<HitBoxComponent>
        {
            bodyHitBox, headHitBox, leftArmHitBox, rightArmHitBox
        };
    }
    
    // 获取头部 HitBox
    public HitBoxComponent GetHeadHitBox()
    {
        return headHitBox;
    }
}
```

### 示例 5: 动态启用/禁用 HitBox

```csharp
// 根据状态启用/禁用 HitBox
public class CharacterHealth : MonoBehaviour
{
    [SerializeField] private HitBoxComponent[] hitBoxes;
    
    // 禁用所有 HitBox（无敌状态）
    public void SetInvincible(bool invincible)
    {
        foreach (var hitBox in hitBoxes)
        {
            hitBox.GetComponent<Collider>().enabled = !invincible;
        }
    }
    
    // 仅禁用头部 HitBox（戴头盔时）
    public void SetHeadProtected(bool protected_)
    {
        var headHitBox = GetComponentInChildren<HitBoxComponent>(
            c => c.HitBoxType == HitBoxType.Head
        );
        if (headHitBox != null)
        {
            headHitBox.GetComponent<Collider>().enabled = !protected_;
        }
    }
}
```

---

## Unity 编辑器配置

### 步骤 1: 创建 HitBox GameObject

```
1. 在角色模型下创建空物体
2. 命名为 "HitBox_Body" 或 "HitBox_Head"
3. 调整位置到对应身体部位
```

### 步骤 2: 添加 Collider

```
1. 添加 BoxCollider / SphereCollider / CapsuleCollider
2. 调整大小包裹对应身体部位
3. 不要勾选 Is Trigger（HitBoxComponent 会自动设置）
```

### 步骤 3: 添加 HitBoxComponent

```
1. 添加 HitBoxComponent 脚本
2. 在 Inspector 中设置 HitBoxType
   - Normal: 身体、四肢
   - Head: 头部
```

### 步骤 4: 调整 Collider 大小

```
确保 Collider 完全包裹对应身体部位，但不要太大：
- Body: 包裹躯干
- Head: 包裹头部
- Arms: 包裹手臂
- Legs: 包裹腿部
```

---

## 技术要点

### 1. 自动设置 Trigger

```csharp
void Awake()
{
    var co = GetComponent<Collider>();
    if (co != null) co.isTrigger = true;
}
```
- 确保 HitBox 使用触发器模式
- 不会产生物理碰撞反应
- 仅用于检测击中

### 2. 类型标记

```csharp
public HitBoxType HitBoxType;
```
- 通过 Inspector 配置
- 用于区分不同部位的击中效果
- 支持爆头等特殊判定

### 3. 层级结构

```
推荐将 HitBox 作为角色的子物体：
Character
├── Model
├── HitBox_Head
├── HitBox_Body
└── HitBox_Limb
```

---

## 性能优化

### 1. HitBox 数量

根据项目需求平衡精度和性能：
```
简单模式（3-5 个 HitBox）:
- Head
- Body
- (可选) Limbs

精细模式（8-12 个 HitBox）:
- Head
- Chest
- Abdomen
- Left Arm (Upper/Lower)
- Right Arm (Upper/Lower)
- Left Leg (Upper/Lower)
- Right Leg (Upper/Lower)
```

### 2. Collider 类型选择

```
SphereCollider  → 头部（性能好）
BoxCollider     → 身体、四肢（精度高）
CapsuleCollider → 手臂、腿部（折中）
```

### 3. 层级过滤

配合 CheckHitLayerType 使用：
```csharp
// 只检测 HitBox 层级
HitInfo hit = HitSystem.Raycast(origin, dir, dist, CheckHitLayerType.OnlyHitBox);
```

---

## 相关文档

- **HitBoxType**: [HitBoxType.cs.md](./HitBoxType.cs.md) - HitBox 类型枚举
- **HitInfo**: [HitInfo.cs.md](./HitInfo.cs.md) - 击中信息结构
- **CheckHitLayerType**: [CheckHitLayerType.cs.md](./CheckHitLayerType.cs.md) - 检测层级类型
- **ColliderBoxComponent**: [ColliderBoxComponent.cs.md](./ColliderBoxComponent.cs.md) - 碰撞触发器

---

## 注意事项

### ⚠️ Collider 必需

HitBoxComponent 依赖 Collider 组件，缺少会导致功能失效。

### ⚠️ Trigger 模式

Collider 必须设置为 Trigger 模式，HitBoxComponent 会在 Awake 时自动设置。

### ⚠️ 命名规范

建议统一命名：
```
HitBox_Head
HitBox_Body
HitBox_LeftArm
HitBox_RightArm
HitBox_LeftLeg
HitBox_RightLeg
```

### ⚠️ 调试可视化

在编辑器中可开启 Gizmos 查看 HitBox 范围：
```csharp
#if UNITY_EDITOR
void OnDrawGizmos()
{
    var collider = GetComponent<Collider>();
    if (collider != null)
    {
        Gizmos.color = HitBoxType == HitBoxType.Head ? Color.red : Color.green;
        // 绘制 Collider 边界
    }
}
#endif
```

---

*文档生成时间：2026-03-01 | OpenClaw AI 助手*
