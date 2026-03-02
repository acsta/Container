# DependWindow.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | DependWindow.cs |
| **路径** | Assets/Scripts/Editor/ArtEditor/Resource/DependWindow.cs |
| **所属模块** | Editor → ArtEditor → Resource |
| **文件职责** | 依赖查找工具窗口，使用 Odin Inspector 展示资源依赖关系 |

---

## 类/结构体说明

### DependWindow

| 属性 | 说明 |
|------|------|
| **职责** | Unity Editor 扩展窗口，提供可视化资源依赖查找功能 |
| **泛型参数** | 无 |
| **继承关系** | 继承 `OdinEditorWindow` |
| **实现的接口** | 无 |

**设计模式**: Odin Inspector 窗口 + 属性驱动 UI

```csharp
// 窗口打开方式
DependWindow.ShowWindow();
```

**依赖条件**: 需要 Odin Inspector 插件 (`#if ODIN_INSPECTOR`)

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `target` | `Object` | `public` | 目标资源对象，用户选择要分析的资源 |
| `depends` | `List<Object>` | `public` | 依赖资源列表，存储查找结果 |

---

## 方法说明（按重要程度排序）

### ShowWindow()

**签名**:
```csharp
public static void ShowWindow()
```

**职责**: 显示依赖查找窗口

**核心逻辑**:
```
1. 创建 600x600 的窗口
2. 设置窗口标题为"依赖查找"
3. 显示窗口
```

**调用者**: MenuItem 菜单项或代码调用

---

### Dependent()

**签名**:
```csharp
[Button("查找")]
public void Dependent()
```

**职责**: 执行依赖查找，填充 depends 列表

**核心逻辑**:
```
1. 清空 depends 列表
2. 获取 target 的资源路径 AssetDatabase.GetAssetPath(target)
3. 获取所有依赖路径 AssetDatabase.GetDependencies(path, true)
4. 遍历依赖路径，加载资源对象并添加到 depends 列表
```

**Odin 特性**:
- `[Button("查找")]` - 在 Inspector 中显示为按钮

**调用者**: 用户点击按钮

---

## Mermaid 流程图

### 依赖查找流程

```mermaid
flowchart TD
    A[ShowWindow] --> B[窗口显示]
    B --> C[用户选择 target 资源]
    C --> D[点击"查找"按钮]
    D --> E[Dependent 方法]
    E --> F[清空 depends 列表]
    F --> G[获取资源路径]
    G --> H[GetDependencies 获取所有依赖]
    H --> I[遍历依赖路径]
    I --> J[LoadAssetAtPath 加载资源]
    J --> K[添加到 depends 列表]
    K --> L[Odin 自动展示列表]
```

---

## 使用示例

### 打开窗口

```csharp
// 代码调用
DependWindow.ShowWindow();
```

### 查找资源依赖

```
1. 在窗口中拖拽目标资源到 target 字段
2. 点击"查找"按钮
3. depends 列表自动填充所有依赖资源
4. Odin Inspector 自动展示列表内容
```

### 示例场景

```csharp
// 查找 Prefab 的依赖
// target: Assets/AssetsPackage/Characters/Player.prefab
// 点击"查找"后，depends 列表包含:
// - 引用的 Material
// - 引用的 Texture
// - 引用的 AnimatorController
// - 引用的 ScriptableObject
// - 等所有依赖资源
```

---

## 相关文档链接

- [[ArtToolsWindow.cs.md]] - 美术资源分析工具
- [[FindReferences.cs.md]] - 资源引用查找工具
- [[Finddependent.cs.md]] - 依赖分析工具
- [[ResourceCheckTool.cs.md]] - 资源检查工具

---

*文档生成时间：2026-03-02*
