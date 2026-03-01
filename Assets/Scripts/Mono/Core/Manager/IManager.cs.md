# IManager.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | IManager.cs |
| **路径** | Assets/Scripts/Mono/Core/Manager/IManager.cs |
| **所属模块** | 框架层 → Mono/Core/Manager |
| **文件职责** | 定义 Manager 的标准接口，规范 Manager 的生命周期和行为 |

---

## 接口说明

### IManagerDestroy

**定义**:
```csharp
public interface IManagerDestroy
{
    void Destroy();
}
```

| 属性 | 说明 |
|------|------|
| **职责** | 定义 Manager 的销毁方法 |
| **用途** | 所有 Manager 必须实现此接口，确保能被正确清理 |

---

### IManager

**定义**:
```csharp
public interface IManager : IManagerDestroy
{
    void Init();
}
```

| 属性 | 说明 |
|------|------|
| **职责** | 定义无参数初始化的 Manager 接口 |
| **继承** | 继承 `IManagerDestroy` |
| **用途** | 适用于不需要初始化参数的 Manager |

---

### IManager<P1>

**定义**:
```csharp
public interface IManager<P1> : IManagerDestroy
{
    void Init(P1 p1);
}
```

| 属性 | 说明 |
|------|------|
| **职责** | 定义带一个参数初始化的 Manager 接口 |
| **泛型参数** | `P1` - 第一个初始化参数类型 |
| **用途** | 适用于需要一个初始化参数的 Manager |

---

### IManager<P1, P2>

**定义**:
```csharp
public interface IManager<P1, P2> : IManagerDestroy
{
    void Init(P1 p1, P2 p2);
}
```

| 属性 | 说明 |
|------|------|
| **职责** | 定义带两个参数初始化的 Manager 接口 |
| **泛型参数** | `P1` - 第一个初始化参数类型<br>`P2` - 第二个初始化参数类型 |
| **用途** | 适用于需要两个初始化参数的 Manager |

---

### IManager<P1, P2, P3>

**定义**:
```csharp
public interface IManager<P1, P2, P3> : IManagerDestroy
{
    void Init(P1 p1, P2 p2, P3 p3);
}
```

| 属性 | 说明 |
|------|------|
| **职责** | 定义带三个参数初始化的 Manager 接口 |
| **泛型参数** | `P1` - 第一个初始化参数类型<br>`P2` - 第二个初始化参数类型<br>`P3` - 第三个初始化参数类型 |
| **用途** | 适用于需要三个初始化参数的 Manager |

---

## 接口继承关系

```
IManagerDestroy
    └── IManager
    └── IManager<P1>
    └── IManager<P1, P2>
    └── IManager<P1, P2, P3>
```

---

## 相关更新接口

除了初始化接口，Manager 还可以实现以下更新接口：

### IUpdate
```csharp
public interface IUpdate
{
    void Update();
}
```
**用途**: 每帧更新（对应 Unity 的 Update）

### ILateUpdate
```csharp
public interface ILateUpdate
{
    void LateUpdate();
}
```
**用途**: 每帧延迟更新（对应 Unity 的 LateUpdate）

### IFixedUpdate
```csharp
public interface IFixedUpdate
{
    void FixedUpdate();
}
```
**用途**: 固定时间步长更新（对应 Unity 的 FixedUpdate）

---

## 使用示例

### 示例 1: 基础 Manager

```csharp
public class TimerManager : IManager, IUpdate
{
    public static TimerManager Instance { get; private set; }
    
    public void Init()
    {
        Instance = this;
        // 初始化定时器系统
    }
    
    public void Destroy()
    {
        Instance = null;
        // 清理资源
    }
    
    public void Update()
    {
        // 每帧检查定时器
    }
}

// 注册
ManagerProvider.RegisterManager<TimerManager>();
```

### 示例 2: 带参数的 Manager

```csharp
public class ConfigManager : IManager<string>
{
    public static ConfigManager Instance { get; private set; }
    private string configPath;
    
    public void Init(string path)
    {
        Instance = this;
        configPath = path;
        LoadConfig(path);
    }
    
    public void Destroy()
    {
        Instance = null;
    }
    
    private void LoadConfig(string path)
    {
        // 加载配置
    }
}

// 注册带参数
ManagerProvider.RegisterManager<ConfigManager, string>("Assets/Config");
```

### 示例 3: 多更新类型的 Manager

```csharp
public class PhysicsManager : IManager, IUpdate, IFixedUpdate
{
    public void Init() { }
    public void Destroy() { }
    
    public void Update()
    {
        // 每帧逻辑（插值、预测）
    }
    
    public void FixedUpdate()
    {
        // 物理更新（固定时间步长）
    }
}

// 注册后会自动加入 updateManagers 和 fixedUpdateManagers
ManagerProvider.RegisterManager<PhysicsManager>();
```

### 示例 4: 带多个参数的 Manager

```csharp
public class NetworkManager : IManager<string, int, bool>
{
    public static NetworkManager Instance { get; private set; }
    
    public void Init(string host, int port, bool useEncryption)
    {
        Instance = this;
        // 初始化网络连接
        Connect(host, port, useEncryption);
    }
    
    public void Destroy()
    {
        Instance = null;
        Disconnect();
    }
}

// 注册带多个参数
ManagerProvider.RegisterManager<NetworkManager, string, int, bool>(
    "127.0.0.1", 8080, true
);
```

---

## 设计原则

### 1. 生命周期规范化
所有 Manager 遵循统一的 `Init() → Update() → Destroy()` 生命周期

### 2. 接口分离
- `IManagerDestroy` 单独分离，便于只要求销毁能力的场景
- 不同参数数量的接口分开，避免默认参数混乱

### 3. 泛型灵活性
使用泛型参数 `P1, P2, P3` 支持各种初始化需求，同时保持类型安全

### 4. 多态更新
通过 `IUpdate/ILateUpdate/IFixedUpdate` 接口，ManagerProvider 可以统一调度不同类型的更新

---

## 阅读指引

### 建议的阅读顺序

1. **理解接口作用** - 为什么需要统一的 Manager 接口
2. **看接口继承** - 理解 IManagerDestroy 是基础
3. **了解泛型版本** - 理解带参数的初始化
4. **结合 ManagerProvider** - 理解接口如何被使用

### 最值得学习的技术点

1. **接口继承**: `IManager` 继承 `IManagerDestroy`
2. **泛型接口**: `IManager<P1>` 支持参数化初始化
3. **接口组合**: Manager 可实现多个接口（IManager + IUpdate + IFixedUpdate）
4. **生命周期管理**: 统一的 Init/Destroy 模式

---

## 相关文档

- [ManagerProvider.cs.md](./ManagerProvider.cs.md) - 管理器注册与调度中心
- [TimerManager.cs.md](../Module/Timer/TimerManager.cs.md) - 定时器管理器实现示例
- [SceneManager.cs.md](../../Code/Module/Scene/SceneManager.cs.md) - 场景管理器实现示例

---

*文档生成时间：2026-03-01 | OpenClaw AI 助手*
