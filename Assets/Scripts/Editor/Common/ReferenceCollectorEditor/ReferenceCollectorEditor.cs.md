# ReferenceCollectorEditor.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | ReferenceCollectorEditor.cs |
| **路径** | Assets/Scripts/Editor/Common/ReferenceCollectorEditor/ReferenceCollectorEditor.cs |
| **所属模块** | Editor 工具 → Common/ReferenceCollectorEditor |
| **文件职责** | ReferenceCollector 类的自定义编辑器，提供可视化界面管理引用集合 |

---

## 类/结构体说明

### ReferenceCollectorEditor

| 属性 | 说明 |
|------|------|
| **职责** | 为 ReferenceCollector 组件提供自定义 Inspector 界面，支持添加、删除、搜索、拖拽引用 |
| **泛型参数** | 无 |
| **继承关系** | 继承自 `UnityEditor.Editor` |
| **特性标记** | `[CustomEditor(typeof(ReferenceCollector))]` |

**设计模式**: 自定义编辑器模式

```csharp
// 绑定到 ReferenceCollector 组件
[CustomEditor(typeof(ReferenceCollector))]
public class ReferenceCollectorEditor : Editor
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `referenceCollector` | `ReferenceCollector` | `private` | 被编辑的 ReferenceCollector 实例 |
| `searchKey` | `string` | `private property` | 搜索框输入的键值，带 setter 自动更新 heroPrefab |
| `heroPrefab` | `Object` | `private` | 当前搜索到的引用对象 (UnityEngine.Object) |
| `_searchKey` | `string` | `private` | 搜索键值的内部存储字段 |

---

## 方法说明（按重要程度排序）

### OnEnable()

**签名**:
```csharp
private void OnEnable()
```

**职责**: 编辑器启用时初始化，获取目标对象

**核心逻辑**:
```
1. 将 target 转换为 ReferenceCollector 实例
2. 赋值给 referenceCollector 字段
```

**调用者**: Unity 编辑器生命周期

---

### OnInspectorGUI()

**签名**:
```csharp
public override void OnInspectorGUI()
```

**职责**: 绘制自定义 Inspector 界面

**核心逻辑**:
```
1. 记录撤销操作 Undo.RecordObject()
2. 获取 data 属性 (SerializedProperty)
3. 绘制按钮行：添加引用/全部删除/删除空引用/排序
4. 绘制搜索行：文本框 + 对象选择框 + 删除按钮
5. 遍历 data 列表，绘制每个引用项 (key 文本框 + 对象选择框 + 删除按钮)
6. 处理拖拽事件 (DragUpdated/DragPerform)
7. 执行删除操作
8. 应用修改 serializedObject.ApplyModifiedProperties()
```

**调用者**: Unity 编辑器生命周期

**界面布局**:
```
┌─────────────────────────────────────────────────────┐
│ [添加引用] [全部删除] [删除空引用] [排序]          │
├─────────────────────────────────────────────────────┤
│ 搜索 Key: [________] [Object 选择框] [删除]        │
├─────────────────────────────────────────────────────┤
│ [key1: ______] [Object: _____] [X]                 │
│ [key2: ______] [Object: _____] [X]                 │
│ [key3: ______] [Object: _____] [X]                 │
│ ...                                                 │
├─────────────────────────────────────────────────────┤
│ ← 拖拽区域：可直接拖拽资源到此处添加引用            │
└─────────────────────────────────────────────────────┘
```

---

### DelNullReference()

**签名**:
```csharp
private void DelNullReference()
```

**职责**: 删除所有空引用 (null Object)

**核心逻辑**:
```
1. 获取 data 数组的 SerializedProperty
2. 倒序遍历数组 (从后往前，避免索引问题)
3. 检查每个元素的 gameObject 字段是否为 null
4. 如果为 null，删除该数组元素
5. 标记对象为脏数据 EditorUtility.SetDirty()
6. 应用修改
```

**调用者**: OnInspectorGUI() 中的"删除空引用"按钮

---

### AddReference()

**签名**:
```csharp
private void AddReference(SerializedProperty dataProperty, string key, Object obj)
```

**职责**: 向 ReferenceCollector 添加新的引用项

**参数**:
- `dataProperty`: data 数组的 SerializedProperty
- `key`: 引用键值
- `obj`: 引用对象 (UnityEngine.Object)

**核心逻辑**:
```
1. 获取数组当前大小作为新索引
2. 在数组末尾插入新元素
3. 设置新元素的 key 字段
4. 设置新元素的 gameObject 字段
```

**调用者**: OnInspectorGUI() 中的"添加引用"按钮、拖拽事件处理

---

## 编辑器界面功能

### 按钮功能

| 按钮 | 功能说明 |
|------|----------|
| **添加引用** | 添加新引用项，key 为随机 GUID 哈希值，对象为 null |
| **全部删除** | 清空 ReferenceCollector 的所有引用 |
| **删除空引用** | 删除所有 Object 为 null 的引用项 |
| **排序** | 对引用列表按 key 排序 |
| **删除 (单个)** | 删除搜索框指定 key 的引用项 |
| **X (每项)** | 删除对应行的引用项 |

### 搜索功能

```csharp
// searchKey 属性 setter 会自动更新 heroPrefab
private string searchKey
{
    get => _searchKey;
    set
    {
        if (_searchKey != value)
        {
            _searchKey = value;
            heroPrefab = referenceCollector.Get<Object>(searchKey);
        }
    }
}
```

**使用流程**:
1. 在搜索框输入 key
2. 自动查找对应的 Object 并显示在右侧对象框
3. 点击"删除"按钮移除该引用

---

### 拖拽添加功能

**支持事件**:
- `EventType.DragUpdated`: 拖拽进入区域时
- `EventType.DragPerform`: 拖拽释放时

**核心逻辑**:
```csharp
if (eventType == EventType.DragUpdated || eventType == EventType.DragPerform)
{
    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
    
    if (eventType == EventType.DragPerform)
    {
        DragAndDrop.AcceptDrag();
        foreach (var o in DragAndDrop.objectReferences)
        {
            AddReference(dataProperty, o.name, o);
        }
    }
    Event.current.Use();
}
```

**使用方式**: 直接将资源从 Project 窗口拖拽到 Inspector 区域，自动以资源名作为 key 添加引用

---

## 使用示例

### 在 Unity 编辑器中使用

1. **选择挂载 ReferenceCollector 的 GameObject**
2. **Inspector 窗口显示自定义界面**
3. **添加引用**:
   - 点击"添加引用"按钮
   - 或直接拖拽资源到区域
4. **编辑引用**:
   - 修改 key 文本框
   - 点击对象选择框选择 Object
5. **删除引用**:
   - 搜索 key 后点击"删除"
   - 或点击每项右侧的"X"
   - 或点击"删除空引用"批量清理

### 代码示例

```csharp
// ReferenceCollector 使用示例 (运行时)
public class MyComponent : MonoBehaviour
{
    public ReferenceCollector references;
    
    void Start()
    {
        // 通过 key 获取引用
        var hero = references.Get<GameObject>("Hero");
        var enemy = references.Get<GameObject>("Enemy");
    }
}
```

---

## 技术要点

### 1. SerializedProperty 操作

```csharp
// 获取数组属性
var dataProperty = serializedObject.FindProperty("data");

// 获取数组元素
var element = dataProperty.GetArrayElementAtIndex(i);

// 获取子字段
var keyProperty = element.FindPropertyRelative("key");
var objProperty = element.FindPropertyRelative("gameObject");

// 修改值
keyProperty.stringValue = "newKey";
objProperty.objectReferenceValue = someObject;

// 应用修改
serializedObject.ApplyModifiedProperties();
```

### 2. 撤销支持

```csharp
// 记录撤销操作
Undo.RecordObject(referenceCollector, "Changed Settings");
```

### 3. 脏数据标记

```csharp
// 标记对象需要保存
EditorUtility.SetDirty(referenceCollector);
```

### 4. 拖拽处理

```csharp
// 设置拖拽视觉反馈
DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

// 接受拖拽
DragAndDrop.AcceptDrag();

// 获取拖拽对象
var objects = DragAndDrop.objectReferences;

// 消耗事件
Event.current.Use();
```

---

## 相关文档

- [ReferenceCollector.cs.md](../../Mono/Module/UI/ReferenceCollector.cs.md) - 被编辑的运行时组件
- [CustomEditor 官方文档](https://docs.unity3d.com/ScriptReference/Editor.html) - Unity 自定义编辑器 API

---

*文档生成时间：2026-03-03 | OpenClaw AI 助手*
