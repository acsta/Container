# AssemblyManager.cs 注解文档

## 文件基本信息
- **路径**: Assets/Scripts/Mono/Module/Assembly/AssemblyManager.cs
- **职责**: 程序集管理器，管理所有加载的程序集和类型

## 字段
| 字段 | 类型 | 说明 |
|------|------|------|
| Instance | AssemblyManager | 单例实例 |
| temp | HashSet<Assembly> | 已加载的程序集 |
| hotfixTemp | HashSet<Assembly> | 热更新程序集 |
| allTypes | Dictionary<string, Type> | 所有类型（全名→Type） |
| mapTypes | UnOrderMultiMap<Assembly, Type> | 程序集→类型映射 |

## 方法
### GetTypes
```csharp
public Dictionary<string, Type> GetTypes()
```
获取所有类型的字典

### AddAssembly
```csharp
public void AddAssembly(Assembly assembly)
```
添加程序集，提取所有类型

### AddHotfixAssembly
```csharp
public void AddHotfixAssembly(Assembly assembly)
```
添加热更新程序集（可移除）

### RemoveHotfixAssembly
```csharp
public void RemoveHotfixAssembly()
```
移除所有热更新程序集（用于热更替换）

## 使用示例
```csharp
// 初始化
AssemblyManager.Instance.Init();

// 添加程序集
AssemblyManager.Instance.AddAssembly(typeof(Game).Assembly);

// 添加热更新程序集
AssemblyManager.Instance.AddHotfixAssembly(hotfixAssembly);

// 获取所有类型
var types = AssemblyManager.Instance.GetTypes();

// 热更新时移除旧程序集
AssemblyManager.Instance.RemoveHotfixAssembly();
```
