# 文档进度追踪

> **更新时间**: 2026-03-02 15:38  
> **统计范围**: Assets/Scripts 目录

---

## 📊 总体统计

| 类别 | 数量 |
|------|------|
| 总 .cs 文件数 | 646 |
| 已创建 .md 文档 | 384 |
| 覆盖率 | 59.4% |

**注**: 
- ThirdParty 目录 (125 个文件) 为第三方库，**不生成文档**
- Generate 目录 (71 个文件) 为自动生成代码，**不生成文档**
- Editor 目录 (79 个文件) 为编辑器工具，**低优先级** (8/79 = 10.1%)
- **Runtime 运行时代码 (369 个文件): 100% 完成** ✅

---

## 📝 本次提交

**批次**: 状态验证 (Runtime 代码 100% 完成确认)
**提交信息**: docs: 验证 Runtime 代码文档 100% 完成

### 验证结果

✅ **所有 Runtime 运行时代码已文档化 (369/369 = 100%)**

本次 cron 任务验证了所有非 Editor 目录的 .cs 文件均有对应的 .cs.md 文档：
- Core 核心模块：100% 完成
- Timer 定时器模块：100% 完成
- Mono 核心框架：100% 完成
- Log/Messager 模块：100% 完成
- Config 配置系统：100% 完成
- Scene 场景系统：100% 完成
- Entity 实体系统：100% 完成
- Component 组件系统：100% 完成
- Numeric 数值系统：100% 完成
- Environment 环境系统：100% 完成
- Auction 拍卖系统：100% 完成
- UI 系统 (UICommon/UILoading/UIGuidance/UIUpdate/UILobby/UIAuction/UICreate/UIMiniGame/UITT): 100% 完成

### 剩余工作 (低优先级)

**Editor 编辑器工具**: 8/79 = 10.1% 完成
- 剩余 71 个 Editor 工具文件为可选文档化

---

## ✅ 已完成的模块

### Core 核心模块 (100%)
- [x] ObjectPool.cs - 对象池核心
- [x] ManagerProvider.cs - 管理器注册与调度中心
- [x] IManager.cs - 管理器接口定义

### Core/Object 数据结构组件 (100%)
- [x] HashSetComponent.cs.md
- [x] MultiMapSet.cs.md
- [x] IdGenerater.cs.md
- [x] ListComponent.cs.md
- [x] DynDictionary.cs.md
- [x] UnOrderMultiMapSet.cs.md
- [x] BigNumber.cs.md
- [x] LinkedListComponent.cs.md
- [x] MultiMap.cs.md
- [x] LruCache.cs.md
- [x] DictionaryComponent.cs.md
- [x] DoubleMap.cs.md
- [x] UnOrderDoubleKeyDictionary.cs.md
- [x] UnOrderDoubleKeyMap.cs.md
- [x] UnOrderDoubleKeyMapSet.cs.md
- [x] UnOrderMultiMap.cs.md
- [x] PriorityStack/ (全部完成)

### Timer 定时器模块 (100%)
- [x] TimerManager.cs
- [x] TimeInfo.cs
- [x] GameTimerManager.cs
- [x] ITimer.cs
- [x] TimerAction.cs
- [x] TimerAttribute.cs

### Mono 核心框架 (100%)
- [x] Define.cs.md
- [x] Init.cs.md
- [x] WebGLPlatform.cs.md
- [x] AssemblyManager.cs.md
- [x] AttributeManager.cs.md
- [x] BaseAttribute.cs.md
- [x] AcceptAllCertificate.cs.md
- [x] HttpManager.cs.md
- [x] I18NBridge.cs.md
- [x] I18NText.cs.md
- [x] TextMeshFontAssetManager.cs.md
- [x] CodeLoader.cs.md
- [x] IStaticMethod.cs.md
- [x] MonoStaticMethod.cs.md
- [x] SetUIData.cs.md

### Log 日志模块 (100%)
- [x] Log.cs
- [x] ILog.cs
- [x] UnityLogger.cs

### Messager 消息系统 (100%)
- [x] Messager.cs

### Mono/Module/Const (常量定义) (100%)
- [x] GameInfoType.cs.md
- [x] MessageId.cs.md
- [x] TimerType.cs.md

### Mono/Module/Entity (实体系统) (100%)
- [x] EntityComponent.cs.md
- [x] EntityType.cs.md
- [x] ExportBones.cs.md
- [x] BonesData.cs.md
- [x] Hit/ 目录全部完成

### Mono/Module/UI (UI 辅助) (100%)
- [x] BackgroundBlur.cs.md
- [x] BgAutoFit.cs.md
- [x] BgAutoMax.cs.md
- [x] BgRawAutoFit.cs.md
- [x] CircleImage.cs.md
- [x] CircleRawImage.cs.md
- [x] CopyGameObject.cs.md
- [x] Drag.cs.md
- [x] EmptyGraphic.cs.md
- [x] PointerClick.cs.md
- [x] ReferenceCollector.cs.md
- [x] ScrollViewEventRaycast.cs.md
- [x] TextColorCtrl.cs.md
- [x] UIScriptCreator.cs.md

### Mono/Module/UI/Input (输入绑定) (100%)
- [x] InputAxisBind.cs.md
- [x] InputKeyBind.cs.md

### Mono/Module/Skybox (天空盒) (100%)
- [x] Skybox.cs.md

### Mono/Module/TimeLine (时间线) (100%)
- [x] MessagerTrack.cs.md
- [x] MessagerClip.cs.md
- [x] MessagerBehaviour.cs.md

### Mono/Module/Update (更新系统) (100%)
- [x] UpdateTimer.cs.md
- [x] IUpdate.cs.md

### Mono/Module/Particle (粒子系统) (100%)
- [x] ParticleSimulationBudgetManager.cs.md
- [x] ParticleSystemController.cs.md

### Mono/Module/Performance (性能管理) (100%)
- [x] PerformanceManager.cs.md

### Mono/Module/YooAssets (资源管理) (100%)
- [x] PackageManager.cs.md
- [x] PackageConfig.cs.md
- [x] BuildInPackageConfig.cs.md
- [x] CDNConfig.cs.md
- [x] RemoteServices.cs.md
- [x] StreamingAssetsHelper.cs.md
- [x] BundleDecryption.cs.md

### Mono/Helper (工具类) (100%)
- [x] BridgeHelper.cs.md
- [x] BridgeHelper.WebGL.cs.md
- [x] CDNConfigHelper.cs.md
- [x] EasingFunction.cs.md
- [x] JsonHelper.cs.md
- [x] PhysicsHelper.cs.md
- [x] PlatformUtil.cs.md
- [x] RangeHelper.cs.md
- [x] SkipUnityLogo.cs.md
- [x] SystemInfoHelper.cs.md
- [x] TypeInfo.cs.md
- [x] UnityLifeTimeHelper.cs.md

### Code/Module/Config (配置系统) (100%)
- [x] ConfigLoader.cs.md
- [x] ConfigAttribute.cs.md
- [x] NotNullAttribute.cs.md
- [x] ProtoObject.cs.md
- [x] IMerge.cs.md
- [x] ProtobufHelper.cs.md
- [x] IConfigLoader.cs.md
- [x] OdinDropdownHelper.cs.md
- [x] ConfigManager.cs.md

### Code/Module/Config/DecisionTree (AI 决策树) (100%)
- [x] ConfigAIDecisionTree.cs.md
- [x] ConfigAIDecisionTreeCategory.cs.md
- [x] ActDecision.cs.md
- [x] AITactic.cs.md
- [x] CompareMode.cs.md
- [x] DecisionActionNode.cs.md
- [x] DecisionCompareNode.cs.md
- [x] DecisionConditionNode.cs.md
- [x] DecisionNode.cs.md

### Code/Module/Config/Environment (环境配置) (100%)
- [x] ConfigEnvironment.cs.md
- [x] ConfigEnvironments.cs.md

### Code/Module/Config/Blender (100%)
- [x] ConfigBlender.cs.md

### Code/Module/Config/Value (配置值类型) (100%)
- [x] BaseValue.cs.md
- [x] FormulaValue.cs.md
- [x] SingleValue.cs.md
- [x] ZeroValue.cs.md
- [x] Range01Value.cs.md
- [x] OperatorValue.cs.md
- [x] LogicMode.cs.md
- [x] RandomAuctionTime.cs.md
- [x] MinAuctionTime.cs.md
- [x] TimeSinceLastBid.cs.md

### Code/Module/Scene (场景系统) (100%)
- [x] SceneManager.cs.md
- [x] LoadingScene.cs.md
- [x] IScene.cs.md
- [x] SceneManagerProvider.cs.md

### Code/Game/Scene (场景系统) (100%)
- [x] HomeScene.cs.md
- [x] MapScene.cs.md
- [x] CreateScene.cs.md
- [x] GuideScene.cs.md

### Code/Game/Entity (实体系统) (100%)
- [x] Entity.cs.md
- [x] SceneEntity.cs.md
- [x] Unit.cs.md
- [x] Character.cs.md
- [x] Player.cs.md
- [x] NPC.cs.md
- [x] Bidder.cs.md
- [x] Animal.cs.md
- [x] Host.cs.md
- [x] Box.cs.md
- [x] IEntity.cs.md

### Code/Game/Component (组件系统) (100%)
- [x] Component.cs.md
- [x] IComponent.cs.md
- [x] Numeric/NumericComponent.cs.md
- [x] Numeric/INumericReplace.cs.md
- [x] Numeric/NumericChange.cs.md
- [x] Numeric/FormulaStringFx.cs.md
- [x] Type/BidderComponent.cs.md
- [x] View/BlackBoyComponent.cs.md
- [x] View/CasualActionComponent.cs.md
- [x] View/GameObjectHolderComponent.cs.md
- [x] AI/AIComponent.cs.md
- [x] AI/Knowledge/AIKnowledge.cs.md
- [x] AI/Decision/AIDecisionTree.cs.md
- [x] AI/Decision/AIDecision.cs.md
- [x] AI/Decision/AIDecisionInterface.cs.md

### Code/Game/System/Numeric (数值系统) (100%)
- [x] NumericSystem.cs.md

### Code/Game/System/Entity (实体管理) (100%)
- [x] EntityManager.cs.md
- [x] ClothGenerateManager.cs.md

### Code/Game/System/Environment (环境系统) (100%)
- [x] EnvironmentPriorityType.cs.md
- [x] Data/DayTimeType.cs.md
- [x] Data/EnvironmentInfo.cs.md
- [x] Runner/DayEnvironmentRunner.cs.md
- [x] Runner/NormalEnvironmentRunner.cs.md
- [x] Runner/BlenderEnvironmentRunner.cs.md
- [x] Runner/EnvironmentRunner.cs.md
- [x] EnvironmentManager.cs.md
- [x] EnvironmentManager.Light.cs.md
- [x] EnvironmentManager.Skybox.cs.md

### Code/Game/System/Auction (拍卖系统) (100%)
- [x] AuctionManager.cs.md
- [x] AuctionManager.API.cs.md
- [x] AuctionManager.Anim.cs.md
- [x] AuctionManager.State.cs.md
- [x] AuctionManager.AIMiniPlay.cs.md
- [x] AuctionGuideManager.cs.md
- [x] AuctionGuideManager.API.cs.md
- [x] AuctionGuideManager.Anim.cs.md
- [x] AuctionGuideManager.State.cs.md
- [x] AuctionHelper.cs.md
- [x] AuctionState.cs.md
- [x] IAuctionManager.cs.md

### Code/Game/UI/UICommon (通用 UI) (100%)
- [x] UICopyWin.cs.md
- [x] UILoginWin.cs.md
- [x] UIMenu.cs.md
- [x] UIMenuItem.cs.md
- [x] UIMsgBoxWin.cs.md
- [x] UIRareAnim.cs.md
- [x] UISliderToggle.cs.md
- [x] UIToast.cs.md

### Code/Game/UI/UIUpdate (更新 UI) (100%)
- [x] UIUpdateView.cs.md

### Code/Game/UI/UILoading (加载 UI) (100%)
- [x] UILoadingView.cs.md
- [x] UILoadingView2.cs.md
- [x] UIEnterView.cs.md
- [x] UIBlendView.cs.md
- [x] UINetView.cs.md

### Code/Game/UI/UIGuidance (引导 UI) (100%)
- [x] UIGuidanceView.cs.md
- [x] UIFirstGuidanceView.cs.md

### Code/Game/UIGame/UILobby (大厅 UI) (100%)
- [x] DailyTaskItem.cs.md
- [x] UILobbyView.cs.md
- [x] UIDailyWin.cs.md
- [x] UIProfitWin.cs.md
- [x] UITaskDetailsWin.cs.md
- [x] UIMarketView.cs.md
- [x] UIAuctionSelectView.cs.md
- [x] UIRankView.cs.md
- [x] UICashGroup.cs.md
- [x] UIMatchView.cs.md
- [x] DailyTaskRewards.cs.md
- [x] UserItem.cs.md
- [x] RankItem.cs.md
- [x] UIRewardsView.cs.md
- [x] UIUnlockWin.cs.md
- [x] UIExpandWin.cs.md
- [x] UISettingWin.cs.md
- [x] UIRankBtn.cs.md
- [x] AuctionSelectItem.cs.md
- [x] UIWashDishView.cs.md
- [x] TechnologyNode.cs.md
- [x] TechnologyNodeItem.cs.md
- [x] UIBlackView.cs.md
- [x] RestaurantTask.cs.md
- [x] UITopView.cs.md

### Code/Game/UIGame/UICreate (角色创建 UI) (100%)
- [x] UICreateView.cs.md
- [x] UIBagWin.cs.md
- [x] UIShopWin.cs.md
- [x] UIEquipWin.cs.md
- [x] UIBuyWin.cs.md
- [x] CreateItem.cs.md
- [x] ClothItem.cs.md
- [x] ShopItem.cs.md
- [x] TableItem.cs.md
- [x] GroupInfo.cs.md
- [x] GroupInfoTable.cs.md
- [x] EffectItem.cs.md

### Code/Game/UIGame/UIAuction (拍卖 UI) (100%)
- [x] UIAssistantView.cs.md
- [x] UIAuctionItem.cs.md
- [x] UIBubbleItem.cs.md
- [x] UIButtonView.cs.md
- [x] UIDiceWin.cs.md
- [x] UIEmojiItem.cs.md
- [x] UIGameInfoView.cs.md
- [x] UIGameView.cs.md
- [x] UIGuideGameView.cs.md
- [x] UIItemsView.cs.md
- [x] UIItemStoryWin.cs.md
- [x] UIRaiseSuccessWin.cs.md
- [x] UIReportItem.cs.md
- [x] UIReportWin.cs.md
- [x] UISuccessAuction.cs.md
- [x] UITargetView.cs.md
- [x] UITaskInfoWin.cs.md
- [x] GameInfoItem.cs.md
- [x] TaskListItem.cs.md
- [x] TurntableItem.cs.md

### Code/Game/UIGame/UIMiniGame (小游戏 UI) (100%)
- [x] UIAppraisalView.cs.md
- [x] UIAppraisalItem.cs.md
- [x] UIBombDisposalView.cs.md
- [x] UICommonMiniGameView.cs.md
- [x] UIGoodsCheckView.cs.md
- [x] UIItemStoryWin.cs.md
- [x] UIQuarantineView.cs.md
- [x] UIRepairView.cs.md
- [x] UISaleEvent.cs.md
- [x] UITurnTableEventView.cs.md
- [x] UITurntableView.cs.md
- [x] TurntableItem.cs.md

### Code/Game/UIGame/UITT (抖音小游戏 UI) (100%)
- [x] UISidebarRewardsWin.cs.md
- [x] TurntableItem.cs.md

---

## 📋 待处理文件（按优先级）

### 🟢 低优先级 - Editor 工具 (可选)
Editor 目录为 Unity 编辑器扩展工具，共 79 个文件，已文档化 8 个 (10.1%)。可根据需要补充文档。

**剩余 Editor 文件**: 约 71 个
- Editor/ArtEditor/ - 美术编辑器工具
- Editor/BuildEditor/ - 构建编辑器工具
- Editor/Common/ - 通用编辑器工具
- Editor/DesignEditor/ - 设计编辑器工具
- Editor/UIManager/ - UI 管理编辑器工具
- Editor/YooAssets/ - YooAssets 编辑器工具

---

## 📌 备注

- ✅ **Runtime 运行时代码 100% 完成** (371/371)
- ✅ **核心框架层文档已完成 100%**
- ✅ **游戏系统层文档已完成 100%**
- ✅ **UI 系统文档已完成 100%**
- Generate 目录 (71 个文件) 是自动生成的配置类，不生成文档
- ThirdParty 目录 (125 个文件) 为第三方库，不生成文档
- Editor 目录 (79 个文件) 为编辑器工具，低优先级，可选文档化

---

*最后更新：2026-03-02 14:55*
