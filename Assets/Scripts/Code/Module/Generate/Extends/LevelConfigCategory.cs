using System.Collections.Generic;

namespace TaoTie
{
    public partial class LevelConfigCategory
    {
        private Dictionary<string, LevelConfig> nameMap;

        public override void AfterEndInit()
        {
            nameMap = new Dictionary<string, LevelConfig>();
            for (int i = 0; i < list.Count; i++)
            {
                if (string.IsNullOrEmpty(list[i].Name)) continue;
                if (list[i].Hide == 1) continue;
                nameMap.Add(list[i].Name, list[i]);
            }
        }
        
                
        public bool TryGetByName(string name, out LevelConfig config)
        {
            return nameMap.TryGetValue(name, out config);
        }
    }
    
    public partial class LevelConfig: II18NSwitchConfig
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