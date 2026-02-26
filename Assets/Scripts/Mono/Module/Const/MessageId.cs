namespace TaoTie
{
    public static class MessageId
    {
        /// <summary> 增加餐厅收益时间 </summary>
        public const int SetProfit = -6;
        /// <summary> 进入新手引导 </summary>
        public const int EnterGuideScene = -5;
        /// <summary> 解锁所有衣服 </summary>
        public const int UnlockAllCloth = -4;
        /// <summary> 打开大转盘 </summary>
        public const int OpenTurntable = -3;
        /// <summary> 加钱 </summary>
        public const int AddMoney = -2;
            
            
        /// <summary> 游戏时间缩放改变 </summary>
        public const int TimeScaleChange = -1;
        /// <summary> 数值变化 </summary>
        public const int NumericChangeEvt = 1;
        public const int EnterWayChange = 2;
        public const int SidebarRewards = 3;
        /// <summary> 坐标变化 </summary>
        public const int ChangePositionEvt = 4;
        /// <summary> 方向变化 </summary>
        public const int ChangeRotationEvt = 5;
        /// <summary> 缩放变化 </summary>
        public const int ChangeScaleEvt = 6;
        /// <summary> 刷新拍卖状态 </summary>
        public const int RefreshAuctionState = 7;
        /// <summary> 金币刷新 </summary>
        public const int ChangeMoney = 8;

        /// <summary> 鉴定结果 </summary>
        public const int SetChangeItemResult = 9;
        /// <summary> 解锁科技树 </summary>
        public const int UnlockTreeNode = 10;
        /// <summary> 更新任务进度 </summary>
        public const int UpdateTaskStep = 11;
        /// <summary> 检疫结果 </summary>
        public const int SetChangePriceResult = 12;
        /// <summary> 小助理讲话 </summary>
        public const int AssistantTalk = 13;
        /// <summary> 小助理讲话 </summary>
        public const int GuidanceTalk = 14;
        /// <summary> 更新任务进度 </summary>
        public const int ComplexTask = 15;
        /// <summary> 改变物体数量 </summary>
        public const int ChangeItem = 16;
        /// <summary> 引导物体 </summary>
        public const int GuideBox = 17;
        /// <summary> 新手引导物体 </summary>
        public const int GuideBox2 = 18;
        /// <summary> 按键状态改变 </summary>
        public const int OnKeyInput = 19;
        /// <summary>开始播 </summary>
        public const int ClipStartPlay = 20;
        /// <summary>正在播的每一帧 </summary>
        public const int ClipProcess = 21;
        /// <summary>展示估价 </summary>
        public const int ShowTextRange = 22;
    }
}