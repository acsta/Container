using Nino.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TaoTie
{
    /// <summary>
    /// 操作值
    /// </summary>
    [NinoType(false)]
    public partial class OperatorValue: BaseValue
    {
        [NinoMember(1)][NotNull]
        public BaseValue Left;
        [NinoMember(2)]
        public LogicMode Op;
        [NinoMember(3)][NotNull][ShowIf("@Op != LogicMode.Default")]
        public BaseValue Right;


        public override float Resolve(AIKnowledge knowledge)
        {
            switch (Op)
            {
                case LogicMode.Add:
                    return Left.Resolve(knowledge) + Right.Resolve(knowledge);
                case LogicMode.Red:
                    return Left.Resolve(knowledge) - Right.Resolve(knowledge);
                case LogicMode.Mul:
                    return Left.Resolve(knowledge) * Right.Resolve(knowledge);
                case LogicMode.Div:
                    return Left.Resolve(knowledge) / Right.Resolve(knowledge);
                case LogicMode.Rem:
                    if (Right.Resolve(knowledge) == 0) return Left.Resolve(knowledge);
                    return Left.Resolve(knowledge) % Right.Resolve(knowledge);
                case LogicMode.Pow:
                    return (int) Mathf.Pow(Left.Resolve(knowledge), Right.Resolve(knowledge));
                case LogicMode.Default:
                    return Left.Resolve(knowledge);
            }

            return 0;
        }
    }
}