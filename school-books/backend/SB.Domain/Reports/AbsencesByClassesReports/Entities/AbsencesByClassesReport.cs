namespace SB.Domain;

using System.Collections.Generic;
using System.Linq;
using System;
using static SB.Domain.IAbsencesByClassesReportsQueryRepository;

public class AbsencesByClassesReport : IAggregateRoot
{
    // EF constructor
    private AbsencesByClassesReport()
    {
        this.Version = null!;
    }

    public AbsencesByClassesReport(
        int schoolYear,
        int instId,
        ReportPeriod period,
        string? classBookNames,
        GetItemsForAddVO[] items,
        DateTime createDate,
        int createdBySysUserId)
    {
        this.SchoolYear = schoolYear;
        this.InstId = instId;
        this.Period = period;

        this.ClassBookNames = classBookNames;
        this.items.AddRange(items.Select(i => new AbsencesByClassesReportItem(
            i.ClassBookName,
            i.StudentsCount,
            i.ExcusedAbsencesCount,
            i.ExcusedAbsencesCountAverage,
            i.UnexcusedAbsencesCount,
            i.UnexcusedAbsencesCountAverage,
            i.IsTotal)));

        this.CreateDate = createDate;
        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifyDate = createDate;
        this.ModifiedBySysUserId = createdBySysUserId;
        this.Version = null!;
    }

    public int SchoolYear { get; private set; }

    public int AbsencesByClassesReportId { get; private set; }

    public int InstId { get; private set; }

    public ReportPeriod Period { get; private set; }

    public string? ClassBookNames { get; private set; }

    public DateTime CreateDate { get; private set; }

    public int CreatedBySysUserId { get; private set; }

    public DateTime ModifyDate { get; private set; }

    public int ModifiedBySysUserId { get; private set; }

    public byte[] Version { get; private set; }

    // relations

    private readonly List<AbsencesByClassesReportItem> items = new();
    public IReadOnlyCollection<AbsencesByClassesReportItem> Items => this.items.AsReadOnly();
}
