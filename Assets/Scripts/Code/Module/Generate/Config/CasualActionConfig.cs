using System;
using System.Collections.Generic;
using Nino.Serialization;

namespace TaoTie
{
    [NinoSerialize]
    [Config]
    public partial class CasualActionConfigCategory : ProtoObject, IMerge
    {
        public static CasualActionConfigCategory Instance;
		
        
        [NinoIgnore]
        private Dictionary<int, CasualActionConfig> dict = new Dictionary<int, CasualActionConfig>();
        
        [NinoMember(1)]
        private List<CasualActionConfig> list = new List<CasualActionConfig>();
		
        public CasualActionConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            CasualActionConfigCategory s = o as CasualActionConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            for(int i =0 ;i<list.Count;i++)
            {
                CasualActionConfig config = list[i];
                config.EndInit();
                this.dict.Add(config.Id, config);
                config.AfterEndInit();
            }            
            this.AfterEndInit();
        }
		
        public CasualActionConfig Get(int id)
        {
            this.dict.TryGetValue(id, out CasualActionConfig item);

            if (item == null)
            {
#if !NOT_UNITY
                Log.Error($"配置找不到，配置表名: {nameof (CasualActionConfig)}，配置id: {id}");
#endif
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, CasualActionConfig> GetAll()
        {
            return this.dict;
        }
        public List<CasualActionConfig> GetAllList()
        {
            return this.list;
        }
        public CasualActionConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [NinoSerialize]
	public partial class CasualActionConfig: ProtoObject
	{
		/// <summary>Id</summary>
		[NinoMember(1)]
		public int Id { get; set; }
		/// <summary>动作名</summary>
		[NinoMember(2)]
		public string ActionName { get; set; }
		/// <summary>随机权值</summary>
		[NinoMember(3)]
		public int Widget { get; set; }

	}
}
