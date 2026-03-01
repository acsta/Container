# ExportBones.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | ExportBones.cs |
| **路径** | Assets/Scripts/Mono/Module/Entity/ExportBones.cs |
| **所属模块** | 框架层 → Mono/Module/Entity |
| **文件职责** | 编辑器工具，用于搜集和导出角色模型的骨骼信息到 Prefab |

---

## 类说明

### ExportBones

| 属性 | 说明 |
|------|------|
| **职责** | 在 Unity 编辑器中运行，遍历角色模型的 SkinnedMeshRenderer，搜集骨骼信息并保存到 Prefab 的 BonesData 组件 |
| **继承关系** | `MonoBehaviour` |
| **执行环境** | 仅 Unity 编辑器（`#if UNITY_EDITOR`） |

**设计模式**: 编辑器工具脚本

```csharp
// 使用方式
// 1. 在角色模型 GameObject 上添加 ExportBones 组件
// 2. 在 Inspector 中点击"搜集骨骼信息"按钮
// 3. 脚本会自动遍历子物体，将骨骼信息保存到对应 Prefab
```

**编辑器限定**:
```csharp
#if UNITY_EDITOR
    // 仅在编辑器中可用的代码
#endif
```

---

## 字段说明

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| 无公开字段 | - | - | 仅通过按钮触发功能 |

---

## 方法说明

### Export()

**签名**:
```csharp
[Sirenix.OdinInspector.Button("搜集骨骼信息")]
public void Export()
```

**职责**: 遍历角色模型的所有 SkinnedMeshRenderer，搜集骨骼信息并导出到 Prefab

**核心逻辑**:
```
1. 获取所有子物体的 SkinnedMeshRenderer 组件
2. 遍历每个 SkinnedMeshRenderer:
   a. 检查父级的父级是否为"Parts"（服装部件）
   b. 获取或添加 BonesData 组件
   c. 调用 BonesData.Collect() 搜集骨骼信息
   d. 加载对应的 Prefab 资源
   e. 将骨骼信息复制到 Prefab 的 BonesData 组件
   f. 标记 Prefab 为脏并保存
   g. 清理临时组件
```

**调用者**: Inspector 中的按钮（仅编辑器）

**依赖**:
- Sirenix.OdinInspector（按钮特性）
- Unity 编辑器 API（AssetDatabase, EditorUtility）

---

## 工作流程

### 完整流程图

```
用户点击"搜集骨骼信息"按钮
           │
           ▼
   获取所有 SkinnedMeshRenderer
           │
           ▼
   遍历每个 SkinnedMeshRenderer
           │
           ▼
   ┌──────────────────────┐
   │  父级的父级 == "Parts"? │
   └──────────┬───────────┘
              │ 是
              ▼
   获取/添加 BonesData 组件
              │
              ▼
   调用 BonesData.Collect()
              │
              ▼
   加载对应 Prefab 资源
              │
              ▼
   复制骨骼信息到 Prefab
              │
              ▼
   标记 Prefab 为脏并保存
              │
              ▼
   清理临时 BonesData 组件
              │
              ▼
         完成
```

---

## 目录结构要求

### 资源路径约定

```
Assets/AssetsPackage/Unit/Charater/Prefabs/
├── CharacterA/
│   ├── Body.prefab
│   ├── Head.prefab
│   └── Arm_Left.prefab
├── CharacterB/
│   ├── Body.prefab
│   └── ...
└── ...
```

### 层级结构要求

```
角色模型 GameObject
└── Parts (父级的父级名称必须为"Parts")
    ├── Body
    │   └── SkinnedMeshRenderer
    ├── Head
    │   └── SkinnedMeshRenderer
    └── Arm
        └── SkinnedMeshRenderer
```

**路径计算逻辑**:
```csharp
// 脚本通过层级结构计算 Prefab 路径
"Assets/AssetsPackage/Unit/Charater/Prefabs/" + 
smrs[i].transform.parent.name + "/" +  // Parts 的父级名称（角色名）
smrs[i].transform.name + ".prefab"      // 部件名称
```

---

## 使用示例

### 示例 1: 基础使用

```
步骤：
1. 在项目中打开角色模型场景
2. 选中角色模型 GameObject
3. 添加 ExportBones 组件
4. 在 Inspector 中点击"搜集骨骼信息"按钮
5. 等待处理完成
6. 检查 Prefab 是否已更新
```

### 示例 2: 批量处理多个角色

```csharp
// 创建批量导出工具（扩展）
#if UNITY_EDITOR
public class BatchExportBones : EditorWindow
{
    [MenuItem("Tools/批量导出骨骼信息")]
    static void ShowWindow()
    {
        GetWindow<BatchExportBones>("批量导出");
    }
    
    void OnGUI()
    {
        if (GUILayout.Button("导出所有角色骨骼"))
        {
            string[] guids = AssetDatabase.FindAssets("t:GameObject", 
                new[] { "Assets/AssetsPackage/Unit/Charater" });
            
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                
                if (prefab != null)
                {
                    var exporter = prefab.AddComponent<ExportBones>();
                    exporter.Export();
                    GameObject.DestroyImmediate(exporter);
                }
            }
        }
    }
}
#endif
```

### 示例 3: 验证导出结果

```csharp
// 验证脚本
#if UNITY_EDITOR
public class VerifyBonesExport : MonoBehaviour
{
    [Sirenix.OdinInspector.Button("验证骨骼导出")]
    public void Verify()
    {
        var bonesDataList = GetComponentsInChildren<BonesData>();
        
        foreach (var bonesData in bonesDataList)
        {
            if (bonesData.bones == null || bonesData.bones.Length == 0)
            {
                Debug.LogWarning($"未导出骨骼：{bonesData.gameObject.name}", bonesData);
            }
            else
            {
                Debug.Log($"已导出 {bonesData.bones.Length} 根骨骼：{bonesData.gameObject.name}");
            }
        }
    }
}
#endif
```

---

## 技术要点

### 1. 编辑器限定

```csharp
#if UNITY_EDITOR
    // 仅在编辑器中编译
    [Sirenix.OdinInspector.Button("搜集骨骼信息")]
    public void Export()
    {
        // 使用编辑器 API
        var prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(...);
        UnityEditor.EditorUtility.SetDirty(prefab);
        UnityEditor.AssetDatabase.SaveAssetIfDirty(prefab);
    }
#endif
```

### 2. 资源路径计算

```csharp
// 根据层级结构计算 Prefab 路径
string path = "Assets/AssetsPackage/Unit/Charater/Prefabs/" + 
              smrs[i].transform.parent.parent.name + "/" +
              smrs[i].transform.name + ".prefab";
```

### 3. 临时组件清理

```csharp
// 导出完成后清理临时添加的 BonesData 组件
GameObject.DestroyImmediate(data);
```

### 4. Prefab 修改保存

```csharp
// 标记 Prefab 为脏
UnityEditor.EditorUtility.SetDirty(prefab);

// 保存更改
UnityEditor.AssetDatabase.SaveAssetIfDirty(prefab);
```

---

## 与 BonesData 的关系

```
ExportBones (工具)
    │
    │ 调用 Collect()
    ▼
BonesData (数据组件)
    │
    │ 存储骨骼名称数组
    ▼
Prefab (资源文件)
    │
    │ 运行时加载
    ▼
运行时骨骼映射系统
```

**详细说明**: 参见 [BonesData.cs.md](./BonesData.cs.md)

---

## 注意事项

### ⚠️ 仅编辑器可用

ExportBones 仅在 Unity 编辑器中有效，构建后的游戏不包含此功能。

### ⚠️ Odin Inspector 依赖

使用 `[Sirenix.OdinInspector.Button]` 特性需要安装 Odin Inspector 插件。

如未安装，可改用 Unity 原生方式：
```csharp
#if UNITY_EDITOR
[ContextMenu("搜集骨骼信息")]
public void Export()
{
    // 右键菜单触发
}
#endif
```

### ⚠️ 目录结构要求

确保角色 Prefab 位于正确的目录：
```
Assets/AssetsPackage/Unit/Charater/Prefabs/{角色名}/{部件名}.prefab
```

### ⚠️ "Parts"层级要求

脚本检查 `transform.parent.parent.name == "Parts"`，确保层级结构正确。

### ⚠️ 资源加载

使用 `AssetDatabase.LoadAssetAtPath` 加载 Prefab，确保路径正确。

### ⚠️ 内存管理

导出完成后使用 `DestroyImmediate` 清理临时组件，避免内存泄漏。

---

## 常见问题

### Q: 点击按钮没有反应？

**A**: 检查：
1. 是否在编辑器模式（非播放模式）
2. 是否安装了 Odin Inspector
3. 是否有 SkinnedMeshRenderer 组件
4. 层级结构是否正确（Parts 父级）

### Q: 导出的 Prefab 找不到？

**A**: 检查路径计算逻辑，确保：
1. 角色名与目录名一致
2. 部件名与 Prefab 名一致
3. 目录存在且可写

### Q: 骨骼信息为空？

**A**: 检查 SkinnedMeshRenderer 的 bones 数组是否有数据，可能模型未正确绑定骨骼。

---

## 相关文档

- **BonesData**: [BonesData.cs.md](./BonesData.cs.md) - 骨骼数据组件
- **EntityType**: [EntityType.cs.md](./EntityType.cs.md) - 实体类型定义

---

*文档生成时间：2026-03-01 | OpenClaw AI 助手*
