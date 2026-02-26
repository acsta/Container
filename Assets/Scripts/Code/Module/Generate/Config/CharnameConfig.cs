using System;
using System.Collections.Generic;
using Nino.Serialization;

namespace TaoTie
{
    [NinoSerialize]
    [Config]
    public partial class CharnameConfigCategory : ProtoObject, IMerge
    {
        public static CharnameConfigCategory Instance;
		
        
        [NinoIgnore]
        private Dictionary<int, CharnameConfig> dict = new Dictionary<int, CharnameConfig>();
        
        [NinoMember(1)]
        private List<CharnameConfig> list = new List<CharnameConfig>();
		
        public CharnameConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            CharnameConfigCategory s = o as CharnameConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            for(int i =0 ;i<list.Count;i++)
            {
                CharnameConfig config = list[i];
                config.EndInit();
                this.dict.Add(config.Id, config);
                config.AfterEndInit();
            }            
            this.AfterEndInit();
        }
		
        public CharnameConfig Get(int id)
        {
            this.dict.TryGetValue(id, out CharnameConfig item);

            if (item == null)
            {
#if !NOT_UNITY
                Log.Error($"配置找不到，配置表名: {nameof (CharnameConfig)}，配置id: {id}");
#endif
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, CharnameConfig> GetAll()
        {
            return this.dict;
        }
        public List<CharnameConfig> GetAllList()
        {
            return this.list;
        }
        public CharnameConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [NinoSerialize]
	public partial class CharnameConfig: ProtoObject
	{
		/// <summary>ID</summary>
		[NinoMember(1)]
		public int Id { get; set; }
		/// <summary>名称（中文）</summary>
		[NinoMember(2)]
		public string CharacternameCHS { get; set; }
		/// <summary>名称（英文）</summary>
		[NinoMember(3)]
		public string CharacternameENG { get; set; }

	}
}
