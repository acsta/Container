# UILoopListView2.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | UILoopListView2.cs |
| **路径** | Assets/Scripts/Code/Module/UIComponent/UILoopListView2.cs |
| **所属模块** | 框架层 → Code/Module/UIComponent |
| **文件职责** | 循环列表组件，封装 LoopListView2（SuperScrollView） |

---

## 类/结构体说明

### UILoopListView2

| 属性 | 说明 |
|------|------|
| **职责** | 封装 SuperScrollView.LoopListView2，支持无限循环滚动列表 |
| **泛型参数** | 无 |
| **继承关系** | 继承自 `UIBaseContainer` |
| **实现的接口** | `IOnDestroy` |

```csharp
public class UILoopListView2 : UIBaseContainer, IOnDestroy
{
    // 循环列表组件
}
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `loopListView` | `LoopListView2` | `public` | SuperScrollView LoopListView2 组件 |

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
1. 如果 loopListView 不为 null
2. 调用 ClearListView() 清空列表
3. 设置 loopListView = null
```

**调用者**: `UIBaseContainer.BeforeOnDestroy()`

---

### InitListView(int itemTotalCount, onGetItemByIndex, initParam)

**签名**:
```csharp
public void InitListView(int itemTotalCount,
    System.Func<LoopListView2, int, LoopListViewItem2> onGetItemByIndex,
    LoopListViewInitParam initParam = null)
```

**职责**: 初始化列表

**参数**:
- `itemTotalCount`: 物品总数
- `onGetItemByIndex`: 获取物品的回调（根据索引创建/复用物品）
- `initParam`: 初始化参数

**核心逻辑**:
```
1. 激活 LoopListView2 组件
2. 调用 loopListView.InitListView()
```

**调用者**: 列表初始化代码

**使用示例**:
```csharp
listView.InitListView(itemList.Count, OnGetItemByIndex);

private LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
{
    // 获取或创建物品
    LoopListViewItem2 item = listView.GetNewListItem();
    
    // 获取 UI 组件
    UIItemView itemView = listView.AddItemViewComponent<UIItemView>(item);
    
    // 设置数据
    itemView.SetData(itemList[index]);
    
    return item;
}
```

---

### AddItemViewComponent<T>(LoopListViewItem2 item)

**签名**:
```csharp
public T AddItemViewComponent<T>(LoopListViewItem2 item) where T : UIBaseContainer
```

**职责**: 为物品添加 UI 组件

**参数**:
- `item`: Unity 侧的物品对象

**返回**: UI 侧的组件实例

**核心逻辑**:
```
1. 修改物品名称（添加 ItemId，避免缓存冲突）
2. 添加组件（不创建）AddComponentNotCreate<T>()
3. 设置 Transform
4. 如果实现 IOnCreate，调用 OnCreate()
5. 如果 ActiveSelf=true，设置 Active
6. 如果实现 II18N，注册到 I18NManager
7. 返回组件
```

**调用者**: `InitListView` 的 `onGetItemByIndex` 回调

---

### GetUIItemView<T>(LoopListViewItem2 item)

**签名**:
```csharp
public T GetUIItemView<T>(LoopListViewItem2 item) where T : UIBaseContainer
```

**职责**: 根据 Unity 侧物品获取 UI 侧组件

**参数**:
- `item`: Unity 侧的物品对象

**返回**: UI 侧的组件实例

**核心逻辑**:
```
1. 通过物品名称获取组件
2. 返回组件
```

**调用者**: 需要访问已创建物品的代码

---

### SetListItemCount(int itemCount, bool resetPos)

**签名**:
```csharp
public void SetListItemCount(int itemCount, bool resetPos = true)
```

**职责**: 重新设置物品数量

**参数**:
- `itemCount`: 新的物品数量
- `resetPos`: 是否刷新当前位置

**核心逻辑**:
```
1. 激活 LoopListView2 组件
2. 调用 loopListView.SetListItemCount()
```

**调用者**: 列表数据变化时

**使用示例**:
```csharp
// 物品数量变化后刷新
listView.SetListItemCount(newItemCount);
```

---

### GetShownItemByItemIndex<T>(int itemIndex)

**签名**:
```csharp
public T GetShownItemByItemIndex<T>(int itemIndex) where T : UIBaseContainer
```

**职责**: 获取指定索引的显示物品

**参数**:
- `itemIndex`: 物品索引

**返回**: UI 侧的组件实例（如果未显示返回 null）

**核心逻辑**:
```
1. 激活 LoopListView2 组件
2. 调用 loopListView.GetShownItemByItemIndex()
3. 如果物品为 null，返回 null
4. 否则获取 UI 组件并返回
```

**调用者**: 需要访问特定物品的代码

---

### RefreshAllShownItem()

**签名**:
```csharp
public void RefreshAllShownItem()
```

**职责**: 刷新所有显示的物品

**核心逻辑**:
```
1. 激活 LoopListView2 组件
2. 调用 loopListView.RefreshAllShownItem()
```

**调用者**: 数据变化需要刷新显示

---

### 拖拽事件

| 方法 | 说明 |
|------|------|
| `SetOnBeginDragAction(Action<PointerEventData> callback)` | 开始拖拽 |
| `SetOnDragingAction(Action<PointerEventData> callback)` | 拖拽中 |
| `SetOnEndDragAction(Action<PointerEventData> callback)` | 结束拖拽 |

---

### 滚动定位

| 方法 | 说明 |
|------|------|
| `MovePanelToItemIndex(int index, float offset)` | 滚动到指定索引 |
| `SetSnapTargetItemIndex(int index, float moveMaxAbsVec)` | 设置目标索引 |
| `GetSnapTargetItemIndex()` | 获取目标索引 |
| `SetSnapMaxAbsVec(float maxAbsVec)` | 设置最大滚动速度 |

---

### 快照事件

| 方法 | 说明 |
|------|------|
| `SetOnSnapOverAction(Action<LoopListView2, LoopListViewItem2> callback)` | 快照完成 |
| `SetOnSnapChange(Action<LoopListView2, LoopListViewItem2> callback)` | 快照变化 |

---

### CleanUp(string name)

**签名**:
```csharp
public void CleanUp(string name)
```

**职责**: 清理指定名称的物品

**核心逻辑**:
```
1. 如果 loopListView 为 null，返回
2. 调用 loopListView.CleanUp()
3. 使用 RemoveUIItemAllComponent 移除组件
```

**调用者**: 需要清理特定物品

---

## 阅读指引

### 建议的阅读顺序

1. **理解组件作用** - 为什么需要 UILoopListView2
2. **看 InitListView** - 理解列表初始化
3. **看 AddItemViewComponent** - 理解 UI 组件创建
4. **了解滚动定位** - 理解 MovePanelToItemIndex

### 最值得学习的技术点

1. **无限循环**: LoopListView2 支持虚拟列表
2. **物品复用**: GetNewListItem 复用物品对象
3. **UI 组件管理**: AddItemViewComponent 自动创建 UI 组件
4. **II18N 注册**: 自动注册国际化组件

---

## 使用示例

### 示例 1: 物品列表

```csharp
public class UIItemListView : UIBaseView, IOnCreate
{
    private UILoopListView2 listView;
    private List<ItemConfig> itemList;
    
    public void OnCreate()
    {
        listView = AddComponent<UILoopListView2>("ItemList");
        
        // 初始化列表
        listView.InitListView(itemList.Count, OnGetItemByIndex);
    }
    
    private LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
    {
        // 获取或创建物品
        LoopListViewItem2 item = listView.GetNewListItem();
        
        // 获取 UI 组件
        UIItemView itemView = listView.AddItemViewComponent<UIItemView>(item);
        
        // 设置数据
        itemView.SetData(itemList[index]);
        
        return item;
    }
    
    public void RefreshList()
    {
        listView.SetListItemCount(itemList.Count);
        listView.RefreshAllShownItem();
    }
}
```

### 示例 2: 滚动到指定位置

```csharp
// 滚动到第 10 个物品
listView.MovePanelToItemIndex(10);

// 滚动到第 10 个物品，偏移 50 像素
listView.MovePanelToItemIndex(10, offset: 50);
```

### 示例 3: 拖拽事件

```csharp
// 监听拖拽
listView.SetOnBeginDragAction((data) =>
{
    Log.Info("开始拖拽列表");
});

listView.SetOnEndDragAction((data) =>
{
    Log.Info("结束拖拽列表");
});
```

### 示例 4: 获取显示的物品

```csharp
// 获取第 5 个显示的物品
UIItemView itemView = listView.GetShownItemByItemIndex<UIItemView>(5);
if (itemView != null)
{
    // 物品正在显示
    itemView.Highlight();
}
```

---

## 相关文档

- [UIBaseContainer.cs.md](./UIBaseContainer.cs.md) - UI 容器基类
- [UILoopGridView.cs.md](./UILoopGridView.cs.md) - 循环网格组件
- [UICopyGameObject.cs.md](./UICopyGameObject.cs.md) - GameObject 复制组件

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
