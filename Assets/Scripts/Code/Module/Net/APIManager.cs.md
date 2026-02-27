# APIManager.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | APIManager.cs |
| **路径** | Assets/Scripts/Code/Module/Net/APIManager.cs |
| **所属模块** | 框架层 → Code/Module/Net |
| **文件职责** | 网络 API 管理，封装 HTTP 请求、错误处理、重试机制 |

---

## 类/结构体说明

### APIManager

| 属性 | 说明 |
|------|------|
| **职责** | 管理所有网络 API 请求，提供统一的 HTTP 接口 |
| **泛型参数** | 无 |
| **继承关系** | 无继承 |
| **实现的接口** | `IManager` |

**设计模式**: 单例模式 + 重试模式

```csharp
// 单例实现
public static APIManager Instance { get; private set; }

// 通过 ManagerProvider 注册
ManagerProvider.RegisterManager<APIManager>();
```

---

## 字段与属性

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `Instance` | `APIManager` | `public static` | 单例实例 |
| `GAME` | `const string` | `private` | 游戏标识 "p4" |
| `platform` | `LoginPlatform` | `private` | 当前登录平台 |
| `baseurl` | `string` | `private` | API 基础 URL |

---

## 方法说明（按重要程度排序）

### Init()

**签名**:
```csharp
public void Init()
```

**职责**: 初始化 API 管理器

**核心逻辑**:
```
1. 设置单例 Instance = this
2. 根据 Debug 模式设置 baseurl：
   - Debug: http://192.168.0.6:29527/
   - Release: https://wgsh.hxwgame.cn/dish1/
```

**调用者**: ManagerProvider.RegisterManager<APIManager>()

---

### MiniGameLogin(LoginPlatform platform, string token)

**签名**:
```csharp
public async ETTask<LoginResult> MiniGameLogin(LoginPlatform platform, string token)
```

**职责**: 小游戏登录 API

**参数**:
- `platform`: 登录平台
- `token`: 登录 token

**返回**: `ETTask<LoginResult>` - 登录结果

**核心逻辑**:
```
1. 构建 URL: baseurl + "MiniGameLogin"
2. 保存 platform
3. 构建参数：platform, token
4. 调用 HttpPost<LoginResult>()
5. 返回结果
```

**调用者**: PlayerManager.Login()

**使用示例**:
```csharp
// 登录
LoginResult result = await APIManager.Instance.MiniGameLogin(
    LoginPlatform.WeChat,
    "wx_token_123"
);

if (result != null && result.uid != 0)
{
    // 登录成功
    Log.Info($"登录成功，UID={result.uid}");
}
else
{
    // 登录失败
    Log.Error("登录失败");
}
```

---

### SaveData(int uid, PlayerData data)

**签名**:
```csharp
public async ETTask<bool> SaveData(int uid, PlayerData data)
```

**职责**: 保存玩家数据 API

**参数**:
- `uid`: 玩家 ID
- `data`: 玩家数据

**返回**: `ETTask<bool>` - 是否成功

**核心逻辑**:
```
1. 构建 URL: baseurl + "MiniGameSaveData"
2. 构建参数：uid, data
3. 调用 HttpPost()
4. 返回结果
```

**调用者**: PlayerDataManager.LateUpdate()

---

### GetRankInfo(int uid)

**签名**:
```csharp
public async ETTask<RankList> GetRankInfo(int uid)
```

**职责**: 获取排行榜信息 API

**参数**:
- `uid`: 玩家 ID

**返回**: `ETTask<RankList>` - 排行榜数据

**调用者**: 排行榜 UI

---

### HttpGet/HttpPost (核心方法)

**签名** (多个重载):
```csharp
private async ETTask<bool> HttpGet(string url, Dictionary<string,string> param, int tryCount = 3, bool retryNotice = false)
private async ETTask<T> HttpGet<T>(string url, Dictionary<string,string> param, int tryCount = 3, bool retryNotice = false, int logResDetails = 3)
private async ETTask<bool> HttpPost(string url, Dictionary<string,object> param, int tryCount = 3, bool retryNotice = false)
private async ETTask<T> HttpPost<T>(string url, Dictionary<string,object> param, int tryCount = 3, bool retryNotice = false, int logResDetails = 3)
```

**职责**: HTTP GET/POST 请求核心方法

**参数**:
- `url`: 请求 URL
- `param`: 请求参数
- `tryCount`: 重试次数（默认 3）
- `retryNotice`: 失败时是否显示重试提示
- `logResDetails`: 日志详细程度（bitmask）

**核心逻辑**:
```
1. 添加 game 参数
2. 记录请求日志
3. 调用 HttpManager 发送请求
4. 记录响应日志
5. 检查响应：
   - code == 0 && status == true → 成功
   - 否则 → 失败
6. 如果失败且 tryCount > 0：
   - 递归重试
7. 如果失败且 retryNotice：
   - 显示错误提示框
   - 用户选择重试 → 递归调用
   - 用户选择退出 → BridgeHelper.Quit()
8. 显示错误 Toast
9. 返回结果
```

**日志详细程度**:
```csharp
// logResDetails bitmask
// bit 0: 记录响应详情
// bit 1: 记录请求详情
// 3 = 0b11: 记录请求和响应详情
// 2 = 0b10: 只记录请求详情
// 1 = 0b01: 只记录响应详情
// 0 = 0b00: 不记录详情
```

---

## 错误处理

### 重试机制

```csharp
// 自动重试 3 次
if (tryCount > 0)
{
    tryCount--;
    return await HttpGet(url, param, tryCount, retryNotice);
}
```

### 用户重试提示

```csharp
if (retryNotice)
{
    // 显示错误提示框
    await UIManager.Instance.OpenBox<UIMsgBoxWin, MsgBoxPara>(
        UIMsgBoxWin.PrefabPath,
        new MsgBoxPara
        {
            Content = res?.msg ?? "网络错误",
            CancelText = "退出",
            ConfirmText = "重试",
            CancelCallback = (win) => { BridgeHelper.Quit(); },
            ConfirmCallback = (win) => { /* 重试 */ }
        }
    );
}
```

---

## 阅读指引

### 建议的阅读顺序

1. **理解 API 管理器作用** - 为什么需要 APIManager
2. **看 MiniGameLogin** - 理解登录 API
3. **看 HttpGet/HttpPost** - 理解 HTTP 核心方法
4. **了解错误处理** - 理解重试机制

### 最值得学习的技术点

1. **重试机制**: tryCount 参数控制重试次数
2. **错误提示**: retryNotice 参数控制是否显示重试框
3. **日志控制**: logResDetails bitmask 控制日志详细程度
4. **泛型方法**: HttpGet<T>/HttpPost<T> 支持任意返回类型
5. **参数管理**: DictionaryComponent 自动释放

---

## 使用示例

### 示例 1: 登录 API

```csharp
// 调用登录 API
LoginResult result = await APIManager.Instance.MiniGameLogin(
    LoginPlatform.WeChat,
    "wx_token_123"
);

if (result != null)
{
    // 处理登录结果
    int uid = result.uid;
    string data = result.data;
}
```

### 示例 2: 保存数据

```csharp
// 保存玩家数据
bool success = await APIManager.Instance.SaveData(
    PlayerManager.Instance.Uid,
    PlayerDataManager.Instance.Data
);

if (success)
{
    Log.Info("数据保存成功");
}
else
{
    Log.Error("数据保存失败");
}
```

### 示例 3: 获取排行榜

```csharp
// 获取排行榜
RankList rankList = await APIManager.Instance.GetRankInfo(
    PlayerManager.Instance.Uid
);

// 显示排行榜
ShowRankList(rankList);
```

---

## 相关文档

- [HttpManager.cs.md](./HttpManager.cs.md) - HTTP 管理器
- [PlayerManager.cs.md](../Player/PlayerManager.cs.md) - 玩家管理器
- [PlayerDataManager.cs.md](../Player/PlayerDataManager.cs.md) - 玩家数据管理

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
