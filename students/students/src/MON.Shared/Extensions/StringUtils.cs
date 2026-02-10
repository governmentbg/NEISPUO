namespace MON.Shared.Extensions
{
    using System.Collections.Generic;

    public static class StringUtils
    {
        public static HashSet<T> ToHashSet<T>(this IEnumerable<string> arr) where T: struct
        {
            HashSet<T> result = new HashSet<T>();
            if (arr.IsNullOrEmpty()) return result;

            foreach (var item in arr)
            {
                T? parseResult = item.TryParse<T>();
                if (parseResult != null)
                {
                    result.Add(parseResult.Value);
                }
            }

            return result;
        }


        public static HashSet<T> ToHashSet<T>(this string strArr, char separator) where T : struct
        {
            if (strArr.IsNullOrEmpty())
            {
                return new HashSet<T>();
            }


            return strArr.Split(separator, System.StringSplitOptions.RemoveEmptyEntries).ToHashSet<T>();          
        }
    }
}
