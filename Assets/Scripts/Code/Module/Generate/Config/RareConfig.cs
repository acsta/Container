using System;
using System.Collections.Generic;
using Nino.Serialization;

namespace TaoTie
{
    [NinoSerialize]
    [Config]
    public partial class RareConfigCategory : ProtoObject, IMerge
    {
        public static RareConfigCategory Instance;
		
        
        [NinoIgnore]
        private Dictionary<int, RareConfig> dict = new Dictionary<int, RareConfig>();
        
        [NinoMember(1)]
        private List<RareConfig> list = new List<RareConfig>();
		
        public RareConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            RareConfigCategory s = o as RareConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            for(int i =0 ;i<list.Count;i++)
            {
                RareConfig config = list[i];
                config.EndInit();
                this.dict.Add(config.Id, config);
                config.AfterEndInit();
            }            
            this.AfterEndInit();
        }
		
        public RareConfig Get(int id)
        {
            this.dict.TryGetValue(id, out RareConfig item);

            if (item == null)
            {
#if !NOT_UNITY
                Log.Error($"配置找不到，配置表名: {nameof (RareConfig)}，配置id: {id}");
#endif
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, RareConfig> GetAll()
        {
            return this.dict;
        }
        public List<RareConfig> GetAllList()
        {
            return this.list;
        }
        public RareConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [NinoSerialize]
	public partial class RareConfig: ProtoObject
	{
		/// <summary>Id</summary>
		[NinoMember(1)]
		public int Id { get; set; }
		/// <summary>颜色</summary>
		[NinoMember(2)]
		public string Color { get; set; }
		/// <summary>图标</summary>
		[NinoMember(3)]
		public string Icon { get; set; }

	}
}
