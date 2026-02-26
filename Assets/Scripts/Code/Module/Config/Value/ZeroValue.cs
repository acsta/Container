using Nino.Core;

namespace TaoTie
{
    [NinoType(false)]
    public partial class ZeroValue: BaseValue
    {
        public override float Resolve(AIKnowledge knowledge)
        {
            return 0;
        }
    }
}