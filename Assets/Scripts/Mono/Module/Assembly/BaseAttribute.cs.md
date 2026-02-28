# BaseAttribute.cs 注解文档

## 文件基本信息
- **路径**: Assets/Scripts/Mono/Module/Assembly/BaseAttribute.cs
- **职责**: 所有自定义特性的基类

## 类说明
### BaseAttribute
```csharp
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class BaseAttribute: Attribute
```
- **继承**: System.Attribute
- **用途**: 作为所有项目自定义特性的父类
- **AllowMultiple**: true（一个类可以有多个相同特性）

## 子类示例
```csharp
// 定时器特性
public class TimerAttribute: BaseAttribute
{
    public int Type { get; }
}

// 配置特性
public class ConfigAttribute: BaseAttribute
{
}
```

## 使用示例
```csharp
[Timer(TimerType.FrameUpdate)]
public class FrameUpdateTimer : ATimer<Entity>
{
}

[Config]
public class LevelConfig : ProtoObject
{
}
```
