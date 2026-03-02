# AIDecisionInterface.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | AIDecisionInterface.cs |
| **路径** | Assets/Scripts/Code/Game/Component/AI/Decision/AIDecisionInterface.cs |
| **所属模块** | 游戏层 → AI 决策系统 |
| **文件职责** | AI 决策条件判断方法集合，提供拍卖场景中 AI 决策所需的各种状态判断 |

---

## 类/结构体说明

### AIDecisionInterface

| 属性 | 说明 |
|------|------|
| **职责** | 提供 AI 决策树中使用的各种条件判断静态方法 |
| **泛型参数** | 无 |
| **继承关系** | 无继承 |
| **实现的接口** | 无 |

**设计模式**: 静态方法集合 + 反射注册

```csharp
// 使用方式
bool isGuidance = AIDecisionInterface.IsGuidance(aiKnowledge);
bool isMoneyEnough = AIDecisionInterface.IsMoneyEnoughJudge(aiKnowledge);
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `Methods` | `Dictionary<string, Func<AIKnowledge, bool>>` | `public static` | 决策方法注册表，方法名 → 委托 |
| `MethodNames` | `Dictionary<string, string>` | `public static` | 编辑器显示名称映射（仅 Unity Editor） |

---

## 方法说明（按类别分组）

### 整场属性判断

#### IsGuidance

**签名**:
```csharp
public static bool IsGuidance(AIKnowledge aiKnowledge)
```

**职责**: 判断当前是否为新手引导场次

**核心逻辑**:
```
检查 IAuctionManager.Instance 是否为 AuctionGuideManager 类型
```

**使用场景**: AI 决策树中判断是否需要执行引导逻辑

---

#### IsDiceType3 / IsDiceType5

**签名**:
```csharp
public static bool IsDiceType3(AIKnowledge aiKnowledge)
public static bool IsDiceType5(AIKnowledge aiKnowledge)
```

**职责**: 判断命运骰子配置类型

**说明**:
- Type 3: 抬价意愿提高
- Type 5: 恶意竞争模式

---

#### IsAnyOneLeave

**签名**:
```csharp
public static bool IsAnyOneLeave(AIKnowledge aiKnowledge)
```

**职责**: 判断场上是否有人已退场

---

#### IsOnlyOne

**签名**:
```csharp
public static bool IsOnlyOne(AIKnowledge aiKnowledge)
```

**职责**: 判断场上是否除玩家外只有 1 个 AI

---

#### IsEventTarget1~4

**签名**:
```csharp
public static bool IsEventTarget1(AIKnowledge aiKnowledge)
// ... Target2, Target3, Target4
```

**职责**: 判断当前是否处于指定事件标记

---

### 剩余资金判断

#### IsMoneyEnoughJudge / Low / Medium / High / Allin

**签名**:
```csharp
public static bool IsMoneyEnoughJudge(AIKnowledge aiKnowledge)
public static bool IsMoneyEnoughLow(AIKnowledge aiKnowledge)
public static bool IsMoneyEnoughMedium(AIKnowledge aiKnowledge)
public static bool IsMoneyEnoughHigh(AIKnowledge aiKnowledge)
public static bool IsMoneyEnoughAllin(AIKnowledge aiKnowledge)
```

**职责**: 判断 AI 剩余资金是否足够进行不同档位的叫价

**判断标准**:
| 方法 | 判断条件 |
|------|----------|
| IsMoneyEnoughJudge | 资金 ≥ 估价 |
| IsMoneyEnoughLow | 资金 ≥ 低叫价档位 |
| IsMoneyEnoughMedium | 资金 ≥ 中叫价档位 |
| IsMoneyEnoughHigh | 资金 ≥ 高叫价档位 |
| IsMoneyEnoughAllin | 资金 > 高叫价档位（可梭哈） |

---

### 自身属性判断

#### IsRevenge

**签名**:
```csharp
public static bool IsRevenge(AIKnowledge aiKnowledge)
```

**职责**: 判断 AI 是否有复仇目标且复仇次数 > 0

---

#### IsRevengeAim

**签名**:
```csharp
public static bool IsRevengeAim(AIKnowledge aiKnowledge)
```

**职责**: 判断自己是否被其他 AI 设为复仇目标

**核心逻辑**:
```
遍历场上所有竞拍者
检查是否有 AI 的 RevengeTarget == 当前 AI 的 Entity.Id
```

---

#### IsRaise

**签名**:
```csharp
public static bool IsRaise(AIKnowledge aiKnowledge)
```

**职责**: 判断是否触发诱导抬价状态

---

#### IsAnger

**签名**:
```csharp
public static bool IsAnger(AIKnowledge aiKnowledge)
```

**职责**: 判断 AI 是否生气

**触发条件**: 场上价格超过玩家预判最大值，且上一次是玩家叫价

---

#### IsEnterNegative

**签名**:
```csharp
public static bool IsEnterNegative(AIKnowledge aiKnowledge)
```

**职责**: 判断是否进入消极状态

---

#### IsBidCountEmpty

**签名**:
```csharp
public static bool IsBidCountEmpty(AIKnowledge aiKnowledge)
```

**职责**: 判断本关剩余叫价次数是否为 0

**说明**: 仅进入消极状态后减少本关卡喊价次数

---

#### IsDeterminedToHave

**签名**:
```csharp
public static bool IsDeterminedToHave(AIKnowledge aiKnowledge)
```

**职责**: 判断是否志在必得

**说明**: 特殊盲盒时极小几率出现

---

#### IsHighPriceDeterrence

**签名**:
```csharp
public static bool IsHighPriceDeterrence(AIKnowledge aiKnowledge)
```

**职责**: 判断是否被高价震慑

---

#### IsFollow

**签名**:
```csharp
public static bool IsFollow(AIKnowledge aiKnowledge)
```

**职责**: 判断是否已经触发过跟风

---

#### IsBlack

**签名**:
```csharp
public static bool IsBlack(AIKnowledge aiKnowledge)
```

**职责**: 判断是否为黑衣人

**核心逻辑**: 检查是否挂载 BlackBoyComponent 组件

---

### 当前轮数据判断

#### IsStage1~5

**签名**:
```csharp
public static bool IsStage1(AIKnowledge aiKnowledge)
// ... Stage2, Stage3, Stage4, Stage5
```

**职责**: 判断当前是第几轮

---

#### IsAnyAIRevenge

**签名**:
```csharp
public static bool IsAnyAIRevenge(AIKnowledge aiKnowledge)
```

**职责**: 判断当前轮是否存在复仇者 AI

---

#### HasMiniPlayItem / HasTaskItem

**签名**:
```csharp
public static bool HasMiniPlayItem(AIKnowledge aiKnowledge)
public static bool HasTaskItem(AIKnowledge aiKnowledge)
```

**职责**: 判断当前轮是否有小玩法/任务物品

---

#### IsSpecial / IsAllSpecial

**签名**:
```csharp
public static bool IsSpecial(AIKnowledge aiKnowledge)
public static bool IsAllSpecial(AIKnowledge aiKnowledge)
```

**职责**: 判断是否为特殊/全玩法集装箱场次

**判断逻辑**:
```csharp
var container = ContainerConfigCategory.Instance.Get(Report.ContainerId);
return container.Type != 1;  // IsSpecial
return container.Type == 0;  // IsAllSpecial
```

---

#### IsPlayerRaiseSuccess

**签名**:
```csharp
public static bool IsPlayerRaiseSuccess(AIKnowledge aiKnowledge)
```

**职责**: 判断玩家是否至少抬价成功 1 次

---

#### IsGreaterThanAnchor1~4

**签名**:
```csharp
public static bool IsGreaterThanAnchor1(AIKnowledge aiKnowledge)
// ... Anchor2, Anchor3, Anchor4
```

**职责**: 判断上次出价是否大于 AI 预制价格锚点

**计算公式**:
```csharp
LastAuctionPrice > Config.AIPriceAnchorX * AllPrice / 100
```

---

#### AnyOneAuction

**签名**:
```csharp
public static bool AnyOneAuction(AIKnowledge aiKnowledge)
```

**职责**: 判断当前轮是否有人出价过

---

#### IsLessEqualPlayerRaiseSuccessCount / IsLessEqualRaiseCount

**签名**:
```csharp
public static bool IsLessEqualPlayerRaiseSuccessCount(AIKnowledge aiKnowledge)
public static bool IsLessEqualRaiseCount(AIKnowledge aiKnowledge)
```

**职责**: 判断玩家抬价成功次数是否小于等于锚点值

**计算公式**:
```csharp
// PlayerRaiseSuccessCount 锚点
RaiseSuccessCount <= Config.PlayerRaiseSuccB + Config.PlayerRaiseSuccK * RaiseSuccessCount

// RaiseCount 锚点
RaiseSuccessCount <= Config.RaiseCountB + Config.RaiseCountK * RaiseCount
```

---

#### HasBlack

**签名**:
```csharp
public static bool HasBlack(AIKnowledge aiKnowledge)
```

**职责**: 判断当前轮是否存在黑衣人

---

### 上一轮数据判断

#### IsMoreThanUserMaxJudgePrice

**签名**:
```csharp
public static bool IsMoreThanUserMaxJudgePrice(AIKnowledge aiKnowledge)
```

**职责**: 判断上一轮叫价是否大于玩家预判最大值

---

#### IsMoreThanMaxJudgePrice

**签名**:
```csharp
public static bool IsMoreThanMaxJudgePrice(AIKnowledge aiKnowledge)
```

**职责**: 判断上一轮叫价是否大于 AI 心理价位

---

#### IsLastHesitate

**签名**:
```csharp
public static bool IsLastHesitate(AIKnowledge aiKnowledge)
```

**职责**: 判断上一轮叫价是否犹豫

**说明**: 犹豫即拍卖师开始倒计时后才叫价

---

#### IsLastFast

**签名**:
```csharp
public static bool IsLastFast(AIKnowledge aiKnowledge)
```

**职责**: 判断上一轮叫价是否小于 1 秒

---

#### IsLastRevengeTarget / IsLastRevengeTargetIsMe

**签名**:
```csharp
public static bool IsLastRevengeTarget(AIKnowledge aiKnowledge)
public static bool IsLastRevengeTargetIsMe(AIKnowledge aiKnowledge)
```

**职责**: 
- IsLastRevengeTarget: 上一轮出价者是否是自己的复仇目标
- IsLastRevengeTargetIsMe: 上一轮出价者的复仇目标是否是自己

---

#### IsLastPlayer

**签名**:
```csharp
public static bool IsLastPlayer(AIKnowledge aiKnowledge)
```

**职责**: 判断上一轮出价者是否是玩家

---

### 新手引导判断

#### IsMoreThanAIMinPrice

**签名**:
```csharp
public static bool IsMoreThanAIMinPrice(AIKnowledge aiKnowledge)
```

**职责**: 新手引导中判断当前叫价是否 ≥ AIMinPrice

---

#### IsLessThanAIMaxPrice1/2/3

**签名**:
```csharp
public static bool IsLessThanAIMaxPrice1(AIKnowledge aiKnowledge)
public static bool IsLessThanAIMaxPrice2(AIKnowledge aiKnowledge)
public static bool IsLessThanAIMaxPrice3(AIKnowledge aiKnowledge)
```

**职责**: 新手引导中判断低/中/高价是否 ≤ AIMaxPrice

---

#### IsPlayerMaxRaiseCount

**签名**:
```csharp
public static bool IsPlayerMaxRaiseCount(AIKnowledge aiKnowledge)
```

**职责**: 新手引导中判断玩家抬价成功次数是否达到 PlayerMaxRaiseCount

---

## 静态构造方法

### 静态构造函数

**职责**: 初始化时通过反射注册所有静态决策方法

**核心逻辑**:
```csharp
static AIDecisionInterface()
{
    Methods = new Dictionary<string, Func<AIKnowledge, bool>>();
    var methodInfos = TypeInfo<AIDecisionInterface>.Type.GetMethods();
    
    foreach (var method in methodInfos)
    {
        if (!method.IsStatic) continue;
        
        // 创建委托
        var func = (Func<AIKnowledge, bool>)Delegate.CreateDelegate(
            typeof(Func<AIKnowledge, bool>), null, method);
        Methods.Add(method.Name, func);
        
        // 编辑器中读取 LabelText 属性作为显示名
        #if UNITY_EDITOR
        var attrs = method.GetCustomAttributes(typeof(LabelTextAttribute), false);
        if (attrs.Length > 0)
        {
            MethodNames.Add(method.Name, $"{((LabelTextAttribute)attrs[0]).Text}({method.Name})");
        }
        #endif
    }
}
```

**设计意图**: 
- 自动注册所有静态判断方法，无需手动登记
- 支持 Odin Inspector 的 LabelText 属性，在编辑器中显示友好的中文名称

---

## 使用示例

### 在 AI 决策树中使用

```csharp
// 决策条件：剩余资金是否足够高叫价
if (AIDecisionInterface.IsMoneyEnoughHigh(aiKnowledge))
{
    // 执行高叫价逻辑
    AIAuctionManager.Instance.AuctionHigh();
}

// 决策条件：是否复仇状态
if (AIDecisionInterface.IsRevenge(aiKnowledge))
{
    // 复仇模式：提高叫价意愿
    aiKnowledge.BidAggressiveness += 0.2f;
}

// 决策条件：新手引导场次
if (AIDecisionInterface.IsGuidance(aiKnowledge))
{
    // 执行引导逻辑
    GuidanceManager.ExecuteStage(IAuctionManager.Instance.Stage);
}
```

### 在编辑器配置中使用

```csharp
// 通过方法名获取决策方法
if (AIDecisionInterface.Methods.TryGetValue("IsMoneyEnoughHigh", out var method))
{
    bool result = method.Invoke(aiKnowledge);
}

// 编辑器中显示友好的名称
if (AIDecisionInterface.MethodNames.TryGetValue("IsMoneyEnoughHigh", out var displayName))
{
    // displayName = "剩余资金是否足够高叫价 (IsMoneyEnoughHigh)"
    GUILayout.Label(displayName);
}
```

---

## 相关文档

- [AIKnowledge.cs](../AIKnowledge.cs.md) - AI 知识数据结构
- [AIComponent.cs](../AIComponent.cs.md) - AI 组件
- [AuctionGuideManager.cs](../../../System/Auction/AuctionGuideManager.cs.md) - 拍卖引导管理器
- [BlackBoyComponent.cs](../View/BlackBoyComponent.cs.md) - 黑衣人组件

---

*文档生成时间：2026-03-01 | OpenClaw AI 助手*
