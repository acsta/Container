# LoopGridViewItem.cs - 网格项基类

> **文件路径**: `Assets/Scripts/ThirdParty/SuperScrollView/GridView/LoopGridViewItem.cs`  
> **命名空间**: `SuperScrollView`  
> **文档生成时间**: 2026-03-03  
> **文件类型**: 第三方库 (SuperScrollView)

---

## 📑 文件信息表

| 属性 | 值 |
|------|-----|
| **文件路径** | `Assets/Scripts/ThirdParty/SuperScrollView/GridView/LoopGridViewItem.cs` |
| **命名空间** | `SuperScrollView` |
| **类/结构体** | `LoopGridViewItem` |
| **依赖** | `UnityEngine` |
| **基类** | `MonoBehaviour` |
| **可见性** | `public` |

---

## 🎯 类说明

### LoopGridViewItem

网格项基类，附加在每个网格 Item 上，提供行列索引等元数据。

**核心职责**:
- 存储 Item 在网格中的行列索引（Row, Column）
- 存储 Item 在列表中的索引（ItemIndex）
- 存储 Item 的唯一 ID（ItemId）
- 提供用户数据字段
- 支持双向链表（PrevItem, NextItem）

**设计特点**:
- 轻量级数据容器
- 支持多种用户数据字段
- 自动缓存 RectTransform 引用
- 支持前后 Item 引用（用于链表操作）

---

## 📊 字段表

### 核心字段

| 字段名 | 类型 | 可见性 | 说明 |
|--------|------|--------|------|
| `mItemIndex` | `int` | `private` | Item 在列表中的索引 |
| `mRow` | `int` | `private` | 行索引（从 0 开始） |
| `mColumn` | `int` | `private` | 列索引（从 0 开始） |
| `mItemId` | `int` | `private` | Item 唯一 ID |
| `mParentGridView` | `LoopGridView` | `private` | 父网格视图引用 |
| `mIsInitHandlerCalled` | `bool` | `private` | 初始化处理器是否已调用 |
| `mItemPrefabName` | `string` | `private` | 预制体名称 |
| `mCachedRectTransform` | `RectTransform` | `private` | 缓存的 RectTransform |
| `mItemCreatedCheckFrameCount` | `int` | `private` | Item 创建时的帧计数 |

### 链表字段

| 字段名 | 类型 | 可见性 | 说明 |
|--------|------|--------|------|
| `mPrevItem` | `LoopGridViewItem` | `private` | 前一个 Item |
| `mNextItem` | `LoopGridViewItem` | `private` | 后一个 Item |

### 用户数据字段

| 字段名 | 类型 | 说明 |
|--------|------|------|
| `mUserObjectData` | `object` | 用户对象数据 |
| `mUserIntData1` | `int` | 用户整数数据 1 |
| `mUserIntData2` | `int` | 用户整数数据 2 |
| `mUserStringData1` | `string` | 用户字符串数据 1 |
| `mUserStringData2` | `string` | 用户字符串数据 2 |

---

## 🔧 API 说明

### 索引与 ID

#### Row / Column

```csharp
public int Row { get; set; }
public int Column { get; set; }
```

**说明**: 获取或设置 Item 的行列索引。

---

#### ItemIndex

```csharp
public int ItemIndex { get; set; }
```

**说明**: Item 在列表中的索引（从 0 到 itemTotalCount-1）。

---

#### ItemId

```csharp
public int ItemId { get; set; }
```

**说明**: Item 唯一 ID，从对象池获取时递增。

---

### 父视图引用

#### ParentGridView

```csharp
public LoopGridView ParentGridView { get; set; }
```

**说明**: 获取或设置父网格视图引用。

---

### RectTransform

#### CachedRectTransform

```csharp
public RectTransform CachedRectTransform { get; }
```

**说明**: 缓存的 RectTransform 组件（自动缓存）。

---

### 链表引用

#### PrevItem / NextItem

```csharp
public LoopGridViewItem PrevItem { get; set; }
public LoopGridViewItem NextItem { get; set; }
```

**说明**: 获取或设置前后 Item 引用（用于链表操作）。

---

### 用户数据

#### UserObjectData / UserIntData1 / UserIntData2

```csharp
public object UserObjectData { get; set; }
public int UserIntData1 { get; set; }
public int UserIntData2 { get; set; }
```

**说明**: 用户自定义数据字段。

---

#### UserStringData1 / UserStringData2

```csharp
public string UserStringData1 { get; set; }
public string UserStringData2 { get; set; }
```

**说明**: 用户自定义字符串数据字段。

---

## 💡 使用示例

### 设置 Item 内容

```csharp
LoopGridViewItem OnGetItemByRowColumn(LoopGridView view, int row, int column, int index)
{
    var item = view.NewListViewItem("ItemPrefab");
    
    // 设置行列索引（自动设置）
    // item.Row = row;
    // item.Column = column;
    // item.ItemIndex = index;
    
    // 设置用户数据
    item.UserIntData1 = index;
    item.UserStringData1 = GetData(index);
    
    // 设置显示内容
    var text = item.GetComponentInChildren<Text>();
    text.text = $"Item {index}";
    
    return item;
}
```

---

### 使用用户数据字段

```csharp
// 存储数据
item.UserObjectData = new ItemData { id = 123, name = "Test" };
item.UserIntData1 = 100; // 分数
item.UserIntData2 = 5;   // 等级
item.UserStringData1 = "PlayerName";
item.UserStringData2 = "GuildName";

// 获取数据
var data = (ItemData)item.UserObjectData;
int score = item.UserIntData1;
int level = item.UserIntData2;
string playerName = item.UserStringData1;
```

---

### 链表操作

```csharp
// 遍历可见 Item（通过链表）
var firstItem = gridView.GetShownItemByIndex(0);
var current = firstItem;

while (current != null)
{
    Debug.Log($"Item: {current.ItemIndex}, Row: {current.Row}, Column: {current.Column}");
    current = current.NextItem;
}

// 反向遍历
var lastItem = gridView.GetShownItemByIndex(gridView.ShownItemCount - 1);
current = lastItem;

while (current != null)
{
    Debug.Log($"Item: {current.ItemIndex}");
    current = current.PrevItem;
}
```

---

### 点击事件处理

```csharp
void Start()
{
    var button = item.GetComponent<Button>();
    button.onClick.AddListener(() => OnItemClick(item));
}

void OnItemClick(LoopGridViewItem item)
{
    Debug.Log($"点击了 Item {item.ItemIndex}");
    Debug.Log($"位置：行{item.Row}, 列{item.Column}");
    Debug.Log($"数据：{item.UserStringData1}");
}
```

---

## 📚 相关文档链接

| 文档 | 说明 |
|------|------|
| [LoopGridView.cs.md](./LoopGridView.cs.md) | 网格视图核心 |
| [LoopGridItemPool.cs.md](./LoopGridItemPool.cs.md) | Item 对象池 |

---

## ⚠️ 注意事项

1. **Row/Column**: 由网格视图自动设置，不要手动修改
2. **ItemId 唯一性**: ItemId 在 Item 生命周期内不变
3. **缓存引用**: CachedRectTransform 自动缓存，无需重复获取
4. **用户数据清理**: Item 回收到池前，建议清理用户数据
5. **链表引用**: PrevItem/NextItem 由网格视图管理，不要手动修改

---

*文档由 OpenClaw AI 助手自动生成 | SuperScrollView 版本 2.4.0*
