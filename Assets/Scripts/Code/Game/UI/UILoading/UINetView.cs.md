# UINetView.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | UINetView.cs |
| **路径** | Assets/Scripts/Code/Game/UI/UILoading/UINetView.cs |
| **所属模块** | 游戏层 → Code/Game/UI/UILoading |
| **文件职责** | 网络状态 UI 视图，用于显示网络连接状态提示 |

---

## 类/结构体说明

### UINetView

| 属性 | 说明 |
|------|------|
| **职责** | 显示网络连接状态提示（如"正在连接服务器"、"网络断开"等） |
| **泛型参数** | 无 |
| **继承关系** | 继承 `UIBaseView` 类 |
| **实现的接口** | `IOnCreate`, `IOnEnable` |

**设计模式**: 简单组件模式

```csharp
// 使用方式
// 通过 UIManager 打开
var netView = await UIManager.Instance.OpenWindow<UINetView>(UINetView.PrefabPath, UILayerNames.TipLayer);
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `PrefabPath` | `string` | `public static` | 预制体路径："UI/UILoading/Prefabs/UINetView.prefab" |

---

## 方法说明

### OnCreate()

**签名**:
```csharp
public void OnCreate()
```

**职责**: 创建 UI 组件（当前为空实现）

**调用者**: `UIManager`（窗口创建时）

---

### OnEnable()

**签名**:
```csharp
public void OnEnable()
```

**职责**: UI 启用时的初始化（当前为空实现）

**调用者**: `UIManager`（窗口启用时）

---

## 使用示例

### 打开网络状态提示

```csharp
// 打开 UINetView
var netView = await UIManager.Instance.OpenWindow<UINetView>(
    UINetView.PrefabPath, 
    UILayerNames.TipLayer
);
```

---

## 相关文档链接

- [UIBaseView.cs.md](../../../Module/UI/UIBaseView.cs.md) - UI 基类视图
- [UIManager.cs.md](../../../Module/UI/UIManager.cs.md) - UI 管理器

---

*文档生成时间：2026-03-02*
