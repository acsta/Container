using System;
using System.Collections.Generic;
using Nino.Serialization;

namespace TaoTie
{
    [NinoSerialize]
    [Config]
    public partial class GameInfoConfigCategory : ProtoObject, IMerge
    {
        public static GameInfoConfigCategory Instance;
		
        
        [NinoIgnore]
        private Dictionary<int, GameInfoConfig> dict = new Dictionary<int, GameInfoConfig>();
        
        [NinoMember(1)]
        private List<GameInfoConfig> list = new List<GameInfoConfig>();
		
        public GameInfoConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            GameInfoConfigCategory s = o as GameInfoConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            for(int i =0 ;i<list.Count;i++)
            {
                GameInfoConfig config = list[i];
                config.EndInit();
                this.dict.Add(config.Id, config);
                config.AfterEndInit();
            }            
            this.AfterEndInit();
        }
		
        public GameInfoConfig Get(int id)
        {
            this.dict.TryGetValue(id, out GameInfoConfig item);

            if (item == null)
            {
#if !NOT_UNITY
                Log.Error($"配置找不到，配置表名: {nameof (GameInfoConfig)}，配置id: {id}");
#endif
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, GameInfoConfig> GetAll()
        {
            return this.dict;
        }
        public List<GameInfoConfig> GetAllList()
        {
            return this.list;
        }
        public GameInfoConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [NinoSerialize]
	public partial class GameInfoConfig: ProtoObject
	{
		/// <summary>Id</summary>
		[NinoMember(1)]
		public int Id { get; set; }
		/// <summary>所属关卡</summary>
		[NinoMember(2)]
		public int Level { get; set; }
		/// <summary>难度</summary>
		[NinoMember(3)]
		public int Rare { get; set; }
		/// <summary>委托人名</summary>
		[NinoMember(4)]
		public string Chinese { get; set; }
		/// <summary>委托人名</summary>
		[NinoMember(5)]
		public string English { get; set; }
		/// <summary>描述</summary>
		[NinoMember(6)]
		public string ChineseDesc { get; set; }
		/// <summary>描述</summary>
		[NinoMember(7)]
		public string EnglishDesc { get; set; }
		/// <summary>情报触发条件（0无）</summary>
		[NinoMember(8)]
		public int Condition { get; set; }
		/// <summary>条件内容</summary>
		[NinoMember(9)]
		public int[] Content { get; set; }
		/// <summary>情报奖励目标类型</summary>
		[NinoMember(10)]
		public int Type { get; set; }
		/// <summary>情报奖励目标</summary>
		[NinoMember(11)]
		public int[] Ids { get; set; }
		/// <summary>奖励类型（0加固定值1乘固定值）</summary>
		[NinoMember(12)]
		public int AwardType { get; set; }
		/// <summary>奖励数量</summary>
		[NinoMember(13)]
		public long RewardCount { get; set; }

	}
}
