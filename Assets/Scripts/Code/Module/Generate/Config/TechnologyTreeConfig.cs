using System;
using System.Collections.Generic;
using Nino.Serialization;

namespace TaoTie
{
    [NinoSerialize]
    [Config]
    public partial class TechnologyTreeConfigCategory : ProtoObject, IMerge
    {
        public static TechnologyTreeConfigCategory Instance;
		
        
        [NinoIgnore]
        private Dictionary<int, TechnologyTreeConfig> dict = new Dictionary<int, TechnologyTreeConfig>();
        
        [NinoMember(1)]
        private List<TechnologyTreeConfig> list = new List<TechnologyTreeConfig>();
		
        public TechnologyTreeConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            TechnologyTreeConfigCategory s = o as TechnologyTreeConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            for(int i =0 ;i<list.Count;i++)
            {
                TechnologyTreeConfig config = list[i];
                config.EndInit();
                this.dict.Add(config.Id, config);
                config.AfterEndInit();
            }            
            this.AfterEndInit();
        }
		
        public TechnologyTreeConfig Get(int id)
        {
            this.dict.TryGetValue(id, out TechnologyTreeConfig item);

            if (item == null)
            {
#if !NOT_UNITY
                Log.Error($"配置找不到，配置表名: {nameof (TechnologyTreeConfig)}，配置id: {id}");
#endif
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, TechnologyTreeConfig> GetAll()
        {
            return this.dict;
        }
        public List<TechnologyTreeConfig> GetAllList()
        {
            return this.list;
        }
        public TechnologyTreeConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [NinoSerialize]
	public partial class TechnologyTreeConfig: ProtoObject
	{
		/// <summary>Id</summary>
		[NinoMember(1)]
		public int Id { get; set; }
		/// <summary>不在UI面板中展示（0展示1不展示）</summary>
		[NinoMember(2)]
		public int HideUI { get; set; }
		/// <summary>中文名</summary>
		[NinoMember(3)]
		public string Chinese { get; set; }
		/// <summary>英文名</summary>
		[NinoMember(4)]
		public string English { get; set; }
		/// <summary>介绍</summary>
		[NinoMember(5)]
		public string ChineseDesc { get; set; }
		/// <summary>介绍</summary>
		[NinoMember(6)]
		public string EnglishDesc { get; set; }
		/// <summary>图片</summary>
		[NinoMember(7)]
		public string Icon { get; set; }
		/// <summary>层级(0,拍卖场等极1集装箱类型，2玩法)</summary>
		[NinoMember(8)]
		public int Level { get; set; }
		/// <summary>解锁的内容</summary>
		[NinoMember(9)]
		public int Content { get; set; }
		/// <summary>解锁条件类型（0默认解锁，1花钱解锁，2参与一次对应关卡解锁）</summary>
		[NinoMember(10)]
		public int UnlockType { get; set; }
		/// <summary>解锁花费</summary>
		[NinoMember(11)]
		public int UnlockValue { get; set; }

	}
}
