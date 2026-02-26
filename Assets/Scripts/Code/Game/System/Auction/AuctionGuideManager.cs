using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace TaoTie
{
    public partial class AuctionGuideManager: IManager<GuideScene>
    {
	    private int HostSayStart;
	    private int HostSayInterval;
	    private int[] MiniPlayPercent;
	    
	    private bool isDispose;
	    
        private GuideScene mapScene;
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
        public int GameInfoId { get; } = -1;
        /// <summary>
        /// 命运骰子Id
        /// </summary>
        public int DiceId{ get; } = 0;
        public StageConfig Config => StageConfigCategory.Instance.GetLevelConfigByLvAndStage(Level, Stage);
        public LevelConfig LevelConfig => LevelConfigCategory.Instance.Get(Level);
        public GameInfoConfig GameInfoConfig => GameInfoConfigCategory.Instance.Get(GameInfoId);
        public DiceConfig DiceConfig => DiceConfigCategory.Instance.Get(DiceId);
        public StartEventConfig StartEventConfig => null;
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
        public List<long> Blacks => null;
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
        public int RaiseCount{ get; private set; } //抬价次数，进入抬价那次不算次数
        public int PlayerAuctionCount { get; private set; } //玩家出价次数
        public int AuctionCount { get; private set;  } //总出价次数
        public bool Skip{ get; set; }//快速跳过
        public bool IsAllPlayBox => false;
        
        private int continuousFollowCount;//连续跟风次数
        private int forceUpCount;//抬价次数
        private long startWaitUserTime;//开始等待时间
        private int playerLowAuctionCount;
        private int playerMidAuctionCount;
        private int playerHighAuctionCount;
        private int hostSayCount = 0;//拍卖师倒计时次数

        public bool HasTaskItem { get; private set; } = false;//任务物品
        public bool HasMiniPlayItem { get; private set; }//小玩法物品

        private ETCancellationToken cancellationToken;
        private string curTexture;
        
        #region IManager
        
        public void Init(GuideScene map)
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
	        Messager.Instance.RemoveListener<string,double,double>(0,MessageId.ClipStartPlay,ClipStartPlay);
            if (!string.IsNullOrEmpty(curTexture))
            {
                ImageLoaderManager.Instance.ReleaseImage(curTexture);
                curTexture = null;
            }
            cancellationToken?.Cancel();
            cancellationToken = null;
            centerCharacter = null;
            isDispose = true;
            Bidders.Clear();
            Npcs.Clear();
            SetState(AuctionState.Free);
            var phc = Player.GetComponent<GameObjectHolderComponent>();
            var flag = phc.GetCollectorObj<GameObject>("Flag");
            flag?.SetActive(true);
            Player = null;
            IAuctionManager.Instance = null;
            PerformanceManager.Instance.SetLowFrame();
        }
        
        #endregion
        
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
		        var guide = GuidanceStageConfigCategory.Instance.Get(i + 1);
		        AuctionReports[i].ContainerId = guide.ContainerId;
		        AuctionReports[i].Index = i;
	        }

	        if (!string.IsNullOrEmpty(curTexture))
	        {
		        ImageLoaderManager.Instance.ReleaseImage(curTexture);
	        }
	        curTexture = ContainerConfigCategory.Instance.Get(AuctionReports[0].ContainerId).Skin;
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
	        AllPrice = 0;
	        var containerId = Report.ContainerId;

	        var guidance = GuidanceStageConfigCategory.Instance.Get(Stage);
	        using var itemConfigs = ListComponent<ItemConfig>.Create();
	        for (int i = 0; i < guidance.Items.Length; i++)
	        {
		        var itemConfig = ItemConfigCategory.Instance.Get(guidance.Items[i]);
		        if (itemConfig == null)
		        {
			        Log.Error("空ItemConfig Stage="+Stage+", index = "+i);
			        continue;
		        }
		        itemConfigs.Add(itemConfig);
		        AllPrice += itemConfig.Price;
	        }
	        itemConfigs.Sort((a, b) =>
	        {
		        var unitA = UnitConfigCategory.Instance.Get(a.UnitId);
		        var unitB = UnitConfigCategory.Instance.Get(b.UnitId);
		        return (unitB?.Priority??0) - (unitA?.Priority??0);
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
	        
	        curTexture = ContainerConfigCategory.Instance.Get(containerId).Skin;
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
		        Log.Error("放不下");
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

	        Report.Items = new ItemConfig[pos.Length];
	        Report.BoxTypes = new BoxType[pos.Length];
	        Report.PlayData = new object[pos.Length];
	        Report.AllPriceStr = AllPrice;

	        for (int i = 0; i < pos.Length; i++)
	        {
		        var box = EntityManager.Instance.CreateEntity<Box, int>(itemConfigs[i].Id);
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
	        GuidanceManager.Instance.NoticeEvent("AI_Auction");
	        AfterAuction(id, type);
	        SetState(AuctionState.AIThink);
        }


        /// <summary>
        /// 喊价后数据处理
        /// </summary>
        private void AfterAuction(long id, AITactic type)
        {
	        SoundManager.Instance.PlaySound("Audio/Game/bid.mp3");
	        GuidanceManager.Instance.NoticeEvent("Any_Auction");
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
		        ShowTalk().Coroutine();
	        }

	        if (IsRaising)
	        {
		        RaiseCount++;
		        var volume = SceneManager.Instance?.GetCurrentScene<GuideScene>()?.Volume;
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
		        //对话冲突
		        GuidanceManager.Instance.NoticeEvent("Auction_Raise");
		        if (LastAuctionPlayerId == Player.Id)
		        {
			        RaiseSuccessCount = -1;
		        }
	        }
	        var host = EntityManager.Instance.Get(HostId);
	        host.GetComponent<CasualActionComponent>().PlayAnim("Stand_Idle1");
        }

        private async ETTask ShowTalk()
        {
	        await TimerManager.Instance.WaitAsync(500);
	        //新手引导在npc之后说
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
	        var mul = AuctionHelper.GetRaiseMul(count);
	        var x = mul != 1 ? "x" + AuctionHelper.GetRaiseMul(count) : "";
	        if (count <= 1)
	        {
		        Messager.Instance.Broadcast(0, MessageId.GuidanceTalk,
			        I18NManager.Instance.I18NGetParamText(I18NKey.Assistant_43,
				        I18NManager.Instance.TranslateMoneyToStr(preview) + x), 2000);
	        }
	        else
	        {
		        Messager.Instance.Broadcast(0, MessageId.GuidanceTalk,
			        I18NManager.Instance.I18NGetParamText(I18NKey.Assistant_46, count,
				        I18NManager.Instance.TranslateMoneyToStr(preview) + x), 2000);
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
		        cancellationToken?.Cancel();
		        cancellationToken = new ETCancellationToken();
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
			        HostSayInterval + 100, cancellationToken, UILayerNames.SceneLayer).Coroutine();
	        }
	        host.GetComponent<CasualActionComponent>().PlayAnim("Stand_Idle2");
        }
        
        /// <summary>
        /// 刷新总价（如玩家购买后应用情报、触发鉴定后
        /// </summary>
        private void RefreshPrice()
        {
	        AllPrice = 0;
	        for (int i = 0; i < Boxes.Count; i++)
	        {
		        var box = EntityManager.Instance.Get<Box>(Boxes[i]);
		        AllPrice += box.GetFinalPrice(null);
	        }
	        Report.AllPriceStr = AllPrice;
        }
        
        private async ETTask LevelAuction(long id, bool walk)
        {
	        var during = 10000;
	        var cha = EntityManager.Instance.Get<Character>(id);
	        var anim = cha.GetComponent<CasualActionComponent>();
	        anim.SetEnable(false);
	        anim.SetMove(walk ? 1 : 2);
	        cha.Rotation = Quaternion.LookRotation(-cha.Forward);
	        var timeStart = TimerManager.Instance.GetTimeNow();
	        var startPos = cha.Position;
	        var endPos = cha.Position + cha.Forward * 10;
	        while (true)
	        {
		        await TimerManager.Instance.WaitAsync(1);
		        var timeNow =  TimerManager.Instance.GetTimeNow();
		        cha.Position = Vector3.Lerp(startPos, endPos, (timeNow - timeStart) / (float)during);
		        if (timeNow - timeStart > during)
		        {
			        break;
		        }
	        }
	        EntityManager.Instance.Remove(id);
        }
    }
}