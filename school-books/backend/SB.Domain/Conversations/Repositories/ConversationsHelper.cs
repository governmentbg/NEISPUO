namespace SB.Domain;

using System;

public static class ConversationsHelper
{
    public static int CurrentSchoolYear()
    {
        var now = DateTime.Now;
        return now.Month < 9 ? now.Year - 1 : now.Year;
    }

    public static int[] SchoolYearsRange()
    {
        var now = DateTime.Now;
        return now.Month < 9 ? new[] { now.Year - 1, now.Year - 2 } : new[] { now.Year, now.Year - 1 };
    }
}
