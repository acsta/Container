# ArtistFont.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | ArtistFont.cs |
| **路径** | Assets/Scripts/Editor/ArtEditor/UGUIFont/ArtistFont.cs |
| **所属模块** | Editor → 美术编辑器 → UGUI 字体工具 |
| **文件职责** | 批量创建艺术字体工具，从 BMFont (.fnt) 文件生成 Unity Font 资源 |

---

## 类/结构体说明

### ArtistFont

| 属性 | 说明 |
|------|------|
| **职责** | 提供静态方法批量创建艺术字体，将 BMFont 格式转换为 Unity 可用的 Font 资源 |
| **泛型参数** | 无 |
| **继承关系** | 无继承 |
| **实现的接口** | 无 |

**设计模式**: 工具类模式 (静态方法集合)

```csharp
// 使用方式
[MenuItem("Tools/Batch Create Artist Font")]
public static void CreateFont()
{
    ArtistFont.BatchCreateArtistFont();
}
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| 无 | - | - | 纯静态工具类，无实例字段 |

---

## 方法说明（按重要程度排序）

### BatchCreateArtistFont()

**签名**:
```csharp
public static void BatchCreateArtistFont()
```

**职责**: 从选中的 .fnt 文件批量创建 Unity Font 资源

**核心逻辑**:
```
1. 获取选中的对象，验证是否为 .fnt 文件
2. 解析文件路径和文件名
3. 创建新的 Font 资源 (.fontsettings)
4. 加载 BMFont 文本文件 (.fnt)
5. 使用 BMFontReader 解析 BMFont 数据
6. 为每个字形创建 CharacterInfo:
   - 设置字符索引
   - 计算 UV 坐标 (归一化到纹理空间)
   - 设置顶点偏移和尺寸
   - 设置字符 advance (字间距)
7. 将 CharacterInfo 数组赋值给 Font
8. 加载对应的纹理文件 (.png)
9. 创建材质 (使用 GUI/Custom Text Shader)
10. 将材质赋值给 Font
11. 保存资源并刷新 AssetDatabase
```

**调用者**: Editor 菜单命令

**被调用者**: `BMFontReader.Load()`, `AssetDatabase.CreateAsset()`, `EditorUtility.SetDirty()`

---

## UV 坐标计算

### 计算公式

```csharp
// UV 坐标转换 (从像素空间到归一化纹理空间)
info.uv.x = (float)bmInfo.x / (float)mbFont.texWidth;
info.uv.y = 1 - (float)bmInfo.y / (float)mbFont.texHeight;  // Y 轴翻转
info.uv.width = (float)bmInfo.width / (float)mbFont.texWidth;
info.uv.height = -1f * (float)bmInfo.height / (float)mbFont.texHeight;  // 高度为负

// 顶点偏移
info.vert.x = (float)bmInfo.offsetX;
info.vert.y = (float)bmInfo.offsetY - (float)bmInfo.height / 2;
info.vert.width = (float)bmInfo.width;
info.vert.height = (float)bmInfo.height;

// 字间距
info.width = (float)bmInfo.advance;
```

---

## 使用示例

### 示例 1: 批量创建艺术字体

```csharp
// 1. 在 Project 窗口选中一个 .fnt 文件
// 2. 执行菜单：Tools → Batch Create Artist Font (假设菜单路径)
// 3. 工具自动生成:
//    - xxx.fontsettings (字体资源)
//    - xxx.mat (字体材质)
```

### 示例 2: 代码调用

```csharp
// 在 Editor 脚本中调用
[MenuItem("Tools/Font Tools/Create Artist Font")]
public static void CreateArtistFont()
{
    ArtistFont.BatchCreateArtistFont();
}
```

---

## 依赖关系

### 输入文件

| 文件类型 | 说明 |
|----------|------|
| `*.fnt` | BMFont 格式的字体描述文件 |
| `*.png` | 对应的字体纹理图 (文件名在 .fnt 中指定) |

### 输出文件

| 文件类型 | 说明 |
|----------|------|
| `*.fontsettings` | Unity Font 资源 |
| `*.mat` | 字体材质 (使用 GUI/Custom Text Shader) |

### 依赖类

| 类名 | 作用 |
|------|------|
| `BMFont` | BMFont 数据结构 |
| `BMFontReader` | BMFont 文件解析器 |
| `BMGlyph` | 字形数据 |

---

## 注意事项

1. **文件选择**: 必须选中 .fnt 文件才能执行
2. **纹理路径**: 从 .fnt 文件中读取 spriteName 字段获取纹理名
3. **Shader 依赖**: 需要项目中有 "GUI/Custom Text Shader"
4. **Y 轴翻转**: UV 坐标 Y 轴需要翻转 (BMFont 与 Unity 坐标系不同)
5. **高度为负**: UV 高度设为负值以正确渲染

---

## 相关文件

- [BMFont.cs.md](./BMFont.cs.md) - BMFont 数据结构
- [BMFontReader.cs.md](./BMFontReader.cs.md) - BMFont 文件解析器
- [BMGlyph.cs.md](./BMGlyph.cs.md) - 字形数据
- [FontSubsetEditor.cs.md](./FontSubsetEditor.cs.md) - 字体裁剪工具

---

*文档生成时间：2026-03-02 | OpenClaw AI 助手*
