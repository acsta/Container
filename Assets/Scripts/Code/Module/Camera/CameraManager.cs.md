# CameraManager.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | CameraManager.cs |
| **路径** | Assets/Scripts/Code/Module/Camera/CameraManager.cs |
| **所属模块** | 框架层 → Code/Module/Camera |
| **文件职责** | 相机管理系统，提供相机切换、屏幕震动等功能 |

---

## 类/结构体说明

### CameraManager

| 属性 | 说明 |
|------|------|
| **职责** | 管理所有相机，支持多渲染管线、屏幕震动 |
| **泛型参数** | 无 |
| **继承关系** | 无继承（partial 类） |
| **实现的接口** | `IManager` |

**文件结构**:
- `CameraManager.cs` - 核心逻辑、屏幕震动
- `CameraManager.URP.cs` - URP 渲染管线支持
- `CameraManager.TaoTieRP.cs` - 自研渲染管线支持

---

## 字段与属性

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `Instance` | `CameraManager` | `public static` | 单例实例 |
| `isShake` | `bool` | `private` | 是否正在震动 |

---

## 方法说明（按重要程度排序）

### Init()

**签名**:
```csharp
public void Init()
```

**职责**: 初始化相机管理器

**核心逻辑**:
```
1. 设置单例 Instance = this
```

**调用者**: ManagerProvider.RegisterManager<CameraManager>()

---

### Destroy()

**签名**:
```csharp
public void Destroy()
```

**职责**: 销毁相机管理器

**核心逻辑**:
```
1. 设置 Instance = null
```

**调用者**: ManagerProvider.RemoveManager<CameraManager>()

---

### Shake(float force, int during, float hz)

**签名**:
```csharp
public async ETTask Shake(float force = 1, int during = 1000, float hz = 50)
```

**职责**: 屏幕震动效果

**参数**:
- `force`: 震动强度（默认 1）
- `during`: 震动持续时间（毫秒，默认 1000）
- `hz`: 震动频率（默认 50）

**返回**: `ETTask` - 异步任务

**核心逻辑**:
```
1. 检查是否正在震动（isShake）
2. 检查主相机是否存在
3. 记录开始时间和初始位置
4. 设置 isShake = true
5. 循环震动：
   - 等待 1ms
   - 计算正弦波偏移：force * sin(deltaTime * hz) / addon
   - 设置相机位置
   - 检查是否超时
6. 恢复初始位置
7. 设置 isShake = false
```

**调用者**: 游戏事件（碰撞、爆炸等）

**使用示例**:
```csharp
// 轻微震动（碰撞）
await CameraManager.Instance.Shake(force: 0.5f, during: 500);

// 中等震动（爆炸）
await CameraManager.Instance.Shake(force: 1.0f, during: 1000);

// 强烈震动（大招）
await CameraManager.Instance.Shake(force: 2.0f, during: 2000, hz: 100);
```

---

## 屏幕震动算法

### 正弦波震动

```csharp
// 震动公式
offset = force * Vector3.up * Mathf.Sin(deltaTime * hz) / addon

// 参数说明:
// force: 震动强度（振幅）
// hz: 震动频率（赫兹）
// deltaTime: 经过的时间
// addon: 衰减因子（随时间增大，震动减弱）
```

### 衰减因子

```csharp
// 衰减计算
addon = Mathf.Max(1, deltaTime * 100f / during)

// 说明:
// 震动开始时 addon = 1（最大强度）
// 震动结束时 addon = 100（最小强度）
// 震动逐渐减弱，更自然
```

---

## 阅读指引

### 建议的阅读顺序

1. **理解相机管理器作用** - 为什么需要 CameraManager
2. **看 Shake 方法** - 理解屏幕震动
3. **了解震动算法** - 理解正弦波 + 衰减

### 最值得学习的技术点

1. **异步震动**: ETTask 实现异步震动效果
2. **正弦波**: 使用 sin 函数实现周期性震动
3. **衰减效果**: addon 因子实现震动减弱
4. **防重复**: isShake 标志防止重复震动

---

## 使用示例

### 示例 1: 基础震动

```csharp
// 玩家受到伤害
void OnPlayerHit()
{
    // 轻微震动
    CameraManager.Instance.Shake(force: 0.5f, during: 300).Coroutine();
}
```

### 示例 2: 爆炸震动

```csharp
// 爆炸效果
void OnExplosion()
{
    // 强烈震动
    CameraManager.Instance.Shake(
        force: 2.0f,
        during: 1500,
        hz: 80
    ).Coroutine();
}
```

### 示例 3: 连击震动

```csharp
// 连击效果
void OnCombo(int count)
{
    // 每次连击轻微震动
    for (int i = 0; i < count; i++)
    {
        TimerManager.Instance.NewOnceTimer(
            TimerManager.Instance.GetTimeNow() + i * 200,
            TimerType.Shake,
            null
        );
    }
}

[Timer(TimerType.Shake)]
public class ShakeTimer : ATimer
{
    public override void Handle(object obj)
    {
        CameraManager.Instance.Shake(force: 0.3f, during: 200).Coroutine();
    }
}
```

---

## 相关文档

- [SceneManager.cs.md](../Scene/SceneManager.cs.md) - 场景管理
- [TimerManager.cs.md](../../Mono/Module/Timer/TimerManager.cs.md) - 定时器管理

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
