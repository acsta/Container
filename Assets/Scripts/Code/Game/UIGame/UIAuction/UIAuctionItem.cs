using UnityEngine;

namespace TaoTie
{
    public class UIAuctionItem:UIButton,IOnCreate
    {
        private static int ShineTint = Shader.PropertyToID("_ShineTint");
        public UITextmesh TextPrice;
        public UITextmesh Name;
        public UIImage Icon;
        public UIAnimator IconAnim;
        public UIImage IconCash;
        public UIEmptyView Bottom;
        public UIPointerClick PlayType;
        public UITextmesh TextPriceAddOn;
        public UIImage TextBottomImg;
        public UIImage Hand;
        public UITextmesh PlayOne;
        public UIAnimator Animator;

        public int ConfigId;
        public BoxType BoxType;
        private bool hidePlayType;
        public ItemConfig Config => ItemConfigCategory.Instance.Get(ConfigId);
        private BigNumber price;
        #region override
        public void OnCreate()
        {
            PlayType = AddComponent<UIPointerClick>("PlayType");
            Bottom = AddComponent<UIEmptyView>("Bottom");
            TextPrice = AddComponent<UITextmesh>("Bottom/TextPrice");
            IconCash = AddComponent<UIImage>("Bottom/IconCash");
            TextPriceAddOn = AddComponent<UITextmesh>("TextBottom/TextPriceAddOn");
            TextBottomImg = AddComponent<UIImage>("TextBottom");
            Name = AddComponent<UITextmesh>("Name");
            Icon = AddComponent<UIImage>("Icon");
            PlayType.SetOnClick(OnClickPlayType);
            Hand = AddComponent<UIImage>("Hand");
            IconAnim = AddComponent<UIAnimator>("Icon");
            PlayOne = AddComponent<UITextmesh>("PlayOne");
            Animator = AddComponent<UIAnimator>("TextBottom");
        }
        
        #endregion

        /// <summary>
        /// 设置物品数据
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="hide">是否因为播动画隐藏</param>
        /// <param name="hidePlayType">是否隐藏玩法物品标识</param>
        /// <param name="boxType">物品类型</param>
        public void SetData(ItemConfig cfg, bool hide = false, bool hidePlayType = false, BoxType boxType = BoxType.Normal)
        {
            PlayOne.SetActive(false);
            price = null;
            this.hidePlayType = hidePlayType;
            this.BoxType = boxType;
            this.ConfigId = cfg.Id;
            Icon.SetSpritePath(cfg.ItemPic).Coroutine();
            
            Name.SetText(I18NManager.Instance.I18NGetText(cfg));
            if (boxType == BoxType.Task)
            {
                TextPrice.SetI18NKey(I18NKey.Text_Task_Item);
            }
            else
            {
                TextPrice.SetNum(cfg.Price);
            }
            

            Name.SetActive(!hide);
            Bottom.SetActive(!hide);
            IconCash.SetActive(BoxType == BoxType.Normal);
           
            if (cfg.Type != (int) ItemType.None && cfg.Type % 10 == 0 && PlayTypeConfigCategory.Instance.Contain(cfg.Type))
            {
                var playType = PlayTypeConfigCategory.Instance.Get(cfg.Type);
                Icon.SetMaterialPath(GameConst.PlayTypeMat);
                if (ColorUtility.TryParseHtmlString(playType.FresnelTint, out var mColor))
                {
                    Icon.GetMaterial().SetColor(ShineTint, mColor);
                }
                
                Name.SetTextColor(GameConst.PlayableItemColor);
                TextPrice.SetTextColor(GameConst.PlayableItemColor);
            }
            else if (BoxType == BoxType.Task)
            {
                Name.SetTextColor(GameConst.TaskItemColor);
                TextPrice.SetTextColor(GameConst.TaskItemColor);
                Icon.SetMaterialPath(GameConst.TaskMat);
            }
            else
            {
                Name.SetTextColor(GameConst.WHITE_COLOR);
                TextPrice.SetTextColor(GameConst.ItemPriceColor);
                Icon.SetMaterialPath(null);
            }

            bool showPlayType = !hide && !hidePlayType
                                      && cfg.Type != (int) ItemType.None
                                      && cfg.Type != (int) ItemType.Const
                                      && cfg.Type != (int) ItemType.Container
                                      && cfg.Type != (int) ItemType.Story;
            PlayType.SetActive(showPlayType);
            
            bool show = showPlayType && IAuctionManager.Instance != null && price == null &&
                        IAuctionManager.Instance.LastAuctionPlayerId == IAuctionManager.Instance.Player.Id;
            Hand.SetActive(show);
            IconAnim.SetEnable(show);
            IconAnim.GetRectTransform().localScale = Vector3.one;
        }

        public void OnClickPlayType()
        {
            if(IAuctionManager.Instance.LastAuctionPlayerId != IAuctionManager.Instance.Player.Id) return;
            if(!PlayTypeConfigCategory.Instance.Contain(Config.Type)) return;
            
            if (price != null) return;
            PlayTypeConfig playTypeConfig = PlayTypeConfigCategory.Instance.Get(Config.Type);
            if (playTypeConfig?.ResultType == 0) return;
            if (playTypeConfig?.IsEffected != 0 && IAuctionManager.Instance.AState != AuctionState.Over)
            {
                UIToast.ShowToast(I18NKey.Text_Tips_MiniPlay_Lock);
                return;
            }
            ShowPlayView(Config);
        }

        public void SetChangeItemResult(int newId, bool isAI)
        {
            SetChangeItemResultAsync(newId, isAI).Coroutine();
        }

        private async ETTask SetChangeItemResultAsync(int newId, bool isAI)
        {
            this.ConfigId = newId;
            Icon.SetSpritePath(Config.ItemPic).Coroutine();
            
            Name.SetText(I18NManager.Instance.I18NGetText(Config));
            
            if(isAI) await PlayOneAnimate();
           
            var price = Config.Price;
            if (IAuctionManager.Instance != null)//情报增加价格
            {
                var gameInfoConfig = IAuctionManager.Instance.GetFinalGameInfoConfig();
                //玩法情报要玩过后才加钱
                if (gameInfoConfig != null && gameInfoConfig.IsTargetItem(Config))
                {
                    if (gameInfoConfig.AwardType == 0)
                    {
                        price = price + gameInfoConfig.RewardCount;
                    }
                    else if (gameInfoConfig.AwardType == 1)
                    {
                        price = price * gameInfoConfig.RewardCount;
                    }
                }
            }
            TextPrice.DoNum(price).Coroutine();
            Hand.SetActive(false);
            IconAnim.SetEnable(false);
            IconAnim.GetRectTransform().localScale = Vector3.one;
        }

        public void SetChangePriceResult(BigNumber value)
        {
            price = value;
            TextPrice.DoNum(price).Coroutine();
            if (price < 0)
            {
                TextPrice.SetTextColor(GameConst.LOSS_COLOR);
            }
            Hand.SetActive(false);
            IconAnim.SetEnable(false);
            IconAnim.GetRectTransform().localScale = Vector3.one;
        }

        public void IntenItem()
        {
            Name.SetActive(true);
            Bottom.SetActive(true);
            IconCash.SetActive(BoxType == BoxType.Normal);
            if (IAuctionManager.Instance != null && BoxType == BoxType.Normal)//情报增加价格
            {
                var gameInfoConfig = IAuctionManager.Instance.GetFinalGameInfoConfig();
                //玩法情报要玩过后才加钱
                if (gameInfoConfig != null && gameInfoConfig.Type != (int)GameInfoTargetType.PlayType && gameInfoConfig.IsTargetItem(Config))
                {
                    var isMul = false;
                    BigNumber price = Config.Price;
                    if (gameInfoConfig.AwardType == 0)
                    {
                        TextPriceAddOn.SetText(
                            $"+{I18NManager.Instance.TranslateMoneyToStr(gameInfoConfig.RewardCount)}");
                        //TextPrice.SetNum(cfg.Price + gameInfoConfig.RewardCount);
                    }
                    else if (gameInfoConfig.AwardType == 1)
                    {
                        TextPriceAddOn.SetText(
                            $"x{I18NManager.Instance.TranslateMoneyToStr(gameInfoConfig.RewardCount)}");
                        //TextPrice.SetNum(cfg.Price * gameInfoConfig.RewardCount);
                        isMul = true;
                    }
                   
                    Animate(price, gameInfoConfig.RewardCount, isMul).Coroutine();
                    
                    TextPriceAddOn.SetActive(true);
                    TextBottomImg.SetActive(true);
                }
                else
                {
                    TextPriceAddOn.SetActive(false);
                    TextBottomImg.SetActive(false);
                }
            }
            else
            {
                TextPriceAddOn.SetActive(false);
                TextBottomImg.SetActive(false);
            }

            var cfg = Config;
            bool showPlayType = !hidePlayType
                                && cfg.Type != (int) ItemType.None
                                && cfg.Type != (int) ItemType.Const
                                && cfg.Type != (int) ItemType.Container
                                && cfg.Type != (int) ItemType.Story;
            PlayType.SetActive(showPlayType);
            PlayTypeConfig playTypeConfig = null;
            if (showPlayType && PlayTypeConfigCategory.Instance.Contain(cfg.Type))
            {
                playTypeConfig = PlayTypeConfigCategory.Instance.Get(cfg.Type);
            }
            bool show = showPlayType && IAuctionManager.Instance != null  && price == null &&
                        IAuctionManager.Instance.LastAuctionPlayerId == IAuctionManager.Instance.Player.Id;
            Hand.SetActive(show);
            IconAnim.SetEnable(show);
            IconAnim.GetRectTransform().localScale = Vector3.one;
            if (show && playTypeConfig?.ResultType == 2 && !IAuctionManager.Instance.IsAllPlayBox)
            {
                ShowPlayView(cfg);
            }

            if (IAuctionManager.Instance != null && 
                IAuctionManager.Instance.LastAuctionPlayerId == IAuctionManager.Instance.Player.Id && 
                cfg.Type == (int) ItemType.Story)
            {
                ShowStory(cfg);
            }
        }

        private void ShowPlayView(ItemConfig cfg)
        {
            AuctionHelper.ShowPlayView(cfg);
        }

        private void ShowStory(ItemConfig cfg)
        {
            UIManager.Instance.OpenBox<UIItemStoryWin, int, UIAuctionItem>(UIItemStoryWin.PrefabPath, cfg.Id, this, 
                    UILayerNames.NormalLayer).Coroutine();
        }

        private async ETTask Animate(BigNumber val, BigNumber mul, bool isMul)
        {
            var animTime = 50f;
            for (int i = 0; i < 6; ++i)
            {
                var startTime = TimerManager.Instance.GetTimeNow();
                var originText = isMul ? val * Mathf.Lerp(1, mul, (float)i / 6) : val + Mathf.Lerp(0, mul, (float)i / 6);
                var targetText = isMul ? val * Mathf.Lerp(1, mul, (float)(i + 1) / 6) : val + Mathf.Lerp(0, mul, (float)(i + 1) / 6);
                var biggest = this.TextPrice.GetRectTransform().localScale * 1.2f;
                var isNeg = false;
                
                while (true) 
                {
                    await TimerManager.Instance.WaitAsync(1);
                    if(!ActiveSelf) return;
                    var timeNow = TimerManager.Instance.GetTimeNow();
                    bool isBigger = timeNow - startTime <= animTime / 2;
                    if (isBigger)
                    {
                        this.TextPrice.GetRectTransform().localScale = Vector3.Lerp(Vector3.one, biggest, (timeNow - startTime) / (animTime * 0.5f));
                    }
                    else
                    {
                        if (!isNeg)
                        {
                            isNeg = true;
                            startTime = timeNow;
                        }
                        this.TextPrice.GetRectTransform().localScale = Vector3.Lerp(biggest, Vector3.one, (timeNow - startTime) / (animTime * 0.5f));
                    }
					
                    this.TextPrice.SetText(I18NManager.Instance.TranslateMoneyToStr(originText + (targetText - originText) * Mathf.Clamp01((timeNow - startTime) / animTime)));

                    if (timeNow - startTime >= animTime)
                    {
                        break;
                    }
                }
            }
        }

        private async ETTask PlayOneAnimate()
        {
            PlayOne.SetMaxVisibleCharacters(0);
            PlayOne.SetActive(true);
            
            var animTime = 100;
            for (int i = 1; i <= PlayOne.GetText().Length; ++i)
            {
                await TimerManager.Instance.WaitAsync(animTime);
                
                PlayOne.SetMaxVisibleCharacters(i);
            }
            
            await TimerManager.Instance.WaitAsync(1000);

            var color = PlayOne.GetTextColor();
            var aTime = 100f;
            var startTime = TimerManager.Instance.GetTimeNow();
            while (true)
            {
                await TimerManager.Instance.WaitAsync(1);
                
                var timeNow = TimerManager.Instance.GetTimeNow();
                PlayOne.SetTextColor(new Color(color.r, color.g, color.b, Mathf.Lerp(1, 0, Mathf.Clamp((timeNow - startTime) / aTime, 0, 1))));
                if (timeNow - startTime >= aTime)
                {
                    break;
                }
            }
        }
    }
}