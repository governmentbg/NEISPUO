namespace SB.Domain;

using System.Linq;
using System;
using System.Collections.Generic;
using static SB.Domain.IStudentsAtRiskOfDroppingOutReportsQueryRepository;

public class StudentsAtRiskOfDroppingOutReport : IAggregateRoot
{
    // EF constructor
    public StudentsAtRiskOfDroppingOutReport()
    {
        this.Version = null!;
    }

    public StudentsAtRiskOfDroppingOutReport(
        int schoolYear,
        int instId,
        DateTime reportDate,
        GetItemsForAddVO[] items,
        DateTime createDate,
        int createdBySysUserId)
    {
        this.SchoolYear = schoolYear;
        this.InstId = instId;
        this.ReportDate = reportDate;

        this.items.AddRange(
            items.Select(
                i => new StudentsAtRiskOfDroppingOutReportItem(
                    i.PersonId,
                    i.PersonalId,
                    i.FirstName,
                    i.MiddleName,
                    i.LastName,
                    i.ClassBookName,
                    i.UnexcusedAbsenceHoursCount,
                    i.UnexcusedAbsenceDaysCount)));

        this.CreateDate = createDate;
        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifyDate = createDate;
        this.ModifiedBySysUserId = createdBySysUserId;
        this.Version = null!;
    }

    public int SchoolYear { get; private set; }

    public int StudentsAtRiskOfDroppingOutReportId { get; private set; }

    public int InstId { get; private set; }

    public DateTime ReportDate { get; private set; }

    public DateTime CreateDate { get; private set; }

    public int CreatedBySysUserId { get; private set; }

    public DateTime ModifyDate { get; private set; }

    public int ModifiedBySysUserId { get; private set; }

    public byte[] Version { get; private set; }

    // relations
    private readonly List<StudentsAtRiskOfDroppingOutReportItem> items = new();
    public IReadOnlyCollection<StudentsAtRiskOfDroppingOutReportItem> Items => this.items.AsReadOnly();
}
