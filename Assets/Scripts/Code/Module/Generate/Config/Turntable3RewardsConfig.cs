using System;
using System.Collections.Generic;
using Nino.Serialization;

namespace TaoTie
{
    [NinoSerialize]
    [Config]
    public partial class Turntable3RewardsConfigCategory : ProtoObject, IMerge
    {
        public static Turntable3RewardsConfigCategory Instance;
		
        
        [NinoIgnore]
        private Dictionary<int, Turntable3RewardsConfig> dict = new Dictionary<int, Turntable3RewardsConfig>();
        
        [NinoMember(1)]
        private List<Turntable3RewardsConfig> list = new List<Turntable3RewardsConfig>();
		
        public Turntable3RewardsConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            Turntable3RewardsConfigCategory s = o as Turntable3RewardsConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            for(int i =0 ;i<list.Count;i++)
            {
                Turntable3RewardsConfig config = list[i];
                config.EndInit();
                this.dict.Add(config.Id, config);
                config.AfterEndInit();
            }            
            this.AfterEndInit();
        }
		
        public Turntable3RewardsConfig Get(int id)
        {
            this.dict.TryGetValue(id, out Turntable3RewardsConfig item);

            if (item == null)
            {
#if !NOT_UNITY
                Log.Error($"配置找不到，配置表名: {nameof (Turntable3RewardsConfig)}，配置id: {id}");
#endif
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, Turntable3RewardsConfig> GetAll()
        {
            return this.dict;
        }
        public List<Turntable3RewardsConfig> GetAllList()
        {
            return this.list;
        }
        public Turntable3RewardsConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [NinoSerialize]
	public partial class Turntable3RewardsConfig: ProtoObject
	{
		/// <summary>Id</summary>
		[NinoMember(1)]
		public int Id { get; set; }
		/// <summary>出现场次</summary>
		[NinoMember(2)]
		public int Level { get; set; }
		/// <summary>餐厅等级</summary>
		[NinoMember(3)]
		public int Lv { get; set; }
		/// <summary>权重</summary>
		[NinoMember(4)]
		public int Weight { get; set; }
		/// <summary>奖励类型</summary>
		[NinoMember(5)]
		public int ItemId { get; set; }
		/// <summary>奖励数量</summary>
		[NinoMember(6)]
		public long RewardCount { get; set; }

	}
}
