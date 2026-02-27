# LoginResult.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | LoginResult.cs |
| **路径** | Assets/Scripts/Code/Module/Net/LoginResult.cs |
| **所属模块** | 框架层 → Code/Module/Net |
| **文件职责** | 登录响应数据结构 |

---

## 类说明

### LoginResult

| 属性 | 说明 |
|------|------|
| **职责** | 存储登录响应数据 |
| **类型** | class |

```csharp
public class LoginResult
{
    public string Token;
    public string Uid;
}
```

---

## 字段说明

| 字段 | 类型 | 说明 |
|------|------|------|
| `Token` | `string` | 登录令牌（用于后续 API 请求） |
| `Uid` | `string` | 用户 ID（唯一标识） |

---

## 使用示例

### 示例 1: 登录响应

```csharp
// 登录
LoginResult result = await APIManager.Instance.Login(account, password);

if (result != null)
{
    // 保存 Token
    CacheManager.Instance.SetString(CacheKeys.LastToken, result.Token);
    
    // 保存用户 ID
    CacheManager.Instance.SetString(CacheKeys.Account, result.Uid);
    
    Log.Info($"登录成功，用户 ID: {result.Uid}");
}
```

### 示例 2: 使用 Token

```csharp
// 获取 Token
string token = CacheManager.Instance.GetString(CacheKeys.LastToken);

// 在 API 请求中使用
Dictionary<string, string> param = new Dictionary<string, string>();
param["token"] = token;
param["uid"] = CacheManager.Instance.GetString(CacheKeys.Account);

var data = await APIManager.Instance.HttpGet<PlayerData>(url, param);
```

---

## 登录流程

```
输入账号密码
    ↓
调用 APIManager.Login()
    ↓
服务器验证
    ↓
返回 LoginResult
    ↓
保存 Token 和 Uid
    ↓
加载玩家数据
    ↓
进入游戏
```

---

## 相关文档

- [APIManager.cs.md](./APIManager.cs.md) - API 管理器
- [HttpResult.cs.md](./HttpResult.cs.md) - HTTP 响应结果
- [LoginPlatform.cs.md](./LoginPlatform.cs.md) - 登录平台

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
