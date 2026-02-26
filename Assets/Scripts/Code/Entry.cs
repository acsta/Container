using System;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using YooAsset;

namespace TaoTie
{
    public static class Entry
    {
        private static long START_TIME;
        public static void Start()
        {
            StartAsync().Coroutine();
        }

        private static async ETTask StartAsync()
        {
            try
            {
                GameSetting.Init();
                ManagerProvider.RegisterManager<Messager>();
                ManagerProvider.RegisterManager<LogManager>();
                
                ManagerProvider.RegisterManager<AttributeManager>();
                
                ManagerProvider.RegisterManager<CoroutineLockManager>();
                ManagerProvider.RegisterManager<TimerManager>();
                START_TIME = TimerManager.Instance.GetTimeNow();
                ManagerProvider.RegisterManager<CacheManager>();

                var cm = ManagerProvider.RegisterManager<ConfigManager>();
                ManagerProvider.RegisterManager<ResourcesManager>();
                ManagerProvider.RegisterManager<GameObjectPoolManager>();
                ManagerProvider.RegisterManager<ImageLoaderManager>();
                ManagerProvider.RegisterManager<MaterialManager>();
                
                ManagerProvider.RegisterManager<I18NManager>();
                ManagerProvider.RegisterManager<UIManager>();
               
                
                await UIManager.Instance.OpenWindow<UILoadingView>(UILoadingView.PrefabPath, UILayerNames.TipLayer);
                await cm.LoadAsync();
                
                ManagerProvider.RegisterManager<RedDotManager>();

                ManagerProvider.RegisterManager<CameraManager>();
                ManagerProvider.RegisterManager<SceneManager>();
                
                ManagerProvider.RegisterManager<ServerConfigManager>();

                ManagerProvider.RegisterManager<InputManager>();
                if(PackageManager.Instance.PlayMode == EPlayMode.HostPlayMode && (Define.Networked||Define.ForceUpdate))
                    await UIManager.Instance.OpenWindow<UIUpdateView,Action>(UIUpdateView.PrefabPath,StartGame);//下载热更资源
                else
                    await StartGameAsync();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
        static void StartGame()
        {
            StartGameAsync().Coroutine();
        }

        static async ETTask StartGameAsync()
        {
            ManagerProvider.RegisterManager<APIManager>();
            var sm = ManagerProvider.RegisterManager<SoundManager>();
            ManagerProvider.RegisterManager<ShockManager>();
            ManagerProvider.RegisterManager<PlayerManager>();
            ManagerProvider.RegisterManager<PlayerDataManager>();
            ManagerProvider.RegisterManager<SDKManager>();
            ManagerProvider.RegisterManager<GameRecorderManager>();
            ManagerProvider.RegisterManager<AdManager>();
            ManagerProvider.RegisterManager<GameTimerManager>();
            ManagerProvider.RegisterManager<GuidanceManager>();
            ManagerProvider.RegisterManager<ClothGenerateManager>();
            GameObjectPoolManager.GetInstance().AddPersistentPrefabPath(UIToast.PrefabPath);
            using ListComponent<ETTask> tasks = ListComponent<ETTask>.Create();
            tasks.Add(ManagerProvider.RegisterManager<ConfigAIDecisionTreeCategory>().LoadAsync());
            tasks.Add(ManagerProvider.RegisterManager<EnvironmentManager>().LoadAsync());
            tasks.Add(sm.InitAsync());
            tasks.Add(GameObjectPoolManager.GetInstance().PreLoadGameObjectAsync(UIToast.PrefabPath, 1));
#if UNITY_WEBGL_TT || UNITY_WEBGL_KS || UNITY_WEBGL_WeChat || UNITY_WEBGL_TAPTAP || UNITY_WEBGL_QG || UNITY_WEBGL_MINIHOST || UNITY_WEBGL_BILIGAME || UNITY_WEBGL_4399
            tasks.Add(Login());
#endif
            await ETTaskHelper.WaitAll(tasks);
            
            await PackageManager.Instance.UnloadUnusedAssets(Define.DefaultName);
            GameObjectPoolManager.GetInstance().AddPersistentPrefabPath(UIBlendView.PrefabPath);
            await GameObjectPoolManager.GetInstance().PreLoadGameObjectAsync(UIBlendView.PrefabPath, 1);

            var loadingWin = UIManager.Instance.GetView<UILoadingView>(1);
#if !UNITY_WEBGL
            loadingWin?.SetTipText(I18NKey.Loading_Tip_2);
            await ResourcesManager.Instance.ShaderWarmUp();
#endif
            loadingWin?.SetTipText(I18NKey.Loading_Tip_3);
            UIManager.Instance.OpenWindow<UIGuidanceView>(UIGuidanceView.PrefabPath, UILayerNames.TipLayer).Coroutine();
            if (PlayerManager.Instance.Uid != 0 && PlayerDataManager.Instance?.IsGuideScene == false)
            {
                if (Define.FeedType == 2)
                {
                    //todo: Feed流新用户
                    Log.Error("Feed流新用户 未实现");
                    await SceneManager.Instance.SwitchScene<GuideScene>();
                }
                else
                {
                    //新用户先引导
                    await SceneManager.Instance.SwitchScene<GuideScene>();
                }
            }
            else
            {
                //老用户直接进主场景
                await SceneManager.Instance.SwitchScene<HomeScene>();
            }

            ReportScene();
            SoundManager.Instance.PlayMusic("Audio/Music/MainView.mp3");
        }
        
        static async ETTask Login()
        {
            var loadingWin = UIManager.Instance.GetView<UILoadingView>(1);
            loadingWin?.SetTipText(I18NKey.Loading_Tip_1);
            var res = await PlayerManager.Instance.Login(true);
            if (!res)
            {
                ETTask<bool> task = ETTask<bool>.Create(true);
                UIManager.Instance.OpenBox<UIMsgBoxWin,MsgBoxPara>(UIMsgBoxWin.PrefabPath, new MsgBoxPara()
                {
                    Content = I18NManager.Instance.I18NGetText(I18NKey.Net_Error),
                    CancelText = I18NManager.Instance.I18NGetText(I18NKey.Btn_Quit_Game),
                    CancelCallback = (win) =>
                    {
                        task.SetResult(false); 
                        UIManager.Instance.CloseBox(win).Coroutine();
                    },
                    ConfirmText = I18NManager.Instance.I18NGetText(I18NKey.Global_Fail_ReTry),
                    ConfirmCallback = (win) => 
                    { 
                        task.SetResult(true);
                        UIManager.Instance.CloseBox(win).Coroutine();
                    },
                }).Coroutine();
                if (await task)
                {
                    await Login();
                }
                else
                {
                    BridgeHelper.Quit();
                }
            }
        }


        static void ReportScene()
        {
            long during = TimerManager.Instance.GetTimeNow() - START_TIME;
            Log.Info(during);
#if !UNITY_EDITOR
#if UNITY_WEBGL_TT
            var data = new TTSDK.UNBridgeLib.LitJson.JsonData();
            data["sceneId"] = 7001;
            data["costTime"] = during;
            TTSDK.TT.ReportScene(data);
#elif UNITY_WEBGL_WeChat
            WeChatWASM.WX.ReportScene(new WeChatWASM.ReportSceneOption()
            {
                sceneId = 7,
                costTime = during,
            });
#elif UNITY_WEBGL_BILIGAME
		    WeChatWASM.WX.ReportScene(new WeChatWASM.ReportSceneParams()
            {
                sceneId = 7,
                costTime = during,
            });
#elif UNITY_WEBGL_KS
		    KSWASM.KS.ReportScene(new KSWASM.ReportSceneOption()
            {
                sceneId = 7001,
                costTime = during,
            });
#endif
#endif
        }
    }
    
}