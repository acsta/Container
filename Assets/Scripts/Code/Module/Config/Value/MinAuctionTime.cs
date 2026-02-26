using Nino.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TaoTie
{
    [NinoType(false)][LabelText("配置表最低出价时间")]
    public class MinAuctionTime: BaseValue
    {
        public override float Resolve(AIKnowledge knowledge)
        {
            return knowledge.Config.AuctionTime[0];
        }
    }
}