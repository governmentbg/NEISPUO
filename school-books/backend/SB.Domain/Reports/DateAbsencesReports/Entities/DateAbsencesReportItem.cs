namespace SB.Domain;

public class DateAbsencesReportItem
{
    // EF constructor
    private DateAbsencesReportItem()
    {
        this.ClassBookName = null!;
    }

    public DateAbsencesReportItem(
        int classBookId,
        string classBookName,
        int? shiftId,
        string? shiftName,
        int hourNumber,
        string? absenceStudentNumbers,
        int absenceStudentCount,
        bool isOffDay,
        bool hasScheduleDate)
    {
        this.ClassBookId = classBookId;
        this.ClassBookName = classBookName;
        this.ShiftId = shiftId;
        this.ShiftName = shiftName;
        this.HourNumber = hourNumber;
        this.AbsenceStudentNumbers = absenceStudentNumbers;
        this.AbsenceStudentCount = absenceStudentCount;
        this.IsOffDay = isOffDay;
        this.HasScheduleDate = hasScheduleDate;
    }

    public int DateAbsencesReportId { get; private set; }

    public int SchoolYear { get; private set; }

    public int ClassBookId { get; private set; }

    public string ClassBookName { get; private set; }

    public int? ShiftId { get; private set; }

    public string? ShiftName { get; private set; }

    public int HourNumber { get; private set; }

    public string? AbsenceStudentNumbers { get; private set; }

    public int AbsenceStudentCount { get; private set; }

    public int DateAbsencesReportItemId { get; private set; }

    public bool IsOffDay { get; private set; }

    public bool HasScheduleDate { get; private set; }
}
