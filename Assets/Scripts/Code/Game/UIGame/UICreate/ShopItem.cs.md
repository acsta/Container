# ShopItem.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | ShopItem.cs |
| **路径** | Assets/Scripts/Code/Game/UIGame/UICreate/ShopItem.cs |
| **所属模块** | 游戏层 → Code/Game/UIGame/UICreate |
| **文件职责** | 商店商品项，显示装备商品的价格和购买方式 (广告/金币) |

---

## 类/结构体说明

### ShopItem

| 属性 | 说明 |
|------|------|
| **职责** | 显示商店中的单个商品，包括装备信息、价格、购买按钮 (广告或金币) |
| **泛型参数** | 无 |
| **继承关系** | `UIBaseContainer` |
| **实现的接口** | `IOnCreate` |

**设计模式**: 组件化

```csharp
// 在 UIShopWin 中初始化
ShopItems = new ShopItem[PlayerDataManager.Instance.ClothRefreshCount];
for (int i = 0; i < ShopItems.Length; i++)
{
    ShopItems[i] = AddComponent<ShopItem>("Content/ShopItem" + i);
}
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `ClothItem` | `ClothItem` | `public` | 装备显示项 |
| `AdBtn` | `UIButton` | `public` | 广告购买按钮 |
| `BuyBtn` | `UIButton` | `public` | 金币购买按钮 |
| `Price` | `UITextmesh` | `public` | 价格文本 |
| `clothId` | `int` | `private` | 装备 ID |

---

## 方法说明（按重要程度排序）

### OnCreate()

**签名**:
```csharp
public void OnCreate()
```

**职责**: 初始化组件

**核心逻辑**:
```
1. 添加 ClothItem 装备显示项
2. 添加 AdBtn 广告按钮
3. 添加 BuyBtn 购买按钮
4. 添加 Price 价格文本
```

**调用者**: 父视图通过 AddComponent 调用

---

### SetData(int id, Action<int, int> onClickItem)

**签名**:
```csharp
public void SetData(int id, Action<int, int> onClickItem)
```

**职责**: 设置商品数据

**核心逻辑**:
```
1. 保存 clothId
2. 获取装备配置 ClothConfigCategory.Instance.Get(id)
3. 设置 ClothItem 数据
4. 根据获取方式显示按钮:
   - GetWay=0(广告) 且平台支持广告 → 显示 AdBtn
   - GetWay=1(金币) 或平台不支持广告 → 显示 BuyBtn
5. 绑定按钮点击事件
6. 设置价格文本 (国际化货币格式)
```

**调用者**: UIShopWin.RefreshList, UIBuyWin.RefreshView

**被调用者**: `ClothConfigCategory.Instance.Get()`, `AdManager.Instance.PlatformHasAD()`, `I18NManager.Instance.TranslateMoneyToStr()`

---

### OnClickAdBtn()

**签名**:
```csharp
public void OnClickAdBtn()
```

**职责**: 处理广告购买按钮点击

**核心逻辑**:
```
1. 检查是否已拥有
   - 是 → 显示已拥有提示
2. 如果平台支持广告 → 播放广告 PlayAdAsync()
```

**调用者**: AdBtn 点击

**被调用者**: `AdManager.Instance.PlatformHasAD()`, `PlayAdAsync()`

---

### PlayAdAsync()

**签名**:
```csharp
private async ETTask PlayAdAsync()
```

**职责**: 异步处理广告观看

**核心逻辑**:
```
1. 禁用广告按钮 (防止重复点击)
2. 调用 AdManager.Instance.PlayAd()
3. 如果广告完成:
   - 调用 PlayerDataManager.ChangeItem() 增加物品
   - 显示购买成功提示
4. 异常处理: 记录日志
5. 最终恢复广告按钮可用
```

**调用者**: OnClickAdBtn

**被调用者**: `AdManager.PlayAd()`, `PlayerDataManager.ChangeItem()`, `UIManager.OpenBox<UIToast>()`

---

### OnClickBuyBtn()

**签名**:
```csharp
public void OnClickBuyBtn()
```

**职责**: 处理金币购买按钮点击

**核心逻辑**:
```
1. 检查是否已拥有
   - 是 → 显示已拥有提示
2. 获取装备配置
3. 检查金币是否足够
   - 不足 → 显示金币不足提示
4. 如果足够:
   - 扣除金币
   - 增加物品
   - 显示购买成功提示
```

**调用者**: BuyBtn 点击

**被调用者**: `PlayerDataManager.TotalMoney`, `PlayerDataManager.ChangeMoney()`, `PlayerDataManager.ChangeItem()`

---

## 购买流程

```mermaid
sequenceDiagram
    participant User as 用户
    participant ShopItem as ShopItem
    PlayerDM as PlayerDataManager
    AdMgr as AdManager
    Toast as UIToast

    User->>ShopItem: 点击商品
    ShopItem->>User: 显示详情 (可选)
    
    alt 广告购买
        User->>ShopItem: 点击广告按钮
        ShopItem->>AdMgr: PlayAd()
        AdMgr-->>ShopItem: 完成
        ShopItem->>PlayerDM: ChangeItem(id, 1)
        ShopItem->>Toast: 购买成功
    else 金币购买
        User->>ShopItem: 点击购买按钮
        ShopItem->>PlayerDM: 检查金币
        alt 金币足够
            ShopItem->>PlayerDM: ChangeMoney(-price)
            ShopItem->>PlayerDM: ChangeItem(id, 1)
            ShopItem->>Toast: 购买成功
        else 金币不足
            ShopItem->>Toast: 金币不足
        end
    end
```

---

## 使用示例

### 在 UIShopWin 中使用

```csharp
public class UIShopWin : UIBaseContainer, IOnCreate<Action<int,int>>
{
    public ShopItem[] ShopItems;
    
    public void OnCreate(Action<int,int> clickItem)
    {
        ShopItems = new ShopItem[PlayerDataManager.Instance.ClothRefreshCount];
        for (int i = 0; i < ShopItems.Length; i++)
        {
            ShopItems[i] = AddComponent<ShopItem>("Content/ShopItem" + i);
        }
    }
    
    private async ETTask RefreshList()
    {
        var list = PlayerDataManager.Instance.GetClothsSale();
        for (int i = 0; i < ShopItems.Length; i++)
        {
            if (i < list.Count) 
                ShopItems[i].SetData(list[i], clickItem);
            // ...
        }
    }
}
```

### 在 UIBuyWin 中使用

```csharp
public class UIBuyWin : UIBaseView, IOnEnable<List<int>>
{
    public ShopItem[] ClothItem;
    
    public void RefreshView()
    {
        for (int i = 0; i < ClothItem.Length; i++)
        {
            if (i < items.Count)
            {
                ClothItem[i].SetData(items[i], null);
                ClothItem[i].SetActive(true);
            }
            else
            {
                ClothItem[i].SetActive(false);
            }
        }
    }
}
```

---

## 相关文档

- [UIShopWin.cs.md](./UIShopWin.cs.md) - 商店窗口
- [UIBuyWin.cs.md](./UIBuyWin.cs.md) - 购买窗口
- [ClothItem.cs.md](./ClothItem.cs.md) - 装备显示项
- [AdManager.cs.md](../../Module/Ad/AdManager.cs.md) - 广告管理
- [PlayerDataManager.cs.md](../../DataManager/PlayerDataManager.cs.md) - 玩家数据管理

---

*文档生成时间：2026-03-02 | OpenClaw AI 助手*
