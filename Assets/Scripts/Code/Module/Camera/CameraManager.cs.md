# CameraManager.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | CameraManager.cs |
| **路径** | Assets/Scripts/Code/Module/Camera/CameraManager.cs |
| **所属模块** | 框架层 → Code/Module/Camera |
| **文件职责** | 相机管理器，提供主相机控制和屏幕震动效果 |

---

## 类说明

### CameraManager

| 属性 | 说明 |
|------|------|
| **职责** | 管理游戏主相机，提供相机震动等效果 |
| **设计模式** | 单例模式 (通过 IManager) |
| **继承关系** | partial class, 实现 IManager |

```csharp
public partial class CameraManager : IManager
{
    public static CameraManager Instance { get; private set; }
}
```

---

## IManager 接口实现

### Init()

**签名**:
```csharp
public void Init()
```

**职责**: 初始化相机管理器

**核心逻辑**:
```
1. 设置 Instance 单例引用
```

---

### Destroy()

**签名**:
```csharp
public void Destroy()
```

**职责**: 销毁相机管理器

**核心逻辑**:
```
1. 清空 Instance 引用
```

---

## 方法说明

### Shake()

**签名**:
```csharp
public async ETTask Shake(float force = 1, int during = 1000, float hz = 50)
```

**职责**: 执行相机震动效果

**参数**:
| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `force` | float | 1 | 震动强度（振幅） |
| `during` | int | 1000 | 震动持续时间（毫秒） |
| `hz` | float | 50 | 震动频率（Hz） |

**返回**: ETTask（异步任务）

**核心逻辑**:
```
1. 检查是否正在震动（isShake 标志）
2. 检查主相机是否存在
3. 记录起始时间和起始位置
4. 进入震动循环：
   - 等待 1ms
   - 计算经过时间
   - 使用正弦函数计算偏移量
   - 应用偏移量到相机位置
   - 达到持续时间后恢复原位
5. 清除震动标志
```

**震动公式**:
```csharp
offset = force * Vector3.up * Mathf.Sin(deltaTime * hz) / addon;
// addon 用于衰减振幅
addon = Mathf.Max(1, deltaTime * 100f / during);
```

**使用示例**:
```csharp
// 默认震动
await CameraManager.Instance.Shake();

// 强烈震动（爆炸效果）
await CameraManager.Instance.Shake(force: 3, during: 500);

// 轻微震动（受击效果）
await CameraManager.Instance.Shake(force: 0.5, during: 200, hz: 30);
```

---

### MainCamera()

**签名**:
```csharp
public Camera MainCamera()
```

**职责**: 获取主相机

**返回**: 主相机实例

**说明**: 此方法在 URP 和 TaoTieRP 分部类中实现

**使用示例**:
```csharp
var mainCam = CameraManager.Instance.MainCamera();
Vector3 camPos = mainCam.transform.position;
```

---

## 字段说明

### isShake

```csharp
private bool isShake;
```

**说明**: 震动状态标志，防止重复震动

---

## 使用场景

### 场景 1: 爆炸效果

```csharp
// 爆炸发生时
void OnExplosion()
{
    // 强烈震动
    _ = CameraManager.Instance.Shake(force: 2, during: 800, hz: 60);
    
    // 播放爆炸动画
    PlayExplosionAnim();
}
```

### 场景 2: 角色受击

```csharp
// 角色受到伤害时
void OnDamage()
{
    // 轻微震动
    _ = CameraManager.Instance.Shake(force: 0.3, during: 300);
    
    // 播放受击动画
    PlayHitAnim();
}
```

### 场景 3: UI 反馈

```csharp
// 重要 UI 事件
void OnRareItemDrop()
{
    // 配合特效震动
    _ = CameraManager.Instance.Shake(force: 1, during: 500, hz: 40);
    ShowRareAnim();
}
```

### 场景 4: 技能释放

```csharp
// 强力技能释放
async Task CastUltimateSkill()
{
    // 震屏反馈
    await CameraManager.Instance.Shake(force: 1.5, during: 600);
    
    // 释放技能
    ReleaseSkill();
}
```

---

## 震动效果参数建议

| 场景 | force | during (ms) | hz | 效果 |
|------|-------|-------------|-----|------|
| 轻微受击 | 0.2-0.5 | 200-300 | 30-40 | 轻微波动 |
| 普通攻击 | 0.5-1.0 | 300-500 | 40-50 | 明显震动 |
| 爆炸效果 | 1.5-3.0 | 500-800 | 50-60 | 强烈震动 |
| 地震效果 | 1.0-2.0 | 1000-2000 | 20-30 | 持续晃动 |
| UI 反馈 | 0.3-0.8 | 200-400 | 40-50 | 短暂提示 |

---

## 分部类结构

```
CameraManager (partial)
    ├── CameraManager.cs (主文件)
    │   ├── Init/Destroy
    │   ├── Shake()
    │   └── Instance 单例
    │
    ├── CameraManager.URP.cs (URP 渲染管线)
    │   ├── SetCameraStackAtLoadingStart()
    │   ├── SetCameraStackAtLoadingDone()
    │   └── MainCamera() - URP 实现
    │
    └── CameraManager.TaoTieRP.cs (TaoTie 渲染管线)
        └── (注释掉的 URP 代码，待实现)
```

---

## 注意事项

### 1. 异步调用

Shake() 是异步方法，使用 await 或 _ 忽略返回值：

```csharp
// ✅ 正确
await CameraManager.Instance.Shake();
_ = CameraManager.Instance.Shake();

// ❌ 警告：未等待异步任务
CameraManager.Instance.Shake();
```

### 2. 防止重复震动

isShake 标志防止同时执行多个震动：

```csharp
if (isShake) return;  // 正在震动时忽略新请求
```

### 3. 主相机检查

震动前检查主相机是否存在：

```csharp
if (MainCamera() == null) return;
```

---

## 相关文档

- [CameraManager.URP.cs.md](./CameraManager.URP.cs.md) - URP 相机实现
- [CameraManager.TaoTieRP.cs.md](./CameraManager.TaoTieRP.cs.md) - TaoTieRP 相机实现
- [IManager.cs.md](../../../Mono/Core/Manager/IManager.cs.md) - 管理器接口

---

*文档生成时间：2026-03-01 | OpenClaw AI 助手*
