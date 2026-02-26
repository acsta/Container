using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace TaoTie
{
	public class RestaurantTask : UIBaseContainer, IOnCreate
	{
		public UITextmesh ItemName;
		public UIImage ItemIcon;
		public UISlider Slider;
		public UITextmesh ValueText;
		public UIEmptyView Over;
		public UIEmptyView Mask;
		public UIImage Bg;
		public UIEmptyView Bottom;
		public UIEmptyView Details;
		public UITextmesh Desc;
		public UITextmesh Rewards;
		public UITextmesh RewardsVal;
		public UIPointerClick PointerClick;
		public UIAnimator Animator;
		public UIAnimator Animator2;
		public UIImage Self;
		public UITextmesh ValueText2;
		public UIEmptyView Mask2;
		public UIImage LightImage;
		
		public TaskConfig Config { get; private set; }

		private Action<RestaurantTask> onClickOver;
		#region override
		public void OnCreate()
		{
			this.ValueText2 = AddComponent<UITextmesh>("Content/Bottom/Slider/Mask/ValueText");
			Mask2 = AddComponent<UIEmptyView>("Content/Bottom/Slider/Mask");
			Animator = AddComponent<UIAnimator>();
			Animator2 = AddComponent<UIAnimator>("Content");
			Self = AddComponent<UIImage>("Content/TaskBg");
			ItemName = AddComponent<UITextmesh>("Content/Bg/Bg/Name");
			Desc = AddComponent<UITextmesh>("Content/Desc");
			PointerClick = AddComponent<UIPointerClick>();
			Details = AddComponent<UIEmptyView>("Content/Details");
			Rewards = AddComponent<UITextmesh>("Content/Details/Title");
			RewardsVal = AddComponent<UITextmesh>("Content/Details/Text");
			this.ItemIcon = this.AddComponent<UIImage>("Content/Bg/ItemIcon");
			this.Slider = this.AddComponent<UISlider>("Content/Bottom/Slider");
			this.ValueText = this.AddComponent<UITextmesh>("Content/Bottom/Slider/ValueText");
			this.Over = this.AddComponent<UIEmptyView>("Content/Over");
			this.Mask = this.AddComponent<UIEmptyView>("Content/Mask");
			Bg = this.AddComponent<UIImage>("Content/Bg");
			Bottom = this.AddComponent<UIEmptyView>("Content/Bottom");
			LightImage = this.AddComponent<UIImage>("Content/Light");
		}

		#endregion

		#region 事件绑定

		
		#endregion

		public void SetData(TaskConfig config,Action<RestaurantTask> onClickOver)
		{
			Config = config;
			this.onClickOver = onClickOver;
			Self.SetEnabled(config != null);
			Bg.SetActive(config != null);
			Bottom.SetActive(config != null);
			Details.SetActive(config != null);
			Desc.SetActive(config != null);
			if (config == null)
			{
				Mask.SetActive(false);
				Over.SetActive(false);
				return;
			}
			if (Config.RewardType == 1)
			{
				Rewards.SetI18NKey(I18NKey.Text_Task_Rewards3);
				RewardsVal.SetText(
					I18NManager.Instance.TranslateMoneyToStr(Config.RewardCount * GameConst.ProfitUnitShowTime /
					                                         GameConst.ProfitUnitTime) + "/" +
					TimeInfo.Instance.TransitionToStr2(GameConst.ProfitUnitShowTime));
			}
			else if (Config.RewardType == 2)
			{
				Rewards.SetText("");
				RewardsVal.SetText(I18NManager.Instance.TranslateMoneyToStr(Config.RewardCount));
			}
			else
			{
				Log.Error("指定文本不存在"+Config.RewardType);
			}
			Desc.SetText(I18NManager.Instance.I18NGetText(Config));
			if (config.ItemType == 0)
			{
				ItemConfig itemConfig = ItemConfigCategory.Instance.Get(config.ItemId);
				ItemName.SetText(I18NManager.Instance.I18NGetText(itemConfig));
				ItemIcon.SetSpritePath(itemConfig.ItemPic).Coroutine();
			}
			else if (config.ItemType == 1)
			{
				ContainerConfig containerConfig = ContainerConfigCategory.Instance.Get(config.ItemId);
				ItemName.SetText(I18NManager.Instance.I18NGetText(containerConfig));
				ItemIcon.SetSpritePath(containerConfig.Icon).Coroutine();
			}
			else
			{
				Log.Error("未指定的任务类型" + config.ItemType);
				ItemIcon.SetSpritePath(GameConst.DefaultImage).Coroutine();
				ItemName.SetI18NKey(I18NKey.Global_Unknow);
			}
			// Bg.SetColor(GameConst.RareColor[Mathf.Clamp(config.Rare, 1, GameConst.RareColor.Length) - 1]);
			
			bool isOver = PlayerDataManager.Instance.GetTaskState(config.Id, out var step);
			var text = $"{Mathf.Min(step, config.ItemCount)}/{config.ItemCount}";
			ValueText.SetText(text);
			ValueText2.SetText(text);
			// ItemIcon.SetColor(step >= config.ItemCount ? Color.white : Color.black);
			Slider.SetMaxValue(config.ItemCount);
			Slider.SetMinValue(0);
			Slider.SetValue(step);
			float val = (float) step / config.ItemCount;
			UpdateAsync(val).Coroutine();
			if (isOver)
			{
				Over.SetActive(false);
				Mask.SetActive(false);
				PointerClick.RemoveOnClick();
				Animator2.SetEnable(false);
				Animator2.GetTransform().localScale = Vector3.one;
			}
			else if (step >= config.ItemCount)
			{
				Over.SetActive(true);
				Mask.SetActive(true);
				PointerClick.SetOnClick(OnClickComplex);
				Animator2.SetEnable(true);
			}
			else
			{
				Over.SetActive(false);
				Mask.SetActive(false);
				PointerClick.SetOnClick(OnClickDetails);
				Animator2.SetEnable(false);
				Animator2.GetTransform().localScale = Vector3.one;
			}
			PointerClick.SetEnabled(true);
		}
		
		private void OnClickDetails()
		{
			if(Config==null) return;
			OnClickAnim().Coroutine();
			UIManager.Instance.OpenWindow<UITaskDetailsWin, RestaurantTask>(UITaskDetailsWin.PrefabPath, this)
				.Coroutine();
		}

		private async ETTask UpdateAsync(float val)
		{
			var rect = Mask2.GetRectTransform();
			var parent = (rect.parent as RectTransform);
			while (parent.rect.width <= 0)
			{
				await TimerManager.Instance.WaitAsync(1);
			}
			rect.offsetMin = new Vector2(val * parent.rect.width, 0);
			ValueText2.GetRectTransform().sizeDelta = new Vector2(parent.rect.width, parent.rect.height);
		}

		private void OnClickComplex()
		{
			if (Config == null) return;
			Animator2.SetEnable(false);
			Animator2.GetTransform().localScale = Vector3.one;
			onClickOver?.Invoke(this);
		}

		private async ETTask OnClickAnim()
		{
			Animator2.SetEnable(true);
			await Animator2.Play("UIRestaurantTask_Click");
			Animator2.SetEnable(false);
			Animator2.GetTransform().localScale = Vector3.one;
		}

		public void SetInteractable(bool enable)
		{
			PointerClick.SetEnabled(enable);
		}
		
		public void OnClickLightImage()
		{
			OnClickLightImageAsync().Coroutine();
		}
		public async ETTask OnClickLightImageAsync()
		{
			bool isActive = LightImage.GetGameObject().activeSelf;
			if(!isActive) LightImage.GetGameObject().SetActive(true);
			LightImage.GetRectTransform().localScale = Vector3.one;

			var startTime = TimerManager.Instance.GetTimeNow();
			var time = 100f;
			while (true)
			{
				await TimerManager.Instance.WaitAsync(1);
       
				var timeNow = TimerManager.Instance.GetTimeNow();
				if (!isActive)
				{
					LightImage.SetImageAlpha(Mathf.Lerp(0, 1, (timeNow - startTime) / time));
				}
				else
				{
					LightImage.GetRectTransform().localScale = Vector3.Lerp(Vector3.one, Vector3.zero, (timeNow - startTime) / time);
				}
				if (timeNow - startTime >= time)
				{
					break;
				}
			}
    
			if(isActive) LightImage.GetGameObject().SetActive(false);
		}
	}
}
