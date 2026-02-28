# Define.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|------|
| **文件名** | Define.cs |
| **路径** | Assets/Scripts/Mono/Define.cs |
| **所属模块** | 框架层 → Mono |
| **文件职责** | 定义全局常量、配置和平台宏，提供项目级别的统一配置入口 |

---

## 类/结构体说明

### Define

| 属性 | 说明 |
|------|------|
| **职责** | 静态类，存储项目全局常量、配置开关和平台相关定义 |
| **泛型参数** | 无 |
| **继承关系** | 无 |
| **实现的接口** | 无 |

**设计模式**: 常量模式 + 配置中心

```csharp
// 使用示例
if (Define.Debug)
{
    Log.Debug("调试信息");
}

int configType = Define.ConfigType;  // 0=Json, 1=Bytes
```

---

## 常量与字段详解

### 加密与资源

| 常量 | 类型 | 值 | 说明 |
|------|------|-----|------|
| `KEY` | `byte` | 64 | 加密密钥 |
| `BuildOutputDir` | `string` | "./Temp/Bin/Debug" | 构建输出目录 |
| `DefaultName` | `string` | "DefaultPackage" | 默认资源包名称 |

---

### 热更新路径

| 常量 | 类型 | 说明 |
|------|------|------|
| `HotfixLoadDir` | `string` | "Code/Hotfix/" - 热更代码加载目录 |
| `AOTLoadDir` | `string` | 平台相关的 AOT 代码目录 |

**平台宏定义**:
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

---

### 资源包路径

```csharp
public const string HotfixDir = "Assets/AssetsPackage/" + HotfixLoadDir;
public const string AOTDir = "Assets/AssetsPackage/" + AOTLoadDir;
```

**说明**: 完整的资源包路径（用于编辑器）

---

### 调试模式

```csharp
#if UNITY_EDITOR
    public static readonly bool Debug = true;
#else
    public static readonly bool Debug = UnityEngine.Debug.isDebugBuild;
#endif
```

**说明**: 
- 编辑器模式：始终为 `true`
- 构建模式：取决于是否为 Debug 构建

---

### 设计分辨率

```csharp
private const int dWidth = 768;
private const int dHeight = 1366;

public static readonly float DesignScreenWidth = ...;
public static readonly float DesignScreenHeight = ...;
```

**说明**: 根据屏幕方向动态计算设计分辨率

**逻辑**:
- 横屏：宽 = max(768, 1366), 高 = min(768, 1366)
- 竖屏：宽 = min(768, 1366), 高 = max(768, 1366)

---

### 日志级别

```csharp
public static int LogLevel = Debug ? 1 : 5;
```

**说明**:
- Debug 模式：LogLevel = 1（显示所有日志）
- Release 模式：LogLevel = 5（仅显示严重错误）

---

### 强更开关

```csharp
#if FORCE_UPDATE
    public static bool ForceUpdate = true;   // 强更
#else
    public static bool ForceUpdate = false;  // 非强更
#endif
```

**说明**:
- `ForceUpdate = true`: 无网络或更新失败时只能退出或重试
- `ForceUpdate = false`: 可以跳过更新

---

### 网络状态

```csharp
public static bool Networked =
#if UNITY_EDITOR
    false;
#else
    UnityEngine.Application.internetReachability != UnityEngine.NetworkReachability.NotReachable;
#endif
```

**说明**:
- 编辑器模式：始终为 `false`（模拟无网络）
- 真机：根据实际网络可达性判断

---

### 配置类型

```csharp
/// <summary>
/// 0:Json 1:Bytes
/// </summary>
public static int ConfigType = 1;
```

**说明**:
- `0`: 使用 JSON 格式配置（便于调试）
- `1`: 使用二进制格式配置（发布版本）

---

### 其他配置

| 字段 | 类型 | 说明 |
|------|------|------|
| `RenameList` | `string[]` | 重命名列表（如 "iOS"） |
| `EnterWay` | `int` | 进入方式（0 无/1 侧边栏/2 桌面） |
| `FeedType` | `int` | Feed 类型（0 无/1 复访/2 获客） |
| `MinRepeatedTimerInterval` | `int` | 最小重复定时器间隔（100ms） |

---

### 图层掩码

```csharp
public static readonly int UILayer = LayerMask.GetMask("UI");
public static readonly int AllLayer = LayerMask.GetMask("UI", "Default", "Entity", "Water", "HitBox", "HitScene", "Ignore Raycast", "TransparentFX");
```

**说明**:
- `UILayer`: UI 图层掩码
- `AllLayer`: 所有图层的掩码（用于射线检测等）

---

## 使用示例

### 示例 1: 调试日志

```csharp
if (Define.Debug)
{
    Log.Debug("这是调试信息");
}

// 根据 LogLevel 过滤日志
if (Define.LogLevel <= 3)
{
    Log.Info("信息级别日志");
}
```

### 示例 2: 平台判断

```csharp
#if UNITY_ANDROID
    // Android 特定逻辑
#elif UNITY_IOS
    // iOS 特定逻辑
#endif

// 或使用路径判断
string aotPath = Define.AOTDir;
```

### 示例 3: 设计分辨率

```csharp
// UI 适配时使用
float uiScale = Screen.width / Define.DesignScreenWidth;
canvas.scaleFactor = uiScale;
```

### 示例 4: 网络检查

```csharp
if (!Define.Networked)
{
    ShowNetworkError();
    if (Define.ForceUpdate)
    {
        // 强更模式下只能重试或退出
        ShowRetryOrExit();
    }
    else
    {
        // 非强更模式可以跳过
        SkipUpdate();
    }
}
```

---

## 设计要点

### 为什么使用静态类？

1. **全局访问**: 任何地方都可以直接使用 `Define.XXX`
2. **性能**: 静态字段访问最快
3. **集中管理**: 所有配置在一处，便于维护
4. **类型安全**: 相比字符串常量更安全

### 条件编译的意义

```csharp
#if UNITY_EDITOR
    // 编辑器专用代码
#else
    // 构建版本代码
#endif
```

**优势**:
- 编辑器调试方便
- 构建版本精简
- 平台差异化处理

### 配置开关的设计

```csharp
#if FORCE_UPDATE
    ForceUpdate = true;
#else
    ForceUpdate = false;
#endif
```

**用途**:
- 通过编译符号控制行为
- 无需修改代码即可切换策略
- 便于 CI/CD 自动化

---

## 相关文档

- [SystemInfoHelper.cs.md](../Helper/SystemInfoHelper.cs.md) - 系统信息助手（用于屏幕尺寸）
- [AssemblyManager.cs.md](./Assembly/AssemblyManager.cs.md) - 程序集管理器
- [Unity LayerMask 文档](https://docs.unity3d.com/ScriptReference/LayerMask.html)

---

*文档生成时间：2026-02-28 | OpenClaw AI 助手*
