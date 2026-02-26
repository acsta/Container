using System;
using System.Collections.Generic;
using Nino.Serialization;

namespace TaoTie
{
    [NinoSerialize]
    [Config]
    public partial class SubIdentificationConfigCategory : ProtoObject, IMerge
    {
        public static SubIdentificationConfigCategory Instance;
		
        
        [NinoIgnore]
        private Dictionary<int, SubIdentificationConfig> dict = new Dictionary<int, SubIdentificationConfig>();
        
        [NinoMember(1)]
        private List<SubIdentificationConfig> list = new List<SubIdentificationConfig>();
		
        public SubIdentificationConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            SubIdentificationConfigCategory s = o as SubIdentificationConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            for(int i =0 ;i<list.Count;i++)
            {
                SubIdentificationConfig config = list[i];
                config.EndInit();
                this.dict.Add(config.Id, config);
                config.AfterEndInit();
            }            
            this.AfterEndInit();
        }
		
        public SubIdentificationConfig Get(int id)
        {
            this.dict.TryGetValue(id, out SubIdentificationConfig item);

            if (item == null)
            {
#if !NOT_UNITY
                Log.Error($"配置找不到，配置表名: {nameof (SubIdentificationConfig)}，配置id: {id}");
#endif
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, SubIdentificationConfig> GetAll()
        {
            return this.dict;
        }
        public List<SubIdentificationConfig> GetAllList()
        {
            return this.list;
        }
        public SubIdentificationConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [NinoSerialize]
	public partial class SubIdentificationConfig: ProtoObject
	{
		/// <summary>玩法藏品ID</summary>
		[NinoMember(1)]
		public int Id { get; set; }
		/// <summary>结果1底图</summary>
		[NinoMember(2)]
		public int Rare1 { get; set; }
		/// <summary>结果1物品ID</summary>
		[NinoMember(3)]
		public int Result1 { get; set; }
		/// <summary>结果1权重</summary>
		[NinoMember(4)]
		public int Weight1 { get; set; }
		/// <summary>结果1权重</summary>
		[NinoMember(5)]
		public int AIWeight1 { get; set; }
		/// <summary>结果2底图</summary>
		[NinoMember(6)]
		public int Rare2 { get; set; }
		/// <summary>结果2物品ID</summary>
		[NinoMember(7)]
		public int Result2 { get; set; }
		/// <summary>结果2权重</summary>
		[NinoMember(8)]
		public int Weight2 { get; set; }
		/// <summary>结果2权重</summary>
		[NinoMember(9)]
		public int AIWeight2 { get; set; }
		/// <summary>结果3底图</summary>
		[NinoMember(10)]
		public int Rare3 { get; set; }
		/// <summary>结果3物品ID</summary>
		[NinoMember(11)]
		public int Result3 { get; set; }
		/// <summary>结果3权重</summary>
		[NinoMember(12)]
		public int Weight3 { get; set; }
		/// <summary>结果3权重</summary>
		[NinoMember(13)]
		public int AIWeight3 { get; set; }
		/// <summary>结果4底图</summary>
		[NinoMember(14)]
		public int Rare4 { get; set; }
		/// <summary>结果4物品ID</summary>
		[NinoMember(15)]
		public int Result4 { get; set; }
		/// <summary>结果4权重</summary>
		[NinoMember(16)]
		public int Weight4 { get; set; }
		/// <summary>结果4权重</summary>
		[NinoMember(17)]
		public int AIWeight4 { get; set; }
		/// <summary>结果5底图</summary>
		[NinoMember(18)]
		public int Rare5 { get; set; }
		/// <summary>结果5物品ID</summary>
		[NinoMember(19)]
		public int Result5 { get; set; }
		/// <summary>结果5权重</summary>
		[NinoMember(20)]
		public int Weight5 { get; set; }
		/// <summary>结果5权重</summary>
		[NinoMember(21)]
		public int AIWeight5 { get; set; }
		/// <summary>结果6底图</summary>
		[NinoMember(22)]
		public int Rare6 { get; set; }
		/// <summary>结果6物品ID</summary>
		[NinoMember(23)]
		public int Result6 { get; set; }
		/// <summary>结果6权重</summary>
		[NinoMember(24)]
		public int Weight6 { get; set; }
		/// <summary>结果6权重</summary>
		[NinoMember(25)]
		public int AIWeight6 { get; set; }
		/// <summary>结果7底图</summary>
		[NinoMember(26)]
		public int Rare7 { get; set; }
		/// <summary>结果7物品ID</summary>
		[NinoMember(27)]
		public int Result7 { get; set; }
		/// <summary>结果7权重</summary>
		[NinoMember(28)]
		public int Weight7 { get; set; }
		/// <summary>结果7权重</summary>
		[NinoMember(29)]
		public int AIWeight7 { get; set; }
		/// <summary>结果8底图</summary>
		[NinoMember(30)]
		public int Rare8 { get; set; }
		/// <summary>结果8物品ID</summary>
		[NinoMember(31)]
		public int Result8 { get; set; }
		/// <summary>结果8权重</summary>
		[NinoMember(32)]
		public int Weight8 { get; set; }
		/// <summary>结果8权重</summary>
		[NinoMember(33)]
		public int AIWeight8 { get; set; }
		/// <summary>结果9底图</summary>
		[NinoMember(34)]
		public int Rare9 { get; set; }
		/// <summary>结果9物品ID</summary>
		[NinoMember(35)]
		public int Result9 { get; set; }
		/// <summary>结果9权重</summary>
		[NinoMember(36)]
		public int Weight9 { get; set; }
		/// <summary>结果9权重</summary>
		[NinoMember(37)]
		public int AIWeight9 { get; set; }
		/// <summary>结果10底图</summary>
		[NinoMember(38)]
		public int Rare10 { get; set; }
		/// <summary>结果10物品ID</summary>
		[NinoMember(39)]
		public int Result10 { get; set; }
		/// <summary>结果10权重</summary>
		[NinoMember(40)]
		public int Weight10 { get; set; }
		/// <summary>结果10权重</summary>
		[NinoMember(41)]
		public int AIWeight10 { get; set; }

	}
}
