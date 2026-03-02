# FbxHelperWindow.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | FbxHelperWindow.cs |
| **路径** | Assets/Scripts/Editor/ArtEditor/Resource/FbxHelperWindow.cs |
| **所属模块** | Editor → ArtEditor → Resource |
| **文件职责** | FBX 动画工具窗口，支持从 FBX 导出 anim 动画文件并进行压缩优化 |

---

## 类/结构体说明

### FbxHelperWindow

| 属性 | 说明 |
|------|------|
| **职责** | Unity Editor 扩展窗口，提供 FBX 动画提取、压缩优化功能 |
| **泛型参数** | 无 |
| **继承关系** | 继承 `EditorWindow` |
| **实现的接口** | 无 |

**设计模式**: Editor 窗口 + 批量处理

```csharp
// 窗口打开方式
FbxHelperWindow.ShowWindow();
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `m_fbxFileInfo` | `List<FileInfo>` | `private` | 选中的 FBX 文件信息列表 |
| `m_paths` | `List<string>` | `private` | 选中的路径列表（文件夹/FBX/anim） |

---

## 方法说明（按重要程度排序）

### ShowWindow()

**签名**:
```csharp
public static void ShowWindow()
```

**职责**: 显示动画工具窗口

**核心逻辑**:
```
1. 创建 600x600 的窗口
2. 设置窗口标题为"动画工具"
3. 显示窗口
```

**调用者**: MenuItem 菜单项或代码调用

---

### OnGUI()

**签名**:
```csharp
void OnGUI()
```

**职责**: 绘制窗口界面

**核心逻辑**:
```
1. 显示使用说明标签
2. 获取当前选中路径 getSelectedPaths()
3. 过滤 FBX 文件 FilterFbxFile()
4. 显示选中的 FBX 文件（最多 20 个）
5. 提供"从 FBX 导出 anim 并压缩"按钮
6. 提供"压缩 anim 文件"按钮
```

**调用者**: Unity Editor 自动调用

---

### OnSelectionChange()

**签名**:
```csharp
void OnSelectionChange()
```

**职责**: 处理用户选择变化事件

**核心逻辑**:
```
1. 调用 Repaint() 重绘窗口
2. 自动更新选中的文件列表
```

**调用者**: Unity Editor 自动调用（用户改变选择时）

---

### getSelectedPaths()

**签名**:
```csharp
List<string> getSelectedPaths()
```

**职责**: 获取当前在 Project 窗口中选中的路径

**核心逻辑**:
```
1. 使用 Selection.GetFiltered 获取选中对象
2. 使用 AssetDatabase.GetAssetPath 转换为路径
3. 返回路径列表
```

**调用者**: `OnGUI()`

---

### FilterFbxFile(List<string> selectFile)

**签名**:
```csharp
private List<FileInfo> FilterFbxFile(List<string> selectFile)
```

**职责**: 从选中路径中过滤出所有 FBX 文件

**核心逻辑**:
```
1. 遍历选中路径
2. 如果是目录 → 递归获取所有*.FBX 文件
3. 如果是文件且扩展名为.fbx → 添加到列表
4. 返回 FBX 文件信息列表
```

**调用者**: `OnGUI()`

---

### HandleFBX(string sourcePath, string targetDir = null)

**签名**:
```csharp
public static void HandleFBX(string sourcePath, string targetDir = null)
```

**职责**: 处理 FBX 文件，导出动画并压缩

**核心逻辑**:
```
1. 判断 sourcePath 是目录还是文件
2. 目录 → 递归获取所有*.FBX 文件
3. 文件 → 直接添加到处理列表
4. 调用 ChangeFBXToAnimAndCompess() 批量处理
```

**调用者**: `OnGUI()` (点击"从 FBX 导出 anim 并压缩"按钮)

**被调用者**: `ChangeFBXToAnimAndCompess()`

---

### HandleAnim(string source_path)

**签名**:
```csharp
public static void HandleAnim(string source_path)
```

**职责**: 处理 anim 文件，执行压缩优化

**核心逻辑**:
```
1. 判断 source_path 是目录还是文件
2. 目录 → 获取所有*.anim 文件
3. 文件 → 直接添加到处理列表
4. 遍历调用 ReplaceAniClipAsset() 压缩每个 anim
```

**调用者**: `OnGUI()` (点击"压缩 anim 文件"按钮)

**被调用者**: `ReplaceAniClipAsset()`

---

### ChangeFBXToAnimAndCompess(List<string> filesInfos, string targetDir)

**签名**:
```csharp
public static void ChangeFBXToAnimAndCompess(List<string> filesInfos, string targetDir)
```

**职责**: 批量从 FBX 提取动画并压缩

**核心逻辑**:
```
1. 遍历 FBX 文件列表
2. 获取 ModelImporter
3. 跳过包含"_skin"的文件（蒙皮文件）
4. 确定目标目录（默认为 Animations 子目录）
5. 加载 FBX 中的所有 AnimationClip
6. 调用 CreateAniClipAsset() 创建压缩后的 anim 文件
7. 显示进度条
8. 完成后清除进度条
```

**调用者**: `HandleFBX()`

**被调用者**: `CreateAniClipAsset()`

---

### CreateAniClipAsset(AnimationClip kSrcAniClip, string strClipAssetPath)

**签名**:
```csharp
static private void CreateAniClipAsset(AnimationClip kSrcAniClip, string strClipAssetPath)
```

**职责**: 创建压缩后的动画文件

**核心逻辑**:
```
1. 克隆源动画片段 GameObject.Instantiate()
2. 调用 compess() 压缩动画
3. 检查目标路径是否已存在 anim 文件
4. 已存在 → 使用 EditorUtility.CopySerialized 替换
5. 不存在 → 创建新资源 AssetDatabase.CreateAsset()
6. 卸载未使用资源并保存
```

**调用者**: `ChangeFBXToAnimAndCompess()`

**被调用者**: `compess()`

---

### ReplaceAniClipAsset(string strClipAssetPath)

**签名**:
```csharp
static private void ReplaceAniClipAsset(string strClipAssetPath)
```

**职责**: 压缩现有的 anim 文件

**核心逻辑**:
```
1. 加载 anim 文件
2. 调用 compess() 压缩
3. 卸载未使用资源并保存
```

**调用者**: `HandleAnim()`

---

### compess(AnimationClip clip)

**签名**:
```csharp
public static void compess(AnimationClip clip)
```

**职责**: 压缩动画片段

**核心逻辑**:
```
1. 调用 optmizeAnimationScaleCurve() 优化 scale 曲线
2. 调用 optmizeAnimationFloat() 优化浮点精度
```

**调用者**: `CreateAniClipAsset()`, `ReplaceAniClipAsset()`

---

### optmizeAnimationScaleCurve(AnimationClip clip)

**签名**:
```csharp
static AnimationClip optmizeAnimationScaleCurve(AnimationClip clip)
```

**职责**: 优化动画的 scale 曲线，删除无用的 scale 关键帧

**核心逻辑**:
```
1. 获取所有曲线绑定 EditorCurveBinding
2. 遍历包含"scale"的曲线
3. 检查所有关键帧的值是否接近 1，切线是否接近 0
4. 如果是 → 删除该曲线（无 scale 变化）
5. 返回优化后的 clip
```

**调用者**: `compess()`

---

### optmizeAnimationFloat(AnimationClip clip)

**签名**:
```csharp
static AnimationClip optmizeAnimationFloat(AnimationClip clip)
```

**职责**: 优化动画浮点精度，保留 3 位小数

**核心逻辑**:
```
1. 获取所有曲线数据 AnimationClipCurveData
2. 遍历所有关键帧
3. 将 value/inTangent/outTangent 格式化为 3 位小数
4. 更新曲线数据
5. 返回优化后的 clip
```

**调用者**: `compess()`

---

### GetFileNames(string directoryPath, string searchPattern, bool isSearchChild)

**签名**:
```csharp
public static string[] GetFileNames(string directoryPath, string searchPattern, bool isSearchChild)
```

**职责**: 获取目录下指定扩展名的文件列表

**核心逻辑**:
```
1. 检查目录是否存在
2. 支持多扩展名（用|分隔）
3. isSearchChild = true → 递归搜索子目录
4. isSearchChild = false → 仅搜索当前目录
5. 返回文件路径数组
```

**调用者**: `HandleAnim()`

---

## Mermaid 流程图

### FBX 动画导出流程

```mermaid
flowchart TD
    A[ShowWindow] --> B[用户选中 FBX 文件/文件夹]
    B --> C[点击"从 FBX 导出 anim 并压缩"]
    C --> D[HandleFBX]
    D --> E{是目录？}
    E -->|是 | F[递归获取所有*.FBX]
    E -->|否 | G[直接添加到列表]
    F --> H[ChangeFBXToAnimAndCompess]
    G --> H
    H --> I[遍历 FBX 文件]
    I --> J[获取 ModelImporter]
    J --> K{包含"_skin"?}
    K -->|是 | L[跳过]
    K -->|否 | M[加载所有 AnimationClip]
    M --> N[CreateAniClipAsset]
    N --> O[克隆动画]
    O --> P[compess 压缩]
    P --> Q{目标文件存在？}
    Q -->|是 | R[CopySerialized 替换]
    Q -->|否 | S[CreateAsset 创建]
    R --> T[保存资源]
    S --> T
```

### 动画压缩流程

```mermaid
flowchart TD
    A[compess] --> B[optmizeAnimationScaleCurve]
    A --> C[optmizeAnimationFloat]
    B --> D[获取所有曲线绑定]
    D --> E{包含"scale"?}
    E -->|否 | F[跳过]
    E -->|是 | G[检查关键帧]
    G --> H{值≈1 且切线≈0?}
    H -->|是 | I[删除曲线]
    H -->|否 | F
    C --> J[获取所有曲线数据]
    J --> K[遍历关键帧]
    K --> L[格式化为 3 位小数]
    L --> M[更新曲线]
```

---

## 使用示例

### 打开窗口

```csharp
// 代码调用
FbxHelperWindow.ShowWindow();
```

### 从 FBX 导出动画

```
1. 在 Project 窗口选中 FBX 文件或包含 FBX 的文件夹
2. 在动画工具窗口点击"从 FBX 导出 anim 并压缩"
3. 自动在 Animations 目录下创建压缩后的.anim 文件
4. 跳过包含"_skin"的蒙皮文件
```

### 批量压缩 anim 文件

```
1. 在 Project 窗口选中.anim 文件或包含.anim 的文件夹
2. 在动画工具窗口点击"压缩 anim 文件"
3. 自动优化所有选中的 anim 文件
```

### 压缩效果

```csharp
// 优化前：
// - scale 曲线包含大量值为 1 的关键帧
// - 浮点精度为 6-7 位小数

// 优化后：
// - 删除无用的 scale 曲线
// - 浮点精度保留 3 位小数
// - 文件大小显著减小
```

---

## 相关文档链接

- [[ArtToolsWindow.cs.md]] - 美术资源分析工具
- [[DependWindow.cs.md]] - 依赖查找工具
- [[FindReferences.cs.md]] - 资源引用查找工具
- [[ProcessHelper.cs.md]] - 资源处理工具类

---

*文档生成时间：2026-03-02*
