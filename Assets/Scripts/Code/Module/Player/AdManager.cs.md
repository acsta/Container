# AdManager.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | AdManager.cs |
| **路径** | Assets/Scripts/Code/Module/Player/AdManager.cs |
| **所属模块** | 框架层 → Code/Module/Player |
| **文件职责** | 广告管理器，支持多平台激励视频广告 |

---

## 类/结构体说明

### AdManager

| 属性 | 说明 |
|------|------|
| **职责** | 管理激励视频广告，支持抖音/微信/快手/ TapTap/ 支付宝/Facebook 等平台 |
| **泛型参数** | 无 |
| **继承关系** | 无 |
| **实现的接口** | `IManager` |

```csharp
public class AdManager : IManager
{
    // 广告管理器
}
```

---

## 字段与属性

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `Instance` | `AdManager` | `public static` | 单例实例 |
| `NONE_AD_LINK` | `bool` | `public static` | 无广链接标志 |
| `State` | `AdState` | `private` | 广告状态 |
| `isLoaded` | `bool` | `private` | 是否已加载 |
| `ad` | `TTRewardedVideoAd/WXRewardedVideoAd/...` | `private` | 平台广告实例 |

---

## 广告状态（AdState）

| 状态 | 说明 |
|------|------|
| `Loading` | 加载中 |
| `Loaded` | 已加载 |
| `Playing` | 播放中 |
| `Success` | 播放成功 |
| `Fail` | 播放失败 |

---

## 方法说明（按重要程度排序）

### Init() / Destroy()

**签名**:
```csharp
public void Init()
public void Destroy()
```

**职责**: 初始化和销毁

**核心逻辑**:
```
// Init:
1. 设置 Instance = this
2. 如果是支付宝平台，获取广告管理器
3. 调用 GetNewAd() 预加载广告

// Destroy:
1. 清空 Instance
2. 销毁广告实例（如果存在）
```

**调用者**: `ManagerProvider.Init()`

---

### PlatformHasAD()

**签名**:
```csharp
public bool PlatformHasAD()
```

**职责**: 检查当前平台是否支持广告

**返回**: true=支持，false=不支持

**核心逻辑**:
```
1. 根据平台判断：
   - 抖音/快手/TapTap/ 轻游/编辑器/4399/Facebook → 返回 true
   - 微信 → 检查 IsEligibility()（SDK 版本）
   - 其他 → 返回 NONE_AD_LINK
```

**微信 SDK 版本检查**:
```csharp
private bool IsEligibility()
{
    var vs = WeChatWASM.WX.GetSystemInfoSync().SDKVersion.Split(".");
    var baseVersion = new List<int>() { 2, 0, 4 };
    // 比较版本号
    return data.CompareTo(baseVersion[i]) > 0;
}
```

**调用者**: 需要检查广告支持的代码

---

### PlayAd()

**签名**:
```csharp
public async ETTask<bool> PlayAd()
```

**职责**: 播放广告

**返回**: true=播放成功，false=播放失败

**核心逻辑**:
```
1. 等待广告加载完成（State == Loaded）
2. 如果已加载：
   - 设置 State = Playing
   - 调用 ad.Show()
3. 等待播放完成（State != Playing）
4. 检查结果（State == Success）
5. 预加载下一个广告 GetNewAd()
6. 返回结果
```

**等待加载**:
```csharp
while (State != AdState.Loaded)
{
    await TimerManager.Instance.WaitAsync(1);
}
```

**播放流程**:
```csharp
if (State == AdState.Loaded)
{
    State = AdState.Playing;
    ad.Show();
}

// 等待播放完成
while (State == AdState.Playing)
{
    await TimerManager.Instance.WaitAsync(1);
}

var res = State == AdState.Success;
GetNewAd().Coroutine();  // 预加载下一个
return res;
```

**调用者**: 需要播放广告的场景

---

### GetNewAd()

**签名**:
```csharp
private async ETTask GetNewAd()
```

**职责**: 获取/预加载新广告

**核心逻辑**:
```
1. 检查是否已加载（避免重复加载）
2. 设置 State = Loading
3. 根据平台创建广告实例：
   - 抖音：TTSDK.TT.CreateRewardedVideoAd()
   - 微信：WeChatWASM.WX.CreateRewardedVideoAd()
   - 快手：KSWASM.KS.CreateRewardedVideoAd()
   - TapTap: TapTapMiniGame.Tap.CreateRewardedVideoAd()
   - 轻游：QGMiniGame.QG.CreateRewardedVideoAd()
   - 支付宝：adManager.CreateRewardAd()
   - Facebook: Meta.InstantGames.Sdk.FBInstant.GetRewardedVideoAsync()
4. 注册事件回调：
   - OnLoad: 加载成功
   - OnClose: 关闭广告
   - OnError: 加载错误
5. 调用 ad.Load() 开始加载
```

**事件回调**:
```csharp
ad.OnLoad += OnPlayVideoLoad;
ad.OnClose += OnPlayVideoOver;
ad.OnError += OnPlayVideoError;
```

**调用者**: `Init()`, `PlayAd()`（播放后预加载）

---

### OnPlayVideoLoad()

**签名**:
```csharp
private void OnPlayVideoLoad()
```

**职责**: 广告加载完成回调

**核心逻辑**:
```
1. 设置 isLoaded = true
2. 如果 State == Loading，设置 State = Loaded
3. 记录日志
```

**调用者**: 广告 OnLoad 事件

---

### OnPlayVideoOver(bool ended, int count)

**签名**:
```csharp
private void OnPlayVideoOver(bool ended, int count)
```

**职责**: 广告关闭回调

**参数**:
- `ended`: 是否播放完成
- `count`: 播放次数

**核心逻辑**:
```
1. 记录日志
2. 如果 ended=true，设置 State = Success
3. 否则设置 State = Fail
```

**调用者**: 广告 OnClose 事件

---

### OnPlayVideoError(int errorCode, string msg)

**签名**:
```csharp
private void OnPlayVideoError(int errorCode, string msg)
```

**职责**: 广告错误回调

**参数**:
- `errorCode`: 错误码
- `msg`: 错误信息

**核心逻辑**:
```
1. 记录错误日志
2. 如果 State == Loading，重新加载 GetNewAd()
3. 如果 State == Playing，设置 State = Fail
```

**调用者**: 广告 OnError 事件

---

## 阅读指引

### 建议的阅读顺序

1. **理解管理器作用** - AdManager 管理什么
2. **看 PlayAd** - 理解播放流程
3. **看 GetNewAd** - 理解预加载机制
4. **了解平台差异** - 理解多平台适配

### 最值得学习的技术点

1. **平台适配**: 支持 10+ 个平台广告 SDK
2. **状态机**: AdState 管理广告生命周期
3. **预加载**: 播放后立即预加载下一个
4. **重试机制**: 加载失败自动重试
5. **异步等待**: ETTask 等待广告播放完成

---

## 平台广告 ID 常量

| 平台 | 常量名 | 广告 ID |
|------|--------|--------|
| 抖音 | `TTVideoAdId` | "43if0f154m6110fmej" |
| 微信 | `WXVideoAdId` | "adunit-e4d84cbe60b6e84f" |
| 支付宝 | `AliPayAdId` | "202509282202083890" |
| 快手/TapTap/Facebook 等 | - | "todo:"（待配置） |

---

## 使用示例

### 示例 1: 检查广告支持

```csharp
if (AdManager.Instance.PlatformHasAD())
{
    Log.Info("当前平台支持广告");
}
else
{
    Log.Info("当前平台不支持广告");
}
```

### 示例 2: 播放广告

```csharp
// 播放广告
bool success = await AdManager.Instance.PlayAd();

if (success)
{
    Log.Info("广告播放成功，发放奖励");
    GiveReward();
}
else
{
    Log.Info("广告播放失败");
}
```

### 示例 3: 完整流程

```csharp
async ETTask OnWatchAdForReward()
{
    // 检查支持
    if (!AdManager.Instance.PlatformHasAD())
    {
        ShowToast("当前平台不支持广告");
        return;
    }
    
    // 播放广告
    bool success = await AdManager.Instance.PlayAd();
    
    // 发放奖励
    if (success)
    {
        GiveReward();
        Log.Info("获得奖励");
    }
    else
    {
        ShowToast("广告播放失败，请重试");
    }
}
```

---

## 相关文档

- [GameRecorderManager.cs.md](./GameRecorderManager.cs.md) - 游戏录制管理器
- [SDKManager.cs.md](./SDKManager.cs.md) - SDK 管理器

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
