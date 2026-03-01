# BgAutoFit.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|------|
| **文件名** | BgAutoFit.cs |
| **路径** | Assets/Scripts/Mono/Module/UI/BgAutoFit.cs |
| **所属模块** | 框架层 → Mono/Module/UI |
| **文件职责** | UI 背景图片自动适配屏幕尺寸，保持完整显示 |

---

## 类/结构体说明

### BgAutoFit

| 属性 | 说明 |
|------|------|
| **职责** | 附加到 UI Image 上，自动调整大小以完整显示背景图片 |
| **泛型参数** | 无 |
| **继承关系** | 继承 `MonoBehaviour` |
| **实现的接口** | 无 |

**设计模式**: 组件模式

```csharp
// 添加到 UI Background GameObject
// [RequireComponent(typeof(Image))] 自动添加 Image 组件

// 自动计算并调整 RectTransform.sizeDelta
// 确保背景图片完整显示，无黑边
```

---

## 字段与属性

### rectTransform

| 属性 | 值 |
|------|------|
| **类型** | `RectTransform` |
| **访问级别** | `private` |
| **说明** | 缓存的 RectTransform 组件 |

---

### bg

| 属性 | 值 |
|------|------|
| **类型** | `Image` |
| **访问级别** | `private` |
| **说明** | 缓存的 Image 组件 |

---

### bgSprite

| 属性 | 值 |
|------|------|
| **类型** | `Sprite` |
| **访问级别** | `public` |
| **说明** | 背景图片引用 |

**用途**: Inspector 中可配置，支持运行时更换

---

## 方法说明

### Awake

**签名**:
```csharp
private void Awake()
```

**职责**: 缓存组件引用

**核心逻辑**:
```
1. 获取 RectTransform 组件
2. 获取 Image 组件
```

**调用者**: Unity 生命周期

---

### Start

**签名**:
```csharp
void Start()
```

**职责**: 初始化背景图片并调整大小

**核心逻辑**:
```
1. 如果 bgSprite 为空，使用 Image 的当前 sprite
2. 否则设置 Image.sprite = bgSprite
3. 调用 Size() 调整大小
```

**调用者**: Unity 生命周期

---

### Size

**签名**:
```csharp
void Size()
```

**职责**: 计算并应用背景图片的适配尺寸

**核心逻辑**:
```
1. 获取屏幕尺寸 (screenWidth, screenHeight)
2. 计算屏幕宽高比 flagx = DesignScreenWidth / DesignScreenHeight
3. 计算实际宽高比 flagy = screenWidth / screenHeight
4. 计算缩放标志 signFlag:
   - 如果 flagx > flagy: signFlag = DesignScreenWidth / screenWidth
   - 否则：signFlag = DesignScreenHeight / screenHeight
5. 计算图片缩放比 flag1, flag2
6. 根据 flag1 < flag2 决定适配策略:
   - flag1 < flag2: 高度适配，宽度按比例
   - 否则：宽度适配，高度按比例
7. 设置 rectTransform.sizeDelta
8. 处理 Canvas 父对象（运行时重新挂载）
```

**调用者**: Start(), SetSprite()

---

### SetSprite

**签名**:
```csharp
public void SetSprite(Sprite newBgSprite)
```

**职责**: 运行时更换背景图片并重新适配

**核心逻辑**:
```
1. 设置 bgSprite = newBgSprite
2. 如果 bgSprite 为空，使用 Image 的当前 sprite
3. 否则设置 Image.sprite = bgSprite
4. 调用 Size() 重新调整大小
```

**参数**:
| 参数名 | 类型 | 说明 |
|--------|------|------|
| `newBgSprite` | `Sprite` | 新的背景图片 |

**调用者**: 运行时更换背景的代码

---

## 适配算法详解

### 屏幕比例计算

```csharp
// 设计分辨率
var flagx = Define.DesignScreenWidth / Define.DesignScreenHeight;

// 实际屏幕分辨率
var flagy = (float)screenW / screenH;

// 计算缩放标志
var signFlag = flagx > flagy
    ? Define.DesignScreenWidth / screenW
    : Define.DesignScreenHeight / screenH;
```

**示意图**:
```
设计分辨率：1920x1080 (16:9)
实际屏幕：2340x1080 (19.5:9)

flagx = 1920/1080 = 1.78
flagy = 2340/1080 = 2.17

flagx < flagy → 实际屏幕更宽
signFlag = 1080/1080 = 1.0 (高度基准)
```

### 图片适配策略

```csharp
// 图片缩放比
var flag1 = screenW / texture.bounds.size.x;
var flag2 = screenH / texture.bounds.size.y;

if (flag1 < flag2)
    // 高度适配，宽度按比例
    rectTransform.sizeDelta = new Vector2(
        flag2 * texture.bounds.size.x * signFlag, 
        screenH * signFlag
    );
else
    // 宽度适配，高度按比例
    rectTransform.sizeDelta = new Vector2(
        screenW * signFlag, 
        flag1 * texture.bounds.size.y * signFlag
    );
```

**适配原则**:
- **Cover 模式**: 确保图片完全覆盖屏幕
- 可能有部分图片超出屏幕（被裁剪）
- 无黑边

---

## Canvas 父对象处理

### 运行时重新挂载

```csharp
if (Application.isPlaying && canvas != null)
{
    // 保存当前父对象和索引
    var parent = transform.parent;
    var siblingIndex = transform.GetSiblingIndex();
    
    // 临时挂载到 Canvas 根节点
    transform.SetParent(canvas.transform);
    rectTransform.localPosition = Vector3.zero;
    rectTransform.anchoredPosition = Vector2.zero;
    
    // 恢复原父对象
    transform.SetParent(parent, true);
    transform.SetSiblingIndex(siblingIndex);
}
```

**目的**:
- 确保位置计算基于 Canvas 坐标系
- 避免嵌套 Canvas 导致的位置偏移
- 保持原有层级结构

---

## 使用示例

### 示例 1: 基本使用

```
1. 创建 UI Background GameObject
2. 添加 Image 组件
3. 添加 BgAutoFit 组件
4. 设置 bgSprite
5. 自动适配屏幕
```

### 示例 2: 运行时更换背景

```csharp
// 获取组件
var bgAutoFit = GetComponent<BgAutoFit>();

// 更换背景
bgAutoFit.SetSprite(newBackgroundSprite);
```

### 示例 3: 多背景切换

```csharp
public class BackgroundManager : MonoBehaviour
{
    public Sprite[] backgrounds;
    private BgAutoFit bgAutoFit;
    private int currentIndex;
    
    void Start()
    {
        bgAutoFit = GetComponent<BgAutoFit>();
    }
    
    public void NextBackground()
    {
        currentIndex = (currentIndex + 1) % backgrounds.Length;
        bgAutoFit.SetSprite(backgrounds[currentIndex]);
    }
}
```

---

## 设计要点

### 为什么使用 [ExecuteAlways]？

```csharp
[ExecuteAlways]
public class BgAutoFit : MonoBehaviour
```

**作用**:
- 在编辑器模式下也执行 Start/Update
- 方便编辑器中预览适配效果
- 支持运行时和编辑器双模式

### 为什么处理 Canvas 父对象？

```csharp
transform.SetParent(canvas.transform);
// 设置位置
transform.SetParent(parent, true);
```

**原因**:
- UI 位置计算依赖 Canvas 坐标系
- 嵌套 Canvas 会导致位置偏移
- 临时挂载确保计算正确

### 为什么使用 signFlag？

```csharp
var signFlag = flagx > flagy
    ? Define.DesignScreenWidth / screenW
    : Define.DesignScreenHeight / screenH;
```

**作用**:
- 统一不同分辨率的缩放基准
- 确保设计分辨率与实际分辨率的映射
- 保持 UI 元素相对大小一致

---

## 适配效果对比

### 不同屏幕比例

| 屏幕比例 | 适配策略 | 效果 |
|----------|----------|------|
| 16:9 (设计) | 基准 | 完美适配 |
| 19.5:9 (全面屏) | 高度适配 | 两侧可能有裁剪 |
| 4:3 (平板) | 宽度适配 | 上下可能有裁剪 |
| 21:9 (超宽屏) | 高度适配 | 两侧大量裁剪 |

### Cover vs Fit

```
Cover (BgAutoFit 使用):
┌──────────────────────┐
│ ╔════════════════╗   │
│ ║   图片内容     ║   │ ← 图片覆盖整个屏幕
│ ╚════════════════╝   │ ← 可能有裁剪
└──────────────────────┘

Fit (另一种策略):
┌──────────────────────┐
│ ┌────────────────┐   │
│ │                │   │ ← 图片完整显示
│ │   图片内容     │   │ ← 可能有黑边
│ │                │   │
│ └────────────────┘   │
└──────────────────────┘
```

---

## 性能优化建议

### 1. 避免每帧计算

```csharp
// ❌ 不推荐：每帧调用 Size()
void Update()
{
    Size();
}

// ✅ 推荐：仅在需要时调用
public void OnScreenResize()
{
    Size();
}
```

### 2. 缓存计算结果

```csharp
private Vector2 lastSize;

void Size()
{
    Vector2 newSize = CalculateSize();
    if (newSize == lastSize) return; // 无变化则跳过
    lastSize = newSize;
    rectTransform.sizeDelta = newSize;
}
```

### 3. 使用事件监听

```csharp
// 监听屏幕分辨率变化事件
ScreenResizeEvent.OnResize += () => Size();
```

---

## 相关文档

- [BgRawAutoFit.cs.md](./BgRawAutoFit.cs.md) - RawImage 背景适配
- [BgAutoMax.cs.md](./BgAutoMax.cs.md) - 背景最大适配
- [SystemInfoHelper.cs.md](../../Helper/SystemInfoHelper.cs.md) - 系统信息助手
- [Define.cs.md](../../Define.cs.md) - 全局常量定义

---

*文档生成时间：2026-03-01 | OpenClaw AI 助手*
