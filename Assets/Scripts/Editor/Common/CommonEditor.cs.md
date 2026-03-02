# CommonEditor.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | CommonEditor.cs |
| **路径** | Assets/Scripts/Editor/Common/CommonEditor.cs |
| **所属模块** | Editor 工具 → Common |
| **文件职责** | 通用编辑器工具菜单，提供场景启动、目录创建等快捷操作 |

---

## 类/结构体说明

### CommonEditor

| 属性 | 说明 |
|------|------|
| **职责** | 提供 Unity Editor 菜单项，用于快速启动场景、创建子目录等通用操作 |
| **泛型参数** | 无 |
| **继承关系** | 无继承 |
| **实现的接口** | 无 |

**设计模式**: 静态工具类 + 菜单命令

```csharp
// 静态工具类，无需实例化
public class CommonEditor
{
    [MenuItem("Tools/帮助/启动场景 #_b")]
    static void ChangeInitScene() { ... }
}
```

---

## 字段与属性

本类无成员字段，所有方法均为静态方法。

---

## 方法说明（按重要程度排序）

### ChangeInitScene()

**签名**:
```csharp
[MenuItem("Tools/帮助/启动场景 #_b")]
static void ChangeInitScene()
```

**职责**: 打开初始场景 (Init.unity)

**核心逻辑**:
```
1. 使用 EditorSceneManager.OpenScene() 打开 InitScene
2. 快捷键：Shift + Ctrl + B (#_b)
```

**调用者**: Unity Editor 菜单

**使用示例**:
```csharp
// 通过菜单：Tools → 帮助 → 启动场景
// 或快捷键：Shift + Ctrl + B
```

---

### CreateArtSubFolder()

**签名**:
```csharp
[MenuItem("Assets/工具/创建子目录")]
public static void CreateArtSubFolder()
```

**职责**: 为选中的资源创建标准美术子目录结构

**核心逻辑**:
```
1. 获取当前选中的资源 GUIDs
2. 遍历每个选中资源
3. 调用 FileHelper.CreateArtSubFolder() 创建子目录
4. 保存并刷新 AssetDatabase
```

**调用者**: Unity Editor 右键菜单

**被调用者**: `FileHelper.CreateArtSubFolder()`

**使用示例**:
```csharp
// 在 Project 窗口选中一个文件夹
// 右键 → 工具 → 创建子目录
// 自动创建标准美术目录结构 (如 Model, Texture, Animation 等)
```

---

### Test()

**签名**:
```csharp
[MenuItem("Tools/帮助/测试")]
static void Test()
```

**职责**: 测试功能 - 批量复制动画剪辑

**核心逻辑**:
```
1. 搜索 Assets/Fox 目录下的所有 Model 类型资源
2. 加载每个资源为 AnimationClip
3. 实例化剪辑并保存到新路径
```

**调用者**: Unity Editor 菜单

**备注**: 这是一个测试/开发工具，用于批量处理动画资源。

---

## 菜单命令总览

| 菜单路径 | 快捷键 | 功能 |
|---------|--------|------|
| Tools/帮助/启动场景 | Shift+Ctrl+B | 打开 InitScene.unity |
| Assets/工具/创建子目录 | 无 | 为选中资源创建标准子目录 |
| Tools/帮助/测试 | 无 | 测试功能 (批量复制动画) |

---

## 使用示例

### 示例 1: 启动初始场景

```csharp
// 方法 1: 菜单操作
// Tools → 帮助 → 启动场景

// 方法 2: 快捷键
// Shift + Ctrl + B
```

### 示例 2: 创建美术子目录

```csharp
// 1. 在 Project 窗口选中目标文件夹
// 2. 右键菜单 → 工具 → 创建子目录
// 3. 自动创建标准目录结构:
//    - Model/
//    - Texture/
//    - Animation/
//    - Material/
//    - Prefab/
```

---

## 相关文档

- [FileHelper.cs.md](./Helper/FileHelper.cs.md) - 文件工具类 (CreateArtSubFolder)
- [EditorSceneManager](https://docs.unity3d.com/ScriptReference/SceneManagement.EditorSceneManager.html) - Unity 场景管理 API
- [AssetDatabase](https://docs.unity3d.com/ScriptReference/AssetDatabase.html) - Unity 资源数据库 API

---

*文档生成时间：2026-03-02 | OpenClaw AI 助手*
