using System;
using System.Collections.Generic;
using Nino.Core;
using UnityEngine;

namespace TaoTie
{
    public partial class EnvironmentManager: IManager, IUpdate
    {
        public static EnvironmentManager Instance { get; private set; }
        //游戏世界一天时间，换算成GameTime的总时长，下同
        const int mDayTimeCount = 1200_000;
        //中午开始时间
        const int mNoonTimeStart = 0;
        //晚上开始时间
        const int mNightTimeStart = 800_000;

        private bool isLoad = false;
        
        private PriorityStack<EnvironmentRunner> envInfoStack;
        private Dictionary<long, EnvironmentRunner> envInfoMap;

        private EnvironmentRunner curRunner;
        private EnvironmentInfo preInfo;
        private EnvironmentInfo curInfo;

        public ConfigBlender DefaultBlend;
        
        public int DayTimeCount{ get; private set; }
        public int NoonTimeStart{ get; private set; }
        public int NightTimeStart{ get; private set; }
        public long NowTime{ get; private set; }

        private Dictionary<int, ConfigEnvironment> configs;
        private readonly int SkyDayTexId = Shader.PropertyToID("_SkyDayTex");
        private readonly int SkyNightTexId = Shader.PropertyToID("_SkyNightTex");
        private readonly int SkySunriseTexId = Shader.PropertyToID("_SkySunriseTex");
        private readonly int SkySunsetTexId = Shader.PropertyToID("_SkySunsetTex");

        private Texture SkyDayTex,SkyNightTex,SkySunriseTex,SkySunsetTex;
        
        #region IManager

        public void Init()
        {
            isLoad = false;
            Instance = this;
            DayTimeCount = mDayTimeCount;
            NoonTimeStart = mNoonTimeStart;
            NightTimeStart = mNightTimeStart;
            GameObject obj = new GameObject("DirLight");
            dirLight = obj.AddComponent<Light>();
            dirLight.enabled = false;
            dirLight.type = LightType.Directional;
            GameObject.DontDestroyOnLoad(obj);
        }

        private async ETTask<ConfigEnvironments> GetConfig(string path = "EditConfig/Others/ConfigEnvironments")
        {
            if (Define.ConfigType == 0)
            {
                var jStr = await ResourcesManager.Instance.LoadConfigJsonAsync(path);
                return JsonHelper.FromJson<ConfigEnvironments>(jStr);
            }
            else
            {
                var bytes = await ResourcesManager.Instance.LoadConfigBytesAsync(path);
                return NinoDeserializer.Deserialize<ConfigEnvironments>(bytes);
            }
        }
        public async ETTask LoadAsync()
        {
            #region Config
            
            var config = await GetConfig();
            DefaultBlend = config.DefaultBlend;
            var defaultEnvironmentId = config.DefaultEnvironment.Id;
            configs = new Dictionary<int, ConfigEnvironment>();
            configs.Add(config.DefaultEnvironment.Id,config.DefaultEnvironment);
            if (config.Environments != null)
            {
                for (int i = 0; i < config.Environments.Length; i++)
                {
                    configs.Add(config.Environments[i].Id,config.Environments[i]);
                }
            }
            #endregion

            #region MyRegion

            if(!string.IsNullOrEmpty(config.SkyDayTexPath))
            {
                SkyDayTex = await ResourcesManager.Instance.LoadAsync<Texture>(config.SkyDayTexPath);
                Shader.SetGlobalTexture(SkyDayTexId,SkyDayTex);
            }
            if(!string.IsNullOrEmpty(config.SkyNightTexPath))
            {
                SkyNightTex = await ResourcesManager.Instance.LoadAsync<Texture>(config.SkyNightTexPath);
                Shader.SetGlobalTexture(SkyNightTexId,SkyNightTex);
            }
            if(!string.IsNullOrEmpty(config.SkySunriseTexPath))
            {
                SkySunriseTex = await ResourcesManager.Instance.LoadAsync<Texture>(config.SkySunriseTexPath);
                Shader.SetGlobalTexture(SkySunriseTexId,SkySunriseTex);
            }
            if(!string.IsNullOrEmpty(config.SkySunsetTexPath))
            {
                SkySunsetTex = await ResourcesManager.Instance.LoadAsync<Texture>(config.SkySunsetTexPath);
                Shader.SetGlobalTexture(SkySunsetTexId,SkySunsetTex);
            }
            
            #endregion

            envInfoStack = new PriorityStack<EnvironmentRunner>();
            envInfoMap = new Dictionary<long, EnvironmentRunner>();
            
            NowTime = GameTimerManager.Instance.GetTimeNow();
            Create(defaultEnvironmentId, EnvironmentPriorityType.Default);
            isLoad = true;
        }

        public void Destroy()
        {
            if(SkyDayTex !=null)
            {
                ResourcesManager.Instance.ReleaseAsset(SkyDayTex);
                SkyDayTex = null;
            }
            if(SkyNightTex !=null)
            {
                ResourcesManager.Instance.ReleaseAsset(SkyNightTex);
                SkyNightTex = null;
            }
            if(SkySunriseTex !=null)
            {
                ResourcesManager.Instance.ReleaseAsset(SkySunriseTex);
                SkySunriseTex = null;
            }
            if(SkySunsetTex !=null)
            {
                ResourcesManager.Instance.ReleaseAsset(SkySunsetTex);
                SkySunsetTex = null;
            }
            if (dirLight != null)
            {
                GameObject.Destroy(dirLight.gameObject);
                dirLight = null;
            }
            foreach (var item in envInfoMap)
            {
                item.Value.Dispose();
            }
            envInfoStack = null;
            envInfoMap = null;
            DefaultBlend = null;
            configs = null;
            Instance = null;
        }
        
        public void Update()
        {
            if(!isLoad) return;
            NowTime = GameTimerManager.Instance.GetTimeNow();
            NowTime %= DayTimeCount;
            foreach (var item in envInfoStack.Data)
            {
                if(item.Value == null) continue;
                for (int i = 0; i < item.Value.Count; i++)
                {
                    item.Value[i]?.Update();
                }
            }
            if (envInfoStack.Count == 0) return;

            Process();
            if (curRunner != null)
            {
                ApplyEnvironmentInfo(curRunner.Data);
            }
            else
            {
                ApplyEnvironmentInfo(null);
            }
        }
        #endregion

        private void Process()
        {
            var top = envInfoStack.Peek();
            if (curRunner != top)//栈顶环境变更，需要变换
            {
                if (curRunner == null)
                {
                    curRunner = top;
                    return;
                }
                if (curRunner is BlenderEnvironmentRunner blender) //正在变换
                {
                    envInfoStack.Remove(blender);
                    blender.ChangeTo(top as NormalEnvironmentRunner, false);
                    envInfoStack.Push(blender);
                }
                else//变换到下一个环境
                {
                    while (envInfoStack.Count>0 && envInfoStack.Peek().IsOver)//移除已经over的
                    {
                        envInfoStack.Pop().Dispose();
                    }
                    top = envInfoStack.Peek();
                    if (top is BlenderEnvironmentRunner) //正在变换
                    {
                        return;
                    }

                    if (curRunner != null && top != null && top != curRunner)
                    {
                        blender = CreateRunner(curRunner as NormalEnvironmentRunner,
                            envInfoStack.Peek() as NormalEnvironmentRunner,
                            true);
                        envInfoStack.Push(blender);
                        curRunner = blender;
                    }
                }
            }
            else if (top.IsOver) //播放完毕，需要变换环境
            {
                if (top is BlenderEnvironmentRunner blender) //正在变换
                {
                    envInfoStack.Pop();
                    var newTop = envInfoStack.Peek();
                    if (blender.To.Id == newTop.Id) //是变换完成了
                    {
                        curRunner = envInfoStack.Peek();
                        top.Dispose();
                    }
                    else //变换时，目标环境改变，需要变换到新的环境
                    {
                        blender.ChangeTo(newTop as NormalEnvironmentRunner, false);
                        envInfoStack.Push(blender);
                    }
                }
                else //一般环境被销毁，需要出栈，变换到下一个环境
                {
                    envInfoStack.Pop();
                    while (envInfoStack.Peek().IsOver) //移除已经over的
                    {
                        envInfoStack.Pop().Dispose();
                    }
                    if (envInfoStack.Count <= 0)
                    {
                        top.Dispose();
                        curRunner = null;
                        return;
                    }
                    blender = CreateRunner(top as NormalEnvironmentRunner, envInfoStack.Peek() as
                        NormalEnvironmentRunner, false);
                    envInfoStack.Push(blender);
                    curRunner = blender;
                }
            }
        }
        private void ApplyEnvironmentInfo(EnvironmentInfo info)
        {
            preInfo = curInfo;
            curInfo = info;
            if (preInfo == curInfo && (info == null || !info.Changed)) return;

            ApplySkybox(curInfo);
            ApplyLight(curInfo);
        }

        private partial void ApplySkybox(EnvironmentInfo info);
        private partial void ApplyLight(EnvironmentInfo info);

        private NormalEnvironmentRunner CreateRunner(ConfigEnvironment data, EnvironmentPriorityType type)
        {
            NormalEnvironmentRunner runner = NormalEnvironmentRunner.Create(data, type, this);
            envInfoMap.Add(runner.Id,runner);
            if (curRunner == null) curRunner = runner;
            return runner;
        }
        
        private BlenderEnvironmentRunner CreateRunner(NormalEnvironmentRunner from, NormalEnvironmentRunner to,
            bool isEnter)
        {
            BlenderEnvironmentRunner runner = BlenderEnvironmentRunner.Create(from, to, isEnter, this);
            envInfoMap.Add(runner.Id,runner);
            return runner;
        }
        
        private DayEnvironmentRunner CreateRunner(ConfigEnvironment noon, ConfigEnvironment night,EnvironmentPriorityType priority)
        {
            DayEnvironmentRunner runner = DayEnvironmentRunner.Create(noon,night,priority,this);
            envInfoMap.Add(runner.Id,runner);
            if (curRunner == null) curRunner = runner;
            return runner;
        }
        
        /// <summary>
        /// 创建环境
        /// </summary>
        /// <param name="id"></param>
        /// <param name="priority"></param>
        public long Create(int id, EnvironmentPriorityType priority)
        {
            var data = Get(id);
            var res = CreateRunner(data, priority);
            envInfoStack.Push(res);
            return res.Id;
        }

        /// <summary>
        /// 创建日夜循环环境
        /// </summary>
        public long CreateDayNight(int noonId, int nightId,
            EnvironmentPriorityType priority = EnvironmentPriorityType.DayNight)
        {
            var noon = Get(noonId);
            var night = Get(nightId);
            var res = CreateRunner(noon, night, priority);
            envInfoStack.Push(res);
            return res.Id;
        }

        /// <summary>
        /// 移除环境
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Remove(long id)
        {
            if (envInfoMap.TryGetValue(id, out var info) && !info.IsOver)
            {
                info.IsOver = true;
                
                //非生效环境
                if (curRunner is BlenderEnvironmentRunner blender)
                {
                    if (blender.To.Id != info.Id)
                    {
                        envInfoStack.Remove(info);
                        info.Dispose();
                    }
                }
                else if(curRunner.Id != info.Id)
                {
                    envInfoStack.Remove(info);
                    info.Dispose();
                }
                else if(envInfoStack.Count == 1)
                {
                    envInfoStack.Remove(info);
                    info.Dispose();
                    curRunner = null;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 从索引中删除，请不要手动调用
        /// </summary>
        /// <param name="id"></param>
        public void RemoveFromMap(long id)
        {
            envInfoMap.Remove(id);
        }

        private ConfigEnvironment Get(int id)
        {
            this.configs.TryGetValue(id, out ConfigEnvironment item);

            if (item == null)
            {
                throw new Exception($"配置找不到，配置表名: {nameof (ConfigEnvironment)}，配置id: {id}");
            }

            return item;
        }
    }
}