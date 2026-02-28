# LinkedListComponent.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|------|
| **文件名** | LinkedListComponent.cs |
| **路径** | Assets/Scripts/Mono/Core/Object/LinkedListComponent.cs |
| **所属模块** | 框架层 → Mono/Core/Object |
| **文件职责** | 提供可对象池复用的 LinkedList 组件，减少 GC 压力 |

---

## 类/结构体说明

### LinkedListComponent<T>

| 属性 | 说明 |
|------|------|
| **职责** | 继承自 `LinkedList<T>`，支持对象池复用，自动清理和回收 |
| **泛型参数** | `T` - 链表元素类型 |
| **继承关系** | 继承 `LinkedList<T>`，实现 `IDisposable` |
| **实现的接口** | `IDisposable` |

**设计模式**: 对象池模式 + 工厂模式

```csharp
// 创建链表
using (var list = LinkedListComponent<int>.Create())
{
    list.AddLast(1);
    list.AddLast(2);
    list.AddLast(3);
    
    // 使用链表
    foreach (var item in list)
    {
        Debug.Log(item);
    }
    
    // using 结束时自动 Clear 并回收到对象池
}
```

---

## 方法说明

### Create

**签名**:
```csharp
public static LinkedListComponent<T> Create()
```

**职责**: 从对象池获取 LinkedListComponent 实例

**核心逻辑**:
```
1. 调用 ObjectPool.Instance.Fetch<LinkedListComponent<T>>()
2. 返回复用的实例
```

---

### Dispose

**签名**:
```csharp
public void Dispose()
```

**职责**: 清理链表并回收到对象池

**核心逻辑**:
```
1. 调用 Clear() 清空链表
2. 调用 ObjectPool.Instance.Recycle(this) 回收到对象池
```

---

## 使用示例

### 示例 1: 任务队列

```csharp
// 任务队列（需要频繁插入/删除）
using (var taskQueue = LinkedListComponent<Task>.Create())
{
    taskQueue.AddLast(task1);
    taskQueue.AddLast(task2);
    
    // 处理第一个任务
    Task first = taskQueue.First.Value;
    taskQueue.RemoveFirst();
    
    // 处理最后一个任务
    Task last = taskQueue.Last.Value;
    taskQueue.RemoveLast();
}
```

---

## 与 ListComponent 对比

| 特性 | ListComponent | LinkedListComponent |
|------|---------------|-------------------|
| **访问** | O(1) 索引访问 | O(n) 遍历访问 |
| **插入/删除** | O(n) | O(1)（已知位置） |
| **内存** | 紧凑 | 每个节点额外开销 |
| **使用场景** | 随机访问 | 频繁插入/删除 |

---

## 相关文档

- [ListComponent.cs.md](./ListComponent.cs.md) - 列表组件
- [ObjectPool.cs.md](./ObjectPool.cs.md) - 对象池实现

---

*文档生成时间：2026-02-28 | OpenClaw AI 助手*
