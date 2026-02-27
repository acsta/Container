# UpdateRes.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | UpdateRes.cs |
| **路径** | Assets/Scripts/Code/Module/Update/UpdateRes.cs |
| **所属模块** | 框架层 → Code/Module/Update |
| **文件职责** | 更新结果枚举定义 |

---

## 枚举说明

### UpdateRes

| 属性 | 说明 |
|------|------|
| **职责** | 定义更新流程的返回结果 |
| **类型** | enum |

```csharp
public enum UpdateRes
{
    Fail = -1,
    Over,
    Quit,
    Restart,
}
```

---

## 更新结果

| 值 | 枚举 | 说明 | 处理 |
|------|------|------|------|
| -1 | `Fail` | 更新失败 | 显示错误，可能重试 |
| 0 | `Over` | 更新完成 | 继续游戏流程 |
| 1 | `Quit` | 退出游戏 | 关闭应用 |
| 2 | `Restart` | 需要重启 | 重启游戏 |

---

## 使用场景

### Fail（更新失败）

**场景**:
- 网络请求失败
- 资源下载失败
- 版本检查失败

**处理**:
```csharp
case UpdateRes.Fail:
    Log.Error("Update Fail " + list[i].GetType().Name);
    return UpdateRes.Fail;
```

### Over（更新完成）

**场景**:
- 无需更新
- 用户跳过更新
- 所有更新流程完成

**处理**:
```csharp
case UpdateRes.Over:
    break;  // 继续下一个流程
```

### Quit（退出游戏）

**场景**:
- 强制更新用户取消
- 严重错误无法继续

**处理**:
```csharp
case UpdateRes.Quit:
    BridgeHelper.Quit();
    return UpdateRes.Quit;
```

### Restart（需要重启）

**场景**:
- 资源包更新完成
- 需要重新加载资源

**处理**:
```csharp
case UpdateRes.Restart:
    return UpdateRes.Restart;  // 触发重启
```

---

## 使用示例

### 示例 1: UpdateTask 处理

```csharp
public async ETTask<UpdateRes> Process()
{
    for (int i = 0; i < list.Length; i++)
    {
        if (list[i] == null) continue;
        
        var res = await list[i].Process(this);
        
        switch (res)
        {
            case UpdateRes.Fail:
                Log.Error("Update Fail " + list[i].GetType().Name);
                return UpdateRes.Fail;
                
            case UpdateRes.Over:
                break;  // 继续下一个
                
            case UpdateRes.Quit:
                BridgeHelper.Quit();
                return UpdateRes.Quit;
                
            case UpdateRes.Restart:
                return UpdateRes.Restart;
        }
    }
    
    return UpdateRes.Over;
}
```

### 示例 2: UpdateProcess 返回

```csharp
public override async ETTask<UpdateRes> Process(UpdateTask task)
{
    // 检查更新
    if (!hasUpdate)
    {
        return UpdateRes.Over;  // 无需更新
    }
    
    // 下载更新
    bool success = await DownloadUpdate();
    if (!success)
    {
        return UpdateRes.Fail;  // 下载失败
    }
    
    // 需要重启
    return UpdateRes.Restart;
}
```

### 示例 3: 强制更新处理

```csharp
public override async ETTask<UpdateRes> Process(UpdateTask task)
{
    // 显示更新提示
    bool confirmed = await task.ShowMsgBoxView(
        I18NKey.Update_Info,
        I18NKey.Global_Btn_Confirm,
        forceUpdate ? I18NKey.Btn_Exit : I18NKey.Update_Skip
    );
    
    if (!confirmed)
    {
        if (forceUpdate)
        {
            BridgeHelper.Quit();
            return UpdateRes.Quit;  // 强制更新取消，退出
        }
        return UpdateRes.Over;  // 非强制，跳过
    }
    
    // 开始更新
    return UpdateRes.Restart;
}
```

---

## 更新流程

```
UpdateTask.Process()
    ↓
遍历 UpdateProcess[]
    ↓
每个 Process 返回 UpdateRes
    ↓
根据结果处理:
├─ Fail → 返回 Fail
├─ Over → 继续下一个
├─ Quit → 退出游戏
└─ Restart → 重启游戏
```

---

## 相关文档

- [UpdateTask.cs.md](./UpdateTask.cs.md) - 更新任务
- [UpdateProcess.cs.md](./UpdateProcess/UpdateProcess.cs.md) - 更新流程基类
- [AppUpdateProcess.cs.md](./UpdateProcess/AppUpdateProcess.cs.md) - APP 更新流程

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
