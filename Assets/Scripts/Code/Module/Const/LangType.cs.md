# LangType.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | LangType.cs |
| **路径** | Assets/Scripts/Code/Module/Const/LangType.cs |
| **所属模块** | 框架层 → Code/Module/Const |
| **文件职责** | 语言类型枚举定义 |

---

## 枚举说明

### LangType

| 属性 | 说明 |
|------|------|
| **职责** | 定义支持的语言类型 |
| **类型** | enum（int） |

```csharp
public enum LangType
{
    Chinese = 0,
    English = 1,
}
```

---

## 语言类型

| 值 | 枚举 | 说明 |
|------|------|------|
| 0 | `Chinese` | 中文 |
| 1 | `English` | 英文 |

---

## 使用示例

### 示例 1: 切换语言

```csharp
// 切换到中文
await I18NManager.Instance.SwitchLanguage((int)LangType.Chinese);

// 切换到英文
await I18NManager.Instance.SwitchLanguage((int)LangType.English);
```

### 示例 2: 获取当前语言

```csharp
// 获取当前语言类型
LangType currentLang = I18NManager.Instance.CurLangType;

if (currentLang == LangType.Chinese)
{
    Log.Info("当前是中文");
}
else if (currentLang == LangType.English)
{
    Log.Info("当前是英文");
}
```

### 示例 3: 缓存语言设置

```csharp
// 保存语言设置
CacheManager.Instance.SetInt(CacheKeys.CurLangType, (int)LangType.English);

// 读取语言设置
int langType = CacheManager.Instance.GetInt(CacheKeys.CurLangType, (int)LangType.Chinese);
LangType lang = (LangType)langType;
```

### 示例 4: 条件显示

```csharp
// 根据语言显示不同文本
if (I18NManager.Instance.CurLangType == LangType.Chinese)
{
    titleText.SetText("欢迎来到拍卖场");
}
else
{
    titleText.SetText("Welcome to Auction");
}
```

---

## 扩展新语言

### 步骤

1. **添加枚举值**:
```csharp
public enum LangType
{
    Chinese = 0,
    English = 1,
    Japanese = 2,  // 新增
    Korean = 3,    // 新增
}
```

2. **创建语言配置**:
```csv
Id,Key,Value (中文),Value (English),Value (Japanese),Value (Korean)
1,Global_Btn_Confirm，确定，Confirm，確認，확인
```

3. **更新 I18NManager**:
```csharp
// 在 TranslateMoneyToStr 中添加新语言处理
if (CurLangType == LangType.Japanese)
{
    // 日文缩写逻辑
}
```

---

## 相关文档

- [I18NManager.cs.md](../I18N/I18NManager.cs.md) - 国际化管理器
- [I18NKey.cs.md](./I18NKey.cs.md) - 国际化键
- [CacheKeys.cs.md](./CacheKeys.cs.md) - 缓存键

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
