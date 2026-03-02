# LabelConfig.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | LabelConfig.cs |
| **路径** | Assets/Scripts/Editor/ArtEditor/AssetsManager/Config/LabelConfig.cs |
| **所属模块** | Editor 工具 → 美术编辑器 → 资产管理 → 配置 |
| **文件职责** | 定义资源标签配置结构，用于分类管理资源目录 |

---

## 类/结构体说明

### LabelConfig

| 属性 | 说明 |
|------|------|
| **职责** | 定义资源标签配置，包含大类标签和多个小类收集配置 |
| **泛型参数** | 无 |
| **继承关系** | 无继承 |
| **实现的接口** | 无 |

**设计模式**: 配置数据模式

```csharp
// Odin Inspector 序列化特性
[Serializable]
public class LabelConfig
```

**依赖条件**: 需要 Odin Inspector 插件 (`#if ODIN_INSPECTOR`)

---

## 字段与属性

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `Label` | `string` | `public` | 大类标签名称，用于资源分类 |
| `Collects` | `List<CollectConfig>` | `public` | 小类收集配置列表，包含多个子分类 |

---

## 方法说明

本类为纯数据配置类，无方法。

---

## 使用示例

```csharp
// 创建标签配置
var labelConfig = new LabelConfig
{
    Label = "角色",
    Collects = new List<CollectConfig>
    {
        new CollectConfig 
        { 
            Label = "主角", 
            Objects = new List<Object>() 
        },
        new CollectConfig 
        { 
            Label = "NPC", 
            Objects = new List<Object>() 
        }
    }
};

// 在 AssetsManagerConfig 中使用
var config = ScriptableObject.CreateInstance<AssetsManagerConfig>();
config.Labels.Add(labelConfig);
```

---

## 相关文档

- [AssetsManagerConfig.cs.md](./AssetsManagerConfig.cs.md) - 资产管理配置主类
- [CollectConfig.cs.md](./CollectConfig.cs.md) - 小类收集配置

---

*最后更新：2026-03-02*
