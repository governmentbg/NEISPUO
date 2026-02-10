namespace SB.Domain;

using System;
using System.Collections.Generic;

public class LectureSchedule : IAggregateRoot
{
    // EF constructor
    private LectureSchedule()
    {
        this.OrderNumber = null!;
    }

    public LectureSchedule(
        int schoolYear,
        int instId,
        int teacherPersonId,
        string orderNumber,
        DateTime orderDate,
        DateTime startDate,
        DateTime endDate,
        int[] scheduleLessonIds,
        int createdBySysUserId)
    {
        this.SchoolYear = schoolYear;
        this.InstId = instId;
        this.TeacherPersonId = teacherPersonId;
        this.OrderNumber = orderNumber;
        this.OrderDate = orderDate;
        this.StartDate = startDate;
        this.EndDate = endDate;

        this.SetHours(scheduleLessonIds);

        var now = DateTime.Now;
        this.CreateDate = now;
        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifyDate = now;
        this.ModifiedBySysUserId = createdBySysUserId;
    }

    public int SchoolYear { get; private set; }

    public int LectureScheduleId { get; private set; }

    public int InstId { get; private set; }

    public int TeacherPersonId { get; private set; }

    public string OrderNumber { get; private set; }

    public DateTime OrderDate { get; private set; }

    public DateTime StartDate { get; private set; }

    public DateTime EndDate { get; private set; }

    public DateTime CreateDate { get; private set; }

    public int CreatedBySysUserId { get; private set; }

    public DateTime ModifyDate { get; internal set; }

    public int ModifiedBySysUserId { get; private set; }

    public byte[] Version { get; private set; } = null!;

    // relations
    private readonly List<LectureScheduleHour> hours = new();
    public IReadOnlyCollection<LectureScheduleHour> Hours => this.hours.AsReadOnly();

    internal void UpdateData(
        string orderNumber,
        DateTime orderDate,
        int[] scheduleLessonIds,
        int modifiedBySysUserId)
    {
        this.OrderNumber = orderNumber;
        this.OrderDate = orderDate;

        this.SetHours(scheduleLessonIds);

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;
    }

    private void SetHours(int[] scheduleLessonIds)
    {
        if (scheduleLessonIds == null)
        {
            throw new ArgumentNullException(nameof(scheduleLessonIds));
        }

        if (scheduleLessonIds.Length == 0)
        {
            throw new ArgumentException("At least one hour should be added to the array", nameof(scheduleLessonIds));
        }

        this.hours.Clear();

        foreach (var scheduleLessonId in scheduleLessonIds)
        {
            this.hours.Add(new LectureScheduleHour(
                this,
                scheduleLessonId));
        }
    }
}
