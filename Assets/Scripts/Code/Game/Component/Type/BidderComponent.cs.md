# BidderComponent.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | BidderComponent.cs |
| **路径** | Assets/Scripts/Code/Game/Component/Type/BidderComponent.cs |
| **所属模块** | 游戏层 → Component/Type |
| **文件职责** | 竞拍者组件，标识实体为竞拍者并关联 AI 配置 |

---

## 类说明

### BidderComponent

| 属性 | 说明 |
|------|------|
| **职责** | 标记实体为竞拍者（Bidder），提供 AI 配置访问 |
| **泛型参数** | `IComponent<int>` - 使用 int 作为初始化参数 |
| **继承关系** | 继承 `Component` |
| **实现的接口** | `IComponent<int>` |

**使用场景**: 拍卖系统中的竞拍者 NPC

---

## 字段与属性

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `ConfigId` | `int` | `public private set` | AI 配置 ID，从配置表获取竞拍者 AI 行为 |
| `Config` | `AIConfig` | `public` | AI 配置对象（从配置表获取） |

---

## 方法说明

### Init()

**签名**:
```csharp
public void Init(int id)
```

**职责**: 初始化竞拍者组件

**参数**:
- `id`: AI 配置 ID

**核心逻辑**:
```
1. 设置 ConfigId = id
```

**调用者**: 组件系统创建组件时

---

### Destroy()

**签名**:
```csharp
public void Destroy()
```

**职责**: 销毁竞拍者组件

**核心逻辑**:
```
1. 重置 ConfigId = 0
```

**调用者**: 组件系统销毁组件时

---

## 使用示例

### 示例 1: 创建竞拍者实体

```csharp
// 创建竞拍者实体
var bidder = EntityFactory.CreateEntity<Entity>(scene.Id);

// 添加竞拍者组件（配置 ID=1001）
var bidderComponent = bidder.AddComponent<BidderComponent>();
bidderComponent.Init(1001);

// 访问 AI 配置
AIConfig config = bidderComponent.Config;
Log.Info($"竞拍者 AI: {config.Name}");
```

### 示例 2: 检查实体是否为竞拍者

```csharp
// 检查实体是否有竞拍者组件
if (entity.HasComponent<BidderComponent>())
{
    var bidder = entity.GetComponent<BidderComponent>();
    // 处理竞拍者逻辑
    StartAuctionBehavior(bidder.Config);
}
```

### 示例 3: 在拍卖系统中使用

```csharp
// 拍卖管理器中
public class AuctionManager
{
    private List<Entity> bidders = new List<Entity>();
    
    public void AddBidder(Entity bidder)
    {
        if (bidder.HasComponent<BidderComponent>())
        {
            bidders.Add(bidder);
            
            // 根据 AI 配置设置行为
            var config = bidder.GetComponent<BidderComponent>().Config;
            SetupAIBehavior(bidder, config);
        }
    }
}
```

---

## 设计说明

### 组件模式

BidderComponent 遵循**组件模式**：

```
实体 (Entity)
  └─ BidderComponent (竞拍者能力)
       └─ ConfigId → AIConfig (行为配置)
```

### AI 配置关联

通过 ConfigId 关联 AIConfig 配置表：

```csharp
// AIConfig 配置表结构示例
{
    Id: 1001,
    Name: "激进型竞拍者",
    MinBidIncrement: 100,
    MaxBidIncrement: 500,
    BidProbability: 0.8f,
    // ...
}
```

---

## 相关文档

- [Component.cs.md](../Component.cs.md) - 组件基类
- [IComponent.cs.md](../IComponent.cs.md) - 组件接口
- [Entity.cs.md](../../Entity/Entity.cs.md) - 实体基类
- [AIConfig.cs.md](../../../Module/Config/AIConfig.cs.md) - AI 配置表

---

*文档生成时间：2026-03-02 | OpenClaw AI 助手*
