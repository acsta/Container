# UIBagWin.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | UIBagWin.cs |
| **路径** | Assets/Scripts/Code/Game/UIGame/UICreate/UIBagWin.cs |
| **所属模块** | 游戏层 → Code/Game/UIGame/UICreate |
| **文件职责** | 背包窗口组件，显示玩家已拥有的装备，支持分类浏览和滚动列表 |

---

## 类/结构体说明

### UIBagWin

| 属性 | 说明 |
|------|------|
| **职责** | 管理玩家背包中的装备显示，支持按模块分类、滚动列表展示、套装信息显示 |
| **泛型参数** | 无 |
| **继承关系** | `UIBaseContainer` |
| **实现的接口** | `IOnCreate` |

**设计模式**: 组件化 + 虚拟列表 (LoopListView2)

```csharp
// 作为子组件添加到父视图
BagWin = AddComponent<UIBagWin>("UICommonView/Bg/Content/Table");
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `ScrollView` | `UILoopListView2` | `public` | 滚动列表组件，虚拟列表显示装备 |
| `BagClose` | `UIButton` | `public` | 关闭背包按钮 |
| `Menu` | `UIMenu` | `public` | 顶部模块切换菜单 |
| `Table` | `UIAnimator` | `public` | 背包展开/收起动画控制器 |
| `GroupInfoTable` | `GroupInfoTable` | `public` | 套装激活信息显示 |
| `clothConfigs` | `List<ClothConfig>` | `private` | 当前模块的装备配置列表 |
| `moduleId` | `int` | `private` | 当前选中的模块 ID |
| `onClickItem` | `Action<int, int>` | `private` | 物品点击回调 |

---

## 方法说明（按重要程度排序）

### OnCreate()

**签名**:
```csharp
public void OnCreate()
```

**职责**: 初始化背包窗口组件

**核心逻辑**:
```
1. 初始化 clothConfigs 列表
2. 添加 GroupInfoTable 组件 (套装信息)
3. 添加 ScrollView 滚动列表并初始化
4. 添加 Menu 菜单组件
5. 添加 Table 动画组件
6. 添加 BagClose 关闭按钮
```

**调用者**: 父视图 OnCreate 时通过 AddComponent 调用

---

### SetData(Action<int, int> onClickItem)

**签名**:
```csharp
public void SetData(Action<int, int> onClickItem)
```

**职责**: 设置背包数据

**核心逻辑**:
```
1. 保存点击回调 onClickItem
2. 解析颜色值 #FCC63A(激活) 和 #AF5C09(未激活)
3. 获取所有角色配置 CharacterConfigCategory.Instance.GetAllList()
4. 为每个模块 (从索引 1 开始) 创建 MenuPara
5. 设置 Menu 数据和切换回调 OnMenuChange
6. 绑定 BagClose 关闭按钮事件
```

**调用者**: UICreateView.OnEnable, Open

---

### Open(Action<int, int> onClickItem)

**签名**:
```csharp
public void Open(Action<int, int> onClickItem)
```

**职责**: 打开背包窗口

**核心逻辑**:
```
1. 播放背包展开动画 "Bag_open"
2. 设置数据 SetData
3. 保存当前 moduleId 并重置为 -1
4. 显示之前选中的模块列表
```

**调用者**: UICreateView.OnClickOpenBag

**被调用者**: `Table.Play()`, `SetData()`, `ShowList()`

---

### ShowList(int id)

**签名**:
```csharp
public void ShowList(int id)
```

**职责**: 显示指定模块的装备列表

**核心逻辑**:
```
1. 如果 moduleId 相同则返回 (避免重复刷新)
2. 更新 moduleId
3. 获取该模块的所有装备配置 ClothConfigCategory.Instance.GetModule(id)
4. 获取角色配置 CharacterConfigCategory.Instance.Get(id)
5. 清空 clothConfigs 列表
6. 遍历装备配置，过滤条件:
   - 不是默认装备
   - 玩家拥有数量 > 0
7. 按稀有度降序排序 (b.Rare - a.Rare)
8. 计算表格数量 (每行 4 个)
9. 设置 ScrollView 列表项数量
10. 刷新所有显示项
11. 设置 Menu 激活索引 (moduleId - 2)
```

**调用者**: Open, OnMenuChange

**被调用者**: `ScrollView.SetListItemCount()`, `ScrollView.RefreshAllShownItem()`, `Menu.SetActiveIndex()`

---

### GetScrollViewItemByIndex()

**签名**:
```csharp
public LoopListViewItem2 GetScrollViewItemByIndex(LoopListView2 listView, int index)
```

**职责**: 滚动列表项获取回调 (虚拟列表)

**核心逻辑**:
```
1. 计算总表格数 = (clothConfigs.Count + 4) / 4
2. 如果 index 越界 → 返回 null
3. 创建或获取列表项 "TableItem"
4. 如果是首次初始化 → 添加 TableItem 组件
5. 否则 → 获取已存在的 TableItem 组件
6. 设置列表项尺寸为全宽
7. 调用 TableItem.SetData 设置数据
8. 返回列表项
```

**调用者**: UILoopListView2 滚动时

**被调用者**: `ScrollView.AddItemViewComponent<TableItem>()`, `TableItem.SetData()`

---

### OnMenuChange(MenuPara para)

**签名**:
```csharp
private void OnMenuChange(MenuPara para)
```

**职责**: 处理模块菜单切换

**调用者**: UIMenu 切换回调

**被调用者**: `ShowList(para.Id)`

---

### OnClickCloseBag()

**签名**:
```csharp
private void OnClickCloseBag()
```

**职责**: 关闭背包

**核心逻辑**:
```
1. 播放背包收起动画 "Bag_close"
```

**调用者**: BagClose 按钮点击

---

### OnClickItem(int id, int moduleId)

**签名**:
```csharp
private void OnClickItem(int id, int moduleId)
```

**职责**: 处理物品点击

**调用者**: TableItem 点击回调

**被调用者**: `onClickItem?.Invoke()`

---

## 背包界面结构

```
UIBagWin
├── Top
│   ├── Tip (GroupInfoTable) - 套装激活信息
│   ├── UIMenu - 模块切换菜单
│   └── CloseBag (UIButton) - 关闭按钮
├── ScrollView (UILoopListView2) - 装备滚动列表
│   └── TableItem (动态创建)
│       └── ClothItem[0-3] - 每行 4 个装备项
└── Table (UIAnimator) - 展开/收起动画
```

---

## 使用示例

### 作为子组件添加

```csharp
// 在父视图 OnCreate 中
BagWin = AddComponent<UIBagWin>("UICommonView/Bg/Content/Table");

// 在父视图 OnEnable 中
BagWin.SetData(OnClickItem);

// 打开背包
BagWin.Open(OnClickItem);
```

### 物品点击回调

```csharp
private void OnClickItem(int id, int moduleId)
{
    if (id <= 0)
    {
        // 卸下装备
        player.SetModule(moduleId, 0);
    }
    else
    {
        // 装备物品
        player.SetModule(moduleId, id);
    }
    RefreshGroupInfo();
}
```

---

## 相关文档

- [UICreateView.cs.md](./UICreateView.cs.md) - 父视图
- [UILoopListView2.cs.md](../../Module/UIComponent/UILoopListView2.cs.md) - 滚动列表
- [TableItem.cs.md](./TableItem.cs.md) - 列表项
- [GroupInfoTable.cs.md](./GroupInfoTable.cs.md) - 套装信息表
- [UIMenu.cs.md](../UICommon/UIMenu.cs.md) - 菜单组件
- [ClothConfig.cs.md](../../../Config/ClothConfig.cs.md) - 装备配置

---

*文档生成时间：2026-03-02 | OpenClaw AI 助手*
