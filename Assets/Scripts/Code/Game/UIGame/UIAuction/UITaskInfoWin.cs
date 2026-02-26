using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace TaoTie
{
	struct TaskData
	{
		public Vector2 position;
		public int index;
	}
	
	public class UITaskInfoWin : UIBaseView, IOnCreate, IOnEnable<bool,BigNumber>,IOnWidthPaddingChange,IOnDisable
	{
		public static string PrefabPath => "UIGame/UIAuction/Prefabs/UITaskInfoWin.prefab";
		public UIGameInfoView GameInfoView;
		public UIEmptyView TaskListView;
		public UIPointerClick Close;
		public UILoopListView2 ScrollView;
		public UICashGroup Top;
		public UIAnimator Animator;
		private List<TaskConfig> tasks = new List<TaskConfig>();
		public UIButton ChangeBtn;
		public UIEmptyView Title2;
		public UIEmptyView Title1;
		private bool isInfo;
		private int completeTask;
		public UITextmesh CompleteTaskText;

		private List<TaskData> positionList;
		private bool isClose = false;
		#region override
		public void OnCreate()
		{
			Title1 = AddComponent<UIEmptyView>("UIGameInfoView/Win/Title1");
			Title2 = AddComponent<UIEmptyView>("UICommonWin/Win/Title2");
			Animator = AddComponent<UIAnimator>();
			this.Close = this.AddComponent<UIPointerClick>("Mask");
			this.ScrollView = this.AddComponent<UILoopListView2>("UICommonWin/Win/Content/ScrollView");
			this.ScrollView.InitListView(0,GetScrollViewItemByIndex);
			this.Top = this.AddComponent<UICashGroup>("Top");
			GameInfoView = this.AddComponent<UIGameInfoView>("UIGameInfoView");
			TaskListView = this.AddComponent<UIEmptyView>("UICommonWin");
			ChangeBtn = AddComponent<UIButton>("Change");
			CompleteTaskText = AddComponent<UITextmesh>("UICommonWin/Win/Content/CompleteTaskText");
			CompleteTaskText.SetI18NKey(I18NKey.Text_Task_Today);
		}
		
		public void OnEnable(bool isInfo,BigNumber num)
		{
			isClose = false;
			positionList = new List<TaskData>();
			SoundManager.Instance.PlaySound("Audio/Sound/Win_Open.mp3");
			completeTask = 0;
			Top.SetShowNum(num);
			this.isInfo = isInfo;
			GameInfoView.SetActive(isInfo);
			TaskListView.SetActive(!isInfo);
			this.Close.SetOnClick(OnClickClose);
			tasks.Clear();
			using var list = PlayerDataManager.Instance.GetRunningTaskIds();
			for (int i = 0; i < list.Count; i++)
			{
				if (!PlayerDataManager.Instance.GetTaskState(list[i],out _))
				{
					tasks.Add(TaskConfigCategory.Instance.Get(list[i]));
				}
				else
				{
					completeTask++;
				}
			}
			tasks.Sort(TaskCompare);
			ScrollView.SetListItemCount(tasks.Count);
			ScrollView.RefreshAllShownItem();
			if (IAuctionManager.Instance.GameInfoId > 0)
			{
				var config = IAuctionManager.Instance.GameInfoConfig;
				GameInfoView.GameInfoItem[0].SetActive(config.Rare > 3);
				GameInfoView.GameInfoItem[1].SetActive(config.Rare == 3);
				GameInfoView.GameInfoItem[2].SetActive(config.Rare < 3);
				for (int i = 0; i < GameInfoView.GameInfoItem.Length; i++)
				{
					GameInfoView.GameInfoItem[i].SetData(config, null);
				}
			}
			else
			{
				for (int i = 0; i < GameInfoView.GameInfoItem.Length; i++)
				{
					GameInfoView.GameInfoItem[i].SetActive(false);
				}
			}
			Title2.SetActive(IAuctionManager.Instance.GameInfoId > 0);
			Title1.SetActive(list.Count > 0);
			if (IAuctionManager.Instance.GameInfoId > 0 && list.Count > 0)
			{
				ChangeBtn.SetOnClick(OnChange);
			}
			else
			{
				ChangeBtn.RemoveOnClick();
			}

			SetCompleteText(completeTask);

			Messager.Instance.AddListener(0, MessageId.UpdateTaskStep, UpdateTaskStep);
			//Messager.Instance.AddListener(0, MessageId.ComplexTask, UpdateTaskStep);
		}

		public void OnDisable()
		{
			//Messager.Instance.RemoveListener(0, MessageId.ComplexTask, UpdateTaskStep);
			Messager.Instance.RemoveListener(0, MessageId.UpdateTaskStep, UpdateTaskStep);
			var report = UIManager.Instance.GetView<UIReportWin>(1);
			if (report == null && IAuctionManager.Instance != null)
			{
				GameTimerManager.Instance.SetTimeScale(1);
			}
			var infoWin = UIManager.Instance.GetView<UIGameView>(1);
			if (infoWin != null)
			{
				infoWin.OnSecondWinOver();
			}
		}
		#endregion

		#region 事件绑定

		private void OnChange()
		{
			this.isInfo = !isInfo;
			GameInfoView.SetActive(isInfo);
			TaskListView.SetActive(!isInfo);
		}
		public void OnClickClose()
		{
			if(isClose) return;
			isClose = true;
			OnClickCloseAsync().Coroutine();
		}
		
		public async ETTask OnClickCloseAsync()
		{
			SoundManager.Instance.PlaySound("Audio/Sound/Win_Close.mp3");
			await Animator.Play("TaskInfoWin_Close");
			CloseSelf().Coroutine();
		}
		public LoopListViewItem2 GetScrollViewItemByIndex(LoopListView2 listView, int index)
		{
			if (index < 0 || index >= tasks.Count) return null;
			var item = listView.NewListViewItem("TaskListItem", index);

			TaskListItem reportItem;
			if (!item.IsInitHandlerCalled)
			{
				reportItem = ScrollView.AddItemViewComponent<TaskListItem>(item);
				item.IsInitHandlerCalled = true;
			}
			else
			{
				reportItem = ScrollView.GetUIItemView<TaskListItem>(item);
			}

			reportItem.SetData(tasks[index]);
			return item;
		}
		#endregion
		
		private int TaskCompare(TaskConfig task1, TaskConfig task2)
		{
			bool isOpen1, isOpen2;
			if (task1.ItemType == 0)
			{
				var item1 = ItemConfigCategory.Instance.Get(task1.ItemId);
				var container1 = ContainerConfigCategory.Instance.Get(item1.ContainerId);
				isOpen1 = container1.Level == IAuctionManager.Instance.Level || item1.Type == (int) ItemType.Const;
			}
			else
			{
				var container1 = ContainerConfigCategory.Instance.Get(task1.ItemId);
				isOpen1 = container1.Level == IAuctionManager.Instance.Level;
			}
			if (task2.ItemType == 0)
			{
				var item2 = ItemConfigCategory.Instance.Get(task2.ItemId);
				var container2 = ContainerConfigCategory.Instance.Get(item2.ContainerId);
				isOpen2 = container2.Level == IAuctionManager.Instance.Level || item2.Type == (int) ItemType.Const;
			}
			else
			{
				var container2 = ContainerConfigCategory.Instance.Get(task2.ItemId);
				isOpen2 = container2.Level == IAuctionManager.Instance.Level;
			}
			
			bool over1 = PlayerDataManager.Instance.GetTaskState(task1.Id, out int step1);
			bool over2 = PlayerDataManager.Instance.GetTaskState(task2.Id, out int step2);
			if (over1 != over2)
			{
				return over1 ? 1 : -1;
			}
			
			if (isOpen1 != isOpen2) return isOpen1 ? -1 : 1;
			
			if (step1 == task1.ItemCount != (step2 == task2.ItemCount))
			{
				return step1 == task1.ItemCount ? -1 : 1;
			}

			if (step1 > 0 != step2 > 0)
			{
				return step1 > 0 ? -1 : 1;
			}

			if (task1.ItemCount - step1 != task2.ItemCount - step2)
			{
				return task1.ItemCount - step1 < task2.ItemCount - step2 ? -1 : 1;
			}
			
			if(task1.ItemCount != task2.ItemCount)
			{
				return task1.ItemCount > task2.ItemCount ? -1 : 1;
			}
			
			return task2.Rare - task1.Rare;
		}

		private void RefreshTask()
		{
			tasks.Clear();
			using var list = PlayerDataManager.Instance.GetRunningTaskIds();
			for (int i = 0; i < list.Count; i++)
			{
				if (!PlayerDataManager.Instance.GetTaskState(list[i],out _))
				{
					tasks.Add(TaskConfigCategory.Instance.Get(list[i]));
				}
			}
			tasks.Sort(TaskCompare);
		}
		public void UpdateTaskStep()
		{
			RefreshTask();
			ScrollView.SetListItemCount(tasks.Count,false);
			ScrollView.RefreshAllShownItem();
		}

		public void SetCompleteText(int completedTask)
		{
			CompleteTaskText.SetI18NText(completedTask);
		}

		public void UpdateTaskList()
		{
			RefreshTask();
			if (positionList.Count > 1)
			{
				for (int i = 0; i < positionList.Count; i++)
				{
					ScrollView.GetShownItemByItemIndex<TaskListItem>(positionList[i].index).GetRectTransform().anchoredPosition = positionList[i].position;
				}

				int startIndex = -1;
				for (int i = 0; i < tasks.Count; i++)
				{
					var item = ScrollView.GetShownItemByItemIndex<TaskListItem>(i);
					if (startIndex == -1 && i == positionList[0].index)
					{
						startIndex = 0;
					}
					
					if (item != null)
					{
						startIndex++;
						item.SetData(tasks[i]);
					}

				}	
			}

			var oldPos = ScrollView.loopListView.ContainerTrans.anchoredPosition;
			ScrollView.SetListItemCount(tasks.Count,false);
			ScrollView.RefreshAllShownItem();
			ScrollView.loopListView.ContainerTrans.anchoredPosition = oldPos;
		}
		
		public async ETTask DoAnim(TaskListItem overItem,int overTaskCount)
		{
			positionList = new List<TaskData>();
			
			bool find = false;
			for (int i = 0; i < tasks.Count; i++)
			{
				var item = ScrollView.GetShownItemByItemIndex<TaskListItem>(i);
				if (item == overItem)
				{
					positionList.Add(new TaskData()
					{
						position = item.GetRectTransform().anchoredPosition,
						index = i
					});
					find = true;
					continue;
				}
				if(!find || item==null) continue;

				if (find)
				{
					positionList.Add(new TaskData()
					{
						position = item.GetRectTransform().anchoredPosition,
						index = i
					});
				}
			}

			var animTime = 200f;
			if (positionList.Count > 1)
			{
				var startTime = TimerManager.Instance.GetTimeNow();
				
				while (true)
				{
					await TimerManager.Instance.WaitAsync(1);
					
					var timeNow = TimerManager.Instance.GetTimeNow();
					var progress = Mathf.Clamp01((timeNow - startTime) / animTime);
					for (int i = 1; i < positionList.Count; i++)
					{
						var lastItemPos = positionList[i - 1].position;
						var curritemPos = positionList[i].position;

						ScrollView.GetShownItemByItemIndex<TaskListItem>(positionList[i].index).GetRectTransform().anchoredPosition = Vector2.Lerp(curritemPos, lastItemPos, progress);
					}
					
					if ((timeNow - startTime) >= animTime)
					{
						break;
					}
				}
			}
			UpdateTaskList();
			SetCompleteText(overTaskCount);
		}
	}
}
