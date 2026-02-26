using System;
using System.Collections.Generic;

namespace TaoTie
{
    public static class RangeHelper
    {
        private static Random random = new Random(DateTime.Now.Millisecond);
        
        public static void RandomSort<T>(this List<T> list) 
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                var j = random.Next(0, i);
                (list[j], list[i]) = (list[i], list[j]);
            }
        }
        public static void RandomSort<T>(this T[] list) 
        {
            for (int i = list.Length - 1; i > 0; i--)
            {
                var j = random.Next(0, i);
                (list[j], list[i]) = (list[i], list[j]);
            }
        }
    }
}