# LoadingScene.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | LoadingScene.cs |
| **路径** | Assets/Scripts/Code/Module/Scene/LoadingScene.cs |
| **所属模块** | 框架层 → Code/Module/Scene |
| **文件职责** | 加载场景实现，用于场景切换时的过渡 |

---

## 类/结构体说明

### LoadingScene

| 属性 | 说明 |
|------|------|
| **职责** | 实现 IScene 接口，提供加载场景的默认实现 |
| **泛型参数** | 无 |
| **继承关系** | 实现 `IScene` |
| **实现的接口** | `IScene` |

```csharp
public class LoadingScene : IScene
{
    // 加载场景
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

**返回**: "Loading"

**实现**:
```csharp
public string GetName()
{
    return "Loading";
}
```

---

### GetScenePath()

**签名**:
```csharp
public string GetScenePath()
```

**职责**: 获取场景资源路径

**返回**: "Scenes/LoadingScene/Loading.unity"

**实现**:
```csharp
public string GetScenePath()
{
    return "Scenes/LoadingScene/Loading.unity";
}
```

---

### GetProgressPercent(out cleanup, out loadScene, out prepare)

**签名**:
```csharp
public void GetProgressPercent(out float cleanup, out float loadScene, out float prepare)
```

**职责**: 获取各阶段进度比例

**实现**:
```csharp
public void GetProgressPercent(out float cleanup, out float loadScene, out float prepare)
{
    cleanup = 0.2f;    // 清理 20%
    loadScene = 0.65f; // 加载 65%
    prepare = 0.15f;   // 准备 15%
}
```

---

### GetDontDestroyWindow()

**签名**:
```csharp
public string[] GetDontDestroyWindow()
```

**职责**: 获取不需要销毁的窗口列表

**返回**: null（无保留窗口）

**实现**:
```csharp
public string[] GetDontDestroyWindow()
{
    return null;
}
```

---

### GetScenesChangeIgnoreClean()

**签名**:
```csharp
public List<string> GetScenesChangeIgnoreClean()
```

**职责**: 获取场景切换中不释放的资源列表

**返回**: null（无不释放资源）

**实现**:
```csharp
public List<string> GetScenesChangeIgnoreClean()
{
    return null;
}
```

---

### OnCreate() / OnEnter() / OnLeave() / OnPrepare() / OnComplete() / SetProgress() / OnSwitchSceneEnd()

**签名**:
```csharp
public async ETTask OnCreate()
public async ETTask OnEnter()
public async ETTask OnLeave()
public async ETTask OnPrepare(float progressStart, float progressEnd)
public async ETTask OnComplete()
public async ETTask SetProgress(float value)
public async ETTask OnSwitchSceneEnd()
```

**职责**: 场景生命周期方法

**实现**: 全部为空实现（直接返回）

```csharp
public async ETTask OnCreate()
{
    await ETTask.CompletedTask;
}
```

---

### Dispose()

**签名**:
```csharp
public void Dispose()
```

**职责**: 销毁场景

**实现**: 空实现

```csharp
public void Dispose()
{
    
}
```

---

## 阅读指引

### 建议的阅读顺序

1. **理解场景作用** - LoadingScene 有什么用
2. **看进度配置** - GetProgressPercent
3. **看场景路径** - GetScenePath
4. **了解空实现** - 为什么生命周期方法都是空的

### 最值得学习的技术点

1. **简单实现**: 最小化实现 IScene 接口
2. **进度分配**: 清理 20%、加载 65%、准备 15%
3. **空实现**: 生命周期方法不需要特殊处理
4. **过渡场景**: 用于场景切换时的过渡

---

## 使用示例

### 示例 1: 切换到加载场景

```csharp
// 切换到加载场景（作为过渡）
await SceneManager.Instance.SwitchScene<LoadingScene>();

// 然后切换到目标场景
await SceneManager.Instance.SwitchScene<HomeScene>();
```

### 示例 2: 加载场景用途

```
场景 A → LoadingScene → 场景 B
    ↓           ↓          ↓
  清理 A    显示加载条    加载 B
```

**流程**:
1. 离开场景 A
2. 进入 LoadingScene
3. 显示加载进度条
4. 加载场景 B
5. 离开 LoadingScene
6. 进入场景 B

---

## 进度分配

| 阶段 | 比例 | 说明 |
|------|------|------|
| Cleanup | 20% | 清理旧场景资源 |
| LoadScene | 65% | 加载新场景 Unity 资源 |
| Prepare | 15% | 准备新场景资源（预加载等） |

**总进度条**:
```
0%        20%       85%      100%
│─────────│──────────│────────│
│ Cleanup │LoadScene │Prepare │
```

---

## 与其他场景的区别

| 特性 | LoadingScene | HomeScene/BattleScene |
|------|-------------|----------------------|
| 用途 | 过渡场景 | 实际游戏场景 |
| 生命周期 | 空实现 | 具体实现 |
| 资源保留 | 无 | 可能有 |
| 进度分配 | 固定 | 可自定义 |

---

## 相关文档

- [IScene.cs.md](./IScene.cs.md) - 场景接口
- [SceneManager.cs.md](./SceneManager.cs.md) - 场景管理器
- [SceneManagerProvider.cs.md](./SceneManagerProvider.cs.md) - 场景管理器提供者

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
