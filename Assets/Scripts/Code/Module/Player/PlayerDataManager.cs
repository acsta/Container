using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TaoTie
{
    public class PlayerDataManager: IManager,ILateUpdate
    {
        [Timer(TimerType.DailyRefresh)]
        public class DailyRefreshTimer : ATimer<PlayerDataManager>
        {
            public override void Run(PlayerDataManager self)
            {
                try
                {
                    bool change = false;
                    if (self.DailyRefresh())
                    {
                        change = true;
                    }
                    if (change)
                    {
                        self.SaveData();
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"move timer error: DailyRefresh\n{e}");
                }
            }
        }
        
        public static PlayerDataManager Instance;
        private PlayerData data;

        /// <summary>
        /// 开始刷盘子加倍时间
        /// </summary>
        public long StartAddTime;
        public BigNumber TotalMoney => data?.Money??0;
        public int RestaurantLv => data?.RestaurantLv??0;
        public int[] Show => data?.Show;

        public bool IsGuideScene => data?.IsGuideScene ?? false;
        public int LastLevelId => data?.LastLevelId??0;
        public long LastRefreshDailyTime => data.LastRefreshDailyTime;
        public string Avatar => data?.Avatar;
        public string NickName => data?.NickName;
        public int PlayableCount=> data?.PlayableCount??0;
        public int TurnTableCount=> data?.TurnTableCount??0;

        public bool WashDishAuto => data?.WashDishAuto ?? false;

        private int MaxAdRewardsCount;
        private int AdRewardsResetTime;
        private int ProfitRedDotPercent;
        
        /// <summary>
        /// 餐厅利润时变动间单位（每次界面上进度条变化并且飘字的时间间隔）（ms）
        /// </summary>
        public int ProfitUpdateUnitTime { get; private set; }
        /// <summary>
        /// 每日触发刷新时间（h）
        /// </summary>
        public int DailyRefreshHour { get; private set; }
        /// <summary>
        /// 每日任务刷新保底稀有度
        /// </summary>
        public int DayTaskRefreshRare{ get; private set; }
        /// <summary>
        /// 每日刷新时装数量
        /// </summary>
        public int ClothRefreshCount { get; private set; }

        private long timerId;
        private bool needUpload = false;
        public void Init()
        {
            Instance = this;
            MaxAdRewardsCount = GlobalConfigCategory.Instance.GetInt("MaxAdRewardsCount", 10);
            AdRewardsResetTime = GlobalConfigCategory.Instance.GetInt("AdRewardsResetTime", 6);
            ProfitRedDotPercent = GlobalConfigCategory.Instance.GetInt("ProfitRedDotPercent", 90);
            GameConst.ProfitUnitTime= GlobalConfigCategory.Instance.GetInt("ProfitUnit", 3600000);
            GameConst.ProfitUnitShowTime= GlobalConfigCategory.Instance.GetInt("ProfitUnitShow", 60000);
            ProfitUpdateUnitTime = GlobalConfigCategory.Instance.GetInt("ProfitUpdateUnit", 5000);
            DayTaskRefreshRare = GlobalConfigCategory.Instance.GetInt("DayTaskRefreshRare", 4);
            DailyRefreshHour = GlobalConfigCategory.Instance.GetInt("DailyRefreshHour", 6);
            ClothRefreshCount = GlobalConfigCategory.Instance.GetInt("ClothRefreshCount", 6);
            
            GlobalConfigCategory.Instance.TryGetColor("TaskItemColor", out GameConst.TaskItemColor);
            GlobalConfigCategory.Instance.TryGetColor("PlayableItemColor", out GameConst.PlayableItemColor);
            GlobalConfigCategory.Instance.TryGetColor("ItemPriceColor", out GameConst.ItemPriceColor);
            if (!GlobalConfigCategory.Instance.TryGetArray("TurnTablePercent", out GameConst.TurnTablePercent))
            {
                GameConst.TurnTablePercent = new int[] {50, 10, 5};
            }
            if (!GlobalConfigCategory.Instance.TryGetArray("TurnTableAdCount", out GameConst.TurnTableAdCount))
            {
                GameConst.TurnTableAdCount = new int[] {0, 1, 1, 2};
            }
            if (!GlobalConfigCategory.Instance.TryGetArray("PlayableAdCount", out GameConst.PlayableAdCount))
            {
                GameConst.PlayableAdCount = new int[] {1, 1, 2, 2, 3, 3,};
            }
            GameConst.PlayableMaxAdCount = GlobalConfigCategory.Instance.GetInt("PlayableMaxAdCount", GameConst.PlayableAdCount.Length);
            if (!GlobalConfigCategory.Instance.TryGetArray("OpenStageInterval", out GameConst.OpenStageInterval))
            {
                GameConst.OpenStageInterval = new int[] {1500, 1500, 1000, 1000};
            }
        }
        
        public void Destroy()
        {
            TimerManager.Instance.Remove(ref timerId);
            Instance = null;
        }

        public void LateUpdate()
        {
            if (needUpload)
            {
                if (PlayerManager.Instance.OnLine)
                {
                    APIManager.Instance.SaveData(PlayerManager.Instance.Uid, data).Coroutine();
                }

                needUpload = false;
            }
        }

        public void AfterLogin(PlayerData sdata,string nick,string avatar,LoginPlatform platform)
        {
            this.data = sdata;
            bool change = false;
            if (data == null)
            {
                data = new PlayerData();
                
                if (GlobalConfigCategory.Instance.TryGetString("InitialMoney", out var money))
                {
                    data.Money = money;
                }
                change = true;
                
            }

            data.Platform = platform;
            if (!string.IsNullOrEmpty(nick) && nick != data.NickName)
            {
                data.NickName = nick;
                change = true;
            }
            if (!string.IsNullOrEmpty(avatar) && avatar != data.Avatar)
            {
                data.Avatar = avatar;
                change = true;
            }
            var minLv = int.MaxValue;
            var lst = RestaurantConfigCategory.Instance.GetAllList();
            for (int i = 0; i < lst.Count; i++)
            {
                if (lst[i].Level < minLv)
                {
                    minLv = lst[i].Level;
                }
            }

            if (data.RestaurantLv < minLv)
            {
                data.RestaurantLv = minLv;
                change = true;
            }

            if (data.UnlockTechnologyTreeIds == null)
            {
                data.UnlockTechnologyTreeIds = new HashSet<int>();
            }

            if (data.RunningTaskSteps == null)
            {
                data.RunningTaskSteps = new Dictionary<int, int>();
            }

            if (data.OverTaskCount == null)
            {
                data.OverTaskCount = new Dictionary<int, int>();
            }

            if (data.DailyTaskIds == null)
            {
                data.DailyTaskIds = new List<int>();
            }

            if (data.DailyRewards == null)
            {
                data.DailyRewards = new HashSet<int>();
            }

            if (data.ItemCount == null)
            {
                data.ItemCount = new Dictionary<int, int>();
            }

            if (data.PlayCount == null)
            {
                data.PlayCount = new Dictionary<int, int>();
            }

            if (data.UnlockList == null)
            {
                data.UnlockList = new List<int>();
            }

            if (data.OverGuide == null)
            {
                data.OverGuide = new List<int>();
            }
            
            if (data.Show == null)
            {
                var configs = CharacterConfigCategory.Instance.GetAllList();
                data.Show = new int[configs.Count];
                for (int i = 0; i < configs.Count; i++)
                {
                    if (configs[i].DefaultCloth != 0)
                    {
                        data.Show[i] = configs[i].DefaultCloth;
                        data.ItemCount[configs[i].DefaultCloth] = 1;
                        change = true;
                    }
                }
            }
            else
            {
                var configs = CharacterConfigCategory.Instance.GetAllList();
                for (int i = 0; i < configs.Count; i++)
                {
                    if (configs[i].DefaultCloth != 0 &&
                        (!data.ItemCount.TryGetValue(configs[i].DefaultCloth, out var count) || count <= 0))
                    {
                        data.ItemCount[configs[i].DefaultCloth] = 1;
                        change = true;
                    }
                }
            }

#if UNITY_EDITOR
            if (GetItemCount(GameConst.DiceItemId) <= 0)
            {
                data.ItemCount[GameConst.DiceItemId] = 1;
            }
#endif
            DailyRefresh();
            
            RefreshRedDot();
            if (change)
            {
                SaveData();
            }

            Messager.Instance.AddListener(0, MessageId.SetProfit, SetProfit);
            Messager.Instance.AddListener<int>(0,MessageId.AddMoney,OnClickAdd);
            Messager.Instance.AddListener(0,MessageId.OpenTurntable,OpenTurntable);
            Messager.Instance.AddListener(0,MessageId.UnlockAllCloth,UnlockAllCloth);
            Messager.Instance.AddListener(0,MessageId.EnterGuideScene,EnterGuideScene);
#if !UNITY_EDITOR
            if (data.IsGuideScene)
            {
                GuidanceManager.Instance.UpdateGuidanceDone(data.OverGuide);
            }
#endif
            GuidanceManager.Instance.CheckGroupStart();
        }

        public void UpdateUserInfo(string nick,string avatar)
        {
            bool change = false;
            if (!string.IsNullOrEmpty(nick) && nick != data.NickName)
            {
                data.NickName = nick;
                change = true;
            }
            if (!string.IsNullOrEmpty(avatar) && avatar != data.Avatar)
            {
                data.Avatar = avatar;
                change = true;
            }

            if (change)
            {
                SaveData();
            }
        }
        
        private void SaveData()
        {
            data.Version = TimerManager.Instance.GetTimeNow();
            needUpload = true;
            CacheManager.Instance.SetValue(CacheKeys.PlayerData, data);
            CacheManager.Instance.Save();
        }
        
        #region GM

        private void SetProfit()
        {
            if (data != null && data.RestaurantLv > 0 && data.WashDishAuto)
            {
                var timeNow = TimerManager.Instance.GetTimeNow();
                var max = GetMaxDeltaTime();
                data.LastReceiveRestaurantTime = timeNow - max * TimeInfo.Hour * ProfitRedDotPercent / 100;
                if (max > 0 && (timeNow - data.LastReceiveRestaurantTime) * 100 >= max * TimeInfo.Hour * ProfitRedDotPercent)
                {
                    RedDotManager.Instance.RefreshRedDotViewCount("Restaurant_TimeOut", 1);
                }
                else
                {
                    RedDotManager.Instance.RefreshRedDotViewCount("Restaurant_TimeOut", 0);
                }
            }
        }
        private void OnClickAdd(int num)
        {
            Instance.ChangeMoney(num);
        }

        private void OpenTurntable()
        {
            UIManager.Instance
                .OpenWindow<UITurnTableEventView, BigNumber, int>(UITurnTableEventView.PrefabPath, 10000000, 1,
                    UILayerNames.TipLayer).Coroutine();
        }

        private void UnlockAllCloth()
        {
            if (data == null) return;
            var list = ClothConfigCategory.Instance.GetAllList();
            for (int i = 0; i < list.Count; i++)
            {
                if (!data.ItemCount.TryGetValue(list[i].Id, out var count) || count <= 0)
                {
                    data.ItemCount[list[i].Id] = 1;
                }
            }
            SaveData();
        }

        private void EnterGuideScene()
        {
            GuidanceManager.Instance.UpdateGuidanceNotDone(data.OverGuide);
            SceneManager.Instance.SwitchScene<GuideScene>().Coroutine();
        }
        
        

        #endregion

        #region 刷新
        private void RefreshRedDot()
        {
            RefreshRestaurantProfitRedDot();
            if (data.RestaurantLv > 0)
            {
                RefreshDailyTaskRedDot();
            }

            foreach (var item in data.ItemCount)
            {
                RedDotManager.Instance.RefreshRedDotViewCount("Item_"+item.Key, item.Value);
            }
            RefreshTechRedDot();
            if (CanGotSidebarRewards())
            {
                RedDotManager.Instance.RefreshRedDotViewCount("Sidebar", 1);
            }
            
            var conf = RestaurantConfigCategory.Instance.GetByLv(RestaurantLv, out _);
            if (conf.Need > 0 && !data.IsGotWinRewards)
            {
                RedDotManager.Instance.RefreshRedDotViewCount("DailyTask", 1);
            }
        }

        /// <summary>
        /// 刷新餐厅红点
        /// </summary>
        public void RefreshRestaurantProfitRedDot()
        {
            if (data != null && data.RestaurantLv > 0 && data.WashDishAuto)
            {
                var timeNow = TimerManager.Instance.GetTimeNow();
                var max = GetMaxDeltaTime();
                if (max > 0 && (timeNow - data.LastReceiveRestaurantTime) * 100 >= max * TimeInfo.Hour * ProfitRedDotPercent)
                {
                    RedDotManager.Instance.RefreshRedDotViewCount("Restaurant_TimeOut", 1);
                }
                else
                {
                    RedDotManager.Instance.RefreshRedDotViewCount("Restaurant_TimeOut", 0);
                }
            }
        }
        private bool DailyRefresh()
        {
            var timeNow = TimerManager.Instance.GetTimeNow();
            var dateNow = TimeInfo.Instance.ToDateTime(timeNow - TimeInfo.Hour * this.DailyRefreshHour).Date;
            if (timeNow - data.LastRefreshDailyTime < TimeInfo.OneDay)
            {
                var dateYesterday = TimeInfo.Instance.ToDateTime(data.LastRefreshDailyTime - TimeInfo.Hour * this.DailyRefreshHour).Date;
                if (dateNow == dateYesterday)
                {
                    return false;
                }
            }
            data.LastRefreshDailyTime = TimeInfo.Instance.Transition(dateNow) + TimeInfo.Hour * this.DailyRefreshHour;
            TimerManager.Instance.Remove(ref timerId);
            var time = data.LastRefreshDailyTime + TimeInfo.OneDay;
            var deltaTime = time - timeNow;
            timerId = TimerManager.Instance.NewOnceTimer(deltaTime, TimerType.DailyRefresh, this);
            Log.Info("触发每日刷新");
            data.TurnTableCount = 0;
            data.PlayableCount = 0;
            RefreshDailyTask(true);
            RefreshCloth(true);
            RefreshWinRewards();
            return true;
        }

        /// <summary>
        /// 是否能领取
        /// </summary>
        /// <returns></returns>
        public bool CanGotSidebarRewards()
        {
            var lastGotTime = data.LastSidebarRewards;
            var timeNow = TimerManager.Instance.GetTimeNow();
            var dateNow = TimeInfo.Instance.ToDateTime(timeNow - TimeInfo.Hour * this.DailyRefreshHour).Date;
            if (timeNow - lastGotTime < TimeInfo.OneDay)
            {
                var dateYesterday = TimeInfo.Instance.ToDateTime(lastGotTime - TimeInfo.Hour * this.DailyRefreshHour).Date;
                if (dateNow == dateYesterday)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 领取侧边栏奖励
        /// </summary>
        public void GetSidebarRewards()
        {
            data.LastSidebarRewards = TimerManager.Instance.GetTimeNow();
            SaveData();
        }

        #endregion

        #region 科技树

        private void RefreshTechRedDot()
        {
            foreach (var level in TechnologyTreeConfigCategory.Instance.GetLevels())
            {
                int count = 0;
                if (IsUnlock(level.Id))
                {
                    foreach (var container in TechnologyTreeConfigCategory.Instance.GetContainers(level.Id))
                    {
                        if (IsUnlock(container.Id))
                        {
                            foreach (var playType in TechnologyTreeConfigCategory.Instance.GetPlayTypes(container.Id))
                            {
                                if (!IsUnlock(playType.Id) && TotalMoney >= playType.UnlockValue)
                                {
                                    count++;
                                }
                            }
                        }
                        else
                        {
                            if (TotalMoney >= container.UnlockValue)
                            {
                                count++;
                            }
                        }

                        if (count > 0)
                        {
                            break;
                        }
                    }
                }
                RedDotManager.Instance.RefreshRedDotViewCount("Black_Tech_" + level.Id, count);
            }
        }
        /// <summary>
        /// 获取已解锁的指定场次集装箱Id
        /// </summary>
        /// <param name="level"></param>
        /// <param name="normal"></param>
        /// <param name="special"></param>
        /// <returns></returns>
        public void GetUnlockContainerIds(int level, List<int> normal, List<int> special)
        {
            var tg = TechnologyTreeConfigCategory.Instance.GetContainers(level);
            int allPlayId = -1;
            for (int i = 0; i < tg.Count; i++)
            {
                if (tg[i].UnlockType == 0)
                {
                    //默认解锁
                }
                else if (tg[i].UnlockType == 1)
                {
                    if (!data.UnlockTechnologyTreeIds.Contains(tg[i].Id))
                    {
                        //未解锁
                        continue;
                    }
                }
                else
                {
                    continue;
                }

                var container = ContainerConfigCategory.Instance.Get(tg[i].Content);
                if (container == null)
                {
                    continue;
                }

                if (container.Type == 1)
                {
                    normal?.Add(container.Id);
                }
                else
                {
                    if (container.Type == 0)
                    {
                        allPlayId = container.Id;
                    }
                    else
                    {
                        special?.Add(container.Id);
                    }
                }

                if (allPlayId > 0 && special != null && special.Count > 0)
                {
                    special.Add(allPlayId);
                }
            }
        }

        /// <summary>
        /// 获取已解锁小玩法Id
        /// </summary>
        /// <param name="containerId"></param>
        /// <returns></returns>
        public ListComponent<ItemType> GetUnlockMiniPlayIds(int containerId)
        {
            ListComponent<ItemType> res = ListComponent<ItemType>.Create();
            var pt = TechnologyTreeConfigCategory.Instance.GetPlayTypes(containerId);
            for (int i = 0; i < pt.Count; i++)
            {
                if (pt[i].UnlockType == 0)
                {
                    //默认解锁
                }
                else if (pt[i].UnlockType == 1)
                {
                    if (!data.UnlockTechnologyTreeIds.Contains(pt[i].Id))
                    {
                        //未解锁
                        continue;
                    }
                }
                else
                {
                    continue;
                }
                res.Add((ItemType) pt[i].Content);
            }
            return res;
        }

        /// <summary>
        /// 获取指定节点是否已经解锁
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsUnlock(int id)
        {
            var config = TechnologyTreeConfigCategory.Instance.Get(id);
            if (config.UnlockType == 0)
            {
                //默认解锁
                return true;
            }
            if (config.UnlockType == 1)
            {
                return data.UnlockTechnologyTreeIds.Contains(config.Id);
            }
            if (config.UnlockType == 2)
            {
                if (config.Level == 0)
                {
                    return GetPlayCount(config.Id) > 0;
                }
                if (config.Level == 1)
                {
                    return GetPlayCount(config.ParentId) > 0;
                }
                if (config.Level == 2)
                {
                    var parent = TechnologyTreeConfigCategory.Instance.Get(config.ParentId);
                    return GetPlayCount(parent.ParentId) > 0;
                }
            }
            return false;
        }
        
        /// <summary>
        /// 获取指定节点是否已经解锁
        /// </summary>
        /// <param name="containerId"></param>
        /// <returns></returns>
        public bool IsUnlockContainer(int containerId)
        {
            var container = ContainerConfigCategory.Instance.Get(containerId);
            var tree = TechnologyTreeConfigCategory.Instance.GetContainers(container.Level);
            for (int i = 0; i < tree.Count; i++)
            {
                if (tree[i].Content == containerId)
                {
                    if (tree[i].UnlockType == 0)
                    {
                        return true;
                        //默认解锁
                    }
                    else if (tree[i].UnlockType == 1)
                    {
                        if (data.UnlockTechnologyTreeIds.Contains(tree[i].Id))
                        {
                            //解锁
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        
        /// <summary>
        /// 解锁指定节点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool UnlockTreeNode(int id)
        {
            var config = TechnologyTreeConfigCategory.Instance.Get(id);
            if (config.UnlockType == 1&& !data.UnlockTechnologyTreeIds.Contains(id))
            {
                if (TotalMoney >= config.UnlockValue)
                {
                    data.UnlockTechnologyTreeIds.Add(config.Id);
                    ChangeMoney(-config.UnlockValue);
                    for (int i = 0; i < data.UnlockList.Count; i++)
                    {
                        if (data.UnlockList[i] == config.ParentId)
                        {
                            data.UnlockList[i] = id;
                            Log.Info(JsonHelper.ToJson(data.UnlockList));
                            SaveData();
                            return true;
                        }
                    }
                    data.UnlockList.Add(id);
                    Log.Info(JsonHelper.ToJson(data.UnlockList));
                    SaveData();
                    return true;
                }
                UIManager.Instance.OpenBox<UIToast,I18NKey>(UIToast.PrefabPath,I18NKey.Notice_Common_LackOfMoney).Coroutine();
            }
            return false;
        }

        /// <summary>
        /// 获取刚解锁的玩法
        /// </summary>
        /// <returns></returns>
        public List<int> GetUnlockList()
        {
            return data.UnlockList;
        }

        /// <summary>
        /// 设置刚解锁的玩法已完成
        /// </summary>
        /// <param name="containerId"></param>
        /// <param name="playType"></param>
        public void RemoveUnlockList(int containerId, ItemType playType)
        {
            if (playType == ItemType.None)
            {
                data.UnlockList.Remove(containerId);
            }
            else
            {
                var list = TechnologyTreeConfigCategory.Instance.GetPlayTypes(containerId);
                foreach (var config in list)
                {
                    if (config.Content == (int)playType)
                    {
                        data.UnlockList.Remove(config.Id);
                    }
                }
            }
            SaveData();
        }
        #endregion

        #region 广告
        
        /// <summary>
        /// 是否可以看广告领金币
        /// </summary>
        /// <returns></returns>
        public bool CanShowAdRewardsMoney()
        {
            if (AdManager.Instance.PlatformHasAD())
            {
                if (MaxAdRewardsCount < 0) return true;
                var timeNow = TimeInfo.Instance.ToDateTime(TimerManager.Instance.GetTimeNow()).AddHours(-AdRewardsResetTime);
                var lastTime = TimeInfo.Instance.ToDateTime(data.LastAdTime).AddHours(-AdRewardsResetTime);
                if (lastTime.ToString("yyyy-MM-dd") != timeNow.ToString("yyyy-MM-dd"))
                {
                    return true;
                }
                return data.AdCountTotal < MaxAdRewardsCount;
            }
            return false;
        }

        /// <summary>
        /// 广告奖励金币
        /// </summary>
        public bool AdRewardsMoney(int money)
        {
            var timeNow = TimeInfo.Instance.ToDateTime(TimerManager.Instance.GetTimeNow()).AddHours(-AdRewardsResetTime);
            var lastTime = TimeInfo.Instance.ToDateTime(data.LastAdTime).AddHours(-AdRewardsResetTime);
            if (lastTime.ToString("yyyy-MM-dd") != timeNow.ToString("yyyy-MM-dd"))
            {
                data.AdCountTotal = 1;
            }
            else
            {
                if(data.AdCountTotal >= MaxAdRewardsCount) return false;
                data.AdCountTotal++;
            }
            data.LastAdTime = TimerManager.Instance.GetTimeNow();
            ChangeMoney(money);
            return true;
        }

        /// <summary>
        /// 随机大转盘
        /// </summary>
        /// <returns></returns>
        public bool RandomTurnTable()
        {
            var prop = Mathf.Min(data.TurnTableCount, GameConst.TurnTablePercent.Length - 1);
            if (prop < 0) return false;
            var random = Random.Range(0, 100);
            if (random < GameConst.TurnTablePercent[prop])
            {
                var index = Mathf.Min(data.TurnTableCount, GameConst.TurnTableAdCount.Length - 1);
                var adCount = GameConst.TurnTableAdCount[index];
                return adCount <= 0 || AdManager.Instance.PlatformHasAD();
            }

            return false;
        }

        /// <summary>
        /// 获取本次需要看广告次数
        /// </summary>
        /// <returns></returns>
        public int GetTurnTableNeedAdCount()
        {
            var index = Mathf.Min(data.TurnTableCount, GameConst.TurnTableAdCount.Length - 1);
            return GameConst.TurnTableAdCount[index];
        }

        public void OnTurnTableAd()
        {
            data.TurnTableCount++;
            SaveData();
        }

        /// <summary>
        /// 获取玩法需要看广告次数
        /// </summary>
        /// <returns></returns>
        public int GetPlayableAdCount()
        {
            if (data.PlayableCount < GameConst.PlayableAdCount.Length)
            {
                return GameConst.PlayableAdCount[data.PlayableCount];
            }

            return GameConst.PlayableAdCount[GameConst.PlayableAdCount.Length - 1];
        }

        /// <summary>
        /// 记录玩法看广告
        /// </summary>
        public void OnPlayableAd()
        {
            data.PlayableCount++;
            SaveData();
        }
        #endregion

        #region 任务

        /// <summary>
        /// 获取正在进行中任务（餐厅等级任务+超市每日任务）
        /// </summary>
        /// <returns></returns>
        public ListComponent<int> GetRunningTaskIds()
        {
            ListComponent<int> res = ListComponent<int>.Create();
            res.AddRange(GetDailyTaskIds());
            return res;
        }
        
        /// <summary>
        /// 获取任务状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="step">进度</param>
        /// <returns>是否完成</returns>
        public bool GetTaskState(int id, out int step)
        {
            var task = TaskConfigCategory.Instance.Get(id);
            bool isRunning = data.RunningTaskSteps.TryGetValue(id, out step);
            if (task.ItemId == GameConst.MoneyItemId)
            {
                step = TotalMoney >= task.ItemCount? task.ItemCount : (int) TotalMoney;
            }
            if (isRunning)
            {
                return false;
            }
            if (data.OverTaskCount.TryGetValue(id, out var count) && count > 0)
            {
                step = task.ItemCount;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 更新任务进度
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="count"></param>
        /// <param name="type">物品类型0物品1集装箱</param>
        public void AddTaskStep(int itemId, int count, int type)
        {
            using var taskIds = GetRunningTaskIds();
            bool change = false;
            foreach (var taskId in taskIds)
            {
                var config = TaskConfigCategory.Instance.Get(taskId);
                if (type == config.ItemType && config.ItemId == itemId)
                {
                    if (data.OverTaskCount.TryGetValue(taskId, out var overCount) && overCount > 0)
                    {
                        continue;
                    }
                    data.RunningTaskSteps.TryGetValue(taskId, out var oldV);
                    data.RunningTaskSteps[taskId] = oldV + count;
                    change = true;
                }
            }

            if (change)
            {
                RefreshDailyTaskRedDot();
                Messager.Instance.Broadcast(0,MessageId.UpdateTaskStep);
                SaveData();
            }
        }

        /// <summary>
        /// 提交任务
        /// </summary>
        /// <param name="taskId"></param>
        public bool ComplexTask(int taskId)
        {
            var config = TaskConfigCategory.Instance.Get(taskId);
            if (data.RunningTaskSteps.TryGetValue(taskId, out var value))
            {
                if (value >= config.ItemCount)
                {
                    if (config.RewardType == 2)
                    {
                        ChangeMoney(config.RewardCount);
                    }
                    else if (config.RewardType == 1)
                    {
                        //金币产出效率
                    }
                    else
                    {
                        Log.Error("未处理的奖励类型");
                        return false;
                    }
                    data.RunningTaskSteps.Remove(taskId);
                    data.OverTaskCount.TryGetValue(taskId, out var oldV);
                    data.OverTaskCount[taskId] = oldV + 1;
                    SaveData();
                    RefreshDailyTaskRedDot();
                    Messager.Instance.Broadcast(0,MessageId.ComplexTask);
                    return true;
                }
            }
            if (config.ItemId == GameConst.MoneyItemId)
            {
                if (TotalMoney >= config.ItemCount)
                {
                    data.RunningTaskSteps.Remove(taskId);
                    data.OverTaskCount.TryGetValue(taskId, out var oldV);
                    data.OverTaskCount[taskId] = oldV + 1;
                    ChangeMoney(-config.ItemCount);
                    RefreshDailyTaskRedDot();
                    Messager.Instance.Broadcast(0,MessageId.ComplexTask);
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region 餐厅

        /// <summary>
        /// 解锁自动洗盘子
        /// </summary>
        public void UnlockAutoWash()
        {
            data.WashDishAuto = true;
            data.LastReceiveRestaurantTime = TimerManager.Instance.GetTimeNow();
            SaveData();
        }

        /// <summary>
        /// 扩容
        /// </summary>
        public void Expand()
        {
            data.ExpandTimes++;
            SaveData();
        }
        /// <summary>
        /// 升级
        /// </summary>
        /// <returns></returns>
        public bool LevelUp()
        {
            var config = RestaurantConfigCategory.Instance.GetByLv(RestaurantLv, out var next);
            if (config == null) return false;
            if (TotalMoney < config.Cost)
            {
                UIManager.Instance.OpenBox<UIToast, I18NKey>(UIToast.PrefabPath, I18NKey.Notice_Common_LackOfMoney).Coroutine();
                return false;
            }

            // 初始满金币
            // if (data.RestaurantLv == 0)
            // {
            //     data.LastReceiveRestaurantTime = TimerManager.Instance.GetTimeNow();
            // }
            var oldLv = GetMaxLevel();
            data.RestaurantLv += 1;
            var newLv = GetMaxLevel();
            if (newLv != oldLv)
            {
                data.LastLevelId = newLv;
                RedDotManager.Instance.RefreshRedDotViewCount("Black_Tags_" + newLv, 1);
            }

            if (config.UnlockMarket == 0 && next?.UnlockMarket == 1)
            {
                RefreshDailyTask(true);
            }
            if (config.UnlockCloth == 0 && next?.UnlockCloth == 1)
            {
                RefreshCloth(true);
            }

            if (config.Need == 0 && next?.Need != 0)
            {
                RefreshWinRewards();
            }

            ChangeMoney(-config.Cost);
            return true;
        }

        /// <summary>
        /// 获取配置表单位时间收益
        /// </summary>
        /// <returns></returns>
        public BigNumber CalculateProfitUnit()
        {
            var config = RestaurantConfigCategory.Instance.GetByLv(RestaurantLv, out _);
            BigNumber value = config.CashRestaurant;
            return value;
        }
        
        /// <summary>
        /// 获取更新单位时间收益(不精确！会有小数，只能用作展示，请使用CalculateProfitUnit最后* ProfitUpdateUnitTime / GameConst.ProfitUnitTime计算)
        /// </summary>
        /// <returns></returns>
        public BigNumber CalculateProfitUpdateUnit()
        {
            BigNumber profitUnit = CalculateProfitUnit();
            return profitUnit * ProfitUpdateUnitTime / GameConst.ProfitUnitTime;
        }
        /// <summary>
        /// 获取最高收益积累时间（h）
        /// </summary>
        /// <returns></returns>
        public long GetMaxDeltaTime()
        {
            var config = RestaurantConfigCategory.Instance.GetByLv(RestaurantLv, out _);
            long value = (config.MaxHourLimitRestaurant + data.ExpandTimes);
            return value;
        }
        /// <summary>
        /// 计算收益
        /// </summary>
        /// <returns></returns>
        public BigNumber CalculateProfit()
        {
            if (!data.WashDishAuto) return 0;
            var moneyPreUpdate = CalculateProfitUnit();
            var timeNow = TimerManager.Instance.GetTimeNow();
            if (moneyPreUpdate <= 0)
            {
                return BigNumber.Zero;
            }
            var deltaTime = timeNow - data.LastReceiveRestaurantTime;
            var max = GetMaxDeltaTime() * TimeInfo.Hour;
            int count = (int)Mathf.Min(deltaTime,max) / ProfitUpdateUnitTime;
            return count * moneyPreUpdate * ProfitUpdateUnitTime / GameConst.ProfitUnitTime;
        }

        /// <summary>
        /// 收取收益
        /// </summary>
        /// <param name="ratio">收取倍率</param>
        /// <returns></returns>
        public BigNumber GetProfit(int ratio = 1)
        {
            if (!data.WashDishAuto) return 0;
            var money = CalculateProfit();
            var timeNow = TimerManager.Instance.GetTimeNow();
            if (money <= 0)
            {
                data.LastReceiveRestaurantTime = timeNow;
            }
            else
            {
                var deltaTime = timeNow - data.LastReceiveRestaurantTime;
                var max = GetMaxDeltaTime() * TimeInfo.Hour;
                if (deltaTime > max)
                {
                    data.LastReceiveRestaurantTime = timeNow;
                }
                else
                {
                    var count = deltaTime / ProfitUpdateUnitTime;
                    data.LastReceiveRestaurantTime += count * ProfitUpdateUnitTime;
                }
            }
            ChangeMoney(money*ratio);
            RedDotManager.Instance.RefreshRedDotViewCount("Restaurant_TimeOut", 0);
            return money*ratio;
        }

        /// <summary>
        /// 计算最高暂存利润
        /// </summary>
        /// <returns></returns>
        public BigNumber CalculateMaxStageProfit()
        {
            var moneyPreUpdate = CalculateProfitUnit();
            if (moneyPreUpdate <= 0)
            {
                return BigNumber.Zero;
            }
            var max = GetMaxDeltaTime() * TimeInfo.Hour;
            var count = max / ProfitUpdateUnitTime;
            return count * moneyPreUpdate * ProfitUpdateUnitTime / GameConst.ProfitUnitTime;
        }

        /// <summary>
        /// 获取利润进度
        /// </summary>
        /// <returns></returns>
        public float GetProfitProgress()
        {
            if (!data.WashDishAuto) return 0;
            float max = GetMaxDeltaTime() * TimeInfo.Hour;
            var timeNow = TimerManager.Instance.GetTimeNow();
            var last = data.LastReceiveRestaurantTime;
            return Mathf.Clamp01((timeNow - last) / max);
        }
        #endregion

        #region 超市

        private void RefreshDailyTaskRedDot()
        {
            int total1 = 0;
            int total2 = 0;
            int overTaskCount = 0;
            for (int i = 0; i < data.DailyTaskIds.Count; i++)
            {
                var config = TaskConfigCategory.Instance.Get(data.DailyTaskIds[i]);
                bool isOver = GetTaskState(data.DailyTaskIds[i], out int step);
                if (isOver)
                {
                    overTaskCount++;
                }
                else if (step >= config.ItemCount)
                {
                    total1++;
                }
            }
            var list = DailyTaskRewardsConfigCategory.Instance.GetRewards(RestaurantLv);
            for (int i = 0; i < list.Count; i++)
            {
                if (overTaskCount >= list[i].TaskCount && !IsGetDailyRewards(i))
                {
                    total2++;
                }
            }
            RedDotManager.Instance.RefreshRedDotViewCount("Market_Commiting", total1);
            RedDotManager.Instance.RefreshRedDotViewCount("Market_Commiting2", total1);
            RedDotManager.Instance.RefreshRedDotViewCount("Market_DailyReward", total2);
        }
        /// <summary>
        /// 获取每日任务
        /// </summary>
        /// <returns></returns>
        public List<int> GetDailyTaskIds()
        {
            return data.DailyTaskIds;
        }
        /// <summary>
        /// 刷新每日任务
        /// </summary>
        /// <param name="auto">是否是每天定时固定刷新？</param>
        /// <param name="lockTasks">锁住的任务</param>
        public void RefreshDailyTask(bool auto, HashSet<int> lockTasks = null)
        {
            var conf = RestaurantConfigCategory.Instance.GetByLv(RestaurantLv, out _);
            if (conf.UnlockMarket == 0) return;
            var randomDailyTask = TaskConfigCategory.Instance.GetDailyTask(RestaurantLv);
            if (randomDailyTask.Count == 0)
            {
                Log.Error("超市已解锁 但无超市任务Lv="+RestaurantLv);
                return;
            }
            if (auto)
            {
                data.DailyRewards.Clear();
                RedDotManager.Instance.RefreshRedDotViewCount("Market_RefreshAuto", 1);
            }
            var oldTasks = data.DailyTaskIds;
            List<int> res = new List<int>();
            for (int i = 0; i < oldTasks.Count; i++)
            {
                if (!auto)
                {
                    if (lockTasks != null && lockTasks.Contains(oldTasks[i]))
                    {
                        res.Add(oldTasks[i]);
                        continue;
                    }
                    if(data.OverTaskCount.ContainsKey(oldTasks[i]))
                    {
                        res.Add(oldTasks[i]);
                        continue;
                    }
                }
                else
                {
                    data.OverTaskCount.Remove(oldTasks[i]);
                }

                if (data.RunningTaskSteps.TryGetValue(oldTasks[i], out var val))
                {
                    data.RunningTaskSteps.Remove(oldTasks[i]);
                }
            }
            
            randomDailyTask.RandomSort();
            int ii = 0;
            bool hasRare = false;
            for (int i = res.Count; i < conf.ShowMax && ii<randomDailyTask.Count; i++)
            {
                for (; ii<randomDailyTask.Count; ii++)
                {
                    if (!res.Contains(randomDailyTask[ii].Id))
                    {
                        res.Add(randomDailyTask[ii].Id);
                        if (randomDailyTask[ii].Rare >= DayTaskRefreshRare)
                        {
                            hasRare = true;
                        }
                        break;
                    }
                }
            }
            if (!hasRare)//保底颜色
            {
                for (; ii<randomDailyTask.Count; ii++)
                {
                    if (randomDailyTask[ii].Rare >= DayTaskRefreshRare && !res.Contains(randomDailyTask[ii].Id))
                    {
                        res[res.Count-1] = randomDailyTask[ii].Id;
                        hasRare = true;
                        break;
                    }
                }
            }

            if (!hasRare)
            {
                Log.Info("无对应稀有度，保底失败，请策划检查配置");
            }
            data.DailyTaskIds = res;
            if (!auto)
            {
                RefreshDailyTaskRedDot();
            }
            SaveData();
        }

        /// <summary>
        /// 是否已领取每日任务进度奖励
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool IsGetDailyRewards(int index)
        {
            return data.DailyRewards.Contains(index);
        }

        /// <summary>
        /// 领取每日任务进度奖励
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool GetDailyRewards(int index)
        {
            if (IsGetDailyRewards(index)) return false;
            int overTaskCount = 0;
            var tasks = GetDailyTaskIds();
            for (int i = 0; i < tasks.Count; i++)
            {
                if (GetTaskState(tasks[i], out _))
                {
                    overTaskCount++;
                }
            }

            var rewards = DailyTaskRewardsConfigCategory.Instance.GetRewards(RestaurantLv);
            var config = rewards[index];
            if (overTaskCount >= config.TaskCount)
            {
                data.DailyRewards.Add(index);
                if (config.RewardType == 2)
                {
                    ChangeMoney(config.RewardCount);
                }
                else
                {
                    Log.Error("未处理的奖励类型");
                }

                RefreshDailyTaskRedDot();
                return true;
            }

            return false;
        }
        #endregion

        #region 物品

        /// <summary>
        /// 修改外观
        /// </summary>
        /// <param name="cloth"></param>
        public void ChangeShow(int[] cloth)
        {
            if (cloth == null || cloth.Length != CharacterConfigCategory.Instance.GetAllList().Count)
            {
                return;
            }
            data.Show = cloth;
            SaveData();
        }
        
        /// <summary>
        /// 花费或获得金币
        /// </summary>
        /// <param name="change"></param>
        /// <param name="broadcast"></param>
        public void ChangeMoney(BigNumber change, bool broadcast = true)
        {
            var old = data.Money;
            data.Money += change;
            Log.Info("<color=red>金币变化：</color>" + old.Value + (change > 0 ? "+" : "") + change.Value + " = " + data.Money.Value);
            if (change != BigNumber.Zero && broadcast)
            {
                Messager.Instance.Broadcast(0, MessageId.ChangeMoney, data.Money);
            }
            SaveData();
            RefreshTechRedDot();
            SDKManager.Instance.SetImRankData(data.Money.Value);
        }

        /// <summary>
        /// 修改物品数量
        /// </summary>
        /// <param name="id"></param>
        /// <param name="change"></param>
        public void ChangeItem(int id,int change)
        {
            if (id == GameConst.MoneyItemId)
            {
                ChangeMoney(change);
                return;
            }

            if (data.ItemCount.ContainsKey(id))
            {
                data.ItemCount[id] += change;
            }
            else
            {
                data.ItemCount[id] = change;
            }
            SaveData();
            Messager.Instance.Broadcast(id,MessageId.ChangeItem);
            Messager.Instance.Broadcast(0,MessageId.ChangeItem);
            RedDotManager.Instance.RefreshRedDotViewCount("Item_"+id, data.ItemCount[id]);
        }

        /// <summary>
        /// 获取物品数量
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetItemCount(int id)
        {
            if (id == GameConst.MoneyItemId)
            {
                return data.Money;
            }

            if (data.ItemCount.ContainsKey(id))
            {
                return data.ItemCount[id];
            }

            return 0;
        }

        /// <summary>
        /// 刷新服装
        /// </summary>
        /// <returns></returns>
        public void RefreshCloth(bool auto)
        {
            var config = RestaurantConfigCategory.Instance.GetByLv(RestaurantLv,out _);
            if (config.UnlockCloth == 0) return;
            if (auto)
            {
                RedDotManager.Instance.RefreshRedDotViewCount("Cloth_RefreshAuto", 1);
            }
            List<int> res = new List<int>();
            var list = CharacterConfigCategory.Instance.GetAllList();
            using ListComponent<CharacterConfig> modules = ListComponent<CharacterConfig>.Create();
            modules.AddRange(list);
            modules.RandomSort();
            for (int i = 0; res.Count < ClothRefreshCount; i++)
            {
                var weight = ClothConfigCategory.Instance.GetTotalWeight(modules[i].Id);
                var clothConfigs = ClothConfigCategory.Instance.GetModule(modules[i].Id);
                if (weight == 0) continue;
                clothConfigs.RandomSort();
                var temp = Random.Range(0, weight);
                for (int j = 0; j < clothConfigs.Count; j++)
                {
                    if(clothConfigs[j].Weight == 0) continue;
                    temp -= clothConfigs[j].Weight;
                    if (temp <= 0)
                    {
                        res.Add(clothConfigs[j].Id);
                        break;
                    }
                }
            }
            data.ClothsSale = res;
            SaveData();
        }

        /// <summary>
        /// 获取售卖的服装
        /// </summary>
        /// <returns></returns>
        public List<int> GetClothsSale()
        {
            return data?.ClothsSale;
        }
        
        /// <summary>
        /// 获取外观加成
        /// </summary>
        /// <returns></returns>
        public int GetClothEffect(ClothEffectType effectType)
        {
            var subModule = Show;
            int finalPercent = 0;
            using DictionaryComponent<int, int> temp = DictionaryComponent<int, int>.Create();
            for (int i = 1; i < subModule.Length; i++)
            {
                var module = CharacterConfigCategory.Instance.Get(i + 1);
                if (subModule[i] != 0 && module.DefaultCloth != subModule[i])
                {
                    var cloth = ClothConfigCategory.Instance.Get(subModule[i]);
                    if (cloth.EffectType == (int)effectType)
                    {
                        finalPercent += cloth.Param;
                    }

                    if (cloth.GroupId > 0)
                    {
                        if (temp.ContainsKey(cloth.GroupId))
                        {
                            temp[cloth.GroupId]++;
                        }
                        else
                        {
                            temp[cloth.GroupId] = 1;
                        }
                    }
                }
            }

            foreach (var kv in temp)
            {
                if (EquipGroupConfigCategory.Instance.TryGet(kv.Key, out var group))
                {
                    for (int i = 0; i < group.Count.Length; i++)
                    {
                        if (group.Count[i] > 0 && group.Count[i] <= kv.Value &&
                            group.EffectType[i] == (int)effectType)
                        {
                            finalPercent += group.Param[i];
                        }
                    }
                }
            }

            Log.Info($"获取外观加成{effectType} = " + finalPercent);
            return finalPercent;
        }
        #endregion

        #region 拍卖场次

        /// <summary>
        /// 获取当前能进入的最高等级拍卖场
        /// </summary>
        /// <returns></returns>
        public int GetMaxLevel()
        {
            var rc = RestaurantConfigCategory.Instance.GetByLv(RestaurantLv,out _);
            return rc.MaxLevelId;
        }
        /// <summary>
        /// 设置上次进入关卡
        /// </summary>
        /// <param name="id"></param>
        public void SetLastLevel(int id)
        {
            data.LastLevelId = id;
            SaveData();
        }
        /// <summary>
        /// 获取入场次数
        /// </summary>
        /// <returns></returns>
        public int GetPlayCount(int id)
        {
            if (data?.PlayCount!= null && data.PlayCount.TryGetValue(id, out var count))
            {
                return count;
            }
            return 0;
        }
        
        /// <summary>
        /// 记录入场次数
        /// </summary>
        /// <returns></returns>
        public void AddPlayCount(int id)
        {
            if (data.PlayCount.TryGetValue(id, out var count))
            {
                data.PlayCount[id] = count + 1;
                return;
            }
            data.PlayCount[id] = 1;
            SaveData();
            RefreshTechRedDot();
        }
        /// <summary>
        /// 进入引导
        /// </summary>
        /// <returns></returns>
        public void GuideSceneDone()
        {
            data.IsGuideScene = true;
            SaveData();
        }
        #endregion

        #region 今日盈利

        private void RefreshWinRewards()
        {
            var conf = RestaurantConfigCategory.Instance.GetByLv(RestaurantLv, out _);
            if (conf.Need > 0)
            {
                data.IsGotWinRewards = false;
                data.WinToday = 0;
                RedDotManager.Instance.RefreshRedDotViewCount("DailyTask", 1);
            }
        }

        /// <summary>
        /// 记录每日盈利
        /// </summary>
        /// <param name="val"></param>
        public void RecordWinToday(BigNumber val)
        {
            if (data.IsGotWinRewards)
            {
                return;
            }

            if (val <= 0) return;
            var conf = RestaurantConfigCategory.Instance.GetByLv(RestaurantLv, out _);
            if (data.WinToday >= conf.Need)
            {
                return;
            }

            var res = data.WinToday + val;
            if (res >= conf.Need)
            {
                data.WinToday = conf.Need;
            }
            else
            {
                data.WinToday = res;
            }
        }

        public long GetWinToday()
        {
            return data.WinToday;
        }
        
        public bool GetIsGotWinRewards()
        {
            return data.IsGotWinRewards;
        }
        
        public bool ReceiveWinRewards()
        {
            if (data.IsGotWinRewards) return false;
            var conf = RestaurantConfigCategory.Instance.GetByLv(RestaurantLv, out _);
            if (data.WinToday >= conf.Need)
            {
                data.IsGotWinRewards = true;
                ChangeItem(conf.RewardsType, conf.RewardsCount);
                RedDotManager.Instance.RefreshRedDotViewCount("DailyTask", 0);
                return true;
            }
            
            return false;
        }

        #endregion

        #region 引导

        public void GuideDown(int id)
        {
            if (data.OverGuide.Contains(id)) return;
            data.OverGuide.Add(id);
            SaveData();
        }

        #endregion
    }
}