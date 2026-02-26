using System;
using System.Collections.Generic;
using Nino.Serialization;

namespace TaoTie
{
    [NinoSerialize]
    [Config]
    public partial class RestaurantConfigCategory : ProtoObject, IMerge
    {
        public static RestaurantConfigCategory Instance;
		
        
        [NinoIgnore]
        private Dictionary<int, RestaurantConfig> dict = new Dictionary<int, RestaurantConfig>();
        
        [NinoMember(1)]
        private List<RestaurantConfig> list = new List<RestaurantConfig>();
		
        public RestaurantConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            RestaurantConfigCategory s = o as RestaurantConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            for(int i =0 ;i<list.Count;i++)
            {
                RestaurantConfig config = list[i];
                config.EndInit();
                this.dict.Add(config.Id, config);
                config.AfterEndInit();
            }            
            this.AfterEndInit();
        }
		
        public RestaurantConfig Get(int id)
        {
            this.dict.TryGetValue(id, out RestaurantConfig item);

            if (item == null)
            {
#if !NOT_UNITY
                Log.Error($"配置找不到，配置表名: {nameof (RestaurantConfig)}，配置id: {id}");
#endif
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, RestaurantConfig> GetAll()
        {
            return this.dict;
        }
        public List<RestaurantConfig> GetAllList()
        {
            return this.list;
        }
        public RestaurantConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [NinoSerialize]
	public partial class RestaurantConfig: ProtoObject
	{
		/// <summary>Id</summary>
		[NinoMember(1)]
		public int Id { get; set; }
		/// <summary>开放拍卖场Id</summary>
		[NinoMember(2)]
		public int MaxLevelId { get; set; }
		/// <summary>等级</summary>
		[NinoMember(3)]
		public int Level { get; set; }
		/// <summary>基础金币产出量</summary>
		[NinoMember(4)]
		public long CashRestaurant { get; set; }
		/// <summary>基础金钱存储时限（小时）</summary>
		[NinoMember(5)]
		public int MaxHourLimitRestaurant { get; set; }
		/// <summary>每个盘子收益</summary>
		[NinoMember(6)]
		public long WashRewards { get; set; }
		/// <summary>升到下一级花费</summary>
		[NinoMember(7)]
		public long Cost { get; set; }
		/// <summary>展示内容</summary>
		[NinoMember(8)]
		public string Chinese { get; set; }
		/// <summary>展示内容</summary>
		[NinoMember(9)]
		public string English { get; set; }
		/// <summary>面板显示任务数</summary>
		[NinoMember(10)]
		public int ShowMax { get; set; }
		/// <summary>每日可免费刷新次数上限</summary>
		[NinoMember(11)]
		public int RefreshMax { get; set; }
		/// <summary>每日可付费刷新次数上限（-1无上限）</summary>
		[NinoMember(12)]
		public int PayRefreshMax { get; set; }
		/// <summary>超市是否解锁</summary>
		[NinoMember(13)]
		public int UnlockMarket { get; set; }
		/// <summary>服装店是否解锁</summary>
		[NinoMember(14)]
		public int UnlockCloth { get; set; }
		/// <summary>黑市是否解锁</summary>
		[NinoMember(15)]
		public int UnlockBlack { get; set; }
		/// <summary>今日盈利需求(0未开放入口)</summary>
		[NinoMember(16)]
		public int Need { get; set; }
		/// <summary>今日盈利奖励</summary>
		[NinoMember(17)]
		public int RewardsType { get; set; }
		/// <summary>今日盈利奖励</summary>
		[NinoMember(18)]
		public int RewardsCount { get; set; }

	}
}
