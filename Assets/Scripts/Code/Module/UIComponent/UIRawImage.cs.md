# UIRawImage.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | UIRawImage.cs |
| **路径** | Assets/Scripts/Code/Module/UIComponent/UIRawImage.cs |
| **所属模块** | 框架层 → Code/Module/UIComponent |
| **文件职责** | UI 原始图片组件，封装 RawImage，支持 Texture 加载 |

---

## 类/结构体说明

### UIRawImage

| 属性 | 说明 |
|------|------|
| **职责** | 封装 Unity RawImage，支持本地/网络/ Base64 图片加载 |
| **泛型参数** | 无 |
| **继承关系** | 继承自 `UIBaseContainer` |
| **实现的接口** | `IOnCreate<string>`, `IOnDestroy` |

```csharp
public class UIRawImage : UIBaseContainer, IOnCreate<string>, IOnDestroy
{
    // UI 原始图片组件
}
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `texturePath` | `string` | `private` | 本地 Texture 路径 |
| `image` | `RawImage` | `private` | Unity RawImage 组件 |
| `bgRawAutoFit` | `BgRawAutoFit` | `private` | 背景自动适配组件 |
| `grayState` | `bool` | `private` | 是否灰化状态 |
| `version` | `int` | `private` | 版本号（防止重复加载） |
| `cacheUrl` | `string` | `private` | 缓存的网络图片 URL |
| `isSetTexture` | `bool` | `private` | 是否设置了外部 Texture |
| `base64texture` | `Texture2D` | `private` | Base64 解码的 Texture |

---

## 方法说明（按重要程度排序）

### OnCreate(string path)

**签名**:
```csharp
public void OnCreate(string path)
```

**职责**: 创建时初始化图片路径

**核心逻辑**:
```
1. 调用 SetTexturePath(path)
```

**调用者**: `UIManager.InnerOpenWindowGetGameObject()`

---

### OnDestroy()

**签名**:
```csharp
public void OnDestroy()
```

**职责**: 销毁时清理资源

**核心逻辑**:
```
1. 如果设置了 texturePath，释放本地图片资源
2. 如果设置了 cacheUrl，释放网络图片资源
3. 如果设置了 isSetTexture，清空 texture
4. 清理 Base64 Texture
```

**调用者**: `UIBaseContainer.BeforeOnDestroy()`

---

### SetTexturePath(string texturePath, setNativeSize)

**签名**:
```csharp
public async ETTask SetTexturePath(string texturePath, bool setNativeSize = false)
```

**职责**: 设置本地 Texture 路径

**参数**:
- `texturePath`: Texture 路径
- `setNativeSize`: 是否设置原始大小

**核心逻辑**:
```
1. 增加 version（防止重复加载）
2. 如果路径相同且未设置外部 Texture，直接返回
3. 激活 RawImage 组件
4. 禁用 RawImage（加载完成前）
5. 如果路径为空，清空 texture
6. 否则加载新 texture
7. 设置 image.texture
8. 如果需要，设置原始大小
9. 如果需要，设置背景自动适配
10. 释放旧图片资源
11. 如果 texture 为 null，加载默认图片
```

**版本检查**:
```csharp
version++;
int thisVersion = version;

// 加载完成后检查版本
if (thisVersion != version)
{
    // 已被新请求覆盖，释放资源
    ImageLoaderManager.Instance.ReleaseImage(texturePath);
    return;
}
```

**调用者**: UI 初始化代码

---

### SetOnlineTexturePath(string url, defaultTexturePath)

**签名**:
```csharp
public async ETTask SetOnlineTexturePath(string url, string defaultTexturePath = null)
```

**职责**: 设置网络 Texture 路径

**参数**:
- `url`: 网络图片 URL
- `defaultTexturePath`: 默认 Texture 路径（加载失败时使用）

**核心逻辑**:
```
1. 如果有默认图片，先加载默认图片
2. 增加 version
3. 从网络加载 texture
4. 如果版本未变，设置 texture
5. 释放旧的缓存 URL
6. 保存新的 cacheUrl
7. 如果加载失败，使用默认图片
```

**与 SetTexturePath 的区别**:
- `SetTexturePath`: 加载本地资源
- `SetOnlineTexturePath`: 加载网络资源，有缓存机制

**注意**: 不要混用两个方法

---

### SetTexture(Texture texture)

**签名**:
```csharp
public void SetTexture(Texture texture)
```

**职责**: 直接设置 Texture 对象

**核心逻辑**:
```
1. 激活 RawImage 组件
2. 设置 image.texture = texture
3. 设置 isSetTexture = true
```

**调用者**: 已有 Texture 对象的场景

---

### SetBase64(string data)

**签名**:
```csharp
public void SetBase64(string data)
```

**职责**: 设置 Base64 编码的图片

**参数**:
- `data`: Base64 字符串（可带 data:image/png;base64, 前缀）

**核心逻辑**:
```
1. 移除 Base64 前缀（data:image/png;base64, 等）
2. 将 Base64 字符串转换为字节数组
3. 如果 base64texture 为 null，创建新的 Texture2D
4. 加载图像数据 base64texture.LoadImage(imageBytes)
5. 设置 texture
```

**调用者**: 需要显示 Base64 图片的场景

**使用示例**:
```csharp
// 设置 Base64 图片
rawImage.SetBase64("data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAA...");
```

---

### SetImageColor(Color color) / SetImageAlpha(float a)

**签名**:
```csharp
public void SetImageColor(Color color)
public void SetImageAlpha(float a)
```

**职责**: 设置图片颜色/透明度

**核心逻辑**:
```
1. 激活 RawImage 组件
2. 设置 image.color
```

**调用者**: 需要改变图片颜色/透明度的代码

---

### SetImageGray(bool isGray)

**签名**:
```csharp
public async ETTask SetImageGray(bool isGray)
```

**职责**: 设置灰化效果

**核心逻辑**:
```
1. 如果状态未变化，返回
2. 保存 grayState
3. 加载灰化材质（如果是灰化）
4. 设置 image.material
```

**调用者**: 需要禁用图片但保持可见的场景

---

### SetNativeSize()

**签名**:
```csharp
public void SetNativeSize()
```

**职责**: 设置图片为原始大小

**核心逻辑**:
```
1. 调用 image.SetNativeSize()
```

**调用者**: 需要显示图片原始大小的场景

---

### GetTexture() / GetTexturePath()

**签名**:
```csharp
public Texture GetTexture()
public string GetTexturePath()
```

**职责**: 获取 Texture 对象/路径

**核心逻辑**:
```
1. 激活 RawImage 组件
2. 返回 image.texture / texturePath
```

**调用者**: 需要读取图片信息的代码

---

## 阅读指引

### 建议的阅读顺序

1. **理解组件作用** - 为什么需要 UIRawImage
2. **看 SetTexturePath** - 理解本地图片加载
3. **看 SetOnlineTexturePath** - 理解网络图片加载
4. **看 SetBase64** - 理解 Base64 图片

### 最值得学习的技术点

1. **版本控制**: version 防止重复加载
2. **资源管理**: 自动释放图片资源
3. **网络图片**: 支持 URL 加载和缓存
4. **Base64 支持**: 直接解码 Base64 图片
5. **背景适配**: BgRawAutoFit 自动适配背景

---

## 与 UIImage 的区别

| 特性 | UIImage | UIRawImage |
|------|---------|------------|
| Unity 组件 | Image | RawImage |
| 资源类型 | Sprite | Texture |
| 性能 | 较高（Sprite 批处理） | 较低（RawImage 独立绘制） |
| 适用场景 | UI 图标、按钮 | 视频帧、动态 Texture |

---

## 使用示例

### 示例 1: 本地图片

```csharp
public class UIAvatarView : UIBaseView, IOnCreate
{
    private UIRawImage avatar;
    
    public void OnCreate()
    {
        avatar = AddComponent<UIRawImage>("Avatar");
    }
    
    public void SetAvatar(string texturePath)
    {
        avatar.SetTexturePath(texturePath);
    }
}
```

### 示例 2: 网络图片

```csharp
public class UIPlayerCardView : UIBaseView, IOnCreate
{
    private UIRawImage avatar;
    
    public void OnCreate()
    {
        avatar = AddComponent<UIRawImage>("Avatar");
    }
    
    public void SetPlayer(string avatarUrl)
    {
        avatar.SetOnlineTexturePath(
            avatarUrl,
            defaultTexturePath: "UI/Textures/DefaultAvatar"
        );
    }
}
```

### 示例 3: Base64 图片

```csharp
// 从服务器获取 Base64 图片
string base64Data = await GetAvatarBase64();
rawImage.SetBase64(base64Data);
```

### 示例 4: 灰化效果

```csharp
// 灰化（禁用状态）
await avatar.SetImageGray(isGray: true);

// 恢复
await avatar.SetImageGray(isGray: false);
```

---

## 相关文档

- [UIBaseContainer.cs.md](./UIBaseContainer.cs.md) - UI 容器基类
- [UIImage.cs.md](./UIImage.cs.md) - UI 图片组件（Sprite）
- [ImageLoaderManager.cs.md](../Resource/ImageLoaderManager.cs.md) - 图片加载器

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
