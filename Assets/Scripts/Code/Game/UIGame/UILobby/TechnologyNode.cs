using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace TaoTie
{
	public class TechnologyNode : UIBaseContainer, IOnCreate
	{
		public UICopyGameObject Items;
		public UIImage Icon;
		public UITextmesh Title;
		public UIButton Coin;
		public UIEmptyView Unlock;
		public UITextmesh Text;
		public UIPointerClick Container;
		public UIAnimator Door;

		private int configId;
		public TechnologyTreeConfig Config => TechnologyTreeConfigCategory.Instance.Get(configId);

		private List<TechnologyTreeConfig> items;
		#region override
		public void OnCreate()
		{
			Container = AddComponent<UIPointerClick>("Content/Container");
			this.Items = this.AddComponent<UICopyGameObject>("Content/Items/Viewport/Content");
			this.Items.InitListView(0,GetItemsItemByIndex);
			this.Icon = this.AddComponent<UIImage>("Content/Container/Icon");
			this.Title = this.AddComponent<UITextmesh>("Content/Container/Text");
			this.Coin = this.AddComponent<UIButton>("Content/Container/Coin");
			Unlock = this.AddComponent<UIEmptyView>("Content/Container/Unlock");
			this.Text = this.AddComponent<UITextmesh>("Content/Container/Coin/Text");
			Door = AddComponent<UIAnimator>("Content/Items/Door");
			Coin.SetOnClick(OnClickContainer);
			Container.SetOnClick(OnClickContainer);
		}
		
		#endregion

		#region 事件绑定
		public void GetItemsItemByIndex(int index, GameObject obj)
		{
			var item = Items.GetUIItemView<TechnologyNodeItem>(obj);
			if (item == null)
			{
				item = Items.AddItemViewComponent<TechnologyNodeItem>(obj);
			}
			item.SetData(items[index],PlayerDataManager.Instance.IsUnlock(configId));
		}

		public void OnClickContainer()
		{
			UIManager.Instance.OpenWindow<UIUnlockWin, TechnologyTreeConfig>(UIUnlockWin.PrefabPath, Config).Coroutine();
		}
		#endregion
		
		public void SetData(TechnologyTreeConfig config)
		{
			configId = config.Id;
			Icon.SetSpritePath(config.Icon).Coroutine();
			Title.SetText(I18NManager.Instance.I18NGetText(config));
			bool isUnlock = PlayerDataManager.Instance.IsUnlock(configId);
			Door.SetActive(!isUnlock);
			Coin.SetActive(!isUnlock);
			Unlock.SetActive(isUnlock);
			if (!isUnlock)
			{
				if (Config.UnlockType == 1)
				{
					Text.SetNum(config.UnlockValue);
				}
				else
				{
					Log.Error("解锁类型不对 TechnologyTreeConfig id=" + configId);
				}
			}
			items = TechnologyTreeConfigCategory.Instance.GetPlayTypes(configId, false);
			Items.SetActive(items.Count>0 && isUnlock);
			if (items.Count > 0 && isUnlock)
			{
				Items.SetListItemCount(items.Count);
				Items.RefreshAllShownItem();
			}
		}

		public async ETTask UnlockDoor()
		{
			Coin.SetActive(false);
			Unlock.SetActive(true);
			await Door.Play("Black_Door_Open");
			Door.SetActive(false);
		}
	}
}
