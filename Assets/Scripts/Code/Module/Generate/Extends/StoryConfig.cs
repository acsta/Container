namespace TaoTie
{
    public partial class StoryConfig: II18NSwitchConfig
    {
        public const int Choose0 = 1;
        public const int Choose1 = 2;
        public const int ResultF0 = 3;
        public const int ResultS0 = 4;
        public const int ResultF1 = 5;
        public const int ResultS1 = 6;
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
            if (type == Choose0)
            {
                switch (lang)
                {
                    case LangType.Chinese:
                        return Choose0Chinese;
                    default:
                    case LangType.English:
                        return Choose0English;
                }
            }
            else if (type == Choose1)
            {
                switch (lang)
                {
                    case LangType.Chinese:
                        return Choose1Chinese;
                    default:
                    case LangType.English:
                        return Choose1English;
                }
            }
            else if (type == ResultF0)
            {
                switch (lang)
                {
                    case LangType.Chinese:
                        return ResultFail0Chinese;
                    default:
                    case LangType.English:
                        return ResultFail0English;
                }
            }
            else if (type == ResultS0)
            {
                switch (lang)
                {
                    case LangType.Chinese:
                        return ResultSucc0Chinese;
                    default:
                    case LangType.English:
                        return ResultSucc0English;
                }
            }
            else if (type == ResultF1)
            {
                switch (lang)
                {
                    case LangType.Chinese:
                        return ResultFail1Chinese;
                    default:
                    case LangType.English:
                        return ResultFail1English;
                }
            }
            else if (type == ResultS1)
            {
                switch (lang)
                {
                    case LangType.Chinese:
                        return ResultSucc1Chinese;
                    default:
                    case LangType.English:
                        return ResultSucc1English;
                }
            }
            return GetI18NText(lang);
        }
    }
}