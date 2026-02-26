using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace TaoTie
{
	public class UIGameView : UIBaseView, IOnCreate, IOnEnable, IOnDisable,IOnWidthPaddingChange
	{
		public static string PrefabPath => "UIGame/UIAuction/Prefabs/UIGameView.prefab";

		public UIEmptyView Menu;
		public UIButton DiceBtn;
		public UIButton CSBtn;
		public UIImage DiceIcon;
		public UITextmesh DiceText;
		public UIButton TaskBtn;
		public UIButton[] BidButtons;
		public UIImage[] BidAdIcon;
		public UIImage[] BidIcon;
		public UITextmesh[] ButtonText;
		public UITextmesh TextRangePrice;
		public UIButton SkipBtn;
		public UIButton BackBtn;
		public UINumRedDot Dice;
		public UINumRedDot Task;

		public UIAnimator Animator;
		public UIAnimator Button;
		public UIImage TextRange;

		public UICashGroup CashGroup;
		
		private ETCancellationToken cancel;
		private ETCancellationToken cancelAnim;
		private BigNumber[] auctionMoney = new BigNumber[3];

		private ActionLineVolume actionLineVolume;
		private bool canBack = true;
		#region override
		public void OnCreate()
		{
			CSBtn = AddComponent<UIButton>("Menus/CS");
			Button = AddComponent<UIAnimator>("UnAutoFit/Bottom/Auction");
			Dice = AddComponent<UINumRedDot, string>("Menus/Dice/Reddot", "Item_"+GameConst.DiceItemId);
			Task = AddComponent<UINumRedDot, string>("Menus/Task/Reddot", "Task_Commiting");
			Menu = AddComponent<UIEmptyView>("Menus");
			DiceBtn = AddComponent<UIButton>("Menus/Dice");
			DiceIcon = AddComponent<UIImage>("Menus/Dice/Icon");
			DiceText = AddComponent<UITextmesh>("Menus/Dice/Text");
			TaskBtn = AddComponent<UIButton>("Menus/Task");
			Animator = AddComponent<UIAnimator>();
			CashGroup = AddComponent<UICashGroup>("Top");
			TextRange = AddComponent<UIImage>("UnAutoFit/Bottom/TextRange");
			TextRangePrice = AddComponent<UITextmesh>("UnAutoFit/Bottom/TextRange/TextRangePrice");
			BidButtons = new UIButton[3];
			BidAdIcon = new UIImage[3];
			BidIcon = new UIImage[3];
			ButtonText = new UITextmesh[3];
			for (int i = 1; i <= 3; i++)
			{
				var index = i - 1;
				BidButtons[index] = AddComponent<UIButton>("UnAutoFit/Bottom/Auction/Bid_Button_" + i);
				ButtonText[index] = AddComponent<UITextmesh>("UnAutoFit/Bottom/Auction/Bid_Button_" + i+"/desc");
				BidAdIcon[index] = AddComponent<UIImage>("UnAutoFit/Bottom/Auction/Bid_Button_" + i+"/ad");
				BidIcon[index] = AddComponent<UIImage>("UnAutoFit/Bottom/Auction/Bid_Button_" + i+"/icon");
			}
			SkipBtn = AddComponent<UIButton>("Top/Skip");
			BackBtn = AddComponent<UIButton>("Top/Back");
			var volume = SceneManager.Instance?.GetCurrentScene<MapScene>()?.Volume;
			if (volume != null && volume.sharedProfile.TryGet<ActionLineVolume>(out var co))
			{
				actionLineVolume = co;
				co.active = false;
			}
		}
		public void OnEnable()
		{
			CSBtn.SetActive(false);
			CSBtn.SetOnClick(OnClickCS);
			TaskBtn.SetOnClick(OnClickTask);
			DiceBtn.SetOnClick(OnClickDice);
			RefreshDice();
			
#if UNITY_EDITOR
			SkipBtn.SetOnClick(OnClickSkip);
#else
			SkipBtn.SetActive(false);
#endif
			BackBtn.SetOnClick(OnClickBack);
			OnEnableAsync().Coroutine();
			for (int i = 0; i < 3; i++)
			{
				var index = i;
				BidButtons[index].SetOnClick(() =>
				{
					OnAuction(index);
				});
				BidAdIcon[index].SetActive(false);
				BidIcon[index].SetActive(true);
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
			canBack = false;
			await Animator.Play("UIGameView_Open");
			canBack = true;
		}

		public override async ETTask CloseSelf()
		{
			cancel?.Cancel();
			cancel = null;
			canBack = false;
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
			canBack = false;
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
			canBack = true;
			await Animator.Play("UIGameView_ButtonOpen");
		}
		
		public void ReEnter(ETCancellationToken cancellationToken)
		{
			cancelAnim = cancellationToken;
		}

		/// <summary>
		/// 任務物品動畫
		/// </summary>
		public async ETTask DoTaskMoveAnim(GameObject obj, bool effect)
		{
		    var moveTime = 100f;
		    var scaleTime = 100f;
		    var startPos = obj.transform.position;
		    await DoSingleTaskScaleAnim(obj, scaleTime);
		    await DoSingleTaskMoveAnim(startPos, obj, moveTime);
		    
		    TaskBtn.GetGameObject().GetComponent<Animator>().enabled = false;
		    var scale = Vector3.one * 1.3f;
		    var startTime = TimerManager.Instance.GetTimeNow();
		    var animTime = 100f;
		    while (true)
		    {
		       await TimerManager.Instance.WaitAsync(1);
		       
		       var timeNow = TimerManager.Instance.GetTimeNow();
		       TaskBtn.GetRectTransform().localScale = Vector3.Lerp(Vector3.one, scale, (timeNow - startTime) / animTime);
		       obj.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(1f, 0.5f, (timeNow - startTime) / animTime);
		       if (timeNow - startTime >= animTime)
		       {
		          break;
		       }
		    }
		    
		    startTime = TimerManager.Instance.GetTimeNow();
		    while (true)
		    {
		       await TimerManager.Instance.WaitAsync(1);
		       
		       var timeNow = TimerManager.Instance.GetTimeNow();
		       TaskBtn.GetRectTransform().localScale = Vector3.Lerp(scale, Vector3.one, (timeNow - startTime) / animTime);
		       obj.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(0.5f, 0, (timeNow - startTime) / animTime);
		       if (timeNow - startTime >= animTime)
		       {
		          break;
		       }
		    }
		    
		    TaskBtn.GetGameObject().GetComponent<Animator>().enabled = true;
		    if(effect) PlayFxAsync().Coroutine();
		}

		private async ETTask PlayFxAsync()
		{
			var FX = await GameObjectPoolManager.GetInstance().GetGameObjectAsync(GameConst.TaskPrefab);
			FX.SetActive(false);
			FX.transform.position = Task.GetRectTransform().position;
			FX.SetActive(true);
			await TimerManager.Instance.WaitAsync(1000);
			GameObjectPoolManager.GetInstance().RecycleGameObject(FX);
		}
		
		private async ETTask DoSingleTaskScaleAnim(GameObject target, float animTime = 300f)
		{
			animTime /= 2;
			var startScale = target.transform.localScale;
			var startTime = TimerManager.Instance.GetTimeNow();
			while (true)
			{
				await TimerManager.Instance.WaitAsync(1);
				var timeNow = TimerManager.Instance.GetTimeNow();
				target.transform.localScale = Vector3.Lerp(startScale, Vector3.one, (timeNow - startTime) / animTime);
				if (timeNow - startTime >= animTime)
				{
					break;
				}
			}
    
			startTime = TimerManager.Instance.GetTimeNow();
			while (true)
			{
				await TimerManager.Instance.WaitAsync(1);
				var timeNow = TimerManager.Instance.GetTimeNow();
				target.transform.localScale = Vector3.Lerp(Vector3.one, startScale,  (timeNow - startTime) / animTime);
				if (timeNow - startTime >= animTime)
				{
					break;
				}
			}
		}
		private async ETTask DoSingleTaskMoveAnim(Vector3 startPos, GameObject target, float animTime = 300f)
		{
			var startScale = target.transform.localScale;
			var endPos = TaskBtn.GetRectTransform().position;
			var startTime = TimerManager.Instance.GetTimeNow();
			while (true)
			{
				await TimerManager.Instance.WaitAsync(1);
				var timeNow = TimerManager.Instance.GetTimeNow();
				target.transform.position = Vector3.Lerp(startPos, endPos, (timeNow - startTime) / animTime);
				target.transform.localScale = Vector3.Lerp(startScale, startScale * 0.5f, (timeNow - startTime) / animTime);
				if (timeNow - startTime >= animTime)
				{
					break;
				}
			}
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
				if (PlayerDataManager.Instance.CanShowAdRewardsMoney())
				{
					PlayAd().Coroutine();
				}
				else
				{
					UIManager.Instance.OpenBox<UIToast, I18NKey>(UIToast.PrefabPath, I18NKey.Notice_Common_LackOfMoney).Coroutine();
				}
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
			if (IAuctionManager.Instance.DiceConfig.Type == 4)
			{
				TextRangePrice.SetI18NKey(I18NKey.Text_Game_Price2,IAuctionManager.Instance.AllPrice);
				TextRange.SetSpritePath(
						$"UIGame/UIAuction/Atlas/range_bg_{(IAuctionManager.Instance.AllPrice <= IAuctionManager.Instance.LastAuctionPrice ? 3 : 1)}.png")
					.Coroutine();
			}
			else
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
		}

		private void RefreshAuctionState(AuctionState auctionState)
		{
			switch (auctionState)
			{
				case AuctionState.WaitUser:
					auctionMoney[0] = IAuctionManager.Instance.LowAuction;
					auctionMoney[1] = IAuctionManager.Instance.MediumAuction;
					auctionMoney[2] = IAuctionManager.Instance.HighAuction;
					for (int i = 0; i < auctionMoney.Length; i++)
					{
						bool showAd = auctionMoney[i] > PlayerDataManager.Instance.TotalMoney &&
						              PlayerDataManager.Instance.CanShowAdRewardsMoney();
						if (showAd)
						{
							ButtonText[i].SetNum(IAuctionManager.Instance.LevelConfig.AdMoneyCount);
						}
						else
						{
							ButtonText[i].SetNum(auctionMoney[i]);
						}
						BidAdIcon[i].SetActive(showAd);
						BidIcon[i].SetActive(!showAd);
					}
					RefreshRangePrice();
					TextRange.SetActive(true);
					break;
				case AuctionState.Ready:
					for (int i = 0; i < auctionMoney.Length; i++)
					{
						ButtonText[i].SetText("-");
						ButtonText[i].SetActive(true);
						BidAdIcon[i].SetActive(false);
						BidIcon[i].SetActive(true);
					}
					TextRange.SetActive(false);
					RefreshAuctionStage();
					break;
			}

			if (actionLineVolume != null)
			{
				actionLineVolume.active = IAuctionManager.Instance.AState == AuctionState.WaitUser 
				                          && IAuctionManager.Instance.IsRaising;
			}
			
			bool isShow = IAuctionManager.Instance.LastAuctionPlayerId != IAuctionManager.Instance.Player?.Id &&
			              auctionState == AuctionState.WaitUser;
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
					if (IAuctionManager.Instance.LastAuctionPlayerId == IAuctionManager.Instance.Player?.Id)
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

		private void OnClickCS()
		{
			UIToast.ShowToast(I18NKey.Text_Notice_CS);
		}

		private void OnClickTask()
		{
			GameTimerManager.Instance.SetTimeScale(0);
			Menu.SetActive(false);
			UIManager.Instance.OpenWindow<UITaskInfoWin, bool, BigNumber>(UITaskInfoWin.PrefabPath, false,
				CashGroup.ShowNum, UILayerNames.SceneLayer).Coroutine();
		}

		private void OnClickDice()
		{
			GameTimerManager.Instance.SetTimeScale(0);
			Menu.SetActive(false);
			CashGroup.SetActive(false);
			UIManager.Instance.OpenWindow<UIDiceWin>(UIDiceWin.PrefabPath).Coroutine();
		}

		public void OnSecondWinOver()
		{
			Menu.SetActive(true);
			if (!CashGroup.ActiveSelf)
			{
				CashGroup.SetActive(true);
			}
			RefreshDice();
		}

		private void RefreshDice()
		{
			DiceIcon.SetSpritePath(IAuctionManager.Instance.DiceConfig.Icon).Coroutine();
			DiceText.SetText(I18NManager.Instance.I18NGetText(IAuctionManager.Instance.DiceConfig));
			if (IAuctionManager.Instance.DiceId > 0)
			{
				Dice.ReSetTarget("");
			}
			else
			{
				Dice.ReSetTarget("Item_" + GameConst.DiceItemId);
			}
			if (IAuctionManager.Instance.DiceConfig.Type == 4)
			{
				RefreshRangePrice();
			}
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

		private void OnClickBack()
		{
			GameTimerManager.Instance.SetTimeScale(0);
			UIManager.Instance.OpenWindow<UISettingWin,bool>(UISettingWin.PrefabPath,canBack,UILayerNames.TipLayer).Coroutine();
		}

		private async ETTask PlayAd()
		{
			GameTimerManager.Instance.SetTimeScale(0);
			try
			{
				var res = await AdManager.Instance.PlayAd();
				if (res)
				{
					res = PlayerDataManager.Instance.AdRewardsMoney(IAuctionManager.Instance.LevelConfig.AdMoneyCount);
					if (res)
					{
						RefreshAuctionState(IAuctionManager.Instance.AState);
					}
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex);
			}
			finally
			{
				GameTimerManager.Instance.SetTimeScale(1);
			}
		}

		public async ETTask PlayCSAnim(Vector3 pos)
		{
			if (pos == Vector3.zero)
			{
				CSBtn.SetActive(true);
				return;
			}
			await ETTask.CompletedTask;
			CSBtn.SetActive(true);
		}
	}
}
