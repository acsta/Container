using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace TaoTie
{
    public class MapScene:SceneManagerProvider,IScene
    {
        public int ConfigId;
        public LevelConfig Config => LevelConfigCategory.Instance.Get(ConfigId);
        public ReferenceCollector Collector;

        public UnityEngine.Rendering.Volume Volume;

        private UIMatchView win;
        
        // private long envId;
        public override string GetName()
        {
            return Config.Name;
        }

        public string GetScenePath()
        {
            return Config.Perfab;
        }
        public void GetProgressPercent(out float cleanup, out float loadScene, out float prepare)
        {
            cleanup = 0.2f;
            loadScene = 0.45f;
            prepare = 0.35f;
        }
        
        private string[] dontDestroyWindow =
        {
            TypeInfo<UIEnterView>.TypeName,
            TypeInfo<UIGuidanceView>.TypeName,
            TypeInfo<UIMatchView>.TypeName,
        };

        public string[] GetDontDestroyWindow()
        {
            return dontDestroyWindow;
        }

        public List<string> GetScenesChangeIgnoreClean()
        {
            var res = new List<string>();
            res.Add(UIEnterView.PrefabPath);
            res.Add(UIGuidanceView.PrefabPath); 
            res.Add(UIMatchView.PrefabPath); 
            return res;
        }
        public async ETTask OnCreate()
        {
            await ETTask.CompletedTask;
        }

        public async ETTask OnEnter()
        {
            win = UIManager.Instance.GetView<UIMatchView>(1);
            if (win == null)
            {
                win = await UIManager.Instance.OpenWindow<UIMatchView, int>(UIMatchView.PrefabPath,ConfigId,
                    UILayerNames.TipLayer);
            }
            
            win.SetProgress(0);
            await win.LoadingAnim(true);
            await win.LoadingAnim(false);
            // if (Config.DayNight != null && Config.DayNight.Length == 2)
            // {
            //     envId = EnvironmentManager.Instance.CreateDayNight(Config.DayNight[0], Config.DayNight[1]);
            // }
        }

        public async ETTask OnLeave()
        {
            // EnvironmentManager.Instance.SceneLight = null;
            // EnvironmentManager.Instance.Remove(envId);
            RemoveManager<AuctionManager>();
            RemoveManager<EntityManager>();
            await ETTask.CompletedTask;
        }

        public async ETTask OnPrepare(float progressStart,float progressEnd)
        {
            RegisterManager<EntityManager>();
            Collector = CameraManager.Instance.MainCamera()?.GetComponent<ReferenceCollector>();
            Volume = Collector?.Get<UnityEngine.Rendering.Volume>("Volume");
            if (Volume == null) Volume = GameObject.FindObjectOfType<UnityEngine.Rendering.Volume>();
            if (Volume != null)
            {
                if(Volume.sharedProfile.TryGet<ActionLineVolume>(out var co))
                {
                    co.active = false;
                }
            }
            RegisterManager<AuctionManager, MapScene>(this);
            using ListComponent<ETTask> tasks = ListComponent<ETTask>.Create();
            
            tasks.Add(GameObjectPoolManager.GetInstance().PreLoadGameObjectAsync(GameConst.SmokePrefab, 1));
            tasks.Add(GameObjectPoolManager.GetInstance().PreLoadGameObjectAsync(UIGameView.PrefabPath, 1));
            tasks.Add(GameObjectPoolManager.GetInstance().PreLoadGameObjectAsync(UIEmojiItem.PrefabPath, 1));
            tasks.Add(GameObjectPoolManager.GetInstance().PreLoadGameObjectAsync(UIBubbleItem.PrefabPath, 1));
            tasks.Add(GameObjectPoolManager.GetInstance().PreLoadGameObjectAsync(UIButtonView.PrefabPath,1));
            tasks.Add(GameObjectPoolManager.GetInstance().PreLoadGameObjectAsync(UIItemsView.PrefabPath,1));
            tasks.Add(MaterialManager.Instance.PreLoadMaterial(GameConst.PlayTypeMat));
            tasks.Add(MaterialManager.Instance.PreLoadMaterial(GameConst.TaskMat));
            win.SetProgress(progressStart);
            await ETTaskHelper.WaitAll(tasks);
            win.SetProgress(1);
            while (IAuctionManager.Instance.AState != AuctionState.Prepare 
                   && IAuctionManager.Instance.AState != AuctionState.EnterAnim)
            {
                await TimerManager.Instance.WaitFrameAsync();
            }
        }

        public async ETTask OnComplete()
        {
            await ETTask.CompletedTask;
        }

        public async ETTask SetProgress(float value)
        {
            win.SetProgress(value);
            await ETTask.CompletedTask;   
        }

        public virtual async ETTask OnSwitchSceneEnd()
        {
            SoundManager.Instance.PlayMusic("Audio/Music/Game.mp3");
            if (PerformanceManager.Instance.Level < PerformanceManager.DevicePerformanceLevel.Mid)
            {
                Collector.Get<GameObject>("Env")?.SetActive(false);
            }
            // var light = GameObject.FindWithTag("MainLight").transform;
            // if (light != null)
            // {
            //     EnvironmentManager.Instance.SceneLight = light;
            // }
            //
            
            await UIManager.Instance.DestroyWindow<UIMatchView>();
            win = null;
            Log.Info("进入场景 " + GetScenePath());
        }
    }
}