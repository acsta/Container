# GameTimerManager.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | GameTimerManager.cs |
| **路径** | Assets/Scripts/Mono/Module/Timer/GameTimerManager.cs |
| **所属模块** | 框架层 → Mono/Module/Timer |
| **文件职责** | 游戏时间管理器，提供受时间缩放影响的游戏时间系统 |

---

## 类/结构体说明

### GameTimerManager

| 属性 | 说明 |
|------|------|
| **职责** | 管理游戏时间，支持时间缩放（慢动作、快进、暂停） |
| **泛型参数** | 无 |
| **继承关系** | 继承 `TimerManager` |
| **设计模式** | 单例模式 + 时间缩放 |

```csharp
// 单例实现
public static GameTimerManager Instance { get; private set; }

// 继承 TimerManager，通过 ManagerProvider 注册
ManagerProvider.RegisterManager<GameTimerManager>();
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `Instance` | `GameTimerManager` | `public static` | 单例实例，全局访问点 |
| `timeScale` | `float` | `private` | 时间缩放比例（标准值为 1） |
| `lastUpdateTime` | `long` | `private` | 上次更新时间（服务器时间） |
| `deltaTime` | `long` | `private` | 当前帧的游戏时间增量 |
| `timeNow` | `long` | `private` | 当前游戏时间 |
| `resetTimeScaleId` | `long` | `private` | 重置时间缩放的定时器 ID |

---

## 方法说明（按重要程度排序）

### Init()

**签名**:
```csharp
public override void Init()
```

**职责**: 初始化游戏时间管理器

**核心逻辑**:
```
1. 设置单例 Instance = this
2. 初始化 timeNow = 0（TODO: 从服务器或存档中取当前时间）
3. 初始化 lastUpdateTime = TimerManager.Instance.GetTimeNow()
4. 初始化定时器动作 InitAction()
```

**调用者**: ManagerProvider.RegisterManager<GameTimerManager>()

---

### Destroy()

**签名**:
```csharp
public override void Destroy()
```

**职责**: 销毁游戏时间管理器

**核心逻辑**:
```
1. 设置 Instance = null
2. 遍历所有 TimerAction，调用 Dispose()
3. 清空 childs 字典
```

**调用者**: ManagerProvider.RemoveManager<GameTimerManager>()

---

### Update()

**签名**:
```csharp
public override void Update()
```

**职责**: 每帧更新游戏时间

**核心逻辑**:
```
1. 获取服务器当前时间 serverNow
2. 计算真实时间差 changeTime = serverNow - lastUpdateTime
3. 计算游戏时间增量 deltaTime = changeTime * timeScale
4. 如果 timeScale <= 0（暂停）：
   - 更新 lastUpdateTime
   - 返回（不更新游戏时间）
5. 限制 lastUpdateTime 不超过 serverNow
6. 更新游戏时间 timeNow += deltaTime
7. 处理每帧定时器（同 TimerManager）
8. 检查并执行到期的定时器
```

**调用者**: ManagerProvider.Update()

**时间缩放效果**:
```
timeScale = 1.0  → 正常速度
timeScale = 0.5  → 慢动作（游戏时间流逝速度减半）
timeScale = 2.0  → 快进（游戏时间流逝速度加倍）
timeScale = 0.0  → 暂停（游戏时间停止）
```

---

### SetTimeScale(float scale, int during)

**签名**:
```csharp
public void SetTimeScale(float scale, int during = -1)
```

**职责**: 设置时间缩放比例

**核心逻辑**:
```
1. 如果 scale < 0 → 返回（无效值）
2. 设置 timeScale = scale
3. 移除之前的重置定时器
4. 如果 during >= 0：
   - 创建一次性定时器，在 during 毫秒后重置时间缩放为 1
5. 广播时间缩放变化消息 MessageId.TimeScaleChange
```

**调用者**: 需要控制时间缩放的代码

**使用示例**:
```csharp
// 设置慢动作
GameTimerManager.Instance.SetTimeScale(0.5f);

// 设置 5 秒的慢动作，之后自动恢复
GameTimerManager.Instance.SetTimeScale(0.5f, during: 5000);

// 暂停游戏
GameTimerManager.Instance.SetTimeScale(0f);

// 恢复游戏
GameTimerManager.Instance.SetTimeScale(1f);
```

---

### GetTimeScale()

**签名**:
```csharp
public float GetTimeScale()
```

**职责**: 获取当前时间缩放比例

**返回值**: 当前 timeScale 值

**使用示例**:
```csharp
float scale = GameTimerManager.Instance.GetTimeScale();
if (scale == 0f)
{
    // 游戏已暂停
}
```

---

### GetTimeNow()

**签名**:
```csharp
public override long GetTimeNow()
```

**职责**: 获取当前游戏时间（受时间缩放影响）

**核心逻辑**:
```
1. 返回 timeNow
```

**调用者**: 需要获取游戏时间的代码

**与 TimerManager 的区别**:
- `TimerManager.GetTimeNow()` → 返回服务器时间（真实时间）
- `GameTimerManager.GetTimeNow()` → 返回游戏时间（受缩放影响）

---

### GetDeltaTime()

**签名**:
```csharp
public long GetDeltaTime()
```

**职责**: 获取当前帧的游戏时间增量

**返回值**: deltaTime（毫秒）

**使用示例**:
```csharp
long deltaTime = GameTimerManager.Instance.GetDeltaTime();
// 可用于帧独立的运动计算
```

---

## 定时器类

### ResetTimeScale

**定义**:
```csharp
[Timer(TimerType.ResetTimeScale)]
public class ResetTimeScale : ATimer<GameTimerManager>
{
    public override void Run(GameTimerManager t)
    {
        try
        {
            t.SetTimeScale(1);
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
    }
}
```

**职责**: 重置时间缩放为 1（正常速度）

**触发条件**: `SetTimeScale(scale, during)` 的 during 时间到期时

---

## 时间缩放机制

### 时间流逝计算

```
真实时间差：changeTime = serverNow - lastUpdateTime
游戏时间差：deltaTime = changeTime * timeScale
新游戏时间：timeNow += deltaTime
```

### 时间缩放效果示意

```
真实时间 ───────────────────────────────────►
         │
         │ timeScale = 1.0
         ▼
游戏时间 ───────────────────────────────────►

真实时间 ───────────────────────────────────►
         │
         │ timeScale = 0.5
         ▼
游戏时间 ───────────────►（慢一半）

真实时间 ───────────────────────────────────►
         │
         │ timeScale = 2.0
         ▼
游戏时间 ───────────────────────────────────────────────►（快一倍）

真实时间 ───────────────────────────────────►
         │
         │ timeScale = 0.0
         ▼
游戏时间 ──►（暂停）
```

---

## 阅读指引

### 建议的阅读顺序

1. **理解游戏时间作用** - 为什么需要独立的游戏时间
2. **看字段定义** - 了解 timeScale/timeNow 等核心字段
3. **重点看 Update** - 理解游戏时间计算方式
4. **深入 SetTimeScale** - 理解时间缩放控制
5. **了解与 TimerManager 的区别** - 理解继承关系

### 最值得学习的技术点

1. **时间缩放**: 通过 timeScale 控制游戏时间流逝速度
2. **继承扩展**: 继承 TimerManager，重写 GetTimeNow() 和 Update()
3. **自动恢复**: 使用定时器实现时间缩放的自动恢复
4. **暂停支持**: timeScale = 0 实现游戏暂停
5. **消息广播**: 通过 Messager 广播时间缩放变化

---

## 使用示例

### 示例 1: 慢动作效果

```csharp
// 子弹时间效果
public void ActivateBulletTime()
{
    GameTimerManager.Instance.SetTimeScale(0.3f);
    
    // 播放慢动作音效
    AudioManager.Instance.PlaySlowMotion();
    
    // 5 秒后自动恢复
    GameTimerManager.Instance.SetTimeScale(0.3f, during: 5000);
}
```

### 示例 2: 暂停游戏

```csharp
// 暂停游戏
public void PauseGame()
{
    GameTimerManager.Instance.SetTimeScale(0f);
    UIManager.Instance.OpenWindow<UIPauseView>();
}

// 恢复游戏
public void ResumeGame()
{
    GameTimerManager.Instance.SetTimeScale(1f);
    UIManager.Instance.CloseWindow<UIPauseView>();
}
```

### 示例 3: 快进功能

```csharp
// 战斗快进（2 倍速）
public void SpeedUpBattle()
{
    GameTimerManager.Instance.SetTimeScale(2f);
}

// 恢复正常速度
public void NormalSpeed()
{
    GameTimerManager.Instance.SetTimeScale(1f);
}
```

### 示例 4: 使用游戏时间

```csharp
// 使用游戏时间的定时器（受时间缩放影响）
public async ETTask SkillCooldown()
{
    // 等待 3 秒游戏时间（如果 timeScale=0.5，实际需要 6 秒真实时间）
    await GameTimerManager.Instance.WaitAsync(3000);
    
    // 技能冷却完成
    SkillReady();
}
```

### 示例 5: 帧独立运动

```csharp
// 使用游戏时间的 deltaTime 进行运动计算
void Update()
{
    long deltaTime = GameTimerManager.Instance.GetDeltaTime();
    
    // 位置更新（不受帧率影响）
    position += velocity * deltaTime;
}
```

### 示例 6: 检查时间缩放状态

```csharp
// 根据时间缩放状态调整逻辑
void Update()
{
    float scale = GameTimerManager.Instance.GetTimeScale();
    
    if (scale == 0f)
    {
        // 游戏暂停，跳过更新
        return;
    }
    
    if (scale < 1f)
    {
        // 慢动作，调整音效
        AudioManager.Instance.SetPitch(scale);
    }
    
    // 正常更新
    UpdateGameLogic();
}
```

---

## 与 TimerManager 的对比

| 特性 | TimerManager | GameTimerManager |
|------|--------------|------------------|
| 时间来源 | 服务器时间（真实时间） | 游戏时间（受缩放影响） |
| 时间缩放 | 不支持 | 支持 |
| 暂停支持 | 不支持 | 支持（timeScale=0） |
| 适用场景 | UI 动画、网络超时等 | 游戏逻辑、技能冷却等 |
| GetTimeNow() | 返回服务器时间 | 返回游戏时间 |

---

## 相关文档

- [TimerManager.cs.md](./TimerManager.cs.md) - 基础定时器管理器
- [TimeInfo.cs.md](./TimeInfo.cs.md) - 时间信息服务
- [Messager.cs.md](../Messager/Messager.cs.md) - 消息系统（用于广播时间缩放变化）

---

*文档生成时间：2026-03-01 | OpenClaw AI 助手*
