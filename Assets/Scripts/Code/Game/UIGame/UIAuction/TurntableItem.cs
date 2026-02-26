using UnityEngine;

namespace TaoTie
{
    public class TurntableItem: UIBaseContainer,IOnCreate
    {
        public UIImage Image;
        public UITextmesh Text;
        public void OnCreate()
        {
            Image = AddComponent<UIImage>("Icon");
            Text = AddComponent<UITextmesh>("Text");
        }

        public void SetData(TurntableRewardsConfig config)
        {
            Text.SetText(I18NManager.Instance.TranslateMoneyToStr(config.RewardCount));
            Image.SetSpritePath(config.Icon).Coroutine();
        }
    }
}