# Skybox.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | Skybox.cs |
| **路径** | Assets/Scripts/Mono/Module/Skybox/Skybox.cs |
| **所属模块** | 框架层 → Mono/Module/Skybox |
| **文件职责** | 天空盒昼夜循环系统，控制天空纹理切换、时间流逝和着色器参数更新 |

---

## 类说明

### SkyboxMono

| 属性 | 说明 |
|------|------|
| **职责** | 管理天空盒的昼夜循环效果，包括时间推进、纹理混合、着色器参数传递 |
| **继承关系** | `MonoBehaviour` |
| **执行模式** | 运行时执行（默认） |

**设计模式**: 状态驱动 + 着色器全局参数

```csharp
// 组件使用方式
// 挂载到场景中的 GameObject 上，配置纹理和时间参数
// 自动通过 Shader.SetGlobalXXX 更新全局着色器参数
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `m_currTime` | `float` | `public` | 当前时间角度（0-360 度），核心状态变量 |
| `m_dayCycleProgress` | `float` | `public` | 昼夜循环进度（0-1），用于 UI 或逻辑判断 |
| `m_dayCycleSpeed` | `float` | `public` | 时间流逝速度倍率（1 为正常速度） |
| `m_dayLength` | `float` | `public` | 白天时长（角度制，默认 180） |
| `m_nightLength` | `float` | `public` | 夜晚时长（角度制，默认 180） |
| `m_starNebulaSpeed` | `float` | `public` | 星空/星云旋转速度 |
| `m_skyDayTex` | `Texture2D` | `public` | 白天天空纹理 |
| `m_skyNightTex` | `Texture2D` | `public` | 夜晚天空纹理 |
| `m_skySunriseTex` | `Texture2D` | `public` | 日出天空纹理 |
| `m_skySunsetTex` | `Texture2D` | `public` | 日落天空纹理 |
| `m_Timer` | `GameObject` | `public` | 可视化时间指示器（可选） |

---

## 方法说明

### Start()

**签名**:
```csharp
void Start()
```

**职责**: 初始化天空盒系统，设置初始时间并加载天空纹理到着色器

**核心逻辑**:
```
1. 设置初始时间 m_currTime = 45（日出时刻）
2. 将四张天空纹理注册到着色器全局参数：
   - _SkyDayTex → 白天纹理
   - _SkyNightTex → 夜晚纹理
   - _SkySunriseTex → 日出纹理
   - _SkySunsetTex → 日落纹理
```

**调用者**: Unity 生命周期（场景启动时自动调用）

---

### Update()

**签名**:
```csharp
void Update()
```

**职责**: 每帧更新时间状态并同步到着色器

**核心逻辑**:
```
1. 推进时间：m_currTime += Time.deltaTime * m_dayCycleSpeed
2. 时间归一化：m_currTime %= 360（保持 0-360 范围）
3. 更新着色器全局参数 _CurrTime
4. 计算昼夜进度：m_dayCycleProgress = m_currTime / (m_dayLength + m_nightLength)
5. 旋转时间指示器（如果有）
6. 更新着色器全局参数 _DayCycleProgress
7. 传递配置参数到着色器：
   - _DayLength
   - _NightLength
   - _StarSpeed
   - _NebulaSpeed
```

**调用者**: Unity 生命周期（每帧自动调用）

**时间计算说明**:
```
时间范围：0-360 度
- 0°:   午夜
- 45°:  日出
- 90°:  正午
- 135°: 日落
- 180°: 午夜
- ...循环

默认配置（dayLength=180, nightLength=180）:
- 0-180°:   白天
- 180-360°: 夜晚
```

---

## 着色器全局参数

### 纹理参数

| 参数名 | 类型 | 说明 |
|--------|------|------|
| `_SkyDayTex` | `Texture2D` | 白天天空纹理 |
| `_SkyNightTex` | `Texture2D` | 夜晚天空纹理 |
| `_SkySunriseTex` | `Texture2D` | 日出天空纹理 |
| `_SkySunsetTex` | `Texture2D` | 日落天空纹理 |

### 浮点参数

| 参数名 | 类型 | 说明 |
|--------|------|------|
| `_CurrTime` | `float` | 当前时间角度（0-360） |
| `_DayCycleProgress` | `float` | 昼夜循环进度（0-1） |
| `_DayLength` | `float` | 白天时长（角度） |
| `_NightLength` | `float` | 夜晚时长（角度） |
| `_StarSpeed` | `float` | 星空旋转速度 |
| `_NebulaSpeed` | `float` | 星云旋转速度 |

---

## 使用示例

### 示例 1: 基础配置

```csharp
// 在 Unity 编辑器中：
// 1. 创建空 GameObject，命名为"SkyboxController"
// 2. 添加 SkyboxMono 组件
// 3. 配置四张天空纹理
// 4. 调整时间参数（可选）

// 默认配置即可运行，无需额外代码
```

### 示例 2: 运行时调整时间速度

```csharp
// 获取天空盒控制器
var skybox = FindObjectOfType<SkyboxMono>();

// 加速时间流逝（2 倍速）
skybox.m_dayCycleSpeed = 2f;

// 减慢时间流逝（0.5 倍速）
skybox.m_dayCycleSpeed = 0.5f;

// 暂停时间
skybox.m_dayCycleSpeed = 0f;
```

### 示例 3: 设置特定时间

```csharp
// 直接设置当前时间角度
var skybox = FindObjectOfType<SkyboxMono>();

// 设置为正午（90 度）
skybox.m_currTime = 90f;

// 设置为午夜（0 度或 360 度）
skybox.m_currTime = 0f;

// 设置为黄昏（135 度）
skybox.m_currTime = 135f;
```

### 示例 4: 获取当前昼夜进度

```csharp
var skybox = FindObjectOfType<SkyboxMono>();

// 获取进度（0-1）
float progress = skybox.m_dayCycleProgress;

if (progress < 0.5f)
{
    // 白天逻辑
}
else
{
    // 夜晚逻辑
}
```

### 示例 5: 配置昼夜时长

```csharp
var skybox = FindObjectOfType<SkyboxMono>();

// 长白天短夜晚（白天 270 度，夜晚 90 度）
skybox.m_dayLength = 270f;
skybox.m_nightLength = 90f;

// 长夜晚短白天（白天 90 度，夜晚 270 度）
skybox.m_dayLength = 90f;
skybox.m_nightLength = 270f;
```

---

## 技术要点

### 1. 时间角度系统

使用角度（0-360 度）而非时间（0-24 小时）表示时间：
- 便于三角函数计算
- 便于与旋转动画配合
- 便于着色器插值混合

### 2. 着色器全局参数

通过 `Shader.SetGlobalXXX` 传递参数：
- 所有材质共享同一套天空盒参数
- 无需每材质单独设置
- 性能优化（避免重复设置）

### 3. 时间归一化

```csharp
m_currTime %= 360;
```
确保时间始终在 0-360 范围内循环，避免浮点数溢出。

### 4. 进度计算

```csharp
m_dayCycleProgress = m_currTime / (m_dayLength + m_nightLength);
```
将时间角度转换为 0-1 进度值，便于 UI 显示和逻辑判断。

---

## 相关文档

- **着色器**: 查看项目中的天空盒着色器（通常在天坛/Shader 目录）
- **纹理资源**: Assets/AssetsPackage/Environment/Skybox/ 目录
- **时间系统**: [TimeInfo.cs.md](../../Timer/TimeInfo.cs.md) - 游戏时间信息服务

---

## 注意事项

### ⚠️ 编辑器模式

当前代码未启用 `[ExecuteAlways]`，如需在编辑器中实时预览效果：
```csharp
[ExecuteAlways]  // 取消注释此行
public class SkyboxMono : MonoBehaviour
```

### ⚠️ 纹理资源

确保四张天空纹理：
- 尺寸一致（建议 2048x1024 或 4096x2048）
- 格式为 RGB 或 RGBA
- 已正确导入到 Unity 项目

### ⚠️ 时间指示器

`m_Timer` 为可选的可视化组件，用于在场景中显示当前时间方向。如不需要可留空。

---

*文档生成时间：2026-03-01 | OpenClaw AI 助手*
