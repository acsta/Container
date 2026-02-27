# I18NKey.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | I18NKey.cs |
| **路径** | Assets/Scripts/Code/Module/Const/I18NKey.cs |
| **所属模块** | 框架层 → Code/Module/Const |
| **文件职责** | 国际化键枚举，定义所有多语言文本的键 |

---

## 枚举说明

### I18NKey

| 属性 | 说明 |
|------|------|
| **职责** | 定义所有多语言文本的键（ID），用于 I18NManager 查找翻译 |
| **类型** | enum（int） |
| **用途** | 类型安全的多语言键 |

```csharp
public enum I18NKey
{
    Global_Unknow = 0,
    Btn_Enter_Game = 1,
    // ... 300+ 个键
}
```

---

## 键分类（按功能）

### 全局按钮（Global Buttons）

| 键 | ID | 中文 | 英文 |
|------|-----|------|------|
| `Global_Btn_Confirm` | 3 | 确定 | Confirm |
| `Global_Btn_Cancel` | 4 | 取消 | Cancel |
| `Global_Btn_Back` | 5 | 返回 | Back |
| `Global_Btn_Login` | 6 | 登录 | Login |
| `Global_Btn_Register` | 7 | 注册 | Register |

### 游戏按钮（Game Buttons）

| 键 | ID | 中文 | 英文 |
|------|-----|------|------|
| `Btn_Enter_Game` | 1 | 进入游戏 | Enter Game |
| `Btn_Create_Role` | 2 | 创建角色 | Create Role |
| `Btn_Exit` | 18 | 退出 | Exit |
| `Btn_Quit_Game` | 15 | 退出游戏 | Quit Game |

### 更新相关（Update）

| 键 | ID | 中文 | 英文 |
|------|-----|------|------|
| `Update_Get_Fail` | 22 | 获取更新失败 | Update Failed |
| `Update_ReTry` | 23 | 重试 | Retry |
| `Update_Download_Fail` | 24 | 下载失败 | Download Failed |
| `Update_Info` | 25 | 更新提示 | Update Info |
| `Update_White` | 26 | 白名单提示 | White List |
| `Update_ReDownload` | 27 | 重新下载 | Re-download |
| `Update_SuDownload` | 28 | 稍后下载 | Download Later |
| `Update_Skip` | 34 | 跳过 | Skip |

### 游戏文本（Game Text）

| 键 | ID | 说明 |
|------|-----|------|
| `Text_Normal_Attack` | 19 | 普通攻击 |
| `Text_Game_Stage` | 43 | 关卡 |
| `Text_Game_TimeDown0-3` | 44-47 | 倒计时 |
| `Text_Game_Loss` | 49 | 失败 |
| `Text_Game_Win` | 50 | 胜利 |
| `Text_Game_OtherLoss` | 51 | 他人失败 |
| `Text_Game_OtherWin` | 52 | 他人胜利 |

### 数值单位（中文）

| 键 | ID | 说明 |
|------|-----|------|
| `Text_Common_UnitCn1` | 54 | 万 |
| `Text_Common_UnitCn2` | 55 | 亿 |
| `Text_Common_UnitCn3-12` | 56-65 | 兆、京、垓... |

### 数值单位（英文）

| 键 | ID | 说明 |
|------|-----|------|
| `Text_Common_UnitEn1` | 66 | K |
| `Text_Common_UnitEn2` | 67 | M |
| `Text_Common_UnitEn3` | 68 | B |
| `Text_Common_UnitEn4-16` | 69-81 | T、Qa、Qi... |

### 登录相关（Login）

| 键 | ID | 说明 |
|------|-----|------|
| `Login_Text_Account` | 86 | 账号 |
| `Login_Text_Psd` | 87 | 密码 |
| `Login_Text_Send` | 88 | 发送验证码 |
| `Login_Text_CodeLogin` | 89 | 验证码登录 |
| `Login_Text_Code` | 90 | 验证码 |
| `Login_Text_Phone` | 91 | 手机号 |
| `Login_Text_PswLogin` | 92 | 密码登录 |
| `Login_Text_LoginFail` | 93 | 登录失败 |
| `Login_Text_LoginSuccess` | 94 | 登录成功 |

### 通知提示（Notice）

| 键 | ID | 说明 |
|------|-----|------|
| `Notice_Login_By_Others` | 20 | 被顶号通知 |
| `Notice_Link_Close` | 21 | 连接关闭通知 |
| `Notice_Common_LackOfMoney` | 48 | 金币不足 |
| `Notice_Game_WinOthers` | 53 | 赢得他人 |

### 帮助系统（Help）

| 键 | ID | 说明 |
|------|-----|------|
| `Help_Info` | 35 | 帮助信息 |
| `Help_Test` | 36 | 帮助测试 |
| `Help_Setting` | 37 | 帮助设置 |

### 其他（Others）

| 键 | ID | 说明 |
|------|-----|------|
| `Global_Unknow` | 0 | 未知键 |
| `Global_AutoPlay` | 29 | 自动播放 |
| `Global_QuickPlay` | 30 | 快速播放 |
| `Global_Record` | 31 | 录制 |
| `GalGame_Stop` | 32 | 停止 |
| `RoleName_Unknow` | 33 | 未知角色名 |
| `Net_Error` | 40 | 网络错误 |

---

## 阅读指引

### 建议的阅读顺序

1. **理解枚举作用** - I18NKey 有什么用
2. **看全局按钮** - 最常用键
3. **看数值单位** - 中英文单位
4. **了解分类** - 按功能分类

### 最值得学习的技术点

1. **类型安全**: enum 防止拼写错误
2. **ID 映射**: 每个键对应唯一 ID
3. **分类管理**: 按功能分组键
4. **扩展性**: 容易添加新键

---

## 使用示例

### 示例 1: 获取翻译文本

```csharp
// 简单翻译
string confirmText = I18NManager.Instance.I18NGetText(I18NKey.Global_Btn_Confirm);
// 中文 → "确定"
// 英文 → "Confirm"

// 带参数翻译
string rewardText = I18NManager.Instance.I18NGetParamText(
    I18NKey.Reward_Gold, 
    1000
);
// 中文 → "获得 1000 金币"
```

### 示例 2: UI 文本设置

```csharp
public class UIButton : MonoBehaviour
{
    public I18NKey textKey;
    private Text text;
    
    void Start()
    {
        text = GetComponent<Text>();
        UpdateText();
        
        // 监听语言切换
        I18NManager.Instance.OnLanguageChangeEvt += UpdateText;
    }
    
    void UpdateText()
    {
        text.text = I18NManager.Instance.I18NGetText(textKey);
    }
    
    void OnDestroy()
    {
        I18NManager.Instance.OnLanguageChangeEvt -= UpdateText;
    }
}
```

### 示例 3: 错误提示

```csharp
// 网络错误
UIManager.Instance.OpenBox<UIToast, string>(
    UIToast.PrefabPath,
    I18NManager.Instance.I18NGetText(I18NKey.Net_Error)
);

// 金币不足
UIManager.Instance.OpenBox<UIToast, string>(
    UIToast.PrefabPath,
    I18NManager.Instance.I18NGetText(I18NKey.Notice_Common_LackOfMoney)
);
```

### 示例 4: 数值单位

```csharp
// 中文单位
BigNumber money = new BigNumber(15000000);
string text = I18NManager.Instance.TranslateMoneyToStr(money);
// 使用 Text_Common_UnitCn1 (万) → "1500.00 万"

// 英文单位
string textEn = I18NManager.Instance.TranslateMoneyToStr(money);
// 使用 Text_Common_UnitEn2 (M) → "15.00M"
```

---

## 添加新键

### 步骤

1. **在 I18NKey.cs 添加枚举**:
```csharp
Text_New_Feature = 311,
```

2. **在配置表添加翻译**:
```csv
Id,Key,Value (中文),Value (English)
311,Text_New_Feature,新功能,New Feature
```

3. **使用新键**:
```csharp
string text = I18NManager.Instance.I18NGetText(I18NKey.Text_New_Feature);
```

---

## 相关文档

- [I18NManager.cs.md](../I18N/I18NManager.cs.md) - 国际化管理器
- [I18NConfig.cs.md](../I18N/I18NConfig.cs.md) - 国际化配置
- [II18N.cs.md](../I18N/II18N.cs.md) - 国际化接口

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
