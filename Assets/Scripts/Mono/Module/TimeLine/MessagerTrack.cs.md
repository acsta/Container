# MessagerTrack.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | MessagerTrack.cs |
| **路径** | Assets/Scripts/Mono/Module/TimeLine/MessagerTrack.cs |
| **所属模块** | 框架层 → Mono/Module/TimeLine |
| **文件职责** | Unity Timeline 自定义轨道，用于在时间线上触发消息广播事件 |

---

## 类说明

### MessagerTrack

| 属性 | 说明 |
|------|------|
| **职责** | Timeline 轨道，承载 MessagerClip 剪辑 |
| **继承关系** | 继承自 `TrackAsset` |
| **关联剪辑** | `MessagerClip` |
| **轨道颜色** | RGB(0.875, 0.594, 0.174) - 橙色 |

**Unity Timeline 特性**:
```csharp
[TrackColor(0.875f, 0.5944853f, 0.1737132f)]  // 轨道颜色
[TrackClipType(typeof(MessagerClip))]          // 支持的剪辑类型
public class MessagerTrack: TrackAsset
```

---

## 字段与属性

该类无额外字段与属性，继承自 TrackAsset。

---

## Unity Timeline 集成

### 轨道颜色

```csharp
[TrackColor(0.875f, 0.5944853f, 0.1737132f)]
```

**RGB 值**:
- R: 0.875 (223/255)
- G: 0.594 (151/255)
- B: 0.174 (44/255)

显示为橙色，便于在时间线上识别。

### 剪辑类型限制

```csharp
[TrackClipType(typeof(MessagerClip))]
```

限制该轨道只能添加 `MessagerClip` 类型的剪辑。

---

## 使用场景

### Timeline 中配置

1. 在 Unity 中创建 Timeline Asset
2. 添加 MessagerTrack 轨道
3. 在轨道上创建 MessagerClip
4. 设置 Clip 的 Key 参数（消息键）
5. 调整剪辑的起始时间和持续时间

### 运行时行为

```
时间线播放
    ↓
MessagerClip 开始播放
    ↓
MessagerBehaviour.OnBehaviourPlay()
    ↓
Messager.Instance.Broadcast() 发送 ClipStartPlay 消息
    ↓
MessagerBehaviour.ProcessFrame() 每帧调用
    ↓
Messager.Instance.Broadcast() 发送 ClipProcess 消息
```

---

## 消息广播

### 消息类型

| 消息 ID | 说明 | 触发时机 |
|--------|------|----------|
| `MessageId.ClipStartPlay` | 剪辑开始播放 | OnBehaviourPlay |
| `MessageId.ClipProcess` | 剪辑播放中（每帧） | ProcessFrame |

### 广播参数

```csharp
Messager.Instance?.Broadcast(0, MessageId.ClipStartPlay, Key, playable.GetTime(), playable.GetDuration());
```

**参数说明**:
- `0`: 消息类型/优先级
- `MessageId.ClipStartPlay/ClipProcess`: 消息 ID
- `Key`: 消息键（由 Clip 配置）
- `playable.GetTime()`: 当前播放时间
- `playable.GetDuration()`: 剪辑总时长

---

## 使用示例

### 在 Timeline 中配置

```
Timeline Asset: Cutscene
├── MessagerTrack (橙色)
│   ├── MessagerClip: "IntroStart" (0s - 2s)
│   └── MessagerClip: "IntroEnd" (2s - 4s)
└── AnimationTrack
    └── AnimationClip: "CharacterWalk"
```

### 监听消息

```csharp
// 监听剪辑开始播放
Messager.Instance.Register(MessageId.ClipStartPlay, (key, time, duration) =>
{
    if (key == "IntroStart")
    {
        // 播放开场动画
        PlayIntroAnimation();
    }
});

// 监听剪辑播放进度
Messager.Instance.Register(MessageId.ClipProcess, (key, time, duration) =>
{
    float progress = time / duration;
    // 根据进度更新 UI 或效果
    UpdateProgressBar(progress);
});
```

---

## 相关文档

- [MessagerClip.cs.md](./MessagerClip.cs.md) - Timeline 剪辑定义
- [MessagerBehaviour.cs.md](./MessagerBehaviour.cs.md) - 剪辑行为实现
- [MessageId.cs.md](../Const/MessageId.cs.md) - 消息 ID 定义
- [Messager.cs.md](../Messager/Messager.cs.md) - 消息系统

---

*文档生成时间：2026-03-01 | OpenClaw AI 助手*
