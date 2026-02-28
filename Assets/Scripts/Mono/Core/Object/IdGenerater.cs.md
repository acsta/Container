# IdGenerater.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|------|
| **文件名** | IdGenerater.cs |
| **路径** | Assets/Scripts/Mono/Core/Object/IdGenerater.cs |
| **所属模块** | 框架层 → Mono/Core/Object |
| **文件职责** | 提供分布式唯一 ID 生成器，支持多种 ID 结构（Id/InstanceId/UnitId） |

---

## 类/结构体说明

### IdGenerater

| 属性 | 说明 |
|------|------|
| **职责** | 单例类，生成全局唯一的分布式 ID，支持时间戳 + 进程 ID+ 序列号组合 |
| **泛型参数** | 无 |
| **继承关系** | 无 |
| **实现的接口** | `IDisposable` |

**设计模式**: 单例模式 + 生成器模式

```csharp
// 生成 ID
long id = IdGenerater.Instance.GenerateId();
long instanceId = IdGenerater.Instance.GenerateInstanceId();
long unitId = IdGenerater.Instance.GenerateUnitId(zone: 1);
```

---

## ID 结构

### IdStruct (34-18-16 位)

| 字段 | 位数 | 说明 |
|------|------|------|
| `Time` | 30bit | 距 2020-01-01 的秒数（34 年） |
| `Process` | 18bit | 进程 ID（262144 个进程） |
| `Value` | 16bit | 每秒序列号（65536 个/秒） |

**用途**: 通用唯一 ID

---

### InstanceIdStruct (28-18-18 位)

| 字段 | 位数 | 说明 |
|------|------|------|
| `Time` | 28bit | 距今年 1 月 1 日的秒数 |
| `Process` | 18bit | 进程 ID |
| `Value` | 18bit | 每秒序列号（262144 个/秒） |

**用途**: 实例 ID（更大的序列号空间）

---

### UnitIdStruct (30-10-8-16 位)

| 字段 | 位数 | 说明 |
|------|------|------|
| `Time` | 30bit | 距 2020-01-01 的秒数 |
| `Zone` | 10bit | 区服 ID（1024 个区） |
| `ProcessMode` | 8bit | 进程 ID % 256 |
| `Value` | 16bit | 每秒序列号 |

**用途**: 游戏单位 ID（支持多区服）

---

## 常量

| 常量 | 类型 | 值 | 说明 |
|------|------|-----|------|
| `Mask18bit` | `int` | 0x03ffff | 18 位掩码（262143） |
| `MaxZone` | `int` | 1024 | 最大区服数 |

---

## 字段与属性

### Instance

| 属性 | 值 |
|------|------|
| **类型** | `IdGenerater` |
| **访问级别** | `public static` |
| **说明** | 单例实例，全局访问点 |

---

### epoch2020

| 属性 | 值 |
|------|------|
| **类型** | `long` |
| **访问级别** | `private` |
| **说明** | 2020-01-01 00:00:00 UTC 的时间戳（毫秒） |

---

### epochThisYear

| 属性 | 值 |
|------|------|
| **类型** | `long` |
| **访问级别** | `private` |
| **说明** | 今年 1 月 1 日 00:00:00 UTC 的时间戳（毫秒） |

---

## 方法说明

### GenerateId

**签名**:
```csharp
public long GenerateId()
```

**职责**: 生成通用唯一 ID（IdStruct）

**核心逻辑**:
```
1. 获取当前时间（距 2020 年的秒数）
2. 如果时间大于 lastIdTime：
   - 更新时间
   - 重置序列号 value = 0
3. 否则：
   - 序列号 +1
   - 如果溢出，借用下一秒
4. 创建 IdStruct 并转换为 long
5. 返回
```

**返回值**: `long` - 64 位唯一 ID

**调用者**: 需要唯一 ID 的地方

---

### GenerateInstanceId

**签名**:
```csharp
public long GenerateInstanceId()
```

**职责**: 生成实例 ID（InstanceIdStruct）

**核心逻辑**:
```
1. 获取当前时间（距今年 1 月 1 日的秒数）
2. 如果时间大于 lastInstanceIdTime：
   - 更新时间
   - 重置序列号 instanceIdValue = 0
3. 否则：
   - 序列号 +1
   - 如果溢出（>262143），借用下一秒
4. 创建 InstanceIdStruct 并转换为 long
5. 返回
```

**返回值**: `long` - 64 位实例 ID

---

### GenerateUnitId

**签名**:
```csharp
public long GenerateUnitId(int zone)
```

**职责**: 生成游戏单位 ID（UnitIdStruct）

**核心逻辑**:
```
1. 检查 zone <= MaxZone
2. 获取当前时间（距 2020 年的秒数）
3. 如果时间大于 lastUnitIdTime：
   - 更新时间
   - 重置序列号 unitIdValue = 0
4. 否则：
   - 序列号 +1
   - 如果溢出，借用下一秒
5. 创建 UnitIdStruct 并转换为 long
6. 返回
```

**参数**:
| 参数名 | 类型 | 说明 |
|--------|------|------|
| `zone` | `int` | 区服 ID（0-1023） |

**返回值**: `long` - 64 位单位 ID

---

### GetUnitZone

**签名**:
```csharp
public static int GetUnitZone(long unitId)
```

**职责**: 从单位 ID 中提取区服 ID

**核心逻辑**:
```
1. 右移 24 位
2. 与 0x03ff 按位与（取出 10bit）
3. 返回区服 ID
```

**参数**:
| 参数名 | 类型 | 说明 |
|--------|------|------|
| `unitId` | `long` | 单位 ID |

**返回值**: `int` - 区服 ID（0-1023）

---

## 使用示例

### 示例 1: 生成玩家 ID

```csharp
// 创建玩家时生成唯一 ID
long playerId = IdGenerater.Instance.GenerateId();
player.Id = playerId;

// ID 结构：
// - 时间戳（秒级）
// - 进程 ID
// - 序列号
```

### 示例 2: 生成实例 ID

```csharp
// 创建 Entity 实例
long instanceId = IdGenerater.Instance.GenerateInstanceId();
entity.InstanceId = instanceId;

// InstanceId 特点：
// - 更大的序列号空间（262144/秒）
// - 时间从每年 1 月 1 日开始
```

### 示例 3: 生成多区服单位 ID

```csharp
// 1 区创建单位
long unitId1 = IdGenerater.Instance.GenerateUnitId(zone: 1);

// 2 区创建单位
long unitId2 = IdGenerater.Instance.GenerateUnitId(zone: 2);

// 从 ID 获取区服
int zone = IdGenerater.GetUnitZone(unitId1);  // 返回 1
```

### 示例 4: ID 解析

```csharp
long id = IdGenerater.Instance.GenerateId();

// 解析为结构体
IdStruct idStruct = new IdStruct(id);
Debug.Log($"进程：{idStruct.Process}, 时间：{idStruct.Time}, 序列号：{idStruct.Value}");
```

---

## 设计要点

### 为什么使用 64 位 ID？

**优势**:
1. **唯一性**: 全球分布式唯一
2. **有序性**: 基于时间戳，大致有序
3. **信息丰富**: 包含时间、进程、序列号
4. **性能**: long 类型，数据库索引友好

### 时间戳设计

**IdStruct**: 从 2020-01-01 开始（30bit = 34 年）
**InstanceIdStruct**: 从今年 1 月 1 日开始（28bit = 8.5 年）

**原因**:
- 减少位数占用
- 保证 ID 在时间范围内唯一
- InstanceId 需要更大的序列号空间，所以时间位数较少

### 序列号溢出处理

```csharp
if (value > ushort.MaxValue - 1)
{
    this.value = 0;
    ++this.lastIdTime;  // 借用下一秒
    Log.Error($"id count per sec overflow: {time} {this.lastIdTime}");
}
```

**说明**: 如果每秒生成的 ID 超过 65536 个，借用下一秒的时间戳

**阈值**:
- IdStruct: 65536 个/秒
- InstanceId: 262144 个/秒
- UnitId: 65536 个/秒/进程/区服

### 进程 ID

```csharp
Define.Process  // 全局进程 ID
```

**用途**: 区分不同进程生成的 ID

**配置**: 在 Define.cs 中设置

---

## 相关文档

- [Define.cs.md](../../Define.cs.md) - 全局配置（Process ID）
- [TimeInfo.cs.md](../Timer/TimeInfo.cs.md) - 时间信息工具

---

*文档生成时间：2026-02-28 | OpenClaw AI 助手*
