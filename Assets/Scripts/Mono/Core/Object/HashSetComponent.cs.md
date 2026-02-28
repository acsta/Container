# HashSetComponent.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|------|
| **文件名** | HashSetComponent.cs |
| **路径** | Assets/Scripts/Mono/Core/Object/HashSetComponent.cs |
| **所属模块** | 框架层 → Mono/Core/Object |
| **文件职责** | 提供可对象池复用的 HashSet 组件，减少 GC 压力 |

---

## 类/结构体说明

### HashSetComponent<T>

| 属性 | 说明 |
|------|------|
| **职责** | 继承自 `HashSet<T>`，支持对象池复用，自动清理和回收 |
| **泛型参数** | `T` - 集合元素类型 |
| **继承关系** | 继承 `HashSet<T>`，实现 `IDisposable` |
| **实现的接口** | `IDisposable` |

**设计模式**: 对象池模式 + 工厂模式

```csharp
// 创建 HashSet
using (var set = HashSetComponent<int>.Create())
{
    set.Add(1);
    set.Add(2);
    set.Add(3);
    
    // 使用集合
    if (set.Contains(2))
    {
        Debug.Log("包含 2");
    }
    
    // using 结束时自动 Clear 并回收到对象池
}
```

---

## 方法说明

### Create

**签名**:
```csharp
public static HashSetComponent<T> Create()
```

**职责**: 从对象池获取 HashSetComponent 实例

**核心逻辑**:
```
1. 调用 ObjectPool.Instance.Fetch<HashSetComponent<T>>()
2. 返回复用的实例
```

---

### Dispose

**签名**:
```csharp
public void Dispose()
```

**职责**: 清理集合并回收到对象池

**核心逻辑**:
```
1. 调用 Clear() 清空集合
2. 调用 ObjectPool.Instance.Recycle(this) 回收到对象池
```

---

## 使用示例

### 示例 1: 唯一元素收集

```csharp
// 收集唯一的敌人 ID
using (var enemyIds = HashSetComponent<long>.Create())
{
    foreach (var enemy in enemies)
    {
        enemyIds.Add(enemy.Id);
    }
    
    Debug.Log($"共有 {enemyIds.Count} 个不同的敌人");
}
```

### 示例 2: 快速查找

```csharp
// 创建白名单
using (var whitelist = HashSetComponent<string>.Create())
{
    whitelist.Add("admin");
    whitelist.Add("moderator");
    whitelist.Add("vip");
    
    // O(1) 查找
    if (whitelist.Contains(userRole))
    {
        GrantPrivileges();
    }
}
```

---

## 相关文档

- [ListComponent.cs.md](./ListComponent.cs.md) - 列表组件
- [DictionaryComponent.cs.md](./DictionaryComponent.cs.md) - 字典组件
- [ObjectPool.cs.md](./ObjectPool.cs.md) - 对象池实现

---

*文档生成时间：2026-02-28 | OpenClaw AI 助手*
