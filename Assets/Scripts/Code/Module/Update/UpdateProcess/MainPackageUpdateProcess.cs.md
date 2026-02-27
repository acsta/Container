# MainPackageUpdateProcess.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | MainPackageUpdateProcess.cs |
| **路径** | Assets/Scripts/Code/Module/Update/UpdateProcess/MainPackageUpdateProcess.cs |
| **所属模块** | 框架层 → Code/Module/Update/UpdateProcess |
| **文件职责** | 主资源包更新流程 |

---

## 类/结构体说明

### MainPackageUpdateProcess

| 属性 | 说明 |
|------|------|
| **职责** | 检查并下载主资源包更新 |
| **泛型参数** | 无 |
| **继承关系** | 继承自 `UpdateProcess` |
| **实现的接口** | 无 |

```csharp
public class MainPackageUpdateProcess : UpdateProcess
{
    // 主资源包更新流程
}
```

---

## 方法说明

### Process(UpdateTask task)

**签名**:
```csharp
public override async ETTask<UpdateRes> Process(UpdateTask task)
```

**职责**: 执行主资源包更新

**参数**:
- `task`: 更新任务实例

**返回**: 更新结果 UpdateRes

**核心逻辑**:
```
1. 获取当前渠道
2. 查找最大资源版本号
3. 如果没有最大版本，返回 Over
4. 获取当前资源版本
5. 检查强制更新标志
6. 如果是网络运行模式，继续；否则返回 Over
7. 更新补丁清单（UpdatePackageManifestAsync）
8. 如果更新失败，调用 UpdateFail
9. 创建资源下载器
10. 如果没有需要下载的资源，返回 Over
11. 计算需要下载的大小
12. 显示提示框
13. 如果用户取消：
    - 强制更新 → 退出游戏
    - 非强制 → 恢复版本号，返回 Over
14. 开始下载
15. 更新下载进度回调
16. 等待下载完成
17. 如果下载失败，调用 UpdateFail
18. 下载成功，返回 Restart（需要重启）
```

**补丁清单更新**:
```csharp
var op = PackageManager.Instance.UpdatePackageManifestAsync(
    maxVer.ToString(), 
    task.TimeOut, 
    null
);
await op.Task;

if (op.Status != EOperationStatus.Succeed)
{
    Log.Error(op.Error);
    return await UpdateFail(task, maxVer != version);
}
```

**下载进度回调**:
```csharp
downloader.DownloadUpdateCallback = (a) =>
{
    task.SetDownloadSize(a.TotalDownloadBytes, a.CurrentDownloadBytes);
};
```

**调用者**: `UpdateTask.Process()`

---

### ResetVersion(UpdateTask task)

**签名**:
```csharp
private async ETTask<UpdateRes> ResetVersion(UpdateTask task)
```

**职责**: 恢复版本号（用户跳过更新时）

**参数**:
- `task`: 更新任务实例

**返回**: 更新结果 UpdateRes

**核心逻辑**:
```
1. 获取当前版本号
2. 更新补丁清单到当前版本
3. 如果失败，显示提示框
4. 根据用户选择重试或退出
5. 返回 Over
```

**调用者**: `Process()`, `UpdateFail()`

---

### UpdateFail(UpdateTask task, bool reset)

**签名**:
```csharp
private async ETTask<UpdateRes> UpdateFail(UpdateTask task, bool reset)
```

**职责**: 处理更新失败

**参数**:
- `task`: 更新任务实例
- `reset`: 是否需要恢复版本号

**返回**: 更新结果 UpdateRes

**核心逻辑**:
```
1. 显示失败提示框
2. 如果用户选择重试：
   - 递归调用 Process
3. 如果强制更新且用户取消：
   - 退出游戏
4. 如果需要恢复版本号：
   - 调用 ResetVersion
5. 返回 Over
```

**调用者**: `Process()`, `ResetVersion()`

---

## 阅读指引

### 建议的阅读顺序

1. **理解流程作用** - 主资源包更新做什么
2. **看 Process** - 理解更新逻辑
3. **看 ResetVersion** - 理解版本恢复
4. **看 UpdateFail** - 理解失败处理

### 最值得学习的技术点

1. **YooAsset 集成**: 使用 YooAsset 管理资源
2. **补丁清单**: UpdatePackageManifestAsync 更新清单
3. **资源下载器**: CreateResourceDownloader 创建下载器
4. **进度回调**: DownloadUpdateCallback 更新进度
5. **失败重试**: 支持重试机制
6. **版本恢复**: 用户跳过时恢复版本号

---

## 更新结果

| 结果 | 说明 | 处理 |
|------|------|------|
| `Over` | 无需更新或用户跳过 | 继续游戏 |
| `Fail` | 更新失败 | 显示错误 |
| `Quit` | 退出游戏 | 关闭应用 |
| `Restart` | 需要重启 | 重启游戏 |

---

## 使用示例

### 示例 1: 添加到更新链

```csharp
UpdateTask task = new UpdateTask();
await task.Init(
    onDownloadSizeCallBack,
    new SetWhiteListProcess(),
    new SetUpdateListProcess(),
    new AppUpdateProcess(),
    new MainPackageUpdateProcess()  // 主资源包更新
);

UpdateRes result = await task.Process();
```

### 示例 2: 配置下载参数

```csharp
task.DownloadingMaxNum = 10;   // 最大并发下载数
task.FailedTryAgain = 2;       // 失败重试次数
task.TimeOut = 8;              // 超时时间（秒）
```

### 示例 3: 更新进度显示

```csharp
await task.Init(
    (total, current) => {
        float progress = (float)current / total;
        progressBar.fillAmount = progress;
        
        string sizeText = $"{current / 1024 / 1024:F1}MB / {total / 1024 / 1024:F1}MB";
        sizeLabel.text = sizeText;
    },
    new MainPackageUpdateProcess()
);
```

---

## 流程示例

```
检查版本
    ↓
当前版本：5
最大版本：10
    ↓
更新补丁清单
    ↓
创建下载器
    ↓
发现需要下载 50MB
    ↓
显示提示框："需要下载 50.00MB，是否更新？"
    ↓
用户选择：
├─ 确认 → 开始下载 → 显示进度 → 下载完成 → Restart
└─ 取消 → 
    ├─ 强制更新 → Quit
    └─ 非强制 → 恢复版本号 → Over
```

---

## 相关文档

- [UpdateTask.cs.md](../UpdateTask.cs.md) - 更新任务
- [UpdateProcess.cs.md](./UpdateProcess.cs.md) - 更新流程基类
- [OtherPackageUpdateProcess.cs.md](./OtherPackageUpdateProcess.cs.md) - 分包更新流程

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
