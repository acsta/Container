# II18N.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | II18N.cs |
| **路径** | Assets/Scripts/Code/Module/I18N/II18N.cs |
| **所属模块** | 框架层 → Code/Module/I18N |
| **文件职责** | 国际化监听接口定义 |

---

## 接口说明

### II18N

| 属性 | 说明 |
|------|------|
| **职责** | 定义语言切换时的回调接口 |
| **泛型参数** | 无 |

```csharp
public interface II18N
{
    public void OnLanguageChange();
}
```

---

## 方法说明

### OnLanguageChange()

**签名**:
```csharp
public void OnLanguageChange()
```

**职责**: 语言切换时调用

**调用时机**:
- I18NManager.SwitchLanguage() 成功后
- 广播 MessageId.I18NChangeEvt 事件时

**调用者**: I18NManager

---

## 使用场景

### 场景 1: UI 组件监听语言切换

UI 组件实现 II18N 接口，在语言切换时更新文本：

```csharp
public class UIText : MonoBehaviour, II18N
{
    private int i18nKey;
    
    void Start()
    {
        // 注册监听
        I18NManager.Instance.AddI18NListener(this);
        
        // 初始设置文本
        UpdateText();
    }
    
    public void OnLanguageChange()
    {
        // 语言切换时更新文本
        UpdateText();
    }
    
    private void UpdateText()
    {
        string text = I18NManager.Instance.GetText(i18nKey);
        this.textComponent.text = text;
    }
    
    void OnDestroy()
    {
        // 移除监听
        I18NManager.Instance.RemoveI18NListener(this);
    }
}
```

### 场景 2: 配置表对象监听

配置表对象实现 II18N 接口，缓存多语言文本：

```csharp
public class ItemConfig : II18N
{
    public int Id;
    public string NameKey;
    
    private string cachedName;
    
    public void OnLanguageChange()
    {
        // 重新缓存文本
        cachedName = I18NManager.Instance.GetText(NameKey);
    }
    
    public string GetName()
    {
        return cachedName ?? I18NManager.Instance.GetText(NameKey);
    }
}
```

---

## 与 II18NConfig 的区别

| 特性 | II18N | II18NConfig |
|------|-------|-------------|
| 用途 | 监听语言切换 | 获取翻译文本 |
| 方法 | `OnLanguageChange()` | `GetI18NText(LangType)` |
| 调用者 | I18NManager | 外部代码 |
| 适用场景 | UI 组件、缓存对象 | 配置表文本字段 |

---

## 使用示例

### 示例 1: 注册监听

```csharp
// 在 UI 组件中
public class MyView : MonoBehaviour, II18N
{
    void OnEnable()
    {
        I18NManager.Instance.AddI18NListener(this);
    }
    
    void OnDisable()
    {
        I18NManager.Instance.RemoveI18NListener(this);
    }
    
    public void OnLanguageChange()
    {
        // 更新所有文本
        UpdateAllTexts();
    }
}
```

### 示例 2: I18NManager 内部使用

```csharp
// I18NManager 内部
private List<II18N> listeners = new List<II18N>();

public void SwitchLanguage(LangType lang)
{
    // 切换语言
    CurLangType = lang;
    
    // 通知所有监听者
    foreach (var listener in listeners)
    {
        listener.OnLanguageChange();
    }
    
    // 广播事件
    Messager.Instance.Broadcast(MessageId.I18NChangeEvt);
}

public void AddI18NListener(II18N listener)
{
    if (!listeners.Contains(listener))
    {
        listeners.Add(listener);
    }
}

public void RemoveI18NListener(II18N listener)
{
    listeners.Remove(listener);
}
```

---

## 相关文档

- [I18NManager.cs.md](./I18NManager.cs.md) - 国际化管理器
- [II18NConfig.cs.md](./II18NConfig.cs.md) - 国际化配置接口

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
