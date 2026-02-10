namespace SB.Domain;

using System;

public record CreateAbsenceCommandAbsence
{
    public int? PersonId { get; init; }
    public AbsenceType? Type { get; init; }
    public DateTime? Date { get; init; }
    public int? ScheduleLessonId { get; init; }
    public int? TeacherAbsenceId { get; init; }
    public int? ConvertToLateId { get; init; }
    public int? UndoAbsenceId { get; init; }
}
