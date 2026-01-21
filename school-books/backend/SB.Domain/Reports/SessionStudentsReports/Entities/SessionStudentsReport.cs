namespace SB.Domain;

using System.Linq;
using System;
using System.Collections.Generic;
using static SB.Domain.ISessionStudentsReportsQueryRepository;

public class SessionStudentsReport : IAggregateRoot
{
    // EF constructor
    public SessionStudentsReport()
    {
        this.Version = null!;
    }

    public SessionStudentsReport(
        int schoolYear,
        int instId,
        GetItemsForAddVO[] items,
        DateTime createDate,
        int createdBySysUserId)
    {
        this.SchoolYear = schoolYear;
        this.InstId = instId;

        this.items.AddRange(
            items.Select(
                i => new SessionStudentsReportItem(
                    i.StudentNames,
                    i.ClassBookName,
                    i.IsTransferred,
                    i.Session1CurriculumNames,
                    i.Session2CurriculumNames,
                    i.Session3CurriculumNames)));

        this.CreateDate = createDate;
        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifyDate = createDate;
        this.ModifiedBySysUserId = createdBySysUserId;
        this.Version = null!;
    }

    public int SchoolYear { get; private set; }

    public int SessionStudentsReportId { get; private set; }

    public int InstId { get; private set; }

    public DateTime CreateDate { get; private set; }

    public int CreatedBySysUserId { get; private set; }

    public DateTime ModifyDate { get; private set; }

    public int ModifiedBySysUserId { get; private set; }

    public byte[] Version { get; private set; }

    // relations
    private readonly List<SessionStudentsReportItem> items = new();
    public IReadOnlyCollection<SessionStudentsReportItem> Items => this.items.AsReadOnly();
}
