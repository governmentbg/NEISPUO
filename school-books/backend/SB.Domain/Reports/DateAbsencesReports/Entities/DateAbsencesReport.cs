namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.Linq;
using static SB.Domain.IDateAbsencesReportsQueryRepository;

public class DateAbsencesReport : IAggregateRoot
{
    // EF constructor
    private DateAbsencesReport()
    {
        this.Version = null!;
    }

    public DateAbsencesReport(
        int schoolYear,
        int instId,
        DateTime reportDate,
        bool isUnited,
        string? classBookNames,
        string? shiftNames,
        GetItemsForAddVO[] items,
        DateTime createDate,
        int createdBySysUserId)
    {
        this.SchoolYear = schoolYear;
        this.InstId = instId;
        this.ReportDate = reportDate;
        this.IsUnited = isUnited;

        this.ClassBookNames = classBookNames;
        this.ShiftNames = shiftNames;
        this.items.AddRange(items.Select(i => new DateAbsencesReportItem(
            i.ClassBookId,
            i.ClassBookName,
            i.ShiftId,
            i.ShiftName,
            i.HourNumber,
            i.AbsenceStudentNumbers,
            i.AbsenceStudentCount,
            i.IsOffDay,
            i.HasScheduleDate)));

        this.CreateDate = createDate;
        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifyDate = createDate;
        this.ModifiedBySysUserId = createdBySysUserId;
        this.Version = null!;
    }

    public int SchoolYear { get; private set; }

    public int DateAbsencesReportId { get; private set; }

    public int InstId { get; private set; }

    public DateTime ReportDate { get; private set; }

    public bool IsUnited { get; private set; }

    public string? ClassBookNames { get; private set; }

    public string? ShiftNames { get; private set; }

    public DateTime CreateDate { get; private set; }

    public int CreatedBySysUserId { get; private set; }

    public DateTime ModifyDate { get; private set; }

    public int ModifiedBySysUserId { get; private set; }

    public byte[] Version { get; private set; }

    // relations

    private readonly List<DateAbsencesReportItem> items = new();
    public IReadOnlyCollection<DateAbsencesReportItem> Items => this.items.AsReadOnly();
}
