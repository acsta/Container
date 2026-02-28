# UnityLogger.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|------|
| **文件名** | UnityLogger.cs |
| **路径** | Assets/Scripts/Mono/Module/Log/UnityLogger.cs |
| **所属模块** | 框架层 → Mono/Module/Log |
| **文件职责** | 提供 Unity 平台的日志实现，封装 UnityEngine.Debug |

---

## 类/结构体说明

### UnityLogger

| 属性 | 说明 |
|------|------|
| **职责** | ILog 接口的 Unity 平台实现，将日志输出到 Unity Console |
| **泛型参数** | 无 |
| **继承关系** | 实现 `ILog` |
| **实现的接口** | `ILog` |

**设计模式**: 适配器模式

```csharp
// 使用示例（通过 Log 工具类）
Log.Debug("调试信息");
Log.Info("普通信息");
Log.Error("错误信息");
```

---

## 方法说明

### Trace

**签名**:
```csharp
public void Trace(string msg)
```

**职责**: 记录追踪级别日志

**实现**:
```csharp
UnityEngine.Debug.Log(msg);
```

---

### Debug

**签名**:
```csharp
public void Debug(string msg)
```

**职责**: 记录调试级别日志

**实现**:
```csharp
UnityEngine.Debug.Log(msg);
```

---

### Info

**签名**:
```csharp
public void Info(string msg)
```

**职责**: 记录信息级别日志

**实现**:
```csharp
UnityEngine.Debug.Log(msg);
```

---

### Warning

**签名**:
```csharp
public void Warning(string msg)
```

**职责**: 记录警告级别日志

**实现**:
```csharp
UnityEngine.Debug.LogWarning(msg);
```

---

### Error

**签名**:
```csharp
public void Error(string msg)
public void Error(Exception e)
```

**职责**: 记录错误级别日志

**实现**:
```csharp
UnityEngine.Debug.LogError(msg);
UnityEngine.Debug.LogException(e);
```

---

### 格式化方法

**签名**:
```csharp
public void Debug(string message, params object[] args)
public void Info(string message, params object[] args)
public void Warning(string message, params object[] args)
public void Error(string message, params object[] args)
public void Trace(string message, params object[] args)
```

**职责**: 记录格式化日志

**实现**:
```csharp
UnityEngine.Debug.LogFormat(message, args);
```

**使用示例**:
```csharp
Log.Debug("玩家 {0} 等级提升到 {1}", playerName, newLevel);
```

---

## 使用示例

### 示例 1: 基础日志

```csharp
void Start()
{
    Log.Debug("游戏启动");
    Log.Info($"加载场景：{sceneName}");
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

### 示例 3: 格式化日志

```csharp
Log.Debug("FPS: {0}, Memory: {1}MB", fps, GC.GetTotalMemory(false) / 1024 / 1024);
```

---

## 相关文档

- [ILog.cs.md](./ILog.cs.md) - 日志接口
- [Log.cs.md](./Log.cs.md) - 日志工具类

---

*文档生成时间：2026-02-28 | OpenClaw AI 助手*
