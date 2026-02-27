# IPriorityStackItem.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | IPriorityStackItem.cs |
| **路径** | Assets/Scripts/Mono/Core/Object/PriorityStack/IPriorityStackItem.cs |
| **所属模块** | 框架层 → Mono/Core/Object/PriorityStack |
| **文件职责** | 定义可放入优先级栈的元素接口 |

---

## 类/结构体说明

### IPriorityStackItem

| 属性 | 说明 |
|------|------|
| **职责** | 标记接口，要求实现类提供优先级属性 |
| **泛型参数** | 无 |
| **继承关系** | 无 |
| **实现的接口** | 无 |

```csharp
public interface IPriorityStackItem
{
    int Priority { get; }
}
```

---

## 字段与属性

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `Priority` | `int` | `get` | 优先级值，越大优先级越高 |

**优先级约定**:
- **数值越大，优先级越高**
- 推荐范围：0-100（可根据需求扩展）
- 相同优先级的元素按后进先出（LIFO）顺序处理

---

## 方法说明

本接口只定义属性，无方法。

---

## 实现示例

### 示例 1: 任务项

```csharp
public class TaskItem : IPriorityStackItem
{
    public int Priority { get; set; }
    public string TaskName { get; set; }
    public Action<TaskItem> OnExecute { get; set; }
    
    public void Execute()
    {
        OnExecute?.Invoke(this);
    }
}

// 使用
var task = new TaskItem 
{ 
    Priority = 10, 
    TaskName = "加载资源",
    OnExecute = t => Debug.Log($"执行任务：{t.TaskName}")
};

var stack = new PriorityStack<TaskItem>();
stack.Push(task);
```

---

### 示例 2: 游戏事件

```csharp
public class GameEvent : IPriorityStackItem
{
    public int Priority { get; set; }
    public string EventType { get; set; }
    public object Data { get; set; }
    
    // 紧急事件优先级高
    public static class Priorities
    {
        public const int Critical = 100;  // 崩溃、断网
        public const int High = 80;       // 战斗伤害
        public const int Normal = 50;     // 普通操作
        public const int Low = 10;        // 日志、提示
    }
}

// 使用
var eventStack = new PriorityStack<GameEvent>();
eventStack.Push(new GameEvent { Priority = GameEvent.Priorities.Critical, EventType = "Disconnect" });
eventStack.Push(new GameEvent { Priority = GameEvent.Priorities.Normal, EventType = "Move" });
```

---

### 示例 3: AI 决策

```csharp
public class AIDecision : IPriorityStackItem
{
    public int Priority { get; set; }
    public string DecisionType { get; set; }
    public float Score { get; set; }
    
    // 根据分数动态计算优先级
    public void CalculatePriority()
    {
        Priority = Mathf.FloorToInt(Score * 100);
    }
}

// 使用
var decisionStack = new PriorityStack<AIDecision>();
decisionStack.Push(new AIDecision { DecisionType = "Attack", Score = 0.9f });
decisionStack.Push(new AIDecision { DecisionType = "Defend", Score = 0.5f });

// 优先执行得分最高的决策
var bestDecision = decisionStack.Pop();
```

---

## 设计模式

### 标记接口模式

IPriorityStackItem 是一个典型的标记接口（Marker Interface）：
- 不包含方法，只定义属性
- 用于约束泛型类型
- 提供编译时类型检查

```csharp
// 泛型约束确保只有实现接口的类型才能使用 PriorityStack
public class PriorityStack<T> where T : IPriorityStackItem
{
    // ...
}
```

---

## 与 PriorityStack 的协作

```
┌─────────────────────────────────────┐
│     PriorityStack<T>                │
│  (where T : IPriorityStackItem)     │
├─────────────────────────────────────┤
│  Push(item)                         │
│    ↓                                │
│  读取 item.Priority                 │
│    ↓                                │
│  按优先级插入到 priorityList        │
│    ↓                                │
│  添加到 priorityStacks[item.Priority]│
└─────────────────────────────────────┘
```

---

## 阅读指引

### 建议的阅读顺序

1. **理解接口作用** - 为什么需要这个接口
2. **查看实现示例** - 如何在自己的类中实现
3. **了解优先级约定** - 数值大小与优先级的关系

### 最值得学习的技术点

1. **接口约束**: 通过 `where T : IPriorityStackItem` 确保类型安全
2. **只读属性**: `get` 访问器确保优先级不可被外部修改
3. **简单即有效**: 单一属性的接口设计，职责清晰

---

## 注意事项

### 优先级不变性

**建议**: 元素加入栈后不应修改 Priority

```csharp
// ❌ 错误做法
var item = new TaskItem { Priority = 10 };
stack.Push(item);
item.Priority = 50;  // 会破坏栈的优先级顺序！

// ✅ 正确做法
var item = new TaskItem { Priority = 10 };
stack.Push(item);
// 如需改变优先级，先移除再重新添加
stack.Remove(item);
item.Priority = 50;
stack.Push(item);
```

### 优先级相同时的行为

相同优先级的元素按 **后进先出（LIFO）** 顺序处理：

```csharp
var stack = new PriorityStack<TaskItem>();
stack.Push(new TaskItem { Priority = 5, Name = "A" });
stack.Push(new TaskItem { Priority = 5, Name = "B" });
stack.Push(new TaskItem { Priority = 5, Name = "C" });

// 弹出顺序：C → B → A
```

---

## 相关文档

- [PriorityStack.cs.md](./PriorityStack.cs.md) - 优先级栈实现
- [UnOrderMultiMap.cs.md](../UnOrderMultiMap.cs.md) - 底层存储

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
