using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace TaoTie
{
	public enum AppraisalState
	{
		NotStart,
		During,
		PreOver,
		Over,
	}
	/// <summary>
	/// 鉴定，情报直接一开始就作用到物品
	/// </summary>
	public class UIAppraisalView : UICommonMiniGameView, IOnDisable
	{
		private string DefaultAppraisalBg;
		private float ANIM_DURING;

		public static string PrefabPath => "UIGame/UIMiniGame/Prefabs/UIAppraisalView.prefab";
		public UILoopListView2 ScrollView;
		public UIButton StartBtn;
		public UIButton AdBtn;
		public UITextmesh AdBtnText;
		public UIImage AnimBg;
		public UIImage AnimBg2;
		public UITextmesh Addon;
		public UITextmesh Count;
		public UIAnimator Light;
		public UIEmptyView Mask;

		private AppraisalState state;

		private SubIdentificationConfig config;

		private int newIndex;
		private int randomResult;
		private bool ad;
		private bool isTargetGameInfo;
		#region override
		public override void OnCreate()
		{
			base.OnCreate();
			Mask = AddComponent<UIEmptyView>("View/Bg/Content/ScrollView/Mask");
			Addon = AddComponent<UITextmesh>("View/Bg/Content/Result/Result/TextPriceAddOn");
			this.ScrollView = this.AddComponent<UILoopListView2>("View/Bg/Content/ScrollView");
			this.ScrollView.InitListView(0,GetScrollViewItemByIndex);
			this.StartBtn = this.AddComponent<UIButton>("View/Bg/Content/Buttons/StartBtn");
			this.AdBtn = this.AddComponent<UIButton>("View/Bg/Content/Buttons/AdBtn");
			AnimBg = AddComponent<UIImage>("View/Bg/Content/Result/AnimBg");
			AnimBg2 = AddComponent<UIImage>("View/Bg/Content/Result/AnimBg/Bg2");
			Count = AddComponent<UITextmesh>("View/Bg/Content/Buttons/AdBtn/Count");
			Count.SetI18NKey(I18NKey.Text_TurnTable_Count);
			AdBtnText = AddComponent<UITextmesh>("View/Bg/Content/Buttons/AdBtn/Text");
			ANIM_DURING = GlobalConfigCategory.Instance.GetFloat("AppraisalAnimDuring", 3000);
			DefaultAppraisalBg = GlobalConfigCategory.Instance.GetString("DefaultAppraisalBg", "yellow");
			Light = AddComponent<UIAnimator>("View/Bg/Content/ScrollView/Target");
		}
		public override void OnEnable(int id)
		{
			Mask.SetActive(false);
			base.OnEnable(id);
			Light.Play("Light_Hide").Coroutine();
			Addon.SetActive(false);
			if (!SubIdentificationConfigCategory.Instance.TryGet(id, out config))
			{
				CloseSelf().Coroutine();
				Log.Error("指定物品不是可鉴定类型 = " + id);
				return;
			}
			var min = config.Min;
			var max = config.Max;
			isTargetGameInfo = false;
			if (IAuctionManager.Instance != null)//情报增加价格
			{
				var gameInfoConfig = IAuctionManager.Instance.GetFinalGameInfoConfig();
				if (gameInfoConfig != null && gameInfoConfig.IsTargetItem(ItemConfig))
				{
					isTargetGameInfo = true;
					if (gameInfoConfig.AwardType == 0)
					{
						min += gameInfoConfig.RewardCount;
						max += gameInfoConfig.RewardCount;
					}
					else if (gameInfoConfig.AwardType == 1)
					{
						min *= gameInfoConfig.RewardCount;
						max *= gameInfoConfig.RewardCount;
					}
				}
			}
			ad = false;
			state = AppraisalState.NotStart;
			this.StartBtn.SetOnClick(OnClickStartBtn);
			this.AdBtn.SetOnClick(OnClickAdBtn);
			StartBtn.SetActive(true);
			AdBtn.SetActive(CanAd() && !ad);
			StartBtn.SetInteractable(false);
			AdBtn.SetInteractable(false);
			ScrollView.SetListItemCount(-1);
			ScrollView.MovePanelToItemIndex(0);
			Range.SetI18NText(min, max);
			AnimBg.SetActive(false);
			Count.SetI18NText(Mathf.Max(0, GameConst.PlayableMaxAdCount - PlayerDataManager.Instance.PlayableCount));
			OnEnableAsync().Coroutine();
		}

		private async ETTask OnEnableAsync()
		{
			using ListComponent<ETTask> tasks = ListComponent<ETTask>.Create(); 
			for (int i = 0; i < config.Result.Length; i++)
			{
				var itemC = ItemConfigCategory.Instance.Get(config.Result[i]);
				tasks.Add(ImageLoaderManager.Instance.LoadSpriteTask(itemC.ItemPic));
			}

			await ETTaskHelper.WaitAll(tasks);
			StartBtn.SetInteractable(true);
			AdBtn.SetInteractable(true);
		}

		public void OnDisable()
		{
			Addon.SetActive(false);
			if (state == AppraisalState.Over)
			{
				IAuctionManager.Instance.SetAppraisalResult(config.Id, config.Result[randomResult]);
				Messager.Instance.Broadcast(0, MessageId.SetChangeItemResult, config.Id, config.Result[randomResult], false);
			}
		}
		#endregion

		#region 事件绑定
		public LoopListViewItem2 GetScrollViewItemByIndex(LoopListView2 listView, int index)
		{
			int newId = this.configId;
			string bg = DefaultAppraisalBg;
			if (index != 0)
			{
				var res = Mathf.Abs((index - 1) % config.Result.Length);
				newId = config.Result[res];
				bg = RareConfigCategory.Instance.GetRare(config.Rare[res]).Icon;
				if (index == newIndex + 1 && state == AppraisalState.PreOver)
				{
					newId = config.Result[randomResult];
					bg = RareConfigCategory.Instance.GetRare(config.Rare[randomResult]).Icon;
				}
			}
			else
			{
				bg = RareConfigCategory.Instance.GetRare(1).Icon;
			}
			
			var item = listView.GetShownItemByItemIndex(index);
			if (item == null)
			{
				item = listView.NewListViewItem("UIItem");
			}

			UIAppraisalItem appraisalItem;
			if (!item.IsInitHandlerCalled)
			{
				item.IsInitHandlerCalled = true;
				appraisalItem = ScrollView.AddItemViewComponent<UIAppraisalItem>(item);
			}
			else
			{
				appraisalItem = ScrollView.GetUIItemView<UIAppraisalItem>(item);
			}

			if (state == AppraisalState.During)
			{
				newIndex = index;
			}

			appraisalItem.SetData(newId, isTargetGameInfo, bg);
			return item;
		}
		public void OnClickStartBtn()
		{
			OnClickStartBtnAsync().Coroutine();
		}
		public void OnClickAdBtn()
		{
			AdBtn.SetInteractable(false);
			OnClickAdBtnAsync().Coroutine();
		}
		
		#endregion

		public async ETTask OnClickStartBtnAsync()
		{
			state = AppraisalState.During;
			StartBtn.SetActive(false);
			Back.SetInteractable(false);
			AdBtn.SetActive(false);
			Share.SetActive(false);
			AnimBg.SetActive(true);
			Mask.SetActive(true);
			if (!ad || config.Max <= ItemConfig.Price)
			{
				if (GameSetting.PlayableResult == PlayableResult.Success)
				{
					randomResult = 3;
				}
				else if (GameSetting.PlayableResult == PlayableResult.Fail)
				{
					randomResult = 0;
				}
				else
				{
					var range = Random.Range(0, config.TotalWidget * 10) % config.TotalWidget;
					for (int i = 0; i < config.Widget.Length; i++)
					{
						range -= config.Widget[i];
						randomResult = i;
						if (range <= 0)
						{
							break;
						}
					}
				}
			}
			else
			{
				int totalWidget = 0;
				var price = ItemConfig.Price;
				if (randomResult >= 0)
				{
					var newRes = ItemConfigCategory.Instance.Get(config.Result[randomResult]);
					if (newRes.Price >= price)
					{
						price = newRes.Price;
					}
				}
				using ListComponent<int> results = ListComponent<int>.Create();
				for (int i = 0; i < config.Result.Length; i++)
				{
					var cfg = ItemConfigCategory.Instance.Get(config.Result[i]);
					if (cfg.Price > price)
					{
						totalWidget += config.Widget[i];
						results.Add(i);
					}
				}

				if (totalWidget > 0)
				{
					var range = Random.Range(0, totalWidget * 10) % totalWidget;
					for (int i = 0; i < results.Count; i++)
					{
						range -= config.Widget[results[i]];
						randomResult = results[i];
						if (range <= 0)
						{
							break;
						}
					}
				}
			}

			var itemId = config.Result[randomResult];
			var bg = RareConfigCategory.Instance.GetRare(config.Rare[randomResult]).Icon;
			
			ItemConfig result = ItemConfigCategory.Instance.Get(itemId);
			BigNumber allPrice = result.Price;
			BigNumber lastAuctionPrice = ItemConfig.Price;
			if (IAuctionManager.Instance != null)//情报增加价格
			{
				var gameInfoConfig = IAuctionManager.Instance.GetFinalGameInfoConfig();
				//玩法情报要玩过后才加钱
				if (gameInfoConfig != null && gameInfoConfig.IsTargetItem(result))
				{
					if (gameInfoConfig.AwardType == 1)
					{
						allPrice = allPrice * gameInfoConfig.RewardCount;
						lastAuctionPrice = lastAuctionPrice * gameInfoConfig.RewardCount;
					}
				}
			}
			
			
			float maxSpeed = 20;
			if (PerformanceManager.Instance.Level != PerformanceManager.DevicePerformanceLevel.High)
			{
				maxSpeed = 5;
			}
			var velocity = Vector2.left * Define.DesignScreenWidth * maxSpeed;
			ScrollView.GetLoopListView().ItemSnapEnable = true;
			long ii = -1;
			bool isWin = true;
			int count = (int) (ANIM_DURING / 400);
			if (count % 2 == 0 == allPrice >= lastAuctionPrice)
			{
				count += 1;
			}
			var startTime = TimerManager.Instance.GetTimeNow();
			float animDuring = 400 * count;
			while (true)
			{
				await TimerManager.Instance.WaitAsync(1);
				var timeNow = TimerManager.Instance.GetTimeNow();
				var len = timeNow - startTime;
				if (len / 400 != ii)
				{
					ii = len / 400;
					SetItemWinLoss(isWin ? config.Max : -config.Max);
					AnimBg.SetColor(isWin?GameConst.GREEN_COLOR:GameConst.RED_COLOR);
					AnimBg2.SetColor(isWin ? GameConst.RED_COLOR:GameConst.GREEN_COLOR);
					AnimBg.SetActive(true);
					AnimBg2.SetFillAmount(0);
					isWin = !isWin;
				}
				else
				{
					AnimBg2.SetFillAmount((len % 400) / 400f);
				}
				float progress = len / animDuring;
				if (progress < 0.5)
				{
					ScrollView.GetScrollRect().velocity = Mathf.Clamp01(progress * 5) * velocity;
				}
				else
				{
					ScrollView.GetScrollRect().velocity = Mathf.Clamp01((1 + 1/maxSpeed - progress) * 5) * velocity;
				}
				if (timeNow - startTime > animDuring)
				{
					break;
				}
			}
			state = AppraisalState.PreOver;
			
			ETTask task = ETTask.Create();
			ScrollView.GetLoopListView().mOnSnapItemFinished = (a, b) =>
			{
				if(task.IsCompleted) return;
				task.SetResult();
			};
			ScrollView.SetSnapTargetItemIndex(newIndex+1);
			startTime = TimerManager.Instance.GetTimeNow();
			isWin = allPrice >= lastAuctionPrice;
			AnimBg.SetColor(!isWin?GameConst.GREEN_COLOR:GameConst.RED_COLOR);
			AnimBg2.SetColor(!isWin ? GameConst.RED_COLOR:GameConst.GREEN_COLOR);
			AnimBg.SetActive(true);
			AnimBg2.SetFillAmount(0);
			while (true)
			{
				await TimerManager.Instance.WaitAsync(1);
				var timeNow = TimerManager.Instance.GetTimeNow();
				var len = timeNow - startTime;
				AnimBg2.SetFillAmount(len / 400f);
				if (timeNow - startTime > 400)
				{
					break;
				}
			}
			
			await task;
			ScrollView.MovePanelToItemIndex(newIndex + 1);
			AnimBg.SetActive(false);
			ScrollView.GetLoopListView().mOnSnapItemFinished = null;
			ScrollView.GetLoopListView().ItemSnapEnable = false;
			state = AppraisalState.Over;
			Mask.SetActive(false);
			var item = ScrollView.GetShownItemByItemIndex<UIAppraisalItem>(newIndex + 1);
			item.SetData(itemId, true, bg);
			SetItemWinLossWithContainer(allPrice - lastAuctionPrice);
			Back.SetInteractable(true);
			AdBtn.SetActive(AdManager.Instance.PlatformHasAD() && !ad);
			Share.SetActive(true);
			Light.Play("Light_Show").Coroutine();
		}
		
		public async ETTask OnClickAdBtnAsync()
		{
			var res = await PlayAd();
			if (res)
			{
				Count.SetI18NText(Mathf.Max(0, GameConst.PlayableMaxAdCount - PlayerDataManager.Instance.PlayableCount));
				ad = true;
				await OnClickStartBtnAsync();
			}
			else
			{
				AdBtn.SetInteractable(!ad);
			}
		}

		protected override void AfterPlayAd(int total, int cur)
		{
			base.AfterPlayAd(total, cur);
			AdBtnText.SetText(I18NManager.Instance.I18NGetText(I18NKey.Text_Appraisal_Ad)+$"({cur}/{total})");
		}
	}
}
