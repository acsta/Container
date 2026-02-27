# BlenderEnvironmentRunner.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | BlenderEnvironmentRunner.cs |
| **路径** | Assets/Scripts/Code/Game/System/Environment/Runner/BlenderEnvironmentRunner.cs |
| **所属模块** | 玩法层 → Game/System/Environment/Runner |
| **文件职责** | 过渡环境运行器，实现两个环境之间的平滑过渡 |

---

## 类/结构体说明

### BlenderEnvironmentRunner

| 属性 | 说明 |
|------|------|
| **职责** | 在两个环境之间进行平滑过渡（使用缓动函数） |
| **泛型参数** | 无 |
| **继承关系** | 继承自 `EnvironmentRunner` |
| **实现的接口** | 无 |

```csharp
public class BlenderEnvironmentRunner : EnvironmentRunner
{
    // 过渡环境运行器
}
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `From` | `NormalEnvironmentRunner` | `public` | 起始环境运行器 |
| `To` | `NormalEnvironmentRunner` | `public` | 目标环境运行器 |
| `formData` | `EnvironmentInfo` | `private` | 起始环境数据 |
| `toData` | `EnvironmentInfo` | `private` | 目标环境数据 |
| `startlerpTime` | `long` | `private` | 开始过渡时间 |
| `config` | `ConfigBlender` | `private` | 过渡配置 |
| `lerpFunc` | `EasingFunction.Function` | `private` | 缓动函数 |

---

## 方法说明（按重要程度排序）

### Create(NormalEnvironmentRunner from, NormalEnvironmentRunner to, isEnter, environmentManager)

**签名**:
```csharp
public static BlenderEnvironmentRunner Create(NormalEnvironmentRunner from, NormalEnvironmentRunner to, 
    bool isEnter, EnvironmentManager environmentManager)
```

**职责**: 创建过渡运行器

**参数**:
- `from`: 起始环境
- `to`: 目标环境
- `isEnter`: 是否是进入过渡（true=Enter，false=Leave）
- `environmentManager`: 环境管理器

**返回**: 新的 BlenderEnvironmentRunner 实例

**核心逻辑**:
```
1. 从对象池获取实例
2. 保存 from/to 引用
3. 保存 formData/toData
4. 深拷贝 from.Data 到 Data
5. 设置 Data.IsBlender = true
6. 设置 Priority = to.Priority
7. 选择过渡配置：
   - isEnter=true → to.Config.Enter
   - isEnter=false → from.Config.Leave
   - 如果为空，使用 DefaultBlend
8. 设置开始时间
9. 获取缓动函数
10. 返回实例
```

**调用者**: `EnvironmentManager.CreateRunner()`

---

### Update()

**签名**:
```csharp
public override void Update()
```

**职责**: 每帧更新过渡进度

**核心逻辑**:
```
1. 设置 Data.Changed = false
2. 如果 To.IsOver，设置 IsOver = true
3. 如果 IsOver，返回
4. 计算经过时间
5. 如果超过配置时间，设置 IsOver = true
6. 计算插值系数：
   - lerpVal = (time - startlerpTime) / config.DeltaTime
   - 应用缓动函数
   - 限制在 0-1 范围
7. 如果 DeltaTime=0，直接 lerpVal=1
8. 执行插值 Data.Lerp(formData, toData, lerpVal)
9. 设置 Data.Changed = true
```

**插值计算**:
```csharp
float lerpVal;
if (config.DeltaTime > 0)
{
    lerpVal = lerpFunc((float)(time - startlerpTime) / config.DeltaTime, 0, 1);
    lerpVal = Mathf.Clamp01(lerpVal);
}
else
{
    lerpVal = 1;  // 立即切换
}
Data.Lerp(formData, toData, lerpVal);
```

**调用者**: `EnvironmentManager.Update()`

---

### ChangeTo(NormalEnvironmentRunner to, bool isEnter)

**签名**:
```csharp
public void ChangeTo(NormalEnvironmentRunner to, bool isEnter)
```

**职责**: 切换过渡目标（在过渡过程中改变目标）

**参数**:
- `to`: 新目标环境
- `isEnter`: 是否是进入过渡

**核心逻辑**:
```
1. 更新 To = to
2. 设置 formData = Data（当前过渡状态作为新起点）
3. 深拷贝 Data
4. 更新 toData = to.Data
5. 选择新的过渡配置
6. 更新 Priority
7. 重置 IsOver = false
8. 获取新的缓动函数
9. 重置开始时间
```

**使用场景**:
- 过渡过程中，有更高优先级的环境插入
- 需要改变过渡目标

**调用者**: `EnvironmentManager.Process()`

---

### Dispose()

**签名**:
```csharp
public override void Dispose()
```

**职责**: 销毁并清理资源

**核心逻辑**:
```
1. 从管理器移除（RemoveFromMap）
2. 清空基类字段（Id, Priority, IsOver）
3. 释放 Data
4. 清空本类字段（From, To, formData, toData 等）
5. 回收到对象池
```

**调用者**: `EnvironmentManager.Process()`

---

## 阅读指引

### 建议的阅读顺序

1. **理解过渡作用** - 为什么需要 BlenderEnvironmentRunner
2. **看 Create** - 理解如何创建
3. **看 Update** - 理解过渡逻辑
4. **看 ChangeTo** - 理解动态切换目标

### 最值得学习的技术点

1. **缓动函数**: EasingFunction 实现平滑过渡
2. **对象池**: 使用对象池管理运行器
3. **动态切换**: ChangeTo 支持过渡中改变目标
4. **深拷贝**: DeepClone 保存过渡状态

---

## 过渡配置（ConfigBlender）

| 字段 | 说明 |
|------|------|
| `DeltaTime` | 过渡时间（毫秒） |
| `Ease` | 缓动函数类型（Linear, InQuad, OutQuad 等） |

**缓动函数类型**:
- `Linear`: 线性过渡
- `InQuad`: 加速
- `OutQuad`: 减速
- `InOutQuad`: 先加速后减速
- 等等...

---

## 过渡流程示例

```
时间线:
├─ t=0: 开始过渡 (From)
├─ t=500ms: 50% 过渡
├─ t=1000ms: 100% 过渡 (To)
└─ t>1000ms: IsOver=true

过渡曲线 (OutQuad 缓动):
100% │     ╭────
     │   ╱
 50% │ ╱
     │╱
  0% └─────────
     0   500  1000ms
```

---

## 使用示例

### 示例 1: 创建过渡

```csharp
// 创建起始和目标环境
NormalEnvironmentRunner from = NormalEnvironmentRunner.Create(
    dayConfig, EnvironmentPriorityType.Normal, envManager);
NormalEnvironmentRunner to = NormalEnvironmentRunner.Create(
    nightConfig, EnvironmentPriorityType.Normal, envManager);

// 创建过渡运行器
BlenderEnvironmentRunner blender = BlenderEnvironmentRunner.Create(
    from, to, isEnter: true, envManager);

// 添加到优先级栈
envManager.envInfoStack.Push(blender);
```

### 示例 2: 过渡中切换目标

```csharp
// 在过渡过程中，有更高优先级环境插入
NormalEnvironmentRunner newTo = NormalEnvironmentRunner.Create(
    battleConfig, EnvironmentPriorityType.High, envManager);

// 切换过渡目标
blender.ChangeTo(newTo, isEnter: true);
```

### 示例 3: 过渡进度

```csharp
// 在 Update 中
blender.Update();

// 访问过渡进度
float progress = blender.Data.Progress;  // 0-1
bool isOver = blender.IsOver;

if (isOver)
{
    Log.Info("过渡完成");
}
```

---

## 相关文档

- [EnvironmentRunner.cs.md](./EnvironmentRunner.cs.md) - 环境运行器基类
- [NormalEnvironmentRunner.cs.md](./NormalEnvironmentRunner.cs.md) - 普通环境运行器
- [EnvironmentInfo.cs.md](../Data/EnvironmentInfo.cs.md) - 环境信息

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
