# ConfigAttribute.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | ConfigAttribute.cs |
| **路径** | Assets/Scripts/Code/Module/Config/ConfigAttribute.cs |
| **所属模块** | 框架层 → Code/Module/Config |
| **文件职责** | 定义配置类标记特性，用于标识哪些类是配置类型 |

---

## 类/结构体说明

### ConfigAttribute

| 属性 | 说明 |
|------|------|
| **职责** | 标记一个类为配置类型，供 ConfigManager 扫描和加载 |
| **泛型参数** | 无 |
| **继承关系** | 继承 `BaseAttribute` |
| **实现的接口** | 无 |

**设计模式**: 标记特性（Marker Attribute）

```csharp
// 使用方式
[Config]
public class LevelConfig : ProtoObject
{
    public int Id;
    public string Name;
}
```

---

## 特性定义

### AttributeUsage

```csharp
[AttributeUsage(AttributeTargets.Class)]
```

**说明**: 此特性只能应用于类（Class），不能应用于方法、字段等。

---

## 继承关系

```
System.Attribute
    ↓
TaoTie.BaseAttribute
    ↓
TaoTie.ConfigAttribute
```

**BaseAttribute**: ET 框架的基础特性类，提供特性系统的统一基类。

---

## 工作原理

### ConfigManager 扫描流程

```mermaid
flowchart TD
    A[ConfigManager.LoadAsync] --> B[AttributeManager.GetTypes]
    B --> C[扫描所有程序集]
    C --> D[查找标记 [Config] 的类]
    D --> E[返回配置类型列表]
    E --> F[遍历加载每个配置]
```

### 代码示例

```csharp
// ConfigManager.LoadAsync() 内部
List<Type> types = AttributeManager.Instance.GetTypes(TypeInfo<ConfigAttribute>.Type);
// 返回所有标记了 [Config] 的类型

foreach (Type type in types)
{
    this.LoadOneInThread(type, configBytes);
}
```

---

## 使用示例

### 示例 1: 定义配置类

```csharp
[Config]  // 标记为配置类
public class LevelConfig : ProtoObject
{
    public int Id;
    public string Name;
    public int Difficulty;
}
```

### 示例 2: 定义配置列表类

```csharp
[Config]  // 配置管理器会加载这个类
public class LevelConfigCategory : ProtoObject, IMerge
{
    public List<LevelConfig> LevelList = new List<LevelConfig>();
    
    // 支持合并多个配置源
    public void Merge(object o)
    {
        var other = o as LevelConfigCategory;
        LevelList.AddRange(other.LevelList);
    }
}
```

### 示例 3: 加载配置

```csharp
// 游戏启动时加载所有配置
await ConfigManager.Instance.LoadAsync();

// 访问配置
var levelConfig = ConfigManager.Instance.GetConfig<LevelConfigCategory>();
foreach (var level in levelConfig.LevelList)
{
    Debug.Log($"关卡：{level.Name}");
}
```

---

## 设计要点

### 为什么使用特性标记？

1. **自动发现**: 无需手动注册配置类
2. **解耦**: 配置类定义与加载逻辑分离
3. **可扩展**: 新增配置类无需修改加载代码
4. **类型安全**: 通过 Type 系统保证类型正确

### 与其他特性的关系

```csharp
// 常见组合
[Config]                          // 标记为配置类
public class ItemConfig : ProtoObject
{
    [HideInInspector]             // Unity Inspector 隐藏
    public int InternalId;
    
    [Range(0, 100)]               // Odin/Unity 属性
    public int Rarity;
}
```

---

## 相关文档

- [ConfigManager.cs.md](./ConfigManager.cs.md) - 配置管理器（扫描 [Config] 特性）
- [AttributeManager.cs.md](../../Mono/Module/Assembly/AttributeManager.cs.md) - 特性管理器
- [BaseAttribute.cs.md](../../Mono/Module/Assembly/BaseAttribute.cs.md) - 基础特性类
- [ProtoObject.cs.md](./ProtoObject.cs.md) - 配置对象基类

---

*文档生成时间：2026-02-28 | OpenClaw AI 助手*
