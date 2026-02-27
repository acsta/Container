# GameConst.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | GameConst.cs |
| **路径** | Assets/Scripts/Code/Module/Const/GameConst.cs |
| **所属模块** | 框架层 → Code/Module/Const |
| **文件职责** | 游戏全局常量定义 |

---

## 类说明

### GameConst

| 属性 | 说明 |
|------|------|
| **职责** | 定义游戏全局常量（路径、颜色、ID、配置等） |
| **类型** | static class |

---

## 常量分类

### 预制体路径

| 常量 | 值 | 说明 |
|------|-----|------|
| `SmokePrefab` | "Unit/Box/Prefabs/BoxBoom.prefab" | 烟雾预制体 |
| `SmokePrefab2` | "Unit/Box/Prefabs/VFX_Comet_Impact.prefab" | 烟雾预制体 2 |
| `LightBall_UI` | "UIGame/UILobby/Prefabs/LightBall_UI.prefab" | 光球 UI 预制体 |
| `LightBall_Scene` | "UIGame/UILobby/Prefabs/LightBall.prefab" | 光球场景预制体 |
| `bomb` | "UIGame/UILobby/Prefabs/bomb.prefab" | 炸弹预制体 |
| `TaskPrefab` | "UIGame/UILobby/Prefabs/Task.prefab" | 任务预制体 |

### 图片/材质路径

| 常量 | 值 | 说明 |
|------|-----|------|
| `DefaultImage` | "UI/UICommon/DiscreteImages/PictureUnknow.png" | 默认图片 |
| `TaskMat` | "UIGame/UIAuction/Materials/M_TexturedPlayType.mat" | 任务材质 |
| `PlayTypeMat` | "UIGame/UIAuction/Materials/M_TexturedTask.mat" | 玩法材质 |

### 数值常量

| 常量 | 值 | 说明 |
|------|-----|------|
| `MaxBoxCount` | 6 | 最大箱子数量 |
| `HomeCameraMinX` | -8f | 家园相机最小 X |
| `HomeCameraMaxX` | 20 | 家园相机最大 X |
| `MAX_RUNNING_TASK_COUNT` | 3 | 最大运行任务数 |

### 颜色常量（十六进制）

| 常量 | 值 | 说明 |
|------|-----|------|
| `WIN_COLOR` | "#6B800F" | 胜利颜色（绿色） |
| `LOSS_COLOR` | "#FF0000" | 失败颜色（红色） |
| `DEFAUT_COLOR` | "#3C3C3C" | 默认颜色（深灰） |
| `WHITE_COLOR` | "#FFFFFF" | 白色 |
| `RED_COLOR` | "#EE4751" | 红色 |
| `GRAY_COLOR` | "#B8B8B8" | 灰色 |
| `GREEN_COLOR` | "#83C740" | 绿色 |
| `COMMON_TEXT_COLOR` | "#AD945E" | 通用文本颜色（金色） |

### 实体 ID

| 常量 | 值 | 说明 |
|------|-----|------|
| `TurnTableUnitId` | 3 | 大转盘 ID |
| `CharacterUnitId` | 14 | 角色 ID |
| `UFOUnitId` | 12 | UFO ID |

### 物品 ID

| 常量 | 值 | 说明 |
|------|-----|------|
| `MoneyItemId` | 1 | 金币物品 ID |
| `DiceItemId` | 1001 | 骰子物品 ID |

### 静态配置（运行时设置）

| 字段 | 类型 | 说明 |
|------|------|------|
| `ProfitUnitTime` | `int` | 餐厅利润单位时间（ms） |
| `ProfitUnitShowTime` | `int` | UI 显示餐厅利润单位时间（ms） |
| `TaskItemColor` | `Color` | 任务物品颜色 |
| `PlayableItemColor` | `Color` | 玩法物品颜色 |
| `ItemPriceColor` | `Color` | 物品价值颜色 |
| `TurnTableAdCount` | `int[]` | 大转盘每日出现次数 |
| `PlayableAdCount` | `int[]` | 玩法物品看广告次数 |
| `PlayableMaxAdCount` | `int` | 玩法物品每日最多看广告次数 |
| `OpenStageInterval` | `int[]` | Timeline 触发间隔 |
| `TurnTablePercent` | `int[]` | 大转盘随出现次数的概率 |

---

## 使用示例

### 示例 1: 使用路径常量

```csharp
// 加载默认图片
Sprite defaultSprite = await ImageLoaderManager.Instance.LoadSpriteAsync(
    GameConst.DefaultImage
);

// 实例化预制体
GameObject taskObj = await ResourcesManager.Instance.LoadAsync<GameObject>(
    GameConst.TaskPrefab
);
```

### 示例 2: 使用颜色常量

```csharp
// 设置胜利文本颜色
winText.GetComponent<Text>().color = ColorUtility.TryParseHtmlString(
    GameConst.WIN_COLOR, 
    out var color
) ? color : Color.white;

// 设置失败文本颜色
lossText.GetComponent<Text>().color = ColorUtility.TryParseHtmlString(
    GameConst.LOSS_COLOR, 
    out color
) ? color : Color.red;
```

### 示例 3: 检查数量限制

```csharp
// 检查箱子数量
if (boxCount >= GameConst.MaxBoxCount)
{
    Log.Warning("已达到最大箱子数量");
    return;
}

// 检查任务数量
if (runningTasks.Count >= GameConst.MAX_RUNNING_TASK_COUNT)
{
    Log.Warning("已达到最大运行任务数");
    return;
}
```

### 示例 4: 使用实体 ID

```csharp
// 检查是否是大转盘实体
if (entity.Id == GameConst.TurnTableUnitId)
{
    // 大转盘逻辑
}

// 检查是否是 UFO 实体
if (entity.Id == GameConst.UFOUnitId)
{
    // UFO 逻辑
}
```

---

## 相关文档

- [CacheKeys.cs.md](./CacheKeys.cs.md) - 缓存键常量
- [ItemType.cs.md](./ItemType.cs.md) - 物品类型
- [BoxType.cs.md](./BoxType.cs.md) - 箱子类型

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
