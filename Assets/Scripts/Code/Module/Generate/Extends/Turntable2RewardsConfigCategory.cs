using System;
using System.Collections.Generic;

namespace TaoTie
{
    public partial class Turntable2RewardsConfig :II18NConfig 
    {
        public string GetI18NText(LangType lang)
        {
            switch (lang)
            {
                case LangType.Chinese:
                    return Chinese;
                default:
                case LangType.English:
                    return English;
            }
        }
    }
    public partial class Turntable2RewardsConfigCategory
    {
        public UnOrderDoubleKeyMap<int, int, Turntable2RewardsConfig> maps;
        public override void AfterEndInit()
        {
            base.AfterEndInit();
            maps = new UnOrderDoubleKeyMap<int, int, Turntable2RewardsConfig>();
            for (int i = 0; i < list.Count; i++)
            {
                maps.Add(list[i].Level, list[i].Lv, list[i]);
            }
        }

        public bool TryGet(int level, int lv ,out List<Turntable2RewardsConfig> list)
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