# Container 框架学习计划

> **创建时间**: 2026-03-05  
> **目标**: 循序渐进掌握 Container Unity 游戏框架  
> **预计周期**: 4-6 周 (根据基础调整)

---

## 📋 学习前准备

### 前置知识检查

| 知识点 | 重要程度 | 自查 |
|--------|----------|------|
| C# 基础 (类、接口、泛型、反射) | ⭐⭐⭐⭐⭐ | □ |
| Unity 基础 (MonoBehaviour、生命周期) | ⭐⭐⭐⭐⭐ | □ |
| 异步编程 (async/await) | ⭐⭐⭐⭐ | □ |
| 设计模式 (单例、工厂、观察者) | ⭐⭐⭐⭐ | □ |
| UGUI 基础 | ⭐⭐⭐ | □ |

> 💡 如果有欠缺，建议先补充相关知识再开始。

---

## 🗺️ 学习路线图

```
第 1 周: 框架核心三件套 ──────→ 理解框架的"骨架"
         ↓
第 2 周: 配置与资源系统 ──────→ 理解数据与资源流
         ↓
第 3 周: UI 框架系统 ─────────→ 理解界面架构
         ↓
第 4 周: 场景与 ECS 架构 ─────→ 理解游戏世界构建
         ↓
第 5-6 周: 实战练习 ──────────→ 综合应用
```

---

## 📅 第 1 周：框架核心三件套

**目标**: 理解框架的基础运行机制

### Day 1-2: ManagerProvider (依赖注入容器)

**阅读文档**:
- `./Assets/Scripts/Docs/SystemGuides/框架三件套-Core-Timer-Messager.md`
- `./FRAMEWORK_ARCHITECTURE.md` 第 1 章

**核心概念**:
- 服务定位器模式
- Manager 生命周期 (Init/Update/Destroy)
- 依赖注入与解耦

**实践任务**:
```csharp
// 1. 创建一个简单的 TestManager
public class TestManager : Manager, IUpdate
{
    public override void Init() { }
    public void Update() { }
    public override void Destroy() { }
}

// 2. 在 Entry 中注册并测试
ManagerProvider.RegisterManager<TestManager>();
var test = ManagerProvider.GetManager<TestManager>();
```

**验收标准**:
- [ ] 能解释 ManagerProvider 的作用
- [ ] 能创建并注册自定义 Manager
- [ ] 理解 Init/Update/Destroy 的调用时机

---

### Day 3-4: Messager (事件系统)

**阅读文档**:
- `./Assets/Scripts/Docs/SystemGuides/框架三件套-Core-Timer-Messager.md`
- `./FRAMEWORK_ARCHITECTURE.md` 第 2 章

**核心概念**:
- 观察者模式
- 消息类型定义
- 异步消息处理

**实践任务**:
```csharp
// 1. 定义消息类型
public enum MessageType { TestMessage }

// 2. 创建消息类
public class TestMessage : IMessage { public string Content; }

// 3. 创建处理器
[MessagerHandler(MessageType.TestMessage)]
public class TestHandler : AMessagerHandler<TestMessage>
{
    public override ETTask Run(TestMessage message)
    {
        Log.Info($"收到消息：{message.Content}");
        return ETTask.CompletedTask;
    }
}

// 4. 发送消息
Messager.Instance.SendMessage(new TestMessage { Content = "Hello" });
```

**验收标准**:
- [ ] 能定义消息类型和消息类
- [ ] 能创建消息处理器
- [ ] 理解消息的发送与接收流程

---

### Day 5-6: TimerManager (定时器系统)

**阅读文档**:
- `./Assets/Scripts/Docs/SystemGuides/框架三件套-Core-Timer-Messager.md`
- `./FRAMEWORK_ARCHITECTURE.md` 第 3 章

**核心概念**:
- 延时调用
- 循环定时器
- 定时器取消

**实践任务**:
```csharp
// 1. 一次性延时
TimerManager.Instance.Add(1000, () => {
    Log.Info("1 秒后执行");
});

// 2. 循环定时器
TimerManager.Instance.AddRepeating(500, 5, () => {
    Log.Info("每 0.5 秒执行一次，共 5 次");
});

// 3. 取消定时器
var timer = TimerManager.Instance.Add(5000, callback);
TimerManager.Instance.Remove(timer);
```

**验收标准**:
- [ ] 能使用一次性定时器
- [ ] 能使用循环定时器
- [ ] 理解定时器的取消机制

---

### Day 7: 周复习与小项目

**综合练习**: 创建一个简单的计数器功能
- 使用 ManagerProvider 创建 CounterManager
- 使用 TimerManager 实现每秒计数
- 使用 Messager 在计数达到特定值时发送事件

---

## 📅 第 2 周：配置与资源系统

**目标**: 理解游戏数据与资源的管理方式

### Day 8-9: ConfigManager (配置系统)

**阅读文档**:
- `./Assets/Scripts/Docs/SystemGuides/配置系统-ConfigManager.md`
- `./FRAMEWORK_ARCHITECTURE.md` 第 5 章

**核心概念**:
- Excel → 代码生成
- Nino 序列化
- 配置热更

**实践任务**:
```csharp
// 1. 查看已有配置 (Assets/Scripts/Code/Module/Config/)
// 2. 读取配置
var itemConfig = ConfigManager.Instance.GetConfig<ItemConfig>(1001);
// 3. 遍历配置
var allItems = ConfigManager.Instance.GetAllConfig<ItemConfig>();
```

**验收标准**:
- [ ] 理解配置的加载流程
- [ ] 能读取和使用配置数据
- [ ] 了解配置表的生成方式

---

### Day 10-11: ResourcesManager (资源管理)

**阅读文档**:
- `./FRAMEWORK_ARCHITECTURE.md` 第 6 章
- `./Assets/Scripts/Mono/ResourceManager/` 对应文档

**核心概念**:
- 资源加载 (同步/异步)
- 资源引用计数
- 资源卸载

**实践任务**:
```csharp
// 1. 异步加载预制体
var prefab = await ResourcesManager.Instance.LoadAssetAsync<GameObject>("UIHome");

// 2. 同步加载
var sprite = ResourcesManager.Instance.LoadAsset<Sprite>("icon_coin");

// 3. 卸载资源
ResourcesManager.Instance.UnloadAsset("UIHome");
```

**验收标准**:
- [ ] 能异步加载资源
- [ ] 理解资源引用计数
- [ ] 知道何时卸载资源

---

### Day 12-13: GameObjectPoolManager (对象池)

**阅读文档**:
- `./FRAMEWORK_ARCHITECTURE.md` 第 7 章

**核心概念**:
- 对象复用
- 池化策略
- 性能优化

**实践任务**:
```csharp
// 1. 从池中获取对象
var obj = await GameObjectPoolManager.Instance.GetObjectAsync("Enemy");

// 2. 归还对象到池
GameObjectPoolManager.Instance.ReturnObject(obj);
```

**验收标准**:
- [ ] 理解对象池的作用
- [ ] 能正确使用对象池
- [ ] 知道对象池的性能优势

---

### Day 14: 周复习

**综合练习**: 创建一个简单的物品展示功能
- 从配置读取物品数据
- 异步加载物品图标
- 使用对象池管理物品卡片

---

## 📅 第 3 周：UI 框架系统

**目标**: 掌握完整的 UI 开发流程

### Day 15-16: UI 系统基础

**阅读文档**:
- `./Assets/Scripts/Docs/SystemGuides/UI 系统-UIManager.md`
- `./Assets/Scripts/Code/Module/UI/UI_BASE_CLASS_GUIDE.md`
- `./FRAMEWORK_ARCHITECTURE.md` 第 8 章

**核心概念**:
- UI 层级管理
- UI 生命周期
- UI 对象池

**实践任务**:
```csharp
// 1. 打开窗口
var window = await UIManager.Instance.OpenWindow<UIHome>("UIHome", UILayerNames.Normal);

// 2. 关闭窗口
await UIManager.Instance.CloseWindow(window.View);

// 3. 获取已打开的窗口
var homeView = UIManager.Instance.GetWindow<UIHomeView>();
```

**验收标准**:
- [ ] 理解 UI 层级结构
- [ ] 能打开/关闭窗口
- [ ] 理解 UI 的生命周期方法

---

### Day 17-18: UI 基类与继承

**阅读文档**:
- `./Assets/Scripts/Code/Module/UI/UI_BASE_CLASS_GUIDE.md`

**核心概念**:
- `UIBaseView` - 基础视图类
- `UIWindow` - 窗口管理类
- 生命周期方法 (`OnInit`, `OnOpen`, `OnClose`, `OnDestroy`)

**实践任务**:
```csharp
// 1. 创建视图类
public class UIHomeView : UIBaseView
{
    // 绑定 UI 元素
    private Button btnClick;
    
    protected override void OnInit()
    {
        btnClick = GetUIComponent<Button>("BtnClick");
        btnClick.onClick.AddListener(OnClick);
    }
    
    protected override void OnOpen(object data)
    {
        // 窗口打开时的逻辑
    }
    
    private void OnClick() { }
    
    protected override void OnDestroy()
    {
        btnClick.onClick.RemoveListener(OnClick);
    }
}
```

**验收标准**:
- [ ] 能创建自定义 UI 视图
- [ ] 理解生命周期方法的调用顺序
- [ ] 能正确绑定和解绑 UI 事件

---

### Day 19-20: UI 实战练习

**综合练习**: 创建一个完整的 UI 流程
- 登录界面 → 主界面 → 设置界面
- 实现界面之间的跳转
- 使用事件系统传递数据

---

### Day 21: 周复习

---

## 📅 第 4 周：场景与 ECS 架构

**目标**: 理解游戏世界构建与实体系统

### Day 22-23: SceneManager (场景系统)

**阅读文档**:
- `./Assets/Scripts/Docs/SystemGuides/场景系统-SceneManager.md`
- `./FRAMEWORK_ARCHITECTURE.md` 第 9 章

**核心概念**:
- 场景加载
- 场景切换
- 场景生命周期

**实践任务**:
```csharp
// 1. 加载场景
await SceneManager.Instance.LoadScene("MainScene");

// 2. 场景切换 (带过渡)
await SceneManager.Instance.ChangeScene("BattleScene");
```

**验收标准**:
- [ ] 能加载和切换场景
- [ ] 理解场景的生命周期
- [ ] 了解场景间的资源管理

---

### Day 24-25: ECS 架构 (Entity-Component)

**阅读文档**:
- `./Assets/Scripts/Docs/SystemGuides/ECS 架构-Entity-Component.md`
- `./FRAMEWORK_ARCHITECTURE.md` 第 4 章

**核心概念**:
- Entity (实体)
- Component (组件)
- 组合优于继承

**实践任务**:
```csharp
// 1. 创建实体
var entity = EntityManager.Instance.CreateEntity();

// 2. 添加组件
entity.AddComponent<PositionComponent>();
entity.AddComponent<MovementComponent>();

// 3. 获取组件
var pos = entity.GetComponent<PositionComponent>();
```

**验收标准**:
- [ ] 理解 ECS 的基本概念
- [ ] 能创建实体和组件
- [ ] 理解组合与继承的区别

---

### Day 26-27: 其他核心模块

**阅读文档**:
- `./FRAMEWORK_ARCHITECTURE.md` 第 10 章 (PlayerManager)
- `./GAMEPLAY_SYSTEMS.md`

**了解内容**:
- PlayerManager (玩家管理)
- NumericSystem (数值系统)
- 其他玩法模块

---

### Day 28: 周复习

---

## 📅 第 5-6 周：实战练习

**目标**: 综合运用所学知识

### 项目：简单的 RPG 演示

**功能需求**:
1. 登录界面 → 主界面
2. 角色信息展示 (从配置读取)
3. 简单的战斗场景
4. 使用事件系统解耦各模块

**技术要点**:
- [ ] ManagerProvider 管理所有模块
- [ ] Messager 处理跨模块通信
- [ ] TimerManager 处理延时逻辑
- [ ] ConfigManager 读取角色/物品配置
- [ ] ResourcesManager 加载资源
- [ ] GameObjectPoolManager 对象池优化
- [ ] UIManager 管理所有界面
- [ ] SceneManager 切换场景
- [ ] Entity-Component 构建角色系统

---

## 📚 文档阅读顺序总结

### 必读核心文档 (按顺序)

1. `./README.md` - 项目概览
2. `./Assets/Scripts/Docs/SystemGuides/框架三件套-Core-Timer-Messager.md`
3. `./FRAMEWORK_ARCHITECTURE.md` - 框架架构详解
4. `./Assets/Scripts/Docs/SystemGuides/UI 系统-UIManager.md`
5. `./Assets/Scripts/Docs/SystemGuides/场景系统-SceneManager.md`
6. `./Assets/Scripts/Docs/SystemGuides/ECS 架构-Entity-Component.md`
7. `./Assets/Scripts/Docs/SystemGuides/配置系统-ConfigManager.md`
8. `./Assets/Scripts/Docs/SystemGuides/数值系统-NumericSystem.md`
9. `./GAMEPLAY_SYSTEMS.md` - 玩法系统概览

### 参考文档

- `./Assets/Scripts/Code/Module/UI/UI_BASE_CLASS_GUIDE.md` - UI 开发速查表
- `./Assets/Scripts/Code/Module/DOCUMENT_PROGRESS.md` - 文档进度追踪
- 各模块对应的 `.md` 注解文档

---

## 💡 学习建议

### ✅ 推荐做法

1. **边读边练** - 每学一个概念就写代码验证
2. **做笔记** - 记录关键 API 和使用场景
3. **看源码** - 文档不懂时直接看 `.cs` 源文件
4. **问问题** - 遇到卡点及时记录并寻求解答
5. **复现功能** - 尝试复现框架中的示例功能

### ❌ 避免做法

1. **只看不练** - 不写代码永远学不会
2. **跳步学习** - 基础不牢后面会吃力
3. **死记硬背** - 理解原理比记忆 API 更重要
4. **忽视文档** - 框架文档非常完善，善用它们

---

## 📝 学习记录

### 每日进度

| 日期 | 学习内容 | 完成度 | 备注 |
|------|----------|--------|------|
| | | | |

### 遇到的问题

| 问题 | 解决方案 | 日期 |
|------|----------|------|
| | | |

### 重要笔记

---

## 🎯 完成标志

学完本计划后，你应该能够：

- [ ] 独立创建新的 Manager 模块
- [ ] 使用事件系统解耦模块间通信
- [ ] 开发完整的 UI 界面流程
- [ ] 管理游戏资源和场景
- [ ] 使用 ECS 架构设计游戏实体
- [ ] 阅读框架源码并理解其设计

---

<div align="center">

**🚀 祝学习顺利！有问题随时查阅文档或询问。**

</div>
