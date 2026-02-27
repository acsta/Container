# IOnEnable.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | IOnEnable.cs |
| **路径** | Assets/Scripts/Code/Module/UI/IOnEnable.cs |
| **所属模块** | 框架层 → Code/Module/UI |
| **文件职责** | 定义 UI 窗口启用时的生命周期接口 |

---

## 类/结构体说明

### IOnEnable (接口)

| 属性 | 说明 |
|------|------|
| **职责** | 无参数启用接口 |
| **泛型参数** | 无 |
| **继承关系** | 无 |
| **实现的接口** | 无 |

```csharp
public interface IOnEnable
{
    public void OnEnable();
}
```

---

### IOnEnable<P1> (接口)

| 属性 | 说明 |
|------|------|
| **职责** | 带单个参数的启用接口 |
| **泛型参数** | P1 - 第一个参数 |
| **继承关系** | 无 |
| **实现的接口** | 无 |

```csharp
public interface IOnEnable<P1>
{
    public void OnEnable(P1 p1);
}
```

---

### IOnEnable<P1, P2> (接口)

| 属性 | 说明 |
|------|------|
| **职责** | 带两个参数的启用接口 |
| **泛型参数** | P1, P2 - 参数 |
| **继承关系** | 无 |
| **实现的接口** | 无 |

```csharp
public interface IOnEnable<P1, P2>
{
    public void OnEnable(P1 p1, P2 p2);
}
```

---

### IOnEnable<P1, P2, P3> (接口)

| 属性 | 说明 |
|------|------|
| **职责** | 带三个参数的启用接口 |
| **泛型参数** | P1, P2, P3 - 参数 |
| **继承关系** | 无 |
| **实现的接口** | 无 |

```csharp
public interface IOnEnable<P1, P2, P3>
{
    public void OnEnable(P1 p1, P2 p2, P3 p3);
}
```

---

### IOnEnable<P1, P2, P3, P4> (接口)

| 属性 | 说明 |
|------|------|
| **职责** | 带四个参数的启用接口 |
| **泛型参数** | P1, P2, P3, P4 - 参数 |
| **继承关系** | 无 |
| **实现的接口** | 无 |

```csharp
public interface IOnEnable<P1, P2, P3, P4>
{
    public void OnEnable(P1 p1, P2 p2, P3 p3, P4 p4);
}
```

---

## 接口详解

### OnEnable() 生命周期

**调用时机**: 每次 UI 窗口打开时（可调用多次）

**调用者**: `UIBaseContainer.AfterOnEnable()`

**调用流程**:
```csharp
// UIBaseContainer.AfterOnEnable
void AfterOnEnable()
{
    Walk((component) =>
    {
        if (component is IOnEnable a) a.OnEnable();  // ← 调用 OnEnable
        component.ActiveSelf = true;
        component.AfterOnEnable();
    });
    if (this is IUpdate)
    {
        TimerManager.Instance.Remove(ref timerId);
        timerId = TimerManager.Instance.NewFrameTimer(TimerType.ComponentUpdate, this);
    }
}
```

**与 Unity MonoBehaviour.OnEnable 的区别**:

| 特性 | Unity OnEnable | IOnEnable.OnEnable |
|------|---------------|-------------------|
| 调用时机 | GameObject 激活时 | UI 窗口打开时 |
| 参数支持 | 无 | 支持 0-4 个参数 |
| 调用顺序 | Unity 内部决定 | UI 框架控制 |

---

## OnCreate vs OnEnable

### 关键区别

| 特性 | OnCreate | OnEnable |
|------|----------|----------|
| **调用次数** | 只调用一次 | 每次打开都调用 |
| **用途** | 初始化组件引用、绑定事件 | 更新数据、重置状态 |
| **参数传递** | 首次打开时传递 | 每次打开都可传递 |
| **性能** | 只执行一次，可做较重初始化 | 频繁执行，应轻量 |

### 生命周期流程

```
首次打开:
  OnCreate() → OnEnable()
  
关闭后再次打开:
  OnEnable()  (不再调用 OnCreate)
  
销毁:
  OnDisable() → OnDestroy()
```

---

## 使用示例

### 示例 1: 无参数 OnEnable

```csharp
public class UILobbyView : UIBaseView, IOnCreate, IOnEnable
{
    private Text goldText;
    private Text diamondText;
    
    public void OnCreate()
    {
        // 只执行一次：初始化组件
        goldText = GetTransform().Find("GoldText").GetComponent<Text>();
        diamondText = GetTransform().Find("DiamondText").GetComponent<Text>();
    }
    
    public void OnEnable()
    {
        // 每次打开都执行：更新数据
        goldText.text = PlayerDataManager.Instance.Gold.ToString();
        diamondText.text = PlayerDataManager.Instance.Diamond.ToString();
    }
}
```

---

### 示例 2: 带参数的 OnEnable

```csharp
public class UIItemDetail : UIBaseView, IOnCreate, IOnEnable<ItemData>
{
    private Image itemIcon;
    private Text itemName;
    private Text itemDesc;
    
    public void OnCreate()
    {
        // 初始化组件
        itemIcon = GetTransform().Find("Icon").GetComponent<Image>();
        itemName = GetTransform().Find("Name").GetComponent<Text>();
        itemDesc = GetTransform().Find("Desc").GetComponent<Text>();
    }
    
    public void OnEnable(ItemData p1)
    {
        // 每次打开传入不同的物品数据
        itemIcon.sprite = ResourceHelper.LoadSprite(p1.IconPath);
        itemName.text = p1.Name;
        itemDesc.text = p1.Description;
    }
}

// 打开窗口时传递参数
await UIManager.Instance.OpenWindow<UIItemDetail, ItemData>(
    UIItemDetail.PrefabPath,
    selectedItemData
);
```

---

### 示例 3: 多参数 OnEnable

```csharp
public class UIAuctionResult : UIBaseView, IOnCreate, IOnEnable<List<ItemData>, bool>
{
    private Transform itemList;
    private Text totalCount;
    
    public void OnCreate()
    {
        itemList = GetTransform().Find("ItemList");
        totalCount = GetTransform().Find("TotalCount").GetComponent<Text>();
    }
    
    public void OnEnable(List<ItemData> p1, bool isWin)
    {
        // 清空旧数据
        for (int i = itemList.childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(itemList.GetChild(i).gameObject);
        }
        
        // 显示新数据
        foreach (var item in p1)
        {
            var itemObj = CreateItemView(item);
            itemObj.transform.SetParent(itemList, false);
        }
        
        totalCount.text = $"共获得 {p1.Count} 个物品";
        
        // 根据是否中标显示不同颜色
        totalCount.color = isWin ? Color.green : Color.gray;
    }
}

// 打开时传递多个参数
await UIManager.Instance.OpenWindow<UIAuctionResult, List<ItemData>, bool>(
    UIAuctionResult.PrefabPath,
    auctionItems,
    isWin
);
```

---

### 示例 4: OnEnable 中的动画

```csharp
public class UIPopupView : UIBaseView, IOnCreate, IOnEnable
{
    private Animator animator;
    
    public void OnCreate()
    {
        animator = GetTransform().GetComponent<Animator>();
    }
    
    public void OnEnable()
    {
        // 每次打开都播放弹出动画
        animator.Play("PopupOpen");
    }
}
```

---

## 阅读指引

### 建议的阅读顺序

1. **理解接口作用** - OnEnable 是每次打开时的更新
2. **对比 OnCreate** - 理解两者的区别和使用场景
3. **看无参数版本** - IOnEnable 基础接口
4. **看泛型版本** - 理解参数传递机制

### 最值得学习的技术点

1. **生命周期分离**: OnCreate(一次性) vs OnEnable(多次)
2. **参数传递**: 每次打开都可传递新参数
3. **数据更新**: 在 OnEnable 中更新 UI 数据
4. **性能优化**: OnEnable 应轻量，避免耗时操作

---

## 最佳实践

### ✅ 推荐做法

```csharp
// ✅ OnCreate 中初始化（只执行一次）
public void OnCreate()
{
    // 获取组件引用
    button = GetTransform().Find("Button").GetComponent<Button>();
    
    // 绑定事件监听
    button.onClick.AddListener(OnButtonClick);
    
    // 初始化数据容器
    itemList = new List<ItemData>();
}

// ✅ OnEnable 中更新（每次打开）
public void OnEnable()
{
    // 更新显示数据
    UpdateUI();
    
    // 重置状态
    ResetState();
    
    // 播放动画
    animator.Play("Open");
}
```

### ❌ 避免做法

```csharp
// ❌ 不要在 OnEnable 中重复绑定事件
public void OnEnable()
{
    // 错误：每次打开都绑定，导致事件重复触发
    button.onClick.AddListener(OnButtonClick);
}

// ✅ 正确：在 OnCreate 中绑定
public void OnCreate()
{
    button.onClick.AddListener(OnButtonClick);
}

// ❌ 不要在 OnEnable 中做耗时操作
public void OnEnable()
{
    // 错误：阻塞 UI 打开
    var data = LoadDataFromDisk();
}

// ✅ 正确：使用异步
public async ETTask OnEnableAsync()
{
    await LoadDataAsync();
}
```

---

## 与 OpenWindow 参数的关系

### 参数传递流程

```csharp
// 1. 打开窗口时传递参数
await UIManager.Instance.OpenWindow<UIAuctionView, int>(
    UIAuctionView.PrefabPath,
    levelId  // ← 参数
);

// 2. UIManager 内部调用
async ETTask<T> InnerOpenWindow<T, P1>(UIWindow target, P1 p1)
{
    // ...
    await AddWindowToStack(target, p1);  // ← 传递参数
}

async ETTask AddWindowToStack<P1>(UIWindow target, P1 p1)
{
    // ...
    var view = target.View;
    view.SetActive(true, p1);  // ← 调用 SetActive
}

// 3. UIBaseContainer 调用 OnEnable
public void SetActive(bool active, P1 p1)
{
    if (active)
    {
        if (this is IOnEnable<P1> enable)
        {
            enable.OnEnable(p1);  // ← 最终调用
        }
    }
}
```

---

## 相关文档

- [UIManager.cs.md](./UIManager.cs.md) - UI 管理器
- [IOnCreate.cs.md](./IOnCreate.cs.md) - 创建接口
- [IOnDisable.cs.md](./IOnDisable.cs.md) - 禁用接口
- [UIBaseContainer.cs.md](./UIBaseContainer.cs.md) - UI 容器基类

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
