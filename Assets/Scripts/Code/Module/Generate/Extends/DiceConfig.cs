using System.Collections.Generic;

namespace TaoTie
{
    public partial class DiceConfigCategory
    {
        private UnOrderMultiMap<int, DiceConfig> lvMapDice;
        public override void AfterEndInit()
        {
            base.AfterEndInit();
            lvMapDice = new UnOrderMultiMap<int, DiceConfig>();
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < list[i].Level.Length; j++)
                {
                    lvMapDice.Add(list[i].Level[j], list[i]);
                }
            }
        }

        public bool TryGetDiceConfig(int lv,out List<DiceConfig> list)
        {
            return lvMapDice.TryGetValue(lv, out list);
        }
    }
    public partial class DiceConfig: II18NSwitchConfig
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

        public string GetI18NText(LangType lang, int type = 0)
        {
            if (type == 1)
            {
                switch (lang)
                {
                    case LangType.Chinese:
                        return ChineseDesc;
                    default:
                    case LangType.English:
                        return EnglishDesc;
                }
            }
            return GetI18NText(lang);
        }
    }
}