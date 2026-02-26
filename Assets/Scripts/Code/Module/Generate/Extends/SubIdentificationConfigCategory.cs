using System;
using System.Reflection;

namespace TaoTie
{
    public partial class SubIdentificationConfigCategory
    {
        private const int Len = 10;
        private PropertyInfo[] Results;
        private PropertyInfo[] Widgets;
        private PropertyInfo[] AIWidgets;
        private PropertyInfo[] Rares;
        public override void AfterEndInit()
        {
            base.AfterEndInit();
            var type = TypeInfo<SubIdentificationConfig>.Type;
            Results = new PropertyInfo[Len];
            Widgets = new PropertyInfo[Len];
            AIWidgets = new PropertyInfo[Len];
            Rares = new PropertyInfo[Len];
            for (int i = 1; i <= Len; i++)
            {
                Results[i - 1] = type.GetProperty("Result" + i);
                Widgets[i - 1] = type.GetProperty("Weight" + i);
                AIWidgets[i - 1] = type.GetProperty("AIWeight" + i);
                Rares[i - 1] = type.GetProperty("Rare" + i);
            }
        }

        public bool TryGet(int id,out SubIdentificationConfig config)
        {
            if (!this.dict.TryGetValue(id, out config))
            {
                return false;
            }

            if (config.Result == null)
            {
                config.TotalWidget = 0;
                
                for (int i = Len - 1; i >=0 ; i--)
                {
                    int itemId = (int)Results[i].GetValue(config);
                    if (itemId == 0) continue;
                    var item = ItemConfigCategory.Instance.Get(itemId);
                    if (config.Result == null)
                    {
                        config.Rare = new int[i + 1];
                        config.Result = new int[i + 1];
                        config.Widget = new int[i + 1];
                        config.AIWidget = new int[i + 1];
                        config.Min = item.Price;
                        config.Max = item.Price;
                    }
                    else
                    {
                        if (config.Min > item.Price) config.Min = item.Price;
                        else if (config.Max < item.Price) config.Max = item.Price;
                    }
                    config.Result[i] = itemId;
                    config.Rare[i] = (int)Rares[i].GetValue(config);
                    config.Widget[i] = (int)Widgets[i].GetValue(config);
                    config.AIWidget[i] = (int)AIWidgets[i].GetValue(config);
                    config.TotalWidget += config.Widget[i];
                    config.TotalAIWidget += config.AIWidget[i];
                }
                
                if (config.Result == null)
                {
                    config.Rare = Array.Empty<int>();
                    config.Result = Array.Empty<int>();
                    config.Widget = Array.Empty<int>();
                    config.AIWidget = Array.Empty<int>();
                }
            }

            return true;
        }
    }
    
    
    public partial class SubIdentificationConfig
    {
        public int[] Result;
        public int[] Widget;
        public int[] AIWidget;
        public int[] Rare;
        public BigNumber Max;
        public BigNumber Min;
        public int TotalWidget;
        public int TotalAIWidget;
    }
}