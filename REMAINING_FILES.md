# 剩余待文档化文件清单

**生成时间**: 2026-02-27  
**总代码文件**: ~267 个（排除 Generate/Editor）  
**已完成文档**: 87 个  
**剩余文档**: ~180 个  

---

## 📊 按模块分类

### 1. 游戏 UI 窗口（~80 个文件）⭐⭐⭐⭐⭐

#### UICommon（7 个）
- UIMsgBoxWin.cs - 提示框窗口
- UIRareAnim.cs - 稀有度动画
- UIToast.cs - Toast 提示
- UIMenuItem.cs - 菜单项
- UICopyWin.cs - 复制窗口
- UISliderToggle.cs - 滑块开关
- UIMenu.cs - 菜单
- UILoginWin.cs - 登录窗口

#### UILoading（5 个）
- UIBlendView.cs - 混合加载视图
- UINetView.cs - 网络加载视图
- UILoadingView2.cs - 加载视图 2
- UILoadingView.cs - 加载视图
- UIEnterView.cs - 进入视图

#### UIGuidance（2 个）
- UIGuidanceView.cs - 引导视图
- UIFirstGuidanceView.cs - 首次引导视图

#### UIUpdate（1 个）
- UIUpdateView.cs - 更新视图

#### UIGame/UILobby（~25 个）
- UILobbyView.cs - 大厅视图
- UIDailyWin.cs - 每日任务窗口
- UIProfitWin.cs - 收益窗口
- UITaskDetailsWin.cs - 任务详情窗口
- UIMarketView.cs - 市场视图
- UIRankView.cs - 排行视图
- UIMatchView.cs - 匹配视图
- UIRewardsView.cs - 奖励视图
- UIUnlockWin.cs - 解锁窗口
- UIExpandWin.cs - 扩展窗口
- UIBlackView.cs - 黑名单视图
- UISettingWin.cs - 设置窗口
- UIAuctionSelectView.cs - 拍卖选择视图
- UIWashDishView.cs - 洗碗视图
- TechnologyNode.cs - 科技节点
- TechnologyNodeItem.cs - 科技节点项
- DailyTaskItem.cs - 每日任务项
- DailyTaskRewards.cs - 每日任务奖励
- UserItem.cs - 用户项
- RankItem.cs - 排行项
- AuctionSelectItem.cs - 拍卖选择项
- UICashGroup.cs - 现金组
- RestaurantTask.cs - 餐厅任务
- UITopView.cs - 顶部视图
- UIRankBtn.cs - 排行按钮

#### UIGame/UIAuction（~15 个）
- UIButtonView.cs - 按钮视图
- UIAuctionView.cs - 拍卖视图
- UIAuctionItem.cs - 拍卖项
- UIBidderItem.cs - 竞拍者项
- UIResultView.cs - 结果视图
- （其他拍卖相关 UI）

#### UIGame/UICreate（~12 个）
- UICreateView.cs - 创建视图
- UIBagWin.cs - 背包窗口
- UIShopWin.cs - 商店窗口
- UIEquipWin.cs - 装备窗口
- UIBuyWin.cs - 购买窗口
- CreateItem.cs - 创建项
- ClothItem.cs - 服装项
- EffectItem.cs - 效果项
- ShopItem.cs - 商店项
- TableItem.cs - 表格项
- GroupInfo.cs - 组信息
- GroupInfoTable.cs - 组信息表

#### UIGame/UIMiniGame（~12 个）
- UIAppraisalView.cs - 鉴定视图
- UIAppraisalItem.cs - 鉴定项
- UIQuarantineView.cs - 检疫视图
- UIGoodsCheckView.cs - 货物检查视图
- UIRepairView.cs - 修复视图
- UIBombDisposalView.cs - 拆弹视图
- UITurntableView.cs - 转盘视图
- UITurnTableEventView.cs - 转盘事件视图
- UISaleEvent.cs - 销售事件
- UIItemStoryWin.cs - 物品故事窗口
- UICommonMiniGameView.cs - 通用小游戏视图

#### UIGame/UITT（1 个）
- UISidebarRewardsWin.cs - 侧边栏奖励窗口

---

### 2. 游戏系统（~20 个文件）⭐⭐⭐⭐

#### NumericSystem（1 个）
- NumericSystem.cs - 数值系统

#### Entity（2 个）
- ClothGenerateManager.cs - 服装生成管理器
- EntityManager.cs - 实体管理器（已有部分文档）

#### Environment（4 个）
- EnvironmentManager.Light.cs - 光照管理（Partial）
- EnvironmentManager.Skybox.cs - 天空盒管理（Partial）
- EnvironmentPriorityType.cs - 环境优先级类型
- DayTimeType.cs - 白天时间类型
- DayEnvironmentRunner.cs - 白天环境运行器

---

### 3. 游戏组件（~10 个文件）⭐⭐⭐

#### Numeric（4 个）
- NumericComponent.cs - 数值组件
- INumericReplace.cs - 数值替换接口
- NumericChange.cs - 数值变化
- FormulaStringFx.cs - 公式字符串函数

#### Type（1 个）
- BidderComponent.cs - 竞拍者组件

#### View（3 个）
- BlackBoyComponent.cs - 黑人男孩组件
- CasualActionComponent.cs - 休闲动作组件
- GameObjectHolderComponent.cs - GameObject 持有者组件

#### AI/Decision（1 个）
- AIDecisionInterface.cs - AI 决策接口方法（14KB，重要）

#### 根目录（2 个）
- Component.cs - 组件基类
- IComponent.cs - 组件接口

---

### 4. 框架层 Module（~40 个文件）⭐⭐⭐⭐

#### Player（8 个）
- PlayerData.cs - 玩家数据
- CacheManager.cs - 缓存管理器
- GameRecorderManager.cs - 游戏录制管理器
- AdManager.cs - 广告管理器
- AuctionReport.cs - 拍卖报告

#### Net（5 个）
- LoginPlatform.cs - 登录平台
- HttpResult.cs - HTTP 结果
- LoginResult.cs - 登录结果
- RankInfo.cs - 排行信息

#### I18N（4 个）
- I18NManager.cs - 国际化管理器
- I18NConfig.cs - 国际化配置
- II18N.cs - 国际化接口
- II18NConfig.cs - 国际化配置接口

#### Resource（3 个）
- MaterialManager.cs - 材质管理器
- ShockManager.cs - 震动管理器
- IPackageFinder.cs - 包查找器接口

#### Log（2 个）
- LogManager.cs - 日志管理器
- InterceptorLogger.cs - 拦截器日志

#### CoroutineLock（6 个）
- CoroutineLockManager.cs - 协程锁管理器
- CoroutineLock.cs - 协程锁
- CoroutineLockQueue.cs - 协程锁队列
- CoroutineLockType.cs - 协程锁类型
- CoroutineLockQueueType.cs - 协程锁队列类型
- CoroutineLockTimer.cs - 协程锁定时器

#### Const（10 个）
- GameConst.cs - 游戏常量
- CacheKeys.cs - 缓存键
- I18NKey.cs - 国际化键（重要）
- NumericType.cs - 数值类型
- ClothEffectType.cs - 服装效果类型
- BoxType.cs - 箱子类型
- TaskState.cs - 任务状态
- LangType.cs - 语言类型
- ItemType.cs - 物品类型

#### Config（1 个）
- ConfigManager.cs - 配置管理器（已有文档）

#### Scene（1 个）
- SceneManagerProvider.cs - 场景管理器提供者

#### Input（2 个）
- GameKeyCode.cs - 游戏按键码
- TouchInfo.cs - 触摸信息

#### Update（1 个）
- UpdateRes.cs - 更新结果

#### Camera（2 个）
- CameraManager.URP.cs - URP 相机管理（Partial）
- CameraManager.TaoTieRP.cs - TaoTieRP 相机管理（Partial）

---

### 5. Mono 层（~30 个文件）⭐⭐⭐

#### Messager（已有文档）

#### Timer（可能需要补充）
- TimerManager.cs - 定时器管理器（已有文档）

#### Entity（Mono 层）
- Entity.cs - 实体基类

#### UI（Mono 层）
- UIBaseContainer.cs - UI 容器基类
- 其他 UI 基类

#### 其他 Mono 模块
- GameSetting.cs - 游戏设置
- SetUIData.cs - 设置 UI 数据

---

### 6. 拍卖系统剩余（1 个）⭐⭐⭐⭐⭐

#### Auction（1 个）
- AuctionManager.AIMiniPlay.cs - AI 和小玩法（Partial）
- AuctionGuideManager.State.cs - 引导拍卖状态（Partial）
- AuctionGuideManager.Anim.cs - 引导拍卖动画（Partial）
- AuctionGuideManager.API.cs - 引导拍卖 API（Partial）

---

## 📋 优先级建议

### 第一优先级（⭐⭐⭐⭐⭐）- 核心玩法
1. **游戏 UI 窗口**（~80 个）- 玩家直接感知的界面
2. **拍卖系统剩余**（4 个）- 核心玩法补充
3. **AI 决策接口**（1 个）- AI 行为核心

### 第二优先级（⭐⭐⭐⭐）- 框架层
4. **Player 模块**（8 个）- 玩家数据和管理
5. **I18N 模块**（4 个）- 国际化支持
6. **CoroutineLock**（6 个）- 并发控制
7. **Const 常量**（10 个）- 全局常量定义

### 第三优先级（⭐⭐⭐）- 系统支持
8. **游戏系统**（~20 个）- 数值、实体、环境
9. **游戏组件**（~10 个）- 组件系统
10. **Net 网络**（5 个）- 网络通信
11. **Resource 资源**（3 个）- 资源管理补充

### 第四优先级（⭐⭐）- 辅助功能
12. **Log 日志**（2 个）- 日志系统
13. **Mono 层**（~30 个）- 底层框架

---

## 📝 文档化建议

### 批量处理策略

1. **按 UI 窗口批量**（每次 5-10 个）
   - UICommon 窗口（7 个）
   - UILoading 视图（5 个）
   - UILobby 窗口（~25 个，分 3 批）
   - UIAuction 窗口（~15 个，分 2 批）
   - UICreate 窗口（~12 个，分 2 批）
   - UIMiniGame 视图（~12 个，分 2 批）

2. **按系统模块批量**（每次 3-5 个）
   - Numeric 系统（5 个）
   - Entity 系统（2 个）
   - Environment 补充（5 个）
   - Component 组件（10 个）

3. **按框架层批量**（每次 5-8 个）
   - Player 模块（8 个）
   - I18N 模块（4 个）
   - CoroutineLock（6 个）
   - Const 常量（10 个，分 2 批）

---

## 📊 预计工作量

| 类别 | 文件数 | 预计时间 |
|------|--------|---------|
| 游戏 UI 窗口 | ~80 | 16-20 小时 |
| 游戏系统 | ~20 | 4-5 小时 |
| 游戏组件 | ~10 | 2-3 小时 |
| 框架层 Module | ~40 | 8-10 小时 |
| Mono 层 | ~30 | 6-8 小时 |
| 拍卖系统剩余 | 4 | 1 小时 |
| **总计** | **~184** | **37-49 小时** |

**按每日 3-4 小时计算**: 约 10-14 个工作日

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
