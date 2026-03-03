# 配置系统理解指南 - ConfigManager

> **文档类型**: 系统理解指南  
> **适用范围**: Code/Module/Config  
> **生成时间**: 2026-03-03  
> **前置知识**: Protobuf、异步编程、YooAsset 资源管理

---

## 📑 概述

配置系统负责游戏配置表的加载、解析和缓存管理。

**核心职责**:
- 从 YooAsset 加载配置二进制文件
- 使用 Protobuf 反序列化配置数据
- 多线程并行加载优化性能
- 提供配置缓存和释放机制

**关键文件**:
| 文件 | 职责 |
|------|------|
| `ConfigManager.cs` | 配置管理核心 |
| `ConfigLoader.cs` | 配置加载器（YooAsset 对接） |
| `ProtobufHelper.cs` | Protobuf 序列化工具 |
| `ConfigAttribute.cs` | 配置类型标记 |

---

## 🎯 系统职责

### 解决的核心问题

1. **配置数据管理**: 游戏有大量配置表（物品、技能、关卡等），需要统一管理
2. **加载性能**: 配置表可能很大，需要异步加载避免卡顿
3. **内存优化**: 不需要常驻内存的配置可以释放
4. **热更新支持**: 配置表需要支持热更新

### 设计思路

```
┌─────────────────────────────────────────────────────────┐
│                    ConfigManager                         │
│  ┌─────────────────────────────────────────────────┐   │
│  │         AllConfig: Dictionary<Type, object>     │   │
│  │  缓存已加载的配置表                              │   │
│  └─────────────────────────────────────────────────┘   │
│                        │                                │
│                        ▼                                │
│  ┌─────────────────────────────────────────────────┐   │
│  │            ConfigLoader (IConfigLoader)         │   │
│  │  从 YooAsset 加载配置二进制数据                   │   │
│  └─────────────────────────────────────────────────┘   │
│                        │                                │
│                        ▼                                │
│  ┌─────────────────────────────────────────────────┐   │
│  │          ProtobufHelper (序列化工具)            │   │
│  │  二进制 → Protobuf 对象                           │   │
│  └─────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────┘
```

---

## 🏗️ 架构设计

### 核心类图

```
┌──────────────────────┐       ┌──────────────────────┐
│   ConfigManager      │       │   IConfigLoader      │
│──────────────────────│       │──────────────────────│
│ +Instance            │       │ +GetAllConfigBytes() │
│ +ConfigLoader        │◄──────│ +GetOneConfigBytes() │
│ +AllConfig           │       └──────────▲───────────┘
│──────────────────────│                  │
│ +LoadAsync()         │                  │ 实现
│ +LoadOneConfig<T>()  │                  │
│ +ReleaseConfig<T>()  │                  │
└──────────────────────┘                  │
                                          │
                            ┌─────────────┴─────────────┐
                            │      ConfigLoader         │
                            │───────────────────────────│
                            │ +GetAllConfigBytes()      │
                            │ +GetOneConfigBytes()      │
                            └───────────────────────────┘
```

---

## 🔄 核心流程

### 配置加载流程

```csharp
// 1. 游戏启动时加载所有配置
await ConfigManager.Instance.LoadAsync();

// 2. LoadAsync 内部流程
public async ETTask LoadAsync()
{
    this.AllConfig.Clear();
    
    // 获取所有标记了 [Config] 特性的配置类型
    List<Type> types = AttributeManager.Instance.GetTypes(
        TypeInfo<ConfigAttribute>.Type
    );

    // 从 YooAsset 加载所有配置二进制数据
    Dictionary<string, byte[]> configBytes = new Dictionary<string, byte[]>();
    await this.ConfigLoader.GetAllConfigBytes(configBytes);

    // 多线程并行解析（非 WebGL 平台）
#if UNITY_WEBGL
    // WebGL 不支持多线程，主线程加载
    foreach (Type type in types)
    {
        this.LoadOneInThread(type, configBytes);
    }	
#else
    using (ListComponent<Task> listTasks = ListComponent<Task>.Create())
    {
        foreach (Type type in types)
        {
            // 每个配置表在一个独立线程中解析
            Task assignment = Task.Run(() => 
                this.LoadOneInThread(type, configBytes)
            );
            listTasks.Add(assignment);
        }

        // 等待所有线程完成
        await Task.WhenAll(listTasks.ToArray());
    }
#endif
}

// 3. 单线程加载逻辑
private void LoadOneInThread(Type configType, Dictionary<string, byte[]> configBytes)
{
    // 获取配置二进制数据
    byte[] oneConfigBytes = configBytes[configType.Name];

    // Protobuf 反序列化
    object category = ProtobufHelper.FromBytes(
        configType, 
        oneConfigBytes, 
        0, 
        oneConfigBytes.Length
    );

    // 加入缓存（线程安全）
    lock (this)
    {
        this.AllConfig[configType] = category;
    }
}
```

---

### 加载单个配置流程

```csharp
// 按需加载单个配置
var config = await ConfigManager.Instance.LoadOneConfig<ItemConfig>();

// 内部流程
public async ETTask<T> LoadOneConfig<T>(string name = "", bool cache = false) 
    where T: ProtoObject
{
    Type configType = TypeInfo<T>.Type;
    
    // 默认使用类型全名作为配置名
    if (string.IsNullOrEmpty(name))
        name = configType.FullName;
    
    // 从 YooAsset 加载二进制数据
    byte[] oneConfigBytes = await this.ConfigLoader.GetOneConfigBytes(name);

    // Protobuf 反序列化
    object category = ProtobufHelper.FromBytes(
        configType, 
        oneConfigBytes, 
        0, 
        oneConfigBytes.Length
    );

    // 可选缓存
    if(cache)
        this.AllConfig[configType] = category;

    return category as T;
}
```

---

### ConfigLoader 加载流程

```csharp
// 加载所有配置
public async ETTask GetAllConfigBytes(Dictionary<string, byte[]> output)
{
    // 从 YooAsset 获取所有配置资源信息
    var assets = PackageManager.Instance.GetAssetInfos("config", Define.DefaultName);

    // 并行加载所有配置
    using (ListComponent<ETTask> tasks = new ListComponent<ETTask>())
    {
        foreach (var asset in assets)
        {
            tasks.Add(LoadConfigBytes(asset, output));
        }
        await ETTaskHelper.WaitAll(tasks);
    }
}

// 加载单个配置
private async ETTask LoadConfigBytes(AssetInfo asset, Dictionary<string, byte[]> output)
{
    // 异步加载资源
    var op = PackageManager.Instance.LoadAssetAsync(asset, Define.DefaultName);
    await op.Task;
    
    // 获取 TextAsset 字节数据
    TextAsset v = op.AssetObject as TextAsset;
    string key = asset.Address;
    output[key] = v.bytes;
    
    // 释放资源
    op.Release();
}
```

---

## 💡 使用示例

### 配置表定义

```csharp
// 1. 定义配置类（使用 Protobuf 格式）
[Config]  // 标记为配置类型
public partial class ItemConfigCategory : ProtoObject
{
    // 配置项字典
    public Dictionary<long, ItemConfig> ItemConfigMap = new();
}

[ConfigItem]
public partial class ItemConfig : ProtoObject
{
    public long Id;           // 物品 ID
    public string Name;       // 物品名称
    public int ItemType;      // 物品类型
    public int Quality;       // 品质
    public string Icon;       // 图标路径
    public string Description;// 描述
}
```

### 加载所有配置

```csharp
// 游戏启动时
public static async Task StartAsync()
{
    // 注册 Manager
    ManagerProvider.RegisterManager<ConfigManager>();
    
    // 加载所有配置
    Log.Info("开始加载配置...");
    await ConfigManager.Instance.LoadAsync();
    Log.Info($"配置加载完成，共 {ConfigManager.Instance.AllConfig.Count} 个配置表");
    
    // 继续其他初始化...
    await ResourceManager.Instance.InitAsync();
}
```

### 访问配置数据

```csharp
// 获取配置表
var itemConfig = ConfigManager.Instance.AllConfig[typeof(ItemConfigCategory)] 
    as ItemConfigCategory;

// 查询具体配置
if (itemConfig.ItemConfigMap.TryGetValue(1001, out var item))
{
    Log.Info($"物品名称：{item.Name}");
    Log.Info($"物品品质：{item.Quality}");
}

// 或使用扩展方法（推荐）
if (ItemConfigCategory.Instance.TryGetByName("Sword", out var sword))
{
    Log.Info($"武器攻击力：{sword.Attack}");
}
```

### 按需加载配置

```csharp
// 只加载需要的配置（不缓存）
var rareConfig = await ConfigManager.Instance.LoadOneConfig<RareConfig>(
    "RareConfig", 
    cache: false
);

// 使用完后释放
ConfigManager.Instance.ReleaseConfig<RareConfig>();
```

### 配置热更新

```csharp
// YooAsset 检测到配置更新后
public async ETTask ReloadConfig(string configName)
{
    // 释放旧配置
    var configType = Type.GetType(configName);
    ConfigManager.Instance.AllConfig.Remove(configType);
    
    // 重新加载
    await ConfigManager.Instance.LoadOneConfig(configName, cache: true);
    
    Log.Info($"配置 {configName} 已重新加载");
}
```

---

## 🔗 依赖关系

```
依赖:
├─→ YooAsset (资源管理)
├─→ ProtobufHelper (序列化)
├─→ AttributeManager (特性管理)
└─→ TimerManager (异步等待)

被依赖:
├─→ 所有游戏系统（需要配置数据）
└─→ GameSystem (游戏逻辑)
```

---

## ⚠️ 注意事项

| 问题 | 说明 | 解决方案 |
|------|------|----------|
| WebGL 多线程 | WebGL 不支持多线程 | 使用 `#if UNITY_WEBGL` 条件编译 |
| 配置缓存 | 大配置表占用内存 | 使用 `ReleaseConfig<T>()` 释放 |
| 加载顺序 | 配置必须在其他系统前加载 | 在 `Entry.StartAsync` 最先加载 |
| 配置命名 | 配置名必须与类型名一致 | 使用 `TypeInfo<T>.Type.Name` |
| 线程安全 | 多线程加载需要锁 | 使用 `lock(this)` 保护共享数据 |

---

## 🔍 性能优化

### 多线程加载

```
场景：10 个配置表，每个 5MB

单线程加载:
- 加载时间：10 × 0.5s = 5s
- 总时间：5s

多线程加载（4 核）:
- 加载时间：10 × 0.5s / 4 ≈ 1.25s
- 总时间：1.25s

优化效果：4 倍提升
```

### 配置缓存策略

```csharp
// ✅ 推荐：常驻配置（频繁访问）
await ConfigManager.Instance.LoadAsync();  // 全部加载并缓存

// ✅ 推荐：临时配置（偶尔访问）
var tempConfig = await ConfigManager.Instance.LoadOneConfig<TempConfig>(
    cache: false
);
// 使用完不缓存，自动释放

// ❌ 避免：重复加载同一配置
var config1 = await ConfigManager.Instance.LoadOneConfig<ItemConfig>();
var config2 = await ConfigManager.Instance.LoadOneConfig<ItemConfig>();
// 应该使用缓存的 AllConfig
```

---

## 📚 相关文档

| 文档 | 说明 |
|------|------|
| [ProtobufHelper.cs.md](./ProtobufHelper.cs.md) | Protobuf 序列化工具 |
| [ConfigLoader.cs.md](./ConfigLoader.cs.md) | 配置加载器 |
| [YooAssets 文档](../../Mono/Module/YooAssets/) | 资源管理系统 |

---

*文档由 OpenClaw AI 助手自动生成 | 配置系统理解指南*
