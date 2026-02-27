# TouchInfo.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | TouchInfo.cs |
| **路径** | Assets/Scripts/Code/Module/Input/TouchInfo.cs |
| **所属模块** | 框架层 → Code/Module/Input |
| **文件职责** | 触摸信息数据结构 |

---

## 类说明

### TouchInfo

| 属性 | 说明 |
|------|------|
| **职责** | 存储单次触摸的详细信息 |
| **类型** | class |
| **实现的接口** | `IDisposable` |

---

## 字段说明

| 字段 | 类型 | 说明 |
|------|------|------|
| `Index` | `int` | 触摸索引 |
| `IsScroll` | `bool` | 是否是滚轮滑动 |
| `IsStartOverUI` | `bool` | 是否开始在 UI 上 |
| `Touch` | `object` | 原始触摸对象 |

---

## 方法说明

### Dispose()

**签名**:
```csharp
public void Dispose()
```

**职责**: 释放触摸信息（回收到对象池）

---

## 使用示例

### 示例 1: 创建触摸信息

```csharp
// 创建触摸信息
TouchInfo info = ObjectPool.Instance.Fetch<TouchInfo>();
info.Index = touchIndex;
info.IsScroll = false;
info.IsStartOverUI = IsPointerOverUI(touch.position);
info.Touch = touch;

// 添加到触摸列表
touchInfos.Add(info);
```

### 示例 2: 检查是否是 UI 触摸

```csharp
// 检查触摸是否开始在 UI 上
if (touchInfo.IsStartOverUI)
{
    // UI 触摸，不处理游戏逻辑
    return;
}

// 非 UI 触摸，处理游戏逻辑
HandleGameTouch(touchInfo);
```

### 示例 3: 检查是否是滚轮

```csharp
// 检查是否是滚轮滑动
if (touchInfo.IsScroll)
{
    // 处理滚轮输入
    float scroll = touch.Touch.deltaPosition.y / 100;
    HandleScroll(scroll);
}
```

---

## 触摸生命周期

```
触摸开始 (TouchPhase.Began)
    ↓
创建 TouchInfo
    ↓
添加到 touchInfos
    ↓
触摸移动 (TouchPhase.Moved)
    ↓
更新 TouchInfo
    ↓
触摸结束 (TouchPhase.Ended)
    ↓
Dispose() 回收到对象池
```

---

## 相关文档

- [InputManager.cs.md](./InputManager.cs.md) - 输入管理器
- [GameKeyCode.cs.md](./GameKeyCode.cs.md) - 游戏按键码

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
