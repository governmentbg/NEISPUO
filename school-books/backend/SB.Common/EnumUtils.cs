namespace SB.Common;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

public static class EnumUtils
{
    private static readonly ConcurrentDictionary<Type, Dictionary<Enum, string>> Cache = new ConcurrentDictionary<Type, Dictionary<Enum, string>>();

    public static T ParseEnum<T>(string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }

    [return: NotNullIfNotNull("element")]
    public static string? GetEnumDescription(this Enum? element)
    {
        return element == null ? null : GetEnumDescriptionInternal(element);
    }

    [return: NotNullIfNotNull("element")]
    public static string? GetFormattedEnumDescription(this Enum? element, params object[] args)
    {
        if (element == null)
        {
            return null;
        }

        var description = GetEnumDescriptionInternal(element);
        return string.Format(description, args);
    }

    private static string GetEnumDescriptionInternal(Enum element)
    {
        var type = element.GetType();

        if (!Cache.TryGetValue(type, out var cachedDescriptions))
        {
            cachedDescriptions = new Dictionary<Enum, string>();
            foreach (Enum enumValue in Enum.GetValues(type))
            {
                var fi = type.GetField(enumValue.ToString())!;
                var attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

                var description = enumValue.ToString();
                foreach (var attribute in attributes)
                {
                    if (attribute is DescriptionAttribute descriptionAttribute)
                    {
                        description = descriptionAttribute.Description;
                        break;
                    }
                }

                cachedDescriptions.Add(enumValue, description);
            }

            Cache.TryAdd(type, cachedDescriptions);
        }

        return cachedDescriptions[element];
    }
}
