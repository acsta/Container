# ITimer.cs 注解文档

## 文件基本信息
- **路径**: Assets/Scripts/Mono/Module/Timer/ITimer.cs
- **职责**: 定义定时器接口和抽象基类

## 接口说明
### ITimer
```csharp
public interface ITimer
{
    void Handle(object args);
}
```
- **职责**: 定时器处理器接口

## 抽象类说明
### ATimer<T>
```csharp
public abstract class ATimer<T>: ITimer where T: class
{
    public void Handle(object args)
    {
        this.Run(args as T);
    }

    public abstract void Run(T t);
}
```
- **职责**: 定时器处理器抽象基类
- **泛型**: T - 目标对象类型
- **设计模式**: 模板方法模式

## 使用示例
```csharp
[Timer(TimerType.SkillCooldown)]
public class SkillCooldownTimer : ATimer<Skill>
{
    public override void Run(Skill skill)
    {
        skill.OnCooldownEnd();
    }
}
```
