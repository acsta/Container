# TimerAttribute.cs 注解文档

## 文件基本信息
- **路径**: Assets/Scripts/Mono/Module/Timer/TimerAttribute.cs
- **职责**: 定义定时器特性，用于标记定时器处理类

## 类说明
### TimerAttribute
- **继承**: BaseAttribute
- **用途**: 标记一个类为定时器处理器，关联定时器类型

```csharp
[Timer(TimerType.ResetTimeScale)]
public class ResetTimeScale : ATimer<GameTimerManager>
{
    public override void Run(GameTimerManager t)
    {
        t.SetTimeScale(1);
    }
}
```

## 字段
| 字段 | 类型 | 说明 |
|------|------|------|
| Type | int | 定时器类型（来自 TimerType 枚举） |

## 使用示例
```csharp
// 定义定时器
[Timer(TimerType.FrameUpdate)]
public class FrameUpdateTimer : ATimer<Entity>
{
    public override void Run(Entity e)
    {
        e.UpdateFrame();
    }
}

// 创建定时器
long timerId = TimerManager.Instance.NewTimer(
    TimerType.FrameUpdate, 
    targetObject
);
```
