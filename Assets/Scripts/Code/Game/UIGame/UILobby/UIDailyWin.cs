using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace TaoTie
{
	public class UIDailyWin : UIBaseView, IOnCreate, IOnEnable
	{
		public static string PrefabPath => "UIGame/UILobby/Prefabs/UIDailyWin.prefab";
		public UIButton Close;
		public UITextmesh Text;
		public UIImage Image;
		public UITextmesh Text2;
		public UIButton ButtonGet;
		public UIButton ButtonGo;
		public UITextmesh GetText;

		public UIAnimator UICommonWin;


		#region override
		public void OnCreate()
		{
			UICommonWin = AddComponent<UIAnimator>("UICommonWin");
			this.Close = this.AddComponent<UIButton>("UICommonWin/Win/Close");
			this.Text = this.AddComponent<UITextmesh>("UICommonWin/Win/Content/Bg/Details/Text");
			this.Image = this.AddComponent<UIImage>("UICommonWin/Win/Content/Bg/Rewards/Title/Image");
			this.Text2 = this.AddComponent<UITextmesh>("UICommonWin/Win/Content/Bg/Rewards/Text");
			this.ButtonGet = this.AddComponent<UIButton>("UICommonWin/Win/Content/ButtonGet");
			this.ButtonGo = this.AddComponent<UIButton>("UICommonWin/Win/Content/ButtonGo");
			GetText = this.AddComponent<UITextmesh>("UICommonWin/Win/Content/ButtonGet/Text");
		}
		public void OnEnable()
		{
			SoundManager.Instance.PlaySound("Audio/Sound/Win_Open.mp3");
			var win = PlayerDataManager.Instance.GetWinToday();
			var conf = RestaurantConfigCategory.Instance.GetByLv(PlayerDataManager.Instance.RestaurantLv, out _);
			Text.SetText(
				$"<color={(win >= conf.Need ? GameConst.GREEN_COLOR : GameConst.RED_COLOR)}>{I18NManager.Instance.TranslateMoneyToStr(win)}</color>/{I18NManager.Instance.TranslateMoneyToStr(conf.Need)}");
			var item = ItemConfigCategory.Instance.Get(conf.RewardsType);
			Image.SetSpritePath(item.ItemPic).Coroutine();
			Text2.SetText("x" + conf.RewardsCount);
			this.Close.SetOnClick(OnClickClose);
			
			ButtonGet.SetActive(win>=conf.Need);
			ButtonGo.SetActive(win<conf.Need);
			ButtonGet.SetBtnGray(PlayerDataManager.Instance.GetIsGotWinRewards()).Coroutine();
			GetText.SetI18NKey(PlayerDataManager.Instance.GetIsGotWinRewards()
				? I18NKey.Text_Come_Tomorrow
				: I18NKey.Global_Btn_Get);
			this.ButtonGet.SetOnClick(OnClickButtonGet);
			this.ButtonGo.SetOnClick(OnClickGo);
		}
		#endregion

		public override async ETTask CloseSelf()
		{
			SoundManager.Instance.PlaySound("Audio/Sound/Win_Close.mp3");
			await UICommonWin.Play("UIWin_Close");
			await base.CloseSelf();
		}
		
		#region 事件绑定
		public void OnClickClose()
		{
			CloseSelf().Coroutine();
		}

		public void OnClickGo()
		{
			CloseSelf().Coroutine();
			UIManager.Instance.OpenWindow<UIAuctionSelectView>(UIAuctionSelectView.PrefabPath).Coroutine();
		}
		public void OnClickButtonGet()
		{
			OnClickButtonGetAsync().Coroutine();
		}

		private async ETTask OnClickButtonGetAsync()
		{
			if (PlayerDataManager.Instance.ReceiveWinRewards())
			{
				ButtonGet.SetBtnGray(PlayerDataManager.Instance.GetIsGotWinRewards()).Coroutine();
				GetText.SetI18NKey(I18NKey.Text_Come_Tomorrow);
				await CloseSelf();
				
				var conf = RestaurantConfigCategory.Instance.GetByLv(PlayerDataManager.Instance.RestaurantLv, out _);
				await UIManager.Instance
					.OpenWindow<UIRewardsView, int, long>(UIRewardsView.PrefabPath, conf.RewardsType, conf.RewardsCount,
						UILayerNames.TipLayer);
			}
		}
		#endregion
	}
}
