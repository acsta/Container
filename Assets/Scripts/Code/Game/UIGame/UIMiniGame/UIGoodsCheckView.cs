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
	/// 验货，情报一开始就作用到物品
	/// </summary>
	public class UIGoodsCheckView : UICommonMiniGameView, IOnDisable
	{
		public static string PrefabPath => "UIGame/UIMiniGame/Prefabs/UIGoodsCheckView.prefab";
		public UIImage Icon;
		
		public UITextmesh Ques;
		public UIButton Ans0;
		public UIButton Ans1;
		public UITextmesh AnsText0;
		public UITextmesh AnsText1;
		public UITextmesh Count;

		public UIButton AdBtn;
		public UITextmesh AdBtnText;
		public UIAnimator Buttons;
		public UIAnimator Right;
		public GoodsCheckConfig Config => GoodsCheckConfigCategory.Instance.Get(configId);
		private BigNumber newPrice;
		private BigNumber oldPrice;
		#region override
		public override void OnCreate()
		{
			base.OnCreate();
			Right = AddComponent<UIAnimator>("View/Bg/Content/UIItem/Image/Right");
			Buttons = AddComponent<UIAnimator>("View/Bg/Content/Buttons");
			this.Icon = this.AddComponent<UIImage>("View/Bg/Content/UIItem/Image/Icon");
			this.Ques = this.AddComponent<UITextmesh>("View/Bg/Content/Title");
			this.Ans0 = this.AddComponent<UIButton>("View/Bg/Content/Buttons/Ans0Btn");
			this.Ans1 = this.AddComponent<UIButton>("View/Bg/Content/Buttons/Ans1Btn");
			this.AnsText0 = this.AddComponent<UITextmesh>("View/Bg/Content/Buttons/Ans0Btn/Text");
			this.AnsText1 = this.AddComponent<UITextmesh>("View/Bg/Content/Buttons/Ans1Btn/Text");
			this.AdBtn = this.AddComponent<UIButton>("View/Bg/Content/Buttons/AdBtn");
			AdBtnText = AddComponent<UITextmesh>("View/Bg/Content/Buttons/AdBtn/Text");
			Count = AddComponent<UITextmesh>("View/Bg/Content/Buttons/AdBtn/Count");
			Count.SetI18NKey(I18NKey.Text_TurnTable_Count);
		}
		public override void OnEnable(int id)
		{
			base.OnEnable(id);
			Right.SetActive(false);
			newPrice = null;
			oldPrice = GetItemPrice();
			Icon.SetSpritePath(ItemConfig.ItemPic).Coroutine();
			Ques.SetText("");
			this.Ans0.SetOnClick(OnClickAns0);
			this.Ans1.SetOnClick(OnClickAns1);
			this.AdBtn.SetOnClick(OnClickAdButton);
			Ans0.SetActive(false);
			Ans1.SetActive(false);
			Ans0.SetSpritePath("UIGame/UIMiniGame/Atlas/button_red.png",true).Coroutine();
			Ans1.SetSpritePath("UIGame/UIMiniGame/Atlas/button_yellow.png",true).Coroutine();
			AdBtn.SetActive(CanAd());
			var config = Config;
			var min = oldPrice * (config.FailMin / 100f);
			var max = oldPrice * (config.SuccessMax / 100f);
			BigNumber.Round2Integer(min);
			BigNumber.Round2Integer(max);
			Range.SetI18NText(min, max);
			OnClickStartButton();
			Count.SetI18NText(Mathf.Max(0, GameConst.PlayableMaxAdCount - PlayerDataManager.Instance.PlayableCount));
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
		
		public void OnClickAns0()
		{
			if (newPrice != null) return;
			Ans0.SetSpritePath("UIGame/UIMiniGame/Atlas/button_red_outline.png",true).Coroutine();
			OnAnswer(Config.RightAns == 0, 0);
		}
		public void OnClickAns1()
		{
			if (newPrice != null) return;
			Ans1.SetSpritePath("UIGame/UIMiniGame/Atlas/button_yellow_outline.png",true).Coroutine();
			OnAnswer(Config.RightAns == 1, 1);
		}
		public void OnClickStartButton()
		{
			Ans0.SetActive(true);
			Ans1.SetActive(true);
			Buttons.Play("GoodsCheck_Answer").Coroutine();
			var cfg = Config;
			Ques.SetText(I18NManager.Instance.I18NGetText(cfg));
			AnsText0.SetText(I18NManager.Instance.I18NGetText(cfg,1));
			AnsText1.SetText(I18NManager.Instance.I18NGetText(cfg,2));
		}

		public void OnClickAdButton()
		{
			OnClickAdBtnAsync().Coroutine();
		}

		public async ETTask OnClickAdBtnAsync()
		{
			var res = await PlayAd();
			if (res)
			{
				newPrice = Random.Range(Config.SuccessMin, Config.SuccessMax + 1) / 100f * oldPrice;
				BigNumber.Round2Integer(newPrice);
				Ans0.SetSpritePath("UIGame/UIMiniGame/Atlas/button_red_outline.png", true).Coroutine();
				AnsText0.SetText(I18NManager.Instance.I18NGetText(Config, Config.RightAns + 1));
				Buttons.Play("GoodsCheck_AdOver").Coroutine();
				SetItemWinLossWithContainer(newPrice - oldPrice);
				Right.SetActive(true);
				Count.SetI18NText(Mathf.Max(0, GameConst.PlayableMaxAdCount - PlayerDataManager.Instance.PlayableCount));
			}
			else
			{
				AdBtn.SetInteractable(true);
			}
		}

		#endregion

		private void OnAnswer(bool res, int index)
		{
			var config = Config;
			if (res)
			{
				Buttons.Play($"GoodsCheck_Right{index}").Coroutine();
				newPrice = Random.Range(config.SuccessMin, config.SuccessMax + 1) / 100f * oldPrice;
				Right.SetActive(true);
			}
			else
			{
				Buttons.Play("GoodsCheck_Error").Coroutine();
				newPrice = Random.Range(config.FailMin, config.FailMax + 1) / 100f * oldPrice;
			}
			
			BigNumber.Round2Integer(newPrice);
			SetItemWinLossWithContainer(newPrice - oldPrice);
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

		protected override void AfterPlayAd(int total, int cur)
		{
			base.AfterPlayAd(total, cur);
			AdBtnText.SetText(I18NManager.Instance.I18NGetText(I18NKey.Text_Check_Ad)+$"({cur}/{total})");
		}
	}
}
