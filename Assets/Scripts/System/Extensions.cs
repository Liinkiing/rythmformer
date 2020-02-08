using System;
using System.Collections.Generic;

namespace Rythmformer
{
    public static class Extensions
    {
        public static T Random<T>(this List<T> list)
        {
            var rng = new Random();
            
            return list[rng.Next(list.Count)];
        }
        
        public static T Random<T>(this List<T> list, int seed)
        {
            var rng = new Random(seed);
            
            return list[rng.Next(list.Count)];
        }
    }
}