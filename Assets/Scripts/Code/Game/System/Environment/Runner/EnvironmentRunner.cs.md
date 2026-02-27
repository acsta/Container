# EnvironmentRunner.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | EnvironmentRunner.cs |
| **路径** | Assets/Scripts/Code/Game/System/Environment/Runner/EnvironmentRunner.cs |
| **所属模块** | 玩法层 → Game/System/Environment/Runner |
| **文件职责** | 环境运行器基类，定义环境运行器的基本接口 |

---

## 类/结构体说明

### EnvironmentRunner

| 属性 | 说明 |
|------|------|
| **职责** | 环境运行器抽象基类，定义优先级、ID、更新等接口 |
| **泛型参数** | 无 |
| **继承关系** | 无（抽象类） |
| **实现的接口** | `IPriorityStackItem`, `IDisposable` |

```csharp
public abstract class EnvironmentRunner : IPriorityStackItem, IDisposable
{
    // 环境运行器基类
}
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `Priority` | `int` | `public` | 优先级（用于优先级栈排序） |
| `Id` | `long` | `public` | 唯一标识 ID |
| `IsOver` | `bool` | `public` | 是否已完成 |
| `Data` | `EnvironmentInfo` | `public` | 环境数据 |
| `environmentManager` | `EnvironmentManager` | `protected` | 环境管理器引用 |

---

## 方法说明（按重要程度排序）

### Update()

**签名**:
```csharp
public virtual void Update()
```

**职责**: 每帧更新环境

**核心逻辑**:
```
1. 默认空实现
2. 子类重写实现具体更新逻辑
```

**调用者**: `EnvironmentManager.Update()`

---

### Dispose()

**签名**:
```csharp
public abstract void Dispose()
```

**职责**: 销毁并清理资源

**要求**: 子类必须实现

**核心逻辑**:
```
1. 从管理器移除
2. 清空字段
3. 回收到对象池
```

**调用者**: `EnvironmentManager.Remove()`, `EnvironmentManager.Process()`

---

## 阅读指引

### 建议的阅读顺序

1. **理解基类作用** - 为什么需要 EnvironmentRunner
2. **看优先级** - 理解 IPriorityStackItem
3. **看 Update** - 理解更新机制
4. **了解 Dispose** - 理解资源清理

### 最值得学习的技术点

1. **抽象基类**: 定义统一接口
2. **优先级栈**: IPriorityStackItem 支持优先级排序
3. **对象池**: 子类使用对象池管理
4. **多态**: 不同子类实现不同环境行为

---

## 子类

| 子类 | 说明 |
|------|------|
| `NormalEnvironmentRunner` | 普通环境运行器（单一环境） |
| `BlenderEnvironmentRunner` | 过渡环境运行器（两个环境之间过渡） |
| `DayEnvironmentRunner` | 昼夜环境运行器（自动昼夜循环） |

---

## 使用示例

### 示例 1: 创建普通环境运行器

```csharp
// 创建配置
ConfigEnvironment config = ConfigEnvironmentCategory.Instance.Get(1);

// 创建运行器
NormalEnvironmentRunner runner = NormalEnvironmentRunner.Create(
    config,
    EnvironmentPriorityType.Normal,
    EnvironmentManager.Instance
);

// 添加到管理器
EnvironmentManager.Instance.envInfoStack.Push(runner);
EnvironmentManager.Instance.envInfoMap[runner.Id] = runner;
```

### 示例 2: 创建过渡环境运行器

```csharp
// 创建过渡运行器
BlenderEnvironmentRunner blender = BlenderEnvironmentRunner.Create(
    fromRunner,  // 起始环境
    toRunner,    // 目标环境
    isEnter: true,
    EnvironmentManager.Instance
);

// 添加到优先级栈
EnvironmentManager.Instance.envInfoStack.Push(blender);
```

### 示例 3: 更新运行器

```csharp
// 在 EnvironmentManager.Update() 中
foreach (var item in envInfoStack.Data)
{
    for (int i = 0; i < item.Value.Count; i++)
    {
        item.Value[i]?.Update();  // 调用 Update
    }
}
```

### 示例 4: 销毁运行器

```csharp
// 销毁并回收
runner.Dispose();

// Dispose 内部：
// 1. 从 map 移除
// 2. 清空字段
// 3. 回收到对象池
```

---

## 优先级栈（IPriorityStackItem）

EnvironmentRunner 实现了 `IPriorityStackItem` 接口，支持优先级栈管理：

```csharp
public interface IPriorityStackItem
{
    int Priority { get; }
}
```

**规则**:
- 优先级高的在栈顶
- 栈顶的环境生效
- 支持多环境叠加

---

## 相关文档

- [EnvironmentManager.cs.md](../EnvironmentManager.cs.md) - 环境管理器
- [NormalEnvironmentRunner.cs.md](./NormalEnvironmentRunner.cs.md) - 普通环境运行器
- [BlenderEnvironmentRunner.cs.md](./BlenderEnvironmentRunner.cs.md) - 过渡环境运行器

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
