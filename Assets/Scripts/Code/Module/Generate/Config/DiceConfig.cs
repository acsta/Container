using System;
using System.Collections.Generic;
using Nino.Serialization;

namespace TaoTie
{
    [NinoSerialize]
    [Config]
    public partial class DiceConfigCategory : ProtoObject, IMerge
    {
        public static DiceConfigCategory Instance;
		
        
        [NinoIgnore]
        private Dictionary<int, DiceConfig> dict = new Dictionary<int, DiceConfig>();
        
        [NinoMember(1)]
        private List<DiceConfig> list = new List<DiceConfig>();
		
        public DiceConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            DiceConfigCategory s = o as DiceConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            for(int i =0 ;i<list.Count;i++)
            {
                DiceConfig config = list[i];
                config.EndInit();
                this.dict.Add(config.Id, config);
                config.AfterEndInit();
            }            
            this.AfterEndInit();
        }
		
        public DiceConfig Get(int id)
        {
            this.dict.TryGetValue(id, out DiceConfig item);

            if (item == null)
            {
#if !NOT_UNITY
                Log.Error($"配置找不到，配置表名: {nameof (DiceConfig)}，配置id: {id}");
#endif
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, DiceConfig> GetAll()
        {
            return this.dict;
        }
        public List<DiceConfig> GetAllList()
        {
            return this.list;
        }
        public DiceConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [NinoSerialize]
	public partial class DiceConfig: ProtoObject
	{
		/// <summary>Id</summary>
		[NinoMember(1)]
		public int Id { get; set; }
		/// <summary>可出现的关卡</summary>
		[NinoMember(2)]
		public int[] Level { get; set; }
		/// <summary>随机权重</summary>
		[NinoMember(3)]
		public int Weight { get; set; }
		/// <summary>中文名</summary>
		[NinoMember(4)]
		public string Chinese { get; set; }
		/// <summary>英文名</summary>
		[NinoMember(5)]
		public string English { get; set; }
		/// <summary>介绍</summary>
		[NinoMember(6)]
		public string ChineseDesc { get; set; }
		/// <summary>介绍</summary>
		[NinoMember(7)]
		public string EnglishDesc { get; set; }
		/// <summary>图片</summary>
		[NinoMember(8)]
		public string Icon { get; set; }
		/// <summary>类型</summary>
		[NinoMember(9)]
		public int Type { get; set; }
		/// <summary>参数</summary>
		[NinoMember(10)]
		public int Param { get; set; }

	}
}
