# AuctionManager.Anim.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | AuctionManager.Anim.cs |
| **路径** | Assets/Scripts/Code/Game/System/Auction/AuctionManager.Anim.cs |
| **所属模块** | 玩法层 → Code/Game/System/Auction |
| **文件职责** | 拍卖动画控制，包括入场、结算、特效等 |

---

## 文件说明

本文件是 `AuctionManager` 的 **Partial Class** 扩展，专注于动画控制。

### 动画类型

| 动画 | 说明 | 触发时机 |
|------|------|----------|
| `PlayEnterAnim` | 入场动画 | 每轮开始 |
| `ExitAnim` | 结算动画 | 叫价结束 |
| `ReEnterAnim` | 再次入场 | 下一轮开始 |
| `AllOverAnim` | 总结算动画 | 所有轮结束 |
| `UFOPickPlayer` | UFO 抓人动画 | 随机触发 |
| `UFOPickUp` | UFO 抓取动画 | 随机触发 |

---

## 核心方法

### WaitPrepare()

**签名**:
```csharp
private async ETTask WaitPrepare()
```

**职责**: 等待所有实体加载完成，准备开始

**核心逻辑**:
```
1. 播放开场动画（如果有）
2. 等待所有实体 GameObject 加载完成：
   - 拍卖师
   - 玩家
   - 所有竞拍者
3. 显示玩家旗帜
4. 等待玩家准备（IAuctionManager.UserReady）
5. 切换到 Prepare 状态
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
3. 打开助手 UI
4. 设置高性能模式
5. 获取 PlayableDirector
6. 绑定所有实体到 Timeline Track：
   - HostTrack → 拍卖师
   - PlayerRootName + "Track" → 玩家
   - BidderRootName + "Track" → 竞拍者
   - NPCRootName + "Track" → NPC
7. 播放动画并等待完成
8. 恢复实体位置和旋转
9. 恢复父节点到 EntityRoot
10. 切换到 Ready 状态
11. 恢复低性能模式
```

**Timeline 绑定示例**:
```csharp
// 绑定拍卖师
if (bindings.TryGetValue("HostTrack", out var playableBinding))
{
    dir.SetGenericBinding(playableBinding.sourceObject,
        hosthc.EntityView.GetComponent(playableBinding.outputTargetType));
}

// 绑定玩家
if (bindings.TryGetValue(Player.RootName + "Track", out playableBinding))
{
    dir.SetGenericBinding(playableBinding.sourceObject,
        phc.EntityView.GetComponent(playableBinding.outputTargetType));
}
```

---

### ExitAnim()

**签名**:
```csharp
private async ETTask ExitAnim()
```

**职责**: 播放结算动画

**核心逻辑**:
```
1. 处理 UFO（如果有）
2. 播放音效：
   - 中标：countDownWin.mp3
   - 流拍：giveup.mp3
3. 更新报告数据
4. 刷新价格显示
5. 如果不是玩家中标：
   - 设置 Report.Type
   - 处理抬价奖励
6. 播放输赢动画
7. 等待动画完成
8. 切换到 OpenBox
```

---

### ReEnterAnim()

**签名**:
```csharp
private async ETTask ReEnterAnim()
```

**职责**: 再次入场动画（下一轮）

**核心逻辑**:
```
1. 设置高性能模式
2. 播放 Timeline 动画
3. 等待完成
4. 恢复实体位置
5. 切换到 Ready 状态
6. 恢复低性能模式
```

---

### AllOverAnim()

**签名**:
```csharp
private async ETTask AllOverAnim()
```

**职责**: 所有轮结束动画

**核心逻辑**:
```
1. 播放总结算动画
2. 展示总收益
3. 播放成就动画
4. 切换到 AllOver 状态
```

---

### UFOPickPlayer()

**签名**:
```csharp
private async ETTask UFOPickPlayer()
```

**职责**: UFO 抓走玩家动画（随机触发）

**核心逻辑**:
```
1. 创建 UFO 实体
2. 播放 UFO 飞入动画
3. 抓取玩家
4. 播放玩家被带走动画
5. UFO 飞走
6. 切换到 OpenBox
```

---

### UFOPickUp(long id)

**签名**:
```csharp
private async ETTask UFOPickUp(long id)
```

**职责**: UFO 抓走竞拍者动画

**参数**:
- `id`: 被抓的竞拍者 ID

**核心逻辑**:
```
1. 创建 UFO 实体
2. 播放 UFO 飞入动画
3. 抓取指定竞拍者
4. 播放竞拍者被带走动画
5. UFO 飞走
6. 切换到 OpenBox
```

---

## 阅读指引

### 建议的阅读顺序

1. **看 WaitPrepare** - 理解准备流程
2. **看 PlayEnterAnim** - 理解入场动画
3. **看 ExitAnim** - 理解结算动画
4. **看 UFOPickPlayer/UFOPickUp** - 理解特殊动画

### 最值得学习的技术点

1. **Timeline 绑定**: PlayableDirector 动态绑定实体
2. **性能管理**: 动画时高性能，平时低性能
3. **异步动画**: ETTask 等待动画完成
4. **实体位置恢复**: 动画后恢复实体 Transform

---

## 相关文档

- [AuctionManager.cs.md](./AuctionManager.cs.md) - 拍卖管理器核心
- [AuctionManager.State.cs.md](./AuctionManager.State.cs.md) - 状态机实现
- [AuctionManager.API.cs.md](./AuctionManager.API.cs.md) - API 接口

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
