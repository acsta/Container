using Nino.Core;
using Sirenix.OdinInspector;

namespace TaoTie
{
    [NinoType(false)]
    public partial class DecisionActionNode: DecisionNode
    {
        [NinoMember(10)][LabelText("动画类型")]
        public ActDecision Act;
        [NinoMember(11)][LabelText("决策类型")]
        public AITactic Tactic;
        [NinoMember(12)][LabelText("延迟执行出价时间(ms)")]
        public BaseValue Delay = new RandomAuctionTime();
#if UNITY_EDITOR
        [ValueDropdown("@"+nameof(OdinDropdownHelper)+"."+nameof(OdinDropdownHelper.GetEmoji)+"()")]
#endif
        [NinoMember(13)][LabelText("表情名")]
        public string Emoji;
    }
}