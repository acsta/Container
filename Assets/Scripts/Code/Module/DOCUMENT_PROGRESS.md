# 文档进度追踪

> **更新时间**: 2026-03-01 18:30  
> **统计范围**: Assets/Scripts 目录

---

## 📊 总体统计

| 类别 | 数量 |
|------|------|
| 总 .cs 文件数 | ~646 |
| 已创建 .md 文档 | 137 |
| 覆盖率 | ~21% |

---

## ✅ 已完成的模块

### Core 核心模块 (100%)
- [x] ObjectPool.cs - 对象池核心
- [x] ManagerProvider.cs - 管理器注册与调度中心
- [x] IManager.cs - 管理器接口定义

### Timer 定时器模块 (100%)
- [x] TimerManager.cs - 定时器管理系统
- [x] TimeInfo.cs - 时间信息服务
- [x] GameTimerManager.cs - 游戏时间管理器
- [x] ITimer.cs - 定时器处理器接口
- [x] TimerAction.cs - 定时器动作数据结构

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

---

## 📋 待处理文件（按优先级）

### 🔴 高优先级 - 核心框架

#### Mono/Core (对象系统)
- [ ] Object/HashSetComponent.cs
- [ ] Object/MultiMapSet.cs
- [ ] Object/IdGenerater.cs
- [ ] Object/ListComponent.cs
- [ ] Object/DynDictionary.cs
- [ ] Object/UnOrderMultiMapSet.cs
- [ ] Object/BigNumber.cs
- [ ] Object/LinkedListComponent.cs
- [ ] Object/MultiMap.cs
- [ ] Object/LruCache.cs
- [ ] Object/UnOrderDoubleKeyMap.cs
- [ ] Object/DictionaryComponent.cs
- [ ] Object/UnOrderMultiMap.cs
- [ ] Object/UnOrderDoubleKeyMapSet.cs

#### Mono/Module/Assembly (程序集管理)
- [ ] AttributeManager.cs
- [ ] BaseAttribute.cs
- [ ] AssemblyManager.cs

#### Mono/Module/Log (日志系统)
- (已完成)

#### Mono/Module/Http (HTTP 请求)
- [ ] AcceptAllCertificate.cs
- [ ] HttpManager.cs

#### Mono/Module/I18N (国际化)
- [ ] I18NText.cs
- [ ] I18NBridge.cs
- [ ] TextMeshFontAssetManager.cs

#### Mono/Module/Entity (实体系统)
- [ ] EntityComponent.cs
- [ ] EntityType.cs
- [ ] Hit/ 目录下的击中检测相关

#### Mono/Module/UI (UI 辅助)
- [ ] ReferenceCollector.cs - UI 引用收集器
- [ ] Drag.cs
- [ ] PointerClick.cs

#### Mono/Helper (工具类)
- [ ] SystemInfoHelper.cs - 系统信息助手
- [ ] JsonHelper.cs
- [ ] PhysicsHelper.cs
- [ ] EasingFunction.cs - 缓动函数
- [ ] TypeInfo.cs
- [ ] BridgeHelper.cs
- [ ] PlatformUtil.cs

#### Mono/Module/YooAssets (资源管理)
- [ ] PackageManager.cs
- [ ] PackageConfig.cs
- [ ] StreamingAssetsHelper.cs

---

### 🟡 中优先级 - 游戏系统

#### Code/Module/Config (配置系统)
- [ ] ConfigLoader.cs
- [ ] ConfigAttribute.cs
- [ ] ProtobufHelper.cs
- [ ] IConfigLoader.cs
- [ ] ProtoObject.cs
- [ ] IMerge.cs
- [ ] OdinDropdownHelper.cs
- [ ] NotNullAttribute.cs

#### Code/Module/Const (常量定义)
- [ ] TimerType.cs
- [ ] GameInfoType.cs
- [ ] MessageId.cs

#### Mono/Module/Update (更新系统)
- [ ] UpdateTimer.cs
- [ ] IUpdate.cs

#### Mono/Module/TimeLine (时间线)
- [ ] MessagerTrack.cs
- [ ] MessagerClip.cs
- [ ] MessagerBehaviour.cs

#### Mono/Module/Particle (粒子系统)
- [ ] ParticleSimulationBudgetManager.cs
- [ ] ParticleSystemController.cs

#### Mono/Module/Performance (性能管理)
- [ ] PerformanceManager.cs

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

#### Code/Module/Config/Value (配置值类型)
- [ ] Range01Value.cs
- [ ] OperatorValue.cs
- [ ] ZeroValue.cs
- [ ] RandomAuctionTime.cs
- [ ] LogicMode.cs
- [ ] FormulaValue.cs
- [ ] SingleValue.cs
- [ ] MinAuctionTime.cs
- [ ] TimeSinceLastBid.cs
- [ ] BaseValue.cs

#### Code/Module/Config/Environment (环境配置)
- [ ] ConfigEnvironment.cs
- [ ] ConfigEnvironments.cs

#### Code/Module/Config/Blender
- [ ] ConfigBlender.cs

#### Mono/Module/CodeLoader (代码加载)
- [ ] CodeLoader.cs
- [ ] IStaticMethod.cs
- [ ] MonoStaticMethod.cs

#### Mono/Module/Skybox
- [ ] Skybox.cs

#### Mono/Module/Entity/Hit (击中检测)
- [ ] ColliderBoxComponent.cs
- [ ] CheckHitLayerType.cs
- [ ] HitBoxComponent.cs
- [ ] HitInfo.cs
- [ ] HitBoxType.cs
- [ ] ExportBones.cs
- [ ] BonesData.cs

#### Mono/Module/UI/Input (输入绑定)
- [ ] InputAxisBind.cs
- [ ] InputKeyBind.cs

#### Mono/Module/UI (UI 效果)
- [ ] TextColorCtrl.cs
- [ ] BgRawAutoFit.cs
- [ ] BgAutoFit.cs
- [ ] BgAutoMax.cs
- [ ] CircleImage.cs
- [ ] BackgroundBlur.cs
- [ ] EmptyGraphic.cs
- [ ] CircleRawImage.cs
- [ ] ScrollViewEventRaycast.cs
- [ ] UIScriptCreator.cs

#### Mono/Module/Const (其他常量)
- [ ] GameSetting.cs

#### Mono/Module/YooAssets (其他)
- [ ] BuildInPackageConfig.cs
- [ ] RemoteServices.cs
- [ ] CDNConfig.cs
- [ ] BundleDecryption.cs

#### Mono/Helper (其他)
- [ ] BridgeHelper.WebGL.cs
- [ ] UnityLifeTimeHelper.cs
- [ ] RangeHelper.cs
- [ ] CDNConfigHelper.cs
- [ ] SkipUnityLogo.cs

#### Mono/其他
- [ ] Define.cs
- [ ] Init.cs
- [ ] SetUIData.cs
- [ ] WebGLPlatform.cs

---

## 📝 下一步建议

### 建议处理顺序

1. **核心框架层** (优先完成)
   - Mono/Core 对象系统（数据结构基础）
   - Mono/Module/Timer 定时器系统
   - Mono/Module/Log 日志系统
   - Mono/Helper/SystemInfoHelper.cs

2. **UI 辅助系统**
   - ReferenceCollector.cs
   - UI 输入绑定相关

3. **配置系统**
   - ConfigLoader.cs
   - 配置基础类

4. **游戏系统**
   - 实体系统
   - AI 决策树
   - 其他游戏逻辑

---

## 📌 备注

- Generate 目录下的文件是自动生成的配置类，暂不处理
- 游戏具体逻辑代码（Game 目录）优先级较低
- 优先完成框架层文档，便于新成员快速上手

---

*最后更新：2026-03-01 18:30*
