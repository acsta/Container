# Define.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | Define.cs |
| **路径** | Assets/Scripts/Mono/Define.cs |
| **所属模块** | Mono 核心框架 |
| **命名空间** | `TaoTie` |
| **文件职责** | 全局常量与配置定义，包含构建设置、设计分辨率、调试开关等 |

---

## 类/结构体说明

### Define

| 属性 | 说明 |
|------|------|
| **职责** | 提供全局常量、配置开关、平台相关路径定义 |
| **类型** | `static class` |
| **继承关系** | 无继承 |

**设计模式**: 静态工具类

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `KEY` | `byte` | `public const` | 加密密钥常量 (64) |
| `BuildOutputDir` | `string` | `public const` | 构建输出目录 (`./Temp/Bin/Debug`) |
| `DefaultName` | `string` | `public const` | 默认资源包名称 (`DefaultPackage`) |
| `IsSH` | `bool` | `public static` | 是否为微信小游戏标志 |
| `HotfixLoadDir` | `string` | `public const` | 热更新代码加载目录 (`Code/Hotfix/`) |
| `AOTLoadDir` | `string` | `public const` | AOT 代码加载目录 (平台相关) |
| `HotfixDir` | `string` | `public const` | 热更新代码完整路径 |
| `AOTDir` | `string` | `public const` | AOT 代码完整路径 |
| `Debug` | `bool` | `public static readonly` | 调试模式标志 (编辑器下恒为 true) |
| `DesignScreenWidth` | `float` | `public static readonly` | 设计屏幕宽度 (768/1366 根据横竖屏) |
| `DesignScreenHeight` | `float` | `public static readonly` | 设计屏幕高度 (1366/768 根据横竖屏) |
| `LogLevel` | `int` | `public static` | 日志级别 (调试=1, 发布=5) |
| `ForceUpdate` | `bool` | `public static` | 是否强制更新 (受 FORCE_UPDATE 宏控制) |
| `Networked` | `bool` | `public static` | 网络是否可达 |
| `Process` | `int` | `public static` | 进程数 (默认 1) |
| `ConfigType` | `int` | `public static` | 配置类型 (0=Json, 1=Bytes) |
| `RenameList` | `string[]` | `public static readonly` | 重命名列表 (`{"iOS"}`) |
| `EnterWay` | `int` | `public static` | 进入方式 (0 无/1 侧边栏/2 桌面) |
| `FeedType` | `int` | `public static` | 用户类型 (0 无/1 复访/2 获客) |
| `MinRepeatedTimerInterval` | `int` | `public const` | 最小重复定时器间隔 (100ms) |
| `UILayer` | `int` | `public static readonly` | UI 层掩码 |
| `AllLayer` | `int` | `public static readonly` | 所有层掩码 (UI+Default+Entity+Water+HitBox+HitScene+Ignore Raycast+TransparentFX) |

---

## 平台相关路径

### AOT 加载目录

```csharp
#if UNITY_ANDROID
    public const string AOTLoadDir = "Code/AOTAndroid/";
#elif UNITY_IOS
    public const string AOTLoadDir = "Code/AOTiOS/";
#elif UNITY_WEBGL
    public const string AOTLoadDir = "Code/AOTWebGL/";
#else
    public const string AOTLoadDir = "Code/AOT/";
#endif
```

**说明**: 根据不同构建平台使用不同的 AOT 代码目录，避免平台间复用导致的裁剪问题。

---

## 设计分辨率

### 自适应设计

```csharp
private const int dWidth = 768;
private const int dHeight = 1366;

public static readonly float DesignScreenWidth =
    SystemInfoHelper.screenWidth > SystemInfoHelper.screenHeight 
    ? Mathf.Max(dWidth, dHeight) 
    : Mathf.Min(dWidth, dHeight);

public static readonly float DesignScreenHeight =
    SystemInfoHelper.screenWidth > SystemInfoHelper.screenHeight 
    ? Mathf.Min(dWidth, dHeight) 
    : Mathf.Max(dWidth, dHeight);
```

**说明**: 
- 横屏时：宽 1366, 高 768
- 竖屏时：宽 768, 高 1366

---

## 调试与发布配置

### Debug 标志

```csharp
#if UNITY_EDITOR
    public static readonly bool Debug = true;
#else
    public static readonly bool Debug = UnityEngine.Debug.isDebugBuild;
#endif
```

### 日志级别

```csharp
public static int LogLevel = Debug ? 1 : 5;
```

**说明**: 
- 调试模式：LogLevel = 1 (详细日志)
- 发布模式：LogLevel = 5 (仅错误日志)

### 强制更新

```csharp
#if FORCE_UPDATE
    public static bool ForceUpdate = true;  // 无网络或更新失败只能退出或重试
#else
    public static bool ForceUpdate = false; // 无网络或更新失败可跳过或重试
#endif
```

---

## 网络状态

```csharp
public static bool Networked =
#if UNITY_EDITOR
    false;
#else
    UnityEngine.Application.internetReachability != UnityEngine.NetworkReachability.NotReachable;
#endif
```

**说明**: 编辑器下恒为 false，真机根据实际网络状态判断。

---

## 配置类型

```csharp
/// <summary>
/// 0:Json 1:Bytes
/// </summary>
public static int ConfigType = 1;
```

**说明**: 配置表加载格式，1 表示使用 Bytes 格式 (Protobuf 序列化)。

---

## 用户进入方式

```csharp
/// <summary>
/// 0 无 1 侧边栏 2 桌面
/// </summary>
public static int EnterWay;

/// <summary>
/// 0 无，1 复访用户，2 获客用户
/// </summary>
public static int FeedType = 0;
```

**说明**: 用于区分用户进入游戏的渠道和类型，便于数据分析和个性化处理。

---

## 层掩码

```csharp
public static readonly int UILayer = LayerMask.GetMask("UI");

public static readonly int AllLayer = LayerMask.GetMask(
    "UI", "Default", "Entity", "Water", "HitBox", "HitScene",
    "Ignore Raycast", "TransparentFX"
);
```

**说明**: 
- `UILayer`: 仅 UI 层
- `AllLayer`: 所有常用层的组合，用于全层射线检测

---

## 使用示例

### 检查调试模式

```csharp
if (Define.Debug)
{
    Log.Debug("调试模式开启");
}
```

### 获取设计分辨率

```csharp
float width = Define.DesignScreenWidth;
float height = Define.DesignScreenHeight;
```

### 检查网络状态

```csharp
if (Define.Networked)
{
    // 网络可用，执行在线逻辑
}
else
{
    // 网络不可用，执行离线逻辑
}
```

### 使用层掩码

```csharp
// UI 层射线检测
var hits = Physics2D.Raycast(origin, direction, distance, Define.UILayer);

// 全层射线检测
var hits = Physics2D.Raycast(origin, direction, distance, Define.AllLayer);
```

### 检查用户类型

```csharp
if (Define.FeedType == 1)
{
    // 复访用户逻辑
}
else if (Define.FeedType == 2)
{
    // 获客用户逻辑
}
```

---

## 相关文档

- [SystemInfoHelper.cs.md](./Helper/SystemInfoHelper.cs.md) - 系统信息助手
- [Log.cs.md](./Module/Log/Log.cs.md) - 日志系统
- [ConfigManager.cs.md](../../Code/Module/Config/ConfigManager.cs.md) - 配置管理器

---

*文档生成时间：2026-03-02 | OpenClaw AI 助手*
