# RestaurantTask.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | RestaurantTask.cs |
| **路径** | Assets/Scripts/Code/Game/UIGame/UILobby/RestaurantTask.cs |
| **所属模块** | 游戏 UI → UILobby (大厅 UI) |
| **文件职责** | 餐厅任务项组件，展示任务进度、奖励和完成状态 |

---

## 类/结构体说明

### RestaurantTask

| 属性 | 说明 |
|------|------|
| **职责** | 餐厅任务项 UI 组件，展示任务信息、进度条、奖励和完成状态 |
| **泛型参数** | 无 |
| **继承关系** | `UIBaseContainer` |
| **实现的接口** | `IOnCreate` |

**设计模式**: 组件模式 + 数据绑定

```csharp
public class RestaurantTask : UIBaseContainer, IOnCreate
{
    public TaskConfig Config { get; private set; }
    private Action<RestaurantTask> onClickOver;
}
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `Config` | `TaskConfig` | `public` | 当前任务配置数据 |
| `ItemName` | `UITextmesh` | `public` | 物品名称文本 |
| `ItemIcon` | `UIImage` | `public` | 物品图标 |
| `Slider` | `UISlider` | `public` | 进度条组件 |
| `ValueText` | `UITextmesh` | `public` | 进度数值文本 |
| `Over` | `UIEmptyView` | `public` | 完成标记容器 |
| `Mask` | `UIEmptyView` | `public` | 遮罩容器 |
| `Details` | `UIEmptyView` | `public` | 详情按钮容器 |
| `Desc` | `UITextmesh` | `public` | 任务描述文本 |
| `Rewards` | `UITextmesh` | `public` | 奖励标题文本 |
| `RewardsVal` | `UITextmesh` | `public` | 奖励数值文本 |
| `PointerClick` | `UIPointerClick` | `public` | 点击事件组件 |
| `Animator` | `UIAnimator` | `public` | 动画组件 |
| `Animator2` | `UIAnimator` | `public` | 内容动画组件 |
| `LightImage` | `UIImage` | `public` | 高亮图片 (完成时闪烁) |

---

## 方法说明（按重要程度排序）

### OnCreate()

**签名**:
```csharp
public void OnCreate()
```

**职责**: 创建组件时初始化所有子组件

**核心逻辑**:
```
1. 添加所有 UI 组件 (文本、图片、按钮、动画等)
2. 设置组件路径引用
```

**调用者**: UIManager (组件创建时)

---

### SetData(TaskConfig config, Action<RestaurantTask> onClickOver)

**签名**:
```csharp
public void SetData(TaskConfig config, Action<RestaurantTask> onClickOver)
```

**职责**: 设置任务数据并更新 UI 显示

**核心逻辑**:
```
1. 保存配置和完成回调
2. 检查配置有效性，设置组件显隐
3. 根据奖励类型设置奖励文本:
   - 类型 1: 金币奖励 (带时间转换)
   - 类型 2: 直接数值奖励
4. 根据物品类型设置物品信息:
   - 类型 0: 物品 (ItemConfig)
   - 类型 1: 容器 (ContainerConfig)
5. 获取任务进度状态
6. 更新进度条和文本
7. 根据状态设置交互:
   - 已完成: 禁用交互
   - 可完成: 显示完成标记，绑定完成回调
   - 进行中: 显示详情按钮
8. 更新遮罩宽度动画
```

**调用者**: 父视图 (如 UILobbyView)

**使用示例**:
```csharp
var task = view.AddComponent<RestaurantTask>("path");
task.SetData(config, OnTaskCompleted);
```

---

### OnClickDetails()

**签名**:
```csharp
private void OnClickDetails()
```

**职责**: 点击详情按钮处理

**核心逻辑**:
```
1. 播放点击动画
2. 打开任务详情窗口 (UITaskDetailsWin)
```

**调用者**: PointerClick (详情按钮)

---

### OnClickComplex()

**签名**:
```csharp
private void OnClickComplex()
```

**职责**: 点击完成按钮处理

**核心逻辑**:
```
1. 禁用动画
2. 调用完成回调 (onClickOver)
```

**调用者**: PointerClick (完成按钮)

---

### UpdateAsync(float val)

**签名**:
```csharp
private async ETTask UpdateAsync(float val)
```

**职责**: 异步更新进度遮罩宽度

**核心逻辑**:
```
1. 等待父容器宽度有效
2. 根据进度值计算遮罩宽度
3. 更新遮罩 offsetMin
4. 更新文本区域大小
```

**调用者**: SetData()

---

### OnClickAnim()

**签名**:
```csharp
private async ETTask OnClickAnim()
```

**职责**: 播放点击动画

**核心逻辑**:
```
1. 启用动画
2. 播放 UIRestaurantTask_Click 动画
3. 禁用动画并重置缩放
```

**调用者**: OnClickDetails()

---

### OnClickLightImage() / OnClickLightImageAsync()

**签名**:
```csharp
public void OnClickLightImage()
public async ETTask OnClickLightImageAsync()
```

**职责**: 高亮图片动画 (完成时闪烁效果)

**核心逻辑**:
```
1. 检查当前状态
2. 100ms 内渐变:
   - 淡入: 透明度 0→1
   - 淡出: 缩放 1→0
3. 完成后隐藏图片
```

**调用者**: 完成事件触发时

---

### SetInteractable(bool enable)

**签名**:
```csharp
public void SetInteractable(bool enable)
```

**职责**: 设置组件交互状态

**核心逻辑**:
```
1. 设置 PointerClick 启用状态
```

**调用者**: 父视图

---

## 任务状态流程

### 状态转换图

```mermaid
stateDiagram-v2
    [*] --> 无效：config == null
    [*] --> 进行中：step < itemCount
    [*] --> 可完成：step >= itemCount && !isOver
    [*] --> 已完成：isOver == true
    
    进行中 --> 可完成：进度达到 100%
    可完成 --> 已完成：玩家点击完成
    已完成 --> [*]: 任务重置
    
    note right of 无效
        隐藏所有组件
        Mask/Over 不显示
    end note
    
    note right of 进行中
        显示详情按钮
        进度条可交互
    end note
    
    note right of 可完成
        显示完成标记
        播放闪烁动画
    end note
    
    note right of 已完成
        隐藏遮罩和标记
        禁用交互
    end note
```

---

## 奖励类型说明

### RewardType

| 类型 | 说明 | 显示格式 |
|------|------|----------|
| `1` | 金币奖励 (按时间计算) | "金额/时间" (如 "100/8h") |
| `2` | 直接数值奖励 | "金额" (如 "500") |

---

## 使用示例

### 示例 1: 创建任务项

```csharp
// 创建任务项组件
var task = parent.AddComponent<RestaurantTask>("Content/Task");

// 设置任务数据
task.SetData(taskConfig, (t) => {
    // 任务完成回调
    Debug.Log($"任务完成：{t.Config.Id}");
});
```

### 示例 2: 设置交互状态

```csharp
// 禁用交互 (如网络请求中)
task.SetInteractable(false);

// 启用交互
task.SetInteractable(true);
```

### 示例 3: 播放高亮动画

```csharp
// 播放完成高亮动画
await task.OnClickLightImageAsync();
```

---

## 与其他模块的交互

```mermaid
graph TD
    subgraph RestaurantTask["RestaurantTask"]
        UI[UI 组件]
        Data[TaskConfig]
    end
    
    subgraph Systems["游戏系统"]
        Player[PlayerDataManager]
        I18N[I18NManager]
        Item[ItemConfig]
        Container[ContainerConfig]
    end
    
    subgraph UI["其他 UI"]
        Details[UITaskDetailsWin]
        Anim[UIAnimator]
    end
    
    UI --> Data
    UI --> Player
    UI --> I18N
    UI --> Item
    UI --> Container
    UI --> Details
    UI --> Anim
    
    note right of UI "餐厅任务项展示<br/>任务进度、奖励和状态"
    
    style RestaurantTask fill:#e1f5ff
    style Systems fill:#fff4e1
    style UI fill:#e8f5e9
```

---

## 阅读指引

### 建议的阅读顺序

1. **理解组件作用** - 餐厅任务项用于展示任务进度和奖励
2. **看字段定义** - 了解各个 UI 组件的作用
3. **重点看 SetData** - 理解数据绑定和状态更新
4. **了解状态流程** - 理解不同状态下的 UI 表现
5. **查看动画效果** - 了解点击和高亮动画

### 最值得学习的技术点

1. **数据绑定模式**: SetData 方法统一更新所有 UI 元素
2. **状态管理**: 根据任务状态动态调整 UI 显隐和交互
3. **异步遮罩更新**: UpdateAsync 等待父容器尺寸后更新遮罩
4. **奖励类型处理**: 根据 RewardType 显示不同格式
5. **高亮动画**: OnClickLightImageAsync 实现完成闪烁效果

---

## 相关文档

- [UILobbyView.cs.md](./UILobbyView.cs.md) - 大厅主界面
- [UITaskDetailsWin.cs.md](./UITaskDetailsWin.cs.md) - 任务详情窗口
- [UIAnimator.cs.md](../../../Module/UIComponent/UIAnimator.cs.md) - 动画组件
- [UISlider.cs.md](../../../Module/UIComponent/UISlider.cs.md) - 进度条组件
- [TaskConfig.cs.md](../../../Module/Generate/Config/TaskConfig.cs.md) - 任务配置

---

*文档生成时间：2026-03-02 | OpenClaw AI 助手*
