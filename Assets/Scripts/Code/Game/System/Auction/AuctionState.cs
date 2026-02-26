namespace TaoTie
{
    public enum AuctionState
    {
        /// <summary> 空闲 </summary>
        Free,
        /// <summary> 第一场进入前 </summary>
        Awake,
        /// <summary> 第一场进入准备中 </summary>
        Prepare,
        /// <summary> 开场动画 </summary>
        EnterAnim,
        /// <summary> 当前轮准备完成 </summary>
        Ready,
        /// <summary> 当前轮进行中 </summary>
        AIThink,
        /// <summary> 等待玩家操作 </summary>
        WaitUser,
        /// <summary> 当前轮结束动画 </summary>
        ExitAnim,
        /// <summary> 等待玩家开箱 </summary>
        OpenBox,
        /// <summary> 当前轮结算 </summary>
        Over,
        /// <summary> 再次入场动画 </summary>
        ReEnterAnim,
        /// <summary> 所有结束动画 </summary>
        AllOverAnim,
        /// <summary> 所有轮结束 </summary>
        AllOver,
        /// <summary> 再来一局 </summary>
        RePrepare,
    }
}