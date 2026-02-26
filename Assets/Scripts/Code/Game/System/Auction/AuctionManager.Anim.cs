using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Playables;

namespace TaoTie
{
	public partial class AuctionManager
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
			while (!IAuctionManager.UserReady)
			{
				await TimerManager.Instance.WaitAsync(1);
			}
			SetState(AuctionState.Prepare);
		}

		/// <summary>
		/// 场景加载完成后播放入场动画
		/// </summary>
		private async ETTask PlayEnterAnim()
		{
			if (!isEnterState) return;
			Stage++;
			CreateItems();
			await UIManager.Instance.OpenWindow<UIAssistantView>(UIAssistantView.PrefabPath, UILayerNames.SceneLayer);
			PerformanceManager.Instance.SetHighFrame();
			var dir = mapScene.Collector.Get<PlayableDirector>("Character");
			if (dir != null)
			{
				using DictionaryComponent<string, PlayableBinding> bindings =
					DictionaryComponent<string, PlayableBinding>.Create();
				foreach (var o in dir.playableAsset.outputs)
				{
					if (!bindings.ContainsKey(o.streamName))
						bindings.Add(o.streamName, o);
				}

				var host = EntityManager.Instance.Get(HostId) as Host;
				var hosthc = host.GetComponent<GameObjectHolderComponent>();
				if (bindings.TryGetValue("HostTrack", out var playableBinding))
				{
					dir.SetGenericBinding(playableBinding.sourceObject,
						hosthc.EntityView.GetComponent(playableBinding.outputTargetType));
				}

				if (bindings.TryGetValue("HostRoot", out playableBinding))
				{
					hosthc.EntityView.SetParent(dir.GetGenericBinding(playableBinding.sourceObject) as Transform);
				}

				var phc = Player.GetComponent<GameObjectHolderComponent>();
				if (bindings.TryGetValue(Player.RootName + "Track", out playableBinding))
				{
					dir.SetGenericBinding(playableBinding.sourceObject,
						phc.EntityView.GetComponent(playableBinding.outputTargetType));
				}

				if (bindings.TryGetValue(Player.RootName + "Root", out playableBinding))
				{
					phc.EntityView.SetParent(dir.GetGenericBinding(playableBinding.sourceObject) as Transform);
				}

				for (int i = 0; i < Bidders.Count; i++)
				{
					var npc = EntityManager.Instance.Get(Bidders[i]) as Bidder;
					var ghc = npc.GetComponent<GameObjectHolderComponent>();
					if (bindings.TryGetValue(npc.RootName + "Track", out playableBinding))
					{
						dir.SetGenericBinding(playableBinding.sourceObject,
							ghc.EntityView.GetComponent(playableBinding.outputTargetType));
					}

					if (bindings.TryGetValue(npc.RootName + "Root", out playableBinding))
					{
						ghc.EntityView.SetParent(dir.GetGenericBinding(playableBinding.sourceObject) as Transform);
					}
				}

				for (int i = 0; i < Npcs.Count; i++)
				{
					var npc = EntityManager.Instance.Get(Npcs[i]) as NPC;
					var ghc = npc.GetComponent<GameObjectHolderComponent>();
					if (bindings.TryGetValue(npc.RootName + "Track", out playableBinding))
					{
						dir.SetGenericBinding(playableBinding.sourceObject,
							ghc.EntityView.GetComponent(playableBinding.outputTargetType));
					}

					if (bindings.TryGetValue(npc.RootName + "Root", out playableBinding))
					{
						ghc.EntityView.SetParent(dir.GetGenericBinding(playableBinding.sourceObject) as Transform);
					}
				}

				dir.enabled = true;
				dir.Resume();
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

			if (AState == AuctionState.EnterAnim) SetState(AuctionState.Ready);
			PerformanceManager.Instance.SetLowFrame();
		}

		async ETTask ExitAnimAsync()
		{
			if (ufoId > 0)
			{
				Animal ufo = EntityManager.Instance.Get<Animal>(ufoId);
				var ghc = ufo.GetComponent<GameObjectHolderComponent>();
				ghc.EntityView.SetParent(EntityManager.Instance.GameObjectRoot, true);
				ufo.SyncViewPosition(ghc.EntityView.position);
				ufo.SyncViewRotation(ghc.EntityView.rotation);
				ufo.SyncViewLocalScale(ghc.EntityView.localScale);
			}
	        
			if (LastAuctionPlayerId > 0)
			{
				SoundManager.Instance.PlaySound("Audio/Game/countDownWin.mp3");
			}
			else
			{
				SoundManager.Instance.PlaySound("Audio/Game/giveup.mp3");
			}

			// RecycleEventSmoke();
			Report.LastAuctionPriceStr = LastAuctionPrice;
			RefreshPrice();
			if (LastAuctionPlayerId != Player.Id)
			{
				Report.Type = LastAuctionPlayerId < 0 ? ReportType.Pass : ReportType.Others;
				if (IsRaising)
				{
					var playerRaiseRewardsMoney = Report.RaisePrice;
					if (playerRaiseRewardsMoney > BigNumber.Zero)
					{
						var gameInfo = GetFinalGameInfoConfig(true);
						float mul = 0;
						bool isMul = false;
						if (gameInfo?.Type != (int) GameInfoTargetType.Raise)
						{
							mul = 0;
						}
						else
						{
							if (gameInfo.AwardType == 0)
							{
								mul = gameInfo.RewardCount;
								isMul = false;
							}
							else
							{
								mul = gameInfo.RewardCount;
								isMul = true;
							}

							Log.Info("触发情报抬价收益");
						}

						Report.RaiseSuccessCount = RaiseSuccessCount;
						var win = await UIManager.Instance.OpenWindow<UIRaiseSuccessWin, BigNumber, float, float, bool>(
							UIRaiseSuccessWin.PrefabPath,
							playerRaiseRewardsMoney, AuctionHelper.GetRaiseMul(Report.RaiseSuccessCount), mul, isMul,
							UILayerNames.TipLayer);
						while (win.ActiveSelf)
						{
							await TimerManager.Instance.WaitAsync(1);
						}
					}
				}
			}
			else
			{
				Report.Type = ReportType.Self;
				UIManager.Instance.OpenWindow<UISuccessAuction>(UISuccessAuction.PrefabPath).Coroutine();
				//先给钱
				PlayerDataManager.Instance.ChangeMoney(-LastAuctionPrice, false);
				PlayerDataManager.Instance.AddTaskStep(Report.ContainerId, 1, 1);
			}

			PerformanceManager.Instance.SetHighFrame();
			cancellationToken = new ETCancellationToken();
			var gameView = UIManager.Instance.GetView<UIGameView>(1);
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
						await TimerManager.Instance.WaitAsync(1, cancellationToken);
					}

					if (!isMove && dir.time > dir.duration * 0.4f)
					{
						for (int i = 0; i < Boxes.Count; i++)
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
			var gameView = UIManager.Instance.GetView<UIGameView>(1);
			if (gameView != null)
			{
				gameView.ShowButtons().Coroutine();
			}
			else
			{
				UIManager.Instance.OpenWindow<UIGameView>(UIGameView.PrefabPath).Coroutine();
			}

			await GameTimerManager.Instance.WaitAsync(GameConst.OpenStageInterval[1]);
			if (IAuctionManager.Instance == null) return;
			Messager.Instance.Broadcast(0, MessageId.ShowTextRange);
			await GameTimerManager.Instance.WaitAsync(GameConst.OpenStageInterval[2]);
			if (IAuctionManager.Instance == null) return;
			SoundManager.Instance.PlaySound("Audio/Game/bubble.mp3");
			UpdateHostSay(I18NManager.Instance.I18NGetText(I18NKey.Text_Game_StartPrice),
				I18NManager.Instance.TranslateMoneyToStr(LastAuctionPrice));
			SetState(AuctionState.AIThink);
			await GameTimerManager.Instance.WaitAsync(GameConst.OpenStageInterval[3]);
			ShowStartAssistantTalk();
			var obj = mapScene.Collector.Get<GameObject>("UFORoot");
			obj.SetActive(true);
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
				for (int i = Boxes.Count - 1; i >= 0; i--)
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
						if (box.BoxType == BoxType.GodOfWealthEvt && LastAuctionPlayerId == Player.Id)
						{
							hasGodOfWealth = true;
							var gameView = UIManager.Instance.GetView<UIGameView>(1);
							if (gameView != null)
							{
								gameView.PlayCSAnim(Vector3.zero).Coroutine();
							}
						}
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
					if(InputManager.Instance.IsPointerOverUI(mousePos)) return;
					Ray ray = CameraManager.Instance.MainCamera().ScreenPointToRay(mousePos);

					var len = PhysicsHelper.RaycastNonAllocEntity(ray.origin, ray.direction, 100,
						new EntityType[] {EntityType.Box}, out long[] ids);
					if (len > 0)
					{
						using ListComponent<ETTask> anim = ListComponent<ETTask>.Create();
						if (Boxes.Contains(ids[0]) && !OpenBoxes.Contains(ids[0]))
						{
							anim.Add(OpenTargetBox(view, ids[0]));
						}

						lastClickTime = timeNow;
						isGuide = false;
						Messager.Instance.Broadcast<Transform, int>(0, MessageId.GuideBox, null, 0);
						await ETTaskHelper.WaitAll(anim);
						if (view.overAnim.Count == Boxes.Count)
						{
							SetState(AuctionState.Over);
						}
					}
				}
				else if (!isGuide && lastClickTime + 3000 < timeNow)
				{
					for (int i = 0; i < Boxes.Count; i++)
					{
						if (!OpenBoxes.Contains(Boxes[i]))
						{
							var box = EntityManager.Instance.Get<Box>(Boxes[i]);
							var trans = box?.GetComponent<GameObjectHolderComponent>()?.EntityView;
							if (trans != null)
							{
								Messager.Instance.Broadcast(0, MessageId.GuideBox, trans, box.ConfigId);
								isGuide = true;
								break;
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
					if (Boxes.Contains(temp[i]) && !OpenBoxes.Contains(temp[i]))
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
					}

					if (cancellationToken.IsCancel()) return;
				}

				RefreshWinLossAnim(true);
				await AIRandomMiniPlay();
				SetState(AuctionState.Over);
			}
		}

		private async ETTask OpenTargetBox(UIItemsView view, long id)
		{
			var box = EntityManager.Instance.Get<Box>(id);
			var ghc = box.GetComponent<GameObjectHolderComponent>();
			var size = Mathf.Max(box.Config.Size[0], box.Config.Size[2]);
			if (box.BoxType == BoxType.Empty)
			{
				AuctionHelper.PlayFx(GameConst.SmokePrefab, ghc.EntityView.position + Vector3.back * size / 2)
					.Coroutine();
				Boxes.Remove(id);
				box.Dispose();
			}
			else if (box.BoxType == BoxType.GodOfWealthEvt)
			{
				AuctionHelper.PlayFx(GameConst.SmokePrefab, ghc.EntityView.position + Vector3.back * size / 2)
					.Coroutine();
				var pos = ghc.EntityView.position;
				Boxes.Remove(id);
				box.Dispose();
				if (LastAuctionPlayerId == Player.Id)
				{
					hasGodOfWealth = true;
					var gameView = UIManager.Instance.GetView<UIGameView>(1);
					if (gameView != null)
					{
						await TimerManager.Instance.WaitAsync(100, cancellationToken);
						gameView.PlayCSAnim(pos).Coroutine();
					}
				}
			}
			else if (box.BoxType == BoxType.RandOpenEvt)
			{
				Boxes.Remove(id);
				AuctionHelper.PlayFx(GameConst.SmokePrefab2, ghc.EntityView.position + Vector3.back * size / 2)
					.Coroutine();
				await TimerManager.Instance.WaitAsync(250, cancellationToken);
				OpenEvent(ghc.EntityView.position, ghc.EntityView.rotation).Coroutine();
				box.Dispose();
			}
			else
			{
				AuctionHelper.PlayFx(GameConst.SmokePrefab, ghc.EntityView.position + Vector3.back * size / 2)
					.Coroutine();
				OpenBoxes.Add(id);
				if (view != null)
				{
					await TimerManager.Instance.WaitAsync(250, cancellationToken);
					await view.PlayAnim(id, cancellationToken);
				}
			}
		}

		/// <summary>
		/// 开箱事件
		/// </summary>
		private async ETTask OpenEvent(Vector3 pos, Quaternion rot)
		{
			bool isPlayer = LastAuctionPlayerId == Player.Id;
			var events = OpenBoxEventConfigCategory.Instance.GetAllList();
			int totalEvts = 0;
			
			for (int i = 0; i < events.Count; i++)
			{
				if (isPlayer || events[i].Npc == 1)
				{
					totalEvts += events[i].Weight;
				}
			}

			if (totalEvts <= 0)
			{
				Log.Error("无OpenBoxEvent事件可触发");
				return;
			}

			OpenBoxEventConfig openBoxEventConfig = null;
			var evtRange = Random.Range(0, totalEvts);
			for (int i = 0; i < events.Count; i++)
			{
				if (isPlayer || events[i].Npc == 1)
				{
					evtRange -= events[i].Weight;
					if (evtRange <= 0)
					{
						openBoxEventConfig = events[i];
						break;
					}
				}
			}
			if (openBoxEventConfig?.Type == 1)
			{
				if (Turntable3RewardsConfigCategory.Instance.TryGet(Level, PlayerDataManager.Instance.RestaurantLv, out var list))
				{
					var total = 0;
					for (int i = 0; i < list.Count; i++)
					{
						total += list[i].Weight;
					}

					var range = Random.Range(0, total);
					var index = -1;
					for (int i = 0; i < list.Count; i++)
					{
						range -= list[i].Weight;
						if (range <= 0)
						{
							index = i;
							break;
						}
					}

					var item = list[index];
					var unit = EntityManager.Instance.CreateEntity<Animal, int>(openBoxEventConfig.UnitId);
					unit.Position = pos;
					unit.Rotation = rot;
					unit.LocalScale = Player.LocalScale;
					var ghc = unit.GetComponent<GameObjectHolderComponent>();
					await ghc.WaitLoadGameObjectOver();
					await TimerManager.Instance.WaitAsync(openBoxEventConfig.During/2);
					UIManager.Instance
						.OpenWindow<UIRewardsView, int, long>(UIRewardsView.PrefabPath, item.ItemId,
							item.RewardCount).Coroutine();
					await TimerManager.Instance.WaitAsync(openBoxEventConfig.During/2);
					unit.Dispose();
				}
			}
			else if (openBoxEventConfig?.Type == 2)
			{
				var unit = EntityManager.Instance.CreateEntity<Animal, int>(openBoxEventConfig.UnitId);
				unit.Position = pos;
				unit.LocalScale = Player.LocalScale;
				var vec = unit.Position.x > 0 ? Vector3.left : Vector3.right;
				unit.Rotation = Quaternion.LookRotation(vec, Vector3.up);
				var timeStart = TimerManager.Instance.GetTimeNow();
				while (true)
				{
					await TimerManager.Instance.WaitAsync(1);
					var timeNow = TimerManager.Instance.GetTimeNow();
					unit.Position = pos + vec * (timeNow - timeStart) / 500f;
					if (timeNow - timeStart > openBoxEventConfig.During)
					{
						break;
					}
				}
				unit.Dispose();
			}
			else if (openBoxEventConfig?.Type == 3 && Bidders.Count > 0)
			{
				var index = Random.Range(0, Bidders.Count);
				float min = float.MaxValue;
				Bidder bidder = null;
				for (int i = 0; i < Bidders.Count; i++)
				{
					if(Bidders[index] == LastAuctionPlayerId) continue;
					var temp = EntityManager.Instance.Get<Bidder>(Bidders[index]);
					var sqlM = Vector3.SqrMagnitude(temp.Position - pos);
					if (sqlM < min)
					{
						min = sqlM;
						bidder = temp;
					}
				}

				if (bidder == null) return;
				var unit = EntityManager.Instance.CreateEntity<Animal, int>(openBoxEventConfig.UnitId);
				unit.Position = pos;
				unit.LocalScale = Player.LocalScale;
				unit.Rotation = Quaternion.LookRotation(bidder.Position - unit.Position, Vector3.up);
				Leave(bidder.Id, 1);
				var timeStart = TimerManager.Instance.GetTimeNow();
				while (true)
				{
					await TimerManager.Instance.WaitAsync(1);
					var timeNow = TimerManager.Instance.GetTimeNow();
					unit.Position = pos + (bidder.Position - pos).normalized * (timeNow - timeStart) / 500f;
					if (timeNow - timeStart > openBoxEventConfig.During)
					{
						break;
					}
				}
				unit.Dispose();
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
	        if (cancellationToken == null|| cancellationToken.IsCancel())
	        {
		        cancellationToken = new ETCancellationToken();
		        var gameView = UIManager.Instance.GetView<UIGameView>(1);
		        if (gameView != null) gameView.ReEnter(cancellationToken);
	        }
	        var dir = mapScene.Collector.Get<PlayableDirector>("NewBox");
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
		        var gameView = UIManager.Instance.GetView<UIGameView>(1);
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

        private void ShowStartAssistantTalk()
        {
	        if (Boxes == null) return;
	        for (int i = 0; i < Boxes.Count; i++)
	        {
		        var box = EntityManager.Instance?.Get<Box>(Boxes[i]);
		        if (box?.BoxType == BoxType.Task)
		        {
			        var containerId = Report.ContainerId;
			        var config = ContainerConfigCategory.Instance.Get(containerId);
			        var name = I18NManager.Instance.I18NGetText(config);
			        Messager.Instance.Broadcast(0, MessageId.AssistantTalk,
				        I18NManager.Instance.I18NGetParamText(I18NKey.Assistant_11, name, 
					        I18NManager.Instance.I18NGetText(I18NKey.Text_Assistant_0)),true);
			        return;
		        }
	        }
        }
        
        private void ShowRefreshAuctionPriceAssistantTalk()
        {
	        var container = ContainerConfigCategory.Instance.Get(Report.ContainerId);
	        var name = $"<color=#{container.Color}>{I18NManager.Instance.I18NGetText(container)}</color>";
	        var min = I18NManager.Instance.ApproximateMoneyToStr(AllPrice * SysJudgePriceMin,
		        I18NManager.ApproximateType.Floor);
	        var max = I18NManager.Instance.ApproximateMoneyToStr(AllPrice * SysJudgePriceMax,
		        I18NManager.ApproximateType.Cell);
	        Messager.Instance.Broadcast(0, MessageId.AssistantTalk,
		        I18NManager.Instance.I18NGetParamText(I18NKey.Assistant_21, name, min, max),true);
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

        private async ETTask UFOPickUp(long id)
        {
	        Animal ufo = EntityManager.Instance.Get<Animal>(ufoId);
	        ufoId = 0;
	        Bidders.Remove(id);
	        Blacks.Remove(id);
	        var offset = Vector3.up * 10;
	        var startPos = ufo.Position;
	        var target = EntityManager.Instance.Get<Character>(id);
	        var ghc = target.GetComponent<GameObjectHolderComponent>();
	        target.SyncViewPosition(ghc.EntityView.position);
	        target.SyncViewRotation(ghc.EntityView.rotation);
	        var endPos = new Vector3(target.Position.x, startPos.y, target.Position.z);
	        var timeStart = TimerManager.Instance.GetTimeNow();
	        float during1 = 500;
	        float during2 = 1500;
	        while (true)
	        {
		        await TimerManager.Instance.WaitAsync(1);
		        var timeNow = TimerManager.Instance.GetTimeNow();
		        var delta = timeNow - timeStart;
		        var progress = Mathf.Clamp01(delta / during1);
		        ufo.Position = Vector3.Lerp(startPos, endPos, progress);
		        if (delta > during1)
		        {
			        break;
		        }
	        }
	        
	        timeStart = TimerManager.Instance.GetTimeNow();
	        var playerPos = target.Position;
	        var euler = target.Rotation.eulerAngles;
	        while (true)
	        {
		        await TimerManager.Instance.WaitAsync(1);
		        var timeNow = TimerManager.Instance.GetTimeNow();
		        var delta = timeNow - timeStart;
		        var progress = Mathf.Clamp01(delta / during2);
		        ufo.Position = Vector3.Lerp(endPos, endPos + offset, progress);
		        target.Position = Vector3.Lerp(playerPos, playerPos + offset/2, progress);
		        target.Rotation = Quaternion.Euler(euler.x, delta * 2, euler.z);
		        if (delta > during2)
		        {
			        break;
		        }
	        }
	        if(id != Player?.Id) target.Dispose();
	        ufo.Dispose();
        }

        private async ETTask UFOPickPlayer()
        {
	        await UFOPickUp(Player.Id);
	        ForceAllOver();
        }
	        
    }
}