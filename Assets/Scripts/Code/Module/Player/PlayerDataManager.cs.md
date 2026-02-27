# PlayerDataManager.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | PlayerDataManager.cs |
| **路径** | Assets/Scripts/Code/Module/Player/PlayerDataManager.cs |
| **所属模块** | 框架层 → Code/Module/Player |
| **文件职责** | 玩家数据管理，负责数据初始化、存储、同步、每日刷新等 |

---

## 类/结构体说明

### PlayerDataManager

| 属性 | 说明 |
|------|------|
| **职责** | 管理玩家所有数据，包括金钱、等级、任务、物品、时装等 |
| **泛型参数** | 无 |
| **继承关系** | 无继承 |
| **实现的接口** | `IManager`, `ILateUpdate` |

**设计模式**: 单例模式 + 数据驱动

```csharp
// 单例实现
public static PlayerDataManager Instance { get; private set; }

// 通过 ManagerProvider 注册
ManagerProvider.RegisterManager<PlayerDataManager>();
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `Instance` | `PlayerDataManager` | `public static` | 单例实例 |
| `data` | `PlayerData` | `private` | 玩家数据对象 |
| `TotalMoney` | `BigNumber` | `public` | 总金钱 |
| `RestaurantLv` | `int` | `public` | 餐厅等级 |
| `Show` | `int[]` | `public` | 展示的角色时装 |
| `IsGuideScene` | `bool` | `public` | 是否在引导场景 |
| `LastLevelId` | `int` | `public` | 上次玩的关卡 ID |
| `Avatar` | `string` | `public` | 头像 URL |
| `NickName` | `string` | `public` | 昵称 |
| `WashDishAuto` | `bool` | `public` | 是否自动刷盘子 |
| `ProfitUpdateUnitTime` | `int` | `public` | 利润更新间隔 (ms) |
| `DailyRefreshHour` | `int` | `public` | 每日刷新时间 (小时) |

---

## 方法说明（按重要程度排序）

### Init()

**签名**:
```csharp
public void Init()
```

**职责**: 初始化玩家数据管理器

**核心逻辑**:
```
1. 设置单例 Instance = this
2. 从 GlobalConfigCategory 读取配置：
   - MaxAdRewardsCount: 最大广告奖励次数
   - AdRewardsResetTime: 广告奖励重置时间
   - ProfitRedDotPercent: 利润红点百分比
   - ProfitUpdateUnitTime: 利润更新间隔
   - DailyRefreshHour: 每日刷新时间
   - ClothRefreshCount: 每日刷新时装数量
3. 读取颜色配置
4. 读取概率数组配置（转盘、广告等）
```

**调用者**: ManagerProvider.RegisterManager<PlayerDataManager>()

---

### AfterLogin(PlayerData sdata, string nick, string avatar, LoginPlatform platform)

**签名**:
```csharp
public void AfterLogin(PlayerData sdata, string nick, string avatar, LoginPlatform platform)
```

**职责**: 登录后初始化玩家数据

**核心逻辑**:
```
1. 保存玩家数据 data = sdata
2. 如果 data 为 null，创建新 PlayerData
3. 设置平台、昵称、头像
4. 初始化所有集合字段（解锁列表、任务、物品等）
5. 初始化角色展示数组
6. 调用 DailyRefresh() 每日刷新
7. 刷新红点 RefreshRedDot()
8. 注册事件监听
9. 检查引导状态
```

**调用者**: PlayerManager.Login()

---

### DailyRefresh()

**签名**:
```csharp
public bool DailyRefresh()
```

**职责**: 每日刷新逻辑

**核心逻辑**:
```
1. 检查是否需要刷新（距离上次刷新超过 24 小时）
2. 刷新每日任务
3. 刷新每日奖励
4. 刷新免费广告次数
5. 刷新转盘次数
6. 刷新时装商店
7. 更新 LastRefreshDailyTime
8. 返回是否发生变化
```

**调用者**: AfterLogin(), DailyRefreshTimer

---

### SaveData()

**签名**:
```csharp
private void SaveData()
```

**职责**: 保存玩家数据

**核心逻辑**:
```
1. 更新 Version = TimerManager.Instance.GetTimeNow()
2. 设置 needUpload = true（延迟上传）
3. 缓存到本地 CacheManager.Instance.SetValue()
```

**调用者**: 数据变更时

---

### LateUpdate()

**签名**:
```csharp
public void LateUpdate()
```

**职责**: 每帧延迟更新，上传数据到服务器

**核心逻辑**:
```
1. 检查 needUpload 标志
2. 如果在线，调用 APIManager.SaveData()
3. 重置 needUpload = false
```

**调用者**: ManagerProvider.LateUpdate()

---

### UpdateUserInfo(string nick, string avatar)

**签名**:
```csharp
public void UpdateUserInfo(string nick, string avatar)
```

**职责**: 更新用户信息

**核心逻辑**:
```
1. 检查昵称/头像是否变化
2. 更新 data.NickName / data.Avatar
3. 如果有变化，调用 SaveData()
```

**调用者**: SDKManager.Auth()

---

### GetItemCount(int itemId)

**签名**:
```csharp
public int GetItemCount(int itemId)
```

**职责**: 获取物品数量

**返回**: 物品数量

**使用示例**:
```csharp
// 检查是否有骰子
if (PlayerDataManager.Instance.GetItemCount(GameConst.DiceItemId) > 0)
{
    // 可以使用骰子
}
```

---

### AddMoney(BigNumber amount)

**签名**:
```csharp
public void AddMoney(BigNumber amount)
```

**职责**: 增加金钱

**核心逻辑**:
```
1. data.Money += amount
2. 刷新红点
3. SaveData()
```

**调用者**: 拍卖结算、任务奖励等

---

## 每日刷新定时器

### DailyRefreshTimer

```csharp
[Timer(TimerType.DailyRefresh)]
public class DailyRefreshTimer : ATimer<PlayerDataManager>
{
    public override void Run(PlayerDataManager self)
    {
        try
        {
            bool change = false;
            if (self.DailyRefresh())
            {
                change = true;
            }
            if (change)
            {
                self.SaveData();
            }
        }
        catch (Exception e)
        {
            Log.Error($"move timer error: DailyRefresh\n{e}");
        }
    }
}
```

**触发时机**: 每日刷新时间（默认 6:00）

---

## 阅读指引

### 建议的阅读顺序

1. **理解数据管理器作用** - 为什么需要 PlayerDataManager
2. **看 AfterLogin** - 理解数据初始化流程
3. **看 DailyRefresh** - 理解每日刷新逻辑
4. **看 SaveData/LateUpdate** - 理解数据保存机制

### 最值得学习的技术点

1. **数据驱动**: 所有游戏数据集中在 PlayerData
2. **延迟上传**: LateUpdate 批量上传，减少 API 调用
3. **每日刷新**: 定时器自动刷新每日任务/奖励
4. **版本控制**: Version 字段解决数据冲突
5. **本地缓存**: CacheManager 离线模式支持

---

## 数据结构

### PlayerData 主要字段

```csharp
public class PlayerData
{
    // 基础信息
    public LoginPlatform Platform;
    public long Version;
    public bool IsGuideScene;
    public string Avatar;
    public string NickName;
    
    // 资源
    public BigNumber Money;
    public int RestaurantLv;
    
    // 进度
    public int LastLevelId;
    public HashSet<int> UnlockTechnologyTreeIds;
    public List<int> UnlockList;
    
    // 任务
    public Dictionary<int, int> RunningTaskSteps;
    public Dictionary<int, int> OverTaskCount;
    public List<int> DailyTaskIds;
    public HashSet<int> DailyRewards;
    
    // 物品
    public Dictionary<int, int> ItemCount;
    public Dictionary<int, int> PlayCount;
    
    // 角色
    public int[] Show;  // 展示的角色时装
    public List<int> OverGuide;  // 已完成的引导
}
```

---

## 使用示例

### 示例 1: 获取玩家信息

```csharp
// 获取昵称和头像
string nick = PlayerDataManager.Instance.NickName;
string avatar = PlayerDataManager.Instance.Avatar;

// 获取金钱
BigNumber money = PlayerDataManager.Instance.TotalMoney;

// 获取餐厅等级
int level = PlayerDataManager.Instance.RestaurantLv;
```

### 示例 2: 检查物品

```csharp
// 检查是否有骰子
if (PlayerDataManager.Instance.GetItemCount(GameConst.DiceItemId) > 0)
{
    // 可以使用骰子
    UseDice();
}

// 检查时装数量
int clothCount = PlayerDataManager.Instance.GetItemCount(clothId);
```

### 示例 3: 增加金钱

```csharp
// 拍卖结算
BigNumber reward = auctionReport.FinalPrice;
PlayerDataManager.Instance.AddMoney(reward);

// 任务奖励
BigNumber taskReward = taskConfig.Reward;
PlayerDataManager.Instance.AddMoney(taskReward);
```

---

## 相关文档

- [PlayerManager.cs.md](./PlayerManager.cs.md) - 玩家管理器
- [PlayerData.cs.md](./PlayerData.cs.md) - 玩家数据结构
- [CacheManager.cs.md](./CacheManager.cs.md) - 缓存管理
- [APIManager.cs.md](../Net/APIManager.cs.md) - 网络 API

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
