using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TaoTie
{
	public class UILobbyView : UIBaseView, IOnCreate,IOnEnable,IOnEnable<bool>,IUpdate,IOnWidthPaddingChange
	{
		public static string PrefabPath => "UIGame/UILobby/Prefabs/UILobbyView.prefab";
		public UIButton btnStart;
		public UIAnimator Animator;
		public UIButton btnCollection;
		public UIButton btnBar;
		public UIButton btnCloth;
		public UIButton btnBlack;
		public UIButton btnMarket;
		public UIButton btnRestaurant;
		public UIButton btnStock;
		public UIButton btnSetting;
		public UIRankBtn btnRank;

		#region SDK

		public UIButton btnSidebar;
		public UIButton btnGameGroup;
		public UIButton btnShare;
		public UIButton btnRecommend;
		public UIButton btnCollect;
		public UIButton btnFollow;
		public UIButton btnDesktop;
		#endregion
		
		public UIButton btnDailyTask;
		private UIEventTrigger eventTrigger;
		private ScrollRect ScrollRect;
		
		public UIPointerClick Mask;
		public UIEmptyView Bottom;
		public UIEmptyView Mid;

		private bool isDraging;
		private long lastDragTime;
		private float len;
		private float panding;
		private bool moveTurn;
		private ReferenceCollector collector;
		private Vector2 startPos;
		#region override
		public void OnCreate()
		{
			Animator = AddComponent<UIAnimator>();
			Mid = AddComponent<UIEmptyView>("Mid");
			Bottom = AddComponent<UIEmptyView>("Bottom");
			Mask = AddComponent<UIPointerClick>("Mask");
			this.btnStart = this.AddComponent<UIButton>("StartBtn");
			btnCollection = this.AddComponent<UIButton>("Bottom/Collection");
			btnBar = this.AddComponent<UIButton>("Bottom/Bar");
			btnCloth = this.AddComponent<UIButton>("Bottom/Cloth");
			btnBlack = this.AddComponent<UIButton>("Bottom/Black");
			btnMarket = this.AddComponent<UIButton>("Bottom/Market");
			btnRestaurant = this.AddComponent<UIButton>("Bottom/Restaurant");
			btnGameGroup = AddComponent<UIButton>("Mid/Right/btn_gameGroup");
			btnShare = AddComponent<UIButton>("Mid/Right/btn_Share");
			btnRecommend = AddComponent<UIButton>("Mid/Right/btn_Recommend");
			btnCollect = AddComponent<UIButton>("Mid/Right/btn_Collect");
			btnFollow = AddComponent<UIButton>("Mid/Right/btn_Follow");
			btnDesktop = AddComponent<UIButton>("Mid/Right/btn_Desktop");
			btnStock = this.AddComponent<UIButton>("Bottom/Stock");
			btnSetting = this.AddComponent<UIButton>("Mid/Left/btn_setting");
			btnRank = AddComponent<UIRankBtn>("Mid/Left/btn_rank");
			eventTrigger = AddComponent<UIEventTrigger>("ScrollView");
			btnSidebar = AddComponent<UIButton>("Mid/Right/btn_sidebar");
			btnDailyTask = AddComponent<UIButton>("Mid/Left/btn_dailytask");
			ScrollRect = eventTrigger.GetRectTransform().GetComponent<ScrollRect>();
			AddComponent<UIRedDot,string>("Bottom/Collection/Icon/RedDot","Collection");
			AddComponent<UIRedDot,string>("Bottom/Bar/Icon/RedDot","Bar");
			AddComponent<UIRedDot,string>("Bottom/Cloth/Icon/RedDot","Cloth");
			AddComponent<UIRedDot,string>("Bottom/Black/Icon/RedDot","Black");
			AddComponent<UIRedDot,string>("Bottom/Restaurant/Icon/RedDot","Restaurant");
			AddComponent<UIRedDot,string>("Bottom/Market/Icon/RedDot","Market");
			AddComponent<UIRedDot,string>("Bottom/Stock/Icon/RedDot","Stock");
			AddComponent<UIRedDot, string>("Mid/Left/btn_rank/Icon/RedDot", "Rank");
			AddComponent<UIRedDot, string>("Mid/Right/btn_sidebar/Icon/RedDot", "Sidebar");
			AddComponent<UIRedDot, string>("Mid/Left/btn_dailytask/Icon/RedDot", "DailyTask");
		}

		public void OnEnable()
		{
			OnEnable(false);
		}

		public void OnEnable(bool isCloth)
		{
			PlayerDataManager.Instance?.RefreshRestaurantProfitRedDot();
			Refresh3rdBtns();
			Refresh();
			
			Mask.SetOnClick(OnClickLogin);
			btnStart.SetInteractable(true);
			this.btnStart.SetOnClick(OnClickBtnStart);
			btnCollection.SetOnClick(OnClickCollection);
			btnBar.SetOnClick(OnClickBar);
			btnCloth.SetOnClick(OnClickCloth);
			btnBlack.SetOnClick(OnClickBtnBlack);
			btnMarket.SetOnClick(OnClickBtnMarket);
			btnRestaurant.SetOnClick(OnClickBtnRestaurant);
			btnStock.SetOnClick(OnClickStock);
			btnSetting.SetOnClick(OnClickSetting);
			btnRank.SetOnClick(OnClickRank);
			btnSidebar.SetOnClick(OnClickSidebar);
			btnGameGroup.SetOnClick(OnClickGameGroup);
			btnShare.SetOnClick(OnClickShare);
			btnRecommend.SetOnClick(OnClickRecommend);
			btnCollect.SetOnClick(OnClickCollect);
			btnFollow.SetOnClick(OnClickFollow);
			btnDesktop.SetOnClick(OnClickDesktop);
			btnDailyTask.SetOnClick(OnClickDailyTask);
			ScrollRect.content.anchoredPosition = Vector2.zero;
			eventTrigger.AddOnBeginDrag(OnBeginDrag);
			eventTrigger.AddOnEndDrag(OnEndDrag);
			panding = eventTrigger.GetRectTransform().rect.width;
			len = ScrollRect.content.rect.width - panding;
			Bottom.SetActive(PlayerManager.Instance.Uid != 0);
			Mid.SetActive(PlayerManager.Instance.Uid != 0);
			Mask.SetActive(PlayerManager.Instance.Uid == 0);
			btnStart.SetActive(PlayerManager.Instance.Uid != 0);
			var mainCamera = CameraManager.Instance.MainCamera();
			if (mainCamera == null) return;
			mainCamera.cullingMask = Define.AllLayer;
			if (collector == null)
			{
				collector = mainCamera.GetComponent<ReferenceCollector>();
			}
			if (isCloth)
			{
				Move2Building("Cloth", false).Coroutine();
			}
			else
			{
				var pos = mainCamera.transform.localPosition;
				var progress = (pos.x - GameConst.HomeCameraMinX) /
				               (GameConst.HomeCameraMaxX - GameConst.HomeCameraMinX);
				ScrollRect.content.anchoredPosition =
					new Vector2(len / 2 - progress * len, ScrollRect.content.anchoredPosition.y);
				if (Define.FeedType == 1)
				{
					OnClickBtnRestaurant();
				}
			}
		}

		private void Refresh3rdBtns()
		{
			btnGameGroup.SetActive(false);
			btnRecommend.SetActive(false);
			btnSidebar.SetActive(SDKManager.Instance.CanSliderBar());
			btnCollect.SetActive(false);
			btnFollow.SetActive(false);
			btnShare.SetActive(SDKManager.Instance.CanShare());
			btnDesktop.SetActive(false);
#if UNITY_WEBGL_WeChat || UNITY_EDITOR
			btnGameGroup.SetActive(true);
			btnRecommend.SetActive(true);
#endif
			
#if UNITY_WEBGL_TT || UNITY_EDITOR
			btnGameGroup.SetActive(true);
			btnCollect.SetActive(true);
			btnFollow.SetActive(true);
			btnDesktop.SetActive(true);
#endif
			
#if UNITY_WEBGL_TAPTAP || UNITY_EDITOR
			btnDesktop.SetActive(true);
#endif
			
#if UNITY_WEBGL_BILIGAME || UNITY_EDITOR
			btnDesktop.SetActive(true);
#endif
			
#if UNITY_WEBGL_KS || UNITY_EDITOR
			btnDesktop.SetActive(true);
			btnCollect.SetActive(true);
			btnFollow.SetActive(true);
			btnGameGroup.SetActive(true);
#endif
		}
		public void Refresh()
		{
			var conf = RestaurantConfigCategory.Instance.GetByLv(PlayerDataManager.Instance.RestaurantLv, out _);
			btnMarket.SetActive(conf.UnlockMarket == 1);
			btnCloth.SetActive(conf.UnlockCloth == 1);
			btnBlack.SetActive(conf.UnlockBlack == 1);
		}

		public void Update()
		{
			var mainCamera = CameraManager.Instance.MainCamera();
			if (mainCamera == null) return;
			if (collector == null)
			{
				collector = mainCamera.GetComponent<ReferenceCollector>();
			}
			var timeNow = TimerManager.Instance.GetTimeNow();
			var rect = ScrollRect.content;
			var posX = rect.anchoredPosition.x;
			var progress = (posX + len / 2) / len;
			if (UIManager.Instance.GetTopWindow(UILayerNames.NormalLayer).View == this)
			{
				if (!isDraging && timeNow > lastDragTime + 5000)
				{
					if (!moveTurn)
					{
						if (progress > 0.98f)
						{
							progress = Mathf.Lerp(progress, 1.01f, 0.5f * Time.deltaTime);
						}
						else
						{
							progress += 0.02f * Time.deltaTime;
						}

						if (progress >= 1)
						{
							moveTurn = !moveTurn;
						}
					}
					else
					{
						if (progress < 0.02f)
						{
							progress = Mathf.Lerp(progress, -0.01f, 0.5f * Time.deltaTime);
						}
						else
						{
							progress -= 0.02f * Time.deltaTime;
						}

						if (progress <= 0)
						{
							moveTurn = !moveTurn;
						}
					}
					posX = progress * len - len / 2;
					rect.anchoredPosition = new Vector2(posX, rect.anchoredPosition.y);
				}
			}
			var pos = mainCamera.transform.localPosition;
			var x = GameConst.HomeCameraMaxX + (GameConst.HomeCameraMinX - GameConst.HomeCameraMaxX) * progress;
			if (x != pos.x)
			{
				mainCamera.transform.localPosition = new Vector3(x, pos.y, pos.z);
			}
			if(GuidanceManager.GuideTarget != null) return;
			if(PlayerManager.Instance.Uid < 0) return;
			if (InputManager.Instance.GetKeyDown(GameKeyCode.NormalAttack))
			{
				var mousePos = InputManager.Instance.GetLastTouchPos();
				if (!InputManager.Instance.IsPointerOverUI(mousePos, ScrollRect.viewport))
				{
					startPos = mousePos;
				}
			}
			if (InputManager.Instance.GetKeyUp(GameKeyCode.NormalAttack))
			{
				var mousePos = InputManager.Instance.GetLastTouchPos();
				if (!InputManager.Instance.IsPointerOverUI(mousePos, ScrollRect.viewport) && mousePos == startPos)
				{
					var ray = mainCamera.ScreenPointToRay(mousePos);
					if (PhysicsHelper.RaycastNonAlloc(ray.origin, ray.direction, out var res, 100,
						    PhysicsHelper.hitscene))
					{
						switch (res.transform.gameObject.name)
						{
							case "Collection":
								OnClickCollection();
								break;
							case "Bar":
								OnClickBar();
								break;
							case "Cloth":
								OnClickCloth();
								break;
							case "Black":
								OnClickBtnBlack();
								break;
							case "Restaurant":
								OnClickBtnRestaurant();
								break;
							case "Market":
								OnClickBtnMarket();
								break;
							case "Stock":
								OnClickStock();
								break;
							case "Station":
								OnClickStation();
								break;
							default:
								Log.Info(res.transform.gameObject.name);
								break;
						}
					}

					startPos = Vector2.zero;
				}
				
			}
		}
		#endregion

		#region 事件绑定

		private void OnClickLogin()
		{
			OnClickLoginAsync().Coroutine();
		}

		private async ETTask OnClickLoginAsync()
		{
			var res = await PlayerManager.Instance.Login();
			if (res)
			{
				Bottom.SetActive(PlayerManager.Instance.Uid != 0);
				Mid.SetActive(PlayerManager.Instance.Uid != 0);
				Mask.SetActive(PlayerManager.Instance.Uid == 0);
				btnStart.SetActive(PlayerManager.Instance.Uid != 0);
				await UIManager.Instance.OpenWindow<UITopView>(UITopView.PrefabPath, UILayerNames.TipLayer);
				Refresh();
			}
		}
		private void OnBeginDrag(PointerEventData data)
		{
			isDraging = true;
		}
		private void OnEndDrag(PointerEventData data)
		{
			isDraging = false;
			lastDragTime = TimerManager.Instance.GetTimeNow();
		}
		public void OnClickBtnStart()
		{
			CloseSelf().Coroutine();
			UIManager.Instance.OpenWindow<UIAuctionSelectView>(UIAuctionSelectView.PrefabPath).Coroutine();
		}

		public void OnClickCollection()
		{
 			// Move2Building("Collection").Coroutine();
			UIManager.Instance.OpenBox<UIToast, I18NKey>(UIToast.PrefabPath, I18NKey.Notice_Not_Open_Yet).Coroutine();
		}
		
		public void OnClickBar()
        {
	        // Move2Building("Bar").Coroutine();
	        UIManager.Instance.OpenBox<UIToast, I18NKey>(UIToast.PrefabPath, I18NKey.Notice_Not_Open_Yet).Coroutine();
        }

		public void OnClickCloth()
		{
			var config = RestaurantConfigCategory.Instance.GetByLv(PlayerDataManager.Instance.RestaurantLv, out _);
			if (config == null || config.UnlockMarket == 0)
			{
				UIManager.Instance.OpenBox<UIToast, I18NKey>(UIToast.PrefabPath, I18NKey.Text_Toast_Lock).Coroutine();
				return;
			}
			OnClickClothAsync().Coroutine();
		}

		private async ETTask OnClickClothAsync()
		{
			await Move2Building("Cloth");
			UIManager.Instance.CloseWindow<UITopView>().Coroutine();
			await CloseSelf();
			var blendView = await UIManager.Instance.OpenWindow<UIBlendView>(UIBlendView.PrefabPath,UILayerNames.TopLayer);
			await blendView.CaptureBg();
			await SceneManager.Instance.SwitchScene<CreateScene>();
		}

		public void OnClickBtnBlack()
		{
			OnClickBtnBlackAsync().Coroutine();
		}
		private async ETTask OnClickBtnBlackAsync()
		{
			using ListComponent<ETTask> task = ListComponent<ETTask>.Create();
			task.Add(GameObjectPoolManager.GetInstance().PreLoadGameObjectAsync(UIBlackView.PrefabPath,0));
			task.Add(Move2Building("Black"));
			await ETTaskHelper.WaitAll(task);
			UIManager.Instance.OpenWindow<UIBlackView>(UIBlackView.PrefabPath).Coroutine();
		}

		public void OnClickBtnMarket()
		{
			var config = RestaurantConfigCategory.Instance.GetByLv(PlayerDataManager.Instance.RestaurantLv, out _);
			if (config == null || config.UnlockMarket == 0)
			{
				UIManager.Instance.OpenBox<UIToast, I18NKey>(UIToast.PrefabPath, I18NKey.Text_Toast_Lock).Coroutine();
				return;
			}

			OnClickBtnMarketAsync().Coroutine();
		}
		private async ETTask OnClickBtnMarketAsync()
		{
			using ListComponent<ETTask> task = ListComponent<ETTask>.Create();
			task.Add(GameObjectPoolManager.GetInstance().PreLoadGameObjectAsync(UIMarketView.PrefabPath,0));
			task.Add(Move2Building("Market"));
			await ETTaskHelper.WaitAll(task);
			RedDotManager.Instance.RefreshRedDotViewCount("Market_RefreshAuto", 0);
			UIManager.Instance.OpenWindow<UIMarketView>(UIMarketView.PrefabPath).Coroutine();
		}

		public void OnClickBtnRestaurant()
		{
			OnClickBtnRestaurantAsync().Coroutine();
		}
		private async ETTask OnClickBtnRestaurantAsync()
		{
			using ListComponent<ETTask> task = ListComponent<ETTask>.Create();
			task.Add(GameObjectPoolManager.GetInstance().PreLoadGameObjectAsync(UIWashDishView.PrefabPath,0));
			task.Add(Move2Building("Restaurant"));
			await ETTaskHelper.WaitAll(task);
			UIManager.Instance.OpenWindow<UIWashDishView>(UIWashDishView.PrefabPath).Coroutine();
		}
		public void OnClickStock()
		{
			// Move2Building("Stock").Coroutine();
			UIManager.Instance.OpenBox<UIToast, I18NKey>(UIToast.PrefabPath, I18NKey.Notice_Not_Open_Yet).Coroutine();
		}

		public void OnClickSetting()
		{
			UIManager.Instance.OpenWindow<UISettingWin>(UISettingWin.PrefabPath).Coroutine();
		}

		public void OnClickRank()
		{
			OnClickRankAsync().Coroutine();
		}

		public void OnClickStation()
		{
			// Move2Building("Station").Coroutine();
			UIManager.Instance.OpenBox<UIToast, I18NKey>(UIToast.PrefabPath, I18NKey.Notice_Not_Open_Yet).Coroutine();
		}

		private async ETTask OnClickRankAsync()
		{
			using ListComponent<ETTask<bool>> tasks = ListComponent<ETTask<bool>>.Create();
			tasks.Add(I18NManager.Instance.AddSystemFonts());
			tasks.Add(TimerManager.Instance.WaitAsync(5000));
			var task1 = GameObjectPoolManager.GetInstance().PreLoadGameObjectAsync(UIRankView.PrefabPath, 0);
			var task2 = APIManager.Instance.GetRankInfo(PlayerManager.Instance.Uid);
			await UIManager.Instance.OpenWindow<UINetView>(UINetView.PrefabPath);
			await ETTaskHelper.WaitAny(tasks);
			var list = await task2;
			await task1;
			await UIManager.Instance.CloseWindow<UINetView>();
			CloseSelf().Coroutine();
			UIManager.Instance.OpenWindow<UIRankView, RankList>(UIRankView.PrefabPath, list).Coroutine();
		}

		public void OnClickSidebar()
		{
			UIManager.Instance.OpenWindow<UISidebarRewardsWin>(UISidebarRewardsWin.PrefabPath).Coroutine();
		}

		public void OnClickGameGroup()
		{
			SDKManager.Instance.GameGroup();
		}

		public void OnClickShare()
		{
			SDKManager.Instance.ShareGlobal().Coroutine();
		}
		
		public void OnClickRecommend()
		{
			SDKManager.Instance.Recommend();
		}
		
		public void OnClickCollect()
		{
			SDKManager.Instance.Collect();
		}
		
		public void OnClickFollow()
		{
			SDKManager.Instance.Follow();
		}

		public void OnClickDesktop()
		{
			SDKManager.Instance.Desktop();
		}
		public void OnClickDailyTask()
		{
			var conf = RestaurantConfigCategory.Instance.GetByLv(PlayerDataManager.Instance.RestaurantLv, out _);
			if (conf.Need == 0)
			{
				UIManager.Instance.OpenBox<UIToast, I18NKey>(UIToast.PrefabPath, I18NKey.Text_Toast_Lock).Coroutine();
				return;
			}
			UIManager.Instance.OpenWindow<UIDailyWin>(UIDailyWin.PrefabPath).Coroutine();
		}
		#endregion

		private async ETTask Move2Building(string name, bool isEnter = true)
		{
			if (collector != null)
			{
				var build = collector.Get<BoxCollider>(name);
				if (build != null)
				{
					var camera = CameraManager.Instance.MainCamera();
					var startPos = camera.transform.position;
					var endPos = new Vector3(build.transform.position.x, startPos.y, startPos.z);
					isDraging = true;
					var startTime = TimerManager.Instance.GetTimeNow();
					float interval = Mathf.Abs(endPos.x - startPos.x) * 30;
					while (isEnter)
					{
						await TimerManager.Instance.WaitAsync(1);
						var timeNow = TimerManager.Instance.GetTimeNow();
						var deltaTime = timeNow - startTime;
						camera.transform.position = Vector3.Lerp(startPos, endPos, deltaTime / interval);
						if (deltaTime> interval) break;
					}
					camera.transform.position = endPos;
					var rect = ScrollRect.content;
					var progress = (camera.transform.localPosition.x - GameConst.HomeCameraMaxX) /
					               (GameConst.HomeCameraMinX - GameConst.HomeCameraMaxX);
					var posX = progress * len - len / 2;
					rect.anchoredPosition = new Vector2(posX, rect.anchoredPosition.y);
					isDraging = false;
					lastDragTime = TimerManager.Instance.GetTimeNow();
					if (isEnter)
					{
						CloseSelf().Coroutine();
						var enter = await UIManager.Instance.OpenWindow<UIEnterView>(UIEnterView.PrefabPath);
						await enter.EnterTarget(build.gameObject);
					}
				}
			}
		}

		public override async ETTask CloseSelf()
		{
			await Animator.Play("Lobby_Close");
			await base.CloseSelf();
			CameraManager.Instance.MainCamera().cullingMask = Define.UILayer;
		}
	}
}
