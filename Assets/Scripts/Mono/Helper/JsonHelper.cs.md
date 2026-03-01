# JsonHelper.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | JsonHelper.cs |
| **路径** | Assets/Scripts/Mono/Helper/JsonHelper.cs |
| **所属模块** | Mono/Helper |
| **文件职责** | JSON 序列化/反序列化工具类 (基于 LitJson) |

---

## 类说明

### JsonHelper

| 属性 | 说明 |
|------|------|
| **职责** | 封装 LitJson 的序列化/反序列化操作 |
| **类型** | `static class` |

---

## 方法说明

### ToJson<T>(T message)

```csharp
public static string ToJson<T>(T message)
```

**职责**: 对象转 JSON 字符串

---

### FromJson<T>(string json)

```csharp
public static T FromJson<T>(string json)
```

**职责**: JSON 字符串转对象

---

### FromJson(Type type, string json)

```csharp
public static object FromJson(Type type, string json)
```

**职责**: JSON 字符串转指定类型对象

---

### TryFromJson<T>(string json, out T res)

```csharp
public static bool TryFromJson<T>(string json, out T res)
```

**职责**: 尝试反序列化，失败不抛异常

**返回**: 成功返回 true，失败返回 false

---

## 使用示例

```csharp
// 序列化
string json = JsonHelper.ToJson(playerData);

// 反序列化
PlayerData data = JsonHelper.FromJson<PlayerData>(json);

// 安全反序列化
if (JsonHelper.TryFromJson<PlayerData>(json, out var result))
{
    // 使用 result
}
```

---

## 相关文档

- [LitJson 文档](https://github.com/LitJSON/litjson)

---

*文档生成时间：2026-03-02 | OpenClaw AI 助手*
