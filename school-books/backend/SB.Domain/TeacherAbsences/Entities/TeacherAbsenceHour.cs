namespace SB.Domain;

public class TeacherAbsenceHour
{
    // EF constructor
    private TeacherAbsenceHour()
    {
        this.TeacherAbsence = null!;
    }

    public TeacherAbsenceHour(
        TeacherAbsence teacherAbsence,
        int scheduleLessonId,
        int? replTeacherPersonId,
        bool? replTeacherIsNonSpecialist,
        string? extReplTeacherName)
    {
        this.TeacherAbsence = teacherAbsence;
        this.ScheduleLessonId = scheduleLessonId;
        this.ReplTeacherPersonId = replTeacherPersonId;
        this.ReplTeacherIsNonSpecialist = replTeacherIsNonSpecialist;
        this.ExtReplTeacherName = extReplTeacherName;
    }

    public int SchoolYear { get; private set; }

    public int TeacherAbsenceId { get; private set; }

    public int ScheduleLessonId { get; private set; }

    public int? ReplTeacherPersonId { get; private set; }

    public bool? ReplTeacherIsNonSpecialist { get; private set; }

    public string? ExtReplTeacherName { get; private set; }

    // relations
    public TeacherAbsence TeacherAbsence { get; private set; }
}
