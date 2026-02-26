using System.Collections.Generic;

namespace TaoTie
{
    public partial class TechnologyTreeConfigCategory
    {
        private static List<TechnologyTreeConfig> empty = new List<TechnologyTreeConfig>();
        private List<TechnologyTreeConfig> roots;
        private Dictionary<int, TechnologyTreeConfig> levelMapRoot;
        private MultiMap<int,TechnologyTreeConfig> allcontainers;
        private MultiMap<int,TechnologyTreeConfig> allplayType;
        private MultiMap<int,TechnologyTreeConfig> containers;
        private MultiMap<int,TechnologyTreeConfig> playType;
        public override void AfterEndInit()
        {
            base.AfterEndInit();
            roots = new List<TechnologyTreeConfig>();
            levelMapRoot = new Dictionary<int, TechnologyTreeConfig>();
            containers = new MultiMap<int, TechnologyTreeConfig>();
            playType = new MultiMap<int, TechnologyTreeConfig>();
            allcontainers = new MultiMap<int, TechnologyTreeConfig>();
            allplayType = new MultiMap<int, TechnologyTreeConfig>();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Level == 0)
                {
                    if(list[i].HideUI == 0) roots.Add(list[i]);
                    levelMapRoot.Add(list[i].Content, list[i]);
                }
                else if (list[i].Level == 1)
                {
                    if(list[i].HideUI == 0) containers.Add(list[i].ParentId, list[i]);
                    allcontainers.Add(list[i].ParentId, list[i]);
                }
                else if (list[i].Level == 2)
                {
                    if(list[i].HideUI == 0) playType.Add(list[i].ParentId, list[i]);
                    allplayType.Add(list[i].ParentId, list[i]);
                }
            }
        }
        /// <summary>
        /// 获取场次类型
        /// </summary>
        /// <returns></returns>
        public List<TechnologyTreeConfig> GetLevels()
        {
            return roots;
        }


        /// <summary>
        /// 获取集装箱科技树列表
        /// </summary>
        /// <param name="levelId"></param>
        /// <param name="showHideUI">是否返回隐藏UI的</param>
        /// <returns></returns>
        public List<TechnologyTreeConfig> GetContainers(int levelId, bool showHideUI = true)
        {
            if (!levelMapRoot.TryGetValue(levelId, out var target))
            {
                return empty;
            }
            
            if (showHideUI && allcontainers.TryGetValue(target.Id, out var res))
            {
                return res;
            }
            if (!showHideUI && containers.TryGetValue(target.Id, out res))
            {
                return res;
            }
            return empty;
        }
        
        /// <summary>
        /// 获取集装箱玩法科技树列表
        /// </summary>
        /// <param name="containerId"></param>
        /// <param name="showHideUI">是否返回隐藏UI的</param>
        /// <returns></returns>
        public List<TechnologyTreeConfig> GetPlayTypes(int containerId, bool showHideUI = true)
        {
            if (showHideUI && allplayType.TryGetValue(containerId, out var res))
            {
                return res;
            }
            if (!showHideUI && playType.TryGetValue(containerId, out res))
            {
                return res;
            }
            return empty;
        }
    }
    
    public partial class TechnologyTreeConfig: II18NSwitchConfig
    {
        public int ParentId => Id / 1000;
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