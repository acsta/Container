# ManagerProvider.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | ManagerProvider.cs |
| **路径** | Assets/Scripts/Mono/Core/Manager/ManagerProvider.cs |
| **所属模块** | Mono 层 → Core/Manager |
| **文件职责** | 全局管理器注册、查找和生命周期管理 |

---

## 类说明

### ManagerProvider

| 属性 | 说明 |
|------|------|
| **职责** | 单例模式的管理器提供者，管理所有游戏管理器的生命周期 |
| **设计模式** | 单例模式 |
| **线程安全** | 主线程访问 |

```csharp
public class ManagerProvider
{
    // 全局单例
    static ManagerProvider Instance { get; } = new ManagerProvider();
}
```

---

## 核心数据结构

### 管理器存储

```csharp
// 双键字典：类型 + 名称 → 管理器实例
UnOrderDoubleKeyDictionary<Type, string, object> managersDictionary;

// 所有管理器链表
LinkedList<object> allManagers;

// 更新链表（按更新类型分类）
LinkedList<IUpdate> updateManagers;
LinkedList<ILateUpdate> lateUpdateManagers;
LinkedList<IFixedUpdate> fixedUpdateManagers;
```

### 数据结构关系

```
ManagerProvider (单例)
    ├── managersDictionary (Type + Name → Manager)
    ├── allManagers (所有管理器)
    ├── updateManagers (Update 更新)
    ├── lateUpdateManagers (LateUpdate 更新)
    └── fixedUpdateManagers (FixedUpdate 更新)
```

---

## 方法说明

### GetManager\<T\>()

**签名**:
```csharp
public static T GetManager<T>(string name = "") where T : class, IManagerDestroy
```

**职责**: 获取指定类型的管理器实例

**参数**:
- `name`: 管理器名称（可选，用于同名多实例）

**返回**: 管理器实例，不存在则返回 null

**核心逻辑**:
```
1. 获取类型信息 TypeInfo<T>.Type
2. 从 managersDictionary 查找 (type, name)
3. 返回找到的管理器或 null
```

**使用示例**:
```csharp
// 获取默认管理器
var audioManager = ManagerProvider.GetManager<AudioManager>();

// 获取命名管理器
var sceneManager = ManagerProvider.GetManager<SceneManager>("HomeScene");
```

---

### RegisterManager\<T\>()

**签名**:
```csharp
public static T RegisterManager<T>(string name = "") where T : class, IManager
```

**职责**: 注册无参数管理器

**核心逻辑**:
```
1. 检查是否已存在
2. 创建实例 Activator.CreateInstance
3. 根据接口类型添加到更新链表 (IUpdate/ILateUpdate/IFixedUpdate)
4. 调用 Init() 初始化
5. 添加到字典和链表
6. 返回实例
```

**使用示例**:
```csharp
// 注册管理器
var audioManager = ManagerProvider.RegisterManager<AudioManager>();
```

---

### RegisterManager\<T, P1\>() / RegisterManager\<T, P1, P2\>() / RegisterManager\<T, P1, P2, P3\>()

**签名**:
```csharp
public static T RegisterManager<T, P1>(P1 p1, string name = "") where T : class, IManager<P1>
public static T RegisterManager<T, P1, P2>(P1 p1, P2 p2, string name = "") where T : class, IManager<P1, P2>
public static T RegisterManager<T, P1, P2, P3>(P1 p1, P2 p2, P3 p3, string name = "") where T : class, IManager<P1, P2, P3>
```

**职责**: 注册带参数的管理器

**核心逻辑**:
```
1. 检查是否已存在
2. 创建实例
3. 添加到更新链表
4. 调用 Init(p1, p2, p3) 带参初始化
5. 添加到字典和链表
6. 返回实例
```

**使用示例**:
```csharp
// 注册带参数管理器
var configManager = ManagerProvider.RegisterManager<ConfigManager, string>("/configs");
var sceneManager = ManagerProvider.RegisterManager<SceneManager, string, int, bool>("Home", 1, true);
```

---

### RemoveManager\<T\>()

**签名**:
```csharp
public static void RemoveManager<T>(string name = "")
```

**职责**: 移除并销毁管理器

**核心逻辑**:
```
1. 从更新链表移除 (IUpdate/ILateUpdate/IFixedUpdate)
2. 从 managersDictionary 移除
3. 从 allManagers 移除
4. 调用 Destroy() 销毁
```

**使用示例**:
```csharp
// 移除管理器
ManagerProvider.RemoveManager<AudioManager>();
```

---

### Clear()

**签名**:
```csharp
public static void Clear()
```

**职责**: 清空所有管理器

**核心逻辑**:
```
1. 清空所有字典和链表
2. 遍历 allManagers 调用 Destroy()
```

**用途**: 场景切换或游戏重置时清理所有管理器

---

### Update() / LateUpdate() / FixedUpdate()

**签名**:
```csharp
public static void Update()
public static void LateUpdate()
public static void FixedUpdate()
```

**职责**: 帧更新驱动

**核心逻辑**:
```
Update():
1. 遍历 updateManagers 调用 Update()
2. 处理 Update 完成的 ETTask

LateUpdate():
1. 遍历 lateUpdateManagers 调用 LateUpdate()
2. 处理 LateUpdate 完成的 ETTask

FixedUpdate():
1. 遍历 fixedUpdateManagers 调用 FixedUpdate()
2. 处理 FixedUpdate 完成的 ETTask
```

**调用者**: Unity 生命周期 (MonoBehaviour.Update/LateUpdate/FixedUpdate)

---

## 管理器生命周期

```
注册 (RegisterManager)
    ↓
    创建实例 (Activator.CreateInstance)
    ↓
    添加到更新链表 (如果实现 IUpdate 等)
    ↓
    初始化 (Init)
    ↓
    每帧更新 (Update/LateUpdate/FixedUpdate)
    ↓
移除 (RemoveManager)
    ↓
    从更新链表移除
    ↓
    销毁 (Destroy)
```

---

## 使用场景

### 场景 1: 全局单例管理器

```csharp
// 音频管理器（全局单例）
public class AudioManager : IManager
{
    public void Init() { /* 初始化 */ }
    public void Destroy() { /* 清理 */ }
}

// 注册
var audioManager = ManagerProvider.RegisterManager<AudioManager>();

// 使用
audioManager.PlaySound("bgm");
```

### 场景 2: 多实例管理器

```csharp
// 场景管理器（多个场景）
public class SceneManager : IManager<string>
{
    public void Init(string sceneName) { /* 初始化场景 */ }
    public void Destroy() { /* 清理场景 */ }
}

// 注册多个场景
ManagerProvider.RegisterManager<SceneManager, string>("HomeScene");
ManagerProvider.RegisterManager<SceneManager, string>("BattleScene");

// 获取指定场景
var homeScene = ManagerProvider.GetManager<SceneManager>("HomeScene");
```

### 场景 3: 带更新的管理器

```csharp
// 需要每帧更新的管理器
public class InputManager : IManager, IUpdate
{
    public void Init() { /* 初始化 */ }
    public void Destroy() { /* 清理 */ }
    public void Update() { /* 处理输入 */ }
}

// 注册后自动加入 Update 循环
ManagerProvider.RegisterManager<InputManager>();
```

---

## 与 Unity 生命周期的集成

```csharp
// 在 MonoBehaviour 中驱动
public class GameBoot : MonoBehaviour
{
    void Update()
    {
        ManagerProvider.Update();
    }
    
    void LateUpdate()
    {
        ManagerProvider.LateUpdate();
    }
    
    void FixedUpdate()
    {
        ManagerProvider.FixedUpdate();
    }
}
```

---

## 相关文档

- [IManager.cs.md](./IManager.cs.md) - 管理器接口定义
- [ObjectPool.cs.md](../ObjectPool.cs.md) - 对象池
- [SceneManagerProvider.cs.md](../../../Code/Module/Scene/SceneManagerProvider.cs.md) - 场景管理器提供者

---

*文档生成时间：2026-03-01 | OpenClaw AI 助手*
