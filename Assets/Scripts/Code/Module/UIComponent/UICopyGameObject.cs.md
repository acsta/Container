# UICopyGameObject.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | UICopyGameObject.cs |
| **路径** | Assets/Scripts/Code/Module/UIComponent/UICopyGameObject.cs |
| **所属模块** | 框架层 → Code/Module/UIComponent |
| **文件职责** | GameObject 复制组件，封装 CopyGameObject |

---

## 类/结构体说明

### UICopyGameObject

| 属性 | 说明 |
|------|------|
| **职责** | 封装 CopyGameObject，支持 GameObject 列表复制 |
| **泛型参数** | 无 |
| **继承关系** | 继承自 `UIBaseContainer` |
| **实现的接口** | `IOnDestroy` |

```csharp
public class UICopyGameObject : UIBaseContainer, IOnDestroy
{
    // GameObject 复制组件
}
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `comp` | `CopyGameObject` | `private` | CopyGameObject 组件 |

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
1. 调用 comp.Clear() 清空列表
```

**调用者**: `UIBaseContainer.BeforeOnDestroy()`

---

### InitListView(int totalCount, onGetItemCallback, startSiblingIndex)

**签名**:
```csharp
public void InitListView(int totalCount, Action<int, GameObject> onGetItemCallback = null,
    int? startSiblingIndex = null)
```

**职责**: 初始化列表

**参数**:
- `totalCount`: 物品总数
- `onGetItemCallback`: 获取物品的回调（根据索引创建/复用物品）
- `startSiblingIndex`: 起始兄弟索引（可选）

**核心逻辑**:
```
1. 激活 CopyGameObject 组件
2. 调用 comp.InitListView()
```

**调用者**: 列表初始化代码

**使用示例**:
```csharp
copyGO.InitListView(itemList.Count, OnGetItem);

private void OnGetItem(int index, GameObject item)
{
    // 获取 UI 组件
    UIItemView itemView = copyGO.AddItemViewComponent<UIItemView>(item);
    
    // 设置数据
    itemView.SetData(itemList[index]);
}
```

---

### AddItemViewComponent<T>(GameObject item)

**签名**:
```csharp
public T AddItemViewComponent<T>(GameObject item) where T : UIBaseContainer
```

**职责**: 为物品添加 UI 组件

**参数**:
- `item`: Unity 侧的物品对象

**返回**: UI 侧的组件实例

**核心逻辑**:
```
1. 添加组件（不创建）AddComponentNotCreate<T>()
2. 设置 Transform
3. 如果实现 IOnCreate，调用 OnCreate()
4. 如果 ActiveSelf=true，设置 Active
5. 如果实现 II18N，注册到 I18NManager
6. 返回组件
```

**调用者**: `InitListView` 的 `onGetItemCallback` 回调

---

### GetUIItemView<T>(GameObject item)

**签名**:
```csharp
public T GetUIItemView<T>(GameObject item) where T : UIBaseContainer
```

**职责**: 根据 Unity 侧物品获取 UI 侧组件

---

### SetListItemCount(int totalCount, startSiblingIndex)

**签名**:
```csharp
public void SetListItemCount(int totalCount, int? startSiblingIndex = null)
```

**职责**: 重新设置物品数量

---

### RefreshAllShownItem(int? startSiblingIndex)

**签名**:
```csharp
public void RefreshAllShownItem(int? startSiblingIndex = null)
```

**职责**: 刷新所有显示的物品

---

### GetItemByIndex(int index)

**签名**:
```csharp
public GameObject GetItemByIndex(int index)
```

**职责**: 根据索引获取物品

**参数**:
- `index`: 物品索引

**返回**: GameObject

**调用者**: 需要访问特定物品的代码

---

### GetListItemCount()

**签名**:
```csharp
public int GetListItemCount()
```

**职责**: 获取物品数量

**返回**: 当前物品数量

---

## 阅读指引

### 建议的阅读顺序

1. **理解组件作用** - 为什么需要 UICopyGameObject
2. **看 InitListView** - 理解列表初始化
3. **看 AddItemViewComponent** - 理解 UI 组件创建
4. **了解物品获取** - 理解 GetItemByIndex

### 最值得学习的技术点

1. **GameObject 复制**: CopyGameObject 组件复制 GameObject
2. **UI 组件管理**: AddItemViewComponent 自动创建 UI 组件
3. **II18N 注册**: 自动注册国际化组件
4. **灵活索引**: startSiblingIndex 支持自定义位置

---

## 与 UILoopListView2 的区别

| 特性 | UILoopListView2 | UICopyGameObject |
|------|-----------------|------------------|
| 底层组件 | LoopListView2 | CopyGameObject |
| 滚动优化 | 支持（虚拟列表） | 不支持 |
| 适用场景 | 长列表 | 短列表、固定数量 |
| 性能 | 高（物品复用） | 中 |

---

## 使用示例

### 示例 1: 简单列表

```csharp
public class UISimpleListView : UIBaseView, IOnCreate
{
    private UICopyGameObject copyGO;
    private List<string> items;
    
    public void OnCreate()
    {
        copyGO = AddComponent<UICopyGameObject>("ItemList");
        
        // 初始化列表
        copyGO.InitListView(items.Count, OnGetItem);
    }
    
    private void OnGetItem(int index, GameObject item)
    {
        // 获取 UI 组件
        UITextItemView itemView = copyGO.AddItemViewComponent<UITextItemView>(item);
        
        // 设置数据
        itemView.SetText(items[index]);
    }
    
    public void RefreshList()
    {
        copyGO.SetListItemCount(items.Count);
        copyGO.RefreshAllShownItem();
    }
}
```

### 示例 2: 获取物品

```csharp
// 获取第 3 个物品
GameObject item = copyGO.GetItemByIndex(3);
if (item != null)
{
    // 处理物品
    Log.Info($"物品名称：{item.name}");
}
```

### 示例 3: 获取物品数量

```csharp
int count = copyGO.GetListItemCount();
Log.Info($"当前物品数量：{count}");
```

---

## 相关文档

- [UIBaseContainer.cs.md](./UIBaseContainer.cs.md) - UI 容器基类
- [UILoopListView2.cs.md](./UILoopListView2.cs.md) - 循环列表组件
- [UILoopGridView.cs.md](./UILoopGridView.cs.md) - 循环网格组件

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
