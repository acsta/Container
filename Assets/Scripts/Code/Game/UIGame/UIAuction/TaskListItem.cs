using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace TaoTie
{
	public class TaskListItem : UIBaseContainer, IOnCreate
	{
		private const string MC1 = "#ECA549";
		private const string MC2 = "#D4852B";
		public UIImage Bg1;
		public UIImage Bg2;
		public UIImage Bg3;
		public UITextmesh Level;
		public UITextmesh From;
		public UITextmesh Drop;
		public UIImage Icon;
		public UITextmesh Value;
		public UITextmesh Name;
		public UIButton Bg;
		public UIImage TaskItem;
		public UIEmptyView Over;
		public UIImage Slider;
		public UIButton Btn;
		public UITextmesh ValueText2;
		public UIEmptyView Mask;

		public UITextmesh Rewards;
		public UITextmesh RewardsVal;
		
		public UIImage LightImage;
		public UIEmptyView content;
		public UIAnimator Animator;

		private TaskConfig config;
		#region override
		public void OnCreate()
		{
			Bg1= AddComponent<UIImage>("Content/Bg1");
			Bg2= AddComponent<UIImage>("Content/Bg2");
			Bg3= AddComponent<UIImage>("Content/Bg3");
			this.ValueText2 = AddComponent<UITextmesh>("Content/Mid/Progress/Mask/ValueText");
			Mask = AddComponent<UIEmptyView>("Content/Mid/Progress/Mask");
			TaskItem = AddComponent<UIImage>("Content/TaskItem");
			Bg = AddComponent<UIButton>("Content/Bg");
			this.Level = this.AddComponent<UITextmesh>("Content/Level");
			this.From = this.AddComponent<UITextmesh>("Content/From");
			this.Drop = this.AddComponent<UITextmesh>("Content/Mid/Drop");
			this.Icon = this.AddComponent<UIImage>("Content/TaskItem/ItemIcon");
			Name= this.AddComponent<UITextmesh>("Content/TaskItem/Bg/Name");
			this.Value = this.AddComponent<UITextmesh>("Content/Mid/Progress/Value");
			Slider= this.AddComponent<UIImage>("Content/Mid/Progress/Progress");
			Over = AddComponent<UIEmptyView>("Content/Over");
			Btn = AddComponent<UIButton>();
			Rewards = AddComponent<UITextmesh>("Content/Mid/Rewards/Title");
			RewardsVal = AddComponent<UITextmesh>("Content/Mid/Rewards/Content");
			LightImage = AddComponent<UIImage>("LightImage");
			content = AddComponent<UIEmptyView>("Content");
			Animator = AddComponent<UIAnimator>("");
			Btn.SetOnClick(OnClickSelf);
		}
		#endregion
		
		public void SetData(TaskConfig config)
		{
			this.config = config;
			bool isOver = PlayerDataManager.Instance.GetTaskState(config.Id, out var step);
			Over.SetActive(isOver || step >= config.ItemCount);
			From.SetI18NKey(I18NKey.Text_Title_Market);
			Drop.SetActive(config.ItemId != GameConst.MoneyItemId);
			ContainerConfig container = null;
			bool isCoin = false;
			if (config.ItemType == 0)
			{
				ItemConfig itemConfig = ItemConfigCategory.Instance.Get(config.ItemId);
				container = ContainerConfigCategory.Instance.Get(itemConfig.ContainerId);
				Icon.SetSpritePath(itemConfig.ItemPic).Coroutine();
				Name.SetText(I18NManager.Instance.I18NGetText(itemConfig));
				isCoin = itemConfig.Type == (int) ItemType.Const;
			}
			else if (config.ItemType == 1)
			{
				container = ContainerConfigCategory.Instance.Get(config.ItemId);
				Icon.SetSpritePath(container.Icon).Coroutine();
				Name.SetText(I18NManager.Instance.I18NGetText(container));
			}
			else
			{
				Log.Error("未指定的任务类型" + config.ItemType);
				Icon.SetSpritePath(GameConst.DefaultImage).Coroutine();
				Name.SetI18NKey(I18NKey.Global_Unknow);
			}
			
			Bg1.SetColor(MC1);
			Bg2.SetColor(MC1);
			Bg3.SetColor(MC2);

			if (config.RewardType == 1)
			{
				Rewards.SetI18NKey(I18NKey.Text_Task_Rewards4);
				RewardsVal.SetText(
					I18NManager.Instance.TranslateMoneyToStr(config.RewardCount * GameConst.ProfitUnitShowTime /
					                                         GameConst.ProfitUnitTime) + "/" +
					TimeInfo.Instance.TransitionToStr2(GameConst.ProfitUnitShowTime));
			}
			else if (config.RewardType == 2)
			{
				Rewards.SetI18NKey(I18NKey.Text_Task_Rewards2);
				RewardsVal.SetText(I18NManager.Instance.TranslateMoneyToStr(config.RewardCount));
			}
			else
			{
				Log.Error("指定文本不存在"+config.RewardType);
			}
			
			TaskItem.SetSpritePath(config.Rare > 3 ?"UIGame/UILobby/Atlas/task_item_bg2.png":"UIGame/UILobby/Atlas/task_item_bg.png").Coroutine();
			var text = $"{Mathf.Min(step, config.ItemCount)}/{config.ItemCount}";
			Value.SetText(text);
			ValueText2.SetText(text);
			float val = (float) step / config.ItemCount;
			Slider.SetFillAmount(val);
			var rect = Mask.GetRectTransform();
			var size = (rect.parent as RectTransform).sizeDelta;
			rect.offsetMin = new Vector2(val * size.x, 0);
			if (container != null)
			{
				var level = LevelConfigCategory.Instance.Get(container.Level);
				Level.SetText(I18NManager.Instance.I18NGetText(level));
				Drop.SetI18NKey(I18NKey.Text_Item_Drop_NoOpen);
				Drop.SetI18NText(I18NManager.Instance.I18NGetText(container));
				Bg.SetActive(false);
				if (container.Level == IAuctionManager.Instance.Level || isCoin )
				{
					Bg.SetActive(false);
				}
				else
				{
					Bg.SetActive(isOver || step >= config.ItemCount ? false : true);
				}
			}
		}

		public void OnClickSelf()
		{
			OnClickSelfAsync().Coroutine();
		}

		public async ETTask OnClickSelfAsync()
		{
			bool isOver = PlayerDataManager.Instance.GetTaskState(config.Id, out var step);
			if (!isOver && step >= config.ItemCount)
			{
				if (IAuctionManager.Instance?.AState == AuctionState.OpenBox
				    || IAuctionManager.Instance?.AState == AuctionState.ExitAnim
				    || IAuctionManager.Instance?.AState == AuctionState.Over
				    || IAuctionManager.Instance?.AState == AuctionState.Over)
				{
					UIManager.Instance.OpenBox<UIToast,I18NKey>(UIToast.PrefabPath,I18NKey.Text_Notice_Get_Wait).Coroutine();
					return;
				}
				if (PlayerDataManager.Instance.ComplexTask(config.Id))
				{
					UIManager.Instance.OpenBox<UIToast,I18NKey>(UIToast.PrefabPath,I18NKey.Text_Task_Get_Success).Coroutine();
					
					var tasks = PlayerDataManager.Instance.GetDailyTaskIds();
					int overTaskCount = 0;
					for (int i = 0; i < tasks.Count; i++)
					{ 
						if (PlayerDataManager.Instance.GetTaskState(tasks[i], out _))
						{
							overTaskCount++;
						}
					}

					OnClickLightImageAsync().Coroutine();
					await TimerManager.Instance.WaitAsync(50);
					
					var gameView = UIManager.Instance.GetView<UIGameView>(1);
					await gameView?.CashGroup?.DoMoneyMoveAnim(config.RewardCount, LightImage.GetRectTransform().position, new Vector2(200, 300) , Mathf.Max(1, config.Rare * 2));
					
					await OnClickLightImageAsync();

					var taskInfoWin = UIManager.Instance.GetView<UITaskInfoWin>(1);
					await taskInfoWin.DoAnim(this, overTaskCount);
					content.GetRectTransform().GetComponent<CanvasGroup>().alpha = 1;
					
					await TimerManager.Instance.WaitAsync(50);
					
				}
			}
			else if (!isOver)
			{
				await Animator.Play("TaskListItem_ClickOff");
				UIManager.Instance.OpenBox<UIToast,I18NKey>(UIToast.PrefabPath,I18NKey.Tips_Recieve_NotOpen).Coroutine();
			}
		}
		
		public async ETTask OnClickLightImageAsync()
		{
			bool isActive = LightImage.GetGameObject().activeSelf;
			if (!isActive)
			{
				LightImage.GetGameObject().SetActive(true);
				LightImage.GetRectTransform().localScale = Vector3.one;
				content.GetRectTransform().GetComponent<CanvasGroup>().alpha = 0;
			}

			var startTime = TimerManager.Instance.GetTimeNow();
			var time = 100f;
			while (true)
			{
				await TimerManager.Instance.WaitAsync(1);
       
				var timeNow = TimerManager.Instance.GetTimeNow();
				if (!isActive)
				{
					LightImage.GetRectTransform().localScale = Vector3.Lerp(Vector3.one, Vector3.zero, (timeNow - startTime) / time);
				}
				else
				{
					break;
				}
				if (timeNow - startTime >= time)
				{
					break;
				}
			}

			if (isActive)
			{
				LightImage.GetGameObject().SetActive(false);
				LightImage.GetRectTransform().localScale = Vector3.one;
			}
		}
	}
}
