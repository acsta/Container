# LogManager.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | LogManager.cs |
| **路径** | Assets/Scripts/Code/Module/Log/LogManager.cs |
| **所属模块** | 框架层 → Code/Module/Log |
| **文件职责** | 日志管理器，配置日志系统和日志上传 |

---

## 类/结构体说明

### LogManager

| 属性 | 说明 |
|------|------|
| **职责** | 初始化日志系统，配置日志级别，支持日志上传 |
| **泛型参数** | 无 |
| **继承关系** | 无 |
| **实现的接口** | `IManager` |

```csharp
public class LogManager : IManager
{
    // 日志管理器
}
```

---

## 字段与属性

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `Instance` | `LogManager` | `public static` | 单例实例 |
| `logger` | `InterceptorLogger` | `private` | 拦截器日志实例 |

---

## 方法说明（按重要程度排序）

### Init()

**签名**:
```csharp
public void Init()
```

**职责**: 初始化日志系统

**核心逻辑**:
```
1. 设置 Instance = this
2. 创建 InterceptorLogger 实例
3. 设置 Log.ILog = logger（替换默认日志）
4. 设置 logger.CheckLv = Define.LogLevel
5. 设置 Define.LogLevel = 1（启用日志）
```

**日志拦截**:
```csharp
Log.ILog = logger = new InterceptorLogger();
```

**调用者**: `ManagerProvider.Init()`

---

### Destroy()

**签名**:
```csharp
public void Destroy()
```

**职责**: 销毁日志系统

**核心逻辑**:
```
1. 恢复 Define.LogLevel
2. 恢复 Log.ILog = UnityLogger（默认日志）
3. 清空 logger
4. 清空 Instance
```

**调用者**: `ManagerProvider.Destroy()`

---

### PushLog2Server()

**签名**:
```csharp
public void PushLog2Server()
```

**职责**: 上传日志到服务器

**核心逻辑**:
```
// TODO: 实现日志上传
```

**当前状态**: 空实现，待实现

**调用者**: 需要上传日志的场景（如崩溃上报）

---

## 阅读指引

### 建议的阅读顺序

1. **理解管理器作用** - LogManager 管理什么
2. **看 Init** - 理解日志初始化
3. **看 Destroy** - 理解日志恢复
4. **了解日志上传** - 理解 PushLog2Server

### 最值得学习的技术点

1. **日志拦截**: InterceptorLogger 拦截日志输出
2. **日志级别**: Define.LogLevel 控制日志输出
3. **可替换**: Log.ILog 可替换为不同实现
4. **待实现**: PushLog2Server 待实现日志上传

---

## InterceptorLogger

### 作用

InterceptorLogger 是一个日志拦截器，用于：
- 拦截所有日志输出
- 可以自定义日志处理逻辑
- 支持日志收集、过滤、上传

### 字段

| 字段 | 说明 |
|------|------|
| `CheckLv` | 日志级别检查 |

### 使用

```csharp
// 创建拦截器
InterceptorLogger logger = new InterceptorLogger();

// 设置日志级别
logger.CheckLv = Define.LogLevel;

// 替换默认日志
Log.ILog = logger;
```

---

## 日志级别（Define.LogLevel）

| 级别 | 值 | 说明 |
|------|-----|------|
| None | 0 | 不输出日志 |
| Error | 1 | 只输出错误日志 |
| Warning | 2 | 输出错误 + 警告 |
| Info | 3 | 输出错误 + 警告 + 信息 |
| Debug | 4 | 输出所有日志 |

---

## 使用示例

### 示例 1: 日志初始化

```csharp
// LogManager.Init() 内部:
Log.ILog = new InterceptorLogger();
Define.LogLevel = 1;  // 只输出错误日志
```

### 示例 2: 日志输出

```csharp
// 使用 Log 类输出日志
Log.Info("这是一条信息日志");
Log.Warning("这是一条警告日志");
Log.Error("这是一条错误日志");

// 根据日志级别，可能不会输出
```

### 示例 3: 日志上传（待实现）

```csharp
// 崩溃时上传日志
try
{
    // 游戏逻辑
}
catch (Exception ex)
{
    Log.Error(ex);
    LogManager.Instance.PushLog2Server();  // TODO: 实现
}
```

---

## 相关文档

- [InterceptorLogger.cs.md](./InterceptorLogger.cs.md) - 拦截器日志
- [Define.cs.md](../Const/Define.cs.md) - 全局配置

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
