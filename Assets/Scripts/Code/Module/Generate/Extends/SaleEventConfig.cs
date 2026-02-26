namespace TaoTie
{
    public partial class SaleEventConfig: II18NConfig
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
}