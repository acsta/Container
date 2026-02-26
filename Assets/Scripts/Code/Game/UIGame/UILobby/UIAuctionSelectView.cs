using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace TaoTie
{
	public class UIAuctionSelectView : UIBaseView, IOnCreate, IOnEnable,IOnWidthPaddingChange,IUpdate,IOnEnable<int>
	{
		public static string PrefabPath => "UIGame/UILobby/Prefabs/UIAuctionSelectView.prefab";
		public UIButton Start;
		public UIAnimator Hide;
		public UITextmesh Title;
		public UILoopListView2 ScrollView;
		public UIButton Back;
		public UITextmesh PriceText;
		public UIEmptyView Price;
		public UIAnimator UICommonView;
		public UIEmptyView Center;
		public UIAnimator Hand;
		private List<LevelConfig> levelConfigs = new List<LevelConfig>();
		#region override
		public void OnCreate()
		{
			Hand = AddComponent<UIAnimator>("UICommonView/Bg/Content/Hide/btnStart/btnStart/Hand");
			Center = AddComponent<UIEmptyView>("UICommonView/Bg/Content/ScrollView/Viewport/Center");
			UICommonView = AddComponent<UIAnimator>("UICommonView");
			this.ScrollView = this.AddComponent<UILoopListView2>("UICommonView/Bg/Content/ScrollView");
			this.ScrollView.InitListView(0,GetContentItemByIndex);
			ScrollView.SetOnSnapChange(OnSnapChange);
			this.Back = this.AddComponent<UIButton>("UICommonView/Bg/Close");
			Start = AddComponent<UIButton>("UICommonView/Bg/Content/Hide/btnStart/btnStart");
			Title = AddComponent<UITextmesh>("UICommonView/Bg/Content/Hide/Title/Text");
			Hide = AddComponent<UIAnimator>("UICommonView/Bg/Content/Hide");
			PriceText = AddComponent<UITextmesh>("UICommonView/Bg/Content/Hide/Price/Value");
			Price = AddComponent<UIEmptyView>("UICommonView/Bg/Content/Hide/Price");
			ScrollView.SetOnSnapOverAction((a, b) =>
			{
				Hide.SetActive(true);
			});
			ScrollView.SetOnBeginDragAction((a) =>
			{
				BeginDrag().Coroutine();
				Hand.SetActive(false);
			});
		}
		public void OnEnable()
		{
			SoundManager.Instance.PlaySound("Audio/Sound/Auction_Open.mp3");
			var lastLv = PlayerDataManager.Instance.LastLevelId;
			if (lastLv == 0)
			{
				lastLv = PlayerDataManager.Instance.GetMaxLevel();
			}
			OnEnable(lastLv);
			Hand.SetActive(false);
			OnEnableAsync().Coroutine();
		}

		private async ETTask OnEnableAsync()
		{
			Hide.SetActive(false);
			await TimerManager.Instance.WaitAsync(200);
			Hide.SetActive(true);
		}

		public void OnEnable(int id)
		{
			Hand.SetActive(true);
			this.Back.SetOnClick(OnClickBack);
			levelConfigs.Clear();
			var list = LevelConfigCategory.Instance.GetAllList();
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Hide != 1) levelConfigs.Add(list[i]);
			}
			ScrollView.SetListItemCount(levelConfigs.Count);
			ScrollView.RefreshAllShownItem();
			Start.SetOnClick(OnClickStart);
			int index = 0;
			for (int i = 0; i < levelConfigs.Count; i++)
			{
				if (levelConfigs[i].Id == id)
				{
					index = i;
					break;
				}
			}
			var lv = levelConfigs[index];
			ScrollView.MovePanelToItemIndex(index);
			SetCurData(lv);
		}
		private async ETTask BeginDrag()
		{
			if (Hide.GetGameObject().activeSelf)
			{
				await Hide.Play("AuctionSelect_Close");
				Hide.SetActive(false);
			}
		}
		public void Update()
		{
			var startPos = Center.GetRectTransform().position.x;
			for (int i = 0; i < levelConfigs.Count; i++)
			{
				var item = ScrollView.GetShownItemByItemIndex<AuctionSelectItem>(i);
				if(item!=null)
				{
					var offset = item.GetRectTransform().position.x - startPos;
					item.Icon.GetRectTransform().localScale = item.Lock.GetRectTransform().localScale =
						Vector3.one * Mathf.Clamp(1 - Mathf.Abs(offset) / 1000, 0.8f, 1f);
				}
			}
		
		}
		#endregion

		#region 事件绑定

		public void OnSnapChange(LoopListView2 view, LoopListViewItem2 item)
		{
			var index = ScrollView.GetSnapTargetItemIndex();
			if (index >= 0 && index < levelConfigs.Count)
			{
				var lv = levelConfigs[index];
				SetCurData(lv);
			}
		}
		public LoopListViewItem2 GetContentItemByIndex(LoopListView2 listView2,int index)
		{
			if (index < 0 || index >= levelConfigs.Count) return null;
			LoopListViewItem2 item = listView2.NewListViewItem("LevelItem", index);
			if (!item.IsInitHandlerCalled)
			{
				ScrollView.AddItemViewComponent<AuctionSelectItem>(item);
				item.IsInitHandlerCalled = true;
			}
			var taskGroup = this.ScrollView.GetUIItemView<AuctionSelectItem>(item);
			taskGroup.SetData(levelConfigs[index]);
			return item;
		}
		public void OnClickBack()
		{
			OnClickCloseAsync().Coroutine();
		}
		public async ETTask OnClickCloseAsync()
		{
			UIManager.Instance.OpenWindow<UILobbyView>(UILobbyView.PrefabPath).Coroutine();
			await UICommonView.Play("UIView_Close");
			CloseSelf().Coroutine();
		}
		public void OnClickStart()
		{
			OnClickBtnStartAsync().Coroutine();
		}
		#endregion

		public void SetCurData(LevelConfig lv)
		{
			PriceText.SetText(I18NManager.Instance.TranslateMoneyToStr(lv.Money));
			Title.SetText(I18NManager.Instance.I18NGetText(lv));
			Price.SetActive(lv.Money > 0);
		}

		private async ETTask OnClickBtnStartAsync()
		{
			var index = ScrollView.GetSnapTargetItemIndex();
			var config = levelConfigs[index];
			if (PlayerDataManager.Instance.GetMaxLevel() < config.Id)
			{
				UIManager.Instance.OpenBox<UIToast, I18NKey>(UIToast.PrefabPath, I18NKey.Tips_Auction_NotOpen).Coroutine();
				return;
			}
			if (config.Money > 0 && PlayerDataManager.Instance.TotalMoney < config.Money)
			{
				UIManager.Instance.OpenBox<UIToast, I18NKey>(UIToast.PrefabPath, I18NKey.Tips_Auction_Money_NotEnough).Coroutine();
				return;
			}
			await UIManager.Instance.OpenWindow<UIMatchView,int>(UIMatchView.PrefabPath, config.Id, UILayerNames.TipLayer);
			await SceneManager.Instance.SwitchMapScene(config.Name);
		}

	}
}
