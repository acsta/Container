# DynDictionary.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | DynDictionary.cs |
| **路径** | Assets/Scripts/Mono/Core/Object/DynDictionary.cs |
| **所属模块** | Mono 层 → Core/Object |
| **文件职责** | 动态字典，支持变量变更事件和父级继承 |

---

## 类说明

### DynDictionary

| 属性 | 说明 |
|------|------|
| **职责** | 存储 float 类型变量的动态字典，支持事件通知和层级继承 |
| **值类型** | float |
| **设计模式** | 对象池模式、观察者模式 |
| **继承关系** | IDisposable |

```csharp
public class DynDictionary : IDisposable
{
    // 变量字典
    private Dictionary<string, float> _varDict;
    
    // 父级字典（支持继承）
    private DynDictionary _parent;
    
    // 变量变更事件
    public event OnVariableChangeDelegate onValueChange;
}
```

---

## 委托定义

### OnVariableChangeDelegate

```csharp
public delegate void OnVariableChangeDelegate(string key, float value, float oldValue);
```

**说明**: 变量值变更时的回调委托

**参数**:
- `key`: 变量名
- `value`: 新值
- `oldValue`: 旧值

---

## 方法说明

### Create()

**签名**:
```csharp
public static DynDictionary Create()
```

**职责**: 从对象池创建动态字典实例

**返回**: 新的或复用的 DynDictionary 实例

**核心逻辑**:
```
1. 调用 ObjectPool.Instance.Fetch<DynDictionary>()
2. 返回实例
```

**使用示例**:
```csharp
var dict = DynDictionary.Create();
dict.Set("score", 100);
```

---

### Set()

**签名**:
```csharp
public void Set(string key, float val)
```

**职责**: 设置变量值

**参数**:
- `key`: 变量名
- `val`: 值

**核心逻辑**:
```
1. 获取旧值 Get(key)
2. 设置新值 SetInternal(key, val)
3. 触发 onValueChange 事件
```

**使用示例**:
```csharp
var dict = DynDictionary.Create();
dict.Set("health", 100);
dict.Set("mana", 50.5f);
```

---

### Get()

**签名**:
```csharp
public float Get(string key)
```

**职责**: 获取变量值

**参数**:
- `key`: 变量名

**返回**: 变量值，不存在则返回 default(float)

**核心逻辑**:
```
1. 在 _varDict 中查找
2. 如果不存在且有父级，从父级获取
3. 否则返回 default
```

**使用示例**:
```csharp
var dict = DynDictionary.Create();
dict.Set("score", 100);

float score = dict.Get("score");  // 100
float level = dict.Get("level");  // 0 (不存在)
```

---

### TryGet()

**签名**:
```csharp
public bool TryGet(string key, out float res)
```

**职责**: 尝试获取变量值

**参数**:
- `key`: 变量名
- `res`: 输出参数，接收值

**返回**: 是否找到

**核心逻辑**:
```
1. 在 _varDict 中查找
2. 如果不存在且有父级，从父级获取
3. 返回是否找到
```

**使用示例**:
```csharp
var dict = DynDictionary.Create();
dict.Set("score", 100);

if (dict.TryGet("score", out float value))
{
    Console.WriteLine($"Score: {value}");
}
```

---

### Remove()

**签名**:
```csharp
public void Remove(string key)
```

**职责**: 移除变量

**参数**:
- `key`: 变量名

**核心逻辑**:
```
1. 检查是否存在
2. 从 _varDict 移除
```

**注意**: 只移除本级变量，不影响父级

---

### Contain()

**签名**:
```csharp
public bool Contain(string key)
```

**职责**: 检查是否包含变量

**返回**: 是否包含（只检查本级）

---

### Clear()

**签名**:
```csharp
public void Clear()
```

**职责**: 清空本级所有变量

**核心逻辑**:
```
1. 清空 _varDict
2. 不影響父級
```

---

### SetParent()

**签名**:
```csharp
public void SetParent(DynDictionary parent)
```

**职责**: 设置父级字典

**用途**: 建立层级继承关系

**使用示例**:
```csharp
var globalDict = DynDictionary.Create();
globalDict.Set("maxHealth", 100);

var playerDict = DynDictionary.Create();
playerDict.SetParent(globalDict);

// playerDict 可以继承 globalDict 的变量
float maxHealth = playerDict.Get("maxHealth");  // 100 (从父级继承)
```

---

### Dispose()

**签名**:
```csharp
public void Dispose()
```

**职责**: 清理并回收到对象池

**核心逻辑**:
```
1. 清空 _varDict
2. 解除父级引用
3. 回收到对象池
```

**使用示例**:
```csharp
using (var dict = DynDictionary.Create())
{
    dict.Set("temp", 1);
    // 使用
} // 自动回收
```

---

## 使用场景

### 场景 1: 游戏变量系统

```csharp
// 全局游戏变量
var globalVars = DynDictionary.Create();
globalVars.Set("gameSpeed", 1.0f);
globalVars.Set("difficulty", 2.0f);

// 玩家变量（继承全局）
var playerVars = DynDictionary.Create();
playerVars.SetParent(globalVars);
playerVars.Set("health", 100);

// 获取变量（自动继承）
float speed = playerVars.Get("gameSpeed");    // 1.0 (从父级)
float health = playerVars.Get("health");      // 100 (本级)
```

### 场景 2: 变量变更监听

```csharp
var vars = DynDictionary.Create();

// 监听变量变更
vars.onValueChange += (key, newValue, oldValue) => {
    Console.WriteLine($"{key}: {oldValue} -> {newValue}");
};

vars.Set("score", 0);    // 触发事件
vars.Set("score", 100);  // 触发事件
```

### 场景 3: 作用域变量

```csharp
// 全局作用域
var globalScope = DynDictionary.Create();
globalScope.Set("gravity", 9.8f);

// 关卡作用域
var levelScope = DynDictionary.Create();
levelScope.SetParent(globalScope);
levelScope.Set("timeLimit", 300);

// 玩家作用域
var playerScope = DynDictionary.Create();
playerScope.SetParent(levelScope);
playerScope.Set("lives", 3);

// 玩家可访问所有层级变量
float gravity = playerScope.Get("gravity");    // 9.8 (祖父级)
float timeLimit = playerScope.Get("timeLimit"); // 300 (父级)
float lives = playerScope.Get("lives");        // 3 (本级)
```

---

## 层级继承关系

```
globalVars (根)
    ├── Set("maxHealth", 100)
    └── Set("gameSpeed", 1.0)
    
    ↓ 继承
    
playerVars (子)
    ├── Set("health", 50)
    └── Set("score", 0)
    
    ↓ 继承
    
battleVars (孙)
        └── Set("enemyCount", 5)

查找 "maxHealth":
    battleVars → playerVars → globalVars ✓ (100)
    
查找 "health":
    battleVars → playerVars ✓ (50)
```

---

## 事件机制

### 触发时机

```csharp
// 设置新值时触发
dict.Set("key", 100);  // 触发 onValueChange("key", 100, 0)
dict.Set("key", 200);  // 触发 onValueChange("key", 200, 100)
```

### 事件参数

```csharp
dict.onValueChange += (key, newValue, oldValue) => {
    // key: 变量名
    // newValue: 新值
    // oldValue: 旧值（包括从父级继承的值）
};
```

---

## 注意事项

### 1. 必须回收

```csharp
// ✅ 正确：使用 using
using (var dict = DynDictionary.Create())
{
    // 使用
}

// ❌ 错误：忘记回收
var dict = DynDictionary.Create();
// 忘记 Dispose
```

### 2. 事件订阅管理

长期存在的字典注意及时取消订阅，避免内存泄漏：

```csharp
var dict = DynDictionary.Create();
dict.onValueChange += Handler;

// 使用完毕后
dict.onValueChange -= Handler;
dict.Dispose();
```

### 3. 父级循环引用

避免创建循环继承关系：

```csharp
// ❌ 错误：循环引用
dictA.SetParent(dictB);
dictB.SetParent(dictA);  // 可能导致无限递归
```

---

## 相关文档

- [ObjectPool.cs.md](../ObjectPool.cs.md) - 对象池
- [DictionaryComponent.cs.md](./DictionaryComponent.cs.md) - 字典组件

---

*文档生成时间：2026-03-01 | OpenClaw AI 助手*
