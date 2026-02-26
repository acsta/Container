using UnityEngine;

namespace TaoTie
{
    public class AIComponent: Component,IComponent<string>
    {
        /// <summary> 收集的信息 </summary>
        protected AIKnowledge knowledge;
        /// <summary> 这一次决策结果 </summary>
        protected AIDecision decision = new AIDecision();
        /// <summary> 上一次决策结果 </summary>
        protected AIDecision decisionOld = new AIDecision();
        
        #region IComponent

        public void Init(string decisionArchetype)
        {
            knowledge = ObjectPool.Instance.Fetch<AIKnowledge>();
            knowledge.Init(parent, decisionArchetype);
        }
        
        public void Destroy()
        {
            knowledge.Dispose();
            knowledge = null;
        }

        #endregion

        /// <summary>
        /// 决策
        /// </summary>
        public AIDecision Think()
        {
            decisionOld.Act = decision.Act;
            decisionOld.Tactic = decision.Tactic;
            decisionOld.Delay = decision.Delay;
            decisionOld.Emoji = decision.Emoji;
            AIDecisionTree.Think(knowledge, decision);
            if (decision.Tactic == AITactic.Random)
            {
                RandomTactic();
            }
            else if (decision.Tactic == AITactic.RandomLow)
            {
                RandomLowTactic();
            }
            else if (decision.Tactic == AITactic.LeaveWalk)
            {
                IAuctionManager.Instance.Leave(Id, 0);
                decision.Tactic = AITactic.Sidelines;
                decision.Act = ActDecision.Action_Walk;
            }
            else if (decision.Tactic == AITactic.LeaveRun)
            {
                IAuctionManager.Instance.Leave(Id, 1);
                decision.Tactic = AITactic.Sidelines;
                decision.Act = ActDecision.Action_Run;
            }
            if (decision.Act != ActDecision.NoActDecision)
            {
                parent?.GetComponent<CasualActionComponent>()?.PlayAnim(decision.Act.ToString());
            }
            return decision;
        }

        public AIKnowledge GetKnowledge()
        {
            return knowledge;
        }

        /// <summary>
        /// 配置表随机
        /// </summary>
        private void RandomTactic()
        {
            int lv = 0;
            if (AIDecisionInterface.IsMoneyEnoughHigh(knowledge))
            {
                lv = 3;
            }
            else if (AIDecisionInterface.IsMoneyEnoughMedium(knowledge))
            {
                lv = 2;
            }
            else if (AIDecisionInterface.IsMoneyEnoughLow(knowledge))
            {
                lv = 1;
            }
            else
            {
                decision.Tactic = AITactic.Sidelines;
                return;
            }
            int ran = Random.Range(0,knowledge.Width[lv]);
            for (int i = 0; i <= lv; i++)
            {
                if (ran <= knowledge.Width[i])
                {
                    decision.Tactic = (AITactic) i;
                    break;
                }
            }
        }

        /// <summary>
        /// 配置表随机，只有低价才赋值，其他都是观望
        /// </summary>
        private void RandomLowTactic()
        {
            if (AIDecisionInterface.IsMoneyEnoughLow(knowledge))
            {
                int ran = Random.Range(0,knowledge.Width[3]);
                for (int i = 0; i < 4; i++)
                {
                    if (ran <= knowledge.Width[i])
                    {
                        decision.Tactic = (AITactic) i;
                        break;
                    }
                }

                if (decision.Tactic != AITactic.LowWeight)
                {
                    decision.Tactic = AITactic.Sidelines;
                }
            }
            else
            {
                decision.Tactic = AITactic.Sidelines;
            }
        }
    }
}