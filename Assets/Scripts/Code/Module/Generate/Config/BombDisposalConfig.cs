using System;
using System.Collections.Generic;
using Nino.Serialization;

namespace TaoTie
{
    [NinoSerialize]
    [Config]
    public partial class BombDisposalConfigCategory : ProtoObject, IMerge
    {
        public static BombDisposalConfigCategory Instance;
		
        
        [NinoIgnore]
        private Dictionary<int, BombDisposalConfig> dict = new Dictionary<int, BombDisposalConfig>();
        
        [NinoMember(1)]
        private List<BombDisposalConfig> list = new List<BombDisposalConfig>();
		
        public BombDisposalConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            BombDisposalConfigCategory s = o as BombDisposalConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            for(int i =0 ;i<list.Count;i++)
            {
                BombDisposalConfig config = list[i];
                config.EndInit();
                this.dict.Add(config.Id, config);
                config.AfterEndInit();
            }            
            this.AfterEndInit();
        }
		
        public BombDisposalConfig Get(int id)
        {
            this.dict.TryGetValue(id, out BombDisposalConfig item);

            if (item == null)
            {
#if !NOT_UNITY
                Log.Error($"配置找不到，配置表名: {nameof (BombDisposalConfig)}，配置id: {id}");
#endif
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, BombDisposalConfig> GetAll()
        {
            return this.dict;
        }
        public List<BombDisposalConfig> GetAllList()
        {
            return this.list;
        }
        public BombDisposalConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [NinoSerialize]
	public partial class BombDisposalConfig: ProtoObject
	{
		/// <summary>Id</summary>
		[NinoMember(1)]
		public int Id { get; set; }
		/// <summary>随机权值</summary>
		[NinoMember(2)]
		public int Weight { get; set; }
		/// <summary>成功范围小</summary>
		[NinoMember(3)]
		public int SuccessMin { get; set; }
		/// <summary>成功范围大</summary>
		[NinoMember(4)]
		public int SuccessMax { get; set; }
		/// <summary>失败范围小</summary>
		[NinoMember(5)]
		public int FailMin { get; set; }
		/// <summary>失败范围大</summary>
		[NinoMember(6)]
		public int FailMax { get; set; }
		/// <summary>额外线头数</summary>
		[NinoMember(7)]
		public int LineCount { get; set; }
		/// <summary>成功范围小</summary>
		[NinoMember(8)]
		public int AdMin { get; set; }
		/// <summary>成功范围大</summary>
		[NinoMember(9)]
		public int AdMax { get; set; }

	}
}
