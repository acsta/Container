# IManager.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | IManager.cs |
| **路径** | Assets/Scripts/Mono/Core/Manager/IManager.cs |
| **所属模块** | 框架层 → Mono/Core/Manager |
| **文件职责** | 定义 Manager 的标准接口，规范所有 Manager 的生命周期行为 |

---

## 类/结构体说明

### IManagerDestroy

| 属性 | 说明 |
|------|------|
| **职责** | 定义 Manager 的销毁方法，用于资源清理 |
| **泛型参数** | 无 |
| **继承关系** | 无 |
| **实现的接口** | 无 |

```csharp
public interface IManagerDestroy
{
    public void Destroy();
}
```

---

### IManager

| 属性 | 说明 |
|------|------|
| **职责** | 标准 Manager 接口，无参数初始化 |
| **泛型参数** | 无 |
| **继承关系** | 继承自 IManagerDestroy |
| **实现的接口** | IManagerDestroy |

```csharp
public interface IManager : IManagerDestroy
{
    public void Init();
}
```

---

### IManager<P1>

| 属性 | 说明 |
|------|------|
| **职责** | 带单个参数的 Manager 接口 |
| **泛型参数** | P1 - 第一个初始化参数 |
| **继承关系** | 继承自 IManagerDestroy |
| **实现的接口** | IManagerDestroy |

```csharp
public interface IManager<P1> : IManagerDestroy
{
    public void Init(P1 p1);
}
```

---

### IManager<P1,P2>

| 属性 | 说明 |
|------|------|
| **职责** | 带两个参数的 Manager 接口 |
| **泛型参数** | P1, P2 - 初始化参数 |
| **继承关系** | 继承自 IManagerDestroy |
| **实现的接口** | IManagerDestroy |

```csharp
public interface IManager<P1,P2> : IManagerDestroy
{
    public void Init(P1 sceneGroups, P2 p2);
}
```

**注意**: 第一个参数命名为 `sceneGroups`，暗示这个接口可能最初为 SceneManager 设计

---

### IManager<P1,P2,P3>

| 属性 | 说明 |
|------|------|
| **职责** | 带三个参数的 Manager 接口 |
| **泛型参数** | P1, P2, P3 - 初始化参数 |
| **继承关系** | 继承自 IManagerDestroy |
| **实现的接口** | IManagerDestroy |

```csharp
public interface IManager<P1,P2,P3> : IManagerDestroy
{
    public void Init(P1 p1, P2 p2, P3 p3);
}
```

---

## 字段与属性

本文件只包含接口定义，无字段或属性。

---

## 方法说明

### IManagerDestroy.Destroy()

**签名**:
```csharp
public void Destroy();
```

**职责**: 清理 Manager 占用的资源

**实现示例**:
```csharp
public class UIManager : IManager
{
    public void Init()
    {
        // 初始化 UI 系统
    }
    
    public void Destroy()
    {
        // 关闭所有窗口
        // 清理事件监听
        // 释放资源
    }
}
```

---

### IManager.Init()

**签名**:
```csharp
public void Init();
```

**职责**: 初始化 Manager

**调用时机**: ManagerProvider.RegisterManager 时自动调用

---

### IManager<P>.Init(P p1)

**签名**:
```csharp
public void Init(P1 p1);
```

**职责**: 带参数初始化 Manager

**典型用途**: 需要外部依赖或配置时使用

---

## 设计模式

### 接口分离原则

通过多个接口定义不同的初始化方式：

```
IManagerDestroy (基础销毁接口)
    ↑
IManager (无参初始化)
IManager<P1> (单参初始化)
IManager<P1,P2> (双参初始化)
IManager<P1,P2,P3> (三参初始化)
```

### 泛型接口

使用泛型支持灵活的参数类型：

```csharp
// SceneManager 需要 SceneGroup 参数
public class SceneManager : IManager<SceneGroup>
{
    public void Init(SceneGroup sceneGroup) { }
}

// ConfigManager 无需参数
public class ConfigManager : IManager
{
    public void Init() { }
}
```

---

## 与 ManagerProvider 的协作

### 注册流程

```csharp
// ManagerProvider 根据接口类型调用不同的 Init
public static T RegisterManager<T,P1>(P1 p1, string name = "") 
    where T : class, IManager<P1>
{
    res = Activator.CreateInstance(type) as T;
    (res as T).Init(p1);  // ← 调用带参数的 Init
    // ...
}
```

### 销毁流程

```csharp
public static void RemoveManager<T>(string name = "")
{
    // ...
    (res as IManagerDestroy)?.Destroy();  // ← 调用 Destroy
}
```

---

## 阅读指引

### 建议的阅读顺序

1. **先看 IManagerDestroy** - 最基础的接口
2. **再看 IManager** - 标准无参版本
3. **最后看泛型版本** - 理解参数传递

### 最值得学习的技术点

1. **接口继承**: IManager 继承 IManagerDestroy，确保所有 Manager 都可销毁
2. **泛型接口**: 支持不同类型和数量的初始化参数
3. **约束设计**: ManagerProvider 通过泛型约束确保类型安全

---

## 实现该接口的 Manager 示例

| Manager | 接口 | 说明 |
|---------|------|------|
| `UIManager` | `IManager` | 无需外部参数 |
| `ConfigManager` | `IManager` | 无需外部参数 |
| `SceneManager` | `IManager<SceneGroup>` | 需要 SceneGroup |
| `EntityManager` | `IManager<Scene>` | 需要 Scene |

---

## 相关文档

- [ManagerProvider.cs.md](./ManagerProvider.cs.md) - Manager 注册和管理
- [Entry.cs.md](../../Code/Entry.cs.md) - Manager 注册示例
- [PROJECT_DOCUMENTATION.md](../../../../PROJECT_DOCUMENTATION.md) - 项目架构说明

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
