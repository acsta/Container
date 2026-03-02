# IUpdate.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | IUpdate.cs |
| **路径** | Assets/Scripts/Mono/Module/Update/IUpdate.cs |
| **所属模块** | 框架层 → Mono/Module/Update |
| **文件职责** | 定义 Unity 生命周期更新接口（Update/FixedUpdate/LateUpdate） |

---

## 接口说明

### IUpdate

| 属性 | 说明 |
|------|------|
| **职责** | 定义每帧更新方法 |
| **调用时机** | Unity Update 生命周期 |
| **方法** | `void Update()` |

**使用示例**:
```csharp
public class MyComponent: IUpdate
{
    public void Update()
    {
        // 每帧执行的逻辑
    }
}
```

---

### IFixedUpdate

| 属性 | 说明 |
|------|------|
| **职责** | 定义物理更新方法 |
| **调用时机** | Unity FixedUpdate 生命周期（物理帧） |
| **方法** | `void FixedUpdate()` |

**使用示例**:
```csharp
public class PhysicsComponent: IFixedUpdate
{
    public void FixedUpdate()
    {
        // 物理相关逻辑，如力的应用
    }
}
```

---

### ILateUpdate

| 属性 | 说明 |
|------|------|
| **职责** | 定义延迟更新方法 |
| **调用时机** | Unity LateUpdate 生命周期（所有 Update 完成后） |
| **方法** | `void LateUpdate()` |

**使用示例**:
```csharp
public class CameraFollow: ILateUpdate
{
    public void LateUpdate()
    {
        // 相机跟随逻辑（在所有 Update 之后执行）
    }
}
```

---

## 接口对比

| 接口 | 调用时机 | 用途 |
|------|----------|------|
| `IUpdate` | 每帧 | 常规逻辑更新 |
| `IFixedUpdate` | 物理帧（固定时间间隔） | 物理相关逻辑 |
| `ILateUpdate` | 所有 Update 完成后 | 相机跟随、依赖其他对象更新后的逻辑 |

---

## 与定时器系统集成

### UpdateTimer 调用 IUpdate

```csharp
[Timer(TimerType.ComponentUpdate)]
public class UpdateTimer: ATimer<IUpdate>
{
    public override void Run(IUpdate t)
    {
        try
        {
            t.Update();  // 调用 IUpdate 接口的 Update 方法
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
    }
}
```

### 注册与触发

```csharp
// 1. 组件实现 IUpdate 接口
public class MyComponent: IUpdate
{
    public void Update()
    {
        // ...
    }
}

// 2. 框架通过定时器系统自动触发
// TimerManager 会扫描所有 IUpdate 实现类
// 并在每帧调用 UpdateTimer.Run()
```

---

## Unity 生命周期对比

### 原生 MonoBehaviour

```csharp
public class MonoBehaviorExample: MonoBehaviour
{
    void Update() { }        // 每帧
    void FixedUpdate() { }   // 物理帧
    void LateUpdate() { }    // Update 后
}
```

### 框架接口方式

```csharp
public class FrameworkExample: IUpdate, IFixedUpdate, ILateUpdate
{
    public void Update() { }        // 每帧
    public void FixedUpdate() { }   // 物理帧
    public void LateUpdate() { }    // Update 后
}
```

**优势**:
- 解耦 MonoBehaviour
- 支持纯 C# 类使用
- 便于单元测试
- 与框架定时器系统集成

---

## 使用示例

### 实现多个更新接口

```csharp
public class PlayerController: IUpdate, IFixedUpdate, ILateUpdate
{
    private Vector3 velocity;
    
    public void Update()
    {
        // 输入处理、状态更新
        HandleInput();
        UpdateState();
    }
    
    public void FixedUpdate()
    {
        // 物理移动
        ApplyPhysics();
    }
    
    public void LateUpdate()
    {
        // 相机跟随、动画更新
        UpdateCamera();
        UpdateAnimation();
    }
    
    private void HandleInput() { }
    private void UpdateState() { }
    private void ApplyPhysics() { }
    private void UpdateCamera() { }
    private void UpdateAnimation() { }
}
```

### 仅实现 IUpdate

```csharp
public class Rotator: IUpdate
{
    public Transform target;
    public float speed = 90f;
    
    public void Update()
    {
        target.Rotate(Vector3.up, speed * Time.deltaTime);
    }
}
```

---

## 相关文档

- [UpdateTimer.cs.md](./UpdateTimer.cs.md) - 更新定时器实现
- [TimerManager.cs.md](../../Core/Manager/TimerManager.cs.md) - 定时器管理系统
- [ATimer.cs.md](../../Core/Timer/ATimer.cs.md) - 定时器基类

---

*文档生成时间：2026-03-01 | OpenClaw AI 助手*
