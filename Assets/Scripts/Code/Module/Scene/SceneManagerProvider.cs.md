# SceneManagerProvider.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | SceneManagerProvider.cs |
| **路径** | Assets/Scripts/Code/Module/Scene/SceneManagerProvider.cs |
| **所属模块** | 框架层 → Code/Module/Scene |
| **文件职责** | 场景管理器提供者抽象基类 |

---

## 类/结构体说明

### SceneManagerProvider

| 属性 | 说明 |
|------|------|
| **职责** | 定义场景管理器提供者的抽象接口，支持多场景管理器 |
| **泛型参数** | 无 |
| **继承关系** | 抽象类 |

```csharp
public abstract class SceneManagerProvider
{
    // 场景管理器提供者
}
```

---

## 方法说明

### GetName()

**签名**:
```csharp
public abstract string GetName()
```

**职责**: 获取场景管理器名称

**返回**: 场景管理器名称字符串

**要求**: 子类必须实现

**用途**: 用于 ManagerProvider 注册和查找

---

### GetManager<T>()

**签名**:
```csharp
public T GetManager<T>() where T : class, IManagerDestroy
```

**职责**: 获取指定类型的管理器

**返回**: 管理器实例

**核心逻辑**:
```
1. 调用 GetName() 获取场景名称
2. 调用 ManagerProvider.GetManager<T>(name)
3. 返回管理器
```

**调用者**: 场景代码

**使用示例**:
```csharp
// 在场景管理器中
var uiManager = GetManager<UIManager>();
var audioManager = GetManager<AudioManager>();
```

---

### RegisterManager<T>()

**签名**:
```csharp
public T RegisterManager<T>() where T : class, IManager
```

**职责**: 注册管理器

**返回**: 注册的管理器实例

**核心逻辑**:
```
1. 调用 GetName() 获取场景名称
2. 调用 ManagerProvider.RegisterManager<T>(name)
3. 返回管理器
```

**调用者**: 场景初始化代码

---

### RegisterManager<T, P1>() / RegisterManager<T, P1, P2>() / RegisterManager<T, P1, P2, P3>()

**签名**:
```csharp
public T RegisterManager<T, P1>(P1 p1) where T : class, IManager<P1>
public T RegisterManager<T, P1, P2>(P1 p1, P2 p2) where T : class, IManager<P1, P2>
public T RegisterManager<T, P1, P2, P3>(P1 p1, P2 p2, P3 p3) where T : class, IManager<P1, P2, P3>
```

**职责**: 注册带参数的管理器

**核心逻辑**:
```
1. 调用 GetName() 获取场景名称
2. 调用 ManagerProvider.RegisterManager<T, P1>(p1, name)
3. 返回管理器
```

---

### RemoveManager<T>()

**签名**:
```csharp
public void RemoveManager<T>()
```

**职责**: 移除管理器

**核心逻辑**:
```
1. 调用 GetName() 获取场景名称
2. 调用 ManagerProvider.RemoveManager<T>(name)
```

---

## 使用场景

### 场景 1: 多场景管理器

游戏可能有多个场景管理器，每个管理不同的场景类型：

```
SceneManagerProvider (抽象基类)
    ├── HomeSceneManagerProvider (家园场景)
    ├── BattleSceneManagerProvider (战斗场景)
    └── GuideSceneManagerProvider (引导场景)
```

### 场景 2: 场景隔离

不同场景的管理器相互隔离，避免冲突：

```csharp
// 家园场景管理器
public class HomeSceneManager : SceneManagerProvider
{
    public override string GetName() => "Home";
    
    public void Init()
    {
        // 注册家园场景专属管理器
        RegisterManager<HomeUIManager>();
        RegisterManager<HomeAudioManager>();
    }
}

// 战斗场景管理器
public class BattleSceneManager : SceneManagerProvider
{
    public override string GetName() => "Battle";
    
    public void Init()
    {
        // 注册战斗场景专属管理器
        RegisterManager<BattleUIManager>();
        RegisterManager<BattleAudioManager>();
    }
}
```

---

## 使用示例

### 示例 1: 实现场景管理器

```csharp
public class HomeSceneManagerProvider : SceneManagerProvider
{
    public override string GetName()
    {
        return "HomeScene";
    }
    
    public void Init()
    {
        // 注册家园场景管理器
        RegisterManager<HomeUIManager>();
        RegisterManager<HomeAudioManager>();
    }
    
    public void Cleanup()
    {
        // 移除管理器
        RemoveManager<HomeUIManager>();
        RemoveManager<HomeAudioManager>();
    }
}
```

### 示例 2: 获取管理器

```csharp
// 在场景代码中
HomeSceneManagerProvider provider = GetProvider();

// 获取管理器
UIManager uiManager = provider.GetManager<UIManager>();
AudioManager audioManager = provider.GetManager<AudioManager>();
```

### 示例 3: 注册带参数管理器

```csharp
// 注册带参数的管理器
var configManager = RegisterManager<ConfigManager, string>("HomeConfig");

// 使用
configManager.LoadConfig("home_settings");
```

---

## 与 ManagerProvider 的关系

```
SceneManagerProvider (场景级)
    ↓
    调用 GetName() 获取场景名称
    ↓
ManagerProvider (全局)
    ↓
    根据场景名称 + 管理器类型查找
    ↓
返回管理器实例
```

---

## 相关文档

- [SceneManager.cs.md](./SceneManager.cs.md) - 场景管理器
- [IScene.cs.md](./IScene.cs.md) - 场景接口
- [ManagerProvider.cs.md](../../Mono/Core/Manager/ManagerProvider.cs.md) - 管理器提供者

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
