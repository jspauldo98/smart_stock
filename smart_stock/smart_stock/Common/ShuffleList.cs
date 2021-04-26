using System;
using System.Collections.Generic;

namespace smart_stock.Common
{
    public static class ListExtensions
    {
        public static void ShuffleList<T>(this IList<T> list)
        {
            Random random = new Random();
            for (int i = list.Count - 1; i > 1; i--)
            {
                int rnd = random.Next(i + 1);
                T value = list[rnd];
                list[rnd] = list [i];
                list[i] = value;
            }
        }
    }
}