using System.Collections.Generic;

namespace TaoTie
{
    public class RedDotManager: IManager
    {
        public static RedDotManager Instance;
        private UnOrderMultiMap<string, string> redDotNodeParentsDict = new UnOrderMultiMap<string, string>();
        private Dictionary<string, int> retainViewCount = new Dictionary<string, int>();
        private Dictionary<string, string> toParentDict = new Dictionary<string, string>();
        private MultiMap<string, UIRedDot> redDotMonoViewDict = new MultiMap<string, UIRedDot>();
        #region IManager

        public void Init()
        {
            Instance = this;
            foreach (var item in RedDotConfigCategory.Instance.GetAll())
            {
                AddRodDotNode(item.Value.Parent,item.Value.Target);
            }

            foreach (var level in TechnologyTreeConfigCategory.Instance.GetLevels())
            {
                AddRodDotNode("Black", "Black_" + level.Id);
                AddRodDotNode("Black_" + level.Id, "Black_Tags_" + level.Id);
                AddRodDotNode("Black_" + level.Id, "Black_Tech_" + level.Id);
            }

            foreach (var item in ItemConfigCategory.Instance.GetAllList())
            {
                if (item.Type == (int) ItemType.Const)
                {
                    AddRodDotNode("Null", "Item_"+item.Id);
                }
            }
        }

        public void Destroy()
        {
            redDotNodeParentsDict.Clear();
            toParentDict.Clear();
            redDotMonoViewDict.Clear();
            retainViewCount.Clear();
            Instance = null;
        }

        #endregion
        
        /// <summary>
        /// 创建树————添加节点
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="target"></param>
        private void AddRodDotNode(string parent, string target)
        {
            if (string.IsNullOrEmpty(target))
            {
                Log.Error($"target is null");
                return;
            }
            
            if (string.IsNullOrEmpty(parent))
            {
                Log.Error($"parent is null");
                return;
            }

            if (toParentDict.ContainsKey(target))
            {
                Log.Error($"{target} is already exist!");
                return;
            }

            toParentDict.Add(target, parent);

            if (!retainViewCount.ContainsKey(target))
            {
                retainViewCount.Add(target, 0);
            }
            
            redDotNodeParentsDict.Add(parent, target);
        }
        
        /// <summary>
        /// 创建树————移除节点
        /// </summary>
        /// <param name="target"></param>
        private void RemoveRedDotNode(string target)
        {
            if (!toParentDict.TryGetValue(target, out string parent))
            {
                return ;
            }

            if (!IsLeafNode(target))
            {
                Log.Error("can not remove parent node!");
                return ;
            }
            
            toParentDict.Remove(target);
            if (!string.IsNullOrEmpty(parent))
            {
                redDotNodeParentsDict[parent].Remove(target);
            }
        }

        /// <summary>
        /// 添加UI红点
        /// </summary>
        /// <param name="target"></param>
        /// <param name="uiRedDotComponent"></param>
        public void AddUIRedDotComponent(string target, UIRedDot uiRedDotComponent)
        {
            redDotMonoViewDict.Add(target,uiRedDotComponent);
        }
        /// <summary>
        /// 移除UI红点
        /// </summary>
        /// <param name="target"></param>
        /// <param name="uiRedDotComponent"></param>
        public void RemoveUIRedDotComponent(string target, UIRedDot uiRedDotComponent)
        {
            redDotMonoViewDict.Remove(target, uiRedDotComponent);
        }
        /// <summary>
        /// 是否是子节点
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private bool IsLeafNode(string target)
        {
            return !redDotNodeParentsDict.ContainsKey(target);
        }

        public int GetRedDotViewCount(string target)
        {
            if (retainViewCount.TryGetValue(target, out var count))
            {
                return count;
            }

            return 0;
        }
        /// <summary>
        /// 刷新红点数量
        /// </summary>
        /// <param name="target"></param>
        /// <param name="count"></param>
        public void RefreshRedDotViewCount(string target, int count)
        {
            if (!IsLeafNode(target))
            {
                Log.Error("can not refresh parent node view count");
                return;
            }
            
            retainViewCount[target] = count;
            if (redDotMonoViewDict.TryGetValue(target, out var comps))
            {
                for (int i = 0; i < comps.Count; i++)
                {
                    comps[i].RefreshRedDot();
                }
            }

            bool isParentExist = toParentDict.TryGetValue(target, out string parent);

            while (isParentExist)
            {
                var viewCount = 0;
                
                foreach (var childNode in redDotNodeParentsDict[parent])
                {
                    viewCount += retainViewCount[childNode];
                }

                retainViewCount[parent] = viewCount;
                if (redDotMonoViewDict.TryGetValue(parent, out comps))
                {
                    for (int i = 0; i < comps.Count; i++)
                    {
                        comps[i].RefreshRedDot();
                    }
                }

                isParentExist = toParentDict.TryGetValue(parent, out parent);
            }
        }
    }
}