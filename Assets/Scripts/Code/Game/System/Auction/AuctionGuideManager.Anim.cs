using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Playables;

namespace TaoTie
{
    public partial class AuctionGuideManager
    {
        private async ETTask WaitPrepare()
        {
	        var dir = mapScene.Collector.Get<PlayableDirector>("Character");
	        if (dir != null)
	        {
		        dir.extrapolationMode = DirectorWrapMode.Hold;
		        dir.Play();
		        await TimerManager.Instance.WaitFrameAsync();
		        dir.time = 0;
		        dir.Pause();
	        }
	        else
	        {
		        Log.Error("开场动画丢失");
	        }
	        using ListComponent<ETTask<bool>> tasks = ListComponent<ETTask<bool>>.Create();
	        var host = EntityManager.Instance.Get(HostId) as Host;
	        var hosthc = host.GetComponent<GameObjectHolderComponent>();
	        tasks.Add(hosthc.WaitLoadGameObjectOver());
	        var phc = Player.GetComponent<GameObjectHolderComponent>();
	        tasks.Add(phc.WaitLoadGameObjectOver());
	        for (int i = 0; i < Bidders.Count; i++)
	        {
		        var npc = EntityManager.Instance.Get(Bidders[i]);
		        var ghc = npc.GetComponent<GameObjectHolderComponent>();
		        tasks.Add(ghc.WaitLoadGameObjectOver());
	        }

	        await ETTaskHelper.WaitAll(tasks);
	        var flag = phc.GetCollectorObj<GameObject>("Flag");
	        flag?.SetActive(true);
	        await UIManager.Instance.OpenWindow<UIFirstGuidanceView>(UIFirstGuidanceView.PrefabPath);
	        SetState(AuctionState.Prepare);
        }

        /// <summary>
	    /// 场景加载完成后播放入场动画
	    /// </summary>
	    private async ETTask PlayEnterAnim()
        {
	        if(!isEnterState) return;
	        Stage++;
	        CreateItems();
	        PerformanceManager.Instance.SetHighFrame();
	        var dir = mapScene.Collector.Get<PlayableDirector>("Character");
	        if (dir != null)
            {
	            using DictionaryComponent<string, PlayableBinding> bindings =
		            DictionaryComponent<string, PlayableBinding>.Create();
	            foreach (var o in dir.playableAsset.outputs)
	            {
		            if(!bindings.ContainsKey(o.streamName))
			            bindings.Add(o.streamName, o);
	            }
	            var host = EntityManager.Instance.Get(HostId) as Host;
	            var hosthc = host.GetComponent<GameObjectHolderComponent>();
	            if (bindings.TryGetValue("HostTrack",out var playableBinding))
	            {
		            dir.SetGenericBinding(playableBinding.sourceObject,
			            hosthc.EntityView.GetComponent(playableBinding.outputTargetType));
	            }
	            if (bindings.TryGetValue("HostRoot",out playableBinding))
	            {
		            hosthc.EntityView.SetParent(dir.GetGenericBinding(playableBinding.sourceObject) as Transform);
	            }
	            
	            var phc = Player.GetComponent<GameObjectHolderComponent>();
	            if (bindings.TryGetValue(Player.RootName+ "Track",out playableBinding))
	            {
		            dir.SetGenericBinding(playableBinding.sourceObject,
			            phc.EntityView.GetComponent(playableBinding.outputTargetType));
	            }
	            if (bindings.TryGetValue(Player.RootName+"Root",out playableBinding))
	            {
		            phc.EntityView.SetParent(dir.GetGenericBinding(playableBinding.sourceObject) as Transform);
	            }
	            for (int i = 0; i < Bidders.Count; i++)
	            {
		            var npc = EntityManager.Instance.Get(Bidders[i]) as Bidder;
		            var ghc = npc.GetComponent<GameObjectHolderComponent>();
		            if (bindings.TryGetValue(npc.RootName+ "Track",out playableBinding))
		            {
			            dir.SetGenericBinding(playableBinding.sourceObject,
				            ghc.EntityView.GetComponent(playableBinding.outputTargetType));
		            }
		            if (bindings.TryGetValue(npc.RootName+"Root",out playableBinding))
		            {
			            ghc.EntityView.SetParent(dir.GetGenericBinding(playableBinding.sourceObject) as Transform);
		            }
	            }
	            for (int i = 0; i < Npcs.Count; i++)
	            {
		            var npc = EntityManager.Instance.Get(Npcs[i]) as NPC;
		            var ghc = npc.GetComponent<GameObjectHolderComponent>();
		            if (bindings.TryGetValue( npc.RootName+ "Track",out playableBinding))
		            {
			            dir.SetGenericBinding(playableBinding.sourceObject,
				            ghc.EntityView.GetComponent(playableBinding.outputTargetType));
		            }
		            if (bindings.TryGetValue(npc.RootName+"Root",out playableBinding))
		            {
			            ghc.EntityView.SetParent(dir.GetGenericBinding(playableBinding.sourceObject) as Transform);
		            }
	            }
	            dir.enabled = true;
	            dir.Play();
	            //dir.time = dir.duration - 1;
	            while (dir.time < dir.duration)
	            {
		            await TimerManager.Instance.WaitFrameAsync();
	            }
	
	            dir.enabled = false;
	            host.Position = hosthc.EntityView.position;
	            host.Rotation = hosthc.EntityView.rotation;
	            hosthc.EntityView.SetParent(EntityManager.Instance.GameObjectRoot, true);
	            
	            Player.Position = phc.EntityView.position;
	            Player.Rotation = phc.EntityView.rotation;
	            phc.EntityView.SetParent(EntityManager.Instance.GameObjectRoot, true);
	           
	            for (int i = 0; i < Bidders.Count; i++)
	            {
		            var npc = EntityManager.Instance.Get(Bidders[i]) as Bidder;
		            var ghc = npc.GetComponent<GameObjectHolderComponent>();
		            npc.Position = ghc.EntityView.position;
		            npc.Rotation = ghc.EntityView.rotation;
		            ghc.EntityView.SetParent(EntityManager.Instance.GameObjectRoot, true);
	            }
	            for (int i = 0; i < Npcs.Count; i++)
	            {
		            var npc = EntityManager.Instance.Get(Npcs[i]) as NPC;
		            var ghc = npc.GetComponent<GameObjectHolderComponent>();
		            npc.Position = ghc.EntityView.position;
		            npc.Rotation = ghc.EntityView.rotation;
		            ghc.EntityView.SetParent(EntityManager.Instance.GameObjectRoot, true);
	            }
            }
	        SetState(AuctionState.Ready);
	        PerformanceManager.Instance.SetLowFrame();
        }
                
        async ETTask ExitAnimAsync()
        {
	        SoundManager.Instance.PlaySound("Audio/Game/countDownWin.mp3");
	        Report.LastAuctionPriceStr = LastAuctionPrice;
	        RefreshPrice();
	        if (LastAuctionPlayerId != Player.Id)
	        {
		        Report.Type = LastAuctionPlayerId < 0 ? ReportType.Pass : ReportType.Others;
		        if (LastAuctionPlayerId > 0)
		        {
			        GuidanceManager.Instance.NoticeEvent("Auction_AnyPay");
			        GuidanceManager.Instance.NoticeEvent("Auction_AIPay");
		        }
		        if (IsRaising)
		        {
			        var playerRaiseRewardsMoney = Report.RaisePrice;
                    if (playerRaiseRewardsMoney > BigNumber.Zero)
                    {
                        Report.RaiseSuccessCount = RaiseSuccessCount;
                        var win = await UIManager.Instance.OpenWindow<UIRaiseSuccessWin, BigNumber,float>(UIRaiseSuccessWin.PrefabPath,
                            playerRaiseRewardsMoney, AuctionHelper.GetRaiseMul(Report.RaiseSuccessCount), UILayerNames.TipLayer);
                        while (win.ActiveSelf)
                        {
                            await TimerManager.Instance.WaitAsync(1);
                        }
                    }
		        }
	        }
	        else
	        {
		        UIManager.Instance.OpenWindow<UISuccessAuction>(UISuccessAuction.PrefabPath).Coroutine();
		        GuidanceManager.Instance.NoticeEvent("Auction_AnyPay");
		        GuidanceManager.Instance.NoticeEvent("Auction_UserPay");
		        Report.Type = ReportType.Self;
		        //先给钱
		        PlayerDataManager.Instance.ChangeMoney(-LastAuctionPrice, false);
		        PlayerDataManager.Instance.AddTaskStep(Report.ContainerId, 1, 1);
	        }
	        PerformanceManager.Instance.SetHighFrame();
	        cancellationToken = new ETCancellationToken();
	        var gameView = UIManager.Instance.GetView<UIGuideGameView>(1);
	        if (gameView != null) gameView.HideButtons(cancellationToken).Coroutine();
	        var dir = mapScene.Collector.Get<PlayableDirector>("openbox");
	        if (dir != null)
	        {
		        dir.extrapolationMode = DirectorWrapMode.Hold;
		        dir.enabled = true;
		        dir.time = 0;
		        dir.Play();
		        bool isMove = false;
		        while (dir.time < dir.duration)
		        {
			        if (cancellationToken.IsCancel())
			        {
				        if (dir.time < dir.duration - 0.1f) dir.time = dir.duration - 0.1f;
				        await TimerManager.Instance.WaitAsync(1);
			        }
			        else
			        {
				        await TimerManager.Instance.WaitAsync(1,cancellationToken);
			        }
			        if (!isMove && dir.time > dir.duration * 0.4f)
			        {
				        for (int i = 0; i < Boxes.Count ; i++)
				        {
					        var box = EntityManager.Instance.Get<Box>(Boxes[i]);
					        var ghc = box.GetComponent<GameObjectHolderComponent>();
					        if (ghc.EntityView != null)
					        {
						        var boxRoot = mapScene.Collector.Get<Transform>("box_" + (i + 1));
						        if (boxRoot != null)
						        {
							        ghc.EntityView.SetParent(boxRoot);
							        ghc.EntityView.localPosition = Vector3.zero;
							        ghc.EntityView.localRotation = Quaternion.identity;
							        ghc.EntityView.localScale = Vector3.one;
						        }
						       
					        }
				        }

				        isMove = true;
			        }
			       
		        }
		        dir.enabled = false;
	        }
	        else
	        {
		        Log.Error("结算动画丢失");
	        }
	        
	        UpdateNpcMoney();
	        UpdateRevenge(LastAuctionPrice < AllPrice);
	        SetState(AuctionState.OpenBox);
	        PerformanceManager.Instance.SetLowFrame();
        }
        
        async ETTask ShowReady()
        {
	        var containerId = Report.ContainerId;
	        var container = ContainerConfigCategory.Instance.Get(containerId);
	        SoundManager.Instance.PlaySound("Audio/Game/bubble.mp3");
	        UpdateHostSay(I18NManager.Instance.I18NGetParamText(I18NKey.Text_Game_Stage, Stage)
	                      + " " + I18NManager.Instance.I18NGetText(container), null);
	        await GameTimerManager.Instance.WaitAsync(GameConst.OpenStageInterval[0]);
	        if (IAuctionManager.Instance == null) return;
	        MoveBack().Coroutine();
	        ShowRefreshAuctionPriceAssistantTalk();
	        var gameView = UIManager.Instance.GetView<UIGuideGameView>(1);
	        if (gameView != null)
	        {
		        gameView.ShowButtons().Coroutine();
	        }
	        else
	        {
		        UIManager.Instance.OpenWindow<UIGuideGameView>(UIGuideGameView.PrefabPath).Coroutine();
	        }

	        await GameTimerManager.Instance.WaitAsync(GameConst.OpenStageInterval[1]+1000);
	        if (IAuctionManager.Instance == null) return;
	        Messager.Instance.Broadcast(0, MessageId.ShowTextRange);
	        await GameTimerManager.Instance.WaitAsync(GameConst.OpenStageInterval[2]);
	        if (IAuctionManager.Instance == null) return;
	        SoundManager.Instance.PlaySound("Audio/Game/bubble.mp3");
	        UpdateHostSay(I18NManager.Instance.I18NGetText(I18NKey.Text_Game_StartPrice),
		        I18NManager.Instance.TranslateMoneyToStr(LastAuctionPrice));
	        SetState(AuctionState.AIThink);
        }
        
        private long lastClickTime;
        private bool isGuide = false;
        private async ETTask OpenBoxAsync()
        {
	        if (isEnterState)
	        {
		        lastClickTime = TimerManager.Instance.GetTimeNow();
		        isGuide = false;
		        Move2Center().Coroutine();
		        await UIManager.Instance.OpenWindow<UIButtonView, List<long>>(UIButtonView.PrefabPath, OpenBoxes,	
			        UILayerNames.GameLayer);
		        await UIManager.Instance.OpenWindow<UIItemsView, List<long>>(UIItemsView.PrefabPath, OpenBoxes,
			        UILayerNames.GameLayer);
	        }

	        if (cancellationToken.IsCancel())
	        {
		        UIItemsView view = UIManager.Instance.GetView<UIItemsView>(1);
		        if (view == null) return;
		        for (int i = Boxes.Count-1; i >= 0; i--)
		        {
			        var box = EntityManager.Instance.Get<Box>(Boxes[i]);
			        if (box.BoxType == BoxType.Normal || box.BoxType == BoxType.Task)
			        {
				        if (!OpenBoxes.Contains(Boxes[i]))
				        {
					        OpenBoxes.Add(Boxes[i]);
				        }
			        }
			        else
			        {
				        Boxes.RemoveAt(i);
				        box.Dispose();
			        }
		        }
		        SetState(AuctionState.Over);
		        return;
	        }
	        if (OpenBoxes.Count == Boxes.Count)
	        {
		        if (OpenBoxes.Count == 0)
		        {
			        SetState(AuctionState.Over);
			        return;
		        }
		        return;
	        }
	        if (LastAuctionPlayerId == Player.Id)
	        {
		        var timeNow = TimerManager.Instance.GetTimeNow();
		        if (InputManager.Instance.GetKey(GameKeyCode.NormalAttack))
		        {
			        UIItemsView view = UIManager.Instance.GetView<UIItemsView>(1);
			        if (view == null) return;
			        var mousePos = InputManager.Instance.GetLastTouchPos();
			        Ray ray = CameraManager.Instance.MainCamera().ScreenPointToRay(mousePos);

			        var len = PhysicsHelper.RaycastNonAllocEntity(ray.origin, ray.direction, 100,
				        new EntityType[] { EntityType.Box }, out long[] ids);
			        if (len > 0)
			        {
				        using ListComponent<ETTask> anim = ListComponent<ETTask>.Create(); 
				        anim.Add(OpenTargetBox(view, ids[0]));
				        lastClickTime = timeNow;
				        isGuide = false;
				        Messager.Instance.Broadcast<Transform,int>(0, MessageId.GuideBox2, null, 0);
				        await ETTaskHelper.WaitAll(anim);
				        if (view.overAnim.Count == Boxes.Count)
				        {
					        SetState(AuctionState.Over);
				        }
			        }
		        }
		        else if(!isGuide)
		        {
			        var config = GuidanceStageConfigCategory.Instance.Get(Stage);
			        if (config.OpenGuideBox == 0 || lastClickTime + 3000 < timeNow)
			        {
				        for (int i = 0; i < Boxes.Count; i++)
				        {
					        if (!OpenBoxes.Contains(Boxes[i]))
					        {
						        var box = EntityManager.Instance.Get<Box>(Boxes[i]);
						        var trans = box?.GetComponent<GameObjectHolderComponent>()?.EntityView;
						        if (trans != null)
						        {
							        Messager.Instance.Broadcast(0, MessageId.GuideBox2, trans, box.ConfigId);
							        isGuide = true;
							        break;
						        }
					        }
				        }
			        }
		        }
	        }
	        else if (isEnterState)
	        {
		        UIItemsView view = UIManager.Instance.GetView<UIItemsView>(1);
		        if (view == null) return;
		        using ListComponent<long> temp = ListComponent<long>.Create();
		        temp.AddRange(Boxes);
		        temp.RandomSort();
		        for (int i = 0; i < temp.Count; i++)
		        {
			        if (i < temp.Count - 1)
			        {
				        OpenTargetBox(view, temp[i]).Coroutine();
				        await TimerManager.Instance.WaitAsync(100);
			        }
			        else
			        {
				        await OpenTargetBox(view, temp[i]);
			        }
			        if(cancellationToken.IsCancel()) return;
		        }
		        SetState(AuctionState.Over);
	        }
        }

        private async ETTask OpenTargetBox(UIItemsView view, long id)
        {
	        if (Boxes.Contains(id) && !OpenBoxes.Contains(id))
	        {
		        var box = EntityManager.Instance.Get<Box>(id);
		        var ghc = box.GetComponent<GameObjectHolderComponent>();
		        var size = Mathf.Max(box.Config.Size[0],box.Config.Size[2]);
		        if (box.BoxType == BoxType.Empty || box.BoxType == BoxType.RandOpenEvt)
		        {
			        AuctionHelper.PlayFx(GameConst.SmokePrefab, ghc.EntityView.position + Vector3.back * size/2).Coroutine();
			        Boxes.Remove(id);
			        box.Dispose();
		        }
		        else
		        {
			        OpenBoxes.Add(id);
			        AuctionHelper.PlayFx(GameConst.SmokePrefab, ghc.EntityView.position + Vector3.back * size/2).Coroutine();
			        await TimerManager.Instance.WaitAsync(250, cancellationToken);
			        await view.PlayAnim(id, cancellationToken);
		        }
	        }
        }
        
        private async ETTask ReEnterAnimAsync()
        {
        	cancellationToken?.Cancel();
        	cancellationToken = new ETCancellationToken();
	        Stage++;
	        for (int i = 0; i < Boxes.Count; i++)
	        {
		        EntityManager.Instance.Remove(Boxes[i]);
	        }
	        Boxes.Clear();
	        OpenBoxes.Clear();
	        CreateItems();
	        PerformanceManager.Instance.SetHighFrame();
	        var dir = mapScene.Collector.Get<PlayableDirector>("NewBox");
	        if (dir != null)
	        {
		        dir.extrapolationMode = DirectorWrapMode.Hold;
		        dir.enabled = true;
		        dir.time = 0;
		        dir.Play();
		        while (dir.time < dir.duration)
		        {
			        await TimerManager.Instance.WaitAsync(1);
		        }
		        dir.enabled = false;
	        }
	        if(AState == AuctionState.ReEnterAnim) SetState(AuctionState.Ready);
	        PerformanceManager.Instance.SetLowFrame();
        }
        
        
        private void ClipStartPlay(string key,double time,double total)
        {
	        if (key == "RunNextState")
	        {
		        SetState(AuctionState.Ready);
	        }
	        if (key == "Drop")
	        {
		        Move2Center().Coroutine();
	        }
	        if (key == "Shock")
	        {
		        if(AState == AuctionState.ReEnterAnim) SetState(AuctionState.Ready);
		        MoveBack().Coroutine();
		        ShockManager.Instance.LongVibrate();
		        CameraManager.Instance.Shake(0.1f).Coroutine();
	        }
        }
        
        private async ETTask AllOverAnimAsync()
        {
	        for (int i = 0; i < Boxes.Count; i++)
	        {
		        EntityManager.Instance.Remove(Boxes[i]);
	        }
	        Boxes.Clear();
	        OpenBoxes.Clear();
	        PerformanceManager.Instance.SetHighFrame();
	        if (cancellationToken == null|| cancellationToken.IsCancel())
	        {
		        cancellationToken = new ETCancellationToken();
		        var gameView = UIManager.Instance.GetView<UIGuideGameView>(1);
		        if (gameView != null) gameView.ReEnter(cancellationToken);
	        }
	        var dir = mapScene.Collector.Get<PlayableDirector>("Exit");
	        if (dir != null)
	        {
		        dir.extrapolationMode = DirectorWrapMode.Hold;
		        dir.enabled = true;
		        dir.time = 0;
		        dir.Play();
		        while (dir.time < dir.duration)
		        {
			        if (cancellationToken.IsCancel())
			        {
				        if (dir.time < dir.duration - 0.1f) dir.time = dir.duration - 0.1f;
				        await TimerManager.Instance.WaitAsync(1);
			        }
			        else
			        {
				        await TimerManager.Instance.WaitAsync(1,cancellationToken);
			        }
		        }
		        dir.enabled = false;
	        }
	        SetState(AuctionState.AllOver);
	        PerformanceManager.Instance.SetLowFrame();
        }
        
	    private void ShowRefreshAuctionPriceAssistantTalk()
        {
	        var container = ContainerConfigCategory.Instance.Get(Report.ContainerId);
	        var name = $"<color=#{container.Color}>{I18NManager.Instance.I18NGetText(container)}</color>";
	        var min = I18NManager.Instance.ApproximateMoneyToStr(AllPrice * SysJudgePriceMin,
		        I18NManager.ApproximateType.Floor);
	        var max = I18NManager.Instance.ApproximateMoneyToStr(AllPrice * SysJudgePriceMax,
		        I18NManager.ApproximateType.Cell);
	        Messager.Instance.BroadcastNextFrame(0, MessageId.GuidanceTalk,
		        I18NManager.Instance.I18NGetParamText(I18NKey.Assistant_21, name, min, max), 2000);
        }

	    private Vector3 startPos;
        private Quaternion startRot;
        private Character centerCharacter;
        /// <summary>
        /// 移动到场上
        /// </summary>
        private async ETTask Move2Center()
        {
	        var animPos = mapScene.Collector.Get<Transform>("AnimPos");
	        centerCharacter = EntityManager.Instance.Get<Character>(LastAuctionPlayerId);
	        if (animPos == null || centerCharacter == null || Skip)
	        {
		        return;
	        }
	        if (centerCharacter.Position == animPos.position)
	        {
		        return;
	        }
	        var ease = EasingFunction.GetEasingFunction(EasingFunction.Ease.EaseOutQuart);
	        startPos = centerCharacter.Position;
	        startRot = centerCharacter.Rotation;
	        var endPos = animPos.position;
	        var endRot = animPos.rotation;
	        var startTime = GameTimerManager.Instance.GetTimeNow();
	        var ghc = centerCharacter.GetComponent<CasualActionComponent>();
	        ghc?.SetWinLoss(0);
	        ghc?.SetEnable(false);
	        var time = 500;
	        while (true)
	        {
		        await GameTimerManager.Instance.WaitAsync(1);
		        var timeNow = GameTimerManager.Instance.GetTimeNow();
		        var during = timeNow - startTime;
		        var val = ease.Invoke(during, 0, 1, 500);
		        centerCharacter.Position = Vector3.Lerp(startPos, endPos, val);
		        centerCharacter.Rotation = Quaternion.Lerp(startRot, endRot, val);
		        if (during > time)
		        {
			        break;
		        }
	        }
	        centerCharacter.Position = endPos;
        }
        /// <summary>
        /// 移动回来
        /// </summary>
        private async ETTask MoveBack()
        {
	        var animPos = mapScene?.Collector?.Get<Transform>("AnimPos");
	        if (centerCharacter == null)
	        {
		        return;
	        }
	        if (Skip || animPos == null)
	        {
		        centerCharacter.Position = startPos;
		        centerCharacter.Rotation = startRot;
		        centerCharacter = null;
		        return;
	        }
	        
	        var ease = EasingFunction.GetEasingFunction(EasingFunction.Ease.EaseOutQuart);
	        var endPos = animPos.position;
	        var endRot = animPos.rotation;
	        var ghc = centerCharacter.GetComponent<CasualActionComponent>();
	        centerCharacter.Position = endPos;
	        centerCharacter.Rotation = endRot;
	        ghc?.SetWinLoss(0);
	        ghc?.SetEnable(true);
	        var startTime = GameTimerManager.Instance.GetTimeNow();
	        var time = 500;
	        while (true)
	        {
		        await GameTimerManager.Instance.WaitAsync(1);
		        if (centerCharacter == null)
		        {
			        return;
		        }
		        var timeNow = GameTimerManager.Instance.GetTimeNow();
		        var during = timeNow - startTime;
		        var val = 1 - ease.Invoke(during, 0, 1, 500);
		        centerCharacter.Position = Vector3.Lerp(startPos, endPos, val);
		        centerCharacter.Rotation = Quaternion.Lerp(startRot, endRot, val);
		        if (during > time)
		        {
			        break;
		        }
	        }

	        centerCharacter.Position = startPos;
	        centerCharacter.Rotation = startRot;
	        centerCharacter = null;
        }

        public GameInfoConfig GetFinalGameInfoConfig(bool ignoreId = false)
        {
	        return null;
        }
    }
}