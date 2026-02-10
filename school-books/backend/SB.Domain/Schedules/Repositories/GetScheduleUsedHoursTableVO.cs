namespace SB.Domain;

using System;

public partial interface ISchedulesQueryRepository
{
    public record GetScheduleUsedHoursTableVO(
        int HourNumber,
        string CurriculumName,
        DateTime[] Absences,
        DateTime[] Grades,
        DateTime[] Topics,
        GetScheduleUsedHoursTableVOTeacherAbsence[] TeacherAbsences,
        GetScheduleUsedHoursTableVOLectureSchedule[] LectureSchedules);

    public record GetScheduleUsedHoursTableVOTeacherAbsence(
        DateTime StartDate,
        DateTime EndDate,
        string TeacherPersonName);

    public record GetScheduleUsedHoursTableVOLectureSchedule(
        DateTime StartDate,
        DateTime EndDate,
        string TeacherPersonName);
}
