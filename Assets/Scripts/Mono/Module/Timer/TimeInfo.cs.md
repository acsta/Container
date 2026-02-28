# TimeInfo.cs 注解文档

## 文件基本信息
- **路径**: Assets/Scripts/Mono/Module/Timer/TimeInfo.cs
- **职责**: 单例时间信息服务，处理时区、时间戳转换、客户端/服务器时间同步

## 常量
| 常量 | 值 | 说明 |
|------|-----|------|
| OneDay | 86400000 | 一天的毫秒数 |
| Hour | 3600000 | 一小时的毫秒数 |
| Minute | 60000 | 一分钟的毫秒数 |

## 字段
| 字段 | 类型 | 说明 |
|------|------|------|
| TimeZone | int | 时区（小时） |
| ServerMinusClientTime | long | 服务器时间 - 客户端时间（用于同步） |
| FrameTime | long | 当前帧时间 |

## 方法
### ToDateTime
```csharp
public DateTime ToDateTime(long timeStamp)
```
将时间戳转换为 DateTime

### ClientNow
```csharp
public long ClientNow()
```
获取当前客户端时间戳（毫秒）

### ServerNow
```csharp
public long ServerNow()
```
获取当前服务器时间戳（考虑时差）

### Transition
```csharp
public long Transition(DateTime d)
```
将 DateTime 转换为时间戳

### TransitionToStr
```csharp
public string TransitionToStr(long time)
```
将毫秒转换为 "Xd Xh Xm Xs" 格式

### TransitionToStr2
```csharp
public string TransitionToStr2(long time)
```
将毫秒转换为本地化格式（使用 I18NBridge）

## 使用示例
```csharp
// 获取服务器时间
long serverTime = TimeInfo.Instance.ServerNow();

// 时间戳转 DateTime
DateTime dt = TimeInfo.Instance.ToDateTime(serverTime);

// 时间差格式化
long remaining = 3725000;  // 1 小时 2 分 5 秒
string str = TimeInfo.Instance.TransitionToStr(remaining);
// 输出："1h2m5s"
```
