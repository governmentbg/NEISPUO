namespace SB.Domain;
using System;

public class ReplrParticipation : IAggregateRoot
{
    // EF constructor
    private ReplrParticipation()
    {
        this.Attendees = null!;
        this.Version = null!;
    }

    public ReplrParticipation(
        int schoolYear,
        int classBookId,
        int replrParticipationTypeId,
        DateTime date,
        string? topic,
        int? instId,
        string attendees,
        int createdBySysUserId)
    {
        this.SchoolYear = schoolYear;
        this.ClassBookId = classBookId;
        this.ReplrParticipationTypeId = replrParticipationTypeId;
        this.Date = date;
        this.Topic = topic;
        this.InstId = instId;
        this.Attendees = attendees;

        var now = DateTime.Now;
        this.CreateDate = now;
        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifyDate = now;
        this.ModifiedBySysUserId = createdBySysUserId;
        this.Version = null!;
    }

    public int SchoolYear { get; private set; }
    public int ReplrParticipationId { get; private set; }
    public int ClassBookId { get; private set; }
    public int ReplrParticipationTypeId { get; private set; }
    public DateTime Date { get; private set; }
    public string? Topic { get; private set; }
    public int? InstId { get; private set; }
    public string Attendees { get; private set; }
    public DateTime CreateDate { get; private set; }
    public int CreatedBySysUserId { get; private set; }
    public DateTime ModifyDate { get; private set; }
    public int ModifiedBySysUserId { get; private set; }
    public byte[] Version { get; private set; }

    public void UpdateData(
        int replrParticipationTypeId,
        DateTime date,
        string? topic,
        int? instId,
        string attendees,
        int modifiedBySysUserId)
    {
        this.ReplrParticipationTypeId = replrParticipationTypeId;
        this.Date = date;
        this.Topic = topic;
        this.InstId = instId;
        this.Attendees = attendees;

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;
    }
}
