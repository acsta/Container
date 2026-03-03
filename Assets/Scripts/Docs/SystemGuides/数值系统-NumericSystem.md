# 数值系统理解指南 - NumericSystem

> **文档类型**: 系统理解指南  
> **适用范围**: Code/Game/System/Numeric  
> **生成时间**: 2026-03-03  
> **前置知识**: ECS 架构、定时器、配置表

---

## 📑 概述

数值系统负责管理游戏实体的数值属性（如 HP、攻击、防御等），支持数值恢复（回血/回蓝）和动态变化。

**核心职责**:
- 管理所有 NumericComponent 实例
- 定时检测并恢复数值（如回血、回蓝）
- 支持数值关联（如最大 HP 变化时当前 HP 也变化）
- 提供数值变化事件

**关键文件**:
| 文件 | 职责 |
|------|------|
| `NumericSystem.cs` | 数值管理核心 |
| `NumericComponent.cs` | 数值组件 |
| `NumericType.cs` | 数值类型定义 |
| `AttributeConfig.cs` | 属性配置 |

---

## 🎯 系统职责

### 解决的核心问题

1. **数值恢复**: 角色需要自动回血/回蓝
2. **数值关联**: 最大 HP 变化时，当前 HP 应该按比例变化
3. **性能优化**: 大量实体数值更新需要高效处理
4. **配置驱动**: 数值恢复规则由配置表定义

### 设计思路

```
数值系统设计:
1. NumericComponent: 存储实体数值数据
2. NumericSystem: 管理所有 NumericComponent
3. 定时器：每 250ms 检查一次数值变化
4. 配置表：定义哪些属性需要恢复、恢复速度、最大值

恢复逻辑:
每 250ms 检查一次:
  当前值 += 恢复速度 × (250ms / 1000ms)
  如果 当前值 > 最大值，则 当前值 = 最大值
```

---

## 🏗️ 架构设计

### 核心类图

```
┌─────────────────────────────────────────────────────────┐
│                   NumericSystem                          │
│  ┌─────────────────────────────────────────────────┐   │
│  │  Data: List<NumericComponent>                   │   │
│  │  所有数值组件列表                                │   │
│  └─────────────────────────────────────────────────┘   │
│  ┌─────────────────────────────────────────────────┐   │
│  │  Timer: long                                    │   │
│  │  定时器 ID                                       │   │
│  └─────────────────────────────────────────────────┘   │
│  ┌─────────────────────────────────────────────────┐   │
│  │  attrList/reUpList/maxList                      │   │
│  │  恢复属性/恢复速度/最大值 映射                   │   │
│  └─────────────────────────────────────────────────┘   │
│                                                          │
│  +NumericUpdate()                                       │
│  +AddComponent(component)                               │
│  +RemoveComponent(component)                            │
└─────────────────────────────────────────────────────────┘
                            │
                            │ 管理
                            ▼
┌─────────────────────────────────────────────────────────┐
│                  NumericComponent                        │
│─────────────────────────────────────────────────────────│
│ +numericDic: Dictionary<int, long>                     │
│ +floatDic: Dictionary<int, float>                      │
│─────────────────────────────────────────────────────────│
│ +Set(field, value)                                     │
│ +Get(field)                                            │
│ +GetAsFloat(field)                                     │
│ +Add(field, delta)                                     │
└─────────────────────────────────────────────────────────┘
```

---

## 🔄 核心流程

### 数值更新流程

```csharp
// 1. 定时器触发（每 250ms）
[Timer(TimerType.NumericUpdate)]
public class NumericUpdateTimer : ATimer<NumericSystem>
{
    public override void Run(NumericSystem t)
    {
        t.NumericUpdate();
    }
}

// 2. 数值更新逻辑
public void NumericUpdate()
{
    // 遍历所有数值组件
    for (int i = 0; i < Data.Count; i++)
    {
        var numc = Data[i];
        
        // 遍历所有需要恢复的属性
        for (int j = 0; j < attrList.Count; j++)
        {
            // 获取恢复速度
            float reUpNum = numc.GetAsFloat(reUpList[j]);
            
            // 按时间比例计算恢复量
            // 250ms / 1000ms = 0.25（每秒恢复量的 1/4）
            reUpNum = reUpNum * ATTRCHANGE_CHECKTIME / ATTRCHANGE_DELTATIME;
            
            if (reUpNum > 0)
            {
                // 获取最大值
                var maxValue = numc.GetAsInt(maxList[j]);
                
                // 获取当前值
                float nowValue = numc.GetAsInt(attrList[j]);
                
                // 恢复数值（不超过最大值）
                if (nowValue < maxValue)
                {
                    nowValue += reUpNum;
                    if (nowValue > maxValue) nowValue = maxValue;
                    numc.Set(attrList[j], nowValue);
                }
            }
        }
    }
}
```

---

### 初始化流程

```csharp
public void Init()
{
    attrList = new List<int>();
    reUpList = new List<int>();
    maxList = new List<int>();
    
    // 从配置表获取所有属性
    var attrs = AttributeConfigCategory.Instance.GetAllList();
    
    for (int i = 0; i < attrs.Count; i++)
    {
        // 检查是否有恢复属性和最大值属性
        if (!string.IsNullOrEmpty(attrs[i].AttrReUp) && 
            !string.IsNullOrEmpty(attrs[i].MaxAttr))
        {
            // 获取恢复速度和最大值的数值类型 ID
            var reup = NumericType.GetKey(attrs[i].AttrReUp);
            var max = NumericType.GetKey(attrs[i].MaxAttr);
            
            if (reup >= 0 && max >= 0)
            {
                // 加入映射列表（*10+1 是数值系统的编码规则）
                attrList.Add(attrs[i].Id * 10 + 1);
                reUpList.Add(reup * 10 + 1);
                maxList.Add(max * 10 + 1);
            }
        }
    }
    
    Data = new List<NumericComponent>();
}
```

---

### 组件管理流程

```csharp
// 添加数值组件
public void AddComponent(NumericComponent component)
{
    Data.Add(component);
}

// 移除数值组件
public void RemoveComponent(NumericComponent component)
{
    Data.Remove(component);
}

// Component 内部会自动注册/注销
public class NumericComponent : Component
{
    public override void Init()
    {
        base.Init();
        NumericSystem.Instance.AddComponent(this);
    }
    
    public override void Destroy()
    {
        NumericSystem.Instance.RemoveComponent(this);
        base.Destroy();
    }
}
```

---

## 💡 使用示例

### 配置表示例

```csharp
// AttributeConfig.csv
Id,Name,AttrReUp,MaxAttr
1001,HP,HPReUp,MaxHP
1002,MP,MPReUp,MaxMP
1003,Stamina,StaminaReUp,MaxStamina

// NumericType.csv
Id,Name
10010,HP
10011,HPReUp
10012,MaxHP
10020,MP
10021,MPReUp
10022,MaxMP
```

### 创建带数值的实体

```csharp
// 创建玩家
var player = EntityManager.Instance.Create<Player>();

// 添加数值组件
var numeric = player.AddComponent<NumericComponent>();

// 设置初始数值
numeric.Set(NumericType.HP, 100);      // 当前 HP
numeric.Set(NumericType.MaxHP, 100);   // 最大 HP
numeric.Set(NumericType.HPReUp, 10);   // 每秒恢复 10 HP

// 设置 MP
numeric.Set(NumericType.MP, 50);
numeric.Set(NumericType.MaxMP, 100);
numeric.Set(NumericType.MPReUp, 5);
```

### 数值变化

```csharp
// 受到伤害
numeric.Add(NumericType.HP, -20);  // HP -20

// 使用药水
numeric.Add(NumericType.HP, 30);   // HP +30

// 升级增加最大 HP
var maxHP = numeric.Get(NumericType.MaxHP);
numeric.Set(NumericType.MaxHP, maxHP + 10);

// 获取当前数值
int currentHP = numeric.GetAsInt(NumericType.HP);
float currentHPFloat = numeric.GetAsFloat(NumericType.HP);

// 检查是否死亡
if (numeric.GetAsInt(NumericType.HP) <= 0)
{
    Die();
}
```

### 自动恢复演示

```csharp
// 假设配置：
// HP 当前值：50
// HP 最大值：100
// HP 恢复速度：10/秒

// 250ms 后：
// HP = 50 + 10 × (250/1000) = 50 + 2.5 = 52.5

// 500ms 后：
// HP = 52.5 + 2.5 = 55

// ... 持续恢复直到 100

// 代码验证
var numeric = player.GetComponent<NumericComponent>();
numeric.Set(NumericType.HP, 50);
numeric.Set(NumericType.MaxHP, 100);
numeric.Set(NumericType.HPReUp, 10);

// 等待 1 秒
await TimerManager.Instance.WaitAsync(1000);

// HP 应该恢复到 60 左右（50 + 10）
int hp = numeric.GetAsInt(NumericType.HP);
Log.Info($"当前 HP: {hp}");
```

---

## 🔗 依赖关系

```
依赖:
├─→ GameTimerManager (定时器)
├─→ AttributeConfigCategory (属性配置)
├─→ NumericType (数值类型定义)
└─→ NumericComponent (数值组件)

被依赖:
├─→ 所有需要数值的实体（Player/NPC/Enemy）
├─→ 战斗系统（伤害计算）
└─→ 技能系统（Buff 效果）
```

---

## ⚠️ 注意事项

| 问题 | 说明 | 解决方案 |
|------|------|----------|
| 数值溢出 | 大数值可能超出 int 范围 | 使用 `GetAsFloat` 或 BigNumber |
| 恢复过快 | 帧率影响恢复速度 | 使用固定时间间隔（250ms） |
| 配置错误 | 配置表字段名错误 | 检查 `NumericType.GetKey()` 返回值 |
| 内存泄漏 | 组件移除后仍在列表 | 确保 `Destroy()` 中移除 |
| 性能问题 | 大量实体同时更新 | 限制检查频率，分批处理 |

---

## 🔍 设计原理

### 为什么使用 250ms 间隔？

```
100ms 间隔:
- 优点：恢复更平滑
- 缺点：CPU 开销大（每秒 10 次检查）

250ms 间隔（推荐）:
- 优点：平衡性能和流畅度
- 缺点：略微不精确（误差 < 250ms）

1000ms 间隔:
- 优点：CPU 开销最小
- 缺点：恢复不连贯，玩家能感觉到"跳变"

选择 250ms 的原因:
- 每秒 4 次检查，人眼难以察觉
- CPU 开销适中
- 误差在可接受范围内
```

### 数值编码规则

```
数值 ID 编码：fieldId * 10 + subType

subType:
0 = 基础值
1 = 当前值
2 = 最大值
3 = 恢复速度

示例:
HP 属性 ID = 1001
HP 当前值 ID = 1001 * 10 + 1 = 10011
HP 最大值 ID = 1001 * 10 + 2 = 10012
HP 恢复速度 ID = 1001 * 10 + 3 = 10013

优势:
- 统一管理所有数值
- 易于扩展新属性
- 配置表驱动
```

---

## 📚 相关文档

| 文档 | 说明 |
|------|------|
| [NumericComponent.cs.md](../../Component/Numeric/NumericComponent.cs.md) | 数值组件详细文档 |
| [TimerManager.cs.md](../../Mono/Module/Timer/TimerManager.cs.md) | 定时器系统 |
| [配置系统](./配置系统-ConfigManager.md) | 配置表管理 |

---

*文档由 OpenClaw AI 助手自动生成 | 数值系统理解指南*
