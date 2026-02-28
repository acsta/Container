# ProtobufHelper.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | ProtobufHelper.cs |
| **路径** | Assets/Scripts/Code/Module/Config/ProtobufHelper.cs |
| **所属模块** | 框架层 → Code/Module/Config |
| **文件职责** | 提供基于 Nino 的 Protobuf 序列化/反序列化工具方法 |

---

## 类/结构体说明

### ProtobufHelper

| 属性 | 说明 |
|------|------|
| **职责** | 静态工具类，封装 Nino 序列化库的常用操作 |
| **泛型参数** | 无 |
| **继承关系** | 无 |
| **实现的接口** | 无 |

**设计模式**: 工具类模式（静态方法集合）

```csharp
// 所有方法都是静态的，直接通过类名调用
byte[] bytes = ProtobufHelper.ToBytes(config);
var config = ProtobufHelper.FromBytes<LevelConfig>(bytes);
```

**依赖库**: [Nino.Serialization](https://github.com/ninochan/Nino) - 高性能 .NET 序列化库

---

## 方法说明（按重要程度排序）

### FromBytes<T> (泛型)

**签名**:
```csharp
public static T FromBytes<T>(byte[] bytes)
```

**职责**: 将二进制数据反序列化为指定类型的对象

**核心逻辑**:
```
1. 检查 bytes 长度是否为 0 → 是则返回 default
2. 使用 Nino Deserializer.Deserialize<T> 反序列化
3. 如果对象实现 ISupportInitialize，调用 EndInit()
4. 返回反序列化后的对象
```

**调用者**: `ConfigManager.LoadOneInThread()`, 任何需要解析配置的地方

**被调用者**: `Nino.Deserializer.Deserialize<T>()`

**使用示例**:
```csharp
byte[] configBytes = await ConfigLoader.GetOneConfigBytes("LevelConfig");
LevelConfigCategory levelConfig = ProtobufHelper.FromBytes<LevelConfigCategory>(configBytes);
```

---

### FromBytes (Type 参数)

**签名**:
```csharp
public static object FromBytes(Type type, byte[] bytes)
```

**职责**: 将二进制数据反序列化为指定类型的对象（运行时类型）

**核心逻辑**:
```
1. 检查 bytes 长度是否为 0 → 是则返回 null
2. 使用 Nino Deserializer.Deserialize(type, bytes) 反序列化
3. 如果对象实现 ISupportInitialize，调用 EndInit()
4. 返回反序列化后的对象
```

**调用者**: `ConfigManager.LoadOneInThread()`

**被调用者**: `Nino.Deserializer.Deserialize(Type, byte[])`

---

### FromBytes (带索引范围)

**签名**:
```csharp
public static object FromBytes(Type type, byte[] bytes, int index, int count)
```

**职责**: 从字节数组的指定范围反序列化对象

**核心逻辑**:
```
1. 检查 bytes 长度是否为 0 → 是则返回 null
2. 如果 index==0 且 count==bytes.Length，委托给 FromBytes(Type, byte[])
3. 否则复制指定范围的字节到临时数组
4. 反序列化临时数组
5. 如果对象实现 ISupportInitialize，调用 EndInit()
6. 返回反序列化后的对象
```

**调用者**: `ConfigLoader.LoadConfigBytes()`, `ProtoObject.Clone()`

**参数**:
| 参数名 | 类型 | 说明 |
|--------|------|------|
| `type` | `Type` | 目标类型 |
| `bytes` | `byte[]` | 源字节数组 |
| `index` | `int` | 起始索引 |
| `count` | `int` | 字节数量 |

---

### ToBytes

**签名**:
```csharp
public static byte[] ToBytes(object message)
```

**职责**: 将对象序列化为二进制数据

**核心逻辑**:
```
1. 使用 Nino Serializer.Serialize 序列化对象
2. 返回序列化后的字节数组
```

**调用者**: `ProtoObject.Clone()`, `ToStream()`

**被调用者**: `Nino.Serializer.Serialize(object)`

**使用示例**:
```csharp
var config = new LevelConfig { Id = 1, Name = "关卡 1" };
byte[] bytes = ProtobufHelper.ToBytes(config);
// bytes 可以保存到文件或网络传输
```

---

### ToStream

**签名**:
```csharp
public static void ToStream(object message, MemoryStream stream)
```

**职责**: 将对象序列化并写入内存流

**核心逻辑**:
```
1. 调用 ToBytes 序列化对象
2. 将字节数组写入 MemoryStream
```

**调用者**: 网络发送相关代码

**参数**:
| 参数名 | 类型 | 说明 |
|--------|------|------|
| `message` | `object` | 要序列化的对象 |
| `stream` | `MemoryStream` | 目标内存流 |

---

### FromStream

**签名**:
```csharp
public static object FromStream(Type type, MemoryStream stream)
```

**职责**: 从内存流反序列化对象

**核心逻辑**:
```
1. 从流的当前位置读取剩余字节
2. 调用 FromBytes 反序列化
3. 如果对象实现 ISupportInitialize，调用 EndInit()
4. 返回反序列化后的对象
```

**调用者**: 网络接收相关代码

**参数**:
| 参数名 | 类型 | 说明 |
|--------|------|------|
| `type` | `Type` | 目标类型 |
| `stream` | `MemoryStream` | 源内存流 |

---

### Init

**签名**:
```csharp
public static void Init()
```

**职责**: 初始化 ProtobufHelper（当前为空实现）

**说明**: 预留的初始化方法，可用于注册自定义序列化器等

---

## ISupportInitialize 接口支持

ProtobufHelper 支持 `ISupportInitialize` 接口，用于对象初始化后的回调：

```csharp
public interface ISupportInitialize
{
    void BeginInit();  // 开始初始化
    void EndInit();    // 结束初始化
}
```

**工作流程**:
```
反序列化完成后
    ↓
检查是否实现 ISupportInitialize
    ↓
是 → 调用 EndInit()
    ↓
返回对象
```

**用途**: 在反序列化完成后执行额外的初始化逻辑，如建立缓存、验证数据等

---

## 序列化流程

### 对象 → 字节数组

```mermaid
flowchart TD
    A[对象 message] --> B[ProtobufHelper.ToBytes]
    B --> C[Nino.Serializer.Serialize]
    C --> D[byte[] 字节数组]
    D --> E[保存到文件/网络传输]
```

### 字节数组 → 对象

```mermaid
flowchart TD
    A[byte[] 字节数组] --> B[ProtobufHelper.FromBytes]
    B --> C[Nino.Deserializer.Deserialize]
    C --> D{实现 ISupportInitialize?}
    D -->|是 | E[调用 EndInit]
    D -->|否 | F[返回对象]
    E --> F
```

---

## 使用示例

### 示例 1: 配置序列化/反序列化

```csharp
// 序列化配置
var levelConfig = new LevelConfig
{
    Id = 1,
    Name = "关卡 1",
    Difficulty = 3
};
byte[] bytes = ProtobufHelper.ToBytes(levelConfig);

// 反序列化配置
LevelConfig loadedConfig = ProtobufHelper.FromBytes<LevelConfig>(bytes);
Debug.Log($"加载配置：{loadedConfig.Name}");
```

### 示例 2: 从文件加载配置

```csharp
// 读取文件
byte[] fileBytes = File.ReadAllBytes("LevelConfig.bytes");

// 反序列化
LevelConfigCategory config = ProtobufHelper.FromBytes<LevelConfigCategory>(fileBytes);

// 访问配置数据
foreach (var level in config.LevelList)
{
    Debug.Log($"关卡 {level.Id}: {level.Name}");
}
```

### 示例 3: 网络传输

```csharp
// 发送端：序列化并写入流
using (MemoryStream stream = new MemoryStream())
{
    ProtobufHelper.ToStream(message, stream);
    byte[] data = stream.ToArray();
    network.Send(data);
}

// 接收端：从流反序列化
using (MemoryStream stream = new MemoryStream(receivedData))
{
    var message = ProtobufHelper.FromStream(messageType, stream);
    HandleMessage(message);
}
```

### 示例 4: 对象克隆

```csharp
// ProtoObject 提供 Clone 方法
var original = new LevelConfig { Id = 1, Name = "原始" };
var clone = original.Clone() as LevelConfig;

// 修改克隆对象不影响原对象
clone.Name = "克隆";
Debug.Log(original.Name);  // 输出："原始"
Debug.Log(clone.Name);     // 输出："克隆"
```

---

## 性能优化

### Nino 序列化优势

1. **高性能**: 比 protobuf-net 快 2-10 倍
2. **零配置**: 无需标记 [ProtoContract] 等特性
3. **支持泛型**: 完整支持泛型类型序列化
4. **AOT 友好**: 支持 IL2CPP 编译

### 使用建议

1. **避免频繁序列化**: 配置数据只需加载时序列化一次
2. **使用缓存**: 反序列化后的对象可以缓存复用
3. **注意内存**: 大对象序列化时注意内存占用

---

## 相关文档

- [ConfigLoader.cs.md](./ConfigLoader.cs.md) - 配置加载器（使用 FromBytes）
- [ConfigManager.cs.md](./ConfigManager.cs.md) - 配置管理器（使用 FromBytes）
- [ProtoObject.cs.md](./ProtoObject.cs.md) - ProtoObject 基类（使用 ToBytes 实现 Clone）
- [Nino 官方文档](https://github.com/ninochan/Nino)

---

*文档生成时间：2026-02-28 | OpenClaw AI 助手*
