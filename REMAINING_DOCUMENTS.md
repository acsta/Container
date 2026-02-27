# 剩余待文档化文件归档

**生成时间**: 2026-02-27 22:08  
**总代码文件**: 267 个（排除 Generate/Editor）  
**已完成文档**: 98 个  
**剩余文档**: 169 个  

---

## 📊 总体进度

| 类别 | 已完成 | 剩余 | 完成度 |
|------|--------|------|--------|
| **框架层 Module** | 78 | 50 | 61% |
| **游戏 UI 窗口** | 0 | 84 | 0% |
| **游戏系统** | 11 | 14 | 44% |
| **游戏组件** | 7 | 8 | 47% |
| **Mono 层** | 2 | 13 | 13% |
| **总计** | **98** | **169** | **37%** |

---

## 📁 按模块分类

### 1. 框架层 Module（剩余 50 个）

#### Const 常量（2 个）✅ 基本完成
- [ ] I18NConfig.cs
- [ ] II18NConfig.cs

#### CoroutineLock（2 个）✅ 完成
- [x] CoroutineLockQueue.cs（已有）
- [x] CoroutineLockType.cs（已有）

#### Player 玩家（3 个）✅ 完成
- [x] PlayerManager.cs（已有）
- [x] PlayerDataManager.cs（已有）
- [x] SDKManager.cs（已有）

#### Net 网络（2 个）
- [ ] LoginPlatform.cs
- [ ] HttpResult.cs

#### Resource 资源（2 个）
- [ ] IPackageFinder.cs
- [ ] GameObjectPoolManager.cs（已有）

#### Log 日志（1 个）
- [ ] InterceptorLogger.cs

#### I18N 国际化（1 个）
- [ ] II18NConfig.cs

#### Scene 场景（1 个）
- [ ] SceneManagerProvider.cs

#### Update 更新（1 个）
- [ ] UpdateRes.cs

#### Camera 相机（2 个）
- [ ] CameraManager.URP.cs
- [ ] CameraManager.TaoTieRP.cs

#### Config 配置（1 个）
- [ ] ConfigManager.cs（可能有）

#### 其他 Module 文件（32 个）
- Guidance 引导模块
- Performance 性能模块
- TimeLine 时间线模块
- Skybox 天空盒模块
- Particle 粒子模块
- Http HTTP 模块
- YooAssets 资源模块
- Const 其他常量

---

### 2. 游戏 UI 窗口（剩余 84 个）⭐⭐⭐⭐⭐

#### UICommon（7 个）
- [ ] UIMsgBoxWin.cs
- [ ] UIRareAnim.cs
- [ ] UIToast.cs
- [ ] UIMenuItem.cs
- [ ] UICopyWin.cs
- [ ] UISliderToggle.cs
- [ ] UIMenu.cs
- [ ] UILoginWin.cs

#### UILoading（5 个）
- [ ] UIBlendView.cs
- [ ] UINetView.cs
- [ ] UILoadingView2.cs
- [ ] UILoadingView.cs
- [ ] UIEnterView.cs

#### UIGuidance（2 个）
- [ ] UIGuidanceView.cs
- [ ] UIFirstGuidanceView.cs

#### UIUpdate（1 个）
- [ ] UIUpdateView.cs

#### UIGame/UILobby（~25 个）
- [ ] UILobbyView.cs
- [ ] UIDailyWin.cs
- [ ] UIProfitWin.cs
- [ ] UITaskDetailsWin.cs
- [ ] UIMarketView.cs
- [ ] UIRankView.cs
- [ ] UIMatchView.cs
- [ ] UIRewardsView.cs
- [ ] UIUnlockWin.cs
- [ ] UIExpandWin.cs
- [ ] UIBlackView.cs
- [ ] UISettingWin.cs
- [ ] UIAuctionSelectView.cs
- [ ] UIWashDishView.cs
- [ ] TechnologyNode.cs
- [ ] TechnologyNodeItem.cs
- [ ] DailyTaskItem.cs
- [ ] DailyTaskRewards.cs
- [ ] UserItem.cs
- [ ] RankItem.cs
- [ ] AuctionSelectItem.cs
- [ ] UICashGroup.cs
- [ ] RestaurantTask.cs
- [ ] UITopView.cs
- [ ] UIRankBtn.cs

#### UIGame/UIAuction（~15 个）
- [ ] UIButtonView.cs
- [ ] UIAuctionView.cs
- [ ] UIAuctionItem.cs
- [ ] UIBidderItem.cs
- [ ] UIResultView.cs
- [ ] （其他拍卖相关 UI）

#### UIGame/UICreate（~12 个）
- [ ] UICreateView.cs
- [ ] UIBagWin.cs
- [ ] UIShopWin.cs
- [ ] UIEquipWin.cs
- [ ] UIBuyWin.cs
- [ ] CreateItem.cs
- [ ] ClothItem.cs
- [ ] EffectItem.cs
- [ ] ShopItem.cs
- [ ] TableItem.cs
- [ ] GroupInfo.cs
- [ ] GroupInfoTable.cs

#### UIGame/UIMiniGame（~12 个）
- [ ] UIAppraisalView.cs
- [ ] UIAppraisalItem.cs
- [ ] UIQuarantineView.cs
- [ ] UIGoodsCheckView.cs
- [ ] UIRepairView.cs
- [ ] UIBombDisposalView.cs
- [ ] UITurntableView.cs
- [ ] UITurnTableEventView.cs
- [ ] UISaleEvent.cs
- [ ] UIItemStoryWin.cs
- [ ] UICommonMiniGameView.cs

#### UIGame/UITT（1 个）
- [ ] UISidebarRewardsWin.cs

---

### 3. 游戏系统（剩余 14 个）

#### Numeric 数值（1 个）
- [ ] NumericSystem.cs

#### Entity 实体（2 个）
- [ ] ClothGenerateManager.cs
- [ ] EntityManager.cs（部分完成）

#### Environment 环境（4 个）
- [ ] EnvironmentManager.Light.cs
- [ ] EnvironmentManager.Skybox.cs
- [ ] EnvironmentPriorityType.cs
- [ ] DayTimeType.cs
- [ ] DayEnvironmentRunner.cs

#### Auction 拍卖（4 个）
- [ ] AuctionManager.AIMiniPlay.cs
- [ ] AuctionGuideManager.State.cs
- [ ] AuctionGuideManager.Anim.cs
- [ ] AuctionGuideManager.API.cs

---

### 4. 游戏组件（剩余 8 个）

#### Numeric 数值（4 个）
- [ ] NumericComponent.cs
- [ ] INumericReplace.cs
- [ ] NumericChange.cs
- [ ] FormulaStringFx.cs

#### Type 类型（1 个）
- [ ] BidderComponent.cs

#### View 视图（3 个）
- [ ] BlackBoyComponent.cs
- [ ] CasualActionComponent.cs
- [ ] GameObjectHolderComponent.cs

#### AI/Decision（1 个）⭐ 重要
- [ ] AIDecisionInterface.cs（14KB，AI 决策条件方法）

#### 根目录（2 个）
- [ ] Component.cs
- [ ] IComponent.cs

---

### 5. Mono 层（剩余 13 个）

#### Entity 实体
- [ ] Entity.cs（Mono 层）

#### UI UI 基类
- [ ] UIBaseContainer.cs
- [ ] UIBaseView.cs（可能有）
- [ ] UIWindow.cs（可能有）
- [ ] UILayer.cs（可能有）
- [ ] UILayerNames.cs（可能有）
- [ ] UIManager.Layers.cs（可能有）
- [ ] IOnCreate.cs（可能有）
- [ ] IOnDestroy.cs（可能有）
- [ ] IOnEnable.cs（可能有）
- [ ] IOnDisable.cs（可能有）
- [ ] IOnBeforeCloseWin.cs（可能有）

#### 其他
- [ ] GameSetting.cs
- [ ] SetUIData.cs

---

## 📋 优先级建议

### 第一优先级（⭐⭐⭐⭐⭐）- 核心玩法
1. **游戏 UI 窗口**（84 个）- 玩家直接感知的界面
2. **拍卖系统剩余**（4 个）- 核心玩法补充
3. **AI 决策接口**（1 个）- AI 行为核心

### 第二优先级（⭐⭐⭐⭐）- 框架层
4. **Net 网络**（2 个）- 网络通信
5. **Resource 资源**（2 个）- 资源管理补充
6. **Camera 相机**（2 个）- 相机控制
7. **I18N 国际化**（1 个）- 国际化支持

### 第三优先级（⭐⭐⭐）- 系统支持
8. **游戏系统**（14 个）- 数值、实体、环境
9. **游戏组件**（8 个）- 组件系统
10. **Const 常量**（2 个）- 全局常量

### 第四优先级（⭐⭐）- 辅助功能
11. **Log 日志**（1 个）- 日志系统
12. **Mono 层**（13 个）- 底层框架

---

## 📝 批量处理建议

### 批次 1: UI 窗口（每次 5-10 个）
1. UICommon（7 个）
2. UILoading（5 个）
3. UIGuidance + UIUpdate（3 个）
4. UILobby（25 个，分 3 批）
5. UIAuction（15 个，分 2 批）
6. UICreate（12 个，分 2 批）
7. UIMiniGame（12 个，分 2 批）
8. UITT（1 个）

### 批次 2: 游戏系统（每次 3-5 个）
1. NumericSystem + Entity（3 个）
2. Environment（5 个）
3. Auction 剩余（4 个）

### 批次 3: 游戏组件（每次 3-5 个）
1. Numeric 组件（4 个）
2. View 组件（3 个）
3. AI 决策接口（1 个）
4. Component 基类（2 个）

### 批次 4: 框架层剩余（每次 5-8 个）
1. Net 网络（2 个）
2. Resource 资源（2 个）
3. Camera 相机（2 个）
4. I18N/Log/Scene/Update（5 个）
5. 其他 Module（32 个，分 5 批）

### 批次 5: Mono 层（每次 5 个）
1. UI 基类（11 个，分 3 批）
2. 其他（2 个）

---

## ⏱️ 预计工作量

| 类别 | 文件数 | 预计时间 |
|------|--------|---------|
| 游戏 UI 窗口 | 84 | 17-21 小时 |
| 游戏系统 | 14 | 3-4 小时 |
| 游戏组件 | 8 | 2-3 小时 |
| 框架层 Module | 50 | 10-13 小时 |
| Mono 层 | 13 | 3-4 小时 |
| **总计** | **169** | **35-45 小时** |

**按每日 3-4 小时计算**: 约 9-12 个工作日

---

## 📊 已完成模块总结

### ✅ 已完成 100% 的模块
- UI 组件（18/18）🎉
- AI 系统（4/4）🎉
- 环境系统（5/5）🎉
- 热更新系统（11/11）🎉
- 输入/场景（3/3）🎉
- Const 常量（8/10）≈ 完成
- CoroutineLock（6/6）🎉
- Player 玩家（5/5）🎉

### ⏳ 进行中的模块
- 框架层 Module（78/128）≈ 61%
- 游戏系统（11/25）≈ 44%
- 游戏组件（7/15）≈ 47%

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
