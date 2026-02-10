using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace MON.Shared
{
    public static class Common
    {
        /// <summary>
        /// Връща най-вътрешния Exception
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static Exception GetInnerMostException(this Exception ex)
        {
            Exception currentEx = ex;
            while (currentEx.InnerException != null)
            {
                currentEx = currentEx.InnerException;
            }

            return currentEx;
        }

        /// <summary>
        /// Връща стойността на <see cref="DescriptionAttribute"/> на дадена енумерация
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string GetEnumDescriptionAttrValue<T>(this T e) where T : IConvertible
        {
            string description = null;

            if (e is Enum)
            {
                Type type = e.GetType();
                Array values = Enum.GetValues(type);

                foreach (int val in values)
                {
                    if (val == e.ToInt32(CultureInfo.InvariantCulture))
                    {
                        var memInfo = type.GetMember(type.GetEnumName(val));
                        var descriptionAttributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                        if (descriptionAttributes.Length > 0)
                        {
                            description = ((DescriptionAttribute)descriptionAttributes[0]).Description;
                        }

                        break;
                    }
                }
            }

            return description ?? "";
        }

        public static bool HasProperty(this Type obj, string propertyName)
        {
            return obj?.GetProperty(propertyName) != null;
        }

        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;

            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static string SafeSqlStringLiteral(this string str)
        {
            return str.IsNullOrWhiteSpace()
                ? str
                : str.Replace("'", "''");
        }

        public static IEnumerable<TResult> FullOuterJoin<TSource, TInner, TKey, TResult>(this IEnumerable<TSource> source,
            IEnumerable<TInner> inner,
            Func<TSource, TKey> pk,
            Func<TInner, TKey> fk,
            Func<TSource, TInner, TResult> result) where TSource : class where TInner : class
        {

            var left = source.LeftJoin(inner, pk, fk, result).ToList();
            var right = source.RightJoin(inner, pk, fk, result).ToList();

            return left.Union(right);
        }
        public static IEnumerable<TResult> LeftJoin<TSource, TInner, TKey, TResult>(this IEnumerable<TSource> source,
            IEnumerable<TInner> inner,
            Func<TSource, TKey> pk,
            Func<TInner, TKey> fk,
            Func<TSource, TInner, TResult> result) where TSource : class where TInner : class
        {
            IEnumerable<TResult> _result = Enumerable.Empty<TResult>();

            _result = from s in source
                      join i in inner
                      on pk(s) equals fk(i) into joinData
                      from left in joinData.DefaultIfEmpty()
                      select result(s, left);

            return _result;
        }

        public static IEnumerable<TResult> RightJoin<TSource, TInner, TKey, TResult>(this IEnumerable<TSource> source,
            IEnumerable<TInner> inner,
            Func<TSource, TKey> pk,
            Func<TInner, TKey> fk,
            Func<TSource, TInner, TResult> result) where TSource : class where TInner : class
        {
            IEnumerable<TResult> _result = Enumerable.Empty<TResult>();

            _result = from i in inner
                      join s in source
                      on fk(i) equals pk(s) into joinData
                      from right in joinData.DefaultIfEmpty()
                      select result(right, i);

            return _result;
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
        {
            return collection == null || !collection.Any();
        }

        public static string ToCamelCase(this string str)
        {
            if (!str.IsNullOrWhiteSpace() && str.Length > 1)
            {
                return char.ToLowerInvariant(str[0]) + str[1..];
            }

            return str;
        }

        /// <summary>
        /// Стринг към дата.
        /// Поддържани формати:
        /// "o", "O" - ound-trip date/time pattern. 2009-06-15T13:45:30.0000000
        /// "s" - Sortable date/time pattern. 2009-06-15T13:45:30
        /// "g" - General date/time pattern (short time). 15/06/2009 13:45 (es-ES)
        /// "G" - General date/time pattern (long time). 15/06/2009 13:45:30 (es-ES)
        /// "d" - Short date pattern.  15/06/2009 (fr-FR)
        /// "yyyy-MM"
        /// https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static DateTime? ToNullableDateTime(this string val)
        {
            string[] formats = new[] { "o", "O", "s", "g", "G", "d", "yyyy-MM-dd" };

            if (DateTime.TryParseExact(val, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        public static bool? ToNullableBool(this string val)
        {
            if (bool.TryParse(val, out bool result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        public static int? ToNullableInt(this string val)
        {
            if (int.TryParse(val, out int result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        public static long? ToNullableLong(this string val)
        {
            if (long.TryParse(val, out long result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Връща учебна година според detailedSchoolType. За Спортните училища(detailedSchoolType = 114) номерацията започва от 01.09!
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="detailedSchoolType"></param>
        /// <returns></returns>
        public static short GetSchoolYearForSchoolTypeBySelectedRegDate(this DateTime dateTime, int? detailedSchoolType)
        {
            var targetDate = (detailedSchoolType ?? 0) == 114
                ? new DateTime(dateTime.Year, 9, 1)
                : new DateTime(dateTime.Year, 9, 15);

            return dateTime.Date < targetDate.Date
                ? (short)(dateTime.Year - 1)
                : (short)(dateTime.Year);
        }

        public static DateTime GetYearFromSchoolYear(int schoolYear, int month)
        {
            return new DateTime(month < 9 ? schoolYear + 1 : schoolYear, month, 1);
        }

        public static short GetSchoolYearFromYearMonth(short year, short month)
        {
            if (month <= 8)
            {
                return (short)(year - 1);
            }

            return year;
        }

        public static T? TryParse<T>(this object obj) where T : struct
        {
            if (obj == null) return null;

            T? result = null;
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter != null)
            {
                try
                {
                    string str = obj.ToString();
                    result = (T)converter.ConvertFromString(str);
                }
                catch
                {
                    // Ignore
                }
            }

            return result;
        }

        public static T ConvertFromDBVal<T>(this object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return default; // returns the default value for the type
            }
            else
            {
                return (T)obj;
            }
        }
    }
}

