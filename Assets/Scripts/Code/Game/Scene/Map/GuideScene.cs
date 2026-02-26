using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace TaoTie
{
    public class GuideScene:SceneManagerProvider,IScene
    {
        public const int ConfigId = -2;
        public LevelConfig Config => LevelConfigCategory.Instance.Get(ConfigId);
        public ReferenceCollector Collector;

        public UnityEngine.Rendering.Volume Volume;
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
        
        private UILoadingView win;
        private string[] dontDestroyWindow =
        {
            TypeInfo<UILoadingView>.TypeName,
            TypeInfo<UIGuidanceView>.TypeName,
        };

        public string[] GetDontDestroyWindow()
        {
            return dontDestroyWindow;
        }

        public List<string> GetScenesChangeIgnoreClean()
        {
            var res = new List<string>();
            res.Add(UILoadingView.PrefabPath); 
            res.Add(UIGuidanceView.PrefabPath); 
            return res;
        }
        public async ETTask OnCreate()
        {
            await ETTask.CompletedTask;
        }

        public async ETTask OnEnter()
        {
            win = UIManager.Instance.GetView<UILoadingView>(1);
            if (win == null)
            {
                win = await UIManager.Instance.OpenWindow<UILoadingView>(UILoadingView.PrefabPath,
                    UILayerNames.TipLayer);
            }
            win.SetProgress(0);
        }

        public async ETTask OnLeave()
        {
            RemoveManager<AuctionGuideManager>();
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
            RegisterManager<AuctionGuideManager, GuideScene>(this);
            using ListComponent<ETTask> tasks = ListComponent<ETTask>.Create();
            var gStep = GuidanceStageConfigCategory.Instance.GetAllList();
            for (int i = 0; i < gStep.Count; i++)
            {
                for (int j = 0; j < gStep[i].Items.Length; j++)
                {
                    var unitId = ItemConfigCategory.Instance.Get(gStep[i].Items[i]).UnitId;
                    var unit = UnitConfigCategory.Instance.Get(unitId);
                    tasks.Add(GameObjectPoolManager.GetInstance().PreLoadGameObjectAsync(unit.Perfab, 1));
                }
            }
            tasks.Add(GameObjectPoolManager.GetInstance().PreLoadGameObjectAsync(GameConst.SmokePrefab, 1));
            tasks.Add(GameObjectPoolManager.GetInstance().PreLoadGameObjectAsync(UIGuideGameView.PrefabPath, 1));
            tasks.Add(GameObjectPoolManager.GetInstance().PreLoadGameObjectAsync(UIEmojiItem.PrefabPath, 1));
            tasks.Add(GameObjectPoolManager.GetInstance().PreLoadGameObjectAsync(UIBubbleItem.PrefabPath, 1));
            tasks.Add(GameObjectPoolManager.GetInstance().PreLoadGameObjectAsync(UIButtonView.PrefabPath,1));
            tasks.Add(GameObjectPoolManager.GetInstance().PreLoadGameObjectAsync(UIItemsView.PrefabPath,1));
            tasks.Add(MaterialManager.Instance.PreLoadMaterial(GameConst.PlayTypeMat));
            tasks.Add(MaterialManager.Instance.PreLoadMaterial(GameConst.TaskMat));
            
            while (IAuctionManager.Instance.AState != AuctionState.Prepare 
                   && IAuctionManager.Instance.AState != AuctionState.EnterAnim)
            {
                await TimerManager.Instance.WaitFrameAsync();
            }
            await ETTaskHelper.WaitAll(tasks);
        }

        public async ETTask OnComplete()
        {
            await ETTask.CompletedTask;
        }

        public async ETTask SetProgress(float value)
        {
            win?.SetProgress(value);
            await ETTask.CompletedTask;
        }

        public virtual async ETTask OnSwitchSceneEnd()
        {
            SoundManager.Instance.PlayMusic("Audio/Music/Game.mp3");
            await UIManager.Instance.DestroyWindow<UILoadingView>();
            Log.Info("进入场景 " + GetScenePath());
        }
        
    }
}