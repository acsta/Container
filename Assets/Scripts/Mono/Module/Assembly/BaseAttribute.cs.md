# BaseAttribute.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|------|
| **文件名** | BaseAttribute.cs |
| **路径** | Assets/Scripts/Mono/Module/Assembly/BaseAttribute.cs |
| **所属模块** | 框架层 → Mono/Module/Assembly |
| **文件职责** | 定义基础特性类，所有自定义特性的基类 |

---

## 类/结构体说明

### BaseAttribute

| 属性 | 说明 |
|------|------|
| **职责** | 所有自定义特性的基类，用于标记特定类型的类 |
| **泛型参数** | 无 |
| **继承关系** | 继承 `System.Attribute` |
| **实现的接口** | 无 |

**设计模式**: 基类模式

```csharp
// 继承 BaseAttribute
[AttributeUsage(AttributeTargets.Class)]
public class TimerAttribute : BaseAttribute
{
    public int Type { get; }
    
    public TimerAttribute(int type)
    {
        this.Type = type;
    }
}

// 使用特性
[Timer(TimerType.Heartbeat)]
public class HeartbeatTimer : ATimer<NetworkManager>
{
    public override void Run(NetworkManager t)
    {
        t.SendHeartbeat();
    }
}
```

---

## 特性定义

### AttributeUsage

```csharp
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
```

**说明**:
- `AttributeTargets.Class`: 只能应用于类
- `AllowMultiple = true`: 同一个类可以应用多个此特性

---

## 继承关系

```
System.Attribute
    ↓
TaoTie.BaseAttribute
    ↓
各种自定义特性（TimerAttribute, ConfigAttribute, etc.）
```

---

## 使用示例

### 示例 1: 定义定时器特性

```csharp
[AttributeUsage(AttributeTargets.Class)]
public class TimerAttribute : BaseAttribute
{
    public int Type { get; }
    
    public TimerAttribute(int type)
    {
        this.Type = type;
    }
}

// 使用
[Timer(TimerType.ResetTimeScale)]
public class ResetTimeScale : ATimer<GameTimerManager>
{
    public override void Run(GameTimerManager t)
    {
        t.SetTimeScale(1);
    }
}
```

### 示例 2: 定义配置特性

```csharp
[AttributeUsage(AttributeTargets.Class)]
public class ConfigAttribute : BaseAttribute
{
}

// 使用
[Config]
public class LevelConfig : ProtoObject
{
    public int Id;
    public string Name;
}
```

### 示例 3: 扫描特性

```csharp
// AttributeManager 扫描所有标记了 BaseAttribute 子类的类型
var allTypes = AssemblyManager.Instance.GetTypes();

foreach (var item in allTypes)
{
    Type type = item.Value;
    
    // 获取所有 BaseAttribute 标记
    object[] objects = type.GetCustomAttributes(TypeInfo<BaseAttribute>.Type, true);
    
    foreach (object o in objects)
    {
        Type attrType = o.GetType();
        // 记录类型和特性的关系
        types.Add(attrType, type);
    }
}
```

---

## 设计要点

### 为什么需要 BaseAttribute？

1. **统一标记**: 所有自定义特性都继承 BaseAttribute
2. **便于扫描**: AttributeManager 只需扫描 BaseAttribute 类型
3. **解耦**: 不依赖具体的特性类型
4. **可扩展**: 新增特性无需修改扫描逻辑

### AllowMultiple 的意义

```csharp
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
```

**用途**: 允许一个类有多个相同的特性

**示例**:
```csharp
[EventHandler("Event1")]
[EventHandler("Event2")]
public class MultiEventHandler : BaseAttribute
{
    // 可以处理多个事件
}
```

### 与 AttributeManager 配合

```
BaseAttribute (基类)
    ↑
TimerAttribute, ConfigAttribute, ... (具体特性)
    ↓
AttributeManager (扫描所有标记了这些特性的类)
```

---

## 相关文档

- [AttributeManager.cs.md](./AttributeManager.cs.md) - 属性管理器（扫描 BaseAttribute）
- [AssemblyManager.cs.md](./AssemblyManager.cs.md) - 程序集管理器
- [TimerAttribute.cs.md](../Timer/TimerAttribute.cs.md) - 定时器特性示例
- [ConfigAttribute.cs.md](../../Code/Module/Config/ConfigAttribute.cs.md) - 配置特性示例

---

*文档生成时间：2026-02-28 | OpenClaw AI 助手*
