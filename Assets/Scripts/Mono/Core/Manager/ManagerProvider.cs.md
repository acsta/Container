# ManagerProvider.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | ManagerProvider.cs |
| **路径** | Assets/Scripts/Mono/Core/Manager/ManagerProvider.cs |
| **所属模块** | 框架层 → Mono/Core/Manager |
| **文件职责** | 整个项目的依赖注入容器，负责所有 Manager 的注册、获取、生命周期管理 |

---

## 类/结构体说明

### ManagerProvider

| 属性 | 说明 |
|------|------|
| **职责** | 服务定位器模式实现，统一管理所有 Manager 的生命周期 |
| **泛型参数** | 无 |
| **继承关系** | 无继承 |
| **实现的接口** | 无（但管理实现 IUpdate/ILateUpdate/IFixedUpdate 的 Manager） |

**设计模式**: 单例模式 + 服务定位器

```csharp
// 单例实现
static ManagerProvider Instance { get; } = new ManagerProvider();
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `Instance` | `ManagerProvider` | `static private` | 单例实例，全局唯一 |
| `managersDictionary` | `UnOrderDoubleKeyDictionary<Type,string,object>` | `private` | 双键字典存储 Manager，支持按类型 + 名称索引 |
| `allManagers` | `LinkedList<object>` | `private` | 所有 Manager 的列表，用于清理 |
| `updateManagers` | `LinkedList<IUpdate>` | `private` | 需要 Update 的 Manager 列表 |
| `lateUpdateManagers` | `LinkedList<ILateUpdate>` | `private` | 需要 LateUpdate 的 Manager 列表 |
| `fixedUpdateManagers` | `LinkedList<IFixedUpdate>` | `private` | 需要 FixedUpdate 的 Manager 列表 |

---

## 方法说明（按重要程度排序）

### RegisterManager<T>(string name = "")

**签名**:
```csharp
public static T RegisterManager<T>(string name = "") where T : class, IManager
```

**职责**: 注册并初始化一个 Manager 实例

**核心逻辑**:
```
1. 检查字典中是否已存在该类型 + 名称的 Manager
2. 如果不存在：
   - 使用 Activator.CreateInstance 创建实例
   - 如果实现 IUpdate，加入 updateManagers 列表
   - 如果实现 ILateUpdate，加入 lateUpdateManagers 列表
   - 如果实现 IFixedUpdate，加入 fixedUpdateManagers 列表
   - 调用 Init() 初始化
   - 加入字典和 allManagers 列表
3. 返回实例
```

**调用者**: Entry.cs（游戏启动时注册所有 Manager）

**被调用者**: `Activator.CreateInstance`, `TypeInfo<T>.Type`

---

### RegisterManager<T,P1>(P1 p1, string name = "")

**签名**:
```csharp
public static T RegisterManager<T,P1>(P1 p1, string name = "") where T : class, IManager<P1>
```

**职责**: 注册并初始化一个带单个参数的 Manager

**核心逻辑**: 与无参版本类似，但调用 `Init(p1)` 传递参数

**调用者**: 需要初始化参数的 Manager（如 SceneManager 需要 SceneGroup）

---

### RegisterManager<T,P1,P2>(P1 p1, P2 p2, string name = "")

**签名**:
```csharp
public static T RegisterManager<T,P1,P2>(P1 p1, P2 p2, string name = "") where T : class, IManager<P1,P2>
```

**职责**: 注册并初始化一个带两个参数的 Manager

---

### RegisterManager<T,P1,P2,P3>(P1 p1, P2 p2, P3 p3, string name = "")

**签名**:
```csharp
public static T RegisterManager<T,P1,P2,P3>(P1 p1, P2 p2, P3 p3, string name = "") where T : class, IManager<P1,P2,P3>
```

**职责**: 注册并初始化一个带三个参数的 Manager

---

### GetManager<T>(string name = "")

**签名**:
```csharp
public static T GetManager<T>(string name = "") where T : class, IManagerDestroy
```

**职责**: 根据类型和名称获取已注册的 Manager

**核心逻辑**:
```
1. 获取泛型类型 TypeInfo<T>.Type
2. 从 managersDictionary 中查找
3. 找到则返回，否则返回 null
```

**调用者**: 几乎所有需要访问其他 Manager 的代码

**被调用者**: `managersDictionary.TryGetValue`

---

### RemoveManager<T>(string name = "")

**签名**:
```csharp
public static void RemoveManager<T>(string name = "")
```

**职责**: 移除并销毁一个 Manager

**核心逻辑**:
```
1. 从字典中找到 Manager
2. 从 updateManagers/lateUpdateManagers/fixedUpdateManagers 中移除
3. 从字典和 allManagers 中移除
4. 调用 Destroy() 清理资源
```

---

### Clear()

**签名**:
```csharp
public static void Clear()
```

**职责**: 清空所有 Manager，用于场景切换或游戏重启

**核心逻辑**:
```
1. 清空所有字典和列表
2. 遍历 allManagers，逐个调用 Destroy()
3. 清空 allManagers
```

---

### Update() / LateUpdate() / FixedUpdate()

**签名**:
```csharp
public static void Update()
public static void LateUpdate()
public static void FixedUpdate()
```

**职责**: 在 Unity 生命周期中驱动所有 Manager 的更新

**核心逻辑**:
```
1. 遍历对应的 Manager 列表（updateManagers/lateUpdateManagers/fixedUpdateManagers）
2. 逐个调用 Update()/LateUpdate()/FixedUpdate()
3. 处理 UnityLifeTimeHelper 的异步任务队列
```

**调用者**: Unity 生命周期（通过 Entry.cs 转发）

**被调用者**: 各个 Manager 的 Update 方法

---

## Unity 生命周期集成

### 与 Unity 的集成方式

ManagerProvider 本身不是 MonoBehaviour，但通过静态方法集成到 Unity 生命周期：

```csharp
// Entry.cs 中调用
void Update() => ManagerProvider.Update();
void LateUpdate() => ManagerProvider.LateUpdate();
void FixedUpdate() => ManagerProvider.FixedUpdate();
```

### 生命周期方法说明

| 方法 | 作用 | 典型用途 |
|------|------|----------|
| `Update()` | 每帧调用 | 逻辑更新、输入处理 |
| `LateUpdate()` | Update 后调用 | 相机跟随、UI 更新 |
| `FixedUpdate()` | 固定时间间隔 | 物理计算 |

### 协程支持

通过 `UnityLifeTimeHelper` 实现协程支持：

```csharp
// Update 完成后处理异步任务
int count = UnityLifeTimeHelper.UpdateFinishTask.Count;
while (count-- > 0)
{
    ETTask task = UnityLifeTimeHelper.UpdateFinishTask.Dequeue();
    task.SetResult();
}
```

---

## 阅读指引

### 建议的阅读顺序

1. **先看 IManager.cs** - 了解 Manager 接口定义
2. **再看 ManagerProvider.cs 的字段** - 理解数据结构
3. **重点看 RegisterManager** - 理解注册逻辑
4. **最后看 Update 方法** - 理解生命周期驱动

### 最值得学习的技术点

1. **服务定位器模式**: 通过静态方法提供全局访问点
2. **双键字典**: 支持按类型 + 名称索引 Manager
3. **接口分离**: IUpdate/ILateUpdate/IFixedUpdate 分离更新逻辑
4. **泛型约束**: `where T : class, IManager` 确保类型安全
5. **自动生命周期管理**: 根据实现的接口自动加入对应更新列表

---

## 相关文档

- [IManager.cs.md](./IManager.cs.md) - Manager 接口定义
- [Entry.cs.md](../../Code/Entry.cs.md) - 游戏入口，注册所有 Manager
- [PROJECT_DOCUMENTATION.md](../../../../PROJECT_DOCUMENTATION.md) - 项目全景文档

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
