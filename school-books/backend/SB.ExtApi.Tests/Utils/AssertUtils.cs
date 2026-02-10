namespace SB.ExtApi.Tests;

using System.Collections.Generic;
using Xunit.Sdk;

public static class AssertUtils
{
    public static void SetsEqualWithoutDuplicates<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        => SetsEqualWithoutDuplicates(expected, actual, EqualityComparer<T>.Default);

    public static void SetsEqualWithoutDuplicates<T>(IEnumerable<T> expected, IEnumerable<T> actual, IEqualityComparer<T> comparer)
    {
        if (!expected.SetsEqualWithoutDuplicates(actual, comparer, out SetDifference<T> difference))
        {
            if (difference.Type == SetDifferenceType.Duplicates)
            {
                throw new AssertActualExpectedException(
                    difference.FirstSetExcessOrDuplicates,
                    difference.SecondSetExcessOrDuplicates,
                    "Sets have duplicates",
                    "Expected set duplicates",
                    "Actuall set duplicates");
            }
            if (difference.Type == SetDifferenceType.Different)
            {
                throw new AssertActualExpectedException(
                    difference.FirstSetExcessOrDuplicates,
                    difference.SecondSetExcessOrDuplicates,
                    "Sets differ",
                    "Expected set excess",
                    "Actuall set excess");
            }

            throw new XunitException("Oooops, incorrect SetDifferenceType");
        }
    }
}
