using Nino.Core;
using Sirenix.OdinInspector;

namespace TaoTie
{
    [NinoType(false)]
    public partial class ConfigAIDecisionTree
    {
        [NinoMember(1)][LabelText("AI类型")]
        public string Type;
        [NinoMember(2)]
        public DecisionNode Node;
    }
}