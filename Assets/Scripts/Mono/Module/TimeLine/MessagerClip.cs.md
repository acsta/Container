# MessagerClip.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | MessagerClip.cs |
| **路径** | Assets/Scripts/Mono/Module/TimeLine/MessagerClip.cs |
| **所属模块** | 框架层 → Mono/Module/TimeLine |
| **文件职责** | Unity Timeline 自定义剪辑，用于在时间线上配置消息广播事件 |

---

## 类说明

### MessagerClip

| 属性 | 说明 |
|------|------|
| **职责** | Timeline 剪辑，配置消息键并创建对应的 PlayableBehaviour |
| **继承关系** | 继承自 `PlayableAsset`，实现 `ITimelineClipAsset` |
| **关联轨道** | `MessagerTrack` |
| **关联行为** | `MessagerBehaviour` |

**Unity Timeline 特性**:
```csharp
[Serializable]
public class MessagerClip : PlayableAsset, ITimelineClipAsset
```

---

## 字段与属性

### clipCaps

**签名**:
```csharp
public ClipCaps clipCaps { get { return ClipCaps.None; } }
```

**说明**: 剪辑功能限制

**ClipCaps 枚举**:
| 值 | 说明 |
|---|------|
| `None` | 无特殊功能（不支持剪辑混合、速度调整等） |
| `Looping` | 支持循环 |
| `Hold` | 支持保持最后一帧 |
| `ClipIn` | 支持剪辑内偏移 |
| `SpeedMultiplier` | 支持速度调整 |
| `Blend` | 支持混合 |
| `All` | 支持所有功能 |

**MessagerClip 选择 `None` 的原因**:
- 消息触发不需要混合
- 不需要速度调整
- 简单的开始/结束触发

---

### Key

**签名**:
```csharp
public string Key;
```

**说明**: 消息键，用于标识触发的事件

**使用示例**:
- `"IntroStart"` - 开场动画开始
- `"BossEnter"` - Boss 入场
- `"MusicChange"` - 音乐切换
- `"CameraSwitch"` - 相机切换

---

## 方法说明

### CreatePlayable(PlayableGraph graph, GameObject owner)

**签名**:
```csharp
public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
```

**职责**: 创建 PlayableBehaviour 实例，配置消息键

**核心逻辑**:
```
1. 创建 ScriptPlayable<MessagerBehaviour>
2. 获取 MessagerBehaviour 实例
3. 设置 behaviour.Key = Key
4. 返回 Playable
```

**代码实现**:
```csharp
public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
{
    var playable = ScriptPlayable<MessagerBehaviour>.Create(graph);
    var behaviour = playable.GetBehaviour();
    behaviour.Key = Key;
    return playable;
}
```

---

## Unity Timeline 集成

### 剪辑创建流程

```
Unity Timeline 编辑器
    ↓
用户添加 MessagerClip 到 MessagerTrack
    ↓
Timeline 调用 CreatePlayable()
    ↓
创建 MessagerBehaviour 实例
    ↓
设置 Key 参数
    ↓
时间线播放时触发行为
```

### 播放时序

```
时间线播放到剪辑起点
    ↓
MessagerBehaviour.OnBehaviourPlay()
    ↓
发送 ClipStartPlay 消息

时间线播放中（每帧）
    ↓
MessagerBehaviour.ProcessFrame()
    ↓
发送 ClipProcess 消息

时间线播放到剪辑终点
    ↓
MessagerBehaviour.OnBehaviourPause() / OnBehaviourStop()
```

---

## 使用示例

### 在 Timeline 中配置

1. 打开 Unity Timeline 窗口
2. 创建或选择 Timeline Asset
3. 右键添加轨道 → 选择 `MessagerTrack`
4. 在轨道上右键 → 添加 `MessagerClip`
5. 在 Inspector 中设置 Key 值

**Inspector 配置**:
```
MessagerClip
├── Key: "BossEnter"
└── (Timeline 编辑器中调整时间范围)
```

### 代码监听

```csharp
// 注册消息监听
public class CutsceneListener: MonoBehaviour
{
    void OnEnable()
    {
        Messager.Instance.Register(MessageId.ClipStartPlay, OnClipStart);
        Messager.Instance.Register(MessageId.ClipProcess, OnClipProcess);
    }
    
    void OnDisable()
    {
        Messager.Instance.Unregister(MessageId.ClipStartPlay, OnClipStart);
        Messager.Instance.Unregister(MessageId.ClipProcess, OnClipProcess);
    }
    
    void OnClipStart(object key, object time, object duration)
    {
        string keyStr = key as string;
        float timeVal = (float)time;
        float durationVal = (float)duration;
        
        switch (keyStr)
        {
            case "BossEnter":
                SpawnBoss();
                break;
            case "MusicChange":
                ChangeMusic();
                break;
        }
    }
    
    void OnClipProcess(object key, object time, object duration)
    {
        string keyStr = key as string;
        float timeVal = (float)time;
        float durationVal = (float)duration;
        float progress = timeVal / durationVal;
        
        // 根据进度更新效果
        if (keyStr == "FadeOut")
        {
            SetAlpha(1 - progress);
        }
    }
    
    void SpawnBoss() { }
    void ChangeMusic() { }
    void SetAlpha(float alpha) { }
}
```

---

## 相关文档

- [MessagerTrack.cs.md](./MessagerTrack.cs.md) - Timeline 轨道定义
- [MessagerBehaviour.cs.md](./MessagerBehaviour.cs.md) - 剪辑行为实现
- [MessageId.cs.md](../Const/MessageId.cs.md) - 消息 ID 定义
- [Messager.cs.md](../Messager/Messager.cs.md) - 消息系统

---

*文档生成时间：2026-03-01 | OpenClaw AI 助手*
