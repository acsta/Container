using System;
using System.Collections.Generic;
using Nino.Serialization;

namespace TaoTie
{
    [NinoSerialize]
    [Config]
    public partial class SaleEventConfigCategory : ProtoObject, IMerge
    {
        public static SaleEventConfigCategory Instance;
		
        
        [NinoIgnore]
        private Dictionary<int, SaleEventConfig> dict = new Dictionary<int, SaleEventConfig>();
        
        [NinoMember(1)]
        private List<SaleEventConfig> list = new List<SaleEventConfig>();
		
        public SaleEventConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            SaleEventConfigCategory s = o as SaleEventConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            for(int i =0 ;i<list.Count;i++)
            {
                SaleEventConfig config = list[i];
                config.EndInit();
                this.dict.Add(config.Id, config);
                config.AfterEndInit();
            }            
            this.AfterEndInit();
        }
		
        public SaleEventConfig Get(int id)
        {
            this.dict.TryGetValue(id, out SaleEventConfig item);

            if (item == null)
            {
#if !NOT_UNITY
                Log.Error($"配置找不到，配置表名: {nameof (SaleEventConfig)}，配置id: {id}");
#endif
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, SaleEventConfig> GetAll()
        {
            return this.dict;
        }
        public List<SaleEventConfig> GetAllList()
        {
            return this.list;
        }
        public SaleEventConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [NinoSerialize]
	public partial class SaleEventConfig: ProtoObject
	{
		/// <summary>Id</summary>
		[NinoMember(1)]
		public int Id { get; set; }
		/// <summary>类型(0买1卖)</summary>
		[NinoMember(2)]
		public int Type { get; set; }
		/// <summary>出价范围最大比例倍数</summary>
		[NinoMember(3)]
		public float SliderMaxValue { get; set; }
		/// <summary>根据基础金额得到NPC期望金额</summary>
		[NinoMember(4)]
		public float NPCTargetValue { get; set; }
		/// <summary>NPC期望金额范围的最小比值</summary>
		[NinoMember(5)]
		public float NPCTargetRangeMinValue { get; set; }
		/// <summary>NPC期望金额范围的最大比值</summary>
		[NinoMember(6)]
		public float NPCTargetRangeMaxValue { get; set; }
		/// <summary>抢劫NPC报价最小比值</summary>
		[NinoMember(7)]
		public float NPCNewPriceRangeMinValue { get; set; }
		/// <summary>抢劫NPC报价最大比值</summary>
		[NinoMember(8)]
		public float NPCNewPriceRangeMaxValue { get; set; }
		/// <summary>出现事件的概率(阈值)</summary>
		[NinoMember(9)]
		public float OnThreshold { get; set; }
		/// <summary>反抗成功概率</summary>
		[NinoMember(10)]
		public float BattleThreshold { get; set; }
		/// <summary>一口价概率</summary>
		[NinoMember(11)]
		public float FixedPriceThreshold { get; set; }
		/// <summary>普通赚钱</summary>
		[NinoMember(12)]
		public float NormalProfit { get; set; }
		/// <summary>广告赚钱</summary>
		[NinoMember(13)]
		public float ADProfit { get; set; }
		/// <summary>亏钱</summary>
		[NinoMember(14)]
		public float LoseMoney { get; set; }
		/// <summary>名字</summary>
		[NinoMember(15)]
		public string Chinese { get; set; }
		/// <summary>名字</summary>
		[NinoMember(16)]
		public string English { get; set; }
		/// <summary>图片</summary>
		[NinoMember(17)]
		public string Icon { get; set; }
		/// <summary>第一轮NPC文本</summary>
		[NinoMember(18)]
		public string FirstStepNPCText { get; set; }
		/// <summary>第一轮用户文本</summary>
		[NinoMember(19)]
		public string FirstStepUserText { get; set; }
		/// <summary>超出NPC期望</summary>
		[NinoMember(20)]
		public string NPCGood { get; set; }
		/// <summary>与NPC期望相差无几</summary>
		[NinoMember(21)]
		public string NPCSoso { get; set; }
		/// <summary>与NPC期望过低</summary>
		[NinoMember(22)]
		public string NPCBad { get; set; }
		/// <summary>一口价</summary>
		[NinoMember(23)]
		public string NPCFixedPrice { get; set; }
		/// <summary>交易成功</summary>
		[NinoMember(24)]
		public string DealSuccess { get; set; }
		/// <summary>交易失败</summary>
		[NinoMember(25)]
		public string DealFail { get; set; }
		/// <summary>战斗成功</summary>
		[NinoMember(26)]
		public string BattleSuccess { get; set; }
		/// <summary>战斗失败</summary>
		[NinoMember(27)]
		public string BattleFail { get; set; }
		/// <summary>认栽按钮</summary>
		[NinoMember(28)]
		public string FailButton { get; set; }
		/// <summary>广告按钮</summary>
		[NinoMember(29)]
		public string ADButton { get; set; }
		/// <summary>一口价同意按钮</summary>
		[NinoMember(30)]
		public string FixedPriceAgree { get; set; }
		/// <summary>一口价拒绝按钮</summary>
		[NinoMember(31)]
		public string FixedPriceRefuse { get; set; }
		/// <summary>结算战斗成功</summary>
		[NinoMember(32)]
		public string ResultBattleSuccess { get; set; }
		/// <summary>结算战斗失败</summary>
		[NinoMember(33)]
		public string ResultBattleFail { get; set; }
		/// <summary>战斗失败主城</summary>
		[NinoMember(34)]
		public string BackBattleFail { get; set; }
		/// <summary>结算交易成功</summary>
		[NinoMember(35)]
		public string ResultSaleSuccess { get; set; }
		/// <summary>结算交易失败</summary>
		[NinoMember(36)]
		public string ResultSaleFail { get; set; }
		/// <summary>战斗失败转场途中文本</summary>
		[NinoMember(37)]
		public string TurnBattleFail { get; set; }

	}
}
