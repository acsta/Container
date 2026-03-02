# SerializationHelper.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | SerializationHelper.cs |
| **路径** | Assets/Scripts/Editor/Common/Nino/SerializationHelper.cs |
| **所属模块** | Editor 工具 → Common/Nino |
| **文件职责** | Nino 序列化代码生成器，为热更新 DLL 生成序列化代码 |

---

## 类/结构体说明

### SerializationHelper

| 属性 | 说明 |
|------|------|
| **职责** | 提供 Unity 编辑器菜单，生成 Nino 序列化代码，支持热更新 DLL |
| **泛型参数** | 无 |
| **继承关系** | 静态类 |
| **编译条件** | `#if UNITY_2017_1_OR_NEWER` |

**设计模式**: 工具类模式 + 编辑器菜单

```csharp
// 命名空间
namespace Nino.Editor
{
    public static class SerializationHelper
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `ExternalDLLPath` | `string[]` | `private static readonly` | 外部 DLL 路径数组，用于热更新 DLL (如 ILRuntime/huatuo) |
| `ExportPath` | `const string` | `private const` | 代码导出路径，生成到 Assets/Scripts/Code/Module/Generate/Nino/ |

---

## 方法说明（按重要程度排序）

### GenerateSerializationCode()

**签名**:
```csharp
[MenuItem("Nino/Generator/Serialization Code")]
public static void GenerateSerializationCode()
```

**职责**: 生成所有类型的 Nino 序列化代码

**核心逻辑**:
```
1. 获取当前 AppDomain 的所有程序集
2. 如果有 ILRuntime 定义，加载外部热更新 DLL
3. 合并程序集列表
4. 调用 CodeGenerator.GenerateSerializationCodeForAllTypePossible()
5. 刷新资源数据库 AssetDatabase.Refresh()
```

**调用者**: Unity 编辑器菜单 "Nino/Generator/Serialization Code"

**菜单路径**:
```
Unity 顶部菜单 → Nino → Generator → Serialization Code
```

---

## 编译条件

### ILRuntime 支持

```csharp
#if ILRuntime
    "Assets/Nino/Test/Editor/Serialization/Test11.bytes"
#endif
```

**说明**: 当项目使用 ILRuntime 热更新框架时，会加载额外的测试 DLL 进行代码生成

---

## 导出路径配置

### 默认路径

```csharp
private const string ExportPath = "Scripts/Code/Module/Generate/Nino/";
```

**完整路径**: `Assets/Scripts/Code/Module/Generate/Nino/`

**修改方法**: 直接修改 `ExportPath` 常量

**路径规则**:
- 相对于 `Assets` 目录
- 使用 `../` 可导出到 Assets 外部
- 示例：`"../Generated/Nino/"` 导出到项目根目录

---

## 使用示例

### 在 Unity 编辑器中使用

1. **打开 Unity 编辑器**
2. **点击顶部菜单**: `Nino → Generator → Serialization Code`
3. **等待代码生成完成**
4. **查看生成结果**: `Assets/Scripts/Code/Module/Generate/Nino/`

### 生成的代码示例

```csharp
// 生成的序列化代码示例 (伪代码)
namespace Nino.Generated
{
    public static class SomeTypeSerializer
    {
        public static void Serialize(SomeType obj, Writer writer)
        {
            writer.Write(obj.Field1);
            writer.Write(obj.Field2);
        }
        
        public static SomeType Deserialize(Reader reader)
        {
            var obj = new SomeType();
            obj.Field1 = reader.Read<Type1>();
            obj.Field2 = reader.Read<Type2>();
            return obj;
        }
    }
}
```

---

## 技术要点

### 1. 程序集加载

```csharp
// 获取当前域的所有程序集
var assemblies = System.AppDomain.CurrentDomain.GetAssemblies().ToList();

// 加载外部 DLL (热更新 DLL)
assemblies.AddRange(ExternalDLLPath.Select(s => 
    Assembly.Load(File.ReadAllBytes(s))
));
```

### 2. 编辑器菜单

```csharp
// 定义菜单项
[MenuItem("Nino/Generator/Serialization Code")]
public static void GenerateSerializationCode()
{
    // 菜单点击时执行
}
```

### 3. 代码生成器调用

```csharp
// 调用 Nino 的代码生成器
CodeGenerator.GenerateSerializationCodeForAllTypePossible(
    ExportPath,      // 导出路径
    assemblies.ToArray()  // 程序集数组
);
```

### 4. 资源刷新

```csharp
// 刷新资源数据库，使生成的代码立即可用
AssetDatabase.Refresh();
```

---

## Nino 序列化框架

### 什么是 Nino?

Nino 是一个高性能的 C# 序列化库，特点:
- 支持 AOT (Ahead-Of-Time) 代码生成
- 适用于 ILRuntime 等热更新框架
- 比 JSON/XML 更快的序列化速度
- 支持泛型、继承、多态

### 为什么需要代码生成?

**问题**: ILRuntime 等热更新框架无法在运行时使用反射生成序列化代码

**解决方案**: 在编辑器阶段预先生成序列化代码，编译到热更新 DLL 中

**流程**:
```
编辑器阶段                    运行阶段
    │                           │
    ├─ 扫描所有类型             │
    ├─ 生成序列化代码 ────────► │ 直接使用生成的代码
    ├─ 编译到 DLL               │ (无需反射)
    └─ 刷新资源数据库           │
```

---

## 配置说明

### 添加外部 DLL

如果需要为其他热更新 DLL 生成代码，修改 `ExternalDLLPath`:

```csharp
private static readonly string[] ExternalDLLPath = new string[]
{
    "Assets/Nino/Test/Editor/Serialization/Test11.bytes",
    "Assets/HotUpdate/Code.bytes",  // 添加新的 DLL
    "Assets/HotUpdate/Logic.bytes", // 添加新的 DLL
};
```

### 修改导出路径

```csharp
// 导出到 Assets 内部
private const string ExportPath = "Scripts/Code/Module/Generate/Nino/";

// 导出到 Assets 外部
private const string ExportPath = "../Generated/Nino/";
```

---

## 相关文档

- [Nino 官方文档](https://github.com/NinoGuo/Nino) - Nino 序列化库
- [ILRuntime 文档](https://github.com/Ourpalm/ILRuntime) - 热更新框架
- [CodeGenerator API](https://github.com/NinoGuo/Nino/wiki) - 代码生成器 API

---

*文档生成时间：2026-03-03 | OpenClaw AI 助手*
