# UISlider.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | UISlider.cs |
| **路径** | Assets/Scripts/Code/Module/UIComponent/UISlider.cs |
| **所属模块** | 框架层 → Code/Module/UIComponent |
| **文件职责** | UI 滑块组件，封装 Slider，支持进度条、值列表 |

---

## 类/结构体说明

### UISlider

| 属性 | 说明 |
|------|------|
| **职责** | 封装 Unity Slider，支持进度条、值列表、事件监听 |
| **泛型参数** | 无 |
| **继承关系** | 继承自 `UIBaseContainer` |
| **实现的接口** | `IOnDestroy` |

```csharp
public class UISlider : UIBaseContainer, IOnDestroy
{
    // UI 滑块组件
}
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `slider` | `Slider` | `private` | Unity Slider 组件 |
| `onValueChanged` | `UnityAction<float>` | `private` | 值变化事件回调 |
| `isWholeNumbers` | `bool` | `private` | 是否整数模式 |
| `valueList` | `ArrayList` | `private` | 值列表（用于枚举选项） |

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

### SetOnValueChanged(UnityAction<float> callback)

**签名**:
```csharp
public void SetOnValueChanged(UnityAction<float> callback)
```

**职责**: 设置值变化事件回调

**参数**:
- `callback`: 值变化回调函数

**核心逻辑**:
```
1. 激活 Slider 组件
2. 移除旧的监听
3. 保存 callback
4. 添加新的监听
```

**调用者**: 需要监听滑块变化的代码

**使用示例**:
```csharp
// 获取滑块组件
var volumeSlider = view.AddComponent<UISlider>("VolumeSlider");

// 设置值变化事件
volumeSlider.SetOnValueChanged((value) =>
{
    Log.Info($"音量：{value}");
    SetVolume(value);
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
2. 从 slider.onValueChanged 移除监听
3. 清空 onValueChanged
```

**调用者**: `SetOnValueChanged()`, `OnDestroy()`

---

### SetWholeNumbers(bool wholeNumbers)

**签名**:
```csharp
public void SetWholeNumbers(bool wholeNumbers)
```

**职责**: 设置是否整数模式

**参数**:
- `wholeNumbers`: true=整数，false=小数

**核心逻辑**:
```
1. 激活 Slider 组件
2. 设置 slider.wholeNumbers = wholeNumbers
3. 保存 isWholeNumbers
```

**调用者**: 需要整数模式的场景

**使用示例**:
```csharp
// 整数模式（用于等级选择）
levelSlider.SetWholeNumbers(true);

// 小数模式（用于音量）
volumeSlider.SetWholeNumbers(false);
```

---

### SetValueList(ArrayList valueList)

**签名**:
```csharp
public void SetValueList(ArrayList valueList)
```

**职责**: 设置值列表（用于枚举选项）

**参数**:
- `valueList`: 值列表

**核心逻辑**:
```
1. 保存 valueList
2. 设置整数模式
3. 设置 minValue = 0
4. 设置 maxValue = valueList.Count - 1
```

**调用者**: 需要枚举选项的场景

**使用示例**:
```csharp
// 设置难度选项
var difficulties = new ArrayList { "简单", "普通", "困难" };
difficultySlider.SetValueList(difficulties);

// 获取当前选择的难度
string currentDifficulty = (string)difficultySlider.GetWholeNumbersValue();
```

---

### GetWholeNumbersValue() / SetWholeNumbersValue(object value)

**签名**:
```csharp
public object GetWholeNumbersValue()
public void SetWholeNumbersValue(object value)
```

**职责**: 获取/设置值列表中的值

**核心逻辑**:
```
// GetWholeNumbersValue:
1. 检查是否整数模式
2. 获取当前索引 index = (int)slider.value
3. 返回 valueList[index]

// SetWholeNumbersValue:
1. 检查是否整数模式
2. 遍历 valueList 查找匹配的值
3. 设置 slider.value = index
```

**调用者**: 使用值列表的场景

---

### SetValue(float value) / GetValue()

**签名**:
```csharp
public void SetValue(float value)
public float GetValue()
```

**职责**: 设置/获取滑块值

**核心逻辑**:
```
// SetValue:
1. 激活 Slider 组件
2. 如果是整数模式，转换为 int
3. 设置 slider.value = value

// GetValue:
1. 激活 Slider 组件
2. 返回 slider.value
```

**调用者**: 需要设置/读取滑块值的代码

**使用示例**:
```csharp
// 设置进度
progressSlider.SetValue(0.5f);  // 50%

// 获取进度
float progress = progressSlider.GetValue();
```

---

### SetNormalizedValue(float value) / GetNormalizedValue()

**签名**:
```csharp
public void SetNormalizedValue(float value)
public float GetNormalizedValue()
```

**职责**: 设置/获取归一化值（0-1）

**核心逻辑**:
```
// SetNormalizedValue:
1. 激活 Slider 组件
2. 设置 slider.normalizedValue = value

// GetNormalizedValue:
1. 激活 Slider 组件
2. 返回 slider.normalizedValue
```

**与 SetValue 的区别**:
- `SetValue`: 设置实际值（minValue 到 maxValue 之间）
- `SetNormalizedValue`: 设置归一化值（0 到 1 之间）

**使用示例**:
```csharp
// 设置 50% 进度
slider.SetNormalizedValue(0.5f);

// 获取进度百分比
float percent = slider.GetNormalizedValue() * 100;
```

---

### SetMaxValue(float value) / SetMinValue(float value)

**签名**:
```csharp
public void SetMaxValue(float value)
public void SetMinValue(float value)
```

**职责**: 设置最大值/最小值

**核心逻辑**:
```
1. 激活 Slider 组件
2. 设置 slider.maxValue / slider.minValue
```

**调用者**: 需要自定义范围的场景

**使用示例**:
```csharp
// 设置范围 0-100
slider.SetMinValue(0);
slider.SetMaxValue(100);

// 设置范围 -1 到 1
slider.SetMinValue(-1);
slider.SetMaxValue(1);
```

---

### SetEnable(bool value)

**签名**:
```csharp
public void SetEnable(bool value)
```

**职责**: 设置滑块启用/禁用

**参数**:
- `value`: true=启用，false=禁用

**核心逻辑**:
```
1. 激活 Slider 组件
2. 设置 slider.enabled = value
```

**调用者**: 需要控制滑块状态的代码

---

## 阅读指引

### 建议的阅读顺序

1. **理解滑块组件作用** - 为什么需要 UISlider
2. **看 SetOnValueChanged** - 理解事件监听
3. **看 SetValue/GetValue** - 理解值设置
4. **看 SetValueList** - 理解值列表功能

### 最值得学习的技术点

1. **值列表**: 支持枚举选项
2. **整数模式**: wholeNumbers 支持整数滑块
3. **归一化值**: normalizedValue 支持百分比
4. **事件管理**: 自动清理事件监听

---

## 使用示例

### 示例 1: 进度条

```csharp
public class UILoadingView : UIBaseView, IOnCreate
{
    private UISlider progressBar;
    
    public void OnCreate()
    {
        progressBar = AddComponent<UISlider>("ProgressBar");
        progressBar.SetMinValue(0);
        progressBar.SetMaxValue(1);
    }
    
    public void SetProgress(float value)
    {
        progressBar.SetValue(value);
    }
}
```

### 示例 2: 音量控制

```csharp
public class UISettingsView : UIBaseView, IOnCreate
{
    private UISlider volumeSlider;
    
    public void OnCreate()
    {
        volumeSlider = AddComponent<UISlider>("VolumeSlider");
        volumeSlider.SetMinValue(0);
        volumeSlider.SetMaxValue(1);
        volumeSlider.SetOnValueChanged(OnVolumeChanged);
    }
    
    private void OnVolumeChanged(float value)
    {
        Log.Info($"音量：{value}");
        SoundManager.Instance.SetSoundVolume((int)(value * 100));
    }
}
```

### 示例 3: 难度选择

```csharp
public class UIDifficultyView : UIBaseView, IOnCreate
{
    private UISlider difficultySlider;
    
    public void OnCreate()
    {
        difficultySlider = AddComponent<UISlider>("DifficultySlider");
        
        // 设置难度选项
        var difficulties = new ArrayList { "简单", "普通", "困难" };
        difficultySlider.SetValueList(difficulties);
        difficultySlider.SetOnValueChanged(OnDifficultyChanged);
    }
    
    private void OnDifficultyChanged(float index)
    {
        string difficulty = (string)difficultySlider.GetWholeNumbersValue();
        Log.Info($"选择难度：{difficulty}");
    }
}
```

---

## 相关文档

- [UIBaseContainer.cs.md](./UIBaseContainer.cs.md) - UI 容器基类
- [UIButton.cs.md](./UIButton.cs.md) - UI 按钮组件
- [UIDropdown.cs.md](./UIDropdown.cs.md) - UI 下拉框

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
