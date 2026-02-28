# UnOrderDoubleKeyMapSet.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|------|
| **文件名** | UnOrderDoubleKeyMapSet.cs |
| **路径** | Assets/Scripts/Mono/Core/Object/UnOrderDoubleKeyMapSet.cs |
| **所属模块** | 框架层 → Mono/Core/Object |
| **文件职责** | 提供双键多值映射的数据结构 |

---

## 类/结构体说明

### UnOrderDoubleKeyMapSet<K1, K2, V>

| 属性 | 说明 |
|------|------|
| **职责** | 双键多值映射，可以通过 K1 或 K2 访问多个值（使用 HashSet 存储） |
| **泛型参数** | `K1` - 第一个键类型，`K2` - 第二个键类型，`V` - 值类型 |
| **继承关系** | 无 |
| **实现的接口** | 无 |

**设计模式**: 双向索引 + 多值模式

```csharp
// 创建双键多值映射
var map = new UnOrderDoubleKeyMapSet<long, string, Entity>();

// 添加
map.Add(entity.Zone, entity.Type, entity);

// 通过区服查找
HashSet<Entity> zoneEntities = map.GetByKey1(1);

// 通过类型查找
HashSet<Entity> typeEntities = map.GetByKey2("enemy");
```

---

## 使用示例

### 示例 1: 多条件查询

```csharp
// 按区服和类型查找实体
UnOrderDoubleKeyMapSet<int, string, Entity> entities = new UnOrderDoubleKeyMapSet<int, string, Entity>();

// 添加实体
entities.Add(1, "enemy", enemy1);
entities.Add(1, "enemy", enemy2);
entities.Add(1, "npc", npc1);

// 查找 1 区所有敌人
HashSet<Entity> enemies = entities.GetByKey1(1);

// 查找所有 NPC
HashSet<Entity> npcs = entities.GetByKey2("npc");
```

---

## 相关文档

- [UnOrderDoubleKeyMap.cs.md](./UnOrderDoubleKeyMap.cs.md) - 单值版本

---

*文档生成时间：2026-02-28 | OpenClaw AI 助手*
