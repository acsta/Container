# TimerAction.cs 注解文档

## 文件基本信息
- **路径**: Assets/Scripts/Mono/Module/Timer/TimerAction.cs
- **职责**: 定时器动作对象，可对象池复用

## 字段
| 字段 | 类型 | 说明 |
|------|------|------|
| TimerClass | TimerClass | 定时器分类 |
| Object | object | 目标对象 |
| Time | long | 触发时间 |
| Type | int | 定时器类型 |
| Id | long | 唯一标识 |

## 方法
### Create
```csharp
public static TimerAction Create(TimerClass timerClass, long time, int type, object obj)
```
从对象池创建 TimerAction 实例

### Dispose
```csharp
public void Dispose()
```
清理并回收到对象池

## 使用示例
```csharp
// 创建定时器动作
var action = TimerAction.Create(
    TimerClass.Once,
    GameTimerManager.Instance.GetTimeNow() + 1000,
    TimerType.SkillCooldown,
    skill
);

// 使用完毕后回收
action.Dispose();
```
