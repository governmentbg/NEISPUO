namespace SB.Domain;

using System;

public partial interface ILectureSchedulesQueryRepository
{
    public record GetLessonsVO
    {
        public int ScheduleLessonId { get; init; }
        public DateTime Date { get; init; }
        public required int[] CurriculumTeacherPersonIds { get; init; }
        public int? LectureScheduleId { get; init; }
    }
}
