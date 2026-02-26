using System;
using UnityEngine;
using UnityEngine.Playables;
using Random = UnityEngine.Random;

namespace TaoTie
{
    public partial class AuctionManager: IAuctionManager
    {
	    /// <summary>
        /// 强行退出
        /// </summary>
        public void ForceAllOver()
        {
            isDispose = true;
            SceneManager.Instance.SwitchScene<HomeScene>().Coroutine();
        }
        
        /// <summary>
        /// 玩家出价
        /// </summary>
        /// <param name="type"></param>
        public void UserAuction(AITactic type)
        {
	        if(AState != AuctionState.WaitUser) return;
	        if (type == AITactic.Sidelines || type == AITactic.AllIn)
	        {
		        return;
	        }
	        
	        var money = PlayerDataManager.Instance.TotalMoney;
	        switch (type)
	        {
		        case AITactic.LowWeight:
			        if (money < LowAuction)
			        {
				        UIManager.Instance.OpenBox<UIToast, I18NKey>(UIToast.PrefabPath, I18NKey.Notice_Common_LackOfMoney).Coroutine();
				        // Log.Error("<color=#88FFEF>[Auction]</color> 钱不够");
				        return;
			        }

			        playerLowAuctionCount++;
			        LastAuctionPrice = LowAuction;
			        break;
		        case AITactic.MediumWeight:
			        if (money < MediumAuction)
			        {
				        UIManager.Instance.OpenBox<UIToast, I18NKey>(UIToast.PrefabPath, I18NKey.Notice_Common_LackOfMoney).Coroutine();
				        // Log.Error("<color=#88FFEF>[Auction]</color> 钱不够");
				        return;
			        }

			        playerMidAuctionCount++;
			        LastAuctionPrice = MediumAuction;
			        break;
		        case AITactic.HighWeight:
			        if (money < HighAuction)
			        {
				        UIManager.Instance.OpenBox<UIToast, I18NKey>(UIToast.PrefabPath, I18NKey.Notice_Common_LackOfMoney).Coroutine();
				        // Log.Error("<color=#88FFEF>[Auction]</color> 钱不够");
				        return;
			        }

			        playerHighAuctionCount++;
			        LastAuctionPrice = HighAuction;
			        break;
	        }
	        cancellationToken?.Cancel();
	        hostCancelToken?.Cancel();
	        AfterAuction(Player.Id, type);

	        cancellationToken?.Cancel();
	        var head = Player.GetComponent<GameObjectHolderComponent>()?.GetCollectorObj<Transform>("Head");
	        if (head != null)
	        {
		        cancellationToken = new ETCancellationToken();
		        UIManager.Instance.OpenBox<UIBubbleItem,UIBubbleItem.BubbleData, int, ETCancellationToken>(UIBubbleItem.PrefabPath, 
			        new UIBubbleItem.BubbleData
			        {
				        front = null,
				        end = I18NManager.Instance.TranslateMoneyToStr(LastAuctionPrice),
				        emoji = -1,
				        worldSpace = head.position,
				        isPlayer = true,
				        anim = true,
				        iconType = (int) type,
				        raiseBubble = IsRaising,
			        },
			        15000, cancellationToken, UILayerNames.SceneLayer).Coroutine();
	        }
	        SetState(AuctionState.AIThink);
        }

        /// <summary>
        /// 进行下一场
        /// </summary>
        public void RunNextStage()
        {
	        if (ufoId > 0)
	        {
		        if (LastAuctionPlayerId != Player.Id && GlobalConfigCategory.Instance.TryGetInt("UFOPickPlayerPercent", out int percent)
		            && Random.Range(0,100) < percent)
		        {
			        UFOPickPlayer().Coroutine();
			        return;
		        }

		        var index = Random.Range(0, Bidders.Count);
		        long id = Bidders[0];
		        for (int i = 0; i < Bidders.Count; i++)
		        {
			        if(Bidders[i] == LastAuctionPlayerId) break;
			        id = Bidders[index];
		        }
		        UFOPickUp(id).Coroutine();
	        }
	        
	        if (LastAuctionPlayerId == Player.Id)
	        {
		        for (int i = 0; i < Boxes.Count; i++)
		        {
			        var box = EntityManager.Instance.Get<Box>(Boxes[i]);
			        if (box?.BoxType == BoxType.Task)
			        {
				        PlayerDataManager.Instance.AddTaskStep(box.ItemId, 1, 0);
			        }
		        }
	        }
	        PlayerDataManager.Instance.RemoveUnlockList(Report.ContainerId, ItemType.None);
	        RefreshWinLossAnim(false);
	        if (StageConfigCategory.Instance.GetLevelConfigByLvAndStage(Level, Stage+1) == null)
	        {
		        SetState(AuctionState.AllOverAnim);
	        }
	        else
	        {
		        SetState(AuctionState.ReEnterAnim);
	        }
        }

        /// <summary>
        /// 设置鉴定结果
        /// </summary>
        /// <param name="configId"></param>
        /// <param name="newId"></param>
        public void SetAppraisalResult(int configId, int newId)
        {
	        var boxes = Boxes;
	        for (int i = 0; i < boxes.Count; i++)
	        {
		        var box = EntityManager.Instance.Get<Box>(boxes[i]);
		        if (box.ItemId == configId)
		        {
			        box.SetAppraisalResult(newId);
			        Report.PlayData[i] = box.ItemResult;
			        RefreshPrice();
			        RefreshWinLossAnim(true);
			        break;
		        }
	        }
        }
        
        /// <summary>
        /// 设置小游戏结果
        /// </summary>
        /// <param name="configId"></param>
        /// <param name="newPrice"></param>
        public void SetMiniGameResult(int configId, BigNumber newPrice)
        {
	        var boxes = Boxes;
	        for (int i = 0; i < boxes.Count; i++)
	        {
		        var box = EntityManager.Instance.Get<Box>(boxes[i]);
		        if (box.ItemId == configId)
		        {
			        box.SetMiniGameResult(newPrice);
			        Report.PlayData[i] = box.Price.Value;
			        RefreshPrice();
			        RefreshWinLossAnim(true);
			        break;
		        }
	        }
        }

        /// <summary>
        /// 根据当前状态判断是否应用情报并返回
        /// </summary>
        public GameInfoConfig GetFinalGameInfoConfig(bool ignoreId = false)
        {
	        if (Player != null && (ignoreId || LastAuctionPlayerId == Player.Id) && GameInfoId > 0)
	        {
		        if (GameInfoConfig.Condition == (int) GameInfoConditionType.MinRaiseCount)
		        {
			        if (GameInfoConfig.Content.Length <= 0 || RaiseSuccessCount < GameInfoConfig.Content[0])
				        return null;
		        }
		        else if (GameInfoConfig.Condition == (int) GameInfoConditionType.MaxAuctionCount)
		        {
			        if (GameInfoConfig.Content.Length <= 0 || PlayerAuctionCount > GameInfoConfig.Content[0])
				        return null;
		        }

		        if (AState == AuctionState.OpenBox
		            || AState == AuctionState.ExitAnim
		            || AState == AuctionState.Over) //到了开箱阶段就可以应用
		        {
			        return GameInfoConfig;
		        }
		        else
		        {
			        return null;
		        }
	        }

	        return null;
        }
        
        public void SelectGameInfo(int id)
        {
	        GameInfoId = id;
        }
        
        public void SelectDice(int id, Action onSelectOver)
        {
	        DiceId = id;
	        if (DiceConfig.Type == 1 && AuctionReports != null)
	        {
		        var container = ContainerConfigCategory.Instance.Get(DiceConfig.Param);
		        if (container != null)
		        {
			        Log.Info("骰子指定集装箱为" + DiceConfig.Param);
			        if (container.Level != Level)
			        {
				        Log.Error("骰子指定集装箱不属于当前场次，骰子Id = " + DiceId);
			        }
			        for (int i = Stage; i < AuctionReports.Length; i++)
			        {
				        AuctionReports[i].ContainerId = DiceConfig.Param;
			        }

			        if (Stage == 0)
			        {
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
					        onSelectOver?.Invoke();
				        }).Coroutine();
			        }
			        else
			        {
				        onSelectOver?.Invoke();
			        }
		        }
		        else
		        {
			        Log.Error("骰子指定集装箱不存在，集装箱Id =" + DiceConfig.Param + "，骰子Id = " + DiceId);
			        onSelectOver?.Invoke();
		        }
	        }
	        else
	        {
		        onSelectOver?.Invoke();
	        }
        }
        
        
        /// <summary>
        /// 播放动作
        /// </summary>
        /// <param name="play"></param>
        public void RefreshWinLossAnim(bool play)
        {
	        if (centerCharacter == null)
	        {
		        return;
	        }
	        bool winLoss = AllPrice >= LastAuctionPrice;
	        var ghc = centerCharacter.GetComponent<CasualActionComponent>();
	        ghc?.SetWinLoss(play ? (winLoss ? 1 : -1) : 0);
	        hostCancelToken?.Cancel();
	        if (play && LastAuctionPlayerId != Player.Id)
	        {
		        var head = centerCharacter.GetComponent<GameObjectHolderComponent>()?.GetCollectorObj<Transform>("Head");
		        if (head != null)
		        {
			        hostCancelToken = new ETCancellationToken();
			        UIManager.Instance.OpenBox<UIBubbleItem, UIBubbleItem.BubbleData, int, ETCancellationToken>(
				        UIBubbleItem.PrefabPath,
				        new UIBubbleItem.BubbleData
				        {
					        end = null,
					        front= I18NManager.Instance.I18NGetText(winLoss
						        ? I18NKey.Game_Win_Say
						        : I18NKey.Game_Loss_Say),
					        emoji = -1,
					        worldSpace = head.position,
					        isPlayer = false,
					        anim = true,
					        iconType = 3,
					        raiseBubble = false,
				        },
				        15000, hostCancelToken, UILayerNames.SceneLayer).Coroutine();
		        }
	        }
        }

        /// <summary>
        /// AI离场
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type">类型：0走开,1跑开</param>
        public void Leave(long id, int type)
        {
	        Bidders.Remove(id);
	        Blacks.Remove(id);
	        if (type == 0 || type == 1)
	        {
		        LevelAuction(id, type == 0).Coroutine();
	        }
	        else
	        {
		        EntityManager.Instance.Remove(id);
	        }
        }
        
        public int GetLevelCount()
        {
	        return decisions.Length - Bidders.Count;
        }
    }
}