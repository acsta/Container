# EnvironmentManager.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | EnvironmentManager.cs |
| **路径** | Assets/Scripts/Code/Game/System/Environment/EnvironmentManager.cs |
| **所属模块** | 玩法层 → Game/System/Environment |
| **文件职责** | 环境管理系统，管理场景环境（光照、天空盒、雾效等）的切换和过渡 |

---

## 类/结构体说明

### EnvironmentManager

| 属性 | 说明 |
|------|------|
| **职责** | 管理环境系统的优先级栈、环境切换、光照控制 |
| **泛型参数** | 无 |
| **继承关系** | 无 |
| **实现的接口** | `IManager`, `IUpdate` |

```csharp
public partial class EnvironmentManager : IManager, IUpdate
{
    // 环境管理系统
}
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `Instance` | `EnvironmentManager` | `public static` | 单例实例 |
| `mDayTimeCount` | `int` | `private const` | 游戏世界一天总时长（1200000） |
| `mNoonTimeStart` | `int` | `private const` | 中午开始时间（0） |
| `mNightTimeStart` | `int` | `private const` | 晚上开始时间（800000） |
| `envInfoStack` | `PriorityStack<EnvironmentRunner>` | `private` | 环境优先级栈 |
| `envInfoMap` | `Dictionary<long, EnvironmentRunner>` | `private` | 环境运行器映射 |
| `curRunner` | `EnvironmentRunner` | `private` | 当前环境运行器 |
| `curInfo` | `EnvironmentInfo` | `private` | 当前环境信息 |
| `DefaultBlend` | `ConfigBlender` | `public` | 默认过渡配置 |
| `DayTimeCount` | `int` | `public` | 游戏世界一天总时长 |
| `NowTime` | `long` | `public` | 当前时间 |
| `dirLight` | `Light` | `private` | 方向光组件 |

---

## 方法说明（按重要程度排序）

### Init()

**签名**:
```csharp
public void Init()
```

**职责**: 初始化环境管理器

**核心逻辑**:
```
1. 设置 isLoad = false
2. 设置 Instance = this
3. 设置时间常量
4. 创建方向光 GameObject
5. 配置 Light 组件
6. 设置 DontDestroyOnLoad
```

**调用者**: `ManagerProvider.Init()`

---

### LoadAsync()

**签名**:
```csharp
public async ETTask LoadAsync()
```

**职责**: 异步加载环境配置和资源

**核心逻辑**:
```
1. 加载环境配置（ConfigEnvironments）
2. 保存 DefaultBlend
3. 保存默认环境 ID
4. 加载天空盒纹理（Day/Night/Sunrise/Sunset）
5. 设置全局 Shader 属性
6. 初始化 envInfoStack 和 envInfoMap
7. 创建默认环境
8. 设置 isLoad = true
```

**天空盒加载**:
```csharp
if (!string.IsNullOrEmpty(config.SkyDayTexPath))
{
    SkyDayTex = await ResourcesManager.Instance.LoadAsync<Texture>(config.SkyDayTexPath);
    Shader.SetGlobalTexture(SkyDayTexId, SkyDayTex);
}
```

**调用者**: 游戏初始化流程

---

### Update()

**签名**:
```csharp
public void Update()
```

**职责**: 每帧更新环境

**核心逻辑**:
```
1. 如果未加载，返回
2. 更新 NowTime（取模一天时长）
3. 更新所有环境运行器
4. 调用 Process() 处理环境切换
5. 如果 curRunner 不为 null，应用环境信息
```

**调用者**: `ManagerProvider.Update()`

---

### Process()

**签名**:
```csharp
private void Process()
```

**职责**: 处理环境切换逻辑

**核心逻辑**:
```
1. 获取栈顶环境（最高优先级）
2. 如果栈顶变化：
   - 如果当前是 BlenderEnvironmentRunner，切换到新目标
   - 否则创建过渡运行器
3. 如果栈顶已完成：
   - 如果是过渡完成，设置 curRunner
   - 如果目标变化，重新过渡
4. 清理已完成的环境
```

**切换逻辑**:
```csharp
var top = envInfoStack.Peek();
if (curRunner != top) // 栈顶环境变更
{
    if (curRunner is BlenderEnvironmentRunner blender)
    {
        // 正在过渡，改变目标
        envInfoStack.Remove(blender);
        blender.ChangeTo(top as NormalEnvironmentRunner, false);
        envInfoStack.Push(blender);
    }
    else
    {
        // 创建过渡运行器
        blender = CreateRunner(curRunner as NormalEnvironmentRunner,
            envInfoStack.Peek() as NormalEnvironmentRunner, true);
        envInfoStack.Push(blender);
        curRunner = blender;
    }
}
```

**调用者**: `Update()`

---

### Create(int configId, EnvironmentPriorityType priorityType, blendTime, onOver)

**签名**:
```csharp
public void Create(int configId, EnvironmentPriorityType priorityType, 
    float blendTime = -1, Action onOver = null)
```

**职责**: 创建环境

**参数**:
- `configId`: 环境配置 ID
- `priorityType`: 优先级类型
- `blendTime`: 过渡时间（-1 使用默认）
- `onOver`: 完成回调

**核心逻辑**:
```
1. 检查配置是否存在
2. 创建 NormalEnvironmentRunner
3. 设置 priority 和 onOver
4. 推入优先级栈
5. 从 envInfoMap 移除（如果存在）
6. 添加到 envInfoMap
```

**调用者**: 需要切换环境的代码

**使用示例**:
```csharp
// 切换到夜晚环境（高优先级）
EnvironmentManager.Instance.Create(
    configId: 2,
    priorityType: EnvironmentPriorityType.High
);
```

---

### Remove(EnvironmentPriorityType priorityType)

**签名**:
```csharp
public void Remove(EnvironmentPriorityType priorityType)
```

**职责**: 移除指定优先级的环境

**参数**:
- `priorityType`: 优先级类型

**核心逻辑**:
```
1. 遍历 envInfoStack
2. 找到匹配优先级的运行器
3. 从栈中移除
4. 从 map 中移除
5. 调用 Dispose
```

**调用者**: 需要恢复环境的代码

---

### ApplyEnvironmentInfo(EnvironmentInfo info)

**签名**:
```csharp
private void ApplyEnvironmentInfo(EnvironmentInfo info)
```

**职责**: 应用环境信息到场景

**核心逻辑**:
```
1. 如果 info 为 null，使用默认值
2. 设置天空盒材质
3. 设置方向光（颜色、强度、方向）
4. 设置雾效
5. 设置星星/星云速度
```

**光照设置**:
```csharp
if (dirLight != null)
{
    dirLight.enabled = info.UseDirLight;
    dirLight.color = info.LightColor;
    dirLight.intensity = info.LightIntensity;
    dirLight.transform.rotation = Quaternion.Euler(info.LightDir);
    dirLight.shadows = info.LightShadows;
}
```

**调用者**: `Update()`

---

## 阅读指引

### 建议的阅读顺序

1. **理解系统作用** - 环境管理做什么
2. **看优先级栈** - 理解环境优先级
3. **看 Process** - 理解切换逻辑
4. **了解过渡系统** - 理解 BlenderEnvironmentRunner

### 最值得学习的技术点

1. **优先级栈**: PriorityStack 管理多环境叠加
2. **平滑过渡**: BlenderEnvironmentRunner 实现环境渐变
3. **时间系统**: 游戏世界时间循环
4. **对象池**: EnvironmentInfo 使用对象池

---

## 环境优先级（EnvironmentPriorityType）

| 优先级 | 值 | 说明 |
|--------|-----|------|
| `Default` | 0 | 默认环境（最低） |
| `Low` | 1 | 低优先级 |
| `Normal` | 2 | 普通优先级 |
| `High` | 3 | 高优先级 |
| `Highest` | 4 | 最高优先级 |

**规则**: 栈顶（最高优先级）的环境生效

---

## 使用示例

### 示例 1: 切换环境

```csharp
// 切换到夜晚环境（高优先级）
EnvironmentManager.Instance.Create(
    configId: 2,  // 夜晚环境 ID
    priorityType: EnvironmentPriorityType.High
);

// 5 秒后恢复
await TimerManager.Instance.WaitAsync(5000);
EnvironmentManager.Instance.Remove(EnvironmentPriorityType.High);
```

### 示例 2: 临时环境效果

```csharp
// 进入战斗时切换到战斗环境
void EnterBattle()
{
    EnvironmentManager.Instance.Create(
        configId: 10,  // 战斗环境 ID
        priorityType: EnvironmentPriorityType.Highest,
        blendTime: 2.0f  // 2 秒过渡
    );
}

// 战斗结束恢复
void ExitBattle()
{
    EnvironmentManager.Instance.Remove(EnvironmentPriorityType.Highest);
}
```

### 示例 3: 获取当前时间

```csharp
// 获取当前游戏时间
long nowTime = EnvironmentManager.Instance.NowTime;
int dayTime = EnvironmentManager.Instance.DayTimeCount;

// 计算时间百分比
float timePercent = (float)nowTime / dayTime;

// 判断是白天还是夜晚
bool isNight = nowTime >= EnvironmentManager.Instance.NightTimeStart;
```

---

## 相关文档

- [EnvironmentInfo.cs.md](./Data/EnvironmentInfo.cs.md) - 环境信息数据结构
- [EnvironmentRunner.cs.md](./Runner/EnvironmentRunner.cs.md) - 环境运行器基类
- [BlenderEnvironmentRunner.cs.md](./Runner/BlenderEnvironmentRunner.cs.md) - 过渡环境运行器

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
