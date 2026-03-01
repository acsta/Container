# BonesData.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | BonesData.cs |
| **路径** | Assets/Scripts/Mono/Module/Entity/BonesData.cs |
| **所属模块** | 框架层 → Mono/Module/Entity |
| **文件职责** | 存储 SkinnedMeshRenderer 的骨骼名称数组，用于运行时骨骼映射 |

---

## 类说明

### BonesData

| 属性 | 说明 |
|------|------|
| **职责** | 附加到带有 SkinnedMeshRenderer 的 GameObject 上，存储骨骼名称数组，用于运行时骨骼重映射或动画系统 |
| **继承关系** | `MonoBehaviour` |
| **执行环境** | 运行时 + 编辑器（Collect 方法仅编辑器可用） |

**设计模式**: 数据组件

```csharp
// 使用方式
// 1. 在角色模型部件上添加 BonesData 组件
// 2. 在编辑器中点击"搜集骨骼信息"按钮
// 3. 骨骼名称数组会被保存，运行时可用于骨骼映射
```

---

## 字段说明

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `bones` | `string[]` | `public` | 骨骼名称数组，存储 SkinnedMeshRenderer 所有骨骼的名称 |

---

## 方法说明

### Collect()

**签名**:
```csharp
[Sirenix.OdinInspector.Button("搜集骨骼信息")]
public void Collect()
```

**职责**: 从 SkinnedMeshRenderer 中提取骨骼名称并存储到 bones 数组

**核心逻辑**:
```
1. 获取 SkinnedMeshRenderer 组件
2. 检查是否有效（组件存在且 bones 数组非空）
3. 创建新的字符串数组（长度与骨骼数相同）
4. 遍历所有骨骼，复制骨骼名称到数组
5. 保存到 bones 字段
```

**调用者**: 
- Inspector 中的按钮（编辑器）
- ExportBones.Export()（批量导出时）

**编辑器限定**:
```csharp
#if UNITY_EDITOR
    // 仅在编辑器中可用
#endif
```

---

## 使用示例

### 示例 1: 基础使用 - 编辑器搜集

```
步骤：
1. 选中带有 SkinnedMeshRenderer 的 GameObject
2. 添加 BonesData 组件
3. 在 Inspector 中点击"搜集骨骼信息"按钮
4. bones 数组会被填充骨骼名称
5. 保存场景/Prefab
```

### 示例 2: 运行时读取骨骼信息

```csharp
public class BoneMapper : MonoBehaviour
{
    void Start()
    {
        var bonesData = GetComponent<BonesData>();
        if (bonesData != null && bonesData.bones != null)
        {
            Debug.Log($"模型包含 {bonesData.bones.Length} 根骨骼:");
            foreach (string boneName in bonesData.bones)
            {
                Debug.Log($"  - {boneName}");
            }
        }
    }
}
```

### 示例 3: 骨骼映射系统

```csharp
public class SkeletonMapper : MonoBehaviour
{
    [SerializeField] private BonesData sourceBones;
    [SerializeField] private BonesData targetBones;
    [SerializeField] private Transform sourceSkeleton;
    [SerializeField] private Transform targetSkeleton;
    
    void Start()
    {
        MapBones();
    }
    
    void MapBones()
    {
        if (sourceBones.bones == null || targetBones.bones == null)
        {
            Debug.LogError("骨骼数据未初始化");
            return;
        }
        
        // 根据骨骼名称映射
        for (int i = 0; i < targetBones.bones.Length; i++)
        {
            string targetBoneName = targetBones.bones[i];
            
            // 在源骨骼中查找同名骨骼
            int sourceIndex = System.Array.IndexOf(sourceBones.bones, targetBoneName);
            if (sourceIndex >= 0)
            {
                // 找到对应骨骼，建立映射
                Transform sourceBone = FindBoneByName(sourceSkeleton, targetBoneName);
                Transform targetBone = FindBoneByName(targetSkeleton, targetBoneName);
                
                if (sourceBone != null && targetBone != null)
                {
                    // 复制动画或建立关联
                    Debug.Log($"映射：{targetBoneName}");
                }
            }
        }
    }
    
    Transform FindBoneByName(Transform root, string name)
    {
        foreach (Transform child in root.GetComponentsInChildren<Transform>())
        {
            if (child.name == name)
                return child;
        }
        return null;
    }
}
```

### 示例 4: 换装系统

```csharp
public class EquipmentSystem : MonoBehaviour
{
    // 将装备的骨骼映射到角色骨骼
    public void AttachEquipment(GameObject equipment, GameObject character)
    {
        var equipBones = equipment.GetComponent<BonesData>();
        var charBones = character.GetComponent<BonesData>();
        
        if (equipBones == null || charBones == null)
        {
            Debug.LogError("缺少 BonesData 组件");
            return;
        }
        
        // 遍历装备骨骼，找到对应角色骨骼
        var equipSkinner = equipment.GetComponent<SkinnedMeshRenderer>();
        var charSkinner = character.GetComponent<SkinnedMeshRenderer>();
        
        if (equipSkinner != null && charSkinner != null)
        {
            // 重新绑定装备到角色骨骼
            equipSkinner.bones = charSkinner.bones;
            equipSkinner.rootBone = charSkinner.rootBone;
        }
    }
}
```

### 示例 5: 骨骼动画重定向

```csharp
public class AnimationRetargeter : MonoBehaviour
{
    [System.Serializable]
    public class BoneMapping
    {
        public string sourceBoneName;
        public string targetBoneName;
        public Transform sourceTransform;
        public Transform targetTransform;
    }
    
    public List<BoneMapping> boneMappings = new List<BoneMapping>();
    
    public void SetupMapping(BonesData sourceData, BonesData targetData, 
                             Transform sourceRoot, Transform targetRoot)
    {
        boneMappings.Clear();
        
        foreach (string boneName in sourceData.bones)
        {
            // 检查目标是否有同名骨骼
            if (System.Array.IndexOf(targetData.bones, boneName) >= 0)
            {
                boneMappings.Add(new BoneMapping
                {
                    sourceBoneName = boneName,
                    targetBoneName = boneName,
                    sourceTransform = FindBone(sourceRoot, boneName),
                    targetTransform = FindBone(targetRoot, boneName)
                });
            }
        }
    }
    
    void LateUpdate()
    {
        // 每帧复制骨骼变换
        foreach (var mapping in boneMappings)
        {
            if (mapping.sourceTransform != null && mapping.targetTransform != null)
            {
                mapping.targetTransform.localPosition = mapping.sourceTransform.localPosition;
                mapping.targetTransform.localRotation = mapping.sourceTransform.localRotation;
                mapping.targetTransform.localScale = mapping.sourceTransform.localScale;
            }
        }
    }
    
    Transform FindBone(Transform root, string name)
    {
        foreach (Transform t in root.GetComponentsInChildren<Transform>())
        {
            if (t.name == name) return t;
        }
        return null;
    }
}
```

---

## 与 ExportBones 的配合

### 工作流程

```
ExportBones (编辑器工具)
    │
    │ 遍历 SkinnedMeshRenderer
    ▼
BonesData (组件)
    │
    │ Collect() 搜集骨骼名称
    ▼
bones[] (字符串数组)
    │
    │ 保存到 Prefab
    ▼
运行时使用
    │
    ├→ 骨骼映射
    ├→ 换装系统
    └→ 动画重定向
```

### ExportBones 调用示例

```csharp
// ExportBones.Export() 中的调用
BonesData data = smrs[i].gameObject.GetComponent<BonesData>();
if (data == null)
{
    data = smrs[i].gameObject.AddComponent<BonesData>();
}
data.Collect();  // 搜集骨骼信息

// 复制到 Prefab
var newData = prefab.GetComponent<BonesData>();
if (newData == null)
{
    newData = prefab.AddComponent<BonesData>();
}
newData.bones = data.bones;  // 复制数组
```

---

## 技术要点

### 1. 骨骼名称 vs 骨骼引用

```csharp
// 存储名称（BonesData 方式）
public string[] bones;  // ["Hips", "Spine", "Head", ...]

// vs 存储引用（直接方式）
public Transform[] bones;  // [Transform, Transform, ...]

// 名称方式的优势：
// - 可序列化到 Prefab
// - 不依赖场景引用
// - 运行时动态查找
```

### 2. 编辑器限定

```csharp
#if UNITY_EDITOR
    [Sirenix.OdinInspector.Button("搜集骨骼信息")]
    public void Collect()
    {
        // 仅编辑器可用
    }
#endif
```

### 3. SkinnedMeshRenderer 骨骼

```csharp
// SkinnedMeshRenderer.bones 返回 Transform[]
var smr = GetComponent<SkinnedMeshRenderer>();
Transform[] bones = smr.bones;

// BonesData 存储名称
string[] boneNames = new string[bones.Length];
for (int i = 0; i < bones.Length; i++)
{
    boneNames[i] = bones[i].name;
}
```

---

## 数据结构

### 典型骨骼名称数组

```csharp
bones = new string[]
{
    "Hips",
    "Spine",
    "Spine1",
    "Spine2",
    "Neck",
    "Head",
    "LeftArm",
    "LeftForeArm",
    "LeftHand",
    "RightArm",
    "RightForeArm",
    "RightHand",
    "LeftUpLeg",
    "LeftLeg",
    "LeftFoot",
    "RightUpLeg",
    "RightLeg",
    "RightFoot"
};
```

---

## 相关文档

- **ExportBones**: [ExportBones.cs.md](./ExportBones.cs.md) - 骨骼导出工具
- **EntityType**: [EntityType.cs.md](./EntityType.cs.md) - 实体类型定义
- **Unity 文档**: SkinnedMeshRenderer - Unity 官方文档

---

## 注意事项

### ⚠️ 编辑器功能

`Collect()` 方法仅在编辑器中可用，运行时无法更新骨骼数据。

### ⚠️ 骨骼名称唯一性

确保骨骼名称在模型内唯一，否则映射可能出错。

### ⚠️ Prefab 保存

使用 ExportBones 导出后，确保 Prefab 已正确保存。

### ⚠️ 空数组检查

运行时使用前检查数组是否为空：
```csharp
if (bonesData.bones == null || bonesData.bones.Length == 0)
{
    Debug.LogWarning("骨骼数据为空");
    return;
}
```

### ⚠️ Odin Inspector 依赖

按钮特性需要 Odin Inspector 插件，如未安装可改用：
```csharp
#if UNITY_EDITOR
[ContextMenu("搜集骨骼信息")]
public void Collect() { }
#endif
```

---

*文档生成时间：2026-03-01 | OpenClaw AI 助手*
