# AuctionManager.API.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | AuctionManager.API.cs |
| **路径** | Assets/Scripts/Code/Game/System/Auction/AuctionManager.API.cs |
| **所属模块** | 玩法层 → Code/Game/System/Auction |
| **文件职责** | 拍卖系统对外 API 接口实现 |

---

## 文件说明

本文件是 `AuctionManager` 的 **Partial Class** 扩展，实现 `IAuctionManager` 接口定义的方法。

---

## 核心方法

### ForceAllOver()

**签名**:
```csharp
public void ForceAllOver()
```

**职责**: 强行退出拍卖（玩家中途离开）

**核心逻辑**:
```
1. 设置 isDispose = true
2. 切换到家园场景 SceneManager.SwitchScene<HomeScene>()
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
2. 检查叫价类型是否有效
3. 检查金钱是否足够
4. 根据 type 更新 LastAuctionPrice：
   - Low → LowAuction
   - Medium → MediumAuction
   - High → HighAuction
5. 更新统计计数
6. 取消当前定时器
7. 显示气泡对话框
8. 调用 AfterAuction() 处理后续
9. 切换到 AIThink 状态
```

**调用者**: UI 按钮点击事件

**使用示例**:
```csharp
// 玩家选择低价叫价
AuctionManager.Instance.UserAuction(AITactic.Low);

// 玩家选择中价叫价
AuctionManager.Instance.UserAuction(AITactic.Medium);

// 玩家选择高价叫价
AuctionManager.Instance.UserAuction(AITactic.High);
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
1. 处理 UFO（如果有）：
   - 随机选择是否抓走玩家
   - 或随机抓走一个竞拍者
2. 如果玩家中标：
   - 完成任务计数
3. 移除解锁列表
4. 刷新输赢动画
5. 检查是否还有下一轮：
   - 有 → ReEnterAnim
   - 无 → AllOverAnim
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

**核心逻辑**:
```
1. 遍历所有箱子
2. 找到匹配的物品 ID
3. 设置鉴定结果 box.SetAppraisalResult(newId)
4. 更新 Report.PlayData
5. 刷新价格显示
6. 刷新输赢动画
```

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

**核心逻辑**:
```
1. 遍历所有箱子
2. 找到匹配的物品 ID
3. 设置小游戏结果 box.SetMiniGameResult(newPrice)
4. 更新 Report.PlayData
5. 刷新价格显示
6. 刷新输赢动画
```

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

**返回**: 应用后的情报配置，如果不满足条件返回 null

**核心逻辑**:
```
1. 检查是否是玩家中标
2. 检查情报 ID 是否有效
3. 检查情报条件：
   - MinRaiseCount: 抬价次数 >= 要求
   - MaxAuctionCount: 叫价次数 <= 要求
   - 其他条件...
4. 如果满足条件，返回 GameInfoConfig
5. 否则返回 null
```

**调用者**: 结算逻辑

---

### SelectGameInfo(int id)

**签名**:
```csharp
public void SelectGameInfo(int id)
```

**职责**: 选择情报

**参数**:
- `id`: 情报 ID

**核心逻辑**:
```
1. 设置 GameInfoId = id
2. 关闭情报选择 UI
3. 设置 UserReady = true
```

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

**核心逻辑**:
```
1. 设置 DiceId = id
2. 关闭骰子选择 UI
3. 调用回调 onSelectOver()
```

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

**核心逻辑**:
```
1. 遍历所有箱子
2. 根据中标情况设置动画：
   - 玩家中标 → 赢动画
   - 他人中标 → 输动画
   - 流拍 → 无动画
3. 如果 play=true，播放动画
```

**调用者**: 结算逻辑

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

**核心逻辑**:
```
1. 获取竞拍者实体
2. 播放离场动画
3. 从 Bidders 列表移除
4. 添加到 Blacks（如果是黑市商人）
```

**调用者**: AI 决策逻辑

---

### GetLevelCount()

**签名**:
```csharp
public int GetLevelCount()
```

**职责**: 获取离场人数

**返回**: 已离场的 AI 数量

**核心逻辑**:
```
1. 返回 Blacks.Count
```

**调用者**: 统计逻辑

---

## 阅读指引

### 建议的阅读顺序

1. **看 UserAuction** - 理解玩家叫价
2. **看 RunNextStage** - 理解轮次切换
3. **看 SetAppraisalResult/SetMiniGameResult** - 理解小玩法
4. **看 GetFinalGameInfoConfig** - 理解情报系统

### 最值得学习的技术点

1. **接口实现**: Partial Class 实现 IAuctionManager 接口
2. **参数验证**: 检查状态、金钱等条件
3. **小玩法集成**: 鉴定/小游戏结果处理
4. **条件判断**: 情报条件检查

---

## 相关文档

- [AuctionManager.cs.md](./AuctionManager.cs.md) - 拍卖管理器核心
- [AuctionManager.State.cs.md](./AuctionManager.State.cs.md) - 状态机实现
- [AuctionManager.Anim.cs.md](./AuctionManager.Anim.cs.md) - 动画控制
- [IAuctionManager.cs.md](./IAuctionManager.cs.md) - 接口定义

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
