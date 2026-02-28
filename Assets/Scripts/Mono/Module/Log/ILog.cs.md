# ILog.cs 注解文档

## 文件基本信息
- **路径**: Assets/Scripts/Mono/Module/Log/ILog.cs
- **职责**: 定义日志输出接口

## 接口方法
```csharp
public interface ILog
{
    void Trace(string message);
    void Debug(string message);
    void Info(string message);
    void Warning(string message);
    void Error(string message);
    
    // 格式化版本
    void Trace(string message, params object[] args);
    void Debug(string message, params object[] args);
    void Info(string message, params object[] args);
    void Warning(string message, params object[] args);
    void Error(string message, params object[] args);
}
```

## 实现示例
### UnityLogger
```csharp
public class UnityLogger : ILog
{
    public void Debug(string message)
    {
        UnityEngine.Debug.Log(message);
    }
    
    public void Error(string message)
    {
        UnityEngine.Debug.LogError(message);
    }
    
    // ... 其他方法
}
```

## 使用方式
```csharp
// 设置日志实现
Log.ILog = new UnityLogger();

// 使用日志
Log.Debug("调试信息");
Log.Error("错误信息");
```
