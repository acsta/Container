using System.Collections;
using System.Collections.Generic;
using System;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace TaoTie
{
	public class UIMatchView : UIBaseView, IOnCreate, IOnEnable<int>, IOnDisable, IOnBeforeCloseWin
	{
		private const int MAX_COUNT = 9;
		[Timer(TimerType.UIMatchUpdate)]
		public class UIMatchViewTimer : ATimer<UIMatchView>
		{
			public override void Run(UIMatchView self)
			{
				try
				{
					self.Update();
				}
				catch (Exception e)
				{
					Log.Error($"move timer error: UIMatchView\n{e}");
				}
			}
		}

		private const float animTime = 200f;
		public static string PrefabPath => "UIGame/UILobby/Prefabs/UIMatchView.prefab";
		public UITextmesh Match;
		public UICopyGameObject Center;
		public UITextmesh Count;
		public UIButton btn_start;
		public UIEmptyView LoadingScreen;
		public UIImage MaskImage;
		public UITextmesh Lv;
		public UITextmesh Ready;

		private UserItem[] items = new UserItem[MAX_COUNT];
		private long startTime;
		private long timer;
		private int count;

		private bool isAnime;
		#region override

		public void OnCreate()
		{
			Ready = AddComponent<UITextmesh>("View/Ready");
			Lv =  AddComponent<UITextmesh>("View/Title/Text");
			MaskImage = AddComponent<UIImage>("Mask");
			LoadingScreen = AddComponent<UIEmptyView>("View");
			this.Match = this.AddComponent<UITextmesh>("View/Match");
			this.Center = this.AddComponent<UICopyGameObject>("View/Center");
			this.Center.InitListView(0, GetCenterItemByIndex);
			this.Count = this.AddComponent<UITextmesh>("View/Count");
			this.btn_start = this.AddComponent<UIButton>("View/StartBtn");
		}

		public void OnEnable(int levelId)
		{
			IAuctionManager.UserReady = false;
			Ready.SetActive(false);
			this.Match.SetActive(true);
			btn_start.SetActive(true);
			startTime = TimerManager.Instance.GetTimeNow();
			var config = LevelConfigCategory.Instance.Get(levelId);
			Lv.SetText(I18NManager.Instance.I18NGetText(config));
			var max = AuctionHelper.GetMaxCharacter();
			var len = Mathf.Min(max, config.AIIds.Length);
			count = Mathf.Min(len, MAX_COUNT);
			this.Center.SetListItemCount(count);
			this.Match.SetI18NKey(I18NKey.Text_Match_Time, 0);
			this.Count.SetText("0/" + count);
			this.btn_start.SetOnClick(OnClickbtn_start);
			TimerManager.Instance.Remove(ref timer);
			timer = TimerManager.Instance.NewRepeatedTimer(1000, TimerType.UIMatchUpdate, this);
		}

		public void OnDisable()
		{
			TimerManager.Instance.Remove(ref timer);
		}

		public void Update()
		{
			var timeNow = TimerManager.Instance.GetTimeNow();
			this.Match.SetI18NText((timeNow - startTime) / 1000);
		}
		public async ETTask OnBeforeDisable()
		{
			var starTime = TimerManager.Instance.GetTimeNow();
			while (true)
			{
				await TimerManager.Instance.WaitAsync(1);

				var timeNow = TimerManager.Instance.GetTimeNow();
				MaskImage.SetImageAlpha(Mathf.Lerp(0f, 1f, (timeNow - starTime) / animTime));
				if (timeNow - starTime >= animTime)
				{
					break;
				}
			}

			LoadingScreen.SetActive(false);

			starTime = TimerManager.Instance.GetTimeNow();
			while (true)
			{
				await TimerManager.Instance.WaitAsync(1);

				var timeNow = TimerManager.Instance.GetTimeNow();
				MaskImage.SetImageAlpha(Mathf.Lerp(1f, 0f, (timeNow - starTime) / animTime));
				if (timeNow - starTime >= animTime)
				{
					break;
				}
			}
		}


		#endregion

		#region 事件绑定

		public void GetCenterItemByIndex(int index, GameObject obj)
		{
			var item = Center.GetUIItemView<UserItem>(obj);
			if (item == null)
			{
				item = Center.AddItemViewComponent<UserItem>(obj);
			}
			items[index] = item;
			item.SetActive(false);
		}

		public void OnClickbtn_start()
		{
			Ready.SetActive(true);
			btn_start.SetActive(false);
			IAuctionManager.UserReady = true;
		}

		#endregion

		public void SetProgress(float progress)
		{
			if (progress >= 1)
			{
				TimerManager.Instance.Remove(ref timer);
				Match.SetActive(false);
			}
			int current = (int) (Mathf.Clamp01(progress) * count);
			this.Count.SetText(current + "/" + count);
			for (int i = 0; i < count; i++)
			{
				if (i < current)
				{
					if (!items[i].GetGameObject().activeSelf)
					{
						items[i].RefreshHead();
					}
					items[i].SetActive(true);
				}
				else
				{
					break;
				}
			}
		}
		public async ETTask LoadingAnim(bool isToBlack)
		{
			isAnime = true;
			LoadingScreen.SetActive(!isToBlack);
			MaskImage.SetImageAlpha(isToBlack ? 0f : 1f);

			var starTime = TimerManager.Instance.GetTimeNow();
			while (true)
			{
				await TimerManager.Instance.WaitAsync(1);

				var timeNow = TimerManager.Instance.GetTimeNow();
				MaskImage.SetImageAlpha(Mathf.Lerp(isToBlack ? 0f : 1f, isToBlack ? 1f : 0f,
					(timeNow - starTime) / animTime));
				if (timeNow - starTime >= animTime)
				{
					break;
				}
			}

			if (isToBlack) LoadingScreen.SetActive(true);
		}
	}
}
