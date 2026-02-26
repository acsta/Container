using System.Collections.Generic;
namespace TaoTie
{
    public class NumericType
    {
        public static int GetKey(string key)
        {
            if (Map.TryGetValue(key, out var res))
            {
                return res;
            }
            Log.Error($"{key}属性不存在");
            return -1;
        }
        private static Dictionary<string, int> map;
        public static Dictionary<string, int> Map
        {
            get
            {
                if (map == null)
                {
                    map = new Dictionary<string, int>();
                }
                return map;
            }
        }
		public const int Max = 10000;
    }
}
