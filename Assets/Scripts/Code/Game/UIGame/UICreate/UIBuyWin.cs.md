# UIBuyWin.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | UIBuyWin.cs |
| **路径** | Assets/Scripts/Code/Game/UIGame/UICreate/UIBuyWin.cs |
| **所属模块** | 游戏层 → Code/Game/UIGame/UICreate |
| **文件职责** | 购买窗口，处理未付费装备的批量购买，支持广告解锁和金币购买 |

---

## 类/结构体说明

### UIBuyWin

| 属性 | 说明 |
|------|------|
| **职责** | 显示需要购买的装备列表，支持通过观看广告或支付金币购买 |
| **泛型参数** | 无 |
| **继承关系** | `UIBaseView` |
| **实现的接口** | `IOnCreate`, `IOnEnable<List<int>>`, `IOnDisable` |

**设计模式**: MVC 视图层 + 消息订阅

```csharp
// 通过 UIManager 打开
UIManager.Instance.OpenWindow<UIBuyWin, List<int>>(UIBuyWin.PrefabPath, noPayItems);
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `PrefabPath` | `string` | `public static` | 预制体路径 "UIGame/UICreate/Prefabs/UIBuyWin.prefab" |
| `Close` | `UIButton` | `public` | 关闭按钮 |
| `ClothItem` | `ShopItem[]` | `public` | 6 个商品展示项 |
| `Ad` | `UIButton` | `public` | 观看广告按钮 |
| `Buy` | `UIButton` | `public` | 金币购买按钮 |
| `Value` | `UITextmesh` | `public` | 总价显示 |
| `Progress` | `UITextmesh` | `public` | 广告进度显示 (0/X) |
| `Animator` | `UIAnimator` | `public` | 窗口动画控制器 |
| `totalPrice` | `BigNumber` | `private` | 总价格 |
| `adCount` | `int` | `private` | 需要看广告的物品数量 |
| `items` | `List<int>` | `private` | 待购买物品 ID 列表 |

---

## 方法说明（按重要程度排序）

### OnCreate()

**签名**:
```csharp
public void OnCreate()
```

**职责**: 初始化购买窗口组件

**核心逻辑**:
```
1. 添加 Animator 动画组件
2. 添加 Close 关闭按钮
3. 初始化 6 个 ShopItem 商品项
4. 添加 Ad 广告按钮
5. 添加 Progress 进度文本
6. 添加 Buy 购买按钮
7. 添加 Value 价格文本
```

**调用者**: UIManager 创建窗口时

---

### OnEnable(List<int> items)

**签名**:
```csharp
public void OnEnable(List<int> items)
```

**职责**: 启用窗口时设置数据

**核心逻辑**:
```
1. 播放窗口打开音效
2. 保存待购买物品列表
3. 刷新视图 RefreshView()
4. 绑定 Close 关闭按钮事件
5. 订阅 MessageId.ChangeItem 消息 (物品变化时刷新)
```

**调用者**: UIManager 打开窗口时

**被调用者**: `RefreshView()`, `Messager.Instance.AddListener()`

---

### OnDisable()

**签名**:
```csharp
public void OnDisable()
```

**职责**: 禁用窗口时清理

**核心逻辑**:
```
1. 取消订阅 MessageId.ChangeItem 消息
```

**调用者**: UIManager 关闭窗口时

---

### RefreshView()

**签名**:
```csharp
public void RefreshView()
```

**职责**: 刷新购买视图

**核心逻辑**:
```
1. 重置 totalPrice = 0, adCount = 0
2. 遍历 items 列表，移除已拥有的物品
3. 遍历 ClothItem 数组 (最多 6 个):
   - 如果 i < items.Count → 设置 ShopItem 数据并显示
   - 获取装备配置，统计广告物品和付费物品数量
   - 否则 → 隐藏 ShopItem
4. 根据 totalPrice 和 adCount 显示/隐藏购买按钮
5. 绑定 Ad 和 Buy 按钮事件
6. 设置总价文本
7. 设置广告进度文本 (0/adCount)
```

**调用者**: OnEnable, OnClickAdAsync

**被调用者**: `PlayerDataManager.Instance.GetItemCount()`, `ClothConfigCategory.Instance.Get()`

---

### OnClickAd()

**签名**:
```csharp
public void OnClickAd()
```

**职责**: 处理广告按钮点击

**调用者**: Ad 按钮点击

**被调用者**: `OnClickAdAsync()`

---

### OnClickAdAsync()

**签名**:
```csharp
private async ETTask OnClickAdAsync()
```

**职责**: 异步处理广告观看

**核心逻辑**:
```
1. 调用 AdManager.Instance.PlayAd() 播放广告
2. 如果广告完成:
   - 遍历 items 列表
   - 找到第一个 GetWay=0(广告) 且未拥有的物品
   - 调用 PlayerDataManager.ChangeItem() 增加物品
   - 从 items 列表移除
   - 如果 items 为空 → 关闭窗口
   - 否则 → 刷新视图
3. 异常处理: 记录错误日志
```

**调用者**: OnClickAd

**被调用者**: `AdManager.PlayAd()`, `PlayerDataManager.ChangeItem()`, `RefreshView()`

---

### OnClickBuy()

**签名**:
```csharp
public void OnClickBuy()
```

**职责**: 处理金币购买

**核心逻辑**:
```
1. 检查玩家金币是否足够 (PlayerDataManager.TotalMoney >= totalPrice)
2. 如果足够:
   - 扣除金币 PlayerDataManager.ChangeMoney(-totalPrice)
   - 遍历 items，给 GetWay=1(金币) 的物品增加数量
   - 关闭窗口
3. 如果不足:
   - 显示金币不足提示 UIToast
```

**调用者**: Buy 按钮点击

**被调用者**: `PlayerDataManager.ChangeMoney()`, `PlayerDataManager.ChangeItem()`, `UIManager.OpenBox<UIToast>()`

---

### OnClickClose()

**签名**:
```csharp
public void OnClickClose()
```

**职责**: 关闭窗口

**调用者**: Close 按钮点击

**被调用者**: `CloseSelf()`

---

### CloseSelf()

**签名**:
```csharp
public override async ETTask CloseSelf()
```

**职责**: 关闭窗口 (带动画)

**核心逻辑**:
```
1. 播放窗口关闭音效
2. 播放关闭动画 "UIWin_Close"
3. 调用基类 CloseSelf()
```

**调用者**: OnClickClose, OnClickAdAsync, OnClickBuy

---

## 购买流程

```mermaid
sequenceDiagram
    participant User as 用户
    participant BuyWin as UIBuyWin
    AdMgr as AdManager
    PlayerDM as PlayerDataManager
    Toast as UIToast

    User->>BuyWin: 点击保存 (有未付费物品)
    BuyWin->>BuyWin: RefreshView()
    
    alt 有广告物品
        User->>BuyWin: 点击观看广告
        BuyWin->>AdMgr: PlayAd()
        AdMgr-->>BuyWin: 广告完成
        BuyWin->>PlayerDM: ChangeItem(广告物品)
        BuyWin->>BuyWin: RefreshView()
        alt 还有物品
            BuyWin->>User: 继续购买
        else 全部完成
            BuyWin->>BuyWin: CloseSelf()
        end
    end
    
    alt 有付费物品
        User->>BuyWin: 点击购买
        BuyWin->>PlayerDM: 检查金币
        alt 金币足够
            BuyWin->>PlayerDM: ChangeMoney(-totalPrice)
            BuyWin->>PlayerDM: ChangeItem(付费物品)
            BuyWin->>BuyWin: CloseSelf()
        else 金币不足
            BuyWin->>Toast: 显示金币不足
        end
    end
```

---

## 使用示例

### 打开购买窗口

```csharp
// 检查未付费物品
List<int> noPay = new List<int>();
for (int i = 0; i < player.SubModule.Length; i++)
{
    if (player.SubModule[i] != PlayerDataManager.Instance.Show[i])
    {
        if (PlayerDataManager.Instance.GetItemCount(player.SubModule[i]) <= 0)
        {
            noPay.Add(player.SubModule[i]);
        }
    }
}

// 打开购买窗口
if (noPay.Count > 0)
{
    UIManager.Instance.OpenWindow<UIBuyWin, List<int>>(
        UIBuyWin.PrefabPath, noPay).Coroutine();
}
```

---

## 相关文档

- [UICreateView.cs.md](./UICreateView.cs.md) - 调用方
- [ShopItem.cs.md](./ShopItem.cs.md) - 商品项
- [AdManager.cs.md](../../Module/Ad/AdManager.cs.md) - 广告管理
- [PlayerDataManager.cs.md](../../DataManager/PlayerDataManager.cs.md) - 玩家数据管理
- [UIToast.cs.md](../UICommon/UIToast.cs.md) - 提示框

---

*文档生成时间：2026-03-02 | OpenClaw AI 助手*
