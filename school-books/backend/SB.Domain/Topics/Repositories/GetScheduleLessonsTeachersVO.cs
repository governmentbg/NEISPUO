namespace SB.Domain;

public partial interface ITopicsQueryRepository
{
    public record GetScheduleLessonsTeachersVO(
        int ScheduleLessonId,
        int PersonId,
        bool IsReplTeacher);
}
