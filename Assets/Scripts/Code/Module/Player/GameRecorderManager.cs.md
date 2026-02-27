# GameRecorderManager.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | GameRecorderManager.cs |
| **路径** | Assets/Scripts/Code/Module/Player/GameRecorderManager.cs |
| **所属模块** | 框架层 → Code/Module/Player |
| **文件职责** | 游戏录制管理器，支持抖音/微信/快手小游戏录制 |

---

## 类/结构体说明

### GameRecorderManager

| 属性 | 说明 |
|------|------|
| **职责** | 管理游戏视频录制，支持抖音/微信/快手小游戏平台 |
| **泛型参数** | 无 |
| **继承关系** | 无 |
| **实现的接口** | `IManager` |

```csharp
public class GameRecorderManager : IManager
{
    // 游戏录制管理器
}
```

---

## 字段与属性

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `Instance` | `GameRecorderManager` | `public static` | 单例实例 |
| `recorder` | `TTGameRecorder/WXGameRecorder/KSGameRecorder` | `private` | 平台录制器实例 |
| `videoPath` | `string` | `private` | 录制的视频路径（抖音） |

---

## 方法说明（按重要程度排序）

### Init() / Destroy()

**签名**:
```csharp
public void Init()
public void Destroy()
```

**职责**: 初始化和销毁

**核心逻辑**:
```
// Init:
1. 设置 Instance = this
2. 检查性能等级（低性能设备不启用）
3. 根据平台获取录制器：
   - 抖音：TTSDK.TT.GetGameRecorder()
   - 微信：WeChatWASM.WX.GetGameRecorder()
   - 快手：KSWASM.KS.GetGameRecorder()

// Destroy:
1. 清空 Instance
```

**性能检查**:
```csharp
if (PerformanceManager.Instance.Level < PerformanceManager.DevicePerformanceLevel.Mid)
    return;  // 低性能设备不启用录制
```

**调用者**: `ManagerProvider.Init()`

---

### Support()

**签名**:
```csharp
public bool Support()
```

**职责**: 检查是否支持录制

**返回**: true=支持，false=不支持

**核心逻辑**:
```
1. 如果是抖音/微信/快手平台
2. 检查 recorder 是否为 null
3. 返回结果
```

**调用者**: 需要检查录制功能的代码

---

### StartRecorder(int tryCount)

**签名**:
```csharp
public async ETTask<bool> StartRecorder(int tryCount = 3)
```

**职责**: 开始录制

**参数**:
- `tryCount`: 重试次数（默认 3 次）

**返回**: true=成功，false=失败

**核心逻辑**:
```
// 抖音:
1. 检查 recorder 是否存在
2. 清空 videoPath
3. 重试录制启动
4. 调用 recorder.Start(true, 0)

// 微信/快手:
1. 检查 recorder 是否存在
2. 配置录制选项（hookBgm=true）
3. 调用 recorder.Start()
4. 等待 100ms
5. 返回 true
```

**抖音重试逻辑**:
```csharp
for (int i = 0; i < tryCount - 1; i++)
{
    if (recorder.Start(true, 0)) return true;
    await TimerManager.Instance.WaitAsync(100);
}
return recorder.Start(true, 0);
```

**调用者**: 需要开始录制的场景

---

### PauseRecorder(bool pause)

**签名**:
```csharp
public bool PauseRecorder(bool pause)
```

**职责**: 暂停/恢复录制

**参数**:
- `pause`: true=暂停，false=恢复

**返回**: true=成功，false=失败

**核心逻辑**:
```
// 抖音:
1. 设置 recorder.SetEnabled(!pause)
2. 返回 !recorder.GetEnabled()

// 微信/快手:
1. 如果 pause=true，调用 recorder.Pause()
2. 如果 pause=false，调用 recorder.Resume()
3. 返回 !pause
```

**调用者**: 需要暂停录制的场景（如切后台）

---

### StopRecorder()

**签名**:
```csharp
public async ETTask<bool> StopRecorder()
```

**职责**: 停止录制

**返回**: true=成功，false=失败

**核心逻辑**:
```
// 抖音:
1. 重试停止录制
2. 注册回调获取视频路径
3. 等待回调完成

// 微信/快手:
1. 调用 recorder.Stop()
2. 等待 100ms
3. 返回 true
```

**抖音回调**:
```csharp
ETTask<bool> res = ETTask<bool>.Create();
recorder.Stop((path) =>
{
    videoPath = path;
    res.SetResult(true);
}, (code, msg) =>
{
    Log.Error(msg);
    res.SetResult(false);
});
return await res;
```

**调用者**: 需要结束录制的场景

---

### CanShareVideo()

**签名**:
```csharp
public bool CanShareVideo()
```

**职责**: 检查是否可以分享视频

**返回**: true=可以分享，false=不可以

**核心逻辑**:
```
// 抖音:
1. 检查录制状态是否为 RECORD_COMPLETED 或 RECORD_STOPED

// 微信/快手:
1. 返回 true（总是可以）
```

**调用者**: 分享视频前检查

---

### PublishVideo()

**签名**:
```csharp
public void PublishVideo()
```

**职责**: 发布/分享视频

**核心逻辑**:
```
// 抖音:
1. 检查录制状态
2. 调用 recorder.ShareVideo()
3. 注册成功/失败/取消回调

// 微信:
1. 调用 WeChatWASM.WX.OperateGameRecorderVideo()

// 快手:
1. 调用 recorder.PublishVideo()
2. 注册回调
```

**调用者**: 分享视频按钮

---

## 阅读指引

### 建议的阅读顺序

1. **理解管理器作用** - GameRecorderManager 做什么
2. **看 StartRecorder** - 理解开始录制
3. **看 PauseRecorder/StopRecorder** - 理解录制控制
4. **了解平台差异** - 理解多平台适配

### 最值得学习的技术点

1. **平台适配**: 支持抖音/微信/快手录制 API
2. **性能检查**: 低性能设备不启用录制
3. **重试机制**: StartRecorder 支持重试
4. **异步回调**: StopRecorder 使用 ETTask 等待回调

---

## 平台差异

| 平台 | 录制器类型 | 特性 |
|------|-----------|------|
| 抖音 | TTSDK.TTGameRecorder | 支持视频路径回调 |
| 微信 | WeChatWASM.WXGameRecorder | 支持 hookBgm |
| 快手 | KSWASM.KSGameRecorder | 支持 hookBgm |

---

## 使用示例

### 示例 1: 开始录制

```csharp
// 检查是否支持
if (GameRecorderManager.Instance.Support())
{
    // 开始录制
    bool success = await GameRecorderManager.Instance.StartRecorder();
    if (success)
    {
        Log.Info("开始录制");
    }
}
```

### 示例 2: 暂停/恢复录制

```csharp
// 暂停录制（切后台时）
GameRecorderManager.Instance.PauseRecorder(pause: true);

// 恢复录制（回前台时）
GameRecorderManager.Instance.PauseRecorder(pause: false);
```

### 示例 3: 停止并分享

```csharp
// 停止录制
bool success = await GameRecorderManager.Instance.StopRecorder();
if (success)
{
    Log.Info("录制完成");
    
    // 检查是否可以分享
    if (GameRecorderManager.Instance.CanShareVideo())
    {
        // 分享视频
        GameRecorderManager.Instance.PublishVideo();
    }
}
```

### 示例 4: 完整录制流程

```csharp
// 进入拍卖场景
async ETTask OnEnterAuction()
{
    // 开始录制
    await GameRecorderManager.Instance.StartRecorder();
    
    // 进行游戏...
    await PlayGame();
    
    // 停止录制
    await GameRecorderManager.Instance.StopRecorder();
    
    // 显示分享按钮
    ShowShareButton();
}
```

---

## 相关文档

- [PerformanceManager.cs.md](../Performance/PerformanceManager.cs.md) - 性能管理器
- [AdManager.cs.md](./AdManager.cs.md) - 广告管理器

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
