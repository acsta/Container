# CasualActionComponent.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | CasualActionComponent.cs |
| **路径** | Assets/Scripts/Code/Game/Component/View/CasualActionComponent.cs |
| **所属模块** | 游戏层 → Component/View |
| **文件职责** | 休闲动作组件，管理角色的随机闲置动画播放 |

---

## 类说明

### CasualActionComponent

| 属性 | 说明 |
|------|------|
| **职责** | 控制角色播放随机闲置动画（如伸懒腰、打哈欠等休闲动作） |
| **泛型参数** | `IComponent`, `IComponent<int,int>`, `IUpdate` |
| **继承关系** | 继承 `Component` |
| **实现的接口** | `IComponent` (无参), `IComponent<int,int>` (双参), `IUpdate` (每帧更新) |

**使用场景**: NPC 或玩家的闲置动画，增加角色生动性

---

## 字段与属性

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `animator` | `Animator` | `private` | 动画控制器（从 GameObjectHolderComponent 获取） |
| `nextPlayTime` | `long` | `private` | 下次播放动画的时间戳（毫秒） |
| `enableAutoAction` | `bool` | `private` | 是否启用自动播放 |
| `start` | `int` | `private` | 随机时间范围最小值（毫秒） |
| `end` | `int` | `private` | 随机时间范围最大值（毫秒） |

---

## 方法说明

### Init() (无参版本)

**签名**:
```csharp
public void Init()
```

**职责**: 初始化组件（默认时间范围）

**核心逻辑**:
```
1. 设置 start = 3000 (3 秒)
2. 设置 end = 5000 (5 秒)
3. 计算下次播放时间 = 当前时间 + 随机 (3000, 5000)
```

**调用者**: 组件系统创建组件时

---

### Init(int start, int end) (双参版本)

**签名**:
```csharp
public void Init(int start, int end)
```

**职责**: 初始化组件（自定义时间范围）

**参数**:
- `start`: 最小间隔时间（毫秒）
- `end`: 最大间隔时间（毫秒）

**核心逻辑**:
```
1. 设置自定义 start 和 end
2. 计算下次播放时间 = 当前时间 + 随机 (start, end)
```

**调用者**: 组件系统创建组件时（带参数）

**使用示例**:
```csharp
// 5-10 秒播放一次随机动画
var comp = entity.AddComponent<CasualActionComponent>();
comp.Init(5000, 10000);
```

---

### Destroy()

**签名**:
```csharp
public void Destroy()
```

**职责**: 销毁组件

**核心逻辑**:
```
1. 无需特殊清理（动画控制器由 GameObjectHolderComponent 管理）
```

**调用者**: 组件系统销毁组件时

---

### Update()

**签名**:
```csharp
public void Update()
```

**职责**: 每帧检查是否需要播放动画

**核心逻辑**:
```
1. 检查 enableAutoAction → 未启用则返回
2. 获取当前时间
3. 如果当前时间 > nextPlayTime:
   - 从配置表随机获取一个动作 CasualActionConfigCategory.Instance.RandomAction()
   - 调用 PlayAnim() 播放
```

**调用者**: GameTimerManager（通过 IUpdate 接口自动注册）

---

### PlayAnim()

**签名**:
```csharp
public void PlayAnim(string name, int during = -1)
```

**职责**: 播放指定动画

**参数**:
- `name`: 动画名称
- `during`: 持续时间（毫秒），-1 表示使用随机间隔

**核心逻辑**:
```
1. 调用 animator.CrossFade(name, 0.2f) 淡入播放
2. 如果 during < 0:
   - 下次播放时间 = 当前时间 + 随机 (start, end)
3. 否则:
   - 下次播放时间 = 当前时间 + during
```

**调用者**: `Update()`, 外部手动触发

**设计说明**: 
- 使用 CrossFade 实现动画平滑过渡（0.2 秒过渡时间）
- 支持自定义持续时间或自动随机间隔

---

### SetEnable()

**签名**:
```csharp
public void SetEnable(bool enable)
```

**职责**: 启用/禁用自动播放

**参数**:
- `enable`: 是否启用

**核心逻辑**:
```
1. 设置 enableAutoAction = enable
2. 如果启用:
   - 重新计算下次播放时间
```

**调用者**: 需要控制自动播放的代码

**使用示例**:
```csharp
// 进入战斗时禁用
casualAction.SetEnable(false);

// 战斗结束后启用
casualAction.SetEnable(true);
```

---

### SetWinLoss()

**签名**:
```csharp
public void SetWinLoss(int val)
```

**职责**: 设置胜负状态动画参数

**参数**:
- `val`: 胜负值（由 Animator Controller 定义含义）

**核心逻辑**:
```
1. 检查 animator 是否为空
2. 调用 animator.SetInteger("WinLoss", val)
```

**调用者**: 拍卖结束、游戏结算等场景

**Animator 参数示例**:
```
WinLoss = 0  → 平局动画
WinLoss = 1  → 胜利动画
WinLoss = -1 → 失败动画
```

---

### SetMove()

**签名**:
```csharp
public void SetMove(int val)
```

**职责**: 设置移动状态动画

**参数**:
- `val`: 移动标志值

**核心逻辑**:
```
1. 检查 animator 是否为空
2. 播放 Idle 动画（重置状态）
3. 设置 MotionFlag 参数
```

**调用者**: 角色移动控制代码

**Animator 参数示例**:
```
MotionFlag = 0 → 静止
MotionFlag = 1 → 行走
MotionFlag = 2 → 奔跑
```

---

## 使用示例

### 示例 1: 基础使用

```csharp
// 创建组件（默认 3-5 秒播放一次）
var casualAction = entity.AddComponent<CasualActionComponent>();

// 启用自动播放
casualAction.SetEnable(true);

// 角色会自动播放随机闲置动画
```

### 示例 2: 自定义时间范围

```csharp
// 创建组件（5-10 秒播放一次）
var casualAction = entity.AddComponent<CasualActionComponent>();
casualAction.Init(5000, 10000);
casualAction.SetEnable(true);
```

### 示例 3: 手动播放动画

```csharp
// 手动播放特定动画
casualAction.PlayAnim("Happy_Dance");

// 播放动画并指定持续时间
casualAction.PlayAnim("Celebrate", 3000);  // 3 秒后恢复自动播放
```

### 示例 4: 拍卖结束动画

```csharp
// 拍卖成功
casualAction.SetWinLoss(1);  // 胜利动画
casualAction.PlayAnim("Victory");

// 拍卖失败
casualAction.SetWinLoss(-1);  // 失败动画
casualAction.PlayAnim("Sad");
```

### 示例 5: 移动状态切换

```csharp
// 开始移动
casualAction.SetMove(1);  // 行走
casualAction.SetEnable(false);  // 禁用休闲动画

// 停止移动
casualAction.SetMove(0);  // 静止
casualAction.SetEnable(true);  // 启用休闲动画
```

---

## 设计说明

### 时间控制机制

```
当前时间 ─────────────────────────────→
         │              │              │
      播放 1        nextPlayTime     播放 2
                      ↓
               随机 (start, end) 后
```

### 动画优先级

```
手动播放 PlayAnim() > 自动播放 Update()

手动播放会覆盖自动播放的时间安排
```

### Animator 参数管理

组件管理以下 Animator 参数：
- `WinLoss`: 胜负状态
- `MotionFlag`: 移动状态

---

## 配置依赖

### CasualActionConfig

```csharp
// 配置表示例
{
    Id: 1,
    ActionName: "Stretch",      // 伸懒腰
    Weight: 100,                // 权重（用于随机选择）
    // ...
}

// RandomAction() 根据权重随机返回一个动作名称
```

---

## 注意事项

1. **依赖组件**: 依赖 GameObjectHolderComponent 提供 Animator
2. **性能**: Update 每帧检查，但只在到期时播放动画
3. **Animator 状态**: 确保 Animator Controller 中定义了相应参数和动画
4. **时间同步**: 使用 GameTimerManager 统一时间管理

---

## 相关文档

- [Component.cs.md](../Component.cs.md) - 组件基类
- [GameObjectHolderComponent.cs.md](./GameObjectHolderComponent.cs.md) - 游戏对象持有组件
- [CasualActionConfig.cs.md](../../../Module/Config/CasualActionConfig.cs.md) - 休闲动作配置
- [GameTimerManager.cs.md](../../../../Mono/Module/Timer/GameTimerManager.cs.md) - 游戏定时器

---

*文档生成时间：2026-03-02 | OpenClaw AI 助手*
