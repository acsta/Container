using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TaoTie
{
	public class AIDecisionInterface
	{
		public static Dictionary<string, Func<AIKnowledge, bool>> Methods;
#if UNITY_EDITOR
		public static Dictionary<string, string> MethodNames;
#endif
		static AIDecisionInterface()
		{
			Methods = new Dictionary<string, Func<AIKnowledge, bool>>();
#if UNITY_EDITOR
			MethodNames = new Dictionary<string, string>();
#endif
			var methodInfos = TypeInfo<AIDecisionInterface>.Type.GetMethods();
			for (int i = 0; i < methodInfos.Length; i++)
			{
				if (!methodInfos[i].IsStatic)
				{
					continue;
				}
				var func = (Func<AIKnowledge, bool>)Delegate.CreateDelegate(TypeInfo<Func<AIKnowledge, bool>>.Type, null, methodInfos[i]);
				Methods.Add(methodInfos[i].Name,func);
#if UNITY_EDITOR
				var attrs = methodInfos[i].GetCustomAttributes(TypeInfo<LabelTextAttribute>.Type,false);
				if (attrs.Length > 0)
				{
					MethodNames.Add(methodInfos[i].Name,$"{(attrs[0] as LabelTextAttribute).Text}({methodInfos[i].Name})");
				}
				else
				{
					MethodNames.Add(methodInfos[i].Name, methodInfos[i].Name);
				}
#endif
			}
		}

		#region 整场属性
		[LabelText("是否新手引导场次")]
		public static bool IsGuidance(AIKnowledge aiKnowledge)
		{
			return IAuctionManager.Instance is AuctionGuideManager;
		}
		[LabelText("是否命运骰子抬价意愿提高")]
		public static bool IsDiceType3(AIKnowledge aiKnowledge)
		{
			return IAuctionManager.Instance.DiceConfig.Type == 3;
		}
		[LabelText("是否命运骰子恶意竞争")]
		public static bool IsDiceType5(AIKnowledge aiKnowledge)
		{
			return IAuctionManager.Instance.DiceConfig.Type == 5;
		}
		
		[LabelText("场上是否有人退场")]
		public static bool IsAnyOneLeave(AIKnowledge aiKnowledge)
		{
			return IAuctionManager.Instance.GetLevelCount() > 0;
		}

		[LabelText("场上是否除玩家外只有1人")]
		public static bool IsOnlyOne(AIKnowledge aiKnowledge)
		{
			return IAuctionManager.Instance.Bidders.Count == 1;
		}
		
		[LabelText("场上是否处于事件标记1")]
		public static bool IsEventTarget1(AIKnowledge aiKnowledge)
		{
			return IAuctionManager.Instance.StartEventConfig?.AIFlag == 1;
		}
		
		[LabelText("场上是否处于事件标记2")]
		public static bool IsEventTarget2(AIKnowledge aiKnowledge)
		{
			return IAuctionManager.Instance.StartEventConfig?.AIFlag == 2;
		}
		
		[LabelText("场上是否处于事件标记3")]
		public static bool IsEventTarget3(AIKnowledge aiKnowledge)
		{
			return IAuctionManager.Instance.StartEventConfig?.AIFlag == 3;
		}
		
		[LabelText("场上是否处于事件标记4")]
		public static bool IsEventTarget4(AIKnowledge aiKnowledge)
		{
			return IAuctionManager.Instance.StartEventConfig?.AIFlag == 4;
		}
		#endregion
		
		#region 剩余资金判断
		[LabelText("剩余资金是否大于等于估价")]
		public static bool IsMoneyEnoughJudge(AIKnowledge aiKnowledge)
		{
			return aiKnowledge.Money >= aiKnowledge.Judge;
		}
		[LabelText("剩余资金是否足够低叫价")]
		public static bool IsMoneyEnoughLow(AIKnowledge aiKnowledge)
		{
			return aiKnowledge.Money >= IAuctionManager.Instance.LowAuction;
		}
		
		[LabelText("剩余资金是否足够中叫价")]
		public static bool IsMoneyEnoughMedium(AIKnowledge aiKnowledge)
		{
			return aiKnowledge.Money >= IAuctionManager.Instance.MediumAuction;
		}
		
		[LabelText("剩余资金是否足够高叫价")]
		public static bool IsMoneyEnoughHigh(AIKnowledge aiKnowledge)
		{
			return aiKnowledge.Money >= IAuctionManager.Instance.HighAuction;
		}
		
		[LabelText("剩余资金是否能梭哈")]
		public static bool IsMoneyEnoughAllin(AIKnowledge aiKnowledge)
		{
			return aiKnowledge.Money > IAuctionManager.Instance.HighAuction;
		}
		#endregion

		#region 自身属性

		[LabelText("自己是否复仇者")]
		public static bool IsRevenge(AIKnowledge aiKnowledge)
		{
			return aiKnowledge.RevengeTarget != -1 && aiKnowledge.RevengeCount > 0;
		}
		
		[LabelText("自己是否被复仇目标")][Tooltip("只能包括AI的")]
		public static bool IsRevengeAim(AIKnowledge aiKnowledge)
		{
			for (int i = 0; i < IAuctionManager.Instance.Bidders.Count; i++)
			{
				var bidder = EntityManager.Instance.Get(IAuctionManager.Instance.Bidders[i]);
				if (bidder == null) continue;
				var ai = bidder.GetComponent<AIComponent>();
				if (ai.GetKnowledge().RevengeTarget == aiKnowledge.Entity.Id)
				{
					return true;
				}
			}
			return false;
		}

		[LabelText("自己是否触发诱导抬价")]
		public static bool IsRaise(AIKnowledge aiKnowledge)
		{
			return aiKnowledge.IsRaisePrice && aiKnowledge.RaisePriceCount > 0;
		}
		[LabelText("自己是否生气")][Tooltip("如果场上的价格已经超过了玩家预判的最大值，且上一次是玩家叫价，那么npc有概率生气")]
		public static bool IsAnger(AIKnowledge aiKnowledge)
		{
			return aiKnowledge.IsAnger;
		}

		[LabelText("自己是否进入消极")]
		public static bool IsEnterNegative(AIKnowledge aiKnowledge)
		{
			return aiKnowledge.IsNegative;
		}
		
		[LabelText("自己本关剩余叫价次数是否为0")][Tooltip("目前仅进入消极状态后减少本关卡喊价次数")]
		public static bool IsBidCountEmpty(AIKnowledge aiKnowledge)
		{
			return aiKnowledge.BidCount <= 0;
		}
		
		[LabelText("自己是否志在必得")][Tooltip("特殊盲盒时极小几率出现")]
		public static bool IsDeterminedToHave(AIKnowledge aiKnowledge)
		{
			return aiKnowledge.DeterminedToHave;
		}
		
		[LabelText("自己是否被高价震慑")]
		public static bool IsHighPriceDeterrence(AIKnowledge aiKnowledge)
		{
			return aiKnowledge.IsHighPriceDeterrence;
		}

		[LabelText("自己是否已经触发过跟风")]
		public static bool IsFollow(AIKnowledge aiKnowledge)
		{
			return aiKnowledge.IsFollow;
		}
		[LabelText("自己是否黑衣人")]
		public static bool IsBlack(AIKnowledge aiKnowledge)
		{
			return aiKnowledge.Entity.GetComponent<BlackBoyComponent>() != null;
		}
		#endregion
		
		#region 当前轮数据
		
		[LabelText("当前轮是第1轮")]
		public static bool IsStage1(AIKnowledge aiKnowledge)
		{
			return IAuctionManager.Instance.Stage == 1;
		}
		[LabelText("当前轮是第2轮")]
		public static bool IsStage2(AIKnowledge aiKnowledge)
		{
			return IAuctionManager.Instance.Stage == 2;
		}
		[LabelText("当前轮是第3轮")]
		public static bool IsStage3(AIKnowledge aiKnowledge)
		{
			return IAuctionManager.Instance.Stage == 3;
		}
		[LabelText("当前轮是第4轮")]
		public static bool IsStage4(AIKnowledge aiKnowledge)
		{
			return IAuctionManager.Instance.Stage == 4;
		}
		[LabelText("当前轮是第5轮")]
		public static bool IsStage5(AIKnowledge aiKnowledge)
		{
			return IAuctionManager.Instance.Stage == 5;
		}
		[LabelText("当前轮是否存在复仇者")]
		public static bool IsAnyAIRevenge(AIKnowledge aiKnowledge)
		{
			for (int i = 0; i < IAuctionManager.Instance.Bidders.Count; i++)
			{
				var bidder = EntityManager.Instance.Get(IAuctionManager.Instance.Bidders[i]);
				if (bidder == null) continue;
				var ai = bidder.GetComponent<AIComponent>();
				if (ai.GetKnowledge().RevengeTarget != -1 && ai.GetKnowledge().RevengeCount>0)
				{
					return true;
				}
			}

			return false;
		}
		
		[LabelText("当前轮是否有小玩法")]
		public static bool HasMiniPlayItem(AIKnowledge aiKnowledge)
		{
			return IAuctionManager.Instance.HasMiniPlayItem;
		}
		
		[LabelText("当前轮是否有任务物品")]
		public static bool HasTaskItem(AIKnowledge aiKnowledge)
		{
			return IAuctionManager.Instance.HasTaskItem;
		}
		
		[LabelText("当前轮是否特殊集装箱场次")]
		public static bool IsSpecial(AIKnowledge aiKnowledge)
		{
			if (IAuctionManager.Instance.Report == null) return false;
			var container = ContainerConfigCategory.Instance.Get(IAuctionManager.Instance.Report.ContainerId);
			return container.Type != 1;
		}
		
		[LabelText("当前轮是否全玩法集装箱场次")]
		public static bool IsAllSpecial(AIKnowledge aiKnowledge)
		{
			if (IAuctionManager.Instance.Report == null) return false;
			var container = ContainerConfigCategory.Instance.Get(IAuctionManager.Instance.Report.ContainerId);
			return container.Type == 0;
		}
		[LabelText("当前轮玩家是否至少抬价成功1次")]
		public static bool IsPlayerRaiseSuccess(AIKnowledge aiKnowledge)
		{
			return IAuctionManager.Instance.Report.RaiseSuccessCount > 0;
		}
		
		[LabelText("当前轮上次出价是否大于AI预制价格锚点1")]
		public static bool IsGreaterThanAnchor1(AIKnowledge aiKnowledge)
		{
			return IAuctionManager.Instance.LastAuctionPrice > IAuctionManager.Instance.Config.AIPriceAnchor1 *
				IAuctionManager.Instance.AllPrice / 100;
		}

		
		[LabelText("当前轮上次出价是否大于AI预制价格锚点2")]
		public static bool IsGreaterThanAnchor2(AIKnowledge aiKnowledge)
		{
			return IAuctionManager.Instance.LastAuctionPrice > IAuctionManager.Instance.Config.AIPriceAnchor2 *
				IAuctionManager.Instance.AllPrice / 100;
		}
		
		[LabelText("当前轮上次出价是否大于AI预制价格锚点3")]
		public static bool IsGreaterThanAnchor3(AIKnowledge aiKnowledge)
		{
			return IAuctionManager.Instance.LastAuctionPrice > IAuctionManager.Instance.Config.AIPriceAnchor3 *
				IAuctionManager.Instance.AllPrice / 100;
		}

		
		[LabelText("当前轮上次出价是否大于AI预制价格锚点4")]
		public static bool IsGreaterThanAnchor4(AIKnowledge aiKnowledge)
		{
			return IAuctionManager.Instance.LastAuctionPrice > IAuctionManager.Instance.Config.AIPriceAnchor4 *
				IAuctionManager.Instance.AllPrice / 100;
		}

		[LabelText("当前轮是否有人出价过")]
		public static bool AnyOneAuction(AIKnowledge aiKnowledge)
		{
			return IAuctionManager.Instance.LastAuctionPlayerId > 0;
		}
		
		[LabelText("当前轮玩家成功抬价成功次数是否小于等于抬价成功次数锚点")]
		public static bool IsLessEqualPlayerRaiseSuccessCount(AIKnowledge aiKnowledge)
		{
			return IAuctionManager.Instance.RaiseSuccessCount <= IAuctionManager.Instance.Config.PlayerRaiseSuccB +
				IAuctionManager.Instance.Config.PlayerRaiseSuccK * IAuctionManager.Instance.RaiseSuccessCount;
		}
		
		[LabelText("当前轮玩家成功抬价次数是否小于等于抬价总次数锚点")]
		public static bool IsLessEqualRaiseCount(AIKnowledge aiKnowledge)
		{
			return IAuctionManager.Instance.RaiseSuccessCount <= IAuctionManager.Instance.Config.RaiseCountB +
				IAuctionManager.Instance.Config.RaiseCountK * IAuctionManager.Instance.RaiseCount;
		}
		
		[LabelText("当前轮是否存在黑衣人")]
		public static bool HasBlack(AIKnowledge aiKnowledge)
		{
			return IAuctionManager.Instance.Blacks != null && IAuctionManager.Instance.Blacks.Count > 0;
		}
		#endregion
		
		#region 上一轮数据

		[LabelText("上一轮叫价是否大于玩家预判最大值")]
		public static bool IsMoreThanUserMaxJudgePrice(AIKnowledge aiKnowledge)
		{
			return IAuctionManager.Instance.LastAuctionPrice >
			       IAuctionManager.Instance.AllPrice * IAuctionManager.Instance.SysJudgePriceMax;
		}
		[LabelText("上一轮叫价是否大于AI心理价位")]
		public static bool IsMoreThanMaxJudgePrice(AIKnowledge aiKnowledge)
		{
			return IAuctionManager.Instance.LastAuctionPrice > aiKnowledge.Judge;
		}

		[LabelText("上一轮叫价是否犹豫")][Tooltip("犹豫即拍卖师开始倒计时后才叫价")]
		public static bool IsLastHesitate(AIKnowledge aiKnowledge)
		{
			return IAuctionManager.Instance.LastHostSayCount > 0;
		}
		
		[LabelText("上一轮叫价是否小于1秒")]
		public static bool IsLastFast(AIKnowledge aiKnowledge)
		{
			return IAuctionManager.Instance.LastAuctionTime < 1000;
		}
		
		[LabelText("上一轮出价者是否复仇目标")]
		public static bool IsLastRevengeTarget(AIKnowledge aiKnowledge)
		{
			return aiKnowledge.RevengeTarget == IAuctionManager.Instance.LastAuctionPlayerId && aiKnowledge.RevengeCount > 0;
		}
		
		[LabelText("上一轮出价者复仇目标是否是自己")]
		public static bool IsLastRevengeTargetIsMe(AIKnowledge aiKnowledge)
		{
			if (IAuctionManager.Instance.LastAuctionPlayerId == IAuctionManager.Instance.Player.Id) return false;
			var bidder = EntityManager.Instance.Get(IAuctionManager.Instance.LastAuctionPlayerId);
			if (bidder == null) return false;
			var ai = bidder.GetComponent<AIComponent>().GetKnowledge();
			return ai.RevengeTarget == aiKnowledge.Entity.Id;
		}
		
		[LabelText("上一轮出价者是否是玩家")]
		public static bool IsLastPlayer(AIKnowledge aiKnowledge)
		{
			return IAuctionManager.Instance.LastAuctionPlayerId == IAuctionManager.Instance.Player.Id;
		}
		#endregion

		#region 新手引导
		
		[LabelText("新手引导当前叫价是否大于等于AIMinPrice")]
		public static bool IsMoreThanAIMinPrice(AIKnowledge aiKnowledge)
		{
			var config = GuidanceStageConfigCategory.Instance.Get(IAuctionManager.Instance.Stage);
			return IAuctionManager.Instance.LastAuctionPrice >= (config?.AIMinPrice ?? 0);
		}
		[LabelText("新手引导出低价是否小于等于AIMaxPrice")]
		public static bool IsLessThanAIMaxPrice1(AIKnowledge aiKnowledge)
		{
			var config = GuidanceStageConfigCategory.Instance.Get(IAuctionManager.Instance.Stage);
			return IAuctionManager.Instance.LowAuction <= (config?.AIMaxPrice ?? 0);
		}
		[LabelText("新手引导出中价是否小于等于AIMaxPrice")]
		public static bool IsLessThanAIMaxPrice2(AIKnowledge aiKnowledge)
		{
			var config = GuidanceStageConfigCategory.Instance.Get(IAuctionManager.Instance.Stage);
			return IAuctionManager.Instance.MediumAuction <= (config?.AIMaxPrice ?? 0);
		}
		[LabelText("新手引导出高价是否小于等于AIMaxPrice")]
		public static bool IsLessThanAIMaxPrice3(AIKnowledge aiKnowledge)
		{
			var config = GuidanceStageConfigCategory.Instance.Get(IAuctionManager.Instance.Stage);
			return IAuctionManager.Instance.HighAuction <= (config?.AIMaxPrice ?? 0);
		}
		[LabelText("新手引导玩家抬价成功次数是否等于PlayerMaxRaiseCount")]
		public static bool IsPlayerMaxRaiseCount(AIKnowledge aiKnowledge)
		{
			var config = GuidanceStageConfigCategory.Instance.Get(IAuctionManager.Instance.Stage);
			return IAuctionManager.Instance.Report.RaiseSuccessCount >= (config?.PlayerMaxRaiseCount??0);
		}
		#endregion
	}
}