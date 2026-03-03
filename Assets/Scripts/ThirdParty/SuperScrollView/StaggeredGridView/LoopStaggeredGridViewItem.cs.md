# LoopStaggeredGridViewItem.cs - 交错网格项基类

> **文件路径**: `Assets/Scripts/ThirdParty/SuperScrollView/StaggeredGridView/LoopStaggeredGridViewItem.cs`  
> **命名空间**: `SuperScrollView`  
> **文档生成时间**: 2026-03-03  
> **文件类型**: 第三方库 (SuperScrollView)

---

## 📑 文件信息表

| 属性 | 值 |
|------|-----|
| **文件路径** | `Assets/Scripts/ThirdParty/SuperScrollView/StaggeredGridView/LoopStaggeredGridViewItem.cs` |
| **命名空间** | `SuperScrollView` |
| **类/结构体** | `LoopStaggeredGridViewItem` |
| **依赖** | `UnityEngine` |
| **基类** | `MonoBehaviour` |
| **可见性** | `public` |

---

## 🎯 类说明

### LoopStaggeredGridViewItem

交错网格项基类，提供索引、组索引等元数据。

**核心职责**:
- 存储 Item 索引
- 存储组索引（列/行索引）
- 提供用户数据字段
- 缓存 RectTransform 引用

---

## 📊 字段表

| 字段名 | 类型 | 可见性 | 说明 |
|--------|------|--------|------|
| `mItemIndex` | `int` | `private` | Item 索引 |
| `mGroupIndex` | `int` | `private` | 组索引（列/行） |
| `mIndexInGroup` | `int` | `private` | 组内索引 |
| `mItemId` | `int` | `private` | Item 唯一 ID |
| `mParentListView` | `LoopStaggeredGridView` | `private` | 父视图引用 |
| `mCachedRectTransform` | `RectTransform` | `private` | 缓存的 RectTransform |

---

## 🔧 API 说明

### ItemIndex / GroupIndex / IndexInGroup

```csharp
public int ItemIndex { get; set; }
public int GroupIndex { get; set; }
public int IndexInGroup { get; set; }
```

**说明**: 获取或设置 Item 索引、组索引、组内索引。

---

### ParentListView

```csharp
public LoopStaggeredGridView ParentListView { get; set; }
```

**说明**: 获取或设置父视图引用。

---

### CachedRectTransform

```csharp
public RectTransform CachedRectTransform { get; }
```

**说明**: 缓存的 RectTransform 组件。

---

## 📚 相关文档链接

| 文档 | 说明 |
|------|------|
| [LoopStaggeredGridView.cs.md](./LoopStaggeredGridView.cs.md) | 交错网格视图核心 |

---

*文档由 OpenClaw AI 助手自动生成 | SuperScrollView 版本 2.4.0*
