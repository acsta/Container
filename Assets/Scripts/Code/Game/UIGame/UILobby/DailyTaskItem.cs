using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UIElements.Image;

namespace TaoTie
{
    public struct TaskItemData
    {
        public float currProgress;
        public int showIndex;
    }
    
    public class DailyTaskItem: UIBaseContainer, IOnCreate
    {
        public UITextmesh Title;
        public UITextmesh Progress;
        public UIAnimator Animator2;
        public UIImage Icon;
        public UIImage IconBg;
        public UITextmesh Rewards;
        public UIImage ProgressMask;
        public UIPointerClick PointerClick;
        public UIEmptyView Over;
        public UIImage Mask;
        public UIImage MaskIcon;
        public UIImage Coin;
        public UITextmesh ValueText2;
        public UIEmptyView Mask2;
        public UIImage Light;
        public UIMonoBehaviour<CanvasGroup> CanvasGroup;

        private HashSet<int> locks;
        private int configId;
        public TaskConfig Config => TaskConfigCategory.Instance.Get(configId);
        private bool isAnim;
        public void OnCreate()
        {
            CanvasGroup = AddComponent<UIMonoBehaviour<CanvasGroup>>("Content");
            Light = AddComponent<UIImage>("Content/Light");
            Animator2 = AddComponent<UIAnimator>("Content");
            this.ValueText2 = AddComponent<UITextmesh>("Content/Progress/Mask/ValueText");
            Mask2 = AddComponent<UIEmptyView>("Content/Progress/Mask");
            Coin = AddComponent<UIImage>("Content/Rewards/Image");
            PointerClick = AddComponent<UIPointerClick>();
            Title = AddComponent<UITextmesh>("Content/IconBg/Bg/Name");
            Progress = AddComponent<UITextmesh>("Content/Progress/Value");
            Icon = AddComponent<UIImage>("Content/IconBg/ItemIcon");
            IconBg = AddComponent<UIImage>("Content/IconBg");
            Rewards = AddComponent<UITextmesh>("Content/Rewards/Value");
            ProgressMask = AddComponent<UIImage>("Content/Progress/Progress");
            Over = AddComponent<UIEmptyView>("Content/Over");
            Mask = AddComponent<UIImage>("Content/Mask");
            MaskIcon = AddComponent<UIImage>("Content/Mask/Image");
        }

        public void SetData(TaskConfig task,HashSet<int> locks)
        {
            isAnim = false;
            this.locks = locks;
            configId = task.Id;
            RefreshLockState();
            var config = Config;
            if (config == null)
            {
                return;
            }
            
            if (config.ItemType == 0)
            {
                ItemConfig itemConfig = ItemConfigCategory.Instance.Get(config.ItemId);
                Icon.SetSpritePath(itemConfig.ItemPic).Coroutine();
                Title.SetText(I18NManager.Instance.I18NGetText(itemConfig));
            }
            else if (config.ItemType == 1)
            {
                ContainerConfig containerConfig = ContainerConfigCategory.Instance.Get(config.ItemId);
                Icon.SetSpritePath(containerConfig.Icon).Coroutine();
                Title.SetText(I18NManager.Instance.I18NGetText(containerConfig));
            }
            else
            {
                Log.Error("未指定的任务类型" + config.ItemType);
                Icon.SetSpritePath(GameConst.DefaultImage).Coroutine();
                Title.SetI18NKey(I18NKey.Global_Unknow);
            }
            // IconBg.SetColor(GameConst.RareColor[Mathf.Clamp(config.Rare, 1, GameConst.RareColor.Length) - 1]);
            IconBg.SetSpritePath(config.Rare > 3 ?"UIGame/UILobby/Atlas/task_item_bg2.png":"UIGame/UILobby/Atlas/task_item_bg.png").Coroutine();
            bool isOver = PlayerDataManager.Instance.GetTaskState(config.Id, out var step);
            var text = $"{Mathf.Min(step, config.ItemCount)}/{config.ItemCount}";
            Progress.SetText(text);
            ValueText2.SetText(text);
            float val = (float) step / config.ItemCount;
            ProgressMask.SetFillAmount(val);
            var rect = Mask2.GetRectTransform();
            var size = (rect.parent as RectTransform).sizeDelta;
            rect.offsetMin = new Vector2(val * size.x, 0);
            if (config.RewardType == 2)
            {
                Rewards.SetNum(config.RewardCount);
            }
            else
            {
                Rewards.SetText("None");
            }

            if (isOver)
            {
                // Flag.SetActive(false);
                PointerClick.RemoveOnClick();
                Over.SetActive(false);
                Mask.SetActive(true);
                Animator2.SetEnable(false);
                Animator2.GetTransform().localScale = Vector3.one;
                Light.SetActive(false);
            }
            else if (step >= config.ItemCount)
            {
                // Flag.SetActive(locks != null);
                PointerClick.SetOnClick(OnClickComplex);
                Over.SetActive(true);
                Mask.SetActive(false);
                Animator2.SetEnable(true);
                Light.SetActive(true);
            }
            else
            {
                // Flag.SetActive(locks != null);
                PointerClick.SetOnClick(OnClickDetails);
                Over.SetActive(false);
                Mask.SetActive(false);
                Animator2.SetEnable(false);
                Animator2.GetTransform().localScale = Vector3.one;
                Light.SetActive(false);
            }
            PointerClick.SetEnabled(true);
        }

        public void OnClickFlag()
        {
            if (locks != null && locks.Contains(configId))
            {
                locks.Remove(configId);
            }
            else
            {
                locks?.Add(configId);
            }

            RefreshLockState();
        }

        private void RefreshLockState()
        {
            // if (locks != null)
            // {
            //     bool isLock = locks.Contains(configId);
            //     Flag.SetBtnGray(!isLock, true, false).Coroutine();
            //     Flag.GetRectTransform().localEulerAngles = new Vector3(0, 0, !isLock ? -45 : 0);
            // }
            // else
            // {
            //     Flag.SetActive(false);
            // }
        }

        public void OnClickComplex()
        {
            OnClickComplexAsync().Coroutine();
        }
        private async ETTask OnClickComplexAsync()
        {
            if(isAnim) return;
            if (Config == null) return;
            isAnim = true;
            bool isOver = PlayerDataManager.Instance.GetTaskState(configId, out var step);
            if (!isOver && step >= Config.ItemCount)
            {
                Animator2.SetEnable(false);
                Animator2.GetTransform().localScale = Vector3.one;
                var top = UIManager.Instance.GetView<UITopView>(1);
                if (top != null && Config.RewardType == 2)
                {
                    var rect = this.GetRectTransform();
                    await PlayStampAnim();
                    var starTime = TimerManager.Instance.GetTimeNow();
                    var animTime = 200f;
                    while (true)
                    {
                        await TimerManager.Instance.WaitAsync(1);
                        
                        var timeNow = TimerManager.Instance.GetTimeNow();
                        Over.GetRectTransform().GetComponent<CanvasGroup>().alpha = Mathf.Lerp(1f, 0f, Mathf.Clamp01((timeNow - starTime) / animTime));
                        if (timeNow - starTime >= animTime)
                        {
                            break;
                        }
                    }
                    Over.SetActive(false);
                    Over.GetRectTransform().GetComponent<CanvasGroup>().alpha = 1f;
                    await top.Top.DoMoneyMoveAnim(Config.RewardCount, rect.position, new Vector2(200, 300) , Mathf.Max(1, Config.Rare * 2));
                    Light.SetActive(false);
                }
                bool res = PlayerDataManager.Instance.ComplexTask(configId);
                if (res)
                {
                    // Flag.SetActive(false);
                    var view = UIManager.Instance.GetView<UIMarketView>(1);
                    
                    view?.OnTaskComplete();
                }
                else
                {
                    Messager.Instance.Broadcast(0, MessageId.ChangeMoney);
                }
            }
            isAnim = false;
        }
        
        public void OnClickDetails()
        {
            OnClickAnim().Coroutine();
            UIManager.Instance.OpenWindow<UITaskDetailsWin, TaskConfig>(UITaskDetailsWin.PrefabPath, Config)
                .Coroutine();
        }
        
        private async ETTask OnClickAnim()
        {
            Animator2.SetEnable(true);
            await Animator2.Play("UIRestaurantTask_Click");
            Animator2.SetEnable(false);
            Animator2.GetTransform().localScale = Vector3.one;
        }

        private async ETTask PlayStampAnim()
        {
            SoundManager.Instance.PlaySound("Audio/Sound/Restaurant_Stamp.mp3");
            Mask.SetActive(false);
            MaskIcon.SetActive(false);
            var maskCanvasGroup = Mask.GetGameObject().GetComponent<CanvasGroup>();
            var maskIconCG = MaskIcon.GetGameObject().GetComponent<CanvasGroup>();
            MaskIcon.GetRectTransform().localScale = Vector3.one * 1.3f;
            maskCanvasGroup.alpha = 0f;
            maskIconCG.alpha = 0f;

            Mask.SetActive(true);
            var starTime = TimerManager.Instance.GetTimeNow();
            var animTime = 100f;
            while (true)
            {
                await TimerManager.Instance.WaitAsync(1);
                
                var timeNow = TimerManager.Instance.GetTimeNow();
                maskCanvasGroup.alpha = Mathf.Lerp(0f, 1f, (timeNow - starTime) / animTime);
                if (timeNow - starTime >= animTime)
                {
                    break;
                }
            }

            MaskIcon.SetActive(true);
            var oldScale = MaskIcon.GetRectTransform().localScale;
            var newScale = Vector3.one * 0.8f;
            starTime = TimerManager.Instance.GetTimeNow();
            animTime = 100f;
            while (true)
            {
                await TimerManager.Instance.WaitAsync(1);
                
                var timeNow = TimerManager.Instance.GetTimeNow();
                maskIconCG.alpha = Mathf.Lerp(0f, 1f, (timeNow - starTime) / animTime);
                MaskIcon.GetRectTransform().localScale = Vector3.Lerp(oldScale, newScale, (timeNow - starTime) / animTime);
                if (timeNow - starTime >= animTime)
                {
                    break;
                }
            }
            
            starTime = TimerManager.Instance.GetTimeNow();
            animTime = 200f;
            oldScale = newScale;
            while (true)
            {
                await TimerManager.Instance.WaitAsync(1);
                
                var timeNow = TimerManager.Instance.GetTimeNow();
                MaskIcon.GetRectTransform().localScale = Vector3.Lerp(oldScale, Vector3.one, (timeNow - starTime) / animTime);
                if (timeNow - starTime >= animTime)
                {
                    break;
                }
            }
        }
    }
}