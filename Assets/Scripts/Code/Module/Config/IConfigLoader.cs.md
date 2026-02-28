# IConfigLoader.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | IConfigLoader.cs |
| **路径** | Assets/Scripts/Code/Module/Config/IConfigLoader.cs |
| **所属模块** | 框架层 → Code/Module/Config |
| **文件职责** | 定义配置加载器接口，规范配置文件的加载行为 |

---

## 类/结构体说明

### IConfigLoader

| 属性 | 说明 |
|------|------|
| **职责** | 定义配置加载器的标准接口，用于解耦配置加载逻辑 |
| **泛型参数** | 无 |
| **继承关系** | 无 |
| **实现的接口** | 无 |

**设计模式**: 策略模式接口

```csharp
// ConfigManager 依赖此接口，不依赖具体实现
public IConfigLoader ConfigLoader { get; set; }

// 可以灵活替换加载器实现
ConfigManager.Instance.ConfigLoader = new ConfigLoader();  // 默认 YooAsset 加载
ConfigManager.Instance.ConfigLoader = new CustomLoader();  // 自定义加载
```

---

## 方法说明

### GetAllConfigBytes

**签名**:
```csharp
ETTask GetAllConfigBytes(Dictionary<string, byte[]> output)
```

**职责**: 异步加载所有配置文件到输出字典

**参数**:
| 参数名 | 类型 | 说明 |
|--------|------|------|
| `output` | `Dictionary<string, byte[]>` | 输出字典，key 为配置名称，value 为二进制数据 |

**返回值**: `ETTask` - 异步任务

**实现者**: `ConfigLoader`

---

### GetOneConfigBytes

**签名**:
```csharp
ETTask<byte[]> GetOneConfigBytes(string configName)
```

**职责**: 异步加载单个配置文件的二进制数据

**参数**:
| 参数名 | 类型 | 说明 |
|--------|------|------|
| `configName` | `string` | 配置文件名称（不含扩展名） |

**返回值**: `ETTask<byte[]>` - 配置文件的二进制数据

**实现者**: `ConfigLoader`

---

## 接口设计意图

### 为什么需要接口？

1. **解耦**: ConfigManager 不依赖具体加载实现
2. **可测试**: 可以轻松创建 Mock 实现进行单元测试
3. **可扩展**: 支持多种加载策略（YooAsset、网络、本地文件等）
4. **可替换**: 运行时可以动态切换加载器

### 典型实现

```csharp
// 默认实现：从 YooAsset 加载
public class ConfigLoader : IConfigLoader
{
    public async ETTask GetAllConfigBytes(Dictionary<string, byte[]> output)
    {
        // 从 YooAsset 批量加载配置
    }
    
    public async ETTask<byte[]> GetOneConfigBytes(string configName)
    {
        // 从 YooAsset 加载单个配置
    }
}

// 自定义实现：从网络加载
public class NetworkConfigLoader : IConfigLoader
{
    public async ETTask GetAllConfigBytes(Dictionary<string, byte[]> output)
    {
        // 从远程服务器下载所有配置
    }
    
    public async ETTask<byte[]> GetOneConfigBytes(string configName)
    {
        // 从远程服务器下载单个配置
    }
}

// 测试实现：Mock 数据
public class MockConfigLoader : IConfigLoader
{
    public async ETTask GetAllConfigBytes(Dictionary<string, byte[]> output)
    {
        // 返回预设的测试数据
    }
    
    public async ETTask<byte[]> GetOneConfigBytes(string configName)
    {
        // 返回预设的测试数据
    }
}
```

---

## 使用示例

### 示例 1: 使用默认加载器

```csharp
// ConfigManager 初始化时使用默认 ConfigLoader
ConfigManager.Instance.Init();
// 内部执行：ConfigLoader = new ConfigLoader();
```

### 示例 2: 自定义加载器

```csharp
// 创建自定义加载器
var customLoader = new NetworkConfigLoader();

// 替换默认加载器
ConfigManager.Instance.ConfigLoader = customLoader;

// 后续配置加载将使用自定义加载器
await ConfigManager.Instance.LoadAsync();
```

### 示例 3: 测试场景

```csharp
// 单元测试中使用 Mock 加载器
[Test]
public void TestConfigManager()
{
    var mockLoader = new MockConfigLoader();
    ConfigManager.Instance.ConfigLoader = mockLoader;
    
    // 测试配置加载逻辑
    var config = await ConfigManager.Instance.LoadOneConfig<LevelConfig>();
    Assert.IsNotNull(config);
}
```

---

## 相关文档

- [ConfigLoader.cs.md](./ConfigLoader.cs.md) - 默认配置加载器实现
- [ConfigManager.cs.md](./ConfigManager.cs.md) - 配置管理器
- [ProtobufHelper.cs.md](./ProtobufHelper.cs.md) - Protobuf 序列化工具

---

*文档生成时间：2026-02-28 | OpenClaw AI 助手*
