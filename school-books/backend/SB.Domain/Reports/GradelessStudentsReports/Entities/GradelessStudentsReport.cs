namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.Linq;
using static SB.Domain.IGradelessStudentsReportsQueryRepository;

public class GradelessStudentsReport : IAggregateRoot
{
    // EF constructor
    private GradelessStudentsReport()
    {
        this.Version = null!;
    }

    public GradelessStudentsReport(
        int schoolYear,
        int instId,
        bool onlyFinalGrades,
        ReportPeriod period,
        string? classBookNames,
        GetItemsForAddVO[] items,
        DateTime createDate,
        int createdBySysUserId)
    {
        this.SchoolYear = schoolYear;
        this.InstId = instId;
        this.OnlyFinalGrades = onlyFinalGrades;
        this.Period = period;

        this.ClassBookNames = classBookNames;
        this.items.AddRange(items.Select(i => new GradelessStudentsReportItem(i.ClassBookName, i.StudentName, i.CurriculumName)));

        this.CreateDate = createDate;
        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifyDate = createDate;
        this.ModifiedBySysUserId = createdBySysUserId;
        this.Version = null!;
    }

    public int SchoolYear { get; private set; }

    public int GradelessStudentsReportId { get; private set; }

    public int InstId { get; private set; }

    public ReportPeriod Period { get; private set; }

    public string? ClassBookNames { get; private set; }

    public bool OnlyFinalGrades { get; private set; }

    public DateTime CreateDate { get; private set; }

    public int CreatedBySysUserId { get; private set; }

    public DateTime ModifyDate { get; private set; }

    public int ModifiedBySysUserId { get; private set; }

    public byte[] Version { get; private set; }

    // relations

    private readonly List<GradelessStudentsReportItem> items = new();
    public IReadOnlyCollection<GradelessStudentsReportItem> Items => this.items.AsReadOnly();
}
