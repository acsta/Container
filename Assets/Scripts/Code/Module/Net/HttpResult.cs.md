# HttpResult.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | HttpResult.cs |
| **路径** | Assets/Scripts/Code/Module/Net/HttpResult.cs |
| **所属模块** | 框架层 → Code/Module/Net |
| **文件职责** | HTTP 响应结果数据结构 |

---

## 类/结构体说明

### HttpResult

| 属性 | 说明 |
|------|------|
| **职责** | 存储无数据 HTTP 响应结果 |
| **泛型参数** | 无 |
| **继承关系** | 无 |

```csharp
public class HttpResult
{
    public string msg;
    public int code;
    public bool status;
}
```

### HttpResult<T>

| 属性 | 说明 |
|------|------|
| **职责** | 存储带数据的 HTTP 响应结果（泛型） |
| **泛型参数** | `T` - 响应数据类型 |
| **继承关系** | 无 |

```csharp
public class HttpResult<T>
{
    public string msg;
    public int code;
    public bool status;
    public T data;
}
```

---

## 字段说明

### HttpResult 字段

| 字段 | 类型 | 说明 |
|------|------|------|
| `msg` | `string` | 响应消息（错误信息或成功提示） |
| `code` | `int` | 响应码（0=成功，非 0=错误码） |
| `status` | `bool` | 是否成功（true=成功，false=失败） |

### HttpResult<T> 字段

| 字段 | 类型 | 说明 |
|------|------|------|
| `msg` | `string` | 响应消息 |
| `code` | `int` | 响应码 |
| `status` | `bool` | 是否成功 |
| `data` | `T` | 响应数据（泛型） |

---

## 使用示例

### 示例 1: 无数据响应

```csharp
// API 调用（无返回值）
HttpResult result = await HttpManager.Instance.HttpGetResult<HttpResult>(url, null, param);

if (result != null && result.code == 0 && result.status)
{
    Log.Info("请求成功");
}
else
{
    Log.Error($"请求失败：{result?.msg}");
}
```

### 示例 2: 带数据响应

```csharp
// API 调用（有返回值）
HttpResult<PlayerData> result = await HttpManager.Instance.HttpGetResult<HttpResult<PlayerData>>(url, null, param);

if (result != null && result.code == 0 && result.status)
{
    PlayerData data = result.data;
    Log.Info($"玩家昵称：{data.NickName}");
}
else
{
    Log.Error($"请求失败：{result?.msg}");
}
```

### 示例 3: APIManager 中的使用

```csharp
// APIManager 内部使用
private async ETTask<bool> HttpGet(string url, Dictionary<string, string> param)
{
    var res = await HttpManager.Instance.HttpGetResult<HttpResult>(url, null, param);
    
    if (res != null && res.code == 0 && res.status)
    {
        return true;  // 成功
    }
    
    // 失败处理
    Log.Error(res?.msg);
    return false;
}

private async ETTask<T> HttpGet<T>(string url, Dictionary<string, string> param)
{
    var res = await HttpManager.Instance.HttpGetResult<HttpResult<T>>(url, null, param);
    
    if (res != null && res.code == 0 && res.status)
    {
        return res.data;  // 返回数据
    }
    
    // 失败处理
    Log.Error(res?.msg);
    return default(T);
}
```

---

## HTTP 响应格式

### 无数据响应

```json
{
  "code": 0,
  "status": true,
  "msg": "操作成功"
}
```

### 带数据响应

```json
{
  "code": 0,
  "status": true,
  "msg": "",
  "data": {
    "NickName": "玩家 1",
    "Money": "1000",
    ...
  }
}
```

---

## 响应码约定

| code | status | 说明 |
|------|--------|------|
| 0 | true | 成功 |
| 非 0 | false | 失败（具体错误码由服务器定义） |

---

## 相关文档

- [APIManager.cs.md](./APIManager.cs.md) - API 管理器
- [LoginResult.cs.md](./LoginResult.cs.md) - 登录响应
- [HttpManager.cs.md](../Http/HttpManager.cs.md) - HTTP 管理器

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
