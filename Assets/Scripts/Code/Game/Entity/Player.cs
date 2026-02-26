using System.Collections.Generic;
using UnityEngine;

namespace TaoTie
{
    public class Player: Character,IEntity<int[]>
    {
        public override EntityType Type => EntityType.Player;
        
        public int[] SubModule => subModule;

        #region IEntity

        public void Init(int[] modules)
        {
            Name = "我";
            if (modules == null)
            {
                var configs = CharacterConfigCategory.Instance.GetAllList();
                modules = new int[configs.Count];
                for (int i = 0; i < configs.Count; i++)
                {
                    if (configs[i].DefaultCloth != 0)
                    {
                        modules[i] = configs[i].DefaultCloth;
                    }
                }
            }
            subModule = modules;
            base.Init();
        }

        #endregion
    }
}