using System;
using System.Collections.Generic;
using Nino.Serialization;

namespace TaoTie
{
    [NinoSerialize]
    [Config]
    public partial class PlayTypeConfigCategory : ProtoObject, IMerge
    {
        public static PlayTypeConfigCategory Instance;
		
        
        [NinoIgnore]
        private Dictionary<int, PlayTypeConfig> dict = new Dictionary<int, PlayTypeConfig>();
        
        [NinoMember(1)]
        private List<PlayTypeConfig> list = new List<PlayTypeConfig>();
		
        public PlayTypeConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            PlayTypeConfigCategory s = o as PlayTypeConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            for(int i =0 ;i<list.Count;i++)
            {
                PlayTypeConfig config = list[i];
                config.EndInit();
                this.dict.Add(config.Id, config);
                config.AfterEndInit();
            }            
            this.AfterEndInit();
        }
		
        public PlayTypeConfig Get(int id)
        {
            this.dict.TryGetValue(id, out PlayTypeConfig item);

            if (item == null)
            {
#if !NOT_UNITY
                Log.Error($"配置找不到，配置表名: {nameof (PlayTypeConfig)}，配置id: {id}");
#endif
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, PlayTypeConfig> GetAll()
        {
            return this.dict;
        }
        public List<PlayTypeConfig> GetAllList()
        {
            return this.list;
        }
        public PlayTypeConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [NinoSerialize]
	public partial class PlayTypeConfig: ProtoObject
	{
		/// <summary>Id</summary>
		[NinoMember(1)]
		public int Id { get; set; }
		/// <summary>中文名</summary>
		[NinoMember(2)]
		public string Chinese { get; set; }
		/// <summary>英文名</summary>
		[NinoMember(3)]
		public string English { get; set; }
		/// <summary>介绍</summary>
		[NinoMember(4)]
		public string ChineseDesc { get; set; }
		/// <summary>介绍</summary>
		[NinoMember(5)]
		public string EnglishDesc { get; set; }
		/// <summary>图片</summary>
		[NinoMember(6)]
		public string Icon { get; set; }
		/// <summary>玩法类型(0普通玩法，1全场玩法不重复，2全场玩法可重复)</summary>
		[NinoMember(7)]
		public int Type { get; set; }
		/// <summary>全场时间权值</summary>
		[NinoMember(8)]
		public int Weight { get; set; }
		/// <summary>箱子材质参数</summary>
		[NinoMember(9)]
		public float FresnelPow { get; set; }
		/// <summary>箱子材质参数</summary>
		[NinoMember(10)]
		public float FresnelIntensity { get; set; }
		/// <summary>箱子材质参数</summary>
		[NinoMember(11)]
		public string FresnelTint { get; set; }
		/// <summary>开启类型(0普通物品，4财神爷)</summary>
		[NinoMember(12)]
		public int BoxType { get; set; }
		/// <summary>是否受其他物品影响</summary>
		[NinoMember(13)]
		public int IsEffected { get; set; }
		/// <summary>普通物品结果类型（0无，1玩法界面，2自动打开玩法界面）</summary>
		[NinoMember(14)]
		public int ResultType { get; set; }

	}
}
