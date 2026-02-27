# PriorityStack.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | PriorityStack.cs |
| **路径** | Assets/Scripts/Mono/Core/Object/PriorityStack/PriorityStack.cs |
| **所属模块** | 框架层 → Mono/Core/Object |
| **文件职责** | 实现优先级栈数据结构，支持按优先级排序的压栈/弹栈操作 |

---

## 类/结构体说明

### PriorityStack<T>

| 属性 | 说明 |
|------|------|
| **职责** | 按优先级管理的栈结构，高优先级元素先出栈 |
| **泛型参数** | T - 必须实现 IPriorityStackItem 接口 |
| **继承关系** | 无继承 |
| **实现的接口** | `IEnumerable<T>`, `IEnumerable` |

**数据结构**:
```
priorityList:  LinkedList<int>          // 优先级链表（降序排列）
                    ↓
priorityStacks:  UnOrderMultiMap<int,T> // 优先级 → 元素列表
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `priorityStacks` | `UnOrderMultiMap<int,T>` | `protected` | 核心存储，按优先级分组的多值映射 |
| `priorityList` | `LinkedList<int>` | `protected` | 优先级链表，维护优先级顺序（降序） |
| `count` | `int` | `protected` | 元素总数 |
| `Count` | `int` | `public` | 只读属性，返回元素总数 |
| `Data` | `UnOrderMultiMap<int,T>` | `public` | 暴露内部存储，用于调试或高级操作 |

---

## 方法说明（按重要程度排序）

### Push(T item)

**签名**:
```csharp
public void Push(T item)
```

**职责**: 将元素压入栈中，按优先级排序

**核心逻辑**:
```
1. 遍历 priorityList 查找插入位置
2. 如果找到相同优先级，直接添加到该优先级组
3. 如果找到更小的优先级，在其之前插入新优先级节点
4. 如果遍历完都没找到，添加到链表末尾（最低优先级）
5. 将元素添加到 priorityStacks 对应优先级组
6. count++
```

**调用者**: 需要按优先级管理对象的系统

**被调用者**: `priorityList.AddBefore`, `priorityList.AddLast`, `priorityStacks.Add`

---

### Pop(int index = 0)

**签名**:
```csharp
public T Pop(int index = 0)
```

**职责**: 弹出栈顶元素（默认最高优先级第一个）

**核心逻辑**:
```
1. 通过索引器获取元素
2. 从 priorityStacks 中移除该元素
3. 如果该优先级组为空，从 priorityList 移除该优先级节点
4. count--
5. 返回元素
```

**调用者**: 需要获取并移除最高优先级对象的系统

**被调用者**: `this[index]`, `priorityStacks.Remove`, `priorityList.Remove`

---

### Peek(int index = 0)

**签名**:
```csharp
public T Peek(int index = 0)
```

**职责**: 查看栈顶元素但不移除

**核心逻辑**: 直接通过索引器返回元素

**调用者**: 需要查看但不移除元素的场景

---

### Remove(T res)

**签名**:
```csharp
public T Remove(T res)
```

**职责**: 移除指定元素（任意位置）

**核心逻辑**:
```
1. 从 priorityStacks 移除该元素
2. 如果该优先级组为空，从 priorityList 移除该优先级节点
3. count--
4. 返回被移除的元素
```

**调用者**: 需要移除特定对象的场景

---

### 索引器 this[int index]

**签名**:
```csharp
public T this[int index]
```

**职责**: 按全局索引访问元素（从栈顶开始计数）

**核心逻辑**:
```
1. 遍历 priorityList（从高优先级到低优先级）
2. 对于每个优先级组，计算是否在该组内
3. 如果 index >= 组大小，index -= 组大小，继续下一组
4. 否则，返回该组内对应位置的元素（倒序，栈顶优先）
```

**示例**:
```
优先级链表：[10, 5, 1]  // 三个优先级组
priorityStacks:
  10 → [A, B]  // 2 个元素
  5  → [C]     // 1 个元素
  1  → [D, E]  // 2 个元素

索引访问:
  [0] → B (优先级 10 的栈顶)
  [1] → A (优先级 10 的第二个)
  [2] → C (优先级 5 的栈顶)
  [3] → E (优先级 1 的栈顶)
  [4] → D (优先级 1 的第二个)
```

---

### GetEnumerator()

**签名**:
```csharp
IEnumerator IEnumerable.GetEnumerator()
IEnumerator<T> IEnumerable<T>.GetEnumerator()
```

**职责**: 支持 foreach 遍历

**核心逻辑**: 遍历 priorityStacks 中的所有元素

**使用示例**:
```csharp
foreach (var item in priorityStack)
{
    // 处理元素
}
```

---

## Unity 生命周期集成

PriorityStack 是纯数据结构，不直接集成 Unity 生命周期。

**典型使用场景**:
```csharp
// 在 Manager 中使用
public class SomeManager : IManager
{
    private PriorityStack<TaskItem> taskStack;
    
    public void Init()
    {
        taskStack = new PriorityStack<TaskItem>();
    }
    
    public void Update()
    {
        // 每帧处理最高优先级任务
        if (taskStack.Count > 0)
        {
            var task = taskStack.Pop();
            task.Execute();
        }
    }
    
    public void Destroy()
    {
        taskStack = null;
    }
}
```

---

## 阅读指引

### 建议的阅读顺序

1. **先看 IPriorityStackItem.cs** - 了解元素接口要求
2. **再看字段定义** - 理解数据结构
3. **重点看 Push 方法** - 理解优先级插入逻辑
4. **最后看索引器** - 理解跨优先级组的索引计算

### 最值得学习的技术点

1. **优先级链表维护**: 使用 LinkedList 动态维护优先级顺序
2. **多值映射**: UnOrderMultiMap 支持一个优先级对应多个元素
3. **栈的倒序访问**: 同优先级内后进先出（`list.Count - 1 - index`）
4. **泛型约束**: `where T : IPriorityStackItem` 确保元素有优先级

---

## 使用示例

### 示例 1: 任务调度

```csharp
public class TaskItem : IPriorityStackItem
{
    public int Priority { get; set; }
    public string Name { get; set; }
    public Action Action { get; set; }
    
    public void Execute() => Action?.Invoke();
}

// 使用
var taskStack = new PriorityStack<TaskItem>();
taskStack.Push(new TaskItem { Priority = 1, Name = "低优先级任务" });
taskStack.Push(new TaskItem { Priority = 10, Name = "高优先级任务" });
taskStack.Push(new TaskItem { Priority = 5, Name = "中优先级任务" });

// 弹出顺序：高 → 中 → 低
while (taskStack.Count > 0)
{
    var task = taskStack.Pop();
    task.Execute();
}
```

### 示例 2: 事件处理

```csharp
public class Event : IPriorityStackItem
{
    public int Priority { get; set; }  // 紧急程度
    public string Type { get; set; }
}

// 高优先级事件优先处理
var eventStack = new PriorityStack<Event>();
eventStack.Push(new Event { Priority = 100, Type = "崩溃" });
eventStack.Push(new Event { Priority = 50, Type = "警告" });
eventStack.Push(new Event { Priority = 10, Type = "提示" });
```

---

## 相关文档

- [IPriorityStackItem.cs.md](./IPriorityStackItem.cs.md) - 元素接口定义
- [UnOrderMultiMap.cs.md](../UnOrderMultiMap.cs.md) - 底层存储结构
- [ManagerProvider.cs.md](../../Manager/ManagerProvider.cs.md) - Manager 系统

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
