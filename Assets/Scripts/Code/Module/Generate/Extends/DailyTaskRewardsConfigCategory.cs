using System.Collections.Generic;

namespace TaoTie
{
    public partial class DailyTaskRewardsConfigCategory
    {
        private MultiMap<int, DailyTaskRewardsConfig> lvRewards;
        private int max;
        public override void AfterEndInit()
        {
            base.AfterEndInit();
            max = int.MinValue;
            lvRewards = new MultiMap<int, DailyTaskRewardsConfig>();
            for (int i = 0; i < list.Count; i++)
            {
                lvRewards.Add(list[i].Lv,list[i]);
                if (list[i].Lv > max)
                {
                    max = list[i].Lv;
                }
            }
        }

        public List<DailyTaskRewardsConfig> GetRewards(int lv)
        {
            if (lvRewards.TryGetValue(lv, out var res))
            {
                return res;
            }

            return lvRewards[max];
        }
    }
}