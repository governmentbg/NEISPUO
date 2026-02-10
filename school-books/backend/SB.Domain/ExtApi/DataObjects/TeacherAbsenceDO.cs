namespace SB.Domain;

using System;
using System.ComponentModel.DataAnnotations;
using SB.Common;

/// <summary>{{TeacherAbsenceDO.Summary}}</summary>
public class TeacherAbsenceDO
{
    /// <summary>{{TeacherAbsenceDO.TeacherAbsenceId}}</summary>
    public int? TeacherAbsenceId { get; init; }

    /// <summary>{{TeacherAbsenceDO.TeacherPersonId}}</summary>
    public int TeacherPersonId { get; init; }

    /// <summary>{{TeacherAbsenceDO.StartDate}}</summary>
    public DateTime StartDate { get; init; }

    /// <summary>{{TeacherAbsenceDO.EndDate}}</summary>
    public DateTime EndDate { get; init; }

    /// <summary>{{TeacherAbsenceDO.Reason}}</summary>
    [MaxLength(1000)]
    public required string Reason { get; init; }

    /// <summary>{{TeacherAbsenceDO.Hours}}</summary>
    public TeacherAbsenceHourDO[] Hours { get; init; } = Array.Empty<TeacherAbsenceHourDO>();
}

/// <summary>{{TeacherAbsenceHourDO.Summary}}</summary>
public class TeacherAbsenceHourDO
{
    /// <summary>{{TeacherAbsenceHourDO.ScheduleLessonId}}</summary>
    public int ScheduleLessonId { get; init; }

    /// <summary>{{TeacherAbsenceHourDO.Type}}</summary>
    public TeacherAbsenceHourType Type { get; init; }

    /// <summary>{{TeacherAbsenceHourDO.ReplTeacherPersonId}}</summary>
    public int? ReplTeacherPersonId { get; init; }
}

/// <summary>{{TeacherAbsenceHourType.Summary}}</summary>
public enum TeacherAbsenceHourType
{
    [LocalizationKey("{{TeacherAbsenceHourType.EmptyHour}}")]
    EmptyHour = 1,

    [LocalizationKey("{{TeacherAbsenceHourType.NonSpecialist}}")]
    NonSpecialist = 2,

    [LocalizationKey("{{TeacherAbsenceHourType.Specialist}}")]
    Specialist = 3,

    [LocalizationKey("{{TeacherAbsenceHourType.ExtTeacher}}")]
    ExtTeacher = 4
}
