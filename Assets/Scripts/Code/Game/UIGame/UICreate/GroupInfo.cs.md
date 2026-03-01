# GroupInfo.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | GroupInfo.cs |
| **路径** | Assets/Scripts/Code/Game/UIGame/UICreate/GroupInfo.cs |
| **所属模块** | 游戏层 → UIGame/UICreate |
| **文件职责** | 套装信息 UI 容器，显示套装图标、数量和效果详情 |

---

## 类说明

### GroupInfo

| 属性 | 说明 |
|------|------|
| **职责** | 显示套装 (Group) 的图标、激活数量、效果描述 |
| **继承关系** | 继承 `UIBaseContainer` |
| **实现的接口** | `IOnCreate` |

**使用场景**: 角色创建界面的套装效果展示

---

## 字段与属性

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `Count` | `UITextmesh` | `public` | 激活数量文本 |
| `Icon` | `UIImage` | `public` | 套装图标 |
| `Details` | `UITextmesh` | `public` | 效果详情文本 |

---

## 方法说明

### OnCreate()

**签名**:
```csharp
public void OnCreate()
```

**职责**: 创建时初始化 UI 组件

**核心逻辑**:
```
1. 获取 Icon 组件
2. 获取 Count 组件 (Icon 的子对象)
3. 获取 Details 组件
```

**调用者**: UIManager 创建容器时

---

### SetData()

**签名**:
```csharp
public void SetData(int index, int count, int effect, int param, bool active)
```

**职责**: 设置套装数据

**参数**:
- `index`: 套装索引（用于图标路径）
- `count`: 激活数量（已装备的套装件数）
- `effect`: 效果类型 ID
- `param`: 效果参数值
- `active`: 是否激活（达到激活条件）

**核心逻辑**:
```
1. 设置图标路径: $"UIGame/UICreate/Atlas/group{index}.png"
2. 设置灰度状态：!active (未激活时灰色)
3. 设置数量文本：count.ToString()
4. 设置效果文本:
   - I18NKey = "Text_Equip_Effect_{effect}_Short"
   - 参数化显示 param 值
5. 设置文本颜色:
   - active=true → 白色 (GameConst.WHITE_COLOR)
   - active=false → 灰色 (GameConst.GRAY_COLOR)
```

**调用者**: UICreateView 更新套装信息时

---

## 使用示例

### 示例 1: 设置套装数据

```csharp
// 获取 GroupInfo 容器
var groupInfo = uiView.GetContainer<GroupInfo>("Group1");

// 设置数据
groupInfo.SetData(
    index: 1,           // 套装 1
    count: 2,           // 已装备 2 件
    effect: 3,          // 效果类型 3
    param: 15,          // 效果值 15
    active: true        // 已激活
);

// 显示效果：
// - 图标：group1.png (彩色)
// - 数量：2
// - 文本："+15 攻击力" (白色)
```

### 示例 2: 未激活状态

```csharp
// 未达到激活条件
groupInfo.SetData(1, 1, 3, 15, false);

// 显示效果：
// - 图标：group1.png (灰色)
// - 数量：1
// - 文本："+15 攻击力" (灰色)
```

### 示例 3: 批量更新套装

```csharp
// 更新所有套装信息
var groups = new[] { group1, group2, group3 };
var套装配置 = new[]
{
    (index: 1, count: 2, effect: 3, param: 15, active: true),
    (index: 2, count: 1, effect: 5, param: 10, active: false),
    (index: 3, count: 3, effect: 7, param: 20, active: true),
};

for (int i = 0; i < groups.Length; i++)
{
    var config = 套装配置 [i];
    groups[i].SetData(config.index, config.count, config.effect, config.param, config.active);
}
```

---

## UI 结构

```
GroupInfo (Container)
├── Icon (UIImage)
│   └── Count (UITextmesh) → "2"
└── Details (UITextmesh) → "+15 攻击力"
```

---

## 设计说明

### 国际化文本

```csharp
// I18NKey 格式："Text_Equip_Effect_{effect}_Short"
// 示例:
// effect=3 → "Text_Equip_Effect_3_Short" → "+{0} 攻击力"
// effect=5 → "Text_Equip_Effect_5_Short" → "+{0}% 暴击率"

// 参数化显示
Details.SetI18NKey(I18NKey.Text_Equip_Effect_3_Short, 15);
// 显示："+15 攻击力"
```

### 视觉反馈

| 状态 | 图标 | 文本颜色 |
|------|------|----------|
| 激活 (active=true) | 彩色 | 白色 |
| 未激活 (active=false) | 灰色 | 灰色 |

---

## 相关文档

- [UIBaseContainer.cs.md](../../../UI/UIBaseContainer.cs.md) - UI 容器基类
- [UICreateView.cs.md](../UICreateView.cs.md) - 角色创建界面
- [GroupInfoTable.cs.md](./GroupInfoTable.cs.md) - 套装信息表

---

*文档生成时间：2026-03-02 | OpenClaw AI 助手*
