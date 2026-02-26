using UnityEngine;

namespace TaoTie
{
    public class UIAppraisalItem: UIBaseContainer, IOnCreate
    {
        public UIImage Icon;
        public UITextmesh Name;
        public UITextmesh TextPrice;
        public UIImage Bg;
        private int configId;
        public ItemConfig Config => ItemConfigCategory.Instance.Get(configId);
        public void OnCreate()
        {
            Bg = AddComponent<UIImage>();
            Icon = AddComponent<UIImage>("Icon");
            TextPrice = AddComponent<UITextmesh>("Bottom/TextPrice");
            Name = AddComponent<UITextmesh>("Name");
        }

        public void SetData(int cfgId, bool isTargetGameInfo, string bg)
        {
            configId = cfgId;
            var config = Config;
            Bg.SetSpritePath($"UIGame/UIMiniGame/Atlas/{bg}.png").Coroutine();
            Icon.SetSpritePath(config.ItemPic).Coroutine();
            Name.SetText(I18NManager.Instance.I18NGetText(config));
            TextPrice.SetNum(config.Price);
            if (IAuctionManager.Instance != null)//情报增加价格
            {
                var gameInfoConfig = IAuctionManager.Instance.GetFinalGameInfoConfig();
                if (gameInfoConfig != null && isTargetGameInfo)
                {
                    if (gameInfoConfig.AwardType == 0)
                    {
                        TextPrice.SetNum(config.Price + gameInfoConfig.RewardCount);
                    }
                    else if (gameInfoConfig.AwardType == 1)
                    {
                        TextPrice.SetNum(config.Price * gameInfoConfig.RewardCount);
                    }
                }
            }
 
        }
    }
}