using Nino.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TaoTie
{
    [NinoType(false)][LabelText("配置表随机出价时间")]
    public class RandomAuctionTime: BaseValue
    {
        public override float Resolve(AIKnowledge knowledge)
        {
            return Random.Range(knowledge.Config.AuctionTime[0], knowledge.Config.AuctionTime[1]);
        }
    }
}