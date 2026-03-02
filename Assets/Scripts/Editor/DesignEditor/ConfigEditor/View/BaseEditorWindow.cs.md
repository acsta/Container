# BaseEditorWindow.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | BaseEditorWindow.cs |
| **路径** | Assets/Scripts/Editor/DesignEditor/ConfigEditor/View/BaseEditorWindow.cs |
| **所属模块** | Editor 工具 → DesignEditor/ConfigEditor/View |
| **文件职责** | 基于 Odin Inspector 的配置编辑器窗口基类，提供 JSON 配置文件的创建、打开、保存功能 |

---

## 类/结构体说明

### BaseEditorWindow<T>

| 属性 | 说明 |
|------|------|
| **职责** | 提供通用的配置编辑器窗口功能，支持 JSON 配置文件的可视化编辑 |
| **泛型参数** | `T` - 配置数据类型 (必须是 class) |
| **继承关系** | 继承自 `Sirenix.OdinInspector.Editor.OdinEditorWindow` |
| **命名空间** | `TaoTie` |

**设计模式**: 模板方法模式 + 泛型基类

```csharp
namespace TaoTie
{
    public abstract class BaseEditorWindow<T> : OdinEditorWindow where T : class
    {
        // 通用配置编辑功能
    }
}
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `fileName` | `string` | `protected virtual` | 默认文件名，使用类型名 |
| `folderPath` | `string` | `protected virtual` | 默认文件夹路径，默认"Assets/AssetsPackage" |
| `oldJson` | `string` | `private` | 上次保存的 JSON 字符串，用于检测修改 |
| `filePath` | `string` | `public` | 当前配置文件路径，只读显示 |
| `data` | `T` | `public` | 当前配置数据对象，Odin 可视化编辑 |

---

## 方法说明（按重要程度排序）

### Init()

**签名**:
```csharp
public void Init(T data, string searchPath)
```

**职责**: 初始化窗口，加载配置数据

**参数**:
- `data`: 配置数据对象
- `searchPath`: 配置文件路径

**核心逻辑**:
```
1. 设置 data 字段
2. 将 data 序列化为 JSON 保存为 oldJson
3. 设置 filePath
```

**调用者**: 子类窗口、OnOpenAsset 回调

---

### CreateInstance()

**签名**:
```csharp
protected virtual T CreateInstance()
```

**职责**: 创建新的配置数据实例

**返回值**: 新的 T 类型实例

**核心逻辑**:
```
1. 使用 Activator.CreateInstance<T>() 创建实例
2. 返回新实例
```

**调用者**: CreateJson()

**扩展**: 子类可重写此方法提供自定义初始化

---

### Open()

**签名**:
```csharp
[Button("打开")]
public void Open()
```

**职责**: 打开文件选择对话框，加载 JSON 配置文件

**核心逻辑**:
```
1. 显示文件选择对话框 (EditorUtility.OpenFilePanel)
2. 读取选中的文件内容
3. 反序列化为 T 类型
4. 如果成功，设置 data 和 filePath
5. 如果失败，显示错误通知
```

**调用者**: Odin Inspector 按钮点击

**特性**:
```csharp
[Button("打开")]
```
- Odin 特性，在 Inspector 显示为按钮
- 按钮文本为"打开"

---

### CreateJson()

**签名**:
```csharp
[Button("新建")]
public void CreateJson()
```

**职责**: 创建新的 JSON 配置文件

**核心逻辑**:
```
1. 显示保存文件对话框 (EditorUtility.SaveFilePanel)
2. 创建新的 T 类型实例
3. 序列化为 JSON 并写入文件
4. 保存 oldJson 用于修改检测
5. 刷新资源数据库
```

**调用者**: Odin Inspector 按钮点击

---

### SaveJson()

**签名**:
```csharp
[Button("保存")]
[ShowIf("@data!=null")]
public void SaveJson()
```

**职责**: 保存当前配置到 JSON 文件

**核心逻辑**:
```
1. 调用 BeforeSaveData() 预处理
2. 序列化为 JSON
3. 写入文件
4. 同时保存为 .bytes 格式 (Nino 序列化)
5. 刷新资源数据库
6. 显示成功通知
```

**调用者**: Odin Inspector 按钮点击

**条件显示**:
```csharp
[ShowIf("@data!=null")]
```
- 只在 data 不为 null 时显示按钮

---

### BeforeSaveData()

**签名**:
```csharp
protected virtual void BeforeSaveData()
```

**职责**: 保存前的预处理钩子

**核心逻辑**:
```
1. 空实现 (由子类重写)
```

**调用者**: SaveJson()

**扩展**: 子类可重写此方法在保存前处理数据

---

### SaveNewJson()

**签名**:
```csharp
[Button("另存为")]
[ShowIf("@data!=null")]
public void SaveNewJson()
```

**职责**: 另存为新文件

**核心逻辑**:
```
1. 解析当前文件路径，提取文件名
2. 显示保存文件对话框 (默认目录为当前文件目录)
3. 序列化并写入新文件
4. 同时保存为 .bytes 格式
5. 刷新资源数据库
6. 更新 filePath 为新路径
```

**调用者**: Odin Inspector 按钮点击

---

### OnDestroy()

**签名**:
```csharp
protected override void OnDestroy()
```

**职责**: 窗口销毁时检查未保存的修改

**核心逻辑**:
```
1. 调用基类 OnDestroy()
2. 如果 data 不为 null:
   - 序列化当前数据
   - 与 oldJson 比较
   - 如果有修改，显示确认对话框
   - 用户选择保存则调用 SaveJson()
```

**调用者**: Unity 编辑器生命周期 (窗口关闭时)

---

## 界面布局

### Odin Inspector 显示效果

```
┌─────────────────────────────────────────────────────────┐
│ BaseEditorWindow<ConfigEnvironments>                    │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  [打开]  [新建]  [保存]  [另存为]                       │
│                                                         │
├─────────────────────────────────────────────────────────┤
│ File Path: Assets/AssetsPackage/EditConfig/Env.json    │
├─────────────────────────────────────────────────────────┤
│ Data:                                                   │
│  ┌────────────────────────────────────────────────────┐ │
│  │ Environment List                                   │ │
│  │  ├─ Environment 1                                  │ │
│  │  │   ├─ Name: "Day"                               │ │
│  │  │   ├─ Light Intensity: 1.0                      │ │
│  │  │   └─ Skybox: "DaySkybox"                       │ │
│  │  └─ Environment 2                                  │ │
│  │      ├─ Name: "Night"                             │ │
│  │      ├─ Light Intensity: 0.3                      │ │
│  │      └─ Skybox: "NightSkybox"                     │ │
│  └────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────┘
```

---

## 使用示例

### 1. 创建子类窗口

```csharp
public class EnvironmentEditor : BaseEditorWindow<ConfigEnvironments>
{
    // 自定义文件夹路径
    protected override string folderPath => base.folderPath + "/EditConfig";

    // 菜单项
    [MenuItem("Tools/工具/策划/配置编辑器/Environment")]
    static void OpenAI()
    {
        EditorWindow.GetWindow<EnvironmentEditor>().Show();
    }

    // 双击资源打开
    [OnOpenAsset(0)]
    public static bool OnBaseDataOpened(int instanceID, int line)
    {
        var data = EditorUtility.InstanceIDToObject(instanceID) as TextAsset;
        var path = AssetDatabase.GetAssetPath(data);
        return InitializeData(data, path);
    }

    public static bool InitializeData(TextAsset asset, string path)
    {
        if (asset == null) return false;
        if (path.EndsWith(".json") && 
            JsonHelper.TryFromJson<ConfigEnvironments>(asset.text, out var aiJson))
        {
            var win = GetWindow<EnvironmentEditor>();
            win.Init(aiJson, path);
            return true;
        }
        return false;
    }
}
```

### 2. 配置数据结构

```csharp
[Serializable]
public class ConfigEnvironments
{
    public List<EnvironmentInfo> environments;
}

[Serializable]
public class EnvironmentInfo
{
    public string name;
    public float lightIntensity;
    public string skyboxName;
}
```

### 3. 使用流程

```
1. 点击菜单 Tools → 工具 → 策划 → 配置编辑器 → Environment
2. 窗口打开，显示空配置
3. 点击"新建"创建新配置文件
4. 在 Inspector 中编辑配置数据
5. 点击"保存"保存到文件
6. 下次双击 .json 文件可直接打开编辑
```

---

## 技术要点

### 1. Odin EditorWindow

```csharp
public abstract class BaseEditorWindow<T> : OdinEditorWindow
```

**说明**:
- Odin Inspector 提供的编辑器窗口基类
- 自动支持 Odin 特性 ([Button], [ShowIf], etc.)
- 提供可视化属性编辑

### 2. Odin 特性

```csharp
[Button("打开")]           // 显示为按钮
[ShowIf("@data!=null")]   // 条件显示
[ReadOnly]                // 只读
[HideReferenceObjectPicker]  // 隐藏对象选择器
```

### 3. JSON 序列化

```csharp
// 序列化
var jStr = JsonHelper.ToJson(data);

// 反序列化
data = JsonHelper.FromJson<T>(text);

// 尝试反序列化 (不抛异常)
JsonHelper.TryFromJson<T>(text, out var result)
```

### 4. Nino 序列化

```csharp
// 序列化为二进制
var bytes = NinoSerializer.Serialize(data);

// 保存为 .bytes 文件
File.WriteAllBytes(filePath.Replace("json", "bytes"), bytes);
```

**说明**: 同时保存 JSON (可读) 和 bytes (运行时高效) 两种格式

### 5. 文件对话框

```csharp
// 打开文件
string path = EditorUtility.OpenFilePanel(
    "标题",      // 对话框标题
    folderPath,  // 默认目录
    "json"       // 文件扩展名
);

// 保存文件
string path = EditorUtility.SaveFilePanel(
    "标题",
    folderPath,
    fileName,
    "json"
);
```

### 6. 修改检测

```csharp
// 保存时记录
oldJson = JsonHelper.ToJson(data);

// 关闭时比较
var jStr = JsonHelper.ToJson(data);
if (oldJson != jStr)
{
    // 有未保存的修改
    var res = EditorUtility.DisplayDialog("提示", "是否需要保存？", "是", "否");
}
```

---

## 文件结构

### 生成的文件

```
Assets/AssetsPackage/EditConfig/
├─ ConfigEnvironments.json   # 可读的 JSON 格式
└─ ConfigEnvironments.bytes  # 二进制格式 (运行时使用)
```

### 文件用途

| 文件 | 用途 |
|------|------|
| `.json` | 编辑器可读可写，版本控制友好 |
| `.bytes` | 运行时快速加载，节省解析时间 |

---

## 相关文档

- [EnvironmentEditor.cs.md](../ConfigEditor/View/EnvironmentEditor.cs.md) - 环境配置编辑器示例
- [JsonHelper.cs.md](../../../Mono/Helper/JsonHelper.cs.md) - JSON 序列化工具
- [NinoSerializer 文档](https://github.com/NinoGuo/Nino) - Nino 序列化库

---

*文档生成时间：2026-03-03 | OpenClaw AI 助手*
