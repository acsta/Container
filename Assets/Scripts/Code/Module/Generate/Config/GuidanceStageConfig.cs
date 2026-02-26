using System;
using System.Collections.Generic;
using Nino.Serialization;

namespace TaoTie
{
    [NinoSerialize]
    [Config]
    public partial class GuidanceStageConfigCategory : ProtoObject, IMerge
    {
        public static GuidanceStageConfigCategory Instance;
		
        
        [NinoIgnore]
        private Dictionary<int, GuidanceStageConfig> dict = new Dictionary<int, GuidanceStageConfig>();
        
        [NinoMember(1)]
        private List<GuidanceStageConfig> list = new List<GuidanceStageConfig>();
		
        public GuidanceStageConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            GuidanceStageConfigCategory s = o as GuidanceStageConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            for(int i =0 ;i<list.Count;i++)
            {
                GuidanceStageConfig config = list[i];
                config.EndInit();
                this.dict.Add(config.Id, config);
                config.AfterEndInit();
            }            
            this.AfterEndInit();
        }
		
        public GuidanceStageConfig Get(int id)
        {
            this.dict.TryGetValue(id, out GuidanceStageConfig item);

            if (item == null)
            {
#if !NOT_UNITY
                Log.Error($"配置找不到，配置表名: {nameof (GuidanceStageConfig)}，配置id: {id}");
#endif
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, GuidanceStageConfig> GetAll()
        {
            return this.dict;
        }
        public List<GuidanceStageConfig> GetAllList()
        {
            return this.list;
        }
        public GuidanceStageConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [NinoSerialize]
	public partial class GuidanceStageConfig: ProtoObject
	{
		/// <summary>场次</summary>
		[NinoMember(1)]
		public int Id { get; set; }
		/// <summary>箱子Id</summary>
		[NinoMember(2)]
		public int ContainerId { get; set; }
		/// <summary>物品Id</summary>
		[NinoMember(3)]
		public int[] Items { get; set; }
		/// <summary>开放按钮类型</summary>
		[NinoMember(4)]
		public int[] Button { get; set; }
		/// <summary>当前价格超过该值时AI不出价</summary>
		[NinoMember(5)]
		public int AIMinPrice { get; set; }
		/// <summary>Npc最多叫到什么价格</summary>
		[NinoMember(6)]
		public int AIMaxPrice { get; set; }
		/// <summary>主持人倒计时类型</summary>
		[NinoMember(7)]
		public int HosterType { get; set; }
		/// <summary>最多允许玩家抬价成功次数</summary>
		[NinoMember(8)]
		public int PlayerMaxRaiseCount { get; set; }
		/// <summary>是否超时才引导点击箱子</summary>
		[NinoMember(9)]
		public int OpenGuideBox { get; set; }
		/// <summary>AI出价多少次后玩家才能出价</summary>
		[NinoMember(10)]
		public int BeforePlayerAuction { get; set; }

	}
}
