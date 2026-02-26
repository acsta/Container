using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace TaoTie
{
	public class UIRankView : UIBaseView, IOnCreate, IOnEnable<RankList>,IOnWidthPaddingChange
	{
		public static string PrefabPath => "UIGame/UILobby/Prefabs/UIRankView.prefab";
		public UIButton Close;
		public UILoopListView2 ScrollView;
		public RankItem RankItem;
		public UIAnimator UICommonView;

		private RankInfo[] list;
		#region override
		public void OnCreate()
		{
			UICommonView = AddComponent<UIAnimator>("UICommonView");
			this.Close = this.AddComponent<UIButton>("UICommonView/Bg/Close");
			this.ScrollView = this.AddComponent<UILoopListView2>("UICommonView/Bg/Content/ScrollView");
			this.ScrollView.InitListView(0,GetScrollViewItemByIndex);
			this.RankItem = this.AddComponent<RankItem>("UICommonView/Bg/Content/RankItem");
		}
		public void OnEnable(RankList data)
		{
			SoundManager.Instance.PlaySound("Audio/Sound/Common_Open.mp3");
			this.list = data?.list;
			this.Close.SetOnClick(OnClickClose);
			ScrollView.SetListItemCount(0);
			if (list != null)
			{
				if (data.my > 0 && data.my < list.Length && list[data.my - 1]?.uid == PlayerManager.Instance.Uid)
				{
					list[data.my - 1].Avatar = PlayerDataManager.Instance.Avatar;
					list[data.my - 1].NickName = PlayerDataManager.Instance.NickName;
					list[data.my - 1].RankValue = PlayerDataManager.Instance.TotalMoney.Value;
				}
			}
			RankItem.SetData((data?.my??101) - 1, true, null);
			OnEnableAsync().Coroutine();
		}
		
		private async ETTask OnEnableAsync()
		{
			ScrollView.GetScrollRect().vertical = false;
			await TimerManager.Instance.WaitAsync(200);
			ScrollView.SetListItemCount(list?.Length??0);
			ScrollView.RefreshAllShownItem();
			for (int i = 0; i < (list?.Length??0); i++)
			{
				var item = ScrollView.GetShownItemByItemIndex<RankItem>(i);
				if (item != null)
				{
					var pos = item.GetRectTransform().anchoredPosition;
					item.GetRectTransform().anchoredPosition = new Vector2(-1000,pos.y);
				}
			}

			var len = Mathf.Min(10, list?.Length ?? 0);
			if (len < 0) return;
			var timeStart = TimerManager.Instance.GetTimeNow();
			while (true)
			{
				await TimerManager.Instance.WaitAsync(1);
				var timeNow = TimerManager.Instance.GetTimeNow();
				var deltaTime = timeNow - timeStart;
				for (int i = 0; i < len; i++)
				{
					var item = ScrollView.GetShownItemByItemIndex<RankItem>(i);
					if (item != null)
					{
						var progress = Mathf.Clamp01((deltaTime - i * 100) / 200f);
						var pos = item.GetRectTransform().anchoredPosition;
						item.GetRectTransform().anchoredPosition = new Vector2(Mathf.Lerp(-1000,0,progress),pos.y);
					}
				}

				if (timeNow - timeStart > len * 100 + 200)
				{
					break;
				}
			}
			for (int i = 0; i < len; i++)
			{
				var item = ScrollView.GetShownItemByItemIndex<RankItem>(i);
				if (item != null)
				{
					var pos = item.GetRectTransform().anchoredPosition;
					item.GetRectTransform().anchoredPosition = new Vector2(0,pos.y);
				}
			}
			ScrollView.GetScrollRect().vertical = true;
		}
		#endregion

		#region 事件绑定
		public void OnClickClose()
		{
			OnClickCloseAsync().Coroutine();
		}

		public async ETTask OnClickCloseAsync()
		{
			UIManager.Instance.OpenWindow<UILobbyView>(UILobbyView.PrefabPath).Coroutine();
			await UICommonView.Play("UIView_Close");
			CloseSelf().Coroutine();
		}
		public LoopListViewItem2 GetScrollViewItemByIndex(LoopListView2 listView, int index)
		{
			if (index < 0) return null;
			LoopListViewItem2 item = listView.NewListViewItem("RankItem", index);
			if (!item.IsInitHandlerCalled)
			{
				ScrollView.AddItemViewComponent<RankItem>(item);
				item.IsInitHandlerCalled = true;
			}

			var rankItem = this.ScrollView.GetUIItemView<RankItem>(item);
			RankInfo data = null;
			if (list != null && index < list.Length)
			{
				data = list[index];
			}
			rankItem.SetData(index, false, data);
			var y = rankItem.GetRectTransform().sizeDelta.y;
			var x = ScrollView.GetRectTransform().rect.width;
			rankItem.GetRectTransform().sizeDelta = new Vector2(x, y);
			return item;
		}

		#endregion
	}
}
