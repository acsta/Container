# IOnWidthPaddingChange.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | IOnWidthPaddingChange.cs |
| **路径** | Assets/Scripts/Code/Module/UI/IOnWidthPaddingChange.cs |
| **所属模块** | 框架层 → Code/Module/UI |
| **文件职责** | 定义屏幕安全区域宽度变化通知接口，用于刘海屏/挖孔屏适配 |

---

## 接口说明

### IOnWidthPaddingChange

**职责**: 标记接口，表示实现类需要响应屏幕安全区域宽度变化

**使用场景**: 
- 当设备安全区域变化时（如刘海屏、挖孔屏、折叠屏）
- UI 需要动态调整布局以适应新的安全区域

```csharp
// 标记接口，无方法定义
public interface IOnWidthPaddingChange
{
}
```

---

### IOnTopWidthPaddingChange

**职责**: 继承自 IOnWidthPaddingChange，标记需要响应顶部安全区域变化的 UI

**使用场景**:
- 状态栏高度变化
- 刘海屏顶部区域适配

```csharp
public interface IOnTopWidthPaddingChange : IOnWidthPaddingChange
{
}
```

---

### IOnMiniGameWidthPaddingChange

**职责**: 继承自 IOnWidthPaddingChange，针对小游戏平台的特殊适配接口

**平台差异**:
```csharp
public interface IOnMiniGameWidthPaddingChange : IOnWidthPaddingChange
{
#if UNITY_WEBGL_TT
    // 抖音小游戏平台：按钮间距 80 像素
    public const int ButtonSpace = 80;
#else
    // 其他平台：无额外间距
    public const int ButtonSpace = 0;
#endif
}
```

**平台说明**:
| 平台 | ButtonSpace | 说明 |
|------|-------------|------|
| 抖音小游戏 (UNITY_WEBGL_TT) | 80 | 需要预留按钮区域 |
| 其他平台 | 0 | 无需额外间距 |

---

## 使用示例

### 实现安全区域变化响应

```csharp
// 在 UIManager 中检测安全区域变化
public void ResetSafeArea()
{
    var safeArea = SystemInfoHelper.safeArea;
    SetWidthPadding(Mathf.Max(safeArea.xMin, SystemInfoHelper.screenWidth - safeArea.xMax));
    
    // 通知所有实现 IOnWidthPaddingChange 的 UI 组件
    NotifyWidthPaddingChange();
}

// UI 组件实现接口
public class SafeAreaView : UIBaseView, IOnWidthPaddingChange, IOnCreate
{
    private UIText txtContent;
    
    public void OnCreate()
    {
        txtContent = AddComponent<UIText>("txtContent");
    }
    
    // 当安全区域变化时，UIManager 会调用此方法（需配合 Messager 事件）
    public void OnWidthPaddingChanged(float newPadding)
    {
        // 调整布局
        var rect = GetRectTransform();
        rect.offsetMax = new Vector2(-newPadding, rect.offsetMax.y);
        rect.offsetMin = new Vector2(newPadding, rect.offsetMin.y);
    }
}
```

### 小游戏平台适配

```csharp
public class MiniGameButton : UIBaseView, IOnMiniGameWidthPaddingChange, IOnCreate
{
    private UIButton btnAction;
    
    public void OnCreate()
    {
        btnAction = AddComponent<UIButton>("btnAction");
        
#if UNITY_WEBGL_TT
        // 抖音小游戏：调整按钮位置避开特殊区域
        var rect = btnAction.GetRectTransform();
        rect.anchoredPosition = new Vector2(0, -ButtonSpace);
#endif
    }
}
```

---

## 与其他模块的交互

```mermaid
graph TB
    subgraph UI["UI 模块"]
        IOPC[IOnWidthPaddingChange]
        IOTPC[IOnTopWidthPaddingChange]
        IOMPC[IOnMiniGameWidthPaddingChange]
        UM[UIManager]
    end
    
    subgraph System["系统信息"]
        SIH[SystemInfoHelper<br/>safeArea]
    end
    
    subgraph Platform["平台定义"]
        TT[UNITY_WEBGL_TT<br/>抖音小游戏]
    end
    
    UM --> SIH
    UM -.通知.-> IOPC
    IOTPC --|> IOPC
    IOMPC --|> IOPC
    IOMPC --> TT
    
    note right of IOPC "标记接口，用于<br/>安全区域变化通知"
    
    style UI fill:#e1f5ff
    style System fill:#fff4e1
    style Platform fill:#e8f5e9
```

---

## 学习重点与陷阱

### ✅ 学习重点

1. **标记接口**: IOnWidthPaddingChange 本身无方法，用于标识需要响应安全区域变化的 UI
2. **平台适配**: IOnMiniGameWidthPaddingChange 提供平台特定的常量定义
3. **继承关系**: 三个接口形成继承链，子类接口自动拥有父接口标记

### ⚠️ 陷阱与注意事项

| 问题 | 说明 | 解决方案 |
|------|------|----------|
| **接口无方法** | IOnWidthPaddingChange 是标记接口，无回调方法 | 需配合 Messager 事件或 UIManager 通知机制 |
| **平台宏定义** | UNITY_WEBGL_TT 仅在抖音小游戏构建时生效 | 使用 #if 预处理指令包裹平台特定代码 |
| **ButtonSpace 使用** | 仅 IOnMiniGameWidthPaddingChange 有此常量 | 不要在其他接口中访问此常量 |

---

## 相关文档

- [UIManager.cs.md](./UIManager.cs.md) - UI 管理器（包含 ResetSafeArea 方法）
- [UIBaseView.cs.md](./UIBaseView.cs.md) - UI 视图基类
- [SystemInfoHelper](../Helper/SystemInfoHelper.cs.md) - 系统信息助手（待创建）

---

*文档由 OpenClaw AI 助手自动生成 | 基于静态代码分析*
