namespace SB.Domain;

using System;

public class IndividualWork : IAggregateRoot
{
    // EF constructor
    private IndividualWork()
    {
        this.IndividualWorkActivity = null!;
        this.Version = null!;
    }

    public IndividualWork(
        int schoolYear,
        int classBookId,
        int personId,
        DateTime date,
        string individualWorkActivity,
        int createdBySysUserId)
    {
        this.SchoolYear = schoolYear;
        this.ClassBookId = classBookId;
        this.PersonId = personId;
        this.Date = date;
        this.IndividualWorkActivity = individualWorkActivity;

        var now = DateTime.Now;
        this.CreateDate = now;
        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifyDate = now;
        this.ModifiedBySysUserId = createdBySysUserId;
        this.Version = null!;
    }

    public int SchoolYear { get; private set; }
    public int IndividualWorkId { get; private set; }
    public int ClassBookId { get; private set; }
    public int PersonId { get; private set; }
    public DateTime Date { get; private set; }
    public string IndividualWorkActivity { get; private set; }
    public DateTime CreateDate { get; private set; }
    public int CreatedBySysUserId { get; private set; }
    public DateTime ModifyDate { get; private set; }
    public int ModifiedBySysUserId { get; private set; }
    public byte[] Version { get; private set; }

    public void UpdateData(
        DateTime date,
        string individualWorkActivity,
        int modifiedBySysUserId)
    {
        this.Date = date;
        this.IndividualWorkActivity = individualWorkActivity;

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;
    }
}
