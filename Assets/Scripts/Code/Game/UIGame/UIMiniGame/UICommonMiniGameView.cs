using System;
using UnityEngine;

namespace TaoTie
{
    public class UICommonMiniGameView: UIBaseView, IOnCreate,IOnEnable<int>,IOnWidthPaddingChange
    {
        public UIAnimator UICommonView;
        public UIButton Back;
        public UIButton Share;
        public UITextmesh Range;
        public UIImage ContainerIcon;
        public UIImage WinLossIcon;
        public UITextmesh WinLossText;
        public UIAnimator WinLoss;
        public UITextmesh Title;
        public UITextmesh Result;
        public UIImage ResultGroup;
        protected int configId { get; private set; }
        protected int seeAdCount;
        public ItemConfig ItemConfig => ItemConfigCategory.Instance.Get(configId);

        protected BigNumber containerWinLoss;
        private bool isClose;
        public virtual void OnCreate()
        {
            ResultGroup = AddComponent<UIImage>("View/Bg/Content/Result");
            Title = AddComponent<UITextmesh>("View/Bg/Content/Result/Title");
            Result = AddComponent<UITextmesh>("View/Bg/Content/Result/Result");
            ContainerIcon = AddComponent<UIImage>("View/Bg/Content/WinLoss/Icon");
            WinLossIcon = AddComponent<UIImage>("View/Bg/Content/WinLoss/Result");
            WinLossText = AddComponent<UITextmesh>("View/Bg/Content/WinLoss/Result/Text");
            WinLoss = AddComponent<UIAnimator>("View/Bg/Content/WinLoss");
            UICommonView = AddComponent<UIAnimator>("View");
            this.Back = this.AddComponent<UIButton>("View/Bg/Close");
            this.Share = this.AddComponent<UIButton>("View/Bg/Content/Share");
            Range = this.AddComponent<UITextmesh>("View/Bg/Content/Range/Text");
            Range.SetI18NKey(I18NKey.Appraisal_Price_Range);
        }

        public virtual void OnEnable(int id)
        {
            OnEnableAsync().Coroutine();
            isClose = false;
            seeAdCount = 0;
            WinLoss.SetActive(false);
            configId = id;
            this.Back.SetOnClick(OnClickBack);
            this.Share.SetOnClick(OnClickShare);
            
            Share.SetActive(false);
            var container = ContainerConfigCategory.Instance.Get(ItemConfig.ContainerId);
            ContainerIcon.SetSpritePath(container.Icon).Coroutine();
            
            containerWinLoss = 0;
            var data = IAuctionManager.Instance.Boxes;
            var gameInfoConfig = IAuctionManager.Instance.GetFinalGameInfoConfig();
            for (int i = 0; i < data.Count; i++)
            {
                var box = EntityManager.Instance.Get<Box>(data[i]);
                containerWinLoss += box.GetFinalPrice(gameInfoConfig);
            }

            var value = containerWinLoss - IAuctionManager.Instance.LastAuctionPrice;
            if (value >= 0)
            {
                WinLossText.SetI18NKey(I18NKey.Text_Game_Win2, value);
                WinLossIcon.SetColor(GameConst.GREEN_COLOR);
            }
            else
            {
                WinLossText.SetI18NKey(I18NKey.Text_Game_Loss2, -value);
                WinLossIcon.SetColor(GameConst.RED_COLOR);
            }
            Title.SetI18NKey(I18NKey.Text_Game_Win4);
            ResultGroup.SetColor(GameConst.GREEN_COLOR);
            Result.SetNum(0,false);
            var adCount = PlayerDataManager.Instance.GetPlayableAdCount();
            AfterPlayAd(adCount, seeAdCount);
        }

        private async ETTask OnEnableAsync()
        {
            var mainCamera = CameraManager.Instance.MainCamera();
            if (mainCamera == null) return;
            await UICommonView.Play("UIView_Open");
            CameraManager.Instance.MainCamera().cullingMask = Define.UILayer;
        }

        public void OnClickBack()
        {
            if(isClose) return;
            isClose = true;
            OnClickCloseAsync().Coroutine();
        }
        public async ETTask OnClickCloseAsync()
        {
            var mainCamera = CameraManager.Instance.MainCamera();
            if (mainCamera != null)
            {
                mainCamera.cullingMask = Define.AllLayer;
            }
            await UICommonView.Play("UIView_Close");
            CloseSelf().Coroutine();
        }
        
        public void OnClickShare()
        {

        }

        public void SetContainerWinLoss(BigNumber value)
        {
            WinLoss.SetActive(IAuctionManager.Instance.AState == AuctionState.Over);
            if (value >= 0)
            {
                WinLossText.SetI18NKey(I18NKey.Text_Game_Win2);
                WinLossText.DoI18NNum(value).Coroutine();
                WinLossIcon.SetColor(GameConst.GREEN_COLOR);
            }
            else
            {
                WinLossText.SetI18NKey(I18NKey.Text_Game_Loss2);
                WinLossText.DoI18NNum(-value).Coroutine();
                WinLossIcon.SetColor(GameConst.RED_COLOR);
            }
        }

        public void SetItemWinLoss(BigNumber val)
        {
            if (val<0)
            {
                Title.SetI18NKey(I18NKey.Text_Game_Loss4);
                ResultGroup.SetColor(GameConst.RED_COLOR);
                Result.DoNum(val,false).Coroutine();
            }
            else
            {
                Title.SetI18NKey(I18NKey.Text_Game_Win4);
                ResultGroup.SetColor(GameConst.GREEN_COLOR);
                Result.DoNum(val,false).Coroutine();
            }
        }
        
        public void SetItemWinLossWithContainer(BigNumber val)
        {
            if (val<0)
            {
                Title.SetI18NKey(I18NKey.Text_Game_Loss4);
                ResultGroup.SetColor(GameConst.RED_COLOR);
                Result.DoNum(val,false).Coroutine();
            }
            else
            {
                Title.SetI18NKey(I18NKey.Text_Game_Win4);
                ResultGroup.SetColor(GameConst.GREEN_COLOR);
                Result.DoNum(val,false).Coroutine();
            }
            SetContainerWinLoss(containerWinLoss + val - IAuctionManager.Instance.LastAuctionPrice);
        }

        protected bool CanAd()
        {
            return AdManager.Instance.PlatformHasAD() &&
                   PlayerDataManager.Instance.PlayableCount < GameConst.PlayableMaxAdCount;
        }

        
        protected async ETTask<bool> PlayAd()
        {
            try
            {
                var adCount = PlayerDataManager.Instance.GetPlayableAdCount();
                Log.Info($"总共需要看{adCount}次广告,已经看了{seeAdCount}次广告");
                for (; seeAdCount < adCount; seeAdCount++)
                {
                    var res = await AdManager.Instance.PlayAd();
                    if (!res)
                    {
                        return false;
                    }
                    AfterPlayAd(adCount, seeAdCount);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }

            PlayerDataManager.Instance.OnPlayableAd();
            return true;
        }

        protected virtual void AfterPlayAd(int total, int cur)
        {
            
        }
    }
}