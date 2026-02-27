# ResourcesManager.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | ResourcesManager.cs |
| **路径** | Assets/Scripts/Code/Module/Resource/ResourcesManager.cs |
| **所属模块** | 框架层 → Code/Module/Resource |
| **文件职责** | 资源管理系统，提供异步资源加载、场景加载、缓存管理 |

---

## 类/结构体说明

### ResourcesManager

| 属性 | 说明 |
|------|------|
| **职责** | 管理所有资源加载，提供统一的异步加载接口 |
| **泛型参数** | 无 |
| **继承关系** | 无继承 |
| **实现的接口** | `IManager`, `IManager<IPackageFinder>` |

**设计模式**: 单例模式 + 缓存模式

```csharp
// 单例实现
public static ResourcesManager Instance { get; private set; }

// 通过 ManagerProvider 注册
ManagerProvider.RegisterManager<ResourcesManager>();
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `Instance` | `ResourcesManager` | `public static` | 单例实例 |
| `packageFinder` | `IPackageFinder` | `public` | 包查找器接口 |
| `temp` | `Dictionary<object, AssetHandle>` | `private` | 资源句柄缓存 {资源对象：句柄} |
| `cachedAssetOperationHandles` | `List<AssetHandle>` | `private` | 缓存的资源句柄列表（可清理） |
| `persistentAssetOperationHandles` | `List<AssetHandle>` | `private` | 持久化资源句柄列表（不清理） |
| `loadingOp` | `HashSet<AssetHandle>` | `private` | 正在加载的操作集合 |
| `preloadScene` | `SceneHandle` | `private` | 预加载的场景句柄 |

---

## 方法说明（按重要程度排序）

### LoadAsync<T>(string path, Action<T> callback, string package, bool isPersistent)

**签名**:
```csharp
public ETTask<T> LoadAsync<T>(string path, Action<T> callback = null, string package = null, bool isPersistent = false)
    where T : UnityEngine.Object
```

**职责**: 异步加载资源

**参数**:
- `path`: 资源路径
- `callback`: 加载完成回调
- `package`: 资源包名称（可选，自动查找）
- `isPersistent`: 是否持久化（不清理）

**返回**: `ETTask<T>` - 异步任务

**核心逻辑**:
```
1. 检查路径是否为空
2. 如果 package 为空，通过 packageFinder 获取
3. 调用 PackageManager.LoadAssetAsync()
4. 添加加载完成回调：
   - 从 loadingOp 移除
   - 如果资源有效，加入 temp 缓存
   - 根据 isPersistent 加入不同列表
   - 调用用户 callback
5. 返回 ETTask
```

**调用者**: 需要加载资源的代码

**使用示例**:
```csharp
// 加载预制体
var prefab = await ResourcesManager.Instance.LoadAsync<GameObject>("UI/UIButton");

// 加载纹理
var texture = await ResourcesManager.Instance.LoadAsync<Texture2D>("Images/Icon");

// 带回调
ResourcesManager.Instance.LoadAsync<GameObject>("UI/UIWindow", (go) =>
{
    // 使用加载的 GameObject
});
```

---

### LoadSceneAsync(string path, bool isAdditive, string package)

**签名**:
```csharp
public ETTask<SceneHandle> LoadSceneAsync(string path, bool isAdditive, string package = null)
```

**职责**: 异步加载场景

**参数**:
- `path`: 场景路径
- `isAdditive`: 是否 additive 模式（多场景）
- `package`: 资源包名称

**返回**: `ETTask<SceneHandle>` - 场景句柄

**使用示例**:
```csharp
// 加载单场景（替换当前场景）
var sceneHandle = await ResourcesManager.Instance.LoadSceneAsync("Scenes/HomeScene", false);

// 加载多场景（additive）
var sceneHandle = await ResourcesManager.Instance.LoadSceneAsync("Scenes/EffectScene", true);
```

---

### PreLoadScene(string path, bool isAdditive, string package)

**签名**:
```csharp
public SceneHandle PreLoadScene(string path, bool isAdditive, string package = null)
```

**职责**: 预加载场景（后台加载，不激活）

**参数**:
- `path`: 场景路径
- `isAdditive`: 是否 additive 模式
- `package`: 资源包名称

**返回**: `SceneHandle` - 预加载句柄

**核心逻辑**:
```
1. 检查是否已有预加载
2. 调用 PackageManager.LoadSceneAsync(..., preload: true)
3. 保存 preloadScene 句柄
4. 添加完成回调（自动卸载）
5. 返回句柄
```

**使用示例**:
```csharp
// 预加载下一个场景
ResourcesManager.Instance.PreLoadScene("Scenes/NextScene", false);

// 等待预加载完成
await TimerManager.Instance.WaitAsync(2000);  // 或其他方式检查

// 正式加载（会直接使用预加载的资源）
await ResourcesManager.Instance.LoadSceneAsync("Scenes/NextScene", false);
```

---

### CleanUp(List<UnityEngine.Object> ignoreClearAssets)

**签名**:
```csharp
public void CleanUp(List<UnityEngine.Object> ignoreClearAssets = null)
```

**职责**: 清理所有非持久化资源

**参数**:
- `ignoreClearAssets`: 不需要清理的资源列表

**核心逻辑**:
```
1. 创建 ignore 集合（从 ignoreClearAssets）
2. 遍历 cachedAssetOperationHandles：
   - 如果不在 ignore 中，释放资源
   - 从 temp 移除
   - 从列表移除
3. 持久化资源不受影响
```

**调用者**: 场景切换、内存清理时

**使用示例**:
```csharp
// 清理所有非持久化资源
ResourcesManager.Instance.CleanUp();

// 清理时保留指定资源
var ignore = new List<UnityEngine.Object> { persistentUI, persistentEffect };
ResourcesManager.Instance.CleanUp(ignore);
```

---

### ClearAssetsCache()

**签名**:
```csharp
public void ClearAssetsCache()
```

**职责**: 清理所有资源（包括持久化）

**核心逻辑**:
```
1. 清理 cachedAssetOperationHandles
2. 清理 persistentAssetOperationHandles
3. 清空 temp 字典
```

**调用者**: 游戏重启、彻底清理时

---

### IsProcessRunning()

**签名**:
```csharp
public bool IsProcessRunning()
```

**职责**: 检查是否有加载任务正在进行

**返回**: `true` = 有加载任务，`false` = 空闲

**调用者**: SceneManager（检查是否可以切换场景）

---

### IsPreloadScene()

**签名**:
```csharp
public bool IsPreloadScene()
```

**职责**: 检查是否有预加载场景正在进行

**返回**: `true` = 有预加载，`false` = 无

**调用者**: 场景切换逻辑

---

## 阅读指引

### 建议的阅读顺序

1. **理解资源管理器作用** - 为什么需要 ResourcesManager
2. **看 LoadAsync** - 理解资源加载
3. **看 LoadSceneAsync** - 理解场景加载
4. **看 CleanUp** - 理解资源清理

### 最值得学习的技术点

1. **异步加载**: ETTask 统一异步接口
2. **缓存管理**: temp 字典 + 两个列表管理资源
3. **持久化标记**: isPersistent 区分可清理/不可清理
4. **预加载机制**: PreLoadScene 提前加载场景
5. **包查找器**: IPackageFinder 抽象资源包定位

---

## 使用示例

### 示例 1: 加载资源

```csharp
// 简单加载
var prefab = await ResourcesManager.Instance.LoadAsync<GameObject>("UI/UIButton");

// 带回调
ResourcesManager.Instance.LoadAsync<Texture2D>(
    "Images/Icon",
    (texture) =>
    {
        // 使用 texture
        rawImage.texture = texture;
    }
);

// 持久化加载（不清理）
var persistentUI = await ResourcesManager.Instance.LoadAsync<GameObject>(
    "UI/UILoadingView",
    isPersistent: true
);
```

### 示例 2: 场景加载

```csharp
// 加载单场景
await ResourcesManager.Instance.LoadSceneAsync("Scenes/HomeScene", false);

// 加载多场景（additive）
await ResourcesManager.Instance.LoadSceneAsync("Scenes/EffectScene", true);

// 预加载场景
ResourcesManager.Instance.PreLoadScene("Scenes/NextScene", false);

// 检查预加载状态
if (ResourcesManager.Instance.IsPreloadScene())
{
    // 正在预加载
}
```

### 示例 3: 资源清理

```csharp
// 场景切换时清理
async ETTask OnSceneChange()
{
    // 清理非持久化资源
    ResourcesManager.Instance.CleanUp();
    
    // 卸载未使用资源
    await PackageManager.Instance.UnloadUnusedAssets(Define.DefaultName);
    
    // 加载新场景
    await ResourcesManager.Instance.LoadSceneAsync("Scenes/NewScene", false);
}

// 彻底清理（包括持久化）
ResourcesManager.Instance.ClearAssetsCache();
```

---

## 相关文档

- [GameObjectPoolManager.cs.md](./GameObjectPoolManager.cs.md) - GameObject 对象池
- [ImageLoaderManager.cs.md](./ImageLoaderManager.cs.md) - 图片加载器
- [SceneManager.cs.md](../Scene/SceneManager.cs.md) - 场景管理
- [PackageManager.cs.md](../../../ThirdParty/YooAssets/PackageManager.cs.md) - YooAsset 包管理

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
