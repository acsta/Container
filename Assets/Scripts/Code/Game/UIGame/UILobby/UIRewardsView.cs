using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace TaoTie
{
	public class UIRewardsView : UIBaseView, IOnCreate, IOnEnable<int, long>
	{
		public static string PrefabPath => "UIGame/UILobby/Prefabs/UIRewardsView.prefab";
		public UIImage Icon;
		public UIButton Get;
		public UITextmesh Count;

		#region override
		public void OnCreate()
		{
			this.Icon = this.AddComponent<UIImage>("Icon");
			this.Get = this.AddComponent<UIButton>("Get");
			Count = AddComponent<UITextmesh>("Icon/Text");
		}
		public void OnEnable(int id, long count)
		{
			var conf = ItemConfigCategory.Instance.Get(id);
			Icon.SetSpritePath(conf.ItemPic,true).Coroutine();
			this.Get.SetOnClick(OnClickGet);
			Count.SetActive(count > 1);
			Count.SetText(count.ToString());
		}
		#endregion

		#region 事件绑定
		public void OnClickGet()
		{
			CloseSelf().Coroutine();
		}
		#endregion
	}
}
