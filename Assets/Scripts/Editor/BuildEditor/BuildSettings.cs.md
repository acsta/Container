# BuildSettings.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | BuildSettings.cs |
| **路径** | Assets/Scripts/Editor/BuildEditor/BuildSettings.cs |
| **所属模块** | Editor → BuildEditor |
| **文件职责** | 打包配置 ScriptableObject，存储 Unity 打包工具的各项参数设置 |

---

## 类/结构体说明

### BuildSettings

| 属性 | 说明 |
|------|------|
| **职责** | 存储打包工具的配置参数，通过 ScriptableObject 持久化到 Asset 文件 |
| **泛型参数** | 无 |
| **继承关系** | `ScriptableObject` |
| **实现的接口** | 无 |

**设计模式**: ScriptableObject 配置模式

```csharp
// 通过 CreateInstance 创建实例
buildSettings = CreateInstance<BuildSettings>();
AssetDatabase.CreateAsset(buildSettings, settingAsset);

// 通过 AssetDatabase 加载
buildSettings = AssetDatabase.LoadAssetAtPath<BuildSettings>(settingAsset);
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `channel` | `string` | `public` | 打包渠道标识，如 "googleplay", "tiktok", "wechat" 等 |
| `buildMode` | `Mode` | `public` | 服务器模式：本机开发/内网测试/外网测试/自定义服务器 |
| `buildType` | `BuildType` | `public` | 构建类型：Development(开发版) / Release(发布版) |
| `isBuildExe` | `bool` | `public` | 是否打包可执行文件 (整包) |
| `buildHotfixAssembliesAOT` | `bool` | `public` | 热更代码是否打 AOT (IL2CPP 预编译，提升首包性能) |
| `isBuildAll` | `bool` | `public` | 全量资源是否打进包 |
| `isContainsAb` | `bool` | `public` | 是否同时打分包资源 (AssetBundle 分包) |
| `isPackAtlas` | `bool` | `public` | 是否需要重新打图集 |
| `clearBuildCache` | `bool` | `public` | 清理构建缓存 (Library/Bee 等) |
| `clearReleaseFolder` | `bool` | `public` | 清理打包输出文件夹 (Release/) |
| `clearABFolder` | `bool` | `public` | 清理 AB 缓存文件夹 (AssetBundle 输出目录) |
| `collectShaderVariant` | `bool` | `public` | 是否重新收集 Shader 变体 |
| `cdn` | `string` | `public` | 自定义 CDN 地址 (当 buildMode=自定义服务器时使用) |
| `buildAssetBundleOptions` | `BuildAssetBundleOptions` | `public` | AssetBundle 构建选项 |

---

## 配置项说明

### 渠道配置 (channel)

```csharp
// 渠道示例
channel = "googleplay"   // Google Play
channel = "tiktok"       // 抖音小游戏
channel = "wechat"       // 微信小游戏
channel = "kuaiShou"     // 快手小游戏
channel = "bilibili"     // B 站小游戏
channel = "tapTap"       // TapTap
channel = "aliPay"       // 支付宝小程序
channel = "quickGame"    // 快游戏
channel = "facebook"     // Facebook Instant Games
channel = "_4399"        // 4399 小游戏
channel = "miniHost"     // 迷你宿主
```

### 服务器模式 (Mode)

```csharp
public enum Mode : byte
{
    本机开发，      // 本地开发调试
    内网测试，      // 内网测试服务器
    外网测试，      // 外网测试服务器
    自定义服务器    // 自定义 CDN 地址
}
```

### 构建类型 (BuildType)

```csharp
public enum BuildType : byte
{
    Development,    // 开发版：允许调试，连接 Profiler
    Release         // 发布版：优化构建，无调试功能
}
```

---

## 使用示例

### 示例 1: 创建配置

```csharp
// 创建 BuildSettings 配置
var buildSettings = CreateInstance<BuildSettings>();
buildSettings.channel = "tiktok";
buildSettings.buildMode = Mode.内网测试;
buildSettings.isBuildExe = true;
buildSettings.buildHotfixAssembliesAOT = true;
buildSettings.isBuildAll = false;
buildSettings.isContainsAb = true;

// 保存为 Asset 文件
AssetDatabase.CreateAsset(buildSettings, "Assets/Scripts/Editor/BuildEditor/BuildSettings.asset");
AssetDatabase.SaveAssets();
```

### 示例 2: 加载配置

```csharp
// 从 Asset 文件加载配置
var buildSettings = AssetDatabase.LoadAssetAtPath<BuildSettings>("Assets/Scripts/Editor/BuildEditor/BuildSettings.asset");

// 读取配置值
string channel = buildSettings.channel;
Mode mode = buildSettings.buildMode;
bool isBuildExe = buildSettings.isBuildExe;
```

### 示例 3: 修改配置

```csharp
// 修改配置并保存
buildSettings.channel = "wechat";
buildSettings.buildMode = Mode.外网测试;
buildSettings.clearBuildCache = true;

EditorUtility.SetDirty(buildSettings);
AssetDatabase.SaveAssets();
```

---

## 与其他模块的交互

```mermaid
graph TD
    subgraph BuildEditor["打包工具"]
        BE[BuildEditor.cs<br/>UI 界面]
        BS[BuildSettings<br/>配置存储]
    end
    
    subgraph BuildHelper["构建助手"]
        BH[BuildHelper.cs<br/>构建逻辑]
    end
    
    subgraph Platform["平台配置"]
        WPE[WebGLPlatformEditor<br/>WebGL 平台切换]
    end
    
    BE --> BS
    BE --> BH
    BE --> WPE
    BS -.-> BH
    
    note right of BS "BuildSettings 存储所有<br/>打包配置参数"
    
    style BuildEditor fill:#e1f5ff
    style BuildHelper fill:#fff4e1
    style Platform fill:#e8f5e9
```

**依赖关系**:
- **被依赖**: BuildEditor.cs (UI 界面读取/保存配置)
- **依赖**: 无 (纯数据配置类)

---

## 配置持久化

### 存储位置

```
Assets/Scripts/Editor/BuildEditor/BuildSettings.asset
```

### 保存机制

```csharp
// BuildEditor.OnDisable() 中自动保存
private void OnDisable()
{
    SaveSettings();
}

private void SaveSettings()
{
    if (buildSettings == null) return;
    
    // 将 UI 状态同步到 BuildSettings
    buildSettings.clearBuildCache = clearBuildCache;
    buildSettings.clearReleaseFolder = clearReleaseFolder;
    buildSettings.isBuildExe = isBuildExe;
    buildSettings.buildHotfixAssembliesAOT = buildHotfixAssembliesAOT;
    // ... 其他字段
    
    EditorUtility.SetDirty(buildSettings);
    AssetDatabase.SaveAssets();
}
```

---

## 阅读指引

### 建议的阅读顺序

1. **理解配置作用** - BuildSettings 存储什么
2. **看字段定义** - 了解各个配置项含义
3. **了解枚举类型** - Mode/BuildType/PlatformType
4. **查看使用示例** - 如何创建/加载/修改配置

### 最值得学习的技术点

1. **ScriptableObject 配置**: Unity 推荐的配置存储方式
2. **编辑器持久化**: EditorUtility.SetDirty + AssetDatabase.SaveAssets
3. **配置与逻辑分离**: 配置数据 (BuildSettings) 与构建逻辑 (BuildHelper) 分离

---

## 相关文档

- [BuildEditor.cs.md](./BuildEditor.cs.md) - 打包工具 UI 界面
- [BuildHelper.cs.md](./BuildHelper.cs.md) - 构建助手核心逻辑
- [BuildAssemblyEditor.cs.md](./BuildAssemblyEditor.cs.md) - 程序集构建
- [WebGLPlatformEditor.cs.md](./WebGLPlatformEditor.cs.md) - WebGL 平台配置

---

*文档生成时间：2026-03-02 | OpenClaw AI 助手*
