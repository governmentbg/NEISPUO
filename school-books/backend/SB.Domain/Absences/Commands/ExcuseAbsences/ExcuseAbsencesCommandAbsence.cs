namespace SB.Domain;

public record ExcuseAbsencesCommandAbsence(
    int? ScheduleLessonId,
    int? AbsenceId);
