# AppUpdateProcess.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | AppUpdateProcess.cs |
| **路径** | Assets/Scripts/Code/Module/Update/UpdateProcess/AppUpdateProcess.cs |
| **所属模块** | 框架层 → Code/Module/Update/UpdateProcess |
| **文件职责** | APP 版本更新检查流程 |

---

## 类/结构体说明

### AppUpdateProcess

| 属性 | 说明 |
|------|------|
| **职责** | 检查 APP 版本，提示用户更新，打开下载链接 |
| **泛型参数** | 无 |
| **继承关系** | 继承自 `UpdateProcess` |
| **实现的接口** | 无 |

```csharp
public class AppUpdateProcess : UpdateProcess
{
    // APP 版本更新检查流程
}
```

---

## 方法说明

### Process(UpdateTask task)

**签名**:
```csharp
public override async ETTask<UpdateRes> Process(UpdateTask task)
```

**职责**: 执行 APP 版本更新检查

**参数**:
- `task`: 更新任务实例

**返回**: 更新结果 UpdateRes

**核心逻辑**:
```
1. 获取当前渠道
2. 获取渠道更新列表
3. 如果列表为空，返回 Over
4. 查找最大可更新版本号
5. 如果无更新，返回 Over
6. 比较当前版本和最大版本
7. 如果已是最新版本，返回 Over
8. 获取下载链接
9. 检查是否强制更新
10. 检查是否已提示过（非强制）
11. 显示提示框
12. 如果用户确认：
    - 打开下载链接
    - 递归调用 Process（防止用户返回游戏）
13. 如果强制更新且用户取消：
    - 退出游戏
14. 如果非强制且用户取消：
    - 缓存已提示标记
15. 返回 Over
```

**版本比较**:
```csharp
var appVer = task.AppVer;
var flag = appVer - version;
if (flag >= 0)
{
    // 已是最新版本
    return UpdateRes.Over;
}
```

**强制更新检查**:
```csharp
if (!Define.ForceUpdate)
{
    if (verInfo != null && verInfo.ForceUpdate == -1)
        return UpdateRes.Over;  // 直接不提示
}

var forceUpdate = Define.ForceUpdate;
if (verInfo != null && verInfo.ForceUpdate != 0)
    forceUpdate = true;
```

**调用者**: `UpdateTask.Process()`

---

## 阅读指引

### 建议的阅读顺序

1. **理解流程作用** - APP 更新检查做什么
2. **看 Process** - 理解检查逻辑
3. **了解强制更新** - 理解强制更新机制
4. **看缓存逻辑** - 理解防骚扰机制

### 最值得学习的技术点

1. **版本比较**: 整数版本号比较
2. **强制更新**: 支持强制/非强制更新
3. **防骚扰**: 缓存已提示标记
4. **递归调用**: 防止用户返回游戏
5. **跳转渠道**: 支持渠道跳转配置

---

## 更新配置（AppConfig）

| 字段 | 说明 |
|------|------|
| `AppUrl` | APP 下载链接 |
| `AppVer` | 版本信息字典 |
| `JumpChannel` | 跳转渠道（可选） |

**版本信息（Resver）**:
| 字段 | 说明 |
|------|------|
| `ForceUpdate` | 是否强制更新（0=否，1=是，-1=不提示） |
| `Channel` | 适用渠道列表 |
| `UpdateTailNumber` | 适用尾号列表 |

---

## 使用示例

### 示例 1: 添加到更新链

```csharp
UpdateTask task = new UpdateTask();
await task.Init(
    onDownloadSizeCallBack,
    new SetWhiteListProcess(),
    new SetUpdateListProcess(),
    new AppUpdateProcess(),  // APP 版本检查
    new MainPackageUpdateProcess()
);

UpdateRes result = await task.Process();
```

### 示例 2: 配置强制更新

```json
{
  "AppList": {
    "googleplay": {
      "AppUrl": "https://play.google.com/store/apps/details?id=com.example",
      "AppVer": {
        "1": {
          "ForceUpdate": 1,
          "Channel": ["all"],
          "UpdateTailNumber": ["all"]
        }
      }
    }
  }
}
```

**ForceUpdate 值说明**:
- `0`: 非强制更新（可跳过）
- `1`: 强制更新
- `-1`: 不提示

---

## 流程示例

```
当前版本：10
最大版本：15
    ↓
显示提示框："发现新版本，是否更新？"
    ↓
用户选择：
├─ 确认 → 打开下载链接 → 递归检查
└─ 取消 → 
    ├─ 强制更新 → 退出游戏
    └─ 非强制 → 缓存标记，进入游戏
```

---

## 相关文档

- [UpdateTask.cs.md](../UpdateTask.cs.md) - 更新任务
- [UpdateProcess.cs.md](./UpdateProcess.cs.md) - 更新流程基类
- [ServerConfigManager.cs.md](../ServerConfigManager.cs.md) - 服务器配置管理器

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
