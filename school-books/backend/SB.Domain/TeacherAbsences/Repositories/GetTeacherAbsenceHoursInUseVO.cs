namespace SB.Domain;

using System;

public partial interface ITeacherAbsencesQueryRepository
{
    public record GetTeacherAbsenceHoursInUseVO(
        int TeacherAbsenceId,
        int ScheduleLessonId,
        int? ReplTeacherPersonId,
        bool? ReplTeacherIsNonSpecialist,
        DateTime Date,
        int Day,
        int HourNumber,
        string ClassName);
}
