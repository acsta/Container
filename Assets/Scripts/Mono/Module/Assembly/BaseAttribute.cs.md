# BaseAttribute.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | BaseAttribute.cs |
| **路径** | Assets/Scripts/Mono/Module/Assembly/BaseAttribute.cs |
| **所属模块** | 框架层 → Mono/Module/Assembly |
| **命名空间** | `TaoTie` |
| **文件职责** | 提供框架特性的基类，用于标记可被 AttributeManager 扫描的类型 |

---

## 类说明

### BaseAttribute

| 属性 | 说明 |
|------|------|
| **职责** | 所有框架特性的基类，继承自 `System.Attribute` |
| **继承关系** | `Attribute` |
| **特性用法** | `AttributeTargets.Class, AllowMultiple = true` |

**设计模式**: 标记特性模式

```csharp
// 定义派生特性
public class ConfigAttribute : BaseAttribute
{
}

// 标记类
[Config]
public class ItemConfig { }
```

---

## 使用示例

### 示例 1: 配置类标记

```csharp
// 定义配置特性
public class ConfigAttribute : BaseAttribute
{
}

// 标记配置类
[Config]
public class ItemConfig : IConfig
{
    public int Id;
    public string Name;
}
```

### 示例 2: 定时器标记

```csharp
// 定义定时器特性
[AttributeUsage(AttributeTargets.Class)]
public class TimerAttribute : BaseAttribute
{
    public int Type;
}

// 标记定时器类
[Timer(Type = 1001)]
public class MyTimer : ITimer
{
    public void Handle(object obj) { }
}
```

### 示例 3: 组件标记

```csharp
// 定义组件特性
[AttributeUsage(AttributeTargets.Class)]
public class ComponentAttribute : BaseAttribute
{
}

// 标记组件类
[Component]
public class MovementComponent : IComponent { }
```

---

## 扫描机制

```csharp
// AttributeManager 扫描所有带有 BaseAttribute 派生特性的类型
foreach (var type in AssemblyManager.Instance.GetTypes())
{
    if (type.IsAbstract) continue;
    
    // 获取所有 BaseAttribute 派生特性
    object[] attrs = type.GetCustomAttributes(typeof(BaseAttribute), true);
    
    foreach (var attr in attrs)
    {
        // 记录 特性类型 → 被标记类型 的映射
        AttributeManager.Instance.types.Add(attr.GetType(), type);
    }
}
```

---

## 相关文档

- [AttributeManager.cs.md](./AttributeManager.cs.md) - 属性管理器
- [AssemblyManager.cs.md](./AssemblyManager.cs.md) - 程序集管理器

---

*文档生成时间：2026-03-02 | OpenClaw AI 助手*
