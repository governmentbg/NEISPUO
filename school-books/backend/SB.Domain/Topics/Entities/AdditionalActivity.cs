namespace SB.Domain;

using System;

public class AdditionalActivity : IAggregateRoot
{
    // EF constructor
    private AdditionalActivity()
    {
        this.Activity = null!;
    }

    public AdditionalActivity(
        int schoolYear,
        int classBookId,
        int year,
        int weekNumber,
        string activity,
        int createdBySysUserId)
    {
        this.SchoolYear = schoolYear;
        this.ClassBookId = classBookId;
        this.Year = year;
        this.WeekNumber = weekNumber;
        this.Activity = activity;

        var now = DateTime.Now;
        this.CreateDate = now;
        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifyDate = now;
        this.ModifiedBySysUserId = createdBySysUserId;
    }

    public int SchoolYear { get; private set; }

    public int AdditionalActivityId { get; private set; }

    public int ClassBookId { get; private set; }

    public int Year { get; private set; }

    public int WeekNumber { get; private set; }

    public string Activity { get; private set; }

    public DateTime CreateDate { get; internal set; }

    public int CreatedBySysUserId { get; private set; }

    public DateTime ModifyDate { get; internal set; }

    public int ModifiedBySysUserId { get; private set; }

    public byte[] Version { get; private set; } = null!;

    public void UpdateData(string activity, int modifiedBySysUserId)
    {
        this.Activity = activity;

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;
    }
}
