using System.Collections.Generic;
using TaoTie.LitJson.Extensions;

namespace TaoTie
{
    public class PlayerData
    {
        /// <summary>
        /// 渠道（排行榜服务端用）
        /// </summary>
        public LoginPlatform Platform;
        /// <summary>
        /// 数据版本号
        /// </summary>
        public long Version;
        /// <summary>
        /// 上次领取侧边栏奖励时间
        /// </summary>
        public long LastSidebarRewards;
        /// <summary>
        /// 是否引导过
        /// </summary>
        public bool IsGuideScene;
        /// <summary>
        /// 头像地址
        /// </summary>
        public string Avatar;
        /// <summary>
        /// 用户昵称
        /// </summary>
        public string NickName;
        /// <summary>
        /// 形象
        /// </summary>
        public int[] Show = null;
        /// <summary>
        /// 上次广告时间
        /// </summary>
        public long LastAdTime;
        /// <summary>
        /// 上次看广告次数
        /// </summary>
        public int AdCountTotal;
        /// <summary>
        /// 已解锁科技树节点Id
        /// </summary>
        public HashSet<int> UnlockTechnologyTreeIds;
        /// <summary>
        /// 完成任务的Id和次数
        /// </summary>
        public Dictionary<int, int> OverTaskCount;
        /// <summary>
        /// 进行中任务Id和进度
        /// </summary>
        public Dictionary<int, int> RunningTaskSteps;
        /// <summary>
        /// 餐厅等级
        /// </summary>
        public int RestaurantLv;
        /// <summary>
        /// 是否解锁自动洗盘子
        /// </summary>
        public bool WashDishAuto;
        /// <summary>
        /// 上一次收取餐厅奖励时间
        /// </summary>
        public long LastReceiveRestaurantTime;
        /// <summary>
        /// 扩容次数
        /// </summary>
        public int ExpandTimes;
        /// <summary>
        /// 上一次刷新时间
        /// </summary>
        public long LastRefreshDailyTime;
        /// <summary>
        /// 超市任务
        /// </summary>
        public List<int> DailyTaskIds;
        /// <summary>
        /// 超市任务进度条奖励
        /// </summary>
        public HashSet<int> DailyRewards;
        /// <summary>
        /// 服装店衣服
        /// </summary>
        public List<int> ClothsSale;
        /// <summary>
        /// 金钱
        /// </summary>
        public string RankValue = "0";
        /// <summary>
        /// 上一次选关
        /// </summary>
        public int LastLevelId;
        [JsonIgnore]
        public BigNumber Money
        {
            get
            {
                return RankValue;
            }
            set
            {
                RankValue = value;
            }
        }

        /// <summary>
        /// 物品数量
        /// </summary>
        public Dictionary<int,int> ItemCount;
        
        /// <summary>
        /// 拍卖场数（进入就算）
        /// </summary>
        public Dictionary<int,int> PlayCount;

        /// <summary>
        /// 今日盈利
        /// </summary>
        public long WinToday;
        /// <summary>
        /// 是否领取今日盈利
        /// </summary>
        public bool IsGotWinRewards;

        /// <summary>
        /// 解锁黑市后必出
        /// </summary>
        public List<int> UnlockList;
        /// <summary>
        /// 引导
        /// </summary>
        public List<int> OverGuide;

        /// <summary>
        /// 今日玩法看广告次数
        /// </summary>
        public int PlayableCount;
        /// <summary>
        /// 今日大转盘次数
        /// </summary>
        public int TurnTableCount;
    }
}