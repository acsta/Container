# TaskState.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | TaskState.cs |
| **路径** | Assets/Scripts/Code/Module/Const/TaskState.cs |
| **所属模块** | 框架层 → Code/Module/Const |
| **文件职责** | 任务状态标志枚举定义 |

---

## 枚举说明

### TaskState

| 属性 | 说明 |
|------|------|
| **职责** | 定义任务的状态标志（支持位运算） |
| **类型** | enum（Flags） |

```csharp
[Flags]
public enum TaskState
{
    Running = 1,      // 0b001
    NoStart = 2,      // 0b010
    Over = 4,         // 0b100
    All = 7           // 0b111
}
```

---

## 状态标志

| 值 | 枚举 | 二进制 | 说明 |
|------|------|--------|------|
| 1 | `Running` | 0b001 | 进行中 |
| 2 | `NoStart` | 0b010 | 未开始 |
| 4 | `Over` | 0b100 | 已完成 |
| 7 | `All` | 0b111 | 所有状态（掩码） |

---

## Flags 特性

### 什么是 Flags？

`[Flags]` 特性表示枚举可以进行位运算，支持组合状态。

### 位运算示例

```csharp
// 组合状态
TaskState state = TaskState.Running | TaskState.NoStart;

// 检查是否包含某个状态
if ((state & TaskState.Running) != 0)
{
    Log.Info("任务进行中");
}

// 移除某个状态
state &= ~TaskState.NoStart;
```

---

## 使用示例

### 示例 1: 设置任务状态

```csharp
// 设置任务为进行中
task.State = TaskState.Running;

// 设置任务为未开始
task.State = TaskState.NoStart;

// 设置任务为已完成
task.State = TaskState.Over;
```

### 示例 2: 检查任务状态

```csharp
// 检查是否进行中
if ((task.State & TaskState.Running) != 0)
{
    Log.Info("任务进行中");
}

// 检查是否未开始
if ((task.State & TaskState.NoStart) != 0)
{
    Log.Info("任务未开始");
}

// 检查是否已完成
if ((task.State & TaskState.Over) != 0)
{
    Log.Info("任务已完成");
}
```

### 示例 3: 组合状态

```csharp
// 设置组合状态（进行中 + 未开始）
task.State = TaskState.Running | TaskState.NoStart;

// 检查是否包含任意状态
if ((task.State & TaskState.All) != 0)
{
    Log.Info("任务有任何状态");
}
```

### 示例 4: 状态转换

```csharp
// 从未开始 → 进行中
if ((task.State & TaskState.NoStart) != 0)
{
    task.State = TaskState.Running;
}

// 从进行中 → 已完成
if ((task.State & TaskState.Running) != 0)
{
    task.State = TaskState.Over;
}
```

### 示例 5: 筛选任务

```csharp
// 筛选所有进行中的任务
var runningTasks = tasks.Where(t => 
    (t.State & TaskState.Running) != 0
).ToList();

// 筛选所有未开始的任务
var noStartTasks = tasks.Where(t => 
    (t.State & TaskState.NoStart) != 0
).ToList();

// 筛选所有已完成的任务
var overTasks = tasks.Where(t => 
    (t.State & TaskState.Over) != 0
).ToList();
```

---

## 状态流转

```
未开始 (NoStart)
    ↓ 接受任务
进行中 (Running)
    ↓ 完成任务
已完成 (Over)
```

---

## 相关文档

- [PlayerData.cs.md](../Player/PlayerData.cs.md) - 玩家数据（包含任务相关字段）
- [GameConst.cs.md](./GameConst.cs.md) - 游戏常量

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
