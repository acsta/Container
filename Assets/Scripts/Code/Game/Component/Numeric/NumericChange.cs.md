# NumericChange.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | NumericChange.cs |
| **路径** | Assets/Scripts/Code/Game/Component/Numeric/NumericChange.cs |
| **所属模块** | 游戏层 → Component/Numeric |
| **文件职责** | 数值变化事件数据结构 |

---

## 结构体说明

### NumericChange

| 属性 | 说明 |
|------|------|
| **职责** | 携带数值变化信息的事件数据，用于广播通知 |
| **类型** | `struct` (值类型) |
| **设计注意** | 其他地方不要持有对此结构的引用（避免生命周期问题） |

---

## 字段说明

| 字段 | 类型 | 说明 |
|------|------|------|
| `Parent` | `Entity` | 发生数值变化的实体对象 |
| `NumericType` | `int` | 数值类型 ID（参考 NumericType 枚举） |
| `Old` | `decimal` | 变化前的旧值 |
| `New` | `decimal` | 变化后的新值 |

---

## 使用示例

### 监听数值变化事件

```csharp
// 注册事件监听
Messager.Instance.AddEventListener(MessageId.NumericChangeEvt, OnNumericChange);

void OnNumericChange(object sender, object args)
{
    // 安全复制结构体（值类型拷贝）
    var change = (NumericChange)args;
    
    Log.Info($"实体 {change.Parent.Id} 的数值 {change.NumericType} " +
             $"从 {change.Old} 变为 {change.New}");
    
    // ⚠️ 不要持有对 change 的长期引用
    // 如果需要保存，复制字段值
    int type = change.NumericType;
    decimal newValue = change.New;
}
```

### 广播数值变化事件

```csharp
// 在 NumericComponent.Insert() 中
NumericChange args = new NumericChange
{
    Parent = this.parent,           // 所属实体
    NumericType = numericType,       // 数值类型
    Old = oldValue,                  // 旧值
    New = value                      // 新值
};

Messager.Instance.Broadcast(Id, MessageId.NumericChangeEvt, args);
```

### UI 更新示例

```csharp
// UI 监听数值变化并更新显示
public class NumericDisplay : MonoBehaviour
{
    private Entity targetEntity;
    
    void OnEnable()
    {
        Messager.Instance.AddEventListener(MessageId.NumericChangeEvt, OnNumericChange);
    }
    
    void OnDisable()
    {
        Messager.Instance.RemoveEventListener(MessageId.NumericChangeEvt, OnNumericChange);
    }
    
    void OnNumericChange(object sender, object args)
    {
        var change = (NumericChange)args;
        
        // 只更新目标实体的数值显示
        if (change.Parent == targetEntity)
        {
            UpdateDisplay(change.NumericType, change.New);
        }
    }
    
    void UpdateDisplay(int numericType, decimal value)
    {
        // 更新 UI 文本
        textComponent.text = value.ToString();
    }
}
```

---

## 设计说明

### 为什么使用 struct？

1. **性能**: 值类型避免堆分配，减少 GC 压力
2. **临时性**: 事件数据是瞬时的，不需要长期持有
3. **拷贝语义**: 每次传递都是拷贝，避免意外修改

### 为什么不能持有引用？

```csharp
// ❌ 错误示例
NumericChange? cachedChange = null;

void OnNumericChange(object sender, object args)
{
    cachedChange = (NumericChange)args;  // 危险！
}

// 后续使用 cachedChange 可能访问到无效数据
```

```csharp
// ✅ 正确示例
void OnNumericChange(object sender, object args)
{
    var change = (NumericChange)args;
    
    // 立即处理或复制字段
    int type = change.NumericType;
    decimal newValue = change.New;
    
    ProcessChange(type, newValue);
}
```

---

## 相关消息 ID

| 消息 ID | 说明 |
|---------|------|
| `MessageId.NumericChangeEvt` | 数值变化事件，携带 `NumericChange` 结构体 |

---

## 相关文档

- [NumericComponent.cs.md](./NumericComponent.cs.md) - 数值组件（事件发送方）
- [NumericSystem.cs.md](./NumericSystem.cs.md) - 数值系统管理器
- [MessageId.cs.md](../../../Mono/Module/Const/MessageId.cs.md) - 全局消息 ID 定义

---

*文档生成时间：2026-03-02 | OpenClaw AI 助手*
