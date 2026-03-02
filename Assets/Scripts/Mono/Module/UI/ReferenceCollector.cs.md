# ReferenceCollector.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | ReferenceCollector.cs |
| **路径** | Assets/Scripts/Mono/Module/UI/ReferenceCollector.cs |
| **所属模块** | 框架层 → Mono/Module/UI |
| **文件职责** | UI 引用收集器，用于在 Inspector 中管理 GameObject/Component 引用，运行时通过键名快速访问 |

---

## 类/结构体说明

### ReferenceCollectorData

| 属性 | 说明 |
|------|------|
| **职责** | 序列化数据结构，存储键值对引用（key → UnityEngine.Object） |
| **泛型参数** | 无 |
| **继承关系** | 无继承 |
| **实现的接口** | 无 |

**字段**:
| 字段 | 类型 | 说明 |
|------|------|------|
| `key` | `string` | 引用键名，用于运行时查找 |
| `gameObject` | `Object` | UnityEngine.Object，可以是 GameObject 或 Component |

```csharp
[Serializable]
public class ReferenceCollectorData
{
    public string key;
    public Object gameObject;
}
```

---

### ReferenceCollectorDataComparer

| 属性 | 说明 |
|------|------|
| **职责** | ReferenceCollectorData 比较器，用于按键名排序 |
| **泛型参数** | 无 |
| **继承关系** | 实现 `IComparer<ReferenceCollectorData>` |
| **实现的接口** | `IComparer<ReferenceCollectorData>` |

**比较规则**: 使用 `string.Compare` 的 Ordinal 模式（字节级比较，性能好）

```csharp
public class ReferenceCollectorDataComparer : IComparer<ReferenceCollectorData>
{
    public int Compare(ReferenceCollectorData x, ReferenceCollectorData y)
    {
        return string.Compare(x.key, y.key, StringComparison.Ordinal);
    }
}
```

---

### ReferenceCollector

| 属性 | 说明 |
|------|------|
| **职责** | MonoBehaviour 组件，在 Inspector 中管理引用集合，运行时提供键名访问 |
| **泛型参数** | 无 |
| **继承关系** | 继承 `MonoBehaviour` |
| **实现的接口** | `ISerializationCallbackReceiver` |

**设计模式**: 序列化缓存 + 字典查找

```csharp
public class ReferenceCollector : MonoBehaviour, ISerializationCallbackReceiver
{
    // 序列化数据（Inspector 可见）
    public List<ReferenceCollectorData> data = new List<ReferenceCollectorData>();
    
    // 运行时字典（快速查找）
    private readonly Dictionary<string, Object> dict = new Dictionary<string, Object>();
}
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `data` | `List<ReferenceCollectorData>` | `public` | 序列化数据列表，在 Inspector 中显示和编辑 |
| `dict` | `Dictionary<string, Object>` | `private readonly` | 运行时字典，反序列化后构建，用于快速查找 |

---

## 方法说明（按重要程度排序）

### Get<T>

**签名**:
```csharp
public T Get<T>(string key) where T : class
```

**职责**: 通过键名获取引用对象（泛型版本）

**参数**:
| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `key` | `string` | - | 引用键名 |

**返回值**: `T` - 指定类型的对象，如果不存在返回 null

**核心逻辑**:
```
1. 从 dict 字典查找 key
2. 如果不存在，返回 null
3. 如果存在但类型不匹配：
   - 如果是 GameObject，尝试 GetComponent<T>()
   - 如果是 Component，尝试 GetComponent<T>()
4. 返回转换后的对象
```

**调用者**: 需要访问 UI 引用的代码

**使用示例**:
```csharp
// 获取 Button 组件
var button = referenceCollector.Get<Button>("SubmitBtn");

// 获取 GameObject
var go = referenceCollector.Get<GameObject>("PlayerRoot");

// 获取 Text 组件
var text = referenceCollector.Get<Text>("ScoreText");
```

---

### GetObject

**签名**:
```csharp
public Object GetObject(string key)
```

**职责**: 通过键名获取引用对象（原始 Object 版本）

**参数**:
| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `key` | `string` | - | 引用键名 |

**返回值**: `Object` - UnityEngine.Object，如果不存在返回 null

**核心逻辑**:
```
1. 从 dict 字典查找 key
2. 返回找到的对象或 null
```

**调用者**: 需要访问原始 Object 的代码

**使用示例**:
```csharp
// 获取原始 Object
var obj = referenceCollector.GetObject("SomeRef");
```

---

### OnBeforeSerialize

**签名**:
```csharp
public void OnBeforeSerialize()
```

**职责**: 序列化前回调（ISerializationCallbackReceiver 接口）

**核心逻辑**:
```
1. 空实现（不需要特殊处理）
```

**调用者**: Unity 序列化系统

---

### OnAfterDeserialize

**签名**:
```csharp
public void OnAfterDeserialize()
```

**职责**: 反序列化后回调，构建运行时字典

**核心逻辑**:
```
1. 清空 dict 字典
2. 遍历 data 列表
3. 将每个 ReferenceCollectorData 添加到 dict
4. 如果 key 已存在，跳过（避免重复）
```

**调用者**: Unity 序列化系统

**执行时机**: Prefab 加载、场景加载、对象实例化时

---

### Add（仅编辑器）

**签名**:
```csharp
#if UNITY_EDITOR
public void Add(string key, Object obj)
```

**职责**: 在 Inspector 中添加新的引用（仅编辑器可用）

**参数**:
| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `key` | `string` | - | 引用键名 |
| `obj` | `Object` | - | 要添加的对象 |

**核心逻辑**:
```
1. 创建 SerializedObject
2. 查找 data 属性
3. 遍历检查 key 是否已存在
4. 如果存在：更新现有元素的 gameObject 字段
5. 如果不存在：插入新元素
6. 应用修改并更新
```

**调用者**: 编辑器脚本、自定义 Inspector

**使用示例**:
```csharp
#if UNITY_EDITOR
// 在编辑器中动态添加引用
referenceCollector.Add("NewBtn", buttonObject);
#endif
```

---

### Remove（仅编辑器）

**签名**:
```csharp
#if UNITY_EDITOR
public void Remove(string key)
```

**职责**: 在 Inspector 中删除引用（仅编辑器可用）

**参数**:
| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `key` | `string` | - | 要删除的引用键名 |

**核心逻辑**:
```
1. 创建 SerializedObject
2. 查找 data 属性
3. 遍历查找 key
4. 如果找到，删除对应元素
5. 应用修改并更新
```

**调用者**: 编辑器脚本、自定义 Inspector

---

### Clear（仅编辑器）

**签名**:
```csharp
#if UNITY_EDITOR
public void Clear()
```

**职责**: 清空所有引用（仅编辑器可用）

**核心逻辑**:
```
1. 创建 SerializedObject
2. 清空 data 数组
3. 应用修改并更新
```

**调用者**: 编辑器脚本、自定义 Inspector

---

### Sort（仅编辑器）

**签名**:
```csharp
#if UNITY_EDITOR
public void Sort()
```

**职责**: 按键名排序引用列表（仅编辑器可用）

**核心逻辑**:
```
1. 使用 ReferenceCollectorDataComparer 排序 data 列表
2. 应用修改并更新
```

**调用者**: 编辑器脚本、自定义 Inspector

---

## 序列化机制

### Unity 序列化流程

```
┌─────────────────────────────────────────────────────────┐
│  编辑器中保存 Prefab                                     │
│  ↓                                                       │
│  Unity 序列化 data 列表 → 写入 Prefab 文件                │
│  ↓                                                       │
│  运行时加载 Prefab                                        │
│  ↓                                                       │
│  Unity 反序列化 data 列表                                 │
│  ↓                                                       │
│  调用 OnAfterDeserialize()                               │
│  ↓                                                       │
│  构建 dict 字典（快速查找）                               │
└─────────────────────────────────────────────────────────┘
```

### Prefab 文件示例

```yaml
# Prefab 文件中的 ReferenceCollector 数据
ReferenceCollector:
  data:
    - key: SubmitBtn
      gameObject: {fileID: 1234567890, guid: abcdef..., type: 1}
    - key: CancelBtn
      gameObject: {fileID: 9876543210, guid: fedcba..., type: 1}
    - key: ScoreText
      gameObject: {fileID: 1111111111, guid: 222222..., type: 1}
```

---

## 使用示例

### 示例 1: 基础用法

```csharp
// 1. 在 Inspector 中配置引用
// 挂载 ReferenceCollector 组件，添加以下键值对：
// - "SubmitBtn" → SubmitButton GameObject
// - "CancelBtn" → CancelButton GameObject
// - "ScoreText" → ScoreText GameObject

// 2. 在代码中访问
public class GameView : MonoBehaviour
{
    [SerializeField] private ReferenceCollector referenceCollector;
    
    private Button submitBtn;
    private Button cancelBtn;
    private Text scoreText;
    
    private void Awake()
    {
        // 通过键名获取引用
        submitBtn = referenceCollector.Get<Button>("SubmitBtn");
        cancelBtn = referenceCollector.Get<Button>("CancelBtn");
        scoreText = referenceCollector.Get<Text>("ScoreText");
        
        // 绑定事件
        submitBtn.onClick.AddListener(OnSubmit);
        cancelBtn.onClick.AddListener(OnCancel);
    }
}
```

### 示例 2: 与 UIBaseView 配合

```csharp
public class HomeView : UIBaseView
{
    private ReferenceCollector referenceCollector;
    
    public override async ETTask OnCreate()
    {
        // 获取 ReferenceCollector 组件
        referenceCollector = gameObject.GetComponent<ReferenceCollector>();
        
        // 获取 UI 引用
        var enterBtn = referenceCollector.Get<Button>("EnterBtn");
        var settingBtn = referenceCollector.Get<Button>("SettingBtn");
        
        // 绑定事件
        enterBtn.onClick.AddListener(OnEnter);
        settingBtn.onClick.AddListener(OnSetting);
    }
}
```

### 示例 3: 编辑器扩展

```csharp
#if UNITY_EDITOR
[CustomEditor(typeof(HomeView))]
public class HomeViewEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        var homeView = (HomeView)target;
        var refCollector = homeView.GetComponent<ReferenceCollector>();
        
        if (refCollector != null)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Quick Actions", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Sort References"))
            {
                refCollector.Sort();
            }
            
            if (GUILayout.Button("Clear All"))
            {
                refCollector.Clear();
            }
        }
    }
}
#endif
```

---

## 优势对比

### 与传统 SerializedField 对比

| 特性 | ReferenceCollector | [SerializeField] |
|------|-------------------|------------------|
| **动态添加** | ✅ 支持编辑器动态添加 | ❌ 需要修改代码 |
| **批量管理** | ✅ 集中管理所有引用 | ❌ 分散在各个字段 |
| **键名访问** | ✅ 支持字符串键名 | ❌ 仅支持字段名 |
| **代码侵入** | ✅ 无需修改代码 | ❌ 需要添加字段 |
| **类型安全** | ⚠️ 运行时检查 | ✅ 编译时检查 |

### 与 FindObjectOfType 对比

| 特性 | ReferenceCollector | FindObjectOfType |
|------|-------------------|------------------|
| **性能** | ✅ O(1) 字典查找 | ❌ O(n) 遍历查找 |
| **可靠性** | ✅ 显式配置 | ❌ 依赖场景结构 |
| **灵活性** | ✅ 可配置任意对象 | ❌ 仅场景内对象 |
| **GC 压力** | ✅ 无 GC | ⚠️ 可能产生 GC |

---

## 注意事项

### 1. 键名唯一性

确保每个键名唯一，后添加的会覆盖先前的：

```csharp
// ❌ 错误：重复键名
refCollector.Add("Btn", button1);
refCollector.Add("Btn", button2);  // 覆盖 button1

// ✅ 正确：唯一键名
refCollector.Add("SubmitBtn", button1);
refCollector.Add("CancelBtn", button2);
```

### 2. 类型转换

Get<T> 会尝试类型转换，但如果对象没有目标组件会返回 null：

```csharp
// 如果 "SomeObj" 是 GameObject 但没有 Button 组件
var btn = refCollector.Get<Button>("SomeObj");  // 返回 null

// 应该先获取 GameObject，再手动获取组件
var go = refCollector.Get<GameObject>("SomeObj");
var btn = go?.GetComponent<Button>();
```

### 3. 编辑器专用方法

Add/Remove/Clear/Sort 方法仅在 UNITY_EDITOR 宏下可用：

```csharp
// ✅ 正确：使用宏包裹
#if UNITY_EDITOR
refCollector.Add("NewKey", someObject);
#endif

// ❌ 错误：运行时会编译失败
refCollector.Add("NewKey", someObject);  // 编译错误
```

### 4. 序列化时机

OnAfterDeserialize 在以下时机调用：
- Prefab 加载时
- 场景加载时
- 对象实例化时
- 编辑器保存后重新加载

确保在 Awake 或 Start 之后使用 dict 字典。

---

## 相关文档

- [UIBaseView.cs.md](../UIComponent/UIBaseView.cs.md) - UI 基类
- [UIManager.cs.md](../UIComponent/UIManager.cs.md) - UI 管理器
- [UIButton.cs.md](../UIComponent/UIButton.cs.md) - UI 按钮组件

---

## 技术要点总结

| 要点 | 说明 |
|------|------|
| **序列化缓存** | data 列表存储序列化数据 |
| **字典查找** | dict 字典提供 O(1) 查找 |
| **ISerializationCallbackReceiver** | 反序列化后构建字典 |
| **编辑器扩展** | Add/Remove/Clear/Sort 仅编辑器可用 |
| **泛型访问** | Get<T> 提供类型安全的访问 |
| **键名管理** | 字符串键名便于动态访问 |

---

*文档生成时间：2026-03-01 | OpenClaw AI 助手*
