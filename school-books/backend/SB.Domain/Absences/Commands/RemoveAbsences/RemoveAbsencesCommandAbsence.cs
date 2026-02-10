namespace SB.Domain;

public record RemoveAbsencesCommandAbsence(
    int? ScheduleLessonId,
    int? AbsenceId);
