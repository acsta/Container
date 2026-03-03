# StaggeredGridItemGroup.cs - 交错网格项组

> **文件路径**: `Assets/Scripts/ThirdParty/SuperScrollView/StaggeredGridView/StaggeredGridItemGroup.cs`  
> **命名空间**: `SuperScrollView`  
> **文档生成时间**: 2026-03-03  
> **文件类型**: 第三方库 (SuperScrollView)

---

## 📑 文件信息表

| 属性 | 值 |
|------|-----|
| **文件路径** | `Assets/Scripts/ThirdParty/SuperScrollView/StaggeredGridView/StaggeredGridItemGroup.cs` |
| **命名空间** | `SuperScrollView` |
| **类/结构体** | `StaggeredGridItemGroup` |
| **依赖** | `UnityEngine`, `System.Collections.Generic` |
| **可见性** | `public` |

---

## 🎯 类说明

### StaggeredGridItemGroup

交错网格项组，管理一列或一行的 Item。

**核心职责**:
- 管理组内 Item 列表
- 计算组的总尺寸
- 提供 Item 添加和回收接口

---

## 🔧 API 说明

### Init

```csharp
public void Init(
    LoopStaggeredGridView parent,
    int itemCount,
    int groupIndex,
    Func<int, LoopStaggeredGridViewItem> onGetItem)
```

**说明**: 初始化组。

---

### SetListItemCount

```csharp
public void SetListItemCount(int itemCount)
```

**说明**: 设置 Item 数量。

---

## 📚 相关文档链接

| 文档 | 说明 |
|------|------|
| [LoopStaggeredGridView.cs.md](./LoopStaggeredGridView.cs.md) | 交错网格视图核心 |

---

*文档由 OpenClaw AI 助手自动生成 | SuperScrollView 版本 2.4.0*
