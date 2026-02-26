using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace TaoTie
{
	public class EffectItem : UIBaseContainer, IOnCreate
	{
		public UITextmesh Type;
		public UITextmesh Details;
		

		#region override
		public void OnCreate()
		{
			this.Type = this.AddComponent<UITextmesh>("Type");
			this.Details = this.AddComponent<UITextmesh>("Details");
			Type.SetI18NKey(I18NKey.Text_Equip_Group_Count);
		}

		#endregion

		public void SetData(int effectType,int param,int count,bool active)
		{
			Type.SetI18NText(count);
			Details.SetI18NKey(Enum.Parse<I18NKey>("Text_Equip_Effect_" + effectType), param);
			SetDataAsync().Coroutine();
			Type.SetTextColor(active?GameConst.GREEN_COLOR:GameConst.GRAY_COLOR);
			Details.SetTextColor(active?GameConst.GREEN_COLOR:GameConst.GRAY_COLOR);
		}

		private async ETTask SetDataAsync()
		{
			await UnityLifeTimeHelper.WaitFrameFinish();
			int line = Details.GetLineCount();
			var x = this.GetRectTransform().sizeDelta.x;
			this.GetRectTransform().sizeDelta = new Vector2(x, 30 * line);
		}
	}
}
