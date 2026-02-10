namespace SB.ExtApi.Tests;

using System.Collections.Generic;
using System.Linq;

public enum SetDifferenceType
{
    Equal = 0,
    Duplicates,
    Different
}

public class SetDifference<T>
{
    public SetDifferenceType Type { get; set; } = SetDifferenceType.Equal;
    public IEnumerable<T> FirstSetExcessOrDuplicates { get; set; } = new List<T>();
    public IEnumerable<T> SecondSetExcessOrDuplicates { get; set; } = new List<T>();
}

public static class EnumerableExtensions
{
    public static bool SetsEqualWithoutDuplicates<T>(
        this IEnumerable<T> first,
        IEnumerable<T> second,
        IEqualityComparer<T> comparer)
    {
        return first.SetsEqualWithoutDuplicates(second, comparer, out SetDifference<T> dummy);
    }

    public static bool SetsEqualWithoutDuplicates<T>(
        this IEnumerable<T> first,
        IEnumerable<T> second,
        IEqualityComparer<T> comparer,
        out SetDifference<T> difference)
    {
        var firstSet = new HashSet<T>(comparer);
        var firstSetDuplicates = firstSet.AddRangeAndFindDuplicates(first);

        var secondSet = new HashSet<T>(comparer);
        var secondSetDuplicates = secondSet.AddRangeAndFindDuplicates(second);


        if (firstSetDuplicates.Any() || secondSetDuplicates.Any())
        {
            difference = new SetDifference<T>
            {
                Type = SetDifferenceType.Duplicates,
                FirstSetExcessOrDuplicates = firstSetDuplicates,
                SecondSetExcessOrDuplicates = secondSetDuplicates,
            };
            return false;
        }

        if (!firstSet.SetEquals(secondSet))
        {
            var firstSetExcess = new HashSet<T>(firstSet, comparer);
            firstSetExcess.ExceptWith(secondSet);

            var secondSetExcess = new HashSet<T>(secondSet, comparer);
            secondSetExcess.ExceptWith(firstSet);

            difference = new SetDifference<T>
            {
                Type = SetDifferenceType.Different,
                FirstSetExcessOrDuplicates = firstSetExcess,
                SecondSetExcessOrDuplicates = secondSetExcess,
            };
            return false;
        }

        difference = new SetDifference<T>();
        return true;
    }

    private static IEnumerable<T> AddRangeAndFindDuplicates<T>(this HashSet<T> set, IEnumerable<T> enumerable)
    {
        var duplicates = new List<T>();
        foreach (var item in enumerable)
        {
            if (!set.Add(item))
            {
                duplicates.Add(item);
            }
        }

        return duplicates;
    }
}
