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
        
        public static float Remap (this float from, float fromMin, float fromMax, float toMin,  float toMax)
        {
            var fromAbs  =  from - fromMin;
            var fromMaxAbs = fromMax - fromMin;      
       
            var normal = fromAbs / fromMaxAbs;
 
            var toMaxAbs = toMax - toMin;
            var toAbs = toMaxAbs * normal;
 
            var to = toAbs + toMin;
       
            return to;
        }
    }
}