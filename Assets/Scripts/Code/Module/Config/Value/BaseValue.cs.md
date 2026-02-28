# BaseValue.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|------|
| **文件名** | BaseValue.cs |
| **路径** | Assets/Scripts/Code/Module/Config/Value/BaseValue.cs |
| **所属模块** | 框架层 → Code/Module/Config/Value |
| **文件职责** | 定义 AI 配置值类型的抽象基类，支持动态值解析 |

---

## 类/结构体说明

### BaseValue

| 属性 | 说明 |
|------|------|
| **职责** | AI 配置值的抽象基类，定义值解析接口 |
| **泛型参数** | 无 |
| **继承关系** | 无 |
| **实现的接口** | 无 |

**设计模式**: 抽象基类 + 策略模式

```csharp
// 所有配置值类型都继承 BaseValue
public class SingleValue : BaseValue
{
    public float Value;
    public override float Resolve(AIKnowledge knowledge) => Value;
}

public class FormulaValue : BaseValue
{
    public string Formula;
    public override float Resolve(AIKnowledge knowledge)
    {
        // 根据 knowledge 计算复杂公式
    }
}
```

---

## 方法说明

### Resolve

**签名**:
```csharp
public abstract float Resolve(AIKnowledge knowledge)
```

**职责**: 根据 AI 知识库解析出实际的浮点数值

**参数**:
| 参数名 | 类型 | 说明 |
|--------|------|------|
| `knowledge` | `AIKnowledge` | AI 知识库，包含当前 AI 的状态、环境信息等 |

**返回值**: `float` - 解析后的实际数值

**调用者**: AI 决策系统、数值计算模块

**被调用者**: 子类实现

---

## 设计意图

### 为什么需要抽象值类型？

1. **灵活性**: 支持固定值、公式、随机值等多种值类型
2. **动态计算**: 值可以基于 AI 状态动态计算
3. **配置友好**: 策划可以在配置表中灵活定义数值
4. **扩展性**: 新增值类型无需修改 AI 逻辑

### 值类型层次结构

```
BaseValue (抽象)
├── SingleValue          → 固定单精度浮点数
├── ZeroValue            → 固定值 0
├── Range01Value         → 0-1 范围的值
├── FormulaValue         → 公式计算的值
├── OperatorValue        → 运算符组合的值
├── RandomAuctionTime    → 随机拍卖时间
├── MinAuctionTime       → 最小拍卖时间
├── TimeSinceLastBid     → 距上次出价的时间
└── ... (其他自定义类型)
```

---

## AIKnowledge 参数

### AIKnowledge 作用

`AIKnowledge` 是 AI 的"知识库"，包含：
- AI 当前状态（血量、位置、目标等）
- 环境信息（敌人位置、障碍物等）
- 游戏上下文（时间、关卡等）

### Resolve 使用示例

```csharp
// 固定值
public class SingleValue : BaseValue
{
    public float Value;
    
    public override float Resolve(AIKnowledge knowledge)
    {
        return Value;  // 直接返回固定值
    }
}

// 基于 AI 状态的值
public class HealthPercentValue : BaseValue
{
    public override float Resolve(AIKnowledge knowledge)
    {
        // 返回当前血量百分比
        return knowledge.CurrentHP / knowledge.MaxHP;
    }
}

// 基于距离的值
public class DistanceValue : BaseValue
{
    public override float Resolve(AIKnowledge knowledge)
    {
        // 返回到目标的距离
        return Vector3.Distance(knowledge.Position, knowledge.TargetPosition);
    }
}
```

---

## Nino 序列化

### NinoType

```csharp
[NinoType(false)]
```

**说明**: 标记为 Nino 可序列化类型，支持多态序列化。

**多态序列化**: Nino 可以正确序列化/反序列化 BaseValue 的子类，保留实际类型信息。

```csharp
// 序列化
BaseValue value = new SingleValue { Value = 5.0f };
byte[] bytes = Serializer.Serialize(value);

// 反序列化（正确恢复为 SingleValue 类型）
BaseValue restored = Deserializer.Deserialize<BaseValue>(bytes);
Debug.Log(restored is SingleValue);  // True
```

---

## 使用示例

### 示例 1: 配置 AI 决策阈值

```csharp
// AI 决策树配置
[Config]
public class AIDecisionConfig : ProtoObject
{
    public string DecisionType;
    
    // 使用 BaseValue 支持灵活配置
    public BaseValue AttackThreshold;    // 攻击阈值
    public BaseValue RetreatThreshold;   // 撤退阈值
    public BaseValue SearchRadius;       // 搜索半径
}

// 配置示例（JSON）
{
  "DecisionType": "Combat",
  "AttackThreshold": {
    "$type": "SingleValue",
    "Value": 30.0  // 固定值 30
  },
  "RetreatThreshold": {
    "$type": "FormulaValue",
    "Formula": "MaxHP * 0.3"  // 最大血量的 30%
  },
  "SearchRadius": {
    "$type": "Range01Value",
    "Value": 0.5  // 0-1 范围的值
  }
}
```

### 示例 2: AI 决策中使用

```csharp
public class AIController : MonoBehaviour
{
    public AIDecisionConfig config;
    public AIKnowledge knowledge;
    
    void Update()
    {
        // 解析配置值
        float attackThreshold = config.AttackThreshold.Resolve(knowledge);
        float retreatThreshold = config.RetreatThreshold.Resolve(knowledge);
        
        // 根据阈值做决策
        if (knowledge.CurrentHP < retreatThreshold)
        {
            Flee();
        }
        else if (knowledge.EnemyDistance < attackThreshold)
        {
            Attack();
        }
    }
}
```

### 示例 3: 动态难度调整

```csharp
// 根据玩家等级动态调整 AI 行为阈值
public class DynamicDifficultyValue : BaseValue
{
    public float BaseValue;
    public float ScalingFactor;
    
    public override float Resolve(AIKnowledge knowledge)
    {
        // 根据玩家等级调整难度
        int playerLevel = knowledge.PlayerLevel;
        return BaseValue + (playerLevel * ScalingFactor);
    }
}

// 配置
var difficultyValue = new DynamicDifficultyValue
{
    BaseValue = 10.0f,
    ScalingFactor = 0.5f  // 每级增加 0.5
};

float threshold = difficultyValue.Resolve(knowledge);
// 玩家等级 1: threshold = 10.5
// 玩家等级 10: threshold = 15.0
// 玩家等级 50: threshold = 35.0
```

---

## 子类实现指南

### 实现步骤

1. 继承 `BaseValue`
2. 添加 `[NinoType(false)]` 特性
3. 实现 `Resolve` 方法
4. 添加必要的字段和属性

### 示例实现

```csharp
[NinoType(false)]
public class RandomValue : BaseValue
{
    [NinoMember(1)]
    public float Min;
    
    [NinoMember(2)]
    public float Max;
    
    public override float Resolve(AIKnowledge knowledge)
    {
        return UnityEngine.Random.Range(Min, Max);
    }
}

[NinoType(false)]
public class DistanceBasedValue : BaseValue
{
    [NinoMember(1)]
    public float CloseValue;
    
    [NinoMember(2)]
    public float FarValue;
    
    public override float Resolve(AIKnowledge knowledge)
    {
        float distance = Vector3.Distance(knowledge.Position, knowledge.TargetPosition);
        float t = Mathf.InverseLerp(0, 50, distance);  // 0-50 米映射到 0-1
        return Mathf.Lerp(CloseValue, FarValue, t);
    }
}
```

---

## 相关文档

- [SingleValue.cs.md](./SingleValue.cs.md) - 固定值实现
- [FormulaValue.cs.md](./FormulaValue.cs.md) - 公式值实现
- [Range01Value.cs.md](./Range01Value.cs.md) - 0-1 范围值实现
- [AIKnowledge.cs.md](../AI/AIKnowledge.cs.md) - AI 知识库类
- [Nino 序列化文档](https://github.com/ninochan/Nino) - Nino 序列化库

---

*文档生成时间：2026-02-28 | OpenClaw AI 助手*
