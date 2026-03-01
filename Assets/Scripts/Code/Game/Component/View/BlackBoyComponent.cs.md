# BlackBoyComponent.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | BlackBoyComponent.cs |
| **路径** | Assets/Scripts/Code/Game/Component/View/BlackBoyComponent.cs |
| **所属模块** | 游戏层 → Component/View |
| **文件职责** | 黑色角色效果组件，用于特卖活动时将角色材质变为黑色 |

---

## 类说明

### BlackBoyComponent

| 属性 | 说明 |
|------|------|
| **职责** | 将角色模型的材质全部变为黑色，用于特卖活动视觉效果 |
| **泛型参数** | `IComponent` - 无参数组件 |
| **继承关系** | 继承 `Component` |
| **实现的接口** | `IComponent` |

**使用场景**: 特卖活动（SaleEvent）中的特殊视觉效果

---

## 字段与属性

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `ConfigId` | `int` | `public` | 特卖活动配置 ID（默认 100001） |
| `SaleEventConfig` | `SaleEventConfig` | `public` | 特卖活动配置对象 |
| `ghc` | `GameObjectHolderComponent` | `private` | 游戏对象持有组件（获取实体视图） |
| `skins` | `SkinnedMeshRenderer[]` | `private` | 蒙皮渲染器数组 |

---

## 方法说明

### Init()

**签名**:
```csharp
public void Init()
```

**职责**: 初始化黑色效果

**核心逻辑**:
```
1. 从 GameObjectHolderComponent 获取实体视图
2. 获取所有 SkinnedMeshRenderer 组件
3. 遍历所有材质:
   - 创建新材质实例
   - 设置颜色为黑色 (Color.black)
4. 从配置表随机选择一个特卖活动配置
5. 设置 ConfigId
```

**调用者**: 组件系统创建组件时

**视觉效果**:
```
原角色 → 应用 BlackBoyComponent → 全黑角色
```

---

### Destroy()

**签名**:
```csharp
public void Destroy()
```

**职责**: 销毁黑色效果，恢复原材质

**核心逻辑**:
```
1. 遍历所有 SkinnedMeshRenderer:
   - 遍历所有材质:
     * 销毁新材质实例 GameObject.Destroy()
     * 恢复为原始共享材质 sharedMaterials[j]
2. 清空 skins 数组引用
```

**调用者**: 组件系统销毁组件时

**注意事项**: 
- 必须销毁动态创建的材质实例，避免内存泄漏
- 必须恢复原始材质，避免影响其他组件

---

## 使用示例

### 示例 1: 应用黑色效果

```csharp
// 创建特卖活动角色
var entity = EntityFactory.CreateEntity<Entity>(scene.Id);

// 添加 GameObjectHolderComponent（持有模型）
var ghc = entity.AddComponent<GameObjectHolderComponent>();
ghc.Init(modelPath);

// 添加 BlackBoyComponent（黑色效果）
var blackBoy = entity.AddComponent<BlackBoyComponent>();
// 自动调用 Init()，角色变为黑色
```

### 示例 2: 移除黑色效果

```csharp
// 移除组件，自动恢复原材质
entity.RemoveComponent<BlackBoyComponent>();
// 或
blackBoy.Dispose();
```

### 示例 3: 特卖活动管理

```csharp
public class SaleEventManager
{
    public void StartSaleEvent(Entity participant)
    {
        // 添加黑色效果
        if (!participant.HasComponent<BlackBoyComponent>())
        {
            participant.AddComponent<BlackBoyComponent>();
        }
        
        // 获取活动配置
        var config = participant.GetComponent<BlackBoyComponent>().SaleEventConfig;
        ApplySaleEventRules(config);
    }
    
    public void EndSaleEvent(Entity participant)
    {
        // 移除黑色效果
        participant.RemoveComponent<BlackBoyComponent>();
    }
}
```

---

## 设计说明

### 材质管理

组件使用**动态材质实例**方式：

```csharp
// 创建新材质实例（不影响原始材质）
var newMat = skin.materials[j];  // 这会创建实例
newMat.color = Color.black;

// 销毁时恢复原始共享材质
GameObject.Destroy(skin.materials[j]);
skin.materials[j] = skin.sharedMaterials[j];  // 恢复原始
```

### 为什么使用 sharedMaterials？

- `materials`: 返回材质实例数组（修改会影响渲染）
- `sharedMaterials`: 返回共享材质引用（修改会影响所有使用该材质的对象）

组件在 Init 时修改 `materials`（创建实例），Destroy 时恢复 `sharedMaterials`（原始资源）。

---

## 特卖活动配置

### SaleEventConfig 结构

```csharp
// 配置表示例
{
    Id: 100001,
    Name: "黑色星期五特卖",
    DiscountRate: 0.5f,      // 5 折优惠
    Duration: 3600,          // 持续 1 小时
    // ...
}
```

### 随机选择逻辑

```csharp
// 从所有特卖活动中随机选择一个
var list = SaleEventConfigCategory.Instance.GetAllList();
var index = Random.Range(0, list.Count);
ConfigId = list[index].Id;
```

---

## 注意事项

1. **材质泄漏**: 必须销毁动态创建的材质实例
2. **恢复原始**: 销毁时必须恢复原始共享材质
3. **依赖组件**: 依赖 GameObjectHolderComponent 提供实体视图
4. **性能考虑**: 遍历所有 SkinnedMeshRenderer，复杂模型可能影响性能

---

## 相关文档

- [Component.cs.md](../Component.cs.md) - 组件基类
- [GameObjectHolderComponent.cs.md](./GameObjectHolderComponent.cs.md) - 游戏对象持有组件
- [SaleEventConfig.cs.md](../../../Module/Config/SaleEventConfig.cs.md) - 特卖活动配置

---

*文档生成时间：2026-03-02 | OpenClaw AI 助手*
