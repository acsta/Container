# ImageLoaderManager.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | ImageLoaderManager.cs |
| **路径** | Assets/Scripts/Code/Module/Resource/ImageLoaderManager.cs |
| **所属模块** | 框架层 → Code/Module/Resource |
| **文件职责** | 图片加载系统，自动识别图集，支持 Sprite/Texture 加载 |

---

## 类/结构体说明

### ImageLoaderManager

| 属性 | 说明 |
|------|------|
| **职责** | 管理图片加载，自动识别图集，缓存管理 |
| **泛型参数** | 无 |
| **继承关系** | 无继承 |
| **实现的接口** | `IManager` |

**设计模式**: 单例模式 + LRU 缓存 + 对象池

```csharp
// 单例实现
public static ImageLoaderManager Instance { get; private set; }

// 通过 ManagerProvider 注册
ManagerProvider.RegisterManager<ImageLoaderManager>();
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `Instance` | `ImageLoaderManager` | `public static` | 单例实例 |
| `cacheSingleSprite` | `LruCache<string, SpriteValue>` | `private` | 单图缓存（LRU） |
| `cacheSpriteAtlas` | `LruCache<string, SpriteAtlasValue>` | `private` | 图集缓存（LRU） |
| `cacheOnlineImage` | `Dictionary<string, SpriteValue>` | `private` | 在线图片缓存 |

---

## 方法说明（按重要程度排序）

### LoadSpriteAsync(string imagePath, Action<Sprite> callback)

**签名**:
```csharp
public async ETTask<Sprite> LoadSpriteAsync(string imagePath, Action<Sprite> callback = null)
```

**职责**: 异步加载 Sprite（自动识别图集）

**参数**:
- `imagePath`: 图片路径
- `callback`: 加载完成回调

**返回**: `ETTask<Sprite>` - Sprite 对象

**核心逻辑**:
```
1. 获取协程锁（防止重复加载）
2. 解析路径，判断是单图还是图集
3. 如果是单图：
   - 调用 LoadSingleImageAsyncInternal()
4. 如果是图集：
   - 调用 LoadSpriteImageAsyncInternal()
5. 释放协程锁
6. 返回 Sprite
```

**调用者**: UI 图片加载

**使用示例**:
```csharp
// 加载图片
Sprite sprite = await ImageLoaderManager.Instance.LoadSpriteAsync("UI/Images/Icon");
imageComponent.sprite = sprite;

// 带回调
ImageLoaderManager.Instance.LoadSpriteAsync("UI/Images/Icon", (sprite) =>
{
    imageComponent.sprite = sprite;
});
```

---

### LoadTextureAsync(string imagePath, Action<Texture> callback)

**签名**:
```csharp
public async ETTask<Texture> LoadTextureAsync(string imagePath, Action<Texture> callback = null)
```

**职责**: 异步加载 Texture（从 Sprite 或图集生成）

**参数**:
- `imagePath`: 图片路径
- `callback`: 加载完成回调

**返回**: `ETTask<Texture>` - Texture 对象

**核心逻辑**:
```
1. 获取协程锁
2. 解析路径，判断是单图还是图集
3. 如果是单图：
   - 加载 Sprite，返回 sprite.texture
4. 如果是图集：
   - 加载 Sprite
   - 如果 sprite.texture 不可读，直接返回
   - 否则，创建新 Texture2D，复制像素
5. 释放协程锁
6. 返回 Texture
```

**调用者**: 需要 Texture 的场景（如 RawImage）

**使用示例**:
```csharp
// 加载 Texture
Texture tex = await ImageLoaderManager.Instance.LoadTextureAsync("UI/Images/Icon");
rawImage.texture = tex;
```

---

### ReleaseImage(string imagePath)

**签名**:
```csharp
public void ReleaseImage(string imagePath)
```

**职责**: 释放图片引用

**参数**:
- `imagePath`: 图片路径

**核心逻辑**:
```
1. 解析路径，判断是单图还是图集
2. 如果是单图：
   - 从 cacheSingleSprite 获取
   - RefCount--
3. 如果是图集：
   - 从 cacheSpriteAtlas 获取
   - 找到对应子 Sprite
   - RefCount--
   - 如果 RefCount <= 0，销毁 Sprite
```

**调用者**: 不再需要图片时

**使用示例**:
```csharp
// 使用完毕，释放引用
ImageLoaderManager.Instance.ReleaseImage("UI/Images/Icon");
```

---

### Cleanup()

**签名**:
```csharp
public void Cleanup()
```

**职责**: 清理 LRU 缓存（移除 RefCount=0 的资源）

**调用者**: 场景切换、内存清理时

---

### Clear()

**签名**:
```csharp
public void Clear()
```

**职责**: 清空所有缓存

**核心逻辑**:
```
1. 清理 cacheSingleSprite
2. 清理 cacheSpriteAtlas
3. 清理 cacheOnlineImage
```

**调用者**: 彻底清理时

---

## 阅读指引

### 建议的阅读顺序

1. **理解图片加载系统作用** - 为什么需要 ImageLoaderManager
2. **看 LoadSpriteAsync** - 理解 Sprite 加载
3. **看 LoadTextureAsync** - 理解 Texture 加载
4. **看 ReleaseImage** - 理解引用计数

### 最值得学习的技术点

1. **自动识别图集**: 通过路径判断是单图还是图集
2. **LRU 缓存**: LruCache 自动管理内存
3. **引用计数**: RefCount 控制资源释放
4. **协程锁**: 防止重复加载同一资源
5. **Texture 生成**: 从图集 Sprite 生成独立 Texture

---

## 使用示例

### 示例 1: 加载 Sprite

```csharp
// 简单加载
Sprite sprite = await ImageLoaderManager.Instance.LoadSpriteAsync("UI/Images/Icon");
image.sprite = sprite;

// 带回调
ImageLoaderManager.Instance.LoadSpriteAsync("UI/Images/Icon", (sprite) =>
{
    image.sprite = sprite;
    // 使用完毕后释放
    ImageLoaderManager.Instance.ReleaseImage("UI/Images/Icon");
});
```

### 示例 2: 加载 Texture

```csharp
// 从 Sprite 生成 Texture
Texture tex = await ImageLoaderManager.Instance.LoadTextureAsync("UI/Images/Icon");
rawImage.texture = tex;

// 注意：Texture 不需要手动释放，由缓存管理
```

### 示例 3: 预加载

```csharp
// 预加载图片（不持有引用）
await ImageLoaderManager.Instance.PreLoadSpriteTask("UI/Images/Icon");

// 后续使用时会直接从缓存获取
Sprite sprite = await ImageLoaderManager.Instance.LoadSpriteAsync("UI/Images/Icon");
```

---

## 相关文档

- [ResourcesManager.cs.md](./ResourcesManager.cs.md) - 资源管理器
- [GameObjectPoolManager.cs.md](./GameObjectPoolManager.cs.md) - GameObject 对象池
- [UIManager.cs.md](../UI/UIManager.cs.md) - UI 管理器

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
