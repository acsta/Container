using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace TaoTie
{
	public class UIReportWin : UIBaseView, IOnCreate, IOnEnable<AuctionReport[],int>, IOnEnable<AuctionReport[],int, bool>,IOnDisable
	{
		public static string PrefabPath => "UIGame/UIAuction/Prefabs/UIReportWin.prefab";
		public UILoopListView2 ScrollView;
		public UIButton Task;
		public UIButton Share;
		public UITextmesh TextResult;
		public UIButton Back;
		public UIButton ReGame;
		public UITextmesh ReGameText;
		public UITextmesh Title;
		private AuctionReport[] list;
		public UIImage Bottom;
		public UIImage Icon;
		private bool isGameOver;
		#region override
		public void OnCreate()
		{
			Bottom = AddComponent<UIImage>("Win/Bottom");
			Icon = AddComponent<UIImage>("Win/Icon");
			this.ScrollView = this.AddComponent<UILoopListView2>("Win/Content/ScrollView");
			this.ScrollView.InitListView(0,GetScrollViewItemByIndex);
			this.Task = this.AddComponent<UIButton>("Win/Task");
			this.Share = this.AddComponent<UIButton>("Win/Share");
			this.TextResult = this.AddComponent<UITextmesh>("Win/Bottom/TextResult");
			this.Back = this.AddComponent<UIButton>("Win/Button/Back");
			this.ReGame = this.AddComponent<UIButton>("Win/Button/ReGame");
			ReGameText = AddComponent<UITextmesh>("Win/Button/ReGame/Text");
			Title = AddComponent<UITextmesh>("Win/Title");
			
		}

		public void OnEnable(AuctionReport[] reports, int level)
		{
			OnEnable(reports, level, true);
		}

		public void OnEnable(AuctionReport[] reports, int level, bool isGameOver)
		{
			this.isGameOver = isGameOver;
			list = reports;
			this.Task.SetOnClick(OnClickTask);
			this.Share.SetOnClick(OnClickShare);
			Share.SetActive(SDKManager.Instance.CanShare());
			this.Back.SetOnClick(OnClickBack);
			this.ReGame.SetOnClick(OnClickReGame);
			ReGame.SetActive(isGameOver);
			ScrollView.SetListItemCount(reports.Length);
			ScrollView.RefreshAllShownItem();
			ReGameText.SetI18NKey(isGameOver?I18NKey.Text_Re_Game:I18NKey.Text_Continue_Game);
			var lv = LevelConfigCategory.Instance.Get(level);
			Title.SetText(I18NManager.Instance.I18NGetText(lv));
			Icon.SetSpritePath(lv.Icon).Coroutine();
			BigNumber total = BigNumber.Zero;
			for (int i = 0; i < reports.Length; i++)
			{
				total += reports[i].FinalUserWin;
			}
			var money = total;
			if (money < 0) money = -money;
			var get = I18NManager.Instance.TranslateMoneyToStr(money);
			TextResult.SetI18NKey(total >= 0 ? I18NKey.Text_Game_Win_Total : I18NKey.Text_Game_Loss_Total, get);
			Bottom.SetColor(total >= 0 ? GameConst.GREEN_COLOR : GameConst.RED_COLOR);
			ApplyClothEffect();
			var mainCamera = CameraManager.Instance.MainCamera();
			if (mainCamera != null)
			{
				CameraManager.Instance.MainCamera().cullingMask = Define.UILayer;
			}
		}

		public void OnDisable()
		{
			var mainCamera = CameraManager.Instance.MainCamera();
			if (mainCamera != null)
			{
				mainCamera.cullingMask = Define.AllLayer;
			}
			GameRecorderManager.Instance.StopRecorder().Coroutine();
		}
		
		#endregion

		#region 事件绑定
		public LoopListViewItem2 GetScrollViewItemByIndex(LoopListView2 listView, int index)
		{
			if (index < 0 || index >= list.Length) return null;
			var item = listView.NewListViewItem("UIReportItem", index);

			UIReportItem reportItem;
			if (!item.IsInitHandlerCalled)
			{
				reportItem = ScrollView.AddItemViewComponent<UIReportItem>(item);
				item.IsInitHandlerCalled = true;
			}
			else
			{
				reportItem = ScrollView.GetUIItemView<UIReportItem>(item);
			}

			reportItem.SetData(list[index]);
			return item;
		}
		public void OnClickTask()
		{
			UIManager.Instance
				.OpenWindow<UITaskInfoWin, bool, BigNumber>(UITaskInfoWin.PrefabPath, false,
					PlayerDataManager.Instance.TotalMoney).Coroutine();
		}

		public void OnClickShare()
		{
			OnClickShareAsync().Coroutine();
		} 
		public async ETTask OnClickShareAsync()
		{
			await GameRecorderManager.Instance.StopRecorder();
			if (GameRecorderManager.Instance.CanShareVideo())
			{
				UIManager.Instance.OpenBox<UIMsgBoxWin,MsgBoxPara>(UIMsgBoxWin.PrefabPath,new MsgBoxPara()
				{
					Content = I18NManager.Instance.I18NGetText(I18NKey.Tips_Share_Video),
					CancelText = I18NManager.Instance.I18NGetText(I18NKey.Text_Share_Invite),
					CancelCallback = (win) =>
					{
						UIManager.Instance.CloseBox(win).Coroutine();
						SDKManager.Instance.ShareGlobal(1).Coroutine();
					},
					ConfirmText = I18NManager.Instance.I18NGetText(I18NKey.Text_Share_Video),
					ConfirmCallback = (win) =>
					{
						UIManager.Instance.CloseBox(win).Coroutine();
						GameRecorderManager.Instance.PublishVideo();
					},
					CanClose = true
				}).Coroutine();
			}
			else
			{
				SDKManager.Instance.ShareGlobal(1).Coroutine();
			}
		}
		public void OnClickBack()
		{
			if (isGameOver)
			{
				SceneManager.Instance.SwitchScene<HomeScene>().Coroutine();
			}
			else
			{
				OnClickBackAsync().Coroutine();
			}
		}
		private async ETTask OnClickBackAsync()
		{
			await CloseSelf();
			var gameView = UIManager.Instance.GetView<UIGameView>(1);
			if (gameView != null)
			{
				await gameView.CloseSelf();
			}
			GameTimerManager.Instance.SetTimeScale(1);
			IAuctionManager.Instance.ForceAllOver();
		}
		public void OnClickReGame()
		{
			if (isGameOver)
			{
				OnClickReGameAsync().Coroutine();
			}
			else
			{
				GameTimerManager.Instance.SetTimeScale(1);
			}
			CloseSelf().Coroutine();
		}
		#endregion

		private async ETTask OnClickReGameAsync()
		{
			await CloseSelf();
			var blendView = await UIManager.Instance.OpenWindow<UIBlendView>(UIBlendView.PrefabPath,UILayerNames.TopLayer);
			await blendView.CaptureBg(true);
			SceneManager.Instance.SwitchMapScene(IAuctionManager.Instance.LevelConfig.Name).Coroutine();
		}

		private void ApplyClothEffect()
		{
			int finalPercent = PlayerDataManager.Instance.GetClothEffect(ClothEffectType.FinalMoneyAddon);
			if (finalPercent > 0)
			{
				BigNumber addon = BigNumber.Zero;
				for (int i = 0; i < list.Length; i++)
				{
					if (list[i].Type == ReportType.Self && list[i].FinalUserWin>0 || 
					    list[i].Type == ReportType.Others && list[i].RaiseSuccessCount>0)
					{
						addon += list[i].FinalUserWin * finalPercent / 100;
					}
				}

				if (addon > 0)
				{
					PlayerDataManager.Instance.RecordWinToday(addon);
					PlayerDataManager.Instance.ChangeMoney(addon);
					UIManager.Instance.OpenBox<UIToast, string>(UIToast.PrefabPath,
						I18NManager.Instance.I18NGetParamText(I18NKey.Text_Equip_Effect_1_Active,
							I18NManager.Instance.TranslateMoneyToStr(addon)), UILayerNames.TopLayer).Coroutine();
				}
			}
		}
	}
}
