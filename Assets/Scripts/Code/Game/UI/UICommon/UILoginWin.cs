using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace TaoTie
{
	public class UILoginWin : UIBaseView, IOnCreate, IOnEnable<ETTask<string>>
	{
		public static string PrefabPath => "UI/UICommon/Prefabs/UILoginWin.prefab";
		public UIButton BtnLogin;
		public UIInputTextmesh InputField;

		private ETTask<string> task;
		#region override
		public void OnCreate()
		{
			this.BtnLogin = this.AddComponent<UIButton>("Win/Confirm");
			this.InputField = this.AddComponent<UIInputTextmesh>("Win/Psw/UserName");
		}
		public void OnEnable(ETTask<string> task)
		{
			SoundManager.Instance.PlaySound("Audio/Sound/Win_Open.mp3");
			InputField.SetText(CacheManager.Instance.GetString(CacheKeys.LastToken, Guid.NewGuid().ToString()));
			this.task = task;
			this.BtnLogin.SetOnClick(OnClickBtnLogin);
		}
		#endregion

		#region 事件绑定

		public void OnClickBtnLogin()
		{
			var text = InputField.GetText();
			if (!string.IsNullOrEmpty(text))
			{
				task.SetResult(text);
				CloseSelf().Coroutine();
				task = null;
			}
		}
		#endregion
	}
}
