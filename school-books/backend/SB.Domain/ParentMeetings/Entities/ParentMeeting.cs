namespace SB.Domain;

using System;

public class ParentMeeting : IAggregateRoot
{
    // EF constructor
    private ParentMeeting()
    {
        this.Title = null!;
    }

    public ParentMeeting(
        int schoolYear,
        int classBookId,
        DateTime date,
        TimeSpan startTime,
        string? location,
        string title,
        string? description,
        int createdBySysUserId)
    {
        this.SchoolYear = schoolYear;
        this.ClassBookId = classBookId;
        this.Date = date;
        this.StartTime = startTime;
        this.Location = location;
        this.Title = title;
        this.Description = description;

        var now = DateTime.Now;
        this.CreateDate = now;
        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifyDate = now;
        this.ModifiedBySysUserId = createdBySysUserId;
    }

    public int SchoolYear { get; private set; }

    public int ParentMeetingId { get; private set; }

    public int ClassBookId { get; private set; }

    public DateTime Date { get; private set; }

    public TimeSpan StartTime { get; private set; }

    public string? Location { get; private set; }

    public string Title { get; private set; }

    public string? Description { get; private set; }

    public DateTime CreateDate { get; private set; }

    public int CreatedBySysUserId { get; private set; }

    public DateTime ModifyDate { get; internal set; }

    public int ModifiedBySysUserId { get; private set; }

    public byte[] Version { get; private set; } = null!;

    public void UpdateData(DateTime date, TimeSpan startTime, string? location, string title, string? description, int modifiedBySysUserId)
    {
        this.Date = date;
        this.StartTime = startTime;
        this.Location = location;
        this.Title = title;
        this.Description = description;

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;
    }
}
