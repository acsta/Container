using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace TaoTie
{
	public class UIItemsView : UIBaseView, IOnCreate, IOnEnable<List<long>>,IOnDisable
	{
		public static string PrefabPath => "UIGame/UIAuction/Prefabs/UIItemsView.prefab";
		public UICopyGameObject Space;

		private List<long> boxes;
		private UIAuctionItem[] items = new UIAuctionItem[GameConst.MaxBoxCount];
		public List<long> overAnim = new List<long>();
		private Vector3[] MoneyPos = new Vector3[GameConst.MaxBoxCount];
		private BigNumber[] Money = new BigNumber[GameConst.MaxBoxCount];
		#region override
		public void OnCreate()
		{
			this.Space = this.AddComponent<UICopyGameObject>("Items");
			this.Space.InitListView(0,GetItemsItemByIndex);
		}
		public void OnEnable(List<long> data)
		{
			boxes = data;
			overAnim.Clear();
			this.Space.SetListItemCount(GameConst.MaxBoxCount);
			//this.Space.RefreshAllShownItem();
			Messager.Instance.AddListener<int,BigNumber, bool>(0,MessageId.SetChangePriceResult,SetChangePriceResult);
			Messager.Instance.AddListener<int,int, bool>(0,MessageId.SetChangeItemResult,SetChangeItemResult);
		}

		public void OnDisable()
		{
			Messager.Instance.RemoveListener<int,BigNumber, bool>(0,MessageId.SetChangePriceResult,SetChangePriceResult);
			Messager.Instance.RemoveListener<int,int, bool>(0,MessageId.SetChangeItemResult,SetChangeItemResult);
		}
		#endregion

		#region 事件绑定
		public void GetItemsItemByIndex(int index, GameObject obj)
		{
			var uiItem = Space.GetUIItemView<UIAuctionItem>(obj);
			if (uiItem == null)
			{
				uiItem = Space.AddItemViewComponent<UIAuctionItem>(obj);
				items[index] = uiItem;
			}

			if (index >= boxes.Count)
			{
				uiItem.SetActive(false);
			}
			else
			{
				var box = EntityManager.Instance.Get<Box>(boxes[index]);
				ItemConfig cfg = box.ItemResultId == 0 ? box.ItemConfig : box.ItemResult;
				uiItem.SetData(cfg, !overAnim.Contains(boxes[index]), false, box.BoxType);
				Money[index] = box.GetFinalPrice(IAuctionManager.Instance.GetFinalGameInfoConfig());
			}
		}

		private void SetChangeItemResult(int old, int result, bool isAI = false)
		{
			for (int i = 0; i < items.Length; i++)
			{
				if (items[i].ConfigId == old)
				{
					items[i].SetChangeItemResult(result, isAI);
					return;
				}
			}
		}

		private void SetChangePriceResult(int itemId, BigNumber result, bool isAI)
		{
			for (int i = 0; i < items.Length; i++)
			{
				if (items[i].ConfigId == itemId)
				{
					items[i].SetChangePriceResult(result);
					return;
				}
			}
		}
		#endregion

		public void GetAnimParam(out Vector3[] pos, out BigNumber[] money)
		{
			for (int i = 0; i < overAnim.Count; i++)
			{
				if (items[i].BoxType == BoxType.Task) continue;
				MoneyPos[i] = items[i].Icon.GetRectTransform().position;
			}

			money = Money;
			pos = MoneyPos;
		}
		

		public async ETTask PlayAnim(long id, ETCancellationToken cancel)
		{
			await PlayAnimInner(id, cancel);
			var view = UIManager.Instance.GetView<UIButtonView>(1);
			view?.OnEnable(overAnim);
		}
		
		public void PlayWithoutAnim()
		{
			// int sp = -1;
			for (int i = 0; i < boxes.Count; i++)
			{
				var box = EntityManager.Instance.Get<Box>(boxes[i]);
				if (!overAnim.Contains(boxes[i]))
				{
					overAnim.Add(boxes[i]);
					ItemConfig cfg = box.ItemResultId == 0 ? box.ItemConfig : box.ItemResult;
					UIAuctionItem uiAuctionItem = items[i];
					uiAuctionItem.SetData(cfg, true, false, box.BoxType);
					uiAuctionItem.SetActive(true);
					uiAuctionItem.IntenItem();
				}
				box.Position = Vector3.one * 1000;
				// if (!IAuctionManager.Instance.IsAllPlayBox && box.ItemConfig.Type != (int) ItemType.None 
				//                                            && box.ItemResultId == 0 && box.Price == null)
				// {
				// 	if (sp == -1)
				// 	{
				// 		sp = i;
				// 	}
				// 	else
				// 	{
				// 		sp = -2;//多个箱子
				// 	}
				// }
			}
			// if (sp > 0 && IAuctionManager.Instance.LastAuctionPlayerId == IAuctionManager.Instance.Player?.Id)
			// {
			// 	PlayAim(sp).Coroutine();
			// }
		}

		// private async ETTask PlayAim(int index)
		// {
		// 	var win = await UIManager.Instance.OpenWindow<UITargetView>(UITargetView.PrefabPath);
		// 	var item = Space.GetItemByIndex(index);
		// 	await TimerManager.Instance.WaitAsync(1);
		// 	await win.EnterTarget(item.gameObject);
		// }

		private async ETTask PlayAnimInner(long id,ETCancellationToken cancel)
		{
			var index = boxes.IndexOf(id);
			if (index < 0) return;
			if (index >= boxes.Count) return;
			
			var box = EntityManager.Instance.Get<Box>(id);
			var ghc = box.GetComponent<GameObjectHolderComponent>();
			UIAuctionItem uiAuctionItem = items[index];
			if (ghc?.EntityView != null)
			{
				Vector3 startPos = ghc.EntityView.position;
				
				ItemConfig cfg = box.ItemResultId == 0 ? box.ItemConfig : box.ItemResult;
				uiAuctionItem.SetData(cfg, true, false, box.BoxType);
				uiAuctionItem.SetActive(true);
				uiAuctionItem.Icon.SetActive(false);
				if (cancel.IsCancel())
				{
					overAnim.Add(id);
					box.Position = Vector3.one * 1000;
					return;
				}
				var sprite = await ImageLoaderManager.Instance.LoadSpriteAsync(cfg.ItemPic);
				var image = GameObject.Instantiate(uiAuctionItem.Icon.GetGameObject()).transform as RectTransform;
				image.SetParent(uiAuctionItem.GetRectTransform(), false);
				image.gameObject.SetActive(true);
				image.localPosition = Vector3.zero;
				image.localScale = Vector3.one;
				image.GetComponent<Image>().sprite = sprite;
				var canvas = image.GetComponentInParent<Canvas>();
				bool isUI = canvas != null && canvas.renderMode != RenderMode.WorldSpace;
				if (isUI)
				{
					var anchorPos = UIManager.Instance.ScreenPointToUILocalPoint(uiAuctionItem.GetRectTransform(),
						CameraManager.Instance.MainCamera().WorldToScreenPoint(startPos));
					image.anchoredPosition = anchorPos;
					startPos = image.position;
				}
				image.position = startPos;
				box.Position = Vector3.one * 1000;
				var timeStart = TimerManager.Instance.GetTimeNow();
				while (true)
				{
					if (cancel.IsCancel())
					{
						break;
					}
					await TimerManager.Instance.WaitAsync(1,cancel);
					var timeNow = TimerManager.Instance.GetTimeNow();
					var progress = (timeNow - timeStart) / 500f;
					var endPos = uiAuctionItem.Icon.GetRectTransform().position;
					image.position = Vector3.Lerp(startPos, endPos, progress);
					image.localEulerAngles = Vector3.Lerp(Vector3.zero, new Vector3(0,0,360), progress);
					if (timeNow - timeStart > 500)
					{
						break;
					}
				}
				ImageLoaderManager.Instance.ReleaseImage(cfg.ItemPic);
				GameObject.Destroy(image.gameObject);
				uiAuctionItem.Icon.SetActive(true);
				image.localEulerAngles = Vector3.zero;
			}
			overAnim.Add(id);
			uiAuctionItem.IntenItem();
		}

		public async ETTask PlayOnClickSaleButtonAnim(bool isMe)
		{
			using ListComponent<ETTask> tasks = ListComponent<ETTask>.Create(); 
			if (isMe)
			{
				GameObject taskItem = null;
				int itemId = 0;
				for (int i = 0; i < items.Length; i++)
				{
					if (items[i] != null && items[i].BoxType == BoxType.Task)
					{
						var obj = GameObject.Instantiate(items[i].GetGameObject());
						var rt = obj.GetComponent<RectTransform>();
						var layer = UIManager.Instance.GetLayer(UILayerNames.TipLayer);
						rt.SetParent(items[i].GetTransform().parent, true);
						rt.localScale = items[i].GetRectTransform().localScale;
						rt.SetParent(layer.RectTransform, true);
						rt.position = items[i].GetRectTransform().position;
						rt.sizeDelta = new Vector2(200, 200);
						taskItem = obj;
						items[i].GetGameObject().GetComponent<CanvasGroup>().alpha = 0;
						itemId = items[i].ConfigId;
						break;
					}
				}
				if (taskItem != null)
				{
					using var taskIds = PlayerDataManager.Instance.GetRunningTaskIds();
					bool change = false;
					foreach (var taskId in taskIds)
					{
						var config = TaskConfigCategory.Instance.Get(taskId);
						if (config.ItemId == itemId)
						{
							var over = PlayerDataManager.Instance.GetTaskState(taskId, out var step);
							if (!over && step == config.ItemCount - 1)
							{
								change = true;
								break;
							}
						}
					}
					var gameView = UIManager.Instance.GetView<UIGameView>(1);
					if (gameView != null)
					{
						tasks.Add(gameView.DoTaskMoveAnim(taskItem, change));
					}
					else
					{
						GameObject.Destroy(taskItem);
					}
				}
			}
			for (int i = 0; i < items.Length; i++)
			{
				if (items[i] == null) break;
				if (!isMe || items[i].BoxType == BoxType.Normal)
				{
					tasks.Add(SingleItemAnim(items[i], 100f));
				}
			}
			PlayBoxAnim().Coroutine();
			await ETTaskHelper.WaitAll(tasks);
			await CloseSelf();
			ResetCanvasGroup();
		}

		private async ETTask PlayBoxAnim()
		{
			bool show = false;
			
			if (SceneManager.Instance.IsInTargetScene<MapScene>())
			{
				var map = SceneManager.Instance?.GetCurrentScene<MapScene>();
				if (map?.Collector != null)
				{
					var target = map.Collector.Get<GameObject>("BoxBoom");
					target?.SetActive(true);
					show = true;
				}
			}
			else
			{
				var map = SceneManager.Instance?.GetCurrentScene<GuideScene>();
				if (map?.Collector != null)
				{
					var target = map.Collector.Get<GameObject>("BoxBoom");
					target?.SetActive(true);
					show = true;
				}
			}

			if (show)
			{
				await TimerManager.Instance.WaitAsync(200);
				if (SceneManager.Instance.IsInTargetScene<MapScene>())
				{
					var map = SceneManager.Instance?.GetCurrentScene<MapScene>();
					if (map?.Collector != null)
					{
						var target = map.Collector.Get<GameObject>("woodbox");
						target?.SetActive(true);
					}
				}
				else
				{
					var map = SceneManager.Instance?.GetCurrentScene<GuideScene>();
					if (map?.Collector != null)
					{
						var target = map.Collector.Get<GameObject>("woodbox");
						target?.SetActive(true);
					}
				}
			}
		}

		private async ETTask SingleItemAnim(UIAuctionItem auctionItem, float animTime = 200f)
		{
			var canvasGroup = auctionItem.GetGameObject().GetComponent<CanvasGroup>();
			var startTime = TimerManager.Instance.GetTimeNow();
			while (true)
			{
				await TimerManager.Instance.WaitAsync(1);
				
				var timeNow = TimerManager.Instance.GetTimeNow();
				canvasGroup.alpha = Mathf.Lerp(1f, 0f, (timeNow - startTime) / animTime);
				if (timeNow - startTime >= animTime)
				{
					break;
				}
			}
		}

		public void ResetCanvasGroup()
		{
			int index = 0;
			while(true)
			{
				if (index >= items.Length) break;
				if (items[index] == null) break;
				
				var canvasGroup = items[index].GetGameObject().GetComponent<CanvasGroup>();
				canvasGroup.alpha = 1f;
				index++;
			}
		}
	}
}
