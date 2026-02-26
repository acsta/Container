using System;
using System.Collections.Generic;
using Nino.Serialization;

namespace TaoTie
{
    [NinoSerialize]
    [Config]
    public partial class AIConfigCategory : ProtoObject, IMerge
    {
        public static AIConfigCategory Instance;
		
        
        [NinoIgnore]
        private Dictionary<int, AIConfig> dict = new Dictionary<int, AIConfig>();
        
        [NinoMember(1)]
        private List<AIConfig> list = new List<AIConfig>();
		
        public AIConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            AIConfigCategory s = o as AIConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            for(int i =0 ;i<list.Count;i++)
            {
                AIConfig config = list[i];
                config.EndInit();
                this.dict.Add(config.Id, config);
                config.AfterEndInit();
            }            
            this.AfterEndInit();
        }
		
        public AIConfig Get(int id)
        {
            this.dict.TryGetValue(id, out AIConfig item);

            if (item == null)
            {
#if !NOT_UNITY
                Log.Error($"配置找不到，配置表名: {nameof (AIConfig)}，配置id: {id}");
#endif
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, AIConfig> GetAll()
        {
            return this.dict;
        }
        public List<AIConfig> GetAllList()
        {
            return this.list;
        }
        public AIConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [NinoSerialize]
	public partial class AIConfig: ProtoObject
	{
		/// <summary>Id</summary>
		[NinoMember(1)]
		public int Id { get; set; }
		/// <summary>模型Id</summary>
		[NinoMember(2)]
		public int UnitId { get; set; }
		/// <summary>行为树类型</summary>
		[NinoMember(3)]
		public string AIType { get; set; }
		/// <summary>拍价后休闲动作</summary>
		[NinoMember(4)]
		public int[] ActionInterval { get; set; }
		/// <summary>初始资金</summary>
		[NinoMember(5)]
		public int InitMoney { get; set; }
		/// <summary>判断误差</summary>
		[NinoMember(6)]
		public float[] Deviation { get; set; }
		/// <summary>出价延迟时间</summary>
		[NinoMember(7)]
		public int[] AuctionTime { get; set; }
		/// <summary>不出价权重</summary>
		[NinoMember(8)]
		public int SidelinesWeight { get; set; }
		/// <summary>喊低价权重</summary>
		[NinoMember(9)]
		public int LowWeight { get; set; }
		/// <summary>喊中价权重</summary>
		[NinoMember(10)]
		public int MediumWeight { get; set; }
		/// <summary>喊高价权重</summary>
		[NinoMember(11)]
		public int HighWeight { get; set; }
		/// <summary>抬价激活次数</summary>
		[NinoMember(12)]
		public int ForceUp { get; set; }
		/// <summary>诱导次数</summary>
		[NinoMember(13)]
		public int Induce { get; set; }
		/// <summary>跟风敏感度次数</summary>
		[NinoMember(14)]
		public int Follow { get; set; }
		/// <summary>跟风判断价值误差</summary>
		[NinoMember(15)]
		public float[] FollowExpand { get; set; }
		/// <summary>复仇次数</summary>
		[NinoMember(16)]
		public int Revenge { get; set; }
		/// <summary>高价震慑</summary>
		[NinoMember(17)]
		public float Shock { get; set; }
		/// <summary>抬价阶段高价震慑</summary>
		[NinoMember(18)]
		public float ShockRaise { get; set; }
		/// <summary>消极度</summary>
		[NinoMember(19)]
		public float Negative { get; set; }
		/// <summary>触发消极度后本关叫价总次数与权重</summary>
		[NinoMember(20)]
		public int[][] NegativeBehavior { get; set; }
		/// <summary>玩家与其连续抬价多少次触发生气</summary>
		[NinoMember(21)]
		public int AngerTimes { get; set; }
		/// <summary>生气概率</summary>
		[NinoMember(22)]
		public float AngerProp { get; set; }

	}
}
