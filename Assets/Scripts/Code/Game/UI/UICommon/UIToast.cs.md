# UIToast.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | UIToast.cs |
| **路径** | Assets/Scripts/Code/Game/UI/UICommon/UIToast.cs |
| **所属模块** | 玩法层 → UI 通用组件 |
| **文件职责** | 轻提示 Toast 组件，短暂显示后自动消失 |

---

## 类/结构体说明

### UIToast

| 属性 | 说明 |
|------|------|
| **职责** | Toast 轻提示视图类 |
| **泛型参数** | 无 |
| **继承关系** | 继承 `UIBaseView` |
| **实现的接口** | `IOnCreate`, `IOnEnable<string>`, `IOnEnable<I18NKey>`, `IOnEnable<string, int>`, `IOnEnable<I18NKey, int>` |

**设计模式**: 单例快捷调用 + 多重重载

```csharp
// 使用示例 - 快捷方法
UIToast.ShowToast(I18NKey.Text_Copy_Over);

// 使用示例 - 直接打开
UIManager.Instance.OpenBox<UIToast, string>(UIToast.PrefabPath, "操作成功");
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `PrefabPath` | `string` | `public const` | 预制体路径：`"UI/UICommon/Prefabs/UIToast.prefab"` |
| `CanvasGroup` | `UIMonoBehaviour<CanvasGroup>` | `public` | CanvasGroup 组件，控制透明度 |
| `Text` | `UITextmesh` | `public` | 提示文本组件 |

---

## 静态方法

### ShowToast(I18NKey key)

**签名**:
```csharp
public static void ShowToast(I18NKey key)
```

**职责**: 快捷显示国际化 Toast 提示

**核心逻辑**:
```
1. 调用 UIManager.Instance.OpenBox<UIToast, I18NKey>
2. 传入预制体路径和国际化键
3. 启动协程执行
```

**调用者**: 任意需要显示 Toast 的代码

**使用示例**:
```csharp
UIToast.ShowToast(I18NKey.Text_Copy_Over); // "复制成功"
```

---

## 生命周期方法

### OnCreate()

**签名**:
```csharp
public void OnCreate()
```

**职责**: 初始化组件引用

**核心逻辑**:
```
1. 获取 CanvasGroup 组件
2. 获取 Text 文本组件
```

**调用者**: UIManager（组件创建时自动调用）

---

### OnEnable(string content)

**签名**:
```csharp
public void OnEnable(string content)
```

**职责**: 启用 Toast，显示普通文本（默认 1.5 秒）

**核心逻辑**:
```
1. 设置文本内容
2. 启动 OnEnableAsync() 协程
```

**调用者**: UIManager（窗口打开时自动调用）

---

### OnEnable(I18NKey key)

**签名**:
```csharp
public void OnEnable(I18NKey key)
```

**职责**: 启用 Toast，显示国际化文本（默认 1.5 秒）

**核心逻辑**:
```
1. 设置国际化文本 Text.SetI18NKey(key)
2. 启动 OnEnableAsync() 协程
```

**调用者**: UIManager（窗口打开时自动调用）

---

### OnEnable(string content, int time)

**签名**:
```csharp
public void OnEnable(string content, int time)
```

**职责**: 启用 Toast，显示普通文本（自定义时长）

**核心逻辑**:
```
1. 设置文本内容
2. 启动 OnEnableAsync(time) 协程
```

**参数说明**:
- `content`: 提示文本内容
- `time`: 显示时长（毫秒）

**调用者**: UIManager（窗口打开时自动调用）

---

### OnEnable(I18NKey key, int time)

**签名**:
```csharp
public void OnEnable(I18NKey key, int time)
```

**职责**: 启用 Toast，显示国际化文本（自定义时长）

**核心逻辑**:
```
1. 设置国际化文本 Text.SetI18NKey(key)
2. 启动 OnEnableAsync(time) 协程
```

**调用者**: UIManager（窗口打开时自动调用）

---

## 私有方法

### OnEnableAsync(int time = 1500)

**签名**:
```csharp
private async ETTask OnEnableAsync(int time = 1500)
```

**职责**: Toast 显示动画流程

**核心逻辑**:
```
1. 获取 CanvasGroup 组件
2. 设置透明度 alpha = 1（完全可见）
3. 等待指定时长 time（默认 1500ms）
4. 记录开始时间 startTime
5. 淡出动画循环：
   - 每帧等待 1ms
   - 计算经过时间 timeNow - startTime
   - 如果超过 500ms → alpha = 0，跳出循环
   - 否则 → alpha = Mathf.Lerp(1, 0, 进度)
6. 关闭自身 CloseSelf()
```

**动画曲线**:
```
时间轴:
0ms          1500ms       2000ms
│             │            │
▼             ▼            ▼
显示 (alpha=1) ────────→ 淡出 (500ms) → 关闭
```

**调用者**: 所有 OnEnable 重载方法

**异步行为**: 
- 等待显示时长
- 逐帧淡出动画
- 自动关闭

---

## 使用场景

### 1. 操作成功提示
```csharp
UIToast.ShowToast(I18NKey.Text_Copy_Over); // "复制成功"
```

### 2. 自定义文本提示
```csharp
UIManager.Instance.OpenBox<UIToast, string>(
    UIToast.PrefabPath,
    "保存成功！"
);
```

### 3. 自定义时长提示
```csharp
UIManager.Instance.OpenBox<UIToast, string, int>(
    UIToast.PrefabPath,
    "正在处理，请稍候...",
    3000  // 3 秒
);
```

### 4. 国际化提示
```csharp
UIManager.Instance.OpenBox<UIToast, I18NKey>(
    UIToast.PrefabPath,
    I18NKey.Text_Save_Success
);
```

---

## UI 结构

```
UIToast (UIBaseView)
├── CanvasGroup (UIMonoBehaviour<CanvasGroup>) - 透明度控制
└── Text (UITextmesh) - 提示文本
```

---

## 动画参数

| 阶段 | 时长 | 透明度变化 |
|------|------|------------|
| 显示 | 默认 1500ms | alpha = 1（保持） |
| 淡出 | 500ms | alpha: 1 → 0（线性插值） |
| 关闭 | - | 销毁窗口 |

**可自定义**: 通过 `OnEnable(content, time)` 的 `time` 参数调整显示时长

---

## 相关文档

- [UIManager.cs.md](../../Module/UI/UIManager.cs.md) - UI 管理器
- [UIBaseView.cs.md](../../Module/UI/UIBaseView.cs.md) - UI 视图基类
- [UITextmesh.cs.md](../../Module/UIComponent/UITextmesh.cs.md) - 文本组件
- [I18NKey.cs.md](../../Module/Const/I18NKey.cs.md) - 国际化键

---

*文档生成时间：2026-03-01 | OpenClaw AI 助手*
