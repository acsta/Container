using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace TaoTie
{
	/// <summary>
	/// 检疫，用情报后的全局价格为基准
	/// </summary>
	public class UIQuarantineView : UICommonMiniGameView, IOnDisable,IOnDestroy
	{
		static readonly Color BASE_COLOR = new Color(0.4470589f, 0.4392157f, 0.3960785f, 1);
		public static string PrefabPath => "UIGame/UIMiniGame/Prefabs/UIQuarantineView.prefab";
		public UIImage Icon;
		public UIEventTrigger StartButton;
		public UIRawImage Mask;
		public UITextmesh MaskText;
		public UIButton AdBtn;
		public UITextmesh AdBtnText;
		public UIImage Qualified;
		public UITextmesh Desc; 
		public UITextmesh ResultText;
		public UITextmesh Count;
		private UIEmptyView Hit;
		public QuarantineConfig Config => QuarantineConfigCategory.Instance.Get(configId);

		private bool isSuccess;
		private BigNumber newPrice;
		private Texture baseTexture;
		private Texture2D maskTexture;
		private long startDragTime;
		private long totalDragTime;
		private long lastCheckTime;
		private Vector2 lastCheck;
		Vector3[] worldCorners = new Vector3[4];
		#region override
		public override void OnCreate()
		{
			base.OnCreate();
			Hit = AddComponent<UIEmptyView>("View/Bg/Content/Report/Result/Qualified/Hit");
			this.Icon = this.AddComponent<UIImage>("View/Bg/Content/Report/Icon");
			this.StartButton = this.AddComponent<UIEventTrigger>("View/Bg/Content/Report/Result/Mask");
			MaskText = AddComponent<UITextmesh>("View/Bg/Content/Report/Result/Mask/Text (TMP)");
			Mask = this.AddComponent<UIRawImage>("View/Bg/Content/Report/Result/Mask");
			this.AdBtn = this.AddComponent<UIButton>("View/Bg/Content/Buttons/AdBtn");
			AdBtnText = AddComponent<UITextmesh>("View/Bg/Content/Buttons/AdBtn/Text");
			Qualified = AddComponent<UIImage>("View/Bg/Content/Report/Result/Qualified");
			Desc = AddComponent<UITextmesh>("View/Bg/Content/Report/Desc");
			ResultText = AddComponent<UITextmesh>("View/Bg/Content/Report/Result/Text");
			Desc.SetI18NKey(I18NKey.Text_Quarantine_Report_Desc);
			ResultText.SetI18NKey(I18NKey.Text_Quarantine_Report_Result);
			baseTexture = Mask.GetTexture();
			Count = AddComponent<UITextmesh>("View/Bg/Content/Buttons/AdBtn/Count");
			Count.SetI18NKey(I18NKey.Text_TurnTable_Count);
			Range.SetI18NKey(I18NKey.Quarantine_Price_Range);
		}
		public override void OnEnable(int id)
		{
			base.OnEnable(id);
			Hit.SetActive(false);
			lastCheck = Vector2.zero;
			totalDragTime = 0;
			lastCheckTime = 0;
			var total = GetBasePrice();
			newPrice = null;
			var containerId = ItemConfigCategory.Instance.Get(configId).ContainerId;
			var container = ContainerConfigCategory.Instance.Get(containerId);
			Icon.SetSpritePath(container.Icon).Coroutine();
			StartButton.AddOnPointerDown(OnBeginDrag);
			StartButton.AddOnDrag(OnDrag);
			StartButton.AddOnPointerUp(OnEndDrag);
			this.AdBtn.SetOnClick(OnClickAdButton);
			AdBtn.SetActive(CanAd());
			StartButton.SetActive(true);
			MaskText.SetActive(true);
			var config = Config;
			var min = total * (config.FailMin / 100f - 1);
			var max = total * (config.SuccessMax / 100f - 1);
			BigNumber.Round2Integer(min);
			BigNumber.Round2Integer(max);
			Range.SetI18NText(min, max);
			Desc.SetI18NText(IAuctionManager.Instance.Boxes.Count - 1);
			if (maskTexture != null)
			{
				GameObject.Destroy(maskTexture);
			}
			maskTexture = new Texture2D(baseTexture.width, baseTexture.height);
			maskTexture.wrapMode = TextureWrapMode.Clamp;
			maskTexture.filterMode = FilterMode.Point;
			for (int i = 0; i < maskTexture.width; i++)
			{
				for (int j = 0; j < maskTexture.height; j++)
				{
					maskTexture.SetPixel(i, j, BASE_COLOR);
				}
			}
			maskTexture.Apply();
			Mask.SetTexture(maskTexture);
			Mask.SetImageColor(Color.white);
			if (GameSetting.PlayableResult == PlayableResult.Success)
			{
				isSuccess = true;
			}
			else if (GameSetting.PlayableResult == PlayableResult.Fail)
			{
				isSuccess = false;
			}
			else
			{
				isSuccess = Random.Range(0, 100) < config.Percent;
			}
			if (isSuccess)
			{
				Qualified.SetSpritePath("UIGame/UIMiniGame/Atlas/qualified.png").Coroutine();
			}
			else
			{
				Qualified.SetSpritePath("UIGame/UIMiniGame/Atlas/unqualified.png").Coroutine();
			}
			ResultText.SetI18NText("?");
			Qualified.SetActive(false);
			Count.SetI18NText(Mathf.Max(0, GameConst.PlayableMaxAdCount - PlayerDataManager.Instance.PlayableCount));
		}

		public void OnDisable()
		{
			if (maskTexture != null)
			{
				GameObject.Destroy(maskTexture);
				maskTexture = null;
				Mask.SetTexture(baseTexture);
			}
			if (newPrice != null)
			{
				IAuctionManager.Instance.SetMiniGameResult(configId, newPrice);
				Messager.Instance.Broadcast(0, MessageId.SetChangePriceResult, configId, newPrice, false);
			}
		}

		public void OnDestroy()
		{
			if (maskTexture != null)
			{
				GameObject.Destroy(maskTexture);
				maskTexture = null;
			}
			Mask.SetTexture(baseTexture);
		}

		#endregion

		#region 事件绑定

		public void OnBeginDrag(PointerEventData data)
		{
			lastCheck = Vector2.zero;
			startDragTime = TimerManager.Instance.GetTimeNow();
			MaskText.SetActive(false);
		}
		
		public void OnEndDrag(PointerEventData data)
		{
			lastCheck = Vector2.zero;
			var timeNow = TimerManager.Instance.GetTimeNow();
			totalDragTime += timeNow - startDragTime;
			
			if (totalDragTime > 3000 && newPrice == null)
			{
				ShowResult().Coroutine();
			}
		}
		public void OnDrag(PointerEventData data)
		{
			
			if (newPrice != null) return;
			Mask.GetRectTransform().GetWorldCorners(worldCorners);
			Vector2 min = RectTransformUtility.WorldToScreenPoint(UIManager.Instance.UICamera, worldCorners[0]);//左下
			Vector2 max = RectTransformUtility.WorldToScreenPoint(UIManager.Instance.UICamera, worldCorners[2]);//右上
			if (lastCheck == Vector2.zero)
			{
				Touch(data.position, min,max);
			}
			else
			{
				var dis = Vector2.Distance(data.position, lastCheck);
				for (int i = 0; i < dis; i += 20)
				{
					var temp = Vector2.Lerp(data.position, lastCheck, i / dis);
					Touch(temp, min, max);
				}
			}
			lastCheck = data.position;
			maskTexture.Apply();
			var timeNow = TimerManager.Instance.GetTimeNow();
			if (timeNow + totalDragTime > startDragTime + 3000)
			{
				ShowResult().Coroutine();
			}
			else if (timeNow > lastCheckTime + 100)
			{
				lastCheckTime = timeNow;
				float total = 0;
				for (int i = 0; i < baseTexture.width; i++)
				{
					for (int j = 0; j < baseTexture.height; j++)
					{
						total += 1 - Mathf.Clamp01(maskTexture.GetPixel(i, j).a);
					}
				}

				long totalSize = baseTexture.width * baseTexture.height / 2;
				if (total > totalSize)
				{
					ShowResult().Coroutine();
				}
			}
		}

		private void Touch(Vector2 position,Vector2 min,Vector2 max)
		{
			int x = (int) ((position.x - min.x) / (max.x - min.x) * baseTexture.width);
			int y = (int) ((position.y - min.y) / (max.y - min.y) * baseTexture.height);
			var range = baseTexture.width / 20;
			for (int i = x - range; i < x + range; i++)
			{
				if (i < 0 || i >= baseTexture.width)
				{
					continue;
				}
				for (int j = y - range; j < y + range; j++)
				{
					if (j < 0 || j >= baseTexture.height)
					{
						continue;
					}
					var dis = Mathf.Sqrt(Mathf.Pow(i - x, 2) + Mathf.Pow(j - y, 2));
					dis = Mathf.Clamp01(dis / range);
					if (dis < 0.9)
					{
						dis = 1;
					}
					else
					{
						dis = (1 - dis) * 10;
					}
					maskTexture.SetPixel(i, j, Color.Lerp(maskTexture.GetPixel(i, j), Color.clear, dis));
				}
			}
		}
		public async ETTask ShowResult()
		{
			var config = Config;
			var basePrice = GetBasePrice();
			BigNumber price;
			if (isSuccess)
			{
				price = Random.Range(config.SuccessMin, config.SuccessMax + 1) / 100f * basePrice;
			}
			else
			{
				price = Random.Range(config.FailMin, config.FailMax + 1) / 100f * basePrice;
			}

			newPrice = price - basePrice;
			BigNumber.Round2Integer(newPrice);
			var startTime = TimerManager.Instance.GetTimeNow();
			// var textColor = ResultText.GetTextColor();
			// var startColor = new Color(textColor.r, textColor.g, textColor.b, 0);
			// var endColor = new Color(textColor.r, textColor.g, textColor.b, 1);
			// ResultText.SetTextColor(startColor);
			if (isSuccess)
			{
				Qualified.SetSpritePath("UIGame/UIMiniGame/Atlas/qualified.png").Coroutine();
				ResultText.SetI18NText($"<color={GameConst.GREEN_COLOR}>{I18NManager.Instance.I18NGetParamText(I18NKey.Text_Quarantine_Add, newPrice)}</color>");
			}
			else
			{
				Qualified.SetSpritePath("UIGame/UIMiniGame/Atlas/unqualified.png").Coroutine();
				ResultText.SetI18NText($"<color={GameConst.RED_COLOR}>{I18NManager.Instance.I18NGetParamText(I18NKey.Text_Quarantine_Reduce, -newPrice)}</color>");
			}
			
			while (true)
			{
				await TimerManager.Instance.WaitAsync(1);
				var timeNow = TimerManager.Instance.GetTimeNow();
				var during = (timeNow - startTime) / 1000f;
				Mask.SetImageColor(Color.Lerp(Color.white, Color.clear, during));
				// ResultText.SetTextColor(Color.Lerp(startColor, endColor, during));
				if (timeNow - startTime > 1000)
				{
					break;
				}
			}

			Qualified.SetActive(true);
			// ResultText.SetTextColor(endColor);
			StartButton.SetActive(false);
			SetContainerWinLoss(newPrice + containerWinLoss - IAuctionManager.Instance.LastAuctionPrice);
			AdBtn.SetActive(!isSuccess && CanAd());
		}
	
		public void OnClickAdButton()
		{
			AdBtn.SetInteractable(false);
			OnClickAdBtnAsync().Coroutine();
		}
		#endregion

		public async ETTask OnClickAdBtnAsync()
		{
			var res = await PlayAd();
			if (res)
			{
				Qualified.SetActive(false);
				Hit.SetActive(false);
				var config = Config;
				var basePrice = GetBasePrice();
				var price = Random.Range(config.SuccessMin, config.SuccessMax + 1) / 100f * basePrice;
			
				newPrice = price - basePrice;
				BigNumber.Round2Integer(newPrice);
				SetContainerWinLoss(newPrice + containerWinLoss - IAuctionManager.Instance.LastAuctionPrice);
				Qualified.SetSpritePath("UIGame/UIMiniGame/Atlas/qualified.png").Coroutine();
				Qualified.SetActive(true);
				ResultText.SetI18NText(
					$"<color={GameConst.GREEN_COLOR}>{I18NManager.Instance.I18NGetParamText(I18NKey.Text_Quarantine_Add, newPrice)}</color>");
				AdBtn.SetActive(false);
				StartButton.SetActive(false);
				Count.SetI18NText(Mathf.Max(0, GameConst.PlayableMaxAdCount - PlayerDataManager.Instance.PlayableCount));
			}
			else
			{
				AdBtn.SetInteractable(true);
			}
		}

		private BigNumber GetBasePrice()
		{
			if (IAuctionManager.Instance != null)//情报增加价格
			{
				var gameInfoConfig = IAuctionManager.Instance.GetFinalGameInfoConfig();
				if (gameInfoConfig != null && gameInfoConfig.IsTargetItem(ItemConfig))
				{
					if (gameInfoConfig.AwardType == 0)
					{
						return IAuctionManager.Instance.AllPrice + gameInfoConfig.RewardCount;
					}
					else if (gameInfoConfig.AwardType == 1)
					{
						return IAuctionManager.Instance.AllPrice * gameInfoConfig.RewardCount;
					}
				}
				return IAuctionManager.Instance.AllPrice;
			}
			return 0;
		}

		protected override void AfterPlayAd(int total, int cur)
		{
			base.AfterPlayAd(total, cur);
			AdBtnText.SetText(I18NManager.Instance.I18NGetText(I18NKey.Text_Quarantine_Ad)+$"({cur}/{total})");
		}
	}
}
