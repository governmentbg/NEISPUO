namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.Linq;
using static SB.Domain.IScheduleAndAbsencesByMonthReportsQueryRepository;

public class ScheduleAndAbsencesByMonthReport : IAggregateRoot
{
    // EF constructor
    private ScheduleAndAbsencesByMonthReport()
    {
        this.Version = null!;
        this.ClassBookName = null!;
    }

    public ScheduleAndAbsencesByMonthReport(
        int schoolYear,
        int instId,
        int year,
        int month,
        string classBookName,
        bool isDPLR,
        GetWeeksForAddVO[] weeks,
        DateTime createDate,
        int createdBySysUserId)
    {
        this.SchoolYear = schoolYear;
        this.InstId = instId;
        this.Year = year;
        this.Month = month;
        this.ClassBookName = classBookName;
        this.IsDPLR = isDPLR;

        this.weeks.AddRange(weeks.Select(i => new ScheduleAndAbsencesByMonthReportWeek(i.WeekName, i.StudentName, i.AdditionalActivities, i.Days)));

        this.CreateDate = createDate;
        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifyDate = createDate;
        this.ModifiedBySysUserId = createdBySysUserId;
        this.Version = null!;
    }

    public int SchoolYear { get; private set; }

    public int ScheduleAndAbsencesByMonthReportId { get; private set; }

    public int InstId { get; private set; }

    public int Year { get; private set; }

    public int Month { get; private set; }

    public string ClassBookName { get; private set; }

    public bool IsDPLR { get; private set; }

    public DateTime CreateDate { get; private set; }

    public int CreatedBySysUserId { get; private set; }

    public DateTime ModifyDate { get; private set; }

    public int ModifiedBySysUserId { get; private set; }

    public byte[] Version { get; private set; }

    // relations
    private readonly List<ScheduleAndAbsencesByMonthReportWeek> weeks = new();
    public IReadOnlyCollection<ScheduleAndAbsencesByMonthReportWeek> Weeks => this.weeks.AsReadOnly();
}
