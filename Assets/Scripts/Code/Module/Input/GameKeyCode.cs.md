# GameKeyCode.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | GameKeyCode.cs |
| **路径** | Assets/Scripts/Code/Module/Input/GameKeyCode.cs |
| **所属模块** | 框架层 → Code/Module/Input |
| **文件职责** | 游戏按键码枚举定义 |

---

## 枚举说明

### GameKeyCode

| 属性 | 说明 |
|------|------|
| **职责** | 定义游戏逻辑按键码 |
| **类型** | enum（int） |

---

## 按键码（按 InputManager.Default）

| 值 | 按键 | 默认键位 | 说明 |
|------|------|---------|------|
| 0 | `W` | KeyCode.W | 前 |
| 1 | `S` | KeyCode.S | 后 |
| 2 | `A` | KeyCode.A | 左 |
| 3 | `D` | KeyCode.D | 右 |
| 4 | `Space` | KeyCode.Space | 空格 |
| 5 | `Mouse0` | KeyCode.Mouse0 | 鼠标左键 |
| 6 | `F` | KeyCode.F | F 键 |
| 7 | `LeftAlt` | KeyCode.LeftAlt | 左 Alt |
| 8 | `Escape` | KeyCode.Escape | Esc |
| 9 | `Max` | - | 最大按键码 |

---

## 使用示例

### 示例 1: 检查按键

```csharp
// 检查是否按住 W 键（前进）
if (InputManager.Instance.GetKey((int)GameKeyCode.W))
{
    MoveForward();
}

// 检查是否按下空格键（跳跃）
if (InputManager.Instance.GetKeyDown((int)GameKeyCode.Space))
{
    Jump();
}
```

### 示例 2: 自定义键位

```csharp
// 将 W 键改为 Up 箭头
InputManager.Instance.SetInputKeyMap(
    (int)GameKeyCode.W, 
    KeyCode.UpArrow
);

// 将 S 键改为 Down 箭头
InputManager.Instance.SetInputKeyMap(
    (int)GameKeyCode.S, 
    KeyCode.DownArrow
);
```

### 示例 3: 移动端适配

```csharp
// 移动端使用虚拟摇杆
if (PlatformUtil.IsMobile())
{
    // 虚拟摇杆控制移动
    Vector2 moveInput = virtualJoystick.GetInput();
    
    if (moveInput.y > 0.5f)
    {
        // 相当于按 W 键
        InputManager.Instance.GetKey((int)GameKeyCode.W);
    }
}
```

---

## 相关文档

- [InputManager.cs.md](./InputManager.cs.md) - 输入管理器
- [TouchInfo.cs.md](./TouchInfo.cs.md) - 触摸信息

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
