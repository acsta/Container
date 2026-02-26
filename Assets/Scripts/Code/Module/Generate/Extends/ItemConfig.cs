namespace TaoTie
{
    public partial class ItemConfig: II18NSwitchConfig
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