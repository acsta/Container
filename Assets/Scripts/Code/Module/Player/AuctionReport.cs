using TaoTie.LitJson.Extensions;

namespace TaoTie
{
    public enum ReportType
    {
        NoResult,//无结果
        Self,//我拍下
        Others,//他人拍下
        Pass,//流拍
    }
    public class AuctionReport
    {
        public int Index;
        /// <summary>
        /// 集装箱类型
        /// </summary>
        public int ContainerId;
        /// <summary>
        /// 物品
        /// </summary>
        public ItemConfig[] Items;
        /// <summary>
        /// 物品类型
        /// </summary>
        public BoxType[] BoxTypes;
        /// <summary>
        /// 小玩法数据
        /// </summary>
        public object[] PlayData;
        /// <summary>
        /// 拍卖结果
        /// </summary>
        public ReportType Type;
        /// <summary>
        /// 抬价成功时的抬价次数，没成功为0
        /// </summary>
        public int RaiseSuccessCount;
        [JsonIgnore]
        public BigNumber RaisePrice => RaisePriceStr;
        /// <summary>
        /// 抬价金额
        /// </summary>
        public string RaisePriceStr;
        /// <summary>
        /// 任务物品序号（-1无）
        /// </summary>
        public int TaskItemIndex;
        /// <summary>
        /// 出售类型 0 普通出售
        /// </summary>
        public int SaleType;
        /// <summary>
        /// 拍得价格
        /// </summary>
        public string LastAuctionPriceStr;
        /// <summary>
        /// 情报Id
        /// </summary>
        public int GameInfoId;
        [JsonIgnore]
        public BigNumber LastAuctionPrice => LastAuctionPriceStr;
        
        /// <summary>
        /// 最终售价
        /// </summary>
        public string AllPriceStr;
        [JsonIgnore]
        public BigNumber AllPrice => AllPriceStr;


        /// <summary>
        /// 玩家最终盈利
        /// </summary>
        public BigNumber FinalUserWin
        {
            get
            {
                if (Type == ReportType.Self)
                {
                    if (SaleType == 0) return AllPrice - LastAuctionPrice;
                    Log.Error("未处理的出手类型=" + SaleType);
                }

                if (RaiseSuccessCount > 0)
                {
                    var playerRaiseRewardsMoney = RaisePrice * AuctionHelper.GetRaiseMul(RaiseSuccessCount);
                    if (GameInfoId > 0)
                    {
                        var gameInfo = GameInfoConfigCategory.Instance.Get(GameInfoId);
                        playerRaiseRewardsMoney = gameInfo.GetRaiseRewards(playerRaiseRewardsMoney);
                    }
                    return playerRaiseRewardsMoney;
                }

                return BigNumber.Zero;
            }
        }

    }
}