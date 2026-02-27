# IPackageFinder.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | IPackageFinder.cs |
| **路径** | Assets/Scripts/Code/Module/Resource/IPackageFinder.cs |
| **所属模块** | 框架层 → Code/Module/Resource |
| **文件职责** | 包查找器接口定义 |

---

## 接口说明

### IPackageFinder

| 属性 | 说明 |
|------|------|
| **职责** | 定义包查找接口，用于确定资源属于哪个资源包 |
| **泛型参数** | 无 |

```csharp
public interface IPackageFinder
{
    public string GetPackageName(string path);
}
```

### DefaultPackageFinder

| 属性 | 说明 |
|------|------|
| **职责** | 默认包查找器实现，返回默认包名 |
| **继承关系** | 实现 `IPackageFinder` |

```csharp
public class DefaultPackageFinder : IPackageFinder
{
    public string GetPackageName(string path)
    {
        return Define.DefaultName;
    }
}
```

---

## 方法说明

### GetPackageName(string path)

**签名**:
```csharp
public string GetPackageName(string path)
```

**职责**: 根据资源路径获取所属包名

**参数**:
- `path`: 资源路径

**返回**: 包名称字符串

**默认实现**:
```csharp
public string GetPackageName(string path)
{
    return Define.DefaultName;  // 返回默认包名
}
```

---

## 使用场景

### 场景 1: 分包加载

当游戏使用 YooAsset 分包时，需要确定每个资源属于哪个包：

```
资源路径 → IPackageFinder → 包名
"UI/LoginPanel.prefab" → DefaultPackageFinder → "DefaultPackage"
"GamePlay/Level01.prefab" → CustomPackageFinder → "GamePlayPackage"
```

### 场景 2: 自定义包查找

可以自定义 IPackageFinder 实现复杂的路径到包名映射：

```csharp
public class CustomPackageFinder : IPackageFinder
{
    private Dictionary<string, string> pathToPackage;
    
    public string GetPackageName(string path)
    {
        // 根据路径前缀判断包名
        if (path.StartsWith("UI/"))
        {
            return "UIPackage";
        }
        else if (path.StartsWith("GamePlay/"))
        {
            return "GamePlayPackage";
        }
        else if (path.StartsWith("Audio/"))
        {
            return "AudioPackage";
        }
        
        return Define.DefaultName;
    }
}
```

---

## 使用示例

### 示例 1: 使用默认查找器

```csharp
// 创建默认查找器
IPackageFinder finder = new DefaultPackageFinder();

// 获取包名
string packageName = finder.GetPackageName("UI/LoginPanel.prefab");
// 返回："DefaultPackage"（Define.DefaultName）
```

### 示例 2: 自定义查找器

```csharp
// 创建自定义查找器
IPackageFinder finder = new CustomPackageFinder();

// 获取包名
string uiPackage = finder.GetPackageName("UI/LoginPanel.prefab");
// 返回："UIPackage"

string gamePackage = finder.GetPackageName("GamePlay/Level01.prefab");
// 返回："GamePlayPackage"
```

### 示例 3: 在 ResourcesManager 中使用

```csharp
// ResourcesManager 内部使用
public async ETTask<T> LoadAsync<T>(string path)
{
    // 确定资源属于哪个包
    string packageName = packageFinder.GetPackageName(path);
    
    // 从指定包加载
    var operation = package.LoadAssetAsync<T>(packageName, path);
    await operation.Task;
    
    return operation.AssetObject as T;
}
```

---

## 相关文档

- [ResourcesManager.cs.md](./ResourcesManager.cs.md) - 资源管理器
- [Define.cs.md](../Const/Define.cs.md) - 全局配置

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
