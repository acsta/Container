using System;
using System.Collections.Generic;
using Nino.Serialization;

namespace TaoTie
{
    [NinoSerialize]
    [Config]
    public partial class ShareConfigCategory : ProtoObject, IMerge
    {
        public static ShareConfigCategory Instance;
		
        
        [NinoIgnore]
        private Dictionary<int, ShareConfig> dict = new Dictionary<int, ShareConfig>();
        
        [NinoMember(1)]
        private List<ShareConfig> list = new List<ShareConfig>();
		
        public ShareConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            ShareConfigCategory s = o as ShareConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            for(int i =0 ;i<list.Count;i++)
            {
                ShareConfig config = list[i];
                config.EndInit();
                this.dict.Add(config.Id, config);
                config.AfterEndInit();
            }            
            this.AfterEndInit();
        }
		
        public ShareConfig Get(int id)
        {
            this.dict.TryGetValue(id, out ShareConfig item);

            if (item == null)
            {
#if !NOT_UNITY
                Log.Error($"配置找不到，配置表名: {nameof (ShareConfig)}，配置id: {id}");
#endif
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, ShareConfig> GetAll()
        {
            return this.dict;
        }
        public List<ShareConfig> GetAllList()
        {
            return this.list;
        }
        public ShareConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [NinoSerialize]
	public partial class ShareConfig: ProtoObject
	{
		/// <summary>ID</summary>
		[NinoMember(1)]
		public int Id { get; set; }
		/// <summary>文本(不填使用游戏名)</summary>
		[NinoMember(2)]
		public string Title { get; set; }
		/// <summary>内容文本(不填为空)</summary>
		[NinoMember(3)]
		public string Content { get; set; }
		/// <summary>场景(0主界面1结算界面)</summary>
		[NinoMember(4)]
		public int Scene { get; set; }
		/// <summary>图片类型（0微信网络图片，1微信专用Id，2抖音专用Id，3抖音选取本地图片，4taptap模板）</summary>
		[NinoMember(5)]
		public int Type { get; set; }
		/// <summary>值</summary>
		[NinoMember(6)]
		public string Value { get; set; }

	}
}
