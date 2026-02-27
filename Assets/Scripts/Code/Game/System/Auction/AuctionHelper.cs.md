# AuctionHelper.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | AuctionHelper.cs |
| **路径** | Assets/Scripts/Code/Game/System/Auction/AuctionHelper.cs |
| **所属模块** | 玩法层 → Code/Game/System/Auction |
| **文件职责** | 拍卖系统辅助工具类，提供抬价倍率、随机数量、装箱算法等工具方法 |

---

## 类/结构体说明

### AuctionHelper (静态类)

| 属性 | 说明 |
|------|------|
| **职责** | 提供拍卖系统相关的静态工具方法 |
| **泛型参数** | 无 |
| **继承关系** | 无继承 |
| **实现的接口** | 无 |

**设计模式**: 工具类模式（静态方法集合）

```csharp
// 静态类，无需实例化
public static class AuctionHelper
{
    // 所有方法都是静态的
    public static float GetRaiseMul(int successCount) { }
    public static int RandomSpecialCount(int[] counts, int[] weights) { }
    public static bool PackBoxes(...) { }
}
```

---

## 字段与属性

### raiseMul (静态字段)

**签名**:
```csharp
private static float[] raiseMul;
```

**职责**: 抬价收益倍率数组（从全局配置读取）

**初始化**:
```csharp
static AuctionHelper()
{
    if (!GlobalConfigCategory.Instance.TryGetArray("RaiseCoefficient", out raiseMul))
    {
        raiseMul = new float[1] { 1 };  // 默认倍率 1
    }
}
```

**配置来源**: `GlobalConfigCategory` 中的 `RaiseCoefficient` 数组

---

### Rotations (静态只读数组)

**签名**:
```csharp
public static readonly Quaternion[] Rotations = new Quaternion[]
{
    Quaternion.identity,                    // 0° (无旋转)
    Quaternion.Euler(new Vector3(0, 0, 90)),   // Z 轴 90°
    Quaternion.Euler(new Vector3(0, 90, 0)),   // Y 轴 90°
    Quaternion.Euler(new Vector3(90, 0, 0)),   // X 轴 90°
};
```

**职责**: 预定义的 4 种旋转角度，用于装箱算法

---

### Flag (静态只读数组)

**签名**:
```csharp
public static readonly Vector3[] Flag = new Vector3[]
{
    new Vector3(0.5f, 0, -0.5f),
    new Vector3(-1, 0.5f, -0.5f),
    new Vector3(0.5f, 0, 0.5f),
    new Vector3(0.5f, -0.5f, -1),
};
```

**职责**: 旗帜位置偏移量数组

---

### Space (结构体)

**签名**:
```csharp
public struct Space
{
    public Vector3 Position;  // 空间左下角坐标
    public Vector3 Size;      // 空间尺寸 (x=宽，y=高，z=深)

    public Space(Vector3 position, Vector3 size)
    {
        Position = position;
        Size = size;
    }
}
```

**职责**: 定义 3D 空间的数据结构，用于装箱算法

---

## 方法说明（按重要程度排序）

### GetRaiseMul(int successCount)

**签名**:
```csharp
public static float GetRaiseMul(int successCount)
```

**职责**: 获取抬价收益倍率

**参数**:
- `successCount`: 玩家成功抬价次数

**核心逻辑**:
```
1. successCount -= 1（从 0 开始索引）
2. 如果 < 0，返回 1（默认倍率）
3. 如果 >= 数组长度，返回最后一个元素
4. 否则返回对应索引的倍率
```

**调用者**: 结算逻辑

**使用示例**:
```csharp
// 玩家成功抬价 3 次
float mul = AuctionHelper.GetRaiseMul(3);
// 假设配置为 [1, 1.2, 1.5, 2.0]
// 返回 1.5（索引 2）

// 计算最终收益
BigNumber finalPrice = basePrice * mul;
```

---

### RandomSpecialCount(int[] counts, int[] weights)

**签名**:
```csharp
public static int RandomSpecialCount(int[] counts, int[] weights)
```

**职责**: 根据权重随机特殊集装箱数量

**参数**:
- `counts`: 数量数组（如 [1, 2, 3, 4, 5]）
- `weights`: 权重数组（如 [10, 30, 40, 15, 5]）

**核心逻辑**:
```
1. 检查参数有效性
2. 如果只有 1 个元素，直接返回
3. 计算总权重 total = sum(weights)
4. 随机生成 0 到 total*10 的数，然后取模
5. 遍历权重数组，累减权重
6. 当 val <= 0 时，返回对应的数量
```

**调用者**: `AuctionManager.CreateContainer()`

**使用示例**:
```csharp
// 配置：特殊集装箱数量及权重
int[] counts = new int[] { 1, 2, 3, 4, 5 };
int[] weights = new int[] { 10, 30, 40, 15, 5 };

// 随机生成特殊集装箱数量
int specialCount = AuctionHelper.RandomSpecialCount(counts, weights);
// 结果：
// 1 个：10% 概率
// 2 个：30% 概率
// 3 个：40% 概率
// 4 个：15% 概率
// 5 个：5% 概率
```

**权重计算示例**:
```
weights = [10, 30, 40, 15, 5]
total = 100

随机数范围：0-999 (total * 10)
假设随机到 450:
450 % 100 = 50

遍历:
i=0: 50 - 10 = 40 > 0, 继续
i=1: 40 - 30 = 10 > 0, 继续
i=2: 10 - 40 = -30 <= 0, 返回 counts[2] = 3
```

---

### PackBoxes(Vector3 containerSize, List<UnitConfig> boxSizes, ...)

**签名**:
```csharp
public static bool PackBoxes(
    Vector3 containerSize, 
    List<UnitConfig> boxSizes, 
    out int[] rotations, 
    out Vector3[] positions
)
```

**职责**: 3D 装箱算法，将箱子装入集装箱

**参数**:
- `containerSize`: 集装箱尺寸
- `boxSizes`: 箱子尺寸列表
- `rotations`: 输出参数，每个箱子的旋转索引
- `positions`: 输出参数，每个箱子的位置

**返回**: `bool` - 是否成功装入所有箱子

**核心逻辑**:
```
1. 按体积降序排序箱子（大箱子优先）
2. 初始化剩余空间列表（从容器左下角开始）
3. 遍历每个箱子：
   a. 尝试每个剩余空间
   b. 尝试每种旋转方向
   c. 检查是否能放入
   d. 如果放入：
      - 记录位置和旋转
      - 分割剩余空间为 3 个新空间
      - 移除原空间，添加新空间
   e. 如果无法放入任何空间，返回 false
4. 所有箱子都放入，返回 true
```

**调用者**: 集装箱物品摆放逻辑

**使用示例**:
```csharp
// 集装箱尺寸
Vector3 containerSize = new Vector3(10, 5, 5);

// 箱子列表
List<UnitConfig> boxes = new List<UnitConfig>
{
    new UnitConfig { Size = new float[] { 3, 2, 2 } },
    new UnitConfig { Size = new float[] { 2, 2, 2 } },
    new UnitConfig { Size = new float[] { 4, 3, 2 } }
};

// 执行装箱
bool success = AuctionHelper.PackBoxes(
    containerSize, 
    boxes, 
    out int[] rotations, 
    out Vector3[] positions
);

if (success)
{
    // 所有箱子都装下了
    for (int i = 0; i < boxes.Count; i++)
    {
        Vector3 pos = positions[i];
        Quaternion rot = AuctionHelper.Rotations[rotations[i]];
        // 放置箱子...
    }
}
else
{
    // 装不下所有箱子
    Log.Error("装箱失败");
}
```

**空间分割示意图**:
```
放置箱子后，剩余空间分割为 3 部分：

原空间: [space]
放置箱子: [box]

分割:
- 右侧空间：newPos1 = space.Position + (boxSize.x, 0, 0)
           newSize1 = (space.x - box.x, space.y, box.z)
           
- 上方空间：newPos2 = space.Position + (0, boxSize.y, 0)
           newSize2 = (box.x, space.y - box.y, box.z)
           
- 后方空间：newPos3 = space.Position + (0, 0, boxSize.z)
            newSize3 = (space.x, space.y, space.z - box.z)
```

---

### GetMaxCharacter()

**签名**:
```csharp
public static int GetMaxCharacter()
```

**职责**: 根据设备性能获取最大角色数量

**返回**: 最大角色数

**配置**:
| 性能等级 | 最大角色数 |
|---------|-----------|
| High    | 99        |
| Mid     | 7         |
| Low     | 5         |

**调用者**: `AuctionManager.Awake()`

**使用示例**:
```csharp
// 根据设备性能限制竞拍者数量
int max = AuctionHelper.GetMaxCharacter();
int len = Mathf.Min(LevelConfig.AIIds.Length, target.Count);
len = Mathf.Min(max, len);  // 应用性能限制
```

---

### PlayFx(string path, Vector3 pos, int during)

**签名**:
```csharp
public static async ETTask PlayFx(string path, Vector3 pos, int during = 2000)
```

**职责**: 播放特效并自动回收

**参数**:
- `path`: 特效预制体路径
- `pos`: 播放位置
- `during`: 持续时间（毫秒，默认 2000）

**核心逻辑**:
```
1. 从对象池获取特效预制体
2. 设置位置
3. 等待指定时间
4. 回收到对象池
```

**调用者**: 需要播放特效的地方

**使用示例**:
```csharp
// 播放开箱特效
await AuctionHelper.PlayFx("FX/OpenBox", boxPosition, 2000);

// 播放庆祝特效
await AuctionHelper.PlayFx("FX/Win", playerPosition, 3000);
```

---

### ShowPlayView(ItemConfig cfg)

**签名**:
```csharp
public static void ShowPlayView(ItemConfig cfg)
```

**职责**: 打开小玩法界面

**参数**:
- `cfg`: 物品配置

**核心逻辑**:
```
1. 根据物品类型获取 UI 名称
2. 打开对应的小玩法 UI
3. 从解锁列表中移除该物品
```

**调用者**: 开箱后检测到小玩法物品时

**使用示例**:
```csharp
// 检测到小玩法物品
if (item.HasMiniPlay)
{
    AuctionHelper.ShowPlayView(item.Config);
}
```

---

## 阅读指引

### 建议的阅读顺序

1. **理解工具类作用** - 为什么需要 AuctionHelper
2. **看 GetRaiseMul** - 理解抬价倍率计算
3. **看 RandomSpecialCount** - 理解权重随机算法
4. **看 PackBoxes** - 理解 3D 装箱算法
5. **了解辅助方法** - GetMaxCharacter/PlayFx/ShowPlayView

### 最值得学习的技术点

1. **权重随机**: 根据权重数组随机数量
2. **3D 装箱算法**: 大箱子优先 + 空间分割
3. **静态工具类**: 无需实例化的工具方法集合
4. **async/await**: 异步播放特效并自动回收

---

## 装箱算法详解

### 算法原理

**贪心策略**: 大箱子优先放置

```
1. 按体积降序排序
   [大箱子] → [中箱子] → [小箱子]

2. 对于每个箱子：
   a. 遍历所有剩余空间
   b. 尝试所有旋转方向
   c. 找到第一个能放入的空间
   d. 放置并分割空间

3. 空间分割：
   放置箱子后，原空间分割为 3 个新空间：
   - 右侧空间
   - 上方空间
   - 后方空间
```

### 可视化示例

```
集装箱：10x5x5

步骤 1: 放置大箱子 4x3x2
┌──────────────┐
│████████┌─────┐
│████████│     │
│████████│  1  │
├────────┤     │
│   2    │     │
│        │     │
└────────┴─────┘

剩余空间：
1. 右侧：6x5x2
2. 上方：4x2x2
3. 后方：10x5x3

步骤 2: 放置中箱子 3x2x2 到空间 1
...
```

---

## 相关文档

- [AuctionManager.cs.md](./AuctionManager.cs.md) - 拍卖管理器（使用者）
- [ContainerConfig.cs.md](../../Module/Config/ContainerConfig.cs.md) - 集装箱配置
- [GameObjectPoolManager.cs.md](../../Module/Resource/GameObjectPoolManager.cs.md) - 对象池管理

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
