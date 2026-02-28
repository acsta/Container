# UnOrderMultiMapSet.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|------|
| **文件名** | UnOrderMultiMapSet.cs |
| **路径** | Assets/Scripts/Mono/Core/Object/UnOrderMultiMapSet.cs |
| **所属模块** | 框架层 → Mono/Core/Object |
| **文件职责** | 提供无序多对多映射的数据结构，性能优于有序版本 |

---

## 类/结构体说明

### UnOrderMultiMapSet<K, V>

| 属性 | 说明 |
|------|------|
| **职责** | 无序多对多映射，一个键可以对应多个值（使用 HashSet 存储值） |
| **泛型参数** | `K` - 键类型，`V` - 值类型 |
| **继承关系** | 无 |
| **实现的接口** | 无 |

**设计模式**: 多值字典模式

```csharp
// 创建无序多对多映射
var multiMap = new UnOrderMultiMapSet<string, int>();

multiMap.Add("fruits", 1);
multiMap.Add("fruits", 2);

// 获取所有水果
HashSet<int> fruits = multiMap["fruits"];
```

---

## 与 MultiMapSet 对比

| 特性 | MultiMapSet | UnOrderMultiMapSet |
|------|-------------|-------------------|
| **顺序** | 有序 | 无序 |
| **性能** | 中 | 高 |
| **内部实现** | SortedDictionary | Dictionary |
| **使用场景** | 需要排序 | 不需要排序 |

---

## 使用示例

### 示例 1: 属性管理器

```csharp
// 按特性类型索引类
UnOrderMultiMapSet<Type, Type> types = new UnOrderMultiMapSet<Type, Type>();

// 添加
types.Add(typeof(TimerAttribute), typeof(HeartbeatTimer));
types.Add(typeof(TimerAttribute), typeof(ResetTimeScaleTimer));

// 获取所有定时器类
HashSet<Type> timerTypes = types[typeof(TimerAttribute)];
```

---

## 相关文档

- [MultiMapSet.cs.md](./MultiMapSet.cs.md) - 有序多对多映射
- [AttributeManager.cs.md](../../Module/Assembly/AttributeManager.cs.md) - 属性管理器（使用 UnOrderMultiMapSet）

---

*文档生成时间：2026-02-28 | OpenClaw AI 助手*
