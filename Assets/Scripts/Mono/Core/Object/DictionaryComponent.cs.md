# DictionaryComponent.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | DictionaryComponent.cs |
| **路径** | Assets/Scripts/Mono/Core/Object/DictionaryComponent.cs |
| **所属模块** | 框架层 → Mono/Core/Object |
| **命名空间** | `TaoTie` |
| **文件职责** | 提供可对象池复用的 Dictionary 组件 |

---

## 类说明

### DictionaryComponent<T, V>

| 属性 | 说明 |
|------|------|
| **职责** | 继承自 `Dictionary<T, V>`，支持对象池创建与回收，实现 `IDisposable` 接口 |
| **泛型参数** | `T` - 键类型<br>`V` - 值类型 |
| **继承关系** | `Dictionary<T, V>` |
| **实现的接口** | `IDisposable` |

**设计模式**: 对象池模式

---

## 方法说明

### Create()

**签名**:
```csharp
public static DictionaryComponent<T, V> Create()
```

**职责**: 从对象池获取 `DictionaryComponent<T, V>` 实例

---

### Dispose()

**签名**:
```csharp
public void Dispose()
```

**职责**: 清理字典并回收到对象池

**核心逻辑**:
```
1. 调用 Clear() 清空字典
2. 回收到 ObjectPool
```

---

## 使用示例

```csharp
// 创建字典
var dict = DictionaryComponent<string, int>.Create();

try
{
    // 添加数据
    dict.Add("key1", 100);
    dict.Add("key2", 200);

    // 访问
    int value = dict["key1"];

    // 遍历
    foreach (var kvp in dict)
    {
        Log.Info($"{kvp.Key}: {kvp.Value}");
    }
}
finally
{
    // 回收
    dict.Dispose();
}

// 或使用 using 语句
using (var dict = DictionaryComponent<string, int>.Create())
{
    dict["key"] = 100;
    // 自动回收
}
```

---

## 相关文档

- [ObjectPool.cs.md](../ObjectPool.cs.md) - 对象池核心
- [ListComponent.cs.md](./ListComponent.cs.md) - List 组件
- [HashSetComponent.cs.md](./HashSetComponent.cs.md) - HashSet 组件

---

*文档生成时间：2026-03-02 | OpenClaw AI 助手*
