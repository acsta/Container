# GameObjectPoolManager.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | GameObjectPoolManager.cs |
| **路径** | Assets/Scripts/Code/Module/Resource/GameObjectPoolManager.cs |
| **所属模块** | 框架层 → Code/Module/Resource |
| **文件职责** | GameObject 对象池管理，减少实例化/销毁开销 |

---

## 类/结构体说明

### GameObjectPoolManager

| 属性 | 说明 |
|------|------|
| **职责** | 管理 GameObject 对象池，支持预加载、回收、清理 |
| **泛型参数** | 无 |
| **继承关系** | 无继承 |
| **实现的接口** | 无 |

**设计模式**: 对象池模式 + 单例模式

```csharp
// 单例实现
public static GameObjectPoolManager GetInstance() { }

// 无需注册，直接使用
var pool = GameObjectPoolManager.GetInstance();
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `poolDict` | `Dictionary<string, Queue<GameObject>>` | `private` | 对象池字典 {prefabPath: [pool]} |
| `persistentPaths` | `HashSet<string>` | `private` | 持久化预制体路径（不清理） |
| `preloadTasks` | `Dictionary<string, ETTask>` | `private` | 预加载任务缓存 |

---

## 方法说明（按重要程度排序）

### GetInstance()

**签名**:
```csharp
public static GameObjectPoolManager GetInstance()
```

**职责**: 获取单例实例

**核心逻辑**:
```
1. 检查实例是否存在
2. 如果不存在，创建新实例
3. 返回实例
```

---

### GetGameObjectAsync(string path)

**签名**:
```csharp
public async ETTask<GameObject> GetGameObjectAsync(string path)
```

**职责**: 从对象池获取 GameObject

**参数**:
- `path`: 预制体路径

**返回**: GameObject 实例

**核心逻辑**:
```
1. 检查对象池是否有缓存
2. 如果有：出队并返回
3. 如果没有：
   - 从 YooAsset 加载预制体
   - 实例化
   - 返回
```

**调用者**: 需要 GameObject 的代码

**使用示例**:
```csharp
// 获取 UI 预制体
var go = await GameObjectPoolManager.GetInstance().GetGameObjectAsync("UI/UILoadingView");

// 使用
go.transform.SetParent(parent, false);
```

---

### RecycleGameObject(GameObject obj, int clear = -1)

**签名**:
```csharp
public void RecycleGameObject(GameObject obj, int clear = -1)
```

**职责**: 回收 GameObject 到对象池

**参数**:
- `obj`: 要回收的 GameObject
- `clear`: 缓存阈值（-1 = 无限缓存）

**核心逻辑**:
```
1. 获取预制体路径
2. 加入对象池队列
3. 如果队列长度 > clear：
   - 销毁多余的 GameObject
```

**调用者**: 不再需要 GameObject 的代码

**使用示例**:
```csharp
// 回收到对象池（无限缓存）
GameObjectPoolManager.GetInstance().RecycleGameObject(uiView.gameObject);

// 回收到对象池（最多缓存 5 个）
GameObjectPoolManager.GetInstance().RecycleGameObject(uiView.gameObject, clear: 5);
```

---

### PreLoadGameObjectAsync(string path, int count)

**签名**:
```csharp
public async ETTask PreLoadGameObjectAsync(string path, int count)
```

**职责**: 预加载 GameObject 到对象池

**参数**:
- `path`: 预制体路径
- `count`: 预加载数量

**核心逻辑**:
```
1. 检查是否已预加载
2. 循环 count 次：
   - 加载预制体
   - 实例化
   - 回收到对象池
3. 缓存预加载任务
```

**调用者**: 启动时预加载资源

**使用示例**:
```csharp
// 预加载 10 个 UI 窗口
await GameObjectPoolManager.GetInstance().PreLoadGameObjectAsync(
    "UI/UILoadingView",
    10
);
```

---

### AddPersistentPrefabPath(string path)

**签名**:
```csharp
public void AddPersistentPrefabPath(string path)
```

**职责**: 添加持久化预制体路径（清理时不销毁）

**参数**:
- `path`: 预制体路径

**调用者**: 需要持久缓存的 UI

**使用示例**:
```csharp
// 设置常用 UI 为持久化
GameObjectPoolManager.GetInstance().AddPersistentPrefabPath("UI/UILoadingView");
GameObjectPoolManager.GetInstance().AddPersistentPrefabPath("UI/UICommonDialog");
```

---

### Cleanup(bool force, List<string> ignorePaths)

**签名**:
```csharp
public void Cleanup(bool force, List<string> ignorePaths)
```

**职责**: 清理对象池（释放内存）

**参数**:
- `force`: 是否强制清理（包括持久化路径）
- `ignorePaths`: 忽略的路径列表

**核心逻辑**:
```
1. 遍历所有对象池
2. 如果是持久化路径且 !force：跳过
3. 如果在 ignorePaths 中：跳过
4. 否则：销毁所有缓存的 GameObject
5. 清空对象池
```

**调用者**: 场景切换、内存紧张时

**使用示例**:
```csharp
// 清理所有非持久化对象池
GameObjectPoolManager.GetInstance().Cleanup(false, null);

// 强制清理所有（包括持久化）
GameObjectPoolManager.GetInstance().Cleanup(true, null);

// 清理时保留指定路径
var ignore = new List<string> { "UI/UILoadingView" };
GameObjectPoolManager.GetInstance().Cleanup(false, ignore);
```

---

## 阅读指引

### 建议的阅读顺序

1. **理解对象池作用** - 为什么需要 GameObjectPoolManager
2. **看 GetGameObjectAsync** - 理解获取逻辑
3. **看 RecycleGameObject** - 理解回收逻辑
4. **了解 Cleanup** - 理解内存管理

### 最值得学习的技术点

1. **对象池模式**: 减少 Instantiate/Destroy 开销
2. **异步加载**: ETTask 实现异步获取
3. **缓存阈值**: clear 参数控制缓存数量
4. **持久化路径**: 常用资源不清理
5. **预加载**: 提前加载减少卡顿

---

## 使用示例

### 示例 1: 基础用法

```csharp
// 获取 GameObject
var go = await GameObjectPoolManager.GetInstance().GetGameObjectAsync("UI/UIButton");

// 使用
go.transform.SetParent(parent, false);
go.SetActive(true);

// 回收
GameObjectPoolManager.GetInstance().RecycleGameObject(go);
```

### 示例 2: 预加载

```csharp
// 启动时预加载常用 UI
async ETTask PreloadUI()
{
    var pool = GameObjectPoolManager.GetInstance();
    
    await pool.PreLoadGameObjectAsync("UI/UILoadingView", 5);
    await pool.PreLoadGameObjectAsync("UI/UICommonDialog", 10);
    await pool.PreLoadGameObjectAsync("UI/UIToast", 20);
    
    // 设置为持久化
    pool.AddPersistentPrefabPath("UI/UILoadingView");
    pool.AddPersistentPrefabPath("UI/UICommonDialog");
}
```

### 示例 3: 场景切换清理

```csharp
// 场景切换时清理资源
async ETTask OnSceneChange()
{
    var pool = GameObjectPoolManager.GetInstance();
    
    // 保留常用 UI
    var ignore = new List<string>
    {
        "UI/UILoadingView",
        "UI/UIMsgBox"
    };
    
    // 清理其他资源
    pool.Cleanup(false, ignore);
    
    // 卸载未使用资源
    await PackageManager.Instance.UnloadUnusedAssets(Define.DefaultName);
}
```

---

## 性能优化

### 对象池优势

| 操作 | 直接 Instantiate/Destroy | 对象池 |
|------|------------------------|--------|
| 首次创建 | 慢 | 慢 |
| 重复使用 | 慢（每次都创建） | 快（复用） |
| GC 压力 | 高 | 低 |
| 内存占用 | 低 | 高（缓存） |

### 最佳实践

```csharp
// ✅ 推荐：频繁创建销毁的对象使用对象池
// UI 窗口、子弹、特效等

// ❌ 不推荐：只创建一次的对象使用对象池
// 场景根节点、单例 Manager 等
```

---

## 相关文档

- [ResourcesManager.cs.md](./ResourcesManager.cs.md) - 资源管理器
- [PackageManager.cs.md](../../../ThirdParty/YooAssets/PackageManager.cs.md) - YooAsset 包管理
- [UIManager.cs.md](../UI/UIManager.cs.md) - UI 管理器（主要使用者）

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
