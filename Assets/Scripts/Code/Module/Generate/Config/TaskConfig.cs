using System;
using System.Collections.Generic;
using Nino.Serialization;

namespace TaoTie
{
    [NinoSerialize]
    [Config]
    public partial class TaskConfigCategory : ProtoObject, IMerge
    {
        public static TaskConfigCategory Instance;
		
        
        [NinoIgnore]
        private Dictionary<int, TaskConfig> dict = new Dictionary<int, TaskConfig>();
        
        [NinoMember(1)]
        private List<TaskConfig> list = new List<TaskConfig>();
		
        public TaskConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            TaskConfigCategory s = o as TaskConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            for(int i =0 ;i<list.Count;i++)
            {
                TaskConfig config = list[i];
                config.EndInit();
                this.dict.Add(config.Id, config);
                config.AfterEndInit();
            }            
            this.AfterEndInit();
        }
		
        public TaskConfig Get(int id)
        {
            this.dict.TryGetValue(id, out TaskConfig item);

            if (item == null)
            {
#if !NOT_UNITY
                Log.Error($"配置找不到，配置表名: {nameof (TaskConfig)}，配置id: {id}");
#endif
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, TaskConfig> GetAll()
        {
            return this.dict;
        }
        public List<TaskConfig> GetAllList()
        {
            return this.list;
        }
        public TaskConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [NinoSerialize]
	public partial class TaskConfig: ProtoObject
	{
		/// <summary>Id</summary>
		[NinoMember(1)]
		public int Id { get; set; }
		/// <summary>餐厅等级多少解锁</summary>
		[NinoMember(2)]
		public int Lv { get; set; }
		/// <summary>Npc中文名</summary>
		[NinoMember(3)]
		public string Chinese { get; set; }
		/// <summary>Npc英文名</summary>
		[NinoMember(4)]
		public string English { get; set; }
		/// <summary>任务描述</summary>
		[NinoMember(5)]
		public string ChineseDesc { get; set; }
		/// <summary>任务描述</summary>
		[NinoMember(6)]
		public string EnglishDesc { get; set; }
		/// <summary>稀有度</summary>
		[NinoMember(7)]
		public int Rare { get; set; }
		/// <summary>需求物品类型(0物品1集装箱)</summary>
		[NinoMember(8)]
		public int ItemType { get; set; }
		/// <summary>需求物品Id</summary>
		[NinoMember(9)]
		public int ItemId { get; set; }
		/// <summary>需求数量</summary>
		[NinoMember(10)]
		public int ItemCount { get; set; }
		/// <summary>出现在所属集装箱百分率</summary>
		[NinoMember(11)]
		public int Percent { get; set; }
		/// <summary>奖励类型（1金币产出效率，2金币）</summary>
		[NinoMember(12)]
		public int RewardType { get; set; }
		/// <summary>奖励数量</summary>
		[NinoMember(13)]
		public long RewardCount { get; set; }
		/// <summary>是否有看广告键</summary>
		[NinoMember(14)]
		public int AdvertisementButton { get; set; }

	}
}
