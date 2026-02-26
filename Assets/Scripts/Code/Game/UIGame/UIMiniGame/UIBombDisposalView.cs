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
	/// 拆弹，用情报后的全局价格为基准
	/// </summary>
	public class UIBombDisposalView : UICommonMiniGameView, IOnDisable
	{
		public static string PrefabPath => "UIGame/UIMiniGame/Prefabs/UIBombDisposalView.prefab";
		
		public UIButton AdBtn;
		public UITextmesh AdBtnText;
		public UITextmesh Count;
		public UIEmptyView Explode;
		public UIButton LeftButton;
		public UIButton RightButton;
		public UIRawImage writeImage;
		public UITextmesh A;
		public UITextmesh B;

		private int diffId;
		public BombDisposalConfig Config =>BombDisposalConfigCategory.Instance.Get(diffId);
		
		private BigNumber newPrice;
		private Texture2D texture2D;
		private int res;
		#region override
		public override void OnCreate()
		{
			base.OnCreate();
			this.AdBtn = this.AddComponent<UIButton>("View/Bg/Content/Buttons/AdBtn");
			LeftButton = this.AddComponent<UIButton>("View/Bg/Content/Buttons/LeftBtn");
			RightButton = this.AddComponent<UIButton>("View/Bg/Content/Buttons/RightBtn");
			writeImage = this.AddComponent<UIRawImage>("View/Bg/Content/UIItem/RawImage");
			A = this.AddComponent<UITextmesh>("View/Bg/Content/A");
			B = this.AddComponent<UITextmesh>("View/Bg/Content/B");
			 
			AdBtnText = AddComponent<UITextmesh>("View/Bg/Content/Buttons/AdBtn/Text");
			Explode = AddComponent<UIEmptyView>("View/Bg/Content/UIItem/Explode");
			Count = AddComponent<UITextmesh>("View/Bg/Content/Buttons/AdBtn/Count");
			Count.SetI18NKey(I18NKey.Text_TurnTable_Count);
			Range.SetI18NKey(I18NKey.Quarantine_Price_Range);
		}
		public override void OnEnable(int id)
		{
			base.OnEnable(id);
			var total = GetBasePrice();
			Explode.SetActive(false);
			A.SetActive(false);
			B.SetActive(false);
			newPrice = null;

			var weight = BombDisposalConfigCategory.Instance.TotalWeight;
			var target = Random.Range(0, weight);
			var list = BombDisposalConfigCategory.Instance.GetAllList();
			for (int i = 0; i < list.Count; i++)
			{
				diffId = list[i].Id;
				target -= list[i].Weight;
				if (target <= 0)
				{
					break;
				}
			}

			this.AdBtn.SetOnClick(OnClickAdButton);
			LeftButton.SetOnClick(OnClickRes0);
			RightButton.SetOnClick(OnClickRes1);
			AdBtn.SetActive(false);
			LeftButton.SetActive(true);
			RightButton.SetActive(true);
			var config = Config;
			var min = total * (config.FailMin / 100f - 1);
			var max = total * (config.SuccessMax / 100f - 1);
			BigNumber.Round2Integer(min);
			BigNumber.Round2Integer(max);
			Range.SetI18NText(min, max);
			Count.SetI18NText(Mathf.Max(0, GameConst.PlayableMaxAdCount - PlayerDataManager.Instance.PlayableCount));
			DrawLine();
		}

		private void DrawLine()
		{
			res = Random.Range(0, 2);
			if (texture2D != null)
			{
				Texture2D.DestroyImmediate(texture2D);
			}
			texture2D = new Texture2D(512, 512);
			int lineWidth = 3;
			int headWidth = lineWidth * 4;
			int headHeight = lineWidth * 4 * 4;

			int minHeadGap = headWidth;
			int[] headPosX = new int[Config.LineCount + 2];
			{
				headPosX[0] = Random.Range(0, texture2D.width - lineWidth * 4);
				while (true)
				{
					int newPosX = Random.Range(0, texture2D.width - lineWidth * 4);
					if (Mathf.Abs(newPosX - headPosX[0]) < minHeadGap)
					{
						continue;
					}
				
					headPosX[1] = newPosX;
					break;
				}
			
				for (int i = 2; i < headPosX.Length; i++)
				{
					int newPosX;
					while (true)
					{
						newPosX = Random.Range(0, texture2D.width - lineWidth * 4);
						bool isSuit = true;
						for (int j = 0; j < i; ++j)
						{
							if (Mathf.Abs(newPosX - headPosX[j]) < minHeadGap)
							{
								isSuit = false;
								break;
							}
						}

						if (isSuit)
						{
							headPosX[i] = newPosX;
							break;	
						}
					}
				}
				
				var color = new Color[headWidth * headHeight];
				for (int i = 0; i < color.Length; ++i)
				{
					color[i] = Color.red;
				}
				texture2D.SetPixels(headPosX[0], 512 - headHeight, headWidth, headHeight, color);
			
				for (int i = 0; i < color.Length; ++i)
				{
					color[i] = Color.blue;
				}
				texture2D.SetPixels(headPosX[1], 512 - headHeight, headWidth, headHeight, color);
			
				for (int i = 0; i < color.Length; ++i)
				{
					color[i] = Color.gray;
				}
				for (int i = 2; i < headPosX.Length; ++i)
				{
					texture2D.SetPixels(headPosX[i], 512 - headHeight, headWidth, headHeight, color);
				}
			}

			int turnCount = 7;
			int yCount = turnCount % 2 != 0 ? Mathf.FloorToInt(turnCount / 2) + 1 : Mathf.FloorToInt(turnCount / 2);
			int xCount = Mathf.FloorToInt(turnCount / 2);
			int minGap = lineWidth * 2;
			using HashSetComponent<int> historyPosX = HashSetComponent<int>.Create();
			using HashSetComponent<int> historyPosY = HashSetComponent<int>.Create();
			{
				int totalHeight = 512 - headHeight;
				int totalWidth = 512;
				int randomRange = 50;
				for (int i = 0; i < headPosX.Length; ++i)
				{
					int drawCount = turnCount;
					Vector2 startPos = new Vector2(Mathf.Clamp(headPosX[i] + headWidth / 2, 0, 512), 512 - headHeight);
					int index = 1;
					int lastPosX = (int)startPos.x;
					int lastPosY = (int)startPos.y;

					bool isOdd = false;
					while (drawCount > 0)
					{
						if (!isOdd)
						{
							int newPosY = 0;
							if (drawCount == 1)
							{
								newPosY = 0;
							}
							else
							{
								while (true)
								{
									newPosY = Random.Range(Mathf.Clamp(512 - index * totalHeight / yCount - randomRange, 0, lastPosY), 
										Mathf.Clamp(512 - index * totalHeight / yCount + randomRange, 0, lastPosY));
									bool isNew = true;
									foreach (var historyPos in historyPosY)
									{
										if (Mathf.Abs(historyPos - newPosY) < minGap)
										{
											isNew = false;
											break;
										}
									}

									if (!isNew) continue;
									if(Mathf.Abs(lastPosY - newPosY) >= minGap)
									{
										lastPosY = newPosY;
										historyPosY.Add(newPosY);
										break;
									}
								}	
							}
							
							var color = new Color[lineWidth * ((int)startPos.y - newPosY)];
							for (int j = 0; j < color.Length; ++j)
							{
								color[j] = Color.grey;
							}
							texture2D.SetPixels(lastPosX, newPosY, lineWidth, (int)startPos.y - newPosY, color);
							startPos = new Vector2((int)startPos.x, newPosY);
							index++;
						}
						else
						{
							int newPosX = 0;
							if (drawCount == 2 && (i == 0 || i == 1))
							{
								newPosX = headPosX[i];
							}
							else
							{
								while (true)
								{
									newPosX = Random.Range(Mathf.Clamp(0 + lineWidth, 0, 512), Mathf.Clamp(totalWidth - lineWidth, 0, 512));
									bool isNew = true;
									foreach (var historyPos in historyPosX)
									{
										if (Mathf.Abs(historyPos - newPosX) < minGap)
										{
											isNew = false;
											break;
										}
									}

									if (!isNew) continue;
									if (Mathf.Abs(lastPosX - newPosX) >= minGap)
									{
										historyPosX.Add(newPosX);
										break;
									}
								}	
							}

							lastPosX = newPosX;
							var color = new Color[lineWidth * (int)(Mathf.Abs(startPos.x - newPosX))];
							for (int j = 0; j < color.Length; ++j)
							{
								color[j] = Color.grey;
							}

							bool isLess = startPos.x - newPosX < 0;
							texture2D.SetPixels(isLess ? Mathf.Clamp((int)startPos.x + 2, 0, 512) : newPosX, (int)startPos.y, 
								(int)Mathf.Abs(startPos.x - newPosX), lineWidth, color);
							startPos = new Vector2(newPosX, (int)startPos.y);
						}

						drawCount--;
						isOdd = isOdd == true ? false : true;
					}
				}
			}
			
			texture2D.Apply();
			
			writeImage.SetTexture(texture2D);

			var pos = writeImage.GetRectTransform().anchoredPosition;
			var redPosX = headPosX[0] * 400 / 512;
			var bluePosX = headPosX[1] * 400 / 512;

			A.GetRectTransform().anchoredPosition = new Vector2(-200 + (res == 0 ? redPosX : bluePosX),
				A.GetRectTransform().anchoredPosition.y);
			B.GetRectTransform().anchoredPosition = new Vector2(-200 + (res == 0 ? bluePosX : redPosX),
				B.GetRectTransform().anchoredPosition.y);
			A.SetActive(true);
			B.SetActive(true);
		}

		public void OnDisable()
		{
			if (newPrice != null)
			{
				IAuctionManager.Instance.SetMiniGameResult(configId, newPrice);
				Messager.Instance.Broadcast(0, MessageId.SetChangePriceResult, configId, newPrice, false);
			}
			Explode.SetActive(false);
			Texture2D.DestroyImmediate(texture2D);
		}

		#endregion

		#region 事件绑定
		
		public void OnClickRes0()
		{
			ShowResult(res == 0);
		}
		
		public void OnClickRes1()
		{
			ShowResult(res == 1);
		}
		
		public void ShowResult(bool isSuccess)
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

			if (!isSuccess)
			{
				Explode.SetActive(true);
				AdBtn.SetActive(true);
				ShockManager.Instance.LongVibrate();
				SoundManager.Instance.PlaySound("Audio/Game/exploded.mp3");
			}

			LeftButton.SetActive(false);
			RightButton.SetActive(false);
			newPrice = price - basePrice;
			BigNumber.Round2Integer(newPrice);
			
			SetItemWinLossWithContainer(newPrice);
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
				var config = Config;
				var basePrice = GetBasePrice();
				var price = Random.Range(config.AdMin, config.AdMax + 1) / 100f * basePrice;
				newPrice = price - basePrice;
				BigNumber.Round2Integer(newPrice);
				SetItemWinLossWithContainer(newPrice);
				AdBtn.SetActive(false);
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
