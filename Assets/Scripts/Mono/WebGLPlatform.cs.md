# WebGLPlatform.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|------|
| **文件名** | WebGLPlatform.cs |
| **路径** | Assets/Scripts/Mono/WebGLPlatform.cs |
| **所属模块** | 框架层 → Mono |
| **文件职责** | 定义 WebGL 小游戏平台枚举 |

---

## 枚举说明

### WebGLPlatform

| 属性 | 说明 |
|------|------|
| **类型** | `enum` |
| **说明** | 支持的小游戏平台列表 |

| 枚举值 | 说明 | 平台 |
|--------|------|------|
| `WebGL` | 标准 WebGL | 通用 Web 平台 |
| `TikTok` | 抖音小游戏 | 字节跳动 |
| `WeChat` | 微信小游戏 | 腾讯微信 |
| `KuaiShou` | 快手小游戏 | 快手 |
| `Bilibili` | B 站小游戏 | 哔哩哔哩 |
| `TapTap` | TapTap 小游戏 | TapTap |
| `AliPay` | 支付宝小游戏 | 蚂蚁集团 |
| `QuickGame` | 快游戏 | OPPO/小米/荣耀快应用 |
| `MeiTuan` | 美团小游戏 | 美团 |
| `FaceBook` | Facebook 小游戏 | Meta |
| `_4399` | 4399 小游戏 | 4399 游戏 |
| `MiniHost` | Unity 小游戏宿主 | 自定义宿主 |

---

## 使用示例

### 示例 1: 平台判断

```csharp
// 判断当前平台
#if UNITY_WEBGL_TT
    WebGLPlatform platform = WebGLPlatform.TikTok;
#elif UNITY_WEBGL_WeChat
    WebGLPlatform platform = WebGLPlatform.WeChat;
#elif UNITY_WEBGL
    WebGLPlatform platform = WebGLPlatform.WebGL;
#endif
```

### 示例 2: 平台特定逻辑

```csharp
public void InitPlatform()
{
    WebGLPlatform platform = GetCurrentPlatform();
    
    switch (platform)
    {
        case WebGLPlatform.TikTok:
            // 初始化抖音 SDK
            TTSDK.Init();
            break;
        case WebGLPlatform.WeChat:
            // 初始化微信 SDK
            WeChatWASM.Init();
            break;
        case WebGLPlatform.KuaiShou:
            // 初始化快手 SDK
            break;
        // ... 其他平台
    }
}
```

### 示例 3: 平台能力检查

```csharp
public bool SupportsFeature(WebGLPlatform platform, Feature feature)
{
    switch (platform)
    {
        case WebGLPlatform.TikTok:
        case WebGLPlatform.WeChat:
            return true; // 支持广告
        case WebGLPlatform.WebGL:
            return false; // 标准 WebGL 不支持
        default:
            return false;
    }
}
```

---

## 平台 SDK 对应关系

| 平台 | SDK 命名空间 | 文档 |
|------|-------------|------|
| TikTok | `TTSDK.TT` | 抖音小游戏文档 |
| WeChat | `WeChatWASM.WX` | 微信小游戏文档 |
| KuaiShou | `KuaiShouSDK` | 快手小游戏文档 |
| Bilibili | `BiliSDK` | B 站小游戏文档 |
| AliPay | `AliPaySDK` | 支付宝小游戏文档 |

---

## 相关文档

- [PlatformUtil.cs.md](./Helper/PlatformUtil.cs.md) - 平台工具类
- [BridgeHelper.cs.md](./Helper/BridgeHelper.cs.md) - 桥接助手

---

*文档生成时间：2026-03-01 | OpenClaw AI 助手*
