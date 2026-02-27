# CoroutineLockType.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | CoroutineLockType.cs |
| **路径** | Assets/Scripts/Code/Module/CoroutineLock/CoroutineLockType.cs |
| **所属模块** | 框架层 → Code/Module/CoroutineLock |
| **文件职责** | 协程锁类型常量定义 |

---

## 类说明

### CoroutineLockType

| 属性 | 说明 |
|------|------|
| **职责** | 定义协程锁的类型常量 |
| **类型** | class（静态常量） |

```csharp
public class CoroutineLockType
{
    public const int None = 0;
    public const int Resources = 1;
    public const int UIManager = 2;
    public const int UIMsgBox = 3;
    public const int EnableObjView = 4;
    public const int PathQuery = 5;
    public const int Max = 100;  // 必须最大
}
```

---

## 锁类型说明

### None (0)

**说明**: 无锁类型

**用途**: 默认值，表示不使用协程锁

---

### Resources (1)

**说明**: 资源加载锁

**用途**: 防止重复加载同一资源

**使用场景**:
- 加载预制体
- 加载纹理
- 加载音频

**示例**:
```csharp
// 防止重复加载同一预制体
CoroutineLock lockObj = await CoroutineLockManager.Instance.WaitAsync(
    CoroutineLockType.Resources, 
    "Prefabs/Player"
);

try
{
    GameObject prefab = await ResourcesManager.Instance.LoadAsync<GameObject>("Prefabs/Player");
}
finally
{
    lockObj.Dispose();
}
```

---

### UIManager (2)

**说明**: UI 管理器锁

**用途**: 保护 UI 操作线程安全

**使用场景**:
- 打开/关闭窗口
- 切换界面
- UI 状态变更

**示例**:
```csharp
// 确保 UI 操作顺序执行
CoroutineLock lockObj = await CoroutineLockManager.Instance.WaitAsync(
    CoroutineLockType.UIManager, 
    "OpenWindow"
);

try
{
    UIManager.Instance.OpenWindow("UIAuction");
}
finally
{
    lockObj.Dispose();
}
```

---

### UIMsgBox (3)

**说明**: UI 消息框锁

**用途**: 防止同时显示多个消息框

**使用场景**:
- 显示提示框
- 显示确认框
- 显示错误信息

**示例**:
```csharp
// 确保消息框逐个显示
CoroutineLock lockObj = await CoroutineLockManager.Instance.WaitAsync(
    CoroutineLockType.UIMsgBox, 
    "MsgBox"
);

try
{
    await UIManager.Instance.ShowMsgBox("操作成功");
}
finally
{
    lockObj.Dispose();
}
```

---

### EnableObjView (4)

**说明**: 启用对象视图锁

**用途**: 保护对象启用/禁用操作

**使用场景**:
- 启用/禁用 GameObject
- 切换视图状态

---

### PathQuery (5)

**说明**: 路径查询锁

**用途**: 保护寻路查询操作

**使用场景**:
- A*寻路
- 路径规划
- 导航网格查询

---

### Max (100)

**说明**: 最大类型值

**用途**: 用于数组大小计算、边界检查

**注意**: 这个值必须最大，新增类型不能超过此值

---

## 使用示例

### 示例 1: 资源加载锁

```csharp
public async ETTask LoadResource(string path)
{
    // 等待资源锁
    CoroutineLock lockObj = await CoroutineLockManager.Instance.WaitAsync(
        CoroutineLockType.Resources, 
        path
    );
    
    try
    {
        // 加载资源（同一时间只有一个协程能加载同一资源）
        var resource = await ResourcesManager.Instance.LoadAsync(path);
        return resource;
    }
    finally
    {
        // 释放锁
        lockObj.Dispose();
    }
}
```

### 示例 2: UI 操作锁

```csharp
public async ETTask OpenWindows()
{
    // 确保 UI 操作顺序执行
    CoroutineLock lock1 = await CoroutineLockManager.Instance.WaitAsync(
        CoroutineLockType.UIManager, 
        "Open_UIMain"
    );
    
    try
    {
        UIManager.Instance.OpenWindow("UIMain");
    }
    finally
    {
        lock1.Dispose();
    }
    
    // 打开另一个窗口
    CoroutineLock lock2 = await CoroutineLockManager.Instance.WaitAsync(
        CoroutineLockType.UIManager, 
        "Open_UIAuction"
    );
    
    try
    {
        UIManager.Instance.OpenWindow("UIAuction");
    }
    finally
    {
        lock2.Dispose();
    }
}
```

### 示例 3: 消息框锁

```csharp
public async ETTask ShowMessages()
{
    // 确保消息框逐个显示
    CoroutineLock lock = await CoroutineLockManager.Instance.WaitAsync(
        CoroutineLockType.UIMsgBox, 
        "MsgBox"
    );
    
    try
    {
        await UIManager.Instance.ShowMsgBox("消息 1");
        await UIManager.Instance.ShowMsgBox("消息 2");
        await UIManager.Instance.ShowMsgBox("消息 3");
    }
    finally
    {
        lock.Dispose();
    }
}
```

---

## 锁类型对比

| 类型 | 值 | 用途 | Key 示例 |
|------|-----|------|---------|
| `None` | 0 | 无锁 | - |
| `Resources` | 1 | 资源加载 | "Prefabs/Player" |
| `UIManager` | 2 | UI 管理 | "OpenWindow" |
| `UIMsgBox` | 3 | 消息框 | "MsgBox" |
| `EnableObjView` | 4 | 对象视图 | "PlayerView" |
| `PathQuery` | 5 | 路径查询 | "Map01" |

---

## 新增锁类型

如需新增锁类型，在 `Max` 之前添加：

```csharp
public class CoroutineLockType
{
    public const int None = 0;
    public const int Resources = 1;
    // ... 现有类型
    
    // 新增类型
    public const int Audio = 6;      // 音频播放锁
    public const int Network = 7;    // 网络请求锁
    
    public const int Max = 100;  // 保持最大
}
```

---

## 相关文档

- [CoroutineLockManager.cs.md](./CoroutineLockManager.cs.md) - 协程锁管理器
- [CoroutineLock.cs.md](./CoroutineLock.cs.md) - 协程锁

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
