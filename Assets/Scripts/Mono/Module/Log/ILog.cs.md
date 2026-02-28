# ILog.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|------|
| **文件名** | ILog.cs |
| **路径** | Assets/Scripts/Mono/Module/Log/ILog.cs |
| **所属模块** | 框架层 → Mono/Module/Log |
| **文件职责** | 定义日志接口，规范日志记录行为 |

---

## 类/结构体说明

### ILog 接口

| 属性 | 说明 |
|------|------|
| **职责** | 定义日志记录的标准接口，支持不同平台实现 |
| **泛型参数** | 无 |
| **继承关系** | 无 |
| **实现的接口** | 无 |

**设计模式**: 接口模式 + 策略模式

```csharp
// 实现接口
public class UnityLogger : ILog
{
    public void Debug(object message)
    {
        UnityEngine.Debug.Log(message);
    }
    
    // ... 其他方法
}
```

---

## 方法说明

### Debug

**签名**:
```csharp
void Debug(object message)
```

**职责**: 记录调试级别日志

**参数**:
| 参数名 | 类型 | 说明 |
|--------|------|------|
| `message` | `object` | 日志消息 |

**调用者**: Log.Debug()

---

### Info

**签名**:
```csharp
void Info(object message)
```

**职责**: 记录信息级别日志

---

### Warning

**签名**:
```csharp
void Warning(object message)
```

**职责**: 记录警告级别日志

---

### Error

**签名**:
```csharp
void Error(object message)
```

**职责**: 记录错误级别日志

---

### Fatal

**签名**:
```csharp
void Fatal(object message)
```

**职责**: 记录严重错误级别日志

---

## 实现类

### UnityLogger

```csharp
public class UnityLogger : ILog
{
    public static ILog Instance = new UnityLogger();
    
    public void Debug(object message)
    {
        UnityEngine.Debug.Log($"[DEBUG] {message}");
    }
    
    public void Info(object message)
    {
        UnityEngine.Debug.Log($"[INFO] {message}");
    }
    
    public void Warning(object message)
    {
        UnityEngine.Debug.LogWarning($"[WARNING] {message}");
    }
    
    public void Error(object message)
    {
        UnityEngine.Debug.LogError($"[ERROR] {message}");
    }
    
    public void Fatal(object message)
    {
        UnityEngine.Debug.LogException(new Exception(message.ToString()));
    }
}
```

---

## 使用示例

### 示例 1: 直接使用 ILog

```csharp
// 获取实例
var logger = UnityLogger.Instance;

logger.Debug("调试信息");
logger.Info("普通信息");
logger.Warning("警告");
logger.Error("错误");
logger.Fatal("严重错误");
```

### 示例 2: 通过 Log 工具类

```csharp
// 推荐方式
Log.Debug("调试信息");
Log.Info("普通信息");
```

### 示例 3: 自定义日志实现

```csharp
// 文件日志实现
public class FileLogger : ILog
{
    private StreamWriter writer;
    
    public FileLogger(string path)
    {
        writer = new StreamWriter(path, true);
    }
    
    public void Debug(object message)
    {
        WriteLine($"[DEBUG] {DateTime.Now}: {message}");
    }
    
    // ... 其他方法
    
    private void WriteLine(string line)
    {
        writer.WriteLine(line);
        writer.Flush();
    }
}

// 使用
ILog.Instance = new FileLogger("game.log");
```

---

## 设计要点

### 为什么需要接口？

1. **平台适配**: 不同平台使用不同日志系统
   - Unity: UnityEngine.Debug
   - Server: Console.WriteLine / NLog
   - Web: console.log

2. **可测试**: 测试时使用 Mock 实现

3. **可扩展**: 新增日志实现无需修改 Log 工具类

### Instance 的设计

```csharp
public static ILog Instance = new UnityLogger();
```

**用途**:
- 全局单例访问
- 可替换实现
- 便于测试

---

## 相关文档

- [Log.cs.md](./Log.cs.md) - 日志工具类
- [UnityLogger.cs.md](./UnityLogger.cs.md) - Unity 平台实现

---

*文档生成时间：2026-02-28 | OpenClaw AI 助手*
