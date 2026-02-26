using System;
using System.Collections.Generic;

namespace TaoTie
{
    public partial class TurntableRewardsConfigCategory
    {
        public UnOrderDoubleKeyMap<int, int, TurntableRewardsConfig> maps;
        public override void AfterEndInit()
        {
            base.AfterEndInit();
            maps = new UnOrderDoubleKeyMap<int, int, TurntableRewardsConfig>();
            for (int i = 0; i < list.Count; i++)
            {
                maps.Add(list[i].Level, list[i].Lv, list[i]);
            }
        }

        public bool TryGet(int level, int lv ,out List<TurntableRewardsConfig> list)
        {
            list = null;
            if (maps.TryGetList(level, lv, out list))
            {
                return true;
            }

            if (maps.TryGetValue(level, out var dic))
            {
                int cur = int.MinValue;
                foreach (var kv in dic)
                {
                    if (kv.Key > cur && kv.Key <= lv)
                    {
                        cur = kv.Key;
                        list = kv.Value;
                    }
                }
                if (cur != int.MinValue) return true;
            }
            return false;
        }
    }
}