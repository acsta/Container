# UnityLifeTimeHelper.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | UnityLifeTimeHelper.cs |
| **路径** | Assets/Scripts/Mono/Helper/UnityLifeTimeHelper.cs |
| **所属模块** | Mono/Helper |
| **文件职责** | Unity 生命周期等待工具，提供帧级异步等待 |

---

## 类说明

### UnityLifeTimeHelper

| 属性 | 说明 |
|------|------|
| **职责** | 提供等待 Unity Update/LateUpdate/FixedUpdate/Frame 结束的异步方法 |
| **类型** | `static class` |

---

## 字段

| 名称 | 类型 | 说明 |
|------|------|------|
| `UpdateFinishTask` | `Queue<ETTask>` | 等待 Update 结束的任务队列 |
| `LateUpdateFinishTask` | `Queue<ETTask>` | 等待 LateUpdate 结束的任务队列 |
| `FixedUpdateFinishTask` | `Queue<ETTask>` | 等待 FixedUpdate 结束的任务队列 |
| `FrameFinishTask` | `Queue<ETTask>` | 等待帧结束的任务队列 |

---

## 方法说明

### WaitUpdateFinish()

```csharp
public static ETTask WaitUpdateFinish()
```

**职责**: 等待当前帧 Update 结束

---

### WaitLateUpdateFinish()

```csharp
public static ETTask WaitLateUpdateFinish()
```

**职责**: 等待当前帧 LateUpdate 结束

---

### WaitFixedUpdateFinish()

```csharp
public static ETTask WaitFixedUpdateFinish()
```

**职责**: 等待当前帧 FixedUpdate 结束

---

### WaitFrameFinish()

```csharp
public static ETTask WaitFrameFinish()
```

**职责**: 等待当前帧结束

---

## 使用示例

```csharp
// 等待 Update 结束
await UnityLifeTimeHelper.WaitUpdateFinish();

// 等待帧结束
await UnityLifeTimeHelper.WaitFrameFinish();

// 在 Entry.cs 中调用
UnityLifeTimeHelper.UpdateFinishTask.Dequeue().SetResult();
```

---

## 调用流程

```
Entry.Update()
  → 执行游戏逻辑
  → UnityLifeTimeHelper.UpdateFinishTask.Dequeue().SetResult()
  
Entry.LateUpdate()
  → 执行游戏逻辑
  → UnityLifeTimeHelper.LateUpdateFinishTask.Dequeue().SetResult()
```

---

## 相关文档

- [Entry.cs.md](../../Code/Entry.cs.md) - 游戏入口 (调用任务队列)
- [ETTask.cs.md](../../Module/Async/ETTask.cs.md) - 异步任务

---

*文档生成时间：2026-03-02 | OpenClaw AI 助手*
