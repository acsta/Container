using System.Collections.Generic;
using UnityEngine;

namespace TaoTie
{
    public class UIAssistantView : UIBaseView, IOnCreate, IOnEnable, IOnDisable
    {
        public const string PrefabPath = "UIGame/UIAuction/Prefabs/UIAssistantView.prefab";
        public UIAnimator Talk;
        public UITextmesh TalkText;
        public UIAnimator Assistant;
        public LinkedList<string> talkList = new LinkedList<string>();
        public UIEmptyView Hand;

        private ETCancellationToken cancellationToken;
        public void OnCreate()
        {
            this.Hand = this.AddComponent<UIEmptyView>("GameHand");
            Assistant = AddComponent<UIAnimator>("AssistantHold/Assistant");
            TalkText = AddComponent<UITextmesh>("AssistantHold/Assistant/Talk/Text");
            Talk = AddComponent<UIAnimator>("AssistantHold/Assistant/Talk");
        }

        public void OnEnable()
        {
            Hand.SetActive(false);
            Talk.SetActive(false);
            Messager.Instance.AddListener<string,bool>(0,MessageId.AssistantTalk,ShowTalk);
            Messager.Instance.AddListener<Transform,int>(0,MessageId.GuideBox,SetActiveObj);
            
        }

        public void OnDisable()
        {
            cancellationToken?.Cancel();
            cancellationToken = null;
            talkList.Clear();
            Messager.Instance.RemoveListener<string,bool>(0,MessageId.AssistantTalk,ShowTalk);
            Messager.Instance.RemoveListener<Transform,int>(0,MessageId.GuideBox,SetActiveObj);
        }

        private void SetActiveObj(Transform entity, int confId)
        {
            if (entity == null)
            {
                Hand.SetActive(false);
            }
            else
            {
                var mainCamera = CameraManager.Instance.MainCamera();
                var config = UnitConfigCategory.Instance.Get(confId);
                Vector2 pt = UIManager.Instance.ScreenPointToUILocalPoint(GetRectTransform(),
                    mainCamera.WorldToScreenPoint(entity.position + Vector3.up * (config?.Height ?? 0)));
                Hand.GetRectTransform().anchoredPosition = pt;
                Hand.SetActive(true);
            }
        }

        public void ShowTalk(string content, bool append)
        {
            if (!append || cancellationToken == null)
            {
                cancellationToken?.Cancel();
                cancellationToken = new ETCancellationToken();
                talkList.Clear();
                talkList.AddLast(content);
                ShowTalkAsync(cancellationToken).Coroutine();
            }
            else
            {
                talkList.AddLast(content);
            }
        }

        private async ETTask ShowTalkAsync(ETCancellationToken cancel)
        {
            Assistant.CrossFade("Assistant_Enter");
            await TimerManager.Instance.WaitAsync(200, cancel);
            if (cancel.IsCancel())
            {
                Assistant.Play("Assistant_Leave").Coroutine();
                return;
            }
            Talk.SetActive(true);
            TalkText.SetText("");
            await TimerManager.Instance.WaitAsync(1, cancel);
            if (cancel.IsCancel()) return;
            var content = talkList.First.Value;
            TalkText.SetText(content);
            talkList.RemoveFirst();
            SoundManager.Instance.PlaySound("Audio/Game/bubble.mp3");
            await TimerManager.Instance.WaitAsync(2000, cancel);
            if (cancel.IsCancel()) return;

            if (talkList.Count != 0)
            {
                ShowTalkAsync(cancel).Coroutine();
                return;
            }
            Talk.SetActive(false);
            Assistant.CrossFade("Assistant_Leave");
            cancellationToken = null;
        }
    }
}