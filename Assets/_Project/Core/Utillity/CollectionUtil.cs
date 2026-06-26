using System;
using System.Collections.Generic;

namespace ProjectATLAS.Utility
{
    public static class CollectionUtils
    {
        /// <summary> Clones an array. </summary>
        public static T[] CloneArray<T>(this T[] array)
        {
            return (T[])array.Clone();
        }
        
        /// <summary> Shuffles the reference to the array. </summary>
        public static T[] Shuffle<T>(this T[] array)
        {
            Random random = new();
            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (array[j], array[i]) = (array[i], array[j]);
            }
            return array;
        }
        
        /// <summary> Shuffles the reference to the list. </summary>
        public static List<T> Shuffle<T>(this List<T> array)
        {
            Random random = new();
            for (int i = array.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (array[j], array[i]) = (array[i], array[j]);
            }
            return array;
        }
    }
}
