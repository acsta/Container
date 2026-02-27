# CacheKeys.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | CacheKeys.cs |
| **路径** | Assets/Scripts/Code/Module/Const/CacheKeys.cs |
| **所属模块** | 框架层 → Code/Module/Const |
| **文件职责** | 缓存键常量定义 |

---

## 类说明

### CacheKeys

| 属性 | 说明 |
|------|------|
| **职责** | 定义所有缓存键的常量，避免硬编码字符串 |
| **类型** | static class |

---

## 缓存键常量

### 账号相关

| 常量 | 值 | 说明 |
|------|-----|------|
| `Account` | "Account" | 账号 |
| `LastToken` | "LastToken" | 上次登录 Token |
| `Password` | "Password" | 密码 |

### 设置相关

| 常量 | 值 | 说明 |
|------|-----|------|
| `CurLangType` | "CurLangType" | 当前语言类型 |
| `KeyCodeSetting` | "KeyCodeSetting" | 按键设置 |
| `MusicVolume` | "MusicVolume" | 音乐音量 |
| `SoundVolume` | "SoundVolume" | 音效音量 |
| `ShockCycle` | "ShockCycle" | 震动周期 |

### 调试相关

| 常量 | 值 | 说明 |
|------|-----|------|
| `ColliderDebug` | "ColliderDebug" | 碰撞体调试开关 |
| `TriggerDebug` | "TriggerDebug" | 触发器调试开关 |

### 游戏进度

| 常量 | 值 | 说明 |
|------|-----|------|
| `Guidance` | "Guidance" | 引导进度 |
| `CheckAppUpdate` | "CheckAppUpdate" | APP 更新检查标记 |

### 动态键（带玩家 ID）

| 属性 | 值 | 说明 |
|------|-----|------|
| `PlayerData` | "PlayerData" + Uid | 玩家数据（每个玩家独立） |
| `DiceSetting` | "DiceSetting" + Uid | 骰子设置（每个玩家独立） |

---

## 使用示例

### 示例 1: 存储账号信息

```csharp
// 保存账号
CacheManager.Instance.SetString(CacheKeys.Account, "player123");

// 保存密码
CacheManager.Instance.SetString(CacheKeys.Password, "password123");

// 保存 Token
CacheManager.Instance.SetString(CacheKeys.LastToken, "abc123xyz");
```

### 示例 2: 读取设置

```csharp
// 读取语言类型
int langType = CacheManager.Instance.GetInt(CacheKeys.CurLangType, (int)LangType.Chinese);

// 读取音量
float musicVolume = CacheManager.Instance.GetFloat(CacheKeys.MusicVolume, 1.0f);
float soundVolume = CacheManager.Instance.GetFloat(CacheKeys.SoundVolume, 1.0f);
```

### 示例 3: 存储玩家数据

```csharp
// 保存玩家数据
PlayerData data = GetPlayerData();
CacheManager.Instance.SetValue(CacheKeys.PlayerData, data);

// 读取玩家数据
PlayerData data = CacheManager.Instance.GetValue<PlayerData>(CacheKeys.PlayerData);
```

### 示例 4: 更新检查标记

```csharp
// 检查是否已提示过更新
int checkFlag = CacheManager.Instance.GetInt(
    CacheKeys.CheckAppUpdate + version, 
    0
);

if (checkFlag != 0)
{
    // 已提示过，跳过
    return;
}

// 设置已提示标记
CacheManager.Instance.SetInt(CacheKeys.CheckAppUpdate + version, 1);
CacheManager.Instance.Save();
```

### 示例 5: 调试开关

```csharp
// 读取碰撞体调试开关
bool colliderDebug = CacheManager.Instance.GetInt(CacheKeys.ColliderDebug, 0) == 1;

if (colliderDebug)
{
    // 显示碰撞体
    ShowColliders();
}
```

---

## 设计优点

### 1. 避免硬编码

```csharp
// ❌ 错误：硬编码字符串
CacheManager.Instance.SetString("Account", "player123");

// ✅ 正确：使用常量
CacheManager.Instance.SetString(CacheKeys.Account, "player123");
```

### 2. 类型安全

```csharp
// 编译时检查，避免拼写错误
CacheManager.Instance.GetString(CacheKeys.Account);  // ✅

// 拼写错误会导致编译失败
CacheManager.Instance.GetString(CacheKeys.Accout);  // ❌ 编译错误
```

### 3. 集中管理

所有缓存键在一个文件中定义，便于维护和查找。

---

## 相关文档

- [GameConst.cs.md](./GameConst.cs.md) - 游戏全局常量
- [CacheManager.cs.md](../Player/CacheManager.cs.md) - 缓存管理器
- [PlayerData.cs.md](../Player/PlayerData.cs.md) - 玩家数据

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
