# INumericReplace.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | INumericReplace.cs |
| **路径** | Assets/Scripts/Code/Game/Component/Numeric/INumericReplace.cs |
| **所属模块** | 游戏层 → Component/Numeric |
| **文件职责** | 数值替换接口，支持动态获取替换值 |

---

## 接口说明

### INumericReplace

| 属性 | 说明 |
|------|------|
| **职责** | 定义数值替换能力，允许通过键名动态获取替换值 |
| **类型** | 接口 |
| **使用场景** | 公式计算中需要动态替换变量值的场景 |

---

## 方法说明

### GetReplaceValue()

**签名**:
```csharp
public float GetReplaceValue(string key)
```

**职责**: 根据键名获取替换值

**参数**:
- `key`: 替换键名（如 "Level", "Strength", "Attack" 等）

**返回值**:
- `float`: 替换值的浮点数表示

**使用示例**:
```csharp
// 实现类示例
public class PlayerNumericReplacer : INumericReplace
{
    private Player player;
    
    public float GetReplaceValue(string key)
    {
        return key switch
        {
            "Level" => player.Level,
            "Strength" => player.Strength,
            "Attack" => player.Attack,
            _ => 0f
        };
    }
}

// 使用
var replacer = new PlayerNumericReplacer(player);
float levelValue = replacer.GetReplaceValue("Level");
```

---

## 设计说明

### 用途

此接口用于支持**动态公式计算**场景：

```
公式字符串： "BaseAttack * (1 + Level * 0.1)"
                    ↓
解析后需要替换 "Level" 为实际值
                    ↓
通过 INumericReplace.GetReplaceValue("Level") 获取
```

### 典型应用场景

1. **技能伤害公式**: 使用角色等级、属性等变量
2. **装备属性计算**: 根据强化等级动态计算
3. **Buff 效果**: 根据施法者属性动态调整

---

## 实现示例

### 玩家数值替换器

```csharp
public class PlayerNumericReplacer : INumericReplace
{
    private NumericComponent numericComponent;
    
    public float GetReplaceValue(string key)
    {
        // 从配置表获取数值类型 ID
        if (NumericConfig.TryGetValue(key, out int numericType))
        {
            return numericComponent.GetAsFloat(numericType);
        }
        return 0f;
    }
}
```

### 实体数值替换器

```csharp
public class EntityNumericReplacer : INumericReplace
{
    private Entity entity;
    
    public float GetReplaceValue(string key)
    {
        return key switch
        {
            "HP" => entity.HP,
            "MaxHP" => entity.MaxHP,
            "Level" => entity.Level,
            "Exp" => entity.Exp,
            _ => 0f
        };
    }
}
```

---

## 相关文档

- [NumericComponent.cs.md](./NumericComponent.cs.md) - 数值组件
- [FormulaValue.cs.md](../../Module/Config/Value/FormulaValue.cs.md) - 公式值
- [NumericSystem.cs.md](./NumericSystem.cs.md) - 数值系统

---

*文档生成时间：2026-03-02 | OpenClaw AI 助手*
