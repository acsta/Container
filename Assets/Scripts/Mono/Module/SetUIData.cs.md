# SetUIData.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|------|
| **文件名** | SetUIData.cs |
| **路径** | Assets/Scripts/Mono/Module/SetUIData.cs |
| **所属模块** | 框架层 → Mono/Module |
| **文件职责** | 将 UI 图片的 Atlas 数据传递给 Shader，用于自定义渲染效果 |

---

## 类/结构体说明

### SetUIData

| 属性 | 说明 |
|------|------|
| **职责** | 附加到 UI Image 上，每帧更新 Sprite 的 UV 矩形数据到 Shader |
| **泛型参数** | 无 |
| **继承关系** | 继承 `MonoBehaviour` |
| **实现的接口** | 无 |

**设计模式**: 组件模式

```csharp
// 添加到 UI Image GameObject
// [RequireComponent(typeof(Image))] 自动添加 Image 组件
// [RequireComponent(typeof(Material))] 需要材质

// 自动在 Start 和 Update 中更新 Atlas 数据到 Shader
```

---

## 字段与属性

### AltasData

| 属性 | 值 |
|------|------|
| **类型** | `Vector4` |
| **访问级别** | `private` |
| **说明** | Atlas 数据 (x, y, width, height) |

**用途**: 缓存 Sprite 在 Atlas 中的矩形区域

---

### ID

| 属性 | 值 |
|------|------|
| **类型** | `int` |
| **访问级别** | `private static` |
| **说明** | Shader 属性 ID ("_AltasData") |

**用途**: 快速设置 Shader 向量参数

---

## 方法说明

### Start

**签名**:
```csharp
public void Start()
```

**职责**: 初始化时设置 Atlas 数据到 Shader

**核心逻辑**:
```
1. 获取 Image 组件
2. 获取 Image 的材质
3. 检查 Sprite 是否存在
4. 获取 Sprite 的 textureRect (x, y, width, height)
5. 设置到 Shader 的 _AltasData 参数
```

**调用者**: Unity 生命周期

---

### Update

**签名**:
```csharp
public void Update()
```

**职责**: 每帧更新 Atlas 数据到 Shader

**核心逻辑**:
```
1. 获取 Image 组件
2. 获取 Image 的材质
3. 检查 Sprite 是否存在
4. 获取 Sprite 的 textureRect
5. 设置到 Shader 的 _AltasData 参数
```

**调用者**: Unity 生命周期

**用途**: 支持运行时动态更换 Sprite

---

## Atlas 数据说明

### textureRect 含义

```csharp
AltasData = new Vector4(
    Image.sprite.textureRect.x,      // Atlas 中的 X 偏移
    Image.sprite.textureRect.y,      // Atlas 中的 Y 偏移
    Image.sprite.textureRect.width,  // Sprite 宽度
    Image.sprite.textureRect.height  // Sprite 高度
);
```

**示意图**:
```
Atlas 纹理 (2048x2048)
┌─────────────────────────────────┐
│                                 │
│  ┌──────────┐                   │
│  │ Sprite A │ ← textureRect     │
│  │ (x,y,w,h)│                   │
│  └──────────┘                   │
│                                 │
│         ┌──────────┐            │
│         │ Sprite B │            │
│         └──────────┘            │
│                                 │
└─────────────────────────────────┘
```

---

## Shader 使用示例

### 自定义 Shader

```shader
Shader "Custom/UIImageEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AltasData ("Atlas Data", Vector) = (0,0,1,1)
    }
    
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            
            sampler2D _MainTex;
            float4 _AltasData; // (x, y, width, height)
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                
                // 使用 Atlas 数据调整 UV
                o.uv = v.uv * _AltasData.zw + _AltasData.xy / TEX_SIZE;
                
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
```

---

## 使用示例

### 示例 1: 基本使用

```
1. 创建 UI Image GameObject
2. 添加 SetUIData 组件
3. 自动在 Start/Update 中更新 Atlas 数据
4. Shader 接收 _AltasData 参数
```

### 示例 2: 动态更换 Sprite

```csharp
// 运行时更换 Sprite
public void ChangeSprite(Sprite newSprite)
{
    Image image = GetComponent<Image>();
    image.sprite = newSprite;
    
    // SetUIData.Update() 会自动检测并更新 Atlas 数据
}
```

### 示例 3: 自定义 Shader 效果

```csharp
// 使用自定义材质
Material customMaterial = new Material(Shader.Find("Custom/UIImageEffect"));
Image image = GetComponent<Image>();
image.material = customMaterial;

// SetUIData 会自动将 Atlas 数据传递给 Shader
```

---

## 设计要点

### 为什么需要 Atlas 数据？

**问题**: 
- UI Sprite 通常打包在 Atlas 中
- Shader 需要知道 Sprite 在 Atlas 中的位置
- 标准 UI Shader 不暴露这个信息

**解决方案**:
```
SetUIData 组件
├─ 获取 Sprite.textureRect
├─ 传递给 Shader._AltasData
└─ Shader 使用数据进行 UV 变换
```

### 为什么每帧更新？

```csharp
void Update()
{
    // 每帧都设置
    Material.SetVector(ID, AltasData);
}
```

**原因**:
- 支持运行时动态更换 Sprite
- 支持动画切换 Sprite
- 确保数据始终最新

**性能优化建议**:
- 如果 Sprite 不变，可改为仅在更换时更新
- 使用事件监听 Sprite 变化

### 为什么使用 [ExecuteAlways]？

```csharp
[ExecuteAlways]
public class SetUIData : MonoBehaviour
```

**作用**:
- 在编辑器模式下也执行 Start/Update
- 方便编辑器中预览效果
- 支持运行时和编辑器双模式

---

## 性能考虑

### 优化方案 1: 缓存检查

```csharp
private Sprite lastSprite;

void Update()
{
    if (Image.sprite == lastSprite) return; // 无变化则跳过
    lastSprite = Image.sprite;
    
    // 更新 Atlas 数据
}
```

### 优化方案 2: 事件驱动

```csharp
public void OnSpriteChanged(Sprite newSprite)
{
    // 仅在 Sprite 变化时更新
    UpdateAtlasData();
}
```

### 优化方案 3: 批量处理

```csharp
// 多个 UI 共享一个管理器
public class SetUIDataManager : MonoBehaviour
{
    private List<SetUIData> targets = new List<SetUIData>();
    
    void LateUpdate()
    {
        // 批量更新所有目标
        foreach (var target in targets)
        {
            target.UpdateAtlasData();
        }
    }
}
```

---

## 相关文档

- [UIManager.cs.md](../Code/Module/UI/UIManager.cs.md) - UI 管理器
- [UIImage.cs.md](../Code/Module/UIComponent/UIImage.cs.md) - UI 图片组件

---

*文档生成时间：2026-03-01 | OpenClaw AI 助手*
