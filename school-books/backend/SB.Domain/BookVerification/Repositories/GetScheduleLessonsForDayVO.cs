namespace SB.Domain;
public partial interface IBookVerificationQueryRepository
{
    public record GetScheduleLessonsForDayVO(int ScheduleLessonId,
        int HourNumber,
        string StartTime,
        string EndTime,
        int ClassBookId,
        string ClassBookFullName,
        ClassBookType ClassBookType,
        bool IsIndividualSchedule,
        bool IsIndividualLesson,
        bool IsIndividualCurriculum,
        string StudentFirstName,
        string? StudentMiddleName,
        string StudentLastName,
        int CurriculumId,
        string? CurriculumGroupName,
        string SubjectName,
        string? SubjectNameShort,
        string SubjectTypeName,
        bool? ReplTeacherIsNonSpecialist,
        GetScheduleLessonsForDayVOTeacher[] Teachers,
        string? ExtReplTeacherName,
        bool IsLectureScheduleHour,
        bool IsTaken,
        string[] TopicTitles,
        int AbsencesCount,
        int LateAbsencesCount,
        int GradesCount,
        bool IsVerified);

    public record GetScheduleLessonsForDayVOTeacher
    {
        public int TeacherPersonId { get; init; }
        public string TeacherFirstName { get; init; } = null!;
        public string TeacherLastName { get; init; } = null!;
        public bool IsReplTeacher { get; init; }
        public bool MarkedAsNoReplacement { get; init; }
    }
}
