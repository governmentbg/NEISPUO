namespace SB.Domain;

using System;

public class ScheduleAndAbsencesByTermAllClassesReport : IAggregateRoot
{
    // EF constructor
    private ScheduleAndAbsencesByTermAllClassesReport()
    {
        this.Version = null!;
    }

    public ScheduleAndAbsencesByTermAllClassesReport(
        int schoolYear,
        int instId,
        SchoolTerm term,
        int blobId,
        DateTime createDate,
        int createdBySysUserId)
    {
        this.SchoolYear = schoolYear;
        this.InstId = instId;
        this.Term = term;
        this.BlobId = blobId;

        this.CreateDate = createDate;
        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifyDate = createDate;
        this.ModifiedBySysUserId = createdBySysUserId;
        this.Version = null!;
    }

    public int SchoolYear { get; private set; }

    public int ScheduleAndAbsencesByTermAllClassesReportId { get; private set; }

    public int InstId { get; private set; }

    public SchoolTerm Term { get; private set; }

    public int BlobId { get; private set; }

    public DateTime CreateDate { get; private set; }

    public int CreatedBySysUserId { get; private set; }

    public DateTime ModifyDate { get; private set; }

    public int ModifiedBySysUserId { get; private set; }

    public byte[] Version { get; private set; }
}
