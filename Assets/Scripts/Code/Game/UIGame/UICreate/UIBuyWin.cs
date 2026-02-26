using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace TaoTie
{
	public class UIBuyWin : UIBaseView, IOnCreate, IOnEnable<List<int>>, IOnDisable
	{
		public static string PrefabPath => "UIGame/UICreate/Prefabs/UIBuyWin.prefab";
		public UIButton Close;
		public ShopItem[] ClothItem;
		public UIButton Ad;
		public UITextmesh Value;
		public UIButton Buy;
		public UITextmesh Progress;
		public UIAnimator Animator;
		private BigNumber totalPrice;
		private int adCount;

		private List<int> items;
		#region override
		public void OnCreate()
		{
			Animator = AddComponent<UIAnimator>("UICommonWin");
			this.Close = this.AddComponent<UIButton>("UICommonWin/Win/Close");
			ClothItem = new ShopItem[6];
			for (int i = 0; i < ClothItem.Length; i++)
			{
				ClothItem[i] =	this.AddComponent<ShopItem>("UICommonWin/Win/Content/Content/ShopItem"+i);
			}
			this.Ad = this.AddComponent<UIButton>("UICommonWin/Win/Content/Buttons/Ad");
			this.Progress = this.AddComponent<UITextmesh>("UICommonWin/Win/Content/Buttons/Ad/Progress");
			this.Buy = this.AddComponent<UIButton>("UICommonWin/Win/Content/Buttons/Buy");
			this.Value = this.AddComponent<UITextmesh>("UICommonWin/Win/Content/Buttons/Buy/Value");
		}
		public void OnEnable(List<int> items)
		{
			SoundManager.Instance.PlaySound("Audio/Sound/Win_Open.mp3");
			this.items = items;
			RefreshView();
			this.Close.SetOnClick(OnClickClose);
			Messager.Instance.AddListener(0, MessageId.ChangeItem, RefreshView);
		}

		public void OnDisable()
		{
			Messager.Instance.RemoveListener(0, MessageId.ChangeItem, RefreshView);
		}
		#endregion

		#region 事件绑定

		public void RefreshView()
		{
			totalPrice = 0;
			adCount = 0;
			for (int i = items.Count - 1; i >= 0; i--)
			{
				if (PlayerDataManager.Instance.GetItemCount(items[i]) > 0)
				{
					items.RemoveAt(i);
				}
			}
			for (int i = 0; i < ClothItem.Length; i++)
			{
				if (i < items.Count)
				{
					ClothItem[i].SetData(items[i], null);
					ClothItem[i].SetActive(true);
					var config = ClothConfigCategory.Instance.Get(items[i]);
					if (config.GetWay == 0)
					{
						adCount++;
					}
					else if (config.GetWay == 1)
					{
						totalPrice += config.Price;
					}
				}
				else
				{
					ClothItem[i].SetActive(false);
				}
			}

			Buy.SetActive(totalPrice > 0);
			Ad.SetActive(adCount > 0);
			this.Ad.SetOnClick(OnClickAd);
			this.Buy.SetOnClick(OnClickBuy);
			Value.SetText(I18NManager.Instance.TranslateMoneyToStr(totalPrice));
			Progress.SetText($"(0/{adCount})");
		}
		public void OnClickClose()
		{
			CloseSelf().Coroutine();
		}
		public void OnClickAd()
		{
			OnClickAdAsync().Coroutine();
		}

		private async ETTask OnClickAdAsync()
		{
			try
			{
				var res = await AdManager.Instance.PlayAd();
				if (res)
				{
					for (int i = 0; i < items.Count; i++)
					{
						var config = ClothConfigCategory.Instance.Get(items[i]);
						if (config.GetWay == 0 && PlayerDataManager.Instance.GetItemCount(config.Id) <= 0)
						{
							PlayerDataManager.Instance.ChangeItem(config.Id, 1);
							items.Remove(config.Id);
							break;
						}
					}
					if (items.Count > 0)
					{
						RefreshView();
					}
					else
					{
						CloseSelf().Coroutine();
					}
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex);
			}
		}
		public void OnClickBuy()
		{
			if (PlayerDataManager.Instance.TotalMoney >= totalPrice)
			{
				PlayerDataManager.Instance.ChangeMoney(-totalPrice);
				for (int i = 0; i < items.Count; i++)
				{
					var config = ClothConfigCategory.Instance.Get(items[i]);
					if (config.GetWay == 1)
					{
						PlayerDataManager.Instance.ChangeItem(config.Id, 1);
					}
				}
				CloseSelf().Coroutine();
			}
			else
			{
				UIManager.Instance.OpenBox<UIToast,I18NKey>(UIToast.PrefabPath,I18NKey.Notice_Common_LackOfMoney).Coroutine();
			}
		}
		#endregion
		
		public override async ETTask CloseSelf()
		{
			SoundManager.Instance.PlaySound("Audio/Sound/Win_Close.mp3");
			await Animator.Play("UIWin_Close");
			await base.CloseSelf();
		}
	}
}
