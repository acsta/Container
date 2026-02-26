using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace TaoTie
{
	public class UIProfitWin : UIBaseView, IOnCreate, IOnEnable
	{
		public static string PrefabPath => "UIGame/UILobby/Prefabs/UIProfitWin.prefab";
		public UIButton Close;
		public UITextmesh Desc;
		public UIButton ButtonExpand;
		public UIButton Button;
		public UIButton ButtonAd;
		public UITextmesh Text;
		public UITextmesh Speed;

		private bool anim = false;
		public UIAnimator UICommonWin;


		#region override
		public void OnCreate()
		{
			UICommonWin = AddComponent<UIAnimator>("UICommonWin");
			this.Close = this.AddComponent<UIButton>("UICommonWin/Win/Close");
			this.Desc = this.AddComponent<UITextmesh>("UICommonWin/Win/Content/Bg/Desc/Desc");
			this.ButtonExpand = this.AddComponent<UIButton>("UICommonWin/Win/Content/Bg/Desc/ButtonExpand");
			this.Button = this.AddComponent<UIButton>("UICommonWin/Win/Content/Button");
			this.ButtonAd = this.AddComponent<UIButton>("UICommonWin/Win/Content/ButtonAd");
			this.Text = this.AddComponent<UITextmesh>("UICommonWin/Win/Content/Bg/Details/Text");
			Speed = AddComponent<UITextmesh>("UICommonWin/Win/Content/Bg/Speed");
			Speed.SetI18NKey(I18NKey.Text_Profit_Speed);
			Desc.SetI18NKey(I18NKey.Text_Restaurant_Profit_Hour);
		}
		public void OnEnable()
		{
			SoundManager.Instance.PlaySound("Audio/Sound/Win_Open.mp3");
			this.Close.SetOnClick(OnClickClose);
			this.ButtonExpand.SetOnClick(OnClickButtonExpand);
			this.Button.SetOnClick(OnClickButton);
			this.ButtonAd.SetOnClick(OnClickButtonAd);
			Desc.SetI18NText(PlayerDataManager.Instance.GetMaxDeltaTime());
			Text.SetNum(PlayerDataManager.Instance.CalculateProfit());
			Speed.SetI18NText(I18NManager.Instance.TranslateMoneyToStr(PlayerDataManager.Instance
				.CalculateProfitUnit() * GameConst.ProfitUnitShowTime / GameConst.ProfitUnitTime)+ "/" +
			                  TimeInfo.Instance.TransitionToStr2(GameConst.ProfitUnitShowTime));
		}

		public void Update(BigNumber value)
		{
			if(!anim) Text.DoNum(value).Coroutine();
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
		public void OnClickButtonExpand()
		{
			if (AdManager.Instance.PlatformHasAD())
			{
				PlayAdAsync().Coroutine();
			}
		}
		public async ETTask PlayAdAsync()
		{
			ButtonExpand.SetInteractable(false);
			try
			{
				var res = await AdManager.Instance.PlayAd();
				if (res)
				{
					PlayerDataManager.Instance.Expand();
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex);
			}
			finally
			{
				ButtonExpand.SetInteractable(true);
				Desc.SetI18NText(PlayerDataManager.Instance.GetMaxDeltaTime());
			}
		}
		public void OnClickButton()
		{
			ButtonAd.SetInteractable(false);
			OnClickButtonAsync().Coroutine();
		}
		public async ETTask OnClickButtonAsync()
		{
			CloseSelf().Coroutine();
			var top = UIManager.Instance.GetView<UITopView>(1);
			if (top != null)
			{
				anim = true;
				var current = PlayerDataManager.Instance.CalculateProfit();
				var value = Mathf.CeilToInt(PlayerDataManager.Instance.GetProfitProgress() * 10);
				await top.Top.DoMoneyMoveAnim(current, GetTransform().position, value, 1.3f);
				anim = false;
			}
			PlayerDataManager.Instance.GetProfit();
			var win = UIManager.Instance.GetView<UIWashDishView>(1);
			win?.UpdateImmediate();
			ButtonAd.SetInteractable(true);
		}
		public void OnClickButtonAd()
		{
			OnClickButtonAdAsync().Coroutine();
		}
		public async ETTask OnClickButtonAdAsync()
		{
			if (AdManager.Instance.PlatformHasAD())
			{
				try
				{
					ButtonAd.SetInteractable(false);
					var res = await AdManager.Instance.PlayAd();
					if (res)
					{
						CloseSelf().Coroutine();
						var top = UIManager.Instance.GetView<UITopView>(1);
						if (top != null)
						{
							anim = true;
							var current = PlayerDataManager.Instance.CalculateProfit()*2;
							var value = Mathf.CeilToInt(PlayerDataManager.Instance.GetProfitProgress() * 10);
							await top.Top.DoMoneyMoveAnim(current, GetTransform().position, value * 2, 1.3f);
							anim = false;
						}
						PlayerDataManager.Instance.GetProfit(2);
						var win = UIManager.Instance.GetView<UIWashDishView>(1);
						win?.UpdateImmediate();
					}
				}
				catch (Exception e)
				{
					Log.Error(e);
				}
				finally
				{
					ButtonAd.SetInteractable(true);
				}
			}
		}
		#endregion
	}
}
