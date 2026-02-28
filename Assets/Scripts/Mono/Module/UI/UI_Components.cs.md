# UI 组件综合文档

> **覆盖文件**: ReferenceCollector, CopyGameObject, Drag, PointerClick, CircleImage, CircleRawImage, BgAutoFit, BgRawAutoFit, BgAutoMax, TextColorCtrl, BackgroundBlur, EmptyGraphic, ScrollViewEventRaycast, UIScriptCreator  
> **生成时间**: 2026-02-28  
> **命名空间**: `TaoTie`

---

## 📑 目录

1. [ReferenceCollector - 引用收集器](#1-referencecollector---引用收集器)
2. [CopyGameObject - 列表复制](#2-copygameobject---列表复制)
3. [Drag - 拖拽组件](#3-drag---拖拽组件)
4. [PointerClick - 点击组件](#4-pointerclick---点击组件)
5. [CircleImage/CircleRawImage - 圆形图片](#5-circleimagecirclerawimage---圆形图片)
6. [BgAutoFit 系列 - 背景自适应](#6-bgautofit 系列---背景自适应)
7. [其他 UI 组件](#7-其他 ui 组件)

---

## 1. ReferenceCollector - 引用收集器

### 类说明

| 属性 | 说明 |
|------|------|
| **职责** | Inspector 面板引用管理，序列化/反序列化 Unity 对象引用 |
| **继承** | `MonoBehaviour`, `ISerializationCallbackReceiver` |
| **使用场景** | Prefab 引用管理、代码访问 Inspector 配置的引用 |

### 数据结构

```csharp
[Serializable]
public class ReferenceCollectorData
{
    public string key;          // 引用键名
    public Object gameObject;   // Unity 对象引用
}
```

### Inspector 使用

1. **添加组件**: 在 GameObject 上添加 `ReferenceCollector` 组件
2. **配置引用**: 在 Inspector 中配置 key 和对应的 GameObject/Component
3. **排序**: 点击 Sort 按钮按 key 排序

### API

```csharp
// 获取组件（泛型）
T comp = referenceCollector.Get<T>("button");

// 获取对象
Object obj = referenceCollector.GetObject("panel");

// 编辑器下添加/删除引用（仅 UNITY_EDITOR）
#if UNITY_EDITOR
referenceCollector.Add("newRef", someObject);
referenceCollector.Remove("oldRef");
referenceCollector.Clear();
#endif
```

### 使用示例

#### Prefab 引用管理

```csharp
// 在 Prefab 上配置 ReferenceCollector
// key: "btnSubmit" → Button 组件
// key: "txtTitle" → Text 组件
// key: "panel" → Panel GameObject

public class MyUIView : MonoBehaviour
{
    private ReferenceCollector refCollector;
    private Button btnSubmit;
    private Text txtTitle;
    
    void Awake()
    {
        refCollector = GetComponent<ReferenceCollector>();
        
        // 获取引用
        btnSubmit = refCollector.Get<Button>("btnSubmit");
        txtTitle = refCollector.Get<Text>("txtTitle");
        
        // 绑定事件
        btnSubmit.onClick.AddListener(OnSubmitClick);
    }
    
    void OnSubmitClick()
    {
        Log.Info("Submit clicked");
    }
}
```

#### 与 UIBaseContainer 集成

```csharp
// UIBaseContainer 内部使用 ReferenceCollector
Transform ActivatingComponent()
{
    if (this.transform == null)
    {
        var pTrans = this.GetParentTransform();
        var rc = pTrans.GetComponent<ReferenceCollector>();
        
        if (rc != null)
        {
            // 从 ReferenceCollector 获取引用（高性能）
            transform = rc.Get<Transform>(path);
        }
        
        if (this.transform == null)
        {
            // 回退到 Find（慢）
            this.transform = pTrans.Find(path);
        }
    }
    return this.transform;
}
```

---

## 2. CopyGameObject - 列表复制

### 类说明

| 属性 | 说明 |
|------|------|
| **职责** | 动态复制列表项，用于简单列表/网格布局 |
| **继承** | `MonoBehaviour` |
| **使用场景** | 简单列表、选项卡、物品栏 |

### API

```csharp
// 初始化列表
public void InitListView(
    int totalCount, 
    Action<int, GameObject> onGetItemCallback = null,
    int? startSiblingIndex = null
)

// 设置列表数量
public void SetListItemCount(int totalCount, int? startSiblingIndex = null)

// 刷新所有显示项
public void RefreshAllShownItem(int? startSiblingIndex = null)

// 获取项
public GameObject GetItemByIndex(int index)

// 获取数量
public int GetListItemCount()

// 清空
public void Clear()
```

### 使用示例

#### 简单列表

```csharp
public class ItemList : MonoBehaviour
{
    public CopyGameObject copyGameObject;
    public List<ItemData> items;
    
    void Start()
    {
        // 初始化列表
        copyGameObject.InitListView(
            items.Count,
            OnGetItem
        );
    }
    
    void OnGetItem(int index, GameObject itemObj)
    {
        // 配置每个列表项
        ItemView itemView = itemObj.GetComponent<ItemView>();
        itemView.SetData(items[index]);
    }
}
```

#### 动态更新

```csharp
public void UpdateList(List<ItemData> newItems)
{
    items = newItems;
    
    // 更新数量
    copyGameObject.SetListItemCount(items.Count);
    
    // 刷新显示
    copyGameObject.RefreshAllShownItem();
}
```

### ⚠️ 注意事项

```csharp
// 代码中提示：totalCount > 10 时会输出警告
// 原因：CopyGameObject 不是对象池，超过 10 个建议用 UILoopListView2
if (totalCount > 10) Debug.Log("totalCount 不建议超过 10 个");

// 大量列表项请使用 UILoopGridView/UILoopListView2（对象池优化）
```

---

## 3. Drag - 拖拽组件

### 类说明

| 属性 | 说明 |
|------|------|
| **职责** | 封装 Unity 拖拽事件，提供 UnityEvent 回调 |
| **继承** | `MonoBehaviour`, `IDragHandler`, `IBeginDragHandler`, `IEndDragHandler` |
| **使用场景** | 拖拽 UI、拖拽物品、滑动控制 |

### API

```csharp
// 事件（Inspector 中可配置）
public UnityEvent<PointerEventData> OnBeginDragHandler;
public UnityEvent<PointerEventData> OnDragHandler;
public UnityEvent<PointerEventData> OnEndDragHandler;
```

### 使用示例

#### Inspector 配置

1. 添加 `Drag` 组件到 GameObject
2. 在 Inspector 中配置事件：
   - On Begin Drag → 拖拽开始回调
   - On Drag → 拖拽中回调
   - On End Drag → 拖拽结束回调

#### 代码绑定

```csharp
public class DraggableUI : MonoBehaviour
{
    private Drag drag;
    
    void Awake()
    {
        drag = GetComponent<Drag>();
        
        // 绑定事件
        drag.OnBeginDragHandler.AddListener(OnBeginDrag);
        drag.OnDragHandler.AddListener(OnDrag);
        drag.OnEndDragHandler.AddListener(OnEndDrag);
    }
    
    void OnBeginDrag(PointerEventData eventData)
    {
        Log.Info("Begin drag");
    }
    
    void OnDrag(PointerEventData eventData)
    {
        // 跟随鼠标移动
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition += eventData.delta;
    }
    
    void OnEndDrag(PointerEventData eventData)
    {
        Log.Info("End drag");
    }
}
```

---

## 4. PointerClick - 点击组件

### 类说明

| 属性 | 说明 |
|------|------|
| **职责** | 封装 Unity 点击事件，提供 UnityEvent 回调 |
| **继承** | `MonoBehaviour`, `IPointerClickHandler` |
| **使用场景** | 需要点击回调但无 Button 组件的 UI |

### API

```csharp
public UnityEvent onClick;
```

### 使用示例

#### Inspector 配置

1. 添加 `PointerClick` 组件
2. 在 Inspector 中配置 On Click 事件

#### 代码绑定

```csharp
public class ClickableItem : MonoBehaviour
{
    private PointerClick pointerClick;
    
    void Awake()
    {
        pointerClick = GetComponent<PointerClick>();
        pointerClick.onClick.AddListener(OnClick);
    }
    
    void OnClick()
    {
        Log.Info("Item clicked");
    }
}
```

---

## 5. CircleImage/CircleRawImage - 圆形图片

### 类说明

| 属性 | 说明 |
|------|------|
| **职责** | 渲染圆形/扇形图片，支持进度控制 |
| **继承** | `Image` / `RawImage` |
| **使用场景** | 进度环、技能 CD、血量环 |

### 核心属性

```csharp
// CircleImage
public float fillAmount;      // 填充量 0-1
public bool fillClockwise;    // 顺时针填充
public int segments;          // 分段数（圆滑度）
```

### 使用示例

#### 技能 CD 环

```csharp
public class SkillCooldownRing : MonoBehaviour
{
    private CircleImage circleImage;
    private float cooldownDuration = 10f;
    private float remainingTime;
    
    void Awake()
    {
        circleImage = GetComponent<CircleImage>();
    }
    
    public void StartCooldown()
    {
        remainingTime = cooldownDuration;
        StartCoroutine(UpdateCooldown());
    }
    
    IEnumerator UpdateCooldown()
    {
        while (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            circleImage.fillAmount = remainingTime / cooldownDuration;
            yield return null;
        }
        circleImage.fillAmount = 0;
    }
}
```

---

## 6. BgAutoFit 系列 - 背景自适应

### 组件对比

| 组件 | 继承 | 功能 |
|------|------|------|
| **BgAutoFit** | `MonoBehaviour` | 背景自适应内容大小（Image） |
| **BgRawAutoFit** | `MonoBehaviour` | 背景自适应内容大小（RawImage） |
| **BgAutoMax** | `MonoBehaviour` | 背景自适应最大子物体 |

### 使用示例

#### 对话框背景自适应

```csharp
// Unity 配置:
// DialogPanel (BgAutoFit)
// └── Content (ContentSizeFitter)
//     └── Text (动态内容)

// 当 Text 内容变化时，Content 自动调整大小
// BgAutoFit 监听 Content 变化，自动调整背景大小
```

#### 代码控制

```csharp
public class DialogBox : MonoBehaviour
{
    private BgAutoFit bgAutoFit;
    private Text contentText;
    
    void Awake()
    {
        bgAutoFit = GetComponent<BgAutoFit>();
        contentText = transform.Find("Content/Text").GetComponent<Text>();
    }
    
    public void SetContent(string text)
    {
        contentText.text = text;
        // BgAutoFit 会自动调整背景大小
    }
}
```

---

## 7. 其他 UI 组件

### TextColorCtrl - 文本颜色控制

```csharp
// 功能：支持富文本颜色标签
// 使用：<color=red>红色文本</color>
```

### BackgroundBlur - 背景模糊

```csharp
// 功能：UI 背景高斯模糊
// 使用场景：弹窗背景、菜单背景
```

### EmptyGraphic - 空占位

```csharp
// 功能：透明占位组件，用于接收点击事件
// 使用场景：透明点击区域、占位布局
```

### ScrollViewEventRaycast - 滚动视图射线

```csharp
// 功能：优化 ScrollView 射线检测
// 使用场景：嵌套滚动视图、复杂滚动布局
```

### UIScriptCreator - UI 脚本生成器

```csharp
// 功能：编辑器工具，自动生成 UI 引用代码
// 使用场景：批量生成 UI 绑定代码
```

---

## 完整示例：物品列表界面

```csharp
public class ItemInventoryView : UIBaseView, IOnCreate, IOnEnable
{
    private CopyGameObject itemList;
    private List<ItemData> items;
    private ReferenceCollector refCollector;
    
    public void OnCreate()
    {
        // 获取引用收集器
        refCollector = GetGameObject().GetComponent<ReferenceCollector>();
        
        // 获取列表组件
        itemList = refCollector.Get<CopyGameObject>("itemList");
        
        // 初始化列表
        itemList.InitListView(0, OnGetItem);
    }
    
    public void OnEnable()
    {
        // 加载物品数据
        items = InventoryManager.Instance.GetAllItems();
        
        // 更新列表
        itemList.SetListItemCount(items.Count);
        itemList.RefreshAllShownItem();
    }
    
    void OnGetItem(int index, GameObject itemObj)
    {
        // 配置每个物品项
        ItemView itemView = itemObj.GetComponent<ItemView>();
        itemView.SetData(items[index]);
        
        // 绑定点击事件
        PointerClick click = itemObj.GetComponent<PointerClick>();
        click.onClick.RemoveAllListeners();
        click.onClick.AddListener(() => OnItemClick(index));
    }
    
    void OnItemClick(int index)
    {
        Log.Info($"Clicked item {index}: {items[index].Name}");
        // 打开物品详情
        UIManager.Instance.OpenWindow<ItemDetailView>("path/to/ItemDetailView");
    }
}
```

---

## ⚠️ 注意事项

| 问题 | 说明 | 解决方案 |
|------|------|----------|
| **ReferenceCollector 序列化** | 运行时添加的引用不会保存 | 仅用于运行时管理，保存需额外处理 |
| **CopyGameObject 性能** | 超过 10 个对象性能差 | 大量列表用 UILoopGridView |
| **Drag 事件冲突** | 多个 Drag 组件可能冲突 | 确保 EventSystem 配置正确 |
| **CircleImage 分段** | segments 太低会不圆滑 | 设置为 36 或更高 |
| **BgAutoFit 循环依赖** | 子物体也监听父物体会死循环 | 避免嵌套 AutoFit 组件 |

---

## 相关文档

- [UIBaseContainer.cs.md](../../Code/Module/UI/UIBaseContainer.cs.md) - UI 容器基类
- [UIManager.cs.md](../../Code/Module/UI/UIManager.cs.md) - UI 管理器

---

*文档由 OpenClaw AI 助手自动生成 | 基于静态代码分析*
