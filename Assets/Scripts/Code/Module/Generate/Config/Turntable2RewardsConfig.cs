using System;
using System.Collections.Generic;
using Nino.Serialization;

namespace TaoTie
{
    [NinoSerialize]
    [Config]
    public partial class Turntable2RewardsConfigCategory : ProtoObject, IMerge
    {
        public static Turntable2RewardsConfigCategory Instance;
		
        
        [NinoIgnore]
        private Dictionary<int, Turntable2RewardsConfig> dict = new Dictionary<int, Turntable2RewardsConfig>();
        
        [NinoMember(1)]
        private List<Turntable2RewardsConfig> list = new List<Turntable2RewardsConfig>();
		
        public Turntable2RewardsConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            Turntable2RewardsConfigCategory s = o as Turntable2RewardsConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            for(int i =0 ;i<list.Count;i++)
            {
                Turntable2RewardsConfig config = list[i];
                config.EndInit();
                this.dict.Add(config.Id, config);
                config.AfterEndInit();
            }            
            this.AfterEndInit();
        }
		
        public Turntable2RewardsConfig Get(int id)
        {
            this.dict.TryGetValue(id, out Turntable2RewardsConfig item);

            if (item == null)
            {
#if !NOT_UNITY
                Log.Error($"配置找不到，配置表名: {nameof (Turntable2RewardsConfig)}，配置id: {id}");
#endif
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, Turntable2RewardsConfig> GetAll()
        {
            return this.dict;
        }
        public List<Turntable2RewardsConfig> GetAllList()
        {
            return this.list;
        }
        public Turntable2RewardsConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [NinoSerialize]
	public partial class Turntable2RewardsConfig: ProtoObject
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
		/// <summary>角度范围</summary>
		[NinoMember(4)]
		public int[] Range { get; set; }
		/// <summary>奖励倍率百分比</summary>
		[NinoMember(5)]
		public int[] RewardPercent { get; set; }
		/// <summary>图标</summary>
		[NinoMember(6)]
		public string Chinese { get; set; }
		/// <summary>图标</summary>
		[NinoMember(7)]
		public string English { get; set; }

	}
}
