namespace TaoTie
{
    public class TechnologyNodeItem : UIBaseContainer, IOnCreate
    {
        public UIPointerClick PointerClick;
        public UIImage Bg;
        public UIImage Bg2;
        public UIImage Icon;
        public UITextmesh Title;
        public UITextmesh Money;
        public UIButton Coin;
        public UITextmesh UnLock;

        public TechnologyTreeConfig config;
        public void OnCreate()
        {
            PointerClick = AddComponent<UIPointerClick>();
            Bg = AddComponent<UIImage>("Bg");
            Bg2 = AddComponent<UIImage>("Bg/Bg");
            Icon = AddComponent<UIImage>("Bg/Icon");
            Title = AddComponent<UITextmesh>("Bg/Text");
            Money = AddComponent<UITextmesh>("Bg/Coin/Text");
            Coin = AddComponent<UIButton>("Bg/Coin");
            UnLock = AddComponent<UITextmesh>("Bg/Unlock");
            PointerClick.SetOnClick(OnClickSelf);
            Coin.SetOnClick(OnClickSelf);
        }

        public void SetData(TechnologyTreeConfig config, bool canUnlock)
        {
            this.config = config;
            
            Icon.SetSpritePath(config.Icon).Coroutine();
            Title.SetText(I18NManager.Instance.I18NGetText(config));
            bool isUnlock = PlayerDataManager.Instance.IsUnlock(config.Id);
            Coin.SetActive(!isUnlock);
            UnLock.SetActive(isUnlock);
            Bg.SetImageGray(!isUnlock).Coroutine();
            Icon.SetImageGray(!isUnlock).Coroutine();
            Bg2.SetImageGray(!isUnlock).Coroutine();
            Title.SetTextGray(!isUnlock);
            if (!isUnlock)
            {
                if (config.UnlockType == 1)
                {
                    Money.SetNum(config.UnlockValue);
                }
                else
                {
                    Log.Error("解锁类型不对 TechnologyTreeConfig id=" + config.Id);
                }
            }
        }
        
        public void OnClickSelf()
        {
            UIManager.Instance.OpenWindow<UIUnlockWin, TechnologyTreeConfig>(UIUnlockWin.PrefabPath, config).Coroutine();
        }
    }
}