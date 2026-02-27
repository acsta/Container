# UpdateProcess.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | UpdateProcess.cs |
| **路径** | Assets/Scripts/Code/Module/Update/UpdateProcess/UpdateProcess.cs |
| **所属模块** | 框架层 → Code/Module/Update/UpdateProcess |
| **文件职责** | 更新流程抽象基类 |

---

## 类/结构体说明

### UpdateProcess

| 属性 | 说明 |
|------|------|
| **职责** | 定义更新流程的统一接口 |
| **泛型参数** | 无 |
| **继承关系** | 无（抽象类） |
| **实现的接口** | 无 |

```csharp
public abstract class UpdateProcess
{
    // 更新流程基类
}
```

---

## 方法说明

### Process(UpdateTask task)

**签名**:
```csharp
public abstract ETTask<UpdateRes> Process(UpdateTask task)
```

**职责**: 执行更新流程

**参数**:
- `task`: 更新任务实例

**返回**: 更新结果 UpdateRes

**要求**: 子类必须实现

**返回结果**:
- `UpdateRes.Over`: 流程完成，继续下一个
- `UpdateRes.Fail`: 流程失败，终止更新
- `UpdateRes.Quit`: 退出游戏
- `UpdateRes.Restart`: 需要重启游戏

**调用者**: `UpdateTask.Process()`

---

## 阅读指引

### 建议的阅读顺序

1. **理解基类作用** - 为什么需要 UpdateProcess
2. **看 Process** - 理解接口定义
3. **了解子类** - 查看具体实现

### 最值得学习的技术点

1. **抽象基类**: 定义统一接口
2. **多态**: 不同子类实现不同流程
3. **链式执行**: 多个流程顺序执行

---

## 子类列表

| 子类 | 说明 |
|------|------|
| `SetWhiteListProcess` | 设置白名单 |
| `SetUpdateListProcess` | 获取更新列表 |
| `UpdateIsSHProcess` | 检查提审模式 |
| `AppUpdateProcess` | APP 版本更新 |
| `MainPackageUpdateProcess` | 主资源包更新 |
| `OtherPackageUpdateProcess` | 分包资源更新 |

---

## 使用示例

### 示例 1: 实现自定义更新流程

```csharp
public class CustomUpdateProcess : UpdateProcess
{
    public override async ETTask<UpdateRes> Process(UpdateTask task)
    {
        Log.Info("执行自定义更新流程");
        
        // 执行自定义逻辑
        await DoCustomWork();
        
        // 返回结果
        return UpdateRes.Over;
    }
    
    private async ETTask DoCustomWork()
    {
        // 自定义工作
        await ETTask.CompletedTask;
    }
}
```

### 示例 2: 添加到更新链

```csharp
UpdateTask task = new UpdateTask();
await task.Init(
    onDownloadSizeCallBack,
    new SetWhiteListProcess(),
    new SetUpdateListProcess(),
    new CustomUpdateProcess(),  // 添加自定义流程
    new AppUpdateProcess(),
    new MainPackageUpdateProcess()
);

UpdateRes result = await task.Process();
```

### 示例 3: 显示提示框

```csharp
public override async ETTask<UpdateRes> Process(UpdateTask task)
{
    // 显示提示框
    bool confirmed = await task.ShowMsgBoxView(
        I18NKey.Custom_Update_Info,
        I18NKey.Global_Btn_Confirm,
        I18NKey.Update_Skip
    );
    
    if (confirmed)
    {
        // 用户确认
        return UpdateRes.Over;
    }
    else
    {
        // 用户跳过
        return UpdateRes.Over;
    }
}
```

---

## 典型流程

```
UpdateTask.Process()
    ↓
for each process in list:
    ↓
process.Process(task)
    ↓
return UpdateRes
    ↓
Over → 继续下一个
Fail → 终止
Quit → 退出
Restart → 重启
```

---

## 相关文档

- [UpdateTask.cs.md](../UpdateTask.cs.md) - 更新任务
- [ServerConfigManager.cs.md](../ServerConfigManager.cs.md) - 服务器配置管理器

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
