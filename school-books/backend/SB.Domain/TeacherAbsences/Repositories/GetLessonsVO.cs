namespace SB.Domain;

using System;

public partial interface ITeacherAbsencesQueryRepository
{
    public record GetLessonsVO
    {
        public int ClassBookId { get; init; }
        public int ScheduleLessonId { get; init; }
        public DateTime Date { get; init; }
        public required int[] CurriculumTeacherPersonIds { get; init; }
        public int? TeacherAbsenceId { get; init; }
    }
}
