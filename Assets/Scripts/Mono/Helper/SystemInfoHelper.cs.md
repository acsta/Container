# SystemInfoHelper.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | SystemInfoHelper.cs |
| **路径** | Assets/Scripts/Mono/Helper/SystemInfoHelper.cs |
| **所属模块** | Mono/Helper |
| **文件职责** | 跨平台系统信息获取助手，适配各小游戏平台 (微信/抖音/快手/支付宝等) |

---

## 类说明

### SystemInfoHelper

| 属性 | 说明 |
|------|------|
| **职责** | 提供统一的系统信息访问接口，自动适配 WebGL 小游戏平台 |
| **类型** | `static class` |

---

## 字段与属性

| 名称 | 类型 | 说明 |
|------|------|------|
| `screenHeight` | `float` | 屏幕高度 (平台适配) |
| `screenWidth` | `float` | 屏幕宽度 (平台适配) |
| `safeArea` | `Rect` | 安全区域 (避开刘海/圆角) |

---

## 平台适配

支持平台：
- Unity WebGL (通用)
- 微信小程序 (UNITY_WEBGL_WeChat)
- 抖音小游戏 (UNITY_WEBGL_TT)
- 快手小游戏 (UNITY_WEBGL_KS)
- 支付宝小游戏 (UNITY_WEBGL_ALIPAY)
- TapTap 小游戏 (UNITY_WEBGL_TAPTAP)
- OPPO 小游戏 (UNITY_WEBGL_QG)
- B 站小游戏 (UNITY_WEBGL_BILIGAME)
- minihost (UNITY_WEBGL_MINIHOST)

---

## 使用示例

```csharp
// 获取屏幕尺寸
float width = SystemInfoHelper.screenWidth;
float height = SystemInfoHelper.screenHeight;

// 获取安全区域 (适配刘海屏)
Rect safeArea = SystemInfoHelper.safeArea;
```

---

## 相关文档

- [BridgeHelper.cs.md](./BridgeHelper.cs.md) - 桥接助手
- [PlatformUtil.cs.md](./PlatformUtil.cs.md) - 平台工具

---

*文档生成时间：2026-03-02 | OpenClaw AI 助手*
