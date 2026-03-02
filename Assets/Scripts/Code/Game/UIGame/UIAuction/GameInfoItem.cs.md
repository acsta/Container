# GameInfoItem.cs - 游戏情报项组件

## 📄 文件信息

| 属性 | 值 |
|------|------|
| **文件路径** | `Assets/Scripts/Code/Game/UIGame/UIAuction/GameInfoItem.cs` |
| **命名空间** | `TaoTie` |
| **基类** | `UIBaseContainer` |
| **实现接口** | `IOnCreate` |

---

## 🎯 类说明

`GameInfoItem` 是游戏情报系统的列表项组件，用于在情报选择界面中显示单条情报的详细信息，包括情报描述、效果、目标物品列表等。

### 核心职责

- **情报展示**: 显示情报的描述文本和效果
- **目标列表**: 展示情报影响的所有物品
- **稀有度标识**: 根据情报稀有度显示不同颜色
- **点击交互**: 支持点击选择情报

---

## 📋 字段说明

### UI 组件字段

| 字段名 | 类型 | 说明 |
|--------|------|------|
| `PointerClick` | `UIPointerClick` | 点击触发器 |
| `Desc` | `UITextmesh` | 情报描述文本 |
| `Content` | `UICopyGameObject` | 目标物品列表容器 |
| `Eff` | `UITextmesh` | 效果文本（奖励数值） |
| `Light` | `UIImage` | 选中高亮效果 |
| `Mask` | `UIImage` | 未选中遮罩 |
| `Chinese` | `UITextmesh` | 中文差异文本 |
| `English` | `UITextmesh` | 英文差异文本 |

### 数据字段

| 字段名 | 类型 | 说明 |
|--------|------|------|
| `config` | `GameInfoConfig` | 当前情报配置 |
| `onClickThis` | `Action<GameInfoConfig>` | 点击回调 |
| `items` | `List<ItemConfig>` | 目标物品列表 |

---

## 🔧 方法说明

### 生命周期方法

#### `OnCreate()`
组件创建时初始化所有 UI 组件。

```csharp
public void OnCreate()
{
    Mask = AddComponent<UIImage>("Mask");
    Light = AddComponent<UIImage>("Light");
    PointerClick = AddComponent<UIPointerClick>();
    Desc = AddComponent<UITextmesh>("DescBg/Desc");
    Content = AddComponent<UICopyGameObject>("DescBg/ScrollView/Viewport/Content");
    Content.InitListView(0, GetContentItemByIndex);  // 初始化物品列表
    Eff = AddComponent<UITextmesh>("Desc/Text");
    Chinese = AddComponent<UITextmesh>("Diff/Chinese");
    English = AddComponent<UITextmesh>("Diff/English");
    PointerClick.SetOnClick(OnClickSelf);
}
```

---

### 业务方法

#### `SetData(GameInfoConfig config, Action<GameInfoConfig> onClickThis)`
设置情报数据并更新显示。

**参数说明:**
- `config`: 游戏情报配置数据
- `onClickThis`: 点击回调函数（为 null 时表示已选中）

**核心逻辑:**
```
1. 根据当前语言显示中文/英文差异文本
2. 根据 onClickThis 是否为 null 设置 Light/Mask 显示：
   - onClickThis == null: Light 显示（已选中），Mask 隐藏
   - onClickThis != null: Light 隐藏，Mask 显示（可选）
3. 保存 config 和 onClickThis
4. 清空 items 列表
5. 根据 config.Type 收集目标物品：
   - Container: 从 ContainerConfig 获取 ItemConfig
   - Items: 直接从 ItemConfig 获取
   - RandItems: 从 config.TempItems 获取
   - PlayType: 从 PlayTypeConfig 获取 ItemConfig
   - Raise: 直接从 ItemConfig 获取
6. 获取稀有度颜色：RareConfigCategory.Instance.GetRare(config.Rare).Color
7. 构建效果文本：
   - AwardType == 0: "+{RewardCount}"（加法）
   - AwardType == 1: "×{RewardCount}"（乘法）
8. 根据 Type 设置 Eff 文本：
   - Raise: "抬价时{eff}"
   - PlayType: "玩法{物品列表}时{eff}"
   - 其他: "所有{物品/容器}{eff}"
9. 刷新 Content 物品列表
```

**效果文本示例:**
```csharp
// Type == Raise（抬价类情报）
Eff: "抬价时<color=#FFD700>+500</color>"

// Type == PlayType（玩法类情报）
Eff: "玩法鉴定，验货时<color=#FFD700>×2</color>"

// Type == Items（指定物品类）
Eff: "所有青花瓷瓶<color=#FFD700>+1000</color>"

// Type == Container（指定容器类）
Eff: "所有<color=#容器颜色>容器名称</color><color=#FFD700>×1.5</color>"
```

#### `GetContentItemByIndex(int index, GameObject obj)`
获取物品列表项。

```csharp
public void GetContentItemByIndex(int index, GameObject obj)
{
    var cfg = items[index];
    if (Content.GetUIItemView<UIAuctionItem>(obj) == null)
    {
        Content.AddItemViewComponent<UIAuctionItem>(obj);
    }
    var uiitem = Content.GetUIItemView<UIAuctionItem>(obj);
    uiitem.SetData(cfg);  // 设置物品数据
}
```

---

### 事件处理方法

#### `OnClickSelf()`
点击情报项。

```csharp
public void OnClickSelf()
{
    Light.SetActive(true);   // 显示高亮
    Mask.SetActive(false);   // 隐藏遮罩
    onClickThis?.Invoke(config);  // 调用回调
}
```

---

## 📊 组件结构图

```mermaid
graph TD
    subgraph GameInfoItem["GameInfoItem"]
        PointerClick[UIPointerClick]
        Light[UIImage "Light"]
        Mask[UIImage "Mask"]
        DescBg[UIEmptyView "DescBg"]
        Desc[UITextmesh "Desc"]
        ScrollView[ScrollView]
        Content[UICopyGameObject "Content"]
        EffText[UITextmesh "Eff"]
        Diff[UIEmptyView "Diff"]
        Chinese[UITextmesh "Chinese"]
        English[UITextmesh "English"]
    end
    
    subgraph ContentList["物品列表"]
        Item1[UIAuctionItem 0]
        Item2[UIAuctionItem 1]
        Item3[UIAuctionItem 2]
    end
    
    DescBg --> Desc
    DescBg --> ScrollView
    ScrollView --> Content
    Content --> Item1
    Content --> Item2
    Content --> Item3
    
    note right of Content "动态列表<br/>显示所有目标物品"
```

---

## 💡 使用场景

### 情报选择界面

```csharp
// 在 UIGameInfoView 中使用
var item = listPool.Fetch<GameInfoItem>();
item.SetData(config, (selectedConfig) => {
    // 玩家选择了这条情报
    Light.SetActive(true);
    Mask.SetActive(false);
    selectedGameInfo = selectedConfig;
});
```

### 情报配置示例

```csharp
// GameInfoConfig
{
    "Id": 1001,
    "Rare": 3,              // 稀有度（影响颜色）
    "Type": 0,              // 0=Items, 1=Container, 2=RandItems, 3=PlayType, 4=Raise
    "Ids": [101, 102, 103], // 目标物品/容器 ID 列表
    "AwardType": 0,         // 0=加法，1=乘法
    "RewardCount": 500,     // 奖励数值
    "Desc": "鉴定时这些物品价格增加",
    "Chinese": "中文特有描述",
    "English": "English specific description"
}
```

---

## ⚠️ 注意事项

| 问题 | 说明 | 解决方案 |
|------|------|----------|
| **物品列表为空** | 配置 IDs 为空或无效 | 检查配置数据 |
| **回调为 null** | onClickThis 为 null 表示已选中 | Light 显示，Mask 隐藏 |
| **语言切换** | 中英文差异文本 | 根据 I18NManager.Instance.CurLangType 显示 |
| **稀有度颜色** | 不同稀有度不同颜色 | 从 RareConfigCategory 获取 |

---

## 🔗 相关文档

- [UIBaseContainer.cs.md](../UI/UIBaseContainer.cs.md) - UI 容器基类
- [GameInfoConfig.cs.md](../../../Module/Generate/Config/GameInfoConfig.cs.md) - 游戏情报配置
- [UIAuctionItem.cs.md](./UIAuctionItem.cs.md) - 拍卖物品项组件
- [RareConfig.cs.md](../../../Module/Generate/Config/RareConfig.cs.md) - 稀有度配置
- [I18NManager.cs.md](../../../Mono/Module/I18N/I18NManager.cs.md) - 国际化管理器

---

*文档由 OpenClaw AI 助手自动生成 | 基于静态代码分析*
