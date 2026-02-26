using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace TaoTie
{
	public class UIItemStoryWin : UIBaseView, IOnCreate, IOnEnable<int, UIAuctionItem>,IOnDisable
	{
		public static string PrefabPath => "UIGame/UIMiniGame/Prefabs/UIItemStoryWin.prefab";
		public UIImage Icon;
		public UITextmesh Name;
		public UITextmesh Price;
		public UITextmesh Desc;
		public UIButton Button1;
		public UIButton Button2;
		public UITextmesh Button1Txt;
		public UITextmesh Button2Txt;
		public UIAnimator Animator;
		public UIImage Ad1;
		public UIImage Ad2;
		public UIEmptyView Money1;
		public UIEmptyView Money2;
		public UITextmesh MoneyText1;
		public UITextmesh MoneyText2;

		public UIEmptyView FailEx;
		public UIEmptyView Success1;
		public UIEmptyView Success2;
		private UIAuctionItem Target;
		private Vector2 endSize;
		protected int configId { get; private set; }
		public ItemConfig ItemConfig => ItemConfigCategory.Instance.Get(configId);

		private int storyId;
		
		public StoryConfig StoryConfig => StoryConfigCategory.Instance.Get(storyId);

		private BigNumber newPrice;
	
		private int overAD1;
		private int overAD2;
		#region override
		public void OnCreate()
		{
			Animator = AddComponent<UIAnimator>();
			Success1 = AddComponent<UIEmptyView>("UIItem/Success");
			Success2 = AddComponent<UIEmptyView>("UIItem/Icon/Success");
			FailEx = AddComponent<UIEmptyView>("UIItem/Icon/Fail");
			Money1 = AddComponent<UIEmptyView>("Bottom/Button1/Money");
			Money2 = AddComponent<UIEmptyView>("Bottom/Button2/Money");
			MoneyText1 = AddComponent<UITextmesh>("Bottom/Button1/Money/Count");
			MoneyText2 = AddComponent<UITextmesh>("Bottom/Button2/Money/Count");
			Ad1 = AddComponent<UIImage>("Bottom/Button1/Ad");
			Ad2 = AddComponent<UIImage>("Bottom/Button2/Ad");
			this.Icon = this.AddComponent<UIImage>("UIItem/Icon");
			this.Price = this.AddComponent<UITextmesh>("UIItem/Bottom/TextPrice");
			this.Name = this.AddComponent<UITextmesh>("UIItem/Name");
			this.Desc = this.AddComponent<UITextmesh>("Desc/Desc");
			this.Button2 = this.AddComponent<UIButton>("Bottom/Button2");
			this.Button1 = this.AddComponent<UIButton>("Bottom/Button1");
			Button1Txt = this.AddComponent<UITextmesh>("Bottom/Button1/Text");
			Button2Txt = this.AddComponent<UITextmesh>("Bottom/Button2/Text");
			endSize = Icon.GetRectTransform().sizeDelta;
		}
		public void OnEnable(int id, UIAuctionItem target)
		{
			Target = target;
			FailEx.SetActive(false);
			Success1.SetActive(false);
			Success2.SetActive(false);
			overAD1 = 0;
			overAD2 = 0;
			newPrice = null;
			this.configId = id;
			this.Button2.SetOnClick(OnClickButton2);
			this.Button1.SetOnClick(OnClickButton1);
			var config = ItemConfig;
			if (config.StoryIds == null || config.StoryIds.Length <= 0)
			{
				CloseSelf().Coroutine();
				return;
			}
			DoMoveImage().Coroutine();
			Button1.SetInteractable(true);
			Button2.SetInteractable(true);
			Name.SetText(I18NManager.Instance.I18NGetText(config));
			Icon.SetSpritePath(config.ItemPic).Coroutine();
			Price.SetNum(GetItemPrice());
			var total = 0;
			for (int i = 0; i < config.StoryIds.Length; i++)
			{
				StoryConfig c = StoryConfigCategory.Instance.Get(config.StoryIds[i]);
				total += c.Weight;
			}

			var range = Random.Range(0, total);
			var index = -1;
			for (int i = 0; i < config.StoryIds.Length; i++)
			{
				StoryConfig c = StoryConfigCategory.Instance.Get(config.StoryIds[i]);
				range -= c.Weight;
				if (range <= 0)
				{
					index = i;
					break;
				}
			}
			storyId = config.StoryIds[index];
			var story = StoryConfig;
			if (story.ChooseCount <= 1)
			{
				OnChoose(0).Coroutine();
			}
			else
			{
				Button2.SetActive(story.Type1 != 2 || AdManager.Instance.PlatformHasAD());
				Button1.SetActive(story.Type0 != 2 || AdManager.Instance.PlatformHasAD());
				Desc.SetText(I18NManager.Instance.I18NGetText(story));
				
				
				Ad1.SetActive(story.Type0 == 2);
				Ad2.SetActive(story.Type1 == 2);
				if (story.Type0 == 2 && story.Count0 > 1)
				{
					Button1Txt.SetText(I18NManager.Instance.I18NGetText(story, StoryConfig.Choose0)+$"({overAD1}/{story.Count0})");
				}
				else
				{
					Button1Txt.SetText(I18NManager.Instance.I18NGetText(story, StoryConfig.Choose0));
				}
				if (story.Type1 == 2 && story.Count1 > 1)
				{
					Button2Txt.SetText(I18NManager.Instance.I18NGetText(story, StoryConfig.Choose1)+$"({overAD2}/{story.Count1})");
				}
				else
				{
					Button2Txt.SetText(I18NManager.Instance.I18NGetText(story, StoryConfig.Choose1));
				}
				
				Money1.SetActive(story.Type0 == 1);
				Money2.SetActive(story.Type1 == 1);
				MoneyText1.SetNum(story.Count0);
				MoneyText2.SetNum(story.Count1);
			}
		}

		public void OnDisable()
		{
			if (newPrice != null)
			{
				IAuctionManager.Instance.SetMiniGameResult(configId, newPrice);
				Messager.Instance.Broadcast(0, MessageId.SetChangePriceResult, configId, newPrice, false);
			}
		}

		public override async ETTask CloseSelf()
		{
			using ListComponent<ETTask> tasks = ListComponent<ETTask>.Create();
			tasks.Add(Animator.Play("UIItemStoryWin_Quit"));
			tasks.Add(DoMoveImageBack());
			await ETTaskHelper.WaitAll(tasks);
			await base.CloseSelf();
		}

		#endregion

		#region 事件绑定

		private async ETTask OnChoose(int index)
		{
			Money2.SetActive(false);
			Money1.SetActive(false);
			Ad1.SetActive(false);
			Ad2.SetActive(false);
			var story = StoryConfig;
			int range = 100;
			var old = GetItemPrice();
			var success = Random.Range(0, 100) <
			              (index == 0 ? story.Choose0SuccessPercent : story.Choose1SuccessPercent);
			if (success)
			{
				range = Random.Range(index == 0 ? story.ResultSucc0[0] : story.ResultSucc1[0],
					index == 0 ? story.ResultSucc0[1] : story.ResultSucc1[1]);
				newPrice = old * range / 100;
				BigNumber.Round2Integer(newPrice);
				Desc.SetText(I18NManager.Instance.I18NGetText(story,
					index == 0 ? StoryConfig.ResultS0 : StoryConfig.ResultS1));
			}
			else
			{
				range = Random.Range(index == 0 ? story.ResultFail0[0] : story.ResultFail1[0],
					index == 0 ? story.ResultFail0[1] : story.ResultFail1[1]);
				newPrice = old * range / 100;
				BigNumber.Round2Integer(newPrice);
				Desc.SetText(I18NManager.Instance.I18NGetText(story,
					index == 0 ? StoryConfig.ResultF0 : StoryConfig.ResultF1));
				
			}
			Button2.SetActive(false);
			Button1Txt.SetI18NKey(I18NKey.Text_Comfirm_Result);
			if (StoryConfig.ChooseCount <= 1)
			{
				await Animator.Play("UIItemStoryWin_Open");
			}
			await Price.DoNum(newPrice);
			var change = range - 100;
			FailEx.SetActive(change < 0);
			if (change < 0)
			{
				SoundManager.Instance.PlaySound("Audio/Game/giveup.mp3");
			}
			else
			{
				SoundManager.Instance.PlaySound("Audio/Game/niceItem.mp3");
			}
			Success1.SetActive(change > 0);
			Success2.SetActive(change > 0);
			Price.SetText(I18NManager.Instance.TranslateMoneyToStr(newPrice) +
			              (change > 0
				              ? $"(<color={GameConst.WIN_COLOR}>+{change}%</color>)"
				              : $"(<color={GameConst.LOSS_COLOR}>{change}%</color>)"));
		}
		public void OnClickButton1()
		{
			OnClickButton1Async().Coroutine();
		}
		public void OnClickButton2()
		{
			OnClickButton2Async().Coroutine();
		}
		
		public async ETTask OnClickButton1Async()
		{
			if (StoryConfig.Type0 == 1)
			{
				if (PlayerDataManager.Instance.TotalMoney < StoryConfig.Count0)
				{
					UIToast.ShowToast(I18NKey.Notice_Common_LackOfMoney);
					return;
				}
				PlayerDataManager.Instance.ChangeMoney(-StoryConfig.Count0);
			}
			else if (StoryConfig.Type0 == 2)
			{
				try
				{
					Button1.SetInteractable(false);
					Button2.SetInteractable(false);
					bool over = true;
					while (overAD1 < StoryConfig.Count0)
					{
						var res = await AdManager.Instance.PlayAd();
						if (res)
						{
							overAD1++;
							Button1Txt.SetText(I18NManager.Instance.I18NGetText(StoryConfig, StoryConfig.Choose0)+$"({overAD1}/{StoryConfig.Count0})");
						}
						else
						{
							over = false;
							break;
						}
					}
					if (!over)
					{
						return;
					}
				}
				catch (Exception ex)
				{
					Log.Error(ex);
				}
				finally
				{
					Button1.SetInteractable(true);
					Button2.SetInteractable(true);
				}
			}
			if (newPrice != null)
			{
				CloseSelf().Coroutine();
			}
			else
			{
				OnChoose(0).Coroutine();
			}
		}
		public async ETTask OnClickButton2Async()
		{
			if (StoryConfig.Type1 == 1)
			{
				if (PlayerDataManager.Instance.TotalMoney < StoryConfig.Count1)
				{
					UIToast.ShowToast(I18NKey.Notice_Common_LackOfMoney);
					return;
				}
				PlayerDataManager.Instance.ChangeMoney(-StoryConfig.Count1);
			}
			else if (StoryConfig.Type1 == 2)
			{
				try
				{
					Button1.SetInteractable(false);
					Button2.SetInteractable(false);
					bool over = true;
					while (overAD2 < StoryConfig.Count1)
					{
						var res = await AdManager.Instance.PlayAd();
						if (res)
						{
							overAD2++;
							Button1Txt.SetText(I18NManager.Instance.I18NGetText(StoryConfig, StoryConfig.Choose1)+$"({overAD2}/{StoryConfig.Count1})");
						}
						else
						{
							over = false;
							break;
						}
					}
					if (!over)
					{
						return;
					}
				}
				catch (Exception ex)
				{
					Log.Error(ex);
				}
				finally
				{
					Button1.SetInteractable(true);
					Button2.SetInteractable(true);
				}
			}
			OnChoose(1).Coroutine();
		}
		#endregion

		private async ETTask DoMoveImage()
		{
			Target.Icon.SetActive(false);
			var target = Target.Icon.GetRectTransform();
			var trans = Icon.GetRectTransform();
			trans.position = target.position;
			var startSize = trans.sizeDelta = target.sizeDelta;
			var startPos = trans.anchoredPosition;
			var timeStart = TimerManager.Instance.GetTimeNow();
			int during = 1000;
			while (true)
			{
				await TimerManager.Instance.WaitAsync(1);
				var timeNow =  TimerManager.Instance.GetTimeNow();
				var delta = timeNow - timeStart;
				var flag = (float) delta / during;
				trans.sizeDelta = Vector2.Lerp(startSize, endSize, flag);
				trans.anchoredPosition = Vector2.Lerp(startPos, Vector2.zero, flag);
				if (delta > during)
				{
					break;
				}
			}

			trans.sizeDelta = endSize;
			trans.anchoredPosition = Vector2.zero;
		}
		
		private async ETTask DoMoveImageBack()
		{
			Target.Icon.SetActive(false);
			var target = Target.Icon.GetRectTransform();
			var trans = Icon.GetRectTransform();
			trans.position = target.position;
			var startSize = trans.sizeDelta = target.sizeDelta;
			var startPos = trans.anchoredPosition;
			var timeStart = TimerManager.Instance.GetTimeNow();
			int during = 1000;
			while (true)
			{
				await TimerManager.Instance.WaitAsync(1);
				var timeNow =  TimerManager.Instance.GetTimeNow();
				var delta = timeNow - timeStart;
				var flag = (float) delta / during;
				trans.sizeDelta = Vector2.Lerp(endSize, startSize, flag);
				trans.anchoredPosition = Vector2.Lerp(Vector2.zero, startPos, flag);
				if (delta > during)
				{
					break;
				}
			}
			
			Target.Icon.SetActive(true);
		}
		private BigNumber GetItemPrice()
		{
			var cfg = ItemConfig;
			var gameInfoConfig = IAuctionManager.Instance?.GetFinalGameInfoConfig();//情报增加价格
			if (gameInfoConfig != null)
			{
				return gameInfoConfig.GetItemPrice(configId);
			}
			return cfg.Price;
		}
	}
}
