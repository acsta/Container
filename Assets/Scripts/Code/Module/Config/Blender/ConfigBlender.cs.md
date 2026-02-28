# ConfigBlender.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | ConfigBlender.cs |
| **路径** | Assets/Scripts/Code/Module/Config/Blender/ConfigBlender.cs |
| **所属模块** | 框架层 → Code/Module/Config/Blender |
| **文件职责** | 定义配置混合器参数，用于配置值的平滑过渡 |

---

## 类/结构体说明

### ConfigBlender

| 属性 | 说明 |
|------|------|
| **职责** | 存储配置混合/过渡的参数，包括缓动函数和过渡时间 |
| **泛型参数** | 无 |
| **继承关系** | 无 |
| **实现的接口** | 无 |

**设计模式**: 数据传输对象（DTO）

```csharp
// 用于配置值的平滑过渡
var blender = new ConfigBlender
{
    Ease = EasingFunction.Ease.InOutQuad,
    DeltaTime = 500  // 500ms 过渡
};
```

---

## 字段与属性

### Ease

| 属性 | 值 |
|------|------|
| **类型** | `EasingFunction.Ease` |
| **访问级别** | `public` |
| **默认值** | `EasingFunction.Ease.Linear` |
| **说明** | 缓动函数类型，控制过渡的动画曲线 |

**Nino 序列化**: `[NinoMember(2)]`

**可选值** (来自 `EasingFunction.Ease` 枚举):
- `Linear` - 线性过渡（匀速）
- `InQuad` / `OutQuad` / `InOutQuad` - 二次曲线
- `InCubic` / `OutCubic` / `InOutCubic` - 三次曲线
- `InQuart` / `OutQuart` / `InOutQuart` - 四次曲线
- `InQuint` / `OutQuint` / `InOutQuint` - 五次曲线
- `InSine` / `OutSine` / `InOutSine` - 正弦曲线
- `InExpo` / `OutExpo` / `InOutExpo` - 指数曲线
- `InCirc` / `OutCirc` / `InOutCirc` - 圆形曲线
- `InBack` / `OutBack` / `InOutBack` - 回弹效果
- `InElastic` / `OutElastic` / `InOutElastic` - 弹性效果
- `InBounce` / `OutBounce` / `InOutBounce` - 弹跳效果

---

### DeltaTime

| 属性 | 值 |
|------|------|
| **类型** | `int` |
| **访问级别** | `public` |
| **默认值** | `1000` |
| **说明** | 过渡时间，单位为毫秒（ms） |

**Nino 序列化**: `[NinoMember(1)]`

**Odin Inspector**: `[LabelText("过渡时间 (ms)")]`

**取值范围**:
- `0` - 瞬间完成，无过渡
- `1-100` - 快速过渡
- `100-500` - 中等速度
- `500-2000` - 慢速过渡
- `2000+` - 非常缓慢的过渡

---

## Nino 序列化特性

### NinoType

```csharp
[NinoType(false)]
```

**说明**: 标记为 Nino 可序列化类型，参数 `false` 表示不使用隐式成员模式。

### NinoMember

```csharp
[NinoMember(1)]  // DeltaTime
[NinoMember(2)]  // Ease
```

**说明**: 显式指定成员序列化顺序和标识，确保序列化数据的版本兼容性。

---

## 使用场景

### 1. 数值平滑过渡

```csharp
// 从当前值平滑过渡到目标值
var blender = new ConfigBlender
{
    Ease = EasingFunction.Ease.InOutQuad,
    DeltaTime = 500
};

float currentValue = 0;
float targetValue = 100;
float startTime = Time.time;

void Update()
{
    float elapsed = (Time.time - startTime) * 1000;  // 转换为 ms
    float t = Mathf.Min(elapsed / blender.DeltaTime, 1f);
    float easedT = EasingFunction.Ease(blender.Ease, t);
    
    currentValue = Mathf.Lerp(currentValue, targetValue, easedT);
    
    if (t >= 1f)
    {
        // 过渡完成
    }
}
```

### 2. 颜色渐变

```csharp
// UI 颜色渐变
var blender = new ConfigBlender
{
    Ease = EasingFunction.Ease.InOutSine,
    DeltaTime = 300
};

Color startColor = Color.red;
Color targetColor = Color.green;

// 使用 blender 参数控制渐变
```

### 3. 摄像机平滑移动

```csharp
// 摄像机过渡到新位置
var blender = new ConfigBlender
{
    Ease = EasingFunction.Ease.InOutCubic,
    DeltaTime = 800
};

Vector3 startPos = camera.position;
Vector3 targetPos = GetTargetPosition();

// 使用 blender 参数控制移动
```

---

## Odin Inspector 支持

### 编辑器显示

```csharp
// 在 Unity Inspector 中显示友好的标签
[LabelText("过渡时间 (ms)")]
public int DeltaTime = 1000;
```

**效果**: Inspector 中显示为 "过渡时间 (ms)" 而非 "DeltaTime"

### 配置编辑

```csharp
// 配置师可以在 Inspector 中直接调整参数
[Config]
public class CameraConfig : ProtoObject
{
    public ConfigBlender PositionBlender;  // 位置过渡参数
    public ConfigBlender RotationBlender;  // 旋转过渡参数
    public ConfigBlender FOVBlender;       // 视野过渡参数
}
```

---

## 使用示例

### 示例 1: 基础使用

```csharp
// 创建混合器配置
var blender = new ConfigBlender
{
    Ease = EasingFunction.Ease.Linear,
    DeltaTime = 1000  // 1 秒过渡
};

// 使用配置进行过渡
float value = 0;
float target = 100;
CoroutineHelper.StartCoroutine(BlendCoroutine(value, target, blender));
```

### 示例 2: 不同缓动效果对比

```csharp
// 快速响应（游戏性优先）
var quickBlender = new ConfigBlender
{
    Ease = EasingFunction.Ease.Linear,
    DeltaTime = 200
};

// 平滑优雅（表现优先）
var smoothBlender = new ConfigBlender
{
    Ease = EasingFunction.Ease.InOutSine,
    DeltaTime = 600
};

// 弹性效果（趣味性）
var bouncyBlender = new ConfigBlender
{
    Ease = EasingFunction.Ease.OutBounce,
    DeltaTime = 800
};
```

### 示例 3: 配置表中使用

```csharp
// 配置表定义（Excel/JSON）
{
    "Id": 1,
    "Name": "主摄像机",
    "PositionBlender": {
        "DeltaTime": 500,
        "Ease": "InOutQuad"
    },
    "RotationBlender": {
        "DeltaTime": 300,
        "Ease": "Linear"
    }
}

// 加载配置后使用
var config = ConfigManager.Instance.GetConfig<CameraConfig>(1);
await SmoothMoveTo(config.TargetPosition, config.PositionBlender);
```

---

## 相关文档

- [EasingFunction.cs.md](../../Helper/EasingFunction.cs.md) - 缓动函数实现
- [Nino 序列化文档](https://github.com/ninochan/Nino) - Nino 序列化库
- [Odin Inspector 文档](https://odininspector.com/) - Odin Inspector 特性

---

*文档生成时间：2026-02-28 | OpenClaw AI 助手*
