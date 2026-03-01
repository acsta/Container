# TypeInfo.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | TypeInfo.cs |
| **路径** | Assets/Scripts/Mono/Helper/TypeInfo.cs |
| **所属模块** | Mono/Helper |
| **文件职责** | 泛型类型信息缓存，避免重复反射查询 |

---

## 类说明

### TypeInfo<T>

| 属性 | 说明 |
|------|------|
| **职责** | 静态缓存泛型类型的 Type、HashCode、TypeName |
| **类型** | `static class` |
| **设计模式** | 泛型静态缓存 |

---

## 字段

| 名称 | 类型 | 说明 |
|------|------|------|
| `Type` | `Type` | 类型对象 `typeof(T)` |
| `HashCode` | `int` | 类型哈希码 |
| `TypeName` | `string` | 类型名称 |

---

## 使用示例

```csharp
// 获取类型信息 (无反射开销)
Type type = TypeInfo<Player>.Type;
int hash = TypeInfo<Player>.HashCode;
string name = TypeInfo<Player>.TypeName;

// 对比传统方式
Type type = typeof(Player);           // 每次都会查询
int hash = typeof(Player).GetHashCode();
string name = typeof(Player).Name;
```

---

## 优势

- **零运行时开销**: 静态字段在类型加载时初始化
- **类型安全**: 泛型参数编译时检查
- **代码简洁**: 避免重复写 `typeof(T)`

---

## 相关文档

- [ObjectPool.cs.md](../Core/ObjectPool.cs.md) - 对象池 (使用 TypeInfo 优化)

---

*文档生成时间：2026-03-02 | OpenClaw AI 助手*
