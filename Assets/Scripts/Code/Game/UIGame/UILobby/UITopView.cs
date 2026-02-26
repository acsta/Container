using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace TaoTie
{
	public class UITopView : UIBaseView, IOnCreate, IOnEnable,IOnWidthPaddingChange
	{
		public static string PrefabPath => "UIGame/UILobby/Prefabs/UITopView.prefab";
		public UICashGroup Top;
		

		#region override
		public void OnCreate()
		{
			this.Top = this.AddComponent<UICashGroup>("Top");
		}
		public void OnEnable()
		{
		}
		#endregion

	}
}
