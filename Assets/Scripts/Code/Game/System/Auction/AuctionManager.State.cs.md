# AuctionManager.State.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | AuctionManager.State.cs |
| **路径** | Assets/Scripts/Code/Game/System/Auction/AuctionManager.State.cs |
| **所属模块** | 玩法层 → Code/Game/System/Auction |
| **文件职责** | 拍卖状态机实现，处理各状态的具体逻辑 |

---

## 文件说明

本文件是 `AuctionManager` 的 **Partial Class** 扩展，专注于状态机实现。

### 状态列表

| 状态 | 说明 | 主要工作 |
|------|------|----------|
| `Awake` | 初始化 | 创建拍卖师、玩家、AI 竞拍者 |
| `Prepare` | 准备 | 生成集装箱、计算价格区间 |
| `EnterAnim` | 入场动画 | 播放 Timeline 动画 |
| `Ready` | 准备就绪 | 等待开始叫价 |
| `AIThink` | AI 思考 | AI 决策并叫价 |
| `WaitUser` | 等待玩家 | 显示叫价按钮，倒计时 |
| `ExitAnim` | 结束动画 | 播放结算动画 |
| `OpenBox` | 开箱 | 玩家查看物品 |
| `Over` | 结算 | 计算收益，更新金钱 |
| `ReEnterAnim` | 再次入场 | 下一轮入场动画 |
| `AllOverAnim` | 全部结束动画 | 总结算动画 |
| `AllOver` | 全部结束 | 显示最终结果 |
| `RePrepare` | 再来一局 | 准备重新开始 |

---

## 核心方法

### Update()

**签名**:
```csharp
public void Update()
```

**职责**: 状态机主循环（每帧调用）

**核心逻辑**:
```csharp
public void Update()
{
    if (isDispose) return;
    
    var lAState = AState;
    switch (AState)
    {
        case AuctionState.Awake: Awake(); break;
        case AuctionState.Prepare: Prepare(); break;
        case AuctionState.EnterAnim: PlayEnterAnim().Coroutine(); break;
        case AuctionState.Ready: Ready(); break;
        case AuctionState.AIThink: AIThink(); break;
        case AuctionState.WaitUser: WaitUser(); break;
        case AuctionState.ExitAnim: ExitAnim(); break;
        case AuctionState.OpenBox: OpenBox(); break;
        case AuctionState.Over: Over(); break;
        case AuctionState.ReEnterAnim: ReEnterAnim(); break;
        case AuctionState.AllOverAnim: AllOverAnim(); break;
        case AuctionState.AllOver: AllOver(); break;
        case AuctionState.RePrepare: RePrepare(); break;
    }
    
    // 如果状态未变化，清除 isEnterState 标志
    if (AState == lAState) isEnterState = false;
}
```

**调用者**: `ManagerProvider.Update()`（每帧）

---

### Awake()

**签名**:
```csharp
private void Awake()
```

**职责**: 第一次进入拍卖前初始化

**核心逻辑**:
```
1. 设置初始值（Stage=0, DiceId=0）
2. 生成情报 CreateGameInfo()
3. 如果选择骰子，打开骰子选择 UI
4. 创建拍卖师实体（Host）
5. 创建玩家实体（Player）
6. 创建 AI 竞拍者实体（Bidder）
7. 创建背景 NPC（高性能设备）
8. 初始化 AI 决策数组
9. 调用 WaitPrepare() 等待准备
```

**isEnterState 检查**:
```csharp
if (!isEnterState) return;  // 只执行一次
```

---

### Prepare()

**签名**:
```csharp
private void Prepare()
```

**职责**: 第一次进入准备阶段

**核心逻辑**:
```
1. 检查 isEnterState
2. 开始游戏录制 GameRecorderManager.StartRecorder()
3. 生成集装箱 CreateContainer()
4. 生成物品 CreateItems()
5. 计算价格区间 CreateRangePrice()
6. 等待玩家准备
7. 切换到 EnterAnim
```

---

### PlayEnterAnim()

**签名**:
```csharp
private async ETTask PlayEnterAnim()
```

**职责**: 播放入场动画

**核心逻辑**:
```
1. Stage++
2. 生成物品 CreateItems()
3. 打开助手 UI UIAssistantView
4. 设置高性能模式 PerformanceManager.SetHighFrame()
5. 获取 Timeline 导演 PlayableDirector
6. 绑定所有实体到 Timeline Track
7. 播放动画并等待完成
8. 恢复实体位置
9. 切换到 Ready 状态
10. 恢复低性能模式
```

**Timeline 绑定**:
```csharp
// 绑定拍卖师
dir.SetGenericBinding(playableBinding.sourceObject,
    hosthc.EntityView.GetComponent(playableBinding.outputTargetType));

// 绑定玩家
dir.SetGenericBinding(playableBinding.sourceObject,
    phc.EntityView.GetComponent(playableBinding.outputTargetType));

// 绑定竞拍者
dir.SetGenericBinding(playableBinding.sourceObject,
    ghc.EntityView.GetComponent(playableBinding.outputTargetType));
```

---

### AIThink()

**签名**:
```csharp
private void AIThink()
```

**职责**: AI 思考并叫价

**核心逻辑**:
```
1. 遍历所有 AI 竞拍者
2. 对每个 AI：
   - 检查是否已决策
   - 调用 AIDecision.Decide() 决策
   - 根据决策叫价或离场
3. 检查是否所有 AI 完成
4. 如果完成，切换到 WaitUser
```

---

### WaitUser()

**签名**:
```csharp
private void WaitUser()
```

**职责**: 等待玩家操作

**核心逻辑**:
```
1. 检查 isEnterState（首帧初始化）
   - 显示叫价按钮
   - 启动倒计时
   - 拍卖师倒计时语音
2. 检查倒计时
3. 如果超时，自动跳过
4. 如果玩家操作，等待动画完成
```

---

### ExitAnim()

**签名**:
```csharp
private void ExitAnim()
```

**职责**: 当前轮结束动画

**核心逻辑**:
```
1. 播放结算动画
2. 展示最终叫价
3. 恭喜中标者
4. 切换到 OpenBox
```

---

### OpenBox()

**签名**:
```csharp
private void OpenBox()
```

**职责**: 玩家开箱

**核心逻辑**:
```
1. 同步物理 Physics.SyncTransforms()
2. 显示集装箱
3. 播放开箱动画
4. 展示物品列表
5. 检查小玩法物品
6. 如果有小玩法，进入小玩法流程
7. 如果没有，直接切换到 Over
```

---

### Over()

**签名**:
```csharp
private void Over()
```

**职责**: 当前轮结算

**核心逻辑**:
```
1. 销毁掉落物品
2. 计算收益
3. 更新玩家金钱
4. 显示结算界面
5. 检查是否还有下一轮
6. 如果有，调用 RunNextStage()
7. 如果没有，切换到 AllOverAnim
```

---

## 阅读指引

### 建议的阅读顺序

1. **看 Update 方法** - 理解状态机主循环
2. **看 Awake** - 理解初始化流程
3. **看 PlayEnterAnim** - 理解动画播放
4. **看 AIThink/WaitUser** - 理解核心玩法循环
5. **看 OpenBox/Over** - 理解结算流程

### 最值得学习的技术点

1. **isEnterState 标志**: 区分状态首帧和后续帧
2. **Timeline 绑定**: PlayableDirector 绑定实体到动画
3. **性能管理**: 动画时高性能，平时低性能
4. **状态流转**: 清晰的状态切换逻辑

---

## 相关文档

- [AuctionManager.cs.md](./AuctionManager.cs.md) - 拍卖管理器核心
- [AuctionState.cs.md](./AuctionState.cs.md) - 状态枚举
- [AuctionManager.Anim.cs.md](./AuctionManager.Anim.cs.md) - 动画控制
- [AuctionManager.API.cs.md](./AuctionManager.API.cs.md) - API 接口

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
