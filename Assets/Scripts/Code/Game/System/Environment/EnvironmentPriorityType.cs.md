# EnvironmentPriorityType.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | EnvironmentPriorityType.cs |
| **路径** | Assets/Scripts/Code/Game/System/Environment/EnvironmentPriorityType.cs |
| **所属模块** | 游戏层 → System/Environment |
| **文件职责** | 环境优先级类型枚举，定义环境效果的优先级层级 |

---

## 枚举说明

### EnvironmentPriorityType

| 值 | 名称 | 说明 |
|------|------|------|
| 0 | `Default` | 默认优先级，无特殊环境效果 |
| 10 | `Scene` | 场景优先级，场景自带的环境效果 |
| 20 | `DayNight` | 昼夜优先级，昼夜循环系统的环境效果 |
| 30 | `Weather` | 天气优先级，天气系统的环境效果 |

**设计原则**: 数值越大优先级越高，高优先级效果覆盖低优先级

---

## 使用场景

### 环境效果叠加

```
优先级 30 (Weather) → 雨天气效果
    ↓ 覆盖
优先级 20 (DayNight) → 夜晚黑暗效果
    ↓ 覆盖
优先级 10 (Scene) → 场景默认光照
    ↓ 覆盖
优先级 0 (Default) → 系统默认
```

### 优先级应用

```csharp
// 场景默认光照 (优先级 10)
EnvironmentManager.SetEnvironment("DefaultLight", EnvironmentPriorityType.Scene);

// 夜晚效果 (优先级 20，覆盖场景光照)
EnvironmentManager.SetEnvironment("Night", EnvironmentPriorityType.DayNight);

// 下雨效果 (优先级 30，覆盖夜晚效果)
EnvironmentManager.SetEnvironment("Rain", EnvironmentPriorityType.Weather);

// 雨停后，恢复夜晚效果
EnvironmentManager.RemoveEnvironment("Rain");
// 自动回退到 DayNight 优先级的效果
```

---

## 设计说明

### 优先级设计

- **间隔设计**: 每级间隔 10，预留中间值用于子优先级
- **扩展性**: 可以在层级内添加子优先级（如 DayNight = 21, 22, 23）

### 可能的扩展

```csharp
// 子优先级示例
public enum EnvironmentPriorityType
{
    Default = 0,
    
    Scene = 10,
    Scene_Special = 15,      // 场景特殊事件
    
    DayNight = 20,
    DayNight_Dawn = 21,      // 黎明
    DayNight_Day = 22,       // 白天
    DayNight_Dusk = 23,      // 黄昏
    DayNight_Night = 24,     // 夜晚
    
    Weather = 30,
    Weather_Rain = 31,       // 雨天
    Weather_Snow = 32,       // 雪天
    Weather_Fog = 33,        // 雾天
}
```

---

## 使用示例

### 示例 1: 设置环境效果

```csharp
// 设置白天光照
EnvironmentManager.Instance.SetEnvironment(
    "DayLight", 
    EnvironmentPriorityType.DayNight,
    lightIntensity: 1.0f
);

// 设置下雨效果（覆盖白天光照）
EnvironmentManager.Instance.SetEnvironment(
    "Rain", 
    EnvironmentPriorityType.Weather,
    rainIntensity: 0.8f
);

// 当前生效：Rain (优先级 30)
```

### 示例 2: 移除环境效果

```csharp
// 停止下雨
EnvironmentManager.Instance.RemoveEnvironment("Rain");

// 当前生效：DayLight (优先级 20，自动恢复)
```

### 示例 3: 强制覆盖

```csharp
// 剧情需要，强制设置黑夜（最高优先级）
EnvironmentManager.Instance.SetEnvironment(
    "StoryNight", 
    EnvironmentPriorityType.Weather,  // 使用最高优先级
    lightIntensity: 0.1f
);
```

---

## 相关文档

- [EnvironmentManager.cs.md](./EnvironmentManager.cs.md) - 环境管理器
- [EnvironmentRunner.cs.md](./Runner/EnvironmentRunner.cs.md) - 环境运行器
- [DayTimeType.cs.md](./Data/DayTimeType.cs.md) - 昼夜类型

---

*文档生成时间：2026-03-02 | OpenClaw AI 助手*
