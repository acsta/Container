# GuidanceManager.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | GuidanceManager.cs |
| **路径** | Assets/Scripts/Code/Module/Guidance/GuidanceManager.cs |
| **所属模块** | 框架层 → Code/Module/Guidance |
| **文件职责** | 新手引导管理器，控制引导流程 |

---

## 类/结构体说明

### GuidanceManager

| 属性 | 说明 |
|------|------|
| **职责** | 管理新手引导流程，包括引导组、步骤、事件监听等 |
| **泛型参数** | 无 |
| **继承关系** | 实现 `IManager` |

```csharp
public class GuidanceManager : IManager
{
    // 新手引导管理器
}
```

---

## 字段与属性

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `Instance` | `GuidanceManager` | `public static` | 单例实例 |
| `GuideTarget` | `GameObject` | `public static` | 当前引导聚焦的游戏对象 |
| `ShowMask` | `bool` | `public static` | 是否显示遮罩 |
| `Group` | `int` | `private` | 当前引导组 ID |
| `Config` | `GuidanceGroupConfig` | `private` | 当前引导组配置（只读） |
| `CurIndex` | `int` | `private` | 当前步骤索引 |
| `StepConfig` | `GuidanceConfig` | `private` | 当前步骤配置（只读） |
| `CacheValues` | `Dictionary<string, int>` | `private` | 引导完成状态缓存 |

---

## 方法说明

### Init()

**签名**:
```csharp
public void Init()
```

**职责**: 初始化引导管理器

**核心逻辑**:
```
1. 设置 Instance = this
2. 初始化 CacheValues 字典
3. 设置 CurIndex = -1, Group = -1
```

**调用者**: ManagerProvider 注册时

---

### Destroy()

**签名**:
```csharp
public void Destroy()
```

**职责**: 销毁引导管理器

**核心逻辑**:
```
1. 设置 Instance = null
```

**调用者**: ManagerProvider 移除时

---

### UpdateGuidanceDone(List<int> doneList)

**签名**:
```csharp
public void UpdateGuidanceDone(List<int> doneList)
```

**职责**: 服务端通知已完成的引导组

**参数**:
- `doneList`: 已完成的引导组 ID 列表

**核心逻辑**:
```
1. 检查 PlayerManager.Instance.Uid > 0
2. 遍历 doneList
3. 调用 SaveKey(id, 1) 标记为完成
```

**调用者**: 登录成功后

---

### UpdateGuidanceNotDone(List<int> notDoneList)

**签名**:
```csharp
public void UpdateGuidanceNotDone(List<int> notDoneList)
```

**职责**: 服务端通知未完成的引导组

**核心逻辑**:
```
1. 检查 PlayerManager.Instance.Uid > 0
2. 遍历 notDoneList
3. 调用 SaveKey(id, 0) 标记为未完成
```

**调用者**: 登录成功后

---

### CheckGroupStart()

**签名**:
```csharp
public void CheckGroupStart()
```

**职责**: 检查是否有可开启的引导组

**核心逻辑**:
```
1. 如果当前有引导进行中 (Group >= 0)，返回
2. 遍历所有引导组配置
3. 检查引导组是否未完成 (GetKey == 0)
4. 检查引导组条件是否满足 (IsGroupCondition)
5. 如果满足，启动引导 (StartGuide)
```

**调用时机**:
- 进游戏时
- 登录后
- 完成一个引导后

---

### StartGuide(int group)

**签名**:
```csharp
public void StartGuide(int group)
```

**职责**: 开始指定引导组

**参数**:
- `group`: 引导组 ID

**核心逻辑**:
```
1. 检查是否已在进行该引导
2. 检查优先级（不能打断更高优先级的引导）
3. 检查引导配置是否存在
4. 设置 Group = group
5. 从后往前查找可执行的步骤
6. 如果没有，从步骤 0 开始
7. 调用 RunStep 执行步骤
```

**调用者**: CheckGroupStart(), 外部触发

---

### NoticeEvent(string evt)

**签名**:
```csharp
public void NoticeEvent(string evt)
```

**职责**: 通知事件发生（用于等待事件类型的步骤）

**参数**:
- `evt`: 事件名称

**核心逻辑**:
```
1. 如果有进行中的步骤
2. 检查事件是否匹配当前步骤的 Event
3. 如果匹配，完成步骤 (OnStepOver)
4. 如果是 UIRouter 类型且打开了新界面，重新运行步骤
```

**调用者**: Messager 广播事件时

---

### RunStep(int index)

**签名**:
```csharp
private void RunStep(int index)
```

**职责**: 执行指定索引的步骤

**参数**:
- `index`: 步骤索引

**核心逻辑**:
```
1. 如果上一步是对话且未结束，先结束对话
2. 设置 CurIndex = index
3. 根据步骤类型执行:
   - UIRouter: 路由到指定界面，高亮目标
   - FocusGameObject: 聚焦指定游戏对象
   - WaitEvt: 显示对话，等待事件
4. 如果找不到目标，记录错误
```

**步骤类型处理**:
```csharp
if (StepConfig.Steptype == GuidanceStepType.UIRouter)
{
    // 路由到指定界面
    var win = UIManager.Instance.GetTopWindow(...);
    FocusGameObject(win, config.Path, ...);
}
else if (StepConfig.Steptype == GuidanceStepType.FocusGameObject)
{
    // 聚焦游戏对象
    var win = UIManager.Instance.GetWindow(...);
    FocusGameObject(win, StepConfig.Value2, ...);
}
else if (StepConfig.Steptype == GuidanceStepType.WaitEvt)
{
    // 等待事件
    Messager.Instance.Broadcast(..., MessageId.GuidanceTalk, ...);
}
```

**调用者**: StartGuide(), OnStepOver()

---

### OnStepOver(int id)

**签名**:
```csharp
private void OnStepOver(int id)
```

**职责**: 完成一个步骤

**参数**:
- `id`: 步骤 ID

**核心逻辑**:
```
1. 检查当前步骤 ID 是否匹配
2. 如果是关键步骤 (KeyStep == 1)，保存完成状态
3. 检查是否还有下一步
4. 如果有，执行下一步 (RunStep)
5. 如果没有，停止引导 (Stop)
```

**调用者**: NoticeEvent(), 步骤完成时

---

### Stop()

**签名**:
```csharp
private void Stop()
```

**职责**: 停止当前引导

**核心逻辑**:
```
1. 记录引导完成日志
2. 如果当前是对话，结束对话
3. 重置 CurIndex = -1, Group = -1
4. 取消聚焦 (UnFocusGameObject)
5. 检查是否有下一个引导组可启动 (CheckGroupStart)
```

**调用者**: OnStepOver()（最后一步完成时）

---

### GetKey(int id) / SaveKey(int id, int val)

**签名**:
```csharp
private int GetKey(int id)
private void SaveKey(int id, int val)
```

**职责**: 获取/保存引导完成状态

**核心逻辑**:
```
1. 根据配置 Share 字段决定 Key 格式:
   - Share != 0: "Guidance_{id}" (全服共享)
   - Share == 0: "Guidance_{id}_{uid}" (个人独立)
2. 从 CacheValues 缓存读取/写入
3. SaveKey 时同步到服务器 (PlayerDataManager.GuideDown)
```

---

### FocusGameObject(UIWindow win, string path, bool showMask)

**签名**:
```csharp
private void FocusGameObject(UIWindow win, string path, bool showMask)
```

**职责**: 聚焦指定游戏对象

**参数**:
- `win`: UI 窗口
- `path`: 对象路径（相对于窗口）
- `showMask`: 是否显示遮罩

**核心逻辑**:
```
1. 查找对象：win.View.GetRectTransform().Find(path).gameObject
2. 设置 GuideTarget = 找到的对象
3. 设置 ShowMask = showMask
4. 如果找不到，记录错误
```

**调用者**: RunStep()

---

### UnFocusGameObject()

**签名**:
```csharp
private void UnFocusGameObject()
```

**职责**: 取消聚焦

**核心逻辑**:
```
1. 设置 GuideTarget = null
```

**调用者**: RunStep(), Stop()

---

### IsGroupCondition(string condition)

**签名**:
```csharp
private bool IsGroupCondition(string condition)
```

**职责**: 检查引导组开启条件

**参数**:
- `condition`: 条件字符串

**核心逻辑**:
```
1. GuideScene_{id}: 检查是否在指定场景
2. GuideOver_{id}: 检查指定引导是否已完成
3. 其他：默认满足
```

**调用者**: CheckGroupStart()

---

## 阅读指引

### 建议的阅读顺序

1. **理解引导结构** - 引导组 → 步骤 → 事件
2. **看启动流程** - CheckGroupStart → StartGuide → RunStep
3. **看步骤执行** - 三种步骤类型的处理
4. **看完成流程** - NoticeEvent → OnStepOver → Stop

### 最值得学习的技术点

1. **配置驱动**: 引导完全由配置表控制
2. **状态缓存**: CacheValues 避免频繁读缓存
3. **事件驱动**: NoticeEvent 监听游戏事件
4. **优先级控制**: 高优先级引导可打断低优先级
5. **UI 路由**: 自动计算界面间跳转路径

---

## 引导流程

```
进游戏/登录
    ↓
CheckGroupStart() 检查可开启引导
    ↓
StartGuide(group) 启动引导组
    ↓
RunStep(index) 执行步骤
    ├── UIRouter → 路由界面，高亮目标
    ├── FocusGameObject → 聚焦对象
    └── WaitEvt → 等待事件
    ↓
玩家操作触发事件
    ↓
NoticeEvent(evt) 通知事件
    ↓
OnStepOver(id) 完成步骤
    ↓
还有下一步？
├─ 是 → RunStep(index+1)
└─ 否 → Stop() → CheckGroupStart()
```

---

## 使用示例

### 示例 1: 初始化引导

```csharp
// 登录成功后
List<int> doneList = serverData.GuidanceDone;
List<int> notDoneList = serverData.GuidanceNotDone;

GuidanceManager.Instance.UpdateGuidanceDone(doneList);
GuidanceManager.Instance.UpdateGuidanceNotDone(notDoneList);

// 检查是否有可开启引导
GuidanceManager.Instance.CheckGroupStart();
```

### 示例 2: 触发事件

```csharp
// 玩家点击按钮
public void OnClick()
{
    // 通知引导系统
    GuidanceManager.Instance.NoticeEvent("Click_Btn_Start");
}

// 打开界面
public void OpenWindow(string winName)
{
    UIManager.Instance.OpenWindow(winName);
    GuidanceManager.Instance.NoticeEvent($"Open_{winName}");
}
```

### 示例 3: 配置表格式

```csv
Group,Steps,Condition,Share,GroupOrder
1,"[1,2,3]","",0,1
2,"[4,5,6]","GuideOver_1",0,2
```

```csv
Id,Steptype,Event,Value1,Value2,Value3,KeyStep,During
1,UIRouter,"","UICommon","", "1",1,-1
2,FocusGameObject,"","UIMain","Btn_Start","",0,-1
3,WaitEvt,"Click_Btn_Start","","","",0,3000
```

---

## 相关文档

- [GuidanceStepType.cs.md](./GuidanceStepType.cs.md) - 引导步骤类型
- [GuidanceConfig.cs.md](../Generate/Config/GuidanceConfig.cs.md) - 引导配置
- [UIManager.cs.md](../UI/UIManager.cs.md) - UI 管理器
- [Messager.cs.md](../../Mono/Module/Messager/Messager.cs.md) - 消息器

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
