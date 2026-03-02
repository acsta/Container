# Drag.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | Drag.cs |
| **路径** | Assets/Scripts/Mono/Module/UI/Drag.cs |
| **所属模块** | 框架层 → Mono/Module/UI |
| **文件职责** | UI 拖拽事件封装组件，将 Unity EventSystem 的拖拽事件转换为 UnityEvent |

---

## 类说明

### Drag

| 属性 | 说明 |
|------|------|
| **职责** | 附加到 UI 元素上，提供拖拽开始、拖拽中、拖拽结束三个阶段的 UnityEvent 回调 |
| **继承关系** | `MonoBehaviour` |
| **实现的接口** | `IDragHandler`, `IBeginDragHandler`, `IEndDragHandler` |

**设计模式**: 事件代理 + 组件模式

```csharp
// 使用方式
// 1. 在 UI 元素上添加 Drag 组件
// 2. 在 Inspector 中配置 OnBeginDragHandler / OnDragHandler / OnEndDragHandler
// 3. 拖拽时会自动触发对应事件
```

**EventSystem 要求**:
- 需要 EventSystem 组件存在
- UI 元素需要有 Image 或其他可射线检测组件
- 或者添加 RaycastTarget 组件

---

## 字段与属性

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `OnBeginDragHandler` | `UnityEvent<PointerEventData>` | `public` | 拖拽开始事件 |
| `OnDragHandler` | `UnityEvent<PointerEventData>` | `public` | 拖拽进行中事件 |
| `OnEndDragHandler` | `UnityEvent<PointerEventData>` | `public` | 拖拽结束事件 |

---

## 方法说明

### OnBeginDrag(PointerEventData eventData)

**签名**:
```csharp
public void OnBeginDrag(PointerEventData eventData)
```

**职责**: 当拖拽开始时调用

**核心逻辑**:
```
1. 调用 OnBeginDragHandler?.Invoke(eventData)
2. 通知所有订阅者拖拽开始
```

**调用者**: Unity EventSystem（自动调用）

**参数说明**:
| 参数 | 类型 | 说明 |
|------|------|------|
| `eventData` | `PointerEventData` | 指针事件数据（包含位置、按钮状态等） |

---

### OnDrag(PointerEventData eventData)

**签名**:
```csharp
public void OnDrag(PointerEventData eventData)
```

**职责**: 当拖拽进行中每帧调用

**核心逻辑**:
```
1. 调用 OnDragHandler?.Invoke(eventData)
2. 通知所有订阅者拖拽位置更新
```

**调用者**: Unity EventSystem（每帧调用）

---

### OnEndDrag(PointerEventData eventData)

**签名**:
```csharp
public void OnEndDrag(PointerEventData eventData)
```

**职责**: 当拖拽结束时调用

**核心逻辑**:
```
1. 调用 OnEndDragHandler?.Invoke(eventData)
2. 通知所有订阅者拖拽结束
```

**调用者**: Unity EventSystem（自动调用）

---

## PointerEventData 常用属性

| 属性 | 类型 | 说明 |
|------|------|------|
| `position` | `Vector2` | 当前屏幕坐标 |
| `delta` | `Vector2` | 相对于上一帧的位移 |
| `pressPosition` | `Vector2` | 按下时的初始位置 |
| `button` | `PointerEventData.InputButton` | 按下的按钮（Left/Right/Middle） |
| `pointerCurrentRaycast` | `RaycastResult` | 当前射线检测结果 |

---

## 使用示例

### 示例 1: 基础拖拽移动 UI 元素

```csharp
public class DraggableUI : MonoBehaviour
{
    [SerializeField] private Drag dragComponent;
    
    void Awake()
    {
        dragComponent.OnBeginDragHandler.AddListener(OnBeginDrag);
        dragComponent.OnDragHandler.AddListener(OnDrag);
        dragComponent.OnEndDragHandler.AddListener(OnEndDrag);
    }
    
    void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("开始拖拽");
    }
    
    void OnDrag(PointerEventData eventData)
    {
        // 跟随鼠标移动
        Vector2 delta = eventData.delta;
        transform.position += new Vector3(delta.x, delta.y, 0);
    }
    
    void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("结束拖拽");
    }
}
```

### 示例 2: Inspector 配置方式

```
步骤：
1. 选中 UI 元素
2. 添加 Drag 组件
3. 在 Inspector 中展开 OnDragHandler
4. 点击 "+" 添加监听
5. 拖拽目标对象到 Object 字段
6. 选择回调函数

示例配置：
┌─────────────────────────────────────┐
│ Drag (Script)                       │
├─────────────────────────────────────┤
│ On Begin Drag Handler ()            │
│   └─ [+]                            │
│ On Drag Handler ()                  │
│   └─ [+] → CanvasPanel.OnItemDrag   │
│ On End Drag Handler ()              │
│   └─ [+] → CanvasPanel.OnItemEndDrag│
└─────────────────────────────────────┘
```

### 示例 3: 拖拽排序列表

```csharp
public class DragSortList : MonoBehaviour
{
    [SerializeField] private Drag dragComponent;
    private RectTransform rectTransform;
    private int originalSiblingIndex;
    
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        dragComponent.OnBeginDragHandler.AddListener(OnBeginDrag);
        dragComponent.OnDragHandler.AddListener(OnDrag);
        dragComponent.OnEndDragHandler.AddListener(OnEndDrag);
    }
    
    void OnBeginDrag(PointerEventData eventData)
    {
        originalSiblingIndex = transform.GetSiblingIndex();
        // 拖拽时置顶显示
        transform.SetAsLastSibling();
    }
    
    void OnDrag(PointerEventData eventData)
    {
        // 跟随鼠标移动
        rectTransform.anchoredPosition += eventData.delta;
    }
    
    void OnEndDrag(PointerEventData eventData)
    {
        // 计算新位置
        int newIndex = CalculateNewIndex();
        transform.SetSiblingIndex(newIndex);
        
        // 通知列表更新
        ListManager.Instance.OnItemReordered(originalSiblingIndex, newIndex);
    }
    
    int CalculateNewIndex()
    {
        // 根据 Y 位置计算新索引
        // ...
        return 0;
    }
}
```

### 示例 4: 拖拽物品到背包

```csharp
public class ItemDragHandler : MonoBehaviour
{
    [SerializeField] private Drag dragComponent;
    [SerializeField] private ItemData itemData;
    
    void Awake()
    {
        dragComponent.OnBeginDragHandler.AddListener(OnBeginDrag);
        dragComponent.OnEndDragHandler.AddListener(OnEndDrag);
    }
    
    void OnBeginDrag(PointerEventData eventData)
    {
        // 创建拖拽预览
        DragPreviewManager.Instance.ShowPreview(itemData);
    }
    
    void OnEndDrag(PointerEventData eventData)
    {
        // 检查是否 dropped 到背包槽位
        var raycastResult = eventData.pointerCurrentRaycast;
        if (raycastResult.gameObject != null)
        {
            var slot = raycastResult.gameObject.GetComponent<ItemSlot>();
            if (slot != null)
            {
                InventoryManager.Instance.MoveItemToSlot(itemData, slot);
            }
        }
        
        DragPreviewManager.Instance.HidePreview();
    }
}
```

### 示例 5: 限制拖拽区域

```csharp
public class ConstrainedDrag : MonoBehaviour
{
    [SerializeField] private Drag dragComponent;
    [SerializeField] private RectTransform boundsRect;
    private RectTransform rectTransform;
    
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        dragComponent.OnDragHandler.AddListener(OnDrag);
    }
    
    void OnDrag(PointerEventData eventData)
    {
        Vector2 newPosition = rectTransform.anchoredPosition + eventData.delta;
        
        // 限制在边界内
        newPosition.x = Mathf.Clamp(newPosition.x, 
            boundsRect.rect.xMin, boundsRect.rect.xMax);
        newPosition.y = Mathf.Clamp(newPosition.y, 
            boundsRect.rect.yMin, boundsRect.rect.yMax);
        
        rectTransform.anchoredPosition = newPosition;
    }
}
```

---

## Unity EventSystem 集成

### 必需组件

```
UI GameObject
├── Image (或其他 Graphic)
│   └── Raycast Target = true
├── Drag (本组件)
└── Canvas (父级或祖先)

场景中还必须有:
└── EventSystem
    ├── Standalone Input Module
    └── (或其他输入模块)
```

### 接口说明

```csharp
// Unity EventSystem 接口
public interface IBeginDragHandler : IEventSystemHandler
{
    void OnBeginDrag(PointerEventData eventData);
}

public interface IDragHandler : IEventSystemHandler
{
    void OnDrag(PointerEventData eventData);
}

public interface IEndDragHandler : IEventSystemHandler
{
    void OnEndDrag(PointerEventData eventData);
}
```

---

## 技术要点

### 1. UnityEvent 序列化

```csharp
// UnityEvent 可在 Inspector 中配置
public UnityEvent<PointerEventData> OnDragHandler = new UnityEvent<PointerEventData>();

// 支持多监听器
OnDragHandler.AddListener(callback1);
OnDragHandler.AddListener(callback2);
```

### 2. 空条件运算符

```csharp
// 安全调用
OnDragHandler?.Invoke(eventData);

// 等价于
if (OnDragHandler != null)
    OnDragHandler.Invoke(eventData);
```

### 3. 射线检测要求

```csharp
// 确保 UI 元素可被射线检测
Image image = GetComponent<Image>();
if (image != null)
{
    image.raycastTarget = true;  // 必须开启
}
```

---

## 相关文档

- **PointerClick**: [PointerClick.cs.md](./PointerClick.cs.md) - 点击事件组件
- **UIManager**: [UIManager.cs.md](../../../Code/Module/UI/UIManager.cs.md) - UI 管理器
- **Unity 文档**: EventSystem - Unity 官方文档

---

## 注意事项

### ⚠️ EventSystem 必需

场景中必须有 EventSystem 组件，否则拖拽事件不会触发。

### ⚠️ Raycast Target

确保 UI 元素的 Graphic 组件开启了 `raycastTarget`。

### ⚠️ Canvas 层级

Drag 组件需要在 Canvas 下的 UI 元素上才能正常工作。

### ⚠️ 性能考虑

`OnDrag` 每帧调用，避免在回调中进行复杂计算。

### ⚠️ 内存管理

使用代码添加的监听器需要在 OnDestroy 时移除：
```csharp
void OnDestroy()
{
    dragComponent.OnDragHandler.RemoveListener(OnDrag);
}
```

---

*文档生成时间：2026-03-01 | OpenClaw AI 助手*
