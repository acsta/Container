# BMFontReader.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | BMFontReader.cs |
| **路径** | Assets/Scripts/Editor/ArtEditor/UGUIFont/BMFontReader.cs |
| **所属模块** | Editor → 美术编辑器 → UGUI 字体工具 |
| **文件职责** | BMFont 格式解析器，从字节数组加载 BMFont 的字形信息 |

---

## 类/结构体说明

### BMFontReader

| 属性 | 说明 |
|------|------|
| **职责** | 解析 BMFont 格式的字体文件 (.fnt)，提取字形 (glyph)、字距 (kerning)、公共信息 (common) 和纹理页 (page) 数据 |
| **泛型参数** | 无 |
| **继承关系** | 无继承 |
| **实现的接口** | 无 |

**设计模式**: 工具类模式 (静态方法集合)

**来源**: 基于 NGUI (Next-Gen UI kit) 的 BMFontReader，Copyright © 2011-2014 Tasharen Entertainment

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| 无 | - | - | 纯静态工具类，无实例字段 |

---

## 方法说明（按重要程度排序）

### GetString()

**签名**:
```csharp
static string GetString(string s)
```

**职责**: 从 key=value 格式的字符串中提取 value 部分

**核心逻辑**:
```
1. 查找 '=' 字符位置
2. 返回 '=' 之后的子字符串
3. 如果未找到 '='，返回空字符串
```

**示例**:
```csharp
GetString("lineHeight=64")  // 返回 "64"
GetString("file=\"texture.png\"")  // 返回 "\"texture.png\""
```

---

### GetInt()

**签名**:
```csharp
static int GetInt(string s)
```

**职责**: 从 key=value 格式的字符串中提取整数值

**核心逻辑**:
```
1. 调用 GetString() 获取 value 部分
2. 使用 int.TryParse() 解析为整数
3. 解析失败返回 0
```

**示例**:
```csharp
GetInt("id=13")  // 返回 13
GetInt("width=32")  // 返回 32
```

---

### Load()

**签名**:
```csharp
static public void Load(BMFont font, string name, byte[] bytes)
```

**职责**: 从字节数组加载 BMFont 数据

**参数**:
| 参数 | 类型 | 说明 |
|------|------|------|
| `font` | `BMFont` | 目标 BMFont 对象 |
| `name` | `string` | 字体名称 (用于错误提示) |
| `bytes` | `byte[]` | .fnt 文件的字节数据 |

**核心逻辑**:
```
1. 清空 BMFont 对象 font.Clear()
2. 使用 ByteReader 读取字节数据
3. 逐行解析 BMFont 格式:

   a) char 行 (字形定义):
      格式：char id=13 x=506 y=62 width=3 height=3 xoffset=-1 yoffset=50 xadvance=0 page=0 chnl=15
      - 检查 page 数量 (只支持单纹理)
      - 获取字形 ID
      - 创建/获取 BMGlyph 对象
      - 设置字形属性：x, y, width, height, offsetX, offsetY, advance, channel

   b) kerning 行 (字距调整):
      格式：kerning first=84 second=244 amount=-5
      - 获取 first 和 second 字符 ID
      - 获取 amount 字距值
      - 调用 glyph.SetKerning(first, amount)

   c) common 行 (公共信息):
      格式：common lineHeight=64 base=51 scaleW=512 scaleH=512 pages=1 ...
      - 设置 charSize (行高)
      - 设置 baseOffset (基线偏移)
      - 设置 texWidth/texHeight (纹理尺寸)
      - 检查 pages 数量 (必须为 1)

   d) page 行 (纹理页):
      格式：page id=0 file="textureName.png"
      - 提取文件名 (去掉引号和扩展名)
      - 设置 font.spriteName
```

**调用者**: `ArtistFont.BatchCreateArtistFont()`, 任何需要加载 BMFont 的代码

**被调用者**: `BMFont.Clear()`, `BMFont.GetGlyph()`, `ByteReader.ReadLine()`

---

## BMFont 格式示例

### 完整 .fnt 文件示例

```
info face="Arial" size=32 bold=0 italic=0 charset="" unicode=1 stretchH=100 smooth=1 aa=1 padding=1,1,1,1 spacing=1,1 outline=0
common lineHeight=40 base=32 scaleW=512 scaleH=512 pages=1 packed=0
page id=0 file="arial.png"
chars count=95
char id=32 x=0 y=0 width=0 height=0 xoffset=0 yoffset=32 xadvance=8 page=0 chnl=0
char id=65 x=1 y=1 width=24 height=24 xoffset=0 yoffset=8 xadvance=24 page=0 chnl=0
char id=66 x=26 y=1 width=22 height=24 xoffset=1 yoffset=8 xadvance=24 page=0 chnl=0
kernings count=2
kerning first=65 second=65 amount=-2
kerning first=65 second=86 amount=-3
```

### 字段说明

| 字段 | 说明 |
|------|------|
| `info` | 字体基本信息 (face, size, bold, italic 等) |
| `common` | 公共设置 (lineHeight, base, scaleW, scaleH) |
| `page` | 纹理页信息 (id, file) |
| `chars` | 字形总数 |
| `char` | 单个字形定义 (id, x, y, width, height, offset, advance) |
| `kernings` | 字距调整总数 |
| `kerning` | 字距调整 (first, second, amount) |

---

## 使用示例

### 示例 1: 从文件加载 BMFont

```csharp
// 读取 .fnt 文件
byte[] fontBytes = File.ReadAllBytes("Assets/Fonts/arial.fnt");

// 创建 BMFont 对象
BMFont bmFont = new BMFont();

// 加载数据
BMFontReader.Load(bmFont, "arial", fontBytes);

// 访问字形数据
foreach (var glyph in bmFont.glyphs)
{
    Debug.Log($"Char {glyph.index}: x={glyph.x}, y={glyph.y}");
}
```

### 示例 2: 从 TextAsset 加载

```csharp
// Unity 中从 TextAsset 加载
TextAsset fontAsset = Resources.Load<TextAsset>("Fonts/arial");
BMFont bmFont = new BMFont();
BMFontReader.Load(bmFont, fontAsset.name, fontAsset.bytes);
```

---

## 注意事项

1. **单纹理限制**: 只支持单纹理字体 (pages=1)，多纹理会报错
2. **Flash 兼容**: 使用 ByteReader 而非 System.IO 以兼容 Flash 平台
3. **错误处理**: 格式错误时会输出详细的 Debug.LogError
4. **字距 kerning**: 只记录 second 字符的字距信息

---

## 相关文档

- [BMFont.cs.md](./BMFont.cs.md) - BMFont 数据结构
- [BMGlyph.cs.md](./BMGlyph.cs.md) - 字形数据
- [ByteReader.cs.md](./ByteReader.cs.md) - 字节读取器
- [ArtistFont.cs.md](./ArtistFont.cs.md) - 艺术字体创建工具

---

*文档生成时间：2026-03-02 | OpenClaw AI 助手*
