using System.Collections.Generic;

namespace TaoTie
{
    public partial class ClothConfigCategory
    {
        private readonly List<ClothConfig> empty = new List<ClothConfig>();
        private MultiMap<int, ClothConfig> map;
        private Dictionary<int, int> weight;
        private int totalWeight = -1;
        public int TotalWeight
        {
            get
            {
                if (totalWeight >= 0)
                {
                    return totalWeight;
                }

                totalWeight = 0;
                for (int i = 0; i < list.Count; i++)
                {
                    var moduleConfig = CharacterConfigCategory.Instance.Get(list[i].Module);
                    if (moduleConfig.DefaultCloth != list[i].Id)
                    {
                        totalWeight += list[i].Weight;
                        if (!weight.ContainsKey(list[i].Module)) weight[list[i].Module] = 0;
                        weight[list[i].Module] += list[i].Weight;
                    }
                    else
                    {
                        list[i].Weight = 0;
                    }
                }

                return totalWeight;
            }
        }
        public override void AfterEndInit()
        {
            base.AfterEndInit();
            map = new MultiMap<int, ClothConfig>();
            weight = new Dictionary<int, int>();
            for (int i = 0; i < list.Count; i++)
            {
                map.Add(list[i].Module, list[i]);
            }
        }

        public List<ClothConfig> GetModule(int moduleId)
        {
            if (map.TryGetValue(moduleId, out var res))
            {
                return res;
            }
            return empty;
        }

        public int GetTotalWeight(int moduleId)
        {
            if (totalWeight < 0)
            {
                _ = TotalWeight;
            }

            weight.TryGetValue(moduleId, out int res);
            return res;
        }
    }
}