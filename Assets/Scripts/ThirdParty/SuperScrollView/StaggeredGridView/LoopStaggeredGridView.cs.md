# LoopStaggeredGridView.cs - 循环交错网格视图

> **文件路径**: `Assets/Scripts/ThirdParty/SuperScrollView/StaggeredGridView/LoopStaggeredGridView.cs`  
> **命名空间**: `SuperScrollView`  
> **文档生成时间**: 2026-03-03  
> **文件类型**: 第三方库 (SuperScrollView)

---

## 📑 文件信息表

| 属性 | 值 |
|------|-----|
| **文件路径** | `Assets/Scripts/ThirdParty/SuperScrollView/StaggeredGridView/LoopStaggeredGridView.cs` |
| **命名空间** | `SuperScrollView` |
| **类/结构体** | `LoopStaggeredGridView`, `StaggeredGridItemPrefabConfData`, `StaggeredGridViewInitParam`, `GridViewLayoutParam`, `ItemIndexData` |
| **依赖** | `UnityEngine`, `UnityEngine.UI`, `UnityEngine.EventSystems` |
| **基类/接口** | `MonoBehaviour`, `IBeginDragHandler`, `IEndDragHandler`, `IDragHandler` |
| **可见性** | `public` |

---

## 🎯 类说明

### LoopStaggeredGridView

循环交错网格视图，支持多列/行不等高 Item 的网格布局。

**核心职责**:
- 管理交错网格（瀑布流）布局
- 支持多列/行独立滚动
- 实现对象池机制
- 自动计算每列/行的高度

**设计特点**:
- 每组（列/行）独立管理 Item
- 支持自定义列/行偏移
- 动态计算每列高度
- 适合图片墙、卡片墙等场景

---

### GridViewLayoutParam

网格布局参数类。

| 字段 | 类型 | 说明 |
|------|------|------|
| `mColumnOrRowCount` | `int` | 列数或行数 |
| `mItemWidthOrHeight` | `float` | Item 宽度或高度 |
| `mPadding1` | `float` | 边距 1 |
| `mPadding2` | `float` | 边距 2 |
| `mCustomColumnOrRowOffsetArray` | `float[]` | 自定义列/行偏移数组 |

---

## 🔧 核心 API 说明

### 初始化

#### InitListView

```csharp
public void InitListView(
    int itemTotalCount,
    GridViewLayoutParam layoutParam,
    Func<LoopStaggeredGridView, int, LoopStaggeredGridViewItem> onGetItemByItemIndex,
    StaggeredGridViewInitParam initParam = null)
```

**说明**: 初始化交错网格视图。

---

#### ResetGridViewLayoutParam

```csharp
public void ResetGridViewLayoutParam(int itemTotalCount, GridViewLayoutParam layoutParam)
```

**说明**: 重置布局参数。

---

### Item 管理

#### NewListViewItem

```csharp
public LoopStaggeredGridViewItem NewListViewItem(string itemPrefabName)
```

**说明**: 从对象池获取或创建新 Item。

---

#### SetListItemCount

```csharp
public void SetListItemCount(int itemCount, bool resetPos = true)
```

**说明**: 设置 Item 总数。

---

#### MovePanelToItemIndex

```csharp
public void MovePanelToItemIndex(int itemIndex, float offset)
```

**说明**: 滚动到指定 Item 位置。

---

## 💡 使用示例

### 瀑布流布局

```csharp
void Start()
{
    var layoutParam = new GridViewLayoutParam
    {
        mColumnOrRowCount = 2,           // 2 列
        mItemWidthOrHeight = 200,        // Item 宽度
        mPadding1 = 20,                  // 左边距
        mPadding2 = 20                   // 右边距
    };
    
    staggeredGridView.InitListView(100, layoutParam, OnGetItemByIndex);
}

LoopStaggeredGridViewItem OnGetItemByIndex(LoopStaggeredGridView view, int index)
{
    var item = view.NewListViewItem("ItemPrefab");
    // 设置不同高度的内容
    var image = item.GetComponent<Image>();
    image.SetNativeSize(); // 根据图片原始比例设置高度
    return item;
}
```

---

## 📚 相关文档链接

| 文档 | 说明 |
|------|------|
| [LoopStaggeredGridViewItem.cs.md](./LoopStaggeredGridViewItem.cs.md) | Item 基类 |
| [StaggeredGridItemPool.cs.md](./StaggeredGridItemPool.cs.md) | Item 对象池 |
| [StaggeredGridItemGroup.cs.md](./StaggeredGridItemGroup.cs.md) | Item 组 |

---

*文档由 OpenClaw AI 助手自动生成 | SuperScrollView 版本 2.4.0*
