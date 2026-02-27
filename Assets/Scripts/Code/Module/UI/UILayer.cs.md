# UILayer.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | UILayer.cs |
| **路径** | Assets/Scripts/Code/Module/UI/UILayer.cs |
| **所属模块** | 框架层 → Code/Module/UI |
| **文件职责** | UI 层级类，封装 Unity Canvas 及相关组件 |

---

## 类/结构体说明

### UILayer

| 属性 | 说明 |
|------|------|
| **职责** | 封装 UI 层级的 Unity 组件，提供便捷的访问接口 |
| **泛型参数** | 无 |
| **继承关系** | 无继承 |
| **实现的接口** | `IManager<UILayerDefine, GameObject>` |

**设计模式**: Manager 模式（通过 ManagerProvider 管理）

```csharp
public class UILayer : IManager<UILayerDefine, GameObject>
{
    // UI 层级类，封装 Canvas 等组件
}
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `Name` | `UILayerNames` | `public` | 层级名称 |
| `Canvas` | `Canvas` | `public` | Unity Canvas 组件 |
| `CanvasScaler` | `CanvasScaler` | `public` | UI 缩放组件 |
| `GraphicRaycaster` | `GraphicRaycaster` | `public` | UI 射线检测组件 |
| `RectTransform` | `RectTransform` | `public` | UI 变换组件 |
| `GameObject` | `GameObject` | `public` | 层级 GameObject |

---

## 方法说明（按重要程度排序）

### Init(UILayerDefine layer, GameObject gameObject)

**签名**:
```csharp
public void Init(UILayerDefine layer, GameObject gameObject)
```

**职责**: 初始化 UI 层级，配置所有 Unity 组件

**核心逻辑**:
```
1. 设置 Name 和 GameObject
2. 获取或添加 Canvas 组件
   - 设置 renderMode=ScreenSpaceCamera
   - 设置 worldCamera=UIManager.UICamera
   - 设置 planeDistance
   - 设置 sortingLayerName="UI"
   - 设置 sortingOrder
3. 获取或添加 CanvasScaler 组件
   - 设置 uiScaleMode=ScaleWithScreenSize
   - 设置 screenMatchMode=MatchWidthOrHeight
   - 设置 referenceResolution
   - 设置 matchWidthOrHeight（根据层级和屏幕比例）
4. 获取或添加 GraphicRaycaster 组件
5. 获取 RectTransform
```

**调用者**: ManagerProvider.RegisterManager<UILayer, UILayerDefine, GameObject>()

**被调用者**: `GameObject.AddComponent<T>()`, `GameObject.TryGetComponent<T>()`

**代码示例**:
```csharp
public void Init(UILayerDefine layer, GameObject gameObject)
{
    this.Name = layer.Name;
    this.GameObject = gameObject;

    // Canvas
    if (!this.GameObject.TryGetComponent(out Canvas canvas))
    {
        this.Canvas = this.GameObject.AddComponent<Canvas>();
        this.GameObject = this.Canvas.gameObject;  // ⚠️ 重要：AddComponent 后 GameObject 可能变化
    }
    else
    {
        this.Canvas = canvas;
    }

    this.Canvas.renderMode = 
        PlatformUtil.IsWebGl1() ? RenderMode.ScreenSpaceOverlay : RenderMode.ScreenSpaceCamera;
    this.Canvas.worldCamera = UIManager.Instance.UICamera;
    this.Canvas.planeDistance = layer.PlaneDistance;
    this.Canvas.sortingLayerName = SortingLayerNames.UI;
    this.Canvas.sortingOrder = layer.OrderInLayer;

    // CanvasScaler
    if (!this.GameObject.TryGetComponent(out CanvasScaler canvasScaler))
    {
        this.CanvasScaler = this.GameObject.AddComponent<CanvasScaler>();
    }
    else
    {
        this.CanvasScaler = canvasScaler;
    }
    this.CanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
    this.CanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
    this.CanvasScaler.referenceResolution = UIManager.Instance.Resolution;
    
    // 根据层级和屏幕比例决定匹配方式
    if (layer.Name == UILayerNames.GameLayer || 
        (float)Screen.width / Screen.height > Define.DesignScreenWidth / Define.DesignScreenHeight)
        this.CanvasScaler.matchWidthOrHeight = 1;  // 匹配宽度
    else
        this.CanvasScaler.matchWidthOrHeight = 0;  // 匹配高度

    // GraphicRaycaster
    if (!this.GameObject.TryGetComponent(out GraphicRaycaster graphicRaycaster))
    {
        this.GraphicRaycaster = this.GameObject.AddComponent<GraphicRaycaster>();
    }
    else
    {
        this.GraphicRaycaster = graphicRaycaster;
    }

    this.RectTransform = this.GameObject.GetComponent<RectTransform>();
}
```

---

### Destroy()

**签名**:
```csharp
public void Destroy()
```

**职责**: 销毁 UI 层级，清空组件引用

**核心逻辑**:
```
1. 清空所有组件引用
2. 不直接 Destroy GameObject（由 UIManager.DestroyLayer 统一处理）
```

**调用者**: ManagerProvider.RemoveManager<UILayer>()

---

### SetCanvasScaleEditorPortrait(bool flag)

**签名**:
```csharp
public void SetCanvasScaleEditorPortrait(bool flag)
```

**职责**: Editor 下调整 Canvas 缩放（竖屏模式）

**核心逻辑**:
```
1. 如果 flag = true（竖屏）:
   - referenceResolution = (DesignScreenHeight, DesignScreenWidth)  // 交换宽高
   - matchWidthOrHeight = 0
2. 如果 flag = false（横屏）:
   - referenceResolution = 正常分辨率
   - matchWidthOrHeight = 1
```

**调用者**: `UIManager.SetCanvasScaleEditorPortrait()`

**使用场景**: Editor 下调试竖屏 UI 适配

---

## Unity 组件配置详解

### Canvas 配置

| 属性 | 值 | 说明 |
|------|-----|------|
| `renderMode` | ScreenSpaceCamera / ScreenSpaceOverlay | WebGL 1 使用 Overlay，其他使用 Camera |
| `worldCamera` | UIManager.UICamera | UI 专用相机 |
| `planeDistance` | 400-1000 | 层级配置决定 |
| `sortingLayerName` | "UI" | 统一使用 UI 层 |
| `sortingOrder` | 0-5000 | 层级配置决定 |

### CanvasScaler 配置

| 属性 | 值 | 说明 |
|------|-----|------|
| `uiScaleMode` | ScaleWithScreenSize | 随屏幕尺寸缩放 |
| `screenMatchMode` | MatchWidthOrHeight | 根据宽高比匹配 |
| `referenceResolution` | (DesignScreenWidth, DesignScreenHeight) | 设计分辨率 |
| `matchWidthOrHeight` | 0 或 1 | 0=匹配宽度，1=匹配高度 |

### GraphicRaycaster

| 属性 | 值 | 说明 |
|------|-----|------|
| `ignoreReversedGraphics` | true (默认) | 忽略反向图形 |
| `blockingObjects` | None (默认) | 无阻挡对象 |

---

## 阅读指引

### 建议的阅读顺序

1. **看字段定义** - 了解封装的 Unity 组件
2. **看 Init 方法** - 理解组件配置流程
3. **注意 GameObject 变化** - AddComponent 后可能变化
4. **看屏幕适配逻辑** - matchWidthOrHeight 的计算

### 最值得学习的技术点

1. **Manager 接口**: `IManager<UILayerDefine, GameObject>` 带参数初始化
2. **组件安全获取**: TryGetComponent + AddComponent 模式
3. **GameObject 变化**: AddComponent 后重新获取 GameObject
4. **屏幕适配**: 根据屏幕比例动态调整 matchWidthOrHeight
5. **平台适配**: WebGL 1 使用 Overlay，其他使用 Camera

---

## 使用示例

### 示例 1: 通过 ManagerProvider 创建

```csharp
// 创建层级 GameObject
var go = new GameObject("NormalLayer") { layer = 5 };
go.transform.SetParent(UIRoot.transform, false);

// 注册 UILayer Manager
UILayer layer = ManagerProvider.RegisterManager<UILayer, UILayerDefine, GameObject>(
    new UILayerDefine 
    { 
        Name = UILayerNames.NormalLayer,
        PlaneDistance = 600,
        OrderInLayer = 3000
    },
    go,
    "NormalLayer"
);

// 访问组件
Canvas canvas = layer.Canvas;
RectTransform rectTransform = layer.RectTransform;
```

### 示例 2: 设置 UI 父节点

```csharp
// 获取层级
UILayer normalLayer = UIManager.Instance.GetLayer(UILayerNames.NormalLayer);

// 设置 UI 父节点
Transform uiTrans = uiView.GetTransform();
uiTrans.SetParent(normalLayer.RectTransform, false);
uiTrans.SetAsLastSibling();  // 置于顶层
```

---

## 注意事项

### ⚠️ AddComponent 后 GameObject 可能变化

```csharp
// 坑爹：添加 UI 组件后 transform 会被 Unity 替换掉，必须重新获取
if (!this.GameObject.TryGetComponent(out Canvas canvas))
{
    this.Canvas = this.GameObject.AddComponent<Canvas>();
    this.GameObject = this.Canvas.gameObject;  // ← 重要！
}
```

### matchWidthOrHeight 计算

```csharp
// GameLayer 或 宽屏 → 匹配宽度 (1)
// 其他 → 匹配高度 (0)
if (layer.Name == UILayerNames.GameLayer || 
    (float)Screen.width / Screen.height > Define.DesignScreenWidth / Define.DesignScreenHeight)
    this.CanvasScaler.matchWidthOrHeight = 1;
else
    this.CanvasScaler.matchWidthOrHeight = 0;
```

---

## 相关文档

- [UIManager.Layers.cs.md](./UIManager.Layers.cs.md) - 层级系统初始化
- [UILayerNames.cs.md](./UILayerNames.cs.md) - UI 层级枚举
- [UIManager.cs.md](./UIManager.cs.md) - UI 管理器

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
