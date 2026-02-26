using UnityEngine;

namespace TaoTie
{
#if UNITY_EDITOR
    using PlayerPrefs = UnityEngine.PlayerPrefs;
#endif
    public enum ContainerRandType
    {
        All,
        OnlyNormal,
        OnlySp,
        Target,
    }
    public static class GameSetting
    {
        public static void Init()
        {
            OpenAIAuction = PlayerPrefs.GetInt("DEBUG_OpenAIAuction", 1) == 1;
            ContainerRandType = (ContainerRandType)PlayerPrefs.GetInt("DEBUG_ContainerRandType", 0);
            AlwaysPlayType = PlayerPrefs.GetInt("DEBUG_AlwaysPlayType", 0) == 1;
            GameInfoTargetType = (GameInfoTargetType)PlayerPrefs.GetInt("DEBUG_GameInfoTargetType", -1);
            RaiseCount = PlayerPrefs.GetInt("DEBUG_RaiseCount", 0);
            PlayableResult = (PlayableResult)PlayerPrefs.GetInt("DEBUG_PlayableResult", 0);
            ContainerId = PlayerPrefs.GetInt("DEBUG_ContainerId", 1001);
            AlwaysTurnTable = PlayerPrefs.GetInt("DEBUG_AlwaysTurnTable", 0) == 1;
            AlwaysStoryEvent = PlayerPrefs.GetInt("DEBUG_AlwaysStoryEvent", 0) == 1;
        }
        /// <summary>
        /// 开启AI叫价
        /// </summary>
        public static bool OpenAIAuction = true;
        
        /// <summary>
        /// 集装箱随机类型
        /// </summary>
        public static ContainerRandType ContainerRandType = ContainerRandType.All;

        /// <summary>
        /// 集装箱Id
        /// </summary>
        public static int ContainerId = 0;
        /// <summary>
        /// 必出玩法物品
        /// </summary>
        public static bool AlwaysPlayType = false;

        /// <summary>
        /// 随机的情报类型
        /// </summary>
        public static GameInfoTargetType GameInfoTargetType = GameInfoTargetType.Random;

        /// <summary>
        /// 玩家抬价必定成功次数
        /// </summary>
        public static int RaiseCount;

        /// <summary>
        /// 玩法结果
        /// </summary>
        public static PlayableResult PlayableResult = PlayableResult.None;

        /// <summary>
        /// 必出大转盘
        /// </summary>
        public static bool AlwaysTurnTable = false;

        /// <summary>
        /// 必出开箱剧情
        /// </summary>
        public static bool AlwaysStoryEvent = false;
    }
}