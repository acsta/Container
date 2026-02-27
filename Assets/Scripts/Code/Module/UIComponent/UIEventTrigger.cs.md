# UIEventTrigger.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | UIEventTrigger.cs |
| **路径** | Assets/Scripts/Code/Module/UIComponent/UIEventTrigger.cs |
| **所属模块** | 框架层 → Code/Module/UIComponent |
| **文件职责** | UI 事件触发器组件，封装 EventTrigger |

---

## 类/结构体说明

### UIEventTrigger

| 属性 | 说明 |
|------|------|
| **职责** | 封装 Unity EventTrigger，支持多种指针事件（点击、拖拽、滚动等） |
| **泛型参数** | 无 |
| **继承关系** | 继承自 `UIBaseContainer` |
| **实现的接口** | `IOnDestroy` |

```csharp
public class UIEventTrigger : UIBaseContainer, IOnDestroy
{
    // UI 事件触发器组件
}
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `EventTrigger` | `EventTrigger` | `private` | Unity EventTrigger 组件 |
| `events` | `Dictionary<EventTriggerType, UnityAction<BaseEventData>>` | `private` | 事件回调字典 |

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
1. 遍历所有注册的事件
2. 从 EventTrigger.triggers 移除监听
3. 清空 events 字典
```

**调用者**: `UIBaseContainer.BeforeOnDestroy()`

---

### AddEvent(EventTriggerType triggerType, UnityAction<PointerEventData> callback)

**签名**:
```csharp
public void AddEvent(EventTriggerType triggerType, UnityAction<PointerEventData> callback)
```

**职责**: 添加事件监听

**参数**:
- `triggerType`: 事件类型（如 PointerEnter、Drag、Drop 等）
- `callback`: 事件回调函数

**核心逻辑**:
```
1. 激活 EventTrigger 组件
2. 移除旧的同类事件
3. 查找或创建 EventTrigger.Entry
4. 包装回调（转换为 UnityAction<BaseEventData>）
5. 保存到 events 字典
6. 添加监听到 Entry.callback
```

**调用者**: 需要监听指针事件的代码

---

### RemoveEvent(EventTriggerType triggerType)

**签名**:
```csharp
public void RemoveEvent(EventTriggerType triggerType)
```

**职责**: 移除事件监听

**核心逻辑**:
```
1. 如果 events 包含该类型
2. 从 EventTrigger.triggers 移除监听
3. 从 events 字典移除
```

**调用者**: `AddEvent()`, `OnDestroy()`, 以及各 RemoveOn* 方法

---

### 便捷事件方法

UIEventTrigger 提供了以下便捷方法：

| 方法 | 事件类型 | 说明 |
|------|---------|------|
| `AddOnPointerEnter` | PointerEnter | 指针进入 |
| `AddOnPointerExit` | PointerExit | 指针离开 |
| `AddOnDrag` | Drag | 拖拽中 |
| `AddOnDrop` | Drop | 放下 |
| `AddOnPointerDown` | PointerDown | 指针按下 |
| `AddOnPointerUp` | PointerUp | 指针抬起 |
| `AddOnPointerClick` | PointerClick | 点击 |
| `AddOnScroll` | Scroll | 滚动 |
| `AddOnInitializePotentialDrag` | InitializePotentialDrag | 初始化拖拽 |
| `AddOnBeginDrag` | BeginDrag | 开始拖拽 |
| `AddOnEndDrag` | EndDrag | 结束拖拽 |

每个 Add 方法都有对应的 Remove 方法。

---

### SetEnabled(bool flag)

**签名**:
```csharp
public void SetEnabled(bool flag)
```

**职责**: 设置事件触发器启用/禁用

**参数**:
- `flag`: true=启用，false=禁用

**核心逻辑**:
```
1. 激活 EventTrigger 组件
2. 设置 EventTrigger.enabled = flag
```

**调用者**: 需要控制事件触发器状态的代码

---

## 阅读指引

### 建议的阅读顺序

1. **理解组件作用** - 为什么需要 UIEventTrigger
2. **看 AddEvent** - 理解事件注册
3. **看便捷方法** - 了解支持的事件类型
4. **了解事件管理** - 理解自动清理机制

### 最值得学习的技术点

1. **多事件支持**: 支持 11 种指针事件
2. **事件字典**: Dictionary 管理事件回调
3. **自动清理**: OnDestroy 自动移除所有事件
4. **便捷方法**: 为常用事件提供快捷方法

---

## 使用示例

### 示例 1: 拖拽物品

```csharp
public class UIItemView : UIBaseView, IOnCreate
{
    private UIEventTrigger eventTrigger;
    
    public void OnCreate()
    {
        eventTrigger = AddComponent<UIEventTrigger>("Item");
        
        // 监听拖拽事件
        eventTrigger.AddOnBeginDrag((data) =>
        {
            Log.Info("开始拖拽");
            StartDrag(data);
        });
        
        eventTrigger.AddOnDrag((data) =>
        {
            Log.Info("拖拽中");
            OnDragging(data);
        });
        
        eventTrigger.AddOnEndDrag((data) =>
        {
            Log.Info("结束拖拽");
            EndDrag(data);
        });
    }
}
```

### 示例 2: 悬停效果

```csharp
public class UIButtonView : UIBaseView, IOnCreate
{
    private UIEventTrigger eventTrigger;
    private UIImage icon;
    
    public void OnCreate()
    {
        eventTrigger = AddComponent<UIEventTrigger>("Button");
        icon = AddComponent<UIImage>("Icon");
        
        // 悬停高亮
        eventTrigger.AddOnPointerEnter((data) =>
        {
            icon.SetColor(new Color(1.2f, 1.2f, 1.2f));
        });
        
        eventTrigger.AddOnPointerExit((data) =>
        {
            icon.SetColor(Color.white);
        });
    }
}
```

### 示例 3: 滚动事件

```csharp
public class UIScrollView : UIBaseView, IOnCreate
{
    private UIEventTrigger eventTrigger;
    
    public void OnCreate()
    {
        eventTrigger = AddComponent<UIEventTrigger>("ScrollView");
        
        // 监听滚动
        eventTrigger.AddOnScroll((data) =>
        {
            float scroll = data.scrollDelta.y;
            Log.Info($"滚动：{scroll}");
            OnScroll(scroll);
        });
    }
}
```

### 示例 4: 点击事件（替代 UIButton）

```csharp
public class UICustomButtonView : UIBaseView, IOnCreate
{
    private UIEventTrigger eventTrigger;
    
    public void OnCreate()
    {
        eventTrigger = AddComponent<UIEventTrigger>("Button");
        
        // 监听点击
        eventTrigger.AddOnPointerClick((data) =>
        {
            Log.Info("按钮被点击");
            OnClick();
        });
    }
}
```

---

## EventTriggerType 列表

| 事件类型 | 说明 | 触发时机 |
|---------|------|----------|
| PointerEnter | 指针进入 | 鼠标/触摸进入物体 |
| PointerExit | 指针离开 | 鼠标/触摸离开物体 |
| PointerDown | 指针按下 | 鼠标/触摸按下 |
| PointerUp | 指针抬起 | 鼠标/触摸抬起 |
| PointerClick | 点击 | 完整的点击操作 |
| Drag | 拖拽 | 拖拽过程中 |
| Drop | 放下 | 放下物体 |
| Scroll | 滚动 | 鼠标滚轮滚动 |
| BeginDrag | 开始拖拽 | 开始拖拽时 |
| EndDrag | 结束拖拽 | 结束拖拽时 |
| InitializePotentialDrag | 初始化拖拽 | 准备拖拽时 |

---

## 相关文档

- [UIBaseContainer.cs.md](./UIBaseContainer.cs.md) - UI 容器基类
- [UIButton.cs.md](./UIButton.cs.md) - UI 按钮组件
- [UIPointerClick.cs.md](./UIPointerClick.cs.md) - 简化点击组件

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
