using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace TaoTie
{
	public class UIButtonView : UIBaseView, IOnCreate, IOnEnable<List<long>,bool>,IOnEnable<List<long>>,IOnDestroy,
		IOnDisable,II18N
	{
		public static string PrefabPath => "UIGame/UIAuction/Prefabs/UIButtonView.prefab";
		public UIButton Button1;
		public UIButton Button3;
		public UITextmesh Total;
		public UIImage Title;
		public UITextmesh Result;
		public UIImage Text;
		public UIEmptyView MidRight;
		public UIAnimator Animator;

		private List<long> boxes;
		private bool isOver = false;
		#region override
		public void OnCreate()
		{
			MidRight = AddComponent<UIEmptyView>("MidRight");
			Text = AddComponent<UIImage>("MidLeft/Text");
			this.Button1 = this.AddComponent<UIButton>("Bottom/Button1");
			Button3= this.AddComponent<UIButton>("Bottom/Button3");
			Total = AddComponent<UITextmesh>("MidLeft/Total");
			Title = AddComponent<UIImage>("MidRight/Title");
			Result = AddComponent<UITextmesh>("MidRight/Result");
			Animator = AddComponent<UIAnimator>("Bottom");
			Messager.Instance.AddListener<int, int, bool>(0, MessageId.SetChangeItemResult, OnAppraisalResult);
			Messager.Instance.AddListener<int, BigNumber, bool>(0, MessageId.SetChangePriceResult, OnChangePriceResult);
		}
		public void OnDestroy()
		{
			Messager.Instance.RemoveListener<int, int, bool>(0, MessageId.SetChangeItemResult, OnAppraisalResult);
			Messager.Instance.RemoveListener<int, BigNumber, bool>(0, MessageId.SetChangePriceResult,
				OnChangePriceResult);
		}
		public void OnEnable(List<long> data)
		{
			Animator.Play("SaleButtonOpen").Coroutine();
			if (!isOver)
			{
				Button1.SetActive(false);
				Button3.SetActive(false);
			}
			if (IAuctionManager.Instance.LastAuctionPlayerId == -1)
			{
				Total.SetNum(0);
				MidRight.SetActive(false);
				return;
			}
			boxes = data;
			MidRight.SetActive(true);
			BigNumber total = 0;
			var gameInfoConfig = IAuctionManager.Instance.GetFinalGameInfoConfig();
			for (int i = 0; i < data.Count; i++)
			{
				var box = EntityManager.Instance.Get<Box>(data[i]);
				total += box.GetFinalPrice(gameInfoConfig);
			}

			if (data.Count == 0)
			{
				Total.SetNum(total);
			}
			else
			{
				Total.DoNum(total).Coroutine();
			}
			
			var lastAuctionPrice = IAuctionManager.Instance.LastAuctionPrice;
			var money = total - lastAuctionPrice;
			
			if (data.Count == 0)
			{
				if (lastAuctionPrice > total)
				{
					Title.SetSpritePath($"UIGame/UIAuction/Atlas/loss_{I18NManager.Instance.CurLangType}.png").Coroutine();
					Result.SetTextColor(GameConst.LOSS_COLOR);
				}
				else
				{
					Title.SetSpritePath($"UIGame/UIAuction/Atlas/win_{I18NManager.Instance.CurLangType}.png").Coroutine();
					Result.SetTextColor(GameConst.WIN_COLOR);
				}
				Result.SetNum(money, false);
			}
			else
			{
				Result.DoNum(money, false, (res) =>
				{
					if (!res)
					{
						Title.SetSpritePath($"UIGame/UIAuction/Atlas/loss_{I18NManager.Instance.CurLangType}.png").Coroutine();
						Result.SetTextColor(GameConst.LOSS_COLOR);
					}
					else
					{
						Title.SetSpritePath($"UIGame/UIAuction/Atlas/win_{I18NManager.Instance.CurLangType}.png").Coroutine();
						Result.SetTextColor(GameConst.WIN_COLOR);
					}
				}).Coroutine();
			}
		}
		public void OnEnable(List<long> data, bool isOver)
		{
			boxes = data;
			this.isOver = isOver;
			Animator.Play("SaleButtonOpen").Coroutine();
			
			this.Button1.SetOnClick(OnClickButtonSale);
			this.Button3.SetOnClick(OnClickButtonSale);
			
			if (boxes.Count == 0)
			{
				Total.SetNum(IAuctionManager.Instance.AllPrice);
			}
			else
			{
				Total.DoNum(IAuctionManager.Instance.AllPrice).Coroutine();
			}
			Button1.SetInteractable(true);
			Button3.SetInteractable(true);
			if (IAuctionManager.Instance.LastAuctionPlayerId == -1)
			{
				Button3.SetActive(true);
				Button1.SetActive(false);
				MidRight.SetActive(false);
				return;
			}
			Button1.SetActive(true);
			Button3.SetActive(false);
			MidRight.SetActive(true);
			
			var allPrice = IAuctionManager.Instance.AllPrice;
			var lastAuctionPrice = IAuctionManager.Instance.LastAuctionPrice;
			if (IAuctionManager.Instance.LastAuctionPlayerId == IAuctionManager.Instance.Player.Id)
			{
				Button3.SetActive(false);
			}
			else
			{
				Button3.SetActive(true);
				Button1.SetActive(false);
			}
			if (boxes.Count == 0)
			{
				if (lastAuctionPrice > allPrice)
				{
					Title.SetSpritePath($"UIGame/UIAuction/Atlas/loss_{I18NManager.Instance.CurLangType}.png").Coroutine();
					Result.SetTextColor(GameConst.LOSS_COLOR);

				}
				else
				{
					Title.SetSpritePath($"UIGame/UIAuction/Atlas/win_{I18NManager.Instance.CurLangType}.png").Coroutine();
					Result.SetTextColor(GameConst.WIN_COLOR);
				}
				Result.SetNum(allPrice - lastAuctionPrice, false);
			}
			else
			{
				Result.DoNum(allPrice - lastAuctionPrice, false, (res) =>
				{
					if (!res)
					{
						Title.SetSpritePath($"UIGame/UIAuction/Atlas/loss_{I18NManager.Instance.CurLangType}.png").Coroutine();
						Result.SetTextColor(GameConst.LOSS_COLOR);
					}
					else
					{
						Title.SetSpritePath($"UIGame/UIAuction/Atlas/win_{I18NManager.Instance.CurLangType}.png").Coroutine();
						Result.SetTextColor(GameConst.WIN_COLOR);
					}
				}).Coroutine();
			}
		}

		public void OnDisable()
		{
			isOver = false;
			Total.SetText(BigNumber.Zero);
			Result.SetText(BigNumber.Zero);
		}

		public void OnLanguageChange()
		{
			Text.SetSpritePath($"UIGame/UIAuction/Atlas/all_{I18NManager.Instance.CurLangType.ToString()}.png").Coroutine();
		}

		#endregion

		#region 事件绑定
		public void OnClickButtonSale()
		{
			OnClickButtonSaleAsync().Coroutine();
		}

		private async ETTask OnClickButtonSaleAsync()
		{
			Button1.SetInteractable(false);
			Button3.SetInteractable(false);
			bool isMe = IAuctionManager.Instance.LastAuctionPlayerId == IAuctionManager.Instance.Player.Id;
			var itemView = UIManager.Instance.GetView<UIItemsView>(1);
			if (itemView != null)
			{
				await itemView.PlayOnClickSaleButtonAnim(isMe);
			}
			if (isMe)
			{
				if(IAuctionManager.Instance.Blacks!=null && IAuctionManager.Instance.Blacks.Count>0 &&
				   GlobalConfigCategory.Instance.TryGetInt("BlackEventPercent",out var percent) && Random.Range(0,100) < percent)
				{
					var blackId = IAuctionManager.Instance.Blacks[Random.Range(0, IAuctionManager.Instance.Blacks.Count)];
					await UIManager.Instance.OpenWindow<UISaleEvent, BigNumber, long>(UISaleEvent.PrefabPath, IAuctionManager.Instance.AllPrice, blackId);
				}
				else
				{
					IAuctionManager.Instance.RunNextStage();
					PlayerDataManager.Instance.RecordWinToday(IAuctionManager.Instance.AllPrice - IAuctionManager.Instance.LastAuctionPrice);
					PlayerDataManager.Instance.ChangeMoney(IAuctionManager.Instance.AllPrice);
				}
			}
			await Animator.Play("SaleButtonClose");
			if (!isMe)
			{
				IAuctionManager.Instance.RunNextStage();
			}
			CloseSelf().Coroutine();
			UIManager.Instance.CloseWindow<UIItemsView>().Coroutine();
		}
		
		#endregion

		private void OnChangePriceResult(int id, BigNumber change, bool isAI)
		{
			RefreshPrice();
		}

		private void OnAppraisalResult(int old, int result, bool isAI)
		{
			RefreshPrice();
		}

		private void RefreshPrice()
		{
			BigNumber allPrice = 0;
			if (boxes == null) return;
			for (int i = 0; i < boxes.Count; i++)
			{
				var box = EntityManager.Instance.Get<Box>(boxes[i]);
				allPrice += box.GetFinalPrice(IAuctionManager.Instance.GetFinalGameInfoConfig());
			}
			Total.DoNum(allPrice).Coroutine();
			var lastAuctionPrice = IAuctionManager.Instance.LastAuctionPrice;
			if (lastAuctionPrice > allPrice)
			{
				Title.SetSpritePath($"UIGame/UIAuction/Atlas/loss_{I18NManager.Instance.CurLangType}.png").Coroutine();
				Result.SetTextColor(GameConst.LOSS_COLOR);
				Result.DoNum(lastAuctionPrice - allPrice).Coroutine();
			}
			else
			{
				Title.SetSpritePath($"UIGame/UIAuction/Atlas/win_{I18NManager.Instance.CurLangType}.png").Coroutine();
				Result.SetTextColor(GameConst.WIN_COLOR);
				Result.DoNum(allPrice - lastAuctionPrice).Coroutine();
			}
		}
	}
}
