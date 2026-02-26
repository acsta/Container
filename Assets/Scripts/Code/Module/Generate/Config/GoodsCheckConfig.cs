using System;
using System.Collections.Generic;
using Nino.Serialization;

namespace TaoTie
{
    [NinoSerialize]
    [Config]
    public partial class GoodsCheckConfigCategory : ProtoObject, IMerge
    {
        public static GoodsCheckConfigCategory Instance;
		
        
        [NinoIgnore]
        private Dictionary<int, GoodsCheckConfig> dict = new Dictionary<int, GoodsCheckConfig>();
        
        [NinoMember(1)]
        private List<GoodsCheckConfig> list = new List<GoodsCheckConfig>();
		
        public GoodsCheckConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            GoodsCheckConfigCategory s = o as GoodsCheckConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            for(int i =0 ;i<list.Count;i++)
            {
                GoodsCheckConfig config = list[i];
                config.EndInit();
                this.dict.Add(config.Id, config);
                config.AfterEndInit();
            }            
            this.AfterEndInit();
        }
		
        public GoodsCheckConfig Get(int id)
        {
            this.dict.TryGetValue(id, out GoodsCheckConfig item);

            if (item == null)
            {
#if !NOT_UNITY
                Log.Error($"配置找不到，配置表名: {nameof (GoodsCheckConfig)}，配置id: {id}");
#endif
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, GoodsCheckConfig> GetAll()
        {
            return this.dict;
        }
        public List<GoodsCheckConfig> GetAllList()
        {
            return this.list;
        }
        public GoodsCheckConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [NinoSerialize]
	public partial class GoodsCheckConfig: ProtoObject
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
		/// <summary>题目</summary>
		[NinoMember(6)]
		public string QuestionChinese { get; set; }
		/// <summary>题目</summary>
		[NinoMember(7)]
		public string QuestionEnglish { get; set; }
		/// <summary>答案0</summary>
		[NinoMember(8)]
		public string Ans0Chinese { get; set; }
		/// <summary>答案0</summary>
		[NinoMember(9)]
		public string Ans0English { get; set; }
		/// <summary>答案1</summary>
		[NinoMember(10)]
		public string Ans1Chinese { get; set; }
		/// <summary>答案1</summary>
		[NinoMember(11)]
		public string Ans1English { get; set; }
		/// <summary>正确答案0or1</summary>
		[NinoMember(12)]
		public int RightAns { get; set; }

	}
}
