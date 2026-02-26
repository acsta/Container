using System.Text;

namespace TaoTie
{
    public static class AIDecisionTree
    {
        /// <summary>
        /// 决策
        /// </summary>
        /// <param name="knowledge"></param>
        /// <param name="decision"></param>
        public static void Think(AIKnowledge knowledge, AIDecision decision)
        {
            if(!GameSetting.OpenAIAuction) return;
            var conf = ConfigAIDecisionTreeCategory.Instance.Get(knowledge.DecisionArchetype);
            if (conf != null)
            {
                if (conf.Node != null)
                {
#if UNITY_EDITOR
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(knowledge.DecisionArchetype);
                    Handler(knowledge, decision, conf.Node, sb);
                    // Handler(knowledge, decision, conf.Node);
#else
                    Handler(knowledge, decision, conf.Node);
#endif
                }
            }
        }
        
        #region Tree

        private static void Handler(AIKnowledge knowledge, AIDecision decision, DecisionNode tree, StringBuilder sb = null)
        {
            if (tree is DecisionActionNode actionNode)
            {
                decision.Act = actionNode.Act;
                decision.Tactic = actionNode.Tactic;
                decision.Delay = (int)actionNode.Delay.Resolve(knowledge);
                decision.Emoji = actionNode.Emoji;
#if UNITY_EDITOR
                if (sb != null)
                {
                    Log.Info(sb.ToString()); 
                }
#endif
            }
            else if (tree is DecisionConditionNode conditionNode)
            {
                if (AIDecisionInterface.Methods.TryGetValue(conditionNode.Condition, out var func))
                {
#if UNITY_EDITOR
                    if(AIDecisionInterface.MethodNames.TryGetValue(conditionNode.Condition,out var name))
                    {
                        sb?.Append(name);
                    }
                    else
                    {
                        sb?.Append(conditionNode.Condition);
                    }
#endif
                    if (func(knowledge))
                    {
#if UNITY_EDITOR
                        sb?.AppendLine(" true");
#endif
                        Handler(knowledge, decision, conditionNode.True, sb);
                    }
                    else
                    {
#if UNITY_EDITOR
                        sb?.AppendLine(" false");
#endif
                        Handler(knowledge, decision, conditionNode.False, sb);
                    }
                }
                else
                {
                    Log.Error("AI行为树未找到指定方法" + conditionNode.Condition);
                }
            }
            else if (tree is DecisionCompareNode compareNode)
            {
                var left = compareNode.LeftValue?.Resolve(knowledge)??0;
                var right = compareNode.RightValue?.Resolve(knowledge)??0;
#if UNITY_EDITOR
                sb?.Append(compareNode.LeftValue?.GetType().Name + " " + compareNode.CompareMode + " " +
                           compareNode.RightValue?.GetType().Name);
#endif
                if (IsMatch(left,right,compareNode.CompareMode))
                {
#if UNITY_EDITOR
                    sb?.AppendLine(" true");
#endif
                    Handler(knowledge, decision, compareNode.True, sb);
                }
                else
                {
#if UNITY_EDITOR
                    sb?.AppendLine(" false");
#endif
                    Handler(knowledge, decision, compareNode.False, sb);
                }
            }
            else
            {
                Log.Error("AI行为树未配置节点\r\n"+sb);
            }
        }

        private static bool IsMatch(float l,float r, CompareMode mode)
        {
            switch (mode)
            {
                case CompareMode.Equal:
                    return l == r;
                case CompareMode.NotEqual:
                    return l != r;
                case CompareMode.Greater:
                    return l > r;
                case CompareMode.Less:
                    return l < r;
                case CompareMode.LEqual:
                    return l <= r;
                case CompareMode.GEqual:
                    return l >= r;
            }
            return false;
        }
        #endregion
    }
}