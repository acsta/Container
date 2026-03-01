# CDNConfigHelper.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | CDNConfigHelper.cs |
| **路径** | Assets/Scripts/Mono/Helper/CDNConfigHelper.cs |
| **所属模块** | Mono/Helper |
| **文件职责** | CDN 配置扩展方法，处理渠道重命名逻辑 |

---

## 类说明

### CDNConfigHelper

| 属性 | 说明 |
|------|------|
| **职责** | 扩展 CDNConfig，获取实际渠道名 (处理重命名) |
| **类型** | `static class` |

---

## 方法说明

### GetChannel(this CDNConfig self)

```csharp
public static string GetChannel(this CDNConfig self)
```

**职责**: 获取 CDN 渠道名

**逻辑**:
- 非 WebGL 平台：检查是否在 RenameList 中，不在则返回 "common"
- WebGL 平台：直接返回原渠道名

---

## 使用示例

```csharp
CDNConfig config = CDNConfigCategory.Instance.Get(channelId);
string channel = config.GetChannel(); // 获取实际渠道名
```

---

## 相关文档

- [Define.cs.md](../Define.cs.md) - 全局定义 (RenameList)
- [CDNConfig.cs.md](../../Config/CDNConfig.cs.md) - CDN 配置

---

*文档生成时间：2026-03-02 | OpenClaw AI 助手*
