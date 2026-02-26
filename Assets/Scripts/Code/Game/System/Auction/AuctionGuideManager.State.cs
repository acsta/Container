using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace TaoTie
{
    public partial class AuctionGuideManager:IUpdate
    {
        private void SetState(AuctionState state)
        {
            if (AState != state)
            {
                AState = state;
                isEnterState = true;
                GuidanceManager.Instance.NoticeEvent("AuctionState_"+AState);
                Messager.Instance.Broadcast(0, MessageId.RefreshAuctionState,AState);
                Log.Info("<color=#88FFEF>[Auction]</color> Change state to " + state);
            }
        }

        public void Update()
        {
            if (isDispose) return;
            var lAState = AState;
            switch (AState)
            {
	            case AuctionState.Awake:
		            Awake();
		            break;
	            case AuctionState.Prepare:
                    Prepare();
                    break;
	            case AuctionState.EnterAnim:
		            PlayEnterAnim().Coroutine();
		            break;
                case AuctionState.Ready:
                    Ready();
                    break;
                case AuctionState.AIThink:
                    AIThink();
                    break;
                case AuctionState.WaitUser:
                    WaitUser();
                    break;
                case AuctionState.ExitAnim:
	                Skip = false;
                    ExitAnim();
                    break;
                case AuctionState.OpenBox:
	                Physics.SyncTransforms();
                    OpenBox();
                    break;
                case AuctionState.Over:
                    Over();
                    break;
	            case AuctionState.ReEnterAnim:
                    ReEnterAnim();
                    break;
                case AuctionState.AllOverAnim:
                    AllOverAnim();
                    break;
                case AuctionState.AllOver:
                    AllOver();
                    break;
                case AuctionState.RePrepare:
	                RePrepare();
	                break;
            }
            if(AState == lAState) isEnterState = false;
        }
        
        #region State

        /// <summary>
        /// 第一次进入前，情报，命运骰子
        /// </summary>
        private void Awake()
        {
	        if (!isEnterState) return;
	        Stage = 0;
	        
	        var host = EntityManager.Instance.CreateEntity<Host, int>(LevelConfig.Hoster);
            HostId = host.Id;
            var root = mapScene.Collector.Get<Transform>("Host");
            if (root != null)
            {
	            host.Position = root.position;
	            host.Rotation = root.rotation;
	            host.LocalScale = root.localScale;
            }
            using ListComponent<Transform> target = ListComponent<Transform>.Create();
            int i = 0;
            while (true)
            {
	            root = mapScene.Collector.Get<Transform>("Npc_"+i);
	            if (root != null)
	            {
		            target.Add(root);
		            i++;
	            }
	            else
	            {
		            break;
	            }
            }
            Player = EntityManager.Instance.CreateEntity<Player, int[]>(PlayerDataManager.Instance.Show);
            var cac = Player.AddComponent<CasualActionComponent>();
            cac.SetEnable(true);
            root = target[0];
            Player.Position = root.position;
            Player.Rotation = root.rotation;
            Player.LocalScale = root.localScale;
            Player.RootName = root.name;
            target.RemoveAt(0);

            var len = Mathf.Min(LevelConfig.AIIds.Length, target.Count);
            var reduce = Mathf.Max(0, target.Count - len);
            if (LevelConfig.NeedNpc == 1)
            {
	            ClothGenerateManager.Instance.Generate(target.Count, Level);
            }
            else
            {
	            ClothGenerateManager.Instance.Generate(len, Level);
            }
            for (i = 0; i < len; i++)
            {
                var bidder = EntityManager.Instance.CreateEntity<Bidder, int>(LevelConfig.AIIds[i]);
                Bidders.Add(bidder.Id);
                var index = Random.Range(0, target.Count - reduce);
                root = target[index];
                bidder.Position = root.position;
                bidder.Rotation = root.rotation;
                bidder.LocalScale = root.localScale;
                bidder.RootName = root.name;
                target.RemoveAt(index);
            }

            if (LevelConfig.NeedNpc == 1)
            {
	            for (i = 0; i < target.Count; i++)
	            {
		            var npc = EntityManager.Instance.CreateEntity<NPC>();
		            Npcs.Add(npc.Id);
		            root = target[i];
		            npc.Position = root.position;
		            npc.Rotation = root.rotation;
		            npc.LocalScale = root.localScale;
		            npc.RootName = root.name;
	            }
            }
            decisions = new AIDecision[Bidders.Count];
            WaitPrepare().Coroutine();
        }

        /// <summary>
        /// 第一次进入准备数据，开场动画等
        /// </summary>
        private void Prepare()
        {
	        if (!isEnterState)
	        {
		        if (UIManager.Instance.GetView<UIFirstGuidanceView>(1) == null)
		        {
			        SetState(AuctionState.EnterAnim);
		        }
		        return;
	        }
	        //生成集装箱
	        CreateContainer();
        }
        /// <summary>
        /// 再来一局
        /// </summary>
        private void RePrepare()
        {
	        if (!isEnterState) return;
	        Stage = 0;
	        //生成集装箱
	        CreateContainer();
	        SetState(AuctionState.ReEnterAnim);
        }
        /// <summary>
        /// 当前轮准备完成
        /// </summary>
        private void Ready()
        {
	        if (!isEnterState) return;
	        BigNumber prefabDeviation = 0;
	        if (Random.Range(1, 100) >= Config.AiDeviation)
	        {
		        using ListComponent<BigNumber> arr1 = ListComponent<BigNumber>.Create();
		        using ListComponent<int> arr2 = ListComponent<int>.Create();
		        int totalWight = 0;
		        foreach (var deviationPrefab in Config.DeviationPrefab)
		        {
			        if (deviationPrefab == null || deviationPrefab.Length < 2)
			        {
				        Log.Error("配置错误 LevelConfig.deviationPrefab == null || LevelConfig.deviationPrefab.Length < 2");
				        continue;
			        }
			        arr1.Add(deviationPrefab[0]);
			        totalWight += (int) deviationPrefab[1];
			        arr2.Add(totalWight);
		        }

		        if (totalWight > 1)
		        {
			        int ran = Random.Range(1, totalWight);
			        for (int i = 0; i < arr2.Count; i++)
			        {
				        if (ran < arr2[i])
				        {
					        prefabDeviation = arr1[i];
					        break;
				        }
			        }
		        }
	        }
	        
	        Log.Info("<color=#88FFEF>[Auction]</color> 本局调用预制误差" + prefabDeviation);
	        for (int i = 0; i < Bidders.Count; i++)
	        {
		        var bidder = EntityManager.Instance.Get(Bidders[i]);
		        var ai = bidder.GetComponent<AIComponent>();
		        ai.GetKnowledge().Ready(prefabDeviation);
	        }
	        if (LastAuctionPlayerId == Player.Id)
	        {
		        Player.GetComponent<CasualActionComponent>().SetEnable(false);
	        }
	        else if (LastAuctionPlayerId > 0)
	        {
		        var bidder = EntityManager.Instance.Get<Entity>(LastAuctionPlayerId);
		        bidder.GetComponent<CasualActionComponent>().SetEnable(false);
	        }

	        CreateRangePrice();
	        CreateAiJudge();
	        LastAuctionPlayerId = -1;
	        LastAuctionPrice = Config.BaseAuction;
	        lastAuctionAllPlayers.Clear();
	        LastAuctionPriceType = AITactic.Sidelines;
	        continuousFollowCount = 0;
	        forceUpCount = 0;
	        playerLowAuctionCount = 0;
	        playerMidAuctionCount = 0;
	        playerHighAuctionCount = 0;
	        LastHostSayCount = 0;
	        RaiseSuccessCount = 0;
	        RaiseCount = 0;
	        PlayerAuctionCount = 0;
	        AuctionCount = 0;
	        LastAuctionTime = 0;
	        IsRaising = false;
	        Skip = false;
	        
	        ShowReady().Coroutine();
        }

        /// <summary>
        /// 竞拍
        /// </summary>
        private void AIThink()
        {
	        bool has = GameSetting.RaiseCount > 0 && RaiseSuccessCount <= GameSetting.RaiseCount;
	        for (int i = 0; i < Bidders.Count; i++)
	        {
		        if (LastAuctionPlayerId == Bidders[i])
		        {
			        decisions[i].Tactic = AITactic.Sidelines;
			        continue;
		        }
		        var npc = EntityManager.Instance.Get(Bidders[i]);
		        var ai = npc.GetComponent<AIComponent>();
		        var res = ai.Think();
		        decisions[i] = res;
		        if (has && decisions[i].Tactic == AITactic.Sidelines && LastAuctionPlayerId == Player.Id)
		        {
			        decisions[i].Tactic = AITactic.LowWeight;
		        }
	        }

	        LastHostSayCount = hostSayCount;
	        hostSayCount = 0;
	        SetState(AuctionState.WaitUser);
        }

        /// <summary>
        /// 等待玩家或下一个AI竞价
        /// </summary>
        private void WaitUser()
        {
	        var timeNow = GameTimerManager.Instance.GetTimeNow();
	        if (isEnterState)
	        {
		        startWaitUserTime = timeNow;
		        return;
	        }

	        //计算AI叫价
	        var deltaTime = timeNow - startWaitUserTime;
	        for (int i = 0; i < Bidders.Count; i++)
	        {
		        if (deltaTime >= decisions[i].Delay || Skip)
		        {
			        if (decisions[i].Tactic != AITactic.Sidelines)
			        {
				        AIAuction(Bidders[i], decisions[i]);
				        if(AState != AuctionState.WaitUser) return;
			        }
			        else
			        {
				        if (int.TryParse(decisions[i].Emoji, out var emoji))
				        {
					        var npc = EntityManager.Instance.Get(Bidders[i]);
					        var head = npc.GetComponent<GameObjectHolderComponent>()?.GetCollectorObj<Transform>("Head");
					        if (head != null)
					        {
						        UIManager.Instance.OpenBox<UIEmojiItem, int, Vector3>(UIEmojiItem.PrefabPath, emoji,
							        head.position, UILayerNames.GameLayer).Coroutine();
						        decisions[i].Emoji = null;
					        }
				        }
			        }
		        }
	        }

	       
	        
	        
	        var guidance = GuidanceStageConfigCategory.Instance.Get(Stage);
	        //1 玩家出价后倒计时
	        if (guidance.HosterType == 1 && (RaiseSuccessCount < guidance.PlayerMaxRaiseCount || LastAuctionPlayerId != Player.Id)) return;
	        //2 AI出价后倒计时
	        if (guidance.HosterType == 2 && (RaiseSuccessCount < guidance.PlayerMaxRaiseCount || LastAuctionPlayerId <= 0 || LastAuctionPlayerId == Player.Id)) return;
	        //0 或其他正常倒计时
	        //计算主持人叫价
	        if (hostSayCount == 0)
	        {
		        if (deltaTime > HostSayStart || Skip)
		        {
			        hostSayCount++;
			        SoundManager.Instance.PlaySound("Audio/Game/countDown.mp3");
			        UpdateHostSay(null,I18NManager.Instance.I18NGetParamText(I18NKey.Text_Game_TimeDown1,
				        I18NManager.Instance.TranslateMoneyToStr(LastAuctionPrice)));
		        }
		       
	        }
	        else if (hostSayCount == 1)
	        {
		        if (deltaTime > HostSayStart+hostSayCount*HostSayInterval || Skip)
		        {
			        hostSayCount++;
			        SoundManager.Instance.PlaySound("Audio/Game/countDown.mp3");
			        UpdateHostSay(null,I18NManager.Instance.I18NGetParamText(I18NKey.Text_Game_TimeDown2,
				        I18NManager.Instance.TranslateMoneyToStr(LastAuctionPrice)));
		        }
	        }
	        else if (hostSayCount == 2)
	        {
		        if (deltaTime > HostSayStart+hostSayCount*HostSayInterval || Skip)
		        {
			        hostSayCount++;
			        SoundManager.Instance.PlaySound("Audio/Game/countDown.mp3");
			        UpdateHostSay(null,I18NManager.Instance.I18NGetParamText(I18NKey.Text_Game_TimeDown3,
				        I18NManager.Instance.TranslateMoneyToStr(LastAuctionPrice)));
		        }
	        }
	        else if (hostSayCount == 3)
	        {
		        if (deltaTime > HostSayStart+hostSayCount*HostSayInterval || Skip)
		        {
			        hostSayCount++;
			        SoundManager.Instance.PlaySound("Audio/Game/countDown.mp3");
			        if (LastAuctionPlayerId >= 0)
			        {
				        UpdateHostSay(null,I18NManager.Instance.I18NGetParamText(I18NKey.Text_Game_TimeDown0,
					        I18NManager.Instance.TranslateMoneyToStr(LastAuctionPrice)));
			        }
			        else
			        {
				        UpdateHostSay(I18NManager.Instance.I18NGetText(I18NKey.Text_Failed_Auction),null);
			        }
			        
			        cancellationToken?.Cancel();
			        SetState(AuctionState.ExitAnim);
		        }
	        }
        }

        /// <summary>
        /// 结算中
        /// </summary>
        private void ExitAnim()
        {
	        if (!isEnterState)
	        {
		        return;
	        }
	        
	        ExitAnimAsync().Coroutine();
        }
        

        /// <summary>
        /// 开箱
        /// </summary>
        private void OpenBox()
        {
	        OpenBoxAsync().Coroutine();
        }

        
        /// <summary>
        /// 结算
        /// </summary>
        private void Over()
        {
	        if (!isEnterState)
	        {
		        return;
	        }
	        UIItemsView view = UIManager.Instance.GetView<UIItemsView>(1);
	        view?.PlayWithoutAnim();
	        Messager.Instance.Broadcast<Transform,int>(0, MessageId.GuideBox, null, 0);
	        if (LastAuctionPlayerId > 0)
	        {
		        if (LastAuctionPlayerId == Player.Id)
		        {
			        RefreshPrice();
		        }
	        }
	        RefreshWinLossAnim(true);
	        UIManager.Instance.OpenWindow<UIButtonView, List<long>, bool>(UIButtonView.PrefabPath, OpenBoxes,
		        true, UILayerNames.GameLayer).Coroutine();
	        
	        if (!string.IsNullOrEmpty(curTexture))
	        {
		        ImageLoaderManager.Instance.ReleaseImage(curTexture);
		        curTexture = null;
	        }
        }

        /// <summary>
        /// 再次进入
        /// </summary>
        private void ReEnterAnim()
        {
	        if (isEnterState)
	        {
		        ReEnterAnimAsync().Coroutine();
	        }
        }
        
        /// <summary>
        /// 全部完成离场
        /// </summary>
        private void AllOverAnim()
        {
	        if (isEnterState)
	        {
		        AllOverAnimAsync().Coroutine();
	        }
        }
        
        /// <summary>
        /// 全部结束
        /// </summary>
        private void AllOver()
        {
            if(!isEnterState) return;
            PlayerDataManager.Instance.GuideSceneDone();
            UIManager.Instance
	            .OpenWindow<UIReportWin, AuctionReport[], int, bool>(UIReportWin.PrefabPath, AuctionReports, Level,
		            false).Coroutine();
        }

        
        #endregion
    }
}