using System;
using System.Collections.Generic;
using Nino.Serialization;

namespace TaoTie
{
    [NinoSerialize]
    [Config]
    public partial class ClothConfigCategory : ProtoObject, IMerge
    {
        public static ClothConfigCategory Instance;
		
        
        [NinoIgnore]
        private Dictionary<int, ClothConfig> dict = new Dictionary<int, ClothConfig>();
        
        [NinoMember(1)]
        private List<ClothConfig> list = new List<ClothConfig>();
		
        public ClothConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            ClothConfigCategory s = o as ClothConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            for(int i =0 ;i<list.Count;i++)
            {
                ClothConfig config = list[i];
                config.EndInit();
                this.dict.Add(config.Id, config);
                config.AfterEndInit();
            }            
            this.AfterEndInit();
        }
		
        public ClothConfig Get(int id)
        {
            this.dict.TryGetValue(id, out ClothConfig item);

            if (item == null)
            {
#if !NOT_UNITY
                Log.Error($"配置找不到，配置表名: {nameof (ClothConfig)}，配置id: {id}");
#endif
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, ClothConfig> GetAll()
        {
            return this.dict;
        }
        public List<ClothConfig> GetAllList()
        {
            return this.list;
        }
        public ClothConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [NinoSerialize]
	public partial class ClothConfig: ProtoObject
	{
		/// <summary>ID</summary>
		[NinoMember(1)]
		public int Id { get; set; }
		/// <summary>中文</summary>
		[NinoMember(2)]
		public string Chinese { get; set; }
		/// <summary>英文</summary>
		[NinoMember(3)]
		public string English { get; set; }
		/// <summary>部位</summary>
		[NinoMember(4)]
		public int Module { get; set; }
		/// <summary>预制体路径</summary>
		[NinoMember(5)]
		public string Path { get; set; }
		/// <summary>SkinnedMesh根节点</summary>
		[NinoMember(6)]
		public string RootBone { get; set; }
		/// <summary>图片路径</summary>
		[NinoMember(7)]
		public string Icon { get; set; }
		/// <summary>品质</summary>
		[NinoMember(8)]
		public int Rare { get; set; }
		/// <summary>获取方式（0看广告1金币）</summary>
		[NinoMember(9)]
		public int GetWay { get; set; }
		/// <summary>价格</summary>
		[NinoMember(10)]
		public long Price { get; set; }
		/// <summary>商店刷新随机权重</summary>
		[NinoMember(11)]
		public int Weight { get; set; }
		/// <summary>装备效果</summary>
		[NinoMember(12)]
		public int EffectType { get; set; }
		/// <summary>参数</summary>
		[NinoMember(13)]
		public int Param { get; set; }
		/// <summary>所属套装（0无套装）</summary>
		[NinoMember(14)]
		public int GroupId { get; set; }
		/// <summary>NPC可能穿戴的拍卖场景</summary>
		[NinoMember(15)]
		public int[] LevelIds { get; set; }
		/// <summary>NPC是否在主城出现</summary>
		[NinoMember(16)]
		public int MainScene { get; set; }

	}
}
