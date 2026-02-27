# UIImage.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | UIImage.cs |
| **路径** | Assets/Scripts/Code/Module/UIComponent/UIImage.cs |
| **所属模块** | 框架层 → Code/Module/UIComponent |
| **文件职责** | UI 图片组件，封装 Image，支持本地/网络图片加载 |

---

## 类/结构体说明

### UIImage

| 属性 | 说明 |
|------|------|
| **职责** | 封装 Unity Image，支持本地/网络图片加载、自动适配、灰化等 |
| **泛型参数** | 无 |
| **继承关系** | 继承自 `UIBaseContainer` |
| **实现的接口** | `IOnCreate<string>`, `IOnDestroy` |

```csharp
public class UIImage : UIBaseContainer, IOnCreate<string>, IOnDestroy
{
    // UI 图片组件
}
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `spritePath` | `string` | `private` | 本地图片路径 |
| `image` | `Image` | `private` | Unity Image 组件 |
| `bgAutoFit` | `BgAutoFit` | `private` | 背景自动适配组件 |
| `grayState` | `bool` | `private` | 是否灰化状态 |
| `isSetSprite` | `bool` | `private` | 是否设置了外部 Sprite |
| `version` | `int` | `private` | 版本号（防止重复加载） |
| `cacheUrl` | `string` | `private` | 缓存的网络图片 URL |

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
1. 调用 SetSpritePath(path)
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
1. 如果设置了 spritePath，释放本地图片资源
2. 如果设置了 isSetSprite，清空 sprite
3. 如果设置了 cacheUrl，释放网络图片资源
```

**调用者**: `UIBaseContainer.BeforeOnDestroy()`

---

### SetSpritePath(string spritePath, setNativeSize, callback)

**签名**:
```csharp
public async ETTask SetSpritePath(string spritePath, bool setNativeSize = false, Action callback = null)
```

**职责**: 设置本地图片路径

**参数**:
- `spritePath`: 图片路径
- `setNativeSize`: 是否设置原始大小
- `callback`: 加载完成回调

**核心逻辑**:
```
1. 增加 version（防止重复加载）
2. 如果路径相同且未设置外部 Sprite，直接返回
3. 激活 Image 组件
4. 禁用 Image（加载完成前）
5. 如果路径为空，清空 sprite
6. 否则加载新 sprite
7. 设置 image.sprite
8. 如果需要，设置原始大小
9. 如果需要，设置背景自动适配
10. 释放旧图片资源
11. 调用 callback
12. 如果 sprite 为 null，加载默认图片
```

**版本检查**:
```csharp
version++;
int thisVersion = version;

// 加载完成后检查版本
if (thisVersion != version)
{
    // 已被新请求覆盖，释放资源
    ImageLoaderManager.Instance.ReleaseImage(spritePath);
    return;
}
```

**调用者**: UI 初始化代码

**使用示例**:
```csharp
// 获取图片组件
var icon = view.AddComponent<UIImage>("Icon");

// 设置图片路径
await icon.SetSpritePath("UI/Icons/Item001");

// 带回调
await icon.SetSpritePath("UI/Icons/Item001", callback: () =>
{
    Log.Info("图片加载完成");
});
```

---

### SetOnlineSpritePath(string url, setNativeSize, defaultSpritePath)

**签名**:
```csharp
public async ETTask SetOnlineSpritePath(string url, bool setNativeSize = false, string defaultSpritePath = null)
```

**职责**: 设置网络图片路径

**参数**:
- `url`: 网络图片 URL
- `setNativeSize`: 是否设置原始大小
- `defaultSpritePath`: 默认图片路径（加载失败时使用）

**核心逻辑**:
```
1. 如果有默认图片，先加载默认图片
2. 增加 version
3. 从网络加载 sprite
4. 如果版本未变，设置 sprite
5. 释放旧的缓存 URL
6. 保存新的 cacheUrl
7. 如果加载失败，使用默认图片
```

**与 SetSpritePath 的区别**:
- `SetSpritePath`: 加载本地资源
- `SetOnlineSpritePath`: 加载网络资源，有缓存机制

**注意**: 不要混用两个方法

**调用者**: 需要显示网络图片的场景（如玩家头像）

**使用示例**:
```csharp
// 加载玩家头像
var avatar = view.AddComponent<UIImage>("Avatar");
await avatar.SetOnlineSpritePath(
    "https://example.com/avatar/123.png",
    defaultSpritePath: "UI/Icons/DefaultAvatar"
);
```

---

### SetSprite(Sprite sprite)

**签名**:
```csharp
public void SetSprite(Sprite sprite)
```

**职责**: 直接设置 Sprite 对象

**参数**:
- `sprite`: Sprite 对象

**核心逻辑**:
```
1. 激活 Image 组件
2. 设置 image.sprite = sprite
3. 设置 isSetSprite = true
4. 如果需要，设置背景自动适配
```

**调用者**: 已有 Sprite 对象的场景

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

### SetColor(string colorStr) / SetColor(Color color)

**签名**:
```csharp
public void SetColor(string colorStr)
public void SetColor(Color color)
```

**职责**: 设置图片颜色（ tint ）

**参数**:
- `colorStr`: 颜色字符串（如 "#FF0000" 或 "FF0000"）
- `color`: Color 对象

**核心逻辑**:
```
1. 激活 Image 组件
2. 设置 image.color = color
```

**调用者**: 需要改变图片颜色的场景

**使用示例**:
```csharp
// 设置红色
icon.SetColor("#FF0000");

// 设置半透明
icon.SetColor(new Color(1, 1, 1, 0.5f));
```

---

### SetGray(bool isGray)

**签名**:
```csharp
public void SetGray(bool isGray)
```

**职责**: 设置灰化效果

**参数**:
- `isGray`: 是否灰化

**核心逻辑**:
```
1. 保存 grayState
2. 激活 Image 组件
3. 加载灰化材质（如果是灰化）
4. 设置 image.material
```

**调用者**: 需要禁用图片但保持可见的场景

---

## 阅读指引

### 建议的阅读顺序

1. **理解图片组件作用** - 为什么需要 UIImage
2. **看 SetSpritePath** - 理解本地图片加载
3. **看 SetOnlineSpritePath** - 理解网络图片加载
4. **了解版本控制** - 理解 version 机制

### 最值得学习的技术点

1. **版本控制**: version 防止重复加载
2. **资源管理**: 自动释放图片资源
3. **网络图片**: 支持 URL 加载和缓存
4. **背景适配**: BgAutoFit 自动适配背景
5. **生命周期**: IOnCreate/IOnDestroy 自动管理

---

## 使用示例

### 示例 1: 本地图片

```csharp
public class UIItemView : UIBaseView, IOnCreate
{
    private UIImage icon;
    
    public void OnCreate()
    {
        icon = AddComponent<UIImage>("Icon");
    }
    
    public void SetItem(int itemId)
    {
        var config = ItemConfigCategory.Instance.Get(itemId);
        icon.SetSpritePath(config.IconPath);
    }
}
```

### 示例 2: 网络图片

```csharp
public class UIPlayerInfoView : UIBaseView, IOnCreate
{
    private UIImage avatar;
    
    public void OnCreate()
    {
        avatar = AddComponent<UIImage>("Avatar");
    }
    
    public void SetPlayer(string avatarUrl)
    {
        avatar.SetOnlineSpritePath(
            avatarUrl,
            defaultSpritePath: "UI/Icons/DefaultAvatar"
        );
    }
}
```

### 示例 3: 图片颜色

```csharp
// 设置红色
icon.SetColor("#FF0000");

// 设置半透明
icon.SetColor(new Color(1, 1, 1, 0.5f));

// 恢复原色
icon.SetColor("#FFFFFF");
```

### 示例 4: 灰化效果

```csharp
// 灰化（禁用状态）
await icon.SetGray(isGray: true);

// 恢复
await icon.SetGray(isGray: false);
```

---

## 相关文档

- [UIBaseContainer.cs.md](./UIBaseContainer.cs.md) - UI 容器基类
- [UIButton.cs.md](./UIButton.cs.md) - UI 按钮组件
- [ImageLoaderManager.cs.md](../Resource/ImageLoaderManager.cs.md) - 图片加载器

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
