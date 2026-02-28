# Helper 工具类与 Log 系统综合文档

> **覆盖文件**: SystemInfoHelper.cs, TypeInfo.cs, JsonHelper.cs, EasingFunction.cs, PlatformUtil.cs, PhysicsHelper.cs, Log.cs  
> **生成时间**: 2026-02-28  
> **命名空间**: `TaoTie`

---

## 📑 目录

1. [SystemInfoHelper - 系统信息](#1-systeminfohelper---系统信息)
2. [TypeInfo - 类型信息缓存](#2-typeinfo---类型信息缓存)
3. [JsonHelper - JSON 序列化](#3-jsonhelper---json-序列化)
4. [EasingFunction - 缓动函数](#4-easingfunction---缓动函数)
5. [PlatformUtil - 平台检测](#5-platformutil---平台检测)
6. [PhysicsHelper - 物理辅助](#6-physicshelper---物理辅助)
7. [Log - 日志系统](#7-log---日志系统)

---

## 1. SystemInfoHelper - 系统信息

### 类说明

| 属性 | 说明 |
|------|------|
| **职责** | 跨平台系统信息获取，支持各小游戏平台适配 |
| **类型** | `static class` |
| **支持平台** | WebGL (抖音/微信/快手/支付宝/ TapTap/QQ/华为/4399 等) |

### 核心 API

#### 屏幕尺寸

```csharp
// 屏幕宽度（像素）
float width = SystemInfoHelper.screenWidth;

// 屏幕高度（像素）
float height = SystemInfoHelper.screenHeight;
```

**平台适配**:
- WebGL 小游戏平台：调用平台 SDK 获取真实屏幕尺寸
- 其他平台：使用 `Screen.width/height`

#### 安全区域 ⭐

```csharp
// 获取安全区域（避开刘海、圆角、Home 条）
Rect safeArea = SystemInfoHelper.safeArea;

// 使用示例：适配 UI
RectTransform rectTransform = GetComponent<RectTransform>();
rectTransform.offsetMin = new Vector2(safeArea.xMin, safeArea.yMin);
rectTransform.offsetMax = new Vector2(-safeArea.xMax, -safeArea.yMax);
```

**平台差异**:
```csharp
// WebGL 小游戏平台
var safeArea = SystemInfo.safeArea; // 平台 SDK 提供
return Rect.MinMaxRect(left, top, right, bottom);

// 其他平台
var screenSafeArea = Screen.safeArea;
// 转换坐标系（Unity Y 轴向上，屏幕 Y 轴向下）
return Rect.MinMaxRect(
    screenSafeArea.xMin, 
    Screen.height - screenSafeArea.yMax,
    screenSafeArea.xMax, 
    Screen.height - screenSafeArea.yMin
);
```

### 使用示例

#### UI 安全区域适配

```csharp
public class SafeAreaFitter : MonoBehaviour
{
    private RectTransform rectTransform;
    
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        ApplySafeArea();
    }
    
    private void ApplySafeArea()
    {
        Rect safeArea = SystemInfoHelper.safeArea;
        
        // 转换为锚点
        Vector2 anchorMin = new Vector2(
            safeArea.xMin / SystemInfoHelper.screenWidth,
            safeArea.yMin / SystemInfoHelper.screenHeight
        );
        Vector2 anchorMax = new Vector2(
            safeArea.xMax / SystemInfoHelper.screenWidth,
            safeArea.yMax / SystemInfoHelper.screenHeight
        );
        
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
    }
}
```

---

## 2. TypeInfo - 类型信息缓存

### 类说明

| 属性 | 说明 |
|------|------|
| **职责** | 泛型类型信息缓存，避免重复反射开销 |
| **类型** | `static class` |
| **设计模式** | 泛型静态字段（每个类型一份） |

### API

```csharp
// 获取类型
Type type = TypeInfo<MyClass>.Type;

// 获取哈希码（用于字典键）
int hashCode = TypeInfo<MyClass>.HashCode;

// 获取类型名
string typeName = TypeInfo<MyClass>.TypeName;
```

### 优势

```csharp
// ❌ 低效：每次反射
Type type1 = typeof(MyClass);
Type type2 = typeof(MyClass); // 再次反射

// ✅ 高效：缓存复用
Type type1 = TypeInfo<MyClass>.Type; // 静态字段，无开销
Type type2 = TypeInfo<MyClass>.Type; // 直接访问
```

### 使用示例

#### 字典键优化

```csharp
// 使用 HashCode 作为字典键
private Dictionary<int, object> typeCache = new Dictionary<int, object>();

public void Register<T>(object obj)
{
    int hashCode = TypeInfo<T>.HashCode;
    typeCache[hashCode] = obj;
}

public T Get<T>()
{
    int hashCode = TypeInfo<T>.HashCode;
    return (T)typeCache[hashCode];
}
```

#### 类型比较

```csharp
// 快速类型比较
if (TypeInfo<A>.HashCode == TypeInfo<B>.HashCode)
{
    // A 和 B 是同一类型
}
```

---

## 3. JsonHelper - JSON 序列化

### 类说明

| 属性 | 说明 |
|------|------|
| **职责** | 基于 LitJson 的序列化封装 |
| **类型** | `static class` |
| **依赖库** | LitJson |

### API

#### 序列化

```csharp
// 对象 → JSON
string json = JsonHelper.ToJson(obj);
```

#### 反序列化

```csharp
// JSON → 对象（泛型）
MyClass obj = JsonHelper.FromJson<MyClass>(json);

// JSON → 对象（Type）
object obj = JsonHelper.FromJson(typeof(MyClass), json);

// 安全反序列化（不抛异常）
bool success = JsonHelper.TryFromJson<MyClass>(json, out var obj);
if (success)
{
    // 使用 obj
}
```

### 使用示例

#### 配置加载

```csharp
public class ConfigManager
{
    public T LoadConfig<T>(string path) where T : class
    {
        string json = File.ReadAllText(path);
        return JsonHelper.FromJson<T>(json);
    }
    
    public bool TryLoadConfig<T>(string path, out T config) where T : class
    {
        try
        {
            string json = File.ReadAllText(path);
            return JsonHelper.TryFromJson(json, out config);
        }
        catch
        {
            config = null;
            return false;
        }
    }
}
```

#### 网络数据解析

```csharp
public class ApiResponse<T>
{
    public int Code { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
}

public async ETTask<T> RequestAsync<T>(string url)
{
    string response = await HttpClient.GetAsync(url);
    
    if (JsonHelper.TryFromJson<ApiResponse<T>>(response, out var apiResp))
    {
        if (apiResp.Code == 200)
        {
            return apiResp.Data;
        }
        else
        {
            throw new Exception(apiResp.Message);
        }
    }
    
    throw new Exception("Invalid JSON response");
}
```

---

## 4. EasingFunction - 缓动函数

### 类说明

| 属性 | 说明 |
|------|------|
| **职责** | 提供 32 种缓动函数，用于动画插值 |
| **类型** | `static class` |
| **参考** | http://easings.net/ |

### 缓动类型

```csharp
public enum Ease
{
    // 二次
    EaseInQuad = 0,      // 加速
    EaseOutQuad = 1,     // 减速
    EaseInOutQuad = 2,   // 先加速后减速
    
    // 三次
    EaseInCubic = 3,
    EaseOutCubic = 4,
    EaseInOutCubic = 5,
    
    // 四次
    EaseInQuart = 6,
    EaseOutQuart = 7,
    EaseInOutQuart = 8,
    
    // 五次
    EaseInQuint = 9,
    EaseOutQuint = 10,
    EaseInOutQuint = 11,
    
    // 正弦
    EaseInSine = 12,
    EaseOutSine = 13,
    EaseInOutSine = 14,
    
    // 指数
    EaseInExpo = 15,
    EaseOutExpo = 16,
    EaseInOutExpo = 17,
    
    // 圆形
    EaseInCirc = 18,
    EaseOutCirc = 19,
    EaseInOutCirc = 20,
    
    // 线性
    Linear = 21,
    
    // 弹跳
    EaseInBounce = 23,
    EaseOutBounce = 24,
    EaseInOutBounce = 25,
    
    // 回弹
    EaseInBack = 26,
    EaseOutBack = 27,
    EaseInOutBack = 28,
    
    // 弹性
    EaseInElastic = 29,
    EaseOutElastic = 30,
    EaseInOutElastic = 31
}
```

### API

```csharp
// 获取缓动函数
Function func = EasingFunction.GetEasingFunction(Ease.EaseOutQuad);

// 调用缓动函数
float value = func.Invoke(time, startValue, changeValue, duration);
```

### 使用示例

#### UI 动画

```csharp
public async ETTask FadeIn(CanvasGroup canvasGroup, float duration = 0.3f)
{
    float startTime = Time.time;
    float startAlpha = canvasGroup.alpha;
    float changeAlpha = 1.0f - startAlpha;
    
    var easing = EasingFunction.GetEasingFunction(EasingFunction.Ease.EaseOutQuad);
    
    while (Time.time - startTime < duration)
    {
        float t = Time.time - startTime;
        canvasGroup.alpha = easing.Invoke(t, startAlpha, changeAlpha, duration);
        await TimerManager.Instance.WaitFrameAsync();
    }
    
    canvasGroup.alpha = 1.0f;
}
```

#### 位移动画

```csharp
public async ETTask MoveTo(Transform transform, Vector3 targetPos, float duration = 0.5f)
{
    float startTime = Time.time;
    Vector3 startPos = transform.position;
    
    var easing = EasingFunction.GetEasingFunction(EasingFunction.Ease.EaseInOutCubic);
    
    while (Time.time - startTime < duration)
    {
        float t = Time.time - startTime;
        float value = easing.Invoke(t, 0, 1, duration);
        transform.position = Vector3.Lerp(startPos, targetPos, value);
        await TimerManager.Instance.WaitFrameAsync();
    }
    
    transform.position = targetPos;
}
```

#### 缩放动画（配合 UIBaseContainer）

```csharp
// UIBaseContainer 已内置 ScaleTo 方法
await uiComponent.ScaleTo(
    scale: new Vector3(1.2f, 1.2f, 1.2f),
    during: 500,
    easing: EasingFunction.Ease.EaseOutBack
);
```

---

## 5. PlatformUtil - 平台检测

### 类说明

| 属性 | 说明 |
|------|------|
| **职责** | 跨平台检测，支持 WebGL 各小游戏平台 |
| **类型** | `static class` |

### API

#### 平台判断

```csharp
// 是否 iOS
bool isiOS = PlatformUtil.IsIphone();

// 是否 Android
bool isAndroid = PlatformUtil.IsAndroid();

// 是否 Windows
bool isWindows = PlatformUtil.IsWindows();

// 是否 WebGL
bool isWebGL = PlatformUtil.IsWebGL();

// 是否移动端（包含各小游戏平台）
bool isMobile = PlatformUtil.IsMobile();

// 是否模拟器
bool isSimulator = PlatformUtil.IsSimulator();
```

#### 平台字符串

```csharp
// 获取平台字符串（忽略编辑器）
string platform = PlatformUtil.GetStrPlatformIgnoreEditor();
// 返回："android" / "ios" / "webgl" / "pc"
```

### 使用示例

#### 平台特定逻辑

```csharp
public void Initialize()
{
    if (PlatformUtil.IsMobile())
    {
        // 移动端优化
        SetLowQuality();
    }
    else
    {
        // PC 端高清
        SetHighQuality();
    }
    
    // 小游戏平台特殊处理
    if (PlatformUtil.IsWebGL())
    {
        // WebGL 内存限制
        SetMemoryLimit();
    }
}
```

#### 平台统计

```csharp
public void ReportAnalytics()
{
    string platform = PlatformUtil.GetStrPlatformIgnoreEditor();
    bool isMobile = PlatformUtil.IsMobile();
    
    Analytics.Report("platform", platform);
    Analytics.Report("is_mobile", isMobile);
}
```

---

## 6. PhysicsHelper - 物理辅助

### 类说明

| 属性 | 说明 |
|------|------|
| **职责** | 高性能物理检测，对象池优化，Entity 过滤 |
| **类型** | `static class` |
| **核心优化** | 使用 NonAlloc 方法避免 GC |

### 核心 API

#### 射线检测

```csharp
// 射线检测（返回最近碰撞）
bool hit = PhysicsHelper.RaycastNonAlloc(
    origin: transform.position,
    direction: transform.forward,
    out RaycastHit hitInfo,
    maxDistance: 100f,
    layerMask: LayerMask.GetMask("Entity")
);

if (hit)
{
    Log.Info($"Hit: {hitInfo.collider.name}, Distance: {hitInfo.distance}");
}
```

#### 球形检测

```csharp
// 球形检测
bool hit = PhysicsHelper.SphereCastNonAlloc(
    origin: transform.position,
    radius: 1f,
    direction: transform.forward,
    out RaycastHit hitInfo,
    maxDistance: 50f,
    layerMask: LayerMask.GetMask("Entity")
);
```

#### Entity 检测

```csharp
// 球形范围内 Entity 检测
int count = PhysicsHelper.OverlapSphereNonAllocEntity(
    center: transform.position,
    radius: 10f,
    filter: new[] { EntityType.Enemy, EntityType.Player },
    out long[] entityIds
);

for (int i = 0; i < count; i++)
{
    long entityId = entityIds[i];
    var entity = EntityManager.Instance.Get(entityId);
    // 处理 Entity
}
```

#### HitInfo 检测（带受击点）

```csharp
// 球形检测，返回 HitInfo（包含受击点、方向等）
int count = PhysicsHelper.OverlapSphereNonAllocHitInfo(
    center: skillPosition,
    radius: skillRadius,
    filter: new[] { EntityType.Enemy },
    type: CheckHitLayerType.OnlyHitBox,
    out HitInfo[] hitInfos
);

for (int i = 0; i < count; i++)
{
    HitInfo hit = hitInfos[i];
    Log.Info($"Entity: {hit.EntityId}, Distance: {hit.Distance}");
    Log.Info($"Hit Position: {hit.HitPos}, Direction: {hit.HitDir}");
    
    // 应用伤害
    ApplyDamage(hit.EntityId, skillDamage);
}
```

### 使用示例

#### 技能范围检测

```csharp
public class SkillAOE : MonoBehaviour
{
    public float radius = 5f;
    public int damage = 100;
    
    public void Cast()
    {
        // 检测范围内敌人
        int count = PhysicsHelper.OverlapSphereNonAllocHitInfo(
            center: transform.position,
            radius: radius,
            filter: new[] { EntityType.Enemy },
            type: CheckHitLayerType.OnlyHitBox,
            out HitInfo[] hitInfos
        );
        
        for (int i = 0; i < count; i++)
        {
            HitInfo hit = hitInfos[i];
            
            // 创建伤害数字
            DamageNumber.Create(hit.HitPos, damage);
            
            // 应用伤害
            var entity = EntityManager.Instance.Get(hit.EntityId);
            entity.GetComponent<NumericComponent>().TakeDamage(damage);
        }
        
        Log.Info($"Hit {count} enemies");
    }
}
```

#### 视线检测

```csharp
public class VisionSystem : MonoBehaviour
{
    public float viewDistance = 20f;
    public float viewAngle = 90f;
    
    public List<long> GetVisibleEnemies()
    {
        // 检测范围内所有 Entity
        int count = PhysicsHelper.OverlapSphereNonAllocEntity(
            center: transform.position,
            radius: viewDistance,
            filter: new[] { EntityType.Enemy },
            out long[] entityIds
        );
        
        var visible = new List<long>();
        
        for (int i = 0; i < count; i++)
        {
            long entityId = entityIds[i];
            var enemy = EntityManager.Instance.Get(entityId);
            
            // 检查是否在视野角度内
            Vector3 dirToEnemy = enemy.transform.position - transform.position;
            float angle = Vector3.Angle(transform.forward, dirToEnemy);
            
            if (angle < viewAngle / 2)
            {
                // 检查是否有遮挡
                if (!HasObstacle(transform.position, enemy.transform.position))
                {
                    visible.Add(entityId);
                }
            }
        }
        
        return visible;
    }
    
    private bool HasObstacle(Vector3 start, Vector3 end)
    {
        return PhysicsHelper.LinecastScene(start, end, out _);
    }
}
```

---

## 7. Log - 日志系统

### 类说明

| 属性 | 说明 |
|------|------|
| **职责** | 统一日志接口，支持日志级别控制 |
| **类型** | `static class` |
| **日志级别** | Trace > Debug > Info > Warning > Error |

### 日志级别

```csharp
// Define.LogLevel 控制输出级别
// 1 = Trace, 2 = Debug, 3 = Info, 4 = Warning, 5 = Error (只输出错误)

Define.LogLevel = 3; // 只输出 Info 及以上
```

### API

#### 基础日志

```csharp
// Trace（最详细，包含堆栈）
Log.Trace("Detailed trace message");

// Debug（调试信息）
Log.Debug("Debug message");

// Info（普通信息）
Log.Info("Info message");
Log.Info(objectValue); // 自动 ToString

// Warning（警告）
Log.Warning("Warning message");

// Error（错误，包含堆栈）
Log.Error("Error message");
Log.Error(exception); // 记录异常
```

#### 格式化日志

```csharp
// 支持 string.Format 格式
Log.Debug("Player {0} HP: {1}", playerName, hp);
Log.Info("Position: {0}, {1}, {2}", x, y, z);
Log.Warning("Cooldown not ready: {0}, remaining: {1}ms", skillName, remaining);
Log.Error("Failed to load config: {0}, error: {1}", path, errorMsg);
```

### 使用示例

#### 自定义日志实现

```csharp
// 实现 ILog 接口
public class UnityLogger : ILog
{
    public void Debug(string msg) => UnityEngine.Debug.Log(msg);
    public void Info(string msg) => UnityEngine.Debug.Log(msg);
    public void Warning(string msg) => UnityEngine.Debug.LogWarning(msg);
    public void Error(string msg) => UnityEngine.Debug.LogError(msg);
    public void Trace(string msg) => UnityEngine.Debug.Log(msg);
}

// 初始化
Log.ILog = new UnityLogger();
```

#### 条件日志

```csharp
public class BattleSystem
{
    private const bool DEBUG_DAMAGE = true;
    
    public void ApplyDamage(long targetId, int damage)
    {
        if (DEBUG_DAMAGE)
        {
            Log.Debug($"ApplyDamage: target={targetId}, damage={damage}");
        }
        
        // 伤害逻辑...
    }
}
```

#### 异常处理

```csharp
public async ETTask LoadDataAsync()
{
    try
    {
        Log.Info("Loading data...");
        var data = await DataService.LoadAsync();
        Log.Info($"Loaded {data.Count} records");
    }
    catch (Exception ex)
    {
        Log.Error($"Failed to load data: {ex.Message}");
        Log.Error(ex); // 记录完整堆栈
    }
}
```

---

## ⚠️ 注意事项

| 问题 | 说明 | 解决方案 |
|------|------|----------|
| **安全区域坐标系** | Unity 和屏幕 Y 轴方向相反 | SystemInfoHelper 已处理转换 |
| **TypeInfo 泛型** | 每个泛型组合生成新静态字段 | 避免过多泛型组合 |
| **JsonHelper 异常** | 反序列化失败会抛异常 | 使用 TryFromJson 安全版本 |
| **EasingFunction 性能** | 委托调用有轻微开销 | 缓存 Function 委托 |
| **PhysicsHelper 对象池** | 内部数组是共享的 | 不要持有返回的数组引用 |
| **Log 性能** | Trace/Debug 包含堆栈，性能开销大 | 生产环境提高 LogLevel |

---

## 相关文档

- [Timer_System.cs.md](./Timer/Timer_System.cs.md) - Timer 系统
- [ObjectPool.cs.md](../Core/Object/ObjectPool.cs.md) - 对象池
- [UIBaseContainer.cs.md](../../Code/Module/UI/UIBaseContainer.cs.md) - UI 容器基类

---

*文档由 OpenClaw AI 助手自动生成 | 基于静态代码分析*
