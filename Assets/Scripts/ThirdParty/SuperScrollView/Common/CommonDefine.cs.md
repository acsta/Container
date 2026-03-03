# CommonDefine.cs - 公共枚举和常量定义

> **文件路径**: `Assets/Scripts/ThirdParty/SuperScrollView/Common/CommonDefine.cs`  
> **命名空间**: `SuperScrollView`  
> **文档生成时间**: 2026-03-03  
> **文件类型**: 第三方库 (SuperScrollView)

---

## 📑 文件信息表

| 属性 | 值 |
|------|-----|
| **文件路径** | `Assets/Scripts/ThirdParty/SuperScrollView/Common/CommonDefine.cs` |
| **命名空间** | `SuperScrollView` |
| **类/结构体** | `SnapStatus`, `ItemCornerEnum`, `ListItemArrangeType`, `GridItemArrangeType`, `GridFixedType`, `RowColumnPair` |
| **依赖** | `System`, `System.Collections.Generic` |
| **可见性** | `public` |

---

## 🎯 枚举说明

### SnapStatus

吸附状态枚举。

| 值 | 名称 | 数值 | 说明 |
|----|------|------|------|
| `NoTargetSet` | 未设置目标 | `0` | 未设置吸附目标 |
| `TargetHasSet` | 已设置目标 | `1` | 已设置吸附目标 |
| `SnapMoving` | 吸附移动中 | `2` | 正在向目标移动 |
| `SnapMoveFinish` | 吸附完成 | `3` | 已到达目标位置 |

**使用场景**:
```csharp
// 检查吸附状态
if (snapStatus == SnapStatus.SnapMoveFinish)
{
    // 吸附完成，执行后续逻辑
}
```

---

### ItemCornerEnum

Item 角落枚举（用于获取 Item 角点位置）。

| 值 | 名称 | 数值 | 说明 |
|----|------|------|------|
| `LeftBottom` | 左下角 | `0` | 左下角点 |
| `LeftTop` | 左上角 | `1` | 左上角点 |
| `RightTop` | 右上角 | `2` | 右上角点 |
| `RightBottom` | 右下角 | `3` | 右下角点 |

**使用场景**:
```csharp
// 获取 Item 左下角在视口中的位置
var cornerPos = listView.GetItemCornerPosInViewPort(item, ItemCornerEnum.LeftBottom);
```

---

### ListItemArrangeType

列表项排列类型枚举。

| 值 | 名称 | 数值 | 说明 |
|----|------|------|------|
| `TopToBottom` | 从上到下 | `0` | 垂直列表，从上向下排列 |
| `BottomToTop` | 从下到上 | `1` | 垂直列表，从下向上排列 |
| `LeftToRight` | 从左到右 | `2` | 水平列表，从左向右排列 |
| `RightToLeft` | 从右到左 | `3` | 水平列表，从右向左排列 |

**使用场景**:
```csharp
// 设置列表排列方向
listView.ArrangeType = ListItemArrangeType.TopToBottom;

// 根据排列类型判断滚动方向
if (listView.ArrangeType == ListItemArrangeType.TopToBottom ||
    listView.ArrangeType == ListItemArrangeType.BottomToTop)
{
    // 垂直列表
}
else
{
    // 水平列表
}
```

---

### GridItemArrangeType

网格项排列类型枚举。

| 值 | 名称 | 数值 | 说明 |
|----|------|------|------|
| `TopLeftToBottomRight` | 左上到右下 | `0` | 从左上角开始，向右下排列 |
| `BottomLeftToTopRight` | 左下到右上 | `1` | 从左下角开始，向右上排列 |
| `TopRightToBottomLeft` | 右上到左下 | `2` | 从右上角开始，向左下排列 |
| `BottomRightToTopLeft` | 右下到左上 | `3` | 从右下角开始，向左上排列 |

**使用场景**:
```csharp
// 设置网格排列方向
gridView.ArrangeType = GridItemArrangeType.TopLeftToBottomRight;
```

---

### GridFixedType

网格固定类型枚举。

| 值 | 名称 | 数值 | 说明 |
|----|------|------|------|
| `ColumnCountFixed` | 固定列数 | `0` | 列数固定，行数动态 |
| `RowCountFixed` | 固定行数 | `1` | 行数固定，列数动态 |

**使用场景**:
```csharp
// 设置网格固定类型
gridView.GridFixedType = GridFixedType.ColumnCountFixed;
gridView.FixedRowOrColumnCount = 3; // 固定 3 列
```

---

## 📊 结构体说明

### RowColumnPair

行列对结构体，用于表示网格中的位置。

**字段**:
| 字段名 | 类型 | 说明 |
|--------|------|------|
| `mRow` | `int` | 行索引（从 0 开始） |
| `mColumn` | `int` | 列索引（从 0 开始） |

**构造函数**:
```csharp
public RowColumnPair(int row1, int column1)
```

**方法**:

#### Equals

```csharp
public bool Equals(RowColumnPair other)
```

**说明**: 判断两个行列对是否相等。

---

#### 运算符重载

```csharp
public static bool operator ==(RowColumnPair a, RowColumnPair b)
public static bool operator !=(RowColumnPair a, RowColumnPair b)
```

**说明**: 支持 `==` 和 `!=` 运算符比较。

---

#### GetHashCode / Equals

```csharp
public override int GetHashCode()
public override bool Equals(object obj)
```

**说明**: 重写哈希和相等比较方法。

**⚠️ 注意**: `GetHashCode()` 返回固定值 `0`，不适用于哈希表键。

---

## 💡 使用示例

### 排列类型使用

```csharp
// 垂直列表（常见）
listView.ArrangeType = ListItemArrangeType.TopToBottom;

// 水平列表
listView.ArrangeType = ListItemArrangeType.LeftToRight;

// 根据排列类型计算位置
float GetItemPosition(ListItemArrangeType type, int index, float itemSize)
{
    switch (type)
    {
        case ListItemArrangeType.TopToBottom:
            return -index * itemSize; // 向下为负
        case ListItemArrangeType.BottomToTop:
            return index * itemSize;  // 向上为正
        case ListItemArrangeType.LeftToRight:
            return index * itemSize;  // 向右为正
        case ListItemArrangeType.RightToLeft:
            return -index * itemSize; // 向左为负
    }
    return 0;
}
```

---

### 网格行列对使用

```csharp
// 创建行列对
var pos1 = new RowColumnPair(0, 0); // 第 0 行第 0 列
var pos2 = new RowColumnPair(1, 2); // 第 1 行第 2 列

// 比较
if (pos1 == pos2)
{
    // 相同位置
}

// 在网格视图中使用
gridView.RefreshItemByRowColumn(pos1.mRow, pos1.mColumn);

// 获取当前吸附的行列
RowColumnPair nearest = gridView.CurSnapNearestItemRowColumn;
Debug.Log($"最近的 Item: 行{nearest.mRow}, 列{nearest.mColumn}");
```

---

### 吸附状态检查

```csharp
void Update()
{
    // 检查吸附状态
    if (snapStatus == SnapStatus.TargetHasSet)
    {
        // 目标已设置，开始吸附
        StartSnapMove();
    }
    else if (snapStatus == SnapStatus.SnapMoving)
    {
        // 吸附移动中
        UpdateSnapMove();
    }
    else if (snapStatus == SnapStatus.SnapMoveFinish)
    {
        // 吸附完成
        OnSnapFinished();
        snapStatus = SnapStatus.NoTargetSet;
    }
}
```

---

### 角点位置获取

```csharp
// 获取 Item 在视口中的四个角点
Vector3 leftBottom = listView.GetItemCornerPosInViewPort(item, ItemCornerEnum.LeftBottom);
Vector3 leftTop = listView.GetItemCornerPosInViewPort(item, ItemCornerEnum.LeftTop);
Vector3 rightTop = listView.GetItemCornerPosInViewPort(item, ItemCornerEnum.RightTop);
Vector3 rightBottom = listView.GetItemCornerPosInViewPort(item, ItemCornerEnum.RightBottom);

// 检查 Item 是否完全在视口内
bool IsItemFullyVisible(LoopListViewItem2 item)
{
    var lb = listView.GetItemCornerPosInViewPort(item, ItemCornerEnum.LeftBottom);
    var rt = listView.GetItemCornerPosInViewPort(item, ItemCornerEnum.RightTop);
    
    return lb.x >= 0 && lb.y >= 0 && rt.x <= viewportWidth && rt.y <= viewportHeight;
}
```

---

## 📚 相关文档链接

| 文档 | 说明 |
|------|------|
| [LoopListView2.cs.md](../ListView/LoopListView2.cs.md) | 列表视图核心 |
| [LoopGridView.cs.md](../GridView/LoopGridView.cs.md) | 网格视图核心 |

---

## ⚠️ 注意事项

1. **RowColumnPair.GetHashCode**: 返回固定值 0，不适用于 Dictionary 键
2. **排列类型**: 设置后会影响滚动方向和位置计算
3. **角点枚举**: 与 Unity 的 RectTransform.GetWorldCorners 返回顺序一致
4. **网格固定类型**: 决定是按列固定还是按行固定

---

*文档由 OpenClaw AI 助手自动生成 | SuperScrollView 版本 2.4.0*
