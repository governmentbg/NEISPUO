namespace SB.Domain;

using System;
using System.Text.Json.Serialization;

public record CreateScheduleCommand : IAuditedCreateCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? ClassBookId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }

    public bool? IsIndividualSchedule { get; init; }

    public int? PersonId { get; init; }

    public SchoolTerm? Term { get; init; }

    public DateTime? StartDate { get; init; }

    public DateTime? EndDate { get; init; }

    public bool? IncludesWeekend { get; init; }

    public bool? HasAdhocShift { get; init; }

    public int? ShiftId { get; init; }

    public bool? AdhocShiftIsMultiday { get; init; }

    public CreateScheduleCommandShiftDay[]? AdhocShiftDays { get; init; }

    public ScheduleCommandWeek[]? Weeks { get; init; }

    public CreateScheduleCommandDay[]? Days { get; init; }

    [JsonIgnore]public string ObjectName => nameof(Schedule);
    [JsonIgnore]public virtual int? ObjectId => null;
}

public record CreateScheduleCommandShiftDay
{
    public int? Day { get; init; }

    public CreateScheduleCommandShiftHour[]? Hours { get; init; } = Array.Empty<CreateScheduleCommandShiftHour>();
}

public record CreateScheduleCommandShiftHour
{
    public int? HourNumber { get; init; }

    public string? StartTime { get; init; }

    public string? EndTime { get; init; }
}

public record CreateScheduleCommandDay
{
    public int Day { get; init; }

    public CreateScheduleCommandDayHour[] Hours { get; init; } = Array.Empty<CreateScheduleCommandDayHour>();
}

public record CreateScheduleCommandDayHour
{
    public int HourNumber { get; init; }

    public CreateScheduleCommandDayHourGroup[] Groups { get; init; } = Array.Empty<CreateScheduleCommandDayHourGroup>();
}

public record CreateScheduleCommandDayHourGroup
{
    public int? CurriculumId { get; init; }
    public string? Location { get; init; }
}
