using System;
using System.Collections.Generic;
using Nino.Serialization;

namespace TaoTie
{
    [NinoSerialize]
    [Config]
    public partial class RepairConfigCategory : ProtoObject, IMerge
    {
        public static RepairConfigCategory Instance;
		
        
        [NinoIgnore]
        private Dictionary<int, RepairConfig> dict = new Dictionary<int, RepairConfig>();
        
        [NinoMember(1)]
        private List<RepairConfig> list = new List<RepairConfig>();
		
        public RepairConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            RepairConfigCategory s = o as RepairConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            for(int i =0 ;i<list.Count;i++)
            {
                RepairConfig config = list[i];
                config.EndInit();
                this.dict.Add(config.Id, config);
                config.AfterEndInit();
            }            
            this.AfterEndInit();
        }
		
        public RepairConfig Get(int id)
        {
            this.dict.TryGetValue(id, out RepairConfig item);

            if (item == null)
            {
#if !NOT_UNITY
                Log.Error($"配置找不到，配置表名: {nameof (RepairConfig)}，配置id: {id}");
#endif
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, RepairConfig> GetAll()
        {
            return this.dict;
        }
        public List<RepairConfig> GetAllList()
        {
            return this.list;
        }
        public RepairConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [NinoSerialize]
	public partial class RepairConfig: ProtoObject
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
		/// <summary>成功距离差距百分比</summary>
		[NinoMember(6)]
		public int Success { get; set; }
		/// <summary>修补范围（百分比）</summary>
		[NinoMember(7)]
		public int Size { get; set; }
		/// <summary>移动一次的时长</summary>
		[NinoMember(8)]
		public int During { get; set; }

	}
}
