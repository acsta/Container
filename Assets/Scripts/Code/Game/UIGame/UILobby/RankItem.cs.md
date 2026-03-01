# RankItem.cs - 排行榜项组件

## 📄 文件信息

| 属性 | 值 |
|------|------|
| 文件路径 | `Assets/Scripts/Code/Game/UIGame/UILobby/RankItem.cs` |
| 命名空间 | `TaoTie` |
| 基类 | `UIBaseContainer` |
| 实现接口 | `IOnCreate` |

---

## 🎯 类说明

`RankItem` 是排行榜列表项组件，用于展示单个玩家的排名信息，包括排名旗帜、头像、昵称和财富值。支持前三名特殊样式和自己排名的特殊显示。

### 核心职责

- **排名展示**: 显示玩家排名（1-3 名特殊旗帜，其他显示数字）
- **头像显示**: 支持在线头像 URL 加载
- **财富展示**: 显示玩家财富值
- **样式区分**: 前三名、自己、普通玩家采用不同样式

---

## 📋 字段说明

### UI 组件字段

| 字段名 | 类型 | 说明 |
|--------|------|------|
| `Bg0` | `UIImage` | 第 1 名背景高亮 |
| `Bg1` | `UIImage` | 第 2 名背景高亮 |
| `Bg2` | `UIImage` | 第 3 名背景高亮 |
| `Flag0` | `UIImage` | 第 1 名旗帜图标 |
| `Flag1` | `UIImage` | 第 2 名旗帜图标 |
| `Flag2` | `UIImage` | 第 3 名旗帜图标 |
| `Flag3` | `UITextmesh` | 第 4 名及以后排名数字 |
| `Icon` | `UIRawImage` | 玩家头像 |
| `Bg` | `UIImage` | 普通背景（可交互） |
| `Name` | `UITextmesh` | 玩家昵称 |
| `Value` | `UITextmesh` | 财富值 |
| `IconNone` | `UIImage` | 无头像时的默认图标 |

---

## 🔧 方法说明

### 生命周期方法

#### `OnCreate()`
初始化所有 UI 组件。

```csharp
public void OnCreate()
{
    IconNone = AddComponent<UIImage>("IconNone");
    Bg = AddComponent<UIImage>();
    Bg0 = AddComponent<UIImage>("Bg0");
    Bg1 = AddComponent<UIImage>("Bg1");
    Bg2 = AddComponent<UIImage>("Bg2");
    Flag0 = AddComponent<UIImage>("Flag0");
    Flag1 = AddComponent<UIImage>("Flag1");
    Flag2 = AddComponent<UIImage>("Flag2");
    Flag3 = AddComponent<UITextmesh>("Flag3");
    Icon = AddComponent<UIRawImage>("Icon");
    Name = AddComponent<UITextmesh>("Name");
    Value = AddComponent<UITextmesh>("Value");
}
```

---

### 业务方法

#### `SetData(int index, bool isMe, RankInfo info)`
设置排行榜项数据。

**参数说明:**
- `index`: 排名索引（0-based）
- `isMe`: 是否是自己
- `info`: 排名信息数据

**处理流程:**
1. 设置背景启用状态（自己或第 4 名以后可交互）
2. 根据排名显示对应旗帜（1-3 名）或数字（4 名以后）
3. 设置昵称（自己显示"我"，其他显示实际昵称）
4. 根据排名设置文本颜色（前三名金色，其他普通色）
5. 设置财富值
6. 设置排名数字（99 名以后显示"99+"）
7. 加载头像（优先在线头像，失败显示默认图标）

**颜色规则:**
- 第 1-3 名：`#7F4500`（金色）
- 普通玩家：`GameConst.COMMON_TEXT_COLOR`
- 自己：`GameConst.WHITE_COLOR`（白色）

---

## 🔄 流程图

```mermaid
flowchart TD
    A[设置数据 SetData] --> B{是自己？}
    B -->|是 | C[启用背景]
    B -->|否 | D{排名<3?}
    D -->|是 | C
    D -->|否 | E[启用背景]
    C --> F[设置旗帜/数字显示]
    F --> G{排名=0?}
    G -->|是 | H[显示 Flag0]
    G -->|否 | I{排名=1?}
    I -->|是 | J[显示 Flag1]
    I -->|否 | K{排名=2?}
    K -->|是 | L[显示 Flag2]
    K -->|否 | M[显示 Flag3 数字]
    M --> N[设置昵称]
    N --> O{是自己？}
    O -->|是 | P[显示"我"]
    O -->|否 | Q[显示实际昵称]
    Q --> R[设置颜色]
    R --> S[设置财富值]
    S --> T[设置排名数字]
    T --> U{有头像 URL?}
    U -->|是 | V[加载在线头像]
    U -->|否 | W[显示默认图标]
    V --> X[完成]
    W --> X
```

---

## 💡 使用示例

### 在排行榜列表中创建排名项

```csharp
// UIRankView 中的列表初始化
public void OnCreate()
{
    ScrollView = AddComponent<UILoopListView2>("UICommonView/Bg/Content/ScrollView");
    ScrollView.InitListView(0, GetScrollViewItemByIndex);
    RankItem = AddComponent<RankItem>("UICommonView/Bg/Content/RankItem");
}

// 列表项创建回调
public LoopListViewItem2 GetScrollViewItemByIndex(LoopListView2 listView, int index)
{
    if (index < 0) return null;
    
    LoopListViewItem2 item = listView.NewListViewItem("RankItem", index);
    if (!item.IsInitHandlerCalled)
    {
        ScrollView.AddItemViewComponent<RankItem>(item);
        item.IsInitHandlerCalled = true;
    }

    var rankItem = ScrollView.GetUIItemView<RankItem>(item);
    RankInfo data = null;
    if (list != null && index < list.Length)
    {
        data = list[index];
    }
    
    rankItem.SetData(index, false, data);
    
    // 设置项宽度为列表宽度
    var y = rankItem.GetRectTransform().sizeDelta.y;
    var x = ScrollView.GetRectTransform().rect.width;
    rankItem.GetRectTransform().sizeDelta = new Vector2(x, y);
    
    return item;
}
```

### 设置自己排名项

```csharp
// 在 UIRankView.OnEnable 中设置底部自己排名
public void OnEnable(RankList data)
{
    // ... 其他初始化 ...
    
    // 设置底部自己排名项（固定显示，不在滚动列表中）
    RankItem.SetData((data?.my ?? 101) - 1, true, null);
}
```

### 排名数据结构

```csharp
// RankInfo 数据结构示例
public class RankInfo
{
    public long uid;         // 用户 ID
    public string NickName;  // 昵称
    public string Avatar;    // 头像 URL
    public long Money;       // 财富值
    public long RankValue;   // 排名值
}
```

---

## 🔗 相关文档

- [UIRankView.cs.md](./UIRankView.cs.md) - 排行榜界面
- [UIRawImage.cs.md](../../../UIComponent/UIRawImage.cs.md) - RawImage UI 组件
- [UITextmesh.cs.md](../../../UIComponent/UITextmesh.cs.md) - TextMesh UI 组件
- [UIImage.cs.md](../../../UIComponent/UIImage.cs.md) - Image UI 组件

---

*最后更新：2026-03-02*
