# HttpManager.cs 注解文档

## 文件基本信息
- **路径**: Assets/Scripts/Mono/Module/Http/HttpManager.cs
- **职责**: HTTP 请求管理器，封装 UnityWebRequest 提供异步 HTTP 请求

## 方法
### HttpGet
```csharp
public UnityWebRequest HttpGet(string url, Dictionary<string, string> headers, Dictionary<string, string> param, int timeout)
```
发送 GET 请求

### HttpPost
```csharp
public UnityWebRequest HttpPost(string url, Dictionary<string, string> headers, Dictionary<string, object> param, int timeout)
```
发送 POST 请求（JSON body）

### HttpGetImageOnline
```csharp
public async ETTask<Texture2D> HttpGetImageOnline(string url, bool local, Dictionary<string, string> headers, int timeout, ETCancellationToken cancelToken)
```
异步下载图片，支持本地缓存

### HttpGetSoundOnline
```csharp
public async ETTask<AudioClip> HttpGetSoundOnline(string url, bool local, Dictionary<string, string> headers, int timeout, ETCancellationToken cancelToken)
```
异步下载音频，支持本地缓存

### HttpGetResult<T>
```csharp
public async ETTask<T> HttpGetResult<T>(string url, Dictionary<string, string> headers, Dictionary<string, string> param, int timeout, ETCancellationToken cancelToken)
```
异步 GET 请求并解析 JSON 结果

### HttpPostResult<T>
```csharp
public async ETTask<T> HttpPostResult<T>(string url, Dictionary<string, string> headers, Dictionary<string, object> param, int timeout, ETCancellationToken cancelToken)
```
异步 POST 请求并解析 JSON 结果

## 使用示例
```csharp
// GET 请求
var response = await HttpManager.Instance.HttpGetResult<ApiResponse>(
    "https://api.example.com/data",
    param: new Dictionary<string, string> { { "page", "1" } }
);

// POST 请求
var result = await HttpManager.Instance.HttpPostResult<LoginResponse>(
    "https://api.example.com/login",
    param: new Dictionary<string, object> { { "username", "test" }, { "password", "123" } }
);

// 下载图片
Texture2D texture = await HttpManager.Instance.HttpGetImageOnline(url, local: true);
```
