namespace TaoTie
{
    public partial class ClothConfig: II18NConfig
    {
        public ItemConfig ItemConfig
        {
            get
            {
                return new ItemConfig()
                {
                    Id = Id,
                    ItemPic = Icon,
                    Chinese = Chinese,
                    English = English,
                    Type = 1,
                    ContainerId = 1001,
                };
            }
        }
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
}