# UnOrderDoubleKeyMap.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|------|
| **文件名** | UnOrderDoubleKeyMap.cs |
| **路径** | Assets/Scripts/Mono/Core/Object/UnOrderDoubleKeyMap.cs |
| **所属模块** | 框架层 → Mono/Core/Object |
| **文件职责** | 提供双键映射的数据结构，可以通过两个键访问值 |

---

## 类/结构体说明

### UnOrderDoubleKeyMap<K1, K2, V>

| 属性 | 说明 |
|------|------|
| **职责** | 双键映射，可以通过 K1 或 K2 访问值 V |
| **泛型参数** | `K1` - 第一个键类型，`K2` - 第二个键类型，`V` - 值类型 |
| **继承关系** | 无 |
| **实现的接口** | 无 |

**设计模式**: 双向索引模式

```csharp
// 创建双键映射
var map = new UnOrderDoubleKeyMap<long, string, Entity>();

// 添加
map.Add(entity.Id, entity.Name, entity);

// 通过 ID 查找
Entity e1 = map.GetByKey1(entityId);

// 通过名称查找
Entity e2 = map.GetByKey2(entityName);
```

---

## 使用示例

### 示例 1: 实体查找

```csharp
// 通过 ID 或名称查找实体
UnOrderDoubleKeyMap<long, string, Entity> entities = new UnOrderDoubleKeyMap<long, string, Entity>();

entities.Add(player.Id, player.Name, player);

// 通过 ID 查找
Entity playerById = entities.GetByKey1(123);

// 通过名称查找
Entity playerByName = entities.GetByKey2("Player1");
```

---

## 相关文档

- [UnOrderDoubleKeyMapSet.cs.md](./UnOrderDoubleKeyMapSet.cs.md) - 使用 HashSet 存储值的版本

---

*文档生成时间：2026-02-28 | OpenClaw AI 助手*
