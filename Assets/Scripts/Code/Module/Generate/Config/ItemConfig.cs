using System;
using System.Collections.Generic;
using Nino.Serialization;

namespace TaoTie
{
    [NinoSerialize]
    [Config]
    public partial class ItemConfigCategory : ProtoObject, IMerge
    {
        public static ItemConfigCategory Instance;
		
        
        [NinoIgnore]
        private Dictionary<int, ItemConfig> dict = new Dictionary<int, ItemConfig>();
        
        [NinoMember(1)]
        private List<ItemConfig> list = new List<ItemConfig>();
		
        public ItemConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            ItemConfigCategory s = o as ItemConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            for(int i =0 ;i<list.Count;i++)
            {
                ItemConfig config = list[i];
                config.EndInit();
                this.dict.Add(config.Id, config);
                config.AfterEndInit();
            }            
            this.AfterEndInit();
        }
		
        public ItemConfig Get(int id)
        {
            this.dict.TryGetValue(id, out ItemConfig item);

            if (item == null)
            {
#if !NOT_UNITY
                Log.Error($"配置找不到，配置表名: {nameof (ItemConfig)}，配置id: {id}");
#endif
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, ItemConfig> GetAll()
        {
            return this.dict;
        }
        public List<ItemConfig> GetAllList()
        {
            return this.list;
        }
        public ItemConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [NinoSerialize]
	public partial class ItemConfig: ProtoObject
	{
		/// <summary>ID</summary>
		[NinoMember(1)]
		public int Id { get; set; }
		/// <summary>所属集装箱</summary>
		[NinoMember(2)]
		public int ContainerId { get; set; }
		/// <summary>物品名称</summary>
		[NinoMember(3)]
		public string Chinese { get; set; }
		/// <summary>物品名称</summary>
		[NinoMember(4)]
		public string English { get; set; }
		/// <summary>物品介绍</summary>
		[NinoMember(5)]
		public string ChineseDesc { get; set; }
		/// <summary>物品介绍</summary>
		[NinoMember(6)]
		public string EnglishDesc { get; set; }
		/// <summary>售价</summary>
		[NinoMember(7)]
		public long Price { get; set; }
		/// <summary>物品图片</summary>
		[NinoMember(8)]
		public string ItemPic { get; set; }
		/// <summary>玩法类型</summary>
		[NinoMember(9)]
		public int Type { get; set; }
		/// <summary>箱子模型Id</summary>
		[NinoMember(10)]
		public int UnitId { get; set; }
		/// <summary>可能出现的剧情</summary>
		[NinoMember(11)]
		public int[] StoryIds { get; set; }

	}
}
