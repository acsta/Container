using System.Collections.Generic;

namespace TaoTie
{
    public partial class RestaurantConfigCategory
    {
        private Dictionary<int, RestaurantConfig> lvMap;
        private int maxLv;
        public override void AfterEndInit()
        {
            base.AfterEndInit();
            maxLv = 0;
            lvMap = new Dictionary<int, RestaurantConfig>();
            foreach (var item in list)
            {
                lvMap.Add(item.Level,item);
                if (item.Level > maxLv)
                {
                    item.Level = item.Level;
                }
            }
        }

        /// <summary>
        /// 通过等级取建筑信息
        /// </summary>
        /// <param name="lv"></param>
        /// <param name="next">下一等级的建筑信息</param>
        /// <returns></returns>
        public RestaurantConfig GetByLv(int lv, out RestaurantConfig next)
        {
            next = null;
            RestaurantConfig res = null;
            lvMap.TryGetValue(lv, out res);
            lvMap.TryGetValue(lv+1, out next);
            return res;
        }
    }
    
    public partial class RestaurantConfig: II18NConfig
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