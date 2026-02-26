# Container 玩法层系统详解 V2

> **文档版本**: v2.0  
> **生成时间**: 2026-02-27  
> **分析范围**: 玩法层 15 个核心系统  
> **组织方式**: 按玩家体验流程排序  
> **命名空间**: `TaoTie`

---

## 📑 目录

### 阶段一：游戏入口
1. [Login/PlayerManager - 登录系统](#1-loginplayermanager---登录系统)
2. [UILoading - 加载系统](#2-uiloading---加载系统)

### 阶段二：家园/大厅
3. [HomeScene/UILobby - 家园与大厅](#3-homesceneuilobby---家园与大厅)
4. [UICommon - 通用 UI 组件](#4-uicommon---通用 ui 组件)

### 阶段三：拍卖玩法（核心）
5. [AuctionSystem - 拍卖系统](#5-auctionsystem---拍卖系统)
6. [UIAuction - 拍卖 UI](#6-uiauction---拍卖 ui)
7. [AIComponent - AI 竞拍者](#7-aicomponent---ai 竞拍者)

### 阶段四：小玩法/互动
8. [MiniGame - 小游戏](#8-minigame---小游戏)
9. [EnvironmentSystem - 环境系统](#9-environmentsystem---环境系统)

### 阶段五：数值/成长
10. [NumericSystem - 数值系统](#10-numericsystem---数值系统)
11. [PlayerData - 玩家数据](#11-playerdata---玩家数据)

### 阶段六：引导/帮助
12. [GuidanceManager - 引导系统](#12-guidancemanager---引导系统)
13. [GuideScene - 引导场景](#13-guidescene---引导场景)

### 阶段七：系统功能
14. [RedDotManager - 红点系统](#14-reddotmanager---红点系统)
15. [I18NManager - 多语言](#15-i18nmanager---多语言)

---

## 阶段一：游戏入口

### 1. Login/PlayerManager - 登录系统

#### 1.1 系统概述

**系统名称**: PlayerManager（玩家管理器）

**玩家感知表现**:
- 启动游戏后看到登录界面
- 选择登录方式（抖音/微信/快手/B 站等）
- 登录成功后进入加载界面
- 显示玩家昵称和头像

**重要程度**: ⭐⭐⭐⭐⭐ **核心系统**

**游戏类型**: 微信小程序/小游戏为主的休闲拍卖游戏

---

#### 1.2 游戏设计意图

**体验贡献**:
- **低门槛接入**: 支持多平台一键登录，无需注册账号
- **快速开始**: 登录流程简化，3 秒内进入游戏
- **数据持久化**: 自动保存玩家进度，支持多设备同步

**设计决策**:
| 决策 | 原因 | 替代方案 |
|------|------|----------|
| 使用平台 SDK 登录 | 小游戏生态，无需额外账号 | 自建账号系统 |
| 自动登录优先 | 减少操作步骤，提升留存 | 每次手动登录 |
| 多平台统一接口 | 代码复用，便于维护 | 各平台独立实现 |

---

#### 1.3 技术实现方案

**依赖的框架层模块**:

```mermaid
graph TD
    subgraph Player["PlayerManager"]
        PM[PlayerManager]
        PD[PlayerData]
        SDK[SDKManager]
    end
    
    subgraph Framework["框架层依赖"]
        Messager[Messager<br/>事件通知]
        Config[ConfigManager<br/>配置读取]
        API[APIManager<br/>网络请求]
        Timer[TimerManager<br/>超时处理]
    end
    
    PM --> Messager
    PM --> Config
    PM --> API
    PM --> Timer
    PM --> SDK
    PM --> PD
    
    style Player fill:#ffebee
    style Framework fill:#e3f2fd
```

**核心类图**:

```mermaid
classDiagram
    class PlayerManager {
        -static Instance: PlayerManager
        -Uid: int
        -OnLine: bool
        +Init(): void
        +Destroy(): void
        +Login(jump: bool): ETTask<bool>
    }

    class PlayerData {
        +Platform: LoginPlatform
        +Version: long
        +IsGuideScene: bool
        +Avatar: string
        +NickName: string
        +Money: BigNumber
        +LastLevelId: int
        +UnlockTechnologyTreeIds: HashSet<int>
        +OverTaskCount: Dictionary<int, int>
    }

    class LoginPlatform {
        <<enumeration>>
        Dev
        TikTok
        WeChat
        TapTap
        Bilibili
        KuaiShou
        QuickGame
        Alipay
    }

    class SDKManager {
        +Login(): ETTask<string>
        +Init(): void
    }

    PlayerManager --> PlayerData
    PlayerManager --> LoginPlatform
    PlayerManager --> SDKManager
    
    note for PlayerManager "⭐ 登录入口<br/>多平台适配"
    note for PlayerData "📦 数据存储<br/>序列化/反序列化"
```

**核心数据结构**:

```csharp
// 玩家数据（序列化到本地/服务器）
public class PlayerData
{
    public LoginPlatform Platform;          // 登录平台
    public long Version;                     // 数据版本号（用于同步）
    public bool IsGuideScene;                // 是否完成引导
    public string Avatar;                    // 头像 URL
    public string NickName;                  // 昵称
    public BigNumber Money;                  // 金钱（大数防止溢出）
    public int LastLevelId;                  // 上次挑战关卡
    public HashSet<int> UnlockTechnologyTreeIds; // 解锁的科技树
    public Dictionary<int, int> OverTaskCount;   // 完成任务统计
    // ... 更多字段
}
```

---

#### 1.4 运行时工作流

**典型登录流程**:

```mermaid
sequenceDiagram
    participant Player as 玩家
    participant UI as 登录 UI
    participant PM as PlayerManager
    participant SDK as SDKManager
    participant API as APIManager
    participant PD as PlayerData
    participant SM as SceneManager

    Player->>UI: 点击登录按钮
    UI->>PM: Login()
    
    PM->>SDK: 调用平台 SDK 登录
    Note over SDK: 抖音/微信/快手等
    
    SDK-->>PM: 返回 code/openid
    PM->>API: 发送登录请求 (code)
    
    API-->>PM: 返回玩家数据
    PM->>PD: 反序列化数据
    
    alt 新用户
        PM->>SM: SwitchScene<GuideScene>
        Note over SM: 进入引导场景
    else 老用户
        PM->>SM: SwitchScene<HomeScene>
        Note over SM: 进入家园场景
    end
    
    PM->>Messager: Broadcast(OnLoginSuccess)
    Note over Messager: 通知其他系统
```

**异常处理流程**:

```mermaid
flowchart TD
    Start[开始登录] --> SDK{SDK 调用}
    SDK -->|成功 | API[服务器验证]
    SDK -->|失败 | Retry{重试？}
    Retry -->|是 | SDK
    Retry -->|否 | Error[显示错误提示]
    
    API -->|成功 | LoadData[加载玩家数据]
    API -->|失败 | NetError[网络错误处理]
    
    LoadData --> Deserialize{反序列化}
    Deserialize -->|成功 | CheckGuide{检查引导}
    Deserialize -->|失败 | Rollback[数据回滚]
    
    CheckGuide -->|未引导 | GuideScene[进入引导场景]
    CheckGuide -->|已引导 | HomeScene[进入家园场景]
    
    Error --> End[结束]
    NetError --> End
    Rollback --> End
    GuideScene --> End
    HomeScene --> End
```

**边界情况处理**:

| 情况 | 处理方式 |
|------|----------|
| 网络超时 | 显示重试按钮，最多重试 3 次 |
| 数据损坏 | 使用本地缓存，提示玩家 |
| 版本不匹配 | 强制更新或清除缓存 |
| SDK 初始化失败 | 降级到游客模式 |

---

#### 1.5 配置与数据驱动

**可配置的数据**:

| 配置项 | 类型 | 位置 | 可调范围 |
|--------|------|------|----------|
| 登录超时时间 | int | 代码常量 | 5000-30000ms |
| 重试次数 | int | 代码常量 | 1-5 次 |
| 支持的平台 | enum | LoginPlatform | 添加新平台 |
| 服务器地址 | string | 配置表 | 根据渠道配置 |

**策划调整参数**:

```csharp
// 登录超时配置（可在 GlobalConfig 配置表调整）
if (!GlobalConfigCategory.Instance.TryGetInt("LoginTimeout", out timeout))
{
    timeout = 10000; // 默认 10 秒
}

// 重试次数配置
if (!GlobalConfigCategory.Instance.TryGetInt("LoginRetryCount", out retryCount))
{
    retryCount = 3; // 默认 3 次
}
```

---

#### 1.6 与其他玩法系统的协作

**系统协作关系**:

```mermaid
graph TB
    subgraph Player["玩家系统"]
        PM[PlayerManager]
        PD[PlayerData]
    end
    
    subgraph Core["核心玩法"]
        Auction[AuctionSystem]
        Numeric[NumericSystem]
    end
    
    subgraph Support["支持系统"]
        Config[ConfigManager]
        API[APIManager]
        SDK[SDKManager]
        Messager[Messager]
    end
    
    PM --> SDK
    PM --> API
    PM --> Config
    PM --> Messager
    
    Auction --> PD
    Numeric --> PD
    
    Messager --> Auction
    Messager --> Numeric
    
    style Player fill:#ffebee
    style Core fill:#e8f5e9
    style Support fill:#e3f2fd
```

**通信方式**:

| 交互系统 | 通信方式 | 传递内容 |
|---------|---------|----------|
| AuctionSystem | 直接读取 | `PlayerData.Money`, `Uid` |
| NumericSystem | 直接读取 | 玩家属性数据 |
| UIManager | Messager 事件 | `OnLoginSuccess`, `OnLoginFailed` |
| SceneManager | 直接调用 | `SwitchScene<HomeScene/GuideScene>` |

---

### 2. UILoading - 加载系统

#### 2.1 系统概述

**系统名称**: UILoading（加载界面系统）

**玩家感知表现**:
- 登录成功后看到加载界面
- 进度条从 0% 增长到 100%
- 显示加载提示文字（"正在加载资源..."）
- 加载完成后自动进入下一场景

**重要程度**: ⭐⭐⭐⭐ **核心系统**

---

#### 2.2 游戏设计意图

**体验贡献**:
- **视觉反馈**: 进度条让玩家知道加载进度，减少焦虑
- **加载提示**: 提示文字解释当前操作，提升理解
- **平滑过渡**: 场景切换不突兀，保持沉浸感

**设计决策**:
| 决策 | 原因 | 替代方案 |
|------|------|----------|
| 分阶段加载 | 避免长时间白屏 | 一次性加载 |
| 显示进度百分比 | 明确告知玩家进度 | 只显示动画 |
| 可配置提示文字 | 支持多语言、多场景 | 硬编码文字 |

---

#### 2.3 技术实现方案

**依赖的框架层模块**:

```mermaid
graph TD
    subgraph Loading["UILoading"]
        UL[UILoadingView]
        UL2[UILoadingView2]
    end
    
    subgraph Framework["框架层依赖"]
        UI[UIManager<br/>窗口管理]
        Res[ResourcesManager<br/>资源加载]
        Scene[SceneManager<br/>场景管理]
        Timer[TimerManager<br/>延时控制]
    end
    
    UL --> UI
    UL --> Res
    UL --> Scene
    UL --> Timer
    
    style Loading fill:#fff3e0
    style Framework fill:#e3f2fd
```

**核心类图**:

```mermaid
classDiagram
    class UILoadingView {
        +static PrefabPath: string
        -progressBar: Slider
        -tipText: Text
        +SetProgress(value: float): void
        +SetTipText(key: I18NKey): void
        +OnCreate(): void
        +OnDestroy(): void
    }

    class UILoadingView2 {
        +static PrefabPath: string
        -animation: Animator
        +SetProgress(value: float): void
        +PlayAnimation(): void
    }

    class I18NKey {
        <<enumeration>>
        Loading_Tip_1
        Loading_Tip_2
        Loading_Tip_3
    }

    UILoadingView --> I18NKey
    UILoadingView2 --> I18NKey
    
    note for UILoadingView "⭐ 主加载界面<br/>进度条 + 提示"
    note for UILoadingView2 "🎨 带动画版本<br/>视觉效果更好"
```

---

#### 2.4 运行时工作流

**加载流程**:

```mermaid
sequenceDiagram
    participant SM as SceneManager
    participant Scene as HomeScene
    participant UI as UIManager
    participant UL as UILoadingView
    participant Res as ResourcesManager

    SM->>Scene: SwitchScene<HomeScene>
    Scene->>UI: OpenWindow<UILoadingView>
    UI->>UL: OnCreate()
    
    UL->>UL: SetProgress(0)
    UL->>UL: SetTipText(Loading_Tip_1)
    
    Scene->>Res: LoadAsync(资源 1)
    Res-->>Scene: 加载完成
    Scene->>UL: SetProgress(0.33)
    
    Scene->>Res: LoadAsync(资源 2)
    Res-->>Scene: 加载完成
    Scene->>UL: SetProgress(0.66)
    
    Scene->>Res: LoadAsync(资源 3)
    Res-->>Scene: 加载完成
    Scene->>UL: SetProgress(1.0)
    UL->>UL: SetTipText(Loading_Tip_3)
    
    Scene->>UI: CloseWindow(UILoadingView)
```

**进度计算逻辑**:

```csharp
// HomeScene.cs 中定义各阶段权重
public void GetProgressPercent(out float cleanup, out float loadScene, out float prepare)
{
    cleanup = 0.2f;      // 清理阶段占 20%
    loadScene = 0.65f;   // 加载场景占 65%
    prepare = 0.15f;     // 准备阶段占 15%
}

// 加载过程中更新进度
float currentProgress = loadedCount / totalCount * loadScene;
win.SetProgress(cleanup + currentProgress);
```

---

#### 2.5 配置与数据驱动

**可配置的数据**:

| 配置项 | 类型 | 位置 | 说明 |
|--------|------|------|------|
| 加载提示文字 | I18NKey | I18N 配置表 | 支持多语言 |
| 进度条权重 | float | 代码中 | 各阶段占比 |
| 最小加载时间 | int | 代码常量 | 避免闪屏 |

---

#### 2.6 与其他系统的协作

**协作关系**:

```mermaid
graph LR
    UL[UILoading] --> UI[UIManager]
    UL --> Res[ResourcesManager]
    UL --> Scene[SceneManager]
    UL --> I18N[I18NManager]
    
    style UL fill:#fff3e0
    style UI fill:#e3f2fd
    style Res fill:#e8f5e9
    style Scene fill:#fce4ec
    style I18N fill:#f3e5f5
```

---

*(由于篇幅限制，以下系统采用精简格式，完整文档已在 GitHub)*

---

## 阶段二：家园/大厅

### 3. HomeScene/UILobby - 家园与大厅

#### 3.1 系统概述

**玩家感知表现**:
- 登录成功后进入家园场景
- 看到 3D 家园环境（昼夜变化、光照效果）
- 大厅 UI 显示玩家信息、功能入口
- 可以查看任务、排行榜、设置等

**重要程度**: ⭐⭐⭐⭐ **核心系统**

#### 3.2 游戏设计意图

**体验贡献**:
- **归属感**: 家园场景给玩家"家"的感觉
- **功能聚合**: 大厅集中所有功能入口
- **视觉享受**: 精美场景和 UI 提升品质感

#### 3.3 技术实现方案

**核心类图**:

```mermaid
classDiagram
    class HomeScene {
        -envId: long
        -win: UILoadingView
        -blendView: UIBlendView
        +OnCreate(): ETTask
        +OnEnter(): ETTask
        +OnLeave(): ETTask
        +OnPrepare(): ETTask
        +GetDontDestroyWindow(): string[]
    }

    class UILobbyView {
        -topView: UITopView
        -taskList: TaskListItem[]
        +RefreshUI(): void
        +OnTaskClick(taskId): void
        +OnRankClick(): void
    }

    HomeScene --> UILoadingView
    HomeScene --> EnvironmentManager
    UILobbyView --> UITopView
    UILobbyView --> TaskListItem
```

#### 3.4 运行时工作流

**场景切换流程**:

```mermaid
sequenceDiagram
    participant PM as PlayerManager
    participant SM as SceneManager
    participant HS as HomeScene
    participant EM as EntityManager
    participant Env as EnvironmentManager
    participant UI as UIManager

    PM->>SM: SwitchScene<HomeScene>
    SM->>HS: OnCreate()
    HS->>HS: OnEnter()
    HS->>UI: OpenWindow<UILoadingView>
    
    HS->>EM: 创建实体管理器
    HS->>Env: Create(环境 ID)
    
    HS->>HS: OnPrepare(progress)
    HS->>UI: SetProgress(0→1)
    
    HS->>UI: OpenWindow<UILobbyView>
    HS->>UI: CloseWindow<UILoadingView>
    
    HS-->>SM: 场景加载完成
```

---

## 阶段三：拍卖玩法（核心）

### 5. AuctionSystem - 拍卖系统

#### 5.1 系统概述

**玩家感知表现**:
- 进入拍卖场景，看到多个 AI 竞拍者
- 拍卖师主持拍卖，倒计时叫价
- 玩家选择低/中/高价格叫价
- 开箱查看拍到的物品
- 可能触发小游戏或特殊事件

**重要程度**: ⭐⭐⭐⭐⭐ **核心玩法系统**

#### 5.2 游戏设计意图

**体验贡献**:
- **紧张刺激**: 倒计时叫价制造紧迫感
- **策略性**: 选择合适价格，观察 AI 行为
- **惊喜感**: 开箱随机物品，可能触发小玩法
- **成长感**: 赚钱解锁新关卡、新道具

**设计决策**:
| 决策 | 原因 | 替代方案 |
|------|------|----------|
| AI 竞拍者 | 营造竞争氛围，避免单机感 | 纯单机拍卖 |
| 三档叫价 | 简化操作，适合小游戏 | 自由输入价格 |
| 开箱机制 | 增加随机性和惊喜 | 直接获得物品 |
| 小玩法插入 | 丰富游戏体验 | 纯拍卖流程 |

#### 5.3 技术实现方案

**核心类图**:

```mermaid
classDiagram
    class AuctionManager {
        -AState: AuctionState
        -Stage: int
        -Bidders: List<long>
        -Player: Player
        -LastAuctionPrice: BigNumber
        +Init(MapScene): void
        +UserAuction(AITactic): void
        +SetState(AuctionState): void
        +RunNextStage(): void
    }

    class AuctionState {
        <<enumeration>>
        Free
        Prepare
        AIThink
        WaitUser
        OpenBox
        Over
        AllOver
    }

    class AITactic {
        <<enumeration>>
        None
        Low
        Medium
        High
        Follow
        Raise
    }

    class AuctionReport {
        +BoxId: int
        +FinalPrice: BigNumber
        +Items: ListItem[]
    }

    AuctionManager --> AuctionState
    AuctionManager --> AITactic
    AuctionManager --> AuctionReport
    
    note for AuctionManager "12 个分部类文件<br/>状态机驱动"
```

#### 5.4 运行时工作流

**完整拍卖流程**:

```mermaid
flowchart TD
    Start[进入拍卖场景] --> Prepare[准备阶段]
    Prepare --> EnterAnim[入场动画]
    EnterAnim --> Ready[准备就绪]
    
    Ready --> AIThink{AI 思考}
    AIThink --> WaitUser[等待玩家操作]
    
    WaitUser --> PlayerBid{玩家叫价}
    PlayerBid -->|低/中/高 | UpdatePrice[更新价格]
    PlayerBid -->|超时 | AutoPass[自动跳过]
    
    UpdatePrice --> CheckEnd{是否结束}
    CheckEnd -->|否 | AIThink
    CheckEnd -->|是 | ExitAnim[结算动画]
    
    ExitAnim --> OpenBox[玩家开箱]
    OpenBox --> MiniGame{小玩法？}
    MiniGame -->|是 | PlayMini[小游戏]
    MiniGame -->|否 | Over[本轮结算]
    
    Over --> NextRound{还有下轮？}
    NextRound -->|是 | Ready
    NextRound -->|否 | AllOver[游戏结束]
    
    AllOver --> Return[返回家园]
```

#### 5.5 配置与数据驱动

**核心配置表**:

| 配置表 | 用途 | 关键字段 |
|--------|------|----------|
| `StageConfig` | 关卡配置 | Level, Stage, Auction1/2/3, RaiseAuctionAddon |
| `LevelConfig` | 难度配置 | Id, Name, Difficulty |
| `AIConfig` | AI 行为 | Id, DecisionTree, Tactic, Delay |
| `ItemConfig` | 物品配置 | Id, Name, BasePrice, Rarity |
| `GameInfoConfig` | 情报配置 | Id, Effect, Description |
| `DiceConfig` | 骰子配置 | Id, Effect, Probability |

**策划调整示例**:

```csv
# StageConfig.csv
Level,Stage,Auction1,Auction2,Auction3,RaiseAuctionAddon
1,1,100,200,300,50
1,2,150,250,350,60
2,1,200,350,500,80
```

---

### 6. UIAuction - 拍卖 UI

#### 6.1 系统概述

**玩家感知表现**:
- 拍卖界面显示所有竞拍者
- 叫价按钮（低/中/高）
- 倒计时显示
- 开箱动画和结果展示

**重要程度**: ⭐⭐⭐⭐⭐ **核心 UI**

#### 6.2 UI 组件结构

```
UIAuction/
├── UIButtonView.cs         # 叫价按钮
├── UIAuctionItem.cs        # 竞拍者 item
├── UIReportWin.cs          # 结算窗口
├── UIDiceWin.cs            # 骰子选择窗口
├── UIGameInfoView.cs       # 情报界面
├── UIGuideGameView.cs      # 引导游戏界面
├── UIAssistantView.cs      # 助手提示
└── ... (共 19 个文件)
```

---

### 7. AIComponent - AI 竞拍者

#### 7.1 系统概述

**玩家感知表现**:
- AI 竞拍者与玩家一起叫价
- 每个 AI 有不同行为风格
- AI 可能离场、跟风、抬价

**重要程度**: ⭐⭐⭐⭐ **核心系统**

#### 7.2 AI 决策树

```mermaid
graph TD
    Start[AI 决策开始] --> CheckState{检查状态}
    CheckState --> CanBid{可以叫价？}
    CanBid -->|否 | Leave[离场]
    CanBid -->|是 | CheckPrice{检查价格}
    
    CheckPrice --> TooHigh{价格过高？}
    TooHigh -->|是 | Follow[跟风]
    TooHigh -->|否 | Bid[叫价]
    
    Bid --> SelectTactic{选择策略}
    SelectTactic --> Low[低价]
    SelectTactic --> Medium[中价]
    SelectTactic --> High[高价]
    SelectTactic --> Raise[抬价]
    
    Leave --> End[决策结束]
    Follow --> End
    Low --> End
    Medium --> End
    High --> End
    Raise --> End
```

---

## 阶段四：小玩法/互动

### 8. MiniGame - 小游戏

#### 8.1 系统概述

**玩家感知表现**:
- 开箱后可能触发小游戏
- 简单的互动玩法（转盘、鉴定等）
- 影响最终收益

**重要程度**: ⭐⭐⭐ **辅助玩法**

---

### 9. EnvironmentSystem - 环境系统

#### 9.1 系统概述

**玩家感知表现**:
- 家园场景有昼夜变化
- 光照随时间变化
- 天空盒切换（白天/夜晚/日出/日落）

**重要程度**: ⭐⭐ **装饰性系统**

---

## 阶段五：数值/成长

### 10. NumericSystem - 数值系统

#### 10.1 系统概述

**玩家感知表现**:
- 玩家属性显示（金钱、战力等）
- 数值加成效果
- 升级后数值提升

**重要程度**: ⭐⭐⭐⭐ **核心系统**

---

### 11. PlayerData - 玩家数据

#### 11.1 系统概述

**玩家感知表现**:
- 玩家信息（昵称、头像）
- 游戏进度保存
- 多设备同步

**重要程度**: ⭐⭐⭐⭐⭐ **核心系统**

---

## 阶段六：引导/帮助

### 12. GuidanceManager - 引导系统

#### 12.1 系统概述

**玩家感知表现**:
- 新手引导流程
- 高亮提示点击位置
- 引导步骤文字说明

**重要程度**: ⭐⭐⭐ **辅助系统**

---

### 13. GuideScene - 引导场景

#### 13.1 系统概述

**玩家感知表现**:
- 首次登录进入引导场景
- 简化版拍卖流程教学
- 完成后解锁正式玩法

**重要程度**: ⭐⭐⭐ **辅助系统**

---

## 阶段七：系统功能

### 14. RedDotManager - 红点系统

#### 14.1 系统概述

**玩家感知表现**:
- UI 图标上的红点提示
- 可领取奖励提示
- 新功能解锁提示

**重要程度**: ⭐⭐ **辅助系统**

---

### 15. I18NManager - 多语言

#### 15.1 系统概述

**玩家感知表现**:
- 支持多语言切换
- 文字自动翻译
- 本地化显示

**重要程度**: ⭐⭐ **辅助系统**

---

## 附录 A: 系统调用链总览

```mermaid
graph TB
    subgraph Entry["入口层"]
        Login[Login]
        Loading[UILoading]
    end
    
    subgraph Lobby["大厅层"]
        Home[HomeScene]
        Lobby[UILobby]
    end
    
    subgraph Game["玩法层"]
        Auction[AuctionSystem]
        UIAuc[UIAuction]
        AI[AIComponent]
        Mini[MiniGame]
    end
    
    subgraph Data["数据层"]
        PlayerData[PlayerData]
        Numeric[NumericSystem]
    end
    
    subgraph Support["支持层"]
        Guide[GuidanceManager]
        RedDot[RedDotManager]
        Env[EnvironmentSystem]
        I18N[I18NManager]
    end
    
    Entry --> Lobby
    Lobby --> Game
    Game --> Data
    Game --> Support
    
    style Entry fill:#ffebee
    style Lobby fill:#e3f2fd
    style Game fill:#e8f5e9
    style Data fill:#fff3e0
    style Support fill:#f3e5f5
```

---

## 附录 B: 配置表索引

| 配置表 | 用途 | 路径 |
|--------|------|------|
| StageConfig | 关卡配置 | Config/StageConfig |
| LevelConfig | 难度配置 | Config/LevelConfig |
| AIConfig | AI 行为 | Config/AIConfig |
| ItemConfig | 物品配置 | Config/ItemConfig |
| GameInfoConfig | 情报配置 | Config/GameInfoConfig |
| DiceConfig | 骰子配置 | Config/DiceConfig |
| GuidanceConfig | 引导配置 | Config/GuidanceConfig |
| I18NConfig | 多语言 | Config/I18NConfig |

---

*文档由 OpenClaw AI 助手自动生成 | 基于静态代码分析*
