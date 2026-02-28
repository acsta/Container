# GameTimerManager.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|------|
| **文件名** | GameTimerManager.cs |
| **路径** | Assets/Scripts/Mono/Module/Timer/GameTimerManager.cs |
| **所属模块** | 框架层 → Mono/Module/Timer |
| **文件职责** | 游戏时间管理器，提供受时间缩放影响的定时器系统 |

---

## 类/结构体说明

### GameTimerManager

| 属性 | 说明 |
|------|------|
| **职责** | 继承 TimerManager，实现游戏时间系统，支持时间缩放（子弹时间/慢动作） |
| **泛型参数** | 无 |
| **继承关系** | 继承 `TimerManager` |
| **实现的接口** | 无 |

**设计模式**: 单例模式 + 时间缩放模式

```csharp
// 获取实例
var timer = GameTimerManager.Instance;

// 设置时间缩放（慢动作）
timer.SetTimeScale(0.5f);  // 半速

// 获取当前游戏时间
long now = timer.GetTimeNow();
```

---

## 内部类

### ResetTimeScale

| 属性 | 说明 |
|------|------|
| **职责** | 定时器回调类，用于在指定时间后重置时间缩放 |
| **继承关系** | 继承 `ATimer<GameTimerManager>` |
| **触发类型** | `TimerType.ResetTimeScale` |

**用途**: 临时时间缩放效果（如技能持续 3 秒的慢动作）

---

## 字段与属性

### Instance

| 属性 | 值 |
|------|------|
| **类型** | `GameTimerManager` |
| **访问级别** | `public static` |
| **说明** | 单例实例，全局访问点 |

---

### timeScale

| 属性 | 值 |
|------|------|
| **类型** | `float` |
| **访问级别** | `private` |
| **默认值** | `1` |
| **说明** | 时间缩放系数，标准值为 1 |

**取值范围**:
- `0`: 时间停止
- `0.5`: 半速（慢动作）
- `1`: 正常速度
- `2`: 双倍速度（快进）

---

### deltaTime

| 属性 | 值 |
|------|------|
| **类型** | `long` |
| **访问级别** | `private` |
| **说明** | 帧间隔时间（受时间缩放影响） |

---

### timeNow

| 属性 | 值 |
|------|------|
| **类型** | `long` |
| **访问级别** | `private` |
| **说明** | 当前游戏时间（毫秒） |

---

## 方法说明

### Init

**签名**:
```csharp
public override void Init()
```

**职责**: 初始化游戏时间管理器

**核心逻辑**:
```
1. 设置单例 Instance = this
2. 初始化 timeNow = 0（可从存档/服务器读取）
3. 记录最后更新时间
4. 调用 InitAction() 初始化动作
```

**调用者**: ManagerProvider.RegisterManager<GameTimerManager>()

---

### Destroy

**签名**:
```csharp
public override void Destroy()
```

**职责**: 销毁管理器，清理所有定时器

**核心逻辑**:
```
1. 设置 Instance = null
2. 遍历并销毁所有子定时器
3. 清空子定时器集合
```

**调用者**: ManagerProvider.RemoveManager<GameTimerManager>()

---

### Update

**签名**:
```csharp
public override void Update()
```

**职责**: 每帧更新游戏时间和定时器

**核心逻辑**:
```
1. 获取服务器当前时间
2. 计算时间差 changeTime
3. 应用时间缩放：deltaTime = changeTime * timeScale
4. 如果 timeScale <= 0，跳过时间累加
5. 更新 lastUpdateTime 和 timeNow
6. 处理每帧执行的定时器（everyFrameTimer）
7. 处理到期的定时器（TimeId）
8. 执行到期的 TimerAction
```

**调用者**: 游戏主循环（每帧调用）

**GC 优化**:
```csharp
// 使用 for 循环而非 foreach，减少 GC
int count = this.everyFrameTimer.Count;
for (int i = 0; i < count; ++i)
{
    long timerId = this.everyFrameTimer.Dequeue();
    // ...
}
```

---

### SetTimeScale

**签名**:
```csharp
public void SetTimeScale(float scale, int during = -1)
```

**职责**: 设置时间缩放系数

**核心逻辑**:
```
1. 检查 scale >= 0
2. 设置 timeScale = scale
3. 移除之前的重置定时器
4. 如果 during >= 0，创建一次性定时器在 during 毫秒后重置
5. 广播时间缩放变化消息
```

**参数**:
| 参数名 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| `scale` | `float` | - | 时间缩放系数（0-∞） |
| `during` | `int` | -1 | 持续时间（毫秒），-1 表示永久 |

**调用者**: 技能系统、剧情系统等

**使用示例**:
```csharp
// 慢动作 3 秒
GameTimerManager.Instance.SetTimeScale(0.5f, 3000);

// 永久双倍速度
GameTimerManager.Instance.SetTimeScale(2f);

// 时间停止
GameTimerManager.Instance.SetTimeScale(0f);
```

---

### GetTimeScale

**签名**:
```csharp
public float GetTimeScale()
```

**职责**: 获取当前时间缩放系数

**返回值**: `float` - 当前时间缩放

---

### GetTimeNow

**签名**:
```csharp
public override long GetTimeNow()
```

**职责**: 获取当前游戏时间（受时间缩放影响）

**返回值**: `long` - 当前游戏时间（毫秒）

**对比**:
- `GameTimerManager.GetTimeNow()`: 游戏时间（受缩放影响）
- `TimerManager.GetTimeNow()`: 真实时间（不受缩放影响）

---

### GetDeltaTime

**签名**:
```csharp
public long GetDeltaTime()
```

**职责**: 获取帧间隔时间（受时间缩放影响）

**返回值**: `long` - 帧间隔时间（毫秒）

**用途**: 帧相关的运动计算

**使用示例**:
```csharp
// 帧率独立的移动
float speed = 100f;  // 单位/秒
long deltaTime = GameTimerManager.Instance.GetDeltaTime();
transform.position += Vector3.forward * speed * deltaTime / 1000f;
```

---

## 时间缩放机制

### 计算公式

```
deltaTime = (serverNow - lastUpdateTime) * timeScale
timeNow += deltaTime
```

### 不同缩放系数的效果

| timeScale | 效果 | 使用场景 |
|-----------|------|----------|
| `0` | 时间停止 | 暂停菜单、技能定格 |
| `0.1-0.5` | 慢动作 | 子弹时间、精彩回放 |
| `1` | 正常速度 | 默认状态 |
| `2-5` | 快速 | 跳过剧情、挂机加速 |

### 自动重置机制

```csharp
// 设置 3 秒慢动作
SetTimeScale(0.5f, 3000);

// 内部创建定时器
resetTimeScaleId = TimerManager.Instance.NewOnceTimer(
    TimerManager.Instance.GetTimeNow() + 3000,
    TimerType.ResetTimeScale,
    this
);

// 3 秒后自动调用 ResetTimeScale.Run()
// 恢复 timeScale = 1
```

---

## 使用示例

### 示例 1: 技能慢动作

```csharp
// 终极技能：子弹时间 5 秒
public void CastUltimateSkill()
{
    // 设置 0.3 倍速度，持续 5 秒
    GameTimerManager.Instance.SetTimeScale(0.3f, 5000);
    
    // 播放技能动画
    PlaySkillAnimation();
    
    // 5 秒后自动恢复正常速度
}
```

### 示例 2: 暂停菜单

```csharp
// 打开暂停菜单
public void OpenPauseMenu()
{
    GameTimerManager.Instance.SetTimeScale(0f);
    pauseMenu.SetActive(true);
}

// 关闭暂停菜单
public void ClosePauseMenu()
{
    GameTimerManager.Instance.SetTimeScale(1f);
    pauseMenu.SetActive(false);
}
```

### 示例 3: 帧率独立运动

```csharp
void Update()
{
    long deltaTime = GameTimerManager.Instance.GetDeltaTime();
    
    // 使用游戏时间的 deltaTime，确保慢动作时运动也变慢
    transform.position += moveDirection * moveSpeed * deltaTime / 1000f;
}
```

### 示例 4: 时间缩放 UI 显示

```csharp
void Update()
{
    float timeScale = GameTimerManager.Instance.GetTimeScale();
    timeScaleText.text = $"时间流速：{timeScale:F1}x";
    
    if (timeScale < 1f)
    {
        slowMotionEffect.SetActive(true);
    }
    else
    {
        slowMotionEffect.SetActive(false);
    }
}
```

---

## 设计要点

### 为什么继承 TimerManager？

1. **复用基础功能**: 定时器管理逻辑复用
2. **扩展游戏时间**: 只添加时间缩放相关逻辑
3. **多态支持**: 可以用 TimerManager 接口访问

### 为什么需要时间缩放？

1. **游戏性**: 子弹时间、慢动作技能
2. **表现力**: 精彩瞬间回放
3. **功能性**: 暂停、快进
4. **调试**: 慢速观察游戏逻辑

### GC 优化

```csharp
// ✅ 使用 for 循环
int count = this.everyFrameTimer.Count;
for (int i = 0; i < count; ++i)
{
    long timerId = this.everyFrameTimer.Dequeue();
    // ...
}

// ❌ 避免 foreach
foreach (var timerId in this.everyFrameTimer)
{
    // 会产生 Iterator GC
}
```

---

## 相关文档

- [TimerManager.cs.md](./TimerManager.cs.md) - 基础定时器管理器
- [TimerAttribute.cs.md](./TimerAttribute.cs.md) - 定时器特性
- [ITimer.cs.md](./ITimer.cs.md) - 定时器接口
- [TimerAction.cs.md](./TimerAction.cs.md) - 定时器动作

---

*文档生成时间：2026-02-28 | OpenClaw AI 助手*
