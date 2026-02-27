# MaterialManager.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | MaterialManager.cs |
| **路径** | Assets/Scripts/Code/Module/Resource/MaterialManager.cs |
| **所属模块** | 框架层 → Code/Module/Resource |
| **文件职责** | 材质管理器，管理材质的加载和缓存 |

---

## 类/结构体说明

### MaterialManager

| 属性 | 说明 |
|------|------|
| **职责** | 管理材质的异步加载、缓存、预加载 |
| **泛型参数** | 无 |
| **继承关系** | 无 |
| **实现的接口** | `IManager` |

```csharp
public class MaterialManager : IManager
{
    // 材质管理器
}
```

---

## 字段与属性

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `Instance` | `MaterialManager` | `public static` | 单例实例 |
| `cacheMaterial` | `Dictionary<string, Material>` | `private` | 材质缓存字典 |

---

## 方法说明（按重要程度排序）

### Init() / Destroy()

**签名**:
```csharp
public void Init()
public void Destroy()
```

**职责**: 初始化和销毁

**核心逻辑**:
```
// Init:
1. 设置 Instance = this
2. 创建 cacheMaterial 字典

// Destroy:
1. 遍历缓存，释放所有材质
2. 清空字典
3. 清空 Instance
```

**调用者**: `ManagerProvider.Init()`

---

### GetFromCache(string address)

**签名**:
```csharp
public Material GetFromCache(string address)
```

**职责**: 从缓存获取材质

**参数**:
- `address`: 材质地址

**返回**: 材质实例（不存在返回 null）

**核心逻辑**:
```
1. 从 cacheMaterial 字典查找
2. 返回结果
```

**调用者**: 需要快速检查缓存的代码

---

### PreLoadMaterial(string address)

**签名**:
```csharp
public async ETTask PreLoadMaterial(string address)
```

**职责**: 预加载材质到缓存

**参数**:
- `address`: 材质地址

**核心逻辑**:
```
1. 等待获取资源锁（CoroutineLock）
2. 检查缓存：
   - 如果存在，直接返回
3. 如果不存在：
   - 调用 ResourcesManager.LoadAsync 加载
   - 保存到 cacheMaterial
4. 释放锁
```

**协程锁**:
```csharp
CoroutineLock coroutineLock = null;
try
{
    coroutineLock = await CoroutineLockManager.Instance.Wait(
        CoroutineLockType.Resources, 
        address.GetHashCode()
    );
    
    if (!cacheMaterial.TryGetValue(address, out res))
    {
        res = await ResourcesManager.Instance.LoadAsync<Material>(address);
        if (res != null)
            cacheMaterial[address] = res;
    }
}
finally
{
    coroutineLock?.Dispose();
}
```

**调用者**: 预加载流程

---

### LoadMaterialAsync(string address, callback)

**签名**:
```csharp
public async ETTask<Material> LoadMaterialAsync(string address, Action<Material> callback = null)
```

**职责**: 异步加载材质

**参数**:
- `address`: 材质地址
- `callback`: 加载完成回调

**返回**: 材质实例

**核心逻辑**:
```
1. 等待获取资源锁
2. 检查缓存：
   - 如果存在，直接返回
3. 如果不存在：
   - 调用 ResourcesManager.LoadAsync 加载
   - 保存到 cacheMaterial
4. 调用 callback
5. 释放锁
6. 返回材质
```

**调用者**: 需要加载材质的代码

**使用示例**:
```csharp
Material mat = await MaterialManager.Instance.LoadMaterialAsync("UI/UICommon/Materials/uigray");
```

---

### LoadMaterialTask(string address, callback)

**签名**:
```csharp
public ETTask LoadMaterialTask(string address, Action<Material> callback = null)
```

**职责**: 加载材质（返回无结果 Task）

**参数**:
- `address`: 材质地址
- `callback`: 加载完成回调

**返回**: ETTask（无结果）

**核心逻辑**:
```
1. 创建 ETTask
2. 调用 LoadMaterialAsync
3. 在 callback 中设置 task 结果
4. 返回 task
```

**与 LoadMaterialAsync 的区别**: 返回无结果的 Task，适合不需要材质实例的场景

---

## 阅读指引

### 建议的阅读顺序

1. **理解管理器作用** - MaterialManager 管理什么
2. **看 LoadMaterialAsync** - 理解异步加载
3. **看 PreLoadMaterial** - 理解预加载
4. **了解缓存机制** - 理解 cacheMaterial

### 最值得学习的技术点

1. **材质缓存**: Dictionary 缓存已加载材质
2. **协程锁**: 防止重复加载同一材质
3. **异步加载**: ETTask 异步加载
4. **回调支持**: Action<Material> 回调
5. **资源释放**: Destroy 时释放所有材质

---

## 使用示例

### 示例 1: 加载材质

```csharp
// 异步加载材质
Material grayMat = await MaterialManager.Instance.LoadMaterialAsync(
    "UI/UICommon/Materials/uigray"
);

// 应用到 UI
image.material = grayMat;
```

### 示例 2: 预加载材质

```csharp
// 预加载灰化材质（游戏启动时）
await MaterialManager.Instance.PreLoadMaterial("UI/UICommon/Materials/uigray");

// 后续使用直接从缓存获取
Material mat = MaterialManager.Instance.GetFromCache("UI/UICommon/Materials/uigray");
```

### 示例 3: 带回调加载

```csharp
// 带回调加载
Material mat = await MaterialManager.Instance.LoadMaterialAsync(
    "UI/UICommon/Materials/uigray",
    (material) => {
        Log.Info($"材质加载完成：{material.name}");
    }
);
```

### 示例 4: 使用 Task

```csharp
// 不需要材质实例，只需等待加载完成
await MaterialManager.Instance.LoadMaterialTask(
    "UI/UICommon/Materials/uigray",
    (material) => {
        // 在回调中使用材质
        image.material = material;
    }
);
```

---

## 缓存机制

### 缓存流程

```
加载请求
    ↓
检查缓存？
├─ 是 → 返回缓存材质
└─ 否 → 加载材质
         ↓
       保存到缓存
         ↓
       返回材质
```

### 协程锁

```
同一材质的多个加载请求
    ↓
CoroutineLock 排队
    ↓
第一个请求加载
    ↓
后续请求从缓存获取
```

---

## 相关文档

- [ResourcesManager.cs.md](./ResourcesManager.cs.md) - 资源管理器
- [CoroutineLockManager.cs.md](../CoroutineLock/CoroutineLockManager.cs.md) - 协程锁管理器
- [ImageLoaderManager.cs.md](./ImageLoaderManager.cs.md) - 图片加载器

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
