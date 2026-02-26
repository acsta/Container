using System;
using System.Collections;
using YooAsset;
using UnityEngine;
using UnityEngine.UI;

namespace TaoTie
{

	public enum CodeMode
	{
		LoadDll = 1, //加载dll
		BuildIn = 2, //直接打进整包

		Wolong = 3,
		LoadFromUrl = 4,
	}

	public class Init : MonoBehaviour
	{
		public CodeMode CodeMode = CodeMode.LoadDll;

		public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;

		private bool IsInit = false;

		private WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

		private async ETTask AwakeAsync()
		{
#if !UNITY_EDITOR
#if UNITY_WEBGL
			if (PlayMode != EPlayMode.WebPlayMode)
			{
				PlayMode = EPlayMode.WebPlayMode;
				Debug.LogError("Error PlayMode! " + PlayMode);
			}
#else
			if (PlayMode == EPlayMode.EditorSimulateMode || PlayMode == EPlayMode.WebPlayMode)
			{
				PlayMode = EPlayMode.HostPlayMode;
				Debug.LogError("Error PlayMode! " + PlayMode);
			}	
#endif
#endif
			InitUnitySetting();

			//设置时区
			TimeInfo.Instance.TimeZone = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).Hours;
			System.AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
			{
				Log.Error(e.ExceptionObject.ToString());
			};

			DontDestroyOnLoad(gameObject);

			ETTask.ExceptionHandler += Log.Error;

			Log.ILog = new UnityLogger();
			await InitSDK();
			await PackageManager.Instance.Init(PlayMode);
#if !UNITY_EDITOR
			if(this.CodeMode == CodeMode.BuildIn && !PackageManager.Instance.CdnConfig.BuildHotfixAssembliesAOT)
				this.CodeMode = CodeMode.Wolong;
#endif
			RegisterManager();

			CodeLoader.Instance.CodeMode = this.CodeMode;
			IsInit = true;

			await CodeLoader.Instance.Start();
		}

		async ETTask InitSDK()
		{
			ETTask task = ETTask.Create(true);
#if UNITY_WEBGL_TT
			TTSDK.TT.InitSDK((code, env) =>
			{
				TTSDK.TT.GetAppLifeCycle().OnShow += (dic) =>
				{
					string scene = null;
					int enterWay = 0;
					if(dic.TryGetValue("scene", out var res))
					{
						scene = (string) res;
					}
					Log.Info("OnShow scene="+scene);
					if (TTSDK.TT.InContainerEnv && !string.IsNullOrEmpty(scene))
					{
						if (scene.EndsWith("1001") || scene.EndsWith("1036") ||
						    scene.EndsWith("1042"))
						{
							enterWay = 1;
						}
						else if (scene.EndsWith("1020"))
						{
							enterWay = 2;
						}
					}
					if (Define.EnterWay != enterWay)
					{
						Define.EnterWay = enterWay;
						Messager.Instance.Broadcast(0, MessageId.EnterWayChange);
					}
				};
				if (TTSDK.TT.InContainerEnv)
				{
					TTSDK.LaunchOption launchOption = TTSDK.TT.GetLaunchOptionsSync();
					
					if (launchOption.Scene.EndsWith("3041") &&
					    launchOption.Query.TryGetValue("feed_game_channel",out string channel))
					{
						if (channel == "1")
						{
							Define.FeedType = 1;
						}
						else if (channel == "2")
						{
							Define.FeedType = 2;
						}
					}

					if (launchOption.Scene.EndsWith("1001") || launchOption.Scene.EndsWith("1036") || 
					    launchOption.Scene.EndsWith("1042"))
					{
						Define.EnterWay = 1;
					}
					else if (launchOption.Scene.EndsWith("1020"))
					{
						Define.EnterWay = 2;
					}
				}
				task.SetResult();
			});
#elif UNITY_WEBGL_TAPTAP
#if UNITY_EDITOR
			task.SetResult();
#else
			TapTapMiniGame.Tap.InitSDK((code) =>
			{
				task.SetResult();
				Log.Info("Tap.InitSDK " + code);
			});
#endif
#elif UNITY_WEBGL_WeChat
#if UNITY_EDITOR
			task.SetResult();
#else
			WeChatWASM.WX.InitSDK((code) =>
			{
				var option = WeChatWASM.WX.GetLaunchOptionsSync();
				if(option.scene == 1037)
				{
					Define.EnterWay = 2;
				}
				WeChatWASM.WX.OnShow((option) =>
				{
					int enterWay = 0;
					if(option.scene == 1037) enterWay = 2;
					Log.Info("OnShow launchScene="+option.scene+"&location="+option.chatType);
					if (Define.EnterWay != enterWay)
					{
						Define.EnterWay = enterWay;
						Messager.Instance.Broadcast(0, MessageId.EnterWayChange);
					}
				});
				task.SetResult();
				Log.Info("WX.InitSDK " + code);
			});
#endif
#elif UNITY_WEBGL_BILIGAME
#if UNITY_EDITOR
			task.SetResult();
#else
			WeChatWASM.WX.InitSDK((code) =>
			{
				var option = WeChatWASM.WX.GetLaunchOptionsSync();
				if(option.scene == 021036)
				{
					Define.EnterWay = 1;
				}
				else (option.scene == 10002)
				{
					Define.EnterWay = 2;
				}
				WeChatWASM.WX.OnShow((option) =>
				{
					int enterWay = 0;
                    if(option.scene == 021036) enterWay = 1;
					else if(option.10002 == 10002) enterWay = 2;
					Log.Info("OnShow launchScene="+option.scene);
					if (Define.EnterWay != enterWay)
					{
						Define.EnterWay = enterWay;
						Messager.Instance.Broadcast(0, MessageId.EnterWayChange);
					}
				});
				task.SetResult();
				Log.Info("WX.InitSDK " + code);
			});
#endif
#elif UNITY_WEBGL_ALIPAY
			AlipaySdk.AlipaySDK.Init();
			task.SetResult();
#elif UNITY_WEBGL_KS
#if UNITY_EDITOR
			task.SetResult();
#else
			KSWASM.KS.InitSDK((code) =>
			{
				var option = KSWASM.KS.GetLaunchOptionsSync();
				if(option.from == "sidebar")
				{
					Define.EnterWay = 1;
				}
				else if(KSWASM.KS.IsLaunchFromShortcut())
				{
					Define.EnterWay = 2;
				}
				KSWASM.KS.OnShow((option) =>
				{
					int enterWay = 0;
					if(option.from == "sidebar") enterWay = 1;
					else if(KSWASM.KS.IsLaunchFromShortcut()) enterWay = 2;
					Log.Info("OnShow launchScene="+option.from);
					if (Define.EnterWay != enterWay)
					{
						Define.EnterWay = enterWay;
						Messager.Instance.Broadcast(0, MessageId.EnterWayChange);
					}
				});
				task.SetResult();
				Log.Info("KS.InitSDK " + code);
			});
#endif
#elif UNITY_WEBGL_FACEBOOK
#if UNITY_EDITOR
			task.SetResult();
#else
			await Meta.InstantGames.Sdk.FBInstant.InitializeAsync();
			await Meta.InstantGames.Sdk.FBInstant.StartGameAsync();
#endif
#elif UNITY_WEBGL_MINIHOST
#if UNITY_EDITOR
			task.SetResult();
#else
			minihost.TJ.InitSDK((code) =>
			{
				task.SetResult();
				Log.Info("minihost.InitSDK " + code);
			});
#endif
#elif UNITY_WEBGL_4399
			H5Game.API.Init(() =>
	        {
		        task.SetResult();
	        });
#else
			task.SetResult();
#endif
			await task;
		}
		private void Start()
		{
		    var canvasScaler = GameObject.Find("Canvas").GetComponent<CanvasScaler>();
            if (canvasScaler != null)
            {
                if ((float)Screen.width / Screen.height > Define.DesignScreenWidth / Define.DesignScreenHeight)
                    canvasScaler.matchWidthOrHeight = 1;
                else
                    canvasScaler.matchWidthOrHeight = 0;
            }
			AwakeAsync().Coroutine();
		}

		private void Update()
		{
			if (!IsInit) return;
			TimeInfo.Instance.Update();
			CodeLoader.Instance.Update?.Invoke();
			ManagerProvider.Update();
			if (CodeLoader.Instance.isReStart)
			{
				ReStart().Coroutine();
			}

			int count = UnityLifeTimeHelper.FrameFinishTask.Count;
			if (count > 0)
			{
				StartCoroutine(WaitFrameFinish());
			}
		}

		private IEnumerator WaitFrameFinish()
		{
			yield return waitForEndOfFrame;
			int count = UnityLifeTimeHelper.FrameFinishTask.Count;
			while (count-- > 0)
			{
				ETTask task = UnityLifeTimeHelper.FrameFinishTask.Dequeue();
				task.SetResult();
			}
		}

		public async ETTask ReStart()
		{
			CodeLoader.Instance.isReStart = false;
			Resources.UnloadUnusedAssets();
			await PackageManager.Instance.ForceUnloadAllAssets(Define.DefaultName);
			Resources.UnloadUnusedAssets();
			ManagerProvider.Clear();
			await PackageManager.Instance.UpdateConfig();
			//清两次，清干净
			GC.Collect();
			GC.Collect();
			Log.Debug("ReStart");

			RegisterManager();

			CodeLoader.Instance.OnApplicationQuit?.Invoke();
			await CodeLoader.Instance.Start();
		}

		private void RegisterManager()
		{
			ManagerProvider.RegisterManager<PerformanceManager>();
			ManagerProvider.RegisterManager<AssemblyManager>();
		}

		private void LateUpdate()
		{
			CodeLoader.Instance.LateUpdate?.Invoke();
			ManagerProvider.LateUpdate();
		}

		private void FixedUpdate()
		{
			CodeLoader.Instance.FixedUpdate?.Invoke();
			ManagerProvider.FixedUpdate();
		}

		private void OnApplicationQuit()
		{
			CodeLoader.Instance.OnApplicationQuit?.Invoke();
		}

		void OnApplicationFocus(bool hasFocus)
		{
			CodeLoader.Instance.OnApplicationFocus?.Invoke(hasFocus);
		}

		void OnApplicationPause(bool pauseStatus)
		{
			CodeLoader.Instance.OnApplicationFocus?.Invoke(!pauseStatus);
		}

		// 一些unity的设置项目
		void InitUnitySetting()
		{
			Input.multiTouchEnabled = false;
			//设置帧率
			QualitySettings.vSyncCount = 0;
			Application.runInBackground = true;
			UnityEngine.Random.InitState((int) TimeInfo.Instance.ServerNow());
#if UNITY_WEBGL_TT
			TTSDK.TT.SetKeepScreenOn(true);
#else 
			Screen.sleepTimeout = SleepTimeout.NeverSleep;
#endif
		}
	}
}