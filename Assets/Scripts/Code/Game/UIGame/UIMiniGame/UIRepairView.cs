using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace TaoTie
{
	/// <summary>
	/// 修理，情报一开始就作用到物品
	/// </summary>
	public class UIRepairView : UICommonMiniGameView, IOnDisable,IUpdate
	{
		public static string PrefabPath => "UIGame/UIMiniGame/Prefabs/UIRepairView.prefab";
		public UIImage Item;
		public UIImage Mask;
		public UIEmptyView RectMask;
		public UIEmptyView Child;
		public UIImage Icon;
		public UIButton StartButton;
		public UIButton AdBtn;
		public UITextmesh AdBtnText;
		public UITextmesh StartText;
		public UITextmesh Count;
		public UIImage Light;
		
		public RepairConfig Config => RepairConfigCategory.Instance.Get(configId);
		private BigNumber newPrice;

		private BigNumber oldPrice;
		private bool isRunning;
		private long startTime;
		private Vector2 startPos;
		private Vector2 endPos;
		#region override
		public override void OnCreate()
		{
			base.OnCreate();
			Light = AddComponent<UIImage>("View/Bg/Content/UIItem/Image/Light");
			StartText = AddComponent<UITextmesh>("View/Bg/Content/Buttons/StartBtn/Text");
			this.Item = this.AddComponent<UIImage>("View/Bg/Content/UIItem/Image/Icon");
			this.Mask = this.AddComponent<UIImage>("View/Bg/Content/UIItem/Image/Mask");
			RectMask = this.AddComponent<UIEmptyView>("View/Bg/Content/UIItem/Image/Icon/Child/Mask");
			this.Child = this.AddComponent<UIEmptyView>("View/Bg/Content/UIItem/Image/Icon/Child");
			this.Icon = this.AddComponent<UIImage>("View/Bg/Content/UIItem/Image/Icon/Child/Mask/Icon");
			this.StartButton = this.AddComponent<UIButton>("View/Bg/Content/Buttons/StartBtn");
			this.AdBtn = this.AddComponent<UIButton>("View/Bg/Content/Buttons/AdBtn");
			AdBtnText = AddComponent<UITextmesh>("View/Bg/Content/Buttons/AdBtn/Text");
			Count = AddComponent<UITextmesh>("View/Bg/Content/Buttons/AdBtn/Count");
			Count.SetI18NKey(I18NKey.Text_TurnTable_Count);
		}
		public override void OnEnable(int id)
		{
			base.OnEnable(id);
			Light.SetActive(false);
			newPrice = null;
			isRunning = false;
			oldPrice = GetItemPrice();
			Item.SetSpritePath(ItemConfig.ItemPic).Coroutine();
			Icon.SetSpritePath(ItemConfig.ItemPic).Coroutine();
			
			this.StartButton.SetOnClick(OnClickStartButton);
			this.AdBtn.SetOnClick(OnClickAdButton);
			StartText.SetI18NKey(I18NKey.Text_Repair_Start);
			AdBtn.SetActive(CanAd());
			StartButton.SetActive(true);
			var range = Random.Range(0, 360);
			var distance = Random.Range(0.1f, 0.25f);
			Vector2 pos = Quaternion.Euler(new Vector3(0, 0, range)) * Vector2.right*Icon.GetRectTransform().sizeDelta.x * distance;
			Mask.GetRectTransform().anchoredPosition = Item.GetRectTransform().anchoredPosition + pos;
			RectMask.GetRectTransform().anchoredPosition = pos;
			Icon.GetRectTransform().anchoredPosition = -pos;
			RectMask.GetRectTransform().sizeDelta = Mask.GetRectTransform().sizeDelta 
				= Icon.GetRectTransform().sizeDelta * Config.Size / 100;
			Child.GetRectTransform().anchoredPosition = new Vector2(-9999, -9999);
			var config = Config;
			var min = oldPrice * (config.FailMin / 100f);
			var max = oldPrice * (config.SuccessMax / 100f);
			BigNumber.Round2Integer(min);
			BigNumber.Round2Integer(max);
			Range.SetI18NText(min, max);
			Count.SetI18NText(Mathf.Max(0, GameConst.PlayableMaxAdCount - PlayerDataManager.Instance.PlayableCount));
		}

		public void Update()
		{
			if (isRunning)
			{
				var timeNow = TimerManager.Instance.GetTimeNow();
				var during = (float)(timeNow - startTime) / Config.During;
				Child.GetRectTransform().anchoredPosition = Vector2.Lerp(startPos, endPos, during);
				if (timeNow - startTime > Config.During)
				{
					ReStart();
				}
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

		#endregion

		#region 事件绑定
		public void OnClickStartButton()
		{
			StartText.SetI18NKey(I18NKey.Text_Repair_Stop);
			OnClickStartButtonAsync().Coroutine();
			StartButton.SetOnClick(OnClickStop);
		}

		private void OnClickStop()
		{
			var timeNow = TimerManager.Instance.GetTimeNow();
			var during = (timeNow - startTime) / (float)Config.During;
			if (GameSetting.PlayableResult == PlayableResult.Success)
			{
				during = 0;
			}
			else if (GameSetting.PlayableResult == PlayableResult.Fail)
			{
				during = 1;
			}
			//Mathf.Abs(during - 0.5f)/0.5f*100 < Config.Success
			OnResult(Mathf.Abs(during - 0.5f) * 200);
			StartButton.SetActive(false);
		}

		public async ETTask OnClickStartButtonAsync()
		{
			await TimerManager.Instance.WaitAsync(1);
			isRunning = true;
			ReStart();
		}
		
		public void OnClickAdButton()
		{
			OnClickAdBtnAsync().Coroutine();
		}
		#endregion

		private void ReStart()
		{
			startTime = TimerManager.Instance.GetTimeNow();
			var range = Random.Range(0, 360);
			startPos = Quaternion.Euler(new Vector3(0, 0, range)) * Vector2.right * 500;
			endPos = -startPos;
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

		public async ETTask OnClickAdBtnAsync()
		{
			var res = await PlayAd();
			if (res)
			{
				Child.GetRectTransform().anchoredPosition = Vector2.zero;
				newPrice = Config.SuccessMax / 100f * oldPrice;
				BigNumber.Round2Integer(newPrice);
				AdBtn.SetActive(false);
				SetItemWinLossWithContainer(newPrice - oldPrice);
				StartButton.SetActive(false);
				Count.SetI18NText(Mathf.Max(0, GameConst.PlayableMaxAdCount - PlayerDataManager.Instance.PlayableCount));
				Light.SetColor(GameConst.GREEN_COLOR);
			}
			else
			{
				AdBtn.SetInteractable(true);
			}
		}
		
		private void OnResult(float progress)
		{
			progress = Mathf.Clamp(progress, 0, 100);
			var config = Config;
			isRunning = false;
			AdBtn.SetActive(progress > 0 && CanAd());
			if (progress <= Config.Success)
			{
				float val = 1 -  progress / Config.Success;
				newPrice = Mathf.Lerp(config.SuccessMin, config.SuccessMax, val) / 100f * oldPrice;
				Light.SetColor(GameConst.GREEN_COLOR);
			}
			else
			{
				float val = (progress - Config.Success) / (100 - Config.Success);
				newPrice = Mathf.Lerp(config.FailMax, config.FailMin, val) / 100f * oldPrice;
				Light.SetColor(GameConst.RED_COLOR);
			}
			BigNumber.Round2Integer(newPrice);
			SetItemWinLossWithContainer(newPrice - oldPrice);
			Light.SetActive(true);
		}

		protected override void AfterPlayAd(int total, int cur)
		{
			base.AfterPlayAd(total, cur);
			AdBtnText.SetText(I18NManager.Instance.I18NGetText(I18NKey.Text_Repair_Ad)+$"({cur}/{total})");
		}
	}
}
