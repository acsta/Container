# GuidanceStepType.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | GuidanceStepType.cs |
| **路径** | Assets/Scripts/Code/Module/Guidance/GuidanceStepType.cs |
| **所属模块** | 框架层 → Code/Module/Guidance |
| **文件职责** | 引导步骤类型常量定义 |

---

## 类说明

### GuidanceStepType

| 属性 | 说明 |
|------|------|
| **职责** | 定义引导步骤的类型常量 |
| **类型** | class（静态常量） |

```csharp
public class GuidanceStepType
{
    public const int UIRouter = 0;         // UI 路由到指定界面
    public const int FocusGameObject = 1;  // 聚焦某个游戏物体
    public const int WaitEvt = 2;          // 等待事件
}
```

---

## 步骤类型说明

### UIRouter (0)

**说明**: UI 路由到指定界面

**用途**: 引导玩家打开特定界面

**配置字段**:
- `Value1`: 目标界面名称
- `Value2`: 高亮对象路径
- `Value3`: 是否显示遮罩 ("0" 或 "1")

**示例配置**:
```csv
Id,Steptype,Value1,Value2,Value3
1,0,UIMain,Btn_Start,1
```

**执行逻辑**:
```
1. 获取当前顶层界面
2. 如果已是目标界面，直接完成
3. 否则，计算路由路径
4. 高亮路径上的按钮
5. 等待玩家点击
```

---

### FocusGameObject (1)

**说明**: 聚焦某个游戏物体

**用途**: 引导玩家关注特定 UI 元素或 3D 对象

**配置字段**:
- `Value1`: 界面名称
- `Value2`: 对象路径（相对于界面）
- `Value3`: 是否显示遮罩

**示例配置**:
```csv
Id,Steptype,Value1,Value2,Value3
2,1,UIMain,Btn_Start,1
```

**执行逻辑**:
```
1. 获取指定界面
2. 查找对象：界面.变换.查找 (路径).gameObject
3. 设置 GuideTarget = 对象
4. 显示对话提示
5. 等待玩家操作
```

---

### WaitEvt (2)

**说明**: 等待事件

**用途**: 等待玩家完成特定操作

**配置字段**:
- `Event`: 等待的事件名称
- `During`: 对话持续时间（毫秒，<0 表示无限）

**示例配置**:
```csv
Id,Steptype,Event,During
3,2,Click_Btn_Start,3000
```

**执行逻辑**:
```
1. 显示对话提示
2. 等待 NoticeEvent(Event) 调用
3. 如果 During > 0，超时自动完成
4. 收到事件后完成步骤
```

**常见事件**:
- `Click_Btn_Start` - 点击开始按钮
- `Open_UIAuction` - 打开拍卖界面
- `Close_UIAuction` - 关闭拍卖界面
- `Buy_Item_001` - 购买物品 001

---

## 使用示例

### 示例 1: 配置引导步骤

```csv
# 引导组 1: 新手入门
Group,Steps,Condition,Share,GroupOrder
1,"[1,2,3,4]","",0,1

# 步骤 1: 路由到主界面
Id,Steptype,Value1,Value2,Value3,KeyStep,During
1,UIRouter,UIMain,,1,1,-1

# 步骤 2: 聚焦开始按钮
2,FocusGameObject,UIMain,Btn_Start,1,0,-1

# 步骤 3: 等待点击开始
3,WaitEvt,Click_Btn_Start,,,,0,3000

# 步骤 4: 路由到拍卖界面
4,UIRouter,UIAuction,,1,1,-1
```

### 示例 2: 触发事件

```csharp
// 玩家点击开始按钮
public void OnClickStart()
{
    // 通知引导系统
    GuidanceManager.Instance.NoticeEvent("Click_Btn_Start");
    
    // 执行游戏逻辑
    StartGame();
}

// 打开界面
public void OpenAuction()
{
    UIManager.Instance.OpenWindow("UIAuction");
    GuidanceManager.Instance.NoticeEvent("Open_UIAuction");
}
```

### 示例 3: 检查步骤类型

```csharp
// GuidanceManager 内部
private bool CheckStepCanRunning(int id)
{
    var step = GuidanceConfigCategory.Instance.Get(id);
    
    if (step.Steptype == GuidanceStepType.UIRouter)
    {
        return false;  // UIRouter 由系统自动处理
    }
    if (step.Steptype == GuidanceStepType.FocusGameObject)
    {
        return false;  // FocusGameObject 由系统自动处理
    }
    if (step.Steptype == GuidanceStepType.WaitEvt)
    {
        return false;  // WaitEvt 由事件触发
    }
    
    Log.Error("未处理的类型 Steptype=" + step.Steptype);
    return false;
}
```

---

## 步骤类型对比

| 类型 | 值 | 自动完成 | 需要事件 | 用途 |
|------|-----|---------|---------|------|
| `UIRouter` | 0 | ✅ | ❌ | 界面跳转 |
| `FocusGameObject` | 1 | ✅ | ❌ | 高亮对象 |
| `WaitEvt` | 2 | ❌ | ✅ | 等待操作 |

---

## 与 GuidanceManager 的关系

```
GuidanceStepType (常量定义)
    ↓
    定义步骤类型
    ↓
GuidanceConfig (配置表)
    ↓
    Steptype 字段引用常量
    ↓
GuidanceManager (运行时)
    ↓
    根据 Steptype 执行不同逻辑
```

---

## 相关文档

- [GuidanceManager.cs.md](./GuidanceManager.cs.md) - 引导管理器
- [GuidanceConfig.cs.md](../Generate/Config/GuidanceConfig.cs.md) - 引导配置

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
