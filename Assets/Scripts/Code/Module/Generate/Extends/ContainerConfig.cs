namespace TaoTie
{
    public partial class ContainerConfig: II18NConfig
    {
        private ItemConfig itemConfig;
        /// <summary>
        /// 转为Item数据格式
        /// </summary>
        public ItemConfig ItemConfig
        {
            get
            {
                if (itemConfig == null)
                {
                    itemConfig = new ItemConfig()
                    {
                        Id = Id,
                        Type = (int) ItemType.Container,
                        ItemPic = Icon,
                        Chinese = Chinese,
                        English = English,
                        ContainerId = Id,
                    };
                }
                return itemConfig;
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