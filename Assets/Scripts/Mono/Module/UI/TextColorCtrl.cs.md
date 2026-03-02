# TextColorCtrl.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | TextColorCtrl.cs |
| **路径** | Assets/Scripts/Mono/Module/UI/TextColorCtrl.cs |
| **所属模块** | 框架层 → Mono/Module/UI |
| **文件职责** | UI 文本颜色控制器，支持 Text/TMP_Text 组件的颜色设置和恢复 |

---

## 类说明

### TextColorCtrl

| 属性 | 说明 |
|------|------|
| **职责** | 附加到 UI 文本元素上，管理文本、描边、阴影的颜色，支持设置和恢复原始颜色 |
| **继承关系** | `MonoBehaviour` |
| **支持组件** | `Text` (Unity UI), `TMPro.TMP_Text` (TextMesh Pro) |

**设计模式**: 状态保存 + 组件模式

```csharp
// 使用方式
// 1. 在文本 GameObject 上添加 TextColorCtrl 组件
// 2. 自动保存原始颜色
// 3. 调用 SetTextColor() 改变颜色
// 4. 调用 ClearTextColor() 恢复原始颜色
```

---

## 字段与属性

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `m_text` | `Text` | `public` | Unity UI Text 组件引用 |
| `m_text2` | `TMPro.TMP_Text` | `public` | TextMesh Pro Text 组件引用 |
| `m_originTextColor` | `Color` | `public` | 原始文本颜色（自动保存） |
| `m_outline` | `Outline` | `public` | 描边组件引用 |
| `m_originOutlineColor` | `Color` | `public` | 原始描边颜色（自动保存） |
| `m_shadow` | `Shadow` | `public` | 阴影组件引用 |
| `m_originShadowColor` | `Color` | `public` | 原始阴影颜色（自动保存） |

---

## 方法说明

### Awake()

**签名**:
```csharp
public void Awake()
```

**职责**: 初始化时自动获取组件并保存原始颜色

**核心逻辑**:
```
1. 获取 TMPro.TMP_Text 组件（优先）
2. 如果没有，获取 Unity Text 组件
3. 保存原始文本颜色
4. 获取 Outline 组件（如果有）
5. 保存原始描边颜色
6. 获取 Shadow 组件（如果有）
7. 保存原始阴影颜色
```

**调用者**: Unity 生命周期（自动调用）

---

### Get(GameObject go)

**签名**:
```csharp
public static TextColorCtrl Get(GameObject go)
```

**职责**: 静态工厂方法，获取或创建 TextColorCtrl 组件

**核心逻辑**:
```
1. 尝试获取已有组件
2. 如果没有，添加新组件
3. 返回组件实例
```

**使用示例**:
```csharp
TextColorCtrl ctrl = TextColorCtrl.Get(gameObject);
ctrl.SetTextColor(Color.red);
```

---

### SetTextColor(Color color)

**签名**:
```csharp
public void SetTextColor(Color color)
```

**职责**: 设置文本颜色

**核心逻辑**:
```
1. 如果 m_text 存在，设置 m_text.color = color
2. 如果 m_text2 存在，设置 m_text2.color = color
```

**调用者**: 任何需要改变文本颜色的代码

---

### ClearTextColor()

**签名**:
```csharp
public void ClearTextColor()
```

**职责**: 恢复文本原始颜色

**核心逻辑**:
```
1. 如果 m_text 存在，恢复 m_text.color = m_originTextColor
2. 如果 m_text2 存在，恢复 m_text2.color = m_originTextColor
```

---

### SetOutlineColor(Color color)

**签名**:
```csharp
public void SetOutlineColor(Color color)
```

**职责**: 设置描边颜色

**核心逻辑**:
```
1. 如果 m_outline 存在，设置 m_outline.effectColor = color
```

---

### ClearOutlineColor()

**签名**:
```csharp
public void ClearOutlineColor()
```

**职责**: 恢复描边原始颜色

---

### SetShadowColor(Color color)

**签名**:
```csharp
public void SetShadowColor(Color color)
```

**职责**: 设置阴影颜色

**核心逻辑**:
```
1. 如果 m_shadow 存在，设置 m_shadow.effectColor = color
```

---

### ClearShadowColor()

**签名**:
```csharp
public void ClearShadowColor()
```

**职责**: 恢复阴影原始颜色

---

## 使用示例

### 示例 1: 基础颜色切换

```csharp
public class HealthText : MonoBehaviour
{
    private TextColorCtrl colorCtrl;
    
    void Awake()
    {
        colorCtrl = TextColorCtrl.Get(gameObject);
    }
    
    public void SetHealth(float health, float maxHealth)
    {
        float percent = health / maxHealth;
        
        if (percent > 0.5f)
        {
            colorCtrl.SetTextColor(Color.green);
        }
        else if (percent > 0.25f)
        {
            colorCtrl.SetTextColor(Color.yellow);
        }
        else
        {
            colorCtrl.SetTextColor(Color.red);
        }
    }
    
    public void ResetColor()
    {
        colorCtrl.ClearTextColor();
    }
}
```

### 示例 2: 按钮交互反馈

```csharp
public class InteractiveButton : MonoBehaviour
{
    [SerializeField] private TextColorCtrl textCtrl;
    
    public void OnPointerEnter()
    {
        // 悬停时高亮
        textCtrl.SetTextColor(Color.yellow);
        textCtrl.SetOutlineColor(Color.white);
    }
    
    public void OnPointerExit()
    {
        // 恢复原始颜色
        textCtrl.ClearTextColor();
        textCtrl.ClearOutlineColor();
    }
    
    public void OnClick()
    {
        // 点击时闪烁
        StartCoroutine(BlinkEffect());
    }
    
    IEnumerator BlinkEffect()
    {
        textCtrl.SetTextColor(Color.white);
        yield return new WaitForSeconds(0.1f);
        textCtrl.ClearTextColor();
    }
}
```

### 示例 3: 任务文本状态

```csharp
public class QuestItemText : MonoBehaviour
{
    [SerializeField] private TextColorCtrl colorCtrl;
    
    public void SetQuestState(QuestState state)
    {
        switch (state)
        {
            case QuestState.NotStarted:
                colorCtrl.SetTextColor(Color.gray);
                break;
                
            case QuestState.InProgress:
                colorCtrl.SetTextColor(Color.white);
                break;
                
            case QuestState.Completed:
                colorCtrl.SetTextColor(Color.green);
                break;
                
            case QuestState.Failed:
                colorCtrl.SetTextColor(Color.red);
                break;
        }
    }
}
```

### 示例 4: 稀有度颜色

```csharp
public class ItemQualityText : MonoBehaviour
{
    [SerializeField] private TextColorCtrl colorCtrl;
    
    // 稀有度颜色配置
    private static readonly Dictionary<ItemQuality, Color> qualityColors = new Dictionary<ItemQuality, Color>
    {
        { ItemQuality.Common, new Color(1f, 1f, 1f) },      // 白色
        { ItemQuality.Uncommon, new Color(0.12f, 1f, 0.24f) }, // 绿色
        { ItemQuality.Rare, new Color(0.2f, 0.6f, 1f) },    // 蓝色
        { ItemQuality.Epic, new Color(0.64f, 0.2f, 0.93f) },  // 紫色
        { ItemQuality.Legendary, new Color(1f, 0.77f, 0.25f) } // 橙色
    };
    
    public void SetItemQuality(ItemQuality quality)
    {
        if (qualityColors.TryGetValue(quality, out Color color))
        {
            colorCtrl.SetTextColor(color);
        }
    }
}
```

### 示例 5: 计时器倒计时效果

```csharp
public class CountdownText : MonoBehaviour
{
    [SerializeField] private TextColorCtrl colorCtrl;
    [SerializeField] private Text text;
    [SerializeField] private float totalTime = 10f;
    
    private float remainingTime;
    
    void Update()
    {
        remainingTime -= Time.deltaTime;
        text.text = Mathf.Ceil(remainingTime).ToString();
        
        // 根据剩余时间改变颜色
        float percent = remainingTime / totalTime;
        
        if (percent > 0.5f)
        {
            colorCtrl.ClearTextColor();  // 正常颜色
        }
        else if (percent > 0.25f)
        {
            colorCtrl.SetTextColor(Color.yellow);  // 警告
        }
        else
        {
            colorCtrl.SetTextColor(Color.red);  // 紧急
        }
        
        if (remainingTime <= 0)
        {
            OnTimeUp();
        }
    }
    
    void OnTimeUp()
    {
        // 时间到
        colorCtrl.SetTextColor(Color.red);
    }
}
```

### 示例 6: 多效果组合

```csharp
public class StatusTextEffect : MonoBehaviour
{
    [SerializeField] private TextColorCtrl colorCtrl;
    
    // 设置中毒效果
    public void SetPoisoned()
    {
        colorCtrl.SetTextColor(new Color(0.5f, 0.2f, 0.8f));  // 紫色
        colorCtrl.SetOutlineColor(Color.green);  // 绿色描边
    }
    
    // 设置燃烧效果
    public void SetBurning()
    {
        colorCtrl.SetTextColor(new Color(1f, 0.5f, 0f));  // 橙色
        colorCtrl.SetShadowColor(Color.red);  // 红色阴影
    }
    
    // 设置冰冻效果
    public void SetFrozen()
    {
        colorCtrl.SetTextColor(new Color(0.5f, 0.8f, 1f));  // 浅蓝
        colorCtrl.SetOutlineColor(Color.blue);
    }
    
    // 清除所有效果
    public void ClearAllEffects()
    {
        colorCtrl.ClearTextColor();
        colorCtrl.ClearOutlineColor();
        colorCtrl.ClearShadowColor();
    }
}
```

---

## 组件配置

### Unity 编辑器配置

```
文本 GameObject
├── Text 或 TextMeshProUGUI
├── Outline (可选)
├── Shadow (可选)
└── TextColorCtrl (本组件)

Inspector 配置:
┌─────────────────────────────────────┐
│ Text Color Ctrl (Script)            │
├─────────────────────────────────────┤
│ M Text: Text (自动获取)             │
│ M Text2: TextMeshProUGUI (自动获取) │
│ M Origin Text Color: RGBA(1,1,1,1)  │
│ M Outline: Outline (自动获取)       │
│ M Origin Outline Color: RGBA(...)   │
│ M Shadow: Shadow (自动获取)         │
│ M Origin Shadow Color: RGBA(...)    │
└─────────────────────────────────────┘
```

---

## 技术要点

### 1. 双文本系统支持

```csharp
// 优先使用 TextMeshPro
m_text2 = GetComponent<TMPro.TMP_Text>();
if (m_text2 != null)
{
    m_originTextColor = m_text2.color;
}
else
{
    // 回退到 Unity Text
    m_text = GetComponent<Text>();
    if (m_text != null)
        m_originTextColor = m_text.color;
}
```

### 2. 静态工厂方法

```csharp
public static TextColorCtrl Get(GameObject go)
{
    var ctrl = go.GetComponent<TextColorCtrl>();
    if (ctrl == null)
    {
        ctrl = go.AddComponent<TextColorCtrl>();
    }
    return ctrl;
}
```

### 3. 可选组件处理

```csharp
// Outline 和 Shadow 是可选的
m_outline = GetComponent<Outline>();
if (m_outline != null)
    m_originOutlineColor = m_outline.effectColor;
```

---

## 相关文档

- **UIManager**: [UIManager.cs.md](../../../Code/Module/UI/UIManager.cs.md) - UI 管理器
- **UIComponent**: UIComponent 目录下的 UI 组件文档

---

## 注意事项

### ⚠️ 组件获取顺序

优先获取 TMPro.TMP_Text，如果没有才获取 Unity Text。

### ⚠️ 颜色恢复

确保在适当时机调用 Clear 方法恢复原始颜色，避免颜色状态混乱。

### ⚠️ 内存管理

TextColorCtrl 在 Awake 时自动获取组件，无需手动清理。

### ⚠️ 运行时添加

如果运行时动态添加 Outline/Shadow，需要重新获取引用：
```csharp
// 动态添加 Outline 后
Outline outline = gameObject.AddComponent<Outline>();
colorCtrl.m_outline = outline;
colorCtrl.m_originOutlineColor = outline.effectColor;
```

---

*文档生成时间：2026-03-01 | OpenClaw AI 助手*
