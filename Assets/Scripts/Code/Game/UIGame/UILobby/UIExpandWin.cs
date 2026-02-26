using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace TaoTie
{
	public class UIExpandWin : UIBaseView, IOnCreate, IOnEnable
	{
		public static string PrefabPath => "UIGame/UILobby/Prefabs/UIExpandWin.prefab";
		public UIButton Close;
		public UITextmesh Desc;
		public UIButton ButtonAd;
		public UIAnimator UICommonWin;


		#region override
		public void OnCreate()
		{
			UICommonWin = AddComponent<UIAnimator>("UICommonWin");
			this.Close = this.AddComponent<UIButton>("UICommonWin/Win/Close");
			this.Desc = this.AddComponent<UITextmesh>("UICommonWin/Win/Content/Desc");
			this.ButtonAd = this.AddComponent<UIButton>("UICommonWin/Win/Content/ButtonAd");
			Desc.SetI18NKey(I18NKey.Text_Restaurant_Profit_Hour);
		}
		public void OnEnable()
		{
			SoundManager.Instance.PlaySound("Audio/Sound/Win_Open.mp3");
			this.Close.SetOnClick(OnClickClose);
			this.ButtonAd.SetOnClick(OnClickButtonAd);
			Desc.SetI18NText(PlayerDataManager.Instance.GetMaxDeltaTime());
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

		public void OnClickButtonAd()
		{
			if (AdManager.Instance.PlatformHasAD())
			{
				ButtonAd.SetInteractable(false);
				PlayAdAsync().Coroutine();
			}
		}
		public async ETTask PlayAdAsync()
		{
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
				ButtonAd.SetInteractable(true);
				Desc.SetI18NText(PlayerDataManager.Instance.GetMaxDeltaTime());
			}
		}
		#endregion
	}
}
