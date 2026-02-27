# UpdateIsSHProcess.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | UpdateIsSHProcess.cs |
| **路径** | Assets/Scripts/Code/Module/Update/UpdateProcess/UpdateIsSHProcess.cs |
| **所属模块** | 框架层 → Code/Module/Update/UpdateProcess |
| **文件职责** | 检查提审模式（SH=ShenHe） |

---

## 类/结构体说明

### UpdateIsSHProcess

| 属性 | 说明 |
|------|------|
| **职责** | 检查并设置提审模式标志 |
| **泛型参数** | 无 |
| **继承关系** | 继承自 `UpdateProcess` |
| **实现的接口** | 无 |

```csharp
public class UpdateIsSHProcess : UpdateProcess
{
    // 提审模式检查流程
}
```

---

## 方法说明

### Process(UpdateTask task)

**签名**:
```csharp
public override async ETTask<UpdateRes> Process(UpdateTask task)
```

**职责**: 执行提审模式检查

**参数**:
- `task`: 更新任务实例

**返回**: 更新结果 UpdateRes

**核心逻辑**:
```
1. 获取当前渠道
2. 读取 PlayerPrefs 中的 DEBUG_IsSH 标志
3. 如果标志为 0（未设置）：
   - 查找最大资源版本号
   - 如果找不到最大版本，设置 Define.IsSH = true
   - 否则设置 Define.IsSH = false
4. 如果标志不为 0：
   - 直接使用标志值（1=true, 2=false）
5. 记录日志
6. 返回 Over
```

**调试标志**:
```csharp
int setVal = UnityEngine.PlayerPrefs.GetInt("DEBUG_IsSH", 0);
if (setVal == 0)
{
    // 未设置，自动判断
    Define.IsSH = !ServerConfigManager.Instance.FindMaxUpdateResVerThisAppVer(
        channel, task.AppVer, out var maxAppResVer
    );
}
else
{
    // 手动设置
    Define.IsSH = setVal == 1;
}
```

**调用者**: `UpdateTask.Process()`

**依赖**: 必须在 `SetUpdateListProcess` 之后执行（需要更新列表）

---

## 提审模式（IsSH）

**用途**:
- 应用商店审核期间使用的特殊模式
- 可能包含审核专用配置
- 避免审核期间影响正常用户

**判断逻辑**:
- 如果找不到可更新的资源版本 → 提审模式
- 如果能找到可更新的资源版本 → 非提审模式

**原理**:
- 提审时，审核包是独立的，不连接正式更新服务器
- 因此找不到更新列表，判断为提审模式

---

## 阅读指引

### 建议的阅读顺序

1. **理解流程作用** - 提审模式是什么
2. **看 Process** - 理解检查逻辑
3. **了解调试标志** - 理解 DEBUG_IsSH
4. **看判断逻辑** - 理解自动判断原理

### 最值得学习的技术点

1. **PlayerPrefs 缓存**: 保存调试标志
2. **自动判断**: 根据更新列表判断提审状态
3. **调试覆盖**: 支持手动设置标志
4. **依赖顺序**: 必须在 SetUpdateListProcess 之后

---

## 使用示例

### 示例 1: 添加到更新链

```csharp
UpdateTask task = new UpdateTask();
await task.Init(
    onDownloadSizeCallBack,
    new SetWhiteListProcess(),
    new SetUpdateListProcess(),  // 必须先获取更新列表
    new UpdateIsSHProcess(),     // 再检查提审模式
    new AppUpdateProcess(),
    new MainPackageUpdateProcess()
);

UpdateRes result = await task.Process();
```

### 示例 2: 手动设置提审模式

```csharp
// 强制设置为提审模式
UnityEngine.PlayerPrefs.SetInt("DEBUG_IsSH", 1);
UnityEngine.PlayerPrefs.Save();

// 强制设置为非提审模式
UnityEngine.PlayerPrefs.SetInt("DEBUG_IsSH", 2);
UnityEngine.PlayerPrefs.Save();

// 恢复自动判断
UnityEngine.PlayerPrefs.SetInt("DEBUG_IsSH", 0);
UnityEngine.PlayerPrefs.Save();
```

### 示例 3: 检查提审状态

```csharp
if (Define.IsSH)
{
    Log.Info("当前是提审模式");
    // 使用提审专用配置
    UseReviewConfig();
}
else
{
    Log.Info("当前是正式模式");
    // 使用正式配置
    UseLiveConfig();
}
```

---

## 流程示例

```
游戏启动
    ↓
SetUpdateListProcess.Process()  // 先获取更新列表
    ↓
UpdateIsSHProcess.Process()
    ↓
读取 DEBUG_IsSH
    ↓
值 = 0？
├─ 是 → 查找最大版本
│        ↓
│    找到？
│    ├─ 是 → Define.IsSH = false（正式模式）
│    └─ 否 → Define.IsSH = true（提审模式）
└─ 否 → 
    ├─ 1 → Define.IsSH = true
    └─ 2 → Define.IsSH = false
    ↓
记录日志："提审模式：{Define.IsSH}"
```

---

## 相关文档

- [SetUpdateListProcess.cs.md](./SetUpdateListProcess.cs.md) - 获取更新列表
- [ServerConfigManager.cs.md](../ServerConfigManager.cs.md) - 服务器配置管理器
- [UpdateTask.cs.md](../UpdateTask.cs.md) - 更新任务

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
