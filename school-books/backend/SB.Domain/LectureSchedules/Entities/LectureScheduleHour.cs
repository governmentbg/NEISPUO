namespace SB.Domain;

public class LectureScheduleHour
{
    // EF constructor
    private LectureScheduleHour()
    {
        this.LectureSchedule = null!;
    }

    public LectureScheduleHour(
        LectureSchedule lectureSchedule,
        int scheduleLessonId)
    {
        this.LectureSchedule = lectureSchedule;
        this.ScheduleLessonId = scheduleLessonId;
    }

    public int SchoolYear { get; private set; }

    public int LectureScheduleId { get; private set; }

    public int ScheduleLessonId { get; private set; }

    // relations
    public LectureSchedule LectureSchedule { get; private set; }
}
