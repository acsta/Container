using System;
using System.Collections.Generic;
using Nino.Serialization;

namespace TaoTie
{
    [NinoSerialize]
    [Config]
    public partial class ContainerConfigCategory : ProtoObject, IMerge
    {
        public static ContainerConfigCategory Instance;
		
        
        [NinoIgnore]
        private Dictionary<int, ContainerConfig> dict = new Dictionary<int, ContainerConfig>();
        
        [NinoMember(1)]
        private List<ContainerConfig> list = new List<ContainerConfig>();
		
        public ContainerConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            ContainerConfigCategory s = o as ContainerConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            for(int i =0 ;i<list.Count;i++)
            {
                ContainerConfig config = list[i];
                config.EndInit();
                this.dict.Add(config.Id, config);
                config.AfterEndInit();
            }            
            this.AfterEndInit();
        }
		
        public ContainerConfig Get(int id)
        {
            this.dict.TryGetValue(id, out ContainerConfig item);

            if (item == null)
            {
#if !NOT_UNITY
                Log.Error($"配置找不到，配置表名: {nameof (ContainerConfig)}，配置id: {id}");
#endif
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, ContainerConfig> GetAll()
        {
            return this.dict;
        }
        public List<ContainerConfig> GetAllList()
        {
            return this.list;
        }
        public ContainerConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [NinoSerialize]
	public partial class ContainerConfig: ProtoObject
	{
		/// <summary>Id</summary>
		[NinoMember(1)]
		public int Id { get; set; }
		/// <summary>场次级别</summary>
		[NinoMember(2)]
		public int Level { get; set; }
		/// <summary>随机权重</summary>
		[NinoMember(3)]
		public int Weight { get; set; }
		/// <summary>盲盒类型(0全玩法，1普通通用，2餐厅，3玩具...)</summary>
		[NinoMember(4)]
		public int Type { get; set; }
		/// <summary>颜色</summary>
		[NinoMember(5)]
		public string Color { get; set; }
		/// <summary>中文名</summary>
		[NinoMember(6)]
		public string Chinese { get; set; }
		/// <summary>英文名</summary>
		[NinoMember(7)]
		public string English { get; set; }
		/// <summary>图片</summary>
		[NinoMember(8)]
		public string Icon { get; set; }
		/// <summary>集装箱皮肤</summary>
		[NinoMember(9)]
		public string Skin { get; set; }

	}
}
