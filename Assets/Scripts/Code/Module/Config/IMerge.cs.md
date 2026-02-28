# IMerge.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | IMerge.cs |
| **路径** | Assets/Scripts/Code/Module/Config/IMerge.cs |
| **所属模块** | 框架层 → Code/Module/Config |
| **文件职责** | 定义配置合并接口，支持配置数据的多源合并 |

---

## 类/结构体说明

### IMerge 接口

| 属性 | 说明 |
|------|------|
| **职责** | 定义配置对象的合并行为，支持配置数据的多源合并 |
| **泛型参数** | 无 |
| **继承关系** | 无 |
| **实现的接口** | 无 |

**设计模式**: 策略模式接口

```csharp
// 配置列表类实现此接口
[Config]
public class LevelConfigCategory : ProtoObject, IMerge
{
    public List<LevelConfig> LevelList = new List<LevelConfig>();
    
    public void Merge(object o)
    {
        var other = o as LevelConfigCategory;
        LevelList.AddRange(other.LevelList);  // 合并配置列表
    }
}
```

---

## 方法说明

### Merge

**签名**:
```csharp
void Merge(object o)
```

**职责**: 将另一个同类型配置对象的数据合并到当前对象

**参数**:
| 参数名 | 类型 | 说明 |
|--------|------|------|
| `o` | `object` | 要合并的另一个配置对象 |

**返回值**: 无

**调用者**: `ConfigManager.ReleaseConfig<T>()`（可选调用）

**被调用者**: 配置类别类的实现

---

## 设计意图

### 为什么需要合并？

1. **配置分拆**: 大型配置可以分拆到多个文件
2. **模块化**: 不同模块可以定义独立的配置
3. **热更新**: 更新配置时无需替换整个配置文件
4. **DLC 支持**: DLC 可以追加新配置而不修改基础配置

### 典型使用场景

```
基础配置 (LevelConfig_Base.bytes)
├── 关卡 1
├── 关卡 2
└── 关卡 3

DLC 配置 (LevelConfig_DLC.bytes)
├── 关卡 4
├── 关卡 5
└── 关卡 6

合并后
├── 关卡 1
├── 关卡 2
├── 关卡 3
├── 关卡 4
├── 关卡 5
└── 关卡 6
```

---

## 实现示例

### 示例 1: 配置列表合并

```csharp
[Config]
public class ItemConfigCategory : ProtoObject, IMerge
{
    public List<ItemConfig> ItemList = new List<ItemConfig>();
    
    public void Merge(object o)
    {
        var other = o as ItemConfigCategory;
        if (other != null)
        {
            ItemList.AddRange(other.ItemList);
        }
    }
}
```

### 示例 2: 配置字典合并

```csharp
[Config]
public class SkillConfigCategory : ProtoObject, IMerge
{
    public Dictionary<int, SkillConfig> SkillDict = new Dictionary<int, SkillConfig>();
    
    public void Merge(object o)
    {
        var other = o as SkillConfigCategory;
        if (other != null)
        {
            foreach (var kvp in other.SkillDict)
            {
                // 如果 ID 冲突，后加载的覆盖先加载的
                SkillDict[kvp.Key] = kvp.Value;
            }
        }
    }
}
```

### 示例 3: 条件合并

```csharp
[Config]
public class EnemyConfigCategory : ProtoObject, IMerge
{
    public List<EnemyConfig> EnemyList = new List<EnemyConfig>();
    
    public void Merge(object o)
    {
        var other = o as EnemyConfigCategory;
        if (other != null)
        {
            // 只添加不存在的敌人（避免覆盖）
            foreach (var enemy in other.EnemyList)
            {
                if (!EnemyList.Any(e => e.Id == enemy.Id))
                {
                    EnemyList.Add(enemy);
                }
            }
        }
    }
}
```

---

## ConfigManager 中的使用

### ReleaseConfig 方法

```csharp
public void ReleaseConfig<T>() where T : ProtoObject, IMerge
{
    Type configType = TypeInfo<T>.Type;
    AllConfig.Remove(configType);
}
```

**说明**: 虽然 `ReleaseConfig` 没有直接调用 `Merge`，但 `IMerge` 约束确保只有支持合并的配置才能被释放（暗示可能重新加载合并）。

### 潜在使用场景

```csharp
// 重新加载配置并合并
var oldConfig = ConfigManager.Instance.GetConfig<ItemConfigCategory>();
var newConfig = await ConfigManager.Instance.LoadOneConfig<ItemConfigCategory>();

if (oldConfig != null && newConfig is IMerge mergeable)
{
    mergeable.Merge(oldConfig);  // 合并旧配置
}
```

---

## 使用示例

### 示例 1: 基础配置 + DLC 配置

```csharp
// 加载基础配置
await ConfigManager.Instance.LoadAsync();
var itemConfig = ConfigManager.Instance.GetConfig<ItemConfigCategory>();

// 加载 DLC 配置（追加）
var dlcConfig = await ConfigManager.Instance.LoadOneConfig<ItemConfigCategory>("ItemConfig_DLC");
if (dlcConfig != null && itemConfig is IMerge mergeable)
{
    mergeable.Merge(dlcConfig);
}

// 现在 itemConfig 包含基础 + DLC 的所有物品
```

### 示例 2: 配置热更新

```csharp
// 下载更新的配置文件
byte[] updatedBytes = await DownloadConfig("EnemyConfig");

// 反序列化新配置
var newConfig = ProtobufHelper.FromBytes<EnemyConfigCategory>(updatedBytes);

// 获取现有配置
var existingConfig = ConfigManager.Instance.GetConfig<EnemyConfigCategory>();

// 合并（新配置优先）
if (existingConfig != null)
{
    newConfig.Merge(existingConfig);
}

// 替换配置
ConfigManager.Instance.AllConfig[typeof(EnemyConfigCategory)] = newConfig;
```

---

## 设计要点

### Merge vs 直接替换

| 方式 | 优点 | 缺点 | 适用场景 |
|------|------|------|----------|
| **Merge** | 保留原有数据，支持增量更新 | 需要实现合并逻辑 | DLC、热更新、配置分拆 |
| **替换** | 简单直接，无合并开销 | 丢失原有数据 | 完整配置更新 |

### 实现建议

1. **明确合并策略**: 是追加、覆盖还是条件合并？
2. **处理 ID 冲突**: 决定哪个配置优先
3. **性能考虑**: 大量数据合并时注意性能
4. **类型安全**: 确保传入的 `object` 是正确类型

---

## 相关文档

- [ConfigManager.cs.md](./ConfigManager.cs.md) - 配置管理器（使用 IMerge 约束）
- [ProtoObject.cs.md](./ProtoObject.cs.md) - 配置对象基类
- [ProtobufHelper.cs.md](./ProtobufHelper.cs.md) - Protobuf 序列化工具

---

*文档生成时间：2026-02-28 | OpenClaw AI 助手*
