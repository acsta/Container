using Sirenix.OdinInspector;

namespace TaoTie
{
    public enum AITactic
    {
        [LabelText("观望")]
        Sidelines = 0,
        [LabelText("喊低价")]
        LowWeight = 1,
        [LabelText("喊中价")]
        MediumWeight = 2,
        [LabelText("喊高价")]
        HighWeight = 3,
        [LabelText("梭哈")]
        AllIn = 4,
        [LabelText("钱够则随机")]
        Random = 5,
        [LabelText("钱够则只随机低价")]
        RandomLow = 6,
        [LabelText("离场")]
        LeaveWalk = 7,
        [LabelText("跑路")]
        LeaveRun = 8
    }
}