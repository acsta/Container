using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace TaoTie
{
	public class UIEnterView : UIBaseView, IOnCreate, IOnEnable
	{
		public static string PrefabPath => "UI/UILoading/Prefabs/UIEnterView.prefab";
		public UIAnimator Aim;
		

		#region override
		public void OnCreate()
		{
			this.Aim = this.AddComponent<UIAnimator>("Aim");
		}
		public void OnEnable()
		{
			Aim.SetActive(false);
		}
		#endregion

		public async ETTask EnterTarget(GameObject target, bool autoClose = true)
		{
			if (target != null)
			{
				if (target.GetComponent<RectTransform>() == null)
				{
					var mainCamera = CameraManager.Instance.MainCamera();
					Vector2 pt = UIManager.Instance.ScreenPointToUILocalPoint(GetRectTransform(),
						mainCamera.WorldToScreenPoint(target.transform.position));
					Aim.GetRectTransform().anchoredPosition = pt;
				}
				else
				{
					Aim.GetRectTransform().position = target.transform.position;
				}
			}
			else
			{
				Aim.GetRectTransform().anchoredPosition = Vector2.zero;
			}
			Aim.SetActive(true);
			await Aim.Play("Enter");
			if(autoClose) CloseLater().Coroutine();
		}

		public async ETTask Exit()
		{
			await Aim.Play("Exit");
			CloseSelf().Coroutine();
		}
		private async ETTask CloseLater()
		{
			await TimerManager.Instance.WaitAsync(1000);
			CloseSelf().Coroutine();
		}
	}
}
