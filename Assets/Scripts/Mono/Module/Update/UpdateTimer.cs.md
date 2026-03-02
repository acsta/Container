# UpdateTimer.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | UpdateTimer.cs |
| **路径** | Assets/Scripts/Mono/Module/Update/UpdateTimer.cs |
| **所属模块** | 框架层 → Mono/Module/Update |
| **文件职责** | 组件更新定时器，负责定时触发 IUpdate 接口的 Update 方法 |

---

## 类/结构体说明

### UpdateTimer

| 属性 | 说明 |
|------|------|
| **职责** | 定时器实现，定时调用 IUpdate 接口的 Update 方法 |
| **泛型参数** | 无 |
| **继承关系** | 继承自 `ATimer<IUpdate>` |
| **实现的接口** | 无 |

**设计模式**: 定时器模式

```csharp
// 通过 Timer 特性注册为组件更新定时器
[Timer(TimerType.ComponentUpdate)]
public class UpdateTimer: ATimer<IUpdate>
```

---

## 字段与属性

该类无额外字段与属性，继承自 ATimer<IUpdate>。

---

## 方法说明

### Run(IUpdate t)

**签名**:
```csharp
public override void Run(IUpdate t)
```

**职责**: 执行更新逻辑，调用 IUpdate 接口的 Update 方法

**核心逻辑**:
```
1. try-catch 包裹执行
2. 调用 t.Update()
3. 捕获异常并记录日志
```

**调用者**: 定时器系统（TimerManager）

**代码示例**:
```csharp
public override void Run(IUpdate t)
{
    try
    {
        t.Update();
    }
    catch (Exception ex)
    {
        Log.Error(ex);
    }
}
```

---

## Timer 特性

### [Timer(TimerType.ComponentUpdate)]

**说明**: 将 UpdateTimer 注册为组件更新定时器

**TimerType 枚举**:
```csharp
public enum TimerType
{
    ComponentUpdate = 1,  // 组件更新定时器
    // ... 其他类型
}
```

**注册机制**:
```csharp
// 框架启动时扫描所有带有 [Timer] 特性的类
// 根据 TimerType 注册到对应的定时器管理器
[Timer(TimerType.ComponentUpdate)]
public class UpdateTimer: ATimer<IUpdate>
```

---

## 使用示例

### 实现 IUpdate 接口

```csharp
public class MyComponent: IUpdate
{
    public void Update()
    {
        // 每帧执行的逻辑
        Debug.Log("Update called");
    }
}
```

### 注册到定时器

```csharp
// 框架自动通过 Timer 特性注册
// 无需手动注册
```

---

## 相关文档

- [IUpdate.cs.md](./IUpdate.cs.md) - 更新接口定义
- [TimerManager.cs.md](../../Core/Manager/TimerManager.cs.md) - 定时器管理系统
- [ATimer.cs.md](../../Core/Timer/ATimer.cs.md) - 定时器基类

---

*文档生成时间：2026-03-01 | OpenClaw AI 助手*
