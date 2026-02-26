using Sirenix.OdinInspector;

namespace TaoTie
{
    [LabelText("距上次出价时间（ms）")]
    public class TimeSinceLastBid: BaseValue
    {
        public override float Resolve(AIKnowledge knowledge)
        {
            return GameTimerManager.Instance.GetTimeNow() - knowledge.LastBidTime;
        }
    }
}