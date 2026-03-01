# MessageId.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | MessageId.cs |
| **路径** | Assets/Scripts/Mono/Module/Const/MessageId.cs |
| **所属模块** | 框架层 → Mono/Module/Const |
| **文件职责** | 定义全局消息 ID 常量，用于事件系统和消息分发 |

---

## 类说明

### MessageId

| 属性 | 说明 |
|------|------|
| **职责** | 提供全局唯一的消息 ID 常量，用于事件订阅和分发 |
| **类型** | `static class` |
| **命名空间** | `TaoTie` |

**设计模式**: 常量集中管理

```csharp
// 使用方式
// 订阅事件：Messager.Instance.AddListener(MessageId.ChangeMoney, callback);
// 发送事件：Messager.Instance.Dispatch(MessageId.ChangeMoney, data);
```

---

## 消息 ID 常量（按功能分组）

### 🎮 调试/开发工具消息（负数 ID）

| ID | 常量名 | 说明 |
|----|--------|------|
| `-6` | `SetProfit` | 增加餐厅收益时间 |
| `-5` | `EnterGuideScene` | 进入新手引导场景 |
| `-4` | `UnlockAllCloth` | 解锁所有衣服（调试用） |
| `-3` | `OpenTurntable` | 打开大转盘 |
| `-2` | `AddMoney` | 加钱（调试用） |
| `-1` | `TimeScaleChange` | 游戏时间缩放改变 |

---

### 📊 基础事件消息（正数 ID 1-10）

| ID | 常量名 | 说明 |
|----|--------|------|
| `1` | `NumericChangeEvt` | 数值变化事件 |
| `2` | `EnterWayChange` | 进入方式变化 |
| `3` | `SidebarRewards` | 侧边栏奖励更新 |
| `4` | `ChangePositionEvt` | 坐标变化事件 |
| `5` | `ChangeRotationEvt` | 方向变化事件 |
| `6` | `ChangeScaleEvt` | 缩放变化事件 |
| `7` | `RefreshAuctionState` | 刷新拍卖状态 |
| `8` | `ChangeMoney` | 金币刷新 |
| `9` | `SetChangeItemResult` | 鉴定结果 |
| `10` | `UnlockTreeNode` | 解锁科技树节点 |

---

### 📋 任务/进度消息（ID 11-20）

| ID | 常量名 | 说明 |
|----|--------|------|
| `11` | `UpdateTaskStep` | 更新任务进度 |
| `12` | `SetChangePriceResult` | 检疫结果 |
| `13` | `AssistantTalk` | 小助理讲话 |
| `14` | `GuidanceTalk` | 引导对话（小助理讲话） |
| `15` | `ComplexTask` | 复杂任务更新 |
| `16` | `ChangeItem` | 改变物体数量 |
| `17` | `GuideBox` | 引导物体高亮 |
| `18` | `GuideBox2` | 新手引导物体高亮（第二种） |
| `19` | `OnKeyInput` | 按键状态改变 |
| `20` | `ClipStartPlay` | 开始播放（时间线/动画） |

---

### 🎬 播放/展示消息（ID 21+）

| ID | 常量名 | 说明 |
|----|--------|------|
| `21` | `ClipProcess` | 正在播放的每一帧（时间线/动画进度） |
| `22` | `ShowTextRange` | 展示估价（价格范围文本） |

---

## 使用示例

### 示例 1: 订阅金币变化事件

```csharp
// 在 UI 或逻辑类中订阅
void OnEnable()
{
    Messager.Instance.AddListener(MessageId.ChangeMoney, OnMoneyChanged);
}

void OnDisable()
{
    Messager.Instance.RemoveListener(MessageId.ChangeMoney, OnMoneyChanged);
}

void OnMoneyChanged(object data)
{
    int newMoney = (int)data;
    UpdateMoneyUI(newMoney);
}
```

### 示例 2: 发送金币变化事件

```csharp
// 当金币数量变化时
int currentMoney = 1000;
Messager.Instance.Dispatch(MessageId.ChangeMoney, currentMoney);
```

### 示例 3: 订阅数值变化事件

```csharp
// 订阅通用数值变化
Messager.Instance.AddListener(MessageId.NumericChangeEvt, OnNumericChange);

void OnNumericChange(object data)
{
    // data 可能包含：数值类型、变化量、最终值等信息
    NumericChangeEvent evt = data as NumericChangeEvent;
    if (evt != null)
    {
        UpdateNumericDisplay(evt.Type, evt.Value);
    }
}
```

### 示例 4: 触发鉴定结果

```csharp
// 鉴定完成后通知 UI
鉴定结果 result = new 鉴定结果
{
    ItemId = itemId,
    Quality = quality,
    Price = price
};
Messager.Instance.Dispatch(MessageId.SetChangeItemResult, result);
```

### 示例 5: 触发引导对话

```csharp
// 小助理说话
string talkContent = "欢迎来到游戏！让我来教你怎么玩~";
Messager.Instance.Dispatch(MessageId.AssistantTalk, talkContent);

// 或者引导对话
Messager.Instance.Dispatch(MessageId.GuidanceTalk, talkContent);
```

### 示例 6: 更新任务进度

```csharp
// 任务步骤更新
TaskStepUpdate step = new TaskStepUpdate
{
    TaskId = taskId,
    CurrentStep = 2,
    TotalSteps = 5
};
Messager.Instance.Dispatch(MessageId.UpdateTaskStep, step);
```

### 示例 7: 引导物体高亮

```csharp
// 高亮引导物体
GuideBoxData guideData = new GuideBoxData
{
    TargetObject = targetGameObject,
    HighlightColor = Color.yellow,
    Duration = 3f
};
Messager.Instance.Dispatch(MessageId.GuideBox, guideData);
```

---

## 消息数据结构参考

### NumericChangeEvent（数值变化）
```csharp
public class NumericChangeEvent
{
    public int Type;      // 数值类型
    public float Value;   // 变化值
    public float Final;   // 最终值
}
```

### TaskStepUpdate（任务进度）
```csharp
public class TaskStepUpdate
{
    public long TaskId;     // 任务 ID
    public int CurrentStep; // 当前步骤
    public int TotalSteps;  // 总步骤
}
```

### GuideBoxData（引导物体）
```csharp
public class GuideBoxData
{
    public GameObject TargetObject;  // 目标物体
    public Color HighlightColor;     // 高亮颜色
    public float Duration;           // 持续时间
}
```

---

## 消息分发流程

```
发送方                          消息中心                          接收方
  │                              │                                  │
  │── Dispatch(MessageId, data) ─▶│                                  │
  │                              │── 查找订阅者 ─────────────────────▶│
  │                              │                                  │
  │                              │── 调用回调 (data) ────────────────▶│
  │                              │                                  │
  │                              │                                  │── 处理逻辑
```

---

## 相关文档

- **消息系统**: [Messager.cs.md](../../Messager/Messager.cs.md) - 消息中心实现
- **UI 生命周期**: [IOnCreate.cs.md](../../../Code/Module/UI/IOnCreate.cs.md) - UI 创建时订阅
- **引导系统**: Guidance 模块相关文档

---

## 注意事项

### ⚠️ 负数 ID

负数 ID（-1 到 -6）保留给调试和开发工具使用，生产环境应谨慎使用。

### ⚠️ 内存泄漏

订阅事件后务必在适当时机取消订阅：
```csharp
// 正确做法
void OnEnable() => Messager.Instance.AddListener(id, callback);
void OnDisable() => Messager.Instance.RemoveListener(id, callback);

// 错误做法 - 会导致内存泄漏
void Start() => Messager.Instance.AddListener(id, callback);
// 没有 RemoveListener
```

### ⚠️ 数据类型

Dispatch 传递的 data 参数类型需要发送方和接收方约定一致：
```csharp
// 发送
Messager.Instance.Dispatch(MessageId.ChangeMoney, 1000);  // int

// 接收
void OnMoneyChanged(object data)
{
    int money = (int)data;  // 必须知道是 int
}
```

### ⚠️ ID 分配

新增消息 ID 时：
- 在现有最大 ID 后递增
- 避免重复
- 在文档中更新说明

---

## 扩展建议

当前消息 ID 已使用到 22，建议预留空间：
- 23-50: UI 相关消息
- 51-100: 游戏逻辑消息
- 101-200: 网络消息
- 201+: 扩展保留

---

*文档生成时间：2026-03-01 | OpenClaw AI 助手*
