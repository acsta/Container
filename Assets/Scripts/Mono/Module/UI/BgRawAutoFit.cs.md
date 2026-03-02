# BgRawAutoFit.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | BgRawAutoFit.cs |
| **路径** | Assets/Scripts/Mono/Module/UI/BgRawAutoFit.cs |
| **所属模块** | 框架层 → Mono/Module/UI |
| **文件职责** | RawImage 背景自适应组件，根据屏幕比例自动调整背景大小 |

---

## 类说明

### BgRawAutoFit

| 属性 | 说明 |
|------|------|
| **职责** | 附加到 RawImage 上，根据屏幕分辨率和设计稿比例自动缩放背景图片，确保全屏覆盖 |
| **继承关系** | `MonoBehaviour` |
| **依赖组件** | `RawImage`（必需） |
| **执行模式** | `[ExecuteAlways]`（编辑器 + 运行时） |

**设计模式**: 自适应布局 + 屏幕适配

```csharp
// 使用方式
// 1. 在 RawImage GameObject 上添加 BgRawAutoFit 组件
// 2. 配置 bgSprite 纹理（可选）
// 3. 启动时自动计算并调整大小
```

---

## 字段与属性

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `rectTransform` | `RectTransform` | `private` | RectTransform 组件引用 |
| `bg` | `RawImage` | `private` | RawImage 组件引用 |
| `bgSprite` | `Texture` | `public` | 背景纹理（可在 Inspector 配置） |

---

## 方法说明

### Awake()

**签名**:
```csharp
private void Awake()
```

**职责**: 获取组件引用

**核心逻辑**:
```
1. 获取 RectTransform 组件
2. 获取 RawImage 组件
```

---

### Start()

**签名**:
```csharp
void Start()
```

**职责**: 初始化背景纹理并计算大小

**核心逻辑**:
```
1. 如果 bgSprite 为空，使用 RawImage 的 texture
2. 否则设置 RawImage 的 texture 为 bgSprite
3. 调用 Size() 计算并调整大小
```

---

### Size()

**签名**:
```csharp
void Size()
```

**职责**: 根据屏幕比例计算并设置背景大小

**核心逻辑**:
```
1. 获取屏幕宽高（SystemInfoHelper.screenWidth/Height）
2. 计算设计稿宽高比 flagx = DesignScreenWidth / DesignScreenHeight
3. 计算实际屏幕宽高比 flagy = screenWidth / screenHeight
4. 计算缩放因子 signFlag
5. 计算纹理缩放比 flag1, flag2
6. 根据比例设置 rectTransform.sizeDelta
7. 调整位置到 Canvas 中心
```

**屏幕适配逻辑**:
```csharp
// 设计稿比例 vs 实际屏幕比例
var flagx = Define.DesignScreenWidth / Define.DesignScreenHeight;  // 如 1920/1080 = 1.778
var flagy = (float)screenW / screenH;  // 实际屏幕比例

// 选择较小的缩放因子（确保覆盖）
var signFlag = flagx > flagy
    ? Define.DesignScreenWidth / screenW
    : Define.DesignScreenHeight / screenH;
```

---

### SetTexture(Texture newBgSprite)

**签名**:
```csharp
public void SetTexture(Texture newBgSprite)
```

**职责**: 运行时动态更换背景纹理

**核心逻辑**:
```
1. 设置 bgSprite = newBgSprite
2. 更新 RawImage.texture
3. 重新调用 Size() 计算大小
```

---

## 使用示例

### 示例 1: 基础背景适配

```
Unity 编辑器配置:
1. 创建 UI 背景 GameObject
2. 添加 RawImage 组件
3. 添加 BgRawAutoFit 组件
4. 配置 Texture
5. 启动时自动适配全屏
```

### 示例 2: 运行时切换背景

```csharp
public class BackgroundManager : MonoBehaviour
{
    [SerializeField] private BgRawAutoFit bgAutoFit;
    
    [SerializeField] private Texture dayBackground;
    [SerializeField] private Texture nightBackground;
    
    public void SetDayBackground()
    {
        bgAutoFit.SetTexture(dayBackground);
    }
    
    public void SetNightBackground()
    {
        bgAutoFit.SetTexture(nightBackground);
    }
}
```

### 示例 3: 多分辨率适配

```csharp
// Define.cs 中的设计稿配置
public class Define
{
    public static int DesignScreenWidth = 1920;
    public static int DesignScreenHeight = 1080;
}

// 不同设备上的适配:
// iPhone 13 Pro: 1170 x 2532 → 自动计算缩放
// iPad Pro: 2048 x 2732 → 自动计算缩放
// Desktop: 1920 x 1080 → 1:1 显示
```

---

## 屏幕适配原理

### 比例计算

```
设计稿：1920 x 1080 (16:9)

情况 1: 实际屏幕 2532 x 1170 (iPhone 13 Pro, 更宽)
├── flagx = 1920/1080 = 1.778
├── flagy = 2532/1170 = 2.164
├── flagx < flagy → 使用高度基准
└── signFlag = 1080/1170 = 0.923

情况 2: 实际屏幕 1080 x 1920 (竖屏)
├── flagx = 1920/1080 = 1.778
├── flagy = 1080/1920 = 0.5625
├── flagx > flagy → 使用宽度基准
└── signFlag = 1920/1080 = 1.778
```

### 适配策略

```
宽屏设备（flagy > flagx）:
└── 按高度缩放，两侧可能有留白

窄屏设备（flagy < flagx）:
└── 按宽度缩放，上下可能有留白

目标：确保背景完全覆盖屏幕
```

---

## 技术要点

### 1. ExecuteAlways 特性

```csharp
[ExecuteAlways]
public class BgRawAutoFit : MonoBehaviour
```
- 在编辑器中也会执行
- 方便实时预览效果

### 2. Prefab 保护

```csharp
#if UNITY_EDITOR
var type = UnityEditor.PrefabUtility.GetPrefabAssetType(gameObject);
var status = UnityEditor.PrefabUtility.GetPrefabInstanceStatus(gameObject);
if (type != UnityEditor.PrefabAssetType.NotAPrefab && 
    status != UnityEditor.PrefabInstanceStatus.NotAPrefab)
{
    return;  // Prefab 资源不执行
}
#endif
```

### 3. Canvas 中心对齐

```csharp
var parent = transform.parent;
var siblingIndex = transform.GetSiblingIndex();
transform.SetParent(canvas.transform);
rectTransform.localPosition = Vector3.zero;
rectTransform.anchoredPosition = Vector2.zero;
transform.SetParent(parent, true);
transform.SetSiblingIndex(siblingIndex);
```

---

## 相关文档

- **BgAutoFit**: [BgAutoFit.cs.md](./BgAutoFit.cs.md) - Image 背景自适应
- **BgAutoMax**: [BgAutoMax.cs.md](./BgAutoMax.cs.md) - 纯大小自适应
- **SystemInfoHelper**: SystemInfoHelper.cs.md - 系统信息助手
- **Define**: Define.cs.md - 全局定义

---

## 注意事项

### ⚠️ RawImage vs Image

BgRawAutoFit 用于 RawImage（Texture），BgAutoFit 用于 Image（Sprite）。

### ⚠️ 纹理尺寸

确保背景纹理足够大，避免缩放后模糊。

### ⚠️ 性能

Size() 方法在 Start 时调用一次，避免每帧调用。

### ⚠️ Define 配置

确保 Define.DesignScreenWidth/Height 正确配置。

---

*文档生成时间：2026-03-01 | OpenClaw AI 助手*
