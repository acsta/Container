# LoopGridItemPool.cs - 网格项对象池

> **文件路径**: `Assets/Scripts/ThirdParty/SuperScrollView/GridView/LoopGridItemPool.cs`  
> **命名空间**: `SuperScrollView`  
> **文档生成时间**: 2026-03-03  
> **文件类型**: 第三方库 (SuperScrollView)

---

## 📑 文件信息表

| 属性 | 值 |
|------|-----|
| **文件路径** | `Assets/Scripts/ThirdParty/SuperScrollView/GridView/LoopGridItemPool.cs` |
| **命名空间** | `SuperScrollView` |
| **类/结构体** | `GridItemPool` |
| **依赖** | `UnityEngine`, `System`, `System.Collections.Generic` |
| **可见性** | `public` |

---

## 🎯 类说明

### GridItemPool

网格项对象池，管理单一类型 Item 的创建、回收和复用。

**核心职责**:
- 管理 Item 预制体
- 维护空闲 Item 列表
- 提供 Item 获取和回收接口
- 支持临时回收队列（批量操作优化）

**设计特点**:
- 与 `ItemPool`（ListView）类似，针对网格优化
- 两级回收队列（临时 + 正式）
- 自动递增 ItemId
- 支持清理回调

---

## 📊 字段表

| 字段名 | 类型 | 可见性 | 说明 |
|--------|------|--------|------|
| `mPrefabObj` | `GameObject` | `private` | 预制体对象 |
| `mPrefabName` | `string` | `private` | 预制体名称 |
| `mInitCreateCount` | `int` | `private` | 初始化创建数量 |
| `mTmpPooledItemList` | `List<LoopGridViewItem>` | `private` | 临时回收队列 |
| `mPooledItemList` | `List<LoopGridViewItem>` | `private` | 正式回收队列 |
| `mCurItemIdCount` | `int` | `private static` | 当前 ItemId 计数器 |
| `mItemParent` | `RectTransform` | `private` | Item 父节点 |

---

## 🔧 API 说明

### 初始化

#### Init

```csharp
public void Init(GameObject prefabObj, int createCount, RectTransform parent)
```

**说明**: 初始化对象池。

**参数**:
| 参数 | 类型 | 说明 |
|------|------|------|
| `prefabObj` | `GameObject` | Item 预制体 |
| `createCount` | `int` | 初始化创建数量 |
| `parent` | `RectTransform` | Item 父节点 |

---

#### ClearPool

```csharp
public void ClearPool()
```

**说明**: 清空对象池，重置 ItemId 计数器。

---

#### CleanUp

```csharp
public void CleanUp(Action<GameObject> beforeDestroy)
```

**说明**: 清理对象池，可指定销毁前回调。

---

### Item 管理

#### GetItem

```csharp
public LoopGridViewItem GetItem(int? index)
```

**说明**: 从对象池获取 Item。

**参数**:
| 参数 | 类型 | 说明 |
|------|------|------|
| `index` | `int?` | 可选的索引 |

**返回值**:
| 类型 | 说明 |
|------|------|
| `LoopGridViewItem` | 获取的 Item |

---

#### RecycleItem

```csharp
public void RecycleItem(LoopGridViewItem item)
```

**说明**: 回收 Item 到临时队列。

---

#### ClearTmpRecycledItem

```csharp
public void ClearTmpRecycledItem()
```

**说明**: 将临时回收队列的 Item 移到正式队列。

---

#### DestroyAllItem

```csharp
public void DestroyAllItem()
```

**说明**: 销毁池中所有 Item。

---

## 💡 使用示例

### 对象池初始化

```csharp
// 由 LoopGridView 自动调用
GridItemPool pool = new GridItemPool();
pool.Init(
    itemPrefab,      // Item 预制体
    5,               // 初始创建 5 个
    containerTrans   // 父节点
);
```

---

### 获取和回收 Item

```csharp
// 获取 Item
var item = pool.GetItem();
item.gameObject.SetActive(true);

// 回收 Item
pool.RecycleItem(item);
item.gameObject.SetActive(false);

// 批量操作结束后
pool.ClearTmpRecycledItem();
```

---

## 📚 相关文档链接

| 文档 | 说明 |
|------|------|
| [LoopGridView.cs.md](./LoopGridView.cs.md) | 网格视图核心 |
| [LoopGridViewItem.cs.md](./LoopGridViewItem.cs.md) | Item 基类 |

---

*文档由 OpenClaw AI 助手自动生成 | SuperScrollView 版本 2.4.0*
