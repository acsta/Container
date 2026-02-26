using System;
using System.Reflection;

namespace TaoTie
{
    public partial class EquipGroupConfigCategory
    {
        private const int Len = 3;
        private PropertyInfo[] Counts;
        private PropertyInfo[] EffectTypes;
        private PropertyInfo[] Params;
        public override void AfterEndInit()
        {
            base.AfterEndInit();
            var type = TypeInfo<EquipGroupConfig>.Type;
            Counts = new PropertyInfo[Len];
            EffectTypes = new PropertyInfo[Len];
            Params = new PropertyInfo[Len];
            for (int i = 0; i < Len; i++)
            {
                Counts[i] = type.GetProperty("Count" + i);
                EffectTypes[i] = type.GetProperty("EffectType" + i);
                Params[i] = type.GetProperty("Param" + i);
            }
        }

        public bool TryGet(int id, out EquipGroupConfig config)
        {
            if (!this.dict.TryGetValue(id, out config))
            {
                return false;
            }

            if (config.Count == null)
            {
                for (int i = Len - 1; i >=0 ; i--)
                {
                    int count = (int)Counts[i].GetValue(config);
                    if (count == 0) continue;
                    int type = (int)EffectTypes[i].GetValue(config);
                    if (type == 0) continue;
                    if (config.Count == null)
                    {
                        config.Count = new int[i + 1];
                        config.EffectType = new int[i + 1];
                        config.Param = new int[i + 1];
                    }
                    config.Count[i] = count;
                    config.EffectType[i] = type;
                    config.Param[i] = (int)Params[i].GetValue(config);
                }
                
                if (config.Count == null)
                {
                    config.Count = Array.Empty<int>();
                    config.EffectType = Array.Empty<int>();
                    config.Param = Array.Empty<int>();
                }
            }

            return true;
        }
    }
    
    public partial class EquipGroupConfig
    {
        public int[] Count;
        public int[] EffectType;
        public int[] Param;
    }
}