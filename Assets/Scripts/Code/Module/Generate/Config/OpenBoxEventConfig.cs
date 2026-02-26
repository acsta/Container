using System;
using System.Collections.Generic;
using Nino.Serialization;

namespace TaoTie
{
    [NinoSerialize]
    [Config]
    public partial class OpenBoxEventConfigCategory : ProtoObject, IMerge
    {
        public static OpenBoxEventConfigCategory Instance;
		
        
        [NinoIgnore]
        private Dictionary<int, OpenBoxEventConfig> dict = new Dictionary<int, OpenBoxEventConfig>();
        
        [NinoMember(1)]
        private List<OpenBoxEventConfig> list = new List<OpenBoxEventConfig>();
		
        public OpenBoxEventConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            OpenBoxEventConfigCategory s = o as OpenBoxEventConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            for(int i =0 ;i<list.Count;i++)
            {
                OpenBoxEventConfig config = list[i];
                config.EndInit();
                this.dict.Add(config.Id, config);
                config.AfterEndInit();
            }            
            this.AfterEndInit();
        }
		
        public OpenBoxEventConfig Get(int id)
        {
            this.dict.TryGetValue(id, out OpenBoxEventConfig item);

            if (item == null)
            {
#if !NOT_UNITY
                Log.Error($"配置找不到，配置表名: {nameof (OpenBoxEventConfig)}，配置id: {id}");
#endif
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, OpenBoxEventConfig> GetAll()
        {
            return this.dict;
        }
        public List<OpenBoxEventConfig> GetAllList()
        {
            return this.list;
        }
        public OpenBoxEventConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [NinoSerialize]
	public partial class OpenBoxEventConfig: ProtoObject
	{
		/// <summary>Id</summary>
		[NinoMember(1)]
		public int Id { get; set; }
		/// <summary>事件类型(1节日礼物，2出现动物，3出现撞人动物)</summary>
		[NinoMember(2)]
		public int Type { get; set; }
		/// <summary>Npc可生效</summary>
		[NinoMember(3)]
		public int Npc { get; set; }
		/// <summary>权重</summary>
		[NinoMember(4)]
		public int Weight { get; set; }
		/// <summary>模型Id</summary>
		[NinoMember(5)]
		public int UnitId { get; set; }
		/// <summary>存在时间</summary>
		[NinoMember(6)]
		public int During { get; set; }

	}
}
