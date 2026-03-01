# UIShopWin.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | UIShopWin.cs |
| **路径** | Assets/Scripts/Code/Game/UIGame/UICreate/UIShopWin.cs |
| **所属模块** | 游戏层 → Code/Game/UIGame/UICreate |
| **文件职责** | 商店窗口组件，显示每日随机刷新的装备商品，支持广告刷新和倒计时 |

---

## 类/结构体说明

### UIShopWin

| 属性 | 说明 |
|------|------|
| **职责** | 管理商店商品展示，每日自动刷新，支持通过广告手动刷新，显示刷新倒计时 |
| **泛型参数** | 无 |
| **继承关系** | `UIBaseContainer` |
| **实现的接口** | `IOnCreate<Action<int,int>>`, `IOnEnable`, `IOnDisable` |

**设计模式**: 组件化 + 定时器

```csharp
// 作为子组件添加到父视图
ShopWin = AddComponent<UIShopWin, Action<int, int>>("UICommonView/Bg/Content/Shop", OnClickItem);
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `RefreshBtn` | `UIButton` | `public` | 刷新按钮 |
| `RefreshText` | `UITextmesh` | `public` | 刷新倒计时文本 |
| `ShopItems` | `ShopItem[]` | `public` | 商品展示项数组 |
| `GroupInfoTable` | `GroupInfoTable` | `public` | 套装激活信息显示 |
| `timerId` | `long` | `private` | 定时器 ID |
| `clickItem` | `Action<int,int>` | `private` | 物品点击回调 |
| `UIShopWinUpdateTimer` | `ATimer<UIShopWin>` | `public` | 定时器类 (TimerType.UIShopWin) |

---

## 类/结构体说明

### UIShopWinUpdateTimer

| 属性 | 说明 |
|------|------|
| **职责** | 商店窗口更新定时器，每秒更新刷新倒计时 |
| **继承关系** | `ATimer<UIShopWin>` |
| **触发类型** | `TimerType.UIShopWin` |

**Run 方法逻辑**:
```
1. 调用 self.UpdateTimeDown()
2. 异常捕获并记录日志
```

---

## 方法说明（按重要程度排序）

### OnCreate(Action<int,int> clickItem)

**签名**:
```csharp
public void OnCreate(Action<int,int> clickItem)
```

**职责**: 初始化商店窗口组件

**核心逻辑**:
```
1. 保存点击回调 clickItem
2. 添加 GroupInfoTable 组件 (套装信息)
3. 添加 RefreshBtn 刷新按钮
4. 添加 RefreshText 倒计时文本
5. 初始化 ShopItems 数组 (数量为 PlayerDataManager.ClothRefreshCount)
6. 设置刷新文本国际化 key
```

**调用者**: 父视图 OnCreate 时通过 AddComponent 调用

---

### OnEnable()

**签名**:
```csharp
public void OnEnable()
```

**职责**: 启用窗口时初始化

**核心逻辑**:
```
1. 根据广告平台支持情况显示/隐藏刷新按钮
2. 绑定刷新按钮点击事件 RefreshCloth
3. 异步刷新商品列表 RefreshList()
4. 更新倒计时 UpdateTimeDown()
5. 移除旧定时器
6. 创建 1 秒重复定时器 (TimerType.UIShopWin)
```

**调用者**: 父视图 OnEnable 时

**被调用者**: `RefreshList()`, `UpdateTimeDown()`, `TimerManager.NewRepeatedTimer()`

---

### OnDisable()

**签名**:
```csharp
public void OnDisable()
```

**职责**: 禁用窗口时清理定时器

**核心逻辑**:
```
1. 移除定时器
```

**调用者**: 父视图 OnDisable 时

---

### RefreshList()

**签名**:
```csharp
private async ETTask RefreshList()
```

**职责**: 刷新商品列表

**核心逻辑**:
```
1. 获取每日刷新商品列表 PlayerDataManager.GetClothsSale()
2. 遍历 ShopItems 数组:
   - 如果 i < list.Count → 设置 ShopItem 数据
   - 先隐藏 ShopItem
   - 如果有数据 → 显示 ShopItem
   - 等待 50ms(逐个显示动画效果)
```

**调用者**: OnEnable, PlayAdAsync

**被调用者**: `PlayerDataManager.GetClothsSale()`, `ShopItem.SetData()`

---

### RefreshCloth()

**签名**:
```csharp
private void RefreshCloth()
```

**职责**: 处理刷新按钮点击

**核心逻辑**:
```
1. 检查广告平台是否支持
2. 如果支持 → 播放广告刷新 PlayAdAsync()
```

**调用者**: RefreshBtn 点击

---

### PlayAdAsync()

**签名**:
```csharp
public async ETTask PlayAdAsync()
```

**职责**: 异步处理广告刷新

**核心逻辑**:
```
1. 禁用刷新按钮 (防止重复点击)
2. 调用 AdManager.PlayAd()
3. 如果广告完成:
   - 调用 PlayerDataManager.RefreshCloth(false) 刷新商品
   - 刷新列表 RefreshList()
   - 播放刷新音效
4. 异常处理: 记录日志
5. 最终恢复刷新按钮可用
```

**调用者**: RefreshCloth

**被调用者**: `AdManager.PlayAd()`, `PlayerDataManager.RefreshCloth()`, `RefreshList()`

---

### UpdateTimeDown()

**签名**:
```csharp
public void UpdateTimeDown()
```

**职责**: 更新刷新倒计时

**核心逻辑**:
```
1. 计算下次刷新时间 = LastRefreshDailyTime + 1 天
2. 计算剩余时间 = 下次刷新时间 - 当前时间
3. 转换为 DateTime
4. 如果小时 > 0 → 显示 "HH:mm"
5. 否则 → 显示 "mm:ss"
```

**调用者**: OnEnable, 定时器每秒触发

---

## 商店刷新流程

```mermaid
sequenceDiagram
    participant User as 用户
    participant ShopWin as UIShopWin
    PlayerDM as PlayerDataManager
    AdMgr as AdManager

    User->>ShopWin: 打开商店
    ShopWin->>PlayerDM: GetClothsSale()
    PlayerDM-->>ShopWin: 商品列表
    ShopWin->>ShopWin: 逐个显示商品 (50ms 间隔)
    ShopWin->>ShopWin: 启动倒计时定时器
    
    loop 每秒
        ShopWin->>ShopWin: UpdateTimeDown()
    end
    
    User->>ShopWin: 点击刷新
    ShopWin->>AdMgr: PlayAd()
    AdMgr-->>ShopWin: 广告完成
    ShopWin->>PlayerDM: RefreshCloth(false)
    PlayerDM-->>ShopWin: 新商品列表
    ShopWin->>ShopWin: RefreshList()
```

---

## 使用示例

### 作为子组件添加

```csharp
// 在父视图 OnCreate 中
ShopWin = AddComponent<UIShopWin, Action<int, int>>(
    "UICommonView/Bg/Content/Shop", OnClickItem);

// 在父视图 OnEnable 中
// ShopWin 会自动通过 IOnEnable 初始化
```

### 物品点击回调

```csharp
private void OnClickItem(int id, int moduleId)
{
    var module = CharacterConfigCategory.Instance.Get(moduleId);
    if (id <= 0 || id == module.DefaultCloth)
    {
        OnEquipItem(id, moduleId);
        return;
    }

    UIManager.Instance
        .OpenWindow<UIEquipWin, int, Action<int,int>, Player>(
            UIEquipWin.PrefabPath, id, OnEquipItem, player)
        .Coroutine();
}
```

---

## 相关文档

- [UICreateView.cs.md](./UICreateView.cs.md) - 父视图
- [ShopItem.cs.md](./ShopItem.cs.md) - 商品项
- [PlayerDataManager.cs.md](../../DataManager/PlayerDataManager.cs.md) - 玩家数据管理
- [AdManager.cs.md](../../Module/Ad/AdManager.cs.md) - 广告管理
- [TimerManager.cs.md](../../Module/Timer/TimerManager.cs.md) - 定时器管理
- [GroupInfoTable.cs.md](./GroupInfoTable.cs.md) - 套装信息表

---

*文档生成时间：2026-03-02 | OpenClaw AI 助手*
