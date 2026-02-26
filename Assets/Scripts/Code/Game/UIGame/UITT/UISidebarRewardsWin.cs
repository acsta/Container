using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace TaoTie
{
    public class UISidebarRewardsWin : UIBaseView, IOnCreate, IOnEnable,IOnDisable
    {
        public static string PrefabPath => "UIGame/UITT/Prefabs/UISidebarRewardsWin.prefab";
        public UIButton Close;
        public UIButton BtnEnter;
        public UITextmesh TextBtn;
        public UIImage Icon;
        public UITextmesh Rewards;
        
        public int ItemId;
        public int ItemCount;
        public UIAnimator UICommonWin;


        #region override
        public void OnCreate()
        {
            UICommonWin = AddComponent<UIAnimator>("UICommonWin");
            if (GlobalConfigCategory.Instance.TryGetArray("SidebarRewards", out int[] tempVal))
            {
                ItemId = tempVal[0];
                ItemCount = tempVal[1];
            }
            else
            {
                ItemId = 1001;
                ItemCount = 5;
            }

            Icon = AddComponent<UIImage>("UICommonWin/Win/Content/Step2/Icon");
            this.Close = this.AddComponent<UIButton>("UICommonWin/Win/Close");
            BtnEnter = this.AddComponent<UIButton>("UICommonWin/Win/Content/BtnEnter");
            TextBtn = this.AddComponent<UITextmesh>("UICommonWin/Win/Content/BtnEnter/Text");
            Rewards = AddComponent<UITextmesh>("UICommonWin/Win/Content/Step2/Rewards");
        }
        
        public void OnEnable()
        {
            SoundManager.Instance.PlaySound("Audio/Sound/Win_Open.mp3");
            bool isGot = !PlayerDataManager.Instance.CanGotSidebarRewards();
            if (isGot)
            {
                TextBtn.SetText("已领取");
            }
            else
            {
                TextBtn.SetText(Define.EnterWay == 1 ? "领取奖励" : "进入侧边栏");
            }
            this.Close.SetOnClick(OnClickBg);
            BtnEnter.SetOnClick(OnClickEnter);
            Messager.Instance.AddListener(0, MessageId.EnterWayChange,RefreshState);
            var config = ItemConfigCategory.Instance.Get(ItemId);
            Icon.SetSpritePath(config.ItemPic).Coroutine();
            Rewards.SetText(I18NManager.Instance.I18NGetText(config)+"x"+ItemCount);
            BtnEnter.SetBtnGray(isGot).Coroutine();
        }
        public void RefreshState()
        {
            TextBtn.SetText(Define.EnterWay == 1?"领取奖励":"进入侧边栏");
        }

        public void OnDisable()
        {
            Messager.Instance.RemoveListener(0, MessageId.EnterWayChange, RefreshState);
        }
        public override async ETTask CloseSelf()
        {
            SoundManager.Instance.PlaySound("Audio/Sound/Win_Close.mp3");
            await UICommonWin.Play("UIWin_Close");
            await base.CloseSelf();
        }
        #endregion

        #region 事件绑定

        public void OnClickEnter()
        {
            if (Define.EnterWay != 1)
            {
#if UNITY_EDITOR
                Define.EnterWay = 1;
#else
#if UNITY_WEBGL_TT
                TTSDK.TT.NavigateToScene(new TTSDK.UNBridgeLib.LitJson.JsonData()
                {
                    ["scene"] = "sidebar"
                }, null, null, null);
#elif UNITY_WEBGL_BILIGAME
                WeChatWASM.WX.NavigateToScene(new WeChatWASM.NavigeateToSceneOption()
                {
                    scene = "sidebar"
                });
#elif UNITY_WEBGL_KS
                KSWASM.KS.NavigateToScene(new KSWASM.NavigateToSceneOption()
                {
                    scene = "sidebar"
                });
#endif
#endif
                RefreshState();
            }
            else
            {
                if (PlayerDataManager.Instance.CanGotSidebarRewards())
                {
                    RedDotManager.Instance.RefreshRedDotViewCount("Sidebar", 0);
                    PlayerDataManager.Instance.ChangeItem(ItemId, ItemCount);
                    UIManager.Instance
                        .OpenWindow<UIRewardsView, int, long>(UIRewardsView.PrefabPath, ItemId, ItemCount,
                            UILayerNames.TipLayer).Coroutine();
                    PlayerDataManager.Instance.GetSidebarRewards();
                    Messager.Instance.Broadcast(0, MessageId.SidebarRewards);
                }
                HideSelf();
            }
        }
        public void OnClickBg()
        {
            HideSelf();
        }
        #endregion
        
        private void HideSelf()
        {
            this.CloseSelf().Coroutine();
        }
    }
}