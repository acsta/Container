# BridgeHelper.WebGL.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | BridgeHelper.WebGL.cs |
| **路径** | Assets/Scripts/Mono/Helper/BridgeHelper.WebGL.cs |
| **所属模块** | 框架层 → Mono/Helper |
| **文件职责** | WebGL 平台桥接辅助，提供与浏览器原生功能交互的接口 |

---

## 类/结构体说明

### BridgeHelper (partial)

| 属性 | 说明 |
|------|------|
| **职责** | 提供 WebGL 平台下与浏览器原生功能的桥接接口 |
| **类型** | `static partial class` |
| **平台限制** | 主要在 `UNITY_WEBGL` 平台下生效 |

```csharp
public static partial class BridgeHelper
{
    // WebGL 平台桥接功能
}
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `CloseWindow` | `extern` | `private static` | 关闭浏览器窗口（WebGL 原生） |
| `Vibrate` | `extern` | `private static` | 触发设备震动（WebGL 原生） |
| `StringReturnValueFunction` | `extern` | `private static` | 获取当前 URL 字符串（WebGL 原生） |
| `NativeDialogPrompt` | `extern` | `private static` | 显示原生输入对话框（WebGL 原生） |
| `SetupOverlayDialogHtml` | `extern` | `private static` | 设置 HTML 覆盖层对话框（WebGL 原生） |

---

## 方法说明（按重要程度排序）

### OpenNativeStringDialog(string title, string defaultValue)

**签名**:
```csharp
public static string OpenNativeStringDialog(string title, string defaultValue)
```

**职责**: 打开原生字符串输入对话框

**参数**:
- `title`: 对话框标题
- `defaultValue`: 默认值

**返回值**: 用户输入的字符串

**核心逻辑**:
```
1. WebGL 平台：调用 NativeDialogPrompt 原生方法
2. 其他平台：直接返回 defaultValue
```

**调用者**: 需要用户输入字符串的场景

---

### SetUpOverlayDialog(string title, string defaultValue, string okBtnText, string cancelBtnText)

**签名**:
```csharp
public static void SetUpOverlayDialog(string title, string defaultValue, string okBtnText, string cancelBtnText)
```

**职责**: 设置 HTML 覆盖层对话框

**参数**:
- `title`: 对话框标题
- `defaultValue`: 默认值
- `okBtnText`: 确认按钮文本
- `cancelBtnText`: 取消按钮文本

**核心逻辑**:
```
1. WebGL 平台：
   - 如果全屏模式：
     - Edge 浏览器：退出全屏
     - 其他浏览器：隐藏 Unity 屏幕
   - 调用 SetupOverlayDialogHtml 设置对话框
2. 其他平台：不执行操作
```

**调用者**: 需要显示自定义对话框的场景

---

### IsOverlayDialogActive()

**签名**:
```csharp
public static bool IsOverlayDialogActive()
```

**职责**: 检查覆盖层对话框是否激活

**返回值**: `true` = 对话框激活，`false` = 未激活

**核心逻辑**:
```
1. WebGL 平台：调用 IsOverlayDialogHtmlActive 原生方法
2. 其他平台：返回 false
```

**调用者**: 需要检查对话框状态的场景

---

### IsOverlayDialogCanceled()

**签名**:
```csharp
public static bool IsOverlayDialogCanceled()
```

**职责**: 检查覆盖层对话框是否被取消

**返回值**: `true` = 已取消，`false` = 未取消

**核心逻辑**:
```
1. WebGL 平台：调用 IsOverlayDialogHtmlCanceled 原生方法
2. 其他平台：返回 false
```

**调用者**: 对话框关闭后检查结果

---

### GetOverlayDialogValue()

**签名**:
```csharp
public static string GetOverlayDialogValue()
```

**职责**: 获取覆盖层对话框的输入值

**返回值**: 用户输入的字符串

**核心逻辑**:
```
1. WebGL 平台：调用 GetOverlayHtmlInputFieldValue 原生方法
2. 其他平台：返回空字符串
```

**调用者**: 对话框确认后获取输入值

---

### OnOpenUploader()

**签名**:
```csharp
public static void OnOpenUploader()
```

**职责**: 打开上传器

**核心逻辑**:
```
1. WebGL 平台：调用 OpenUploader 原生方法
2. 其他平台：不执行操作
```

**调用者**: 需要上传图片/文件的场景

---

### GetUploaderImgBase64()

**签名**:
```csharp
public static string GetUploaderImgBase64()
```

**职责**: 获取上传器的图片 Base64 数据

**返回值**: Base64 编码的图片数据

**核心逻辑**:
```
1. WebGL 平台：调用 GetImgData 原生方法
2. 其他平台：返回 null
```

**调用者**: 上传完成后获取图片数据

---

### GetUrlParams(string para)

**签名**:
```csharp
public static string GetUrlParams(string para)
```

**职责**: 获取 URL 参数值

**参数**:
- `para`: 参数名

**返回值**: 参数值，不存在则返回 null

**核心逻辑**:
```
1. WebGL 平台：调用 StringReturnValueFunction 获取完整 URL
2. 其他平台：使用测试 URL
3. 调用 ParseQueryString 解析 URL 参数
4. 返回指定参数的值
```

**调用者**: 需要从 URL 获取参数的场景

**使用示例**:
```csharp
// 获取 URL 中的 st 参数
string st = BridgeHelper.GetUrlParams("st");
Log.Info("st = " + st);  // 输出：st = 123
```

---

### ParseQueryString(string queryString)

**签名**:
```csharp
private static Dictionary<string, string> ParseQueryString(string queryString)
```

**职责**: 解析 URL 查询字符串为键值对字典

**参数**:
- `queryString`: 查询字符串（可以是完整 URL 或 ? 后的部分）

**返回值**: 参数键值对字典

**核心逻辑**:
```
1. 如果是完整 URL，提取 ? 后的查询部分
2. 使用正则表达式匹配键值对 pattern: ([^&=]+)=([^&]*)
3. 对键和值进行 URL 解码
4. 存入字典并返回
```

**正则表达式**:
```csharp
string pattern = @"([^&=]+)=([^&]*)";
```

**调用者**: `GetUrlParams()`

---

## 阅读指引

### 建议的阅读顺序

1. **理解 WebGL 桥接作用** - 为什么需要 BridgeHelper
2. **看 GetUrlParams** - 最常用的 URL 参数获取
3. **看对话框相关方法** - 理解覆盖层对话框流程
4. **看上传器相关方法** - 理解图片上传流程

### 最值得学习的技术点

1. **平台条件编译**: `#if UNITY_WEBGL` 区分平台
2. **原生方法调用**: `[DllImport("__Internal")]` 调用 JS 方法
3. **URL 参数解析**: 正则表达式解析查询字符串
4. **覆盖层对话框**: 与浏览器原生 UI 交互

---

## 使用示例

### 示例 1: 获取 URL 参数

```csharp
// 假设 URL: http://example.com/game.html?st=123&uid=456
string st = BridgeHelper.GetUrlParams("st");    // "123"
string uid = BridgeHelper.GetUrlParams("uid");  // "456"
string unknown = BridgeHelper.GetUrlParams("x"); // null
```

### 示例 2: 使用覆盖层对话框

```csharp
// 设置对话框
BridgeHelper.SetUpOverlayDialog(
    title: "请输入昵称",
    defaultValue: "玩家 1",
    okBtnText: "确定",
    cancelBtnText: "取消"
);

// 等待用户操作...

// 检查结果
if (BridgeHelper.IsOverlayDialogActive())
{
    if (!BridgeHelper.IsOverlayDialogCanceled())
    {
        string value = BridgeHelper.GetOverlayDialogValue();
        Log.Info("用户输入：" + value);
    }
}
```

### 示例 3: 图片上传

```csharp
// 打开上传器
BridgeHelper.OnOpenUploader();

// 等待上传完成后获取图片
string base64 = BridgeHelper.GetUploaderImgBase64();
if (base64 != null)
{
    // 使用 base64 数据
    UploadToServer(base64);
}
```

---

## 相关文档

- [BridgeHelper.cs.md](./BridgeHelper.cs.md) - 桥接辅助主类
- [PlatformUtil.cs.md](./PlatformUtil.cs.md) - 平台工具类
- [Define.cs.md](../Define.cs.md) - 全局定义

---

*文档生成时间：2026-03-01 | OpenClaw AI 助手*
