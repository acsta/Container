# OdinDropdownHelper.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|------|
| **文件名** | OdinDropdownHelper.cs |
| **路径** | Assets/Scripts/Code/Module/Config/OdinDropdownHelper.cs |
| **所属模块** | 框架层 → Code/Module/Config |
| **文件职责** | 提供 Odin Inspector 下拉菜单数据源，简化配置编辑 |

---

## 文件说明

**条件编译**: `#if UNITY_EDITOR`

**说明**: 此文件仅在 Unity 编辑器中编译，不会包含在构建中。

---

## 类/结构体说明

### OdinDropdownHelper

| 属性 | 说明 |
|------|------|
| **职责** | 静态工具类，提供 Odin Inspector 下拉菜单的数据源生成方法 |
| **泛型参数** | 无 |
| **继承关系** | 无 |
| **实现的接口** | 无 |

**设计模式**: 工具类模式（静态方法集合）

```csharp
// 在配置类中使用
[ValueDropdown("@"+nameof(OdinDropdownHelper)+"."+nameof(OdinDropdownHelper.GetAIDecisionInterface)+"()")]
public string Condition;
```

---

## 内部类

### StringComparer

| 属性 | 说明 |
|------|------|
| **职责** | 实现 ValueDropdownItem<string> 的比较器，用于排序下拉菜单项 |
| **继承关系** | 实现 `IComparer<ValueDropdownItem<string>>` |

---

## 方法说明

### GetAIDecisionInterface

**签名**:
```csharp
public static IEnumerable GetAIDecisionInterface()
```

**职责**: 扫描 AIDecisionInterface 类的所有静态方法，生成下拉菜单选项

**核心逻辑**:
```
1. 获取 AIDecisionInterface 类的所有方法
2. 过滤出静态方法
3. 读取方法的 LabelText 特性获取友好名称
4. 读取 Tooltip 特性获取提示文本
5. 生成 ValueDropdownList<string>
6. 按字母顺序排序
7. 返回列表
```

**返回值**: `IEnumerable` - Odin 下拉菜单数据源

**显示格式**: `友好名称 (方法名)   提示文本`

**示例输出**:
```
- 距离检查 (DistanceCheck)   检查目标距离
- 血量检查 (HealthCheck)     检查当前血量
- 金钱检查 (MoneyCheck)      检查是否有足够金钱
```

---

### GetEmoji

**签名**:
```csharp
public static IEnumerable GetEmoji()
```

**职责**: 生成表情 ID 的下拉菜单选项

**核心逻辑**:
```
1. 创建 ValueDropdownList<string>
2. 添加"无"选项（空字符串）
3. 添加数字选项 "0" - "6"
4. 返回列表
```

**返回值**: `IEnumerable` - 表情下拉菜单数据源

**选项列表**:
| 显示 | 值 | 说明 |
|------|-----|------|
| 无 | "" | 无表情 |
| 0 | "0" | 表情 0 |
| 1 | "1" | 表情 1 |
| 2 | "2" | 表情 2 |
| 3 | "3" | 表情 3 |
| 4 | "4" | 表情 4 |
| 5 | "5" | 表情 5 |
| 6 | "6" | 表情 6 |

---

### GetNumericFinalTypeId

**签名**:
```csharp
public static IEnumerable GetNumericFinalTypeId()
```

**职责**: 扫描 NumericType 枚举的所有字段，生成数值类型下拉菜单

**核心逻辑**:
```
1. 获取 NumericType 类的所有字段
2. 过滤出静态字段
3. 过滤掉 Max 及以上的值
4. 生成 ValueDropdownList<int>
5. 显示格式：字段名 (值)
6. 返回列表
```

**返回值**: `IEnumerable` - 数值类型下拉菜单数据源

**示例输出**:
```
- HP(1)
- MaxHP(2)
- STR(3)
- INT(4)
- Level(5)
- ...
```

---

## 使用示例

### 示例 1: AI 条件下拉菜单

```csharp
// 在 DecisionConditionNode 中
public partial class DecisionConditionNode: DecisionNode
{
    [NinoMember(10)]
    #if UNITY_EDITOR
    [ValueDropdown("@"+nameof(OdinDropdownHelper)+"."+nameof(OdinDropdownHelper.GetAIDecisionInterface)+"()")]
    #endif
    public string Condition;
}
```

**Inspector 效果**:
```
Condition: [距离检查 (DistanceCheck) ▼]
```

---

### 示例 2: 表情下拉菜单

```csharp
// 在 DecisionActionNode 中
#if UNITY_EDITOR
[ValueDropdown("@"+nameof(OdinDropdownHelper)+"."+nameof(OdinDropdownHelper.GetEmoji)+"()")]
#endif
[NinoMember(13)][LabelText("表情名")]
public string Emoji;
```

**Inspector 效果**:
```
表情名：[无 ▼]
```

---

### 示例 3: 数值类型下拉菜单

```csharp
// 在配置类中
#if UNITY_EDITOR
[ValueDropdown("@"+nameof(OdinDropdownHelper)+"."+nameof(OdinDropdownHelper.GetNumericFinalTypeId)+"()")]
#endif
public int NumericType;
```

**Inspector 效果**:
```
数值类型：[HP(1) ▼]
```

---

## 设计要点

### 为什么使用静态方法？

1. **无状态**: 不需要实例化
2. **性能**: 避免重复创建对象
3. **简单**: 直接调用即可
4. **Odin 要求**: ValueDropdown 需要静态方法或实例方法引用

### 反射扫描

```csharp
var methods = typeof(AIDecisionInterface).GetMethods();
```

**说明**: 使用反射扫描类的所有方法

**优势**:
- 自动发现新方法
- 无需手动维护列表
- 扩展友好

### LabelText 特性读取

```csharp
var attrs = methods[i].GetCustomAttributes(TypeInfo<LabelTextAttribute>.Type, false);
string text = (attrs[0] as LabelTextAttribute).Text;
```

**说明**: 读取方法的 LabelText 特性作为显示文本

**优势**:
- 代码与显示分离
- 策划友好的显示名称
- 支持 Tooltip 提示

### 排序

```csharp
list.Sort(DefaultStringComparer);
```

**说明**: 使用 StringComparer 按字母顺序排序

**优势**:
- 易于查找
- 一致性
- 专业感

---

## 扩展建议

### 添加新的下拉菜单

```csharp
/// <summary>
/// 技能类型
/// </summary>
public static IEnumerable GetSkillTypes()
{
    var fields = typeof(SkillType).GetFields();
    ValueDropdownList<int> list = new ValueDropdownList<int>();
    
    foreach (var field in fields)
    {
        if (!field.IsStatic) continue;
        
        var val = (int)field.GetValue(null);
        var attrs = field.GetCustomAttributes(TypeInfo<LabelTextAttribute>.Type, false);
        string text = attrs.Length > 0 ? (attrs[0] as LabelTextAttribute).Text : field.Name;
        
        list.Add($"{text}({val})", val);
    }
    
    list.Sort(DefaultStringComparer);
    return list;
}
```

### 使用方式

```csharp
#if UNITY_EDITOR
[ValueDropdown("@"+nameof(OdinDropdownHelper)+"."+nameof(OdinDropdownHelper.GetSkillTypes)+"()")]
#endif
public int SkillType;
```

---

## 相关文档

- [DecisionConditionNode.cs.md](../DecisionTree/DecisionConditionNode.cs.md) - 使用 GetAIDecisionInterface
- [DecisionActionNode.cs.md](../DecisionTree/DecisionActionNode.cs.md) - 使用 GetEmoji
- [Odin Inspector 文档](https://odininspector.com/)

---

*文档生成时间：2026-02-28 | OpenClaw AI 助手*
