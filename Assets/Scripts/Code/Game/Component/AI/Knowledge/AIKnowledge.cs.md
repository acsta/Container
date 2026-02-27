# AIKnowledge.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | AIKnowledge.cs |
| **路径** | Assets/Scripts/Code/Game/Component/AI/Knowledge/AIKnowledge.cs |
| **所属模块** | 玩法层 → Game/Component/AI/Knowledge |
| **文件职责** | AI 知识库，存储 AI 决策所需的所有信息 |

---

## 类/结构体说明

### AIKnowledge

| 属性 | 说明 |
|------|------|
| **职责** | 存储 AI 决策所需的知识（资金、状态、复仇等） |
| **泛型参数** | 无 |
| **继承关系** | 无 |
| **实现的接口** | `IDisposable` |

```csharp
public class AIKnowledge : IDisposable
{
    // AI 知识库
}
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `Entity` | `Entity` | `public` | AI 实体引用 |
| `DecisionArchetype` | `string` | `public` | 决策原型配置名 |
| `Config` | `AIConfig` | `public` | AI 配置（从 BidderComponent 获取） |
| `Money` | `BigNumber` | `public` | 当前剩余资金 |
| `Judge` | `BigNumber` | `public` | 对总价值的判断（受偏差影响） |
| `IsNegative` | `bool` | `public` | 是否已触发消极 |
| `BidCount` | `long` | `public` | 本关卡剩余喊价次数 |
| `DeviationMin` | `float` | `public` | 判断误差范围（最小值） |
| `DeviationMax` | `float` | `public` | 判断误差范围（最大值） |
| `LastBidTime` | `long` | `public` | 上次叫价时间 |
| `RevengeTarget` | `long` | `public` | 复仇对象 ID |
| `RevengeCount` | `int` | `public` | 剩余复仇次数 |
| `IsRaisePrice` | `bool` | `public` | 是否已激活诱导抬价 |
| `RaisePriceCount` | `int` | `public` | 剩余诱导抬价次数 |
| `IsFollow` | `bool` | `public` | 墙头草是否已触发跟风 |
| `DeterminedToHave` | `bool` | `public` | 是否志在必得（特殊盲盒） |
| `Width` | `BigNumber[]` | `public` | 随机加价权值（4 档） |
| `IsHighPriceDeterrence` | `bool` | `public` | 是否被高价震慑 |
| `IsAnger` | `bool` | `public` | 是否生气 |

---

## 方法说明（按重要程度排序）

### Init(Entity aiEntity, string config)

**签名**:
```csharp
public void Init(Entity aiEntity, string config)
```

**职责**: 初始化知识库

**参数**:
- `aiEntity`: AI 实体
- `config`: 决策原型配置名

**核心逻辑**:
```
1. 保存 Entity 和 DecisionArchetype
2. 初始化 LastBidTime = int.MinValue
3. 初始化复仇相关字段
4. 初始化诱导抬价字段
5. 设置权值数组 Width：
   - Width[0] = SidelinesWeight
   - Width[1] = LowWeight
   - Width[2] = MediumWeight
   - Width[3] = HighWeight
6. 累加权值（用于随机选择）
```

**权值累加**:
```csharp
for (int i = 1; i < Width.Length; i++)
{
    Width[i] += Width[i - 1];
}
```

**调用者**: `AIComponent.Init()`

---

### Dispose()

**签名**:
```csharp
public void Dispose()
```

**职责**: 销毁时清理资源

**核心逻辑**:
```
1. 清空所有字段
2. 回收到对象池
```

**调用者**: `AIComponent.Destroy()`

---

### Ready(float prefabDeviation)

**签名**:
```csharp
public void Ready(float prefabDeviation)
```

**职责**: 每轮开始前准备

**参数**:
- `prefabDeviation`: 预制偏差值（0 表示使用配置）

**核心逻辑**:
```
1. 设置偏差范围：
   - 如果 prefabDeviation=0，使用 Config.Deviation
   - 否则使用 prefabDeviation
2. 重置 BidCount = 9999
3. 重置 IsNegative = false
4. 随机触发消极：
   - 如果 Random < Config.Negative
   - 根据权重重型消极行为
   - 设置 BidCount
5. 重置其他状态字段
```

**消极触发**:
```csharp
if (UnityEngine.Random.Range(0f, 1f) < Config.Negative)
{
    int ran = UnityEngine.Random.Range(1, Config.TotalWight);
    for (int i = 0; i < Config.NegativeBehaviorWight.Length; i++)
    {
        if (ran < Config.NegativeBehaviorWight[i])
        {
            IsNegative = true;
            BidCount = Config.NegativeBehaviorArray[i];
            Log.Info("AI" + Entity.Id + "触发消极。消极喊价次数是" + Config.NegativeBehaviorArray[i]);
            break;
        }
    }
}
```

**调用者**: 每轮拍卖开始前

---

## 阅读指引

### 建议的阅读顺序

1. **理解知识库作用** - AI 如何"感知"世界
2. **看 Init** - 理解初始化流程
3. **看 Ready** - 理解每轮准备
4. **了解状态字段** - 理解 AI 心理状态

### 最值得学习的技术点

1. **知识封装**: 将 AI 决策所需信息集中管理
2. **权值随机**: Width 数组用于随机选择策略
3. **消极系统**: 随机触发消极行为
4. **对象池**: 使用对象池管理知识库

---

## 状态字段详解

### 资金相关

| 字段 | 说明 |
|------|------|
| `Money` | 当前剩余资金 |
| `Judge` | 对总价值的判断（受偏差影响） |
| `Width[]` | 各策略的权值（用于随机） |

### 心理状态

| 字段 | 说明 |
|------|------|
| `IsNegative` | 是否消极（减少喊价次数） |
| `IsAnger` | 是否生气（影响决策） |
| `IsHighPriceDeterrence` | 是否被高价震慑 |
| `DeterminedToHave` | 是否志在必得（特殊盲盒） |

### 行为限制

| 字段 | 说明 |
|------|------|
| `BidCount` | 剩余喊价次数（消极时减少） |
| `LastBidTime` | 上次叫价时间（控制频率） |

### 特殊行为

| 字段 | 说明 |
|------|------|
| `RevengeTarget` | 复仇对象 ID |
| `RevengeCount` | 剩余复仇次数 |
| `IsRaisePrice` | 是否诱导抬价 |
| `RaisePriceCount` | 剩余诱导抬价次数 |
| `IsFollow` | 是否跟风（墙头草） |

---

## 使用示例

### 示例 1: 初始化知识库

```csharp
// 创建 AI 实体
Entity aiEntity = EntityFactory.Create("Bidder");

// 添加 AI 组件
AIComponent ai = aiEntity.AddComponent<AIComponent>();

// 初始化（传入决策原型配置）
ai.Init("AggressiveBidder");

// 获取知识库
AIKnowledge knowledge = ai.GetKnowledge();
Log.Info($"AI 资金：{knowledge.Money}");
Log.Info($"决策原型：{knowledge.DecisionArchetype}");
```

### 示例 2: 每轮准备

```csharp
// 每轮拍卖开始前
foreach (var bidder in bidders)
{
    AIComponent ai = bidder.GetComponent<AIComponent>();
    AIKnowledge knowledge = ai.GetKnowledge();
    
    // 准备新一轮
    knowledge.Ready(prefabDeviation: 0);
    
    Log.Info($"AI{bidder.Id} 剩余喊价次数：{knowledge.BidCount}");
}
```

### 示例 3: 访问知识

```csharp
AIKnowledge knowledge = ai.GetKnowledge();

// 检查资金
if (knowledge.Money > 1000)
{
    Log.Info("AI 资金充足");
}

// 检查心理状态
if (knowledge.IsAnger)
{
    Log.Info("AI 生气了");
}

// 检查复仇
if (knowledge.RevengeTarget > 0)
{
    Log.Info($"AI 要复仇对象{knowledge.RevengeTarget}");
}
```

---

## 相关文档

- [AIComponent.cs.md](../AIComponent.cs.md) - AI 决策组件
- [AIDecisionTree.cs.md](../Decision/AIDecisionTree.cs.md) - AI 决策树
- [AIDecisionInterface.cs.md](../Decision/AIDecisionInterface.cs.md) - AI 决策条件

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
