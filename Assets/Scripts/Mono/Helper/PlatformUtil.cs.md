# PlatformUtil.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | PlatformUtil.cs |
| **路径** | Assets/Scripts/Mono/Helper/PlatformUtil.cs |
| **所属模块** | Mono/Helper |
| **文件职责** | 平台检测工具类，判断运行平台 (iOS/Android/WebGL/模拟器等) |

---

## 类说明

### PlatformUtil

| 属性 | 说明 |
|------|------|
| **职责** | 提供平台检测 API，支持 WebGL 小游戏平台识别 |
| **类型** | `class` |

---

## 方法说明

### GetIntPlatform()

```csharp
public static int GetIntPlatform()
```

**职责**: 获取平台枚举值

---

### GetStrPlatformIgnoreEditor()

```csharp
public static string GetStrPlatformIgnoreEditor()
```

**职责**: 获取平台字符串 (android/ios/webgl/pc)

---

### IsIphone() / IsAndroid() / IsWindows() / IsWebGL()

```csharp
public static bool IsIphone()
public static bool IsAndroid()
public static bool IsWindows()
public static bool IsWebGL()
```

**职责**: 平台类型判断

---

### IsMobile()

```csharp
public static bool IsMobile()
```

**职责**: 判断是否为移动端 (含小游戏平台)

---

### IsMobileWebGL()

```csharp
public static bool IsMobileWebGL()
```

**职责**: 判断是否为移动端 WebGL

---

### IsSimulator()

```csharp
public static bool IsSimulator()
```

**职责**: 判断是否为安卓模拟器

---

### IsHuaWeiGroup()

```csharp
public static bool IsHuaWeiGroup()
```

**职责**: 判断是否为华为渠道

---

### IsWebGl1()

```csharp
public static bool IsWebGl1()
```

**职责**: 判断是否为 WebGL 1.0

---

## 使用示例

```csharp
// 平台判断
if (PlatformUtil.IsMobile())
{
    // 移动端逻辑
}

if (PlatformUtil.IsWebGL())
{
    // WebGL 逻辑
}

if (PlatformUtil.IsSimulator())
{
    // 模拟器特殊处理
}

// 获取平台字符串
string platform = PlatformUtil.GetStrPlatformIgnoreEditor(); // "android" / "ios" / "webgl"
```

---

## 相关文档

- [SystemInfoHelper.cs.md](./SystemInfoHelper.cs.md) - 系统信息助手
- [BridgeHelper.cs.md](./BridgeHelper.cs.md) - 桥接助手

---

*文档生成时间：2026-03-02 | OpenClaw AI 助手*
