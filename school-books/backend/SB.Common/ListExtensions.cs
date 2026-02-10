namespace SB.Common;

using System;
using System.Collections.Generic;

public static class ListExtensions
{
    public static IEnumerable<T> RemoveAllMatching<T>(this List<T> list, Predicate<T> match)
    {
        for (var i = list.Count - 1; i >= 0; i--)
        {
            var item = list[i];

            if (match(item))
            {
                list.RemoveAt(i);
                yield return item;
            }
        }
    }
}
