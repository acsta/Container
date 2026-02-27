# IOnDisable.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | IOnDisable.cs |
| **路径** | Assets/Scripts/Code/Module/UI/IOnDisable.cs |
| **所属模块** | 框架层 → Code/Module/UI |
| **文件职责** | 定义 UI 窗口禁用时的生命周期接口 |

---

## 类/结构体说明

### IOnDisable (接口)

| 属性 | 说明 |
|------|------|
| **职责** | 无参数禁用接口 |
| **泛型参数** | 无 |
| **继承关系** | 无 |
| **实现的接口** | 无 |

```csharp
public interface IOnDisable
{
    public void OnDisable();
}
```

---

### IOnDisable<P1> (接口)

| 属性 | 说明 |
|------|------|
| **职责** | 带单个参数的禁用接口 |
| **泛型参数** | P1 - 参数 |
| **继承关系** | 无 |
| **实现的接口** | 无 |

```csharp
public interface IOnDisable<P1>
{
    public void OnDisable(P1 p1);
}
```

---

### IOnDisable<P1, P2> (接口)

| 属性 | 说明 |
|------|------|
| **职责** | 带两个参数的禁用接口 |
| **泛型参数** | P1, P2 - 参数 |
| **继承关系** | 无 |
| **实现的接口** | 无 |

```csharp
public interface IOnDisable<P1, P2>
{
    public void OnDisable(P1 p1, P2 p2);
}
```

---

### IOnDisable<P1, P2, P3> (接口)

| 属性 | 说明 |
|------|------|
| **职责** | 带三个参数的禁用接口 |
| **泛型参数** | P1, P2, P3 - 参数 |
| **继承关系** | 无 |
| **实现的接口** | 无 |

```csharp
public interface IOnDisable<P1, P2, P3>
{
    public void OnDisable(P1 p1, P2 p2, P3 p3);
}
```

---

### IOnDisable<P1, P2, P3, P4> (接口)

| 属性 | 说明 |
|------|------|
| **职责** | 带四个参数的禁用接口 |
| **泛型参数** | P1, P2, P3, P4 - 参数 |
| **继承关系** | 无 |
| **实现的接口** | 无 |

```csharp
public interface IOnDisable<P1, P2, P3, P4>
{
    public void OnDisable(P1 p1, P2 p2, P3 p3, P4 p4);
}
```

---

## 接口详解

### OnDisable() 生命周期

**调用时机**: 每次 UI 窗口关闭时

**调用者**: `UIBaseContainer.BeforeOnDisable()`

**调用流程**:
```csharp
// UIBaseContainer.BeforeOnDisable
void BeforeOnDisable()
{
    if (this is IUpdate)
    {
        TimerManager.Instance.Remove(ref timerId);  // 移除 Update 定时器
    }
    Walk((component) =>
    {
        component.BeforeOnDisable();
        if (component is IOnDisable a) a.OnDisable();  // ← 调用 OnDisable
    });
}
```

**与 Unity MonoBehaviour.OnDisable 的区别**:

| 特性 | Unity OnDisable | IOnDisable.OnDisable |
|------|----------------|---------------------|
| 调用时机 | GameObject 禁用时 | UI 窗口关闭时 |
| 参数支持 | 无 | 支持 0-4 个参数 |
| 调用顺序 | Unity 内部决定 | UI 框架控制 |

---

## OnEnable vs OnDisable

### 对称关系

| 特性 | OnEnable | OnDisable |
|------|----------|-----------|
| **调用时机** | 窗口打开时 | 窗口关闭时 |
| **调用次数** | 每次打开 | 每次关闭 |
| **用途** | 更新数据、播放动画 | 清理数据、停止动画 |
| **参数** | 接收打开时的参数 | 可接收关闭时的参数 |

### 生命周期流程

```
打开窗口:
  OnEnable() → 窗口显示
  
关闭窗口:
  OnBeforeDisable() → OnDisable() → 窗口隐藏
```

---

## 使用示例

### 示例 1: 无参数 OnDisable

```csharp
public class UIAuctionView : UIBaseView, IOnCreate, IOnEnable, IOnDisable
{
    private Animator animator;
    private AudioClip bgm;
    
    public void OnCreate()
    {
        animator = GetTransform().GetComponent<Animator>();
    }
    
    public void OnEnable()
    {
        // 打开时：播放动画和音乐
        animator.Play("AuctionOpen");
        bgm = SoundManager.Instance.PlayBGM("Auction");
    }
    
    public void OnDisable()
    {
        // 关闭时：停止音乐、重置动画
        if (bgm != null)
        {
            SoundManager.Instance.StopBGM(bgm);
        }
        animator.Play("Default");
    }
}
```

---

### 示例 2: 清理临时数据

```csharp
public class UIItemPreview : UIBaseView, IOnCreate, IOnEnable<ItemData>, IOnDisable
{
    private ItemData currentItem;
    private List<GameObject> effectObjects;
    
    public void OnCreate()
    {
        effectObjects = new List<GameObject>();
    }
    
    public void OnEnable(ItemData p1)
    {
        currentItem = p1;
        
        // 创建特效
        var effect = CreateEffect(p1.EffectPath);
        effectObjects.Add(effect);
    }
    
    public void OnDisable()
    {
        // 清理临时数据
        currentItem = null;
        
        // 销毁特效
        foreach (var effect in effectObjects)
        {
            GameObject.Destroy(effect);
        }
        effectObjects.Clear();
    }
}
```

---

### 示例 3: 保存状态

```csharp
public class UIScrollList : UIBaseView, IOnCreate, IOnEnable, IOnDisable
{
    private ScrollRect scrollRect;
    private Vector2 lastScrollPos;
    
    public void OnCreate()
    {
        scrollRect = GetTransform().Find("ScrollRect").GetComponent<ScrollRect>();
    }
    
    public void OnEnable()
    {
        // 恢复上次滚动位置
        scrollRect.normalizedPosition = lastScrollPos;
    }
    
    public void OnDisable()
    {
        // 保存当前滚动位置
        lastScrollPos = scrollRect.normalizedPosition;
    }
}
```

---

### 示例 4: 带参数的 OnDisable（扩展用法）

```csharp
public class UIModalDialog : UIBaseView, IOnCreate, IOnEnable<DialogData>, IOnDisable<bool>
{
    private DialogData dialogData;
    private bool result;
    
    public void OnEnable(DialogData p1)
    {
        dialogData = p1;
        // 显示对话框
    }
    
    public void OnDisable(bool p1)
    {
        // 接收关闭时的结果（true=确认，false=取消）
        result = p1;
        
        // 根据结果执行不同逻辑
        if (result)
        {
            dialogData.OnConfirm?.Invoke();
        }
        else
        {
            dialogData.OnCancel?.Invoke();
        }
    }
}
```

---

## 阅读指引

### 建议的阅读顺序

1. **理解接口作用** - OnDisable 是关闭时的清理
2. **对比 OnEnable** - 理解对称关系
3. **看使用场景** - 清理、保存、停止等
4. **了解最佳实践** - 什么应该在 OnDisable 中做

### 最值得学习的技术点

1. **生命周期对称**: OnEnable/OnDisable 成对出现
2. **资源清理**: 在 OnDisable 中清理临时资源
3. **状态保存**: 关闭前保存状态供下次恢复
4. **参数传递**: 支持关闭时传递参数（扩展用法）

---

## 最佳实践

### ✅ 推荐做法

```csharp
// ✅ 清理临时数据
public void OnDisable()
{
    temporaryData = null;
    cacheList.Clear();
}

// ✅ 停止动画/音效
public void OnDisable()
{
    animator.Stop();
    SoundManager.Instance.StopBGM(currentBgm);
}

// ✅ 保存状态
public void OnDisable()
{
    PlayerPrefs.SetFloat("ScrollPos", scrollRect.normalizedPosition);
}

// ✅ 移除事件监听（如果 OnCreate 中动态绑定）
public void OnDisable()
{
    someEvent -= OnSomeEvent;
}
```

### ❌ 避免做法

```csharp
// ❌ 不要在 OnDisable 中修改持久数据
public void OnDisable()
{
    // 错误：关闭窗口不应该影响玩家数据
    PlayerDataManager.Instance.Gold = 0;
}

// ❌ 不要在 OnDisable 中打开新窗口
public void OnDisable()
{
    // 错误：可能导致无限循环
    UIManager.Instance.OpenWindow<OtherView>(...);
}

// ❌ 不要依赖 OnDisable 做关键清理
public void OnDisable()
{
    // 错误：关键资源应该在 OnDestroy 中清理
    criticalResource = null;
}
```

---

## 完整生命周期示例

```csharp
public class UIComplexView : UIBaseView, 
    IOnCreate, 
    IOnEnable, 
    IOnBeforeCloseWin,
    IOnDisable, 
    IOnDestroy
{
    private Button closeButton;
    private Animator animator;
    private AudioClip bgm;
    private List<GameObject> tempEffects;
    
    // ========== 创建（只调用一次） ==========
    public void OnCreate()
    {
        Log.Info("OnCreate: 初始化组件");
        closeButton = GetTransform().Find("CloseBtn").GetComponent<Button>();
        animator = GetTransform().GetComponent<Animator>();
        tempEffects = new List<GameObject>();
        
        // 绑定事件
        closeButton.onClick.AddListener(OnCloseClick);
    }
    
    // ========== 打开（每次打开都调用） ==========
    public void OnEnable()
    {
        Log.Info("OnEnable: 更新数据");
        // 播放打开动画
        animator.Play("Open");
        // 播放背景音乐
        bgm = SoundManager.Instance.PlayBGM("ComplexView");
        // 创建特效
        tempEffects.Add(CreateEffect("FX_Open"));
    }
    
    // ========== 关闭前（可选，异步） ==========
    public async ETTask OnBeforeDisable()
    {
        Log.Info("OnBeforeDisable: 关闭前异步操作");
        // 播放关闭动画并等待
        animator.Play("Close");
        await TimerManager.Instance.WaitAsync(500);
    }
    
    // ========== 关闭（每次关闭都调用） ==========
    public void OnDisable()
    {
        Log.Info("OnDisable: 清理临时数据");
        // 停止音乐
        if (bgm != null)
        {
            SoundManager.Instance.StopBGM(bgm);
        }
        // 清理特效
        foreach (var effect in tempEffects)
        {
            GameObject.Destroy(effect);
        }
        tempEffects.Clear();
    }
    
    // ========== 销毁（只调用一次） ==========
    public void OnDestroy()
    {
        Log.Info("OnDestroy: 销毁资源");
        // 解绑事件
        closeButton.onClick.RemoveListener(OnCloseClick);
    }
    
    private void OnCloseClick()
    {
        CloseSelf().Coroutine();
    }
}

/* 生命周期调用顺序:

首次打开:
  OnCreate()
  OnEnable()

关闭:
  OnBeforeDisable() [可选]
  OnDisable()

再次打开:
  OnEnable()

再次关闭:
  OnBeforeDisable() [可选]
  OnDisable()

销毁:
  OnDisable()
  OnDestroy()
*/
```

---

## 相关文档

- [IOnCreate.cs.md](./IOnCreate.cs.md) - 创建接口
- [IOnEnable.cs.md](./IOnEnable.cs.md) - 启用接口
- [IOnDestroy.cs.md](./IOnDestroy.cs.md) - 销毁接口
- [IOnBeforeCloseWin.cs.md](./IOnBeforeCloseWin.cs.md) - 关闭前接口

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
