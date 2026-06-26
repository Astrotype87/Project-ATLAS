using System;

namespace ProjectATLAS.Beta.Quiz
{
    public static class ArrayUtils
    {
        public static T[] Shuffle<T>(T[] array)
        {
            Random random = new();
            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (array[j], array[i]) = (array[i], array[j]);
            }
            return array;
        }
    }
}
