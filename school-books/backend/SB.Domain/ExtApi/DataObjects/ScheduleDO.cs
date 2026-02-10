namespace SB.Domain;

using System;

/// <summary>{{ScheduleDO.Summary}}</summary>
public class ScheduleDO
{
    /// <summary>{{ScheduleDO.ScheduleId}}</summary>
    public int? ScheduleId { get; init; }

    /// <summary>{{ScheduleDO.IsIndividualCurriculum}}</summary>
    public bool IsIndividualCurriculum { get; init; }

    /// <summary>{{Common.Obsolete}}</summary>
    [Obsolete("For removal")]
    public int? ClassId => 0;

    /// <summary>{{ScheduleDO.PersonId}}</summary>
    public int? PersonId { get; init; }

    /// <summary>{{ScheduleDO.Term}}</summary>
    public SchoolTerm? Term { get; init; }

    /// <summary>{{ScheduleDO.StartDate}}</summary>
    public DateTime StartDate { get; init; }

    /// <summary>{{ScheduleDO.EndDate}}</summary>
    public DateTime EndDate { get; init; }

    // TODO deprecated, remove nullability and make it required
    /// <summary>{{ScheduleDO.IncludesWeekend}}</summary>
    public bool? IncludesWeekend { get; init; }

    // TODO deprecated, remove nullability and make it required
    /// <summary>{{ScheduleDO.HasAdhocShift}}</summary>
    public bool? HasAdhocShift { get; init; }

    /// <summary>{{ScheduleDO.AdhocShiftIsMultiday}}</summary>
    public bool? AdhocShiftIsMultiday { get; init; }

    /// <summary>{{ScheduleDO.AdhocShiftHours}}</summary>
    public ScheduleAdhocShiftHourDO[]? AdhocShiftHours { get; init; }

    /// <summary>{{ScheduleDO.ShiftId}}</summary>
    public int? ShiftId { get; init; }

    /// <summary>{{ScheduleDO.Weeks}}</summary>
    public ScheduleWeekDO[] Weeks { get; init; } = Array.Empty<ScheduleWeekDO>();

    /// <summary>{{ScheduleDO.Days}}</summary>
    public ScheduleDayDO[] Days { get; init; } = Array.Empty<ScheduleDayDO>();

    /// <summary>{{ScheduleDO.Lessons}}</summary>
    public ScheduleLessonDO[]? Lessons { get; init; } = Array.Empty<ScheduleLessonDO>();
}

/// <summary>{{ScheduleWeekDO.Summary}}</summary>
public class ScheduleWeekDO
{
    /// <summary>{{ScheduleWeekDO.Year}}</summary>
    public int Year { get; init; }

    /// <summary>{{ScheduleWeekDO.WeekNumber}}</summary>
    public int WeekNumber { get; init; }
}

/// <summary>{{ScheduleDayDO.Summary}}</summary>
public class ScheduleDayDO
{
    /// <summary>{{ScheduleDayDO.Day}}</summary>
    public int Day { get; init; }

    /// <summary>{{ScheduleDayDO.Hours}}</summary>
    public ScheduleHourDO[] Hours { get; init; } = Array.Empty<ScheduleHourDO>();
}

/// <summary>{{ScheduleHourDO.Summary}}</summary>
public class ScheduleHourDO
{
    /// <summary>{{ScheduleHourDO.HourNumber}}</summary>
    public int HourNumber { get; init; }

    /// <summary>{{Common.Obsolete}}</summary>
    [Obsolete("For removal")]
    public int[]? GroupCurriculumIds { get; init; } = Array.Empty<int>();

    /// <summary>{{ScheduleHourDO.Groups}}</summary>
    public ScheduleHourGroupDO[]? Groups { get; init; } = Array.Empty<ScheduleHourGroupDO>();
}

/// <summary>{{ScheduleHourGroupDO.Summary}}</summary>
public record ScheduleHourGroupDO
{
    /// <summary>{{ScheduleHourGroupDO.CurriculumId}}</summary>
    public int? CurriculumId { get; init; }

    /// <summary>{{ScheduleHourGroupDO.Location}}</summary>
    public string? Location { get; init; }
}

/// <summary>{{ScheduleLessonDO.Summary}}</summary>
public class ScheduleLessonDO
{
    /// <summary>{{ScheduleLessonDO.ScheduleLessonId}}</summary>
    public int ScheduleLessonId { get; init; }

    /// <summary>{{ScheduleLessonDO.Date}}</summary>
    public DateTime Date { get; init; }

    /// <summary>{{ScheduleLessonDO.Day}}</summary>
    public int Day { get; init; }

    /// <summary>{{ScheduleLessonDO.HourNumber}}</summary>
    public int HourNumber { get; init; }

    /// <summary>{{ScheduleLessonDO.CurriculumId}}</summary>
    public int CurriculumId { get; init; }
}

/// <summary>{{ScheduleResultDO.Summary}}</summary>
public class ScheduleResultDO
{
    /// <summary>{{ScheduleResultDO.ScheduleId}}</summary>
    public int ScheduleId { get; init; }

    /// <summary>{{ScheduleResultDO.Lessons}}</summary>
    public ScheduleLessonDO[] Lessons { get; init; } = Array.Empty<ScheduleLessonDO>();
}

/// <summary>{{ScheduleAdhocShiftHourDO.Summary}}</summary>
public record ScheduleAdhocShiftHourDO
{
    /// <summary>{{ScheduleAdhocShiftHourDO.Day}}</summary>
    public int Day { get; init; }

    /// <summary>{{ScheduleAdhocShiftHourDO.HourNumber}}</summary>
    public int HourNumber { get; init; }

    /// <summary>{{ScheduleAdhocShiftHourDO.StartTime}}</summary>
    public required string StartTime { get; init; }

    /// <summary>{{ScheduleAdhocShiftHourDO.EndTime}}</summary>
    public required string EndTime { get; init; }
}
