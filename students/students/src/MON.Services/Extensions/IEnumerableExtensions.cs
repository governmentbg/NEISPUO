namespace MON.Services.Extensions
{
    using MON.Shared.Enums;
    using MON.Shared.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class IEnumerableExtensions
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static List<T> FilterByValid<T>(this List<T> allEntries, ValidEnum valid) where T: IValidable
        {
            return allEntries.Where(i => valid switch
            {
                ValidEnum.True => i.IsValid,
                ValidEnum.False => !i.IsValid,
                ValidEnum.All => true,
                _ => true
            }).ToList();
        }
    }
}
