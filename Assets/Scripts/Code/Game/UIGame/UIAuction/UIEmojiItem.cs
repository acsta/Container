using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace TaoTie
{
	public class UIEmojiItem : UIBaseView, IOnCreate, IOnEnable<int,Vector3>,IOnEnable<int,Vector3,int,ETCancellationToken>
	{
		public const string PrefabPath = "UIGame/UIAuction/Prefabs/UIEmojiItem.prefab";
		public UIImage[] Emoji;

		#region override
		public void OnCreate()
		{
			Emoji = new UIImage[7];
			for (int i = 0; i < 7; i++)
			{
				Emoji[i] = AddComponent<UIImage>("Emoji/" + i);
			}
		}
		public void OnEnable(int emoji, Vector3 worldSpace)
		{
			SetData(emoji, worldSpace);
			OnEnableAsync().Coroutine();
		}
		public void OnEnable(int emoji, Vector3 worldSpace, int time, ETCancellationToken cancel)
		{
			SetData(emoji, worldSpace);
			OnEnableAsync(time,cancel).Coroutine();
		}

		private async ETTask OnEnableAsync(int time = 1500, ETCancellationToken cancel = null)
		{
			await TimerManager.Instance.WaitAsync(time, cancel);
			CloseSelf().Coroutine();
		}
		#endregion

		public void SetData(int emoji, Vector3 worldSpace)
		{
			for (int i = 0; i < 7; i++)
			{
				Emoji[i].SetActive(i == emoji);
			}
			var rect = GetRectTransform();
			var mainCamera = CameraManager.Instance.MainCamera();
			if (mainCamera != null)
			{
				Vector2 pt = UIManager.Instance.ScreenPointToUILocalPoint(GetRectTransform().parent as RectTransform,
					mainCamera.WorldToScreenPoint(worldSpace));
				rect.anchoredPosition = pt;
			}
		}
	}
}
