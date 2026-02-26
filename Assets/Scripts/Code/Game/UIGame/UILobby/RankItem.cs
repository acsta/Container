using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace TaoTie
{
	public class RankItem : UIBaseContainer, IOnCreate
	{
		public UIImage Bg0;
		public UIImage Bg1;
		public UIImage Bg2;
		public UIImage Flag0;
		public UIImage Flag1;
		public UIImage Flag2;
		public UITextmesh Flag3;
		public UIRawImage Icon;
		public UIImage Bg;
		public UITextmesh Name;
		public UITextmesh Value;
		public UIImage IconNone;
		

		#region override
		public void OnCreate()
		{
			IconNone = AddComponent<UIImage>("IconNone");
			Bg = AddComponent<UIImage>();
			this.Bg0 = this.AddComponent<UIImage>("Bg0");
			this.Bg1 = this.AddComponent<UIImage>("Bg1");
			this.Bg2 = this.AddComponent<UIImage>("Bg2");
			this.Flag0 = this.AddComponent<UIImage>("Flag0");
			this.Flag1 = this.AddComponent<UIImage>("Flag1");
			this.Flag2 = this.AddComponent<UIImage>("Flag2");
			this.Flag3 = this.AddComponent<UITextmesh>("Flag3");
			this.Icon = this.AddComponent<UIRawImage>("Icon");
			this.Name = this.AddComponent<UITextmesh>("Name");
			this.Value = this.AddComponent<UITextmesh>("Value");
		}

		#endregion
		

		public void SetData(int index, bool isMe, RankInfo info)
		{
			Bg.SetEnabled(isMe || index > 2);
			Bg0.SetActive(!isMe && index == 0);
			Bg1.SetActive(!isMe && index == 1);
			Bg2.SetActive(!isMe && index == 2);
			Flag0.SetActive(index == 0);
			Flag1.SetActive(index == 1);
			Flag2.SetActive(index == 2);
			Flag3.SetActive(index > 2 || index < 0);
			if (isMe)
			{
				Name.SetI18NKey(I18NKey.Text_Rank_I);
			}
			else
			{
				Name.SetText(info?.NickName);
			}

			var color = !isMe && index < 3 ? "#7F4500" : GameConst.COMMON_TEXT_COLOR;
			if (isMe) color = GameConst.WHITE_COLOR;
			Name.SetTextColor(color);
			Value.SetNum(isMe?PlayerDataManager.Instance.TotalMoney : info?.Money);
			Value.SetTextColor(color);
			Flag3.SetText(index > 99 || index < 0 ? "99+" : (index + 1).ToString());
			string url = isMe ? PlayerDataManager.Instance.Avatar : info?.Avatar;
			if (!string.IsNullOrEmpty(url))
			{
				Icon.SetOnlineTexturePath(url, GameConst.DefaultImage).Coroutine();
				Icon.SetActive(true);
				IconNone.SetActive(false);
			}
			else
			{
				IconNone.SetActive(true);
				Icon.SetActive(false);
			}
		}
	}
}
