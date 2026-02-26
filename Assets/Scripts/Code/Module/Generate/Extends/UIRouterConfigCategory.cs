using System.Linq;

namespace TaoTie
{
    public partial class UIRouterConfigCategory
    {
        private UnOrderDoubleKeyDictionary<string, string, UIRouterConfig> routerMapPath;

        public override void AfterEndInit()
        {
            base.AfterEndInit();
            routerMapPath = new UnOrderDoubleKeyDictionary<string, string, UIRouterConfig>();
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                routerMapPath.Add(item.From,item.To,item);
            }
        }
        
        /// <summary>
        /// 获取到目标界面最短路径,广度优先
        /// </summary>
        /// <param name="from"></param>
        /// <param name="aim"></param>
        /// <returns></returns>
        public UIRouterConfig GetNextWay(string from, string aim)
        {
            HashSetComponent<string> overList = HashSetComponent<string>.Create();
            ListComponent<string> ways1 = ListComponent<string>.Create();
            ListComponent<string> ways2 = ListComponent<string>.Create();
            ListComponent<string> route1 = ListComponent<string>.Create();
            ListComponent<string> route2 = ListComponent<string>.Create();
            DictionaryComponent<string,UIRouterConfig> temp = DictionaryComponent<string, UIRouterConfig>.Create();
            bool isFirst = true;
            ways1.Add(from);
            route1.Add("");
            overList.Add(from);
            while (ways1.Count>0)
            {
                ways2.Clear();
                route2.Clear();
                for (int i = 0; i < ways1.Count; i++)
                {
                    if (routerMapPath.TryGetDic(ways1[i], out var dic))
                    {
                        foreach (var item in dic)
                        {
                            if (overList.Contains(item.Key)) continue;
                            var key = "";
                            if (isFirst)
                            {
                                key = item.Key;
                                temp.Add(key,item.Value);
                            }
                            else
                            {
                                key = route1[i] + item.Key;
                                if (temp.ContainsKey(route1[i]))
                                {
                                    temp.Add(key, temp[route1[i]]);
                                }
                            }
                            if (item.Key != aim)
                            {
                                if (!overList.Contains(item.Key))
                                {
                                    overList.Add(item.Key);
                                    ways2.Add(item.Key);
                                    route2.Add(key);
                                }
                            }
                            else
                            {
                                overList.Dispose();
                                ways1.Dispose();
                                ways2.Dispose();
                                route1.Dispose();
                                route2.Dispose();
                                var res = temp[key];
                                temp.Dispose();
                                return res;
                            }
                            
                        }
                    }
                }
                (ways2, ways1) = (ways1, ways2);
                (route2, route1) = (route1, route2);
                isFirst = false;
            }
            ways1.Dispose();
            ways2.Dispose();
            route1.Dispose();
            route2.Dispose();
            temp.Dispose();
            overList.Dispose();
            return null;
            
        }
    }
}