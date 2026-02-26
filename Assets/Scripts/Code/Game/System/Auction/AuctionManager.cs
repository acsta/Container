using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering.Universal;

namespace TaoTie
{
    public partial class AuctionManager: IManager<MapScene>
    {

	    private int HostSayStart;
	    private int HostSayInterval;
	    private int[] MiniPlayPercent;
	    
	    private bool isDispose;
	    
        private MapScene mapScene;
        /// <summary> 是否是进入当前状态的第一帧 </summary>
        private bool isEnterState;
        public AuctionState AState { get; private set; }

        /// <summary> 难度 </summary>
        public int Level { get; private set; }
        /// <summary> 轮次从1开始 </summary>
        public int Stage { get; private set; }
        /// <summary>
        /// 情报Id
        /// </summary>
        public int GameInfoId{ get; private set; }
        /// <summary>
        /// 命运骰子Id
        /// </summary>
        public int DiceId{ get; private set; }
        /// <summary>
        /// 开局事件Id
        /// </summary>
        public int StartEventId{ get; private set; }
        public StageConfig Config => StageConfigCategory.Instance.GetLevelConfigByLvAndStage(Level, Stage);
        public LevelConfig LevelConfig => LevelConfigCategory.Instance.Get(Level);
        public GameInfoConfig GameInfoConfig => GameInfoConfigCategory.Instance.Get(GameInfoId);
        public DiceConfig DiceConfig => DiceConfigCategory.Instance.Get(DiceId);

        public StartEventConfig StartEventConfig =>
	        StartEventId == 0 ? null : StartEventConfigCategory.Instance.Get(StartEventId);
        /// <summary> 本场集装箱盲盒 </summary>
        public AuctionReport[] AuctionReports { get; private set; }
        /// <summary> 本轮数据 </summary>
        public AuctionReport Report => AuctionReports[Stage - 1];
        /// <summary> 低价 </summary>
        public BigNumber LowAuction => LastAuctionPrice + Config.Auction1 + Config.RaiseAuctionAddon * RaiseCount;
        /// <summary> 中价 </summary>
        public BigNumber MediumAuction => LastAuctionPrice + Config.Auction2 + Config.RaiseAuctionAddon * RaiseCount;
        /// <summary> 高价 </summary>
        public BigNumber HighAuction => LastAuctionPrice + Config.Auction3 + Config.RaiseAuctionAddon * RaiseCount;


        public List<long> Bidders { get; } = new List<long>();
        public List<long> Npcs { get; }= new List<long>();
        public List<long> Boxes { get; }= new List<long>();
        public List<long> OpenBoxes { get; }= new List<long>();
        public List<long> Blacks { get; } = new List<long>();
        public Player Player{ get; private set; }
        public long HostId{ get;private set;  }
        private AIDecision[] decisions;
        
        private List<long> lastAuctionAllPlayers = new List<long>();     //后来添加的：所有人的喊价顺序。用于判断两个人一直抬价，抬价的次数

        public BigNumber AllPrice { get; private set; } = BigNumber.Zero; //总价值
        public float SysJudgePriceMin{ get; private set; }  //本关区间价值判断最小值
        public float SysJudgePriceMax{ get; private set; }	//本关区间价值判断最大值
        public long LastAuctionPlayerId { get; private set; } //上一个叫价的人 -1没有，0玩家，其他AIid
        public BigNumber LastAuctionPrice { get; private set; } = BigNumber.Zero; //上一次叫价
        public AITactic LastAuctionPriceType{ get; private set; } //上一次叫价类型
        public long LastAuctionTime{ get; private set; } //上一次叫价间隔时间
        public int LastHostSayCount { get; private set; } //上一次叫价拍卖师倒计时次数;
        public bool IsRaising { get; private set; } //是否抬价阶段
        public int RaiseSuccessCount { get; private set; } //玩家成功抬价次数
        public int PlayerAuctionCount { get; private set; } //玩家出价次数
        public int AuctionCount { get; private set;  } //总出价次数
        public int RaiseCount{ get; private set; } //抬价次数，进入抬价那次不算次数
        public bool IsAllPlayBox { get; private set; }
        public bool Skip{ get; set; }//快速跳过
        private bool hasGodOfWealth;
        private Dictionary<int,int> globalPlayType = new Dictionary<int, int>();
        private int continuousFollowCount;//连续跟风次数
        private int forceUpCount;//抬价次数
        private long startWaitUserTime;//开始等待时间
        private int playerLowAuctionCount;
        private int playerMidAuctionCount;
        private int playerHighAuctionCount;
        private int hostSayCount = 0;//拍卖师倒计时次数
        private long ufoId;
        
        public bool HasMiniPlayItem { get; private set; }//小玩法物品
        public bool HasTaskItem { get; private set; }//任务物品

        private ETCancellationToken cancellationToken;
        private ETCancellationToken hostCancelToken;
        private string curTexture;
        // private GameObject EventSmoke;
        private long RandomDrop;
        #region IManager

        public void Init(MapScene map)
        {
	        if (!GlobalConfigCategory.Instance.TryGetInt("HostSayStart", out HostSayStart))
	        {
		        HostSayStart = 2000;
	        }
	        if (!GlobalConfigCategory.Instance.TryGetInt("HostSayInterval", out HostSayInterval))
	        {
		        HostSayInterval = 1000;
	        }

	        if (!GlobalConfigCategory.Instance.TryGetArray("MiniPlayPercent", out MiniPlayPercent))
	        {
		        MiniPlayPercent = new int[] {0, 10, 20, 30, 40, 50, 60};
	        }
	        isDispose = false;
	        IAuctionManager.Instance = this;
            this.mapScene = map;
            Level = mapScene.Config.Id;
            PlayerDataManager.Instance.SetLastLevel(Level);
            // noWhiteProp = mapScene.Config.NoWhiteProp;
            SetState(AuctionState.Awake);
            Messager.Instance.AddListener<string,double,double>(0,MessageId.ClipStartPlay,ClipStartPlay);
        }

        public void Destroy()
        {
	        // RecycleEventSmoke();
	        DestroyDropItem();
	        
	        if (ufoId != 0)
	        {
		        EntityManager.Instance?.Remove(ufoId);
		        ufoId = 0;
	        }
	        
	        Messager.Instance.RemoveListener<string,double,double>(0,MessageId.ClipStartPlay,ClipStartPlay);
	        if (!string.IsNullOrEmpty(curTexture))
	        {
		        ImageLoaderManager.Instance.ReleaseImage(curTexture);
		        curTexture = null;
	        }
	        cancellationToken?.Cancel();
	        hostCancelToken?.Cancel();
	        cancellationToken = null;
	        hostCancelToken = null;
	        centerCharacter = null;
	        isDispose = true;
	        Bidders.Clear();
	        Npcs.Clear();
	        Blacks.Clear();
            SetState(AuctionState.Free);
            var phc = Player?.GetComponent<GameObjectHolderComponent>();
            var flag = phc?.GetCollectorObj<GameObject>("Flag");
            flag?.SetActive(true);
            Player = null;
            IAuctionManager.Instance = null;
            PerformanceManager.Instance.SetLowFrame();
        }
        
        #endregion

        private void CreateGameInfo()
        {
	        GameInfoId = -1;
	        var infos = GameInfoConfigCategory.Instance.GetByLevel(Level);
	        if (infos.Count > 0)
	        {
		        var random = Random.Range(0, 100);
		        if (LevelConfig.GameInfoPercent > random)
		        {
			        infos.RandomSort();
			        GameInfoId = infos[0].Id;
			        Log.Info("情报事件"+GameInfoId);
		        }
	        }
        }

        /// <summary>
        /// 生成集装箱盲盒
        /// </summary>
        private void CreateContainer()
        {
	        if(!StageConfigCategory.Instance.TryGetStageByLevel(Level,out var stages) || stages.Count == 0)
	        {
		        Log.Error("指定等级场次数为0 level=" + Level);
	        }
	        AuctionReports = new AuctionReport[stages.Count];
	        for (int i = 0; i < AuctionReports.Length; i++)
	        {
		        AuctionReports[i] = new AuctionReport();
	        }

	        if (DiceConfig.Type == 1)
	        {
		        var container = ContainerConfigCategory.Instance.Get(DiceConfig.Param);
		        if (container != null)
		        {
			        Log.Info("骰子指定集装箱为"+ DiceConfig.Param);
			        if (container.Level != Level)
			        {
				        Log.Error("骰子指定集装箱不属于当前场次，骰子Id = " + DiceId);
			        }
			        for (int i = 0; i < AuctionReports.Length; i++)
			        {
				        AuctionReports[i].ContainerId = DiceConfig.Param;
			        }
			        return;
		        }

		        Log.Error("骰子指定集装箱不存在，集装箱Id =" + DiceConfig.Param + "，骰子Id = " + DiceId);
	        }
	        using ListComponent<int> normalContainers = ListComponent<int>.Create();
	        using ListComponent<int> specialContainers = ListComponent<int>.Create();
	        PlayerDataManager.Instance.GetUnlockContainerIds(Level, normalContainers, specialContainers);
	        specialContainers.RandomSort();
	        if(normalContainers.Count == 0)
	        {
		        Log.Error("指定等级场无普通盲盒 level=" + Level);
		        return;
	        }

	        int specialCount = 0;
	        if (specialContainers.Count == 1)
	        {
		        specialCount = AuctionHelper.RandomSpecialCount(LevelConfig.ContainerCounts1, LevelConfig.ContainerWeight1);
	        }
	        else if (specialContainers.Count == 2)
	        {
		        specialCount = AuctionHelper.RandomSpecialCount(LevelConfig.ContainerCounts2, LevelConfig.ContainerWeight2);
	        }
	        else if (specialContainers.Count >= 3)
	        {
		        specialCount = AuctionHelper.RandomSpecialCount(LevelConfig.ContainerCounts3, LevelConfig.ContainerWeight3);
	        }
	        
	        if (GameSetting.ContainerRandType == ContainerRandType.OnlyNormal)
	        {
		        specialCount = 0;
	        }
	        else if (GameSetting.ContainerRandType == ContainerRandType.OnlySp)
	        {
		        specialCount = 5;
	        }
	        else if (GameSetting.ContainerRandType == ContainerRandType.Target && ContainerConfigCategory.Instance.Contain(GameSetting.ContainerId))
	        {
		        for (int i = 0; i < AuctionReports.Length; i++)
		        {
			        AuctionReports[i].Index = i;
			        AuctionReports[i].ContainerId = GameSetting.ContainerId;
		        }
		        ReplaceTexture();
		        return;
	        }

	        if (specialCount > AuctionReports.Length)
	        {
		        specialCount = AuctionReports.Length;
		        Log.Error("特殊集装箱数量超过场次数");
	        }
	        int totalSp = 0;
	        for (int i = 0; i < specialContainers.Count; i++)
	        {
		        totalSp += ContainerConfigCategory.Instance.Get(specialContainers[i]).Weight;
	        }
	        int unLockCount = 0;
	        if (totalSp == 0)
	        {
		        if(specialCount != 0) Log.Error("特殊集装箱权重和为0");
		        specialCount = 0;
	        }
	        else
	        {
		        var unlockList = PlayerDataManager.Instance.GetUnlockList();
		        for (int j = 0; j < unlockList.Count && j < specialCount; j++ )
		        {
			        var config = TechnologyTreeConfigCategory.Instance.Get(unlockList[j]);
			        if (config.Level == 1)
			        {
				        var cc = ContainerConfigCategory.Instance.Get(config.Id);
				        if (cc.Level != Level)
				        {
					        continue;
				        }
				        //集装箱
				        AuctionReports[unLockCount].ContainerId = config.Id;
				        Log.Info($"触发新解锁科技树，打乱顺序前第{unLockCount}个箱子为{config.Id}");
				        unLockCount++;
			        }
			        else if (config.Level == 2)
			        {
				        var cc = ContainerConfigCategory.Instance.Get(config.ParentId);
				        if (cc.Level != Level)
				        {
					        continue;
				        }
				        //玩法
				        AuctionReports[unLockCount].ContainerId = config.ParentId;
				        Log.Info($"触发新解锁科技树，打乱顺序前第{unLockCount}个箱子为{config.ParentId}");
				        unLockCount++;
			        }
			        else
			        {
				        Log.Error("触发新解锁科技树,未处理层级,unlockId="+unlockList[unLockCount]);
			        }
		        }
		        for (int j = unLockCount; j < specialCount; j++)
		        {
			        var random = Random.Range(0, totalSp * 10) % totalSp;
			        for (int i = 0; i < specialContainers.Count; i++)
			        {
				        AuctionReports[j].ContainerId = specialContainers[i];
				        random -= ContainerConfigCategory.Instance.Get(specialContainers[i]).Weight;
				        if (random <= 0)
				        {
					        break;
				        }
			        }
		        }
	        }
	        int totalN = 1;
	        for (int i = 0; i < normalContainers.Count; i++)
	        {
		        totalN += ContainerConfigCategory.Instance.Get(normalContainers[i]).Weight + 1;
	        }
	        for (int j = specialCount; j < AuctionReports.Length; j++)
	        {
		        var random = Random.Range(0, totalN*10)%totalN;
		        for (int i = 0; i < normalContainers.Count; i++)
		        {
			        AuctionReports[j].ContainerId = normalContainers[i];
			        random -= ContainerConfigCategory.Instance.Get(normalContainers[i]).Weight + 1;
			        if (random <= 0)
			        {
				        break;
			        }
		        }
	        }

	        if (unLockCount <= 0)
	        {
		        AuctionReports.RandomSort();
	        }
	        else
	        {
		        Log.Info("触发新解锁科技树，不打乱顺序");
	        }
	        for (int i = 0; i < AuctionReports.Length; i++)
	        {
		        AuctionReports[i].Index = i;
	        }
	        
	        ReplaceTexture();
        }

        private void ReplaceTexture()
        {
	        if (!string.IsNullOrEmpty(curTexture))
	        {
		        ImageLoaderManager.Instance.ReleaseImage(curTexture);
	        }
	        curTexture = ContainerConfigCategory.Instance.Get(AuctionReports[0].ContainerId)?.Skin;
	        if (string.IsNullOrEmpty(curTexture))
	        {
		        Log.Error("集装箱为空"+AuctionReports[0].ContainerId);
		        return;
	        }
	        var thisTex = curTexture;
	        ImageLoaderManager.Instance.LoadTextureAsync(curTexture, (tex) =>
	        {
		        if (thisTex != curTexture)
		        {
			        return;
		        }
		        var meshRenderer = mapScene.Collector.Get<MeshRenderer>("BoxRoot");
		        meshRenderer.sharedMaterial.mainTexture = tex;
	        }).Coroutine();
        }

        
        /// <summary>
        /// 生成本关物品
        /// </summary>
        private void CreateItems()
        {
	        HasMiniPlayItem = false;
	        HasTaskItem = false;
	        var containerId = Report.ContainerId;
	        var cc = ContainerConfigCategory.Instance.Get(containerId);
	        AllPrice = 0;
	        using var itemConfigs = ListComponent<ItemConfig>.Create();
	        ItemConfig taskItem = null;
	        if (cc.Type == 0)
	        {
		        IsAllPlayBox = true;
		        Log.Info("全玩法集装箱");
		        using var normalContainers = ListComponent<int>.Create();
		        using var specialContainers = ListComponent<int>.Create();
		        PlayerDataManager.Instance.GetUnlockContainerIds(Level, normalContainers, specialContainers);
		        if (specialContainers.Count + normalContainers.Count <= 0)
		        {
			        Log.Error("全玩法集装箱 不存在已解锁的集装箱");
			        return;
		        }
		        using var allPlayType = ListComponent<(int, ItemType)>.Create();
		        for (int i = 0; i < normalContainers.Count; i++)
		        {
			        using var play = PlayerDataManager.Instance.GetUnlockMiniPlayIds(normalContainers[i]);
			        for (int j = 0; j < play.Count; j++)
			        {
				        var playType = PlayTypeConfigCategory.Instance.Get((int)play[j]);
				        if (playType == null || playType.IsEffected == 1) continue;
				        var superItems = ItemConfigCategory.Instance.GetItemIdsByContainerAndType(normalContainers[i], play[j]);
				        if (superItems.Count <= 0)
				        {
					        Log.Error("<color=#88FFEF>[Auction]</color> 指定集装箱盲盒不存在特殊玩法物品，盲盒=" + normalContainers[i] +
					                  "playType = " + play[j]);
				        
				        }
				        allPlayType.Add((normalContainers[i], play[j]));
			        }
					
		        }
		        for (int i = 0; i < specialContainers.Count; i++)
		        {
			        using var play = PlayerDataManager.Instance.GetUnlockMiniPlayIds(specialContainers[i]);
			        for (int j = 0; j < play.Count; j++)
			        {
				        var playType = PlayTypeConfigCategory.Instance.Get((int) play[j]);
				        if (playType == null || playType.IsEffected == 1) continue;
				        var superItems =
					        ItemConfigCategory.Instance.GetItemIdsByContainerAndType(specialContainers[i], play[j]);
				        if (superItems.Count <= 0)
				        {
					        Log.Error("<color=#88FFEF>[Auction]</color> 指定集装箱盲盒不存在特殊玩法物品，盲盒=" + specialContainers[i] +
					                  "playType = " + play[j]);
							continue;
				        }

				        allPlayType.Add((specialContainers[i], play[j]));
			        }
		        }
		        if (allPlayType.Count <= 0)
		        {
			        Log.Error("全玩法集装箱 不存在已解锁的玩法");
			        return;
		        }
		        allPlayType.RandomSort();
		        HasMiniPlayItem = true;
		        HasTaskItem = false;

		        for (int i = 0; itemConfigs.Count < 6 && i < 18; i++)
		        {
			        var playType = allPlayType[i % allPlayType.Count];
			        var superItems = ItemConfigCategory.Instance.GetItemIdsByContainerAndType(playType.Item1, playType.Item2);
			        //玩法物品价值占比不能超过预设最小值
			        var range = Random.Range(0, superItems.Count);
			        ItemConfig temp = null;
			        for (int j = 0; j <= superItems.Count; j++)
			        {
				        var index = (range + j) % superItems.Count;
				        if (temp == null || superItems[index].Price < temp.Price)
				        {
					        temp = superItems[index];
					        if (!itemConfigs.Contains(temp) && (temp.Price <= Config.SpValue[1] && 
					             temp.Price >= Config.SpValue[0] || i >= 12))
					        {
						        break;
					        }
				        }
			        }
			        itemConfigs.Add(temp);
			        AllPrice += itemConfigs[itemConfigs.Count - 1].Price;
		        }
	        }
	        else
	        {
		        IsAllPlayBox = false;
		        var normalItems = ItemConfigCategory.Instance.GetItemIdsByContainerAndType(containerId, ItemType.None);
		        if (normalItems.Count == 0)
		        {
			        Log.Error("<color=#88FFEF>[Auction]</color> 指定集装箱盲盒不存在普通物品，盲盒=" + containerId);
			        return;
		        }

		        //随机任务物品
		        using var tasks = PlayerDataManager.Instance.GetRunningTaskIds();
		        tasks.RandomSort();
		        var addon = PlayerDataManager.Instance.GetClothEffect(ClothEffectType.TaskItemAppearAddon);
		        for (int i = 0; i < tasks.Count; i++)
		        {
			        var task = TaskConfigCategory.Instance.Get(tasks[i]);
			        if (task.ItemType != 0) continue;
			        if (PlayerDataManager.Instance.GetTaskState(tasks[i], out var step))
			        {
				        continue;
			        }
			        else if (step >= task.ItemCount)
			        {
				        continue;
			        }

			        var cfg = ItemConfigCategory.Instance.Get(task.ItemId);
			        if (cfg.ContainerId == containerId && cfg.Type != (int) ItemType.Const)
			        {
				        var rand = Random.Range(0, 100);
				        if (rand < task.Percent + addon)
				        {
					        taskItem = cfg;
					        break;
				        }
			        }
		        }

		        //随机通用玩法物品
		        if (GlobalConfigCategory.Instance.TryGetInt("GlobalPlayTypePercent", out var gPrcent) &&
		            Random.Range(0, 100) < gPrcent)
		        {
			        var list = PlayTypeConfigCategory.Instance.GetAllList();
			        using var temp = ListComponent<PlayTypeConfig>.Create();
			        int weightPlayType = 0;
			        for (int i = 0; i < list.Count; i++)
			        {
				        if (list[i].Type == 2)
				        {
					        weightPlayType += list[i].Weight;
					        temp.Add(list[i]);
				        }
				        else if (list[i].Type == 1 && !globalPlayType.ContainsKey(list[i].Id))
				        {
					        weightPlayType += list[i].Weight;
					        temp.Add(list[i]);
				        }
			        }

			        if (temp.Count > 0)
			        {
				        var random = Random.Range(0, weightPlayType);
				        int index = 0;
				        for (int i = 0; i < temp.Count; i++)
				        {
					        index = i;
					        random -= temp[i].Weight;
					        if (random <= 0)
					        {
						        break;
					        }
				        }
				        var items = ItemConfigCategory.Instance.GetGlobalItems((ItemType)temp[index].Id);
				        if (items.Count <= 0)
				        {
					        Log.Error("<color=#88FFEF>[Auction]</color> 不存在全局特殊物品，玩法id=" + temp[index].Id);
				        }
				        else
				        {
					        items.RandomSort();
					        ItemConfig item = items[0];
					        Log.Info($"<color=#88FFEF>[Auction]</color> 全局特殊物品为{item.Id}, 不参与价格计算");
					        itemConfigs.Add(item);
					        if (!globalPlayType.TryGetValue(temp[index].Id, out var res))
					        {
						        res = 0;
					        }
					        globalPlayType[temp[index].Id] = res + 1;
				        }
			        }
			        else
			        {
				        Log.Error("<color=#88FFEF>[Auction]</color> 不存在全局玩法");
			        }
		        }
		        
		        //随机玩法物品
		        using var playList = PlayerDataManager.Instance.GetUnlockMiniPlayIds(containerId);
		        int percent = 0;
		        if (playList.Count >= MiniPlayPercent.Length)
		        {
			        percent = MiniPlayPercent[MiniPlayPercent.Length - 1];
		        }
		        else
		        {
			        percent = MiniPlayPercent[playList.Count];
		        }

		        bool hasPlayable = Random.Range(0, 100) < Mathf.Min(100, percent);
		        ItemType playType = ItemType.None;
		        if (GameSetting.AlwaysPlayType || DiceConfig.Type == 2)
		        {
			        hasPlayable = true;
		        }
		        
		        var unlockList = PlayerDataManager.Instance.GetUnlockList();
		        for (int i = 0; i < unlockList.Count; i++)
		        {
			        var config = TechnologyTreeConfigCategory.Instance.Get(unlockList[i]);
			        if (config.ParentId == containerId)
			        {
				        hasPlayable = true;
				        playType = (ItemType) config.Content;
				        HasMiniPlayItem = true;
				        Log.Info($"<color=#88FFEF>[Auction]</color> 触发新解锁科技树，特殊物品类型为{playType}");
				        break;
			        }
		        }

		        if (playType == ItemType.None)
		        {
			        if (hasPlayable)
			        {
				        if (playList.Count > 0)
				        {
					        playType = playList[Random.Range(0, playList.Count)];
					        Log.Info($"<color=#88FFEF>[Auction]</color> 特殊物品类型为{playType}");
					        HasMiniPlayItem = true;
				        }
				        else
				        {
					        Log.Error("<color=#88FFEF>[Auction]</color> 指定集装箱不存在特殊物品，集装箱id=" + containerId);
				        }
			        }
			        else
			        {
				        Log.Info("<color=#88FFEF>[Auction]</color> 无颜色");
			        }
		        }

		        if (Config.Value == null || Config.Value.Length < 2)
		        {
			        Log.Error("配置错误 LevelConfig.Value == null || LevelConfig.Value.Length < 2");
			        return;
		        }

		        BigNumber minPrice = Config.Value[0]; //规定的本场次物品总价值必须高于这个
		        BigNumber maxPrice = Config.Value[1]; //规定的本场次物品总价值必须低于这个
		        
		        if (taskItem != null)
		        {
			        itemConfigs.Add(taskItem);
			        Log.Info("<color=#88FFEF>[Auction]</color> 有一个任务物品id=" + taskItem.Id);
			        HasTaskItem = true;
			        //AllPrice 任务物品不计入价格
		        }

		        //先生成特殊颜色的东西
		        if (hasPlayable)
		        {
			        var superItems = ItemConfigCategory.Instance.GetItemIdsByContainerAndType(containerId, playType);
			        if (superItems.Count <= 0)
			        {
				        Log.Error("<color=#88FFEF>[Auction]</color> 指定集装箱盲盒不存在特殊玩法物品，盲盒=" + containerId +
				                  "playType = " +
				                  playType);
				        HasMiniPlayItem = false;
			        }
			        else
			        {
				        //玩法物品价值占比不能超过预设最小值
				        var range = Random.Range(0, superItems.Count);
				        ItemConfig temp = null;
				        for (int i = 0; i <= superItems.Count; i++)
				        {
					        var index = (range + i) % superItems.Count;
					        temp = superItems[index];
					        if (temp.Price <= Config.SpValue[1] && 
					            temp.Price >= Config.SpValue[0])
					        {
						        break;
					        }
				        }

				        itemConfigs.Add(temp);
				        AllPrice += itemConfigs[itemConfigs.Count - 1].Price;
			        }
		        }
		        
		        if (!HasMiniPlayItem || PlayTypeConfigCategory.Instance.Get((int) playType).IsEffected == 0)
		        {
			        if (!GlobalConfigCategory.Instance.TryGetInt("StoryItemPercent", out var pp))
			        {
				        pp = 50;
			        }
			        if (Random.Range(0, 100) < pp || GameSetting.AlwaysStoryEvent)
			        {
				        var storyItems = ItemConfigCategory.Instance.GetItemIdsByContainerAndType(containerId, ItemType.Story);
				        if (storyItems.Count <= 0)
				        {
					        Log.Error("<color=#88FFEF>[Auction]</color> 指定集装箱盲盒不存在剧情物品，盲盒=" + containerId);
				        }
				        else
				        {
					        storyItems.RandomSort();
					        var range = Random.Range(0, storyItems.Count);
					        var item = storyItems[range];
					        itemConfigs.Add(item);
					        item.Type = (int) ItemType.Story;
					        AllPrice += item.Price;
					        Log.Info("<color=#88FFEF>[Auction]</color> 有一个剧情物品id=" + item.Id);
					        // CreateEventSmoke().Coroutine();
				        }
			        }
		        }

		        //生成白色的东西,东西的总数量6
		        int whiteCount = 6 - itemConfigs.Count;

		        //先将白色的数量随便找够
		        normalItems.RandomSort();
		        using var tempConfigs = ListComponent<ItemConfig>.Create();
		        //第一遍尽量找价格合适的
		        for (int i = 0; tempConfigs.Count < whiteCount && i < normalItems.Count; i++)
		        {
			        if (tempConfigs.Contains(normalItems[i]) || itemConfigs.Contains(normalItems[i])) continue;
			        if (normalItems[i].Price > Config.NormalValue[1] || 
			            normalItems[i].Price < Config.NormalValue[0]) continue;
			        normalItems[i].Type = (int) ItemType.None;
			        tempConfigs.Add(normalItems[i]);
			        AllPrice += normalItems[i].Price;
		        }

		        //第二遍只要不重复就行
		        for (int i = tempConfigs.Count; tempConfigs.Count < whiteCount && i < normalItems.Count; i++)
		        {
			        if (tempConfigs.Contains(normalItems[i]) || itemConfigs.Contains(normalItems[i])) continue;
			        tempConfigs.Add(normalItems[i]);
			        normalItems[i].Type = (int) ItemType.None;
			        AllPrice += normalItems[i].Price;
		        }
		        
		        //总价格满足区间，则生成完毕
		        for (int i = 0; i < 100; i++)
		        {
			        if (AllPrice >= minPrice && AllPrice <= maxPrice)
			        {
				        break;
			        }
			        //如果总价格不满足区间，那么就先按照价格从低到大排序
			        else
			        {
				        tempConfigs.Sort((o1, o2) => o1.Price >= o2.Price ? 1 : -1);
				        //如果总价格大于上限，那么我就去掉一个最大的
				        if (AllPrice > maxPrice)
				        {
					        ItemConfig popCfg = tempConfigs[tempConfigs.Count - 1];
					        tempConfigs.RemoveAt(tempConfigs.Count - 1);
					        AllPrice -= popCfg.Price;
					        bool find = false;
					        ItemConfig lowCfg = null;
					        //去掉后，再从源配置里面找一个同颜色的，保证再找到一个后，新的总价在区间内
					        foreach (ItemConfig cfg in normalItems)
					        {
						        if (tempConfigs.Contains(cfg) || itemConfigs.Contains(cfg)) continue;
						        if (cfg.Price > Config.NormalValue[1] || 
						            cfg.Price < Config.NormalValue[0]) continue;
						        BigNumber thisCfgPrice = cfg.Price;
						        if (thisCfgPrice + AllPrice >= minPrice && thisCfgPrice + AllPrice <= maxPrice)
						        {
							        AllPrice += thisCfgPrice;
							        cfg.Type = (int) ItemType.None;
							        tempConfigs.Add(cfg);
							        find = true;
							        break;
						        }
						        else
						        {
							        if (lowCfg == null)
							        {
								        lowCfg = cfg;
							        }
							        else if (cfg.Price < lowCfg.Price) //不好判断，只补充小的箱子
							        {
								        lowCfg = cfg;
							        }
						        }
					        }

					        if (!find && lowCfg != null)
					        {
						        tempConfigs.Add(lowCfg);
						        lowCfg.Type = (int) ItemType.None;
						        AllPrice += lowCfg.Price;
					        }
				        }
				        //如果总价格低于下限，那么就去掉一个最小的
				        else if (AllPrice < minPrice)
				        {
					        ItemConfig popCfg = tempConfigs[0];
					        tempConfigs.RemoveAt(0);
					        AllPrice -= popCfg.Price;
					        //去掉后，再从源配置里面找一个同颜色的，保证再找到一个后，新的总价在区间内
					        //如果找到最后都没找到，那么就找一个价格最高的补进去
					        bool find = false;
					        ItemConfig highestCfg = null;
					        foreach (ItemConfig cfg in normalItems)
					        {
						        if (tempConfigs.Contains(cfg) || itemConfigs.Contains(cfg)) continue;
						        if (cfg.Price > Config.NormalValue[1] || 
						            cfg.Price < Config.NormalValue[0]) continue;
						        BigNumber thisCfgPrice = cfg.Price;
						        if (thisCfgPrice + AllPrice >= minPrice && thisCfgPrice + AllPrice <= maxPrice)
						        {
							        AllPrice += thisCfgPrice;
							        tempConfigs.Add(cfg);
							        cfg.Type = (int) ItemType.None;
							        find = true;
							        break;
						        }
						        else
						        {
							        if (highestCfg == null)
							        {
								        highestCfg = cfg;
							        }
							        else if (cfg.Price > highestCfg.Price) //不好判断，只补充小的箱子
							        {
								        highestCfg = cfg;
							        }
						        }
					        }

					        if (!find && highestCfg != null)
					        {
						        tempConfigs.Add(highestCfg);
						        highestCfg.Type = (int) ItemType.None;
						        AllPrice += highestCfg.Price;
					        }
				        }
			        }
		        }

		        itemConfigs.AddRange(tempConfigs);
	        }
	        itemConfigs.Sort((a, b) =>
	        {
		        var unitA = UnitConfigCategory.Instance.Get(a.UnitId);
		        var unitB = UnitConfigCategory.Instance.Get(b.UnitId);
		        return (unitB?.Priority ?? 0) - (unitA?.Priority ?? 0);
	        });
	        var boxCollider = mapScene.Collector.Get<BoxCollider>("BoxRoot");
	        using ListComponent<UnitConfig> configs = ListComponent<UnitConfig>.Create(); 
	        for (int i = 0; i < itemConfigs.Count; i++)
	        {
		        var unit = UnitConfigCategory.Instance.Get(itemConfigs[i].UnitId);
		        if (unit == null)
		        {
			        Log.Error("箱子id不存在 itemId = " + itemConfigs[i].Id + ", boxId = " + itemConfigs[i].UnitId);
			        continue;
		        }

		        configs.Add(unit);
	        }
			
	        var meshRenderer = boxCollider.GetComponent<MeshRenderer>();
	        
	        curTexture = cc.Skin;
	        ImageLoaderManager.Instance.LoadTextureAsync(curTexture, (tex) =>
	        {
		        meshRenderer.sharedMaterial.mainTexture = tex;
	        }).Coroutine();
	       
	        var size = boxCollider.transform.rotation * boxCollider.size;
	        var startPos = boxCollider.transform.position + boxCollider.transform.rotation * boxCollider.center - size / 2;
	        size.z = Mathf.Abs(size.z);
	        bool success = AuctionHelper.PackBoxes(size, configs, out int[] rot, out Vector3[] pos);
	        if (!success)
	        {
		        Log.Error("放不下重新生成");
		        CreateItems();
		        return;
	        }

	        var minZ = float.MaxValue;
	        var maxZ = float.MinValue;
	        for (int i = 0; i < pos.Length; i++)
	        {
		        if (pos[i].z < minZ)
		        {
			        minZ = pos[i].z;
		        }
		        if (pos[i].z > maxZ)
		        {
			        maxZ = pos[i].z;
		        }
	        }

	        //居中
	        if (maxZ - minZ < size.z)
	        {
		        startPos -= Vector3.forward * (size.z - maxZ + minZ) / 2;
	        }
	        Report.GameInfoId = GameInfoId;
	        Report.Items = new ItemConfig[pos.Length];
	        Report.BoxTypes = new BoxType[pos.Length];
	        Report.PlayData = new object[pos.Length];
	        Report.AllPriceStr = AllPrice;
	        if (taskItem != null)
	        {
		        Report.TaskItemIndex = itemConfigs.IndexOf(taskItem);
	        }

	        bool openBoxEvt = GlobalConfigCategory.Instance.TryGetInt("OpenBoxEventPercent", out var per) &&
	                          Random.Range(0, 100) < per;
	        
	        int emptyCount = 0;
	        for (int i = 0; i < pos.Length; i++)
	        {
		        var box = EntityManager.Instance.CreateEntity<Box, int>(itemConfigs[i].Id);
		        if (box.ItemId == taskItem?.Id)
		        {
			        box.BoxType = BoxType.Task;
		        }
		        if (PlayTypeConfigCategory.Instance.Contain(box.ItemConfig.Type))
		        {
			        var res = PlayTypeConfigCategory.Instance.Get(box.ItemConfig.Type);
			        box.BoxType = (BoxType)res.BoxType;
		        }

		        if (StartEventConfig != null && StartEventConfig.EmptyMaxCount > 0 &&
		            emptyCount < StartEventConfig.EmptyMaxCount)
		        {
			        if (box.BoxType == BoxType.Task && (StartEventConfig.Type | 1) == 0) continue;
			        if (box.ItemConfig.Type != (int) ItemType.None && (StartEventConfig.Type | 2) == 0) continue;
			        if (Random.Range(0, 100) < StartEventConfig.EmptyPercent)
			        {
				        emptyCount++;
				        box.BoxType = BoxType.Empty;
			        }
		        }

		        if (openBoxEvt && box.BoxType == BoxType.Normal && box.ItemConfig.Type == (int) ItemType.None)
		        {
			        openBoxEvt = false;
			        box.BoxType = BoxType.RandOpenEvt;
			        Log.Info("有一个开箱事件物品");
		        }

		        Report.Items[i] = itemConfigs[i];
		        Report.BoxTypes[i] = box.BoxType;
		        Boxes.Add(box.Id);
		        var flag = AuctionHelper.Flag[rot[i]];
		        var boxSize = AuctionHelper.Rotations[rot[i]] * new Vector3(box.Config.Size[0], box.Config.Size[1], box.Config.Size[2]);
		        var offset = new Vector3(boxSize.x * flag.x, boxSize.y * flag.y, boxSize.z * flag.z);
		        offset = new Vector3(pos[i].x, pos[i].y, -pos[i].z) + offset;
		        box.Position = startPos + offset;
		        box.Rotation = AuctionHelper.Rotations[rot[i]];
		        SetBoxParent(boxCollider, box).Coroutine();
	        }

	        Log.Info($"总价{AllPrice.Value}, 空箱个数{emptyCount}");
	        CreateUfo().Coroutine();
        }
        
        private async ETTask SetBoxParent(BoxCollider root, Box box)
        {
	        var ghc = box.GetComponent<GameObjectHolderComponent>();
	        await ghc.WaitLoadGameObjectOver();
	        ghc.EntityView.SetParent(root.transform, true);
        }
        /// <summary>
        /// 生成价格的低区间与高区间
        /// </summary>
        private void CreateRangePrice()
        {
	        using ListComponent<int> weightArr = ListComponent<int>.Create();
	        int weight = 0;
	        foreach (var item in Config.FirstJudgeMin)
	        {
		        weight += (int) item[2];
		        weightArr.Add(weight);
	        }

	        int ran = Random.Range(0, weight);
	        for (int i = 0; i < Config.FirstJudgeMin.Length; i++)
	        {
		        if (ran < weightArr[i])
		        {
			        var arr3 = Config.FirstJudgeMin[i];
			        float minValue = arr3[0];
			        float maxValue = arr3[1];
			        SysJudgePriceMin = Random.Range(minValue, maxValue);
			        break;
		        }
	        }
	        
	        weightArr.Clear();
	        weight = 0;
	        foreach (var item in Config.FirstJudgeMax)
	        {
		        weight += (int) item[2];
		        weightArr.Add(weight);
	        }
	        
	        ran = Random.Range(0, weight);
	        for (int i = 0; i < Config.FirstJudgeMax.Length; i++)
	        {
		        if (ran < weightArr[i])
		        {
			        var arr3 = Config.FirstJudgeMax[i];
			        float minValue = arr3[0];
			        float maxValue = arr3[1];
			        SysJudgePriceMax = Random.Range(minValue, maxValue);
			        break;
		        }
	        }
	        
	        var addon = PlayerDataManager.Instance.GetClothEffect(ClothEffectType.JudgePriceReduce);
	        if (addon > 100) addon = 100;
	        SysJudgePriceMin = Mathf.Lerp(SysJudgePriceMin, 1, addon / 100f);
	        SysJudgePriceMax = Mathf.Lerp(SysJudgePriceMax, 1, addon / 100f);
        }

        /// <summary>
        /// 商品价格确定后，生成AI的判断价值
        /// </summary>
        private void CreateAiJudge()
        {
	        for (int i = 0; i < Bidders.Count; i++)
	        {
		        var bidder = EntityManager.Instance.Get(Bidders[i]);
		        var ai = bidder.GetComponent<AIComponent>().GetKnowledge();
		        float minValue = ai.DeviationMin;
		        float maxValue = ai.DeviationMax;
		        var judge = AllPrice * Random.Range(minValue,maxValue);
		        ai.Judge = judge;
	        }

        }

        
        /// <summary>
        /// AI喊价
        /// </summary>
        /// <param name="id"></param>
        /// <param name="decision"></param>
        private void AIAuction(long id, AIDecision decision)
        {
	        var type = decision.Tactic;
	        if (type == AITactic.Sidelines)
	        {
		        Log.Error("<color=#88FFEF>[Auction]</color> 喊价时不能观望");
		        return;
	        }
	        var bidder = EntityManager.Instance.Get<Bidder>(id);
	        var ai = bidder.GetComponent<AIComponent>().GetKnowledge();
	        var money = ai.Money;
	        switch (type)
	        {
		        case AITactic.LowWeight:
			        if (money < LowAuction)
			        {
				        Log.Error("<color=#88FFEF>[Auction]</color> 钱不够");
				        return;
			        }
			        LastAuctionPrice = LowAuction;
			        break;
		        case AITactic.MediumWeight:
			        if (money < MediumAuction)
			        {
				        Log.Error("<color=#88FFEF>[Auction]</color> 钱不够");
				        return;
			        }
			        LastAuctionPrice = MediumAuction;
			        break;
		        case AITactic.HighWeight:
			        if (money < HighAuction)
			        {
				        Log.Error("<color=#88FFEF>[Auction]</color> 钱不够");
				        return;
			        }
			        LastAuctionPrice = HighAuction;
			        break;
		        case AITactic.AllIn:
			        if (money <= HighAuction)
			        {
				        Log.Error("<color=#88FFEF>[Auction]</color> AllIn必须高于最高叫价");
				        return;
			        }
			        LastAuctionPrice = money;
			        break;
	        }
	        
	        Log.Info("<color=#88FFEF>[Auction]</color> "+id + " OnAuction "+type);
	        //消极次数更新
	        ai.BidCount--;
	        //复仇次数
	        if (LastAuctionPlayerId == ai.RevengeTarget)
	        {
		        ai.RevengeCount--;
		        if (ai.RevengeCount <= 0)
		        {
			        ai.RevengeTarget = -1;
		        }
	        }

	        //看看抬价次数是不是没了
	        if (ai.IsRaisePrice)
	        {
		        ai.RaisePriceCount--;
		        if (ai.RaisePriceCount <= 0)
		        {
			        ai.IsRaisePrice = false;
		        }
	        }
	        cancellationToken?.Cancel();
	        hostCancelToken?.Cancel();
	        ai.LastBidTime = GameTimerManager.Instance.GetTimeNow();
	        var head = bidder.GetComponent<GameObjectHolderComponent>()?.GetCollectorObj<Transform>("Head");
	        if (!int.TryParse(decision.Emoji, out var emoji))
	        {
		        emoji = -1;
	        }
	        if (head != null)
	        {
		        cancellationToken = new ETCancellationToken();
		        UIManager.Instance.OpenBox<UIBubbleItem,UIBubbleItem.BubbleData, int, ETCancellationToken>(UIBubbleItem.PrefabPath, 
			        new UIBubbleItem.BubbleData
			        {
				        front = null,
				        end = I18NManager.Instance.TranslateMoneyToStr(LastAuctionPrice),
				        emoji = emoji,
				        worldSpace = head.position,
				        isPlayer = false,
				        anim = !Skip,
				        iconType = Mathf.Min((int) type, 3),
				        raiseBubble = IsRaising,
			        },
			        15000, cancellationToken, UILayerNames.SceneLayer).Coroutine();
	        }

	        AfterAuction(id, type);
	        SetState(AuctionState.AIThink);
        }


        /// <summary>
        /// 喊价后数据处理
        /// </summary>
        private void AfterAuction(long id, AITactic type)
        {
	        SoundManager.Instance.PlaySound("Audio/Game/bid.mp3");
	        AuctionCount++;
	        if (LastAuctionPlayerId == Player.Id)
	        {
		        PlayerAuctionCount++;
		        Player.GetComponent<CasualActionComponent>().SetEnable(false);
		        if (IsRaising)
		        {
			        RaiseSuccessCount++;
			        if (RaiseSuccessCount > 0)
			        {
				        var addon = Config.Auction1 + Config.RaiseAuctionAddon * RaiseCount;
				        if (LastAuctionPriceType == AITactic.MediumWeight)
				        {
					        addon = Config.Auction2 + Config.RaiseAuctionAddon * RaiseCount;
				        }
				        else if (LastAuctionPriceType == AITactic.HighWeight)
				        {
					        addon = Config.Auction3 + Config.RaiseAuctionAddon * RaiseCount;
				        }

				        Report.RaisePriceStr = Report.RaisePrice + addon;
			        }
			        Log.Info("<color=#88FFEF>[Auction]</color> "+ "抬价成功"+RaiseSuccessCount);
		        }
	        }
	        else if (LastAuctionPlayerId > 0)
	        {
		        var bidder = EntityManager.Instance.Get<Entity>(LastAuctionPlayerId);
		        bidder.GetComponent<CasualActionComponent>().SetEnable(false);
	        }
	        
	        if (id == Player.Id && IsRaising)
	        {
		        var count = RaiseSuccessCount + 1;
		        var addon = Config.Auction1 + Config.RaiseAuctionAddon * RaiseCount;
		        if (LastAuctionPriceType == AITactic.MediumWeight)
		        {
			        addon = Config.Auction2 + Config.RaiseAuctionAddon * RaiseCount;
		        }
		        else if (LastAuctionPriceType == AITactic.HighWeight)
		        {
			        addon = Config.Auction3 + Config.RaiseAuctionAddon * RaiseCount;
		        }

		        var preview = Report.RaisePrice + addon;
		        var mul = AuctionHelper.GetRaiseMul(RaiseSuccessCount);
		        var x = mul != 1 ? "x" + AuctionHelper.GetRaiseMul(RaiseSuccessCount) : "";
		        if (count <= 1)
		        {
			        Messager.Instance.Broadcast(0, MessageId.AssistantTalk, I18NManager.Instance.I18NGetParamText(
				        I18NKey.Assistant_43, I18NManager.Instance.TranslateMoneyToStr(preview) + x), true);
		        }
		        else
		        {
			        Messager.Instance.Broadcast(0, MessageId.AssistantTalk,
				        I18NManager.Instance.I18NGetParamText(I18NKey.Assistant_46, count,
					        I18NManager.Instance.TranslateMoneyToStr(preview) + x), true);
		        }
	        }

	        if (IsRaising)
	        {
		        RaiseCount++;
		        var volume = SceneManager.Instance?.GetCurrentScene<MapScene>()?.Volume;
		        if (volume != null && volume.sharedProfile.TryGet<ActionLineVolume>(out var actionLineVolume))
		        {
			        var actionLineMaxCount = GlobalConfigCategory.Instance.GetFloat("ActionLineMaxCount", 10);
			        actionLineVolume.tintLerp.value = Mathf.Clamp01(RaiseCount / actionLineMaxCount);
		        }
	        }
	        lastAuctionAllPlayers.Add(id);
	        LastAuctionPlayerId = id;
	        UpdateFollow(type);
	        if (LastAuctionPlayerId == Player.Id)
	        {
		        Player.GetComponent<CasualActionComponent>().SetEnable(true);
	        }
	        else if (LastAuctionPlayerId > 0)
	        {
		        var bidder = EntityManager.Instance.Get<Entity>(LastAuctionPlayerId);
		        bidder.GetComponent<CasualActionComponent>().SetEnable(true);
	        }
	        for (int i = 0; i < Bidders.Count; i++)
	        {
		        var bidder = EntityManager.Instance.Get(Bidders[i]);
		        var ai = bidder.GetComponent<AIComponent>().GetKnowledge();
		        //高价震慑计算
		        if (type == AITactic.HighWeight || type == AITactic.AllIn)
		        {
			        if (Bidders[i] == id)
			        {
				        ai.IsHighPriceDeterrence = false;
			        }
			        else
			        {
				        if (IsRaising)
				        {
					        ai.IsHighPriceDeterrence = ai.Config.ShockRaise > 0 && Random.Range(0, 1f) < ai.Config.ShockRaise;
				        }
				        else
				        {
					        ai.IsHighPriceDeterrence = ai.Config.Shock > 0 && Random.Range(0, 1f) < ai.Config.Shock;
				        }
			        }
		        }
				//如果场上的价格已经超过了玩家预判的最大值，且上一次是玩家叫价，那么npc有概率生气
		        ai.IsAnger = LastAuctionPlayerId == Player.Id && LastAuctionPrice > AllPrice * SysJudgePriceMax &&
		                     GetFollowWithAi(Bidders[i]) >= ai.Config.AngerTimes &&
		                     Random.Range(0f, 1f) < ai.Config.AngerProp;
	        }

	        LastAuctionTime = GameTimerManager.Instance.GetTimeNow() - startWaitUserTime;

	        if (!IsRaising && LastAuctionPrice >= AllPrice * SysJudgePriceMax)
	        {
		        IsRaising = true;
		        Messager.Instance.Broadcast(0, MessageId.AssistantTalk,
			        I18NManager.Instance.I18NGetText(I18NKey.Assistant_41), true);
		        if (LastAuctionPlayerId == Player.Id)
		        {
			        RaiseSuccessCount = -1;
		        }
	        }
	        var host = EntityManager.Instance.Get(HostId);
	        host.GetComponent<CasualActionComponent>().PlayAnim("Stand_Idle1");
	        
	        
	        // if (HighAuction > PlayerDataManager.Instance.TotalMoney)
	        // {
		       //  Messager.Instance.Broadcast(0, MessageId.AssistantTalk,
			      //   I18NManager.Instance.I18NGetText(I18NKey.Assistant_51), true);
	        // }

	        if (RandomDrop == 0)
	        {
		        //大转盘
		        if (GameSetting.AlwaysTurnTable || TurntableRewardsConfigCategory.Instance.TryGet(IAuctionManager.Instance.Level,
			            PlayerDataManager.Instance.RestaurantLv, out _) && PlayerDataManager.Instance.RandomTurnTable())
		        {
			        CreateDropItem().Coroutine();
		        }
	        }
        }

        /// <summary>
        /// 创建落下的物品
        /// </summary>
        private async ETTask CreateDropItem()
        {
	        DestroyDropItem();
	        var target = mapScene.Collector.Get<Transform>("RandomBox");
	        if (target != null)
	        {
		        var turnTable = EntityManager.Instance.CreateEntity<Animal, int>(GameConst.TurnTableUnitId);
		        turnTable.Position = target.position;
		        turnTable.Rotation = Quaternion.identity;
		        turnTable.LocalScale = Vector3.one;
		        RandomDrop = turnTable.Id;
		        var ghc = turnTable.GetComponent<GameObjectHolderComponent>();
		        await ghc.WaitLoadGameObjectOver();
		        ghc.EntityView.SetParent(target.transform);
		        var endPos = ghc.EntityView.position;
		        var startPos = ghc.EntityView.position + Vector3.up * 5;
		        ghc.EntityView.position = startPos;
		        var timeStart = TimerManager.Instance.GetTimeNow();
		        while (true)
		        {
			        await TimerManager.Instance.WaitAsync(1);
			        var timeNow = TimerManager.Instance.GetTimeNow();
			        var during = (timeNow - timeStart) / 1000f;
			        var pos = startPos + 4.9f * during * during * Vector3.down;
			        ghc.EntityView.position = pos;
			        if (pos.y < endPos.y || RandomDrop == 0)
			        {
				        break;
			        }
		        }
		        ghc.EntityView.localPosition = Vector3.zero;
	        }
	       
        }

        private void DestroyDropItem()
        {
	        if (RandomDrop != 0)
	        {
		        EntityManager.Instance.Remove(RandomDrop);
		        RandomDrop = 0;
	        }
        }
        /// <summary>
        /// 尝试打开落下的物品
        /// </summary>
        private void CheckClick()
        {
	        if (InputManager.Instance.GetKey(GameKeyCode.NormalAttack))
	        {
		        var mousePos = InputManager.Instance.GetLastTouchPos();
		        if (InputManager.Instance.IsPointerOverUI(mousePos)) return;
		        Physics.SyncTransforms();
		        Ray ray = CameraManager.Instance.MainCamera().ScreenPointToRay(mousePos);

		        var len = PhysicsHelper.RaycastNonAllocEntity(ray.origin, ray.direction, 100,
			        new EntityType[] {EntityType.Animal}, out long[] ids);
		        if (len > 0)
		        {
			        var box = EntityManager.Instance.Get<Animal>(ids[0]);
			        if (box != null)
			        {
				        var ghc = box.GetComponent<GameObjectHolderComponent>();
				        GameTimerManager.Instance.SetTimeScale(0);
				        UIManager.Instance.OpenWindow<UITurntableView>(UITurntableView.PrefabPath).Coroutine();
				        var size = Mathf.Max(box.Config.Size[0], box.Config.Size[2]);
				        AuctionHelper.PlayFx(GameConst.SmokePrefab, ghc.EntityView.position + Vector3.back * size / 2)
					        .Coroutine();
				        box.Dispose();
			        }
		        }
	        }
        }
                
        /// <summary>
        /// 获取玩家与ai抬价次数
        /// </summary>
        /// <param name="aiId"></param>
        private int GetFollowWithAi(long aiId)
        {
	        if (lastAuctionAllPlayers.Count <= 1)
	        {
		        return 0;
	        }

	        int total = 0;
	        for (int i = lastAuctionAllPlayers.Count - 1; i >= 0; i--)
	        {
		        long id = lastAuctionAllPlayers[i];
		        if (id == Player.Id)
		        {
			        total++;
		        }
		        else if(id != aiId)//中断一次就不叫抬价了
		        {
			        break;
		        }
	        }
	        return total;
        }
        
        /// <summary>
        /// 更新连续跟风次数
        /// </summary>
        /// <param name="type"></param>
        private void UpdateFollow(AITactic type)
        {
	        LastAuctionPriceType = type;
	        //中高价，就更新连续跟风次数
	        if (type != AITactic.LowWeight)
	        {
		        continuousFollowCount++;
		        forceUpCount = 0;
	        }
	        else
	        {
		        continuousFollowCount = 0;
		        forceUpCount++;
	        }

	        for (int i = 0; i < Bidders.Count; i++)
	        {
		        var bidder = EntityManager.Instance.Get(Bidders[i]);
		        var bidderC = bidder.GetComponent<BidderComponent>();
		        var ai = bidder.GetComponent<AIComponent>().GetKnowledge();
		        if (continuousFollowCount >= bidderC.Config.Follow && !ai.IsFollow)
		        {
			        ai.IsFollow = true;
			        //跟风后会扩大他的估价
			        ai.DeviationMin += bidderC.Config.FollowExpand[0];
			        ai.DeviationMax += bidderC.Config.FollowExpand[1];

			        //重新估价
			        float random = Random.Range(bidderC.Config.FollowExpand[0], bidderC.Config.FollowExpand[1]);
			        ai.Judge = (1 + random) * ai.Judge;
		        }
				//在这里判断该ai是否被激活了诱导抬价
		        if (forceUpCount > bidderC.Config.ForceUp && !ai.IsRaisePrice && LastAuctionPlayerId != Bidders[i])
		        {
			        ai.IsRaisePrice = true;
			        ai.RaisePriceCount = bidderC.Config.Induce;
		        }
	        }
        }

        /// <summary>
        /// 当有人拍走了复仇者的，且赚钱了，则下一把开始对他复仇
        /// </summary>
        private void UpdateRevenge(bool isWin)
        {
	        if (lastAuctionAllPlayers.Count <= 1)
	        {
		       return;
	        }
	        var lastId = LastAuctionPlayerId;
	        var preId = lastAuctionAllPlayers[lastAuctionAllPlayers.Count - 2];
	        var winId = isWin ? lastId : preId;
	        var failId = isWin ? preId : lastId;
	        if (failId == Player.Id) return;//玩家输了
	        var bidder = EntityManager.Instance.Get(failId);
	        var bidderC = bidder.GetComponent<BidderComponent>();
	        var ai = bidder.GetComponent<AIComponent>().GetKnowledge();
	        if (bidderC.Config.Revenge == Player.Id) 
	        {
		        ai.RevengeTarget = -1;
		        ai.RevengeCount = 0;
	        }
	        else
	        {
		        ai.RevengeTarget = winId;
		        ai.RevengeCount = bidderC.Config.Revenge;
	        }
        }

        /// <summary>
        /// 更新Npc金币,并获取抬价奖励
        /// </summary>
        /// <returns>玩家抬价获取金币改变量</returns>
        private void UpdateNpcMoney()
        {
	        if(LastAuctionPlayerId > 0 && LastAuctionPlayerId != Player.Id)
	        {
		        //npc拍到了
		        var bidder = EntityManager.Instance.Get(LastAuctionPlayerId);
		        if (bidder == null) return;
		        var ai = bidder.GetComponent<AIComponent>().GetKnowledge();
		        ai.Money += AllPrice - LastAuctionPrice;
	        }
        }

        /// <summary>
        /// 主持人气泡
        /// </summary>
        /// <param name="front"></param>
        /// <param name="end"></param>
        private void UpdateHostSay(string front, string end)
        {
	        if(isDispose) return;
	        var host = EntityManager.Instance.Get(HostId);
	        var head = host.GetComponent<GameObjectHolderComponent>()?.GetCollectorObj<Transform>("Head");
	        if (head != null)
	        {
		        hostCancelToken?.Cancel();
		        hostCancelToken = new ETCancellationToken();
		        UIManager.Instance.OpenBox<UIBubbleItem,UIBubbleItem.BubbleData, int, ETCancellationToken>(UIBubbleItem.PrefabPath, 
			        new UIBubbleItem.BubbleData
			        {
				        front = front,
				        end = end,
				        emoji = -1,
				        worldSpace = head.position,
				        isPlayer = false,
				        anim = !Skip,
				        iconType = 3
			        },
			        HostSayInterval + 100, hostCancelToken, UILayerNames.SceneLayer).Coroutine();
	        }
	        host.GetComponent<CasualActionComponent>().PlayAnim("Stand_Idle2");
        }

        /// <summary>
        /// 刷新总价（如玩家购买后应用情报、触发鉴定后
        /// </summary>
        private void RefreshPrice()
        {
	        AllPrice = 0;
	        GameInfoConfig infoCfg = GetFinalGameInfoConfig();
	        for (int i = 0; i < Boxes.Count; i++)
	        {
		        var box = EntityManager.Instance.Get<Box>(Boxes[i]);
		        AllPrice += box.GetFinalPrice(infoCfg);
	        }
	        Report.AllPriceStr = AllPrice;
	        Log.Info("刷新总价" + AllPrice.Value);
        }

        private async ETTask CreateUfo()
        {
	        if (ufoId > 0) EntityManager.Instance.Remove(ufoId);
	        if (GlobalConfigCategory.Instance.TryGetInt("UFOPercent", out var percent) &&
	            Random.Range(0, 100) < percent)
	        {
		        Animal ufo = EntityManager.Instance.CreateEntity<Animal, int>(GameConst.UFOUnitId);
		        ufoId = ufo.Id;
		        var ghc = ufo.GetComponent<GameObjectHolderComponent>();
		        await ghc.WaitLoadGameObjectOver();
		        var transform = mapScene.Collector.Get<Transform>("UFORoot");
		        transform.gameObject.SetActive(false);
		        ghc.EntityView.parent = transform;
		        ghc.EntityView.localPosition = Vector3.zero;
		        ghc.EntityView.rotation = Quaternion.identity;
		        ghc.EntityView.localScale = Vector3.one;
	        }
        }

        // private async ETTask CreateEventSmoke()
        // {
	       //  if (EventSmoke != null) return;
	       //  
	       //  var root = mapScene.Collector.Get<Transform>("smoke");
	       //  if (root != null)
	       //  {
		      //   EventSmoke = await GameObjectPoolManager.GetInstance().GetGameObjectAsync(GameConst.EventSmokePrefab);
		      //   EventSmoke.transform.SetParent(root);
		      //   EventSmoke.transform.localPosition = Vector3.zero;
	       //  }
	       //
        // }
        //
        // private void RecycleEventSmoke()
        // {
	       //  if (EventSmoke != null)
	       //  {
		      //   GameObjectPoolManager.GetInstance().RecycleGameObject(EventSmoke);
		      //   EventSmoke = null;
	       //  }
        // }
    }
}