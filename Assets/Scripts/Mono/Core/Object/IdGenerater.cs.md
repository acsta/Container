# IdGenerater.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | IdGenerater.cs |
| **路径** | Assets/Scripts/Mono/Core/Object/IdGenerater.cs |
| **所属模块** | Mono 层 → Core/Object |
| **文件职责** | 分布式 ID 生成器，生成全局唯一 ID |

---

## 概述

IdGenerater 提供多种 ID 结构，用于分布式系统中生成全局唯一的标识符。支持：
- **IdStruct**: 通用 ID（30bit 时间 + 18bit 进程 + 16bit 序列）
- **InstanceIdStruct**: 实例 ID（28bit 时间 + 18bit 进程 + 18bit 序列）
- **UnitIdStruct**: 单元 ID（30bit 时间 + 10bit 区域 + 8bit 进程 + 16bit 序列）

---

## 结构体说明

### IdStruct

| 属性 | 说明 |
|------|------|
| **职责** | 通用 ID 结构 |
| **位分配** | 30bit 时间 + 18bit 进程 + 16bit 序列 |
| **时间范围** | 约 34 年 |
| **进程数** | 最多 262,144 个进程 |
| **序列数** | 每秒每进程最多 65,536 个 ID |

```csharp
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct IdStruct
{
    public uint Time;    // 30bit
    public int Process;  // 18bit
    public ushort Value; // 16bit
}
```

**内存布局**:
```
| 34-63 (30bit) | 16-33 (18bit) | 0-15 (16bit) |
|     Time      |    Process    |    Value     |
```

---

### InstanceIdStruct

| 属性 | 说明 |
|------|------|
| **职责** | 实例 ID 结构 |
| **位分配** | 28bit 时间 + 18bit 进程 + 18bit 序列 |
| **时间范围** | 约 8.5 年（从年初开始计 tick） |
| **进程数** | 最多 262,144 个进程 |
| **序列数** | 每秒每进程最多 262,144 个 ID |

```csharp
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct InstanceIdStruct
{
    public uint Time;   // 28bit 当年开始的 tick
    public int Process; // 18bit
    public uint Value;  // 18bit
}
```

**内存布局**:
```
| 36-63 (28bit) | 18-35 (18bit) | 0-17 (18bit) |
|     Time      |    Process    |    Value     |
```

---

### UnitIdStruct

| 属性 | 说明 |
|------|------|
| **职责** | 游戏单元 ID 结构（用于角色、怪物等） |
| **位分配** | 30bit 时间 + 10bit 区域 + 8bit 进程模式 + 16bit 序列 |
| **时间范围** | 约 34 年 |
| **区域数** | 最多 1,024 个区 |
| **进程数** | 每区最多 256 个进程 |
| **序列数** | 每秒每进程最多 65,536 个 Unit |

```csharp
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct UnitIdStruct
{
    public uint Time;        // 30bit 34 年
    public ushort Zone;      // 10bit 1024 个区
    public byte ProcessMode; // 8bit Process % 256
    public ushort Value;     // 16bit 每秒每个进程最大 16K 个 Unit
}
```

**内存布局**:
```
| 34-63 (30bit) | 24-33 (10bit) | 16-23 (8bit) | 0-15 (16bit) |
|     Time      |     Zone      | ProcessMode  |    Value     |
```

---

## 方法说明

### IdStruct.ToLong()

**签名**:
```csharp
public long ToLong()
```

**职责**: 将 IdStruct 转换为 long

**位操作**:
```csharp
ulong result = 0;
result |= this.Value;              // 低 16 位
result |= (ulong)this.Process << 16;  // 中 18 位
result |= (ulong)this.Time << 34;     // 高 30 位
return (long)result;
```

---

### IdStruct 构造函数

```csharp
// 从分量构造
public IdStruct(uint time, int process, ushort value)

// 从 long 解析
public IdStruct(long id)
```

**解析逻辑**:
```csharp
public IdStruct(long id)
{
    ulong result = (ulong)id;
    this.Value = (ushort)(result & ushort.MaxValue);           // 低 16 位
    result >>= 16;
    this.Process = (int)(result & IdGenerater.Mask18bit);      // 中 18 位
    result >>= 18;
    this.Time = (uint)result;                                  // 高 30 位
}
```

---

### InstanceIdStruct.ToLong()

**签名**:
```csharp
public long ToLong()
```

**职责**: 将 InstanceIdStruct 转换为 long

**位操作**:
```csharp
ulong result = 0;
result |= this.Value;               // 低 18 位
result |= (ulong)this.Process << 18;   // 中 18 位
result |= (ulong)this.Time << 36;      // 高 28 位
return (long)result;
```

---

### InstanceIdStruct 构造函数

```csharp
// 从 long 解析
public InstanceIdStruct(long id)

// 从分量构造
public InstanceIdStruct(uint time, int process, uint value)

// 场景 ID 专用（无时间）
public InstanceIdStruct(int process, uint value)
```

---

## 常量定义

```csharp
// 18 位掩码：0x3FFFF = 262143
public const long Mask18bit = 0x3FFFF;

// 16 位掩码：0xFFFF = 65535
public const long Mask16bit = 0xFFFF;
```

---

## 使用场景

### 场景 1: 实体 ID 生成

```csharp
// 生成实体 ID
var idStruct = new IdStruct(time, processId, sequence);
long entityId = idStruct.ToLong();

// 解析实体 ID
var parsed = new IdStruct(entityId);
Console.WriteLine($"Process: {parsed.Process}, Time: {parsed.Time}, Value: {parsed.Value}");
```

### 场景 2: 实例 ID 生成

```csharp
// 生成年内实例 ID
uint tickSinceYearStart = GetTickSinceYearStart();
var instanceId = new InstanceIdStruct(tickSinceYearStart, processId, sequence);
long id = instanceId.ToLong();
```

### 场景 3: 游戏单元 ID

```csharp
// 生成跨服单元 ID（角色、怪物等）
var unitId = new UnitIdStruct(
    time: currentTime,
    zone: serverZone,      // 0-1023
    processMode: processId % 256,
    value: sequence
);
long id = unitId.ToLong();

// 解析
var parsed = new UnitIdStruct(id);
Console.WriteLine($"Zone: {parsed.Zone}, Process: {parsed.ProcessMode}");
```

---

## ID 格式对比

| ID 类型 | 总位数 | 时间范围 | 进程数 | 序列数/秒 | 适用场景 |
|--------|-------|---------|--------|----------|---------|
| IdStruct | 64bit | 34 年 | 262K | 65K | 通用实体 |
| InstanceIdStruct | 64bit | 8.5 年 | 262K | 262K | 临时实例 |
| UnitIdStruct | 64bit | 34 年 | 256/区 | 65K | 游戏单元 |

---

## 分布式 ID 特性

### 1. 全局唯一

通过 时间 + 进程 + 序列 三元组保证：
- 不同时间生成的 ID 不同
- 同一时间不同进程生成的 ID 不同
- 同一时间同一进程的不同序列 ID 不同

### 2. 趋势递增

时间戳在高位，ID 整体趋势递增，适合：
- 数据库索引优化
- 范围查询
- 按时间排序

### 3. 可解析

ID 包含元数据，可直接解析出：
- 生成时间
- 来源进程
- 所属区域（UnitIdStruct）

---

## 注意事项

### 1. 时间同步

分布式系统需要时间同步，否则可能生成重复 ID

### 2. 序列号管理

每进程需要维护序列号，避免同一毫秒内重复：

```csharp
// 伪代码
static ushort sequence = 0;

long GenerateId()
{
    uint time = GetCurrentTime();
    int process = GetProcessId();
    
    // 序列号自增，溢出时等待下一毫秒
    sequence++;
    
    var idStruct = new IdStruct(time, process, sequence);
    return idStruct.ToLong();
}
```

### 3. 进程 ID 分配

进程 ID 需要全局唯一分配，常见方案：
- 配置文件分配
- 注册中心分配
- IP+Port 哈希

---

## 相关文档

- [ObjectPool.cs.md](../ObjectPool.cs.md) - 对象池
- [EntityComponent.cs.md](../../Module/Entity/EntityComponent.cs.md) - 实体组件

---

*文档生成时间：2026-03-01 | OpenClaw AI 助手*
