using System;
using System.Collections.Generic;
using Nino.Serialization;

namespace TaoTie
{
    [NinoSerialize]
    [Config]
    public partial class EquipGroupConfigCategory : ProtoObject, IMerge
    {
        public static EquipGroupConfigCategory Instance;
		
        
        [NinoIgnore]
        private Dictionary<int, EquipGroupConfig> dict = new Dictionary<int, EquipGroupConfig>();
        
        [NinoMember(1)]
        private List<EquipGroupConfig> list = new List<EquipGroupConfig>();
		
        public EquipGroupConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            EquipGroupConfigCategory s = o as EquipGroupConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            for(int i =0 ;i<list.Count;i++)
            {
                EquipGroupConfig config = list[i];
                config.EndInit();
                this.dict.Add(config.Id, config);
                config.AfterEndInit();
            }            
            this.AfterEndInit();
        }
		
        public EquipGroupConfig Get(int id)
        {
            this.dict.TryGetValue(id, out EquipGroupConfig item);

            if (item == null)
            {
#if !NOT_UNITY
                Log.Error($"配置找不到，配置表名: {nameof (EquipGroupConfig)}，配置id: {id}");
#endif
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, EquipGroupConfig> GetAll()
        {
            return this.dict;
        }
        public List<EquipGroupConfig> GetAllList()
        {
            return this.list;
        }
        public EquipGroupConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [NinoSerialize]
	public partial class EquipGroupConfig: ProtoObject
	{
		/// <summary>ID</summary>
		[NinoMember(1)]
		public int Id { get; set; }
		/// <summary>名称</summary>
		[NinoMember(2)]
		public string Chinese { get; set; }
		/// <summary>名称</summary>
		[NinoMember(3)]
		public string English { get; set; }
		/// <summary>需要同套装备数量</summary>
		[NinoMember(4)]
		public int Count0 { get; set; }
		/// <summary>装备效果</summary>
		[NinoMember(5)]
		public int EffectType0 { get; set; }
		/// <summary>参数</summary>
		[NinoMember(6)]
		public int Param0 { get; set; }
		/// <summary>需要同套装备数量</summary>
		[NinoMember(7)]
		public int Count1 { get; set; }
		/// <summary>装备效果</summary>
		[NinoMember(8)]
		public int EffectType1 { get; set; }
		/// <summary>参数</summary>
		[NinoMember(9)]
		public int Param1 { get; set; }
		/// <summary>需要同套装备数量</summary>
		[NinoMember(10)]
		public int Count2 { get; set; }
		/// <summary>装备效果</summary>
		[NinoMember(11)]
		public int EffectType2 { get; set; }
		/// <summary>参数</summary>
		[NinoMember(12)]
		public int Param2 { get; set; }

	}
}
