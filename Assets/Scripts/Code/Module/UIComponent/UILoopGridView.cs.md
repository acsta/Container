# UILoopGridView.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | UILoopGridView.cs |
| **路径** | Assets/Scripts/Code/Module/UIComponent/UILoopGridView.cs |
| **所属模块** | 框架层 → Code/Module/UIComponent |
| **文件职责** | 循环网格组件，封装 LoopGridView（SuperScrollView） |

---

## 类/结构体说明

### UILoopGridView

| 属性 | 说明 |
|------|------|
| **职责** | 封装 SuperScrollView.LoopGridView，支持无限循环滚动网格 |
| **泛型参数** | 无 |
| **继承关系** | 继承自 `UIBaseContainer` |
| **实现的接口** | `IOnDestroy` |

```csharp
public class UILoopGridView : UIBaseContainer, IOnDestroy
{
    // 循环网格组件
}
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `loopGridView` | `LoopGridView` | `private` | SuperScrollView LoopGridView 组件 |

---

## 方法说明（按重要程度排序）

### OnDestroy()

**签名**:
```csharp
public void OnDestroy()
```

**职责**: 销毁时清理资源

**核心逻辑**:
```
1. 如果 loopGridView 不为 null
2. 调用 ClearListView() 清空网格
3. 设置 loopGridView = null
```

**调用者**: `UIBaseContainer.BeforeOnDestroy()`

---

### InitGridView(int itemTotalCount, onGetItemByRowColumn, settingParam, initParam)

**签名**:
```csharp
public void InitGridView(int itemTotalCount,
    System.Func<LoopGridView, int, int, int, LoopGridViewItem> onGetItemByRowColumn,
    LoopGridViewSettingParam settingParam = null,
    LoopGridViewInitParam initParam = null)
```

**职责**: 初始化网格

**参数**:
- `itemTotalCount`: 物品总数
- `onGetItemByRowColumn`: 获取物品的回调（根据行、列、索引创建/复用物品）
- `settingParam`: 设置参数（列数、间距等）
- `initParam`: 初始化参数

**核心逻辑**:
```
1. 激活 LoopGridView 组件
2. 调用 loopGridView.InitGridView()
```

**调用者**: 网格初始化代码

**使用示例**:
```csharp
// 设置参数（4 列，间距 10 像素）
var settingParam = new LoopGridViewSettingParam(4)
{
    xSpace = 10,
    ySpace = 10
};

gridView.InitGridView(itemList.Count, OnGetItemByRowColumn, settingParam);

private LoopGridViewItem OnGetItemByRowColumn(LoopGridView gridView, int row, int column, int index)
{
    // 获取或创建物品
    LoopGridViewItem item = gridView.GetNewListItem();
    
    // 获取 UI 组件
    UIItemView itemView = gridView.AddItemViewComponent<UIItemView>(item);
    
    // 设置数据
    itemView.SetData(itemList[index]);
    
    return item;
}
```

---

### AddItemViewComponent<T>(LoopGridViewItem item)

**签名**:
```csharp
public T AddItemViewComponent<T>(LoopGridViewItem item) where T : UIBaseContainer
```

**职责**: 为物品添加 UI 组件

**与 UILoopListView2 的区别**: 使用 LoopGridViewItem 而非 LoopListViewItem2

---

### GetUIItemView<T>(LoopGridViewItem item)

**签名**:
```csharp
public T GetUIItemView<T>(LoopGridViewItem item) where T : UIBaseContainer
```

**职责**: 根据 Unity 侧物品获取 UI 侧组件

---

### SetListItemCount(int itemCount, bool resetPos)

**签名**:
```csharp
public void SetListItemCount(int itemCount, bool resetPos = true)
```

**职责**: 重新设置物品数量

---

### GetShownItemByItemIndex(int itemIndex)

**签名**:
```csharp
public LoopGridViewItem GetShownItemByItemIndex(int itemIndex)
```

**职责**: 获取指定索引的显示物品

**返回**: LoopGridViewItem（如果未显示返回 null）

---

### MovePanelToItemByRowColumn(int row, int column, offsetX, offsetY)

**签名**:
```csharp
public void MovePanelToItemByRowColumn(int row, int column, int offsetX = 0, int offsetY = 0)
```

**职责**: 滚动到指定行列

**参数**:
- `row`: 行号
- `column`: 列号
- `offsetX`: X 轴偏移
- `offsetY`: Y 轴偏移

**核心逻辑**:
```
1. 激活 LoopGridView 组件
2. 调用 loopGridView.MovePanelToItemByRowColumn()
```

**调用者**: 需要滚动到特定位置的代码

---

### RefreshAllShownItem()

**签名**:
```csharp
public void RefreshAllShownItem()
```

**职责**: 刷新所有显示的物品

---

### SetItemSize(Vector2 sizeDelta)

**签名**:
```csharp
public void SetItemSize(Vector2 sizeDelta)
```

**职责**: 设置物品大小

**参数**:
- `sizeDelta`: 物品大小

**核心逻辑**:
```
1. 激活 LoopGridView 组件
2. 调用 loopGridView.SetItemSize()
```

**调用者**: 需要动态调整物品大小

---

### 拖拽事件

| 方法 | 说明 |
|------|------|
| `SetOnBeginDragAction(Action<PointerEventData> callback)` | 开始拖拽 |
| `SetOnDragingAction(Action<PointerEventData> callback)` | 拖拽中 |
| `SetOnEndDragAction(Action<PointerEventData> callback)` | 结束拖拽 |

---

### CleanUp(string name)

**签名**:
```csharp
public void CleanUp(string name)
```

**职责**: 清理指定名称的物品

---

## 阅读指引

### 建议的阅读顺序

1. **理解组件作用** - 为什么需要 UILoopGridView
2. **看 InitGridView** - 理解网格初始化
3. **看 AddItemViewComponent** - 理解 UI 组件创建
4. **了解滚动定位** - 理解 MovePanelToItemByRowColumn

### 最值得学习的技术点

1. **网格布局**: 支持多列网格
2. **无限循环**: LoopGridView 支持虚拟网格
3. **物品复用**: GetNewListItem 复用物品对象
4. **行列定位**: 支持按行列滚动

---

## 与 UILoopListView2 的区别

| 特性 | UILoopListView2 | UILoopGridView |
|------|-----------------|----------------|
| 布局 | 单列列表 | 多列网格 |
| 定位 | 按索引 | 按行列 |
| 回调参数 | index | row, column, index |
| 适用场景 | 列表、排行榜 | 背包、图鉴 |

---

## 使用示例

### 示例 1: 背包网格

```csharp
public class UIBagView : UIBaseView, IOnCreate
{
    private UILoopGridView gridView;
    private List<ItemData> bagItems;
    
    public void OnCreate()
    {
        gridView = AddComponent<UILoopGridView>("BagGrid");
        
        // 设置参数（5 列，间距 15 像素）
        var settingParam = new LoopGridViewSettingParam(5)
        {
            xSpace = 15,
            ySpace = 15
        };
        
        // 初始化网格
        gridView.InitGridView(bagItems.Count, OnGetItemByRowColumn, settingParam);
    }
    
    private LoopGridViewItem OnGetItemByRowColumn(LoopGridView gridView, int row, int column, int index)
    {
        // 获取或创建物品
        LoopGridViewItem item = gridView.GetNewListItem();
        
        // 获取 UI 组件
        UIBagItemView itemView = gridView.AddItemViewComponent<UIBagItemView>(item);
        
        // 设置数据
        itemView.SetData(bagItems[index]);
        
        return item;
    }
    
    public void RefreshGrid()
    {
        gridView.SetListItemCount(bagItems.Count);
        gridView.RefreshAllShownItem();
    }
}
```

### 示例 2: 滚动到指定位置

```csharp
// 滚动到第 2 行第 3 列
gridView.MovePanelToItemByRowColumn(2, 3);

// 滚动到第 2 行第 3 列，Y 轴偏移 100 像素
gridView.MovePanelToItemByRowColumn(2, 3, offsetY: 100);
```

### 示例 3: 调整物品大小

```csharp
// 设置物品大小为 100x100
gridView.SetItemSize(new Vector2(100, 100));
```

---

## 相关文档

- [UIBaseContainer.cs.md](./UIBaseContainer.cs.md) - UI 容器基类
- [UILoopListView2.cs.md](./UILoopListView2.cs.md) - 循环列表组件
- [UICopyGameObject.cs.md](./UICopyGameObject.cs.md) - GameObject 复制组件

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
