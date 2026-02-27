# AuctionState.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | AuctionState.cs |
| **路径** | Assets/Scripts/Code/Game/System/Auction/AuctionState.cs |
| **所属模块** | 玩法层 → Code/Game/System/Auction |
| **文件职责** | 定义拍卖系统的状态枚举 |

---

## 类/结构体说明

### AuctionState (枚举)

| 属性 | 说明 |
|------|------|
| **职责** | 定义拍卖系统的所有状态 |
| **泛型参数** | 无 |
| **继承关系** | 继承自 `int`（默认） |
| **实现的接口** | 无 |

```csharp
public enum AuctionState
{
    Free,          // 空闲
    Awake,         // 第一场进入前
    Prepare,       // 第一场进入准备中
    EnterAnim,     // 开场动画
    Ready,         // 当前轮准备完成
    AIThink,       // 当前轮进行中（AI 思考）
    WaitUser,      // 等待玩家操作
    ExitAnim,      // 当前轮结束动画
    OpenBox,       // 等待玩家开箱
    Over,          // 当前轮结算
    ReEnterAnim,   // 再次入场动画
    AllOverAnim,   // 所有结束动画
    AllOver,       // 所有轮结束
    RePrepare,     // 再来一局
}
```

---

## 状态详细说明

### 状态分类

| 阶段 | 状态 | 说明 |
|------|------|------|
| **初始阶段** | `Free` | 空闲状态，拍卖系统未启动 |
| | `Awake` | 第一场进入前，初始化实体和配置 |
| | `Prepare` | 第一场进入准备中，生成集装箱盲盒 |
| **开场阶段** | `EnterAnim` | 开场动画，播放入场动画 |
| | `Ready` | 当前轮准备完成，等待开始叫价 |
| **叫价阶段** | `AIThink` | AI 思考并叫价 |
| | `WaitUser` | 等待玩家操作（叫价或跳过） |
| **结算阶段** | `ExitAnim` | 当前轮结束动画 |
| | `OpenBox` | 玩家开箱，查看拍到的物品 |
| | `Over` | 当前轮结算，计算收益 |
| **循环阶段** | `ReEnterAnim` | 再次入场动画（下一轮） |
| | `AllOverAnim` | 所有轮结束动画 |
| | `AllOver` | 所有轮结束 |
| | `RePrepare` | 再来一局，准备重新开始 |

---

## 状态流转

### 完整流程图

```
Free → Awake → Prepare → EnterAnim → Ready → [AIThink → WaitUser → ExitAnim → OpenBox → Over] × N → AllOverAnim → AllOver → (RePrepare → Awake) 或 Free
```

### 状态说明

#### Free (空闲)

**说明**: 拍卖系统未启动或已完全结束

**触发**: 系统初始化或拍卖完全结束

**下一状态**: `Awake`（开始新游戏）

---

#### Awake (初始化)

**说明**: 第一场进入前，初始化所有实体和配置

**主要工作**:
- 创建拍卖师实体（Host）
- 创建玩家实体（Player）
- 创建 AI 竞拍者实体（Bidder）
- 创建背景 NPC（可选）
- 初始化 AI 决策数组

**触发**: 游戏开始

**下一状态**: `Prepare`

---

#### Prepare (准备)

**说明**: 第一场进入准备中，生成集装箱盲盒数据

**主要工作**:
- 生成集装箱盲盒 AuctionReports[]
- 计算总价值 AllPrice
- 计算系统判断价格区间
- 等待玩家选择情报/骰子

**触发**: Awake 完成

**下一状态**: `EnterAnim`（准备完成）

---

#### EnterAnim (开场动画)

**说明**: 播放开场动画

**主要工作**:
- 播放入场动画序列
- 拍卖师开场白
- 展示规则说明

**触发**: Prepare 完成

**下一状态**: `Ready`（动画完成）

---

#### Ready (准备就绪)

**说明**: 当前轮准备完成，等待开始叫价

**主要工作**:
- 展示当前轮集装箱
- 显示底价信息
- 准备叫价 UI

**触发**: EnterAnim 完成或 ReEnterAnim 完成

**下一状态**: `AIThink`

---

#### AIThink (AI 思考)

**说明**: AI 竞拍者思考并叫价

**主要工作**:
- 遍历所有 AI 竞拍者
- 调用 AIDecision.Decide() 决策
- AI 叫价或离场

**触发**: Ready 完成

**下一状态**: `WaitUser`（所有 AI 完成决策）

---

#### WaitUser (等待玩家)

**说明**: 等待玩家操作

**主要工作**:
- 显示叫价按钮（低/中/高）
- 启动倒计时
- 拍卖师倒计时语音
- 处理玩家超时

**触发**: AIThink 完成

**下一状态**: `ExitAnim`（玩家操作或超时）

---

#### ExitAnim (结束动画)

**说明**: 当前轮结束动画

**主要工作**:
- 播放结算动画
- 展示最终叫价
- 恭喜中标者

**触发**: WaitUser 完成

**下一状态**: `OpenBox`

---

#### OpenBox (开箱)

**说明**: 玩家开箱，查看拍到的物品

**主要工作**:
- 显示集装箱
- 播放开箱动画
- 展示物品列表
- 检查小玩法物品

**触发**: ExitAnim 完成

**下一状态**: `Over`

---

#### Over (结算)

**说明**: 当前轮结算

**主要工作**:
- 计算收益
- 更新玩家金钱
- 显示结算界面
- 检查是否还有下一轮

**触发**: OpenBox 完成

**下一状态**: `ReEnterAnim`（还有下轮）或 `AllOverAnim`（所有轮完成）

---

#### ReEnterAnim (再次入场)

**说明**: 再次入场动画（下一轮）

**主要工作**:
- 播放入场动画
- 展示下一轮信息

**触发**: Over 且还有下轮

**下一状态**: `Ready`

---

#### AllOverAnim (结束动画)

**说明**: 所有轮结束动画

**主要工作**:
- 播放总结算动画
- 展示总收益
- 播放成就动画

**触发**: Over 且所有轮完成

**下一状态**: `AllOver`

---

#### AllOver (所有结束)

**说明**: 所有轮结束

**主要工作**:
- 显示最终结算界面
- 提供"再来一局"选项
- 提供"返回家园"选项

**触发**: AllOverAnim 完成

**下一状态**: `RePrepare`（再来一局）或 `Free`（返回家园）

---

#### RePrepare (再来一局)

**说明**: 准备重新开始

**主要工作**:
- 重置计数器
- 保留玩家数据
- 准备新的拍卖

**触发**: AllOver 选择再来一局

**下一状态**: `Awake`

---

## 状态机实现

### 在 AuctionManager 中的使用

```csharp
public partial class AuctionManager : IUpdate
{
    public AuctionState AState { get; private set; }
    private bool isEnterState;  // 是否刚进入状态

    private void SetState(AuctionState state)
    {
        if (AState != state)
        {
            AState = state;
            isEnterState = true;  // 标记刚进入
            Messager.Instance.Broadcast(0, MessageId.RefreshAuctionState, AState);
            Log.Info($"[Auction] Change state to {state}");
        }
    }

    public void Update()
    {
        if (isDispose) return;
        
        var lAState = AState;
        switch (AState)
        {
            case AuctionState.Awake:
                Awake();
                break;
            case AuctionState.Prepare:
                Prepare();
                break;
            case AuctionState.EnterAnim:
                PlayEnterAnim().Coroutine();
                break;
            case AuctionState.Ready:
                Ready();
                break;
            case AuctionState.AIThink:
                AIThink();
                break;
            case AuctionState.WaitUser:
                WaitUser();
                break;
            case AuctionState.ExitAnim:
                ExitAnim();
                break;
            case AuctionState.OpenBox:
                OpenBox();
                break;
            case AuctionState.Over:
                Over();
                break;
            case AuctionState.ReEnterAnim:
                ReEnterAnim();
                break;
            case AuctionState.AllOverAnim:
                AllOverAnim();
                break;
            case AuctionState.AllOver:
                AllOver();
                break;
            case AuctionState.RePrepare:
                RePrepare();
                break;
        }

        // 如果状态未变化，清除 isEnterState 标志
        if (AState == lAState)
            isEnterState = false;
    }
}
```

---

## isEnterState 标志

### 作用

`isEnterState` 用于区分状态的**首帧**和**后续帧**：

```csharp
// 首帧执行初始化逻辑
if (isEnterState)
{
    // 只执行一次的初始化
    InitializeState();
}

// 每帧执行的逻辑
UpdateState();
```

### 使用示例

```csharp
private void Prepare()
{
    // 首帧：开始录制
    if (isEnterState)
    {
        GameRecorderManager.Instance.StartRecorder().Coroutine();
    }
    
    // 每帧：检查准备进度
    if (IsPrepareComplete())
    {
        SetState(AuctionState.EnterAnim);
    }
}

private void WaitUser()
{
    // 首帧：显示 UI，启动倒计时
    if (isEnterState)
    {
        ShowBidButtons();
        StartCountdown();
    }
    
    // 每帧：检查倒计时
    CheckCountdown();
}
```

---

## 阅读指引

### 建议的阅读顺序

1. **理解状态定义** - 14 个状态的含义
2. **看状态流转** - 理解完整游戏流程
3. **了解 isEnterState** - 理解首帧/后续帧区分
4. **看 Update 实现** - 理解状态机主循环

### 最值得学习的技术点

1. **状态机模式**: 清晰的游戏流程控制
2. **isEnterState 标志**: 区分首帧和后续帧
3. **状态广播**: Messager 通知 UI 更新
4. **枚举设计**: 状态命名清晰易懂

---

## 使用示例

### 示例 1: 检查当前状态

```csharp
// 检查是否在等待玩家操作
if (AuctionManager.Instance.AState == AuctionState.WaitUser)
{
    // 显示叫价按钮
    ShowBidButtons();
}

// 检查是否在开箱
if (AuctionManager.Instance.AState == AuctionState.OpenBox)
{
    // 显示开箱 UI
    ShowOpenBoxUI();
}
```

### 示例 2: 监听状态变化

```csharp
// 注册事件监听
Messager.Instance.AddListener<AuctionState>(
    0, 
    MessageId.RefreshAuctionState, 
    OnAuctionStateChanged
);

// 处理状态变化
private void OnAuctionStateChanged(AuctionState state)
{
    Log.Info($"拍卖状态变化：{state}");
    
    switch (state)
    {
        case AuctionState.WaitUser:
            ShowBidButtons();
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

### 示例 3: 状态机调试

```csharp
// 调试模式下打印状态流转
#if UNITY_EDITOR
private AuctionState lastState = AuctionState.Free;

void Update()
{
    if (AuctionManager.Instance.AState != lastState)
    {
        Debug.Log($"[Debug] {lastState} → {AuctionManager.Instance.AState}");
        lastState = AuctionManager.Instance.AState;
    }
}
#endif
```

---

## 相关文档

- [AuctionManager.cs.md](./AuctionManager.cs.md) - 拍卖管理器（使用者）
- [IAuctionManager.cs.md](./IAuctionManager.cs.md) - 接口定义
- [AuctionManager.State.cs.md](./AuctionManager.State.cs.md) - 状态机实现

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
