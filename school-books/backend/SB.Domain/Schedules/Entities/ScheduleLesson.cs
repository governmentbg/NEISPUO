namespace SB.Domain;

using System;

public class ScheduleLesson
{
    // EF constructor
    protected ScheduleLesson()
    {
        this.Schedule = null!;
    }

    internal ScheduleLesson(
        Schedule schedule,
        DateTime date,
        int day,
        int hourNumber,
        int curriculumId)
    {
        this.Schedule = schedule;
        this.Date = date;
        this.Day = day;
        this.HourNumber = hourNumber;
        this.CurriculumId = curriculumId;
        this.IsVerified = false;
    }

    public int SchoolYear { get; private set; }

    public int ScheduleLessonId { get; private set; }

    public int ScheduleId { get; private set; }

    public DateTime Date { get; private set; }

    public int Day { get; private set; }

    public int HourNumber { get; private set; }

    public int CurriculumId { get; private set; }

    public bool IsVerified { get; private set; }

    public DateTime? VerifyDate { get; private set; }

    public int? VerifiedBySysUserId { get; private set; }

    // relations
    public Schedule Schedule { get; private set; }

    public void UpdateIsVerified(
        bool isVerified,
        int verifiedBySysUserId)
    {
        this.IsVerified = isVerified;

        this.VerifyDate = DateTime.Now;
        this.VerifiedBySysUserId = verifiedBySysUserId;
    }
}
