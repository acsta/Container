# IAuctionManager.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | IAuctionManager.cs |
| **路径** | Assets/Scripts/Code/Game/System/Auction/IAuctionManager.cs |
| **所属模块** | 玩法层 → Code/Game/System/Auction |
| **文件职责** | 定义拍卖系统对外接口，提供全局访问点 |

---

## 类/结构体说明

### IAuctionManager (接口)

| 属性 | 说明 |
|------|------|
| **职责** | 定义拍卖系统的对外接口，解耦实现与使用 |
| **泛型参数** | 无 |
| **继承关系** | 无 |
| **实现的接口** | 无 |

**设计模式**: 接口隔离 + 单例模式

```csharp
// 接口定义
public interface IAuctionManager
{
    // 静态单例
    public static IAuctionManager Instance { get; }
    
    // 属性和方法...
}

// 实现类
public partial class AuctionManager : IAuctionManager, IManager<MapScene>, IUpdate
{
    // 实现接口方法
}

// 使用
IAuctionManager.Instance.UserAuction(AITactic.Low);
```

---

## 静态成员

### Instance (静态属性)

**签名**:
```csharp
public static IAuctionManager Instance { get; }
```

**职责**: 全局单例访问点

**设置时机**:
```csharp
// AuctionManager.Init() 中设置
public void Init(MapScene map)
{
    IAuctionManager.Instance = this;
    // ...
}

// AuctionManager.Destroy() 中清空
public void Destroy()
{
    IAuctionManager.Instance = null;
    // ...
}
```

**使用示例**:
```csharp
// 全局访问拍卖系统
var auction = IAuctionManager.Instance;

// 玩家叫价
auction.UserAuction(AITactic.Low);

// 获取当前状态
var state = auction.AState;
```

---

### UserReady (静态字段)

**签名**:
```csharp
public static bool UserReady { get; set; }
```

**职责**: 标记玩家是否准备就绪

**使用场景**: 开场前等待玩家准备

---

## 属性说明

### 核心状态

| 属性 | 类型 | 说明 |
|------|------|------|
| `AState` | `AuctionState` | 当前拍卖状态 |
| `Level` | `int` | 难度等级 |
| `Stage` | `int` | 轮次（从 1 开始） |

### 配置相关

| 属性 | 类型 | 说明 |
|------|------|------|
| `Config` | `StageConfig` | 当前关卡配置 |
| `LevelConfig` | `LevelConfig` | 难度配置 |
| `GameInfoConfig` | `GameInfoConfig` | 情报配置 |
| `DiceConfig` | `DiceConfig` | 骰子配置 |
| `StartEventConfig` | `StartEventConfig` | 开局事件配置 |

### 拍卖数据

| 属性 | 类型 | 说明 |
|------|------|------|
| `AuctionReports` | `AuctionReport[]` | 所有轮的集装箱盲盒数据 |
| `Report` | `AuctionReport` | 当前轮数据 |
| `LowAuction` | `BigNumber` | 低价（低档叫价） |
| `MediumAuction` | `BigNumber` | 中价（中档叫价） |
| `HighAuction` | `BigNumber` | 高价（高档叫价） |

### 参与者

| 属性 | 类型 | 说明 |
|------|------|------|
| `Bidders` | `List<long>` | 竞拍者 ID 列表 |
| `Npcs` | `List<long>` | NPC ID 列表 |
| `Boxes` | `List<long>` | 集装箱 ID 列表 |
| `OpenBoxes` | `List<long>` | 已开箱的集装箱 ID |
| `Blacks` | `List<long>` | 黑市商人 ID 列表 |
| `Player` | `Player` | 玩家实体 |
| `HostId` | `long` | 拍卖师 ID |

### 统计计数

| 属性 | 类型 | 说明 |
|------|------|------|
| `AllPrice` | `BigNumber` | 总价值 |
| `SysJudgePriceMin` | `float` | 系统判断价格最小值 |
| `SysJudgePriceMax` | `float` | 系统判断价格最大值 |
| `LastAuctionPlayerId` | `long` | 上一个叫价的人 |
| `LastAuctionPrice` | `BigNumber` | 上一次叫价 |
| `LastAuctionPriceType` | `AITactic` | 上一次叫价类型 |
| `LastAuctionTime` | `long` | 上一次叫价间隔时间 |
| `LastHostSayCount` | `int` | 拍卖师倒计时次数 |
| `IsRaising` | `bool` | 是否抬价阶段 |
| `RaiseSuccessCount` | `int` | 玩家成功抬价次数 |
| `RaiseCount` | `int` | 抬价次数 |
| `PlayerAuctionCount` | `int` | 玩家出价次数 |
| `AuctionCount` | `int` | 总出价次数 |

### 特殊标志

| 属性 | 类型 | 说明 |
|------|------|------|
| `HasMiniPlayItem` | `bool` | 是否有小玩法物品 |
| `HasTaskItem` | `bool` | 是否有任务物品 |
| `Skip` | `bool` | 快速跳过标志 |
| `IsAllPlayBox` | `bool` | 是否全玩法箱子 |

---

## 方法说明

### ForceAllOver()

**签名**:
```csharp
public void ForceAllOver()
```

**职责**: 强行退出拍卖（玩家中途离开）

**核心逻辑**:
```
1. 清理所有实体
2. 播放离开动画
3. 切换到 AllOver 状态
4. 返回家园场景
```

**调用者**: 玩家点击返回按钮

---

### UserAuction(AITactic type)

**签名**:
```csharp
public void UserAuction(AITactic type)
```

**职责**: 玩家叫价

**参数**:
- `type`: 叫价类型（Low/Medium/High）

**核心逻辑**:
```
1. 检查当前状态是否为 WaitUser
2. 根据 type 计算叫价
3. 更新 LastAuctionPrice
4. 更新 LastAuctionPlayerId = 0（玩家）
5. 切换到 AIThink 状态
```

**调用者**: UI 按钮点击事件

**使用示例**:
```csharp
// 玩家选择低价
IAuctionManager.Instance.UserAuction(AITactic.Low);

// 玩家选择中价
IAuctionManager.Instance.UserAuction(AITactic.Medium);

// 玩家选择高价
IAuctionManager.Instance.UserAuction(AITactic.High);
```

---

### RunNextStage()

**签名**:
```csharp
public void RunNextStage()
```

**职责**: 进入下一轮拍卖

**核心逻辑**:
```
1. Stage++
2. 检查是否还有下一轮
3. 切换到 ReEnterAnim 或 AllOverAnim
```

**调用者**: `Over()`（当前轮结算完成后）

---

### SetAppraisalResult(int configId, int newId)

**签名**:
```csharp
public void SetAppraisalResult(int configId, int newId)
```

**职责**: 设置鉴定结果（鉴定小玩法）

**参数**:
- `configId`: 原物品 ID
- `newId`: 新物品 ID

**调用者**: 鉴定小玩法完成

---

### SetMiniGameResult(int configId, BigNumber newPrice)

**签名**:
```csharp
public void SetMiniGameResult(int configId, BigNumber newPrice)
```

**职责**: 设置小游戏结果（价格调整类小玩法）

**参数**:
- `configId`: 物品 ID
- `newPrice`: 新价格

**调用者**: 小游戏完成

---

### GetFinalGameInfoConfig(bool ignoreId)

**签名**:
```csharp
public GameInfoConfig GetFinalGameInfoConfig(bool ignoreId = false)
```

**职责**: 获取最终应用的情报配置

**参数**:
- `ignoreId`: 是否忽略 ID 检查

**返回**: 应用后的情报配置

**调用者**: 需要获取情报效果的代码

---

### SelectGameInfo(int id)

**签名**:
```csharp
public void SelectGameInfo(int id)
```

**职责**: 选择情报

**参数**:
- `id`: 情报 ID

**调用者**: 情报选择 UI

---

### SelectDice(int id, Action onSelectOver)

**签名**:
```csharp
public void SelectDice(int id, Action onSelectOver)
```

**职责**: 选择命运骰子

**参数**:
- `id`: 骰子 ID
- `onSelectOver`: 选择完成回调

**调用者**: 骰子选择 UI

---

### RefreshWinLossAnim(bool play)

**签名**:
```csharp
public void RefreshWinLossAnim(bool play)
```

**职责**: 刷新输赢动画

**参数**:
- `play`: 是否播放动画

**调用者**: 结算界面

---

### Leave(long id, int type)

**签名**:
```csharp
public void Leave(long id, int type)
```

**职责**: AI 竞拍者离场

**参数**:
- `id`: AI ID
- `type`: 离场类型（0=走开）

**调用者**: AI 决策逻辑

---

### GetLevelCount()

**签名**:
```csharp
public int GetLevelCount()
```

**职责**: 获取离场人数

**返回**: 已离场的 AI 数量

**调用者**: 统计逻辑

---

## 阅读指引

### 建议的阅读顺序

1. **理解接口作用** - 为什么需要 IAuctionManager
2. **看 Instance 单例** - 理解全局访问点
3. **看核心属性** - 了解 AState/Level/Stage
4. **看 UserAuction 方法** - 理解玩家叫价接口

### 最值得学习的技术点

1. **接口单例**: IAuctionManager.Instance 全局访问
2. **接口隔离**: 将接口定义与实现分离
3. **静态成员**: Instance 和 UserReady 使用 static
4. **属性只读**: 大部分属性只有 getter，确保数据安全

---

## 使用示例

### 示例 1: 玩家叫价

```csharp
// UI 按钮点击事件
public void OnLowBidClick()
{
    // 检查是否可以叫价
    if (IAuctionManager.Instance.AState == AuctionState.WaitUser)
    {
        IAuctionManager.Instance.UserAuction(AITactic.Low);
    }
}

public void OnMidBidClick()
{
    if (IAuctionManager.Instance.AState == AuctionState.WaitUser)
    {
        IAuctionManager.Instance.UserAuction(AITactic.Medium);
    }
}

public void OnHighBidClick()
{
    if (IAuctionManager.Instance.AState == AuctionState.WaitUser)
    {
        IAuctionManager.Instance.UserAuction(AITactic.High);
    }
}
```

### 示例 2: 获取当前状态

```csharp
// UI 根据状态显示不同内容
void Update()
{
    var state = IAuctionManager.Instance.AState;
    
    switch (state)
    {
        case AuctionState.WaitUser:
            ShowBidButtons();
            UpdateBidButtons();  // 更新按钮状态
            break;
        case AuctionState.OpenBox:
            ShowOpenBoxUI();
            break;
        case AuctionState.AllOver:
            ShowResultUI();
            break;
    }
}
```

### 示例 3: 获取叫价信息

```csharp
// 显示当前叫价信息
void UpdateUI()
{
    var auction = IAuctionManager.Instance;
    
    // 显示当前价格
    priceText.text = $"当前价格：{auction.LastAuctionPrice}";
    
    // 显示下一轮叫价
    lowText.text = $"低价：{auction.LowAuction}";
    midText.text = $"中价：{auction.MediumAuction}";
    highText.text = $"高价：{auction.HighAuction}";
    
    // 显示统计
    countText.text = $"第{auction.Stage}轮 | 总出价：{auction.AuctionCount}";
}
```

### 示例 4: 监听状态变化

```csharp
// 注册事件
void OnEnable()
{
    Messager.Instance.AddListener<AuctionState>(
        0, 
        MessageId.RefreshAuctionState, 
        OnAuctionStateChanged
    );
}

void OnDisable()
{
    Messager.Instance.RemoveListener<AuctionState>(
        0, 
        MessageId.RefreshAuctionState, 
        OnAuctionStateChanged
    );
}

// 处理状态变化
void OnAuctionStateChanged(AuctionState state)
{
    Log.Info($"拍卖状态变化：{state}");
    
    // 更新 UI
    switch (state)
    {
        case AuctionState.WaitUser:
            ShowBidButtons();
            break;
        case AuctionState.OpenBox:
            ShowOpenBoxUI();
            break;
    }
}
```

---

## 接口 vs 实现

### 接口定义 (IAuctionManager.cs)

```csharp
// 接口：定义契约
public interface IAuctionManager
{
    public static IAuctionManager Instance { get; }
    public AuctionState AState { get; }
    public void UserAuction(AITactic type);
    // ...
}
```

### 实现类 (AuctionManager.cs)

```csharp
// 实现：具体逻辑
public partial class AuctionManager : IAuctionManager, IManager<MapScene>, IUpdate
{
    // 实现接口
    public static IAuctionManager Instance { get; set; }
    public AuctionState AState { get; private set; }
    
    public void UserAuction(AITactic type)
    {
        // 具体实现
    }
    
    // 额外方法和字段（接口不暴露）
    private void AIThink() { }
    private void WaitUser() { }
}
```

---

## 相关文档

- [AuctionManager.cs.md](./AuctionManager.cs.md) - 拍卖管理器（实现类）
- [AuctionState.cs.md](./AuctionState.cs.md) - 状态枚举
- [AuctionManager.State.cs.md](./AuctionManager.State.cs.md) - 状态机实现

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
