# AcceptAllCertificate.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|------|
| **文件名** | AcceptAllCertificate.cs |
| **路径** | Assets/Scripts/Mono/Module/Http/AcceptAllCertificate.cs |
| **所属模块** | 框架层 → Mono/Module/Http |
| **文件职责** | 提供接受所有 HTTPS 证书的处理器，用于开发环境调试 |

---

## 类/结构体说明

### AcceptAllCertificate

| 属性 | 说明 |
|------|------|
| **职责** | 继承 CertificateHandler，跳过 HTTPS 证书验证 |
| **泛型参数** | 无 |
| **继承关系** | 继承 `CertificateHandler` |
| **实现的接口** | 无 |

**设计模式**: 适配器模式

```csharp
// 使用示例（在 HttpManager 中）
var request = UnityWebRequest.Get(url);
request.certificateHandler = new AcceptAllCertificate();  // 跳过证书验证
```

---

## 方法说明

### ValidateCertificate

**签名**:
```csharp
protected override bool ValidateCertificate(byte[] certificateData)
```

**职责**: 验证 HTTPS 证书

**核心逻辑**:
```
始终返回 true（接受所有证书）
```

**参数**:
| 参数名 | 类型 | 说明 |
|--------|------|------|
| `certificateData` | `byte[]` | 证书数据（未使用） |

**返回值**: `bool` - 始终为 `true`

**调用者**: UnityWebRequest HTTPS 请求时

---

## 使用示例

### 示例 1: 在 HttpManager 中使用

```csharp
public class HttpManager
{
    private AcceptAllCertificate certificateHandler = new AcceptAllCertificate();
    
    public UnityWebRequest HttpGet(string url)
    {
        var request = UnityWebRequest.Get(url);
        request.certificateHandler = certificateHandler;  // 跳过证书验证
        request.SendWebRequest();
        return request;
    }
}
```

### 示例 2: 直接使用

```csharp
var request = UnityWebRequest.Get("https://self-signed.example.com/api");
request.certificateHandler = new AcceptAllCertificate();
await request.SendWebRequest();

if (request.result == UnityWebRequest.Result.Success)
{
    string response = request.downloadHandler.text;
}
```

---

## 设计要点

### 为什么需要跳过证书验证？

**开发环境**:
- 自签名证书（开发服务器）
- 过期证书（测试环境）
- 域名不匹配（本地测试）

**生产环境**:
- ⚠️ **不建议使用**
- 应实现正确的证书验证

### 安全风险

```csharp
protected override bool ValidateCertificate(byte[] certificateData)
{
    return true;  // ⚠️ 接受所有证书（包括伪造的）
}
```

**风险**:
- 中间人攻击（MITM）
- 数据泄露
- 证书伪造

**建议**:
- 开发环境：可以使用
- 生产环境：应实现正确的证书验证
- 配置开关：通过编译符号或配置控制

### 生产环境替代方案

```csharp
#if DEVELOPMENT
    request.certificateHandler = new AcceptAllCertificate();
#else
    request.certificateHandler = null;  // 使用默认验证
#endif
```

或者实现自定义验证：

```csharp
public class SecureCertificateHandler : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        // 验证证书颁发机构
        // 验证证书有效期
        // 验证域名匹配
        return true;  // 仅当验证通过时返回 true
    }
}
```

---

## 相关文档

- [HttpManager.cs.md](./HttpManager.cs.md) - HTTP 管理器（使用 AcceptAllCertificate）
- [UnityWebRequest 文档](https://docs.unity3d.com/ScriptReference/Networking.CertificateHandler.html)

---

*文档生成时间：2026-02-28 | OpenClaw AI 助手*
