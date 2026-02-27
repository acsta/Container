# NormalEnvironmentRunner.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | NormalEnvironmentRunner.cs |
| **路径** | Assets/Scripts/Code/Game/System/Environment/Runner/NormalEnvironmentRunner.cs |
| **所属模块** | 玩法层 → Game/System/Environment/Runner |
| **文件职责** | 普通环境运行器，表示单一环境状态 |

---

## 类/结构体说明

### NormalEnvironmentRunner

| 属性 | 说明 |
|------|------|
| **职责** | 表示单一环境状态，无过渡逻辑 |
| **泛型参数** | 无 |
| **继承关系** | 继承自 `EnvironmentRunner` |
| **实现的接口** | 无 |

```csharp
public class NormalEnvironmentRunner : EnvironmentRunner
{
    // 普通环境运行器
}
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `Config` | `ConfigEnvironment` | `public virtual` | 环境配置 |

---

## 方法说明（按重要程度排序）

### Create(ConfigEnvironment data, EnvironmentPriorityType type, environmentManager)

**签名**:
```csharp
public static NormalEnvironmentRunner Create(ConfigEnvironment data, EnvironmentPriorityType type, 
    EnvironmentManager environmentManager)
```

**职责**: 创建普通环境运行器

**参数**:
- `data`: 环境配置
- `type`: 优先级类型
- `environmentManager`: 环境管理器

**返回**: 新的 NormalEnvironmentRunner 实例

**核心逻辑**:
```
1. 从对象池获取实例
2. 设置 Priority = (int)type
3. 保存 Config
4. 生成唯一 ID
5. 从配置创建 Data（EnvironmentInfo.Create）
6. 保存 environmentManager
7. 设置 IsOver = false
8. 返回实例
```

**调用者**: `EnvironmentManager.Create()`

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
4. 清空 Config
5. 回收到对象池
```

**调用者**: `EnvironmentManager.Remove()`, `EnvironmentManager.Process()`

---

## 阅读指引

### 建议的阅读顺序

1. **理解运行器作用** - NormalEnvironmentRunner 是什么
2. **看 Create** - 理解如何创建
3. **了解 Config** - 理解环境配置
4. **看 Dispose** - 理解资源清理

### 最值得学习的技术点

1. **简单封装**: 直接封装 ConfigEnvironment
2. **对象池**: 使用对象池管理运行器
3. **优先级**: 支持优先级栈管理
4. **无状态**: 没有额外状态，简单高效

---

## 与 BlenderEnvironmentRunner 的区别

| 特性 | NormalEnvironmentRunner | BlenderEnvironmentRunner |
|------|------------------------|-------------------------|
| 用途 | 单一环境 | 过渡环境 |
| 配置 | ConfigEnvironment | ConfigBlender |
| 数据 | 直接来自配置 | 插值计算 |
| Update | 空实现 | 计算插值 |
| 复杂度 | 低 | 高 |

---

## 使用示例

### 示例 1: 创建普通环境

```csharp
// 获取环境配置
ConfigEnvironment config = ConfigEnvironmentCategory.Instance.Get(1);

// 创建运行器
NormalEnvironmentRunner runner = NormalEnvironmentRunner.Create(
    config,
    EnvironmentPriorityType.Normal,
    EnvironmentManager.Instance
);

// 添加到管理器（通过 Create 方法）
EnvironmentManager.Instance.Create(
    configId: 1,
    priorityType: EnvironmentPriorityType.Normal
);
```

### 示例 2: 访问环境数据

```csharp
NormalEnvironmentRunner runner = GetRunner();

// 访问配置
ConfigEnvironment config = runner.Config;
Log.Info($"环境名称：{config.Name}");

// 访问数据
EnvironmentInfo data = runner.Data;
Log.Info($"色调：{data.TintColor}");
Log.Info($"光照强度：{data.LightIntensity}");
```

### 示例 3: 销毁环境

```csharp
// 通过管理器移除
EnvironmentManager.Instance.Remove(EnvironmentPriorityType.Normal);

// Dispose 内部：
// 1. 从 map 移除
// 2. 清空字段
// 3. 回收到对象池
```

---

## 环境配置（ConfigEnvironment）

| 字段 | 说明 |
|------|------|
| `Id` | 环境 ID |
| `Name` | 环境名称 |
| `TintColor` | 色调颜色 |
| `LightColor` | 光照颜色 |
| `LightIntensity` | 光照强度 |
| `LightDir` | 光照方向 |
| `UseDirLight` | 是否使用方向光 |
| `LightShadows` | 阴影类型 |
| `Enter` | 进入过渡配置 |
| `Leave` | 离开过渡配置 |

---

## 生命周期

```
创建 → 使用 → 销毁
  ↓       ↓       ↓
Create  Update  Dispose
        (空实现)
```

**流程**:
1. `Create()`: 从对象池获取，初始化
2. `Update()`: 每帧调用（空实现）
3. `Dispose()`: 回收到对象池

---

## 相关文档

- [EnvironmentRunner.cs.md](./EnvironmentRunner.cs.md) - 环境运行器基类
- [BlenderEnvironmentRunner.cs.md](./BlenderEnvironmentRunner.cs.md) - 过渡环境运行器
- [EnvironmentInfo.cs.md](../Data/EnvironmentInfo.cs.md) - 环境信息

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
