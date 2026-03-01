# ColliderBoxComponent.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | ColliderBoxComponent.cs |
| **路径** | Assets/Scripts/Mono/Module/Entity/Hit/ColliderBoxComponent.cs |
| **所属模块** | 框架层 → Mono/Module/Entity/Hit |
| **文件职责** | 碰撞体触发器组件，监听并转发碰撞进入/离开事件 |

---

## 类说明

### ColliderBoxComponent

| 属性 | 说明 |
|------|------|
| **职责** | 附加到 GameObject 上，监听 Collider 的触发事件并通过 C# 事件转发 |
| **继承关系** | `MonoBehaviour` |
| **依赖组件** | `Collider`（必需）, `Rigidbody`（必需） |

**设计模式**: 事件代理 + 组件模式

```csharp
// 使用方式
// 1. 在 GameObject 上添加 Collider 组件（如 BoxCollider）
// 2. 添加 Rigidbody 组件
// 3. 添加 ColliderBoxComponent 组件
// 4. 订阅 OnTriggerEnterEvt / OnTriggerExitEvt 事件
```

**Required Components**:
```csharp
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class ColliderBoxComponent : MonoBehaviour
```

---

## 字段与属性

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `TriggerList` | `List<Collider>` | `public` | 当前正在触发范围内的碰撞体列表 |
| `OnTriggerEnterEvt` | `Action<Collider>` | `public event` | 碰撞进入事件 |
| `OnTriggerExitEvt` | `Action<Collider>` | `public event` | 碰撞离开事件 |

---

## 方法说明

### OnTriggerEnter(Collider other)

**签名**:
```csharp
public void OnTriggerEnter(Collider other)
```

**职责**: 当其他碰撞体进入触发器时调用

**核心逻辑**:
```
1. 将碰撞体添加到 TriggerList
2. 触发 OnTriggerEnterEvt 事件
```

**调用者**: Unity 物理系统（自动调用）

**参数说明**:
| 参数 | 类型 | 说明 |
|------|------|------|
| `other` | `Collider` | 进入触发器的碰撞体 |

---

### OnTriggerExit(Collider other)

**签名**:
```csharp
public void OnTriggerExit(Collider other)
```

**职责**: 当其他碰撞体离开触发器时调用

**核心逻辑**:
```
1. 从 TriggerList 移除碰撞体
2. 触发 OnTriggerExitEvt 事件
```

**调用者**: Unity 物理系统（自动调用）

**参数说明**:
| 参数 | 类型 | 说明 |
|------|------|------|
| `other` | `Collider` | 离开触发器的碰撞体 |

---

### OnTriggerStay(Collider other)（已注释）

**签名**:
```csharp
// public void OnTriggerStay(Collider other)
```

**说明**: 当前已注释，如需使用可取消注释

**职责**: 当其他碰撞体停留在触发器内时每帧调用

---

## 使用示例

### 示例 1: 基础使用 - 监听进入/离开

```csharp
public class PlayerDetector : MonoBehaviour
{
    private ColliderBoxComponent detector;
    
    void Awake()
    {
        detector = GetComponent<ColliderBoxComponent>();
        detector.OnTriggerEnterEvt += OnPlayerEnter;
        detector.OnTriggerExitEvt += OnPlayerExit;
    }
    
    void OnPlayerEnter(Collider other)
    {
        Debug.Log($"玩家进入：{other.name}");
    }
    
    void OnPlayerExit(Collider other)
    {
        Debug.Log($"玩家离开：{other.name}");
    }
    
    void OnDestroy()
    {
        detector.OnTriggerEnterEvt -= OnPlayerEnter;
        detector.OnTriggerExitEvt -= OnPlayerExit;
    }
}
```

### 示例 2: 检测特定类型的物体

```csharp
void OnPlayerEnter(Collider other)
{
    // 检查是否是玩家
    if (other.CompareTag("Player"))
    {
        Debug.Log("玩家进入了检测范围！");
        // 触发游戏逻辑
        GameManager.Instance.OnPlayerEnterZone(gameObject);
    }
    
    // 或者检查组件
    var player = other.GetComponent<PlayerController>();
    if (player != null)
    {
        // 是玩家，执行逻辑
    }
}
```

### 示例 3: 获取当前触发列表

```csharp
// 获取当前所有在触发范围内的物体
List<Collider> currentTriggers = detector.TriggerList;

foreach (var collider in currentTriggers)
{
    Debug.Log($"当前在范围内：{collider.name}");
}

// 检查是否有特定物体
bool hasPlayer = detector.TriggerList.Any(c => c.CompareTag("Player"));
```

### 示例 4: 区域触发器 - 任务触发

```csharp
public class QuestTriggerZone : MonoBehaviour
{
    [SerializeField] private int questId;
    private ColliderBoxComponent detector;
    private bool triggered = false;
    
    void Awake()
    {
        detector = GetComponent<ColliderBoxComponent>();
        detector.OnTriggerEnterEvt += OnEnter;
    }
    
    void OnEnter(Collider other)
    {
        if (triggered) return;  // 只触发一次
        
        var player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            triggered = true;
            QuestManager.Instance.CompleteQuest(questId);
        }
    }
}
```

### 示例 5: 伤害区域

```csharp
public class DamageZone : MonoBehaviour
{
    [SerializeField] private float damagePerSecond = 10f;
    private ColliderBoxComponent detector;
    private HashSet<Collider> damagingTargets = new HashSet<Collider>();
    
    void Awake()
    {
        detector = GetComponent<ColliderBoxComponent>();
        detector.OnTriggerEnterEvt += OnEnter;
        detector.OnTriggerExitEvt += OnExit;
    }
    
    void OnEnter(Collider other)
    {
        var health = other.GetComponent<IHealth>();
        if (health != null)
        {
            damagingTargets.Add(other);
        }
    }
    
    void OnExit(Collider other)
    {
        damagingTargets.Remove(other);
    }
    
    void Update()
    {
        // 每秒对范围内所有目标造成伤害
        foreach (var target in damagingTargets)
        {
            var health = target.GetComponent<IHealth>();
            health?.TakeDamage(damagePerSecond * Time.deltaTime);
        }
    }
}
```

---

## 组件配置

### Unity 编辑器配置步骤

1. **创建 GameObject**
   - 在场景中创建空物体或使用现有物体

2. **添加 Collider**
   - 添加 `BoxCollider` / `SphereCollider` / `CapsuleCollider`
   - 勾选 `Is Trigger`（重要！）
   - 调整大小和位置

3. **添加 Rigidbody**
   - 添加 `Rigidbody` 组件
   - 勾选 `Is Kinematic`（如果不希望受物理影响）
   - 或者设置 `Use Gravity = false`

4. **添加 ColliderBoxComponent**
   - 添加 `ColliderBoxComponent` 脚本
   - 组件会自动检测依赖

---

## 技术要点

### 1. RequireComponent 特性

```csharp
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
```
- 在编辑器中自动添加依赖组件
- 防止组件缺失导致运行时错误

### 2. 触发器 vs 碰撞体

```
Collider 配置：
├── Is Trigger = true   → 触发器（穿透，调用 OnTriggerEnter）
└── Is Trigger = false  → 碰撞体（阻挡，调用 OnCollisionEnter）

ColliderBoxComponent 使用触发器模式
```

### 3. 事件订阅模式

```csharp
// 订阅
detector.OnTriggerEnterEvt += Callback;

// 取消订阅（重要！防止内存泄漏）
detector.OnTriggerEnterEvt -= Callback;
```

### 4. TriggerList 管理

```csharp
// 进入时添加
TriggerList.Add(other);

// 离开时移除
TriggerList.Remove(other);

// 查询当前状态
bool IsInside(Collider c) => TriggerList.Contains(c);
```

---

## 性能考虑

### ⚠️ TriggerList 查询

`TriggerList` 是 `List<Collider>`，频繁查询时考虑：
```csharp
// O(n) 查询
bool exists = TriggerList.Contains(collider);

// 如需频繁查询，可改用 HashSet
// 但需要修改组件实现
```

### ⚠️ 事件调用

每帧可能多次触发事件，确保回调函数高效：
```csharp
// 好的做法
void OnEnter(Collider other)
{
    // 快速判断
    if (!other.CompareTag("Player")) return;
    // 执行逻辑
}

// 避免的做法
void OnEnter(Collider other)
{
    // 复杂计算
    var result = ExpensiveCalculation();
    // ...
}
```

---

## 相关文档

- **HitBoxComponent**: [HitBoxComponent.cs.md](./HitBoxComponent.cs.md) - HitBox 类型标记
- **HitInfo**: [HitInfo.cs.md](./HitInfo.cs.md) - 击中信息结构
- **HitBoxType**: [HitBoxType.cs.md](./HitBoxType.cs.md) - HitBox 类型枚举
- **Unity 物理**: Unity 官方文档 - Collider, Rigidbody, Trigger

---

## 注意事项

### ⚠️ Rigidbody 必需

Unity 要求触发器事件必须有 Rigidbody 组件：
- 至少一个碰撞体需要有 Rigidbody
- 本组件要求自身有 Rigidbody

### ⚠️ Is Trigger 设置

确保 Collider 的 `Is Trigger` 已勾选，否则不会触发 OnTriggerEnter。

### ⚠️ 内存管理

务必在 OnDestroy 或 OnDisable 时取消事件订阅：
```csharp
void OnDestroy()
{
    detector.OnTriggerEnterEvt -= OnEnter;
    detector.OnTriggerExitEvt -= OnExit;
}
```

### ⚠️ 图层过滤

可通过 Unity 的 Layer Collision Matrix 过滤不必要的碰撞检测：
```
Edit → Project Settings → Physics → Layer Collision Matrix
```

---

*文档生成时间：2026-03-01 | OpenClaw AI 助手*
