# LoginPlatform.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | LoginPlatform.cs |
| **路径** | Assets/Scripts/Code/Module/Net/LoginPlatform.cs |
| **所属模块** | 框架层 → Code/Module/Net |
| **文件职责** | 登录平台枚举定义 |

---

## 枚举说明

### LoginPlatform

| 属性 | 说明 |
|------|------|
| **职责** | 定义支持的登录平台类型 |
| **类型** | enum（int） |

```csharp
public enum LoginPlatform
{
    Dev = 0,
    TikTok = 1,
    KuaiShou = 2,
    WeChat = 3,
    Bilibili = 4,
    TapTap = 5,
    AliPay = 6,
    QuickGame = 7,
    _4399 = 8,
    MiniHost = 9,
}
```

---

## 平台列表

| 值 | 枚举 | 说明 | 平台 |
|------|------|------|------|
| 0 | `Dev` | 开发环境 | 本地开发 |
| 1 | `TikTok` | 抖音小游戏 | 字节跳动 |
| 2 | `KuaiShou` | 快手小游戏 | 快手 |
| 3 | `WeChat` | 微信小游戏 | 腾讯微信 |
| 4 | `Bilibili` | 哔哩哔哩 | B 站 |
| 5 | `TapTap` | TapTap | 心动网络 |
| 6 | `AliPay` | 支付宝小游戏 | 支付宝 |
| 7 | `QuickGame` | 轻游 | 轻游平台 |
| 8 | `_4399` | 4399 小游戏 | 4399 |
| 9 | `MiniHost` | 小游戏主机 | 通用小游戏平台 |

---

## 使用示例

### 示例 1: 获取当前平台

```csharp
// 获取当前登录平台
LoginPlatform platform = GetCurrentPlatform();

if (platform == LoginPlatform.WeChat)
{
    Log.Info("当前是微信小游戏");
}
else if (platform == LoginPlatform.TikTok)
{
    Log.Info("当前是抖音小游戏");
}
```

### 示例 2: 平台相关逻辑

```csharp
// 根据平台选择广告 ID
string GetAdId(LoginPlatform platform)
{
    switch (platform)
    {
        case LoginPlatform.TikTok:
            return AdManager.TTVideoAdId;
        case LoginPlatform.WeChat:
            return AdManager.WXVideoAdId;
        case LoginPlatform.AliPay:
            return AdManager.AliPayAdId;
        default:
            return null;
    }
}
```

### 示例 3: 平台能力检查

```csharp
// 检查平台是否支持录制
bool SupportRecorder(LoginPlatform platform)
{
    return platform == LoginPlatform.TikTok 
        || platform == LoginPlatform.WeChat 
        || platform == LoginPlatform.KuaiShou;
}

// 检查平台是否支持广告
bool HasAds(LoginPlatform platform)
{
    return platform != LoginPlatform.Dev;
}
```

### 示例 4: 存储平台信息

```csharp
// 保存登录平台
PlayerData data = GetPlayerData();
data.Platform = currentPlatform;

// 服务器端用于排行榜分类
var rankList = await APIManager.Instance.GetRankList(
    platform: LoginPlatform.WeChat,
    page: 1
);
```

---

## 平台特性对比

| 平台 | 广告 | 录制 | 支付 | 社交 |
|------|------|------|------|------|
| 抖音 | ✅ | ✅ | ✅ | ✅ |
| 微信 | ✅ | ✅ | ✅ | ✅ |
| 快手 | ✅ | ✅ | ✅ | ✅ |
| 支付宝 | ✅ | ❌ | ✅ | ❌ |
| TapTap | ✅ | ✅ | ✅ | ✅ |
| B 站 | ✅ | ❌ | ✅ | ✅ |
| 4399 | ✅ | ❌ | ✅ | ❌ |

---

## 相关文档

- [APIManager.cs.md](./APIManager.cs.md) - API 管理器
- [AdManager.cs.md](../Player/AdManager.cs.md) - 广告管理器
- [GameRecorderManager.cs.md](../Player/GameRecorderManager.cs.md) - 游戏录制管理器

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
