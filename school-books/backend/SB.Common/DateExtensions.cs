namespace SB.Common;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

public static partial class DateExtensions
{
    [GeneratedRegex("^([0-9]|0[0-9]|1[0-9]|2[0-3]):([0-5][0-9])$")]
    public static partial Regex HourRegex();

    // https://stackoverflow.com/a/9064954/2248587
    public static DateTime GetDateFromIsoWeek(int year, int weekNumber, int day)
    {
        if (weekNumber < 1 || weekNumber > 53)
        {
            throw new Exception("Invalid weekNumber");
        }

        if (day < 1 || day > 7)
        {
            throw new Exception("Invalid day");
        }

        DateTime jan1 = new(year, 1, 1);
        int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

        // Use first Thursday in January to get first week of the year as
        // it will never be in Week 52/53
        DateTime firstThursday = jan1.AddDays(daysOffset);
        var cal = CultureInfo.CurrentCulture.Calendar;
        int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

        var weekNum = weekNumber;
        // As we're adding days to a date in Week 1,
        // we need to subtract 1 in order to get the right date for week #1
        if (firstWeek == 1)
        {
            weekNum -= 1;
        }

        // Using the first Thursday as starting week ensures that we are starting in the right year
        // then we add number of weeks multiplied with days
        var result = firstThursday.AddDays(weekNum * 7);

        // Subtract 3 days from Thursday to get Monday, which is the first weekday in ISO8601
        result = result.AddDays(-3);

        return result.AddDays(day - 1);
    }

    // https://stackoverflow.com/a/11155102/2248587
    // This presumes that weeks start with Monday.
    // Week 1 is the 1st week of the year with a Thursday in it.
    public static int GetIsoWeek(this DateTime date)
    {
        // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll
        // be the same week# as whatever Thursday, Friday or Saturday are,
        // and we always get those right
        DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(date);
        if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
        {
            date = date.AddDays(3);
        }

        // Return the week of our adjusted day
        return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
    }

    public static int GetIsoWeekYear(this DateTime date)
    {
        if (date < GetDateFromIsoWeek(date.Year, 1, 1))
        {
            return date.Year - 1;
        }
        else if (date > GetDateFromIsoWeek(date.Year, GetIsoWeeksInYear(date.Year), 7))
        {
            return date.Year + 1;
        }
        else
        {
            return date.Year;
        }
    }

    public static int GetIsoDayOfWeek(this DateTime date)
    {
        return date.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)date.DayOfWeek;
    }

    public static int GetIsoDayOfWeek(this DayOfWeek dayOfWeek)
    {
        return dayOfWeek == DayOfWeek.Sunday ? 7 : (int)dayOfWeek;
    }

    public static int GetIsoWeeksInYear(int year)
    {
        // Dec 28 is always in the last week of the year
        return GetIsoWeek(new DateTime(year, 12, 28));
    }

    public static bool IsAfterToday(this DateTime date)
    {
        return DateTime.Now.Date.AddDays(1) <= date;
    }

    public static bool IsAfterToday(int year, int weekOfYear, int day)
    {
        return GetDateFromIsoWeek(year, weekOfYear, day).IsAfterToday();
    }

    public static IEnumerable<(int year, int month)> GetMonthsInRange(DateTime from, DateTime to)
    {
        var startDate = new DateTime(from.Year, from.Month, 1);
        var endDate = new DateTime(to.Year, to.Month, 1).AddMonths(1).AddDays(-1);

        while (startDate < endDate)
        {
            yield return (year: startDate.Year, month: startDate.Month);
            startDate = startDate.AddMonths(1);
        }
    }

    public static IEnumerable<DateTime> GetDatesInRange(DateTime from, DateTime to)
    {
        for (var date = from; date <= to; date = date.AddDays(1))
        {
            yield return date;
        }
    }

    public static IEnumerable<(int year, int weekNumber)> GetWeeksInRange(DateTime from, DateTime to)
    {
        return GetDatesInRange(from, to)
            .Select(date => (year: date.GetIsoWeekYear(), weekNumber: date.GetIsoWeek()))
            .Distinct()
            .ToArray();
    }

    public static DateTime Min(DateTime d1, DateTime d2)
    {
        return d1 <= d2 ? d1 : d2;
    }

    public static DateTime Max(DateTime d1, DateTime d2)
    {
        return d1 >= d2 ? d1 : d2;
    }

    public static string GetLocalizedDayName(this DayOfWeek dayOfWeek)
        => GetLocalizedDayName(dayOfWeek.GetIsoDayOfWeek());

    public static string GetLocalizedDayName(int day)
    {
        if (day < 1 || day > 7)
        {
            throw new Exception("Invalid day");
        }

        DateTime someDate = new(2000, 1, 1);
        DateTime someDateWithDayOfWeek = someDate.AddDays(day - someDate.GetIsoDayOfWeek());
        return someDateWithDayOfWeek.ToString("dddd");
    }

    public static long GetDueDateUnixTimeMs(TimeSpan time)
    {
        var now = DateTime.Now.Date;

        var targetDateTime = new DateTime(
        now.Year,
        now.Month,
        now.Day,
        time.Hours,
        time.Minutes,
        time.Seconds,
        DateTimeKind.Local);

        // Convert to Unix time in milliseconds
        var unixTimeMs = new DateTimeOffset(targetDateTime).ToUnixTimeMilliseconds();
        return unixTimeMs;
    }
}
