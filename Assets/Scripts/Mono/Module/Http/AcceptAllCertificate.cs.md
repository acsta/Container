# AcceptAllCertificate.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | AcceptAllCertificate.cs |
| **路径** | Assets/Scripts/Mono/Module/Http/AcceptAllCertificate.cs |
| **所属模块** | 框架层 → Mono/Module/Http |
| **命名空间** | `TaoTie` |
| **文件职责** | 提供接受所有 SSL 证书的证书处理器（用于开发/测试环境） |

---

## 类说明

### AcceptAllCertificate

| 属性 | 说明 |
|------|------|
| **职责** | 继承 `CertificateHandler`，重写验证方法始终返回 true |
| **继承关系** | `CertificateHandler` (Unity Networking) |
| **安全警告** | ⚠️ 仅用于开发/测试环境，生产环境应使用正式证书 |

**设计模式**: 证书处理器模式

---

## 方法说明

### ValidateCertificate()

**签名**:
```csharp
protected override bool ValidateCertificate(byte[] certificateData)
```

**职责**: 验证 SSL 证书

**核心逻辑**:
```
1. 始终返回 true（接受所有证书）
```

**安全警告**: 此实现会接受任何 SSL 证书，包括自签名证书和过期证书，存在中间人攻击风险。

---

## 使用示例

### HttpManager 中的使用

```csharp
public class HttpManager
{
    private AcceptAllCertificate certificateHandler = new AcceptAllCertificate();
    
    public UnityWebRequest HttpGet(string url, ...)
    {
        var request = UnityWebRequest.Get(url);
        request.certificateHandler = certificateHandler; // 接受所有证书
        request.SendWebRequest();
        return request;
    }
}
```

---

## 安全建议

| 环境 | 建议 |
|------|------|
| **开发/测试** | 可使用 AcceptAllCertificate |
| **生产环境** | 应使用正式 SSL 证书，自定义 CertificateHandler 验证 |

### 生产环境替代方案

```csharp
public class SecureCertificateHandler : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        // 实现正式的证书验证逻辑
        // 例如：验证证书链、检查有效期、验证域名等
        return true; // 仅当验证通过时返回 true
    }
}
```

---

## 相关文档

- [HttpManager.cs.md](./HttpManager.cs.md) - HTTP 管理器（使用 AcceptAllCertificate）

---

*文档生成时间：2026-03-02 | OpenClaw AI 助手*
