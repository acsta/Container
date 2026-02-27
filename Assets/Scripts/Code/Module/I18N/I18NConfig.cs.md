# I18NConfig.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | I18NConfig.cs |
| **路径** | Assets/Scripts/Code/Module/I18N/I18NConfig.cs |
| **所属模块** | 框架层 → Code/Module/I18N |
| **文件职责** | 国际化配置数据结构 |

---

## 类/结构体说明

### I18NConfigCategory

| 属性 | 说明 |
|------|------|
| **职责** | 国际化配置表类别，管理多语言配置列表 |
| **泛型参数** | 无 |
| **继承关系** | 继承自 `ProtoObject` |
| **序列化** | `NinoSerialize` |

```csharp
[NinoSerialize]
public partial class I18NConfigCategory : ProtoObject
{
    // 国际化配置表类别
}
```

### I18NConfig

| 属性 | 说明 |
|------|------|
| **职责** | 单个国际化配置项 |
| **泛型参数** | 无 |
| **继承关系** | 继承自 `ProtoObject` |
| **序列化** | `NinoSerialize` |

```csharp
[NinoSerialize]
public partial class I18NConfig : ProtoObject
{
    // 国际化配置项
}
```

---

## 字段说明

### I18NConfigCategory 字段

| 名称 | 类型 | 序列化 ID | 说明 |
|------|------|----------|------|
| `dict` | `Dictionary<int, I18NConfig>` | `[NinoIgnore]` | ID 到配置的映射字典（运行时） |
| `list` | `List<I18NConfig>` | `1` | 配置列表（序列化） |

### I18NConfig 字段

| 名称 | 类型 | 序列化 ID | 说明 |
|------|------|----------|------|
| `Id` | `int` | `1` | 配置 ID（对应 I18NKey 枚举值） |
| `Key` | `string` | - | 索引标识（仅 NOT_UNITY 模式） |
| `Value` | `string` | `3` | 翻译后的文本内容 |

---

## 方法说明

### EndInit()

**签名**:
```csharp
public override void EndInit()
```

**职责**: 配置加载完成后初始化字典

**核心逻辑**:
```
1. 遍历 list 列表
2. 调用每个 config.EndInit()
3. 添加到 dict 字典（Id → Config）
4. 调用 AfterEndInit()
```

**调用者**: 配置加载完成后

---

### Get(int id)

**签名**:
```csharp
public I18NConfig Get(int id)
```

**职责**: 根据 ID 获取配置

**参数**:
- `id`: 配置 ID

**返回**: I18NConfig 实例（找不到返回 null）

**核心逻辑**:
```
1. 从 dict 字典查找
2. 如果未找到，记录错误日志
3. 返回配置
```

**调用者**: I18NManager

---

### Contain(int id)

**签名**:
```csharp
public bool Contain(int id)
```

**职责**: 检查是否包含指定 ID 的配置

**返回**: true=包含，false=不包含

---

### GetAll() / GetAllList() / GetOne()

**签名**:
```csharp
public Dictionary<int, I18NConfig> GetAll()
public List<I18NConfig> GetAllList()
public I18NConfig GetOne()
```

**职责**: 获取所有配置/列表/任意一个配置

---

## 配置表格式

### CSV 格式示例

```csv
Id,Value
1，确定
2，取消
3，返回
4，登录
5，注册
```

### JSON 格式示例

```json
{
  "list": [
    {"Id": 1, "Value": "确定"},
    {"Id": 2, "Value": "取消"},
    {"Id": 3, "Value": "返回"}
  ]
}
```

---

## 多语言配置

### 配置表命名

| 语言 | 配置表名 |
|------|---------|
| 中文 | Chinese |
| 英文 | English |
| 日文 | Japanese |
| 韩文 | Korean |

### 加载流程

```
I18NManager.SwitchLanguage(LangType.English)
    ↓
加载 ConfigEnglish 配置表
    ↓
填充 I18NConfigCategory.dict
    ↓
触发 OnLanguageChangeEvt
    ↓
UI 更新文本
```

---

## 使用示例

### 示例 1: 获取配置

```csharp
// 获取国际化配置表
I18NConfigCategory config = ConfigManager.Instance.GetConfig<I18NConfigCategory>("Chinese");

// 获取单个配置
I18NConfig item = config.Get((int)I18NKey.Global_Btn_Confirm);
string text = item.Value;  // "确定"
```

### 示例 2: 遍历所有配置

```csharp
// 遍历所有配置
foreach (var item in config.GetAllList())
{
    Log.Info($"ID:{item.Id}, Value:{item.Value}");
}
```

### 示例 3: 检查配置是否存在

```csharp
if (config.Contain((int)I18NKey.Global_Btn_Confirm))
{
    // 配置存在
}
else
{
    // 配置不存在，使用默认值
}
```

---

## 相关文档

- [I18NManager.cs.md](./I18NManager.cs.md) - 国际化管理器
- [I18NKey.cs.md](../Const/I18NKey.cs.md) - 国际化键
- [II18NConfig.cs.md](./II18NConfig.cs.md) - 国际化配置接口

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
