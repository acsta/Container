# Define.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | Define.cs |
| **路径** | Assets/Scripts/Mono/Define.cs |
| **所属模块** | Mono 层 → 核心常量定义 |
| **文件职责** | 定义全局常量、配置开关、设计分辨率、网络状态等核心配置 |

---

## 类/结构体说明

### Define

| 属性 | 说明 |
|------|------|
| **职责** | 全局静态配置类，提供项目级别的常量定义和运行时配置 |
| **泛型参数** | 无 |
| **继承关系** | 无继承 |
| **实现的接口** | 无 |

**设计模式**: 静态工具类

```csharp
// 使用方式
if (Define.Debug) { /* 调试模式逻辑 */ }
float designWidth = Define.DesignScreenWidth;
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `KEY` | `const byte` | `public` | 加密密钥常量，值为 64 |
| `BuildOutputDir` | `const string` | `public` | 构建输出目录路径 `"./Temp/Bin/Debug"` |
| `DefaultName` | `const string` | `public` | 默认资源包名称 `"DefaultPackage"` |
| `IsSH` | `static bool` | `public` | 是否为 SH 版本标识 |
| `HotfixLoadDir` | `const string` | `public` | 热更新代码加载目录 `"Code/Hotfix/"` |
| `AOTLoadDir` | `const string` | `public` | AOT 代码加载目录（根据平台动态定义） |
| `HotfixDir` | `const string` | `public` | 热更新资源完整路径 |
| `AOTDir` | `const string` | `public` | AOT 资源完整路径 |
| `Debug` | `static readonly bool` | `public` | 调试模式标志（编辑器下为 true，否则根据 isDebugBuild） |
| `DesignScreenWidth` | `static readonly float` | `public` | 设计分辨率宽度（768 或 1366，根据屏幕方向） |
| `DesignScreenHeight` | `static readonly float` | `public` | 设计分辨率高度（1366 或 768，根据屏幕方向） |
| `LogLevel` | `static int` | `public` | 日志级别（调试模式为 1，否则为 5） |
| `ForceUpdate` | `static bool` | `public` | 是否强制更新（受 FORCE_UPDATE 宏控制） |
| `Networked` | `static bool` | `public` | 网络是否可达（编辑器下为 false，否则检查 internetReachability） |
| `Process` | `static int` | `public` | 进程标识，默认为 1 |
| `ConfigType` | `static int` | `public` | 配置类型：0=Json，1=Bytes |
| `RenameList` | `static readonly string[]` | `public` | 重命名列表（如 iOS） |
| `EnterWay` | `static int` | `public` | 进入方式：0=无，1=侧边栏，2=桌面 |
| `FeedType` | `static int` | `public` | 用户类型：0=无，1=复访用户，2=获客用户 |
| `MinRepeatedTimerInterval` | `const int` | `public` | 最小重复定时器间隔（毫秒） |
| `UILayer` | `static readonly int` | `public` | UI 层掩码 |
| `AllLayer` | `static readonly int` | `public` | 所有层掩码（UI、Default、Entity、Water、HitBox 等） |

---

## 平台相关常量

### AOTLoadDir 平台定义

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

**说明**: 根据不同构建平台动态定义 AOT 代码目录，实现平台适配。

---

## 设计分辨率计算

### 屏幕方向自适应

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

**逻辑**:
- 横屏时：宽度 = 1366，高度 = 768
- 竖屏时：宽度 = 768，高度 = 1366

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

**说明**:
- 编辑器模式下始终为 `true`
- 真机模式下根据是否为 Debug Build 决定

### ForceUpdate 强制更新

```csharp
#if FORCE_UPDATE
    public static bool ForceUpdate = true;  // 无网络或更新失败只能退出或重试
#else
    public static bool ForceUpdate = false; // 无网络或更新失败可跳过或重试
#endif
```

**说明**: 通过编译宏控制默认更新策略。

---

## 网络状态检测

### Networked 属性

```csharp
public static bool Networked =
#if UNITY_EDITOR
    false;
#else
    UnityEngine.Application.internetReachability != UnityEngine.NetworkReachability.NotReachable;
#endif
```

**说明**:
- 编辑器下默认为 `false`（避免误判）
- 真机下检查 `internetReachability` 判断网络可达性

---

## 使用示例

### 示例 1: 检查调试模式

```csharp
if (Define.Debug)
{
    Log.Debug("调试模式日志");
    LogLevel = 1; // 输出详细日志
}
```

### 示例 2: 获取设计分辨率

```csharp
float uiScaleFactor = Screen.width / Define.DesignScreenWidth;
canvasScaler.referenceResolution = new Vector2(Define.DesignScreenWidth, Define.DesignScreenHeight);
```

### 示例 3: 检查网络状态

```csharp
if (Define.Networked)
{
    // 发起网络请求
    await HttpManager.Instance.HttpGetResult<Data>(url);
}
else
{
    // 显示离线提示
    UIMsgBoxWin.Show("网络连接不可用");
}
```

### 示例 4: 使用层掩码

```csharp
// 只检测 UI 层射线
if (Physics2D.OverlapPoint(point, Define.UILayer))
{
    // UI 交互逻辑
}

// 检测所有层
if (Physics2D.OverlapPoint(point, Define.AllLayer))
{
    // 全层检测逻辑
}
```

### 示例 5: 配置类型选择

```csharp
if (Define.ConfigType == 0)
{
    // 使用 Json 配置
    ConfigLoader.LoadJson();
}
else
{
    // 使用 Bytes 配置（二进制）
    ConfigLoader.LoadBytes();
}
```

---

## 相关文档

- [SystemInfoHelper.cs.md](./Helper/SystemInfoHelper.cs.md) - 系统信息助手（用于屏幕尺寸获取）
- [Log.cs.md](./Module/Log/Log.cs.md) - 日志系统（使用 LogLevel）
- [HttpManager.cs.md](./Module/Http/HttpManager.cs.md) - HTTP 管理器（使用 Networked）
- [PackageManager.cs.md](./Module/YooAssets/PackageManager.cs.md) - 资源包管理（使用 DefaultName）

---

*文档生成时间：2026-03-02 | OpenClaw AI 助手*
