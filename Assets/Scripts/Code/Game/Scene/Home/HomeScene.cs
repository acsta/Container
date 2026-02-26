using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace TaoTie
{
    public class HomeScene:SceneManagerProvider,IScene
    {
        private UILoadingView win;
        private UIBlendView blendView;
        
        private long envId;
        private string[] dontDestroyWindow =
        {
            TypeInfo<UILoadingView>.TypeName,
            TypeInfo<UIBlendView>.TypeName,
            TypeInfo<UIEnterView>.TypeName,
            TypeInfo<UIGuidanceView>.TypeName,
            TypeInfo<UILoadingView2>.TypeName,
        };

        public string[] GetDontDestroyWindow()
        {
            return dontDestroyWindow;
        }

        public List<string> GetScenesChangeIgnoreClean()
        {
            var res = new List<string>();
            res.Add(UILoadingView.PrefabPath);
            res.Add(UIEnterView.PrefabPath); 
            res.Add(UIBlendView.PrefabPath); 
            res.Add(UIGuidanceView.PrefabPath); 
            res.Add(UILoadingView2.PrefabPath);
            return res;
        }
        public void GetProgressPercent(out float cleanup, out float loadScene, out float prepare)
        {
            cleanup = 0.2f;
            loadScene = 0.65f;
            prepare = 0.15f;
        }
        
        public async ETTask OnCreate()
        {
            await ETTask.CompletedTask;
        }

        public async ETTask OnEnter()
        {
            blendView = UIManager.Instance.GetView<UIBlendView>(1);
            if (blendView == null && UIManager.Instance.GetView<UIEnterView>(1) == null)
            {
                win = UIManager.Instance.GetView<UILoadingView>(1);
                if (win == null)
                {
                    win = UIManager.Instance.GetView<UILoadingView2>(1);
                }
                if (win == null)
                {
                    win = await UIManager.Instance.OpenWindow<UILoadingView2>(UILoadingView2.PrefabPath,
                        UILayerNames.TipLayer);
                }
                win.SetProgress(0);
            }
        }

        public async ETTask OnLeave()
        {
            var render = GraphicsSettings.renderPipelineAsset as UniversalRenderPipelineAsset;
            if (render!= null)
            {
                render.shadowDistance = 30;
            }
            RemoveManager<EntityManager>();
            EnvironmentManager.Instance.SceneLight = null;
            EnvironmentManager.Instance.Remove(envId);
            await ETTask.CompletedTask;
        }

        public async ETTask OnPrepare(float progressStart,float progressEnd)
        {
            envId = EnvironmentManager.Instance.Create(1, EnvironmentPriorityType.Scene);
            var em = RegisterManager<EntityManager>();
            ReferenceCollector collector = CameraManager.Instance.MainCamera().GetComponent<ReferenceCollector>();
            var root = collector.Get<Transform>("Character_Timeline");
            var dir = root?.GetComponentInChildren<PlayableDirector>();
            if (dir != null)
            {
                MultiMap<string, PlayableBinding> bindings = new MultiMap<string, PlayableBinding>();
                foreach (var o in dir.playableAsset.outputs)
                {
                    if (o.streamName.StartsWith("Npc_"))
                    {
                        if (PerformanceManager.Instance.Level < PerformanceManager.DevicePerformanceLevel.Mid
                            &&o.streamName.EndsWith("_Add"))
                        {
                            continue;
                        }
                        bindings.Add(o.streamName, o);
                    }
                }
                using ListComponent<ETTask<bool>> tasks = ListComponent<ETTask<bool>>.Create(); 
                using ListComponent<NPC> npcs = ListComponent<NPC>.Create();
                ClothGenerateManager.Instance.Generate(bindings.Count, null);
                for (int i = 0; i < bindings.Count; i++)
                {
                    var npc = em.CreateEntity<NPC>();
                    npcs.Add(npc);
                    var ghc = npc.GetComponent<GameObjectHolderComponent>();
                    tasks.Add(ghc.WaitLoadGameObjectOver());
                }

                await ETTaskHelper.WaitAll(tasks);
                int index = 0;
                for (int i = 0; i < root.childCount; i++)
                {
                    var trans = root.GetChild(i);
                    if (bindings.TryGetValue(trans.name, out var binds))
                    {
                        var npc = npcs[index];
                        index++;
                        var ghc = npc.GetComponent<GameObjectHolderComponent>();
                        ghc.EntityView.parent = trans;
                        ghc.EntityView.localPosition = Vector3.zero;
                        ghc.EntityView.localRotation = Quaternion.identity;
                        ghc.EntityView.localScale =  Vector3.one;
                        for (int j = 0; j < binds.Count; j++)
                        {
                            dir.SetGenericBinding(binds[j].sourceObject, ghc.EntityView.GetComponent(binds[j].outputTargetType));
                        }
                    }
                }
            }
            await ETTask.CompletedTask;
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
        public override string GetName()
        {
            return "main";
        }
        public string GetScenePath()
        {
            return "Scenes/Main WBScene/Main WB.unity";
        }

        public async ETTask OnSwitchSceneEnd()
        {
            SoundManager.Instance.PlayMusic("Audio/Music/MainView.mp3");
            var render = GraphicsSettings.renderPipelineAsset as UniversalRenderPipelineAsset;
            if (render!= null)
            {
                render.shadowDistance = 50;
            }
            var pos = CameraManager.Instance.MainCamera().transform.localPosition;
            var x = Mathf.Lerp(GameConst.HomeCameraMaxX, GameConst.HomeCameraMinX, 0.5f);
            CameraManager.Instance.MainCamera().transform.localPosition = new Vector3(x, pos.y, pos.z);
            // var light = GameObject.FindWithTag("MainLight")?.transform.parent;
            // if (light != null)
            // {
            //     EnvironmentManager.Instance.SceneLight = light;
            // }
            UIEnterView enterView = null;
            if (blendView != null)
            {
                await blendView.DoFade();
            }
            else
            {
                enterView = UIManager.Instance.GetView<UIEnterView>(1);
            }
            await UIManager.Instance.OpenWindow<UILobbyView, bool>(UILobbyView.PrefabPath, enterView != null);
            if (PlayerManager.Instance.Uid != 0)
                await UIManager.Instance.OpenWindow<UITopView>(UITopView.PrefabPath, UILayerNames.TipLayer);
            if (enterView != null)
            {
                await enterView.Exit();
            }
            else
            {
                await GameObjectPoolManager.GetInstance().PreLoadGameObjectAsync(UIEnterView.PrefabPath, 1);
            }
            await UIManager.Instance.CloseWindow<UIBlendView>();
            await UIManager.Instance.DestroyWindow<UILoadingView>();
            await UIManager.Instance.DestroyWindow<UILoadingView2>();
            await UIManager.Instance.CloseWindow<UIEnterView>();
            win = null;
            blendView = null;
        }
    }
}