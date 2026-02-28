# AttributeManager.cs 注解文档

## 文件基本信息
- **路径**: Assets/Scripts/Mono/Module/Assembly/AttributeManager.cs
- **职责**: 特性管理器，扫描和管理所有标记了特定特性的类型

## 字段
| 字段 | 类型 | 说明 |
|------|------|------|
| Instance | AttributeManager | 单例实例 |
| types | UnOrderMultiMap<Type, Type> | 特性类型→标记该特性的类型列表 |
| Empty | List<Type> | 空列表（用于无结果时返回） |

## 方法
### GetTypes
```csharp
public List<Type> GetTypes(Type systemAttributeType)
```
获取所有标记了指定特性的类型

```csharp
// 获取所有标记了 [Timer] 的类型
List<Type> timerTypes = AttributeManager.Instance.GetTypes(typeof(TimerAttribute));

// 获取所有标记了 [Config] 的类型
List<Type> configTypes = AttributeManager.Instance.GetTypes(typeof(ConfigAttribute));
```

## 使用示例
```csharp
// 初始化（扫描所有程序集）
AttributeManager.Instance.Init();

// 获取定时器类型
var timerTypes = AttributeManager.Instance.GetTypes(typeof(TimerAttribute));
foreach (var type in timerTypes)
{
    // 创建定时器实例
}

// 获取配置类型
var configTypes = AttributeManager.Instance.GetTypes(typeof(ConfigAttribute));
foreach (var type in configTypes)
{
    // 加载配置
}
```
