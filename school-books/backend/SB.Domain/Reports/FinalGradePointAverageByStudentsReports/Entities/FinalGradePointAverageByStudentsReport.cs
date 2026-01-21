namespace SB.Domain;

using System.Collections.Generic;
using System.Linq;
using System;
using static SB.Domain.IFinalGradePointAverageByStudentsReportsQueryRepository;

public class FinalGradePointAverageByStudentsReport : IAggregateRoot
{
    // EF constructor
    private FinalGradePointAverageByStudentsReport()
    {
        this.Version = null!;
    }

    public FinalGradePointAverageByStudentsReport(
        int schoolYear,
        int instId,
        ReportPeriod period,
        string? classBookNames,
        decimal? minimumGradePointAverage,
        GetItemsForAddVO[] items,
        DateTime createDate,
        int createdBySysUserId)
    {
        this.SchoolYear = schoolYear;
        this.InstId = instId;
        this.Period = period;

        this.ClassBookNames = classBookNames;
        this.MinimumGradePointAverage = minimumGradePointAverage;
        this.items.AddRange(items.Select(i => new FinalGradePointAverageByStudentsReportItem(
            i.ClassBookName,
            i.StudentName,
            i.IsTransferred,
            i.FinalGradePointAverage)));

        this.CreateDate = createDate;
        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifyDate = createDate;
        this.ModifiedBySysUserId = createdBySysUserId;
        this.Version = null!;
    }

    public int SchoolYear { get; private set; }

    public int FinalGradePointAverageByStudentsReportId { get; private set; }

    public int InstId { get; private set; }

    public ReportPeriod Period { get; private set; }

    public string? ClassBookNames { get; private set; }

    public decimal? MinimumGradePointAverage { get; init; }

    public DateTime CreateDate { get; private set; }

    public int CreatedBySysUserId { get; private set; }

    public DateTime ModifyDate { get; private set; }

    public int ModifiedBySysUserId { get; private set; }

    public byte[] Version { get; private set; }

    // relations
    private readonly List<FinalGradePointAverageByStudentsReportItem> items = new();
    public IReadOnlyCollection<FinalGradePointAverageByStudentsReportItem> Items => this.items.AsReadOnly();
}
