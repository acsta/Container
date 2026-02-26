using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace TaoTie
{
	public class ShopItem : UIBaseContainer, IOnCreate
	{
		public ClothItem ClothItem;
		public UIButton AdBtn;
		public UIButton BuyBtn;
		// public UIEmptyView Have;
		public UITextmesh Price;

		private int clothId;
		#region override
		public void OnCreate()
		{
			this.ClothItem = this.AddComponent<ClothItem>("ClothItem");
			this.AdBtn = this.AddComponent<UIButton>("AdBtn");
			this.BuyBtn = this.AddComponent<UIButton>("BuyBtn");
			Price = AddComponent<UITextmesh>("BuyBtn/Text");
			// Have = AddComponent<UIEmptyView>("Have");
		}
		public void SetData(int id, Action<int, int> onClickItem)
		{
			clothId = id;
			var config = ClothConfigCategory.Instance.Get(clothId);
			ClothItem.SetData(config, onClickItem, config.Module);
			// var have = PlayerDataManager.Instance.GetItemCount(clothId) > 0;
			AdBtn.SetActive(/*!have && */config.GetWay == 0 && AdManager.Instance.PlatformHasAD());
			BuyBtn.SetActive(/*!have && */(config.GetWay == 1 || !AdManager.Instance.PlatformHasAD()));
			// Have.SetActive(have);
			this.AdBtn.SetOnClick(OnClickAdBtn);
			this.BuyBtn.SetOnClick(OnClickBuyBtn);
			Price.SetText(I18NManager.Instance.TranslateMoneyToStr(config.Price));
		}
		#endregion

		#region 事件绑定
		public void OnClickAdBtn()
		{
			if (PlayerDataManager.Instance.GetItemCount(clothId) > 0)
			{
				UIManager.Instance.OpenBox<UIToast, I18NKey>(UIToast.PrefabPath,I18NKey.Text_Buy_Already).Coroutine();
				return;
			}
			if(AdManager.Instance.PlatformHasAD()) PlayAdAsync().Coroutine();
		}
        
		private async ETTask PlayAdAsync()
		{
			AdBtn.SetInteractable(false);
			try
			{
				var res = await AdManager.Instance.PlayAd();
				if (res)
				{
					PlayerDataManager.Instance.ChangeItem(clothId, 1);
					UIManager.Instance.OpenBox<UIToast, I18NKey>(UIToast.PrefabPath,I18NKey.Text_Buy_Success).Coroutine();
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex);
			}
			finally
			{
				AdBtn.SetInteractable(true);
			}
		}
		public void OnClickBuyBtn()
		{
			if (PlayerDataManager.Instance.GetItemCount(clothId) > 0)
			{
				UIManager.Instance.OpenBox<UIToast, I18NKey>(UIToast.PrefabPath,I18NKey.Text_Buy_Already).Coroutine();
				return;
			}
			var config = ClothConfigCategory.Instance.Get(clothId);
			if (PlayerDataManager.Instance.TotalMoney < config.Price)
			{
				UIManager.Instance.OpenBox<UIToast, I18NKey>(UIToast.PrefabPath,I18NKey.Notice_Common_LackOfMoney).Coroutine();
				return;
			}

			PlayerDataManager.Instance.ChangeMoney(-config.Price);
			PlayerDataManager.Instance.ChangeItem(clothId, 1);
			UIManager.Instance.OpenBox<UIToast, I18NKey>(UIToast.PrefabPath,I18NKey.Text_Buy_Success).Coroutine();
		}
		#endregion
	}
}
