namespace SB.Domain;

using System.Collections.Generic;
using System.Linq;
using System;
using static SB.Domain.IFinalGradePointAverageByClassesReportsQueryRepository;

public class FinalGradePointAverageByClassesReport : IAggregateRoot
{
    // EF constructor
    private FinalGradePointAverageByClassesReport()
    {
        this.Version = null!;
    }

    public FinalGradePointAverageByClassesReport(
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
        this.items.AddRange(items.Select(i => new FinalGradePointAverageByClassesReportItem(
            i.ClassBookName,
            i.CurriculumInfo,
            i.StudentsCount,
            i.StudentsWithGradesCount,
            i.StudentsWithGradesPercentage,
            i.GradePointAverage,
            i.TotalGradesCount,
            i.PoorGradesCount,
            i.FairGradesCount,
            i.GoodGradesCount,
            i.VeryGoodGradesCount,
            i.ExcellentGradesCount,
            i.IsTotal)));

        this.CreateDate = createDate;
        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifyDate = createDate;
        this.ModifiedBySysUserId = createdBySysUserId;
        this.Version = null!;
    }

    public int SchoolYear { get; private set; }

    public int FinalGradePointAverageByClassesReportId { get; private set; }

    public int InstId { get; private set; }

    public ReportPeriod Period { get; private set; }

    public string? ClassBookNames { get; private set; }

    public DateTime CreateDate { get; private set; }

    public int CreatedBySysUserId { get; private set; }

    public DateTime ModifyDate { get; private set; }

    public int ModifiedBySysUserId { get; private set; }

    public byte[] Version { get; private set; }

    // relations
    private readonly List<FinalGradePointAverageByClassesReportItem> items = new();
    public IReadOnlyCollection<FinalGradePointAverageByClassesReportItem> Items => this.items.AsReadOnly();
}
