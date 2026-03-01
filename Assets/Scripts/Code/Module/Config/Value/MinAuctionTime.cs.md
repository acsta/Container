# MinAuctionTime.cs 文档

## 📄 文件信息表

| 属性 | 值 |
|------|------|
| 文件路径 | `Assets/Scripts/Code/Module/Config/Value/MinAuctionTime.cs` |
| 命名空间 | `TaoTie` |
| 类类型 | 配置值类 |
| 依赖模块 | Nino.Core, Sirenix.OdinInspector, UnityEngine |
| 继承 | `BaseValue` |
| 序列化 | NinoType |

---

## 🏗️ 类说明

**MinAuctionTime** 是一个专用的值类型，用于从配置表中获取最低出价时间。

### 核心职责

- 从 `knowledge.Config.AuctionTime` 配置数组中读取最小值
- 返回固定的最低出价时间
- 用于 AI 出价时间的下限判断

### 配置依赖

依赖 `AIKnowledge.Config.AuctionTime` 数组：
- `AuctionTime[0]`: 最小出价时间（毫秒）

---

## 📊 字段表

| 字段名 | 类型 | 访问修饰符 | 说明 |
|--------|------|------------|------|
| (无实例字段) | - | - | 无状态类 |

---

## 🔧 方法说明

### Resolve

```csharp
public override float Resolve(AIKnowledge knowledge)
```

解析值为配置的最低出价时间。

**参数:**
- `knowledge`: AI 知识对象，包含配置引用

**返回:** `AuctionTime[0]` 的值（毫秒）

**实现逻辑:**
```csharp
return knowledge.Config.AuctionTime[0];
```

---

## 🔄 Mermaid 流程图

### 值解析流程

```mermaid
flowchart TD
    A[Resolve 调用] --> B[从 knowledge 获取 Config]
    B --> C[读取 AuctionTime[0]]
    C --> D[返回最小时间]
    
    style B fill:#e1f5ff
    style C fill:#fff3e1
    style D fill:#90EE90
```

---

## 💡 使用示例

### 基础使用

```csharp
// 创建最低出价时间
var minTime = new MinAuctionTime();

// 解析值（假设配置 AuctionTime = [1000, 3000]）
float minDelay = minTime.Resolve(knowledge);  // 返回 1000
```

### 在决策树中使用

```csharp
// 比较节点：检查延迟是否大于最小时间
var compareNode = new DecisionCompareNode
{
    LeftValue = new TimeSinceLastBid(),  // 距上次出价时间
    CompareMode = CompareMode.Greater,
    RightValue = new MinAuctionTime(),  // 最小出价时间
    True = new DecisionActionNode { Tactic = AITactic.LowWeight },
    False = new DecisionActionNode { Tactic = AITactic.Sidelines }
};
```

### 在配置表中使用

```yaml
# ConfigAIDecisionTree 配置示例
Type: "ConservativeBidderAI"
Node:
  Type: DecisionCompareNode
  LeftValue:
    Type: TimeSinceLastBid  # 距上次出价时间
  CompareMode: Greater
  RightValue:
    Type: MinAuctionTime  # 最小出价时间阈值
  True:
    Type: DecisionActionNode
    Tactic: LowWeight
  False:
    Type: DecisionActionNode
    Tactic: Sidelines  # 时间未到，继续观望
```

### 与其他值类型组合

```csharp
// 最小时间 + 缓冲时间
var safeDelay = new OperatorValue
{
    Left = new MinAuctionTime(),
    Op = LogicMode.Add,
    Right = new SingleValue(200)  # 额外 200ms 缓冲
};

// 最小时间 * 安全系数
var conservativeDelay = new OperatorValue
{
    Left = new MinAuctionTime(),
    Op = LogicMode.Mul,
    Right = new SingleValue(1.2f)  # 1.2 倍安全系数
};
```

---

## 📝 配置示例

### AIKnowledge 配置

```csharp
// AIKnowledge 中的配置结构
public class AIKnowledge
{
    public ConfigAIDecisionTreeCategory Config;
    public long LastBidTime;
    // ...
}

// ConfigAIDecisionTreeCategory 中
public class ConfigAIDecisionTreeCategory
{
    public float[] AuctionTime = new float[] { 1000, 3000 };  // [min, max]
    // ...
}
```

### 配置表设置

在 Unity 编辑器中配置 `ConfigAIDecisionTreeCategory`：

```yaml
AuctionTime:
  - 1000  # 最小出价时间 (ms) - MinAuctionTime 返回此值
  - 3000  # 最大出价时间 (ms) - RandomAuctionTime 使用此范围
```

---

## 🔄 相关值类型对比

| 值类型 | 返回值 | 使用场景 |
|--------|--------|----------|
| `MinAuctionTime` | `AuctionTime[0]` | 最小时间阈值 |
| `RandomAuctionTime` | `Random(AuctionTime[0], AuctionTime[1])` | 随机延迟 |
| `TimeSinceLastBid` | `Now - LastBidTime` | 已等待时间 |

---

## ⚠️ 注意事项

### 配置依赖

- 必须确保 `knowledge.Config.AuctionTime` 已正确配置
- 数组长度必须至少为 1
- 建议值为正数（毫秒）

### 空值保护

```csharp
// 建议在使用前检查配置
if (knowledge.Config?.AuctionTime?.Length >= 1)
{
    var minTime = new MinAuctionTime();
    float minDelay = minTime.Resolve(knowledge);
}
else
{
    // 使用默认值
    float minDelay = 1000;
}
```

---

## 🔗 相关文档链接

- [BaseValue.cs.md](./BaseValue.cs.md) - 值基类
- [RandomAuctionTime.cs.md](./RandomAuctionTime.cs.md) - 随机出价时间
- [TimeSinceLastBid.cs.md](./TimeSinceLastBid.cs.md) - 距上次出价时间
- [AIKnowledge.cs.md](../../../Game/Component/AI/Knowledge/AIKnowledge.cs.md) - AI 知识类
- [DecisionCompareNode.cs.md](../DecisionTree/DecisionCompareNode.cs.md) - 比较节点

---

*最后更新：2026-03-02*
