using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace TaoTie
{
	public class UITurnTableEventView : UIBaseView, IOnCreate, IOnEnable<BigNumber, int>, IUpdate
	{
		private const int REWARDS_COUNT = 7;
		private int during = 1000;
		public static string PrefabPath => "UIGame/UIMiniGame/Prefabs/UITurnTableEventView.prefab";
		public UIAnimator UICommonView;
		public UIImage[] Bgs;
		public UIImage[] Types;
		public UIEmptyView Arrow;
		public UITextmesh Text;
		public UIButton StopBtn;
		public UIButton AdBtn;
		public UITextmesh StopText;
		public UIAnimator Light;
		public UICashGroup CashGroup;
		private UIEmptyView Fireworks;
		private UIEmptyView Fail;
		public UITextmesh Talk;

		private bool winLoss = false;
		private long lastFireworksTime;
		private EasingFunction.Function ease;

		private List<Turntable2RewardsConfig> list;
		private float min;
		private float max;
		private float current;
		private Turntable2RewardsConfig MaxRewards;
		private BigNumber old;
		private BigNumber newP;
		#region override
		public void OnCreate()
		{
			Talk = AddComponent<UITextmesh>("UICommonView/Bg/Content/Human/Talk/Text");
			Fail = AddComponent<UIEmptyView>("Fail");
			StopText = AddComponent<UITextmesh>("UICommonView/Bg/Content/Table/Buttons/StopBtn/Text");
			Fireworks = AddComponent<UIEmptyView>("Fireworks");
			CashGroup = AddComponent<UICashGroup>("CashGroup");
			Light = AddComponent<UIAnimator>("UICommonView/Bg/Content/Light");
			ease = EasingFunction.GetEasingFunction(EasingFunction.Ease.EaseInOutQuad);
			this.UICommonView = this.AddComponent<UIAnimator>("UICommonView");
			this.Types = new UIImage[REWARDS_COUNT];
			Bgs= new UIImage[REWARDS_COUNT];
			for (int i = 0; i < Types.Length; i++)
			{
				Bgs[i] = this.AddComponent<UIImage>($"UICommonView/Bg/Content/Table/Type/Bg{i}");
				this.Types[i] = this.AddComponent<UIImage>($"UICommonView/Bg/Content/Table/Type/Bg{i}/Type");
			}

			Arrow = AddComponent<UIEmptyView>("UICommonView/Bg/Content/Table/Type/Arraw");
			this.Text = this.AddComponent<UITextmesh>("UICommonView/Bg/Content/Table/Price/Text");
			this.StopBtn = this.AddComponent<UIButton>("UICommonView/Bg/Content/Table/Buttons/StopBtn");
			this.AdBtn = this.AddComponent<UIButton>("UICommonView/Bg/Content/Table/Buttons/AdBtn");
			// this.Close = this.AddComponent<UIButton>("UICommonView/Bg/Close");
		}
		public void OnEnable(BigNumber price, int lv)
		{
			winLoss = false;
			lastFireworksTime = 0;
			old = price;
			newP = null;
			Fail.SetActive(false);
			Fireworks.SetActive(false);
			list = null;
			if (!Turntable2RewardsConfigCategory.Instance.TryGet(lv, PlayerDataManager.Instance.RestaurantLv, out list)
			    || list.Count < REWARDS_COUNT)
			{
				Log.Error("Turntable2RewardsConfigCategory not found lv = " + lv);
				CloseSelf().Coroutine();
				return;
			}
			Talk.SetI18NKey(I18NKey.Text_Trun_Notice);
			during = Random.Range(600, 1100);
			AdBtn.SetActive(AdManager.Instance.PlatformHasAD());
			this.StopBtn.SetOnClick(OnClickStopBtn);
			StopText.SetI18NKey(I18NKey.Text_Repair_Stop);
			this.AdBtn.SetOnClick(OnClickAdBtn);
			// this.Close.SetOnClick(OnClickClose);
			min = 180;
			max = -180;
			MaxRewards = list[0];
			var maxPrice = MaxRewards.RewardPercent[0];
			for (int i = 0; i < list.Count; i++)
			{
				for (int j = 0; j < list[i].Range.Length; j++)
				{
					if (list[i].Range[j] > max)
					{
						max = list[i].Range[j];
					}
					if (list[i].Range[j] < min)
					{
						min = list[i].Range[j];
					}

					if (list[i].RewardPercent[0] > maxPrice)
					{
						MaxRewards = list[i];
						maxPrice = list[i].RewardPercent[0];
					}
					
					if (list[i].RewardPercent[1] > maxPrice)
					{
						MaxRewards = list[i];
						maxPrice = list[i].RewardPercent[1];
					}
				}
			}
	
			current = Random.Range(min, max);
			for (int i = 0; i < REWARDS_COUNT; i++)
			{
				Bgs[i].SetEnabled(false);
				this.Types[i].SetSpritePath(I18NManager.Instance.I18NGetText(list[i])).Coroutine();
			}
			Text.SetNum(old);
		}
		
		public override async ETTask CloseSelf()
		{
			using ListComponent<ETTask> task = ListComponent<ETTask>.Create(); 
			task.Add(ChangeMoney());
			task.Add(UICommonView.Play("UIView_Close"));
			Fail.SetActive(false);
			await ETTaskHelper.WaitAll(task);
			await base.CloseSelf();
			winLoss = false;
		}

		private async ETTask ChangeMoney()
		{
			if (newP != null)
			{
				if (newP > 0)
				{
					PlayerDataManager.Instance.RecordWinToday(newP);
					await CashGroup.DoMoneyMoveAnim(newP, Text.GetRectTransform().position, 5);
				}
				PlayerDataManager.Instance.ChangeMoney(newP);
				newP = null;
			}
		}

		public void Update()
		{
			if (list == null) return;
			if (newP != null)
			{
				if (winLoss && lastFireworksTime + 1000 < TimerManager.Instance.GetTimeNow())
				{
					lastFireworksTime = TimerManager.Instance.GetTimeNow();
					Fireworks.GetTransform().localPosition = new Vector3(
						Random.Range(-Define.DesignScreenWidth / 2, Define.DesignScreenWidth / 2),
						Random.Range(-Define.DesignScreenHeight / 2, Define.DesignScreenHeight / 2), 0);
					Fireworks.SetActive(false);
					Fireworks.SetActive(true);
				}
				return;
			}
			Fireworks.SetActive(false);
			var time = TimerManager.Instance.GetTimeNow();
			var cost = time % (2 * during);
			current = ease.Invoke(Mathf.Abs(cost - during), min, max - min, during);
			Arrow.GetRectTransform().localEulerAngles = new Vector3(0, 0, current);
			for (int i = 0; i < REWARDS_COUNT; i++)
			{
				Bgs[i].SetEnabled(current <= list[i].Range[0] == current >= list[i].Range[1]);
			}
		}
		#endregion

		#region 事件绑定

		public void OnClickStopBtn()
		{
			if(newP != null) return;
			OnClickStopBtnAsync().Coroutine();
		}

		private async ETTask OnClickStopBtnAsync()
		{
			this.StopBtn.SetOnClick(OnClickClose);
			StopBtn.SetInteractable(false);
			StopText.SetI18NKey(I18NKey.Global_Btn_Back);
			Light.Play("Turntable_LightFlash").Coroutine();
			var time = TimerManager.Instance.GetTimeNow();
			var cost = time % (2 * during);
			current = ease.Invoke(Mathf.Abs(cost - during), min, max - min, during);
			Turntable2RewardsConfig config = null;
			int index = -1;
			for (int i = 0; i < list.Count; i++)
			{
				var max = Mathf.Max(list[i].Range[0], list[i].Range[1]);
				var min = Mathf.Min(list[i].Range[0], list[i].Range[1]);
				if (current > max)
				{
					index = i;
					current = max;
					config = list[i];
					break;
				}
				if (current > min)
				{
					index = i;
					config = list[i];
					break;
				}
			}
			Arrow.GetRectTransform().localEulerAngles = new Vector3(0, 0, current);
			for (int i = 0; i < REWARDS_COUNT; i++)
			{
				Bgs[i].SetEnabled(i == index);
			}
			var res = Random.Range(config.RewardPercent[0], config.RewardPercent[1]);
			res -= 100;
			Log.Info(res);
			winLoss = res >= 0;
			Fail.SetActive(!winLoss);
			newP = old * res / 100;
			SoundManager.Instance.PlaySound("Audio/Game/pointerStop.mp3");
			await TimerManager.Instance.WaitAsync(500);
			Text.DoNum(newP + old).Coroutine();
			Text.SetTextColor(newP >= 0 ? GameConst.GREEN_COLOR : GameConst.RED_COLOR);
			Talk.SetI18NKey(newP >= 0 ? I18NKey.Text_CS_Win : I18NKey.Text_CS_Loss,
				I18NManager.Instance.TranslateMoneyToStr(newP).Replace("-",""));
			StopBtn.SetInteractable(true);
			
		}
		public void OnClickAdBtn()
		{
			if (AdManager.Instance.PlatformHasAD())
			{
				OnClickAdBtnAsync().Coroutine();
			}
		}

		private async ETTask OnClickAdBtnAsync()
		{
			AdBtn.SetInteractable(false);
			StopBtn.SetInteractable(false);
			try
			{
				var res = await AdManager.Instance.PlayAd();
				if (res)
				{
					winLoss = true;
					Fail.SetActive(false);
					var max = Mathf.Max(MaxRewards.Range[0], MaxRewards.Range[1]);
					var min = Mathf.Min(MaxRewards.Range[0], MaxRewards.Range[1]);
					current = Random.Range(min, max);
					Arrow.GetRectTransform().localEulerAngles = new Vector3(0, 0, current);
					var percent = Random.Range(MaxRewards.RewardPercent[0], MaxRewards.RewardPercent[1]);
					newP = old * percent / 100;
					Text.DoNum(newP + old).Coroutine();
					SoundManager.Instance.PlaySound("Audio/Game/pointerStop.mp3");
					
					Text.SetTextColor(newP >= 0 ? GameConst.GREEN_COLOR : GameConst.RED_COLOR);
					for (int i = 0; i < REWARDS_COUNT; i++)
					{
						Bgs[i].SetEnabled(current <= list[i].Range[0] == current >= list[i].Range[1]);
					}
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex);
			}
			finally
			{
				AdBtn.SetInteractable(true);
				StopBtn.SetInteractable(true);
			}
			
		}
		public void OnClickClose()
		{
			CloseSelf().Coroutine();
		}
		#endregion
	}
}
