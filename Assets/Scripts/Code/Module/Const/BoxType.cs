namespace TaoTie
{
    public enum BoxType
    {
        /// <summary>
        /// 正常物品（只有Normal物品参与算钱）
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 任务物品
        /// </summary>
        Task = 1,
        /// <summary>
        /// 空箱子
        /// </summary>
        Empty = 2,
        /// <summary>
        /// 随机开箱事件
        /// </summary>
        RandOpenEvt = 3,
        /// <summary>
        /// 财神爷事件
        /// </summary>
        GodOfWealthEvt = 4,
    }
}