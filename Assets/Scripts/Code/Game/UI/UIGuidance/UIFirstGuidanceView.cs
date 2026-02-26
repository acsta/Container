using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace TaoTie
{
	public class UIFirstGuidanceView : UIBaseView, IOnCreate, IOnEnable, IOnWidthPaddingChange
	{
		public static string PrefabPath => "UI/UIGuidance/Prefabs/UIFirstGuidanceView.prefab";
		public UIPointerClick Mask;
		public UIAnimator Animator;

		#region override
		public void OnCreate()
		{
			Animator = this.AddComponent<UIAnimator>();
			this.Mask = this.AddComponent<UIPointerClick>("Mask");
		}
		public void OnEnable()
		{
			this.Mask.SetOnClick(OnClickMask);
		}

		public override async ETTask CloseSelf()
		{
			GuidanceManager.Instance.NoticeEvent("Close_UIFirstGuidanceView");
			await Animator.Play("First_Guidance_Close");
			await base.CloseSelf();
		}

		#endregion

		#region 事件绑定
		public void OnClickMask()
		{
			CloseSelf().Coroutine();
		}
		#endregion
	}
}
