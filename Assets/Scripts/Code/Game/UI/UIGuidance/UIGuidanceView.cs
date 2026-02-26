using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace TaoTie
{
	public class UIGuidanceView : UIBaseView, IOnCreate, IOnEnable,IOnDisable,IUpdate
	{
		public static string PrefabPath => "UI/UIGuidance/Prefabs/UIGuidanceView.prefab";
		public UIAnimator Talk;
		public UITextmesh TalkText;
		public UIAnimator Assistant;
		public UIEmptyView AssistantHold;
		public UIImage Mask;
		public string content;

		private ETCancellationToken cancellationToken;
		public UIEmptyView Hand;
		public UIEmptyView GameHand;

		public void OnCreate()
		{
			Mask = AddComponent<UIImage>("Mask");
			AssistantHold = AddComponent<UIEmptyView>("AssistantHold");
			this.Hand = this.AddComponent<UIEmptyView>("Hand");
			GameHand = this.AddComponent<UIEmptyView>("GameHand");
			Assistant = AddComponent<UIAnimator>("AssistantHold/Assistant");
			TalkText = AddComponent<UITextmesh>("AssistantHold/Assistant/Talk/Text");
			Talk = AddComponent<UIAnimator>("AssistantHold/Assistant/Talk");
		}

		public void OnEnable()
		{
			Assistant.SetBool("Open", false);
			Talk.SetActive(false);
			Hand.SetActive(false);
			GameHand.SetActive(false);
			Messager.Instance.AddListener<string>(0, MessageId.GuidanceTalk, ShowTalk);
			Messager.Instance.AddListener<string, int>(0, MessageId.GuidanceTalk, ShowTalk);
			Messager.Instance.AddListener<Transform, int>(0,MessageId.GuideBox2,SetActiveObj);
		}

		public void OnDisable()
		{
			cancellationToken?.Cancel();
			cancellationToken = null;
			content = null;
			Messager.Instance.RemoveListener<string>(0, MessageId.GuidanceTalk, ShowTalk);
			Messager.Instance.RemoveListener<string, int>(0, MessageId.GuidanceTalk, ShowTalk);
			Messager.Instance.RemoveListener<Transform, int>(0,MessageId.GuideBox2,SetActiveObj);
		}
		private void SetActiveObj(Transform entity, int confId)
		{
			if (entity == null)
			{
				GameHand.SetActive(false);
			}
			else
			{
				var mainCamera = CameraManager.Instance.MainCamera();
				var config = UnitConfigCategory.Instance.Get(confId);
				Vector2 pt = UIManager.Instance.ScreenPointToUILocalPoint(GetRectTransform(),
					mainCamera.WorldToScreenPoint(entity.position + Vector3.up * (config?.Height ?? 0)));
				GameHand.GetRectTransform().anchoredPosition = pt;
				GameHand.SetActive(true);
			}
		}
		public void Update()
		{
			if (IsValid(GuidanceManager.GuideTarget))
			{
				Hand.SetActive(true);
				var point = GuidanceManager.GuideTarget.transform.position;
				Hand.GetTransform().position = point;
				if (GuidanceManager.ShowMask)
				{
					var size = GuidanceManager.GuideTarget.GetComponent<RectTransform>()?.sizeDelta ??
					           Vector2.one * 100;
					Mask.GetRectTransform().sizeDelta = size * 2;
					Mask.GetRectTransform().position = point;
					Mask.SetActive(GuidanceManager.ShowMask);
				}

				if (IAuctionManager.Instance != null)
				{
					AssistantHold.GetRectTransform().anchoredPosition = Vector2.zero;
				}
				else
				{
					AssistantHold.GetRectTransform().anchoredPosition =
						new Vector2(0, Hand.GetRectTransform().anchoredPosition.y >= 0 ? 0 : Define.DesignScreenHeight / 2);
				}
			}
			else
			{
				Mask.SetActive(false);
				Hand.SetActive(false);
			}
		}

		private bool IsValid(GameObject obj)
		{
			if (obj == null) return false;
			if (obj.GetComponent<Button>() is Button b)
			{
				return b.enabled && b.interactable;
			}
			if (obj.GetComponent<PointerClick>() is PointerClick p)
			{
				return p.enabled;
			}
			return true;
		}

		public void ShowTalk(string content)
		{
			ShowTalk(content, -1);
		}

		public void ShowTalk(string content, int during)
		{
			if(content == this.content) return;
			this.content = content;
			if (string.IsNullOrEmpty(content))
			{
				cancellationToken?.Cancel();
				cancellationToken = null;
				Assistant.SetBool("Open", false);
				this.content = null;
				Talk.SetActive(false);
				TalkText.SetText(null);
			}
			else
			{
				if (cancellationToken == null)
				{
					cancellationToken = new ETCancellationToken();
					ShowTalkAsync(cancellationToken, during).Coroutine();
				}
				else
				{
					Assistant.SetBool("Open", true);
					TalkText.SetText(content);
				}
			}
		}
		
		private async ETTask ShowTalkAsync(ETCancellationToken cancel,int during)
		{
			Assistant.SetBool("Open", true);
			await TimerManager.Instance.WaitAsync(200, cancel);
			if (cancel.IsCancel())
			{
				Assistant.SetBool("Open", false);
				return;
			}
			Talk.SetActive(true);
			TalkText.SetText("");

			await TimerManager.Instance.WaitAsync(1, cancel);
			if (cancel.IsCancel()) return;
			TalkText.SetText(content);
			SoundManager.Instance.PlaySound("Audio/Game/bubble.mp3");
			if (during > 0)
			{
				await TimerManager.Instance.WaitAsync(during, cancel);
			}
			else
			{
				while (!cancel.IsCancel())
				{
					await TimerManager.Instance.WaitAsync(1, cancel);
				}
			}
			if (cancel.IsCancel()) return;
			Talk.SetActive(false);
			Assistant.SetBool("Open", false);
			content = null;
			TalkText.SetText(null);
			cancellationToken = null;
		}
	}
}
