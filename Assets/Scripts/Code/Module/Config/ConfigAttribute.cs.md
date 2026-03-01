# ConfigAttribute.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | ConfigAttribute.cs |
| **路径** | Assets/Scripts/Code/Module/Config/ConfigAttribute.cs |
| **所属模块** | 框架层 → Config (配置系统) |
| **文件职责** | 配置类标记特性，用于标识可自动加载的配置类 |

---

## 类/结构体说明

### ConfigAttribute

| 属性 | 说明 |
|------|------|
| **职责** | 标记特性，用于标识配置类，使 ConfigManager 能够自动扫描和加载 |
| **泛型参数** | 无 |
| **继承关系** | `BaseAttribute` |
| **实现的接口** | 无 |

**设计模式**: 特性模式 (Attribute Pattern) - 使用元数据标记配置类

```csharp
[AttributeUsage(AttributeTargets.Class)]
public class ConfigAttribute : BaseAttribute
{
}
```

---

## 字段与属性

该类无额外字段，仅作为标记特性使用。

---

## 特性使用说明

### AttributeUsage

```csharp
[AttributeUsage(AttributeTargets.Class)]
```

| 参数 | 值 | 说明 |
|------|-----|------|
| `AttributeTargets` | `Class` | 仅可应用于类 |

---

## 使用示例

### 示例 1: 标记配置类

```csharp
// 使用 ConfigAttribute 标记配置类
[Config]
public class ItemConfigCategory : ProtoObject, IMerge<ItemConfigCategory>
{
    // 配置数据
}

// 或者使用完整类名
[ConfigAttribute]
public class TaskConfigCategory : ProtoObject, IMerge<TaskConfigCategory>
{
    // 配置数据
}
```

### 示例 2: ConfigManager 自动扫描

```csharp
// ConfigManager 会扫描所有标记了 [Config] 的类
// 并自动加载对应的配置文件

// 扫描过程 (伪代码):
var configTypes = Assembly.GetTypes()
    .Where(t => t.GetCustomAttribute<ConfigAttribute>() != null);

foreach (var type in configTypes)
{
    // 加载配置
    var config = await LoadConfig(type);
}
```

---

## 与其他模块的交互

```mermaid
graph TD
    subgraph Attribute["特性系统"]
        CA[ConfigAttribute]
        BA[BaseAttribute]
    end
    
    subgraph Config["配置系统"]
        CM[ConfigManager]
        AM[AttributeManager]
    end
    
    subgraph Configs["配置类"]
        IC[ItemConfigCategory]
        TC[TaskConfigCategory]
    end
    
    CA --|> BA
    CM --> AM
    AM --> CA
    IC --> CA
    TC --> CA
    
    note right of CA "ConfigAttribute 用于标记<br/>可自动加载的配置类"
    
    style Attribute fill:#e1f5ff
    style Config fill:#fff4e1
    style Configs fill:#e8f5e9
```

**依赖关系**:
- **依赖**: `BaseAttribute` (基础特性类)
- **被依赖**: `ConfigManager`, `AttributeManager` (扫描和加载)

---

## 配置类规范

### 标准配置类结构

```csharp
[Config]  // 必须标记
public class XXXConfigCategory : ProtoObject, IMerge<XXXConfigCategory>
{
    // 配置数据字段
    public Dictionary<int, XXXConfig> Dict { get; set; }
    
    // IMerge 实现
    public void Merge(XXXConfigCategory source)
    {
        // 合并逻辑
    }
}
```

### 配置项结构

```csharp
[Config]
public class ItemConfig : ProtoObject
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Type { get; set; }
    // ... 其他字段
}
```

---

## 阅读指引

### 建议的阅读顺序

1. **理解特性作用** - ConfigAttribute 用于标记配置类
2. **看继承关系** - 继承自 BaseAttribute
3. **了解使用方式** - 在配置类上添加 [Config] 标记
4. **查看扫描流程** - 了解 ConfigManager 如何扫描和加载

### 最值得学习的技术点

1. **特性标记**: 使用 Attribute 实现元数据标记
2. **自动扫描**: 通过反射扫描标记的类
3. **约定优于配置**: 标记后自动加载，无需手动注册

---

## 相关文档

- [BaseAttribute.cs.md](../../Mono/Module/Assembly/BaseAttribute.cs.md) - 基础特性类
- [AttributeManager.cs.md](../../Mono/Module/Assembly/AttributeManager.cs.md) - 属性扫描管理器
- [ConfigManager.cs.md](./ConfigManager.cs.md) - 配置管理器
- [ProtoObject.cs.md](./ProtoObject.cs.md) - Protobuf 对象基类

---

*文档生成时间：2026-03-02 | OpenClaw AI 助手*
