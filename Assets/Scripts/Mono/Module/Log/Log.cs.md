# Log.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|------|
| **文件名** | Log.cs |
| **路径** | Assets/Scripts/Mono/Module/Log/Log.cs |
| **所属模块** | 框架层 → Mono/Module/Log |
| **文件职责** | 提供全局日志工具类，封装 ILog 实现 |

---

## 类/结构体说明

### Log

| 属性 | 说明 |
|------|------|
| **职责** | 静态工具类，提供全局日志记录功能 |
| **泛型参数** | 无 |
| **继承关系** | 无 |
| **实现的接口** | 无 |

**设计模式**: 工具类模式 + 单例模式（通过 ILog.Instance）

```csharp
// 使用示例
Log.Debug("调试信息");
Log.Info("普通信息");
Log.Warning("警告信息");
Log.Error("错误信息");
Log.Fatal("严重错误");
```

---

## 方法说明

### Debug

**签名**:
```csharp
public static void Debug(object message)
```

**职责**: 记录调试级别日志

**核心逻辑**:
```
1. 检查 Define.LogLevel <= 1
2. 调用 ILog.Instance.Debug(message)
```

**调用者**: 开发调试代码

**使用示例**:
```csharp
Log.Debug($"玩家位置：{player.position}");
```

---

### Info

**签名**:
```csharp
public static void Info(object message)
```

**职责**: 记录信息级别日志

**核心逻辑**:
```
1. 检查 Define.LogLevel <= 2
2. 调用 ILog.Instance.Info(message)
```

**使用示例**:
```csharp
Log.Info("游戏启动成功");
```

---

### Warning

**签名**:
```csharp
public static void Warning(object message)
```

**职责**: 记录警告级别日志

**核心逻辑**:
```
1. 检查 Define.LogLevel <= 3
2. 调用 ILog.Instance.Warning(message)
```

**使用示例**:
```csharp
Log.Warning("资源加载失败，使用备用资源");
```

---

### Error

**签名**:
```csharp
public static void Error(object message)
```

**职责**: 记录错误级别日志

**核心逻辑**:
```
1. 检查 Define.LogLevel <= 4
2. 调用 ILog.Instance.Error(message)
```

**使用示例**:
```csharp
Log.Error("网络连接失败");
```

---

### Fatal

**签名**:
```csharp
public static void Fatal(object message)
```

**职责**: 记录严重错误级别日志

**核心逻辑**:
```
1. 检查 Define.LogLevel <= 5
2. 调用 ILog.Instance.Fatal(message)
```

**使用示例**:
```csharp
Log.Fatal("数据库连接丢失，无法继续运行");
```

---

## 日志级别

| 级别 | 方法 | LogLevel 阈值 | 说明 |
|------|------|--------------|------|
| Debug | `Debug()` | 1 | 调试信息（仅开发环境） |
| Info | `Info()` | 2 | 普通信息 |
| Warning | `Warning()` | 3 | 警告信息 |
| Error | `Error()` | 4 | 错误信息 |
| Fatal | `Fatal()` | 5 | 严重错误 |

---

## 使用示例

### 示例 1: 基础日志

```csharp
void Start()
{
    Log.Debug("游戏启动");
    Log.Info("加载配置...");
    
    if (!configLoaded)
    {
        Log.Warning("配置加载失败，使用默认配置");
    }
    
    if (!networkConnected)
    {
        Log.Error("网络连接失败");
        return;
    }
    
    Log.Info("游戏启动完成");
}
```

### 示例 2: 异常日志

```csharp
try
{
    LoadResource(resourceName);
}
catch (Exception ex)
{
    Log.Error($"资源加载失败：{resourceName}");
    Log.Error(ex);  // 记录异常堆栈
}
```

### 示例 3: 条件日志

```csharp
if (Define.Debug)
{
    Log.Debug($"FPS: {fps}, Memory: {GC.GetTotalMemory(false)}");
}
```

### 示例 4: 性能日志

```csharp
long startTime = TimeInfo.Instance.ClientNow();

// 执行操作
LoadLargeResource();

long elapsed = TimeInfo.Instance.ClientNow() - startTime;
Log.Debug($"加载耗时：{elapsed}ms");
```

---

## 设计要点

### 为什么使用静态类？

1. **全局访问**: 任何地方都可以直接使用 `Log.XXX()`
2. **简洁**: 无需获取实例
3. **性能**: 静态方法调用最快

### 为什么检查 LogLevel？

```csharp
if (Define.LogLevel <= 1)
{
    ILog.Instance.Debug(message);
}
```

**意义**:
- Debug 模式下显示所有日志
- Release 模式下仅显示 Error/Fatal
- 减少生产环境的日志开销

### 与 ILog 的关系

```
Log (静态工具类)
    ↓
ILog.Instance (接口实现)
    ↓
UnityLogger (Unity 平台实现)
```

**优势**:
- 解耦日志实现
- 支持平台定制
- 便于测试（Mock ILog）

---

## 相关文档

- [ILog.cs.md](./ILog.cs.md) - 日志接口
- [UnityLogger.cs.md](./UnityLogger.cs.md) - Unity 平台实现
- [Define.cs.md](../../Define.cs.md) - 全局配置（LogLevel）

---

*文档生成时间：2026-02-28 | OpenClaw AI 助手*
