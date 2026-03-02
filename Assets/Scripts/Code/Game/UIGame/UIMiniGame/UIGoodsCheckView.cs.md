# UIGoodsCheckView.cs - 验货小游戏视图

## 📄 文件信息

| 属性 | 值 |
|------|------|
| 文件路径 | `Assets/Scripts/Code/Game/UIGame/UIMiniGame/UIGoodsCheckView.cs` |
| 命名空间 | `TaoTie` |
| 基类 | `UICommonMiniGameView` |

---

## 🎯 类说明

`UIGoodsCheckView` 是验货小游戏视图，玩家通过验货可以查看物品的真实品质或隐藏属性。

### 核心职责

- **验货展示**: 显示待验货的物品信息
- **验货过程**: 播放验货动画
- **结果展示**: 显示验货结果（品质/属性）

---

## 🔧 方法说明

### 主要方法

#### `OnCreate()`
初始化验货界面 UI 组件。

#### `OnEnable(int id)`
启用验货界面并设置物品数据。

#### `OnClickCheck()`
开始验货。

---

## 💡 使用示例

```csharp
// 打开验货视图
UIManager.Instance.OpenWindow<UIGoodsCheckView, int>(
    UIGoodsCheckView.PrefabPath,
    itemId
);
```

---

## 🔗 相关文档

- [UICommonMiniGameView.cs.md](./UICommonMiniGameView.cs.md) - 小游戏基类
- [GoodsCheckConfig.cs.md](../../../../Module/Generate/Config/GoodsCheckConfig.cs.md) - 验货配置表

---

*最后更新：2026-03-02*
