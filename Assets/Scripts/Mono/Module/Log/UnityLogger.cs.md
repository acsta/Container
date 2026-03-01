# UnityLogger.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | UnityLogger.cs |
| **路径** | Assets/Scripts/Mono/Module/Log/UnityLogger.cs |
| **所属模块** | 框架层 → Mono/Module/Log |
| **文件职责** | Unity 平台的日志实现，将日志输出到 Unity 控制台 |

---

## 类说明

### UnityLogger

| 属性 | 说明 |
|------|------|
| **职责** | 实现 ILog 接口，将日志输出到 Unity Debug 控制台 |
| **泛型参数** | 无 |
| **继承关系** | 实现 `ILog` |
| **设计模式** | 适配器模式 |

```csharp
// 设置为日志实现
Log.ILog = new UnityLogger();
```

---

## 方法说明

### Trace(string msg)

**签名**:
```csharp
public void Trace(string msg)
```

**职责**: 输出跟踪日志到 Unity 控制台

**实现**:
```csharp
UnityEngine.Debug.Log(msg);
```

---

### Debug(string msg)

**签名**:
```csharp
public void Debug(string msg)
```

**职责**: 输出调试日志到 Unity 控制台

**实现**:
```csharp
UnityEngine.Debug.Log(msg);
```

---

### Info(string msg)

**签名**:
```csharp
public void Info(string msg)
```

**职责**: 输出信息日志到 Unity 控制台

**实现**:
```csharp
UnityEngine.Debug.Log(msg);
```

---

### Warning(string msg)

**签名**:
```csharp
public void Warning(string msg)
```

**职责**: 输出警告日志到 Unity 控制台

**实现**:
```csharp
UnityEngine.Debug.LogWarning(msg);
```

---

### Error(string msg) / Error(Exception e)

**签名**:
```csharp
public void Error(string msg)
public void Error(Exception e)
```

**职责**: 输出错误日志到 Unity 控制台

**实现**:
```csharp
public void Error(string msg)
{
    UnityEngine.Debug.LogError(msg);
}

public void Error(Exception e)
{
    UnityEngine.Debug.LogException(e);
}
```

**说明**: `LogException` 会输出完整的异常信息和堆栈跟踪

---

### 格式化方法

所有日志级别都提供 `params object[] args` 版本：

```csharp
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
```

---

## Unity 控制台输出效果

### 日志级别对应关系

| Log 级别 | UnityLogger 实现 | Unity 控制台显示 |
|----------|------------------|------------------|
| Trace | Debug.Log | 白色普通日志 |
| Debug | Debug.Log | 白色普通日志 |
| Info | Debug.Log | 白色普通日志 |
| Warning | Debug.LogWarning | 黄色警告日志 |
| Error | Debug.LogError | 红色错误日志 |
| Error(Exception) | Debug.LogException | 红色错误 + 堆栈 |

### 控制台截图示意

```
┌─────────────────────────────────────────────────────────┐
│ Unity Console                                           │
├─────────────────────────────────────────────────────────┤
│ [Info] 游戏启动成功                                     │
│ [Debug] 加载资源：player.prefab                         │
│ [Warning] 资源未找到，使用默认值                        │
│ [Error] 加载失败：network_error                         │
│ Exception: NullReferenceException                       │
│   at GameManager.Start () [0x00000]                     │
└─────────────────────────────────────────────────────────┘
```

---

## 使用示例

### 示例 1: 设置 UnityLogger

```csharp
// 程序启动时设置
void Start()
{
    Log.ILog = new UnityLogger();
    
    // 现在可以使用 Log 类输出日志
    Log.Info("游戏启动");
}
```

### 示例 2: 输出各种级别的日志

```csharp
// 跟踪日志
Log.Trace("进入方法 ProcessData");

// 调试日志
Log.Debug($"加载进度：{progress:P2}");

// 信息日志
Log.Info("场景加载完成");

// 警告日志
Log.Warning("帧率过低：{0} FPS", fps);

// 错误日志
Log.Error("加载失败");

// 异常日志
try
{
    RiskyOperation();
}
catch (Exception ex)
{
    Log.Error(ex); // 输出完整异常和堆栈
}
```

### 示例 3: 格式化日志

```csharp
// 格式化信息
Log.Info("玩家 {0} 等级 {1} HP:{2}", playerName, level, hp);

// 格式化警告
Log.Warning("资源 {0} 加载时间过长：{1:F2}s", resourceName, time);

// 格式化错误
Log.Error("网络请求失败：{0}, 状态码：{1}", url, statusCode);
```

### 示例 4: 条件日志

```csharp
// 只在编辑器模式下输出详细日志
#if UNITY_EDITOR
Log.Debug($"详细数据：{data}");
#endif

// 根据日志级别控制
if (Define.LogLevel <= Log.DebugLevel)
{
    Log.Debug($"性能数据：{performanceData}");
}
```

---

## 与其他日志实现的对比

| 特性 | UnityLogger | FileLogger | NetworkLogger |
|------|-------------|------------|---------------|
| 输出目标 | Unity 控制台 | 文件 | 网络服务器 |
| 实时性 | 实时 | 实时 | 可能延迟 |
| 持久化 | 否 | 是 | 是（服务器端） |
| 性能影响 | 低 | 中 | 高 |
| 适用场景 | 开发调试 | 测试/生产 | 生产监控 |

---

## 阅读指引

### 建议的阅读顺序

1. **理解 UnityLogger 作用** - 为什么需要 Unity 平台的日志实现
2. **看方法实现** - 了解如何调用 Unity Debug API
3. **了解控制台效果** - 理解不同级别的显示效果
4. **对比其他实现** - 理解不同日志后端的特点

### 最值得学习的技术点

1. **适配器模式**: 将 Unity Debug API 适配到 ILog 接口
2. **异常处理**: 使用 LogException 输出完整堆栈
3. **格式化支持**: 使用 LogFormat 支持格式化输出
4. **平台特定**: 针对 Unity 平台的优化实现

---

## 最佳实践

### 1. 开发阶段使用 UnityLogger

```csharp
void Start()
{
#if UNITY_EDITOR
    Log.ILog = new UnityLogger();
#else
    Log.ILog = new FileLogger("game.log");
#endif
}
```

### 2. 组合多个日志后端

```csharp
void Start()
{
    var composite = new CompositeLogger();
    composite.AddLogger(new UnityLogger());
    composite.AddLogger(new FileLogger("game.log"));
    Log.ILog = composite;
}
```

### 3. 使用异常日志

```csharp
// ❌ 不好：只输出消息
catch (Exception ex)
{
    Log.Error(ex.Message);
}

// ✅ 好：输出完整异常
catch (Exception ex)
{
    Log.Error(ex);
}
```

### 4. 避免性能陷阱

```csharp
// ❌ 不好：每帧都格式化字符串
void Update()
{
    Log.Debug($"Position: {transform.position}");
}

// ✅ 好：先检查级别
void Update()
{
    if (Define.LogLevel <= Log.DebugLevel)
    {
        Log.Debug($"Position: {transform.position}");
    }
}
```

---

## Unity 控制台技巧

### 1. 使用搜索过滤

在 Unity 控制台中可以使用关键字搜索：
- 输入 `ERROR` 只看错误日志
- 输入 `WARNING` 只看警告日志
- 输入 `Info` 只看信息日志

### 2. 使用堆栈跟踪定位

点击错误日志可以在代码中定位到出错位置

### 3. 使用 Clear 清除日志

```csharp
// 清除控制台（编辑器模式下）
#if UNITY_EDITOR
UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
#endif
```

---

## 相关文档

- [ILog.cs.md](./ILog.cs.md) - 日志接口定义
- [Log.cs.md](./Log.cs.md) - 日志系统入口

---

*文档生成时间：2026-03-01 | OpenClaw AI 助手*
