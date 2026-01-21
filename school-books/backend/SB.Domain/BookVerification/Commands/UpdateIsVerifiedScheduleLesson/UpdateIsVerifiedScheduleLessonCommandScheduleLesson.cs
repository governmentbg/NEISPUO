namespace SB.Domain;

public record UpdateIsVerifiedScheduleLessonCommandScheduleLesson
{
    public int? ScheduleLessonId { get; init; }
    public bool? IsVerified { get; init; }
}
