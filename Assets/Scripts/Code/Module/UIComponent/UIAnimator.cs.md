# UIAnimator.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | UIAnimator.cs |
| **路径** | Assets/Scripts/Code/Module/UIComponent/UIAnimator.cs |
| **所属模块** | 框架层 → Code/Module/UIComponent |
| **文件职责** | UI 动画机组件，封装 Animator |

---

## 类/结构体说明

### UIAnimator

| 属性 | 说明 |
|------|------|
| **职责** | 封装 Unity Animator，提供动画播放控制 |
| **泛型参数** | 无 |
| **继承关系** | 继承自 `UIBaseContainer` |
| **实现的接口** | 无 |

```csharp
public class UIAnimator : UIBaseContainer
{
    // UI 动画机组件
}
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `animator` | `Animator` | `private` | Unity Animator 组件 |
| `clips` | `Dictionary<string, AnimationClip>` | `private` | 动画剪辑字典 |

---

## 方法说明（按重要程度排序）

### ActivatingComponent()

**签名**:
```csharp
void ActivatingComponent()
```

**职责**: 激活组件

**核心逻辑**:
```
1. 如果 animator 为 null
2. 获取 Animator 组件
3. 如果组件不存在，记录错误
4. 否则，缓存所有动画剪辑到 clips 字典
```

**调用者**: 所有公共方法

---

### SetEnable(bool enable)

**签名**:
```csharp
public void SetEnable(bool enable)
```

**职责**: 设置动画机组用/禁用

**参数**:
- `enable`: true=启用，false=禁用

**核心逻辑**:
```
1. 激活组件
2. 设置 animator.enabled = enable
```

**调用者**: 需要控制动画状态的代码

---

### Play(string name, int offset)

**签名**:
```csharp
public async ETTask Play(string name, int offset = 0)
```

**职责**: 播放动画（等待完成）

**参数**:
- `name`: 动画名称
- `offset`: 额外等待时间（毫秒）

**返回**: ETTask（可等待）

**核心逻辑**:
```
1. 激活组件
2. 播放动画 animator.Play(name)
3. 查找动画剪辑
4. 如果找到，等待动画时长 + offset
5. 如果未找到，直接返回
```

**调用者**: 需要等待动画完成的代码

**使用示例**:
```csharp
// 播放动画并等待完成
await animator.Play("OpenAnim");

// 播放动画并额外等待 500ms
await animator.Play("OpenAnim", offset: 500);
```

---

### CrossFade(string name, float during)

**签名**:
```csharp
public void CrossFade(string name, float during = 0.5f)
```

**职责**: 淡入淡出播放动画

**参数**:
- `name`: 动画名称
- `during`: 过渡时间（秒）

**核心逻辑**:
```
1. 激活组件
2. 调用 animator.CrossFade(name, during)
```

**与 Play 的区别**:
- `Play`: 立即切换动画
- `CrossFade`: 平滑过渡到目标动画

**调用者**: 需要平滑过渡的代码

---

### SetBool(string key, bool val)

**签名**:
```csharp
public void SetBool(string key, bool val)
```

**职责**: 设置 Animator 布尔参数

**参数**:
- `key`: 参数名称
- `val`: 参数值

**核心逻辑**:
```
1. 激活组件
2. 调用 animator.SetBool(key, val)
```

**调用者**: 需要控制 Animator 参数的代码

**使用示例**:
```csharp
// 设置"IsOpen"参数为 true
animator.SetBool("IsOpen", true);

// 触发状态切换
animator.SetBool("IsMoving", false);
```

---

## 阅读指引

### 建议的阅读顺序

1. **理解组件作用** - 为什么需要 UIAnimator
2. **看 Play** - 理解动画播放
3. **看 CrossFade** - 理解平滑过渡
4. **了解参数控制** - 理解 SetBool

### 最值得学习的技术点

1. **异步播放**: Play 返回 ETTask 可等待
2. **动画缓存**: clips 字典缓存动画剪辑
3. **平滑过渡**: CrossFade 支持淡入淡出
4. **参数控制**: SetBool 控制 Animator 状态

---

## 使用示例

### 示例 1: 窗口打开动画

```csharp
public class UIWindowView : UIBaseView, IOnCreate
{
    private UIAnimator animator;
    
    public void OnCreate()
    {
        animator = AddComponent<UIAnimator>("Animator");
    }
    
    public async ETTask OpenWindow()
    {
        // 播放打开动画并等待完成
        await animator.Play("OpenAnim");
        
        // 动画完成后设置状态
        animator.SetBool("IsOpen", true);
    }
    
    public async ETTask CloseWindow()
    {
        // 播放关闭动画并等待完成
        await animator.Play("CloseAnim");
        
        // 动画完成后设置状态
        animator.SetBool("IsOpen", false);
    }
}
```

### 示例 2: 平滑过渡

```csharp
// 从当前动画平滑过渡到"Idle"动画
animator.CrossFade("Idle", during: 0.3f);
```

### 示例 3: 状态机控制

```csharp
public class UICharacterView : UIBaseView
{
    private UIAnimator animator;
    
    public void SetMoving(bool isMoving)
    {
        animator.SetBool("IsMoving", isMoving);
    }
    
    public void SetAttacking(bool isAttacking)
    {
        animator.SetBool("IsAttacking", isAttacking);
    }
}
```

### 示例 4: 禁用动画

```csharp
// 禁用动画（用于性能优化）
animator.SetEnable(false);

// 恢复动画
animator.SetEnable(true);
```

---

## 相关文档

- [UIBaseContainer.cs.md](./UIBaseContainer.cs.md) - UI 容器基类
- [UIImage.cs.md](./UIImage.cs.md) - UI 图片组件

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
