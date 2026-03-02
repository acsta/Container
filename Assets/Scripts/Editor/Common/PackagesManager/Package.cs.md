# Package.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | Package.cs |
| **路径** | Assets/Scripts/Editor/Common/PackagesManager/Package.cs |
| **所属模块** | Editor 工具 → Common/PackagesManager |
| **文件职责** | Unity 本地包的 package.json 数据模型 |

---

## 类/结构体说明

### Package

| 属性 | 说明 |
|------|------|
| **职责** | 表示 Unity 本地包的 package.json 文件的数据结构 |
| **泛型参数** | 无 |
| **继承关系** | 无 |
| **命名空间** | `TaoTie` |

**设计模式**: 数据模型 (DTO)

```csharp
namespace TaoTie
{
    public class Package
    {
        public string name;
    }
}
```

---

## 字段与属性

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `name` | `string` | `public` | 包的唯一标识符 (包名) |

---

## JSON 结构示例

### package.json 格式

```json
{
  "name": "com.taotie.framework",
  "version": "1.0.0",
  "displayName": "TaoTie Framework",
  "description": "TaoTie 游戏框架核心模块",
  "unity": "2019.4",
  "dependencies": {
    "com.unity.modules.jsonserialize": "1.0.0"
  }
}
```

### 反序列化后

```csharp
var package = JsonConvert.DeserializeObject<Package>(jsonString);

// package.name = "com.taotie.framework"
```

**注意**: 本文件只定义了 `name` 字段，完整的 package.json 可能包含更多字段 (version, displayName, description 等)

---

## 包命名规范

### 命名格式

```
<公司/组织>.<项目/框架>.<模块>
```

### 示例

| 包名 | 说明 |
|------|------|
| `com.taotie.framework` | TaoTie 框架核心 |
| `com.taotie.game` | 游戏业务模块 |
| `com.unity.modules.ai` | Unity 官方 AI 模块 |
| `com.unity.textmeshpro` | Unity TextMeshPro |

### 命名规则

1. **反向域名表示法**: 使用公司域名的反向形式
2. **小写字母**: 全部使用小写
3. **点号分隔**: 使用 `.` 分隔层级
4. **无特殊字符**: 只允许字母、数字、点号、下划线

---

## 使用场景

### 1. PackagesManagerEditor 读取

```csharp
// PackagesManagerEditor.cs
string packagePath = item.FullName + "/package.json";
if (File.Exists(packagePath))
{
    Package package = JsonConvert.DeserializeObject<Package>(
        File.ReadAllText(packagePath)
    );
    
    // 使用包名检查是否已安装
    bool isInstalled = info.dependencies.ContainsKey(package.name);
}
```

### 2. 遍历本地包目录

```csharp
DirectoryInfo dir = new DirectoryInfo("Modules");
var sources = dir.GetDirectories();

foreach (var source in sources)
{
    string packagePath = source.FullName + "/package.json";
    if (File.Exists(packagePath))
    {
        Package package = JsonConvert.DeserializeObject<Package>(
            File.ReadAllText(packagePath)
        );
        
        Debug.Log($"发现本地包：{package.name}");
    }
}
```

---

## 与 Packages.cs 的关系

### Package.cs (本文件)

```csharp
public class Package
{
    public string name;  // 单个包的名称
}
```

**职责**: 表示单个本地包的 package.json

### Packages.cs

```csharp
public class Packages
{
    public Dictionary<string, string> dependencies;  // 所有依赖
}
```

**职责**: 表示项目级别的 manifest.json

**关系图**:
```
项目根目录/
├─ Packages/
│   └─ manifest.json  → 反序列化为 Packages
│       └─ dependencies: {
│              "com.taotie.framework": "file:../Framework",
│              "com.taotie.game": "file:../Game"
│          }
│
└─ Modules/
    ├─ Framework/
    │   └─ package.json  → 反序列化为 Package
    │       └─ name: "com.taotie.framework"
    │
    └─ Game/
        └─ package.json  → 反序列化为 Package
            └─ name: "com.taotie.game"
```

---

## 完整 package.json 示例

### 框架包

```json
{
  "name": "com.taotie.framework",
  "version": "1.0.0",
  "displayName": "TaoTie Framework",
  "description": "TaoTie 游戏框架核心模块，包含 ECS 架构、对象池、消息系统等",
  "unity": "2019.4",
  "author": {
    "name": "TaoTie Team",
    "email": "team@taotie.com"
  },
  "dependencies": {
    "com.unity.modules.jsonserialize": "1.0.0",
    "com.unity.modules.assetbundle": "1.0.0"
  },
  "keywords": [
    "framework",
    "ecs",
    "game"
  ],
  "license": "MIT",
  "repository": {
    "type": "git",
    "url": "https://github.com/taotie/framework.git"
  }
}
```

### 游戏业务包

```json
{
  "name": "com.taotie.game",
  "version": "1.0.0",
  "displayName": "TaoTie Game",
  "description": "基于 TaoTie 框架的游戏业务模块",
  "unity": "2019.4",
  "dependencies": {
    "com.taotie.framework": "1.0.0"
  }
}
```

---

## 技术要点

### 1. JSON 反序列化

```csharp
// 简单反序列化
var package = JsonConvert.DeserializeObject<Package>(jsonString);

// 只提取 name 字段，其他字段忽略
// Newtonsoft.Json 会自动忽略未定义的字段
```

### 2. 文件读取

```csharp
// 读取 package.json
string json = File.ReadAllText(packagePath);

// 反序列化
Package package = JsonConvert.DeserializeObject<Package>(json);

// 获取包名
string packageName = package.name;
```

### 3. 路径处理

```csharp
// 获取目录信息
DirectoryInfo dir = new DirectoryInfo("Modules");

// 获取所有子目录
DirectoryInfo[] subDirs = dir.GetDirectories();

// 构建 package.json 路径
foreach (var subDir in subDirs)
{
    string packagePath = Path.Combine(subDir.FullName, "package.json");
}
```

---

## 扩展建议

### 添加更多字段

如果需要访问 package.json 的其他字段，可以扩展 `Package` 类:

```csharp
public class Package
{
    public string name;
    public string version;
    public string displayName;
    public string description;
    public string unity;
    public Dictionary<string, string> dependencies;
}
```

### 使用 JsonProperty 特性

```csharp
public class Package
{
    [JsonProperty("name")]
    public string Name { get; set; }
    
    [JsonProperty("version")]
    public string Version { get; set; }
    
    [JsonProperty("displayName")]
    public string DisplayName { get; set; }
}
```

---

## 相关文档

- [Packages.cs.md](./Packages.cs.md) - 项目依赖配置模型
- [PackagesManagerEditor.cs.md](./PackagesManagerEditor.cs.md) - 包管理编辑器工具
- [Unity Package 文档](https://docs.unity3d.com/Manual/upm-manifestPkg.html) - package.json 官方文档

---

*文档生成时间：2026-03-03 | OpenClaw AI 助手*
