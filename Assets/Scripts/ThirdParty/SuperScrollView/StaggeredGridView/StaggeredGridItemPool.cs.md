# StaggeredGridItemPool.cs - 交错网格项对象池

> **文件路径**: `Assets/Scripts/ThirdParty/SuperScrollView/StaggeredGridView/StaggeredGridItemPool.cs`  
> **命名空间**: `SuperScrollView`  
> **文档生成时间**: 2026-03-03  
> **文件类型**: 第三方库 (SuperScrollView)

---

## 📑 文件信息表

| 属性 | 值 |
|------|-----|
| **文件路径** | `Assets/Scripts/ThirdParty/SuperScrollView/StaggeredGridView/StaggeredGridItemPool.cs` |
| **命名空间** | `SuperScrollView` |
| **类/结构体** | `StaggeredGridItemPool` |
| **依赖** | `UnityEngine`, `System`, `System.Collections.Generic` |
| **可见性** | `public` |

---

## 🎯 类说明

### StaggeredGridItemPool

交错网格项对象池，功能与 `ItemPool` 类似。

**核心职责**:
- 管理 Item 预制体
- 维护空闲 Item 列表
- 提供 Item 获取和回收接口

---

## 🔧 API 说明

### Init

```csharp
public void Init(GameObject prefabObj, float padding, int createCount, RectTransform parent)
```

**说明**: 初始化对象池。

---

### GetItem

```csharp
public LoopStaggeredGridViewItem GetItem()
```

**说明**: 从对象池获取 Item。

---

### RecycleItem

```csharp
public void RecycleItem(LoopStaggeredGridViewItem item)
```

**说明**: 回收 Item。

---

## 📚 相关文档链接

| 文档 | 说明 |
|------|------|
| [LoopStaggeredGridView.cs.md](./LoopStaggeredGridView.cs.md) | 交错网格视图核心 |

---

*文档由 OpenClaw AI 助手自动生成 | SuperScrollView 版本 2.4.0*
