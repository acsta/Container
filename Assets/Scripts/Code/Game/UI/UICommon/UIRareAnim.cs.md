# UIRareAnim.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | UIRareAnim.cs |
| **路径** | Assets/Scripts/Code/Game/UI/UICommon/UIRareAnim.cs |
| **所属模块** | 玩法层 → UI 通用组件 |
| **文件职责** | 稀有度动画背景容器，用于显示不同稀有度的颜色效果 |

---

## 类/结构体说明

### UIRareAnim

| 属性 | 说明 |
|------|------|
| **职责** | 稀有度动画背景容器视图类 |
| **泛型参数** | 无 |
| **继承关系** | 继承 `UIBaseContainer` |
| **实现的接口** | `IOnCreate` |

**设计模式**: 简单容器模式

```csharp
// 使用示例
var rareAnim = container.AddComponent<UIRareAnim>();
rareAnim.SetColor("#FFD700"); // 设置金色稀有度
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `bg` | `UIImage` | `private` | 背景图片组件（外层） |
| `inner` | `UIImage` | `private` | 内层图片组件（Bg 子节点） |

---

## 方法说明（按重要程度排序）

### OnCreate()

**签名**:
```csharp
public void OnCreate()
```

**职责**: 初始化容器组件引用

**核心逻辑**:
```
1. 创建 bg 背景图片组件（根节点）
2. 创建 inner 内层图片组件（"Bg" 子节点）
```

**调用者**: UIManager（组件创建时自动调用）

---

### SetColor(string colorStr)

**签名**:
```csharp
public void SetColor(string colorStr)
```

**职责**: 设置稀有度颜色

**核心逻辑**:
```
1. 设置 bg 背景颜色为 colorStr
2. 设置 inner 内层颜色为 colorStr
```

**参数说明**:
- `colorStr`: 颜色字符串（如 "#FFD700" 金色、"#C0C0C0" 银色等）

**调用者**: 需要显示稀有度效果的 UI

---

## 使用场景

### 1. 物品稀有度显示
```csharp
// 在物品展示容器中
var rareAnim = container.AddComponent<UIRareAnim>();
switch (item.Rarity)
{
    case Rarity.SSR:
        rareAnim.SetColor("#FFD700"); // 金色
        break;
    case Rarity.SR:
        rareAnim.SetColor("#C0C0C0"); // 银色
        break;
    case Rarity.R:
        rareAnim.SetColor("#CD7F32"); // 青铜
        break;
}
```

### 2. 卡牌稀有度背景
```csharp
// 在卡牌 UI 中
rareAnim.SetColor(cardData.RarityColor);
```

---

## UI 结构

```
UIRareAnim (UIBaseContainer)
└── Bg (UIImage) - inner 内层
    └── (根节点 bg)
```

**说明**: 双层图片结构，可以同时设置颜色实现渐变或边框效果

---

## 颜色规范建议

| 稀有度 | 颜色代码 | 说明 |
|--------|----------|------|
| SSR/传说 | `#FFD700` | 金色 |
| SR/史诗 | `#C0C0C0` | 银色 |
| R/稀有 | `#CD7F32` | 青铜 |
| N/普通 | `#808080` | 灰色 |

---

## 相关文档

- [UIBaseContainer.cs.md](../../Module/UI/UIBaseContainer.cs.md) - UI 容器基类
- [UIImage.cs.md](../../Module/UIComponent/UIImage.cs.md) - 图片组件

---

*文档生成时间：2026-03-01 | OpenClaw AI 助手*
