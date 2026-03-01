# DayTimeType.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | DayTimeType.cs |
| **路径** | Assets/Scripts/Code/Game/System/Environment/Data/DayTimeType.cs |
| **所属模块** | 游戏层 → System/Environment/Data |
| **文件职责** | 昼夜类型枚举，定义一天中的时间段 |

---

## 枚举说明

### DayTimeType

| 值 | 名称 | 说明 |
|------|------|------|
| -1 | `None` | 无效类型，未定义时间段 |
| 0 | `Noon` | 白天（中午） |
| 1 | `Night` | 夜晚 |
| 2 | `Max` | 最大值（用于边界检查） |

**注释说明**: 
- `Morning` (上午) 和 `Afternoon` (下午) 被注释掉，当前只区分白天/夜晚
- 可根据需要启用更细粒度的时间段划分

---

## 使用场景

### 昼夜循环系统

```csharp
// 根据时间计算当前时间段
DayTimeType currentTime = GetDayTimeType(hour);

switch (currentTime)
{
    case DayTimeType.Noon:
        // 白天逻辑
        SetDayLight();
        break;
    case DayTimeType.Night:
        // 夜晚逻辑
        SetNightLight();
        break;
}
```

### 环境效果切换

```csharp
// 昼夜切换
if (dayTimeType == DayTimeType.Noon)
{
    EnvironmentManager.SetEnvironment("Day", EnvironmentPriorityType.DayNight);
}
else if (dayTimeType == DayTimeType.Night)
{
    EnvironmentManager.SetEnvironment("Night", EnvironmentPriorityType.DayNight);
}
```

---

## 扩展建议

### 启用更细粒度时间段

```csharp
public enum DayTimeType
{
    None = -1,
    Dawn = 0,        // 黎明 (5:00-7:00)
    Morning = 1,     // 上午 (7:00-11:00)
    Noon = 2,        // 中午 (11:00-13:00)
    Afternoon = 3,   // 下午 (13:00-17:00)
    Dusk = 4,        // 黄昏 (17:00-19:00)
    Night = 5,       // 夜晚 (19:00-5:00)
    Max = 6
}
```

### 时间段配置

```json
// DayTimeConfig
{
    "Dawn": {
        "StartTime": 5.0,
        "EndTime": 7.0,
        "LightColor": "#FFA500",
        "LightIntensity": 0.5
    },
    "Noon": {
        "StartTime": 11.0,
        "EndTime": 13.0,
        "LightColor": "#FFFFFF",
        "LightIntensity": 1.0
    },
    "Night": {
        "StartTime": 19.0,
        "EndTime": 5.0,
        "LightColor": "#000033",
        "LightIntensity": 0.1
    }
}
```

---

## 使用示例

### 示例 1: 根据小时获取时间段

```csharp
public DayTimeType GetDayTimeType(float hour)
{
    if (hour >= 6 && hour < 18)
        return DayTimeType.Noon;
    else
        return DayTimeType.Night;
}

// 使用
float currentHour = EnvironmentManager.Instance.GameHour;
DayTimeType timeType = GetDayTimeType(currentHour);
```

### 示例 2: 时间段切换事件

```csharp
private DayTimeType lastTimeType = DayTimeType.None;

void Update()
{
    DayTimeType currentTime = GetDayTimeType(EnvironmentManager.Instance.GameHour);
    
    if (currentTime != lastTimeType && currentTime != DayTimeType.None)
    {
        OnDayTimeChange(lastTimeType, currentTime);
        lastTimeType = currentTime;
    }
}

void OnDayTimeChange(DayTimeType oldType, DayTimeType newType)
{
    Log.Info($"时间变化：{oldType} → {newType}");
    
    // 播放过渡动画
    PlayTransition(oldType, newType);
}
```

### 示例 3: 时间段相关逻辑

```csharp
// NPC 行为根据时间段变化
void UpdateNPCBehavior()
{
    DayTimeType timeType = EnvironmentManager.Instance.DayTimeType;
    
    if (timeType == DayTimeType.Noon)
    {
        // 白天：NPC 在户外工作
        npc.SetState(NPCState.Work);
    }
    else if (timeType == DayTimeType.Night)
    {
        // 夜晚：NPC 回家休息
        npc.SetState(NPCState.Rest);
    }
}
```

---

## 设计说明

### 简化设计

当前只区分白天/夜晚，原因可能是：
1. **性能**: 减少光照切换次数
2. **游戏需求**: 游戏玩法不需要更细的时间划分
3. **美术资源**: 只制作了白天/夜晚两套环境资源

### 边界值 Max

```csharp
// 用于循环和边界检查
for (int i = 0; i < (int)DayTimeType.Max; i++)
{
    // 遍历所有有效时间段
}

// 有效性检查
if (timeType >= 0 && timeType < DayTimeType.Max)
{
    // 有效时间段
}
```

---

## 相关文档

- [EnvironmentManager.cs.md](../EnvironmentManager.cs.md) - 环境管理器
- [EnvironmentPriorityType.cs.md](../EnvironmentPriorityType.cs.md) - 环境优先级
- [DayEnvironmentRunner.cs.md](../Runner/DayEnvironmentRunner.cs.md) - 昼夜环境运行器

---

*文档生成时间：2026-03-02 | OpenClaw AI 助手*
