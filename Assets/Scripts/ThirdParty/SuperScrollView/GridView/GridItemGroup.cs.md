# GridItemGroup.cs - 网格项组

> **文件路径**: `Assets/Scripts/ThirdParty/SuperScrollView/GridView/GridItemGroup.cs`  
> **命名空间**: `SuperScrollView`  
> **文档生成时间**: 2026-03-03  
> **文件类型**: 第三方库 (SuperScrollView)

---

## 📑 文件信息表

| 属性 | 值 |
|------|-----|
| **文件路径** | `Assets/Scripts/ThirdParty/SuperScrollView/GridView/GridItemGroup.cs` |
| **命名空间** | `SuperScrollView` |
| **类/结构体** | `GridItemGroup` |
| **依赖** | `UnityEngine`, `System.Collections.Generic` |
| **可见性** | `public` |

---

## 🎯 类说明

### GridItemGroup

网格项组，管理一行或一列的 Item。

**核心职责**:
- 管理组内 Item 列表
- 提供按行列获取 Item 的接口
- 支持 Item 替换
- 维护 Item 链表（Prev/Next）

**设计特点**:
- 根据 `GridFixedType` 决定管理行还是列
- `ColumnCountFixed`: 每组是一行
- `RowCountFixed`: 每组是一列

---

## 🔧 API 说明

### Item 管理

#### GetItemByColumn

```csharp
public LoopGridViewItem GetItemByColumn(int column)
```

**说明**: 根据列索引获取 Item（当组表示行时）。

---

#### GetItemByRow

```csharp
public LoopGridViewItem GetItemByRow(int row)
```

**说明**: 根据行索引获取 Item（当组表示列时）。

---

#### ReplaceItem

```csharp
public void ReplaceItem(LoopGridViewItem oldItem, LoopGridViewItem newItem)
```

**说明**: 替换 Item。

---

#### AddItem

```csharp
public void AddItem(LoopGridViewItem item)
```

**说明**: 添加 Item 到组。

---

#### ClearItems

```csharp
public void ClearItems()
```

**说明**: 清空组内所有 Item。

---

## 📚 相关文档链接

| 文档 | 说明 |
|------|------|
| [LoopGridView.cs.md](./LoopGridView.cs.md) | 网格视图核心 |

---

*文档由 OpenClaw AI 助手自动生成 | SuperScrollView 版本 2.4.0*
