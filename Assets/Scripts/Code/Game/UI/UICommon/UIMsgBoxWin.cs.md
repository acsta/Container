# UIMsgBoxWin.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | UIMsgBoxWin.cs |
| **路径** | Assets/Scripts/Code/Game/UI/UICommon/UIMsgBoxWin.cs |
| **所属模块** | 玩法层 → UI 通用组件 |
| **文件职责** | 通用消息弹窗窗口，支持自定义内容、按钮文本和回调 |

---

## 类/结构体说明

### MsgBoxPara

| 属性 | 说明 |
|------|------|
| **职责** | 消息弹窗参数数据结构 |
| **泛型参数** | 无 |
| **继承关系** | 无继承 |

**字段说明**:

| 字段 | 类型 | 说明 |
|------|------|------|
| `Content` | `string` | 弹窗显示的主要内容文本 |
| `CancelText` | `string` | 取消按钮文本 |
| `ConfirmText` | `string` | 确认按钮文本 |
| `CancelCallback` | `Action<UIBaseView>` | 取消按钮点击回调 |
| `ConfirmCallback` | `Action<UIBaseView>` | 确认按钮点击回调 |
| `CanClose` | `bool` | 是否显示关闭按钮（默认 false） |
| `HideCancel` | `bool` | 是否隐藏取消按钮（默认 false） |

---

### UIMsgBoxWin

| 属性 | 说明 |
|------|------|
| **职责** | 消息弹窗窗口视图类 |
| **泛型参数** | 无 |
| **继承关系** | 继承 `UIBaseView` |
| **实现的接口** | `IOnCreate`, `IOnEnable<MsgBoxPara>`, `IOnDisable` |

**设计模式**: MVC 视图层 + 回调模式

```csharp
// 使用示例
UIManager.Instance.OpenWindow<UIMsgBoxWin, MsgBoxPara>(
    UIMsgBoxWin.PrefabPath,
    new MsgBoxPara
    {
        Content = "确定要删除吗？",
        CancelText = "取消",
        ConfirmText = "确定",
        ConfirmCallback = (view) => { /* 确认逻辑 */ },
        CanClose = true
    }
);
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `PrefabPath` | `string` | `public static` | 预制体路径：`"UI/UIUpdate/Prefabs/UIMsgBoxWin.prefab"` |
| `Text` | `UITextmesh` | `public` | 内容文本组件 |
| `btn_cancel` | `UIButton` | `public` | 取消按钮组件 |
| `CancelText` | `UITextmesh` | `public` | 取消按钮文本 |
| `btn_confirm` | `UIButton` | `public` | 确认按钮组件 |
| `ConfirmText` | `UITextmesh` | `public` | 确认按钮文本 |
| `CloseBtn` | `UIButton` | `public` | 关闭按钮组件 |
| `Win` | `UIAnimator` | `public` | 窗口动画组件 |
| `para` | `MsgBoxPara` | `private` | 当前弹窗参数 |

---

## 方法说明（按重要程度排序）

### OnCreate()

**签名**:
```csharp
public void OnCreate()
```

**职责**: 初始化窗口组件引用

**核心逻辑**:
```
1. 获取 Win 动画组件
2. 获取 Close 关闭按钮
3. 获取 Text 内容文本
4. 获取 btn_cancel 取消按钮及文本
5. 获取 btn_confirm 确认按钮及文本
```

**调用者**: UIManager（窗口创建时自动调用）

---

### OnEnable(MsgBoxPara a)

**签名**:
```csharp
public void OnEnable(MsgBoxPara a)
```

**职责**: 窗口启用时设置弹窗内容和行为

**核心逻辑**:
```
1. 播放打开音效 "Audio/Sound/Win_Open.mp3"
2. 设置关闭按钮可见性（根据 CanClose）
3. 绑定关闭按钮点击事件
4. 保存参数 para = a
5. 设置内容文本 Text.SetText(a.Content)
6. 根据 HideCancel 设置取消按钮可见性
7. 绑定取消按钮点击事件和文本
8. 绑定确认按钮点击事件和文本
```

**调用者**: UIManager（窗口打开时自动调用）

---

### CloseSelf()

**签名**:
```csharp
public override async ETTask CloseSelf()
```

**职责**: 关闭窗口，播放关闭动画

**核心逻辑**:
```
1. 播放关闭音效 "Audio/Sound/Win_Close.mp3"
2. 播放 Win 组件的 "UIWin_Close" 动画（等待完成）
3. 调用基类 CloseSelf() 关闭窗口
```

**调用者**: `Close()`, UIManager

**异步行为**: 等待动画播放完成后再关闭窗口

---

### OnDisable()

**签名**:
```csharp
public void OnDisable()
```

**职责**: 窗口禁用时清理事件绑定

**核心逻辑**:
```
1. 移除取消按钮点击事件
2. 移除确认按钮点击事件
```

**调用者**: UIManager（窗口禁用时自动调用）

---

### OnClickConfirm()

**签名**:
```csharp
private void OnClickConfirm()
```

**职责**: 处理确认按钮点击

**核心逻辑**:
```
1. 如果 para.ConfirmCallback 不为空
2. 调用回调并传入当前视图实例
```

**调用者**: btn_confirm 点击事件

---

### OnClickCancel()

**签名**:
```csharp
private void OnClickCancel()
```

**职责**: 处理取消按钮点击

**核心逻辑**:
```
1. 如果 para.CancelCallback 不为空
2.   调用回调并传入当前视图实例
3. 否则
4.   调用 Close() 关闭窗口
```

**调用者**: btn_cancel 点击事件

---

### Close()

**签名**:
```csharp
void Close()
```

**职责**: 启动关闭流程

**核心逻辑**:
```
1. 调用 CloseSelf().Coroutine() 将异步任务转为协程
```

**调用者**: OnClickCancel()（当无取消回调时）

---

## 使用场景

### 1. 简单确认框
```csharp
UIManager.Instance.OpenWindow<UIMsgBoxWin, MsgBoxPara>(
    UIMsgBoxWin.PrefabPath,
    new MsgBoxPara
    {
        Content = "确定要删除这个物品吗？",
        CancelText = "取消",
        ConfirmText = "删除",
        ConfirmCallback = (view) => { DeleteItem(); }
    }
);
```

### 2. 带关闭按钮的提示框
```csharp
UIManager.Instance.OpenWindow<UIMsgBoxWin, MsgBoxPara>(
    UIMsgBoxWin.PrefabPath,
    new MsgBoxPara
    {
        Content = "活动已结束",
        ConfirmText = "知道了",
        CanClose = true,
        HideCancel = true
    }
);
```

### 3. 需要自定义行为的弹窗
```csharp
UIManager.Instance.OpenWindow<UIMsgBoxWin, MsgBoxPara>(
    UIMsgBoxWin.PrefabPath,
    new MsgBoxPara
    {
        Content = "登录已过期，是否重新登录？",
        CancelText = "退出",
        ConfirmText = "重新登录",
        CancelCallback = (view) => { Application.Quit(); },
        ConfirmCallback = (view) => { LoginAgain(); }
    }
);
```

---

## UI 结构

```
UIMsgBoxWin (UIBaseView)
└── Win (UIAnimator)
    ├── Close (UIButton) - 关闭按钮（可选）
    ├── Text (UITextmesh) - 内容文本
    ├── btn_cancel (UIButton)
    │   └── Text (UITextmesh) - 取消按钮文本
    └── btn_confirm (UIButton)
        └── Text (UITextmesh) - 确认按钮文本
```

---

## 音效资源

| 音效 | 路径 | 触发时机 |
|------|------|----------|
| 打开音效 | `Audio/Sound/Win_Open.mp3` | OnEnable 时 |
| 关闭音效 | `Audio/Sound/Win_Close.mp3` | CloseSelf 时 |

---

## 动画资源

| 动画 | 组件 | 触发时机 |
|------|------|----------|
| UIWin_Close | Win (UIAnimator) | 关闭窗口时 |

---

## 相关文档

- [UIManager.cs.md](../../Module/UI/UIManager.cs.md) - UI 管理器
- [UIBaseView.cs.md](../../Module/UI/UIBaseView.cs.md) - UI 视图基类
- [UIButton.cs.md](../../Module/UIComponent/UIButton.cs.md) - 按钮组件
- [UITextmesh.cs.md](../../Module/UIComponent/UITextmesh.cs.md) - 文本组件
- [UIAnimator.cs.md](../../Module/UIComponent/UIAnimator.cs.md) - 动画组件

---

*文档生成时间：2026-03-01 | OpenClaw AI 助手*
