# TimeInfo.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|------|
| **文件名** | TimeInfo.cs |
| **路径** | Assets/Scripts/Mono/Module/Timer/TimeInfo.cs |
| **所属模块** | 框架层 → Mono/Module/Timer |
| **文件职责** | 提供时间信息工具，支持时区转换、时间戳格式化、服务器客户端时间同步 |

---

## 类/结构体说明

### TimeInfo

| 属性 | 说明 |
|------|------|
| **职责** | 单例工具类，提供时间戳转换、时区处理、服务器客户端时间同步等功能 |
| **泛型参数** | 无 |
| **继承关系** | 无 |
| **实现的接口** | `IDisposable` |

**设计模式**: 单例模式 + 工具类模式

```csharp
// 获取实例
var timeInfo = TimeInfo.Instance;

// 设置时区
TimeInfo.Instance.TimeZone = 8;  // 东八区

// 获取客户端当前时间
long clientNow = TimeInfo.Instance.ClientNow();
```

---

## 常量

| 常量 | 类型 | 值 | 说明 |
|------|------|-----|------|
| `OneDay` | `long` | 86400000 | 一天的毫秒数 |
| `Hour` | `long` | 3600000 | 一小时的毫秒数 |
| `Minute` | `long` | 60000 | 一分钟的毫秒数 |

---

## 字段与属性

### Instance

| 属性 | 值 |
|------|------|
| **类型** | `TimeInfo` |
| **访问级别** | `public static` |
| **说明** | 单例实例，全局访问点 |

---

### TimeZone

| 属性 | 值 |
|------|------|
| **类型** | `int` |
| **访问级别** | `public` (get/set) |
| **说明** | 时区偏移（小时） |

**设置时触发**:
```csharp
set
{
    this.timeZone = value;
    dt = dt1970.AddHours(TimeZone);  // 重新计算基准时间
}
```

**示例值**:
- `8`: 东八区（中国）
- `0`: UTC
- `-5`: 西五区（美国东部）

---

### ServerMinusClientTime

| 属性 | 值 |
|------|------|
| **类型** | `long` |
| **访问级别** | `public` (private get, set) |
| **说明** | 服务器时间与客户端时间的差值 |

**计算公式**:
```
ServerTime = ClientTime + ServerMinusClientTime
```

**用途**: 修正客户端时间，确保与服务器同步

---

### FrameTime

| 属性 | 值 |
|------|------|
| **类型** | `long` |
| **访问级别** | `public` |
| **说明** | 当前帧的时间戳 |

**更新时机**: 每帧调用 `Update()`

---

## 方法说明

### ToDateTime

**签名**:
```csharp
public DateTime ToDateTime(long timeStamp)
```

**职责**: 将时间戳转换为 DateTime 对象

**核心逻辑**:
```
1. 从 1970-01-01 00:00:00 UTC 开始
2. 加上 timeZone 小时偏移
3. 加上 timeStamp 毫秒（转换为 ticks）
4. 返回 DateTime
```

**参数**:
| 参数名 | 类型 | 说明 |
|--------|------|------|
| `timeStamp` | `long` | 时间戳（毫秒） |

**返回值**: `DateTime` - 转换后的日期时间

**使用示例**:
```csharp
long timeStamp = 1677600000000;  // 2023-03-01 00:00:00 UTC
DateTime dt = TimeInfo.Instance.ToDateTime(timeStamp);
Debug.Log(dt.ToString("yyyy-MM-dd HH:mm:ss"));
```

---

### ClientNow

**签名**:
```csharp
public long ClientNow()
```

**职责**: 获取客户端当前时间戳（毫秒）

**核心逻辑**:
```
1. 获取当前 UTC 时间
2. 计算与 1970-01-01 的 ticks 差值
3. 转换为毫秒
4. 返回
```

**返回值**: `long` - 客户端当前时间戳

**线程安全**: ✅ 是

---

### ServerNow

**签名**:
```csharp
public long ServerNow()
```

**职责**: 获取服务器当前时间戳（客户端时间 + 差值）

**核心逻辑**:
```
return ClientNow() + ServerMinusClientTime
```

**返回值**: `long` - 服务器当前时间戳

**用途**: 确保客户端使用服务器时间（防作弊）

---

### ClientFrameTime

**签名**:
```csharp
public long ClientFrameTime()
```

**职责**: 获取当前帧的客户端时间戳

**返回值**: `long` - 当前帧时间戳

---

### ServerFrameTime

**签名**:
```csharp
public long ServerFrameTime()
```

**职责**: 获取当前帧的服务器时间戳

**核心逻辑**:
```
return FrameTime + ServerMinusClientTime
```

**返回值**: `long` - 当前帧服务器时间戳

---

### Transition

**签名**:
```csharp
public long Transition(DateTime d)
```

**职责**: 将 DateTime 转换为时间戳

**核心逻辑**:
```
1. 计算 DateTime 与基准时间的 ticks 差值
2. 转换为毫秒
3. 返回
```

**参数**:
| 参数名 | 类型 | 说明 |
|--------|------|------|
| `d` | `DateTime` | 日期时间对象 |

**返回值**: `long` - 时间戳（毫秒）

---

### TransitionToStr

**签名**:
```csharp
public string TransitionToStr(long time)
```

**职责**: 将时间戳转换为可读字符串（如 "1d 2h 3m 4s"）

**核心逻辑**:
```
1. 计算天数、小时、分钟、秒
2. 拼接字符串
3. 返回
```

**参数**:
| 参数名 | 类型 | 说明 |
|--------|------|------|
| `time` | `long` | 时间戳（毫秒） |

**返回值**: `string` - 格式化字符串

**示例**:
```csharp
long time = 93784000;  // 1 天 2 小时 3 分钟 4 秒
string str = TimeInfo.Instance.TransitionToStr(time);
// 输出："1d2h3m4s"
```

---

### TransitionToStr2

**签名**:
```csharp
public string TransitionToStr2(long time)
```

**职责**: 将时间戳转换为带本地化的可读字符串

**核心逻辑**:
```
1. 计算天数、小时、分钟、秒
2. 使用 I18NBridge 获取本地化文本
3. 如果只有一项，省略数字（如 "1 天" → "天"）
4. 拼接字符串
5. 返回
```

**返回值**: `string` - 本地化格式化字符串

**示例**:
```csharp
long time = 93784000;
string str = TimeInfo.Instance.TransitionToStr2(time);
// 输出："1 天 2 小时 3 分钟 4 秒"
// 如果只有 1 秒："秒"
```

---

### Update

**签名**:
```csharp
public void Update()
```

**职责**: 每帧更新 FrameTime

**核心逻辑**:
```
FrameTime = ClientNow()
```

**调用者**: 游戏主循环（每帧调用）

---

### Dispose

**签名**:
```csharp
public void Dispose()
```

**职责**: 销毁单例

**核心逻辑**:
```
Instance = null
```

**调用者**: 游戏关闭时

---

## 使用示例

### 示例 1: 时间戳转换

```csharp
// 时间戳 → DateTime
long timeStamp = 1677600000000;
DateTime dt = TimeInfo.Instance.ToDateTime(timeStamp);
Debug.Log(dt.ToString("yyyy-MM-dd HH:mm:ss"));

// DateTime → 时间戳
DateTime now = DateTime.Now;
long ts = TimeInfo.Instance.Transition(now);
```

### 示例 2: 服务器时间同步

```csharp
// 登录时获取服务器时间
long serverTime = GetServerTimeFromAPI();
long clientTime = TimeInfo.Instance.ClientNow();

// 计算差值
TimeInfo.Instance.ServerMinusClientTime = serverTime - clientTime;

// 后续使用服务器时间
long now = TimeInfo.Instance.ServerNow();
```

### 示例 3: 倒计时显示

```csharp
long remainingTime = 93784000;  // 毫秒

// 简单格式
string str1 = TimeInfo.Instance.TransitionToStr(remainingTime);
// "1d2h3m4s"

// 本地化格式
string str2 = TimeInfo.Instance.TransitionToStr2(remainingTime);
// "1 天 2 小时 3 分钟 4 秒"

countdownText.text = str2;
```

### 示例 4: 时区处理

```csharp
// 设置时区为东八区
TimeInfo.Instance.TimeZone = 8;

// 转换时间戳
long timeStamp = 1677600000000;
DateTime beijingTime = TimeInfo.Instance.ToDateTime(timeStamp);
Debug.Log($"北京时间：{beijingTime}");
```

---

## 设计要点

### 为什么需要 ServerMinusClientTime？

1. **防作弊**: 客户端时间可被篡改，服务器时间可信
2. **同步**: 确保所有客户端使用统一时间
3. **补偿**: 修正网络延迟造成的时间差

### 为什么使用单例？

1. **全局访问**: 任何地方都可以使用 `TimeInfo.Instance`
2. **状态共享**: TimeZone、ServerMinusClientTime 等状态需要全局一致
3. **性能**: 避免重复创建 DateTime 对象

### 本地化支持

```csharp
public string TransitionToStr2(long time)
{
    // 使用 I18NBridge 获取本地化文本
    sb.Append(h + I18NBridge.Instance.GetText("Text_Time_Hour"));
}
```

**优势**:
- 支持多语言
- 自动适配不同地区的时间格式

---

## 相关文档

- [GameTimerManager.cs.md](./GameTimerManager.cs.md) - 游戏时间管理器
- [TimerManager.cs.md](./TimerManager.cs.md) - 定时器管理器
- [I18NBridge.cs.md](../I18N/I18NBridge.cs.md) - 国际化桥接

---

*文档生成时间：2026-02-28 | OpenClaw AI 助手*
