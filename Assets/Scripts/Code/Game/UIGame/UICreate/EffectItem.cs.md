# EffectItem.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | EffectItem.cs |
| **路径** | Assets/Scripts/Code/Game/UIGame/UICreate/EffectItem.cs |
| **所属模块** | 游戏层 → UIGame/UICreate |
| **文件职责** | 装备效果 UI 项，显示套装激活条件和效果描述 |

---

## 类说明

### EffectItem

| 属性 | 说明 |
|------|------|
| **职责** | 显示套装效果项，包括激活数量要求和效果描述 |
| **继承关系** | 继承 `UIBaseContainer` |
| **实现的接口** | `IOnCreate` |

**使用场景**: 角色创建界面显示装备套装效果列表

---

## 字段与属性

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `Type` | `UITextmesh` | `public` | 激活数量文本（如"2 件套"） |
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
1. 获取 Type 组件
2. 获取 Details 组件
3. 设置 Type 的国际化文本为"套装数量" (Text_Equip_Group_Count)
```

**调用者**: UIManager 创建容器时

---

### SetData()

**签名**:
```csharp
public void SetData(int effectType, int param, int count, bool active)
```

**职责**: 设置效果数据

**参数**:
- `effectType`: 效果类型 ID
- `param`: 效果参数值
- `count`: 激活所需数量（如 2 件套）
- `active`: 是否已激活

**核心逻辑**:
```
1. 设置 Type 文本：count (如"2")
2. 设置 Details 文本:
   - I18NKey = "Text_Equip_Effect_{effectType}"
   - 参数化显示 param 值
3. 异步调整高度 SetDataAsync()
4. 设置文本颜色:
   - active=true → 绿色 (GameConst.GREEN_COLOR)
   - active=false → 灰色 (GameConst.GRAY_COLOR)
```

**调用者**: UICreateView 更新效果列表时

---

### SetDataAsync()

**签名**:
```csharp
private async ETTask SetDataAsync()
```

**职责**: 异步调整容器高度以适应文本内容

**核心逻辑**:
```
1. 等待一帧完成 UnityLifeTimeHelper.WaitFrameFinish()
   (确保文本已渲染，可以获取行数)
2. 获取文本行数 line = Details.GetLineCount()
3. 获取当前宽度 x
4. 设置高度 = 30 * line (每行 30 像素)
```

**调用者**: SetData()

**设计说明**: 
- 异步等待确保文本布局完成
- 动态高度适应多行文本

---

## 使用示例

### 示例 1: 设置效果数据

```csharp
// 获取 EffectItem 容器
var effectItem = uiView.GetContainer<EffectItem>("Effect1");

// 设置数据：2 件套 +15 攻击力，未激活
effectItem.SetData(
    effectType: 3,    // 效果类型 3 (攻击力)
    param: 15,        // 效果值 15
    count: 2,         // 2 件套
    active: false     // 未激活
);

// 显示效果：
// Type: "2" (灰色)
// Details: "+15 攻击力" (灰色)
```

### 示例 2: 已激活状态

```csharp
// 已激活
effectItem.SetData(3, 15, 2, true);

// 显示效果：
// Type: "2" (绿色)
// Details: "+15 攻击力" (绿色)
```

### 示例 3: 批量更新效果列表

```csharp
// 更新所有效果项
var effects = new[] { effect1, effect2, effect3 };
var 效果配置 = new[]
{
    (type: 3, param: 15, count: 2, active: true),   // 2 件套激活
    (type: 5, param: 10, count: 4, active: false),  // 4 件套未激活
    (type: 7, param: 20, count: 6, active: false),  // 6 件套未激活
};

for (int i = 0; i < effects.Length; i++)
{
    var config = 效果配置 [i];
    effects[i].SetData(config.type, config.param, config.count, config.active);
}
```

---

## UI 结构

```
EffectItem (Container)
├── Type (UITextmesh) → "2"
└── Details (UITextmesh) → "+15 攻击力"
```

### 动态高度

```
单行文本: 高度 = 30px
双行文本: 高度 = 60px
三行文本: 高度 = 90px
...
```

---

## 设计说明

### 国际化文本

```csharp
// I18NKey 格式："Text_Equip_Effect_{effectType}"
// 示例:
// effectType=3 → "Text_Equip_Effect_3" → "+{0} 攻击力"
// effectType=5 → "Text_Equip_Effect_5" → "+{0}% 暴击率"

// 参数化显示
Details.SetI18NKey(I18NKey.Text_Equip_Effect_3, 15);
// 显示："+15 攻击力"
```

### 视觉反馈

| 状态 | Type 颜色 | Details 颜色 |
|------|----------|-------------|
| 激活 (active=true) | 绿色 | 绿色 |
| 未激活 (active=false) | 灰色 | 灰色 |

### 异步高度调整

```csharp
// 为什么需要异步？
// 1. 文本设置后需要等待 Unity 布局系统计算
// 2. GetLineCount() 需要在渲染后才能获取准确值
// 3. 使用 WaitFrameFinish() 等待下一帧

await UnityLifeTimeHelper.WaitFrameFinish();
int line = Details.GetLineCount();  // 现在可以获取准确行数
```

---

## 相关文档

- [UIBaseContainer.cs.md](../../../UI/UIBaseContainer.cs.md) - UI 容器基类
- [UICreateView.cs.md](../UICreateView.cs.md) - 角色创建界面
- [GroupInfo.cs.md](./GroupInfo.cs.md) - 套装信息
- [UnityLifeTimeHelper.cs.md](../../../../Mono/Helper/UnityLifeTimeHelper.cs.md) - Unity 生命周期助手

---

*文档生成时间：2026-03-02 | OpenClaw AI 助手*
