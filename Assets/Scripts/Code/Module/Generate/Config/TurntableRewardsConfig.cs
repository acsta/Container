using System;
using System.Collections.Generic;
using Nino.Serialization;

namespace TaoTie
{
    [NinoSerialize]
    [Config]
    public partial class TurntableRewardsConfigCategory : ProtoObject, IMerge
    {
        public static TurntableRewardsConfigCategory Instance;
		
        
        [NinoIgnore]
        private Dictionary<int, TurntableRewardsConfig> dict = new Dictionary<int, TurntableRewardsConfig>();
        
        [NinoMember(1)]
        private List<TurntableRewardsConfig> list = new List<TurntableRewardsConfig>();
		
        public TurntableRewardsConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            TurntableRewardsConfigCategory s = o as TurntableRewardsConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            for(int i =0 ;i<list.Count;i++)
            {
                TurntableRewardsConfig config = list[i];
                config.EndInit();
                this.dict.Add(config.Id, config);
                config.AfterEndInit();
            }            
            this.AfterEndInit();
        }
		
        public TurntableRewardsConfig Get(int id)
        {
            this.dict.TryGetValue(id, out TurntableRewardsConfig item);

            if (item == null)
            {
#if !NOT_UNITY
                Log.Error($"配置找不到，配置表名: {nameof (TurntableRewardsConfig)}，配置id: {id}");
#endif
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, TurntableRewardsConfig> GetAll()
        {
            return this.dict;
        }
        public List<TurntableRewardsConfig> GetAllList()
        {
            return this.list;
        }
        public TurntableRewardsConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [NinoSerialize]
	public partial class TurntableRewardsConfig: ProtoObject
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
		/// <summary>奖励数量</summary>
		[NinoMember(5)]
		public long RewardCount { get; set; }
		/// <summary>图标</summary>
		[NinoMember(6)]
		public string Icon { get; set; }

	}
}
