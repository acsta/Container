using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace TaoTie
{
	public class UIBubbleItem : UIBaseView, IOnCreate,IOnEnable<UIBubbleItem.BubbleData>,
		IOnEnable<UIBubbleItem.BubbleData,int,ETCancellationToken>,II18N
	{
		public struct BubbleData
		{
			public string front;
			public string end;
			public int emoji;
			public Vector3 worldSpace;
			public bool isPlayer;
			public bool anim;
			public int iconType;
			public bool raiseBubble;
		}
		public const string PrefabPath = "UIGame/UIAuction/Prefabs/UIBubbleItem.prefab";
		public UIImage Root;
		public UIImage Text;
		public UITextmesh Front;
		public UIImage[] Icon;
		public UIEmptyView Price;

		public UITextmesh End;
		public UIAnimator Animator;

		public UIImage[] Emoji;

		#region override
		public void OnCreate()
		{
			Animator = AddComponent<UIAnimator>();
			this.Root = this.AddComponent<UIImage>("Root");
			this.Text = this.AddComponent<UIImage>("Root/Text");
			this.Front = this.AddComponent<UITextmesh>("Root/Front");
			Icon = new UIImage[3];
			for (int i = 0; i < 3; i++)
			{
				this.Icon[i] = this.AddComponent<UIImage>("Root/Price/Icon"+(i+1));
			}

			Emoji = new UIImage[7];
			for (int i = 0; i < 7; i++)
			{
				Emoji[i] = AddComponent<UIImage>("Emoji/" + i);
			}
			Price =  this.AddComponent<UIEmptyView>("Root/Price");
			this.End = this.AddComponent<UITextmesh>("Root/Price/End");
		}

		public void OnEnable(BubbleData data)
		{
			SetData(data.front, data.end, data.emoji, data.worldSpace, data.isPlayer, data.anim, data.iconType,
				data.raiseBubble);
			OnEnableAsync().Coroutine();
		}
		
		public void OnEnable(BubbleData data, int time, ETCancellationToken cancel)
		{
			SetData(data.front, data.end, data.emoji, data.worldSpace, data.isPlayer, data.anim, data.iconType,
				data.raiseBubble);
			OnEnableAsync(time,cancel).Coroutine();
		}
		
		private async ETTask OnEnableAsync(int time = 1500, ETCancellationToken cancel = null)
		{
			await TimerManager.Instance.WaitAsync(time, cancel);
			CloseSelf().Coroutine();
		}
		
		public void OnLanguageChange()
		{
			Text.SetSpritePath($"UIGame/UIAuction/Atlas/myprice_{I18NManager.Instance.CurLangType}.png").Coroutine();
		}
		#endregion

		public void SetData(string front, string end, int emoji, Vector3 worldSpace, bool isPlayer, bool anim = true, 
			int iconType = 3, bool raiseBubble = false)
		{
			Root.SetSpritePath(
				$"UIGame/UIAuction/Atlas/frame_dialogue{(raiseBubble ? 1 : 0)}.png").Coroutine();
			Animator.SetEnable(anim);
			Text.SetActive(isPlayer);
			for (int i = 0; i < 7; i++)
			{
				Emoji[i].SetActive(i == emoji);
			}
			
			Front.SetActive(!string.IsNullOrEmpty(front));
			Front.SetText(front);
			for (int i = 1; i <= 3; i++)
			{
				Icon[i - 1].SetActive(!string.IsNullOrEmpty(end) && i == iconType);
			}
			Price.SetActive(!string.IsNullOrEmpty(end));
			End.SetText(end);
			var rect = GetRectTransform();
			var mainCamera = CameraManager.Instance.MainCamera();
			if (mainCamera != null)
			{
				Vector2 pt = UIManager.Instance.ScreenPointToUILocalPoint(GetRectTransform().parent as RectTransform, 
					mainCamera.WorldToScreenPoint(worldSpace));
				rect.anchoredPosition = pt;
				rect.transform.localScale = pt.x >= -1 ? Vector3.one : Vector3.one + 2 * Vector3.left;
				Text.GetRectTransform().localScale = rect.transform.localScale;
				Price.GetRectTransform().localScale = rect.transform.localScale;
				this.Front.GetRectTransform().localScale = rect.transform.localScale;
			}
		}
	}
}
