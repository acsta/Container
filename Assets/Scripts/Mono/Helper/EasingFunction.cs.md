# EasingFunction.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | EasingFunction.cs |
| **路径** | Assets/Scripts/Mono/Helper/EasingFunction.cs |
| **所属模块** | Mono/Helper |
| **文件职责** | 缓动函数库，提供 30+ 种动画曲线 (基于 easings.net) |

---

## 类说明

### EasingFunction

| 属性 | 说明 |
|------|------|
| **职责** | 提供常用缓动函数，用于动画插值 |
| **类型** | `static class` |
| **版权** | Alan Yeats (as-is license) |

---

## 缓动类型

### Ease 枚举 (31 种)

```csharp
public enum Ease
{
    EaseInQuad, EaseOutQuad, EaseInOutQuad,      // 二次
    EaseInCubic, EaseOutCubic, EaseInOutCubic,   // 三次
    EaseInQuart, EaseOutQuart, EaseInOutQuart,   // 四次
    EaseInQuint, EaseOutQuint, EaseInOutQuint,   // 五次
    EaseInSine, EaseOutSine, EaseInOutSine,      // 正弦
    EaseInExpo, EaseOutExpo, EaseInOutExpo,      // 指数
    EaseInCirc, EaseOutCirc, EaseInOutCirc,      // 圆形
    EaseInBounce, EaseOutBounce, EaseInOutBounce,// 弹跳
    EaseInBack, EaseOutBack, EaseInOutBack,      // 回弹
    EaseInElastic, EaseOutElastic, EaseInOutElastic, // 弹性
    Linear                                        // 线性
}
```

---

## 核心方法

### GetEasingFunction

```csharp
public static Function GetEasingFunction(Ease easingFunction)
```

**职责**: 获取缓动函数委托

---

### EasingLerp

```csharp
public static float EasingLerp(EasingLerpsType type, EasingInOutType inOutType, 
    float time, float a, float b, float lerpTotal = 1.0f)
```

**职责**: 简化版缓动插值

**参数**:
- `type`: 缓动类型 (Sine/Quad/Cubic/Quint/Expo/Circ/Back/Elastic/Bounce)
- `inOutType`: EaseIn/EaseOut/EaseInOut
- `time`: 当前时间
- `a`: 起始值
- `b`: 变化量
- `lerpTotal`: 总时长

---

## 使用示例

```csharp
// 获取缓动函数
var easeFunc = EasingFunction.GetEasingFunction(Ease.EaseOutQuad);
float value = easeFunc(0.5f, 0, 100, 1.0f); // 50% 时的值

// 简化调用
float val = EasingFunction.EasingLerp(
    EasingFunction.EasingLerpsType.Quad,
    EasingFunction.EasingInOutType.EaseOut,
    0.5f, 0, 100, 1.0f);
```

---

## 相关文档

- [UIAnimator.cs.md](../Module/UIComponent/UIAnimator.cs.md) - UI 动画组件 (使用缓动)

---

*文档生成时间：2026-03-02 | OpenClaw AI 助手*
