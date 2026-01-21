namespace SB.Domain.Tests;

using System;
using System.Collections.Generic;

#pragma warning disable CA1000 // Do not declare static members on generic types

// see https://stackoverflow.com/a/6913646/682203
public static class KeyEqualityComparer<T> where T : class
{
    public static IEqualityComparer<T> Create<TKey>(Func<T, TKey> keySelector) where TKey : notnull
    {
        return Create(keySelector, EqualityComparer<TKey>.Default);
    }

    public static IEqualityComparer<T> Create<TKey>(
        Func<T, TKey> keySelector,
        IEqualityComparer<TKey> comparer)
        where TKey : notnull
    {
        return new KeyEqualityComparerHelper<TKey>(keySelector, comparer);
    }

    class KeyEqualityComparerHelper<TKey> : IEqualityComparer<T> where TKey : notnull
    {
        readonly Func<T, TKey> keySelector;
        readonly IEqualityComparer<TKey> comparer;

        public KeyEqualityComparerHelper(
            Func<T, TKey> keySelector,
            IEqualityComparer<TKey> comparer)
        {
            this.keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
            this.comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
        }

        public bool Equals(T? x, T? y)
        {
            if (x == null || y == null)
            {
                if(x == y)
                {
                    return true;
                }

                return false;
            }

            return this.comparer.Equals(this.keySelector(x), this.keySelector(y));
        }

        public int GetHashCode(T obj)
        {
            return this.comparer.GetHashCode(this.keySelector(obj));
        }
    }
}
