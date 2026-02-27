# OtherPackageUpdateProcess.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | OtherPackageUpdateProcess.cs |
| **路径** | Assets/Scripts/Code/Module/Update/UpdateProcess/OtherPackageUpdateProcess.cs |
| **所属模块** | 框架层 → Code/Module/Update/UpdateProcess |
| **文件职责** | 分包资源更新流程 |

---

## 类/结构体说明

### OtherPackageUpdateProcess

| 属性 | 说明 |
|------|------|
| **职责** | 检查并下载分包资源更新 |
| **泛型参数** | 无 |
| **继承关系** | 继承自 `UpdateProcess` |
| **实现的接口** | 无 |

```csharp
public class OtherPackageUpdateProcess : UpdateProcess
{
    // 分包资源更新流程
}
```

---

## 字段与属性

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `PackageNames` | `string[]` | `private` | 分包名称列表 |
| `OnUpdateFail` | `Func<ETTask<bool>>` | `public` | 更新失败回调（可选） |
| `ForceUpdate` | `bool` | `public` | 是否强制更新 |
| `Config` | `PackageConfig` | `private` | 包配置（从 PackageManager 获取） |

---

## 方法说明

### Constructor(params string[] packageNames)

**签名**:
```csharp
public OtherPackageUpdateProcess(params string[] packageNames)
```

**职责**: 构造函数，设置分包名称

**参数**:
- `packageNames`: 分包名称列表（可变参数）

**使用示例**:
```csharp
// 创建分包更新流程
new OtherPackageUpdateProcess("GamePlay", "UIResources");
```

---

### Process(UpdateTask task)

**签名**:
```csharp
public override async ETTask<UpdateRes> Process(UpdateTask task)
```

**职责**: 执行分包资源更新

**核心逻辑**:
```
1. 如果没有分包名称，返回 Over
2. 检查网络运行模式
3. 初始化分包（GetPackageAsync）
4. 等待所有分包初始化完成
5. 遍历所有分包：
   - 获取最大版本号
   - 获取当前版本号
   - 如果版本不同，更新补丁清单
6. 创建资源下载器（合并所有分包）
7. 如果没有需要下载的资源，返回 Over
8. 计算下载大小
9. 显示提示框
10. 如果用户取消：
    - 强制更新 → 退出游戏
    - 非强制 → 恢复版本号
11. 开始下载
12. 更新下载进度回调
13. 等待下载完成
14. 如果下载失败，调用 UpdateFail
15. 下载成功，返回 Over
```

**分包初始化**:
```csharp
using (ListComponent<ETTask<ResourcePackage>> tasks = ListComponent<ETTask<ResourcePackage>>.Create())
{
    for (int i = 0; i < PackageNames.Length; i++)
    {
        tasks.Add(PackageManager.Instance.GetPackageAsync(PackageNames[i]));
    }
    await ETTaskHelper.WaitAll(tasks);
}
```

**下载器合并**:
```csharp
ResourceDownloaderOperation downloader = null;
for (int i = 0; i < PackageNames.Length; i++)
{
    var temp = PackageManager.Instance.CreateResourceDownloader(
        task.DownloadingMaxNum, 
        task.FailedTryAgain, 
        task.TimeOut, 
        PackageNames[i]
    );
    if (temp.TotalDownloadCount != 0)
    {
        if (downloader == null)
        {
            downloader = temp;
        }
        else
        {
            downloader.Combine(temp);  // 合并下载器
        }
    }
}
```

**调用者**: `UpdateTask.Process()`

---

### UpdateFail(UpdateTask task, bool reset)

**签名**:
```csharp
private async ETTask<UpdateRes> UpdateFail(UpdateTask task, bool reset)
```

**职责**: 处理更新失败

**核心逻辑**:
```
1. 如果有 OnUpdateFail 回调，调用回调
2. 否则显示失败提示框
3. 如果用户选择重试：
   - 递归调用 Process
4. 如果强制更新且用户取消：
   - 退出游戏
5. 如果需要恢复版本号：
   - 调用 ResetVersion
6. 返回 Over
```

**自定义失败处理**:
```csharp
bool btnState;
if (OnUpdateFail != null)
{
    btnState = await OnUpdateFail();  // 自定义失败处理
}
else
{
    btnState = await task.ShowMsgBoxView(...);  // 默认提示框
}
```

**调用者**: `Process()`

---

### ResetVersion(UpdateTask task)

**签名**:
```csharp
private async ETTask<UpdateRes> ResetVersion(UpdateTask task)
```

**职责**: 恢复版本号

**核心逻辑**:
```
1. 遍历所有分包：
   - 获取当前版本号
   - 更新补丁清单到当前版本
2. 如果任何分包失败，显示提示框
3. 根据用户选择重试或退出
4. 返回 Over
```

**调用者**: `Process()`, `UpdateFail()`

---

## 阅读指引

### 建议的阅读顺序

1. **理解流程作用** - 分包更新做什么
2. **看构造函数** - 理解如何指定分包
3. **看 Process** - 理解更新逻辑
4. **看下载器合并** - 理解多分包下载

### 最值得学习的技术点

1. **多分包支持**: 支持同时更新多个分包
2. **下载器合并**: Combine 合并多个下载器
3. **自定义失败处理**: OnUpdateFail 回调
4. **并发初始化**: ETTaskHelper.WaitAll 并发初始化
5. **版本恢复**: 用户跳过时恢复版本号

---

## 与 MainPackageUpdateProcess 的区别

| 特性 | MainPackageUpdateProcess | OtherPackageUpdateProcess |
|------|-------------------------|--------------------------|
| 更新对象 | 主资源包 | 分包资源 |
| 分包数量 | 1 个 | 多个 |
| 下载器 | 单个 | 合并多个 |
| 重启 | 需要（Restart） | 不需要（Over） |
| 失败回调 | 无 | OnUpdateFail |

---

## 使用示例

### 示例 1: 创建分包更新

```csharp
UpdateTask task = new UpdateTask();
await task.Init(
    onDownloadSizeCallBack,
    new SetWhiteListProcess(),
    new SetUpdateListProcess(),
    new MainPackageUpdateProcess(),
    new OtherPackageUpdateProcess("GamePlay", "UIResources", "Audio")  // 分包更新
);

UpdateRes result = await task.Process();
```

### 示例 2: 自定义失败处理

```csharp
var分包更新 = new OtherPackageUpdateProcess("GamePlay");
分包更新.OnUpdateFail = async () =>
{
    // 自定义失败处理
    Log.Error("分包更新失败，使用默认资源");
    UseDefaultResources();
    return true;  // 返回 true 表示继续
};

await task.Init(onDownloadSizeCallBack, 分包更新);
```

### 示例 3: 强制更新

```csharp
var分包更新 = new OtherPackageUpdateProcess("GamePlay");
分包更新.ForceUpdate = true;  // 强制更新

await task.Init(onDownloadSizeCallBack, 分包更新);
```

---

## 流程示例

```
初始化分包
    ↓
GamePlay, UIResources, Audio
    ↓
检查各分包版本
    ↓
当前版本：5, 3, 2
最大版本：10, 5, 3
    ↓
更新补丁清单
    ↓
创建下载器（合并）
    ↓
发现需要下载 30MB
    ↓
显示提示框
    ↓
用户选择：
├─ 确认 → 开始下载 → 显示进度 → 下载完成 → Over
└─ 取消 → 
    ├─ 强制更新 → Quit
    └─ 非强制 → 恢复版本号 → Over
```

---

## 相关文档

- [MainPackageUpdateProcess.cs.md](./MainPackageUpdateProcess.cs.md) - 主资源包更新
- [UpdateTask.cs.md](../UpdateTask.cs.md) - 更新任务
- [UpdateProcess.cs.md](./UpdateProcess.cs.md) - 更新流程基类

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
