# UnOrderMultiMap.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|------|
| **文件名** | UnOrderMultiMap.cs |
| **路径** | Assets/Scripts/Mono/Core/Object/UnOrderMultiMap.cs |
| **所属模块** | 框架层 → Mono/Core/Object |
| **文件职责** | 提供无序多对多映射的数据结构，使用 List 存储值 |

---

## 类/结构体说明

### UnOrderMultiMap<K, V>

| 属性 | 说明 |
|------|------|
| **职责** | 无序多对多映射，一个键可以对应多个值（使用 List 存储值） |
| **泛型参数** | `K` - 键类型，`V` - 值类型 |
| **继承关系** | 无 |
| **实现的接口** | 无 |

**设计模式**: 多值字典模式

```csharp
// 创建无序多对多映射
var multiMap = new UnOrderMultiMap<string, int>();

multiMap.Add("fruits", 1);
multiMap.Add("fruits", 2);

// 获取所有水果
List<int> fruits = multiMap["fruits"];
```

---

## 与 UnOrderMultiMapSet 对比

| 特性 | UnOrderMultiMap | UnOrderMultiMapSet |
|------|-----------------|-------------------|
| **值存储** | List | HashSet |
| **重复值** | ✅ 允许 | ❌ 不允许 |
| **查找速度** | O(n) | O(1) |
| **使用场景** | 允许重复 | 不允许重复 |

---

## 使用示例

### 示例 1: 程序集类型映射

```csharp
// 按程序集索引类型
UnOrderMultiMap<Assembly, Type> mapTypes = new UnOrderMultiMap<Assembly, Type>();

foreach (Type type in assembly.GetTypes())
{
    mapTypes.Add(assembly, type);
}

// 获取程序集的所有类型
List<Type> types = mapTypes[assembly];
```

---

## 相关文档

- [UnOrderMultiMapSet.cs.md](./UnOrderMultiMapSet.cs.md) - 使用 HashSet 的版本
- [AssemblyManager.cs.md](../../Module/Assembly/AssemblyManager.cs.md) - 程序集管理器（使用 UnOrderMultiMap）

---

*文档生成时间：2026-02-28 | OpenClaw AI 助手*
