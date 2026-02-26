using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace TaoTie
{
	public class UITaskDetailsWin : UIBaseView, IOnCreate, IOnEnable<TaskConfig>, IOnEnable<RestaurantTask>
	{
		public static string PrefabPath => "UIGame/UILobby/Prefabs/UITaskDetailsWin.prefab";
		public UIButton Close;
		public UIImage Icon;
		public UITextmesh Rewards;
		public UITextmesh RewardsVal;
		public UIPointerClick DropPointer;
		public UITextmesh Drop;
		public UIButton Button1;
		public UITextmesh ButtonText2;
		public UIButton Button2;
		public UITextmesh Title;
		public UITextmesh Need;
		public UITextmesh Desc;
		public UIEmptyView Over;
		public UITextmesh Name;
		public UIEmptyView High;
		public UIAnimator UICommonWin;

		public TaskConfig Config { get; private set; }
		private RestaurantTask uiTask;
		#region override
		public void OnCreate()
		{
			UICommonWin = AddComponent<UIAnimator>("UICommonWin");
			High = AddComponent<UIEmptyView>("UICommonWin/Win/Content/Item/Rare/High");
			Icon = AddComponent<UIImage>("UICommonWin/Win/Content/Item/Rare/Icon");
			Rewards = AddComponent<UITextmesh>("UICommonWin/Win/Content/Desc/Details/Title");
			RewardsVal = AddComponent<UITextmesh>("UICommonWin/Win/Content/Desc/Details/Text");
			this.Close = this.AddComponent<UIButton>("UICommonWin/Win/Close");
			this.Drop = this.AddComponent<UITextmesh>("UICommonWin/Win/Content/Desc/Drop");
			DropPointer= this.AddComponent<UIPointerClick>("UICommonWin/Win/Content/Desc/Drop");
			this.Button1 = this.AddComponent<UIButton>("UICommonWin/Win/Content/Bottom/Button1");
			this.ButtonText2 = this.AddComponent<UITextmesh>("UICommonWin/Win/Content/Bottom/Button2/Text");
			this.Button2 = this.AddComponent<UIButton>("UICommonWin/Win/Content/Bottom/Button2");
			Need = this.AddComponent<UITextmesh>("UICommonWin/Win/Content/Desc/Progress");
			Title = AddComponent<UITextmesh>("UICommonWin/Win/Title");
			Desc = AddComponent<UITextmesh>("UICommonWin/Win/Content/Desc/Desc");
			Over = AddComponent<UIEmptyView>("UICommonWin/Win/Over");
			Name = AddComponent<UITextmesh>("UICommonWin/Win/Content/Item/Table/Text");
			Need.SetI18NKey(I18NKey.Text_Task_Need);
		}

		public void OnEnable(RestaurantTask data)
		{
			SoundManager.Instance.PlaySound("Audio/Sound/Win_Open.mp3");
			uiTask = data;
			SetData(data.Config);
		}
		public void OnEnable(TaskConfig data)
		{
			SoundManager.Instance.PlaySound("Audio/Sound/Win_Open.mp3");
			uiTask = null;
			SetData(data);
		}

		public void SetData(TaskConfig data)
		{
			Config = data;
			High.SetActive(Config.Rare > 3);
			DropPointer.SetOnClick(OnClickDrop);
			this.Close.SetOnClick(OnClickClose);
			this.Button1.SetOnClick(OnClickAdButton);
			this.Button2.SetOnClick(OnClickGoButton);

			ContainerConfig container = null;
			if (data.ItemType == 0)
			{
				var item = ItemConfigCategory.Instance.Get(data.ItemId);
				container = ContainerConfigCategory.Instance.Get(item.ContainerId);
				Name.SetText(I18NManager.Instance.I18NGetText(item));
				Icon.SetSpritePath(item.ItemPic).Coroutine();
			}
			else if (data.ItemType == 1)
			{
				container = ContainerConfigCategory.Instance.Get(data.ItemId);
				Name.SetText(I18NManager.Instance.I18NGetText(container));
				Icon.SetSpritePath(container.Icon).Coroutine();
			}
			else
			{
				Log.Error("未指定的任务类型" + data.ItemType);
				Icon.SetSpritePath(GameConst.DefaultImage).Coroutine();
				Name.SetI18NKey(I18NKey.Global_Unknow);
			}

			if (Config.RewardType == 1)
			{
				Rewards.SetI18NKey(I18NKey.Text_Task_Rewards2);
				RewardsVal.SetText(
					I18NManager.Instance.TranslateMoneyToStr(Config.RewardCount * GameConst.ProfitUnitShowTime /
					                                         GameConst.ProfitUnitTime) + "/" +
					TimeInfo.Instance.TransitionToStr2(GameConst.ProfitUnitShowTime));
			}
			else if (Config.RewardType == 2)
			{
				Rewards.SetI18NKey(I18NKey.Text_Task_Rewards2);
				RewardsVal.SetText(I18NManager.Instance.TranslateMoneyToStr(Config.RewardCount));
			}
			else
			{
				Log.Error("指定文本不存在" + Config.RewardType);
			}

			Title.SetI18NKey(uiTask==null?I18NKey.Text_Title_Task2:I18NKey.Text_Title_Task1);
			Desc.SetText(I18NManager.Instance.I18NGetText(Config, 1));
			bool isOver = PlayerDataManager.Instance.GetTaskState(data.Id, out var step);
			Need.SetI18NText($"{Mathf.Min(step,data.ItemCount)}/{data.ItemCount}");
			Need.SetActive(uiTask == null);
			Desc.SetActive(uiTask != null);
			Button2.SetActive(!isOver);
			bool showReceive = !isOver && step >= data.ItemCount;
			ButtonText2.SetI18NKey(showReceive ? I18NKey.Text_Task_Over : I18NKey.Text_Enter_Auction);
#if UNITY_EDITOR
			this.Button1.SetActive(true);
#else
			this.Button1.SetActive(data.AdvertisementButton == 1 && AdManager.Instance.PlatformHasAD());
#endif
			Over.SetActive(showReceive);
			Drop.SetI18NKey(I18NKey.Text_Item_Drop_NoOpen);
			if (container != null)
			{
				if (!showReceive && !PlayerDataManager.Instance.IsUnlockContainer(container.Id))
				{
					ButtonText2.SetI18NKey(I18NKey.Text_Go_Unlock);
				}
				var level = LevelConfigCategory.Instance.Get(container.Level);
				Drop.SetI18NText(
					$"<color=#{level.Color}>{I18NManager.Instance.I18NGetText(level)}</color> <color=#{container.Color}>{I18NManager.Instance.I18NGetText(container)}</color>");
			}
			else
			{
				Drop.SetI18NText(I18NManager.Instance.I18NGetText(I18NKey.Global_Unknow));
			}
		}

		#endregion

		#region 事件绑定
		public void OnClickClose()
		{
			CloseSelf().Coroutine();
		}

		public override async ETTask CloseSelf()
		{
			SoundManager.Instance.PlaySound("Audio/Sound/Win_Close.mp3");
			await UICommonWin.Play("UIWin_Close");
			await base.CloseSelf();
		}

		public void OnClickGoButton()
		{
			bool isOver = PlayerDataManager.Instance.GetTaskState(Config.Id, out var step);
			if (!isOver && step >= Config.ItemCount)
			{
				OnClickComplexAsync().Coroutine();
			}
			else
			{
				OnClickBtnStartAsync().Coroutine();
			}
		}

		public async ETTask OnClickComplexAsync()
		{
			CloseSelf().Coroutine();
			var top = UIManager.Instance.GetView<UITopView>(1);
			if (top != null && Config.RewardType == 2)
			{
				Button2.SetEnabled(false);
				await top.Top.DoMoneyMoveAnim(Config.RewardCount, RewardsVal.GetTransform().position,
					Mathf.Max(1, Config.Rare)*2);
				Button2.SetEnabled(true);
			}
			bool res = PlayerDataManager.Instance.ComplexTask(Config.Id);
			if (res)
			{
				if (uiTask == null)
				{
					UIManager.Instance.GetView<UIMarketView>(1)?.RefreshTask();
				}
				CloseSelf().Coroutine();
			}
		}

		public void OnClickDrop()
		{
			OnClickBtnStartAsync().Coroutine();
		}
		
		private async ETTask OnClickBtnStartAsync()
		{
			ContainerConfig container = null;
			if (Config.ItemType == 0)
			{
				var item = ItemConfigCategory.Instance.Get(Config.ItemId);
				container = ContainerConfigCategory.Instance.Get(item.ContainerId);
			}
			else if (Config.ItemType == 1)
			{
				container = ContainerConfigCategory.Instance.Get(Config.ItemId);
			}
			else
			{
				return;
			}
			if (PlayerDataManager.Instance.GetMaxLevel() < container.Level)
			{
				UIManager.Instance.OpenBox<UIToast, I18NKey>(UIToast.PrefabPath, I18NKey.Tips_Auction_NotOpen).Coroutine();
				return;
			}
			var level = LevelConfigCategory.Instance.Get(container.Level);
			if (level.Money > 0 && PlayerDataManager.Instance.TotalMoney < level.Money)
			{
				UIManager.Instance.OpenBox<UIToast, I18NKey>(UIToast.PrefabPath, I18NKey.Tips_Auction_Money_NotEnough).Coroutine();
				return;
			}
			CloseSelf().Coroutine();
			UIManager.Instance.CloseWindow<UIMarketView>().Coroutine();
			if (!PlayerDataManager.Instance.IsUnlockContainer(container.Id))
			{
				await UIManager.Instance.OpenWindow<UIBlackView, int>(UIBlackView.PrefabPath, level.Id);
			}
			else
			{
				await UIManager.Instance.OpenWindow<UIAuctionSelectView, int>(UIAuctionSelectView.PrefabPath, level.Id);
			}
		}
		
		public void OnClickAdButton()
		{
#if !UNITY_EDITOR
			if (Config.AdvertisementButton != 1)
			{
				return;
			}
#endif
			Button1.SetInteractable(false);
			if (AdManager.Instance.PlatformHasAD())
			{
				OnClickAdGetAsync().Coroutine();
			}
		}
		#endregion

		private async ETTask OnClickAdGetAsync()
		{
			try
			{
				var res = await AdManager.Instance.PlayAd();
				if (res)
				{
					PlayerDataManager.Instance.AddTaskStep(Config.ItemId, Config.ItemCount, Config.ItemType);
					SetData(Config);
					CloseSelf().Coroutine();
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex);
			}
			finally
			{
				Button1.SetInteractable(true);
			}
		}
	}
}
