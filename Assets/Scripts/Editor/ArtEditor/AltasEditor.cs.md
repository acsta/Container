# AltasEditor.cs 文档

> **文件路径**: `Assets/Scripts/Editor/ArtEditor/AltasEditor.cs`  
> **命名空间**: `TaoTie`  
> **文档生成时间**: 2026-03-02  
> **文件类型**: Unity 编辑器工具

---

## 📋 文件信息表

| 属性 | 值 |
|------|------|
| **类名** | `AltasEditor` |
| **所在程序集** | Editor |
| **依赖命名空间** | `UnityEditor`, `UnityEngine`, `System.IO` |
| **功能分类** | 美术资源编辑工具 |
| **编辑器菜单** | `Tools/工具/UI/*` |

---

## 🎯 类说明

**核心职责**: 提供 Unity 编辑器中 UI 资源批量处理工具的菜单入口。

**解决的核心问题**: 
- 批量设置图片格式为 PNG
- 自动生成 UI 图集
- 搜索和批量替换 Sprite
- 查找未使用的图片资源
- 检查预制体中丢失的 Image 组件
- 创建和裁剪图片字体

**如果没有这个模块**: 美术资源处理需要手动操作，效率低下且容易出错。

---

## 📦 字段与属性

| 字段名 | 类型 | 说明 |
|--------|------|------|
| (无实例字段) | - | 该类为静态工具类，无实例字段 |

---

## 🔧 方法说明

### 静态菜单方法

#### SettingPNG()
```csharp
[MenuItem("Tools/工具/UI/设置图片", false, 31)]
public static void SettingPNG()
```
**功能**: 批量设置选中图片为 PNG 格式  
**调用**: `AtlasHelper.SettingPNG()`  
**菜单路径**: `Tools/工具/UI/设置图片`  
**优先级**: 31

---

#### ClearAllAtlasAndGenerate()
```csharp
[MenuItem("Tools/工具/UI/生成图集", false, 32)]
public static void ClearAllAtlasAndGenerate()
```
**功能**: 清除所有旧图集并重新生成  
**流程**:
1. 开始资源编辑批处理
2. 调用 `AtlasHelper.GeneratingAtlas()`
3. 捕获异常并记录日志
4. 停止批处理并保存资源

**菜单路径**: `Tools/工具/UI/生成图集`  
**优先级**: 32

---

#### ReplaceImage()
```csharp
[MenuItem("Tools/工具/UI/搜索或批量替换 Sprite", false, 502)]
public static void ReplaceImage()
```
**功能**: 打开 Sprite 搜索和批量替换窗口  
**窗口尺寸**: 900x600  
**菜单路径**: `Tools/工具/UI/搜索或批量替换 Sprite`  
**优先级**: 502

---

#### CheckUnUseImage()
```csharp
[MenuItem("Tools/工具/UI/查找未使用的图片", false, 503)]
public static void CheckUnUseImage()
```
**功能**: 打开未使用图片查找窗口  
**窗口尺寸**: 900x600  
**菜单路径**: `Tools/工具/UI/查找未使用的图片`  
**优先级**: 503

---

#### CheckLossImage()
```csharp
[MenuItem("Tools/工具/UI/检查丢失 image", false, 504)]
public static void CheckLossImage()
```
**功能**: 检查预制体中丢失的 Image 组件  
**窗口尺寸**: 900x600  
**菜单路径**: `Tools/工具/UI/检查丢失 image`  
**优先级**: 504

---

#### CreateArtFont()
```csharp
[MenuItem("Tools/工具/UI/创建图片字体", false, 500)]
[MenuItem("Assets/工具/UI/创建图片字体", false, 203)]
public static void CreateArtFont()
```
**功能**: 批量创建图片字体  
**调用**: `ArtistFont.BatchCreateArtistFont()`  
**菜单路径**: 
- `Tools/工具/UI/创建图片字体`
- `Assets/工具/UI/创建图片字体`

---

#### FontSubset()
```csharp
[MenuItem("Tools/工具/UI/裁剪字体", false, 501)]
public static void FontSubset()
```
**功能**: 打开字体裁剪工具窗口  
**窗口尺寸**: 900x600  
**菜单路径**: `Tools/工具/UI/裁剪字体`  
**优先级**: 501

---

#### ResourceAnalysis()
```csharp
[MenuItem("Tools/工具/TA/资源分析输出 excel", false, 202)]
public static void ResourceAnalysis()
```
**功能**: 分析资源并输出 Excel 报告  
**调用**: `ResourceCheckTool.ResourceAnalysis()`  
**菜单路径**: `Tools/工具/TA/资源分析输出 excel`  
**优先级**: 202

---

#### OpenWindow()
```csharp
[MenuItem("Tools/工具/TA/资源可视化窗口", false, 208)]
public static void OpenWindow()
```
**功能**: 打开资源可视化工具窗口  
**调用**: `ArtToolsWindow.OpenWindow()`  
**菜单路径**: `Tools/工具/TA/资源可视化窗口`  
**优先级**: 208

---

#### ShowFbxToolWindow()
```csharp
[MenuItem("Tools/工具/TA/Fbx 压缩工具", false, 54)]
[MenuItem("Assets/工具/TA/Fbx 压缩工具", false, 54)]
public static void ShowFbxToolWindow()
```
**功能**: 打开 FBX 模型压缩工具  
**调用**: `FbxHelperWindow.ShowWindow()`  
**菜单路径**: 
- `Tools/工具/TA/Fbx 压缩工具`
- `Assets/工具/TA/Fbx 压缩工具`

---

#### OpenDependWindow()
```csharp
[MenuItem("Tools/工具/TA/依赖查找", false, 208)]
public static void OpenDependWindow()
```
**功能**: 打开资源依赖查找窗口  
**调用**: `DependWindow.ShowWindow()`  
**菜单路径**: `Tools/工具/TA/依赖查找`

---

#### SetLightMap()
```csharp
[MenuItem("Tools/工具/TA/设置场景贴图格式", false, 208)]
public static void SetLightMap()
```
**功能**: 批量设置场景光照贴图格式  
**调用**: `AtlasHelper.SetSceneTextures()`  
**菜单路径**: `Tools/工具/TA/设置场景贴图格式`

---

#### ClearSelectionAtlasAndGenerate()
```csharp
[MenuItem("Assets/工具/UI/生成图集", false, 400)]
static void ClearSelectionAtlasAndGenerate()
```
**功能**: 根据选中目录生成图集  
**逻辑**:
1. 获取选中的资源 GUID 列表
2. 判断目录类型 (UIItem/UICloth/AssetsPackage 子目录)
3. 调用 `AtlasHelper.SetImagesFormat()` 或 `AtlasHelper.GeneratingAtlasByDir()`

**菜单路径**: `Assets/工具/UI/生成图集`  
**优先级**: 400

---

#### CopyPath()
```csharp
[MenuItem("Assets/复制相对路径", false, 500)]
static void CopyPath()
```
**功能**: 复制选中资源的相对路径到剪贴板  
**格式**: 移除 `AssetsPackage/` 前缀的路径  
**菜单路径**: `Assets/复制相对路径`

---

#### CopyName()
```csharp
[MenuItem("Assets/复制名称", false, 500)]
static void CopyName()
```
**功能**: 复制选中资源的文件名 (无扩展名) 到剪贴板  
**排序**: 按路径排序后输出  
**菜单路径**: `Assets/复制名称`

---

## 🔄 核心流程图

```mermaid
graph TB
    subgraph Menu["编辑器菜单"]
        M1[设置图片]
        M2[生成图集]
        M3[替换 Sprite]
        M4[查找未使用图片]
        M5[检查丢失 image]
        M6[创建图片字体]
        M7[裁剪字体]
        M8[资源分析]
        M9[资源可视化]
        M10[FBX 压缩]
    end
    
    subgraph Helpers[" Helper 类"]
        AH[AtlasHelper]
        AF[ArtistFont]
        RCT[ResourceCheckTool]
        ATW[ArtToolsWindow]
        FHW[FbxHelperWindow]
        DW[DependWindow]
    end
    
    M1 --> AH
    M2 --> AH
    M3 --> RI[ReplaceImage 窗口]
    M4 --> CUI[CheckUnuseImage 窗口]
    M5 --> CEI[CheckEmptyImage 窗口]
    M6 --> AF
    M7 --> FS[FontSubsetEditor 窗口]
    M8 --> RCT
    M9 --> ATW
    M10 --> FHW
    
    note right of Helpers "所有工具方法都委托<br/>给对应的 Helper 类处理"
    
    style Menu fill:#e1f5ff
    style Helpers fill:#fff4e1
```

---

## 💡 使用示例

### 批量设置图片格式
```
1. 在 Project 窗口选中图片目录
2. 点击菜单 `Tools/工具/UI/设置图片`
3. 所有选中图片将被设置为 PNG 格式
```

### 生成 UI 图集
```
1. 确保 UI 资源已按规范组织 (UIItem/UICloth 目录)
2. 点击菜单 `Tools/工具/UI/生成图集`
3. 等待图集生成完成 (查看 Console 日志)
```

### 替换 Sprite
```
1. 点击菜单 `Tools/工具/UI/搜索或批量替换 Sprite`
2. 在弹出窗口中输入搜索条件
3. 选择目标 Sprite 进行批量替换
```

### 复制资源相对路径
```
1. 在 Project 窗口选中资源
2. 右键点击 `Assets/复制相对路径`
3. 路径已复制到剪贴板 (格式：AssetsPackage/UIItem/xxx.png)
```

---

## 🔗 相关文档链接

| 文档 | 说明 |
|------|------|
| [AtlasHelper.cs](../Atlas/AtlasHelper.cs.md) | 图集处理核心逻辑 |
| [ArtistFont.cs](../UGUIFont/ArtistFont.cs.md) | 图片字体创建工具 |
| [ResourceCheckTool.cs](../Resource/ResourceCheckTool.cs.md) | 资源分析工具 |
| [ArtToolsWindow.cs](../Resource/ArtToolsWindow.cs.md) | 资源可视化窗口 |
| [FbxHelperWindow.cs](../Resource/FbxHelperWindow.cs.md) | FBX 压缩工具窗口 |

---

## ⚠️ 注意事项

| 问题 | 说明 | 解决方案 |
|------|------|----------|
| **批量操作耗时** | 大量资源处理可能卡顿 | 工具会自动批处理，耐心等待 |
| **窗口尺寸固定** | 所有工具窗口固定 900x600 | 无法调整，如需更大窗口需修改源码 |
| **菜单冲突** | 部分菜单有重复路径 | 注意区分 `Tools/` 和 `Assets/` 前缀 |

---

*文档由 OpenClaw AI 助手自动生成 | 基于静态代码分析*
