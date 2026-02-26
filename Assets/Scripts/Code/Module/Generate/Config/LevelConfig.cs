using System;
using System.Collections.Generic;
using Nino.Serialization;

namespace TaoTie
{
    [NinoSerialize]
    [Config]
    public partial class LevelConfigCategory : ProtoObject, IMerge
    {
        public static LevelConfigCategory Instance;
		
        
        [NinoIgnore]
        private Dictionary<int, LevelConfig> dict = new Dictionary<int, LevelConfig>();
        
        [NinoMember(1)]
        private List<LevelConfig> list = new List<LevelConfig>();
		
        public LevelConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            LevelConfigCategory s = o as LevelConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            for(int i =0 ;i<list.Count;i++)
            {
                LevelConfig config = list[i];
                config.EndInit();
                this.dict.Add(config.Id, config);
                config.AfterEndInit();
            }            
            this.AfterEndInit();
        }
		
        public LevelConfig Get(int id)
        {
            this.dict.TryGetValue(id, out LevelConfig item);

            if (item == null)
            {
#if !NOT_UNITY
                Log.Error($"配置找不到，配置表名: {nameof (LevelConfig)}，配置id: {id}");
#endif
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, LevelConfig> GetAll()
        {
            return this.dict;
        }
        public List<LevelConfig> GetAllList()
        {
            return this.list;
        }
        public LevelConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [NinoSerialize]
	public partial class LevelConfig: ProtoObject
	{
		/// <summary>Id</summary>
		[NinoMember(1)]
		public int Id { get; set; }
		/// <summary>是否隐藏</summary>
		[NinoMember(2)]
		public int Hide { get; set; }
		/// <summary>拍卖师Id</summary>
		[NinoMember(3)]
		public int Hoster { get; set; }
		/// <summary>场景名</summary>
		[NinoMember(4)]
		public string Name { get; set; }
		/// <summary>颜色</summary>
		[NinoMember(5)]
		public string Color { get; set; }
		/// <summary>名字</summary>
		[NinoMember(6)]
		public string Chinese { get; set; }
		/// <summary>名字</summary>
		[NinoMember(7)]
		public string English { get; set; }
		/// <summary>描述</summary>
		[NinoMember(8)]
		public string ChineseDesc { get; set; }
		/// <summary>描述</summary>
		[NinoMember(9)]
		public string EnglishDesc { get; set; }
		/// <summary>图标</summary>
		[NinoMember(10)]
		public string Icon { get; set; }
		/// <summary>入场门槛</summary>
		[NinoMember(11)]
		public long Money { get; set; }
		/// <summary>场景路径</summary>
		[NinoMember(12)]
		public string Perfab { get; set; }
		/// <summary>登场AI列表</summary>
		[NinoMember(13)]
		public int[] AIIds { get; set; }
		/// <summary>是否补齐氛围NPC</summary>
		[NinoMember(14)]
		public int NeedNpc { get; set; }
		/// <summary>日夜循环</summary>
		[NinoMember(15)]
		public int[] DayNight { get; set; }
		/// <summary>特殊集装箱盲盒解锁1个时随机的特殊盲盒数量</summary>
		[NinoMember(16)]
		public int[] ContainerCounts1 { get; set; }
		/// <summary>特殊集装箱盲盒解锁1个时随机的特殊盲盒权值</summary>
		[NinoMember(17)]
		public int[] ContainerWeight1 { get; set; }
		/// <summary>特殊集装箱盲盒解锁2个时随机的特殊盲盒数量</summary>
		[NinoMember(18)]
		public int[] ContainerCounts2 { get; set; }
		/// <summary>特殊集装箱盲盒解锁2个时随机的特殊盲盒权值</summary>
		[NinoMember(19)]
		public int[] ContainerWeight2 { get; set; }
		/// <summary>特殊集装箱盲盒解锁3个以上时随机的特殊盲盒数量</summary>
		[NinoMember(20)]
		public int[] ContainerCounts3 { get; set; }
		/// <summary>特殊集装箱盲盒解锁3个以上时随机的特殊盲盒权值</summary>
		[NinoMember(21)]
		public int[] ContainerWeight3 { get; set; }
		/// <summary>AI参与小玩法的概率（百分比）</summary>
		[NinoMember(22)]
		public int AIMiniPlayPercent { get; set; }
		/// <summary>随机出情报事件的百分比</summary>
		[NinoMember(23)]
		public int GameInfoPercent { get; set; }
		/// <summary>看广告奖励金钱数额</summary>
		[NinoMember(24)]
		public int AdMoneyCount { get; set; }
		/// <summary>黑衣人出现概率</summary>
		[NinoMember(25)]
		public int BlackPercent { get; set; }
		/// <summary>黑衣人出现最大个数</summary>
		[NinoMember(26)]
		public int BlackCount { get; set; }

	}
}
