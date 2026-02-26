using System;
using System.Collections.Generic;
using Nino.Serialization;

namespace TaoTie
{
    [NinoSerialize]
    [Config]
    public partial class StageConfigCategory : ProtoObject, IMerge
    {
        public static StageConfigCategory Instance;
		
        
        [NinoIgnore]
        private Dictionary<int, StageConfig> dict = new Dictionary<int, StageConfig>();
        
        [NinoMember(1)]
        private List<StageConfig> list = new List<StageConfig>();
		
        public StageConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            StageConfigCategory s = o as StageConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            for(int i =0 ;i<list.Count;i++)
            {
                StageConfig config = list[i];
                config.EndInit();
                this.dict.Add(config.Id, config);
                config.AfterEndInit();
            }            
            this.AfterEndInit();
        }
		
        public StageConfig Get(int id)
        {
            this.dict.TryGetValue(id, out StageConfig item);

            if (item == null)
            {
#if !NOT_UNITY
                Log.Error($"配置找不到，配置表名: {nameof (StageConfig)}，配置id: {id}");
#endif
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, StageConfig> GetAll()
        {
            return this.dict;
        }
        public List<StageConfig> GetAllList()
        {
            return this.list;
        }
        public StageConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [NinoSerialize]
	public partial class StageConfig: ProtoObject
	{
		/// <summary>id</summary>
		[NinoMember(1)]
		public int Id { get; set; }
		/// <summary>场次级别</summary>
		[NinoMember(2)]
		public int Level { get; set; }
		/// <summary>关卡</summary>
		[NinoMember(3)]
		public int Stage { get; set; }
		/// <summary>物品的最小价值与最大价值</summary>
		[NinoMember(4)]
		public long[] Value { get; set; }
		/// <summary>特殊玩法物品的最小价值与最大价值</summary>
		[NinoMember(5)]
		public long[] SpValue { get; set; }
		/// <summary>普通物品的最小价值与最大价值</summary>
		[NinoMember(6)]
		public long[] NormalValue { get; set; }
		/// <summary>玩家出价低</summary>
		[NinoMember(7)]
		public long Auction1 { get; set; }
		/// <summary>玩家出价中</summary>
		[NinoMember(8)]
		public long Auction2 { get; set; }
		/// <summary>玩家出价高</summary>
		[NinoMember(9)]
		public long Auction3 { get; set; }
		/// <summary>抬价阶段每次出价后，基础出价增加值</summary>
		[NinoMember(10)]
		public long RaiseAuctionAddon { get; set; }
		/// <summary>起拍价</summary>
		[NinoMember(11)]
		public long BaseAuction { get; set; }
		/// <summary>有百分之多少概率调用AI误差</summary>
		[NinoMember(12)]
		public int AiDeviation { get; set; }
		/// <summary>AI价格锚点（百分比）</summary>
		[NinoMember(13)]
		public int AIPriceAnchor1 { get; set; }
		/// <summary>AI价格锚点（百分比）</summary>
		[NinoMember(14)]
		public int AIPriceAnchor2 { get; set; }
		/// <summary>AI价格锚点（百分比）</summary>
		[NinoMember(15)]
		public int AIPriceAnchor3 { get; set; }
		/// <summary>AI价格锚点（百分比）</summary>
		[NinoMember(16)]
		public int AIPriceAnchor4 { get; set; }
		/// <summary>预制误差与权重</summary>
		[NinoMember(17)]
		public float[][] DeviationPrefab { get; set; }
		/// <summary>真实价值首次判断最小值区间与权重</summary>
		[NinoMember(18)]
		public float[][] FirstJudgeMin { get; set; }
		/// <summary>真实价值首次判断最大值区间</summary>
		[NinoMember(19)]
		public float[][] FirstJudgeMax { get; set; }
		/// <summary>玩家抬价成功次数锚点系数-k(值=k成功次数+b)</summary>
		[NinoMember(20)]
		public float PlayerRaiseSuccK { get; set; }
		/// <summary>玩家抬价成功次数锚点系数-b(值=k成功次数+b)</summary>
		[NinoMember(21)]
		public int PlayerRaiseSuccB { get; set; }
		/// <summary>抬价总次数锚点系数-k(值=k抬价次数+b)</summary>
		[NinoMember(22)]
		public float RaiseCountK { get; set; }
		/// <summary>抬价总次数锚点系数-b(值=k抬价次数+b)</summary>
		[NinoMember(23)]
		public int RaiseCountB { get; set; }

	}
}
