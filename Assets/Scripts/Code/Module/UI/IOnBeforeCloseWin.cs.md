# IOnBeforeCloseWin.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|------|
| **文件名** | IOnBeforeCloseWin.cs |
| **路径** | Assets/Scripts/Code/Module/UI/IOnBeforeCloseWin.cs |
| **所属模块** | 框架层 → Code/Module/UI |
| **文件职责** | 定义 UI 窗口关闭前的异步回调接口 |

---

## 类/结构体说明

### IOnBeforeCloseWin (接口)

| 属性 | 说明 |
|------|------|
| **职责** | 定义窗口关闭前的异步回调方法 |
| **泛型参数** | 无 |
| **继承关系** | 无 |
| **实现的接口** | 无 |

```csharp
public interface IOnBeforeCloseWin
{
    public ETTask OnBeforeDisable();
}
```

**返回类型**: `ETTask` - 支持异步操作

---

## 接口详解

### OnBeforeDisable() 生命周期

**调用时机**: UI 窗口关闭前（可选）

**调用者**: `UIManager.InnerBeforeCloseWindow()`

**调用流程**:
```csharp
// UIManager.InnerBeforeCloseWindow
async ETTask InnerBeforeCloseWindow(UIWindow target)
{
    if (target.Active)
    {
        var view = target.View;
        if (view is IOnBeforeCloseWin a)
        {
            await a.OnBeforeDisable();  // ← 等待异步完成
        }
    }
}
```

**关键特性**:
- **异步**: 返回 ETTask，可以 await
- **可选**: 只有实现接口的窗口才会调用
- **阻塞**: 关闭窗口前会等待异步完成

---

## 使用场景

### 典型用途

1. **播放关闭动画并等待完成**
2. **保存数据到服务器**
3. **确认对话框等待用户选择**
4. **清理异步资源**

---

## 使用示例

### 示例 1: 播放关闭动画

```csharp
public class UIPopupView : UIBaseView, IOnCreate, IOnBeforeCloseWin
{
    private Animator animator;
    
    public void OnCreate()
    {
        animator = GetTransform().GetComponent<Animator>();
    }
    
    public async ETTask OnBeforeDisable()
    {
        // 播放关闭动画
        animator.Play("Close");
        
        // 等待动画完成（假设动画 0.5 秒）
        await TimerManager.Instance.WaitAsync(500);
    }
}

// 关闭窗口时会自动等待动画完成
await UIManager.Instance.CloseWindow<UIPopupView>();
```

---

### 示例 2: 保存数据

```csharp
public class UIEditView : UIBaseView, IOnCreate, IOnBeforeCloseWin
{
    private InputField nameInput;
    private InputField descInput;
    
    public void OnCreate()
    {
        nameInput = GetTransform().Find("NameInput").GetComponent<InputField>();
        descInput = GetTransform().Find("DescInput").GetComponent<InputField>();
    }
    
    public async ETTask OnBeforeDisable()
    {
        // 保存数据到服务器
        await APIManager.Instance.SaveDataAsync(
            nameInput.text,
            descInput.text
        );
    }
}
```

---

### 示例 3: 确认关闭

```csharp
public class UIUnsavedView : UIBaseView, IOnCreate, IOnBeforeCloseWin
{
    private bool hasUnsavedChanges;
    
    public void OnCreate()
    {
        // 监听内容变化
        hasUnsavedChanges = false;
    }
    
    public async ETTask OnBeforeDisable()
    {
        // 如果有未保存的更改，显示确认框
        if (hasUnsavedChanges)
        {
            var confirmed = await ShowConfirmDialog();
            if (!confirmed)
            {
                // 用户取消关闭，抛出异常或返回
                throw new OperationCanceledException("用户取消关闭");
            }
        }
    }
    
    private async ETTask<bool> ShowConfirmDialog()
    {
        var tcs = new TaskCompletionSource<bool>();
        
        await UIManager.Instance.OpenBox<UIMsgBoxWin, MsgBoxPara>(
            UIMsgBoxWin.PrefabPath,
            new MsgBoxPara
            {
                Content = "有未保存的更改，确定要关闭吗？",
                ConfirmText = "确定",
                CancelText = "取消",
                ConfirmCallback = (win) => 
                { 
                    tcs.SetResult(true);
                    UIManager.Instance.CloseBox(win).Coroutine();
                },
                CancelCallback = (win) => 
                { 
                    tcs.SetResult(false);
                    UIManager.Instance.CloseBox(win).Coroutine();
                }
            }
        );
        
        return await tcs.Task;
    }
}
```

---

### 示例 4: 清理异步资源

```csharp
public class UILoadingView : UIBaseView, IOnCreate, IOnBeforeCloseWin
{
    private List<ETTask> loadingTasks;
    
    public void OnCreate()
    {
        loadingTasks = new List<ETTask>();
    }
    
    public void StartLoading(string path)
    {
        var task = LoadResourceAsync(path);
        loadingTasks.Add(task);
    }
    
    public async ETTask OnBeforeDisable()
    {
        // 等待所有加载任务完成或取消
        foreach (var task in loadingTasks)
        {
            if (!task.IsCompleted)
            {
                // 可以选择等待或取消
                await task;
            }
        }
        loadingTasks.Clear();
    }
    
    private async ETTask LoadResourceAsync(string path)
    {
        // 模拟加载
        await TimerManager.Instance.WaitAsync(1000);
    }
}
```

---

## 阅读指引

### 建议的阅读顺序

1. **理解接口作用** - 关闭前的异步回调
2. **对比 OnDisable** - 理解时机差异
3. **看异步特性** - 理解 await 机制
4. **了解使用场景** - 动画、保存、确认等

### 最值得学习的技术点

1. **异步生命周期**: 支持 await 的关闭回调
2. **阻塞关闭**: 等待异步完成再关闭
3. **可选实现**: 只有需要时才实现
4. **异常处理**: 可抛出异常取消关闭

---

## 生命周期位置

### 完整关闭流程

```
请求关闭窗口
    ↓
InnerBeforeCloseWindow()
    ↓
[IOnBeforeCloseWin.OnBeforeDisable()] ← 异步等待
    ↓
InnerCloseWindow()
    ↓
IOnDisable.OnDisable()
    ↓
窗口隐藏
```

### 与 OnDisable 的区别

| 特性 | OnBeforeDisable | OnDisable |
|------|----------------|-----------|
| **接口** | IOnBeforeCloseWin | IOnDisable |
| **返回类型** | ETTask (异步) | void (同步) |
| **调用时机** | 关闭前 | 关闭时 |
| **可选性** | 可选实现 | 可选实现 |
| **用途** | 异步操作、动画 | 同步清理 |

---

## 注意事项

### ⚠️ 避免死锁

```csharp
// ❌ 错误：在 OnBeforeDisable 中调用 CloseWindow
public async ETTask OnBeforeDisable()
{
    await SomeAsyncOperation();
    // 错误：可能导致死锁
    await UIManager.Instance.CloseWindow(this);
}

// ✅ 正确：让调用者决定何时关闭
public async ETTask OnBeforeDisable()
{
    await SomeAsyncOperation();
    // 完成后返回，由调用者继续关闭流程
}
```

### ⚠️ 超时处理

```csharp
// ✅ 推荐：添加超时保护
public async ETTask OnBeforeDisable()
{
    try
    {
        // 使用超时等待
        await TimerManager.Instance.WaitAsync(operation, timeout: 5000);
    }
    catch (TimeoutException)
    {
        Log.Error("OnBeforeDisable timeout");
    }
}
```

---

## 相关文档

- [IOnCreate.cs.md](./IOnCreate.cs.md) - 创建接口
- [IOnEnable.cs.md](./IOnEnable.cs.md) - 启用接口
- [IOnDisable.cs.md](./IOnDisable.cs.md) - 禁用接口
- [IOnDestroy.cs.md](./IOnDestroy.cs.md) - 销毁接口
- [UIManager.cs.md](./UIManager.cs.md) - UI 管理器

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
