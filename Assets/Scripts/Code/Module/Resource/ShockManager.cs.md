# ShockManager.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | ShockManager.cs |
| **路径** | Assets/Scripts/Code/Module/Resource/ShockManager.cs |
| **所属模块** | 框架层 → Code/Module/Resource |
| **文件职责** | 震动管理器，控制设备震动反馈 |

---

## 类说明

### ShockManager

| 属性 | 说明 |
|------|------|
| **职责** | 管理设备震动反馈 |
| **类型** | class |
| **实现的接口** | `IManager` |

---

## 方法说明

### Vibrate()

**签名**:
```csharp
public void Vibrate()
```

**职责**: 触发设备震动

**核心逻辑**:
```
1. 检查震动开关
2. 调用平台震动 API
```

**调用者**: 按钮点击、重要事件

**使用示例**:
```csharp
// 按钮点击时震动
ShockManager.Instance.Vibrate();
```

---

## 使用示例

### 示例 1: 按钮点击震动

```csharp
public class UIButton : MonoBehaviour
{
    public void OnClick()
    {
        // 播放音效
        SoundManager.Instance.PlaySound("Audio/Sound/Common_Click.mp3");
        
        // 震动反馈
        ShockManager.Instance.Vibrate();
        
        // 执行点击逻辑
        OnClickBtn();
    }
}
```

### 示例 2: 重要事件震动

```csharp
// 拍卖胜利
if (auctionResult == AuctionResult.Win)
{
    // 震动 3 次
    ShockManager.Instance.Vibrate();
    await TimerManager.Instance.WaitAsync(200);
    ShockManager.Instance.Vibrate();
    await TimerManager.Instance.WaitAsync(200);
    ShockManager.Instance.Vibrate();
}
```

---

## 相关文档

- [SoundManager.cs.md](./SoundManager.cs.md) - 音效管理器
- [GameConst.cs.md](../Const/GameConst.cs.md) - 游戏常量

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
