# UpdateTask.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | UpdateTask.cs |
| **路径** | Assets/Scripts/Code/Module/Update/UpdateTask.cs |
| **所属模块** | 框架层 → Code/Module/Update |
| **文件职责** | 更新任务管理器，组织和执行更新流程 |

---

## 类/结构体说明

### UpdateTask

| 属性 | 说明 |
|------|------|
| **职责** | 管理更新流程，执行多个 UpdateProcess，处理下载进度和提示框 |
| **泛型参数** | 无 |
| **继承关系** | 无 |
| **实现的接口** | 无 |

```csharp
public class UpdateTask
{
    // 更新任务管理器
}
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `AppVer` | `int` | `public` | 当前 APP 版本号 |
| `list` | `UpdateProcess[]` | `private` | 更新流程列表 |
| `onDownloadSize` | `Action<long, long>` | `private` | 下载进度回调 |
| `DownloadingMaxNum` | `int` | `public` | 最大并发下载数（默认 10） |
| `FailedTryAgain` | `int` | `public` | 失败重试次数（默认 2） |
| `TimeOut` | `int` | `public` | 超时时间（秒，默认 8） |
| `para` | `MsgBoxPara` | `private` | 提示框参数 |

---

## 方法说明（按重要程度排序）

### Init(Action<long, long> downloadSizeCallBack, params UpdateProcess[] process)

**签名**:
```csharp
public async ETTask Init(Action<long, long> downloadSizeCallBack, params UpdateProcess[] process)
```

**职责**: 初始化更新任务

**参数**:
- `downloadSizeCallBack`: 下载进度回调（total, current）
- `process`: 更新流程列表（可变参数）

**核心逻辑**:
```
1. 保存回调函数
2. 保存流程列表
3. 解析当前 APP 版本号
4. 预加载提示框 UI
```

**版本号解析**:
```csharp
var vs = Application.version.Split(".");
AppVer = int.Parse(vs[vs.Length - 1]);  // 取最后一段作为版本号
```

**调用者**: 游戏启动初始化

**使用示例**:
```csharp
UpdateTask task = new UpdateTask();
await task.Init(
    (total, current) => {
        float progress = (float)current / total;
        UpdateProgressBar(progress);
    },
    new SetWhiteListProcess(),
    new SetUpdateListProcess(),
    new AppUpdateProcess(),
    new MainPackageUpdateProcess()
);
```

---

### Process()

**签名**:
```csharp
public async ETTask<UpdateRes> Process()
```

**职责**: 执行更新流程

**返回**: 更新结果 UpdateRes

**核心逻辑**:
```
1. 检查 list 是否初始化
2. 遍历所有流程：
   - 调用 process.Process(task)
   - 根据结果处理：
     * Fail → 返回 Fail
     * Over → 继续下一个
     * Quit → 退出游戏，返回 Quit
     * Restart → 返回 Restart
3. 所有流程完成，返回 Over
```

**结果处理**:
```csharp
switch (res)
{
    case UpdateRes.Fail:
        Log.Error("Update Fail " + list[i].GetType().Name);
        return UpdateRes.Fail;
    case UpdateRes.Over:
        break;
    case UpdateRes.Quit:
        BridgeHelper.Quit();
        return UpdateRes.Quit;
    case UpdateRes.Restart:
        return UpdateRes.Restart;
}
```

**调用者**: 游戏启动流程

---

### SetDownloadSize(long totalDownloadBytes, long currentDownloadBytes)

**签名**:
```csharp
public void SetDownloadSize(long totalDownloadBytes, long currentDownloadBytes)
```

**职责**: 设置下载进度

**参数**:
- `totalDownloadBytes`: 总下载大小
- `currentDownloadBytes`: 当前已下载大小

**核心逻辑**:
```
1. 调用 onDownloadSize 回调
```

**调用者**: `ResourceDownloaderOperation.DownloadUpdateCallback`

---

### ShowMsgBoxView(I18NKey content, I18NKey confirmBtnText, I18NKey cancelBtnText)

**签名**:
```csharp
public async ETTask<bool> ShowMsgBoxView(I18NKey content, I18NKey confirmBtnText, I18NKey cancelBtnText)
```

**职责**: 显示提示框（国际化版本）

**参数**:
- `content`: 内容文本键
- `confirmBtnText`: 确认按钮文本键
- `cancelBtnText`: 取消按钮文本键

**返回**: true=确认，false=取消

**核心逻辑**:
```
1. 创建 ETTask<bool> 用于等待结果
2. 定义确认/取消回调函数
3. 获取国际化文本
4. 设置参数
5. 打开提示框
6. 等待用户选择
7. 返回结果
```

**调用者**: 各个 UpdateProcess

**使用示例**:
```csharp
bool confirmed = await task.ShowMsgBoxView(
    I18NKey.Update_Info,
    I18NKey.Global_Btn_Confirm,
    I18NKey.Update_Skip
);

if (confirmed)
{
    // 用户确认更新
}
else
{
    // 用户跳过更新
}
```

---

### ShowMsgBoxView(string content, I18NKey confirmBtnText, I18NKey cancelBtnText)

**签名**:
```csharp
public async ETTask<bool> ShowMsgBoxView(string content, I18NKey confirmBtnText, I18NKey cancelBtnText)
```

**职责**: 显示提示框（自定义内容）

**与上一个方法的区别**: content 参数为字符串而非 I18NKey

---

## 阅读指引

### 建议的阅读顺序

1. **理解任务作用** - UpdateTask 管理什么
2. **看 Init** - 理解初始化流程
3. **看 Process** - 理解执行流程
4. **看 ShowMsgBoxView** - 理解用户交互

### 最值得学习的技术点

1. **流程链**: 多个 UpdateProcess 顺序执行
2. **异步等待**: ETTask 等待用户选择
3. **进度回调**: Action 回调更新进度
4. **预加载**: 预加载提示框 UI

---

## 更新结果（UpdateRes）

| 结果 | 说明 | 处理 |
|------|------|------|
| `Over` | 更新完成 | 继续游戏 |
| `Fail` | 更新失败 | 返回错误 |
| `Quit` | 退出游戏 | 关闭应用 |
| `Restart` | 需要重启 | 重启游戏 |

---

## 使用示例

### 示例 1: 初始化更新任务

```csharp
// 创建更新任务
UpdateTask task = new UpdateTask();

// 配置参数
task.DownloadingMaxNum = 10;  // 最大并发下载数
task.FailedTryAgain = 2;      // 失败重试次数
task.TimeOut = 8;             // 超时时间（秒）

// 初始化
await task.Init(
    (total, current) => {
        // 更新进度条
        float progress = (float)current / total;
        progressBar.fillAmount = progress;
        sizeText.text = $"{current / 1024 / 1024}MB / {total / 1024 / 1024}MB";
    },
    new SetWhiteListProcess(),      // 设置白名单
    new SetUpdateListProcess(),     // 获取更新列表
    new UpdateIsSHProcess(),        // 检查提审模式
    new AppUpdateProcess(),         // APP 更新检查
    new MainPackageUpdateProcess()  // 主资源包更新
);
```

### 示例 2: 执行更新流程

```csharp
// 执行更新
UpdateRes result = await task.Process();

// 处理结果
switch (result)
{
    case UpdateRes.Over:
        Log.Info("更新完成，启动游戏");
        StartGame();
        break;
    case UpdateRes.Fail:
        Log.Error("更新失败");
        ShowError();
        break;
    case UpdateRes.Restart:
        Log.Info("需要重启");
        RestartGame();
        break;
}
```

### 示例 3: 显示提示框

```csharp
// 显示更新提示
bool confirmed = await task.ShowMsgBoxView(
    $"发现新版本，需要下载 {sizeMB}MB",
    I18NKey.Global_Btn_Confirm,
    I18NKey.Update_Skip
);

if (confirmed)
{
    // 开始更新
    await StartUpdate();
}
else
{
    // 跳过更新
    SkipUpdate();
}
```

---

## 典型更新流程

```
游戏启动
    ↓
UpdateTask.Init()
    ↓
UpdateTask.Process()
    ├── SetWhiteListProcess（白名单检查）
    ├── SetUpdateListProcess（获取更新列表）
    ├── UpdateIsSHProcess（提审模式检查）
    ├── AppUpdateProcess（APP 版本检查）
    └── MainPackageUpdateProcess（资源包更新）
    ↓
更新完成，启动游戏
```

---

## 相关文档

- [ServerConfigManager.cs.md](./ServerConfigManager.cs.md) - 服务器配置管理器
- [UpdateProcess.cs.md](./UpdateProcess/UpdateProcess.cs.md) - 更新流程基类
- [AppUpdateProcess.cs.md](./UpdateProcess/AppUpdateProcess.cs.md) - APP 更新流程

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
