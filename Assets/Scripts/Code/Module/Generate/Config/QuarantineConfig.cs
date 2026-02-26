using System;
using System.Collections.Generic;
using Nino.Serialization;

namespace TaoTie
{
    [NinoSerialize]
    [Config]
    public partial class QuarantineConfigCategory : ProtoObject, IMerge
    {
        public static QuarantineConfigCategory Instance;
		
        
        [NinoIgnore]
        private Dictionary<int, QuarantineConfig> dict = new Dictionary<int, QuarantineConfig>();
        
        [NinoMember(1)]
        private List<QuarantineConfig> list = new List<QuarantineConfig>();
		
        public QuarantineConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            QuarantineConfigCategory s = o as QuarantineConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            for(int i =0 ;i<list.Count;i++)
            {
                QuarantineConfig config = list[i];
                config.EndInit();
                this.dict.Add(config.Id, config);
                config.AfterEndInit();
            }            
            this.AfterEndInit();
        }
		
        public QuarantineConfig Get(int id)
        {
            this.dict.TryGetValue(id, out QuarantineConfig item);

            if (item == null)
            {
#if !NOT_UNITY
                Log.Error($"配置找不到，配置表名: {nameof (QuarantineConfig)}，配置id: {id}");
#endif
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, QuarantineConfig> GetAll()
        {
            return this.dict;
        }
        public List<QuarantineConfig> GetAllList()
        {
            return this.list;
        }
        public QuarantineConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [NinoSerialize]
	public partial class QuarantineConfig: ProtoObject
	{
		/// <summary>Id</summary>
		[NinoMember(1)]
		public int Id { get; set; }
		/// <summary>成功范围小</summary>
		[NinoMember(2)]
		public int SuccessMin { get; set; }
		/// <summary>成功范围大</summary>
		[NinoMember(3)]
		public int SuccessMax { get; set; }
		/// <summary>失败范围小</summary>
		[NinoMember(4)]
		public int FailMin { get; set; }
		/// <summary>失败范围大</summary>
		[NinoMember(5)]
		public int FailMax { get; set; }
		/// <summary>成功率</summary>
		[NinoMember(6)]
		public int Percent { get; set; }

	}
}
