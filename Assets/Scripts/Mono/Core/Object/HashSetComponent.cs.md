# HashSetComponent.cs 注解文档

## 文件基本信息
- **路径**: Assets/Scripts/Mono/Core/Object/HashSetComponent.cs
- **职责**: 提供可对象池复用的 HashSet 组件

## 类说明
### HashSetComponent<T>
- **继承**: HashSet<T>, IDisposable
- **设计模式**: 对象池模式

## 方法
### Create
```csharp
public static HashSetComponent<T> Create()
```
从对象池获取实例

### Dispose
```csharp
public void Dispose()
```
清理并回收到对象池

## 使用示例
```csharp
using (var set = HashSetComponent<int>.Create())
{
    set.Add(1);
    set.Add(2);
    // 使用...
}  // 自动回收
```
