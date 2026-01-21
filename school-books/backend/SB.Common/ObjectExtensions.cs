namespace SB.Common;

using System;

/// <summary>
/// Provides extension methods for objects.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Determines whether the specified object is of the same type as the current object.
    /// </summary>
    /// <param name="obj1">The current object.</param>
    /// <param name="obj2">The object to compare with the current object.</param>
    /// <returns><c>true</c> if the specified object is of the same type as the current object; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="obj2"/> is <c>null</c>.</exception>
    public static bool IsSameTypeAs(this object obj1, object? obj2)
        => obj1.IsSameTypeAs(obj2?.GetType());

    /// <summary>
    /// Determines whether the specified type is of the same type as the current object.
    /// </summary>
    /// <param name="obj1">The current object.</param>
    /// <param name="obj2Type">The type of object to compare with the current object.</param>
    /// <returns><c>true</c> if the specified type is same as the current object's type; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="obj2Type"/> is <c>null</c>.</exception>
    public static bool IsSameTypeAs(this object obj1, Type? obj2Type)
        => obj1.GetType() == (obj2Type ?? throw new ArgumentNullException(nameof(obj2Type)));
}
