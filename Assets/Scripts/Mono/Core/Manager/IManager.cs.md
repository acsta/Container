# IManager.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | IManager.cs |
| **路径** | Assets/Scripts/Mono/Core/Manager/IManager.cs |
| **所属模块** | Mono 层 → Core/Manager |
| **文件职责** | 定义管理器接口体系 |

---

## 接口说明

### IManagerDestroy

| 属性 | 说明 |
|------|------|
| **职责** | 管理器销毁接口，所有管理器必须实现 |
| **继承关系** | 基础接口 |

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
| **职责** | 无参数管理器接口 |
| **继承关系** | 继承 IManagerDestroy |

```csharp
public interface IManager : IManagerDestroy
{
    public void Init();
}
```

**用途**: 用于不需要初始化参数的管理器

---

### IManager\<P1\>

| 属性 | 说明 |
|------|------|
| **职责** | 单参数管理器接口 |
| **继承关系** | 继承 IManagerDestroy |

```csharp
public interface IManager<P1> : IManagerDestroy
{
    public void Init(P1 p1);
}
```

**用途**: 用于需要一个初始化参数的管理器

---

### IManager\<P1, P2\>

| 属性 | 说明 |
|------|------|
| **职责** | 双参数管理器接口 |
| **继承关系** | 继承 IManagerDestroy |

```csharp
public interface IManager<P1, P2> : IManagerDestroy
{
    public void Init(P1 sceneGroups, P2 p2);
}
```

**用途**: 用于需要两个初始化参数的管理器

---

### IManager\<P1, P2, P3\>

| 属性 | 说明 |
|------|------|
| **职责** | 三参数管理器接口 |
| **继承关系** | 继承 IManagerDestroy |

```csharp
public interface IManager<P1, P2, P3> : IManagerDestroy
{
    public void Init(P1 p1, P2 p2, P3 p3);
}
```

**用途**: 用于需要三个初始化参数的管理器

---

## 设计模式

### 接口泛型化

通过泛型参数支持不同数量的初始化参数：

```
IManagerDestroy (基础)
    ├── IManager (0 参数)
    ├── IManager<P1> (1 参数)
    ├── IManager<P1, P2> (2 参数)
    └── IManager<P1, P2, P3> (3 参数)
```

### 统一销毁接口

所有管理器都实现 `IManagerDestroy`，确保统一的销毁流程。

---

## 使用场景

### 场景 1: 简单管理器

```csharp
public class AudioManager : IManager
{
    public void Init()
    {
        // 初始化音频系统
    }
    
    public void Destroy()
    {
        // 清理音频资源
    }
}
```

### 场景 2: 带参数管理器

```csharp
public class ConfigManager : IManager<string>
{
    public void Init(string configPath)
    {
        // 从指定路径加载配置
    }
    
    public void Destroy()
    {
        // 清理配置
    }
}
```

### 场景 3: 多参数管理器

```csharp
public class SceneManager : IManager<string, int, bool>
{
    public void Init(string sceneName, int sceneId, bool isAsync)
    {
        // 初始化场景
    }
    
    public void Destroy()
    {
        // 清理场景
    }
}
```

---

## 与 ManagerProvider 的配合

```csharp
// 注册管理器
var manager = ManagerProvider.RegisterManager<AudioManager>();

// 注册带参数管理器
var configManager = ManagerProvider.RegisterManager<ConfigManager, string>("/configs");

// 获取管理器
var audioManager = ManagerProvider.GetManager<AudioManager>();

// 移除管理器
ManagerProvider.RemoveManager<AudioManager>();
```

---

## 相关文档

- [ManagerProvider.cs.md](./ManagerProvider.cs.md) - 管理器提供者
- [ObjectPool.cs.md](../ObjectPool.cs.md) - 对象池

---

*文档生成时间：2026-03-01 | OpenClaw AI 助手*
