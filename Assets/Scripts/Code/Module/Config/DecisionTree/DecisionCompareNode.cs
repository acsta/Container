using Nino.Core;
using Sirenix.OdinInspector;

namespace TaoTie
{
    [NinoType(false)]
    public partial class DecisionCompareNode: DecisionNode
    {
        [NinoMember(10)][NotNull]
        public BaseValue LeftValue = new SingleValue();
        [NinoMember(11)]
        public CompareMode CompareMode;
        [NinoMember(12)][NotNull]
        public BaseValue RightValue = new SingleValue();
        [NinoMember(13)][NotNull]
        public DecisionNode True;
        [NinoMember(14)][NotNull]
        public DecisionNode False;
    }
}