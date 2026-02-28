# TimerType.cs 注解文档

## 文件基本信息
- **路径**: Assets/Scripts/Mono/Module/Const/TimerType.cs
- **职责**: 定义定时器类型常量

## 定时器类型
| 常量 | 值 | 说明 |
|------|-----|------|
| NumericUpdate | 1000 | 数值更新 |
| AiAuction | 1001 | AI 拍卖 |
| DelayDestroyEntity | 1003 | 延迟销毁 Entity |
| ComponentUpdate | 1004 | 组件 Update |
| UIRestaurantViewUpdate | 1005 | 餐厅 UI 更新 |
| UIMarketView | 1006 | 市场 UI |
| DailyRefresh | 1007 | 每日刷新 |
| ResetTimeScale | 1008 | 重置时间缩放 |
| UIShopWin | 1009 | 商店 UI |
| UIWashDishViewUpdate | 1010 | 洗碗 UI 更新 |
| UIMatchUpdate | 1011 | 匹配 UI 更新 |

## 使用示例
```csharp
[Timer(TimerType.NumericUpdate)]
public class NumericUpdateTimer : ATimer<Entity>
{
    public override void Run(Entity e)
    {
        e.UpdateNumeric();
    }
}

// 创建定时器
long timerId = TimerManager.Instance.NewTimer(
    TimerType.DelayDestroyEntity,
    entity
);
```
