# UIItemStoryWin.cs - 物品故事窗口

## 📄 文件信息

| 属性 | 值 |
|------|------|
| 文件路径 | `Assets/Scripts/Code/Game/UIGame/UIAuction/UIItemStoryWin.cs` |
| 命名空间 | `TaoTie` |
| 基类 | `UIBaseView` |

---

## 🎯 类说明

`UIItemStoryWin` 是物品故事窗口，当玩家点击故事类物品时显示，展示该物品背后的故事文本。

### 核心职责

- **故事展示**: 显示物品的背景故事
- **逐字显示**: 支持逐字播放故事文本
- **自动关闭**: 阅读完成后自动关闭

---

## 🔧 方法说明

### 主要方法

#### `OnCreate()`
初始化故事窗口 UI 组件。

#### `OnEnable(int itemId, UIAuctionItem sourceItem)`
启用窗口并设置故事数据。

**参数说明:**
- `itemId`: 物品配置 ID
- `sourceItem`: 来源物品项（用于定位）

---

## 💡 使用示例

```csharp
// 打开物品故事窗口
UIManager.Instance.OpenBox<UIItemStoryWin, int, UIAuctionItem>(
    UIItemStoryWin.PrefabPath,
    itemId,
    sourceItem,
    UILayerNames.NormalLayer
);
```

---

## 🔗 相关文档

- [UIAuctionItem.cs.md](./UIAuctionItem.cs.md) - 物品项组件
- [ItemConfig.cs.md](../../../../Module/Generate/Config/ItemConfig.cs.md) - 物品配置表

---

*最后更新：2026-03-02*
