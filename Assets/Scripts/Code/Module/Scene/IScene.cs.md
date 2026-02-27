# IScene.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | IScene.cs |
| **路径** | Assets/Scripts/Code/Module/Scene/IScene.cs |
| **所属模块** | 框架层 → Code/Module/Scene |
| **文件职责** | 场景接口，定义场景生命周期和方法 |

---

## 接口说明

### IScene

| 属性 | 说明 |
|------|------|
| **职责** | 定义场景的标准接口，包括生命周期、进度管理、资源管理 |
| **泛型参数** | 无 |
| **实现类** | LoadingScene, HomeScene, BattleScene 等 |

```csharp
public interface IScene
{
    // 场景接口
}
```

---

## 方法说明（按重要程度排序）

### GetName()

**签名**:
```csharp
public string GetName()
```

**职责**: 获取场景名称

**返回**: 场景名称字符串

**使用示例**:
```csharp
public string GetName()
{
    return "Home";
}
```

**调用者**: `SceneManager`

---

### GetScenePath()

**签名**:
```csharp
public string GetScenePath()
```

**职责**: 获取场景资源路径

**返回**: 场景 Unity 资源路径

**使用示例**:
```csharp
public string GetScenePath()
{
    return "Scenes/HomeScene/Home.unity";
}
```

**调用者**: `SceneManager`

---

### GetProgressPercent(out cleanup, out loadScene, out prepare)

**签名**:
```csharp
public void GetProgressPercent(out float cleanup, out float loadScene, out float prepare)
```

**职责**: 获取各阶段进度比例

**参数**:
- `cleanup`: 清理阶段占比（0-1）
- `loadScene`: 加载场景占比（0-1）
- `prepare`: 准备阶段占比（0-1）

**要求**: 三个值之和应为 1.0

**使用示例**:
```csharp
public void GetProgressPercent(out float cleanup, out float loadScene, out float prepare)
{
    cleanup = 0.2f;    // 清理 20%
    loadScene = 0.65f; // 加载 65%
    prepare = 0.15f;   // 准备 15%
}
```

**调用者**: `SceneManager`（进度条显示）

---

### GetDontDestroyWindow()

**签名**:
```csharp
public string[] GetDontDestroyWindow()
```

**职责**: 获取不需要销毁的窗口列表

**返回**: 窗口预置体路径数组

**用途**: 场景切换时保留这些窗口（如 UI 提示框）

**使用示例**:
```csharp
public string[] GetDontDestroyWindow()
{
    return new[]
    {
        "UI/Windows/UIMsgBoxWin",
        "UI/Windows/UIToastWin"
    };
}
```

**调用者**: `SceneManager`

---

### GetScenesChangeIgnoreClean()

**签名**:
```csharp
public List<string> GetScenesChangeIgnoreClean()
```

**职责**: 获取场景切换中不释放的资源列表

**返回**: 资源路径列表

**用途**: 场景切换时保留这些资源（如共享资源）

**使用示例**:
```csharp
public List<string> GetScenesChangeIgnoreClean()
{
    return new List<string>
    {
        "Resources/Shared/PlayerModel",
        "Resources/Shared/CommonEffects"
    };
}
```

**调用者**: `SceneManager`

---

### OnCreate()

**签名**:
```csharp
public ETTask OnCreate()
```

**职责**: 场景创建时初始化

**用途**: 初始化需要全局保存的状态

**使用示例**:
```csharp
public async ETTask OnCreate()
{
    // 初始化场景数据
    InitSceneData();
    
    // 注册事件
    RegisterEvents();
    
    await ETTask.CompletedTask;
}
```

**调用者**: `SceneManager`

---

### OnEnter()

**签名**:
```csharp
public ETTask OnEnter()
```

**职责**: 进入场景前初始化

**用途**: 加载前的准备工作

**使用示例**:
```csharp
public async ETTask OnEnter()
{
    // 设置相机位置
    SetupCamera();
    
    // 加载必要资源
    await LoadRequiredResources();
    
    await ETTask.CompletedTask;
}
```

**调用者**: `SceneManager`

---

### SetProgress(float value)

**签名**:
```csharp
public ETTask SetProgress(float value)
```

**职责**: 设置加载进度

**参数**:
- `value`: 进度值（0-1）

**用途**: 更新加载进度条显示

**使用示例**:
```csharp
public async ETTask SetProgress(float value)
{
    // 更新 UI 进度条
    UIManager.Instance.GetView<UILoadingView>()?.SetProgress(value);
    
    await ETTask.CompletedTask;
}
```

**调用者**: `SceneManager`

---

### OnPrepare(float progressStart, float progressEnd)

**签名**:
```csharp
public ETTask OnPrepare(float progressStart, float progressEnd)
```

**职责**: 场景加载结束后的资源准备

**参数**:
- `progressStart`: 准备阶段起始进度
- `progressEnd`: 准备阶段结束进度

**用途**: 预加载资源、初始化实体等

**使用示例**:
```csharp
public async ETTask OnPrepare(float progressStart, float progressEnd)
{
    // 预加载角色模型
    await ResourcesManager.Instance.LoadAsync<GameObject>("Player");
    
    // 预加载特效
    await ResourcesManager.Instance.LoadAsync<GameObject>("Effects/Combo");
    
    // 更新进度
    float progress = progressStart + (progressEnd - progressStart) * 0.5f;
    await SetProgress(progress);
    
    await ETTask.CompletedTask;
}
```

**调用者**: `SceneManager`

---

### OnComplete()

**签名**:
```csharp
public ETTask OnComplete()
```

**职责**: 场景加载完毕

**用途**: 加载完成后的收尾工作

**使用示例**:
```csharp
public async ETTask OnComplete()
{
    // 显示场景
    ShowScene();
    
    // 播放入场动画
    await PlayEnterAnimation();
    
    await ETTask.CompletedTask;
}
```

**调用者**: `SceneManager`

---

### OnLeave()

**签名**:
```csharp
public ETTask OnLeave()
```

**职责**: 离开场景时清理资源

**用途**: 清理场景资源、注销事件等

**使用示例**:
```csharp
public async ETTask OnLeave()
{
    // 清理场景数据
    CleanupSceneData();
    
    // 注销事件
    UnregisterEvents();
    
    await ETTask.CompletedTask;
}
```

**调用者**: `SceneManager`

---

### OnSwitchSceneEnd()

**签名**:
```csharp
public ETTask OnSwitchSceneEnd()
```

**职责**: 转场景结束

**用途**: 场景切换完成后的处理

**使用示例**:
```csharp
public async ETTask OnSwitchSceneEnd()
{
    // 清理旧场景
    CleanupOldScene();
    
    await ETTask.CompletedTask;
}
```

**调用者**: `SceneManager`

---

### Dispose()

**签名**:
```csharp
public void Dispose()
```

**职责**: 销毁场景

**用途**: 释放所有资源

**使用示例**:
```csharp
public void Dispose()
{
    // 释放资源
    Resources.Clear();
    
    // 清空列表
    EntityList.Clear();
}
```

**调用者**: `SceneManager`

---

## 阅读指引

### 建议的阅读顺序

1. **理解接口作用** - IScene 定义什么
2. **看生命周期方法** - OnCreate/OnEnter/OnLeave 等
3. **看进度管理** - GetProgressPercent/SetProgress
4. **看资源管理** - GetDontDestroyWindow/GetScenesChangeIgnoreClean

### 最值得学习的技术点

1. **生命周期管理**: 完整的场景生命周期
2. **进度管理**: 分阶段进度控制
3. **资源管理**: 保留/释放资源控制
4. **异步加载**: ETTask 异步方法

---

## 场景生命周期

```
创建 → OnCreate()
  ↓
进入 → OnEnter()
  ↓
加载场景
  ↓
准备 → OnPrepare()
  ↓
完成 → OnComplete()
  ↓
使用中...
  ↓
离开 → OnLeave()
  ↓
切换结束 → OnSwitchSceneEnd()
  ↓
销毁 → Dispose()
```

---

## 使用示例

### 示例 1: 实现场景接口

```csharp
public class HomeScene : IScene
{
    public string GetName()
    {
        return "Home";
    }
    
    public string GetScenePath()
    {
        return "Scenes/HomeScene/Home.unity";
    }
    
    public void GetProgressPercent(out float cleanup, out float loadScene, out float prepare)
    {
        cleanup = 0.2f;
        loadScene = 0.65f;
        prepare = 0.15f;
    }
    
    public async ETTask OnCreate()
    {
        Log.Info("创建家园场景");
        await ETTask.CompletedTask;
    }
    
    public async ETTask OnEnter()
    {
        Log.Info("进入家园场景");
        await ETTask.CompletedTask;
    }
    
    public async ETTask OnComplete()
    {
        Log.Info("家园场景加载完成");
        await ETTask.CompletedTask;
    }
    
    public void Dispose()
    {
        Log.Info("销毁家园场景");
    }
    
    // 其他方法实现...
}
```

### 示例 2: 切换场景

```csharp
// 切换到家园场景
await SceneManager.Instance.SwitchScene<HomeScene>();

// 切换到战斗场景
await SceneManager.Instance.SwitchScene<BattleScene>();
```

---

## 相关文档

- [SceneManager.cs.md](./SceneManager.cs.md) - 场景管理器
- [LoadingScene.cs.md](./LoadingScene.cs.md) - 加载场景
- [SceneManagerProvider.cs.md](./SceneManagerProvider.cs.md) - 场景管理器提供者

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
