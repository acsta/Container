# Log.cs 注解文档

## 文件基本信息
- **路径**: Assets/Scripts/Mono/Module/Log/Log.cs
- **职责**: 静态日志工具类，提供分级日志输出

## 日志级别
| 级别 | 常量 | LogLevel 阈值 | 说明 |
|------|------|--------------|------|
| Trace | TraceLevel | 1 | 最详细，包含堆栈跟踪 |
| Debug | DebugLevel | 2 | 调试信息 |
| Info | InfoLevel | 3 | 普通信息 |
| Warning | WarningLevel | 4 | 警告 |
| Error | - | 始终输出 | 错误（包含堆栈） |

## 方法说明
### Trace
- 输出最详细的日志，包含堆栈跟踪
- 仅在 LogLevel <= 1 时输出

### Debug
- 输出调试信息
- 仅在 LogLevel <= 2 时输出

### Info
- 输出普通信息
- 仅在 LogLevel <= 3 时输出
- 支持 object 参数

### Warning
- 输出警告信息
- 仅在 LogLevel <= 4 时输出

### Error
- 输出错误信息，始终包含堆栈跟踪
- 始终输出（不受 LogLevel 限制）
- 支持 Exception 参数

## 使用示例
```csharp
// 调试日志
Log.Debug("玩家血量：{0}", player.HP);

// 普通信息
Log.Info("游戏开始");

// 警告
Log.Warning("资源加载失败：{0}", path);

// 错误
Log.Error("空引用异常");
Log.Error(exception);

// 带堆栈的详细日志
Log.Trace("方法调用：{0}", methodName);
```
