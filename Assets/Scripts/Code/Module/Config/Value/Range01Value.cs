using Nino.Core;
using UnityEngine;

namespace TaoTie
{
    /// <summary>
    /// 随机[0,1]
    /// </summary>
    [NinoType(false)]
    public partial class Range01Value: BaseValue
    {
        public override float Resolve(AIKnowledge knowledge)
        {
            return Random.Range(0f, 1f);
        }
    }
}