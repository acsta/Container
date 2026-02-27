# UIButton.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | UIButton.cs |
| **路径** | Assets/Scripts/Code/Module/UIComponent/UIButton.cs |
| **所属模块** | 框架层 → Code/Module/UIComponent |
| **文件职责** | UI 按钮组件，封装点击事件、冷却时间、灰化效果 |

---

## 类/结构体说明

### UIButton

| 属性 | 说明 |
|------|------|
| **职责** | 封装 Unity Button，提供点击事件、冷却、灰化等功能 |
| **泛型参数** | 无 |
| **继承关系** | 继承自 `UIBaseContainer` |
| **实现的接口** | `IOnDestroy` |

```csharp
public class UIButton : UIBaseContainer, IOnDestroy
{
    // UI 按钮组件
}
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `onclick` | `Action` | `private` | 点击事件回调 |
| `button` | `Button` | `private` | Unity Button 组件 |
| `image` | `Image` | `private` | Unity Image 组件（用于灰化） |
| `spritePath` | `string` | `private` | 精灵图片路径 |
| `grayState` | `bool` | `private` | 是否灰化状态 |
| `ClickInterval` | `int` | `public` | 点击冷却时间（毫秒，默认 500） |
| `lastClickTime` | `long` | `private` | 上次点击时间 |
| `version` | `int` | `private` | 版本号（防止重复加载） |

---

## 方法说明（按重要程度排序）

### OnDestroy()

**签名**:
```csharp
public virtual void OnDestroy()
```

**职责**: 销毁时清理资源

**核心逻辑**:
```
1. 移除点击事件监听 RemoveOnClick()
2. 如果设置了 spritePath，释放图片资源
3. 清空 onclick 回调
```

**调用者**: `UIBaseContainer.BeforeOnDestroy()`

---

### SetOnClick(Action callback)

**签名**:
```csharp
public void SetOnClick(Action callback)
```

**职责**: 设置点击事件回调

**参数**:
- `callback`: 点击回调函数

**核心逻辑**:
```
1. 激活 Button 组件
2. 移除旧的点击监听
3. 保存 callback
4. 添加新的监听 OnClickBtn
```

**调用者**: UI 初始化代码

**使用示例**:
```csharp
// 获取按钮组件
var button = view.AddComponent<UIButton>("SubmitButton");

// 设置点击事件
button.SetOnClick(() =>
{
    Log.Info("按钮被点击");
    SubmitForm();
});
```

---

### RemoveOnClick()

**签名**:
```csharp
public void RemoveOnClick()
```

**职责**: 移除点击事件监听

**核心逻辑**:
```
1. 如果 onclick 不为 null
2. 从 button.onClick 移除监听
3. 清空 onclick
```

**调用者**: `SetOnClick()`, `OnDestroy()`

---

### OnClickBtn()

**签名**:
```csharp
protected virtual void OnClickBtn()
```

**职责**: 处理按钮点击

**核心逻辑**:
```
1. 获取当前时间
2. 检查冷却时间（防止重复点击）
3. 如果不是编辑器且引导中，检查引导目标
4. 播放点击音效
5. 触发震动反馈
6. 调用 onclick 回调
```

**冷却检查**:
```csharp
var timeNow = TimerManager.Instance.GetTimeNow();
if (timeNow < lastClickTime + ClickInterval)
{
    return;  // 冷却中，忽略点击
}
lastClickTime = timeNow;
```

**调用者**: Unity Button.onClick

---

### SetEnabled(bool flag)

**签名**:
```csharp
public void SetEnabled(bool flag)
```

**职责**: 设置按钮启用/禁用

**参数**:
- `flag`: true=启用，false=禁用

**核心逻辑**:
```
1. 激活 Button 组件
2. 设置 button.enabled = flag
```

**调用者**: 需要控制按钮状态的代码

---

### SetInteractable(bool flag)

**签名**:
```csharp
public void SetInteractable(bool flag)
```

**职责**: 设置按钮是否可交互

**参数**:
- `flag`: true=可交互，false=不可交互

**核心逻辑**:
```
1. 激活 Button 组件
2. 设置 button.interactable = flag
```

**与 SetEnabled 的区别**:
- `SetEnabled`: 完全禁用组件（不响应任何事件）
- `SetInteractable`: 保留组件功能，但不可交互（视觉上变灰）

---

### SetBtnGray(bool isGray, includeText, affectInteractable)

**签名**:
```csharp
public async ETTask SetBtnGray(bool isGray, bool includeText = true, bool affectInteractable = true)
```

**职责**: 设置按钮灰化效果

**参数**:
- `isGray`: 是否灰化
- `includeText`: 是否包含文字（默认 true）
- `affectInteractable`: 是否影响交互（默认 true）

**核心逻辑**:
```
1. 保存 grayState
2. 加载灰化材质（如果是灰化）
3. 设置 Image 材质
4. 如果 affectInteractable，设置 raycastTarget
5. 递归设置所有子 Image 的材质
6. 如果 includeText，设置所有子 Text 的颜色
```

**灰化材质**:
```csharp
// 加载灰化材质
mt = await MaterialManager.Instance.LoadMaterialAsync("UI/UICommon/Materials/uigray.mat");

// 设置文字颜色为灰色
uITextColorCtrl.SetTextColor(new Color(89 / 255f, 93 / 255f, 93 / 255f));
```

**调用者**: 需要禁用按钮但保持可见的场景

**使用示例**:
```csharp
// 灰化按钮（不可点击）
await button.SetBtnGray(isGray: true);

// 恢复按钮（可点击）
await button.SetBtnGray(isGray: false);

// 灰化但不影响文字颜色
await button.SetBtnGray(isGray: true, includeText: false);
```

---

### SetSpritePath(string spritePath, setNativeSize, callback)

**签名**:
```csharp
public async ETTask SetSpritePath(string spritePath, bool setNativeSize = false, Action callback = null)
```

**职责**: 设置按钮图片

**参数**:
- `spritePath`: 图片路径
- `setNativeSize`: 是否设置原始大小
- `callback`: 加载完成回调

**核心逻辑**:
```
1. 增加 version（防止重复加载）
2. 如果路径相同，直接返回
3. 禁用 Image
4. 如果路径为空，清空 sprite
5. 否则加载新 sprite
6. 设置 image.sprite
7. 如果需要，设置原始大小
8. 释放旧图片资源
9. 调用 callback
```

**版本检查**:
```csharp
version++;
int thisVersion = version;

// 加载完成后检查版本
if (thisVersion != version)
{
    // 已被新请求覆盖，释放资源
    ImageLoaderManager.Instance.ReleaseImage(spritePath);
    return;
}
```

**调用者**: 需要动态设置按钮图片的代码

---

## 阅读指引

### 建议的阅读顺序

1. **理解按钮组件作用** - 为什么需要 UIButton
2. **看 SetOnClick** - 理解点击事件设置
3. **看 OnClickBtn** - 理解点击处理逻辑
4. **看 SetBtnGray** - 理解灰化效果

### 最值得学习的技术点

1. **点击冷却**: ClickInterval 防止重复点击
2. **版本控制**: version 防止重复加载
3. **灰化效果**: 材质 + 文字颜色统一处理
4. **资源管理**: 自动释放图片资源
5. **引导集成**: 检查 GuidanceManager.GuideTarget

---

## 使用示例

### 示例 1: 基础按钮

```csharp
// 在 UI 视图中
public class UILoginView : UIBaseView, IOnCreate
{
    private UIButton submitButton;
    private UIButton cancelButton;
    
    public void OnCreate()
    {
        // 获取按钮组件
        submitButton = AddComponent<UIButton>("SubmitButton");
        cancelButton = AddComponent<UIButton>("CancelButton");
        
        // 设置点击事件
        submitButton.SetOnClick(OnSubmit);
        cancelButton.SetOnClick(OnCancel);
    }
    
    private void OnSubmit()
    {
        Log.Info("提交登录");
        // 登录逻辑
    }
    
    private void OnCancel()
    {
        Log.Info("取消登录");
        CloseSelf().Coroutine();
    }
}
```

### 示例 2: 带冷却的按钮

```csharp
// 设置冷却时间为 1 秒
submitButton.ClickInterval = 1000;

// 快速点击只会触发一次
```

### 示例 3: 灰化按钮

```csharp
// 条件不满足时灰化按钮
if (!CanSubmit())
{
    await submitButton.SetBtnGray(isGray: true);
    submitButton.SetInteractable(false);
}
else
{
    await submitButton.SetBtnGray(isGray: false);
    submitButton.SetInteractable(true);
}
```

### 示例 4: 动态图片按钮

```csharp
// 设置按钮图片
await submitButton.SetSpritePath("UI/Buttons/SubmitBtn");

// 带回调
await submitButton.SetSpritePath("UI/Buttons/SubmitBtn", callback: () =>
{
    Log.Info("图片加载完成");
});
```

---

## 相关文档

- [UIBaseContainer.cs.md](./UIBaseContainer.cs.md) - UI 容器基类
- [UIImage.cs.md](./UIImage.cs.md) - UI 图片组件
- [UIText.cs.md](./UIText.cs.md) - UI 文本组件
- [MaterialManager.cs.md](../Resource/MaterialManager.cs.md) - 材质管理

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
