# AssetProcess.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | AssetProcess.cs |
| **路径** | Assets/Scripts/Editor/Common/DragonBones/AssetProcess.cs |
| **所属模块** | Editor 工具 → Common → DragonBones |
| **文件职责** | DragonBones 资源后处理器，自动处理骨骼动画资源导入 |

---

## 类/结构体说明

### AssetProcess

| 属性 | 说明 |
|------|------|
| **职责** | 继承 AssetPostprocessor，在资源导入时自动处理 DragonBones 骨骼动画资源 |
| **泛型参数** | 无 |
| **继承关系** | `AssetPostprocessor` |
| **实现的接口** | 无 |

**设计模式**: 事件处理器 + 自动资源处理

```csharp
// Unity 资源后处理器
public class AssetProcess : AssetPostprocessor
{
    // 资源导入后自动调用
    public static void OnPostprocessAllAssets(...) { ... }
}
```

### SubTextureClass (内部结构)

| 属性 | 说明 |
|------|------|
| **职责** | 表示 DragonBones 纹理图集中的一个子纹理 |
| **字段** | `name`, `x`, `y`, `width`, `height`, `frameX`, `frameY`, `frameWidth`, `frameHeight` |

### TextureDataClass (内部类)

| 属性 | 说明 |
|------|------|
| **职责** | 表示 DragonBones 纹理数据 |
| **字段** | `name`, `imagePath`, `width`, `height`, `SubTexture` (子纹理列表) |

---

## 字段与属性

本类无成员字段，所有方法均为静态方法。

---

## 方法说明（按重要程度排序）

### OnPostprocessAllAssets()

**签名**:
```csharp
public static void OnPostprocessAllAssets(
    string[] imported,
    string[] deletedAssets,
    string[] movedAssets,
    string[] movedFromAssetPaths
)
```

**职责**: 资源导入后处理入口，自动处理 DragonBones 资源

**核心逻辑**:
```
1. 遍历所有导入的资源
2. 按扩展名分类:
   - .png → 图片资源
   - .json → 判断是图集 (_tex.json) 还是骨骼数据
   - .dbbin → 重命名为 .bytes (骨骼数据)
   - .bytes → 检查是否为骨骼数据
3. 如果有骨骼数据，处理关联的纹理图集
4. 调用 ProcessTextureAtlasData() 补充图集宽高信息
```

**调用者**: Unity 资源导入系统 (自动调用)

**被调用者**: `IsValidDragonBonesData()`, `ProcessTextureAtlasData()`

---

### IsValidDragonBonesData()

**签名**:
```csharp
public static bool IsValidDragonBonesData(TextAsset asset)
```

**职责**: 判断 TextAsset 是否为有效的 DragonBones 骨骼数据

**核心逻辑**:
```
1. 检查文件名是否包含 "_ske"
2. 检查内容是否为 "DBDT" (DragonBones Data Binary)
3. 检查内容是否包含 "\"armature\":" (JSON 格式骨骼数据)
```

**参数**:
| 参数 | 类型 | 说明 |
|------|------|------|
| `asset` | `TextAsset` | 要检查的文本资源 |

**返回值**: `bool` - 是否为有效的 DragonBones 数据

**调用者**: `OnPostprocessAllAssets()`

---

### ProcessTextureAtlasData()

**签名**:
```csharp
static void ProcessTextureAtlasData(List<string> atlasPaths)
```

**职责**: 处理纹理图集数据，补充缺失的宽高信息

**核心逻辑**:
```
1. 遍历所有图集 JSON 路径
2. 解析 JSON 为 TextureDataClass
3. 如果 width/height 为 0:
   - 加载对应的 PNG 图片
   - 获取图片实际宽高
   - 更新 JSON 数据并保存
4. 刷新 AssetDatabase
```

**参数**:
| 参数 | 类型 | 说明 |
|------|------|------|
| `atlasPaths` | `List<string>` | 图集 JSON 文件路径列表 |

**调用者**: `OnPostprocessAllAssets()`

**被调用者**: `LoadPNG()`

---

### LoadPNG()

**签名**:
```csharp
static Texture2D LoadPNG(string filePath)
```

**职责**: 从文件路径加载 PNG 图片

**核心逻辑**:
```
1. 检查文件是否存在
2. 读取文件字节
3. 创建 Texture2D 并加载图片
4. 返回 Texture2D
```

**参数**:
| 参数 | 类型 | 说明 |
|------|------|------|
| `filePath` | `string` | PNG 文件路径 |

**返回值**: `Texture2D` - 加载的纹理，失败返回 null

**调用者**: `ProcessTextureAtlasData()`

---

## DragonBones 资源处理流程

```mermaid
sequenceDiagram
    participant Unity as Unity Editor
    participant AP as AssetProcess
    participant FS as 文件系统
    participant DB as DragonBones 数据

    Unity->>AP: OnPostprocessAllAssets()
    
    loop 遍历导入的资源
        AP->>FS: 检查文件扩展名
        alt .png
            AP->>AP: 加入 imagePaths
        alt .json
            AP->>AP: 判断类型 (图集/骨骼)
        alt .dbbin
            AP->>FS: 重命名为 .bytes
        end
    end
    
    alt 存在骨骼数据
        AP->>AP: 查找关联的图集
        AP->>AP: ProcessTextureAtlasData()
        loop 遍历图集
            AP->>DB: 解析 JSON
            alt 宽高为 0
                AP->>FS: 加载 PNG 图片
                AP->>DB: 更新宽高并保存
            end
        end
        AP->>Unity: AssetDatabase.Refresh()
    end
```

---

## DragonBones 资源类型

| 扩展名 | 类型 | 说明 |
|--------|------|------|
| `.png` | 纹理图片 | 骨骼动画使用的精灵图 |
| `_tex.json` | 纹理图集 | 描述子纹理在 PNG 中的位置 |
| `_ske.json` | 骨骼数据 (JSON) | 骨骼、动画、插槽等数据 |
| `.dbbin` | 骨骼数据 (二进制) | 二进制格式的骨骼数据 |
| `.bytes` | 骨骼数据 (通用) | 重命名后的二进制数据 |

---

## 使用示例

### 示例 1: 导入 DragonBones 资源

```
// 将以下文件放入 Assets 目录:
- character.png        (纹理图片)
- character_tex.json   (纹理图集)
- character_ske.json   (骨骼数据)

// Unity 自动触发 AssetProcess.OnPostprocessAllAssets()
// 自动处理资源，补充图集宽高信息
```

### 示例 2: 查看处理后的图集数据

```json
// 处理前 (width/height 为 0)
{
  "name": "character",
  "width": 0,
  "height": 0,
  "SubTexture": [...]
}

// 处理后 (自动填充实际宽高)
{
  "name": "character",
  "width": 1024,
  "height": 1024,
  "SubTexture": [...]
}
```

---

## 注意事项

### ⚠️ 文件命名规范

DragonBones 导出资源时需遵循命名规范:
- 纹理图集：`{name}_tex.json`
- 骨骼数据：`{name}_ske.json` 或 `{name}.dbbin`

### ⚠️ 资源依赖

骨骼数据、图集、图片必须放在同一目录下，否则无法自动关联。

### ⚠️ 性能考虑

大量资源导入时，后处理会同步执行，可能导致导入时间较长。

---

## 相关文档

- [UnityArmatureEditor.cs.md](./UnityArmatureEditor.cs.md) - DragonBones 骨骼编辑器
- [UnityDragonBonesData](https://github.com/DragonBones/DragonBonesUNITY) - DragonBones Unity 运行时
- [AssetPostprocessor](https://docs.unity3d.com/ScriptReference/AssetPostprocessor.html) - Unity 资源后处理器 API

---

*文档生成时间：2026-03-02 | OpenClaw AI 助手*
