# LoopGridView.cs - 循环网格视图核心

> **文件路径**: `Assets/Scripts/ThirdParty/SuperScrollView/GridView/LoopGridView.cs`  
> **命名空间**: `SuperScrollView`  
> **文档生成时间**: 2026-03-03  
> **文件类型**: 第三方库 (SuperScrollView)

---

## 📑 文件信息表

| 属性 | 值 |
|------|-----|
| **文件路径** | `Assets/Scripts/ThirdParty/SuperScrollView/GridView/LoopGridView.cs` |
| **命名空间** | `SuperScrollView` |
| **类/结构体** | `LoopGridView`, `GridViewItemPrefabConfData`, `LoopGridViewInitParam`, `LoopGridViewSettingParam`, `SnapData`, `ItemRangeData` |
| **依赖** | `UnityEngine`, `UnityEngine.UI`, `UnityEngine.EventSystems` |
| **基类/接口** | `MonoBehaviour`, `IBeginDragHandler`, `IEndDragHandler`, `IDragHandler` |
| **可见性** | `public` |

---

## 🎯 类说明

### LoopGridView

高性能循环网格视图组件，支持二维网格布局和对象池复用。

**核心职责**:
- 管理可滚动网格中的项目显示与回收
- 支持固定列数或固定行数布局
- 提供 4 种排列方向
- 实现对象池机制
- 支持 Item 吸附功能

**设计特点**:
- 按行或列分组管理 Item
- 仅渲染可见区域的项目
- 支持动态 Item 尺寸
- 支持多种 Item 预制体类型

---

### GridViewItemPrefabConfData

网格 Item 预制体配置数据类。

| 字段 | 类型 | 说明 |
|------|------|------|
| `mItemPrefab` | `GameObject` | Item 预制体引用 |
| `mInitCreateCount` | `int` | 初始化创建数量 |

---

### LoopGridViewInitParam

初始化参数配置类。

| 字段 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `mSmoothDumpRate` | `float` | 0.3f | 平滑减速率 |
| `mSnapFinishThreshold` | `float` | 0.01f | 吸附完成阈值 |
| `mSnapVecThreshold` | `float` | 145 | 吸附速度阈值 |

---

### LoopGridViewSettingParam

设置参数类（用于运行时覆盖 Inspector 设置）。

| 字段 | 类型 | 说明 |
|------|------|------|
| `mItemSize` | `object` | Item 尺寸 |
| `mPadding` | `object` | 边距 |
| `mItemPadding` | `object` | Item 间距 |
| `mGridFixedType` | `object` | 网格固定类型 |
| `mFixedRowOrColumnCount` | `object` | 固定行/列数 |

---

## 📊 核心字段表

| 字段名 | 类型 | 可见性 | 说明 |
|--------|------|--------|------|
| `mItemPoolDict` | `Dictionary<string, GridItemPool>` | `private` | Item 对象池字典 |
| `mItemPrefabDataList` | `List<GridViewItemPrefabConfData>` | `private` | 预制体配置列表 |
| `mArrangeType` | `GridItemArrangeType` | `private` | 排列类型 |
| `mItemGroupList` | `List<GridItemGroup>` | `private` | 当前显示的 Item 组列表 |
| `mContainerTrans` | `RectTransform` | `private` | 内容容器 |
| `mScrollRect` | `ScrollRect` | `private` | 滚动矩形组件 |
| `mItemTotalCount` | `int` | `private` | Item 总数 |
| `mFixedRowOrColumnCount` | `int` | `private` | 固定行/列数 |
| `mPadding` | `RectOffset` | `private` | 边距 |
| `mItemPadding` | `Vector2` | `private` | Item 间距 |
| `mItemSize` | `Vector2` | `private` | Item 尺寸 |
| `mGridFixedType` | `GridFixedType` | `private` | 网格固定类型 |
| `mRowCount` | `int` | `private` | 总行数 |
| `mColumnCount` | `int` | `private` | 总列数 |
| `mItemSnapEnable` | `bool` | `private` | 是否启用吸附 |
| `mCurSnapNearestItemRowColumn` | `RowColumnPair` | `private` | 当前最近的 Item 行列 |

---

## 🔧 核心 API 说明

### 初始化

#### InitGridView

```csharp
public void InitGridView(
    int itemTotalCount,
    Func<LoopGridView, int, int, int, LoopGridViewItem> onGetItemByRowColumn,
    LoopGridViewSettingParam settingParam = null,
    LoopGridViewInitParam initParam = null)
```

**说明**: 初始化网格视图。

**参数**:
| 参数 | 类型 | 说明 |
|------|------|------|
| `itemTotalCount` | `int` | Item 总数 |
| `onGetItemByRowColumn` | `Func` | 根据行列获取 Item 的回调 |
| `settingParam` | `LoopGridViewSettingParam` | 可选的设置参数 |
| `initParam` | `LoopGridViewInitParam` | 可选的初始化参数 |

**示例**:
```csharp
gridView.InitGridView(100, (view, row, column, index) =>
{
    var item = view.NewListViewItem("ItemPrefab");
    item.SetContent($"Item {index}");
    return item;
});
```

---

#### SetListItemCount

```csharp
public void SetListItemCount(int itemCount, bool resetPos = true)
```

**说明**: 运行时设置 Item 总数。

---

### Item 管理

#### NewListViewItem

```csharp
public LoopGridViewItem NewListViewItem(string itemPrefabName, int? index = null)
```

**说明**: 从对象池获取或创建新 Item。

---

#### RefreshItemByRowColumn

```csharp
public void RefreshItemByRowColumn(int row, int column)
```

**说明**: 刷新指定行列的 Item。

---

#### RefreshItemByItemIndex

```csharp
public void RefreshItemByItemIndex(int itemIndex)
```

**说明**: 刷新指定索引的 Item。

---

### 位置控制

#### MovePanelToItemByRowColumn

```csharp
public void MovePanelToItemByRowColumn(int row, int column)
```

**说明**: 滚动到指定行列位置。

---

#### ClearSnapData

```csharp
public void ClearSnapData()
```

**说明**: 清除当前吸附数据。

---

#### SetSnapTargetItemRowColumn

```csharp
public void SetSnapTargetItemRowColumn(int row, int column)
```

**说明**: 设置吸附目标行列。

---

### 属性

#### ItemSize

```csharp
public Vector2 ItemSize { get; set; }
```

**说明**: 获取或设置 Item 尺寸。

---

#### ItemPadding

```csharp
public Vector2 ItemPadding { get; set; }
```

**说明**: 获取或设置 Item 间距。

---

#### Padding

```csharp
public RectOffset Padding { get; set; }
```

**说明**: 获取或设置边距。

---

#### CurSnapNearestItemRowColumn

```csharp
public RowColumnPair CurSnapNearestItemRowColumn { get; }
```

**说明**: 获取当前最近的吸附 Item 行列。

---

### 清理

#### ClearListView

```csharp
public void ClearListView()
```

**说明**: 清理网格视图，回收所有 Item。

---

#### CleanUp

```csharp
public void CleanUp(string name = null, Action<GameObject> beforeDestroy = null)
```

**说明**: 清理对象池。

---

## 🔄 核心流程图

### 网格布局计算

```mermaid
flowchart TD
    Start[InitGridView] --> CheckType{GridFixedType?}
    CheckType -->|ColumnCountFixed| CalcColumn[固定列数]
    CheckType -->|RowCountFixed| CalcRow[固定行数]
    
    CalcColumn --> RowCount[行数 = ceil(总数/列数)]
    CalcRow --> ColumnCount[列数 = ceil(总数/行数)]
    
    RowCount --> Layout[计算每个 Item 位置]
    ColumnCount --> Layout
    
    Layout --> Visible{检查可见区域}
    Visible -->|可见 | Show[显示 Item]
    Visible -->|不可见 | Recycle[回收到池]
```

---

## 💡 使用示例

### 基础网格列表

```csharp
public class MyGridView : MonoBehaviour
{
    public LoopGridView gridView;
    
    void Start()
    {
        gridView.InitGridView(100, OnGetItemByRowColumn);
    }
    
    LoopGridViewItem OnGetItemByRowColumn(LoopGridView view, int row, int column, int index)
    {
        var item = view.NewListViewItem("ItemPrefab");
        var text = item.GetComponentInChildren<Text>();
        text.text = $"Item {index}";
        return item;
    }
}
```

---

### 固定列数布局

```csharp
void Start()
{
    gridView.GridFixedType = GridFixedType.ColumnCountFixed;
    gridView.FixedRowOrColumnCount = 4; // 固定 4 列
    gridView.ItemSize = new Vector2(200, 200); // Item 尺寸
    gridView.ItemPadding = new Vector2(10, 10); // Item 间距
    gridView.Padding = new RectOffset(20, 20, 20, 20); // 边距
    
    gridView.InitGridView(100, OnGetItemByRowColumn);
}
```

---

### 固定行数布局

```csharp
void Start()
{
    gridView.GridFixedType = GridFixedType.RowCountFixed;
    gridView.FixedRowOrColumnCount = 3; // 固定 3 行
    // 列数动态计算
    gridView.InitGridView(100, OnGetItemByRowColumn);
}
```

---

### 多种 Item 类型

```csharp
LoopGridViewItem OnGetItemByRowColumn(LoopGridView view, int row, int column, int index)
{
    string prefabName = GetPrefabNameByIndex(index);
    var item = view.NewListViewItem(prefabName);
    
    if (prefabName == "RareItem")
    {
        // 设置稀有 Item 内容
    }
    else
    {
        // 设置普通 Item 内容
    }
    
    return item;
}
```

---

### 吸附功能

```csharp
void Start()
{
    gridView.ItemSnapEnable = true;
    gridView.mOnSnapItemFinished += OnSnapFinished;
    gridView.mOnSnapNearestChanged += OnSnapNearestChanged;
}

void OnSnapFinished(LoopGridView view, LoopGridViewItem item)
{
    Debug.Log($"吸附完成：Item {item.ItemIndex}");
}

void OnSnapNearestChanged(LoopGridView view)
{
    var pos = view.CurSnapNearestItemRowColumn;
    Debug.Log($"最近 Item: 行{pos.mRow}, 列{pos.mColumn}");
}
```

---

### 运行时更新 Item

```csharp
// 刷新特定行列的 Item
gridView.RefreshItemByRowColumn(2, 3);

// 刷新特定索引的 Item
gridView.RefreshItemByItemIndex(10);

// 滚动到特定位置
gridView.MovePanelToItemByRowColumn(5, 2);
```

---

## 📚 相关文档链接

| 文档 | 说明 |
|------|------|
| [LoopGridViewItem.cs.md](./LoopGridViewItem.cs.md) | 网格 Item 基类 |
| [LoopGridItemPool.cs.md](./LoopGridItemPool.cs.md) | Item 对象池 |
| [CommonDefine.cs.md](../Common/CommonDefine.cs.md) | 枚举和常量定义 |

---

## ⚠️ 注意事项

1. **只能调用一次 InitGridView**: 多次调用会触发错误日志
2. **itemTotalCount 必须 >= 0**: 不支持无限网格
3. **固定类型选择**: 根据布局需求选择固定列数或固定行数
4. **Item 尺寸**: 必须在初始化前设置或使用 settingParam
5. **吸附功能**: 需要设置 `ItemSnapEnable = true` 才生效

---

*文档由 OpenClaw AI 助手自动生成 | SuperScrollView 版本 2.4.0*
