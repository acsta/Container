# BridgeHelper.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | BridgeHelper.cs |
| **路径** | Assets/Scripts/Mono/Helper/BridgeHelper.cs |
| **所属模块** | Mono/Helper |
| **文件职责** | 平台桥接工具，封装各小游戏平台 API (退出/震动/剪贴板) |

---

## 类说明

### BridgeHelper

| 属性 | 说明 |
|------|------|
| **职责** | 提供跨平台 API 调用 (退出应用、震动反馈、复制文本) |
| **类型** | `static partial class` |

---

## 方法说明

### Quit()

```csharp
public static void Quit()
```

**职责**: 退出应用

**平台适配**:
- WebGL (QG): `QGMiniGame.QG.ExitApplication()`
- WebGL (其他): `CloseWindow()`
- 其他平台: `Application.Quit()`

---

### DoVibrate(int during = 500)

```csharp
public static void DoVibrate(int during = 500)
```

**职责**: 震动反馈

**平台适配**:
- WebGL: `Vibrate(during)`
- Android/iOS: `Handheld.Vibrate()`
- 其他: 日志输出

---

### CopyBuffer(string content)

```csharp
public static void CopyBuffer(string content)
```

**职责**: 复制文本到剪贴板

**平台适配**:
- 抖音：`TTSDK.TT.SetClipboardData()`
- 微信/B 站：`WeChatWASM.WX.SetClipboardData()`
- TapTap: `TapTapMiniGame.Tap.SetClipboardData()`
- minihost: `minihost.TJ.SetClipboardData()`
- 快手：`KSWASM.KS.SetClipboardData()`
- OPPO: `QGMiniGame.QG.SetClipboardData()`
- 支付宝：`AlipaySdk.AlipaySDK.API.SetClipboard()`
- 其他：`GUIUtility.systemCopyBuffer`

---

## 使用示例

```csharp
// 退出应用
BridgeHelper.Quit();

// 震动反馈
BridgeHelper.DoVibrate(500);

// 复制文本
BridgeHelper.CopyBuffer("复制内容");
```

---

## 相关文档

- [SystemInfoHelper.cs.md](./SystemInfoHelper.cs.md) - 系统信息助手
- [PlatformUtil.cs.md](./PlatformUtil.cs.md) - 平台工具

---

*文档生成时间：2026-03-02 | OpenClaw AI 助手*
