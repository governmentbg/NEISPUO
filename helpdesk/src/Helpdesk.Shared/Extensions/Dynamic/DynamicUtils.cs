using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Helpdesk.Shared.Extensions.Dynamic
{

    public static class DynamicUtils
    {
        /// <summary>
        /// Проверява дали дадено свойство съществува в даден dynamic обект.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        public static bool PropertyExists(dynamic obj, string propName)
        {
            if (propName.IsNullOrWhiteSpace()) return false;

            if (obj is ExpandoObject)
            {
                var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                foreach (var kvp in (IDictionary<string, object>)obj)
                {
                    if (dict.ContainsKey(kvp.Key)) continue;

                    dict.Add(kvp.Key, kvp.Value);
                }

                return dict.ContainsKey(propName);
            }


            if (obj is JObject @object)
            {
                return @object.TryGetValue(propName, StringComparison.OrdinalIgnoreCase, out _);
            }

            return obj.GetType().GetProperty(propName) != null;
        }

        /// <summary>
        /// TrtGetValue and return as string. Return null if property does not exist.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        public static object GetPropertyValue(dynamic obj, string propName)
        {
            if (propName.IsNullOrWhiteSpace()) return null;

            if (obj is ExpandoObject)
            {
                var dict = (IDictionary<string, object>)obj;
                if (dict.TryGetValue(propName, out object val))
                {
                    return val;
                }
                else
                {
                    return null;
                }
            }

            if (obj is JObject @object)
            {
                if (@object.TryGetValue(propName, StringComparison.OrdinalIgnoreCase, out JToken token))
                {
                    if (token.IsNullOrEmpty())
                    {
                        return null;
                    }
                    else
                    {
                        return token.Value<object>();
                    }
                }
                else
                {
                    return null;
                }
            }

            var propertyInfo = obj.GetType().GetProperty(propName);

            return propertyInfo?.GetValue(obj, null);
        }

        public static bool IsNullOrEmpty(this JToken token)
        {
            return (token == null) ||
                   (token.Type == JTokenType.Array && !token.HasValues) ||
                   (token.Type == JTokenType.Object && !token.HasValues) ||
                   (token.Type == JTokenType.String && token.ToString() == string.Empty) ||
                   (token.Type == JTokenType.Null);
        }
    }
}
