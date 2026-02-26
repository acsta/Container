using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace TaoTie
{
	public class UIDiceWin : UIBaseView, IOnCreate, IOnEnable, IOnDisable
	{
		private int DiceAdGetCount;
		public static string PrefabPath => "UIGame/UIAuction/Prefabs/UIDiceWin.prefab";
		public UIPointerClick Mask;
		public UITextmesh Title;
		public UIImage Icon;
		public UITextmesh Text1;
		public UIToggle CommonToggle;
		public UIButton Button1;
		public UITextmesh Text2;
		public UIImage Ad1;
		public UIButton Button2;
		public UITextmesh Text;

		public UITextmesh Using;
		
		private int configId;
		public UIAnimator UICommonWin;


		#region override
		public void OnCreate()
		{
			UICommonWin = AddComponent<UIAnimator>("UICommonWin");
			if (!GlobalConfigCategory.Instance.TryGetInt("DiceAdGetCount", out DiceAdGetCount))
			{
				DiceAdGetCount = 3;
			}

			Using = AddComponent<UITextmesh>("UICommonWin/Win/Content/Using");
			Mask = AddComponent<UIPointerClick>("UICommonWin/Mask");
			this.Title = this.AddComponent<UITextmesh>("UICommonWin/Win/Title");
			this.Icon = this.AddComponent<UIImage>("UICommonWin/Win/Content/Icon");
			this.Text = this.AddComponent<UITextmesh>("UICommonWin/Win/Content/Desc/Text");
			this.CommonToggle = this.AddComponent<UIToggle>("UICommonWin/Win/Content/Notice/CommonToggle");
			this.Button1 = this.AddComponent<UIButton>("UICommonWin/Win/Content/Button1");
			this.Text1 = this.AddComponent<UITextmesh>("UICommonWin/Win/Content/Button1/Text");
			this.AddComponent<UINumRedDot, string>("UICommonWin/Win/Content/Button1/Reddot", "Item_" + GameConst.DiceItemId);
			this.Ad1 = this.AddComponent<UIImage>("UICommonWin/Win/Content/Button1/Ad");
			this.Button2 = this.AddComponent<UIButton>("UICommonWin/Win/Content/Button2");
			this.Text2 = this.AddComponent<UITextmesh>("UICommonWin/Win/Content/Button2/Text");
		}
		public void OnEnable()
		{
			SoundManager.Instance.PlaySound("Audio/Sound/Win_Open.mp3");
			configId = IAuctionManager.Instance.DiceId;
			RefreshDice();
			this.CommonToggle.SetOnValueChanged(SetOnCommonToggleValueChanged);
			RefreshBtn();
			this.CommonToggle.SetIsOn(CacheManager.Instance.GetInt(CacheKeys.DiceSetting) == 1, false);
		}
		
		public void OnDisable()
		{
			var report = UIManager.Instance.GetView<UIReportWin>(1);
			if (report == null && IAuctionManager.Instance != null)
			{
				GameTimerManager.Instance.SetTimeScale(1);
			}
			var infoWin = UIManager.Instance.GetView<UIGameView>(1);
			if (infoWin != null)
			{
				infoWin.OnSecondWinOver();
			}
		}
		public override async ETTask CloseSelf()
		{
			SoundManager.Instance.PlaySound("Audio/Sound/Win_Close.mp3");
			await UICommonWin.Play("UIWin_Close");
			await base.CloseSelf();
		}
		#endregion

		#region 事件绑定

		public void OnClickMask()
		{
			CloseSelf().Coroutine();
		}
		public void SetOnCommonToggleValueChanged(bool val)
		{
			CacheManager.Instance.SetInt(CacheKeys.DiceSetting, val ? 1 : 0);
			CacheManager.Instance.Save();
		}
		public void OnClickUseDice()
		{
			if (PlayerDataManager.Instance.GetItemCount(GameConst.DiceItemId) <= 0)
			{
				if (AdManager.Instance.PlatformHasAD())
				{
					AdGetDice().Coroutine();
				}
				else
				{
					UIManager.Instance.OpenBox<UIToast,I18NKey>(UIToast.PrefabPath,I18NKey.Text_Item_NotEnough).Coroutine();
				}
			}
			else
			{
				RandomDice();
			}
		}
		public void OnClickApplyDice()
		{
			Button1.SetInteractable(false);
			Button2.SetInteractable(false);
			Mask.SetOnClick(null);
			IAuctionManager.Instance.SelectDice(configId, OnClickMask);
		}
		#endregion

		private void RefreshDice()
		{
			var config = DiceConfigCategory.Instance.Get(configId);
			Text.SetText(I18NManager.Instance.I18NGetText(config, 1));
			Title.SetText(I18NManager.Instance.I18NGetText(config));
			Icon.SetSpritePath(config.Icon).Coroutine();
		}
		private void RefreshBtn()
		{
			if (IAuctionManager.Instance.DiceId == 0)
			{
				Using.SetActive(false);
				Button1.SetActive(true);
				Button2.SetActive(true);
				if (configId == 0)
				{
					Mask.SetOnClick(OnClickMask);
					if (PlayerDataManager.Instance.GetItemCount(GameConst.DiceItemId) <= 0 &&
					    AdManager.Instance.PlatformHasAD())
					{
						Ad1.SetActive(true);
						Text1.SetI18NKey(I18NKey.Text_Dice_Get, DiceAdGetCount);
					}
					else
					{
						Ad1.SetActive(false);
						Text1.SetI18NKey(I18NKey.Text_Dice_Use);
					}
					Text2.SetI18NKey(I18NKey.Global_Btn_Back);
					Button1.SetOnClick(OnClickUseDice);
					Button2.SetOnClick(OnClickMask);
				}
				else
				{
					Mask.SetOnClick(null);
					if (PlayerDataManager.Instance.GetItemCount(GameConst.DiceItemId) <= 0 &&
					    AdManager.Instance.PlatformHasAD())
					{
						Ad1.SetActive(true);
						Text1.SetI18NKey(I18NKey.Text_Dice_Get, DiceAdGetCount);
					}
					else
					{
						Ad1.SetActive(false);
						Text1.SetI18NKey(I18NKey.Text_Dice_ReSelect);
					}
					Text2.SetI18NKey(I18NKey.Global_Btn_Confirm);
					Button1.SetOnClick(OnClickUseDice);
					Button2.SetOnClick(OnClickApplyDice);
				}
				Button1.SetInteractable(true);
				Button2.SetInteractable(true);
			}
			else
			{
				Using.SetActive(true);
				Mask.SetOnClick(OnClickMask);
				Button1.SetActive(false);
				Button2.SetActive(false);
			}
		}
		private async ETTask AdGetDice()
		{
			Button1.SetInteractable(false);
			Button2.SetInteractable(false);
			try
			{
				var res = await AdManager.Instance.PlayAd();
				if (res)
				{
					PlayerDataManager.Instance.ChangeItem(GameConst.DiceItemId, DiceAdGetCount);
					RefreshBtn();
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex);
			}
			finally
			{
				Button1.SetInteractable(true);
				Button2.SetInteractable(true);
			}
		}

		private void RandomDice()
		{
			if (DiceConfigCategory.Instance.TryGetDiceConfig(IAuctionManager.Instance.Level, out var list))
			{
				var total = 0;
				for (int i = 0; i < list.Count; i++)
				{
					total += list[i].Weight;
				}
				if (total <= 0)
				{
					Log.Error("权值为0");
					return;
				}

				var val = Random.Range(0, total);
				for (int i = 0; i <= list.Count; i++)
				{
					var conf = list[i % list.Count];
					val -= conf.Weight;
					if (val <= 0 && configId != conf.Id)
					{
						PlayerDataManager.Instance.ChangeItem(GameConst.DiceItemId, -1);
						configId = conf.Id;
						break;
					}
				}
			}
			RefreshDice();
			RefreshBtn();
		}
	}
}
