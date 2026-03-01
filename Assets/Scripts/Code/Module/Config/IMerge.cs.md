# IMerge.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | IMerge.cs |
| **路径** | Assets/Scripts/Code/Module/Config/IMerge.cs |
| **所属模块** | 框架层 → Config (配置系统) |
| **文件职责** | 合并接口定义，用于配置数据的热更新合并 |

---

## 类/结构体说明

### IMerge

| 属性 | 说明 |
|------|------|
| **职责** | 合并接口，定义配置对象的合并方法，用于热更新时合并新旧配置 |
| **泛型参数** | 无 |
| **继承关系** | 无 |
| **实现的接口** | 无 |

**设计模式**: 策略模式 - 定义合并策略的接口

```csharp
public interface IMerge
{
    void Merge(object o);
}
```

---

## 方法说明

### Merge(object o)

**签名**:
```csharp
void Merge(object o)
```

**职责**: 将另一个对象的数据合并到当前对象

**参数说明**:
| 参数 | 类型 | 说明 |
|------|------|------|
| `o` | `object` | 要合并的源对象 |

**核心逻辑**:
```
由实现类自行定义合并逻辑，通常包括:
1. 检查源对象类型
2. 遍历源对象的字段/属性
3. 将新数据合并到当前对象
4. 处理冲突 (如: 新数据覆盖旧数据，或保留旧数据)
```

**调用者**: ConfigManager (热更新配置时)

**被调用者**: 无

---

## 使用示例

### 示例 1: 实现 IMerge 接口

```csharp
[Config]
public class ItemConfigCategory : ProtoObject, IMerge<ItemConfigCategory>
{
    public Dictionary<int, ItemConfig> Dict { get; set; }
    
    // 实现 IMerge 接口
    public void Merge(object o)
    {
        if (o is ItemConfigCategory source)
        {
            // 合并字典数据
            if (source.Dict != null)
            {
                if (this.Dict == null)
                {
                    this.Dict = new Dictionary<int, ItemConfig>();
                }
                
                foreach (var kvp in source.Dict)
                {
                    // 新数据覆盖旧数据
                    this.Dict[kvp.Key] = kvp.Value;
                }
            }
        }
    }
}
```

### 示例 2: 泛型版本 (推荐)

```csharp
// 使用泛型版本更安全
public interface IMerge<T>
{
    void Merge(T o);
}

[Config]
public class ItemConfigCategory : ProtoObject, IMerge<ItemConfigCategory>
{
    public Dictionary<int, ItemConfig> Dict { get; set; }
    
    public void Merge(ItemConfigCategory source)
    {
        if (source.Dict != null)
        {
            if (this.Dict == null)
            {
                this.Dict = new Dictionary<int, ItemConfig>();
            }
            
            foreach (var kvp in source.Dict)
            {
                this.Dict[kvp.Key] = kvp.Value;
            }
        }
    }
}
```

### 示例 3: 配置热更新

```csharp
// ConfigManager 热更新流程 (伪代码)
public async ETTask HotfixConfigAsync()
{
    // 加载新配置
    var newConfig = await LoadConfig<ItemConfigCategory>();
    
    // 获取旧配置
    var oldConfig = ConfigManager.Instance.Get<ItemConfigCategory>();
    
    // 合并配置
    if (oldConfig is IMerge<ItemConfigCategory> mergeable)
    {
        mergeable.Merge(newConfig);
    }
    
    // 通知配置已更新
    Messager.Instance.Broadcast(0, MessageId.ConfigUpdated, typeof(ItemConfigCategory));
}
```

---

## 合并策略

### 常见合并策略

| 策略 | 说明 | 适用场景 |
|------|------|----------|
| **覆盖** | 新数据完全覆盖旧数据 | 简单配置、不关心历史数据 |
| **追加** | 新数据追加到旧数据 | 列表型配置、累积数据 |
| **智能合并** | 根据 ID 合并，新数据覆盖同 ID 旧数据 | 字典型配置 (最常用) |
| **保留旧数据** | 仅当旧数据为空时使用新数据 | 用户自定义配置 |

### 智能合并示例

```csharp
public void Merge(ItemConfigCategory source)
{
    if (source.Dict == null) return;
    
    if (this.Dict == null)
    {
        this.Dict = new Dictionary<int, ItemConfig>();
    }
    
    foreach (var kvp in source.Dict)
    {
        if (this.Dict.ContainsKey(kvp.Key))
        {
            // 已存在：更新字段
            var existing = this.Dict[kvp.Key];
            var newItem = kvp.Value;
            
            // 只更新非空字段
            if (!string.IsNullOrEmpty(newItem.Name))
            {
                existing.Name = newItem.Name;
            }
            if (newItem.Type > 0)
            {
                existing.Type = newItem.Type;
            }
        }
        else
        {
            // 不存在：直接添加
            this.Dict[kvp.Key] = kvp.Value;
        }
    }
}
```

---

## 与其他模块的交互

```mermaid
graph TD
    subgraph IMerge["IMerge 接口"]
        IM[IMerge]
    end
    
    subgraph Config["配置系统"]
        CM[ConfigManager]
        CL[ConfigLoader]
    end
    
    subgraph Configs["配置类"]
        IC[ItemConfigCategory]
        TC[TaskConfigCategory]
    end
    
    IM -.-> IC
    IM -.-> TC
    CM --> IM
    CL --> CM
    
    note right of IM "IMerge 用于配置热更新时<br/>合并新旧配置数据"
    
    style IMerge fill:#e1f5ff
    style Config fill:#fff4e1
    style Configs fill:#e8f5e9
```

**依赖关系**:
- **依赖**: 无
- **被依赖**: `ConfigManager` (配置管理), 配置类 (实现合并逻辑)

---

## 阅读指引

### 建议的阅读顺序

1. **理解接口作用** - IMerge 用于配置热更新合并
2. **看方法签名** - 理解 Merge 方法的参数和职责
3. **了解实现方式** - 查看配置类如何实现 IMerge
4. **查看合并策略** - 了解不同的合并策略

### 最值得学习的技术点

1. **接口设计**: 简单的接口定义复杂的合并逻辑
2. **热更新支持**: 通过合并实现配置热更新
3. **策略模式**: 每个配置类自行定义合并策略

---

## 相关文档

- [ProtoObject.cs.md](./ProtoObject.cs.md) - Protobuf 对象基类
- [ConfigManager.cs.md](./ConfigManager.cs.md) - 配置管理器
- [ConfigLoader.cs.md](./ConfigLoader.cs.md) - 配置加载器
- [ConfigAttribute.cs.md](./ConfigAttribute.cs.md) - 配置类标记特性

---

*文档生成时间：2026-03-02 | OpenClaw AI 助手*
