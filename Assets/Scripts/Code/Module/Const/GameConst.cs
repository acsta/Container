using System.Collections.Generic;
using UnityEngine;

namespace TaoTie
{
    public class GameConst
    {
        public const string SmokePrefab = "Unit/Box/Prefabs/BoxBoom.prefab";
        public const string SmokePrefab2 = "Unit/Box/Prefabs/VFX_Comet_Impact.prefab";
        public const string DefaultImage = "UI/UICommon/DiscreteImages/PictureUnknow.png";
        public const string TaskMat = "UIGame/UIAuction/Materials/M_TexturedPlayType.mat";
        public const string PlayTypeMat = "UIGame/UIAuction/Materials/M_TexturedTask.mat";
        public const string LightBall_UI = "UIGame/UILobby/Prefabs/LightBall_UI.prefab";
        public const string LightBall_Scene = "UIGame/UILobby/Prefabs/LightBall.prefab";
        public const string bomb = "UIGame/UILobby/Prefabs/bomb.prefab";
        public const string TaskPrefab = "UIGame/UILobby/Prefabs/Task.prefab";
        public const int MaxBoxCount = 6;
        public const float HomeCameraMinX = -8f;
        public const float HomeCameraMaxX = 20;

        public const int MAX_RUNNING_TASK_COUNT = 3;

        public const string WIN_COLOR = "#6B800F";
        public const string LOSS_COLOR = "#FF0000";
        public const string DEFAUT_COLOR = "#3C3C3C";
        public const string WHITE_COLOR = "#FFFFFF";
        public const string RED_COLOR = "#EE4751";    
        public const string GRAY_COLOR = "#B8B8B8";
        public const string GREEN_COLOR = "#83C740";
        public const string COMMON_TEXT_COLOR = "#AD945E";
        /// <summary>
        /// 大转盘Id
        /// </summary>
        public const int TurnTableUnitId = 3;
        public const int CharacterUnitId = 14;
        public const int UFOUnitId = 12;
        /// <summary>
        /// 金币物品Id
        /// </summary>
        public const int MoneyItemId = 1;
        /// <summary>
        /// 骰子物品Id
        /// </summary>
        public const int DiceItemId = 1001;

        /// <summary>
        /// 全局配置表餐厅利润单位时间（ms）
        /// </summary>
        public static int ProfitUnitTime;
        /// <summary>
        /// UI显示餐厅利润单位时间（ms）
        /// </summary>
        public static int ProfitUnitShowTime;

        /// <summary>
        /// 任务物品颜色
        /// </summary>
        public static Color TaskItemColor;
        /// <summary>
        /// 玩法物品颜色
        /// </summary>
        public static Color PlayableItemColor;
        /// <summary>
        /// 物品价值颜色
        /// </summary>
        public static Color ItemPriceColor;
        /// <summary>
        /// 大转盘每日出现次数
        /// </summary>
        public static int[] TurnTableAdCount;
        /// <summary>
        /// 玩法物品看广告次数
        /// </summary>
        public static int[] PlayableAdCount;
        /// <summary>
        /// 玩法物品每日最多看广告玩的次数
        /// </summary>
        public static int PlayableMaxAdCount;
        /// <summary>
        /// timeline触发主持人报幕，间隔1，小助理估价，间隔2，蓝色估计栏出现，间隔3，起拍价，间隔4，小助理提示任务物品
        /// </summary>
        public static int[] OpenStageInterval;

        /// <summary>
        /// 大转盘随出现次数的出现概率
        /// </summary>
        public static int[] TurnTablePercent;
    }
}