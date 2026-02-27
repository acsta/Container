# ServerConfigManager.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | ServerConfigManager.cs |
| **路径** | Assets/Scripts/Code/Module/Update/ServerConfigManager.cs |
| **所属模块** | 框架层 → Code/Module/Update |
| **文件职责** | 服务器配置管理器，管理多环境配置、白名单、更新列表 |

---

## 类/结构体说明

### ServerConfigManager

| 属性 | 说明 |
|------|------|
| **职责** | 管理服务器配置、环境切换、白名单、更新列表 |
| **泛型参数** | 无 |
| **继承关系** | 无 |
| **实现的接口** | `IManager` |

```csharp
public class ServerConfigManager : IManager
{
    // 服务器配置管理器
}
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `Instance` | `ServerConfigManager` | `public static` | 单例实例 |
| `serverKey` | `string` | `private readonly` | 服务器 ID 缓存键 |
| `defaultServer` | `int` | `private readonly` | 默认服务器 ID（1） |
| `curConfig` | `ServerConfig` | `private` | 当前服务器配置 |
| `inWhiteList` | `bool` | `private` | 是否在白名单中 |
| `resUpdateList` | `Dictionary<string, Dictionary<int, Resver>>` | `private` | 资源更新列表 |
| `appUpdateList` | `Dictionary<string, AppConfig>` | `private` | APP 更新列表 |

---

## 方法说明（按重要程度排序）

### Init()

**签名**:
```csharp
public void Init()
```

**职责**: 初始化服务器配置管理器

**核心逻辑**:
```
1. 设置 Instance = this
2. 如果是调试模式：
   - 从缓存获取服务器 ID
   - 获取对应配置
3. 如果配置为空：
   - 遍历所有配置
   - 优先选择 IsPriority=1 的配置
```

**调用者**: `ManagerProvider.Init()`

---

### GetCurConfig()

**签名**:
```csharp
public ServerConfig GetCurConfig()
```

**职责**: 获取当前服务器配置

**返回**: 当前 ServerConfig

**核心逻辑**:
```
1. 返回 curConfig
```

**调用者**: 需要访问服务器配置的代码

---

### ChangeEnv(int id)

**签名**:
```csharp
public ServerConfig ChangeEnv(int id)
```

**职责**: 切换环境

**参数**:
- `id`: 目标环境 ID

**返回**: 新的 ServerConfig

**核心逻辑**:
```
1. 获取配置
2. 如果配置存在：
   - 更新 curConfig
   - 如果是调试模式，缓存服务器 ID
3. 返回 curConfig
```

**调用者**: 环境切换 UI

**使用示例**:
```csharp
// 切换到测试环境
ServerConfigManager.Instance.ChangeEnv(2);
```

---

### GetUpdateListUrl()

**签名**:
```csharp
public string GetUpdateListUrl()
```

**职责**: 获取更新列表 CDN 地址

**返回**: 更新列表 URL

**核心逻辑**:
```
1. 根据白名单模式选择 URL：
   - 白名单模式 → TestUpdateListUrl
   - 正常模式 → UpdateListUrl
2. 返回 URL
```

**调用者**: `GetUpdateListCdnUrl()`, `GetWhiteListCdnUrl()`

---

### GetWhiteListCdnUrl()

**签名**:
```csharp
public string GetWhiteListCdnUrl()
```

**职责**: 获取白名单下载地址

**返回**: 白名单 URL

**核心逻辑**:
```
1. 获取更新列表 URL
2. 拼接白名单路径
3. 添加时间戳参数
4. 返回 URL
```

**返回格式**:
```
{updateListUrl}/white.list?timestamp={timestamp}
```

**调用者**: `SetWhiteListProcess.Process()`

---

### SetWhiteMode(bool whiteMode)

**签名**:
```csharp
public void SetWhiteMode(bool whiteMode)
```

**职责**: 设置白名单模式

**参数**:
- `whiteMode`: true=白名单模式，false=正常模式

**核心逻辑**:
```
1. 设置 RemoteServices.Instance.whiteMode
```

**调用者**: `SetWhiteListProcess.Process()`

---

### SetWhiteList(List<WhiteConfig> info)

**签名**:
```csharp
public void SetWhiteList(List<WhiteConfig> info)
```

**职责**: 设置白名单列表

**参数**:
- `info`: 白名单配置列表

**核心逻辑**:
```
1. 重置 inWhiteList = false
2. 获取当前环境 ID
3. 获取当前账号
4. 遍历白名单：
   - 如果 EnvId 和 Account 匹配
   - 设置 inWhiteList = true
5. 记录日志
```

**白名单格式**:
```json
{
  "WhiteList": [
    {"EnvId": 1, "Account": 11111}
  ]
}
```

**调用者**: `SetWhiteListProcess.Process()`

---

### IsInWhiteList()

**签名**:
```csharp
public bool IsInWhiteList()
```

**职责**: 检查是否在白名单中

**返回**: true=在名单中，false=不在

**核心逻辑**:
```
1. 返回 inWhiteList
```

**调用者**: 白名单检查逻辑

---

### GetUpdateListCdnUrl()

**签名**:
```csharp
public string GetUpdateListCdnUrl()
```

**职责**: 获取更新列表地址（平台独立）

**返回**: 更新列表 URL

**核心逻辑**:
```
1. 获取更新列表 URL
2. 拼接路径：update_{platform}.list
3. 添加时间戳参数
4. 返回 URL
```

**返回格式**:
```
{updateListUrl}/update_{platform}.list?timestamp={timestamp}
```

**调用者**: `SetUpdateListProcess.Process()`

---

### SetUpdateList(UpdateConfig info)

**签名**:
```csharp
public void SetUpdateList(UpdateConfig info)
```

**职责**: 设置更新列表

**参数**:
- `info`: 更新配置（包含 AppList 和 ResList）

**核心逻辑**:
```
1. 保存 appUpdateList
2. 保存 resUpdateList
```

**调用者**: `SetUpdateListProcess.Process()`

---

### GetAppUpdateListByChannel(string channel)

**签名**:
```csharp
public AppConfig GetAppUpdateListByChannel(string channel)
```

**职责**: 根据渠道获取 APP 更新列表

**参数**:
- `channel`: 渠道名（如 googleplay, appstore）

**返回**: AppConfig（包含版本信息和下载链接）

**核心逻辑**:
```
1. 检查 appUpdateList 是否为 null
2. 获取渠道配置
3. 如果有跳转渠道，处理跳转
4. 返回配置
```

**调用者**: `AppUpdateProcess.Process()`

---

### FindMaxUpdateAppVer(string channel)

**签名**:
```csharp
public int FindMaxUpdateAppVer(string channel)
```

**职责**: 找到可以更新的最大 APP 版本号

**参数**:
- `channel`: 渠道名

**返回**: 最大版本号（-1 表示无更新）

**核心逻辑**:
```
1. 检查 appUpdateList 是否为 null
2. 获取渠道配置
3. 处理跳转渠道
4. 遍历所有版本：
   - 检查渠道匹配
   - 检查尾号匹配
   - 更新最大版本号
5. 返回最大版本号
```

**调用者**: `AppUpdateProcess.Process()`

---

## 阅读指引

### 建议的阅读顺序

1. **理解管理器作用** - 服务器配置管理什么
2. **看 Init** - 理解初始化流程
3. **看白名单相关** - 理解白名单机制
4. **看更新列表相关** - 理解更新列表管理

### 最值得学习的技术点

1. **多环境管理**: 支持多服务器环境切换
2. **白名单系统**: 支持灰度发布
3. **渠道配置**: 支持多渠道更新策略
4. **缓存机制**: 调试模式下缓存环境选择

---

## 使用示例

### 示例 1: 获取当前配置

```csharp
ServerConfig config = ServerConfigManager.Instance.GetCurConfig();
Log.Info($"当前环境：{config.Name}");
Log.Info($"环境 ID: {config.EnvId}");
```

### 示例 2: 切换环境

```csharp
// 切换到测试环境
ServerConfigManager.Instance.ChangeEnv(2);

// 重新加载资源
await LoadResources();
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

### 示例 4: 获取更新列表

```csharp
string url = ServerConfigManager.Instance.GetUpdateListCdnUrl();
Log.Info($"更新列表地址：{url}");

// 下载更新配置
UpdateConfig config = await HttpManager.Instance.HttpGetResult<UpdateConfig>(url);
ServerConfigManager.Instance.SetUpdateList(config);
```

---

## 相关文档

- [UpdateTask.cs.md](./UpdateTask.cs.md) - 更新任务
- [AppUpdateProcess.cs.md](./UpdateProcess/AppUpdateProcess.cs.md) - APP 更新流程
- [SetWhiteListProcess.cs.md](./UpdateProcess/SetWhiteListProcess.cs.md) - 白名单设置流程

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
