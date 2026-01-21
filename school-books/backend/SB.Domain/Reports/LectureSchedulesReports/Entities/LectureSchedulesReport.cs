namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.Linq;
using static SB.Domain.ILectureSchedulesReportsQueryRepository;

public class LectureSchedulesReport : IAggregateRoot
{
    // EF constructor
    private LectureSchedulesReport()
    {
        this.Version = null!;
    }

    public LectureSchedulesReport(
        int schoolYear,
        int instId,
        LectureSchedulesReportPeriod period,
        int? year,
        int? month,
        int? teacherPersonId,
        string? teacherPersonName,
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
        this.TeacherPersonName = teacherPersonName;

        this.items.AddRange(
            items.Select(
                i => new LectureSchedulesReportItem(
                    i.Date,
                    i.TeacherPersonId,
                    i.TeacherPersonName,
                    i.ClassBookId,
                    i.ClassBookName,
                    i.CurriculumId,
                    i.CurriculumName,
                    i.LectureScheduleId,
                    i.OrderNumber,
                    i.OrderDate,
                    i.HoursTaken)));

        this.CreateDate = createDate;
        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifyDate = createDate;
        this.ModifiedBySysUserId = createdBySysUserId;
        this.Version = null!;
    }

    public int SchoolYear { get; private set; }

    public int LectureSchedulesReportId { get; private set; }

    public int InstId { get; private set; }

    public LectureSchedulesReportPeriod Period { get; private set; }

    public int? Year { get; private set; }

    public int? Month { get; private set; }

    public int? TeacherPersonId { get; private set; }

    public string? TeacherPersonName { get; private set; }

    public DateTime CreateDate { get; private set; }

    public int CreatedBySysUserId { get; private set; }

    public DateTime ModifyDate { get; private set; }

    public int ModifiedBySysUserId { get; private set; }

    public byte[] Version { get; private set; }

    // relations
    private readonly List<LectureSchedulesReportItem> items = new();
    public IReadOnlyCollection<LectureSchedulesReportItem> Items => this.items.AsReadOnly();
}
