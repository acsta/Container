using System.Linq;
using UnityEngine;

namespace TaoTie
{
    public class Bidder: Character,IEntity<int>,IEntity<int,bool>
    {
        public override EntityType Type => EntityType.Bidder;
        private bool isBlack;
        #region IEntity

        public void Init(int id)
        {
            Init(id, false);
        }

        public void Init(int id, bool isBlack)
        {
            this.isBlack = isBlack;
            Name = I18NManager.Instance.I18NGetText(CharnameConfigCategory.Instance.RandomItem());
            subModule = ClothGenerateManager.Instance.GetNext();
            var bidderComponent = AddComponent<BidderComponent, int>(id);
            AddComponent<AIComponent,string>(bidderComponent.Config.AIType);
            AddComponent<CasualActionComponent, int, int>(bidderComponent.Config.ActionInterval[0],
                bidderComponent.Config.ActionInterval[1]);
            base.Init();
        }

        protected override async ETTask InitAsync()
        {
            await base.InitAsync();
            if(isBlack) AddComponent<BlackBoyComponent>();
        }

        #endregion
    }
}