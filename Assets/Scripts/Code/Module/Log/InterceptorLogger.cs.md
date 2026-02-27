# InterceptorLogger.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | InterceptorLogger.cs |
| **路径** | Assets/Scripts/Code/Module/Log/InterceptorLogger.cs |
| **所属模块** | 框架层 → Code/Module/Log |
| **文件职责** | 拦截器日志实现，支持日志文件写入和平台日志 SDK |

---

## 类/结构体说明

### InterceptorLogger

| 属性 | 说明 |
|------|------|
| **职责** | 实现 ILog 接口，拦截所有日志输出，写入文件并调用平台日志 SDK |
| **泛型参数** | 无 |
| **继承关系** | 实现 `ILog` |

```csharp
public class InterceptorLogger : ILog
{
    // 拦截器日志
}
```

---

## 字段与属性

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `CheckLv` | `int` | `public` | 日志级别检查阈值 |
| `dir` | `string` | `public static` | 日志目录（Application.persistentDataPath + "/log/"） |
| `logManager` | `TTRealtimeLogManager/WXLogManager/TapLogManager` | `private` | 平台日志管理器 |

---

## 日志级别常量

| 常量 | 值 | 说明 |
|------|-----|------|
| `TraceLevel` | 1 | 追踪日志 |
| `DebugLevel` | 2 | 调试日志 |
| `InfoLevel` | 3 | 信息日志 |
| `WarningLevel` | 4 | 警告日志 |

---

## 方法说明

### Constructor

**签名**:
```csharp
public InterceptorLogger()
```

**职责**: 初始化拦截器日志

**核心逻辑**:
```
1. 设置 CheckLv = Define.LogLevel
2. 清理日志目录（保留今天和昨天的日志）
3. 写入分割线
4. 初始化平台日志管理器（抖音/微信/TapTap）
```

**日志清理**:
```csharp
CleanDirectory(dir,
    new List<string>()
    {
        DateTime.Now.ToString("yyyy-MM-dd"),
        DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd")
    }
);
```

**平台日志管理器**:
```csharp
#if UNITY_WEBGL_TT
    logManager = TTSDK.TT.GetRealtimeLogManager(...);
#elif UNITY_WEBGL_WeChat
    logManager = WeChatWASM.WX.GetLogManager(...);
#elif UNITY_WEBGL_TAPTAP
    logManager = TapTapMiniGame.Tap.GetLogManager(...);
#endif
```

---

### Trace / Debug / Info / Warning / Error

**签名**:
```csharp
public void Trace(string msg)
public void Debug(string msg)
public void Info(string msg)
public void Warning(string msg)
public void Error(string msg)
public void Error(Exception e)
```

**职责**: 输出各级别日志

**核心逻辑**:
```
1. 检查日志级别（CheckLogLevel）
2. 如果满足级别，调用 UnityEngine.Debug
3. 写入日志文件（WriteText）
4. 如果是 Error，调用平台日志 SDK
```

**日志格式**:
```
[Trace] 2026-02-27 10:00:00 消息内容
[Debug] 2026-02-27 10:00:00 消息内容
[Info] 2026-02-27 10:00:00 消息内容
[Warning] 2026-02-27 10:00:00 消息内容
[Error] 2026-02-27 10:00:00 消息内容
```

---

### WriteText(string log)

**签名**:
```csharp
private void WriteText(string log)
```

**职责**: 写入日志到文件

**核心逻辑**:
```
1. 添加换行符
2. 获取日志文件路径（按日期命名）
3. 追加写入文件
4. 捕获异常（防止写入失败影响游戏）
```

**日志文件**:
```
PersistentDataPath/log/2026-02-27
```

---

### CheckLogLevel(int level)

**签名**:
```csharp
private bool CheckLogLevel(int level)
```

**职责**: 检查日志级别是否满足输出条件

**核心逻辑**:
```
1. 返回 CheckLv <= level
```

**说明**: CheckLv 值越小，输出的日志越多

---

### 格式化日志方法

**签名**:
```csharp
public void Trace(string message, params object[] args)
public void Debug(string message, params object[] args)
public void Info(string message, params object[] args)
public void Warning(string message, params object[] args)
public void Error(string message, params object[] args)
```

**职责**: 输出带格式参数的日志

**核心逻辑**:
```
1. 检查日志级别
2. 调用 UnityEngine.Debug.LogFormat
3. 格式化后写入文件
```

---

## 阅读指引

### 建议的阅读顺序

1. **理解拦截器作用** - InterceptorLogger 做什么
2. **看日志级别** - 理解 CheckLv 机制
3. **看文件写入** - 理解日志持久化
4. **了解平台 SDK** - 理解平台日志集成

### 最值得学习的技术点

1. **日志拦截**: 实现 ILog 接口替换默认日志
2. **日志级别**: CheckLv 控制日志输出
3. **文件持久化**: 按日期写入日志文件
4. **平台集成**: 抖音/微信/TapTap 日志 SDK
5. **日志清理**: 保留最近 2 天日志

---

## 日志级别配置

### Define.LogLevel

| 值 | 输出日志 |
|------|---------|
| 1 | Trace + Debug + Info + Warning + Error |
| 2 | Debug + Info + Warning + Error |
| 3 | Info + Warning + Error |
| 4 | Warning + Error |
| 5 | 只输出 Error |

### 使用示例

```csharp
// 设置日志级别（只输出 Warning 和 Error）
Define.LogLevel = 4;

// 创建拦截器
Log.ILog = new InterceptorLogger();
```

---

## 使用示例

### 示例 1: 输出日志

```csharp
// 简单日志
Log.Info("游戏启动");
Log.Warning("资源加载缓慢");
Log.Error("网络请求失败");

// 带参数日志
Log.Info("玩家{0}登录，等级{1}", playerName, level);
Log.Error("加载资源{0}失败：{1}", path, errorMsg);

// 异常日志
try
{
    // 可能出错的代码
}
catch (Exception ex)
{
    Log.Error(ex);
}
```

### 示例 2: 查看日志文件

```bash
# 日志文件位置
Android: /storage/emulated/0/Android/data/<package>/files/log/2026-02-27
iOS: <App Sandbox>/Library/Caches/log/2026-02-27
WebGL: 无法直接访问（需要平台 SDK）
```

### 示例 3: 平台日志 SDK

```csharp
// 抖音小游戏
// 错误日志会自动发送到抖音开发者平台

// 微信小游戏
// 错误日志会自动发送到微信开发者平台

// TapTap
// 错误日志会自动发送到 TapTap 开发者平台
```

---

## 相关文档

- [LogManager.cs.md](./LogManager.cs.md) - 日志管理器
- [Define.cs.md](../Const/Define.cs.md) - 全局配置

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
