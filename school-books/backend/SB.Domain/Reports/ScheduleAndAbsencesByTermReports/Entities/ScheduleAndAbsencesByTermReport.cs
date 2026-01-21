namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.Linq;
using static SB.Domain.IScheduleAndAbsencesByTermReportsQueryRepository;

public class ScheduleAndAbsencesByTermReport : IAggregateRoot
{
    // EF constructor
    private ScheduleAndAbsencesByTermReport()
    {
        this.Version = null!;
        this.ClassBookName = null!;
    }

    public ScheduleAndAbsencesByTermReport(
        int schoolYear,
        int instId,
        SchoolTerm term,
        string classBookName,
        bool isDPLR,
        GetWeeksForAddVO[] weeks,
        DateTime createDate,
        int createdBySysUserId)
    {
        this.SchoolYear = schoolYear;
        this.InstId = instId;
        this.Term = term;
        this.ClassBookName = classBookName;
        this.IsDPLR = isDPLR;

        this.weeks.AddRange(weeks.Select(i => new ScheduleAndAbsencesByTermReportWeek(i.WeekName, i.StudentName, i.AdditionalActivities, i.Days)));

        this.CreateDate = createDate;
        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifyDate = createDate;
        this.ModifiedBySysUserId = createdBySysUserId;
        this.Version = null!;
    }

    public int SchoolYear { get; private set; }

    public int ScheduleAndAbsencesByTermReportId { get; private set; }

    public int InstId { get; private set; }

    public SchoolTerm Term { get; private set; }

    public string ClassBookName { get; private set; }

    public bool IsDPLR { get; private set; }

    public DateTime CreateDate { get; private set; }

    public int CreatedBySysUserId { get; private set; }

    public DateTime ModifyDate { get; private set; }

    public int ModifiedBySysUserId { get; private set; }

    public byte[] Version { get; private set; }

    // relations
    private readonly List<ScheduleAndAbsencesByTermReportWeek> weeks = new();
    public IReadOnlyCollection<ScheduleAndAbsencesByTermReportWeek> Weeks => this.weeks.AsReadOnly();
}
