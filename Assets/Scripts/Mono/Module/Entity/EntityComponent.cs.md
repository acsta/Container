# EntityComponent.cs 注解文档

## 文件基本信息
- **路径**: Assets/Scripts/Mono/Module/Entity/EntityComponent.cs
- **职责**: Unity MonoBehaviour 组件，标识 GameObject 为游戏实体

## 字段
| 字段 | 类型 | 说明 |
|------|------|------|
| Id | long | 实体唯一标识 |
| EntityType | EntityType | 实体类型枚举 |
| CampId | uint | 阵营 ID |
| HolderIndex | int | 持有者索引 |

## 使用示例
```csharp
// 获取实体组件
var entity = gameObject.GetComponent<EntityComponent>();

// 设置实体信息
entity.Id = 12345;
entity.EntityType = EntityType.Player;
entity.CampId = 1;  // 玩家阵营
entity.HolderIndex = 0;

// 根据阵营判断敌友
if (entity.CampId == myCampId)
{
    // 友方
}
else
{
    // 敌方
}
```
