# UIDropdown.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | UIDropdown.cs |
| **路径** | Assets/Scripts/Code/Module/UIComponent/UIDropdown.cs |
| **所属模块** | 框架层 → Code/Module/UIComponent |
| **文件职责** | UI 下拉框组件，封装 Dropdown |

---

## 类/结构体说明

### UIDropdown

| 属性 | 说明 |
|------|------|
| **职责** | 封装 Unity Dropdown，提供值变化和选项选择功能 |
| **泛型参数** | 无 |
| **继承关系** | 继承自 `UIBaseContainer` |
| **实现的接口** | `IOnDestroy` |

```csharp
public class UIDropdown : UIBaseContainer, IOnDestroy
{
    // UI 下拉框组件
}
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `dropdown` | `Dropdown` | `public` | Unity Dropdown 组件 |
| `onValueChanged` | `UnityAction<int>` | `private` | 值变化事件回调 |

---

## 方法说明（按重要程度排序）

### OnDestroy()

**签名**:
```csharp
public void OnDestroy()
```

**职责**: 销毁时清理资源

**核心逻辑**:
```
1. 移除值变化事件监听 RemoveOnValueChanged()
```

**调用者**: `UIBaseContainer.BeforeOnDestroy()`

---

### SetOnValueChanged(UnityAction<int> callback)

**签名**:
```csharp
public void SetOnValueChanged(UnityAction<int> callback)
```

**职责**: 设置值变化事件回调

**参数**:
- `callback`: 值变化回调函数

**核心逻辑**:
```
1. 激活 Dropdown 组件
2. 移除旧的监听
3. 保存 callback
4. 添加新的监听
```

**调用者**: 需要监听下拉框变化的代码

**使用示例**:
```csharp
// 获取下拉框组件
var languageDropdown = view.AddComponent<UIDropdown>("LanguageDropdown");

// 设置值变化事件
languageDropdown.SetOnValueChanged((index) =>
{
    Log.Info($"选择语言索引：{index}");
    ChangeLanguage(index);
});
```

---

### RemoveOnValueChanged()

**签名**:
```csharp
public void RemoveOnValueChanged()
```

**职责**: 移除值变化事件监听

**核心逻辑**:
```
1. 如果 onValueChanged 不为 null
2. 从 dropdown.onValueChanged 移除监听
3. 清空 onValueChanged
```

**调用者**: `SetOnValueChanged()`, `OnDestroy()`

---

### GetValue()

**签名**:
```csharp
public int GetValue()
```

**职责**: 获取当前选择的索引

**返回**: 当前选择的选项索引

**核心逻辑**:
```
1. 激活 Dropdown 组件
2. 返回 dropdown.value
```

**调用者**: 需要读取当前选择的代码

**使用示例**:
```csharp
// 获取当前选择的语言索引
int languageIndex = languageDropdown.GetValue();
```

---

### SetValue(int value)

**签名**:
```csharp
public void SetValue(int value)
```

**职责**: 设置当前选择的索引

**参数**:
- `value`: 要设置的选项索引

**核心逻辑**:
```
1. 激活 Dropdown 组件
2. 设置 dropdown.value = value
```

**调用者**: 需要设置选择的代码

**使用示例**:
```csharp
// 设置为第 2 个选项
languageDropdown.SetValue(1);  // 索引从 0 开始
```

---

## 阅读指引

### 建议的阅读顺序

1. **理解下拉框组件作用** - 为什么需要 UIDropdown
2. **看 SetOnValueChanged** - 理解事件监听
3. **看 GetValue/SetValue** - 理解值设置

### 最值得学习的技术点

1. **简单封装**: 直接封装 Unity Dropdown
2. **事件管理**: 自动清理事件监听
3. **索引选择**: 使用整数索引选择选项

---

## 使用示例

### 示例 1: 语言选择

```csharp
public class UISettingsView : UIBaseView, IOnCreate
{
    private UIDropdown languageDropdown;
    
    public void OnCreate()
    {
        languageDropdown = AddComponent<UIDropdown>("LanguageDropdown");
        languageDropdown.SetOnValueChanged(OnLanguageChanged);
    }
    
    private void OnLanguageChanged(int index)
    {
        Log.Info($"切换语言：{index}");
        I18NManager.Instance.SetLanguage(index);
    }
    
    public void SetCurrentLanguage(int index)
    {
        languageDropdown.SetValue(index);
    }
}
```

### 示例 2: 难度选择

```csharp
public class UIDifficultySelectView : UIBaseView, IOnCreate
{
    private UIDropdown difficultyDropdown;
    
    public void OnCreate()
    {
        difficultyDropdown = AddComponent<UIDropdown>("DifficultyDropdown");
        difficultyDropdown.SetOnValueChanged(OnDifficultyChanged);
    }
    
    private void OnDifficultyChanged(int index)
    {
        string difficulty = GetDifficultyName(index);
        Log.Info($"选择难度：{difficulty}");
    }
    
    private string GetDifficultyName(int index)
    {
        switch (index)
        {
            case 0: return "简单";
            case 1: return "普通";
            case 2: return "困难";
            default: return "未知";
        }
    }
}
```

---

## 相关文档

- [UIBaseContainer.cs.md](./UIBaseContainer.cs.md) - UI 容器基类
- [UISlider.cs.md](./UISlider.cs.md) - UI 滑块组件
- [UIToggle.cs.md](./UIToggle.cs.md) - UI 开关组件

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
