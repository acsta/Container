# MultiMapSet.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|------|
| **文件名** | MultiMapSet.cs |
| **路径** | Assets/Scripts/Mono/Core/Object/MultiMapSet.cs |
| **所属模块** | 框架层 → Mono/Core/Object |
| **文件职责** | 提供多对多映射的数据结构，一个键可以对应多个值 |

---

## 类/结构体说明

### MultiMapSet<K, V>

| 属性 | 说明 |
|------|------|
| **职责** | 多对多映射，一个键可以对应多个值（使用 HashSet 存储值） |
| **泛型参数** | `K` - 键类型，`V` - 值类型 |
| **继承关系** | 无 |
| **实现的接口** | 无 |

**设计模式**: 多值字典模式

```csharp
// 创建多对多映射
var multiMap = new MultiMapSet<string, int>();

multiMap.Add("fruits", 1);
multiMap.Add("fruits", 2);
multiMap.Add("vegetables", 3);

// 获取所有水果
HashSet<int> fruits = multiMap["fruits"];  // {1, 2}
```

---

## 使用示例

### 示例 1: 分类存储

```csharp
// 按类型存储实体
MultiMapSet<string, Entity> entitiesByType = new MultiMapSet<string, Entity>();

foreach (var entity in allEntities)
{
    entitiesByType.Add(entity.Type, entity);
}

// 获取所有敌人
HashSet<Entity> enemies = entitiesByType["enemy"];
```

### 示例 2: 标签系统

```csharp
// 按标签查找对象
MultiMapSet<string, GameObject> objectsByTag = new MultiMapSet<string, GameObject>();

objectsByTag.Add("Player", player);
objectsByTag.Add("Enemy", enemy1);
objectsByTag.Add("Enemy", enemy2);

// 查找所有敌人
HashSet<GameObject> enemies = objectsByTag.Get("Enemy");
```

---

## 相关文档

- [UnOrderMultiMapSet.cs.md](./UnOrderMultiMapSet.cs.md) - 无序多对多映射
- [DictionaryComponent.cs.md](./DictionaryComponent.cs.md) - 字典组件

---

*文档生成时间：2026-02-28 | OpenClaw AI 助手*
