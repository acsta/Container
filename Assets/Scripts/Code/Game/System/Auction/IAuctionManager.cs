using System;
using System.Collections.Generic;

namespace TaoTie
{
    public interface IAuctionManager
    {
        public static bool UserReady;
        public static IAuctionManager Instance;
        public AuctionState AState { get; }
        /// <summary> 难度 </summary>
        public int Level { get; }
        /// <summary> 轮次从1开始 </summary>
        public int Stage { get; }
        /// <summary>
        /// 情报Id
        /// </summary>
        public int GameInfoId{ get; }
        public StageConfig Config{ get; }
        public LevelConfig LevelConfig{ get; }
        public GameInfoConfig GameInfoConfig { get; }
        public DiceConfig DiceConfig { get; }
        public StartEventConfig StartEventConfig { get; }
        public AuctionReport[] AuctionReports { get; }
        
        public AuctionReport Report { get; }
        public BigNumber LowAuction { get; }

        /// <summary> 中价 </summary>
        public BigNumber MediumAuction{ get; }
        /// <summary> 高价 </summary>
        public BigNumber HighAuction{ get; }
        /// <summary>
        /// 命运骰子Id
        /// </summary>
        public int DiceId{ get; }
        public List<long> Bidders { get; }
        public List<long> Npcs { get; }
        public List<long> Boxes { get; }
        public List<long> OpenBoxes { get; }
        public List<long> Blacks { get; }
        public Player Player{ get; }
        public long HostId{ get; }
        public BigNumber AllPrice { get;  } //总价值
        public float SysJudgePriceMin{ get; }  //本关区间价值判断最小值
        public float SysJudgePriceMax{ get;  }	//本关区间价值判断最大值
        public long LastAuctionPlayerId { get; } //上一个叫价的人 -1没有，0玩家，其他AIid
        public BigNumber LastAuctionPrice { get; } //上一次叫价
        public AITactic LastAuctionPriceType{ get;  } //上一次叫价类型
        public long LastAuctionTime{ get; } //上一次叫价间隔时间
        public int LastHostSayCount { get;  } //上一次叫价拍卖师倒计时次数;
        public bool IsRaising { get; } //是否抬价阶段
        public int RaiseSuccessCount { get; } //玩家成功抬价次数
        public int RaiseCount{ get; } //抬价次数，进入抬价那次不算次数
        public int PlayerAuctionCount { get; } //玩家出价次数
        public int AuctionCount { get; } //总出价次数
        public bool HasMiniPlayItem { get;  }//小玩法物品
        public bool HasTaskItem { get; }//任务物品
        public bool Skip{ get; set; }//快速跳过
        public bool IsAllPlayBox{ get; }//是否全玩法箱子
        /// <summary>
        /// 强行退出
        /// </summary>
        public void ForceAllOver();

        /// <summary>
        /// 玩家出价
        /// </summary>
        /// <param name="type"></param>
        public void UserAuction(AITactic type);

        /// <summary>
        /// 进行下一场
        /// </summary>
        public void RunNextStage();

        /// <summary>
        /// 设置鉴定结果
        /// </summary>
        /// <param name="configId"></param>
        /// <param name="newId"></param>
        public void SetAppraisalResult(int configId, int newId);

        /// <summary>
        /// 设置小游戏结果
        /// </summary>
        /// <param name="configId"></param>
        /// <param name="newPrice"></param>
        public void SetMiniGameResult(int configId, BigNumber newPrice);

        /// <summary>
        /// 根据当前状态判断是否应用情报并返回
        /// </summary>
        public GameInfoConfig GetFinalGameInfoConfig(bool ignoreId = false);

        public void SelectGameInfo(int id);

        public void SelectDice(int id, Action onSelectOver);

        public void RefreshWinLossAnim(bool play);

        /// <summary>
        /// AI离场
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type">类型：0走开</param>
        public void Leave(long id, int type);

        /// <summary>
        /// 获取离场人数
        /// </summary>
        /// <returns></returns>
        public int GetLevelCount();
    }
}