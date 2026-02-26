namespace TaoTie
{
    public partial class GoodsCheckConfig: II18NSwitchConfig
    {
        public string GetI18NText(LangType lang)
        {
            switch (lang)
            {
                case LangType.Chinese:
                    return QuestionChinese;
                default:
                case LangType.English:
                    return QuestionEnglish;
            }
        }

        public string GetI18NText(LangType lang, int type = 0)
        {
            if (type == 1)
            {
                switch (lang)
                {
                    case LangType.Chinese:
                        return Ans0Chinese;
                    default:
                    case LangType.English:
                        return Ans0English;
                }
            }
            if (type == 2)
            {
                switch (lang)
                {
                    case LangType.Chinese:
                        return Ans1Chinese;
                    default:
                    case LangType.English:
                        return Ans1English;
                }
            }
            return GetI18NText(lang);
        }
    }
}