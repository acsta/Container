using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace TaoTie
{
	public class UIReportItem : UIBaseContainer, IOnCreate, IOnEnable
	{
		public UITextmesh Title;
		public UIImage Icon;
		public UITextmesh Name;
		public UIImage Result;
		public UITextmesh Text;
		

		#region override
		public void OnCreate()
		{
			this.Title = this.AddComponent<UITextmesh>("Title");
			this.Icon = this.AddComponent<UIImage>("Content/Icon");
			this.Name = this.AddComponent<UITextmesh>("Content/Name");
			this.Result = this.AddComponent<UIImage>("Content/Result");
			this.Text = this.AddComponent<UITextmesh>("Content/Result/Text");
			Title.SetI18NKey(I18NKey.Text_Game_Stage);
		}
		public void OnEnable()
		{
		}
		#endregion

		#region 事件绑定

		public void SetData(AuctionReport data)
		{
			Title.SetI18NText(data.Index + 1);
			var container = ContainerConfigCategory.Instance.Get(data.ContainerId);
			if (container != null)
			{
				Icon.SetSpritePath(container.Icon).Coroutine();
				Name.SetText(I18NManager.Instance.I18NGetText(container));
			}
			else
			{
				Name.SetText(I18NManager.Instance.I18NGetText(I18NKey.Text_Empty));
				Icon.SetSpritePath(GameConst.DefaultImage).Coroutine();
			}

			if (data.Type == ReportType.Self)
			{
				var money = data.FinalUserWin;
				if (money < 0) money = -money;
				var get = I18NManager.Instance.TranslateMoneyToStr(money);
				if (data.FinalUserWin >= 0)
				{
					Text.SetI18NKey(I18NKey.Text_Game_Win2, get);
					Result.SetColor(GameConst.GREEN_COLOR);
				}
				else
				{
					Text.SetI18NKey(I18NKey.Text_Game_Loss2, get);
					Result.SetColor(GameConst.RED_COLOR);
				}
			}
			else if(data.Type == ReportType.Others)
			{
				if (data.RaiseSuccessCount > 0)
				{
					var get = data.FinalUserWin;
					if (get > BigNumber.Zero)
					{
						Text.SetI18NKey(I18NKey.Text_Game_Win2, get);
						Result.SetColor(GameConst.GREEN_COLOR);
					}
					else
					{
						Text.SetI18NKey(I18NKey.Text_Auction_GiveUp);
						Result.SetColor(GameConst.GRAY_COLOR);
					}
				}
				else
				{
					Text.SetI18NKey(I18NKey.Text_Auction_GiveUp);
					Result.SetColor(GameConst.GRAY_COLOR);
				}
			}
			else if(data.Type == ReportType.Pass)
			{
				Text.SetI18NKey(I18NKey.Text_Failed_Auction);
				Result.SetColor(GameConst.GRAY_COLOR);
			}
			else if (data.Type == ReportType.NoResult)
			{
				Text.SetI18NKey(I18NKey.Text_Auction_GiveUp);
				Result.SetColor(GameConst.GRAY_COLOR);
			}
			else 
			{
				Log.Error("未处理的拍卖结果"+data.Type);
				Text.SetI18NKey(I18NKey.Text_Auction_No_Result);
				Result.SetColor(GameConst.GRAY_COLOR);
			}
		}
		#endregion
	}
}
