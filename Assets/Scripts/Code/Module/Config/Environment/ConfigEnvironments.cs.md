# ConfigEnvironments.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|------|
| **文件名** | ConfigEnvironments.cs |
| **路径** | Assets/Scripts/Code/Module/Config/Environment/ConfigEnvironments.cs |
| **所属模块** | 框架层 → Code/Module/Config/Environment |
| **文件职责** | 定义环境配置列表数据结构，管理多个环境配置和天空盒纹理 |

---

## 类/结构体说明

### ConfigEnvironments

| 属性 | 说明 |
|------|------|
| **职责** | 存储所有环境配置的集合，包含默认环境、环境列表和天空盒纹理路径 |
| **泛型参数** | 无 |
| **继承关系** | 无 |
| **实现的接口** | 无 |

**设计模式**: 集合模式 + 配置聚合

```csharp
// 创建环境配置列表
var envs = new ConfigEnvironments
{
    DefaultEnvironment = defaultEnv,
    Environments = new[] { dayEnv, nightEnv, sunsetEnv },
    SkyDayTexPath = "Sky/DaySkybox",
    SkyNightTexPath = "Sky/NightSkybox"
};
```

---

## 字段与属性

### DefaultEnvironment

| 属性 | 值 |
|------|------|
| **类型** | `ConfigEnvironment` |
| **访问级别** | `public` |
| **说明** | 默认环境配置 |

**Nino 序列化**: `[NinoMember(1)]`

**用途**: 当没有特定环境配置时使用此默认配置

---

### Environments

| 属性 | 值 |
|------|------|
| **类型** | `ConfigEnvironment[]` |
| **访问级别** | `public` |
| **说明** | 所有环境配置的数组 |

**Nino 序列化**: `[NinoMember(2)]`

**用途**: 存储所有可用的环境配置

---

### DefaultBlend

| 属性 | 值 |
|------|------|
| **类型** | `ConfigBlender` |
| **访问级别** | `public` |
| **默认值** | `new ConfigBlender()` |
| **说明** | 默认过渡参数 |

**Nino 序列化**: `[NinoMember(3)]`

**Odin Inspector**: `[HideReferenceObjectPicker]`

**用途**: 当环境配置没有指定 Enter/Leave 时使用此默认过渡参数

**Inspector 优化**: 隐藏对象选择器，直接显示字段

---

### SkyDayTexPath

| 属性 | 值 |
|------|------|
| **类型** | `string` |
| **访问级别** | `public` |
| **说明** | 白天天空盒纹理路径 |

**Nino 序列化**: `[NinoMember(4)]`

**Odin Inspector**: `[BoxGroup("SkyDayTex")]`

**用途**: 存储白天天空盒的资源路径

---

### SkyNightTexPath

| 属性 | 值 |
|------|------|
| **类型** | `string` |
| **访问级别** | `public` |
| **说明** | 夜晚天空盒纹理路径 |

**Nino 序列化**: `[NinoMember(5)]`

**Odin Inspector**: `[BoxGroup("SkyNightTex")]`

---

### SkySunriseTexPath

| 属性 | 值 |
|------|------|
| **类型** | `string` |
| **访问级别** | `public` |
| **说明** | 日出天空盒纹理路径 |

**Nino 序列化**: `[NinoMember(6)]`

**Odin Inspector**: `[BoxGroup("SkySunriseTex")]`

---

### SkySunsetTexPath

| 属性 | 值 |
|------|------|
| **类型** | `string` |
| **访问级别** | `public` |
| **说明** | 日落天空盒纹理路径 |

**Nino 序列化**: `[NinoMember(7)]`

**Odin Inspector**: `[BoxGroup("SkySunsetTex")]`

---

## 编辑器专用字段

### SkyDayTex / SkyNightTex / SkySunriseTex / SkySunsetTex

**条件编译**: `#if UNITY_EDITOR`

**类型**: `Texture`

**用途**: 编辑器中预览天空盒纹理

### OnValueChanged 回调

```csharp
[OnValueChanged(nameof(UpdateSkyDayTexPath))][BoxGroup("SkyDayTex")]
public Texture SkyDayTex;

private void UpdateSkyDayTexPath()
{
    if (SkyDayTex == null)
    {
        SkyDayTexPath = null;
        return;
    }

    var path = UnityEditor.AssetDatabase.GetAssetPath(SkyDayTex);
    if (path.StartsWith("Assets/AssetsPackage/"))
    {
        SkyDayTexPath = path.Replace("Assets/AssetsPackage/", "");
    }
    else
    {
        SkyDayTexPath = null;
    }
}
```

**功能**:
1. 当 Texture 字段改变时自动更新 Path
2. 路径格式化为相对路径（去除 "Assets/AssetsPackage/"）
3. 如果纹理不在 AssetsPackage 目录，Path 设为 null

### Button 预览按钮

```csharp
[Button("预览 SkyDayTex")][BoxGroup("SkyDayTex")]
private void PreviewSkyDayTex()
{
    if (!string.IsNullOrEmpty(SkyDayTexPath))
    {
        SkyDayTex = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture>("Assets/AssetsPackage/" + SkyDayTexPath);
        return;
    }
    SkyDayTex = null;
}
```

**功能**:
1. 根据 Path 加载纹理
2. 显示在 Texture 字段中预览
3. 如果 Path 为空，清空 Texture

---

## Nino 序列化特性

### NinoType

```csharp
[NinoType(false)]
```

**说明**: 标记为 Nino 可序列化类型。

### NinoMember

```csharp
[NinoMember(1)]  // DefaultEnvironment
[NinoMember(2)]  // Environments
[NinoMember(3)]  // DefaultBlend
[NinoMember(4)]  // SkyDayTexPath
[NinoMember(5)]  // SkyNightTexPath
[NinoMember(6)]  // SkySunriseTexPath
[NinoMember(7)]  // SkySunsetTexPath
```

**说明**: 显式指定成员序列化顺序。

---

## Odin Inspector 集成

### BoxGroup 分组

```csharp
[NinoMember(4)][BoxGroup("SkyDayTex")]
public string SkyDayTexPath;

[NinoMember(5)][BoxGroup("SkyNightTex")]
public string SkyNightTexPath;
```

**效果**: Inspector 中将天空盒路径按类型分组显示

**布局**:
```
┌─ SkyDayTex ─────────────┐
│ SkyDayTexPath: _________ │
│ SkyDayTex: [Object]     │
│ [预览 SkyDayTex]        │
└─────────────────────────┘
```

### HideReferenceObjectPicker

```csharp
[NinoMember(3)][HideReferenceObjectPicker]
public ConfigBlender DefaultBlend;
```

**效果**: 不显示对象选择器，直接展开显示 ConfigBlender 的字段

**用途**: 简化配置界面

---

## 使用示例

### 示例 1: 完整环境配置

```csharp
var environments = new ConfigEnvironments
{
    DefaultEnvironment = new ConfigEnvironment
    {
        Id = 0,
        TintColor = Color.white,
        UseDirLight = true,
        LightColor = Color.white,
        LightIntensity = 1f
    },
    
    Environments = new[]
    {
        new ConfigEnvironment
        {
            Id = 1,
            TintColor = new Color(1f, 0.95f, 0.8f),
            UseDirLight = true,
            LightColor = new Color(1f, 0.9f, 0.7f),
            LightIntensity = 1.5f,
            LightShadows = LightShadows.Soft,
            StarSpeed = 0f,
            NebulaSpeed = 0f
        },
        new ConfigEnvironment
        {
            Id = 2,
            TintColor = new Color(0.1f, 0.1f, 0.3f),
            UseDirLight = false,
            LightIntensity = 0.3f,
            StarSpeed = 1f,
            NebulaSpeed = 0.5f
        }
    },
    
    DefaultBlend = new ConfigBlender { DeltaTime = 2000 },
    
    SkyDayTexPath = "Sky/DaySkybox",
    SkyNightTexPath = "Sky/NightSkybox",
    SkySunriseTexPath = "Sky/SunriseSkybox",
    SkySunsetTexPath = "Sky/SunsetSkybox"
};
```

### 示例 2: 查询环境配置

```csharp
// 根据 ID 查找环境
ConfigEnvironment GetEnvironmentById(int id)
{
    foreach (var env in environments.Environments)
    {
        if (env.Id == id)
            return env;
    }
    return environments.DefaultEnvironment;
}
```

---

## 路径格式规范

### 资源路径格式

```
Assets/AssetsPackage/Sky/DaySkybox → Sky/DaySkybox
```

**转换逻辑**:
```csharp
if (path.StartsWith("Assets/AssetsPackage/"))
{
    SkyDayTexPath = path.Replace("Assets/AssetsPackage/", "");
}
```

**用途**:
- 统一路径格式
- 便于运行时加载
- 与资源管理系统配合

---

## 相关文档

- [ConfigEnvironment.cs.md](./ConfigEnvironment.cs.md) - 单个环境配置
- [ConfigBlender.cs.md](../Blender/ConfigBlender.cs.md) - 过渡参数
- [Unity Skybox 文档](https://docs.unity3d.com/Manual/class-Skybox.html)

---

*文档生成时间：2026-02-28 | OpenClaw AI 助手*
