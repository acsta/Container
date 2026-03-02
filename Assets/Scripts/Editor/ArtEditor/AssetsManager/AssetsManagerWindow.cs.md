# AssetsManagerWindow.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | AssetsManagerWindow.cs |
| **路径** | Assets/Scripts/Editor/ArtEditor/AssetsManager/AssetsManagerWindow.cs |
| **所属模块** | Editor → ArtEditor/AssetsManager |
| **文件职责** | 模型库管理窗口 - 可视化浏览和管理预制体资源 |
| **编译条件** | `#if ODIN_INSPECTOR` (需安装 Odin Inspector 插件) |
| **依赖插件** | Odin Inspector |

---

## 类/结构体说明

### AssetsManagerWindow

| 属性 | 说明 |
|------|------|
| **职责** | 提供三栏式 Editor 窗口，用于浏览、搜索、拖拽预制体资源 |
| **泛型参数** | 无 |
| **继承关系** | 继承 `EditorWindow` |
| **命名空间** | `TaoTie` |

**设计模式**: Editor 窗口 + 三栏布局 + 可拖拽分割 + 资源预览

```csharp
// Odin Editor 窗口
public class AssetsManagerWindow : EditorWindow
{
    private AssetsManagerConfig config;  // 配置数据
    
    // 滚动位置
    Vector2 scrollViewPos;   // 左栏 (类型)
    Vector2 scrollViewPos2;  // 右栏 (预制体)
    Vector2 scrollViewPos3;  // 中栏 (标签)
    
    // 可拖拽分割线
    float splitterPos = 100;   // 左栏宽度
    float splitterPos2 = 200;  // 中栏宽度
    
    // 资源列表
    private List<GameObject> prefabs = new List<GameObject>();
    
    // 当前选择
    private int curType = -1;
    private int curLabel = 0;
}
```

---

## 字段与属性

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `config` | `AssetsManagerConfig` | `private` | 资产管理配置数据 |
| `scrollViewPos` | `Vector2` | `private` | 左栏 (类型列表) 滚动位置 |
| `scrollViewPos2` | `Vector2` | `private` | 右栏 (预制体列表) 滚动位置 |
| `scrollViewPos3` | `Vector2` | `private` | 中栏 (标签列表) 滚动位置 |
| `splitterPos` | `float` | `private` | 左栏宽度 (可拖拽调整) |
| `splitterPos2` | `float` | `private` | 中栏宽度 (可拖拽调整) |
| `splitterRect` | `Rect` | `private` | 左分割线矩形区域 |
| `splitterRect2` | `Rect` | `private` | 右分割线矩形区域 |
| `dragging` | `bool` | `private` | 是否正在拖拽左分割线 |
| `dragging2` | `bool` | `private` | 是否正在拖拽右分割线 |
| `prefabs` | `List<GameObject>` | `private` | 当前筛选的预制体列表 |
| `temp` | `HashSet<string>` | `private` | 临时去重集合 |
| `curType` | `int` | `private` | 当前选中的类型索引 |
| `curLabel` | `int` | `private` | 当前选中的标签索引 |
| `type` | `string` | `private` | 当前类型名称缓存 |
| `label` | `string` | `private` | 当前标签名称缓存 |

---

## 方法说明

### Open()

**签名**:
```csharp
[MenuItem("Tools/工具/地编/模型库")]
private static void Open()
```

**职责**: 打开模型库管理窗口

**核心逻辑**:
```
1. 调用 GetWindow<AssetsManagerWindow>() 打开窗口
2. 设置窗口标题为"模型库"
3. 设置窗口大小 1200x600，居中显示
4. 初始化分割线位置 (左 100px, 中 200px)
```

**调用者**: Unity Editor 菜单 "Tools/工具/地编/模型库"

---

### OnGUI()

**签名**:
```csharp
private void OnGUI()
```

**职责**: 绘制三栏式窗口 UI

**核心逻辑**:
```
1. 加载或创建 AssetsManagerConfig 配置
2. 绘制水平布局 (三栏)
3. 左栏：类型选择 (SelectionGrid)
   - 从 config.Labels 提取所有类型
   - 支持单选
4. 左分割线：可拖拽调整宽度
5. 中栏：标签选择 (SelectionGrid)
   - 根据选中类型显示对应标签
   - 第一个选项为"全部"
6. 右分割线：可拖拽调整宽度
7. 右栏：预制体预览 (网格布局)
   - 调用 SearchPrefab() 获取预制体列表
   - 绘制预览缩略图
   - 支持拖拽到场景
   - 支持右键菜单 (删除)
8. 处理分割线拖拽事件
   - MouseDown: 开始拖拽
   - MouseDrag: 更新宽度
   - MouseUp: 结束拖拽
```

**调用者**: Unity Editor (窗口绘制时自动调用)

---

### GetPreview()

**签名**:
```csharp
private Texture2D GetPreview(GameObject obj, float height)
```

**职责**: 获取预制体的预览纹理

**核心逻辑**:
```
1. 检查当前滚动位置是否在可视范围内
2. 如果在可视范围 → 返回完整预览 (GetAssetPreview)
3. 如果不在可视范围 → 返回缩略图 (GetMiniThumbnail)
```

**优化**: 延迟加载预览，避免一次性加载所有缩略图导致卡顿

**调用者**: `OnGUI()` (绘制预制体按钮时)

---

### SearchPrefab()

**签名**:
```csharp
private void SearchPrefab(string type, string label)
```

**职责**: 根据类型和标签筛选预制体

**核心逻辑**:
```
1. 检查类型/标签是否变化 (缓存优化)
2. 如果变化 → 清空 prefabs 列表
3. 遍历 config.Labels:
   - 匹配类型 (或"全部")
   - 匹配标签 (或"全部")
   - 收集 Objects 列表
4. 如果是文件夹路径 → 加入 temp 集合
5. 如果是 GameObject → 直接添加到 prefabs
6. 如果有文件夹路径 → 搜索文件夹内的所有 Prefab
7. 加载并添加到 prefabs 列表
```

**调用者**: `OnGUI()`

---

## 窗口布局

```
┌─────────────────────────────────────────────────────────────┐
│  模型库                                          [×]        │
├──────────┬──────────────┬───────────────────────────────────┤
│          │              │                                   │
│  类型    │   标签       │        预制体预览 (网格布局)       │
│          │              │                                   │
│ [全部]   │  [全部]      │  ┌────┐ ┌────┐ ┌────┐ ┌────┐    │
│ [建筑]   │  [住宅]      │  │    │ │    │ │    │ │    │    │
│ [植被]   │  [商业]      │  │ 🏠  │ │ 🏢  │ │ 🏪  │ │ 🏭  │    │
│ [道具]   │  [工业]      │  │    │ │    │ │    │ │    │    │
│          │              │  └────┘ └────┘ └────┘ └────┘    │
│          │              │                                   │
│  │       │      │       │  ┌────┐ ┌────┐ ┌────┐ ┌────┐    │
│  │       │      │       │  │    │ │    │ │    │ │    │    │
│ 可       │      可       │  │ 🌲  │ │ 🌳  │ │ 🪑  │ │ 🛋️  │    │
│ 拖       │      拖       │  │    │ │    │ │    │ │    │    │
│ 拽       │      拽       │  └────┘ └────┘ └────┘ └────┘    │
│          │              │                                   │
│          │              │  ... (滚动加载更多)                │
└──────────┴──────────────┴───────────────────────────────────┘
   100px      200px           自适应宽度
```

---

## 使用示例

### 示例 1: 打开模型库

```csharp
// 1. 菜单：Tools/工具/地编/模型库
// 2. 窗口打开，显示三栏布局
// 3. 首次打开会自动创建 AssetsManagerConfig 配置
```

### 示例 2: 筛选预制体

```csharp
// 1. 左栏选择类型："建筑"
// 2. 中栏选择标签："住宅"
// 3. 右栏显示所有匹配的预制体
// 4. 滚动浏览预览缩略图
```

### 示例 3: 拖拽预制体到场景

```csharp
// 1. 在右栏找到目标预制体
// 2. 鼠标左键拖拽到 Hierarchy 或 Scene 视图
// 3. 释放鼠标，预制体实例化到场景中
```

### 示例 4: 删除预制体

```csharp
// 1. 右键点击预制体缩略图
// 2. 选择"删除"菜单项
// 3. 确认删除对话框
// 4. 预制体从项目和配置中移除
```

---

## 配置数据结构

### AssetsManagerConfig

配置文件路径：`Assets/AssetsPackage/Config/AssetsManagerConfig.asset`

```csharp
// 配置结构 (参考 AssetsManagerConfig.cs)
public class AssetsManagerConfig : ScriptableObject
{
    public const string ConfigPath = "Assets/AssetsPackage/Config/AssetsManagerConfig.asset";
    
    public List<LabelConfig> Labels;  // 类型标签列表
}

public class LabelConfig
{
    public string Label;              // 类型名称 (如"建筑")
    public List<CollectConfig> Collects;  // 子标签列表
}

public class CollectConfig
{
    public string Label;              // 子标签名称 (如"住宅")
    public List<Object> Objects;      // 资源对象列表 (文件夹或预制体)
}
```

---

## 功能特性

### ✅ 三栏布局

- **左栏**: 主类型选择 (建筑/植被/道具等)
- **中栏**: 子标签选择 (住宅/商业/工业等)
- **右栏**: 预制体预览 (网格布局，支持滚动)

### ✅ 可调整宽度

- 分割线支持鼠标拖拽调整
- 左栏最小 100px，最大到中栏 -100px
- 中栏最小左栏 +100px，最大窗口宽度 -150px

### ✅ 资源预览

- 延迟加载缩略图 (仅加载可视区域)
- 使用 Unity 内置 AssetPreview 系统
- 支持 100x100 网格布局

### ✅ 拖拽支持

- 支持拖拽预制体到场景
- 使用 DragAndDrop API
- 支持复制模式 (Copy)

### ✅ 右键菜单

- 右键点击预制体显示上下文菜单
- 支持删除操作 (带确认对话框)

### ✅ 缓存优化

- 类型/标签变化时才重新搜索
- 使用 HashSet 去重
- 避免重复加载

---

## 注意事项

### ⚠️ 编译条件

此文件使用条件编译，仅在安装 Odin Inspector 时可用：

```csharp
#if ODIN_INSPECTOR
// ... 代码 ...
#endif
```

### ⚠️ 配置依赖

窗口依赖 `AssetsManagerConfig` 配置文件：
- 首次打开自动创建默认配置
- 配置保存在 `Assets/AssetsPackage/Config/`
- 可在 Project 窗口编辑配置

### ⚠️ 性能优化

- 大量预制体时滚动可能卡顿
- 建议按类型/标签筛选后再浏览
- 缩略图延迟加载减少内存占用

---

## 相关文档

- [AssetsManagerConfig.cs.md](./Config/AssetsManagerConfig.cs.md) - 资产管理配置
- [LabelConfig.cs.md](./Config/LabelConfig.cs.md) - 标签配置
- [CollectConfig.cs.md](./Config/CollectConfig.cs.md) - 收集配置
- [MeshManager.cs.md](./MeshManager.cs.md) - 模型处理工具

---

*文档生成时间：2026-03-02 | Editor 工具文档*
