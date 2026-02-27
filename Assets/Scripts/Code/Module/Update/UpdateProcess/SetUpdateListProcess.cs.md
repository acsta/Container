# SetUpdateListProcess.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | SetUpdateListProcess.cs |
| **路径** | Assets/Scripts/Code/Module/Update/UpdateProcess/SetUpdateListProcess.cs |
| **所属模块** | 框架层 → Code/Module/Update/UpdateProcess |
| **文件职责** | 获取并设置更新列表 |

---

## 类/结构体说明

### SetUpdateListProcess

| 属性 | 说明 |
|------|------|
| **职责** | 从 CDN 下载更新列表配置 |
| **泛型参数** | 无 |
| **继承关系** | 继承自 `UpdateProcess` |
| **实现的接口** | 无 |

```csharp
public class SetUpdateListProcess : UpdateProcess
{
    // 获取更新列表流程
}
```

---

## 方法说明

### Process(UpdateTask task)

**签名**:
```csharp
public override async ETTask<UpdateRes> Process(UpdateTask task)
```

**职责**: 执行更新列表获取

**核心逻辑**:
```
1. 获取更新列表 CDN URL
2. HTTP GET 请求下载 UpdateConfig
3. 如果下载失败：
   - 显示失败提示框
   - 用户选择重试 → 递归调用
   - 强制更新且取消 → 退出游戏
   - 非强制且取消 → 返回 Over
4. 如果下载成功：
   - 调用 ServerConfigManager.SetUpdateList
5. 返回 Over
```

**HTTP 请求**:
```csharp
var info = await HttpManager.Instance.HttpGetResult<UpdateConfig>(url);
if (info == null)
{
    // 下载失败
    var btnState = await task.ShowMsgBoxView(...);
    if (btnState)
    {
        await this.Process(task);  // 重试
    }
}
else
{
    ServerConfigManager.Instance.SetUpdateList(info);
}
```

**调用者**: `UpdateTask.Process()`

---

## 更新配置（UpdateConfig）

```json
{
  "AppList": {
    "googleplay": {
      "AppUrl": "https://play.google.com/...",
      "AppVer": {
        "1": {
          "ForceUpdate": 0,
          "Channel": ["all"],
          "UpdateTailNumber": ["all"]
        }
      }
    }
  },
  "ResList": {
    "googleplay": {
      "1": {
        "Channel": ["all"],
        "UpdateTailNumber": ["all"]
      }
    }
  }
}
```

**字段说明**:
- `AppList`: APP 更新列表（按渠道）
- `ResList`: 资源更新列表（按渠道和版本）

---

## 阅读指引

### 建议的阅读顺序

1. **理解流程作用** - 获取更新列表做什么
2. **看 Process** - 理解获取逻辑
3. **了解配置格式** - 理解 UpdateConfig 结构
4. **看失败处理** - 理解重试机制

### 最值得学习的技术点

1. **HTTP GET**: HttpManager 下载配置
2. **失败重试**: 递归调用实现重试
3. **强制更新**: Define.ForceUpdate 控制
4. **配置解析**: JSON 反序列化

---

## 使用示例

### 示例 1: 添加到更新链

```csharp
UpdateTask task = new UpdateTask();
await task.Init(
    onDownloadSizeCallBack,
    new SetWhiteListProcess(),
    new SetUpdateListProcess(),  // 获取更新列表
    new AppUpdateProcess(),
    new MainPackageUpdateProcess()
);

UpdateRes result = await task.Process();
```

### 示例 2: 获取更新列表 URL

```csharp
// ServerConfigManager.GetUpdateListCdnUrl() 返回:
// {updateListUrl}/update_{platform}.list?timestamp={timestamp}

// 示例:
// https://cdn.example.com/update_android.list?timestamp=1709020800000
```

---

## 流程示例

```
游戏启动
    ↓
SetUpdateListProcess.Process()
    ↓
GET {cdnUrl}/update_android.list
    ↓
下载成功？
├─ 是 → ServerConfigManager.SetUpdateList(info) → Over
└─ 否 → 显示提示框
         ↓
    用户选择：
    ├─ 重试 → 递归调用
    └─ 取消 → 
        ├─ 强制 → Quit
        └─ 非强制 → Over
```

---

## 相关文档

- [ServerConfigManager.cs.md](../ServerConfigManager.cs.md) - 服务器配置管理器
- [UpdateTask.cs.md](../UpdateTask.cs.md) - 更新任务
- [UpdateProcess.cs.md](./UpdateProcess.cs.md) - 更新流程基类

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
