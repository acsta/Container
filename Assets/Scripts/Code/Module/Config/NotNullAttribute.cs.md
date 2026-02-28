# NotNullAttribute.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | NotNullAttribute.cs |
| **路径** | Assets/Scripts/Code/Module/Config/NotNullAttribute.cs |
| **所属模块** | 框架层 → Code/Module/Config |
| **文件职责** | 定义非空标记特性，用于标识字段不应为空 |

---

## 类/结构体说明

### NotNullAttribute

| 属性 | 说明 |
|------|------|
| **职责** | 标记字段或属性不应为 null，用于代码分析和验证 |
| **泛型参数** | 无 |
| **继承关系** | 继承 `System.Attribute` |
| **实现的接口** | 无 |

**设计模式**: 标记特性（Marker Attribute）

```csharp
// 使用方式
public class PlayerConfig : ProtoObject
{
    [NotNull]
    public string Name;  // 标记为不能为空
    
    [NotNull]
    public List<SkillConfig> Skills;  // 集合不能为 null（但可以为空）
}
```

---

## 特性定义

### 继承自 System.Attribute

```csharp
public class NotNullAttribute: Attribute
```

**说明**: 直接继承 .NET 标准 `Attribute` 类，是最简单的特性定义方式。

---

## 使用场景

### 1. 代码分析工具

```csharp
// 静态分析工具可以检查 [NotNull] 标记的字段
public class QuestConfig : ProtoObject
{
    [NotNull]
    public string Title;  // 分析工具会警告 null 赋值
    
    public void Init()
    {
        Title = null;  // ⚠️ 警告：NotNull 字段不应为 null
    }
}
```

### 2. 反序列化验证

```csharp
// 反序列化后可以验证 [NotNull] 字段
public override void EndInit()
{
    base.EndInit();
    
    // 检查所有 [NotNull] 字段
    var fields = GetType().GetFields()
        .Where(f => f.GetCustomAttribute<NotNullAttribute>() != null);
    
    foreach (var field in fields)
    {
        if (field.GetValue(this) == null)
        {
            Debug.LogError($"{field.Name} 不应为 null!");
        }
    }
}
```

### 3. Odin Inspector 集成

```csharp
// Odin Inspector 可以识别 [NotNull] 并显示验证
public class EditorConfig : MonoBehaviour
{
    [NotNull]  // Odin 会在 Inspector 中标记 null 值
    public Texture2D Icon;
    
    [NotNull]  // 右键菜单快速赋值
    [ContextMenu("Assign Default")]
    void AssignDefault()
    {
        Icon = Resources.Load<Texture2D>("DefaultIcon");
    }
}
```

---

## 与其他特性的关系

### 与 JetBrains 的 [NotNull] 对比

```csharp
// JetBrains.Annotations.NotNull
using JetBrains.Annotations;

public class Config1 : ProtoObject
{
    [NotNull]  // JetBrains 版本，支持 ReSharper/Rider
    public string Name;
}

// 本项目 TaoTie.NotNull
using TaoTie;

public class Config2 : ProtoObject
{
    [NotNull]  // 本项目的版本，功能相同
    public string Name;
}
```

**说明**: 两个 `NotNull` 功能相同，可以互换使用。本项目定义自己的版本以避免外部依赖。

### 与 Unity 的 [SerializeField] 对比

```csharp
public class UnityConfig : MonoBehaviour
{
    [SerializeField]  // Unity 序列化
    [NotNull]         // 非空验证
    private string playerName;
}
```

---

## 使用示例

### 示例 1: 配置类字段标记

```csharp
[Config]
public class HeroConfig : ProtoObject
{
    [NotNull]
    public string HeroName;  // 英雄名称不能为空
    
    [NotNull]
    public List<int> SkillIds;  // 技能列表不能为 null
    
    public string Description;  // 描述可以为空
}
```

### 示例 2: 构造函数初始化

```csharp
public class PlayerData : ProtoObject
{
    [NotNull]
    public string PlayerId;
    
    [NotNull]
    public List<ItemData> Inventory;
    
    public PlayerData()
    {
        PlayerId = Guid.NewGuid().ToString();  // 立即赋值
        Inventory = new List<ItemData>();       // 初始化为空列表
    }
}
```

### 示例 3: 反序列化后验证

```csharp
public class QuestConfig : ProtoObject
{
    [NotNull]
    public string QuestName;
    
    public override void EndInit()
    {
        base.EndInit();
        
        if (QuestName == null)
        {
            Debug.LogError($"[NotNull] 验证失败：QuestName 为 null");
            QuestName = "未命名任务";  // 设置默认值
        }
    }
}
```

---

## 设计要点

### 为什么需要 NotNull？

1. **代码清晰**: 明确表达字段的设计意图
2. **减少 Bug**: 避免 null 引用异常
3. **工具支持**: IDE 和静态分析工具可以提供警告
4. **文档作用**: 代码即文档，其他开发者知道哪些字段不能为空

### 最佳实践

1. **集合字段**: 始终标记为 `[NotNull]`，初始化为空集合而非 null
   ```csharp
   [NotNull]
   public List<int> Ids = new List<int>();  // ✅ 好
   public List<int> Ids = null;              // ❌ 坏
   ```

2. **字符串字段**: 根据业务逻辑决定是否允许 null
   ```csharp
   [NotNull]
   public string Id;          // ID 通常不应为空
   public string Description; // 描述可以为空
   ```

3. **引用类型**: 关键引用应标记为 `[NotNull]`
   ```csharp
   [NotNull]
   public Transform SpawnPoint;  // 出生点必须有
   public Transform Effect;      // 特效可以有可以无
   ```

---

## 相关文档

- [ConfigAttribute.cs.md](./ConfigAttribute.cs.md) - 配置标记特性
- [ProtoObject.cs.md](./ProtoObject.cs.md) - 配置对象基类（可在 EndInit 中验证）
- [OdinDropdownHelper.cs.md](./OdinDropdownHelper.cs.md) - Odin Inspector 工具（可使用 [NotNull]）

---

*文档生成时间：2026-02-28 | OpenClaw AI 助手*
