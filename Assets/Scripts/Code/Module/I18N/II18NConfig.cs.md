# II18NConfig.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | II18NConfig.cs |
| **路径** | Assets/Scripts/Code/Module/I18N/II18NConfig.cs |
| **所属模块** | 框架层 → Code/Module/I18N |
| **文件职责** | 国际化配置接口定义 |

---

## 接口说明

### II18NConfig

| 属性 | 说明 |
|------|------|
| **职责** | 定义获取国际化文本的接口 |
| **泛型参数** | 无 |

```csharp
public interface II18NConfig
{
    public string GetI18NText(LangType lang);
}
```

### II18NSwitchConfig

| 属性 | 说明 |
|------|------|
| **职责** | 定义带类型切换的国际化文本接口 |
| **继承关系** | 继承自 `II18NConfig` |

```csharp
public interface II18NSwitchConfig : II18NConfig
{
    public string GetI18NText(LangType lang, int type = 0);
}
```

---

## 方法说明

### GetI18NText(LangType lang)

**签名**:
```csharp
public string GetI18NText(LangType lang)
```

**职责**: 根据语言类型获取翻译文本

**参数**:
- `lang`: 语言类型

**返回**: 翻译后的文本

---

### GetI18NText(LangType lang, int type)

**签名**:
```csharp
public string GetI18NText(LangType lang, int type = 0)
```

**职责**: 根据语言类型和类型参数获取翻译文本

**参数**:
- `lang`: 语言类型
- `type`: 类型参数（用于多状态文本）

**返回**: 翻译后的文本

---

## 使用场景

### 场景 1: 配置表多语言

配置表中的文本字段实现 II18NConfig 接口，支持多语言：

```csharp
[NinoSerialize]
public class ItemConfig
{
    public int Id;
    public string Name;  // 物品名称
    
    // 实现 II18NConfig 接口（通过部分类）
    public string GetI18NText(LangType lang)
    {
        return I18NManager.Instance.I18NGetText(this);
    }
}
```

### 场景 2: 多状态文本

某些配置有多个状态的文本（如按钮的启用/禁用状态）：

```csharp
public class ButtonConfig : II18NSwitchConfig
{
    public string[] texts;  // [0]=正常，[1]=禁用
    
    public string GetI18NText(LangType lang, int type = 0)
    {
        return texts[type];
    }
}
```

---

## 使用示例

### 示例 1: 简单多语言

```csharp
// 配置实现接口
public class QuestConfig : II18NConfig
{
    public string Title;
    
    public string GetI18NText(LangType lang)
    {
        // 根据语言返回不同文本
        return I18NManager.Instance.I18NGetText(this);
    }
}

// 使用
QuestConfig config = GetQuestConfig(questId);
string title = I18NManager.Instance.I18NGetText(config);
```

### 示例 2: 多状态文本

```csharp
// 配置实现接口
public class TaskConfig : II18NSwitchConfig
{
    public string[] Descriptions;  // [0]=未完成，[1]=已完成
    
    public string GetI18NText(LangType lang, int type = 0)
    {
        return Descriptions[type];
    }
}

// 使用
TaskConfig config = GetTaskConfig(taskId);
string desc = I18NManager.Instance.I18NGetText(config, taskState);
```

### 示例 3: I18NManager 中的使用

```csharp
// I18NManager 内部方法
public string I18NGetText(II18NConfig config)
{
    return config.GetI18NText(CurLangType);
}

public string I18NGetText(II18NSwitchConfig config, int pos = 0)
{
    return config.GetI18NText(CurLangType, pos);
}
```

---

## 与 I18NKey 的区别

| 特性 | I18NKey | II18NConfig |
|------|---------|-------------|
| 类型 | enum | interface |
| 用途 | 固定文本键 | 配置表文本 |
| 灵活性 | 低（编译时确定） | 高（运行时配置） |
| 适用场景 | UI 按钮、提示等 | 物品名、任务描述等 |

---

## 相关文档

- [I18NManager.cs.md](./I18NManager.cs.md) - 国际化管理器
- [I18NConfig.cs.md](./I18NConfig.cs.md) - 国际化配置
- [I18NKey.cs.md](../Const/I18NKey.cs.md) - 国际化键

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
