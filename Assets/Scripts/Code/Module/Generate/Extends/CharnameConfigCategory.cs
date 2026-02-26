using UnityEngine;

namespace TaoTie
{
    public partial class CharnameConfigCategory
    {

        public CharnameConfig RandomItem()
        {
            return list[Random.Range(0, list.Count)];
        } 
    }
    
    public partial class CharnameConfig: II18NConfig
    {
        public string GetI18NText(LangType lang)
        {
            switch (lang)
            {
                case LangType.Chinese:
                    return CharacternameCHS;
                case LangType.English:
                    return CharacternameENG;
            }

            return CharacternameCHS;
        }
    }
}