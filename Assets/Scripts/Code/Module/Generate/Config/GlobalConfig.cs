using System;
using System.Collections.Generic;
using Nino.Serialization;

namespace TaoTie
{
    [NinoSerialize]
    [Config]
    public partial class GlobalConfigCategory : ProtoObject, IMerge
    {
        public static GlobalConfigCategory Instance;
		
        
        [NinoIgnore]
        private Dictionary<int, GlobalConfig> dict = new Dictionary<int, GlobalConfig>();
        
        [NinoMember(1)]
        private List<GlobalConfig> list = new List<GlobalConfig>();
		
        public GlobalConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            GlobalConfigCategory s = o as GlobalConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            for(int i =0 ;i<list.Count;i++)
            {
                GlobalConfig config = list[i];
                config.EndInit();
                this.dict.Add(config.Id, config);
                config.AfterEndInit();
            }            
            this.AfterEndInit();
        }
		
        public GlobalConfig Get(int id)
        {
            this.dict.TryGetValue(id, out GlobalConfig item);

            if (item == null)
            {
#if !NOT_UNITY
                Log.Error($"配置找不到，配置表名: {nameof (GlobalConfig)}，配置id: {id}");
#endif
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, GlobalConfig> GetAll()
        {
            return this.dict;
        }
        public List<GlobalConfig> GetAllList()
        {
            return this.list;
        }
        public GlobalConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [NinoSerialize]
	public partial class GlobalConfig: ProtoObject
	{
		/// <summary>ID</summary>
		[NinoMember(1)]
		public int Id { get; set; }
		/// <summary>键</summary>
		[NinoMember(2)]
		public string Key { get; set; }
		/// <summary>值</summary>
		[NinoMember(3)]
		public string Value { get; set; }

	}
}
