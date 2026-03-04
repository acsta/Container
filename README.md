# Container - Unity 游戏框架

> **版本**: v1.0  
> **引擎**: Unity 2020+  
> **语言**: C#  
> **文档状态**: ✅ 100% 完成 (450/450 文件)

一个组件式的 Unity 游戏框架，提供完整的模块化架构、资源管理、UI 系统和配置工具链。

---

## 🚀 核心特性

| 特性 | 说明 |
|------|------|
| **🧩 组件式架构** | 基于 ManagerProvider 的依赖注入容器，模块解耦 |
| **📦 资源管理** | 基于 YooAsset（可替换），支持动态图集/SpriteAtlas |
| **🎨 UI 框架** | 完整的 UGUI 封装，支持层级管理/对象池/生命周期 |
| **📊 配置系统** | Excel → Nino 序列化，支持配置热更 |
| **⏱️ 异步任务** | ETTask 协程系统，轻量级 async/await 实现 |
| **🔥 热更新** | 预留 HybridCLR/ILRuntime 接入点 |

---

## 📖 文档导航

### 📘 系统理解指南 (System Guides)

适合快速了解核心系统的设计理念和使用方法：

| 文档 | 内容 |
|------|------|
| [框架三件套](./Assets/Scripts/Docs/SystemGuides/框架三件套-Core-Timer-Messager.md) | ManagerProvider + TimerManager + Messager |
| [UI 系统](./Assets/Scripts/Docs/SystemGuides/UI 系统-UIManager.md) | UI 框架完整架构与使用 |
| [场景系统](./Assets/Scripts/Docs/SystemGuides/场景系统-SceneManager.md) | 场景加载/切换/资源管理 |
| [ECS 架构](./Assets/Scripts/Docs/SystemGuides/ECS 架构-Entity-Component.md) | Entity-Component 系统设计 |
| [配置系统](./Assets/Scripts/Docs/SystemGuides/配置系统-ConfigManager.md) | 配置加载/解析/使用 |
| [数值系统](./Assets/Scripts/Docs/SystemGuides/数值系统-NumericSystem.md) | 游戏数值框架 |

### 📁 代码文档 (按目录)

每个 `.cs` 文件都有对应的 `.md` 注解文档：

```
Assets/Scripts/
├── Code/                    # 玩法层代码
│   ├── Entry.cs.md         # 游戏入口
│   └── Module/             # 模块目录
│       ├── UI/             # UI 系统 (79 个文档)
│       ├── Config/         # 配置系统 (45 个文档)
│       ├── Scene/          # 场景系统 (12 个文档)
│       └── ...             # 其他模块
├── Mono/                    # 框架层代码
│   ├── Define/             # 基础定义
│   ├── Init/               # 初始化流程
│   └── ...
└── ThirdParty/              # 第三方库 (可选文档)
    └── ETTask/             # 异步任务系统 (11 个文档)
```

### 📊 架构文档

| 文档 | 说明 |
|------|------|
| [FRAMEWORK_ARCHITECTURE.md](./FRAMEWORK_ARCHITECTURE.md) | 框架层架构详解 |
| [GAMEPLAY_SYSTEMS.md](./GAMEPLAY_SYSTEMS.md) | 玩法系统概览 |
| [PROJECT_DOCUMENTATION.md](./PROJECT_DOCUMENTATION.md) | 项目文档总览 |

### 🛠️ 开发指南

| 文档 | 说明 |
|------|------|
| [UI 基类快速参考](./Assets/Scripts/Code/Module/UI/UI_BASE_CLASS_GUIDE.md) | UI 开发速查表 |
| [文档进度追踪](./Assets/Scripts/Code/Module/DOCUMENT_PROGRESS.md) | 文档化工作记录 |

---

## 🏗️ 架构概览

```
┌─────────────────────────────────────────────────────────┐
│                      游戏入口 (Entry)                    │
└─────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────┐
│                   ManagerProvider                        │
│            (依赖注入 + 生命周期管理)                      │
└─────────────────────────────────────────────────────────┘
         │              │              │
         ▼              ▼              ▼
┌─────────────┐ ┌─────────────┐ ┌─────────────┐
│   Timer     │ │   Messager  │ │   Core      │
│  Manager    │ │   (事件)    │ │  Modules    │
└─────────────┘ └─────────────┘ └─────────────┘
                                         │
                    ┌────────────────────┼────────────────────┐
                    ▼                    ▼                    ▼
             ┌───────────┐        ┌───────────┐        ┌───────────┐
             │    UI     │        │   Scene   │        │  Config   │
             │  Manager  │        │  Manager  │        │  Manager  │
             └───────────┘        └───────────┘        └───────────┘
```

### 核心模块

| 模块 | 职责 | 关键类 |
|------|------|--------|
| **ManagerProvider** | 依赖注入容器 | `ManagerProvider`, `IManager` |
| **TimerManager** | 定时器/延时 | `TimerManager`, `Timer` |
| **Messager** | 事件总线 | `Messager`, `MessageAttribute` |
| **UIManager** | UI 管理 | `UIManager`, `UIWindow`, `UIBaseView` |
| **SceneManager** | 场景切换 | `SceneManager`, `IScene` |
| **ConfigManager** | 配置加载 | `ConfigManager`, `ConfigAttribute` |
| **EntityManager** | ECS 实体 | `Entity`, `Component` |

---

## 🚦 快速开始

### 1. 游戏启动流程

```csharp
// Unity 自动调用 Entry.Start()
// → Entry.StartAsync()
//   → 注册所有 Manager
//   → 初始化配置/资源/UI
//   → 加载初始场景
//   → 进入游戏主循环
```

### 2. 创建新 Manager

```csharp
public class MyManager : Manager, IUpdate
{
    public override void Init()
    {
        // 初始化逻辑
    }

    public void Update()
    {
        // 每帧更新
    }

    public override void Destroy()
    {
        // 清理资源
    }
}

// 注册
ManagerProvider.RegisterManager<MyManager>();
```

### 3. 打开 UI 窗口

```csharp
// 异步打开窗口
var window = await UIManager.Instance.OpenWindow<UIHome>("UIHome", UILayerNames.Normal);

// 关闭窗口
await UIManager.Instance.CloseWindow(window.View);
```

### 4. 发送/监听事件

```csharp
// 监听事件
[MessagerHandler(MessageType.PlayerLevelUp)]
public class PlayerLevelUpHandler : AMessagerHandler<PlayerLevelUp>
{
    public override async ETTask Run(PlayerLevelUp message)
    {
        Log.Info($"玩家升级到 {message.Level} 级");
    }
}

// 发送事件
Messager.Instance.SendMessage(new PlayerLevelUp { Level = 10 });
```

---

## 📂 项目结构

```
container-repo/
├── Assets/
│   └── Scripts/
│       ├── Code/              # 玩法层（游戏逻辑）
│       │   ├── Entry.cs       # 游戏入口
│       │   └── Module/        # 功能模块
│       ├── Mono/              # 框架层（核心框架）
│       │   ├── Define/        # 基础定义
│       │   ├── ManagerProvider/
│       │   ├── TimerManager/
│       │   └── Messager/
│       ├── ThirdParty/        # 第三方库
│       │   ├── ETTask/        # 异步任务
│       │   ├── Nino/          # 序列化
│       │   └── ...
│       └── Docs/              # 文档
│           └── SystemGuides/  # 系统理解指南
├── FRAMEWORK_ARCHITECTURE.md  # 框架架构详解
├── GAMEPLAY_SYSTEMS.md        # 玩法系统概览
└── README.md                  # 本文件
```

---

## 📊 文档统计

| 类别 | .cs 文件 | .md 文档 | 覆盖率 |
|------|---------|---------|--------|
| **Code/Runtime** | 268 | 268 | ✅ 100% |
| **Mono** | 103 | 103 | ✅ 100% |
| **Editor** | 79 | 79 | ✅ 100% |
| **ThirdParty** | 125 | 11 | 可选 |
| **总计** | 646 | 461 | - |
| **需文档化** | 450 | 450 | ✅ 100% |

> 最后更新：2026-03-03

---

## 🛠️ 技术栈

| 类别 | 技术 |
|------|------|
| **引擎** | Unity 2020+ |
| **语言** | C# 9.0+ |
| **资源** | YooAsset |
| **序列化** | Nino |
| **异步** | ETTask (类 Task) |
| **UI** | UGUI |
| **配置** | Excel + Nino |

---

## 🔗 相关链接

- [框架架构详解](./FRAMEWORK_ARCHITECTURE.md)
- [玩法系统概览](./GAMEPLAY_SYSTEMS.md)
- [UI 基类指南](./Assets/Scripts/Code/Module/UI/UI_BASE_CLASS_GUIDE.md)
- [文档进度](./Assets/Scripts/Code/Module/DOCUMENT_PROGRESS.md)

---

## 📝 许可证

内部项目，请勿外传。

---

<div align="center">

**📚 所有代码已文档化，欢迎查阅！**

</div>
