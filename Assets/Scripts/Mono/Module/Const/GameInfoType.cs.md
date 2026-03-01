# GameInfoType.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | GameInfoType.cs |
| **路径** | Assets/Scripts/Mono/Module/Const/GameInfoType.cs |
| **所属模块** | 框架层 → Mono/Module/Const |
| **文件职责** | 定义游戏信息事件的目标类型、条件类型和可玩玩法结果枚举 |

---

## 枚举说明

### GameInfoTargetType

**用途**: 定义游戏信息事件（GameInfo）的目标对象类型

| 值 | 名称 | 说明 |
|----|------|------|
| `-1` | `Random` | 随机目标，仅用于调试 |
| `0` | `Container` | 指定集装箱物品价值增加（Ids 填集装箱 id，逗号分割） |
| `1` | `Items` | 指定物品物品价值增加（Ids 填物品 id，逗号分割） |
| `2` | `RandItems` | 指定集装箱随机物品价值增加（Ids 填集装箱 id + 随机普通物品的数量） |
| `3` | `Raise` | 抬价收益增加（Ids 不填） |
| `4` | `PlayType` | 指定玩法收益增加（Ids 填玩法 id，逗号分割） |

**使用场景**:
```csharp
// 配置游戏信息事件时，指定目标类型
GameInfoEvent evt = new GameInfoEvent
{
    TargetType = GameInfoTargetType.Container,
    TargetIds = "101,102,103",  // 集装箱 ID 列表
    ValueMultiplier = 1.5f       // 价值倍率
};
```

---

### GameInfoConditionType

**用途**: 定义游戏信息事件的触发条件类型

| 值 | 名称 | 说明 |
|----|------|------|
| `0` | `None` | 无条件，始终触发 |
| `1` | `MinRaiseCount` | 最少抬价次数条件 |
| `2` | `MaxAuctionCount` | 最高出价次数条件 |

**使用场景**:
```csharp
// 配置事件触发条件
GameInfoEvent evt = new GameInfoEvent
{
    ConditionType = GameInfoConditionType.MinRaiseCount,
    ConditionValue = 3  // 至少抬价 3 次后触发
};
```

---

### PlayableResult

**用途**: 定义可玩玩法（小游戏/互动玩法）的结果类型

| 值 | 名称 | 说明 |
|----|------|------|
| `0` | `None` | 无结果（未进行或未确定） |
| `1` | `Success` | 必成功（强制成功） |
| `2` | `Fail` | 必失败（强制失败） |

**使用场景**:
```csharp
// 调试模式：强制玩法成功
PlayableResult result = PlayableResult.Success;

// 调试模式：强制玩法失败
PlayableResult result = PlayableResult.Fail;

// 正常模式：根据实际玩法逻辑计算结果
PlayableResult result = PlayableResult.None;
```

---

## 命名空间

```csharp
namespace TaoTie
{
    // 所有枚举定义在 TaoTie 命名空间下
}
```

---

## 使用示例

### 示例 1: 配置集装箱价值增益事件

```csharp
// 创建游戏信息事件
var gameInfo = new GameInfoConfig
{
    Id = 1001,
    Name = "集装箱大丰收",
    TargetType = GameInfoTargetType.Container,
    TargetIds = "1,2,3,4,5",  // 5 个集装箱 ID
    ConditionType = GameInfoConditionType.None,
    EffectValue = 2.0f,  // 价值翻倍
    Duration = 300  // 持续 300 秒
};
```

### 示例 2: 配置抬价收益增益事件

```csharp
var gameInfo = new GameInfoConfig
{
    Id = 1002,
    Name = "拍卖热潮",
    TargetType = GameInfoTargetType.Raise,
    TargetIds = "",  // 不需要指定 ID
    ConditionType = GameInfoConditionType.MinRaiseCount,
    ConditionValue = 5,  // 抬价 5 次后触发
    EffectValue = 1.5f,  // 收益增加 50%
    Duration = 180
};
```

### 示例 3: 配置玩法收益增益事件

```csharp
var gameInfo = new GameInfoConfig
{
    Id = 1003,
    Name = "游戏时间",
    TargetType = GameInfoTargetType.PlayType,
    TargetIds = "10,11,12",  // 玩法 ID 列表
    ConditionType = GameInfoConditionType.None,
    EffectValue = 1.3f,  // 收益增加 30%
    Duration = 600
};
```

### 示例 4: 调试模式强制玩法结果

```csharp
// 调试模式下使用
#if UNITY_EDITOR
    // 测试成功流程
    var result = PlayableResult.Success;
    HandlePlayResult(result);
    
    // 测试失败流程
    result = PlayableResult.Fail;
    HandlePlayResult(result);
#endif
```

### 示例 5: 条件判断

```csharp
// 检查事件目标类型
if (gameInfo.TargetType == GameInfoTargetType.Container)
{
    // 处理集装箱逻辑
    ApplyContainerBonus(gameInfo.TargetIds);
}
else if (gameInfo.TargetType == GameInfoTargetType.Items)
{
    // 处理物品逻辑
    ApplyItemBonus(gameInfo.TargetIds);
}
else if (gameInfo.TargetType == GameInfoTargetType.Raise)
{
    // 处理抬价逻辑
    ApplyRaiseBonus();
}
```

---

## 数据结构关系

```
GameInfoConfig (配置表)
├── TargetType: GameInfoTargetType  → 确定目标对象
├── TargetIds: string               → 目标 ID 列表（逗号分割）
├── ConditionType: GameInfoConditionType → 触发条件
├── ConditionValue: int             → 条件数值
└── EffectValue: float              → 效果倍率
```

---

## 相关文档

- **配置系统**: [ConfigLoader.cs.md](../../../Code/Module/Config/ConfigLoader.cs.md) - 配置加载器
- **配置管理器**: [ConfigManager.cs.md](../../../Code/Module/Config/ConfigManager.cs.md) - 配置管理器
- **游戏信息配置**: 查看 `GameInfoConfigCategory.cs`（自动生成）

---

## 注意事项

### ⚠️ ID 格式

`TargetIds` 字段使用逗号分割的字符串格式：
```csharp
// 正确
TargetIds = "1,2,3,4,5";

// 错误（不要有空格）
TargetIds = "1, 2, 3, 4, 5";
```

### ⚠️ Random 类型

`GameInfoTargetType.Random = -1` 仅用于调试，不应在生产环境使用。

### ⚠️ PlayableResult.None

`PlayableResult.None` 表示结果未确定，需要实际计算。仅在调试或强制结果时使用 `Success`/`Fail`。

---

*文档生成时间：2026-03-01 | OpenClaw AI 助手*
