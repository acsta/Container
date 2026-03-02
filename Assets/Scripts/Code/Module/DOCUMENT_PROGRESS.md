# 文档进度追踪

> **更新时间**: 2026-03-02 11:15  
> **统计范围**: Assets/Scripts 目录

---

## 📊 总体统计

| 类别 | 数量 |
|------|------|
| 总 .cs 文件数 | ~646 |
| 已创建 .md 文档 | 377 |
| 覆盖率 | ~58.4% |

---

## ✅ 已完成的模块

### Core 核心模块 (100%)
- [x] ObjectPool.cs - 对象池核心
- [x] ManagerProvider.cs - 管理器注册与调度中心
- [x] IManager.cs - 管理器接口定义

### Core/Object 数据结构组件 (100%)
- [x] HashSetComponent.cs.md - HashSet 对象池组件
- [x] MultiMapSet.cs.md - 有序多重映射（HashSet 去重）
- [x] IdGenerater.cs.md - 全局唯一 ID 生成器
- [x] ListComponent.cs.md - List 对象池组件
- [x] DynDictionary.cs.md - 动态字典（支持继承和变更通知）
- [x] UnOrderMultiMapSet.cs.md - 无序多重映射（HashSet 去重）
- [x] BigNumber.cs.md - 任意精度大数运算
- [x] LinkedListComponent.cs.md - LinkedList 对象池组件
- [x] MultiMap.cs.md - 有序多重映射（List 允许重复）
- [x] LruCache.cs.md - LRU 缓存（线程安全）

### Timer 定时器模块 (100%)
- [x] TimerManager.cs - 定时器管理系统
- [x] TimeInfo.cs - 时间信息服务
- [x] GameTimerManager.cs - 游戏时间管理器
- [x] ITimer.cs - 定时器处理器接口
- [x] TimerAction.cs - 定时器动作数据结构

### Mono 核心框架 (新增)
- [x] Define.cs.md - 全局常量与配置定义
- [x] AssemblyManager.cs.md - 程序集管理器
- [x] AttributeManager.cs.md - 属性扫描管理器
- [x] BaseAttribute.cs.md - 基础属性定义
- [x] AcceptAllCertificate.cs.md - SSL 证书处理器
- [x] HttpManager.cs.md - HTTP 请求管理器
- [x] I18NBridge.cs.md - 国际化桥接
- [x] I18NText.cs.md - 国际化文本组件
- [x] TextMeshFontAssetManager.cs.md - TextMesh 字体资产管理
- [x] CodeLoader.cs.md - 代码加载器（热更新）
- [x] IStaticMethod.cs.md - 静态方法接口
- [x] MonoStaticMethod.cs.md - Mono 反射实现

### Log 日志模块 (100%)
- [x] Log.cs - 日志系统入口
- [x] ILog.cs - 日志接口定义
- [x] UnityLogger.cs - Unity 日志实现

### UI 模块 (100%)
- [x] UIBaseContainer.cs.md
- [x] UIBaseView.cs.md
- [x] UIManager.cs.md
- [x] UIManager.Layers.cs.md
- [x] UILayer.cs.md
- [x] UILayerNames.cs.md
- [x] UIWindow.cs.md
- [x] IOnCreate.cs.md
- [x] IOnEnable.cs.md
- [x] IOnDisable.cs.md
- [x] IOnDestroy.cs.md
- [x] IOnBeforeCloseWin.cs.md
- [x] IOnWidthPaddingChange.cs.md

### UI/RedDot 模块 (100%)
- [x] UIRedDot.cs.md
- [x] UINumRedDot.cs.md
- [x] RedDotManager.cs.md

### UIComponent 模块 (100%)
- [x] UIAnimator.cs.md
- [x] UIButton.cs.md
- [x] UICopyGameObject.cs.md
- [x] UIDropdown.cs.md
- [x] UIEmptyView.cs.md
- [x] UIEventTrigger.cs.md
- [x] UIImage.cs.md
- [x] UIInput.cs.md
- [x] UIInputTextmesh.cs.md
- [x] UILoopGridView.cs.md
- [x] UILoopListView2.cs.md
- [x] UIMonoBehaviour.cs.md
- [x] UIPointerClick.cs.md
- [x] UIRawImage.cs.md
- [x] UISlider.cs.md
- [x] UIText.cs.md
- [x] UITextmesh.cs.md
- [x] UIToggle.cs.md

### Mono/Module/Skybox (100%)
- [x] Skybox.cs.md - 天空盒昼夜循环系统

### Mono/Module/Const (100%)
- [x] GameInfoType.cs.md - 游戏信息事件类型
- [x] MessageId.cs.md - 全局消息 ID 常量

### Mono/Module/Entity (100%)
- [x] EntityType.cs.md - 实体类型定义
- [x] ExportBones.cs.md - 骨骼导出工具
- [x] BonesData.cs.md - 骨骼数据组件

### Mono/Module/Entity/Hit (100%)
- [x] ColliderBoxComponent.cs.md - 碰撞触发器
- [x] CheckHitLayerType.cs.md - 检测层级类型
- [x] HitBoxComponent.cs.md - HitBox 标记组件
- [x] HitInfo.cs.md - 击中信息结构
- [x] HitBoxType.cs.md - HitBox 类型枚举

### Code/Game/Entity (实体系统) (100%)
- [x] Entity.cs.md - 实体基类
- [x] SceneEntity.cs.md - 场景实体基类
- [x] Unit.cs.md - 场景单位基类
- [x] Character.cs.md - 角色基类
- [x] Player.cs.md - 玩家实体
- [x] NPC.cs.md - NPC 实体
- [x] Bidder.cs.md - 竞拍者实体
- [x] Animal.cs.md - 动物实体
- [x] Host.cs.md - 主机实体
- [x] Box.cs.md - 宝盒实体
- [x] IEntity.cs.md - 实体接口定义

### Code/Game/Scene (场景系统) (100%)
- [x] SceneManager.cs.md - 场景管理器
- [x] IScene.cs.md - 场景接口
- [x] LoadingScene.cs.md - 加载场景
- [x] SceneManagerProvider.cs.md - 场景管理器提供者
- [x] HomeScene.cs.md - 家园场景
- [x] MapScene.cs.md - 地图场景基类
- [x] CreateScene.cs.md - 角色创建场景
- [x] GuideScene.cs.md - 引导场景

### Code/Game/UI/UILoading (加载 UI) (100%)
- [x] UILoadingView.cs.md - 加载界面基类
- [x] UILoadingView2.cs.md - 加载界面版本 2（渐变遮罩）
- [x] UIEnterView.cs.md - 入场动画视图
- [x] UIBlendView.cs.md - 转场淡入淡出视图
- [x] UINetView.cs.md - 网络状态视图

### Code/Game/UI/UIGuidance (引导 UI) (100%)
- [x] UIGuidanceView.cs.md - 引导视图（助手/对话/手势）
- [x] UIFirstGuidanceView.cs.md - 首次引导视图

### Mono/Module/UI/Input (100%)
- [x] InputAxisBind.cs.md - 虚拟摇杆轴绑定
- [x] InputKeyBind.cs.md - UI 按键绑定

### Mono/Module/UI (辅助组件) (100%)
- [x] BgAutoMax.cs.md - 背景自适应组件
- [x] CircleImage.cs.md - 圆形/扇形 Image 组件
- [x] CircleRawImage.cs.md - 圆角 RawImage 组件
- [x] CopyGameObject.cs.md - 游戏对象复制组件
- [x] EmptyGraphic.cs.md - 空图形组件（透明点击区）
- [x] PointerClick.cs.md - 点击事件组件
- [x] ScrollViewEventRaycast.cs.md - 滚动视图事件传递组件
- [x] UIScriptCreator.cs.md - UI 脚本创建器标记组件

### Code/Game/UIGame/UICreate (角色创建 UI) (100%)
- [x] UICreateView.cs.md - 角色创建主界面
- [x] UIBagWin.cs.md - 背包窗口
- [x] UIShopWin.cs.md - 商店窗口
- [x] UIEquipWin.cs.md - 装备详情窗口
- [x] UIBuyWin.cs.md - 购买窗口
- [x] CreateItem.cs.md - 装备槽位项
- [x] ClothItem.cs.md - 装备物品项
- [x] ShopItem.cs.md - 商店商品项
- [x] TableItem.cs.md - 背包列表项
- [x] GroupInfoTable.cs.md - 套装信息表

### Mono/Module/YooAssets (资源管理) (100%)
- [x] PackageManager.cs.md - 资源包管理器核心
- [x] PackageConfig.cs.md - 资源包配置
- [x] BuildInPackageConfig.cs.md - 内置包配置
- [x] CDNConfig.cs.md - CDN 配置
- [x] RemoteServices.cs.md - 远程服务
- [x] StreamingAssetsHelper.cs.md - 构建助手
- [x] BundleDecryption.cs.md - 资源解密

### Code/Module/Config/Value (配置值类型) (100%)
- [x] BaseValue.cs.md - 值基类
- [x] FormulaValue.cs.md - 公式值
- [x] SingleValue.cs.md - 固定值
- [x] ZeroValue.cs.md - 零值
- [x] Range01Value.cs.md - 0-1 范围值
- [x] OperatorValue.cs.md - 运算符值
- [x] LogicMode.cs.md - 逻辑模式
- [x] RandomAuctionTime.cs.md - 随机竞拍时间
- [x] MinAuctionTime.cs.md - 最小竞拍时间
- [x] TimeSinceLastBid.cs.md - 距上次竞拍时间

### Code/Game/Component (组件系统) (100%)
- [x] Component.cs.md - 组件基类
- [x] IComponent.cs.md - 组件接口定义
- [x] Numeric/NumericComponent.cs.md - 数值组件
- [x] Numeric/INumericReplace.cs.md - 数值替换接口
- [x] Numeric/NumericChange.cs.md - 数值变化事件
- [x] Numeric/FormulaStringFx.cs.md - 公式解析器
- [x] Type/BidderComponent.cs.md - 竞拍者组件
- [x] View/BlackBoyComponent.cs.md - 黑色角色效果组件
- [x] View/CasualActionComponent.cs.md - 休闲动作组件
- [x] View/GameObjectHolderComponent.cs.md - 游戏对象持有组件

### Code/Game/System/Numeric (数值系统) (100%)
- [x] NumericSystem.cs.md - 数值系统管理器

### Code/Game/System/Environment (环境系统) (100%)
- [x] EnvironmentPriorityType.cs.md - 环境优先级
- [x] Data/DayTimeType.cs.md - 昼夜类型
- [x] Runner/DayEnvironmentRunner.cs.md - 昼夜环境运行器
- [x] EnvironmentManager.cs.md - 环境管理器
- [x] EnvironmentManager.Light.cs.md - 光照应用
- [x] EnvironmentManager.Skybox.cs.md - 天空盒应用
- [x] EnvironmentInfo.cs.md - 环境配置数据
- [x] Runner/NormalEnvironmentRunner.cs.md - 普通环境运行器
- [x] Runner/BlenderEnvironmentRunner.cs.md - 混合环境运行器
- [x] Runner/EnvironmentRunner.cs.md - 环境运行器基类

### Code/Game/System/Auction (拍卖系统) (新增)
- [x] AuctionManager.AIMiniPlay.cs.md - AI 小玩法模拟

### Code/Game/UIGame/UIAuction (拍卖 UI) (100%)
- [x] UIAssistantView.cs.md - 助手对话视图
- [x] UIAuctionItem.cs.md - 拍卖物品项组件
- [x] UIBubbleItem.cs.md - 对话气泡项
- [x] UIButtonView.cs.md - 拍卖结算按钮视图
- [x] UIDiceWin.cs.md - 骰子选择窗口
- [x] UIEmojiItem.cs.md - 表情项组件
- [x] UIGameInfoView.cs.md - 游戏情报选择视图
- [x] UIGameView.cs.md - 游戏主界面
- [x] UIGuideGameView.cs.md - 引导游戏视图
- [x] UIItemsView.cs.md - 物品列表视图
- [x] UIItemStoryWin.cs.md - 物品故事窗口
- [x] UIRaiseSuccessWin.cs.md - 抬价成功窗口
- [x] UIReportItem.cs.md - 结算报告列表项
- [x] UIReportWin.cs.md - 对局结算报告窗口
- [x] UISuccessAuction.cs.md - 竞拍成功动画
- [x] UITargetView.cs.md - 目标瞄准视图
- [x] UITaskInfoWin.cs.md - 任务详情窗口
- [x] GameInfoItem.cs.md - 情报项组件
- [x] TaskListItem.cs.md - 任务列表项组件

### Code/Game/UIGame/UITT (抖音小游戏 UI) (100%)
- [x] UISidebarRewardsWin.cs.md - 侧边栏奖励窗口
- [x] TurntableItem.cs.md - 转盘奖励项组件

### Code/Game/UIGame/UIMiniGame (小游戏 UI) (100%)
- [x] UIAppraisalView.cs.md - 鉴定小游戏
- [x] UIAppraisalItem.cs.md - 鉴定物品项组件
- [x] UIBombDisposalView.cs.md - 拆弹小游戏视图
- [x] UICommonMiniGameView.cs.md - 小游戏通用视图基类
- [x] UIGoodsCheckView.cs.md - 验货小游戏视图
- [x] UIItemStoryWin.cs.md - 物品故事窗口
- [x] UIQuarantineView.cs.md - 检疫小游戏视图
- [x] UIRepairView.cs.md - 修理小游戏视图
- [x] UISaleEvent.cs.md - 销售事件小游戏视图
- [x] UITurnTableEventView.cs.md - 转盘事件视图
- [x] UITurntableView.cs.md - 大厅转盘视图
- [x] TurntableItem.cs.md - 转盘奖励项组件

### Code/Game/UIGame/UICreate (角色创建 UI) (部分完成)
- [x] GroupInfo.cs.md - 套装信息
- [x] EffectItem.cs.md - 效果项
- [ ] UICreateView.cs (已完成)
- [ ] UIBagWin.cs (已完成)
- [ ] UIShopWin.cs (已完成)
- [ ] UIEquipWin.cs (已完成)
- [ ] UIBuyWin.cs (已完成)
- [ ] CreateItem.cs (已完成)
- [ ] ClothItem.cs (已完成)
- [ ] ShopItem.cs (已完成)
- [ ] TableItem.cs (已完成)
- [ ] GroupInfoTable.cs (已完成)

### Code/Game/UIGame/UILobby (大厅 UI) (100%)
- [x] DailyTaskItem.cs.md - 每日任务项
- [x] UILobbyView.cs.md - 大厅主界面
- [x] UIDailyWin.cs.md - 每日任务奖励窗口
- [x] UIProfitWin.cs.md - 餐厅收益窗口
- [x] UITaskDetailsWin.cs.md - 任务详情窗口
- [x] UIMarketView.cs.md - 市场界面
- [x] UIAuctionSelectView.cs.md - 拍卖选择界面
- [x] UIRankView.cs.md - 排行榜界面
- [x] UICashGroup.cs.md - 金币显示组件
- [x] UIMatchView.cs.md - 匹配界面
- [x] DailyTaskRewards.cs.md - 每日任务阶段奖励组件
- [x] UserItem.cs.md - 玩家头像项组件
- [x] RankItem.cs.md - 排行榜项组件
- [x] UIRewardsView.cs.md - 奖励展示窗口
- [x] UIUnlockWin.cs.md - 解锁确认窗口
- [x] UIExpandWin.cs.md - 收益时间扩展窗口
- [x] UISettingWin.cs.md - 设置窗口
- [x] UIRankBtn.cs.md - 排行榜按钮（SDK 集成）
- [x] AuctionSelectItem.cs.md - 拍卖场选择项组件
- [x] UIWashDishView.cs.md - 餐厅洗碗界面
- [x] TechnologyNode.cs.md - 科技树节点组件
- [x] TechnologyNodeItem.cs.md - 科技树子节点组件
- [x] UIBlackView.cs.md - 黑名单/科技树界面
- [x] RestaurantTask.cs.md - 餐厅任务项组件
- [x] UITopView.cs.md - 顶部信息栏视图

---

## 📋 待处理文件（按优先级）

### 🔴 高优先级 - 核心框架

#### Mono/Core/Object (数据结构组件) (100%)
- (全部完成)

#### Mono/Module/Assembly (程序集管理) (100%)
- (全部完成)

#### Mono/Module/Log (日志系统) (100%)
- (全部完成)

#### Mono/Module/Http (HTTP 请求) (100%)
- (全部完成)

#### Mono/Module/I18N (国际化) (部分完成)
- [x] I18NText.cs.md
- [ ] I18NBridge.cs
- [x] TextMeshFontAssetManager.cs.md

#### Mono/Module/Entity (实体系统) (100%)
- [x] EntityComponent.cs.md
- [x] EntityType.cs.md
- [x] Hit/ 目录全部完成

#### Mono/Module/UI (UI 辅助) (部分完成)
- [x] ReferenceCollector.cs.md
- [x] Drag.cs.md
- [ ] PointerClick.cs
- [ ] BgAutoFit.cs
- [ ] BgAutoMax.cs
- [ ] CircleImage.cs
- [ ] CircleRawImage.cs
- [ ] EmptyGraphic.cs
- [ ] ScrollViewEventRaycast.cs
- [ ] UIScriptCreator.cs

#### Mono/Helper (工具类) (100%)
- (全部完成)

#### Mono/Module/YooAssets (资源管理) (100%)
- (全部完成)

---

### 🟡 中优先级 - 游戏系统

#### Code/Module/Config (配置系统) (部分完成)
- [x] ConfigLoader.cs.md - 配置加载器
- [x] ConfigAttribute.cs.md - 配置类标记特性
- [x] NotNullAttribute.cs.md - 非空标记特性
- [x] ProtoObject.cs.md - Protobuf 对象基类
- [x] IMerge.cs.md - 合并接口
- [ ] ProtobufHelper.cs
- [ ] IConfigLoader.cs
- [ ] OdinDropdownHelper.cs

#### Code/Module/Const (常量定义)
- [ ] TimerType.cs
- [x] GameInfoType.cs.md
- [x] MessageId.cs.md

#### Mono/Module/Update (更新系统)
- [x] UpdateTimer.cs.md
- [x] IUpdate.cs.md

#### Mono/Module/TimeLine (时间线)
- [x] MessagerTrack.cs.md
- [x] MessagerClip.cs.md
- [x] MessagerBehaviour.cs.md

#### Mono/Module/Particle (粒子系统) (100%)
- (全部完成)

#### Mono/Module/Performance (性能管理) (100%)
- (全部完成)

---

### 🟢 低优先级 - 其他

#### Code/Module/Config/DecisionTree (AI 决策树)
- [ ] ConfigAIDecisionTree.cs
- [ ] ActDecision.cs
- [ ] DecisionActionNode.cs
- [ ] AITactic.cs
- [ ] DecisionConditionNode.cs
- [ ] DecisionCompareNode.cs
- [ ] CompareMode.cs
- [ ] DecisionNode.cs

#### Code/Module/Config/Environment (环境配置)
- [ ] ConfigEnvironment.cs
- [ ] ConfigEnvironments.cs

#### Code/Module/Config/Blender
- [ ] ConfigBlender.cs

#### Mono/Module/Skybox
- [ ] Skybox.cs

#### Mono/Module/UI/Input (输入绑定)
- [ ] InputAxisBind.cs
- [ ] InputKeyBind.cs

#### Mono/Module/UI (UI 效果 - 部分完成)
- [x] TextColorCtrl.cs.md
- [x] BgRawAutoFit.cs.md
- [ ] BgAutoFit.cs
- [ ] BgAutoMax.cs
- [ ] CircleImage.cs
- [x] BackgroundBlur.cs.md
- [ ] EmptyGraphic.cs
- [ ] CircleRawImage.cs
- [ ] ScrollViewEventRaycast.cs
- [ ] UIScriptCreator.cs

#### Mono/Module/Const (其他常量)
- [ ] GameSetting.cs

#### Mono/其他
- [ ] Define.cs
- [x] Init.cs.md
- [ ] SetUIData.cs
- [ ] WebGLPlatform.cs

---

## 📝 下一步建议

### 建议处理顺序

1. **剩余核心框架** (优先完成)
   - Mono/Module/I18N/I18NBridge.cs
   - Mono/Module/Skybox/Skybox.cs
   - Mono/Define.cs
   - Mono/WebGLPlatform.cs
   - Mono/SetUIData.cs

2. **UI 辅助系统**
   - Mono/Module/UI 剩余文件 (BgAutoFit, CircleImage, EmptyGraphic 等)
   - Mono/Module/UI/Input 输入绑定

3. **配置系统**
   - ConfigLoader.cs 及配置基础类
   - AI 决策树配置

4. **游戏 UI 系统**
   - UILobby 大厅 UI
   - UIAuction 拍卖 UI
   - UIMiniGame 小游戏 UI

---

## 📌 备注

- Generate 目录下的文件是自动生成的配置类，暂不处理
- 游戏具体逻辑代码（Game 目录）优先级较低
- 优先完成框架层文档，便于新成员快速上手

---

*最后更新：2026-03-01 18:30*
