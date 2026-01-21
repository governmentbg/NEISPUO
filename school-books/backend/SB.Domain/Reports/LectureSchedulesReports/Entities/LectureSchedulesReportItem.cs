namespace SB.Domain;

using System;
public class LectureSchedulesReportItem
{
    // EF constructor
    private LectureSchedulesReportItem()
    {
        this.ClassBookName = null!;
        this.CurriculumName = null!;
        this.OrderNumber = null!;
        this.TeacherPersonName = null!;
    }

    public LectureSchedulesReportItem(
        DateTime date,
        int teacherPersonId,
        string teacherPersonName,
        int classBookId,
        string classBookName,
        int curriculumId,
        string curriculumName,
        int lectureScheduleId,
        string orderNumber,
        DateTime orderDate,
        int hoursTaken)
    {
        this.Date = date;
        this.TeacherPersonId = teacherPersonId;
        this.TeacherPersonName = teacherPersonName;
        this.ClassBookId = classBookId;
        this.ClassBookName = classBookName;
        this.CurriculumId = curriculumId;
        this.CurriculumName = curriculumName;
        this.LectureScheduleId = lectureScheduleId;
        this.OrderNumber = orderNumber;
        this.OrderDate = orderDate;
        this.HoursTaken = hoursTaken;
    }

    public int SchoolYear { get; private set; }

    public int LectureSchedulesReportId { get; private set; }

    public int LectureSchedulesReportItemId { get; private set; }

    public DateTime Date { get; private set; }

    public int TeacherPersonId { get; private set; }

    public string TeacherPersonName { get; private set; }

    public int ClassBookId { get; private set; }

    public string ClassBookName { get; private set; }

    public int CurriculumId { get; private set; }

    public string CurriculumName { get; private set; }

    public int LectureScheduleId { get; private set; }

    public string OrderNumber { get; private set; }

    public DateTime OrderDate { get; private set; }

    public int HoursTaken { get; private set; }
}
