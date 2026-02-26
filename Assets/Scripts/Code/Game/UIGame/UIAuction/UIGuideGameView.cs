using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace TaoTie
{
	public class UIGuideGameView : UIBaseView, IOnCreate, IOnEnable, IOnDisable,IOnWidthPaddingChange
	{
		public static string PrefabPath => "UIGame/UIAuction/Prefabs/UIGuideGameView.prefab";
		
		public UIButton[] BidButtons;
		public UITextmesh[] ButtonText;
		public UITextmesh TextRangePrice;
		public UIButton SkipBtn;
		public UIAnimator Animator;
		public UICashGroup CashGroup;
		private ETCancellationToken cancel;
		private ETCancellationToken cancelAnim;
		private BigNumber[] auctionMoney = new BigNumber[3];
		public UIAnimator Button;
		public UIImage TextRange;
		public UIImage ButtonBg;
		
		private ActionLineVolume actionLineVolume;
		#region override
		public void OnCreate()
		{
			ButtonBg = AddComponent<UIImage>("UnAutoFit/Bottom/Bg");
			Button = AddComponent<UIAnimator>("UnAutoFit/Bottom/Auction");
			Animator = AddComponent<UIAnimator>();
			CashGroup = AddComponent<UICashGroup>("Top");
			TextRange = AddComponent<UIImage>("UnAutoFit/Bottom/TextRange");
			TextRangePrice = AddComponent<UITextmesh>("UnAutoFit/Bottom/TextRange/TextRangePrice");
			BidButtons = new UIButton[3];
			ButtonText = new UITextmesh[3];
			for (int i = 1; i <= 3; i++)
			{
				var index = i - 1;
				BidButtons[index] = AddComponent<UIButton>("UnAutoFit/Bottom/Auction/Bid_Button_" + i);
				ButtonText[index] = AddComponent<UITextmesh>("UnAutoFit/Bottom/Auction/Bid_Button_" + i+"/desc");
			}
			SkipBtn = AddComponent<UIButton>("Top/Skip");
			var volume = SceneManager.Instance?.GetCurrentScene<GuideScene>()?.Volume;
			if (volume != null && volume.sharedProfile.TryGet<ActionLineVolume>(out var co))
			{
				actionLineVolume = co;
				co.active = false;
			}
		}
		public void OnEnable()
		{
#if UNITY_EDITOR
			SkipBtn.SetOnClick(OnClickSkip);
#else
			SkipBtn.SetActive(false);
#endif
			OnEnableAsync().Coroutine();
			for (int i = 0; i < 3; i++)
			{
				var index = i;
				BidButtons[index].SetOnClick(() =>
				{
					OnAuction(index);
				});
			}
			Messager.Instance.AddListener<AuctionState>(0, MessageId.RefreshAuctionState, RefreshAuctionState);
			Messager.Instance.AddListener(0, MessageId.ShowTextRange, ShowTextRange);
			RefreshAuctionState(IAuctionManager.Instance.AState);
			RefreshAuctionStage();
		}
		
		private async ETTask OnEnableAsync()
		{
			cancel?.Cancel();
			cancel = null;
			await Animator.Play("UIGameView_Open");
		}

		public override async ETTask CloseSelf()
		{
			cancel?.Cancel();
			cancel = null;
			UIManager.Instance.CloseWindow<UIAssistantView>().Coroutine();
			await Animator.Play("UIGameView_Close");
			await base.CloseSelf();
		}

		public void OnDisable()
		{
			Messager.Instance.RemoveListener(0, MessageId.ShowTextRange, ShowTextRange);
			Messager.Instance.RemoveListener<AuctionState>(0, MessageId.RefreshAuctionState, RefreshAuctionState);
		}
		
		public async ETTask HideButtons(ETCancellationToken cancellationToken)
		{
			cancelAnim = cancellationToken;
			if (actionLineVolume != null)
			{
				actionLineVolume.active = false;
			}
			await Animator.Play("UIGameView_ButtonClose");
		}
		
		public async ETTask ShowButtons()
		{
			for (int i = 0; i < auctionMoney.Length; i++)
			{
				ButtonText[i].SetText("-");
			}
			cancelAnim = null;
			await Animator.Play("UIGameView_ButtonOpen");
		}
		
		public void ReEnter(ETCancellationToken cancellationToken)
		{
			cancelAnim = cancellationToken;
		}

		
		#endregion

		#region 事件绑定
		public void ShowTextRange()
		{
			RefreshRangePrice();
			TextRange.SetActive(true);
		}
		public void OnAuction(int index)
		{
			if (auctionMoney[index] > PlayerDataManager.Instance.TotalMoney)
			{
				UIManager.Instance.OpenBox<UIToast, I18NKey>(UIToast.PrefabPath, I18NKey.Notice_Common_LackOfMoney).Coroutine();
				return;
			}

			var infoWin = UIManager.Instance.GetView<UITaskInfoWin>(1);
			if (infoWin != null)
			{
				infoWin.OnClickClose();
			}
			if (IAuctionManager.Instance.IsRaising)
			{
				CameraManager.Instance.Shake(0.1f,500).Coroutine();
				ShockManager.Instance.Vibrate();
			}
			IAuctionManager.Instance.UserAuction((AITactic) (index + 1));
			RefreshRangePrice();
		}

		#endregion

		private void RefreshAuctionStage()
		{
			RefreshRangePrice();
			CashGroup.RefreshMoney(PlayerDataManager.Instance.TotalMoney);
		}

		private void RefreshRangePrice()
		{
			var max = IAuctionManager.Instance.AllPrice * IAuctionManager.Instance.SysJudgePriceMax;
			var min = IAuctionManager.Instance.AllPrice * IAuctionManager.Instance.SysJudgePriceMin;
			TextRangePrice.SetI18NKey(I18NKey.Game_Price_Range,
				I18NManager.Instance.ApproximateMoneyToStr(min, I18NManager.ApproximateType.Floor),
				I18NManager.Instance.ApproximateMoneyToStr(max, I18NManager.ApproximateType.Cell));
			if (IAuctionManager.Instance.LastAuctionPrice < min)
			{
				TextRange.SetSpritePath("UIGame/UIAuction/Atlas/range_bg_1.png").Coroutine();
			}
			else if (IAuctionManager.Instance.LastAuctionPrice >= max)
			{
				TextRange.SetSpritePath("UIGame/UIAuction/Atlas/range_bg_3.png").Coroutine();
			}
			else
			{
				TextRange.SetSpritePath("UIGame/UIAuction/Atlas/range_bg_2.png").Coroutine();
			}
		}

		private void RefreshAuctionState(AuctionState auctionState)
		{
			var guidance = GuidanceStageConfigCategory.Instance.Get(IAuctionManager.Instance.Stage);
			switch (auctionState)
			{
				case AuctionState.WaitUser:
					auctionMoney[0] = IAuctionManager.Instance.LowAuction;
					auctionMoney[1] = IAuctionManager.Instance.MediumAuction;
					auctionMoney[2] = IAuctionManager.Instance.HighAuction;
					for (int i = 0; i < auctionMoney.Length; i++)
					{
						ButtonText[i].SetNum(auctionMoney[i]);
					}
					RefreshRangePrice();
					TextRange.SetActive(true);
					break;
				case AuctionState.Ready:
					for (int i = 0; i < auctionMoney.Length; i++)
					{
						ButtonText[i].SetText("-");
						ButtonText[i].SetActive(true);
					}
					ButtonBg.SetSpritePath($"UIGame/UIAuction/Atlas/UI_status_valuation{(guidance.Button.Length<=1?"2":"")}.png").Coroutine();
					RefreshAuctionStage();
					TextRange.SetActive(false);
					break;
			}
			for (int i = 0; i < BidButtons.Length; i++)
			{
				BidButtons[i].SetActive(false);
			}
			for (int i = 0; i < guidance.Button.Length; i++)
			{
				BidButtons[guidance.Button[i]].SetActive(true);
			}
			if (actionLineVolume != null)
			{
				actionLineVolume.active = IAuctionManager.Instance.AState == AuctionState.WaitUser 
				                          && IAuctionManager.Instance.IsRaising;
			}
			
			bool isShow = IAuctionManager.Instance.LastAuctionPlayerId != IAuctionManager.Instance.Player?.Id &&
			              auctionState == AuctionState.WaitUser;
			bool raiseLimit = IAuctionManager.Instance.IsRaising && guidance.PlayerMaxRaiseCount > 0 &&
			                  IAuctionManager.Instance.RaiseSuccessCount >= guidance.PlayerMaxRaiseCount;
			bool beforePlayerAuction = guidance.BeforePlayerAuction > 0 &&
			                           IAuctionManager.Instance.AuctionCount < guidance.BeforePlayerAuction;
			if (raiseLimit)
			{
				isShow = false;
			}

			if (beforePlayerAuction)
			{
				isShow = false;
			}
			cancel?.Cancel();
			if (isShow && IAuctionManager.Instance.LastAuctionPlayerId != -1)
			{
				ShowButtonAsync().Coroutine();
			}
			else
			{
				for (int i = 0; i < BidButtons.Length; i++)
				{
					BidButtons[i].SetInteractable(isShow);
					BidButtons[i].SetBtnGray(!isShow).Coroutine();
					if (IAuctionManager.Instance.LastAuctionPlayerId == IAuctionManager.Instance.Player?.Id || raiseLimit)
					{
						ButtonText[i].SetI18NKey(I18NKey.Text_Auctioned);
					}
				}
				
				if (isShow)
				{
					Button.Play("AuctionBtn").Coroutine();
				}
				else
				{
					Button.CrossFade("AuctionBtn_Hide", 0.1f);
				}
			}
		
		}

		private async ETTask ShowButtonAsync()
		{
			var cancelToken = new ETCancellationToken();
			cancel = cancelToken;
			await TimerManager.Instance.WaitAsync(250,cancelToken);
			if (cancelToken.IsCancel())
			{
				return;
			}
			
			for (int i = 0; i < BidButtons.Length; i++)
			{
				BidButtons[i].SetInteractable(true);
				BidButtons[i].SetBtnGray(false).Coroutine();
			}
			Button.Play("AuctionBtn").Coroutine();
		}
		
		private void OnClickSkip()
		{
			if (cancelAnim != null)
			{
				cancelAnim?.Cancel();
				cancelAnim = null;
			}
			else
			{
				IAuctionManager.Instance.Skip = !IAuctionManager.Instance.Skip;
			}
		}
	}
}
