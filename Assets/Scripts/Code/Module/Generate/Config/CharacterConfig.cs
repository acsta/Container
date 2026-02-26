using System;
using System.Collections.Generic;
using Nino.Serialization;

namespace TaoTie
{
    [NinoSerialize]
    [Config]
    public partial class CharacterConfigCategory : ProtoObject, IMerge
    {
        public static CharacterConfigCategory Instance;
		
        
        [NinoIgnore]
        private Dictionary<int, CharacterConfig> dict = new Dictionary<int, CharacterConfig>();
        
        [NinoMember(1)]
        private List<CharacterConfig> list = new List<CharacterConfig>();
		
        public CharacterConfigCategory()
        {
            Instance = this;
        }
        
        public void Merge(object o)
        {
            CharacterConfigCategory s = o as CharacterConfigCategory;
            this.list.AddRange(s.list);
        }
		
        public override void EndInit()
        {
            for(int i =0 ;i<list.Count;i++)
            {
                CharacterConfig config = list[i];
                config.EndInit();
                this.dict.Add(config.Id, config);
                config.AfterEndInit();
            }            
            this.AfterEndInit();
        }
		
        public CharacterConfig Get(int id)
        {
            this.dict.TryGetValue(id, out CharacterConfig item);

            if (item == null)
            {
#if !NOT_UNITY
                Log.Error($"配置找不到，配置表名: {nameof (CharacterConfig)}，配置id: {id}");
#endif
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, CharacterConfig> GetAll()
        {
            return this.dict;
        }
        public List<CharacterConfig> GetAllList()
        {
            return this.list;
        }
        public CharacterConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [NinoSerialize]
	public partial class CharacterConfig: ProtoObject
	{
		/// <summary>ID</summary>
		[NinoMember(1)]
		public int Id { get; set; }
		/// <summary>默认衣服不为0表示该位置不可为空</summary>
		[NinoMember(2)]
		public int DefaultCloth { get; set; }
		/// <summary>中文</summary>
		[NinoMember(3)]
		public string Chinese { get; set; }
		/// <summary>英文</summary>
		[NinoMember(4)]
		public string English { get; set; }
		/// <summary>图标</summary>
		[NinoMember(5)]
		public string Icon { get; set; }

	}
}
