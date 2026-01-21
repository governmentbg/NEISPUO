namespace Helpdesk.Shared.Extensions
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;

    public static class EnumUtils
    {
        private static readonly ConcurrentDictionary<Type, Dictionary<Enum, string>> Cache = new ConcurrentDictionary<Type, Dictionary<Enum, string>>();

        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static string GetEnumDescription(this Enum element)
        {
            if (element == null)
            {
                return null;
            }

            Type type = element.GetType();

            if (!Cache.TryGetValue(type, out var cachedDescriptions))
            {
                cachedDescriptions = new Dictionary<Enum, string>();
                foreach (Enum enumValue in Enum.GetValues(type))
                {
                    FieldInfo fi = type.GetField(enumValue.ToString())!;
                    DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

                    string description;
                    if (attributes != null && attributes.Length > 0)
                    {
                        description = attributes[0].Description;
                    }
                    else
                    {
                        description = element.ToString();
                    }

                    cachedDescriptions.Add(enumValue, description);
                }

                Cache.TryAdd(type, cachedDescriptions);
            }

            return cachedDescriptions[element];
        }
    }
}
