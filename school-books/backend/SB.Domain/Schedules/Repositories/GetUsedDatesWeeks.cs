namespace SB.Domain;

using System;

public partial interface ISchedulesQueryRepository
{
    public record GetUsedDatesWeeksVO(
        DateTime[] Dates,
        GetUsedDatesWeeksVOWeek[] Weeks);

    public record GetUsedDatesWeeksVOWeek(
        int Year,
        int WeekNumber,
        bool IsPartiallyUsed);
}
