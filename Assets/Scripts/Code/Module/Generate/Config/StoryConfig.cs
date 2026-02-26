using System;
using System.Collections.Generic;
using Nino.Serialization;

namespace TaoTie
{
    [NinoSerialize]
    [Config]
    public partial class StoryConfigCategory : ProtoObject, IMerge
    {
        public static StoryConfigCategory Instance;
		
        
        [NinoIgnore]
        private Dictionary<int, StoryConfig> dict = new Dictionary<int, StoryConfig>();
        
        [NinoMember(1)]
        private List<StoryConfig> list = new List<StoryConfig>();
		
        public StoryConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            StoryConfigCategory s = o as StoryConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            for(int i =0 ;i<list.Count;i++)
            {
                StoryConfig config = list[i];
                config.EndInit();
                this.dict.Add(config.Id, config);
                config.AfterEndInit();
            }            
            this.AfterEndInit();
        }
		
        public StoryConfig Get(int id)
        {
            this.dict.TryGetValue(id, out StoryConfig item);

            if (item == null)
            {
#if !NOT_UNITY
                Log.Error($"配置找不到，配置表名: {nameof (StoryConfig)}，配置id: {id}");
#endif
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, StoryConfig> GetAll()
        {
            return this.dict;
        }
        public List<StoryConfig> GetAllList()
        {
            return this.list;
        }
        public StoryConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [NinoSerialize]
	public partial class StoryConfig: ProtoObject
	{
		/// <summary>Id</summary>
		[NinoMember(1)]
		public int Id { get; set; }
		/// <summary>选项个数</summary>
		[NinoMember(2)]
		public int ChooseCount { get; set; }
		/// <summary>权重</summary>
		[NinoMember(3)]
		public int Weight { get; set; }
		/// <summary>选项0成功结果范围</summary>
		[NinoMember(4)]
		public int[] ResultSucc0 { get; set; }
		/// <summary>选项0失败结果范围</summary>
		[NinoMember(5)]
		public int[] ResultFail0 { get; set; }
		/// <summary>选项0成功概率</summary>
		[NinoMember(6)]
		public int Choose0SuccessPercent { get; set; }
		/// <summary>选项1成功结果范围</summary>
		[NinoMember(7)]
		public int[] ResultSucc1 { get; set; }
		/// <summary>选项1失败结果范围</summary>
		[NinoMember(8)]
		public int[] ResultFail1 { get; set; }
		/// <summary>选项1成功概率</summary>
		[NinoMember(9)]
		public int Choose1SuccessPercent { get; set; }
		/// <summary>内容</summary>
		[NinoMember(10)]
		public string Chinese { get; set; }
		/// <summary>内容</summary>
		[NinoMember(11)]
		public string English { get; set; }
		/// <summary>按钮类型（0无1花钱2看广告）</summary>
		[NinoMember(12)]
		public int Type0 { get; set; }
		/// <summary>数量0</summary>
		[NinoMember(13)]
		public int Count0 { get; set; }
		/// <summary>选项0</summary>
		[NinoMember(14)]
		public string Choose0Chinese { get; set; }
		/// <summary>选项0</summary>
		[NinoMember(15)]
		public string Choose0English { get; set; }
		/// <summary>选项0成功结果</summary>
		[NinoMember(16)]
		public string ResultSucc0Chinese { get; set; }
		/// <summary>选项0成功结果</summary>
		[NinoMember(17)]
		public string ResultSucc0English { get; set; }
		/// <summary>选项0失败结果</summary>
		[NinoMember(18)]
		public string ResultFail0Chinese { get; set; }
		/// <summary>选项0失败结果</summary>
		[NinoMember(19)]
		public string ResultFail0English { get; set; }
		/// <summary>按钮类型（0无1花钱2看广告）</summary>
		[NinoMember(20)]
		public int Type1 { get; set; }
		/// <summary>数量1</summary>
		[NinoMember(21)]
		public int Count1 { get; set; }
		/// <summary>选项1</summary>
		[NinoMember(22)]
		public string Choose1Chinese { get; set; }
		/// <summary>选项1</summary>
		[NinoMember(23)]
		public string Choose1English { get; set; }
		/// <summary>选项1成功结果</summary>
		[NinoMember(24)]
		public string ResultSucc1Chinese { get; set; }
		/// <summary>选项1成功结果</summary>
		[NinoMember(25)]
		public string ResultSucc1English { get; set; }
		/// <summary>选项1失败结果</summary>
		[NinoMember(26)]
		public string ResultFail1Chinese { get; set; }
		/// <summary>选项1失败结果</summary>
		[NinoMember(27)]
		public string ResultFail1English { get; set; }

	}
}
