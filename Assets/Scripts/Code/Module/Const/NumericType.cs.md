# NumericType.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | NumericType.cs |
| **路径** | Assets/Scripts/Code/Module/Const/NumericType.cs |
| **所属模块** | 框架层 → Code/Module/Const |
| **文件职责** | 数值类型映射定义 |

---

## 类说明

### NumericType

| 属性 | 说明 |
|------|------|
| **职责** | 提供数值属性名称到 ID 的映射 |
| **类型** | static class |

---

## 字段与属性

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `Max` | `int` | `public const` | 最大数值类型 ID（10000） |
| `Map` | `Dictionary<string, int>` | `public static` | 名称到 ID 的映射字典 |

---

## 方法说明

### GetKey(string key)

**签名**:
```csharp
public static int GetKey(string key)
```

**职责**: 根据属性名称获取数值类型 ID

**参数**:
- `key`: 属性名称（如 "Attack", "Defense"）

**返回**: 数值类型 ID（找不到返回 -1）

**核心逻辑**:
```
1. 从 Map 字典查找
2. 如果找到，返回 ID
3. 如果未找到，记录错误，返回 -1
```

**调用者**: 数值系统

**使用示例**:
```csharp
int attackId = NumericType.GetKey("Attack");
if (attackId >= 0)
{
    // 使用 ID 访问数值
    numericComponent.Set(attackId, 100);
}
```

---

## 注册数值类型

### 方式 1: 静态注册

```csharp
// 在游戏初始化时注册
NumericType.Map["Attack"] = 1;
NumericType.Map["Defense"] = 2;
NumericType.Map["Speed"] = 3;
NumericType.Map["Health"] = 4;
```

### 方式 2: 动态注册

```csharp
// 从配置表加载时注册
foreach (var config in numericConfigs)
{
    NumericType.Map[config.Name] = config.Id;
}
```

---

## 使用示例

### 示例 1: 获取数值 ID

```csharp
// 获取攻击属性 ID
int attackId = NumericType.GetKey("Attack");

// 获取防御属性 ID
int defenseId = NumericType.GetKey("Defense");

// 获取不存在的属性（返回 -1）
int invalidId = NumericType.GetKey("Invalid");  // -1
```

### 示例 2: 设置数值

```csharp
// 获取数值组件
NumericComponent numeric = entity.GetComponent<NumericComponent>();

// 设置攻击力
int attackId = NumericType.GetKey("Attack");
if (attackId >= 0)
{
    numeric.Set(attackId, 100);
}

// 设置防御力
int defenseId = NumericType.GetKey("Defense");
if (defenseId >= 0)
{
    numeric.Set(defenseId, 50);
}
```

### 示例 3: 获取数值

```csharp
// 获取攻击力
int attackId = NumericType.GetKey("Attack");
if (attackId >= 0)
{
    int attack = numeric.Get(attackId);
    Log.Info($"攻击力：{attack}");
}
```

### 示例 4: 批量注册

```csharp
// 从配置表批量注册
void RegisterNumericTypes()
{
    NumericType.Map.Clear();
    
    NumericType.Map["Attack"] = 1;
    NumericType.Map["Defense"] = 2;
    NumericType.Map["Speed"] = 3;
    NumericType.Map["Health"] = 4;
    NumericType.Map["CritRate"] = 5;
    NumericType.Map["CritDamage"] = 6;
    // ... 更多属性
}
```

---

## 数值系统设计

### 为什么使用 ID 而不是字符串？

1. **性能**: 整数比字符串比较快
2. **内存**: 整数占用内存少
3. **类型安全**: 避免字符串拼写错误

### 映射表的作用

```
属性名称（字符串） → ID（整数） → 数值（int/long）
     "Attack"      →   1    →   100
     "Defense"     →   2    →    50
```

---

## 相关文档

- [NumericComponent.cs.md](../../Game/Component/Numeric/NumericComponent.cs.md) - 数值组件
- [NumericSystem.cs.md](../../Game/System/Numeric/NumericSystem.cs.md) - 数值系统

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
