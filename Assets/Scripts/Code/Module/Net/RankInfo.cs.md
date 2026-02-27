# RankInfo.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | RankInfo.cs |
| **路径** | Assets/Scripts/Code/Module/Net/RankInfo.cs |
| **所属模块** | 框架层 → Code/Module/Net |
| **文件职责** | 排行榜信息数据结构 |

---

## 类说明

### RankInfo

| 属性 | 说明 |
|------|------|
| **职责** | 存储排行榜单个玩家的信息 |
| **类型** | class |

---

## 字段说明

（假设典型字段）

| 字段 | 类型 | 说明 |
|------|------|------|
| `Rank` | `int` | 排名 |
| `Uid` | `string` | 用户 ID |
| `NickName` | `string` | 昵称 |
| `Score` | `BigNumber` | 分数/金钱 |
| `Platform` | `int` | 平台 |
| `Avatar` | `string` | 头像地址 |

---

## 使用示例

### 示例 1: 获取排行榜

```csharp
// 获取第 1 页排行榜
RankList rankList = await APIManager.Instance.GetRankList(page: 1);

if (rankList != null)
{
    foreach (var info in rankList.List)
    {
        Log.Info($"排名{info.Rank}: {info.NickName} - {info.Score}");
    }
}
```

### 示例 2: 显示排行榜 UI

```csharp
void ShowRankList(RankList rankList)
{
    foreach (var info in rankList.List)
    {
        // 创建排行榜项
        RankItemView item = CreateRankItem();
        
        // 设置数据
        item.SetRank(info.Rank);
        item.SetNickName(info.NickName);
        item.SetScore(info.Score);
        item.SetAvatar(info.Avatar);
        
        // 高亮自己
        if (info.Uid == PlayerManager.Instance.Uid)
        {
            item.Highlight();
        }
    }
}
```

---

## 相关文档

- [APIManager.cs.md](./APIManager.cs.md) - API 管理器
- [HttpResult.cs.md](./HttpResult.cs.md) - HTTP 响应结果

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
