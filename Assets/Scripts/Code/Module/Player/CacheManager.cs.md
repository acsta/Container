# CacheManager.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | CacheManager.cs |
| **路径** | Assets/Scripts/Code/Module/Player/CacheManager.cs |
| **所属模块** | 框架层 → Code/Module/Player |
| **文件职责** | 缓存管理器，封装 PlayerPrefs 和平台存储 API |

---

## 类/结构体说明

### CacheManager

| 属性 | 说明 |
|------|------|
| **职责** | 统一管理玩家数据缓存，支持多平台存储 API |
| **泛型参数** | 无 |
| **继承关系** | 无 |
| **实现的接口** | `IManager` |

```csharp
public class CacheManager : IManager
{
    // 缓存管理器
}
```

---

## 字段与属性

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `Instance` | `CacheManager` | `public static` | 单例实例 |
| `cacheObj` | `Dictionary<string, object>` | `private` | 内存缓存对象 |

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
1. 创建 cacheObj 字典
2. 设置 Instance = this

// Destroy:
1. 调用 Save() 保存数据
2. 清空 Instance
```

**调用者**: `ManagerProvider.Init()`

---

### GetString / SetString

**签名**:
```csharp
public string GetString(string key, string defaultValue = null)
public void SetString(string key, string value)
```

**职责**: 获取/设置字符串

**核心逻辑**:
```
// GetString:
1. 如果是轻游平台，调用 QG.StorageGetStringSync
2. 否则调用 PlayerPrefs.GetString
3. 返回结果

// SetString:
1. 如果是轻游平台，调用 QG.StorageSetStringSync
2. 否则调用 PlayerPrefs.SetString
```

**平台适配**:
```csharp
#if UNITY_WEBGL_QG
    return QGMiniGame.QG.StorageGetStringSync(key, defaultValue);
#else
    return PlayerPrefs.GetString(key, defaultValue);
#endif
```

**调用者**: 需要存储字符串的代码

---

### GetInt / SetInt

**签名**:
```csharp
public int GetInt(string key, int defaultValue = 0)
public void SetInt(string key, int value)
```

**职责**: 获取/设置整数

**与 GetString 的区别**: 使用平台原生整数存储

---

### GetLong / SetLong

**签名**:
```csharp
public long GetLong(string key, long defaultValue = 0)
public void SetLong(string key, long value)
```

**职责**: 获取/设置长整数

**核心逻辑**:
```
// GetLong:
1. 如果是轻游平台，获取字符串后解析
2. 否则调用 PlayerPrefs.GetString
3. 尝试解析为 long
4. 如果解析失败，返回 defaultValue
```

**注意**: PlayerPrefs 不直接支持 long，需要转换为字符串存储

---

### GetValue<T> / SetValue<T>

**签名**:
```csharp
public T GetValue<T>(string key) where T : class
public void SetValue<T>(string key, T value) where T : class
```

**职责**: 获取/设置对象（JSON 序列化）

**核心逻辑**:
```
// GetValue:
1. 检查内存缓存 cacheObj
2. 如果有，直接返回
3. 否则从存储读取 JSON 字符串
4. 反序列化为对象
5. 保存到内存缓存
6. 返回对象

// SetValue:
1. 保存到内存缓存
2. 序列化为 JSON
3. 存储到平台存储
```

**内存缓存**:
```csharp
// 先检查内存缓存
if (cacheObj.TryGetValue(key, out var data))
{
    return data as T;
}

// 从存储读取
var jStr = PlayerPrefs.GetString(key, null);
var res = JsonHelper.FromJson<T>(jStr);
cacheObj[key] = res;  // 保存到内存缓存
return res;
```

**调用者**: 需要存储复杂对象的代码

**使用示例**:
```csharp
// 存储玩家数据
PlayerData data = GetPlayerData();
CacheManager.Instance.SetValue("PlayerData", data);

// 读取玩家数据
PlayerData data = CacheManager.Instance.GetValue<PlayerData>("PlayerData");
```

---

### Save()

**签名**:
```csharp
public void Save()
```

**职责**: 保存数据到磁盘

**核心逻辑**:
```
1. 如果是轻游平台，不需要保存（自动保存）
2. 否则调用 PlayerPrefs.Save()
```

**调用者**: `Destroy()`, 需要确保保存的场景

---

### DeleteKey(string key)

**签名**:
```csharp
public void DeleteKey(string key)
```

**职责**: 删除指定键的数据

**核心逻辑**:
```
1. 从内存缓存移除
2. 从平台存储删除
```

**调用者**: 需要清除数据的代码

---

## 阅读指引

### 建议的阅读顺序

1. **理解管理器作用** - CacheManager 管理什么
2. **看 GetString/SetString** - 理解基础存储
3. **看 GetValue/SetValue** - 理解对象存储
4. **了解平台适配** - 理解多平台支持

### 最值得学习的技术点

1. **平台适配**: 支持 PlayerPrefs 和轻游平台 API
2. **内存缓存**: Dictionary 缓存对象，减少 IO
3. **JSON 序列化**: 支持复杂对象存储
4. **泛型支持**: GetValue<T>/SetValue<T> 泛型方法

---

## 使用示例

### 示例 1: 存储基础数据

```csharp
// 存储字符串
CacheManager.Instance.SetString(CacheKeys.Account, "12345");
string account = CacheManager.Instance.GetString(CacheKeys.Account);

// 存储整数
CacheManager.Instance.SetInt(CacheKeys.Level, 10);
int level = CacheManager.Instance.GetInt(CacheKeys.Level);

// 存储长整数
CacheManager.Instance.SetLong(CacheKeys.LastLoginTime, DateTime.Now.Ticks);
long lastLogin = CacheManager.Instance.GetLong(CacheKeys.LastLoginTime);
```

### 示例 2: 存储对象

```csharp
// 存储玩家数据
PlayerData playerData = new PlayerData();
playerData.NickName = "玩家 1";
playerData.Money = new BigNumber(1000);

CacheManager.Instance.SetValue("PlayerData", playerData);

// 读取玩家数据
PlayerData data = CacheManager.Instance.GetValue<PlayerData>("PlayerData");
Log.Info($"玩家昵称：{data.NickName}");
```

### 示例 3: 带默认值

```csharp
// 获取整数（默认值 0）
int level = CacheManager.Instance.GetInt("Level", defaultValue: 1);

// 获取字符串（默认值 null）
string name = CacheManager.Instance.GetString("Name", defaultValue: "默认玩家");
```

### 示例 4: 删除数据

```csharp
// 删除指定键
CacheManager.Instance.DeleteKey("TempData");

// 清除所有数据（需要遍历）
foreach (var key in keysToDelete)
{
    CacheManager.Instance.DeleteKey(key);
}
```

---

## 平台差异

| 平台 | 存储 API | 说明 |
|------|---------|------|
| PC/Mobile | PlayerPrefs | Unity 原生存储 |
| 轻游 (QG) | QGMiniGame.QG.Storage* | 轻游平台 API |
| 微信 | PlayerPrefs | 微信小游戏支持 PlayerPrefs |
| 抖音 | PlayerPrefs | 抖音小游戏支持 PlayerPrefs |

---

## 相关文档

- [PlayerData.cs.md](./PlayerData.cs.md) - 玩家数据
- [PlayerManager.cs.md](./PlayerManager.cs.md) - 玩家管理器
- [CacheKeys.cs.md](../Const/CacheKeys.cs.md) - 缓存键常量

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
