using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace TaoTie
{
	public class UIMarketView : UIBaseView, IOnCreate, IOnEnable, IOnDisable,IOnWidthPaddingChange, IUpdate
	{
		[Timer(TimerType.UIMarketView)]
		public class UIMarketViewUpdateTimer : ATimer<UIMarketView>
		{
			public override void Run(UIMarketView self)
			{
				try
				{
					self.UpdateTimeDown();
				}
				catch (Exception e)
				{
					Log.Error($"move timer error: UIMarketView\n{e}");
				}
			}
		}
		public static string PrefabPath => "UIGame/UILobby/Prefabs/UIMarketView.prefab";
		public UITextmesh RefreshText;
		public UITextmesh TimeDown;
		public UIButton RefreshButton;
		public UICopyGameObject Middle;
		public UISlider Progress;
		public UICopyGameObject Rewards;
		public UITextmesh Text;
		public UIButton Close;
		public UIAnimator UICommonView;
		public UIImage Max;
		public UIImage ForbidClickImage;
		
		private Queue<TaskItemData> _QueueTaskItemDatas;
		private bool _IsProgressAnim;

		private long timerId;

		private HashSet<int> lockTasks = new HashSet<int>();
		private int overTaskCount = 0;

		private List<TaskConfig> datas = new List<TaskConfig>();
	
		#region override
		public void OnCreate()
		{
			UICommonView = AddComponent<UIAnimator>("UICommonView");
			Close = AddComponent<UIButton>("UICommonView/Bg/Close");
			this.RefreshText = this.AddComponent<UITextmesh>("UICommonView/Bg/Content/Top/RefreshText");
			this.TimeDown = this.AddComponent<UITextmesh>("UICommonView/Bg/Content/Top/TimeDown");
			this.RefreshButton = this.AddComponent<UIButton>("UICommonView/Bg/Content/Top/RefreshButton");
			this.Middle = this.AddComponent<UICopyGameObject>("UICommonView/Bg/Content/Middle/Viewport/Content");
			this.Middle.InitListView(0, OnGetItemByIndex);
			this.Progress = this.AddComponent<UISlider>("UICommonView/Bg/Content/Bottom/Progress");
			this.Rewards = this.AddComponent<UICopyGameObject>("UICommonView/Bg/Content/Bottom/Progress/Rewards");
			this.Rewards.InitListView(0, GetRewardsItemByIndex);
			this.Text = this.AddComponent<UITextmesh>("UICommonView/Bg/Content/Bottom/Over/Text");
			this.Max = this.AddComponent<UIImage>("UICommonView/Bg/Content/Bottom/Progress/Max");
			this.ForbidClickImage = this.AddComponent<UIImage>("ForbidClick");
			RefreshText.SetI18NKey(I18NKey.Text_Market_RereshTime);
			Text.SetI18NKey(I18NKey.Text_Market_OverCount);
		}
		public void OnEnable()
		{
			SoundManager.Instance.PlaySound("Audio/Sound/Common_Open.mp3");
			RefreshButton.SetInteractable(true);
			RefreshButton.SetActive(AdManager.Instance.PlatformHasAD());
			ForbidClickImage.SetActive(false);
			_QueueTaskItemDatas = new Queue<TaskItemData>();
			_IsProgressAnim = false;
			lockTasks.Clear();
			this.RefreshButton.SetOnClick(OnClickRefreshButton);
			Close.SetOnClick(OnClickBack);
			RefreshText.SetI18NText(PlayerDataManager.Instance.DailyRefreshHour.ToString("00")+":00");
			Messager.Instance.AddListener(0,MessageId.UpdateTaskStep,RefreshTask);
			RefreshView();
			Middle.SetListItemCount(0);
			UpdateTimeDown();
			TimerManager.Instance.Remove(ref timerId);
			timerId = TimerManager.Instance.NewRepeatedTimer(1000, TimerType.UIMarketView, this);
			PlayOpenAnim().Coroutine();
		}

		public void Update()
		{
			while (!_IsProgressAnim && _QueueTaskItemDatas.Count > 0)
			{
				_IsProgressAnim = true;
				var currItemData = _QueueTaskItemDatas.Peek();
				ProgressAnim(currItemData.currProgress, 
					DailyTaskRewardsConfigCategory.Instance.GetRewards(PlayerDataManager.Instance.RestaurantLv).Count, 
					currItemData.showIndex).Coroutine();
			}
		}

		private async ETTask PlayOpenAnim()
		{
			await TimerManager.Instance.WaitAsync(300);
			await PlayItemAnim();
		}

		private async ETTask PlayItemAnim()
		{
			Middle.SetListItemCount(datas.Count);
			Middle.RefreshAllShownItem();
			for (int i = 0; i < datas.Count; i++)
			{
				var item = Middle.GetUIItemView<DailyTaskItem>(Middle.GetItemByIndex(i));
				item.SetActive(false);
			}
			for (int i = 0; i < datas.Count; i++)
			{
				var item = Middle.GetUIItemView<DailyTaskItem>(Middle.GetItemByIndex(i));
				item.SetActive(true);
				await TimerManager.Instance.WaitAsync(50);
			}
		}
		

		public void OnDisable()
		{
			Messager.Instance.RemoveListener(0,MessageId.UpdateTaskStep,RefreshTask);
			TimerManager.Instance.Remove(ref timerId);
			_QueueTaskItemDatas.Clear();
		}

		public void UpdateTimeDown()
		{
			var time = PlayerDataManager.Instance.LastRefreshDailyTime + TimeInfo.OneDay;
			var deltaTime = time - TimerManager.Instance.GetTimeNow();
			var temp = TimeInfo.Instance.ToDateTime(TimeInfo.Instance.Transition(DateTime.Today) + deltaTime);
			if (temp.Hour > 0)
			{
				TimeDown.SetText(temp.ToString("HH:mm"));
			}
			else
			{
				TimeDown.SetText(temp.ToString("mm:ss"));
			}
		}
		#endregion

		#region 事件绑定

		public void OnClickRefreshButton()
		{
			var conf = RestaurantConfigCategory.Instance.GetByLv(PlayerDataManager.Instance.RestaurantLv, out _);
			if (overTaskCount >= conf.ShowMax)
			{
				UIManager.Instance.OpenBox<UIToast,I18NKey>(UIToast.PrefabPath, I18NKey.Text_Task_Complete_Today).Coroutine();
				return;
			}

			bool has = false;
			var tasks = PlayerDataManager.Instance.GetDailyTaskIds();
			for (int i = 0; i < tasks.Count; i++)
			{
				var taskCfg = TaskConfigCategory.Instance.Get(tasks[i]);
				if (!PlayerDataManager.Instance.GetTaskState(tasks[i], out var step) && step >= taskCfg.ItemCount)
				{
					has = true;
					break;
				}
			}
			if (has)
			{
				UIManager.Instance.OpenBox<UIMsgBoxWin,MsgBoxPara>(UIMsgBoxWin.PrefabPath, new MsgBoxPara()
				{
					Content =  I18NManager.Instance.I18NGetText(I18NKey.Text_Market_Reresh_Notice),
					CancelText = I18NManager.Instance.I18NGetText(I18NKey.Global_Btn_Cancel),
					ConfirmText = I18NManager.Instance.I18NGetText(I18NKey.Global_Btn_Confirm),
					CancelCallback = (win) =>
					{
						UIManager.Instance.CloseBox(win).Coroutine();
					},
					ConfirmCallback = (win) =>
					{
						if (AdManager.Instance.PlatformHasAD())
						{
							RefreshButton.SetInteractable(false);
							OnClickRefreshButtonAsync().Coroutine();
						}
						UIManager.Instance.CloseBox(win).Coroutine();
					}
				}).Coroutine();
			}
			else
			{
				if (AdManager.Instance.PlatformHasAD())
				{
					RefreshButton.SetInteractable(false);
					OnClickRefreshButtonAsync().Coroutine();
				}
			}
		}
		public async ETTask OnClickRefreshButtonAsync()
		{
			try
			{
				var res = await AdManager.Instance.PlayAd();
				if (res)
				{
					PlayerDataManager.Instance.RefreshDailyTask(false, lockTasks);
					RefreshView();
					PlayItemAnim().Coroutine();
					SoundManager.Instance.PlaySound("Audio/Sound/Common_Refresh.mp3");
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex);
			}
			finally
			{
				RefreshButton.SetInteractable(true);
			}
		}
		public void GetRewardsItemByIndex(int index, GameObject obj)
		{
			var item = Rewards.GetUIItemView<DailyTaskRewards>(obj);
			if (item == null)
			{
				item = Rewards.AddItemViewComponent<DailyTaskRewards>(obj);
			}

			item.SetData(index,
				DailyTaskRewardsConfigCategory.Instance.GetRewards(PlayerDataManager.Instance.RestaurantLv)[index],
				overTaskCount);
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

		public void OnGetItemByIndex(int index, GameObject obj)
		{
			var item = Middle.GetUIItemView<DailyTaskItem>(obj);
			if (item == null)
			{
				item = Middle.AddItemViewComponent<DailyTaskItem>(obj);
			}
			item.SetData(datas[index], lockTasks);
		}
		#endregion

		public void OnClickTask()
		{
			var tasks = PlayerDataManager.Instance.GetDailyTaskIds();
		    overTaskCount = 0;
		    for (int i = 0; i < tasks.Count; i++)
		    { if (PlayerDataManager.Instance.GetTaskState(tasks[i], out _))
		       {
		          overTaskCount++;
		       }
		    }
		    Text.SetI18NText(overTaskCount);
		    
		    var list = DailyTaskRewardsConfigCategory.Instance.GetRewards(PlayerDataManager.Instance.RestaurantLv);
		    float progress = 0;
		    int showIndex = -1;
		    int pass = 0;
		    for (int i = 0; i < list.Count; i++)
		    {
		       if (overTaskCount >= list[i].TaskCount)
		       {
		          if (overTaskCount == list[i].TaskCount)
		          {
		             showIndex = i;
		          }
		          progress += 1f;
		          pass = list[i].TaskCount;
		       }
		       else
		       {
		          progress += (float) (overTaskCount - pass) / (list[i].TaskCount - pass);
		          break;
		       }
		    }
		    
		    //ProgressAnim(progress, list.Count, showIndex).Coroutine();
		    TaskItemData item = new TaskItemData();
		    item.showIndex = showIndex;
		    item.currProgress = progress;
		    _QueueTaskItemDatas.Enqueue(item);
		    
		}

		public void RefreshTask()
		{
			Middle.SetListItemCount(datas.Count);
			Middle.RefreshAllShownItem();
		}
		private void RefreshView()
		{
		    datas.Clear();
		    var tasks = PlayerDataManager.Instance.GetDailyTaskIds();
		    overTaskCount = 0;
		    for (int i = 0; i < tasks.Count; i++)
		    {
		       var taskCfg = TaskConfigCategory.Instance.Get(tasks[i]);
		       datas.Add( taskCfg);
		       if (PlayerDataManager.Instance.GetTaskState(tasks[i], out _))
		       {
		          overTaskCount++;
		       }
		    }
		    datas.Sort(TaskCompare);
		    Text.SetI18NText(overTaskCount);
		    
		    var list = DailyTaskRewardsConfigCategory.Instance.GetRewards(PlayerDataManager.Instance.RestaurantLv);
		    Rewards.SetListItemCount(list.Count);
		    Rewards.RefreshAllShownItem();
		    float progress = 0;
		    int pass = 0;
		    for (int i = 0; i < list.Count; i++)
		    {
		       if (overTaskCount >= list[i].TaskCount)
		       {
			       progress += 1f;
		          pass = list[i].TaskCount;
		       }
		       else
		       {
		          progress += (float) (overTaskCount - pass) / (list[i].TaskCount - pass);
		          break;
		       }
		    }
		    Max.SetActive(progress >= list.Count);
		    Progress.SetNormalizedValue(progress / list.Count);
		    
		}

		private async ETTask ProgressAnim(float completedTaskCount, int listCount, int showIndex)
		{
		    var newProgress = completedTaskCount / listCount;
		    var oldProgress = Progress.GetNormalizedValue();
		    var startTime = TimerManager.Instance.GetTimeNow();
		    var animTime = 300f;
		    while (true)
		    {
		       await TimerManager.Instance.WaitAsync(1);
		       
		       var timeNow = TimerManager.Instance.GetTimeNow();
		       Progress.SetNormalizedValue(Mathf.Lerp(oldProgress, newProgress, Mathf.Clamp01((timeNow - startTime) / animTime)));
		       if (timeNow - startTime >= animTime)
		       {
		          break;
		       }
		    }
		    
		    if (showIndex >= 0)
		    { 
			    SoundManager.Instance.PlaySound("Audio/Sound/Restaurant_ProgressOn.mp3");
			    
		       var taskFXGO = await GameObjectPoolManager.GetInstance().GetGameObjectAsync(GameConst.TaskPrefab);
		       taskFXGO.SetActive(false);
		       
		       var item = Rewards.GetUIItemView<DailyTaskRewards>(Rewards.GetItemByIndex(showIndex)).Count;
		       taskFXGO.transform.position = item.GetRectTransform().position;
		       
		       taskFXGO.SetActive(true);
		        
		       
		       await TimerManager.Instance.WaitAsync(500);
		       GameObjectPoolManager.GetInstance().RecycleGameObject(taskFXGO);
		    }
		    
		    Rewards.SetListItemCount(listCount);
		    Rewards.RefreshAllShownItem();
		    Max.SetActive(completedTaskCount >= listCount);
		    _IsProgressAnim = false;
		    _QueueTaskItemDatas.Dequeue();
		}

		private int TaskCompare(TaskConfig task1, TaskConfig task2)
		{
			bool isOpen1 = lockTasks.Contains(task1.Id);
			bool isOpen2 = lockTasks.Contains(task2.Id);
			if (isOpen1 != isOpen2) return isOpen1 ? -1 : 1;
			bool over1 = PlayerDataManager.Instance.GetTaskState(task1.Id, out int step1);
			bool over2 = PlayerDataManager.Instance.GetTaskState(task2.Id, out int step2);
			if (over1 != over2)
			{
				return over1 ? 1 : -1;
			}

			if (step1 == task1.ItemCount != (step2 == task2.ItemCount))
			{
				return step1 == task1.ItemCount ? -1 : 1;
			}
			
			var item = ItemConfigCategory.Instance.Get(task1.ItemId);
			var container1 = ContainerConfigCategory.Instance.Get(item.ContainerId);
			item = ItemConfigCategory.Instance.Get(task2.ItemId);
			var container2 = ContainerConfigCategory.Instance.Get(item.ContainerId);
			if (container1.Level != container2.Level)
			{
				return container2.Level - container1.Level;
			}
			return task2.Rare - task1.Rare;
		}
		
		public void OnTaskComplete()
		{
			OnTaskCompleteAsync().Coroutine();
			OnClickTask();
		}
		private async ETTask OnTaskCompleteAsync()
		{
			var taskFXGO = await GameObjectPoolManager.GetInstance().GetGameObjectAsync("UIGame/UILobby/Prefabs/Task_finish.prefab");
			taskFXGO.SetActive(false);

			taskFXGO.transform.position = this.Text.GetRectTransform().position;
    
			taskFXGO.SetActive(true);
			await TimerManager.Instance.WaitAsync(1000);
			GameObjectPoolManager.GetInstance().RecycleGameObject(taskFXGO);
		}

		public void ForbidClick(bool isActive)
		{
			ForbidClickImage.SetActive(isActive);
		}
	}
}
