using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace TaoTie
{
	public class AuctionSelectItem : UIBaseContainer, IOnCreate
	{

		public UITextmesh Text;
		public UIImage Icon;
		public UIImage Lock;
		
		#region override
		public void OnCreate()
		{
			this.Text = this.AddComponent<UITextmesh>("Icon/Lock/Text");
			this.Icon = this.AddComponent<UIImage>("Icon");
			this.Lock = this.AddComponent<UIImage>("Icon/Lock");
			Text.SetI18NKey(I18NKey.Text_Auction_LvLock);
		}
		public void SetData(LevelConfig config)
		{
			bool islock = config.Id > 0 && PlayerDataManager.Instance.GetMaxLevel() < config.Id;
			Lock.SetActive(islock);
			Icon.SetSpritePath(config.Icon).Coroutine();
			if (islock)
			{
				var lvs = RestaurantConfigCategory.Instance.GetAllList();
				var max = 0;
				for (int i = 0; i < lvs.Count; i++)
				{
					if (lvs[i].MaxLevelId >= config.Id)
					{
						max = Mathf.Max(max,lvs[i].Level);
						break;
					}
				}
				Text.SetI18NText(max);
			}

			Icon.SetColor(islock ? Color.gray : Color.white);
		}
		#endregion
	}
}
