# Messager.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | Messager.cs |
| **路径** | Assets/Scripts/Mono/Module/Messager/Messager.cs |
| **所属模块** | 框架层 → Mono/Module/Messager |
| **文件职责** | 消息事件系统，提供观察者模式的实现 |

---

## 类/结构体说明

### Messager

| 属性 | 说明 |
|------|------|
| **职责** | 全局消息事件系统，支持订阅/发布模式 |
| **泛型参数** | 无 |
| **继承关系** | 无继承 |
| **实现的接口** | `IManager` |

**设计模式**: 观察者模式（发布 - 订阅）+ 单例模式

```csharp
// 单例实现
public static Messager Instance { get; private set; }

// 通过 ManagerProvider 注册
ManagerProvider.RegisterManager<Messager>();
```

---

## 字段与属性

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `Instance` | `Messager` | `public static` | 单例实例，全局访问点 |
| `evtGroup` | `Dictionary<long, MultiMapSet<int, MulticastDelegate>>` | `private` | 事件分组存储 {groupId: {eventId: [delegates]}} |

---

## 方法说明（按重要程度排序）

### AddListener(...)

**签名** (0-5 个参数重载):
```csharp
public void AddListener(long id, int name, Action evt)
public void AddListener<P1>(long id, int name, Action<P1> evt)
public void AddListener<P1, P2>(long id, int name, Action<P1, P2> evt)
// ... 最多 5 个参数
```

**职责**: 订阅事件

**参数**:
- `id`: 事件组 ID（用于分组管理）
- `name`: 事件名称/类型
- `evt`: 回调委托

**调用者**: 需要监听事件的代码

**使用示例**:
```csharp
// 订阅无参数事件
Messager.Instance.AddListener(0, MessageId.OnGameStart, OnGameStart);

// 订阅带参数事件
Messager.Instance.AddListener<int, string>(0, MessageId.OnPlayerJoin, OnPlayerJoin);

void OnGameStart() { }
void OnPlayerJoin(int playerId, string playerName) { }
```

---

### RemoveListener(...)

**签名** (0-5 个参数重载):
```csharp
public void RemoveListener(long id, int name, Action evt)
public void RemoveListener<P1>(long id, int name, Action<P1> evt)
// ... 最多 5 个参数
```

**职责**: 取消订阅事件

**调用者**: 不再需要监听事件的代码

**使用示例**:
```csharp
// 取消订阅
Messager.Instance.RemoveListener(0, MessageId.OnGameStart, OnGameStart);
```

---

### Broadcast(...)

**签名** (0-5 个参数重载):
```csharp
public void Broadcast(long id, int name)
public void Broadcast<P1>(long id, int name, P1 p1)
public void Broadcast<P1, P2>(long id, int name, P1 p1, P2 p2)
// ... 最多 5 个参数
```

**职责**: 广播事件，通知所有订阅者

**参数**:
- `id`: 事件组 ID
- `name`: 事件名称
- `p1, p2, ...`: 传递给回调的参数

**核心逻辑**:
```
1. 从 evtGroup 查找事件组
2. 从事件组查找对应事件的委托列表
3. 遍历并调用所有委托
4. 支持多态：如果委托签名不匹配，使用 DynamicInvoke
```

**调用者**: 触发事件的代码

**使用示例**:
```csharp
// 广播无参数事件
Messager.Instance.Broadcast(0, MessageId.OnGameStart);

// 广播带参数事件
Messager.Instance.Broadcast(0, MessageId.OnPlayerJoin, playerId, playerName);
```

---

## 事件分组机制

### 为什么需要分组？

`evtGroup` 使用两层结构：
```
evtGroup: Dictionary<long, MultiMapSet<int, MulticastDelegate>>
    ↓
groupId (long) → eventId (int) → [delegates]
```

**优点**:
1. **隔离性**: 不同模块使用不同 groupId，避免事件冲突
2. **批量管理**: 可以按组清理事件
3. **性能**: 缩小查找范围

### groupId 约定

| groupId | 用途 |
|---------|------|
| `0` | 全局事件（默认） |
| `1` | UI 事件 |
| `2` | 游戏逻辑事件 |
| `3` | 网络事件 |
| `entityId` | 实体私有事件 |

---

## 阅读指引

### 建议的阅读顺序

1. **理解消息系统作用** - 为什么需要 Messager
2. **看 AddListener** - 理解事件订阅
3. **看 Broadcast** - 理解事件触发
4. **了解分组机制** - 理解 evtGroup 结构

### 最值得学习的技术点

1. **泛型重载**: 支持 0-5 个参数的泛型方法
2. **多态支持**: Broadcast 中使用 DynamicInvoke 支持多态
3. **分组管理**: Dictionary + MultiMapSet 两层结构
4. **委托列表**: 使用 MulticastDelegate 存储多个订阅者

---

## 使用示例

### 示例 1: 基础用法

```csharp
// 定义事件 ID
public static class MessageId
{
    public const int OnGameStart = 1001;
    public const int OnPlayerJoin = 1002;
    public const int OnGameEnd = 1003;
}

// 订阅事件
void OnEnable()
{
    Messager.Instance.AddListener(0, MessageId.OnGameStart, OnGameStart);
    Messager.Instance.AddListener<int, string>(0, MessageId.OnPlayerJoin, OnPlayerJoin);
}

// 取消订阅
void OnDisable()
{
    Messager.Instance.RemoveListener(0, MessageId.OnGameStart, OnGameStart);
    Messager.Instance.RemoveListener<int, string>(0, MessageId.OnPlayerJoin, OnPlayerJoin);
}

// 触发事件
void StartGame()
{
    Messager.Instance.Broadcast(0, MessageId.OnGameStart);
}

void PlayerJoin(int id, string name)
{
    Messager.Instance.Broadcast(0, MessageId.OnPlayerJoin, id, name);
}
```

### 示例 2: 实体私有事件

```csharp
// 实体使用自己的 ID 作为 groupId
public class Entity
{
    public long Id { get; set; }
    
    public void Subscribe(Action<int> onHpChange)
    {
        // 使用实体 ID 作为 groupId
        Messager.Instance.AddListener(Id, MessageId.OnHpChange, onHpChange);
    }
    
    public void TakeDamage(int damage)
    {
        Hp -= damage;
        // 触发私有事件
        Messager.Instance.Broadcast(Id, MessageId.OnHpChange, Hp);
    }
}
```

### 示例 3: UI 事件

```csharp
// UI 模块使用 groupId = 1
public class UIManager : IManager
{
    public void Init()
    {
        // 订阅 UI 相关事件
        Messager.Instance.AddListener(1, MessageId.OnOpenWindow, OnOpenWindow);
        Messager.Instance.AddListener(1, MessageId.OnCloseWindow, OnCloseWindow);
    }
    
    public void OpenWindow(string windowName)
    {
        // 触发 UI 事件
        Messager.Instance.Broadcast(1, MessageId.OnOpenWindow, windowName);
    }
}
```

---

## 注意事项

### ⚠️ 内存泄漏

订阅事件后必须取消订阅：

```csharp
// ❌ 错误：只订阅不取消
void OnEnable()
{
    Messager.Instance.AddListener(0, MessageId.OnEvent, OnEvent);
}

// ✅ 正确：订阅和取消配对
void OnEnable()
{
    Messager.Instance.AddListener(0, MessageId.OnEvent, OnEvent);
}

void OnDisable()
{
    Messager.Instance.RemoveListener(0, MessageId.OnEvent, OnEvent);
}
```

### ⚠️ 异常处理

Broadcast 中某个回调抛出异常会影响其他回调：

```csharp
// 建议在回调中处理异常
void OnEvent()
{
    try
    {
        // 逻辑
    }
    catch (Exception ex)
    {
        Log.Error(ex);
    }
}
```

---

## 相关文档

- [MessageId.cs.md](../Const/MessageId.cs.md) - 消息 ID 定义
- [ManagerProvider.cs.md](../../Core/Manager/ManagerProvider.cs.md) - Manager 注册

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
