using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TaoTie
{
	public class UIBlackView : UIBaseView, IOnCreate, IOnEnable, IOnDisable, II18N,IOnWidthPaddingChange,IOnEnable<int>
	{
		public static string PrefabPath => "UIGame/UILobby/Prefabs/UIBlackView.prefab";
		public UIMenu UIMenu;
		public UILoopListView2 View;
		//public UIEmptyView Arrow;
		public UIButton Back;
		public UITextmesh UnLock;
		private MenuPara CurMenu; 
		private List<MenuPara> menus= new List<MenuPara>();
		private List<TechnologyTreeConfig> list;
		public UIAnimator UICommonView;
		#region override
		public void OnCreate()
		{
			UICommonView = AddComponent<UIAnimator>("UICommonView");
			//Arrow = AddComponent<UIEmptyView>("UICommonView/Bg/Content/View/Arrow");
			this.UIMenu = this.AddComponent<UIMenu>("UICommonView/Bg/Content/UIMenu");
			this.View = this.AddComponent<UILoopListView2>("UICommonView/Bg/Content/View");
			this.View.InitListView(0,GetViewItemByIndex);
			View.SetOnBeginDragAction(OnClickDrag);
			this.UnLock = this.AddComponent<UITextmesh>("UICommonView/Bg/Content/UnLock/Text");
			Back = AddComponent<UIButton>("UICommonView/Bg/Close");
			UnLock.SetI18NKey(I18NKey.Text_Black_UnlockPercent);
		}

		public void OnEnable(int id)
		{
			SoundManager.Instance.PlaySound("Audio/Sound/Common_Open.mp3");
			Messager.Instance.AddListener<int>(0, MessageId.UnlockTreeNode, UnlockContainer);
			Back.SetOnClick(OnClickBack);
			OnLanguageChange();
			for (int i = 0; i < menus.Count; i++)
			{
				if (menus[i].Id == id)
				{
					UIMenu.SetActiveIndex(i, true);
					return;
				}
			}
			UIMenu.SetActiveIndex(0,true);
		}

		public void OnEnable()
		{
			SoundManager.Instance.PlaySound("Audio/Sound/Common_Open.mp3");
			Messager.Instance.AddListener<int>(0, MessageId.UnlockTreeNode, UnlockContainer);
			Back.SetOnClick(OnClickBack);
			OnLanguageChange();
			UIMenu.SetActiveIndex(0,true);
		}

		public void OnDisable()
		{
			Messager.Instance.RemoveListener<int>(0, MessageId.UnlockTreeNode, UnlockContainer);
		}

		public void OnClickDrag(PointerEventData data)
		{
			//Arrow.SetActive(false);
		}
		public void OnLanguageChange()
		{
			menus.Clear();
			if (!ColorUtility.TryParseHtmlString(GameConst.COMMON_TEXT_COLOR, out var color))
			{
				color = Color.black;
			}
			var list = TechnologyTreeConfigCategory.Instance.GetLevels();
			for (int i = 0; i < list.Count; i++)
			{
				var menu = new MenuPara()
				{
					Id = list[i].Id,
					Name = I18NManager.Instance.I18NGetText(list[i]),
					ActiveColor = color,
					UnActiveColor = color,
				};
				menus.Add(menu);
				menu.RedDot = "Black_"+list[i].Id;
			}

			UIMenu.SetData(menus, OnMenuItemChange, -1, CanMenuItemChange);
		}
		#endregion

		#region 事件绑定

		private bool CanMenuItemChange(MenuPara para)
		{
			var config = TechnologyTreeConfigCategory.Instance.Get(para.Id);
			if (config.UnlockType == 2)
			{
				var res = PlayerDataManager.Instance.GetPlayCount(para.Id) > 0;
				if (!res)
				{
					var lv = LevelConfigCategory.Instance.Get(para.Id);
					var toast = I18NManager.Instance.I18NGetParamText(I18NKey.Tips_Enter_Auction,
						I18NManager.Instance.I18NGetText(lv));
					UIManager.Instance.OpenBox<UIToast, string, int>(UIToast.PrefabPath, toast, 1500).Coroutine();
				}

				return res;
			}

			if (!PlayerDataManager.Instance.IsUnlock(config.Id))
			{
				UIManager.Instance.OpenWindow<UIUnlockWin, TechnologyTreeConfig>(UIUnlockWin.PrefabPath, config).Coroutine();
				return false;
			}
			return true;
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
		public void OnMenuItemChange(MenuPara para)
		{
			CurMenu = para;
			RefreshData(true);
			RedDotManager.Instance.RefreshRedDotViewCount("Black_Tags_" + para.Id, 0);
		}

		private void RefreshData(bool resetPos)
		{
			int unlock = 0, all = 0;
			list = TechnologyTreeConfigCategory.Instance.GetContainers(CurMenu.Id, false);
			for (int i = 0; i < list.Count; i++)
			{
				var plays = TechnologyTreeConfigCategory.Instance.GetPlayTypes(list[i].Id, false);
				if (PlayerDataManager.Instance.IsUnlock(list[i].Id))
				{
					unlock++;
					for (int j = 0; j < plays.Count; j++)
					{
						if (PlayerDataManager.Instance.IsUnlock(plays[j].Id))
						{
							unlock++;
						}
					}
				}
				all += plays.Count + 1;
			}

			UnLock.SetI18NText(all == 0 ? 0 : unlock * 100 / all);
			this.View.SetListItemCount(list.Count, resetPos);
			this.View.RefreshAllShownItem();
			if (resetPos)
			{
				PlayEnterAnim().Coroutine();
			}
			
			//Arrow.SetActive(list.Count > 3);
		}

		private async ETTask PlayEnterAnim()
		{
			for (int i = 0; i < list.Count; i++)
			{
				var node = View.GetShownItemByItemIndex<TechnologyNode>(i);
				if(node != null) node.SetActive(false);
			}
			await TimerManager.Instance.WaitAsync(200);
			for (int i = 0; i < list.Count; i++)
			{
				var node = View.GetShownItemByItemIndex<TechnologyNode>(i);
				if(node == null) return;
				node.SetActive(true);
				await TimerManager.Instance.WaitAsync(50);
			}
		}
		private void UnlockContainer(int id)
		{
			UnlockContainerAsync(id).Coroutine();
		}
		private async ETTask UnlockContainerAsync(int id)
		{
			list = TechnologyTreeConfigCategory.Instance.GetContainers(CurMenu.Id, false);
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Id == id)
				{
					var node = View.GetShownItemByItemIndex<TechnologyNode>(i);
					if (node != null)
					{
						await node.UnlockDoor();
					}
					break;
				}
			}
			RefreshData(false);
		}
		public LoopListViewItem2 GetViewItemByIndex(LoopListView2 listView, int index)
		{
			if (index < 0 || index >= list.Count) return null;
			var item = listView.NewListViewItem("TechnologyNode", index);

			TechnologyNode node;
			if (!item.IsInitHandlerCalled)
			{
				node = View.AddItemViewComponent<TechnologyNode>(item);
				item.IsInitHandlerCalled = true;
			}
			else
			{
				node = View.GetUIItemView<TechnologyNode>(item);
			}

			node.SetData(list[index]);
			return item;
		}
		#endregion
	}
}
