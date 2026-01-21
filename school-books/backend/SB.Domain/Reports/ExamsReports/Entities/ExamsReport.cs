namespace SB.Domain;

using System.Linq;
using System;
using System.Collections.Generic;
using static SB.Domain.IExamsReportsQueryRepository;

public class ExamsReport : IAggregateRoot
{
    // EF constructor
    public ExamsReport()
    {
        this.Version = null!;
    }

    public ExamsReport(
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
                i => new ExamsReportItem(
                    i.Date,
                    i.ClassBookName,
                    i.BookExamType,
                    i.CurriculumName,
                    i.CreatedBySysUserName)));

        this.CreateDate = createDate;
        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifyDate = createDate;
        this.ModifiedBySysUserId = createdBySysUserId;
        this.Version = null!;
    }

    public int SchoolYear { get; private set; }

    public int ExamsReportId { get; private set; }

    public int InstId { get; private set; }

    public DateTime CreateDate { get; private set; }

    public int CreatedBySysUserId { get; private set; }

    public DateTime ModifyDate { get; private set; }

    public int ModifiedBySysUserId { get; private set; }

    public byte[] Version { get; private set; }

    // relations
    private readonly List<ExamsReportItem> items = new();
    public IReadOnlyCollection<ExamsReportItem> Items => this.items.AsReadOnly();
}
