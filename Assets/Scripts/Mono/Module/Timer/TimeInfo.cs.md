# TimeInfo.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | TimeInfo.cs |
| **路径** | Assets/Scripts/Mono/Module/Timer/TimeInfo.cs |
| **所属模块** | 框架层 → Mono/Module/Timer |
| **文件职责** | 时间信息服务，提供时间戳转换、时区管理、服务器 - 客户端时间同步等功能 |

---

## 类/结构体说明

### TimeInfo

| 属性 | 说明 |
|------|------|
| **职责** | 提供全局时间服务，包括客户端时间、服务器时间、时间戳转换等 |
| **泛型参数** | 无 |
| **继承关系** | 实现 `IDisposable` |
| **设计模式** | 单例模式 |

```csharp
// 单例实现
public static TimeInfo Instance = new TimeInfo();

// 私有构造函数
private TimeInfo()
```

---

## 常量定义

```csharp
public const long OneDay = 86400000;    // 1 天的毫秒数
public const long Hour = 3600000;       // 1 小时的毫秒数
public const long Minute = 60000;       // 1 分钟的毫秒数
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `Instance` | `TimeInfo` | `public static` | 单例实例，全局访问点 |
| `TimeZone` | `int` | `public` | 时区（小时偏移） |
| `ServerMinusClientTime` | `long` | `public` | 服务器时间减去客户端时间的差值（用于时间同步） |
| `FrameTime` | `long` | `public` | 当前帧的时间戳 |
| `dt1970` | `DateTime` | `private` | 1970 年 1 月 1 日 UTC（Unix 纪元起点） |
| `dt` | `DateTime` | `private` | 考虑时区后的纪元起点 |

---

## 方法说明（按重要程度排序）

### ClientNow()

**签名**:
```csharp
public long ClientNow()
```

**职责**: 获取客户端当前时间（Unix 时间戳，毫秒）

**核心逻辑**:
```
1. 获取当前 UTC 时间 DateTime.UtcNow.Ticks
2. 减去 1970 年起点的 Ticks
3. 除以 10000 转换为毫秒
```

**返回值**: 毫秒时间戳

**使用示例**:
```csharp
long now = TimeInfo.Instance.ClientNow();
```

---

### ServerNow()

**签名**:
```csharp
public long ServerNow()
```

**职责**: 获取服务器当前时间（考虑了服务器 - 客户端时间差）

**核心逻辑**:
```
1. 获取客户端时间 ClientNow()
2. 加上 ServerMinusClientTime 差值
```

**返回值**: 服务器毫秒时间戳

**使用示例**:
```csharp
// 先同步时间差
TimeInfo.Instance.ServerMinusClientTime = serverTime - clientTime;

// 获取服务器时间
long serverNow = TimeInfo.Instance.ServerNow();
```

---

### ClientFrameTime()

**签名**:
```csharp
public long ClientFrameTime()
```

**职责**: 获取当前帧的客户端时间

**核心逻辑**:
```
1. 返回 FrameTime
```

**使用示例**:
```csharp
long frameTime = TimeInfo.Instance.ClientFrameTime();
```

---

### ServerFrameTime()

**签名**:
```csharp
public long ServerFrameTime()
```

**职责**: 获取当前帧的服务器时间

**核心逻辑**:
```
1. 返回 FrameTime + ServerMinusClientTime
```

**使用示例**:
```csharp
long serverFrameTime = TimeInfo.Instance.ServerFrameTime();
```

---

### Update()

**签名**:
```csharp
public void Update()
```

**职责**: 每帧更新时间

**核心逻辑**:
```
1. 更新 FrameTime = ClientNow()
```

**调用者**: 游戏主循环

---

### ToDateTime(long timeStamp)

**签名**:
```csharp
public DateTime ToDateTime(long timeStamp)
```

**职责**: 将时间戳转换为 DateTime

**核心逻辑**:
```
1. 将毫秒时间戳转换为 ticks（乘以 10000）
2. 加上时区调整的 dt
3. 返回 DateTime
```

**使用示例**:
```csharp
long timeStamp = 1709280000000; // 2024-03-01 00:00:00
DateTime dt = TimeInfo.Instance.ToDateTime(timeStamp);
// 输出：2024-03-01 00:00:00
```

---

### Transition(DateTime d)

**签名**:
```csharp
public long Transition(DateTime d)
```

**职责**: 将 DateTime 转换为时间戳

**核心逻辑**:
```
1. 获取 DateTime 的 Ticks
2. 减去 dt 的 Ticks
3. 除以 10000 转换为毫秒
```

**使用示例**:
```csharp
DateTime dt = new DateTime(2024, 3, 1, 0, 0, 0);
long timeStamp = TimeInfo.Instance.Transition(dt);
```

---

### TransitionToStr(long time)

**签名**:
```csharp
public string TransitionToStr(long time)
```

**职责**: 将毫秒时间差转换为可读字符串（英文格式）

**核心逻辑**:
```
1. 计算天数、小时数、分钟数、秒数
2. 拼接为 "Xd Xh Xm Xs" 格式
```

**使用示例**:
```csharp
long time = 90061000; // 1 天 1 小时 1 分钟 1 秒
string str = TimeInfo.Instance.TransitionToStr(time);
// 输出："1d1h1m1s"
```

---

### TransitionToStr2(long time)

**签名**:
```csharp
public string TransitionToStr2(long time)
```

**职责**: 将毫秒时间差转换为可读字符串（国际化格式）

**核心逻辑**:
```
1. 计算天数、小时数、分钟数、秒数
2. 使用 I18NBridge 获取本地化文本
3. 如果只有一个单位，省略数字（如"1 天"→"天"）
```

**使用示例**:
```csharp
long time = 3600000; // 1 小时
string str = TimeInfo.Instance.TransitionToStr2(time);
// 输出："1 小时" 或 "小时"（根据本地化配置）
```

---

### Dispose()

**签名**:
```csharp
public void Dispose()
```

**职责**: 释放 TimeInfo 单例

**核心逻辑**:
```
1. 设置 Instance = null
```

---

## 时间同步机制

### 服务器 - 客户端时间同步

```csharp
// 客户端
long clientTime = TimeInfo.Instance.ClientNow();

// 从服务器获取当前时间
long serverTime = GetServerTime(); // 通过网络请求

// 计算时间差
TimeInfo.Instance.ServerMinusClientTime = serverTime - clientTime;

// 之后获取的时间都是服务器时间
long now = TimeInfo.Instance.ServerNow();
```

### 时间关系图

```
客户端时间 ──────────────────────────────►
              │
              │ ServerMinusClientTime
              ▼
服务器时间 ──────────────────────────────►

FrameTime 每帧更新，用于获取当前帧的时间
```

---

## 阅读指引

### 建议的阅读顺序

1. **理解时间服务作用** - 为什么需要 TimeInfo
2. **看字段定义** - 了解 ServerMinusClientTime 的作用
3. **重点看 ServerNow/ClientNow** - 理解时间获取方式
4. **深入时间转换** - 理解 ToDateTime/Transition
5. **了解时间格式化** - 理解 TransitionToStr

### 最值得学习的技术点

1. **时间同步**: 通过 ServerMinusClientTime 实现服务器时间同步
2. **Unix 时间戳**: 使用 1970 年起点的毫秒数
3. **时区支持**: 通过 TimeZone 属性支持时区调整
4. **帧时间缓存**: FrameTime 避免每帧重复计算
5. **国际化格式化**: TransitionToStr2 支持本地化文本

---

## 使用示例

### 示例 1: 获取当前时间

```csharp
// 获取客户端时间
long clientNow = TimeInfo.Instance.ClientNow();

// 获取服务器时间（需先同步）
long serverNow = TimeInfo.Instance.ServerNow();

// 获取当前帧时间
long frameTime = TimeInfo.Instance.ClientFrameTime();
```

### 示例 2: 时间同步

```csharp
// 登录时同步服务器时间
async ETTask SyncServerTime()
{
    long clientTime = TimeInfo.Instance.ClientNow();
    long serverTime = await NetworkManager.Instance.GetServerTime();
    TimeInfo.Instance.ServerMinusClientTime = serverTime - clientTime;
}
```

### 示例 3: 时间戳转换

```csharp
// 时间戳 → DateTime
long timeStamp = 1709280000000;
DateTime dt = TimeInfo.Instance.ToDateTime(timeStamp);
Console.WriteLine(dt.ToString("yyyy-MM-dd HH:mm:ss"));

// DateTime → 时间戳
DateTime now = DateTime.Now;
long timeStamp2 = TimeInfo.Instance.Transition(now);
```

### 示例 4: 倒计时显示

```csharp
// 显示剩余时间
long remainingTime = 90061000; // 1 天 1 小时 1 分钟 1 秒
string text = TimeInfo.Instance.TransitionToStr(remainingTime);
// 输出："1d1h1m1s"

// 或使用国际化版本
string text2 = TimeInfo.Instance.TransitionToStr2(remainingTime);
// 输出："1 天 1 小时 1 分钟 1 秒"
```

### 示例 5: 设置时区

```csharp
// 设置东八区（中国）
TimeInfo.Instance.TimeZone = 8;

// 设置西五区（美国东部）
TimeInfo.Instance.TimeZone = -5;
```

### 示例 6: 每帧更新时间

```csharp
// 在游戏主循环中
void Update()
{
    TimeInfo.Instance.Update();
    
    // 使用帧时间
    long frameTime = TimeInfo.Instance.ClientFrameTime();
}
```

---

## 时间常量使用

```csharp
// 1 天后
long tomorrow = TimeInfo.Instance.ClientNow() + TimeInfo.OneDay;

// 1 小时后
long oneHourLater = TimeInfo.Instance.ClientNow() + TimeInfo.Hour;

// 30 分钟后
long thirtyMinLater = TimeInfo.Instance.ClientNow() + TimeInfo.Minute * 30;
```

---

## 相关文档

- [TimerManager.cs.md](./TimerManager.cs.md) - 定时器管理器（使用 TimeInfo）
- [TimerAction.cs.md](./TimerAction.cs.md) - 定时器动作
- [I18NBridge.cs.md](../I18N/I18NBridge.cs.md) - 国际化桥接（用于 TransitionToStr2）

---

*文档生成时间：2026-03-01 | OpenClaw AI 助手*
