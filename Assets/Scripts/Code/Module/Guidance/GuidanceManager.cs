using System.Collections.Generic;
using UnityEngine;

namespace TaoTie
{
    public class GuidanceManager: IManager
    {
        public static GuidanceManager Instance;

        public static GameObject GuideTarget;
        public static bool ShowMask;
        private int Group;
        private GuidanceGroupConfig Config => GuidanceConfigCategory.Instance.GetGroup(Group);
        private int CurIndex;
        private GuidanceConfig StepConfig => Config?.Steps[this.CurIndex];
        private Dictionary<string, int> CacheValues;
        #region IManager

        public void Init()
        {
            Instance = this;
            CacheValues = new Dictionary<string, int>();
            CurIndex = -1;
            Group = -1;
        }
        
        public void Destroy()
        {
            Instance = null;
        }

        #endregion
        
        /// <summary>
        /// 服务端通知已完成引导组
        /// </summary>
        /// <param name="doneList"></param>
        public void UpdateGuidanceDone(List<int> doneList)
        {
            if (PlayerManager.Instance.Uid<=0)//根据游戏类型确定按账号还是角色id、存档id
            {
                Log.Error("PlayerComponent.Instance.Account == null");
                return;
            }
            for (int i = 0; i < doneList.Count; i++)
            {
                this.SaveKey(doneList[i], 1);
            }
        }
        
        public void UpdateGuidanceNotDone(List<int> notDoneList)
        {
            if (PlayerManager.Instance.Uid<=0)//根据游戏类型确定按账号还是角色id、存档id
            {
                Log.Error("PlayerComponent.Instance.Account == null");
                return;
            }
            for (int i = 0; i < notDoneList.Count; i++)
            {
                this.SaveKey(notDoneList[i], 0);
            }
        }

        /// <summary>
        /// 检查是否有可开启引导，进游戏检查一次，登录检查一次，完成一个引导检查一次
        /// </summary>
        public void CheckGroupStart()
        {
            if (this.Group >= 0) return;
            for (int i = 0; i < GuidanceConfigCategory.Instance.GetAllGroupList().Count; i++)
            {
                var item = GuidanceConfigCategory.Instance.GetAllGroupList()[i];
                var val = this.GetKey(item.Group);
                if (val == 0 && IsGroupCondition(item.Condition))
                {
                    this.StartGuide(item.Group);
                    return;
                }
            }
        }
        
        
        /// <summary>
        /// 开始引导 todo:登录后获取服务端数据
        /// </summary>
        /// <param name="group"></param>
        public void StartGuide(int group)
        {
            if(this.Group==group) return;
            if (this.Group >= 0)
            {
                if (this.Config.Grouporder < GuidanceConfigCategory.Instance.GetGroup(group).Grouporder)
                {
                    return;
                }
            }

            if (GuidanceConfigCategory.Instance.GetGroup(group) == null)
            {
                Log.Error("引导不存在 "+group);
                return;
            }
            Log.Info("开启引导 "+group);
            this.Group = group;
            for (int i = this.Config.Steps.Count-1; i >=0 ; i--)
            {
                if (this.CheckStepCanRunning(this.Config.Steps[i].Id))
                {
                    this.RunStep(i);
                    return;
                }
            }
            this.RunStep(0);
        }

        /// <summary>
        /// 完成一个事件
        /// </summary>
        /// <param name="evt"></param>
        public void NoticeEvent(string evt)
        {
            if (this.CurIndex >=0 )
            {
                if (this.StepConfig.Event == evt)
                {
                    this.OnStepOver(this.StepConfig.Id);
                    return;
                }

                if (this.StepConfig.Steptype == GuidanceStepType.UIRouter
                    && (evt.StartsWith("Open_") || evt.StartsWith("Close_"))) //路由进行中打开了新界面
                {
                    if (evt != "Open_UIGuidanceView") //打开引导界面忽略
                        this.RunStep(this.CurIndex);
                    return;
                }
            }

        }
        
        /// <summary>
        /// 检查是否可以执行次步骤
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool CheckStepCanRunning(int id)
        {
            var step = GuidanceConfigCategory.Instance.Get(id);
            if (step.Steptype == GuidanceStepType.UIRouter)
            {
                return false;
            }
            if (step.Steptype == GuidanceStepType.FocusGameObject)
            {
                return false;
            }
            if (step.Steptype == GuidanceStepType.WaitEvt)
            {
                return false;
            }
            Log.Error("未处理的类型 Steptype="+step.Steptype);
            return false;
        }
        
        /// <summary>
        /// 完成一个步骤
        /// </summary>
        /// <param name="id"></param>
        private void OnStepOver(int id)
        {
            if (this.CurIndex >=0 && this.StepConfig.Id == id)
            {
                if (this.StepConfig.KeyStep == 1)//关键步骤
                {
                    this.SaveKey(this.Config.Group,1);
                }
                var index = this.CurIndex+1;
                if (index >= this.Config.Steps.Count)
                {
                    this.Stop();
                    return;
                }

                this.RunStep(index);
            }
        }

        /// <summary>
        /// 进行一个步骤
        /// </summary>
        /// <param name="index"></param>
        private void RunStep(int index)
        {
            if (CurIndex >= 0 &&StepConfig!=null && StepConfig.During < 0 && 
                !string.IsNullOrEmpty(I18NManager.Instance.I18NGetText(StepConfig)))
            {
                Messager.Instance.Broadcast<string>(0,MessageId.GuidanceTalk, null);
            }
            this.CurIndex = index;
            if (this.StepConfig.Steptype == GuidanceStepType.UIRouter)
            {
                var win = UIManager.Instance.GetTopWindow(UILayerNames.TopLayer,UILayerNames.TipLayer);
                if (win != null)
                {
                    if (win.Name == StepConfig.Value1)
                    {
                        OnStepOver(this.StepConfig.Id);
                        return;
                    }
                    var config = UIRouterConfigCategory.Instance.GetNextWay(win.Name, this.StepConfig.Value1);
                    if (config != null)
                    {
                        Messager.Instance.Broadcast(0, MessageId.GuidanceTalk,
                            I18NManager.Instance.I18NGetText(StepConfig), StepConfig.During);
                        FocusGameObject(win, config.Path , this.StepConfig.Value3 == "1");
                        return;
                    }
                    else
                    {
                        Log.Info("没找到从{0}到{1}的路径",win.Name, this.StepConfig.Value1);
                    }
                }
            }
            else if (this.StepConfig.Steptype == GuidanceStepType.FocusGameObject)
            {
                var win = UIManager.Instance.GetWindow(this.StepConfig.Value1, 1);
                if (win != null)
                {
                    Messager.Instance.Broadcast(0, MessageId.GuidanceTalk, I18NManager.Instance.I18NGetText(StepConfig),
                        StepConfig.During);
                    FocusGameObject(win, this.StepConfig.Value2, this.StepConfig.Value3 == "1");
                    return;
                }
            }
            else if (this.StepConfig.Steptype == GuidanceStepType.WaitEvt)
            {
                Messager.Instance.Broadcast(0, MessageId.GuidanceTalk, I18NManager.Instance.I18NGetText(StepConfig),
                    StepConfig.During);
            }
            UnFocusGameObject();
        }

        /// <summary>
        /// 停止
        /// </summary>
        private void Stop()
        {
            Log.Info(this.Group + "  引导完成");
            if (CurIndex >= 0 &&StepConfig!=null && StepConfig.During < 0 && 
                !string.IsNullOrEmpty(I18NManager.Instance.I18NGetText(StepConfig)))
            {
                Messager.Instance.Broadcast<string>(0,MessageId.GuidanceTalk, null);
            }
            this.CurIndex = -1;
            this.Group = -1;
            UnFocusGameObject();
            this.CheckGroupStart();
        }

        private int GetKey(int id)
        {
            string key;
            var config = GuidanceConfigCategory.Instance.GetGroup(id);
            if (config.Share != 0)
            {
                key = CacheKeys.Guidance + "_" + id;
            }
            else
            {
                key = CacheKeys.Guidance+"_"+id+"_"+PlayerManager.Instance?.Uid;
            }
            if (!this.CacheValues.TryGetValue(key, out var res))
            {
                this.CacheValues[key] = res;
            }
            return res;
        }
        
        private void SaveKey(int id, int val)
        {
            string key;
            var config = GuidanceConfigCategory.Instance.GetGroup(id);
            if (config.Share == 0)
            {
                key = CacheKeys.Guidance + "_" + id + "_" + PlayerManager.Instance?.Uid;
            }
            else
            {
                key = CacheKeys.Guidance + "_" + id;
            }
            this.CacheValues[key] = val;
            PlayerDataManager.Instance.GuideDown(id);
        }
        
        private void FocusGameObject(UIWindow win, string path, bool showMask)
        {
            if (win == null)
            {
                UnFocusGameObject();
            }
            else
            {
                var view = win.View;
                if (view != null)
                {
                    GuideTarget = win.View.GetRectTransform()?.Find(path)?.gameObject;
                    ShowMask = showMask;
                    if (GuideTarget == null)
                    {
                        Log.Error($"引导物体未找到{win.Name}的{path}");
                    }
                }
            }
        }
        
        private void UnFocusGameObject()
        {
            GuideTarget = null;
        }

        private bool IsGroupCondition(string condition)
        {
            if (condition.StartsWith("GuideScene_"))
            {
                if (!SceneManager.Instance.IsInTargetScene<GuideScene>())
                {
                    return false;
                }
            }

            if (condition.StartsWith("GuideOver_"))
            {
                return int.TryParse(condition[condition.Length - 1].ToString(), out int id) && GetKey(id) == 1;
            }

            return true;
        }
    }
}