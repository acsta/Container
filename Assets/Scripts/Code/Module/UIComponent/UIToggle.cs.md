# UIToggle.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | UIToggle.cs |
| **路径** | Assets/Scripts/Code/Module/UIComponent/UIToggle.cs |
| **所属模块** | 框架层 → Code/Module/UIComponent |
| **文件职责** | UI 开关组件，封装 Toggle |

---

## 类/结构体说明

### UIToggle

| 属性 | 说明 |
|------|------|
| **职责** | 封装 Unity Toggle，提供开关状态和事件监听 |
| **泛型参数** | 无 |
| **继承关系** | 继承自 `UIBaseContainer` |
| **实现的接口** | `IOnDestroy` |

```csharp
public class UIToggle : UIBaseContainer, IOnDestroy
{
    // UI 开关组件
}
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `toggle` | `Toggle` | `private` | Unity Toggle 组件 |
| `callBack` | `UnityAction<bool>` | `private` | 状态变化事件回调 |

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
1. 移除状态变化事件监听 RemoveOnValueChanged()
```

**调用者**: `UIBaseContainer.BeforeOnDestroy()`

---

### GetIsOn()

**签名**:
```csharp
public bool GetIsOn()
```

**职责**: 获取开关状态

**返回**: `true` = 开启，`false` = 关闭

**核心逻辑**:
```
1. 激活 Toggle 组件
2. 返回 toggle.isOn
```

**调用者**: 需要读取开关状态的代码

**使用示例**:
```csharp
// 获取开关状态
bool isMusicOn = musicToggle.GetIsOn();
if (isMusicOn)
{
    // 音乐已开启
}
```

---

### SetIsOn(bool ison, bool broadcast)

**签名**:
```csharp
public void SetIsOn(bool ison, bool broadcast = true)
```

**职责**: 设置开关状态

**参数**:
- `ison`: true=开启，false=关闭
- `broadcast`: 是否触发事件（默认 true）

**核心逻辑**:
```
1. 激活 Toggle 组件
2. 如果 broadcast = true:
   - 设置 toggle.isOn = ison（触发事件）
3. 如果 broadcast = false:
   - 调用 toggle.SetIsOnWithoutNotify(ison)（不触发事件）
```

**调用者**: 需要设置开关状态的代码

**使用示例**:
```csharp
// 开启音乐（触发事件）
musicToggle.SetIsOn(true);

// 关闭音乐（不触发事件）
musicToggle.SetIsOn(false, broadcast: false);
```

---

### SetOnValueChanged(Action<bool> cb)

**签名**:
```csharp
public void SetOnValueChanged(Action<bool> cb)
```

**职责**: 设置状态变化事件回调

**参数**:
- `cb`: 状态变化回调函数

**核心逻辑**:
```
1. 激活 Toggle 组件
2. 包装回调（转换为 UnityAction<bool>）
3. 保存 callBack
4. 添加监听
```

**调用者**: 需要监听开关变化的代码

**使用示例**:
```csharp
// 获取开关组件
var musicToggle = view.AddComponent<UIToggle>("MusicToggle");

// 设置状态变化事件
musicToggle.SetOnValueChanged((isOn) =>
{
    Log.Info($"音乐开关：{isOn}");
    if (isOn)
    {
        SoundManager.Instance.PlayMusic("Audio/Music/BGM.mp3");
    }
    else
    {
        SoundManager.Instance.StopMusic();
    }
});
```

---

### RemoveOnValueChanged()

**签名**:
```csharp
public void RemoveOnValueChanged()
```

**职责**: 移除状态变化事件监听

**核心逻辑**:
```
1. 如果 callBack 不为 null
2. 从 toggle.onValueChanged 移除监听
3. 清空 callBack
```

**调用者**: `SetOnValueChanged()`, `OnDestroy()`

---

## 阅读指引

### 建议的阅读顺序

1. **理解开关组件作用** - 为什么需要 UIToggle
2. **看 GetIsOn** - 理解状态获取
3. **看 SetIsOn** - 理解状态设置
4. **看 SetOnValueChanged** - 理解事件监听

### 最值得学习的技术点

1. **无通知设置**: SetIsOnWithoutNotify 支持不触发事件
2. **事件管理**: 自动清理事件监听
3. **布尔状态**: 简单的开/关状态

---

## 使用示例

### 示例 1: 音乐开关

```csharp
public class UISettingsView : UIBaseView, IOnCreate
{
    private UIToggle musicToggle;
    private UIToggle soundToggle;
    
    public void OnCreate()
    {
        musicToggle = AddComponent<UIToggle>("MusicToggle");
        soundToggle = AddComponent<UIToggle>("SoundToggle");
        
        musicToggle.SetOnValueChanged(OnMusicChanged);
        soundToggle.SetOnValueChanged(OnSoundChanged);
    }
    
    private void OnMusicChanged(bool isOn)
    {
        Log.Info($"音乐开关：{isOn}");
        if (isOn)
        {
            SoundManager.Instance.PlayMusic("Audio/Music/BGM.mp3");
        }
        else
        {
            SoundManager.Instance.StopMusic();
        }
    }
    
    private void OnSoundChanged(bool isOn)
    {
        Log.Info($"音效开关：{isOn}");
        SoundManager.Instance.SetSoundVolume(isOn ? 100 : 0);
    }
}
```

### 示例 2: 协议勾选

```csharp
public class UIRegisterView : UIBaseView, IOnCreate
{
    private UIToggle agreeToggle;
    private UIButton submitButton;
    
    public void OnCreate()
    {
        agreeToggle = AddComponent<UIToggle>("AgreeToggle");
        submitButton = AddComponent<UIButton>("SubmitButton");
        
        submitButton.SetOnClick(OnSubmit);
    }
    
    private void OnSubmit()
    {
        if (!agreeToggle.GetIsOn())
        {
            // 未勾选协议，提示用户
            UIManager.Instance.OpenBox<UIToast, string>(
                UIToast.PrefabPath,
                "请先勾选用户协议"
            ).Coroutine();
            return;
        }
        
        // 提交注册
        SubmitRegister();
    }
}
```

### 示例 3: 程序化设置

```csharp
// 初始化时设置开关状态（不触发事件）
musicToggle.SetIsOn(PlayerDataManager.Instance.IsMusicOn, broadcast: false);
soundToggle.SetIsOn(PlayerDataManager.Instance.IsSoundOn, broadcast: false);
```

---

## 相关文档

- [UIBaseContainer.cs.md](./UIBaseContainer.cs.md) - UI 容器基类
- [UIButton.cs.md](./UIButton.cs.md) - UI 按钮组件
- [UISlider.cs.md](./UISlider.cs.md) - UI 滑块组件

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
