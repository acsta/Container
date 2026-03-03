# 场景系统理解指南 - SceneManager

> **文档类型**: 系统理解指南  
> **适用范围**: Code/Module/Scene + Code/Game/Scene  
> **生成时间**: 2026-03-03  
> **前置知识**: Unity 场景管理、异步加载、资源管理

---

## 📑 概述

场景系统负责游戏场景的切换、加载进度管理和资源调度。

**核心职责**:
- 管理场景生命周期（创建/进入/离开/销毁）
- 控制场景异步加载流程
- 显示 Loading UI 和进度条
- 协调资源预加载和 GC
- 支持场景预加载

**关键文件**:
| 文件 | 职责 |
|------|------|
| `SceneManager.cs` | 场景管理核心 |
| `IScene.cs` | 场景接口 |
| `LoadingScene.cs` | 加载场景 |
| `SceneManagerProvider.cs` | 场景注册中心 |
| `HomeScene.cs` / `MapScene.cs` | 具体场景实现 |

---

## 🎯 系统职责

### 解决的核心问题

1. **场景切换卡顿**: 大场景加载需要时间，需要异步加载 + Loading 界面
2. **资源管理**: 场景切换时需要清理旧资源，加载新资源
3. **进度反馈**: 玩家需要看到加载进度，避免以为游戏卡死
4. **内存优化**: 及时卸载不用的资源，避免内存泄漏

### 设计思路

```
场景切换流程:
1. 显示 Loading 界面
2. 清理旧场景资源 (GC)
3. 异步加载新场景
4. 新场景初始化
5. 隐藏 Loading 界面
6. 进入新场景

关键设计:
- IScene 接口统一场景行为
- 进度条分阶段显示
- 资源清理分步骤进行
- 支持场景预加载
```

---

## 🏗️ 架构设计

### 核心类图

```
┌─────────────────────────────────────────────────────────┐
│                    SceneManager                          │
│  ┌─────────────────────────────────────────────────┐   │
│  │  CurrentScene: IScene                           │   │
│  │  当前场景实例                                    │   │
│  └─────────────────────────────────────────────────┘   │
│  ┌─────────────────────────────────────────────────┐   │
│  │  currentSceneOp: SceneHandle                    │   │
│  │  场景加载句柄                                    │   │
│  └─────────────────────────────────────────────────┘   │
│  ┌─────────────────────────────────────────────────┐   │
│  │  Busing: bool                                   │   │
│  │  是否正在加载                                    │   │
│  └─────────────────────────────────────────────────┘   │
│                                                          │
│  +SwitchScene<T>()                                      │
│  +PreloadScene<T>()                                     │
│  +GetCurrentScene<T>()                                  │
└─────────────────────────────────────────────────────────┘
                            │
                            │ 使用
                            ▼
┌─────────────────────────────────────────────────────────┐
│                      IScene                              │
│─────────────────────────────────────────────────────────│
│ +OnCreate(): ETTask                                    │
│ +OnEnter(): ETTask                                     │
│ +OnComplete(): ETTask                                  │
│ +OnLeave(): ETTask                                     │
│ +OnPrepare(): ETTask                                   │
│ +SetProgress(): ETTask                                 │
│ +GetScenePath(): string                                │
└─────────────────────────────────────────────────────────┘
                            ▲
                            │ 实现
            ┌───────────────┼───────────────┐
            │               │               │
    ┌───────────────┐ ┌───────────────┐ ┌───────────────┐
    │  HomeScene    │ │   MapScene    │ │ LoadingScene  │
    │  大厅场景     │ │   地图场景     │ │   加载场景     │
    └───────────────┘ └───────────────┘ └───────────────┘
```

---

## 🔄 核心流程

### 场景切换完整流程

```csharp
// 1. 调用切换场景
await SceneManager.Instance.SwitchScene<HomeScene>();

// 2. 内部流程
public async ETTask SwitchScene<T>(bool needClean = true) where T : class, IScene
{
    if (Busing) return;  // 防止重复切换
    if (IsInTargetScene<T>()) return;  // 已在目标场景
    
    Busing = true;
    
    // 获取不需要清理的资源列表
    var ignoreClean = CurrentScene?.GetScenesChangeIgnoreClean();
    
    // 执行切换
    await InnerSwitchScene<T>(needClean, ignoreClean: ignoreClean);
    
    // 清理 Loading 资源
    GameObjectPoolManager.GetInstance().CleanupWithPathArray(ignoreClean);
    
    Busing = false;
}

// 3. 核心切换逻辑
async ETTask InnerSwitchScene<T>(bool needClean = false, ...) where T : class, IScene
{
    float slidValue = 0;
    
    // === 阶段 1: 创建新场景 ===
    var scene = await GetScene<T>();  // 从对象池获取并初始化
    
    // 离开旧场景
    if (CurrentScene != null)
    {
        await CurrentScene.OnLeave();
        ObjectPool.Instance.Recycle(CurrentScene);
    }
    
    // === 阶段 2: 计算进度权重 ===
    scene.GetProgressPercent(out float cleanup, out float loadScene, out float prepare);
    float total = cleanup + loadScene + prepare;
    // 各阶段占比（预留 10% 缓冲）
    cleanup = cleanup / total * 0.9f;
    loadScene = loadScene / total * 0.9f;
    prepare = prepare / total * 0.9f;
    
    // === 阶段 3: 进入新场景 ===
    await scene.OnEnter();
    await scene.SetProgress(slidValue);
    
    // === 阶段 4: 等待资源管理器 ===
    while (ResourcesManager.Instance.IsProcessRunning())
    {
        await TimerManager.Instance.WaitAsync(1);
    }
    
    // === 阶段 5: 清理 UI ===
    await UIManager.Instance.DestroyWindowExceptNames(scene.GetDontDestroyWindow());
    await UIManager.Instance.DestroyAllBox();
    
    // === 阶段 6: 清理资源 ===
    ImageLoaderManager.Instance.Cleanup();
    if (needClean)
    {
        GameObjectPoolManager.GetInstance().Cleanup(true, ignoreClean);
        await PackageManager.Instance.UnloadUnusedAssets();
    }
    
    // === 阶段 7: GC ===
    GC.Collect();
    GC.Collect();  // 两次清理更干净
    Resources.UnloadUnusedAssets();
    
    // === 阶段 8: 异步加载场景 ===
    var op = await ResourcesManager.Instance.LoadSceneAsync(scene.GetScenePath(), false);
    currentSceneOp?.UnloadAsync();
    currentSceneOp = op;
    await scene.OnComplete();
    
    // === 阶段 9: 场景准备工作 ===
    await scene.OnPrepare(slidValue, slidValue + prepare);
    
    // === 阶段 10: 完成 ===
    await scene.SetProgress(1);
    await TimerManager.Instance.WaitAsync(500);  // 等待一下，避免跳太快
    
    CurrentScene = scene;
    await scene.OnSwitchSceneEnd();
    FinishLoad();  // 通知等待的任务
    
    GuidanceManager.Instance.CheckGroupStart();  // 检查引导
}
```

---

### 进度条分阶段

```
进度条总览 (0% → 100%):

0% ───────────────────────────────────────────────────── 100%
│         │              │              │               │
│  进入   │   清理资源   │   加载场景   │   准备工作    │
│  场景   │   (GC 等)    │   (异步)     │   (预加载)    │
│         │              │              │               │
0%       10%            40%            70%            100%
         │              │              │
         ▼              ▼              ▼
      cleanup        loadScene      prepare
      (9% 权重)      (27% 权重)      (27% 权重)

每个阶段调用 scene.SetProgress() 更新 UI
```

---

### 场景生命周期

```
┌─────────┐     ┌─────────┐     ┌─────────┐     ┌─────────┐
│ OnCreate│────►│ OnEnter │────►│OnComplete│────►│OnPrepare│
│ 创建场景 │     │ 进入场景 │     │ 加载完成 │     │ 准备工作 │
└─────────┘     └─────────┘     └─────────┘     └─────────┘
     │                                                       │
     │                                                       ▼
     │                                                ┌─────────┐
     │                                                │OnSwitch │
     │                                                │SceneEnd │
     │                                                │ 切换结束 │
     │                                                └─────────┘
     │                                                       │
     ▼                                                       ▼
┌─────────┐                                          ┌─────────┐
│ OnLeave │◄─────────────────────────────────────────│ 下次切换 │
│ 离开场景 │                                          └─────────┘
└─────────┘
```

---

## 💡 使用示例

### 定义场景

```csharp
// 1. 实现 IScene 接口
public class HomeScene : IScene
{
    public async ETTask OnCreate()
    {
        // 场景创建时的初始化
        Log.Info("大厅场景创建");
    }
    
    public async ETTask OnEnter()
    {
        // 进入场景（Loading 界面已显示）
        Log.Info("进入大厅场景");
    }
    
    public async ETTask OnComplete()
    {
        // 场景资源加载完成
        Log.Info("大厅场景加载完成");
    }
    
    public async ETTask OnPrepare(float startProgress, float endProgress)
    {
        // 准备工作：预加载资源等
        await ResourcesManager.Instance.PreloadAsync("home_objects");
        await SetProgress(endProgress);
    }
    
    public async ETTask OnLeave()
    {
        // 离开场景
        Log.Info("离开大厅场景");
    }
    
    public async ETTask OnSwitchSceneEnd()
    {
        // 切换完成，可以开始游戏逻辑
        Log.Info("大厅场景切换完成");
    }
    
    public string GetScenePath()
    {
        return "Scenes/HomeScene";
    }
    
    public List<string> GetDontDestroyWindow()
    {
        // 不销毁的 UI 窗口
        return new List<string> { "UITopView" };
    }
    
    public void GetProgressPercent(out float cleanup, out float loadScene, out float prepare)
    {
        // 各阶段权重（总和任意，会归一化）
        cleanup = 10;    // 清理资源占 10%
        loadScene = 30;  // 加载场景占 30%
        prepare = 30;    // 准备工作占 30%
    }
    
    public async ETTask SetProgress(float progress)
    {
        // 更新 Loading UI 进度条
        var loadingWin = UIManager.Instance.GetWindow<UILoadingView>();
        loadingWin?.SetProgress(progress);
        await TimerManager.Instance.WaitAsync(1);
    }
}
```

---

### 切换场景

```csharp
// 切换到大厅场景
await SceneManager.Instance.SwitchScene<HomeScene>();

// 切换到地图场景（带配置 ID）
await SceneManager.Instance.SwitchMapScene("Map_001");

// 切换时不清理资源（快速切换）
await SceneManager.Instance.SwitchScene<HomeScene>(needClean: false);
```

---

### 预加载场景

```csharp
// 预加载场景（后台加载）
await SceneManager.Instance.PreloadScene<HomeScene>();

// 或预加载地图
SceneManager.Instance.PreloadMapScene("Map_001");

// 预加载后切换会更快
SceneManager.Instance.PreloadScene<HomeScene>();
// ... 玩家看剧情 ...
await SceneManager.Instance.SwitchScene<HomeScene>();  // 秒切
```

---

### 获取当前场景

```csharp
// 获取当前场景
var current = SceneManager.Instance.GetCurrentScene<IScene>();

// 检查是否在目标场景
bool isInHome = SceneManager.Instance.IsInTargetScene<HomeScene>();

// 检查是否在特定地图
bool isInMap001 = SceneManager.Instance.IsInTargetMapScene("Map_001");
```

---

### 等待加载完成

```csharp
// 等待场景加载完成
await SceneManager.Instance.WaitLoadOver();

// 使用场景
var loadingTask = SceneManager.Instance.SwitchScene<HomeScene>();
await SceneManager.Instance.WaitLoadOver();
// 确保场景已完全加载
```

---

## 🔗 依赖关系

```
依赖:
├─→ ResourcesManager (资源加载)
├─→ UIManager (UI 管理)
├─→ GameObjectPoolManager (对象池)
├─→ PackageManager (资源包管理)
├─→ CameraManager (相机管理)
├─→ GuidanceManager (引导管理)
└─→ TimerManager (异步等待)

被依赖:
├─→ 所有场景类 (HomeScene/MapScene 等)
└─→ 游戏流程 (需要切换场景的地方)
```

---

## ⚠️ 注意事项

| 问题 | 说明 | 解决方案 |
|------|------|----------|
| 重复切换 | 快速点击导致多次切换 | 检查 `Busing` 标志 |
| 资源未清理 | 切换后内存泄漏 | 确保 `needClean = true` |
| Loading 闪烁 | 加载太快导致 Loading 闪现 | 最后等待 500ms |
| GC 卡顿 | 大场景 GC 导致卡顿 | 分两次 GC，交替执行 |
| 预加载冲突 | 预加载时切换其他场景 | 等待预加载完成再切换 |

---

## 🔍 性能优化

### 场景预加载

```
场景：从大厅切换到地图

不预加载:
1. 玩家点击切换
2. 显示 Loading
3. 加载地图资源 (3 秒)
4. 进入地图
总时间：3 秒 + Loading

预加载:
1. 大厅中后台预加载地图
2. 玩家点击切换
3. 显示 Loading (瞬间)
4. 进入地图
总时间：0.5 秒 (仅清理 + 切换)

优化效果：6 倍提升
```

### 资源清理策略

```csharp
// 保留某些资源不清理
public List<string> GetScenesChangeIgnoreClean()
{
    return new List<string>
    {
        "UI/UITopView",      // 顶部 UI 保留
        "UI/UIToast",        // 提示保留
        "Audio/BGM",         // 背景音乐保留
    };
}

// 切换后清理
GameObjectPoolManager.GetInstance().CleanupWithPathArray(ignoreClean);
```

---

## 📚 相关文档

| 文档 | 说明 |
|------|------|
| [SceneManager.cs.md](../Scene/SceneManager.cs.md) | SceneManager 详细文档 |
| [IScene.cs.md](../Scene/IScene.cs.md) | IScene 接口文档 |
| [ResourcesManager.cs.md](../../Mono/Module/Resource/ResourcesManager.cs.md) | 资源管理器 |
| [UIManager.cs.md](../../Mono/Module/UI/UIManager.cs.md) | UI 管理器 |

---

*文档由 OpenClaw AI 助手自动生成 | 场景系统理解指南*
