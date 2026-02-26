using System.Linq;
using UnityEngine;

namespace TaoTie
{
    public class NPC: Character,IEntity
    {
        public override EntityType Type => EntityType.Npc;
        public int[] SubModule => subModule;
        
        #region IEntity

        public override void Init()
        {
            Name = I18NManager.Instance.I18NGetText(CharnameConfigCategory.Instance.RandomItem());
            subModule = ClothGenerateManager.Instance.GetNext();
            var cac = AddComponent<CasualActionComponent>();
            cac.SetEnable(true);
            base.Init();
        }

        #endregion
    }
}