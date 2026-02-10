namespace SB.Domain;

public record ScheduleLessonNomVO(
    int ScheduleLessonId,
    int? TeacherAbsenceId,
    int? IndividualCurriculumPersonId,
    string Name,
    bool IsVerified);
