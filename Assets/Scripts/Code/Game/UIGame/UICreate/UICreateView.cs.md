# UICreateView.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | UICreateView.cs |
| **路径** | Assets/Scripts/Code/Game/UIGame/UICreate/UICreateView.cs |
| **所属模块** | 游戏层 → Code/Game/UIGame/UICreate |
| **文件职责** | 角色创建/换装界面主视图，管理角色外观配置、装备预览、背包和商店入口 |

---

## 类/结构体说明

### UICreateView

| 属性 | 说明 |
|------|------|
| **职责** | 角色创建界面主控制器，处理角色外观配置、装备切换、保存确认等交互 |
| **泛型参数** | 无 |
| **继承关系** | `UIBaseView` |
| **实现的接口** | `IOnCreate`, `IOnEnable<Player>`, `IOnTopWidthPaddingChange` |

**设计模式**: MVC 视图层 + 组件化

```csharp
// 通过 UIManager 打开
UIManager.Instance.OpenWindow<UICreateView, Player>(UICreateView.PrefabPath, player);
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `PrefabPath` | `string` | `public static` |预制体路径 "UIGame/UICreate/Prefabs/UICreateView.prefab" |
| `btn_start` | `UIButton` | `public` | 开始/保存按钮 |
| `Item` | `CreateItem[]` | `public` | 8 个装备槽位显示项 (索引 0-7 对应模块 2-9) |
| `Close` | `UIButton` | `public` | 关闭按钮 |
| `player` | `Player` | `private` | 当前玩家数据 |
| `Drager` | `UIEventTrigger` | `public` | 角色旋转拖拽区域 |
| `BagWin` | `UIBagWin` | `public` | 背包窗口组件 |
| `ShopWin` | `UIShopWin` | `public` | 商店窗口组件 |
| `BagOpen` | `UIButton` | `public` | 打开背包按钮 |
| `CashGroup` | `UICashGroup` | `public` | 货币显示组 |
| `startRot` | `Vector3` | `private` | 拖拽起始旋转角度 |
| `startDragPos` | `Vector2` | `private` | 拖拽起始位置 |

---

## 方法说明（按重要程度排序）

### OnCreate()

**签名**:
```csharp
public void OnCreate()
```

**职责**: 初始化界面组件

**核心逻辑**:
```
1. 添加 BagWin 组件 (背包窗口)
2. 添加 ShopWin 组件 (商店窗口，传入点击回调)
3. 添加 BagOpen 按钮 (打开背包)
4. 添加 CashGroup 组件 (货币显示)
5. 添加 btn_start 按钮 (保存)
6. 初始化 8 个 CreateItem 装备槽位
7. 添加 Close 按钮
8. 添加 Drager 拖拽区域并绑定拖拽事件
```

**调用者**: UIManager 创建窗口时

---

### OnEnable(Player player)

**签名**:
```csharp
public void OnEnable(Player player)
```

**职责**: 启用窗口时设置数据

**核心逻辑**:
```
1. 设置 BagWin 数据 (传入点击回调)
2. 绑定 BagOpen 按钮点击事件
3. 保存 player 引用
4. 绑定 btn_start 保存按钮事件
5. 绑定 Close 关闭按钮事件
6. 初始化 8 个装备槽位数据 (模块 ID 2-9)
7. 显示模块 2 的装备列表
8. 刷新套装信息
```

**调用者**: UIManager 打开窗口时

---

### OnClickSave()

**签名**:
```csharp
public void OnClickSave()
```

**职责**: 保存角色外观配置

**核心逻辑**:
```
1. 遍历玩家所有模块，检查是否有未付费的装备
2. 如果有变更且有未付费物品 → 打开购买窗口 UIBuyWin
3. 如果有变更且都已付费 → 调用 PlayerDataManager.ChangeShow() 保存
4. 显示保存成功提示 UIToast
```

**调用者**: btn_start 按钮点击

**被调用者**: `UIManager.OpenWindow<UIBuyWin>()`, `PlayerDataManager.ChangeShow()`, `UIManager.OpenBox<UIToast>()`

---

### OnClickClose()

**签名**:
```csharp
public void OnClickClose()
```

**职责**: 关闭界面并处理未保存提示

**核心逻辑**:
```
1. 检查是否有未保存的变更
2. 如果有变更 → 打开确认框 UIMsgBoxWin
   - 取消 → 关闭确认框
   - 确认 → 关闭确认框并切换到 HomeScene
3. 如果没有变更 → 直接切换到 HomeScene
```

**调用者**: Close 按钮点击

**被调用者**: `UIManager.OpenBox<UIMsgBoxWin>()`, `SceneManager.SwitchScene<HomeScene>()`

---

### OnClickItem(int id, int moduleId)

**签名**:
```csharp
public void OnClickItem(int id, int moduleId)
```

**职责**: 处理装备物品点击

**核心逻辑**:
```
1. 获取角色配置 CharacterConfigCategory.Instance.Get(moduleId)
2. 如果 id<=0 或等于默认装备 → 直接装备
3. 否则 → 打开装备详情窗口 UIEquipWin
```

**调用者**: BagWin, ShopWin 的物品点击回调

**被调用者**: `UIManager.OpenWindow<UIEquipWin>()`

---

### OnEquipItem(int id, int moduleId)

**签名**:
```csharp
public void OnEquipItem(int id, int moduleId)
```

**职责**: 装备/卸下物品

**核心逻辑**:
```
1. 如果 id<=0 且模块有默认装备 → 使用默认装备 ID
2. 如果 moduleId>1 → 更新对应 CreateItem 显示
3. 调用 player.SetModule() 设置模块装备
4. 刷新套装信息
```

**调用者**: UIEquipWin 确认回调, OnClickItem

**被调用者**: `Player.SetModule()`, `RefreshGroupInfo()`

---

### OnClickEquipItem(int id, int moduleId)

**签名**:
```csharp
public void OnClickEquipItem(int id, int moduleId)
```

**职责**: 处理装备槽位点击

**核心逻辑**:
```
1. 显示对应模块的装备列表 ShowList(moduleId)
2. 如果 id>0 → 触发 OnClickItem 打开详情
```

**调用者**: CreateItem 点击回调

---

### RefreshGroupInfo()

**签名**:
```csharp
private void RefreshGroupInfo()
```

**职责**: 刷新套装激活信息

**核心逻辑**:
```
1. 创建临时字典统计各套装激活数量
2. 遍历玩家所有模块 (1-8)
3. 如果装备不是默认装备且有 GroupId → 累加计数
4. 设置 BagWin 和 ShopWin 的 GroupInfoTable 数据
```

**调用者**: OnEquipItem, OnEnable

**被调用者**: `GroupInfoTable.SetData()`

---

### OnBeginDrag(PointerEventData data)

**签名**:
```csharp
private void OnBeginDrag(PointerEventData data)
```

**职责**: 记录拖拽起始状态

**核心逻辑**:
```
1. 保存起始拖拽位置
2. 保存玩家起始旋转角度
```

**调用者**: UIEventTrigger 拖拽开始事件

---

### OnDrag(PointerEventData data)

**签名**:
```csharp
private void OnDrag(PointerEventData data)
```

**职责**: 处理拖拽旋转角色

**核心逻辑**:
```
1. 计算 X 轴拖拽偏移量
2. 根据偏移量更新玩家旋转角度 (绕 Y 轴)
```

**调用者**: UIEventTrigger 拖拽事件

---

### ShowList(int id)

**签名**:
```csharp
public void ShowList(int id)
```

**职责**: 显示指定模块的装备列表

**调用者**: OnClickEquipItem

**被调用者**: `BagWin.ShowList()`

---

## 界面交互流程

### 装备购买流程

```mermaid
sequenceDiagram
    participant User as 用户
    participant UICreate as UICreateView
    player as Player
    UI as UIManager
    BagWin as UIBagWin
    EquipWin as UIEquipWin
    BuyWin as UIBuyWin
    Toast as UIToast

    User->>UICreate: 点击装备槽位
    UICreate->>BagWin: ShowList(moduleId)
    User->>BagWin: 点击物品
    BagWin->>UICreate: OnClickItem(id, moduleId)
    UICreate->>EquipWin: OpenWindow(id, OnEquipItem, player)
    
    User->>EquipWin: 点击试穿/购买
    EquipWin->>UICreate: OnEquipItem(id, moduleId)
    UICreate->>player: SetModule(moduleId, id)
    UICreate->>UICreate: RefreshGroupInfo()
    
    User->>UICreate: 点击保存
    UICreate->>UICreate: 检查未付费物品
    alt 有未付费物品
        UICreate->>BuyWin: OpenWindow(noPayItems)
    else 都已付费
        UICreate->>player: ChangeShow(subModules)
        UICreate->>Toast: 显示保存成功
    end
```

### 关闭确认流程

```mermaid
sequenceDiagram
    participant User as 用户
    participant UICreate as UICreateView
    MsgBox as UIMsgBoxWin
    Scene as SceneManager

    User->>UICreate: 点击关闭
    UICreate->>UICreate: 检查是否有变更
    alt 有变更
        UICreate->>MsgBox: OpenBox(确认提示)
        User->>MsgBox: 点击取消
        MsgBox->>MsgBox: CloseBox
        User->>MsgBox: 点击确认
        MsgBox->>MsgBox: CloseBox
        MsgBox->>Scene: SwitchScene<HomeScene>
    else 无变更
        UICreate->>Scene: SwitchScene<HomeScene>
    end
```

---

## 使用示例

### 打开角色创建界面

```csharp
// 从其他场景切换到创建场景
await SceneManager.Instance.SwitchScene<CreateScene>();

// 创建界面会自动通过 IOnEnable<Player> 接收玩家数据
```

### 装备切换

```csharp
// 用户点击装备槽位
UICreateView view = UIManager.Instance.GetView<UICreateView>();
view.OnClickEquipItem(1001, 2); // 装备 ID 1001, 模块 2

// 或直接从背包选择
view.OnClickItem(1001, 2);
```

### 保存配置

```csharp
// 用户点击保存按钮
view.OnClickSave();

// 如果有未付费物品，会自动打开购买窗口
// 如果都已付费，会保存并显示成功提示
```

---

## 相关文档

- [UIBagWin.cs.md](./UIBagWin.cs.md) - 背包窗口
- [UIShopWin.cs.md](./UIShopWin.cs.md) - 商店窗口
- [UIEquipWin.cs.md](./UIEquipWin.cs.md) - 装备详情窗口
- [UIBuyWin.cs.md](./UIBuyWin.cs.md) - 购买窗口
- [CreateItem.cs.md](./CreateItem.cs.md) - 装备槽位项
- [Player.cs.md](../../Entity/Player.cs.md) - 玩家实体
- [SceneManager.cs.md](../../Module/Scene/SceneManager.cs.md) - 场景管理

---

*文档生成时间：2026-03-02 | OpenClaw AI 助手*
