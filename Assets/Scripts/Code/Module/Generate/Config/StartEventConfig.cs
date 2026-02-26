using System;
using System.Collections.Generic;
using Nino.Serialization;

namespace TaoTie
{
    [NinoSerialize]
    [Config]
    public partial class StartEventConfigCategory : ProtoObject, IMerge
    {
        public static StartEventConfigCategory Instance;
		
        
        [NinoIgnore]
        private Dictionary<int, StartEventConfig> dict = new Dictionary<int, StartEventConfig>();
        
        [NinoMember(1)]
        private List<StartEventConfig> list = new List<StartEventConfig>();
		
        public StartEventConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            StartEventConfigCategory s = o as StartEventConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            for(int i =0 ;i<list.Count;i++)
            {
                StartEventConfig config = list[i];
                config.EndInit();
                this.dict.Add(config.Id, config);
                config.AfterEndInit();
            }            
            this.AfterEndInit();
        }
		
        public StartEventConfig Get(int id)
        {
            this.dict.TryGetValue(id, out StartEventConfig item);

            if (item == null)
            {
#if !NOT_UNITY
                Log.Error($"配置找不到，配置表名: {nameof (StartEventConfig)}，配置id: {id}");
#endif
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, StartEventConfig> GetAll()
        {
            return this.dict;
        }
        public List<StartEventConfig> GetAllList()
        {
            return this.list;
        }
        public StartEventConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [NinoSerialize]
	public partial class StartEventConfig: ProtoObject
	{
		/// <summary>Id</summary>
		[NinoMember(1)]
		public int Id { get; set; }
		/// <summary>随机权值</summary>
		[NinoMember(2)]
		public int Weight { get; set; }
		/// <summary>类型</summary>
		[NinoMember(3)]
		public int Type { get; set; }
		/// <summary>行为树标记参数</summary>
		[NinoMember(4)]
		public int AIFlag { get; set; }
		/// <summary>其他物品是否可空箱（0不可1任务可2玩法可3都可）</summary>
		[NinoMember(5)]
		public int EmptyType { get; set; }
		/// <summary>空箱随机概率百分比</summary>
		[NinoMember(6)]
		public int EmptyPercent { get; set; }
		/// <summary>空箱最多个数</summary>
		[NinoMember(7)]
		public int EmptyMaxCount { get; set; }

	}
}
