# APIManager.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | APIManager.cs |
| **路径** | Assets/Scripts/Code/Module/Net/APIManager.cs |
| **所属模块** | 框架层 → Code/Module/Net |
| **文件职责** | API 管理器，封装 HTTP 请求和错误处理 |

---

## 类/结构体说明

### APIManager

| 属性 | 说明 |
|------|------|
| **职责** | 封装游戏服务器 API 请求，支持重试、错误提示、登录态检测 |
| **泛型参数** | 无 |
| **继承关系** | 无 |
| **实现的接口** | `IManager` |

```csharp
public class APIManager : IManager
{
    // API 管理器
}
```

---

## 字段与属性

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `Instance` | `APIManager` | `public static` | 单例实例 |
| `GAME` | `string` | `const` | 游戏标识（"p4"） |
| `platform` | `LoginPlatform` | `private` | 登录平台 |
| `baseurl` | `string` | `private` | 服务器基础 URL |

---

## 方法说明（按重要程度排序）

### Init() / Destroy()

**签名**:
```csharp
public void Init()
public void Destroy()
```

**职责**: 初始化和销毁

**核心逻辑**:
```
// Init:
1. 设置 Instance = this
2. 根据 Debug 模式设置 baseurl：
   - Debug → http://192.168.0.6:29527/
   - Release → https://wgsh.hxwgame.cn/dish1/

// Destroy:
1. 清空 Instance
```

**调用者**: `ManagerProvider.Init()`

---

### HttpGet(string url, Dictionary<string, string> param, tryCount, retryNotice)

**签名**:
```csharp
private async ETTask<bool> HttpGet(string url, Dictionary<string, string> param, 
    int tryCount = 3, bool retryNotice = false)
```

**职责**: 发送 HTTP GET 请求（无返回值）

**参数**:
- `url`: 请求 URL
- `param`: 请求参数
- `tryCount`: 重试次数（默认 3 次）
- `retryNotice`: 失败是否显示重试提示

**返回**: true=成功，false=失败

**核心逻辑**:
```
1. 添加 game 参数
2. 记录请求日志
3. 发送 HTTP 请求
4. 记录响应日志
5. 检查响应：
   - code == 0 && status == true → 成功
6. 如果失败且还有重试次数：
   - 递归调用 HttpGet
7. 如果需要重试提示：
   - 显示错误提示框
   - 用户选择重试 → 递归调用
   - 用户选择退出 → 退出游戏
8. 显示错误 Toast
9. 返回 false
```

**错误处理**:
```csharp
if (retryNotice)
{
    ETTask<bool> task = ETTask<bool>.Create(true);
    string content = res?.msg ?? I18NManager.Instance.I18NGetText(I18NKey.Net_Error);
    
    await UIManager.Instance.OpenBox<UIMsgBoxWin, MsgBoxPara>(...);
    
    if (await task)
    {
        return await HttpGet(url, param, 3, retryNotice);  // 重试
    }
}
```

**调用者**: 各种 API 方法

---

### HttpGet<T>(string url, Dictionary<string, string> param, tryCount, retryNotice, logResDetails)

**签名**:
```csharp
private async ETTask<T> HttpGet<T>(string url, Dictionary<string, string> param, 
    int tryCount = 3, bool retryNotice = false, int logResDetails = 3)
```

**职责**: 发送 HTTP GET 请求（带返回值）

**参数**:
- `url`: 请求 URL
- `param`: 请求参数
- `tryCount`: 重试次数
- `retryNotice`: 失败是否显示重试提示
- `logResDetails`: 日志详细程度（位掩码）

**返回**: 响应数据 T

**核心逻辑**:
```
1. 添加 game 参数
2. 记录请求日志（根据 logResDetails）
3. 发送 HTTP 请求
4. 记录响应日志
5. 检查响应：
   - code == 0 && status == true → 返回 data
6. 如果失败且还有重试次数：
   - 递归调用
7. 如果需要重试提示：
   - 显示错误提示框
8. 显示错误 Toast
9. 返回 default(T)
```

**日志详细程度**:
```csharp
// logResDetails 位掩码:
// bit 0 (1): 记录响应详情
// bit 1 (2): 记录请求详情
// 3 = 两者都记录
// 2 = 只记录请求
// 1 = 只记录响应
// 0 = 都不记录

Log.Info((logResDetails & 2) != 0 ? JsonHelper.ToJson(param) : "hide details");
Log.Info((logResDetails & 1) != 0 ? JsonHelper.ToJson(res) : "hide details");
```

**调用者**: 各种 API 方法

---

### 具体 API 方法

APIManager 封装了多个具体 API 方法：

| 方法 | 说明 |
|------|------|
| `Login(string account, string psd)` | 账号密码登录 |
| `CodeLogin(string phone, string code)` | 验证码登录 |
| `SendCode(string phone)` | 发送验证码 |
| `Register(string account, string psd)` | 注册账号 |
| `CreateRole(string nickName)` | 创建角色 |
| `GetPlayerData()` | 获取玩家数据 |
| `SavePlayerData(PlayerData data)` | 保存玩家数据 |
| `SubmitAuctionReport(AuctionReport report)` | 提交拍卖报告 |
| `GetRankList(int page)` | 获取排行榜 |
| `...` | 更多 API |

---

## 阅读指引

### 建议的阅读顺序

1. **理解管理器作用** - APIManager 封装什么
2. **看 HttpGet** - 理解 HTTP 请求
3. **看错误处理** - 理解重试机制
4. **了解具体 API** - 查看可用接口

### 最值得学习的技术点

1. **HTTP 封装**: 统一处理请求/响应
2. **重试机制**: 自动重试 + 用户选择重试
3. **错误提示**: Toast + 提示框
4. **日志控制**: logResDetails 位掩码
5. **泛型支持**: HttpGet<T> 返回任意类型

---

## 使用示例

### 示例 1: 登录

```csharp
// 账号密码登录
bool success = await APIManager.Instance.Login("player1", "password123");
if (success)
{
    Log.Info("登录成功");
}

// 验证码登录
bool success = await APIManager.Instance.CodeLogin("13800138000", "123456");
```

### 示例 2: 获取玩家数据

```csharp
// 获取玩家数据
PlayerData data = await APIManager.Instance.GetPlayerData();
if (data != null)
{
    Log.Info($"玩家昵称：{data.NickName}");
    Log.Info($"金钱：{data.Money}");
}
```

### 示例 3: 提交拍卖报告

```csharp
// 创建拍卖报告
AuctionReport report = new AuctionReport();
report.Index = 1;
report.ContainerId = 101;
report.Type = ReportType.Self;
report.LastAuctionPriceStr = "1000";

// 提交报告
bool success = await APIManager.Instance.SubmitAuctionReport(report);
if (success)
{
    Log.Info("拍卖报告提交成功");
}
```

### 示例 4: 获取排行榜

```csharp
// 获取第 1 页排行榜
RankList rankList = await APIManager.Instance.GetRankList(page: 1);
if (rankList != null)
{
    foreach (var info in rankList.List)
    {
        Log.Info($"排名{info.Rank}: {info.NickName} - {info.Score}");
    }
}
```

---

## HTTP 响应格式

```json
{
  "code": 0,
  "status": true,
  "msg": "",
  "data": { ... }
}
```

**字段说明**:
- `code`: 错误码（0=成功）
- `status`: 是否成功
- `msg`: 错误信息
- `data`: 响应数据（泛型 T）

---

## 相关文档

- [HttpManager.cs.md](../Http/HttpManager.cs.md) - HTTP 管理器
- [HttpResult.cs.md](./HttpResult.cs.md) - HTTP 响应结果
- [LoginPlatform.cs.md](./LoginPlatform.cs.md) - 登录平台

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
