namespace SB.Domain;

public record CreateTeacherAbsenceCommandHour
{
    public int? ScheduleLessonId { get; init; }
    public int? ReplTeacherPersonId { get; init; }
    public bool? ReplTeacherIsNonSpecialist { get; init; }
    public string? ExtReplTeacherName { get; init; }
}
