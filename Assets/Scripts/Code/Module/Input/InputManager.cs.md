# InputManager.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | InputManager.cs |
| **路径** | Assets/Scripts/Code/Module/Input/InputManager.cs |
| **所属模块** | 框架层 → Code/Module/Input |
| **文件职责** | 输入管理器，统一处理键盘、鼠标、触摸输入 |

---

## 类/结构体说明

### InputManager

| 属性 | 说明 |
|------|------|
| **职责** | 统一管理键盘、鼠标、触摸输入，支持移动端和 PC 端 |
| **泛型参数** | 无 |
| **继承关系** | 无 |
| **实现的接口** | `IManager`, `IUpdate` |

```csharp
public class InputManager : IManager, IUpdate
{
    // 输入管理器
}
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `Instance` | `InputManager` | `public static` | 单例实例 |
| `IsPause` | `bool` | `public` | 是否暂停输入 |
| `MouseScrollWheel` | `float` | `public` | 鼠标滚轮值 |
| `MouseAxisX` | `float` | `public` | 鼠标 X 轴增量 |
| `MouseAxisY` | `float` | `public` | 鼠标 Y 轴增量 |
| `keySetMap` | `KeyCode[]` | `private` | 按键绑定映射 |
| `keyStatus` | `int[]` | `private` | 按键状态数组 |
| `touchInfos` | `List<TouchInfo>` | `private` | 触摸信息列表 |
| `Default` | `KeyCode[]` | `public static` | 默认按键配置 |

---

## 常量定义

| 常量 | 值 | 说明 |
|------|-----|------|
| `KeyDown` | 1 | 按键按下标志 |
| `KeyUp` | 2 | 按键抬起标志 |
| `Key` | 4 | 按键按住标志 |

---

## 方法说明（按重要程度排序）

### Init()

**签名**:
```csharp
public void Init()
```

**职责**: 初始化输入管理器

**核心逻辑**:
```
1. 设置 Instance = this
2. 清空输入事件绑定列表
3. 复制默认按键配置到 keySetMap
```

**默认按键**:
```csharp
KeyCode.W,      // 前
KeyCode.S,      // 后
KeyCode.A,      // 左
KeyCode.D,      // 右
KeyCode.Space,  // 空格
KeyCode.Mouse0, // 鼠标左键
KeyCode.F,      // F 键
KeyCode.LeftAlt,// 左 Alt
KeyCode.Escape  // Esc
```

**调用者**: `ManagerProvider.Init()`

---

### Update()

**签名**:
```csharp
public void Update()
```

**职责**: 每帧更新输入状态

**核心逻辑**:
```
1. 添加新触摸 AddTouch()
2. 清空按键状态
3. 如果未暂停：
   - 移动端：处理触摸输入
   - PC 端：处理键盘输入
   - 遍历所有按键，检查状态变化
   - 如果状态变化，广播 OnKeyInput 消息
   - 更新鼠标数据 UpdateMouse()
4. 移除结束触摸 RemoveTouch()
```

**按键状态广播**:
```csharp
if (keyStatus[i] != val)
{
    keyStatus[i] = val;
    Messager.Instance.Broadcast(0, MessageId.OnKeyInput, i, val);
}
```

**调用者**: `ManagerProvider.Update()`

---

### GetKey(int keyCode, bool ignoreUI)

**签名**:
```csharp
public bool GetKey(int keyCode, bool ignoreUI = false)
```

**职责**: 获取按键是否按住

**参数**:
- `keyCode`: 游戏按键 ID（GameKeyCode）
- `ignoreUI`: 是否忽略 UI 上的输入

**返回**: true=按键按住，false=未按住

**核心逻辑**:
```
1. 检查 keyStatus 是否包含 Key 标志
2. 如果 ignoreUI=true 且未绑定事件：
   - 移动端：检查是否有触摸
   - 检查触摸位置是否在 UI 上
3. 返回结果
```

**调用者**: 游戏逻辑代码

**使用示例**:
```csharp
// 检查是否按住 W 键（前进）
if (InputManager.Instance.GetKey((int)GameKeyCode.W))
{
    MoveForward();
}
```

---

### GetKeyDown(int keyCode, bool ignoreUI)

**签名**:
```csharp
public bool GetKeyDown(int keyCode, bool ignoreUI = false)
```

**职责**: 获取按键是否按下（帧检测）

**返回**: true=按键刚按下，false=未按下

**与 GetKey 的区别**:
- `GetKey`: 按住期间每帧都返回 true
- `GetKeyDown`: 只在按下那一帧返回 true

**使用示例**:
```csharp
// 检查是否按下空格键（跳跃）
if (InputManager.Instance.GetKeyDown((int)GameKeyCode.Space))
{
    Jump();
}
```

---

### GetKeyUp(int keyCode, bool ignoreUI)

**签名**:
```csharp
public bool GetKeyUp(int keyCode, bool ignoreUI = false)
```

**职责**: 获取按键是否抬起（帧检测）

**返回**: true=按键刚抬起，false=未抬起

**使用示例**:
```csharp
// 检查是否抬起鼠标左键
if (InputManager.Instance.GetKeyUp((int)GameKeyCode.Mouse0))
{
    StopAttack();
}
```

---

### GetLastTouchPos()

**签名**:
```csharp
public Vector2 GetLastTouchPos()
```

**职责**: 获取最后触摸位置

**返回**: 触摸位置（屏幕坐标）

**核心逻辑**:
```
1. 移动端：返回 mousePosition
2. PC 端：返回 Input.mousePosition
```

**调用者**: UI 输入检测

---

### IsPointerOverUI(Vector2 mousePosition, params RectTransform[] ignore)

**签名**:
```csharp
public bool IsPointerOverUI(Vector2 mousePosition, params RectTransform[] ignore)
```

**职责**: 检查触摸位置是否在 UI 上

**参数**:
- `mousePosition`: 触摸位置
- `ignore`: 忽略的 UI 元素（可选）

**返回**: true=在 UI 上，false=不在 UI 上

**核心逻辑**:
```
1. 创建 PointerEventData
2. 设置位置
3. 射线检测 EventSystem.RaycastAll
4. 如果有命中：
   - 检查是否在忽略列表中
   - 返回结果
```

**调用者**: GetKey, GetKeyDown, GetKeyUp（ignoreUI 模式）

---

### SetInputKeyMap(int key, KeyCode keyCode)

**签名**:
```csharp
public void SetInputKeyMap(int key, KeyCode keyCode)
```

**职责**: 设置按键映射（自定义键位）

**参数**:
- `key`: 游戏按键 ID
- `keyCode`: Unity KeyCode

**使用示例**:
```csharp
// 将 W 键改为 Up 箭头
InputManager.Instance.SetInputKeyMap((int)GameKeyCode.W, KeyCode.UpArrow);
```

---

### GetGyroAttitude()

**签名**:
```csharp
public Quaternion GetGyroAttitude()
```

**职责**: 获取陀螺仪姿态

**返回**: 陀螺仪四元数

**调用者**: 体感控制代码

---

## 阅读指引

### 建议的阅读顺序

1. **理解管理器作用** - InputManager 管理什么
2. **看 GetKey/GetKeyDown/GetKeyUp** - 理解输入检测
3. **看 Update** - 理解输入更新流程
4. **了解触摸处理** - 理解移动端输入

### 最值得学习的技术点

1. **统一输入**: 键盘、鼠标、触摸统一接口
2. **状态广播**: Messager 广播按键状态变化
3. **UI 检测**: 射线检测判断是否在 UI 上
4. **触摸模拟**: 触摸模拟鼠标滚轮和轴向
5. **对象池**: TouchInfo 使用对象池管理

---

## 使用示例

### 示例 1: 基础输入检测

```csharp
void Update()
{
    // 检查前进
    if (InputManager.Instance.GetKey((int)GameKeyCode.W))
    {
        MoveForward();
    }
    
    // 检查跳跃（只检测一次）
    if (InputManager.Instance.GetKeyDown((int)GameKeyCode.Space))
    {
        Jump();
    }
    
    // 检查攻击
    if (InputManager.Instance.GetKey((int)GameKeyCode.Mouse0))
    {
        Attack();
    }
}
```

### 示例 2: 忽略 UI 输入

```csharp
// 只在非 UI 区域响应空格键
if (InputManager.Instance.GetKeyDown((int)GameKeyCode.Space, ignoreUI: true))
{
    // 玩家不在 UI 上时才跳跃
    Jump();
}
```

### 示例 3: 获取触摸位置

```csharp
Vector2 touchPos = InputManager.Instance.GetLastTouchPos();

if (InputManager.Instance.IsPointerOverUI(touchPos))
{
    Log.Info("点击在 UI 上");
}
else
{
    Log.Info("点击在游戏场景上");
    MoveToPosition(touchPos);
}
```

### 示例 4: 自定义键位

```csharp
// 设置自定义键位
InputManager.Instance.SetInputKeyMap((int)GameKeyCode.W, KeyCode.UpArrow);
InputManager.Instance.SetInputKeyMap((int)GameKeyCode.S, KeyCode.DownArrow);
InputManager.Instance.SetInputKeyMap((int)GameKeyCode.A, KeyCode.LeftArrow);
InputManager.Instance.SetInputKeyMap((int)GameKeyCode.D, KeyCode.RightArrow);
```

### 示例 5: 获取鼠标数据

```csharp
// 获取鼠标滚轮
float scroll = InputManager.Instance.MouseScrollWheel;
if (scroll > 0)
{
    ZoomIn();
}
else if (scroll < 0)
{
    ZoomOut();
}

// 获取鼠标移动
float mouseX = InputManager.Instance.MouseAxisX;
float mouseY = InputManager.Instance.MouseAxisY;
RotateCamera(mouseX, mouseY);
```

---

## 移动端触摸模拟

### 单指触摸

| 触摸行为 | 模拟输入 |
|---------|---------|
| 触摸开始 | Mouse0 Down |
| 触摸按住 | Mouse0 Hold |
| 触摸结束 | Mouse0 Up |
| 触摸移动 | Mouse X/Y |
| 垂直快速滑动 | Mouse ScrollWheel |

### 双指触摸

| 触摸行为 | 模拟输入 |
|---------|---------|
| 双指捏合 | Mouse ScrollWheel |
| 双指张开 | Mouse ScrollWheel |

---

## 相关文档

- [GameKeyCode.cs.md](./GameKeyCode.cs.md) - 游戏按键码
- [TouchInfo.cs.md](./TouchInfo.cs.md) - 触摸信息
- [Messager.cs.md](../Messager/Messager.cs.md) - 消息管理器

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
