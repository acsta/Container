# SetWhiteListProcess.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | SetWhiteListProcess.cs |
| **路径** | Assets/Scripts/Code/Module/Update/UpdateProcess/SetWhiteListProcess.cs |
| **所属模块** | 框架层 → Code/Module/Update/UpdateProcess |
| **文件职责** | 获取并设置白名单 |

---

## 类/结构体说明

### SetWhiteListProcess

| 属性 | 说明 |
|------|------|
| **职责** | 从 CDN 下载白名单，检查当前用户是否在名单中 |
| **泛型参数** | 无 |
| **继承关系** | 继承自 `UpdateProcess` |
| **实现的接口** | 无 |

```csharp
public class SetWhiteListProcess : UpdateProcess
{
    // 获取白名单流程
}
```

---

## 方法说明

### Process(UpdateTask task)

**签名**:
```csharp
public override async ETTask<UpdateRes> Process(UpdateTask task)
```

**职责**: 执行白名单获取

**核心逻辑**:
```
1. 获取白名单 CDN URL
2. 如果 URL 为空，返回 Over
3. HTTP GET 请求下载白名单列表
4. 如果下载成功：
   - 调用 ServerConfigManager.SetWhiteList
   - 检查是否在白名单中
   - 如果在名单中：
     * 显示提示框
     * 用户确认 → 设置白名单模式
5. 返回 Over
```

**白名单检查**:
```csharp
var info = await HttpManager.Instance.HttpGetResult<List<WhiteConfig>>(url);
if (info != null)
{
    ServerConfigManager.Instance.SetWhiteList(info);
    if (ServerConfigManager.Instance.IsInWhiteList())
    {
        var btnState = await task.ShowMsgBoxView(
            I18NKey.Update_White,
            I18NKey.Global_Btn_Confirm,
            I18NKey.Global_Btn_Cancel
        );
        if (btnState)
        {
            ServerConfigManager.Instance.SetWhiteMode(true);
        }
    }
}
```

**调用者**: `UpdateTask.Process()`

---

## 白名单配置（WhiteConfig）

```json
{
  "EnvId": 1,
  "Account": 11111
}
```

**字段说明**:
- `EnvId`: 环境 ID（1=正式，2=测试等）
- `Account`: 账号 ID

**完整格式**:
```json
{
  "WhiteList": [
    {"EnvId": 1, "Account": 11111},
    {"EnvId": 1, "Account": 22222},
    {"EnvId": 2, "Account": 33333}
  ]
}
```

---

## 阅读指引

### 建议的阅读顺序

1. **理解流程作用** - 白名单有什么用
2. **看 Process** - 理解获取逻辑
3. **了解配置格式** - 理解 WhiteConfig 结构
4. **看提示框** - 理解用户交互

### 最值得学习的技术点

1. **灰度发布**: 白名单支持灰度测试
2. **环境匹配**: 根据 EnvId 匹配环境
3. **账号匹配**: 根据 Account 匹配账号
4. **用户确认**: 提示用户是否启用白名单模式

---

## 白名单用途

1. **测试功能**: 白名单用户可访问测试功能
2. **灰度发布**: 部分用户先体验新版本
3. **内部测试**: 内部员工测试环境
4. **问题排查**: 特定用户复现问题

---

## 使用示例

### 示例 1: 添加到更新链

```csharp
UpdateTask task = new UpdateTask();
await task.Init(
    onDownloadSizeCallBack,
    new SetWhiteListProcess(),  // 白名单检查
    new SetUpdateListProcess(),
    new AppUpdateProcess(),
    new MainPackageUpdateProcess()
);

UpdateRes result = await task.Process();
```

### 示例 2: 白名单 URL

```csharp
// ServerConfigManager.GetWhiteListCdnUrl() 返回:
// {updateListUrl}/white.list?timestamp={timestamp}

// 示例:
// https://cdn.example.com/white.list?timestamp=1709020800000
```

### 示例 3: 检查白名单

```csharp
if (ServerConfigManager.Instance.IsInWhiteList())
{
    Log.Info("当前用户在白名单中");
    // 启用测试功能
    EnableTestFeatures();
}
```

---

## 流程示例

```
游戏启动
    ↓
SetWhiteListProcess.Process()
    ↓
GET {cdnUrl}/white.list
    ↓
下载成功？
├─ 是 → SetWhiteList(info)
│        ↓
│    在名单中？
│    ├─ 是 → 显示提示框 → 用户确认 → SetWhiteMode(true)
│    └─ 否 → Over
└─ 否 → Over
```

**提示框内容**:
```
"检测到您在白名单中，是否启用测试模式？"
[确认] [取消]
```

---

## 相关文档

- [ServerConfigManager.cs.md](../ServerConfigManager.cs.md) - 服务器配置管理器
- [UpdateTask.cs.md](../UpdateTask.cs.md) - 更新任务
- [UpdateProcess.cs.md](./UpdateProcess.cs.md) - 更新流程基类

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
