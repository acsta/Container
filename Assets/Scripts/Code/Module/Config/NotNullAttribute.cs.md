# NotNullAttribute.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | NotNullAttribute.cs |
| **路径** | Assets/Scripts/Code/Module/Config/NotNullAttribute.cs |
| **所属模块** | 框架层 → Config (配置系统) |
| **文件职责** | 非空标记特性，用于标识配置字段不能为空 |

---

## 类/结构体说明

### NotNullAttribute

| 属性 | 说明 |
|------|------|
| **职责** | 标记特性，用于标识字段或属性不能为空，可用于配置验证 |
| **泛型参数** | 无 |
| **继承关系** | `System.Attribute` |
| **实现的接口** | 无 |

**设计模式**: 特性模式 (Attribute Pattern) - 使用元数据标记非空约束

```csharp
public class NotNullAttribute : Attribute
{
}
```

---

## 字段与属性

该类无额外字段，仅作为标记特性使用。

---

## 特性使用说明

### AttributeUsage (隐式)

该类未显式指定 `AttributeUsage`，因此使用默认值：
- 可应用于任何目标 (类、方法、属性、字段等)
- 不支持多次应用
- 不支持继承

---

## 使用示例

### 示例 1: 标记配置字段

```csharp
public class ItemConfig : ProtoObject
{
    public int Id { get; set; }
    
    [NotNull]  // 标记 Name 不能为空
    public string Name { get; set; }
    
    [NotNull]  // 标记 Description 不能为空
    public string Description { get; set; }
    
    public int Type { get; set; }  // 可为空
}
```

### 示例 2: 配置验证

```csharp
// 配置加载时验证 (伪代码)
public void ValidateConfig(object config)
{
    var properties = config.GetType().GetProperties();
    
    foreach (var prop in properties)
    {
        var notNullAttr = prop.GetCustomAttribute<NotNullAttribute>();
        if (notNullAttr != null)
        {
            var value = prop.GetValue(config);
            if (value == null)
            {
                throw new Exception($"配置字段 {prop.Name} 不能为空");
            }
        }
    }
}
```

### 示例 3: 与 Odin Inspector 集成

```csharp
// 如果使用 Odin Inspector，可结合使用
public class ItemConfig : ProtoObject
{
    [NotNull]
    [LabelText("物品名称")]
    public string Name { get; set; }
    
    [NotNull]
    [Multiline(3)]
    [LabelText("物品描述")]
    public string Description { get; set; }
}
```

---

## 与其他模块的交互

```mermaid
graph TD
    subgraph Attribute["特性系统"]
        NA[NotNullAttribute]
        A[Attribute]
    end
    
    subgraph Config["配置系统"]
        CM[ConfigManager]
        CV[ConfigValidator]
    end
    
    subgraph Configs["配置类"]
        IC[ItemConfig]
        TC[TaskConfig]
    end
    
    NA --|> A
    CV --> NA
    IC --> NA
    TC --> NA
    
    note right of NA "NotNullAttribute 用于标记<br/>不能为空的配置字段"
    
    style Attribute fill:#e1f5ff
    style Config fill:#fff4e1
    style Configs fill:#e8f5e9
```

**依赖关系**:
- **依赖**: `System.Attribute` (基础特性类)
- **被依赖**: 配置验证器 (如实现)

---

## 配置验证规范

### 验证时机

| 时机 | 说明 |
|------|------|
| **配置加载时** | 加载配置后立即验证 |
| **配置热更新时** | 热更新配置后验证 |
| **编辑器模式** | 在 Unity 编辑器中实时验证 |

### 验证规则

```csharp
// 验证规则
1. 标记 [NotNull] 的字段不能为 null
2. 对于字符串类型，空字符串 "" 通常也视为无效
3. 对于引用类型，null 视为无效
4. 对于值类型，[NotNull] 无意义 (值类型永不为 null)
```

---

## 阅读指引

### 建议的阅读顺序

1. **理解特性作用** - NotNullAttribute 用于标记非空约束
2. **看继承关系** - 继承自 System.Attribute
3. **了解使用方式** - 在字段或属性上添加 [NotNull] 标记
4. **查看验证流程** - 了解如何基于特性进行验证

### 最值得学习的技术点

1. **特性标记**: 使用 Attribute 实现元数据标记
2. **数据验证**: 基于特性进行配置数据验证
3. **约定约束**: 通过标记表达字段约束

---

## 相关文档

- [ConfigAttribute.cs.md](./ConfigAttribute.cs.md) - 配置类标记特性
- [ConfigManager.cs.md](./ConfigManager.cs.md) - 配置管理器
- [ProtoObject.cs.md](./ProtoObject.cs.md) - Protobuf 对象基类

---

*文档生成时间：2026-03-02 | OpenClaw AI 助手*
