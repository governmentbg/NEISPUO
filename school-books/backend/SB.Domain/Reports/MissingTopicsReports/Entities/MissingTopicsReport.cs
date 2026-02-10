namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.Linq;
using static SB.Domain.IMissingTopicsReportsQueryRepository;

public class MissingTopicsReport : IAggregateRoot
{
    // EF constructor
    private MissingTopicsReport()
    {
        this.Version = null!;
    }

    public MissingTopicsReport(
        int schoolYear,
        int instId,
        MissingTopicsReportPeriod period,
        int? year,
        int? month,
        int? teacherPersonId,
        GetItemsForAddVO[] items,
        DateTime createDate,
        int createdBySysUserId)
    {
        this.SchoolYear = schoolYear;
        this.InstId = instId;
        this.Period = period;
        this.Year = year;
        this.Month = month;
        this.TeacherPersonId = teacherPersonId;

        this.items.AddRange(items.Select(i => new MissingTopicsReportItem(i.Date, i.ClassBookName, i.CurriculumName, i.TeacherNames)));

        this.CreateDate = createDate;
        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifyDate = createDate;
        this.ModifiedBySysUserId = createdBySysUserId;
        this.Version = null!;
    }

    public int SchoolYear { get; private set; }

    public int MissingTopicsReportId { get; private set; }

    public int InstId { get; private set; }

    public MissingTopicsReportPeriod Period { get; private set; }

    public int? Year { get; private set; }

    public int? Month { get; private set; }

    public int? TeacherPersonId { get; private set; }

    public DateTime CreateDate { get; private set; }

    public int CreatedBySysUserId { get; private set; }

    public DateTime ModifyDate { get; private set; }

    public int ModifiedBySysUserId { get; private set; }

    public byte[] Version { get; private set; }

    // relations
    private readonly List<MissingTopicsReportItem> items = new();
    public IReadOnlyCollection<MissingTopicsReportItem> Items => this.items.AsReadOnly();
}
