# ILog.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | ILog.cs |
| **路径** | Assets/Scripts/Mono/Module/Log/ILog.cs |
| **所属模块** | 框架层 → Mono/Module/Log |
| **文件职责** | 定义日志系统接口，规范日志记录行为，支持不同的日志后端实现 |

---

## 接口说明

### ILog

**定义**:
```csharp
public interface ILog
{
    void Trace(string message);
    void Warning(string message);
    void Info(string message);
    void Debug(string message);
    void Error(string message);
    
    void Trace(string message, params object[] args);
    void Warning(string message, params object[] args);
    void Info(string message, params object[] args);
    void Debug(string message, params object[] args);
    void Error(string message, params object[] args);
}
```

| 属性 | 说明 |
|------|------|
| **职责** | 定义日志记录的标准接口 |
| **用途** | 所有日志后端实现必须实现此接口 |
| **设计模式** | 策略模式 |

---

## 方法说明

### Trace(string message)

**职责**: 记录跟踪日志（最详细级别，通常包含堆栈信息）

**使用场景**: 深度调试，方法调用跟踪

---

### Debug(string message)

**职责**: 记录调试日志

**使用场景**: 开发阶段的调试信息

---

### Info(string message)

**职责**: 记录信息日志

**使用场景**: 程序正常运行的状态信息

---

### Warning(string message)

**职责**: 记录警告日志

**使用场景**: 潜在问题，但不影响程序运行

---

### Error(string message)

**职责**: 记录错误日志

**使用场景**: 错误信息，需要立即关注

---

### 格式化方法

所有日志级别都提供 `params object[] args` 版本的格式化方法：

```csharp
void Info(string message, params object[] args);
```

**使用示例**:
```csharp
log.Info("玩家 {0} 进入场景 {1}", playerName, sceneName);
```

---

## 实现示例

### UnityLogger 实现

```csharp
public class UnityLogger : ILog
{
    public void Trace(string msg)
    {
        UnityEngine.Debug.Log(msg);
    }

    public void Debug(string msg)
    {
        UnityEngine.Debug.Log(msg);
    }

    public void Info(string msg)
    {
        UnityEngine.Debug.Log(msg);
    }

    public void Warning(string msg)
    {
        UnityEngine.Debug.LogWarning(msg);
    }

    public void Error(string msg)
    {
        UnityEngine.Debug.LogError(msg);
    }

    public void Error(Exception e)
    {
        UnityEngine.Debug.LogException(e);
    }

    // 格式化版本
    public void Trace(string message, params object[] args)
    {
        UnityEngine.Debug.LogFormat(message, args);
    }

    public void Debug(string message, params object[] args)
    {
        UnityEngine.Debug.LogFormat(message, args);
    }

    public void Info(string message, params object[] args)
    {
        UnityEngine.Debug.LogFormat(message, args);
    }

    public void Warning(string message, params object[] args)
    {
        UnityEngine.Debug.LogWarningFormat(message, args);
    }

    public void Error(string message, params object[] args)
    {
        UnityEngine.Debug.LogErrorFormat(message, args);
    }
}
```

### 文件日志实现示例

```csharp
public class FileLogger : ILog
{
    private string logPath;
    private StreamWriter writer;

    public FileLogger(string path)
    {
        logPath = path;
        writer = new StreamWriter(path, true);
    }

    public void Trace(string message)
    {
        WriteLine("TRACE", message);
    }

    public void Debug(string message)
    {
        WriteLine("DEBUG", message);
    }

    public void Info(string message)
    {
        WriteLine("INFO", message);
    }

    public void Warning(string message)
    {
        WriteLine("WARNING", message);
    }

    public void Error(string message)
    {
        WriteLine("ERROR", message);
    }

    public void Trace(string message, params object[] args)
    {
        WriteLine("TRACE", string.Format(message, args));
    }

    public void Debug(string message, params object[] args)
    {
        WriteLine("DEBUG", string.Format(message, args));
    }

    public void Info(string message, params object[] args)
    {
        WriteLine("INFO", string.Format(message, args));
    }

    public void Warning(string message, params object[] args)
    {
        WriteLine("WARNING", string.Format(message, args));
    }

    public void Error(string message, params object[] args)
    {
        WriteLine("ERROR", string.Format(message, args));
    }

    private void WriteLine(string level, string message)
    {
        string line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";
        writer.WriteLine(line);
        writer.Flush();
    }

    public void Dispose()
    {
        writer?.Close();
        writer?.Dispose();
    }
}
```

### 网络日志实现示例

```csharp
public class NetworkLogger : ILog
{
    private string serverUrl;

    public NetworkLogger(string url)
    {
        serverUrl = url;
    }

    public void Error(string message)
    {
        // 错误日志发送到服务器
        SendToServer("ERROR", message);
    }

    public void Warning(string message)
    {
        SendToServer("WARNING", message);
    }

    public void Info(string message)
    {
        SendToServer("INFO", message);
    }

    public void Debug(string message)
    {
        // 调试日志本地缓存，批量发送
        BufferLog("DEBUG", message);
    }

    public void Trace(string message)
    {
        BufferLog("TRACE", message);
    }

    // 格式化版本...

    private void SendToServer(string level, string message)
    {
        // HTTP POST 发送到日志服务器
    }

    private void BufferLog(string level, string message)
    {
        // 缓存到本地，定期批量发送
    }
}
```

### 组合日志实现示例

```csharp
public class CompositeLogger : ILog
{
    private List<ILog> loggers = new List<ILog>();

    public void AddLogger(ILog logger)
    {
        loggers.Add(logger);
    }

    public void Trace(string message)
    {
        foreach (var logger in loggers)
        {
            logger.Trace(message);
        }
    }

    public void Debug(string message)
    {
        foreach (var logger in loggers)
        {
            logger.Debug(message);
        }
    }

    public void Info(string message)
    {
        foreach (var logger in loggers)
        {
            logger.Info(message);
        }
    }

    public void Warning(string message)
    {
        foreach (var logger in loggers)
        {
            logger.Warning(message);
        }
    }

    public void Error(string message)
    {
        foreach (var logger in loggers)
        {
            logger.Error(message);
        }
    }

    // 格式化版本...
}

// 使用
var composite = new CompositeLogger();
composite.AddLogger(new UnityLogger());
composite.AddLogger(new FileLogger("game.log"));
composite.AddLogger(new NetworkLogger("http://log-server.com"));

Log.ILog = composite;
```

---

## 使用示例

### 示例 1: 设置日志实现

```csharp
// 程序启动时设置
void Start()
{
    Log.ILog = new UnityLogger();
}
```

### 示例 2: 多环境日志配置

```csharp
void Start()
{
#if UNITY_EDITOR
    // 开发环境：Unity 控制台 + 文件
    var composite = new CompositeLogger();
    composite.AddLogger(new UnityLogger());
    composite.AddLogger(new FileLogger("debug.log"));
    Log.ILog = composite;
#else
    // 生产环境：文件 + 网络
    var composite = new CompositeLogger();
    composite.AddLogger(new FileLogger("game.log"));
    composite.AddLogger(new NetworkLogger("http://log-server.com"));
    Log.ILog = composite;
#endif
}
```

### 示例 3: 自定义日志实现

```csharp
public class ColoredLogger : ILog
{
    public void Trace(string message)
    {
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine($"[TRACE] {message}");
        Console.ResetColor();
    }

    public void Debug(string message)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"[DEBUG] {message}");
        Console.ResetColor();
    }

    public void Info(string message)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"[INFO] {message}");
        Console.ResetColor();
    }

    public void Warning(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[WARNING] {message}");
        Console.ResetColor();
    }

    public void Error(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[ERROR] {message}");
        Console.ResetColor();
    }

    // 格式化版本...
}
```

---

## 阅读指引

### 建议的阅读顺序

1. **理解 ILog 作用** - 为什么需要日志接口
2. **看方法定义** - 了解日志级别
3. **了解 UnityLogger** - 理解默认实现
4. **探索其他实现** - 理解接口的灵活性

### 最值得学习的技术点

1. **策略模式**: 通过接口支持不同的日志后端
2. **接口分离**: 每个日志级别独立方法
3. **格式化支持**: `params object[] args` 支持格式化
4. **可扩展性**: 轻松添加新的日志后端

---

## 最佳实践

### 1. 选择合适的日志后端

- **开发阶段**: UnityLogger（控制台输出）
- **测试阶段**: UnityLogger + FileLogger
- **生产阶段**: FileLogger + NetworkLogger

### 2. 使用组合模式

```csharp
// 同时输出到多个目标
var composite = new CompositeLogger();
composite.AddLogger(new UnityLogger());
composite.AddLogger(new FileLogger("game.log"));
Log.ILog = composite;
```

### 3. 实现异步日志

```csharp
public class AsyncLogger : ILog
{
    private ConcurrentQueue<string> queue = new ConcurrentQueue<string>();
    private Thread writerThread;

    public AsyncLogger()
    {
        writerThread = new Thread(ProcessQueue);
        writerThread.IsBackground = true;
        writerThread.Start();
    }

    public void Info(string message)
    {
        queue.Enqueue($"[INFO] {message}");
    }

    private void ProcessQueue()
    {
        while (true)
        {
            if (queue.TryDequeue(out var message))
            {
                WriteToFile(message);
            }
            else
            {
                Thread.Sleep(10);
            }
        }
    }
}
```

---

## 相关文档

- [Log.cs.md](./Log.cs.md) - 日志系统入口
- [UnityLogger.cs.md](./UnityLogger.cs.md) - Unity 日志实现

---

*文档生成时间：2026-03-01 | OpenClaw AI 助手*
