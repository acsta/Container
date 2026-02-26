using System;
using System.Collections.Generic;
using Nino.Serialization;

namespace TaoTie
{
    [NinoSerialize]
    [Config]
    public partial class DailyTaskRewardsConfigCategory : ProtoObject, IMerge
    {
        public static DailyTaskRewardsConfigCategory Instance;
		
        
        [NinoIgnore]
        private Dictionary<int, DailyTaskRewardsConfig> dict = new Dictionary<int, DailyTaskRewardsConfig>();
        
        [NinoMember(1)]
        private List<DailyTaskRewardsConfig> list = new List<DailyTaskRewardsConfig>();
		
        public DailyTaskRewardsConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            DailyTaskRewardsConfigCategory s = o as DailyTaskRewardsConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            for(int i =0 ;i<list.Count;i++)
            {
                DailyTaskRewardsConfig config = list[i];
                config.EndInit();
                this.dict.Add(config.Id, config);
                config.AfterEndInit();
            }            
            this.AfterEndInit();
        }
		
        public DailyTaskRewardsConfig Get(int id)
        {
            this.dict.TryGetValue(id, out DailyTaskRewardsConfig item);

            if (item == null)
            {
#if !NOT_UNITY
                Log.Error($"配置找不到，配置表名: {nameof (DailyTaskRewardsConfig)}，配置id: {id}");
#endif
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, DailyTaskRewardsConfig> GetAll()
        {
            return this.dict;
        }
        public List<DailyTaskRewardsConfig> GetAllList()
        {
            return this.list;
        }
        public DailyTaskRewardsConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [NinoSerialize]
	public partial class DailyTaskRewardsConfig: ProtoObject
	{
		/// <summary>Id</summary>
		[NinoMember(1)]
		public int Id { get; set; }
		/// <summary>餐厅等级</summary>
		[NinoMember(2)]
		public int Lv { get; set; }
		/// <summary>需求数量</summary>
		[NinoMember(3)]
		public int TaskCount { get; set; }
		/// <summary>奖励类型（1金币产出效率，2金币）</summary>
		[NinoMember(4)]
		public int RewardType { get; set; }
		/// <summary>奖励数量</summary>
		[NinoMember(5)]
		public long RewardCount { get; set; }
		/// <summary>图标</summary>
		[NinoMember(6)]
		public string Icon { get; set; }

	}
}
