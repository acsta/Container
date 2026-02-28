# HttpManager.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|------|
| **文件名** | HttpManager.cs |
| **路径** | Assets/Scripts/Mono/Module/Http/HttpManager.cs |
| **所属模块** | 框架层 → Mono/Module/Http |
| **文件职责** | 提供 HTTP 请求管理器，封装 UnityWebRequest 支持 GET/POST/图片/音频下载 |

---

## 类/结构体说明

### HttpManager

| 属性 | 说明 |
|------|------|
| **职责** | 单例类，提供 HTTP 请求、图片下载、音频下载等功能 |
| **泛型参数** | 无 |
| **继承关系** | 无 |
| **实现的接口** | 无 |

**设计模式**: 单例模式 + 外观模式

```csharp
// GET 请求
var result = await HttpManager.Instance.HttpGetResult<UserInfo>("https://api.example.com/user");

// POST 请求
var response = await HttpManager.Instance.HttpPostResult<LoginResponse>("https://api.example.com/login", param: loginData);

// 下载图片
Texture2D texture = await HttpManager.Instance.HttpGetImageOnline(url, local: true);
```

---

## 常量

| 常量 | 类型 | 值 | 说明 |
|------|------|-----|------|
| `DEFAULT_TIMEOUT` | `int` | 10 | 默认请求超时时间（秒） |

---

## 字段与属性

### Instance

| 属性 | 值 |
|------|------|
| **类型** | `HttpManager` |
| **访问级别** | `public static` |
| **说明** | 单例实例，全局访问点 |

---

### certificateHandler

| 属性 | 值 |
|------|------|
| **类型** | `AcceptAllCertificate` |
| **访问级别** | `private` |
| **说明** | 证书处理器（接受所有证书） |

**用途**: 开发环境跳过 HTTPS 证书验证

---

## 方法说明

### HttpGet

**签名**:
```csharp
public UnityWebRequest HttpGet(string url, Dictionary<string, string> headers = null,
    Dictionary<string, string> param = null, int timeout = DEFAULT_TIMEOUT)
```

**职责**: 发送 HTTP GET 请求

**核心逻辑**:
```
1. 转换参数为查询字符串
2. 创建 UnityWebRequest.Get
3. 设置证书处理器、超时、请求头
4. 发送请求
5. 返回 UnityWebRequest
```

**参数**:
| 参数名 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| `url` | `string` | - | 请求 URL |
| `headers` | `Dictionary<string, string>` | null | 请求头 |
| `param` | `Dictionary<string, string>` | null | 查询参数 |
| `timeout` | `int` | 10 | 超时时间（秒） |

**返回值**: `UnityWebRequest` - 请求对象（需自行等待完成）

**使用示例**:
```csharp
var request = HttpManager.Instance.HttpGet("https://api.example.com/data");
while (!request.isDone)
{
    await TimerManager.Instance.WaitAsync(1);
}

if (request.result == UnityWebRequest.Result.Success)
{
    string response = request.downloadHandler.text;
}
```

---

### HttpPost

**签名**:
```csharp
public UnityWebRequest HttpPost(string url, Dictionary<string, string> headers = null,
    Dictionary<string, object> param = null, int timeout = DEFAULT_TIMEOUT)
```

**职责**: 发送 HTTP POST 请求（JSON 格式）

**核心逻辑**:
```
1. 将参数序列化为 JSON
2. 创建 UnityWebRequest POST
3. 设置请求体、请求头、超时
4. 发送请求
5. 返回 UnityWebRequest
```

**使用示例**:
```csharp
var loginData = new Dictionary<string, object>
{
    { "username", "player1" },
    { "password", "secret" }
};

var request = HttpManager.Instance.HttpPost("https://api.example.com/login", param: loginData);
```

---

### HttpGetResult

**签名**:
```csharp
public async ETTask<string> HttpGetResult(string url, Dictionary<string, string> headers = null,
    Dictionary<string, string> param = null, int timeout = DEFAULT_TIMEOUT, ETCancellationToken cancelToken = null)
```

**职责**: 发送 GET 请求并返回响应文本

**核心逻辑**:
```
1. 调用 HttpGet
2. 等待请求完成（支持取消）
3. 检查请求结果
4. 返回响应文本或 null
```

**返回值**: `ETTask<string>` - 响应文本

**使用示例**:
```csharp
string response = await HttpManager.Instance.HttpGetResult("https://api.example.com/data");
```

---

### HttpGetResult<T>

**签名**:
```csharp
public async ETTask<T> HttpGetResult<T>(string url, Dictionary<string, string> headers = null,
    Dictionary<string, string> param = null, int timeout = DEFAULT_TIMEOUT, ETCancellationToken cancelToken = null) where T : class
```

**职责**: 发送 GET 请求并反序列化为指定类型

**返回值**: `ETTask<T>` - 反序列化后的对象

**使用示例**:
```csharp
UserInfo userInfo = await HttpManager.Instance.HttpGetResult<UserInfo>("https://api.example.com/user/123");
Debug.Log($"用户名：{userInfo.Name}");
```

---

### HttpPostResult<T>

**签名**:
```csharp
public async ETTask<T> HttpPostResult<T>(string url, Dictionary<string, string> headers = null,
    Dictionary<string, object> param = null, int timeout = DEFAULT_TIMEOUT, ETCancellationToken cancelToken = null) where T : class
```

**职责**: 发送 POST 请求并反序列化为指定类型

**使用示例**:
```csharp
var loginData = new Dictionary<string, object>
{
    { "username", "player1" },
    { "password", "secret" }
};

LoginResponse response = await HttpManager.Instance.HttpPostResult<LoginResponse>(
    "https://api.example.com/login",
    param: loginData
);

Debug.Log($"登录成功，Token: {response.Token}");
```

---

### HttpGetImageOnline

**签名**:
```csharp
public async ETTask<Texture2D> HttpGetImageOnline(string url, bool local, Dictionary<string, string> headers = null,
    int timeout = DEFAULT_TIMEOUT, ETCancellationToken cancelToken = null)
```

**职责**: 下载图片，支持本地缓存

**核心逻辑**:
```
1. 验证 URL 有效性
2. 如果 local=true，检查本地缓存
3. 下载图片（使用 UnityWebRequestTexture）
4. 返回 Texture2D
```

**参数**:
| 参数名 | 类型 | 说明 |
|--------|------|------|
| `url` | `string` | 图片 URL |
| `local` | `bool` | 是否使用本地缓存 |
| `headers` | `Dictionary<string, string>` | 请求头 |
| `timeout` | `int` | 超时时间 |
| `cancelToken` | `ETCancellationToken` | 取消令牌 |

**使用示例**:
```csharp
// 下载头像（带缓存）
Texture2D avatar = await HttpManager.Instance.HttpGetImageOnline(avatarUrl, local: true);
image.sprite = Sprite.Create(avatar, new Rect(0, 0, avatar.width, avatar.height), Vector2.zero);
```

---

### HttpGetSoundOnline

**签名**:
```csharp
public async ETTask<AudioClip> HttpGetSoundOnline(string url, bool local, Dictionary<string, string> headers = null,
    int timeout = DEFAULT_TIMEOUT, ETCancellationToken cancelToken = null)
```

**职责**: 下载音频，支持本地缓存

**返回值**: `ETTask<AudioClip>` - 音频剪辑

**使用示例**:
```csharp
// 下载背景音乐
AudioClip bgm = await HttpManager.Instance.HttpGetSoundOnline(bgmUrl, local: true);
audioSource.clip = bgm;
audioSource.Play();
```

---

### LocalFile

**签名**:
```csharp
public string LocalFile(string url, string dir = "downloadimage", string extends = ".png")
```

**职责**: 根据 URL 生成本地缓存路径（使用 MD5 哈希）

**核心逻辑**:
```
1. 计算 URL 的 MD5 哈希
2. 生成路径：persistentDataPath/dir/md5Hash.ext
3. 创建目录（如果不存在）
4. 返回路径
```

**使用示例**:
```csharp
string url = "https://example.com/image.png";
string localPath = HttpManager.Instance.LocalFile(url);
// 返回：/persistent/data/downloadimage/a1b2c3d4e5f6.png
```

---

## 使用示例

### 示例 1: 用户登录

```csharp
public async ETTask Login(string username, string password)
{
    var loginData = new Dictionary<string, object>
    {
        { "username", username },
        { "password", password }
    };
    
    try
    {
        LoginResponse response = await HttpManager.Instance.HttpPostResult<LoginResponse>(
            "https://api.example.com/login",
            param: loginData
        );
        
        if (response != null)
        {
            Debug.Log($"登录成功，Token: {response.Token}");
            PlayerPrefs.SetString("Token", response.Token);
        }
    }
    catch (Exception ex)
    {
        Log.Error($"登录失败：{ex.Message}");
    }
}
```

### 示例 2: 获取用户信息

```csharp
public async ETTask<UserInfo> GetUserInfo()
{
    string token = PlayerPrefs.GetString("Token");
    var headers = new Dictionary<string, string>
    {
        { "Authorization", $"Bearer {token}" }
    };
    
    UserInfo userInfo = await HttpManager.Instance.HttpGetResult<UserInfo>(
        "https://api.example.com/user",
        headers: headers
    );
    
    return userInfo;
}
```

### 示例 3: 下载并缓存图片

```csharp
public async ETTask LoadAvatar(string url, RawImage image)
{
    // 显示加载占位图
    image.texture = loadingTexture;
    
    // 下载图片（带缓存）
    Texture2D avatar = await HttpManager.Instance.HttpGetImageOnline(url, local: true);
    
    if (avatar != null)
    {
        image.texture = avatar;
    }
    else
    {
        // 显示错误占位图
        image.texture = errorTexture;
    }
}
```

### 示例 4: 带取消的请求

```csharp
ETCancellationToken cancelToken = new ETCancellationToken();

// 下载图片
Task<Texture2D> downloadTask = HttpManager.Instance.HttpGetImageOnline(url, local: true, cancelToken: cancelToken);

// 如果用户取消
if (userClickedCancel)
{
    cancelToken.Cancel();
}
```

---

## 设计要点

### 为什么返回 UnityWebRequest？

部分方法（HttpGet/HttpPost）返回 `UnityWebRequest` 而非直接返回结果：

**原因**:
- 灵活性：调用者可以自行处理进度、取消等
- 兼容性：支持旧代码风格
- 控制力：调用者可以访问响应头、状态码等详细信息

### 本地缓存策略

```csharp
public string LocalFile(string url, string dir = "downloadimage", string extends = ".png")
{
    // 使用 URL 的 MD5 作为文件名
    byte[] input = Encoding.Default.GetBytes(url.Trim());
    byte[] output = MD5.Create().ComputeHash(input);
    string md5URLString = BitConverter.ToString(output).Replace("-", "");
    
    string savePath = $"{persistentDataPath}/{dir}/{md5URLString}{extends}";
    return savePath;
}
```

**优势**:
- 相同 URL 只下载一次
- 离线时可加载缓存
- 使用 MD5 确保文件名唯一性

### 证书验证

```csharp
private AcceptAllCertificate certificateHandler = new AcceptAllCertificate();
```

**说明**: 接受所有 HTTPS 证书（开发环境方便调试）

**生产环境建议**: 实现正确的证书验证逻辑

---

## 相关文档

- [AcceptAllCertificate.cs.md](./AcceptAllCertificate.cs.md) - 证书处理器
- [JsonHelper.cs.md](../Helper/JsonHelper.cs.md) - JSON 序列化工具
- [UnityWebRequest 文档](https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.html)

---

*文档生成时间：2026-02-28 | OpenClaw AI 助手*
