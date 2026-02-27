# PlayerData.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | PlayerData.cs |
| **路径** | Assets/Scripts/Code/Module/Player/PlayerData.cs |
| **所属模块** | 框架层 → Code/Module/Player |
| **文件职责** | 玩家数据结构，存储玩家所有游戏数据 |

---

## 类/结构体说明

### PlayerData

| 属性 | 说明 |
|------|------|
| **职责** | 存储玩家所有游戏数据（金钱、任务、物品、成就等） |
| **泛型参数** | 无 |
| **继承关系** | 无 |
| **实现的接口** | 无 |

```csharp
public class PlayerData
{
    // 玩家数据结构
}
```

---

## 字段详解（按功能分类）

### 基础信息

| 字段 | 类型 | 说明 |
|------|------|------|
| `Platform` | `LoginPlatform` | 登录渠道（排行榜用） |
| `Version` | `long` | 数据版本号 |
| `Avatar` | `string` | 头像地址 |
| `NickName` | `string` | 用户昵称 |
| `Show` | `int[]` | 形象配置 |

### 货币与数值

| 字段 | 类型 | 说明 |
|------|------|------|
| `RankValue` | `string` | 金钱（字符串存储 BigNumber） |
| `Money` | `BigNumber` | 金钱（计算属性） |
| `WinToday` | `long` | 今日盈利 |
| `IsGotWinRewards` | `bool` | 是否领取今日盈利 |
| `ExpandTimes` | `int` | 扩容次数 |

### 任务系统

| 字段 | 类型 | 说明 |
|------|------|------|
| `OverTaskCount` | `Dictionary<int, int>` | 完成任务 ID 和次数 |
| `RunningTaskSteps` | `Dictionary<int, int>` | 进行中任务 ID 和进度 |
| `DailyTaskIds` | `List<int>` | 超市任务（每日任务） |
| `DailyRewards` | `HashSet<int>` | 每日任务进度条奖励 |

### 科技与解锁

| 字段 | 类型 | 说明 |
|------|------|------|
| `UnlockTechnologyTreeIds` | `HashSet<int>` | 已解锁科技树节点 ID |
| `UnlockList` | `List<int>` | 解锁黑市后必出物品 |
| `OverGuide` | `List<int>` | 已完成的引导 |

### 拍卖相关

| 字段 | 类型 | 说明 |
|------|------|------|
| `PlayCount` | `Dictionary<int, int>` | 各关卡拍卖场数 |
| `LastLevelId` | `int` | 上一次选关 ID |

### 广告相关

| 字段 | 类型 | 说明 |
|------|------|------|
| `LastAdTime` | `long` | 上次看广告时间 |
| `AdCountTotal` | `int` | 上次看广告次数 |
| `PlayableCount` | `int` | 今日玩法看广告次数 |
| `TurnTableCount` | `int` | 今日大转盘次数 |

### 餐厅系统

| 字段 | 类型 | 说明 |
|------|------|------|
| `RestaurantLv` | `int` | 餐厅等级 |
| `WashDishAuto` | `bool` | 是否解锁自动洗盘子 |
| `LastReceiveRestaurantTime` | `long` | 上一次收取餐厅奖励时间 |

### 物品系统

| 字段 | 类型 | 说明 |
|------|------|------|
| `ItemCount` | `Dictionary<int, int>` | 物品数量 |
| `ClothsSale` | `List<int>` | 服装店衣服 |

### 时间相关

| 字段 | 类型 | 说明 |
|------|------|------|
| `LastSidebarRewards` | `long` | 上次领取侧边栏奖励时间 |
| `LastRefreshDailyTime` | `long` | 上一次刷新时间 |

### 引导相关

| 字段 | 类型 | 说明 |
|------|------|------|
| `IsGuideScene` | `bool` | 是否引导过 |

---

## 计算属性

### Money

**签名**:
```csharp
public BigNumber Money { get; set; }
```

**职责**: 金钱的 BigNumber 封装

**实现**:
```csharp
get => RankValue;
set => RankValue = value;
```

**用途**: 将字符串转换为 BigNumber 进行计算

---

## 阅读指引

### 建议的阅读顺序

1. **理解数据结构** - PlayerData 存储什么
2. **看基础信息** - 玩家基本信息
3. **看货币数值** - 金钱和盈利
4. **看任务系统** - 任务进度管理

### 最值得学习的技术点

1. **BigNumber 存储**: 使用字符串存储大数值
2. **字典管理**: Dictionary 管理任务、物品等
3. **HashSet 去重**: HashSet 管理已解锁内容
4. **时间戳**: long 存储时间戳

---

## 使用示例

### 示例 1: 创建玩家数据

```csharp
PlayerData playerData = new PlayerData();
playerData.NickName = "玩家 1";
playerData.Money = new BigNumber(1000);
playerData.RestaurantLv = 1;
```

### 示例 2: 访问金钱

```csharp
// 获取金钱
BigNumber money = playerData.Money;
Log.Info($"玩家金钱：{money}");

// 设置金钱
playerData.Money = new BigNumber(5000);
```

### 示例 3: 任务管理

```csharp
// 添加完成任务
if (!playerData.OverTaskCount.ContainsKey(taskId))
{
    playerData.OverTaskCount[taskId] = 0;
}
playerData.OverTaskCount[taskId]++;

// 检查任务进度
if (playerData.RunningTaskSteps.TryGetValue(taskId, out int progress))
{
    Log.Info($"任务{taskId}进度：{progress}");
}
```

### 示例 4: 物品管理

```csharp
// 添加物品
if (!playerData.ItemCount.ContainsKey(itemId))
{
    playerData.ItemCount[itemId] = 0;
}
playerData.ItemCount[itemId] += count;

// 检查物品数量
if (playerData.ItemCount.TryGetValue(itemId, out int count))
{
    Log.Info($"物品{itemId}数量：{count}");
}
```

---

## 相关文档

- [PlayerManager.cs.md](./PlayerManager.cs.md) - 玩家管理器
- [PlayerDataManager.cs.md](./PlayerDataManager.cs.md) - 玩家数据管理器
- [CacheManager.cs.md](./CacheManager.cs.md) - 缓存管理器

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
