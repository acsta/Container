# EnvironmentInfo.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | EnvironmentInfo.cs |
| **路径** | Assets/Scripts/Code/Game/System/Environment/Data/EnvironmentInfo.cs |
| **所属模块** | 玩法层 → Game/System/Environment/Data |
| **文件职责** | 环境信息数据结构，存储环境状态（光照、天空盒、雾效等） |

---

## 类/结构体说明

### EnvironmentInfo

| 属性 | 说明 |
|------|------|
| **职责** | 存储环境状态数据，支持插值过渡 |
| **泛型参数** | 无 |
| **继承关系** | 无 |
| **实现的接口** | `IDisposable` |

```csharp
public class EnvironmentInfo : IDisposable
{
    // 环境信息数据结构
}
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `isDispose` | `bool` | `private` | 是否已销毁 |
| `Changed` | `bool` | `public` | 是否已变化 |
| `IsBlender` | `bool` | `public` | 是否是过渡中 |
| `IsDayNight` | `bool` | `public` | 是否是昼夜系统 |
| `Progress` | `float` | `public` | 过渡进度（0-1） |
| `TintColor` | `Color` | `public` | 色调颜色 |
| `TintColor2` | `Color` | `public` | 目标色调颜色（过渡用） |
| `LightColor` | `Color` | `public` | 光照颜色 |
| `LightIntensity` | `float` | `public` | 光照强度 |
| `LightDir` | `Vector3` | `public` | 光照方向（欧拉角） |
| `UseDirLight` | `bool` | `public` | 是否使用方向光 |
| `LightShadows` | `LightShadows` | `public` | 阴影类型 |
| `StarSpeed` | `float` | `public` | 星星旋转速度 |
| `NebulaSpeed` | `float` | `public` | 星云旋转速度 |

---

## 方法说明（按重要程度排序）

### Create(ConfigEnvironment config)

**签名**:
```csharp
public static EnvironmentInfo Create(ConfigEnvironment config)
```

**职责**: 从配置创建环境信息

**参数**:
- `config`: 环境配置

**返回**: 新的 EnvironmentInfo 实例

**核心逻辑**:
```
1. 从对象池获取实例
2. 设置 isDispose = false
3. 从 config 复制所有字段
4. 返回实例
```

**调用者**: `NormalEnvironmentRunner` 初始化

---

### DeepClone(EnvironmentInfo other)

**签名**:
```csharp
public static EnvironmentInfo DeepClone(EnvironmentInfo other)
```

**职责**: 深拷贝环境信息

**参数**:
- `other`: 源环境信息

**返回**: 新的 EnvironmentInfo 实例

**核心逻辑**:
```
1. 从对象池获取实例
2. 设置 isDispose = false
3. 复制 Progress
4. 复制所有颜色、方向、速度字段
5. 返回实例
```

**调用者**: 需要复制环境信息的场景

---

### Lerp(EnvironmentInfo from, EnvironmentInfo to, float val)

**签名**:
```csharp
public void Lerp(EnvironmentInfo from, EnvironmentInfo to, float val)
```

**职责**: 在两个环境信息之间插值

**参数**:
- `from`: 起始环境
- `to`: 目标环境
- `val`: 插值系数（0-1）

**核心逻辑**:
```
1. 设置 Progress = val
2. 设置 TintColor = from.TintColor
3. 设置 TintColor2 = to.TintColor
4. 设置 UseDirLight = from 或 to
5. 根据 UseDirLight 情况插值光照：
   - 两者都用：Lerp 颜色和强度
   - 只用 from：使用 from 的值
   - 只用 to：使用 to 的值
6. 处理 LightDir 的 360 度跨越
7. Lerp LightDir、StarSpeed、NebulaSpeed
```

**360 度处理**:
```csharp
if (from.LightDir.x > 0 && to.LightDir.x < from.LightDir.x)
{
    to.LightDir.x += 360;
    if (from.LightDir.x > 360 && to.LightDir.x > 360)
    {
        from.LightDir.x -= 360;
        to.LightDir.x -= 360;
    }
}
LightDir = Vector3.Lerp(from.LightDir, to.LightDir, val);
```

**调用者**: `BlenderEnvironmentRunner.Update()`

---

### Lerp(ConfigEnvironment from, ConfigEnvironment to, float val)

**签名**:
```csharp
public void Lerp(ConfigEnvironment from, ConfigEnvironment to, float val)
```

**职责**: 在两个配置之间插值

**与上一个方法的区别**: 直接从 ConfigEnvironment 插值，而非 EnvironmentInfo

---

### Dispose()

**签名**:
```csharp
public void Dispose()
```

**职责**: 销毁并回收到对象池

**核心逻辑**:
```
1. 如果已销毁，返回
2. 清空所有字段
3. 设置 isDispose = true
4. 回收到对象池
```

**调用者**: `EnvironmentRunner.Dispose()`

---

## 阅读指引

### 建议的阅读顺序

1. **理解数据结构** - EnvironmentInfo 存储什么
2. **看 Create** - 理解如何创建
3. **看 Lerp** - 理解插值逻辑
4. **了解 360 度处理** - 理解方向光特殊处理

### 最值得学习的技术点

1. **对象池**: 使用对象池管理环境信息
2. **插值过渡**: Lerp 支持平滑过渡
3. **360 度处理**: 特殊处理方向光的角度跨越
4. **条件插值**: 根据 UseDirLight 条件插值

---

## 插值逻辑详解

### 光照插值

```csharp
// 两者都使用方向光
if (from.UseDirLight && to.UseDirLight)
{
    LightColor = Color.Lerp(from.LightColor, to.LightColor, val);
    LightIntensity = Mathf.Lerp(from.LightIntensity, to.LightIntensity, val);
    LightShadows = to.LightShadows;
}
// 只有 from 使用
else if (from.UseDirLight)
{
    LightColor = from.LightColor;
    LightIntensity = from.LightIntensity;
    LightShadows = from.LightShadows;
}
// 只有 to 使用
else if (to.UseDirLight)
{
    LightColor = to.LightColor;
    LightIntensity = to.LightIntensity;
    LightShadows = to.LightShadows;
}
```

### 360 度角度处理

当光照方向跨越 360 度时，需要特殊处理：

```
示例：from.x = 350°, to.x = 10°

如果不处理：Lerp(350, 10, 0.5) = 180°（错误，应该经过 0°）
处理后：to.x += 360 = 370°
       Lerp(350, 370, 0.5) = 360° = 0°（正确）
```

---

## 使用示例

### 示例 1: 创建环境信息

```csharp
// 从配置创建
ConfigEnvironment config = ConfigEnvironmentCategory.Instance.Get(1);
EnvironmentInfo info = EnvironmentInfo.Create(config);

Log.Info($"色调：{info.TintColor}");
Log.Info($"光照强度：{info.LightIntensity}");
```

### 示例 2: 深拷贝

```csharp
EnvironmentInfo original = EnvironmentInfo.Create(config);
EnvironmentInfo clone = EnvironmentInfo.DeepClone(original);

// 修改 clone 不会影响 original
clone.LightIntensity = 2.0f;
```

### 示例 3: 插值过渡

```csharp
EnvironmentInfo from = EnvironmentInfo.Create(configDay);
EnvironmentInfo to = EnvironmentInfo.Create(configNight);
EnvironmentInfo current = EnvironmentInfo.Create(configDay);

// 在 Update 中
float progress = Mathf.Clamp01(progress + Time.deltaTime / 5.0f);
current.Lerp(from, to, progress);

// 应用到场景
EnvironmentManager.Instance.ApplyEnvironmentInfo(current);
```

### 示例 4: 回收对象

```csharp
EnvironmentInfo info = EnvironmentInfo.Create(config);

// 使用完毕后回收
info.Dispose();  // 自动回收到对象池
```

---

## 相关文档

- [EnvironmentManager.cs.md](../EnvironmentManager.cs.md) - 环境管理器
- [EnvironmentRunner.cs.md](../Runner/EnvironmentRunner.cs.md) - 环境运行器
- [BlenderEnvironmentRunner.cs.md](../Runner/BlenderEnvironmentRunner.cs.md) - 过渡环境运行器

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
