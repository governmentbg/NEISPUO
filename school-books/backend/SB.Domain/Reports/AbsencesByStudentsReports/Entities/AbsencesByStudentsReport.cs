namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.Linq;
using static SB.Domain.IAbsencesByStudentsReportsQueryRepository;

public class AbsencesByStudentsReport : IAggregateRoot
{
    // EF constructor
    private AbsencesByStudentsReport()
    {
        this.Version = null!;
    }

    public AbsencesByStudentsReport(
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
        this.items.AddRange(items.Select(i => new AbsencesByStudentsReportItem(
            i.ClassBookName,
            i.StudentName,
            i.IsTransferred,
            i.ExcusedAbsencesCount,
            i.UnexcusedAbsencesCount,
            i.LateAbsencesCount,
            i.IsTotal)));

        this.CreateDate = createDate;
        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifyDate = createDate;
        this.ModifiedBySysUserId = createdBySysUserId;
        this.Version = null!;
    }

    public int SchoolYear { get; private set; }

    public int AbsencesByStudentsReportId { get; private set; }

    public int InstId { get; private set; }

    public ReportPeriod Period { get; private set; }

    public string? ClassBookNames { get; private set; }

    public DateTime CreateDate { get; private set; }

    public int CreatedBySysUserId { get; private set; }

    public DateTime ModifyDate { get; private set; }

    public int ModifiedBySysUserId { get; private set; }

    public byte[] Version { get; private set; }

    // relations

    private readonly List<AbsencesByStudentsReportItem> items = new();
    public IReadOnlyCollection<AbsencesByStudentsReportItem> Items => this.items.AsReadOnly();
}
