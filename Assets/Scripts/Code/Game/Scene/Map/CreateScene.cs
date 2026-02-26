using System.Collections.Generic;
using UnityEngine;

namespace TaoTie
{
    public class CreateScene:SceneManagerProvider,IScene
    {
        private UIBlendView win;
        private string[] dontDestroyWindow =
        {
            TypeInfo<UIEnterView>.TypeName,
            TypeInfo<UIBlendView>.TypeName,
            TypeInfo<UIGuidanceView>.TypeName,
        };
        private Player player;
        public string[] GetDontDestroyWindow()
        {
            return dontDestroyWindow;
        }

        public List<string> GetScenesChangeIgnoreClean()
        {
            var res = new List<string>();
            res.Add(UIEnterView.PrefabPath); 
            res.Add(UIBlendView.PrefabPath); 
            res.Add(UIGuidanceView.PrefabPath); 
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
            win = UIManager.Instance.GetView<UIBlendView>();
            if (win == null)
            {
                win = await UIManager.Instance.OpenWindow<UIBlendView>(UIBlendView.PrefabPath,UILayerNames.TopLayer);
                await win.CaptureBg(true);
            }
        }

        public async ETTask OnLeave()
        {
            var blendView = await UIManager.Instance.OpenWindow<UIEnterView>(UIEnterView.PrefabPath,UILayerNames.TopLayer);
            await blendView.EnterTarget(player.GetComponent<GameObjectHolderComponent>()?.EntityView?.gameObject,false);
            await UIManager.Instance.DestroyWindow<UICreateView>();
            RemoveManager<EntityManager>();
            await ETTask.CompletedTask;
        }

        public async ETTask OnPrepare(float progressStart,float progressEnd)
        {
            var em = RegisterManager<EntityManager>();
            if (PlayerDataManager.Instance.Show != null)
            {
                int[] temp = new int[PlayerDataManager.Instance.Show.Length];
                for (int i = 0; i < temp.Length; i++)
                {
                    temp[i] = PlayerDataManager.Instance.Show[i];
                }
                player = em.CreateEntity<Player, int[]>(temp);
            }
            else
            {
                player = em.CreateEntity<Player, int[]>(null);
            }
            player.Position = Vector3.zero;
            player.Rotation = Quaternion.identity;
            player.LocalScale = Vector3.one;
            await ETTask.CompletedTask;
        }

        public async ETTask OnComplete()
        {
            await ETTask.CompletedTask;
        }

        public async ETTask SetProgress(float value)
        {
            await ETTask.CompletedTask;
        }

        public override string GetName()
        {
            return "Create";
        }

        public string GetScenePath()
        {
            return "Scenes/CreateScene/Create.unity";
        }

        public async ETTask OnSwitchSceneEnd()
        {
            var trans = CameraManager.Instance.MainCamera().transform;
            if (trans != null)
            {
                var flagStart = 1;
                var flagEnd = 0.8003906f;
                var flag =  (Define.DesignScreenHeight * SystemInfoHelper.screenWidth )/(Define.DesignScreenWidth * 
                    (SystemInfoHelper.screenHeight + SystemInfoHelper.safeArea.yMin));
               
                flag = (flag - flagStart) / (flagEnd - flagStart);
                if (flag < 0) flag = 0;
                trans.position = new Vector3(0, 1, 6 + 1.5f * flag);
                trans.eulerAngles = new Vector3(20 - 3 * flag, 180, 0);
            }
            await UIManager.Instance.OpenWindow<UICreateView, Player>(UICreateView.PrefabPath, player);
            await DoFade();
            win = null;
        }
        private async ETTask DoFade()
        {
            await win.DoFade();
            await UIManager.Instance.CloseWindow<UIBlendView>();
            win = null;
        }
    }
}