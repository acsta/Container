using System.Collections.Generic;
using UnityEngine;

namespace TaoTie
{
    public partial class TaskConfigCategory
    {
        private MultiMap<int, TaskConfig> dailyTask;//对应等级每日任务

        private List<TaskConfig> empty = new List<TaskConfig>();
        public override void AfterEndInit()
        {
            base.AfterEndInit();

            dailyTask = new MultiMap<int, TaskConfig>();
            for (int i = 0; i < list.Count; i++)
            {
                dailyTask.Add(list[i].Lv,list[i]);
            }
        }

        /// <summary>
        /// 获取可随机的每日任务
        /// </summary>
        /// <param name="lv"></param>
        /// <returns></returns>
        public List<TaskConfig> GetDailyTask(int lv)
        {
            if (dailyTask.TryGetValue(lv, out var res))
            {
                return res;
            }

            return empty;
        }
    }
    
    public partial class TaskConfig: II18NSwitchConfig
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