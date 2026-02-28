# ConfigEnvironment.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|------|
| **文件名** | ConfigEnvironment.cs |
| **路径** | Assets/Scripts/Code/Module/Config/Environment/ConfigEnvironment.cs |
| **所属模块** | 框架层 → Code/Module/Config/Environment |
| **文件职责** | 定义环境配置数据结构，包含光照、天空盒、颜色等环境参数 |

---

## 类/结构体说明

### ConfigEnvironment

| 属性 | 说明 |
|------|------|
| **职责** | 存储单个环境配置的完整参数，用于场景环境控制 |
| **泛型参数** | 无 |
| **继承关系** | 无 |
| **实现的接口** | 无 |

**设计模式**: 数据传输对象（DTO）

```csharp
// 创建环境配置
var env = new ConfigEnvironment
{
    Id = 1,
    TintColor = Color.blue,
    LightDir = new Vector3(50, -30, 0),
    UseDirLight = true,
    LightColor = Color.white,
    LightIntensity = 1.5f
};
```

---

## 字段与属性

### Id

| 属性 | 值 |
|------|------|
| **类型** | `int` |
| **访问级别** | `public` |
| **说明** | 环境配置的唯一标识符 |

**Nino 序列化**: `[NinoMember(1)]`

**用途**: 用于查询和引用特定环境配置

---

### Enter

| 属性 | 值 |
|------|------|
| **类型** | `ConfigBlender` |
| **访问级别** | `public` |
| **说明** | 进入此环境时的过渡参数 |

**Nino 序列化**: `[NinoMember(2)]`

**用途**: 控制从其他环境切换到此环境时的过渡效果（缓动函数 + 时间）

---

### Leave

| 属性 | 值 |
|------|------|
| **类型** | `ConfigBlender` |
| **访问级别** | `public` |
| **说明** | 离开此环境时的过渡参数 |

**Nino 序列化**: `[NinoMember(3)]`

**用途**: 控制从此环境切换到其他环境时的过渡效果

---

### TintColor

| 属性 | 值 |
|------|------|
| **类型** | `Color` |
| **访问级别** | `public` |
| **说明** | 环境色调颜色 |

**Nino 序列化**: `[NinoMember(5)]`

**用途**: 整体场景的颜色过滤/着色

---

### LightDir

| 属性 | 值 |
|------|------|
| **类型** | `Vector3` |
| **访问级别** | `public` |
| **默认值** | `new Vector3(50, -30, 0)` |
| **说明** | 光照方向（欧拉角） |

**Nino 序列化**: `[NinoMember(9)]`

**Odin Inspector**: `[LabelText("光照方向")]`

**用途**: 定义平行光的方向

---

### UseDirLight

| 属性 | 值 |
|------|------|
| **类型** | `bool` |
| **访问级别** | `public` |
| **默认值** | `false` |
| **说明** | 是否使用直接光（平行光） |

**Nino 序列化**: `[NinoMember(6)]`

**Odin Inspector**: `[LabelText("使用直接光")]`

**用途**: 开关直接光照明

---

### LightColor

| 属性 | 值 |
|------|------|
| **类型** | `Color` |
| **访问级别** | `public` |
| **默认值** | `Color.white` |
| **说明** | 直接光颜色 |

**Nino 序列化**: `[NinoMember(7)]`

**Odin Inspector**: `[LabelText("光照颜色")]`

**条件显示**: `[ShowIf(nameof(UseDirLight))]`

**用途**: 定义直接光的颜色

---

### LightIntensity

| 属性 | 值 |
|------|------|
| **类型** | `float` |
| **访问级别** | `public` |
| **默认值** | `1` |
| **说明** | 直接光强度 |

**Nino 序列化**: `[NinoMember(8)]`

**Odin Inspector**: `[LabelText("光照强度")][MinValue(0)]`

**条件显示**: `[ShowIf(nameof(UseDirLight))]`

**约束**: 最小值为 0

---

### LightShadows

| 属性 | 值 |
|------|------|
| **类型** | `LightShadows` |
| **访问级别** | `public` |
| **默认值** | `LightShadows.None` |
| **说明** | 阴影类型 |

**Nino 序列化**: `[NinoMember(10)]`

**Odin Inspector**: `[LabelText("阴影类型")]`

**条件显示**: `[ShowIf(nameof(UseDirLight))]`

**Unity 枚举值**:
- `None` - 无阴影
- `Hard` - 硬阴影
- `Soft` - 软阴影

---

### StarSpeed

| 属性 | 值 |
|------|------|
| **类型** | `float` |
| **访问级别** | `public` |
| **默认值** | `1` |
| **说明** | 星星移动速度 |

**Nino 序列化**: `[NinoMember(11)]`

**Odin Inspector**: `[LabelText("星星移动速度")]`

**用途**: 控制天空盒星星动画速度

---

### NebulaSpeed

| 属性 | 值 |
|------|------|
| **类型** | `float` |
| **访问级别** | `public` |
| **默认值** | `1` |
| **说明** | 银河移动速度 |

**Nino 序列化**: `[NinoMember(12)]`

**Odin Inspector**: `[LabelText("银河移动速度")]`

**用途**: 控制天空盒银河动画速度

---

### Remarks

| 属性 | 值 |
|------|------|
| **类型** | `string` |
| **访问级别** | `public` |
| **说明** | 策划备注 |

**条件编译**: `#if UNITY_EDITOR`

**Odin Inspector**: `[LabelText("策划备注")]`

**用途**: 策划填写设计意图说明（仅编辑器）

---

## Nino 序列化特性

### NinoType

```csharp
[NinoType(false)]
```

**说明**: 标记为 Nino 可序列化类型。

### NinoMember

```csharp
[NinoMember(1)]   // Id
[NinoMember(2)]   // Enter
[NinoMember(3)]   // Leave
[NinoMember(5)]   // TintColor
[NinoMember(6)]   // UseDirLight
[NinoMember(7)]   // LightColor
[NinoMember(8)]   // LightIntensity
[NinoMember(9)]   // LightDir
[NinoMember(10)]  // LightShadows
[NinoMember(11)]  // StarSpeed
[NinoMember(12)]  // NebulaSpeed
```

**说明**: 显式指定成员序列化顺序。注意编号不连续（4 被跳过）。

---

## Odin Inspector 集成

### ShowIf 条件显示

```csharp
[NinoMember(7)][LabelText("光照颜色")][ShowIf(nameof(UseDirLight))]
public Color LightColor;
```

**效果**: 
- 当 `UseDirLight == true` 时显示
- 当 `UseDirLight == false` 时隐藏

**用途**: 简化配置界面，只显示相关字段

### MinValue 验证

```csharp
[NinoMember(8)][LabelText("光照强度")][MinValue(0)]
public float LightIntensity;
```

**效果**: Inspector 中限制最小值为 0

**用途**: 防止配置负数强度

---

## 使用示例

### 示例 1: 白天环境

```csharp
var dayEnvironment = new ConfigEnvironment
{
    Id = 1,
    TintColor = Color.white,
    UseDirLight = true,
    LightDir = new Vector3(50, -30, 0),
    LightColor = new Color(1f, 0.95f, 0.8f),  // 暖白色
    LightIntensity = 1.5f,
    LightShadows = LightShadows.Soft,
    StarSpeed = 0f,    // 白天不显示星星
    NebulaSpeed = 0f,  // 白天不显示银河
    Enter = new ConfigBlender { DeltaTime = 2000, Ease = EasingFunction.Ease.InOutQuad },
    Leave = new ConfigBlender { DeltaTime = 1000 }
};
```

### 示例 2: 夜晚环境

```csharp
var nightEnvironment = new ConfigEnvironment
{
    Id = 2,
    TintColor = new Color(0.1f, 0.1f, 0.3f),  // 深蓝色调
    UseDirLight = false,  // 夜晚无直接光
    LightColor = Color.white,
    LightIntensity = 0.3f,
    LightShadows = LightShadows.None,
    StarSpeed = 1f,    // 星星移动
    NebulaSpeed = 0.5f // 银河移动
};
```

### 示例 3: 黄昏环境

```csharp
var sunsetEnvironment = new ConfigEnvironment
{
    Id = 3,
    TintColor = new Color(1f, 0.6f, 0.3f),  // 橙色调
    UseDirLight = true,
    LightDir = new Vector3(80, -10, 0),  // 低角度阳光
    LightColor = new Color(1f, 0.5f, 0.2f),  // 橙红色
    LightIntensity = 0.8f,
    LightShadows = LightShadows.Soft,
    StarSpeed = 0f,
    NebulaSpeed = 0f
};
```

---

## 环境过渡

### Enter/Leave Blender

```csharp
// 进入环境：2 秒缓动过渡
Enter = new ConfigBlender
{
    DeltaTime = 2000,
    Ease = EasingFunction.Ease.InOutQuad
};

// 离开环境：1 秒线性过渡
Leave = new ConfigBlender
{
    DeltaTime = 1000,
    Ease = EasingFunction.Ease.Linear
};
```

**用途**: 控制环境切换时的平滑过渡效果

---

## 相关文档

- [ConfigEnvironments.cs.md](./ConfigEnvironments.cs.md) - 环境配置列表
- [ConfigBlender.cs.md](../Blender/ConfigBlender.cs.md) - 过渡参数
- [Unity Light 文档](https://docs.unity3d.com/Manual/class-Light.html)

---

*文档生成时间：2026-02-28 | OpenClaw AI 助手*
