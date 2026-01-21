namespace SB.Domain;

using System;

public partial interface ISchedulesQueryRepository
{
    public record GetVO(
        bool IsIndividualSchedule,
        int? PersonId,
        SchoolTerm? Term,
        DateTime StartDate,
        DateTime EndDate,
        bool IncludesWeekend,
        int ShiftId,
        GetVOWeek[] Weeks,
        GetVODay[] Days,
        DateTime[] ReadOnlyDates);

    public record GetVOWeek(
        int Year,
        int WeekNumber,
        bool IsReadOnly);

    public record GetVODay(
        int Day,
        GetVODayHour[] Hours,
        bool HasReadOnlyHours);

    public record GetVODayHour(
        int HourNumber,
        int? CurriculumId,
        bool IsReadOnly,
        string? Location);
}
