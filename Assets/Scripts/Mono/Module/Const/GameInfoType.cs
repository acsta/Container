namespace TaoTie
{
    public enum GameInfoTargetType
    {
        /// <summary>
        /// 随机，debug用
        /// </summary>
        Random = -1,
        /// <summary> 指定集装箱物品价值增加（Ids填集装箱id逗号分割）</summary>
        Container = 0,
        /// <summary> 指定物品物品价值增加（Ids填物品id逗号分割） </summary>
        Items = 1,
        /// <summary> 指定集装箱随机物品价值增加（Ids填集装箱id,随机普通物品的数量） </summary>
        RandItems = 2,
        /// <summary> 抬价收益增加（Ids不填） </summary>
        Raise = 3,
        /// <summary> 指定玩法收益增加（Ids填玩法id逗号分割） </summary>
        PlayType = 4,
    }

    public enum GameInfoConditionType
    {
        /// <summary> 无</summary>
        None = 0,
        /// <summary> 最少抬价次数</summary>
        MinRaiseCount = 1,
        /// <summary> 最高出价次数</summary>
        MaxAuctionCount = 2,
    }
    
    public enum PlayableResult
    {
        /// <summary> 无</summary>
        None = 0,
        /// <summary> 必成功</summary>
        Success = 1,
        /// <summary> 必失败</summary>
        Fail = 2,
    }
}