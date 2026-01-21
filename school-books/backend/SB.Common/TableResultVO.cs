namespace SB.Common;

using System;

public static class TableResultVO
{
    private static class EmptyTableResultVO<T>
    {
        internal static readonly TableResultVO<T> Value = new(Array.Empty<T>(), 0);
    }

    public static TableResultVO<T> Empty<T>() => EmptyTableResultVO<T>.Value;
}

public record TableResultVO<T>(T[] Result, int Length);
