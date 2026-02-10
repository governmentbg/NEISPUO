namespace SB.Domain;

public class AbsencesByStudentsReportItem
{
    // EF constructor
    private AbsencesByStudentsReportItem()
    {
        this.ClassBookName = null!;
        this.StudentName = null!;
    }

    public AbsencesByStudentsReportItem(
        string classBookName,
        string studentName,
        bool isTransferred,
        int excusedAbsencesCount,
        int unexcusedAbsencesCount,
        int lateAbsencesCount,
        bool isTotal)
    {
        this.ClassBookName = classBookName;
        this.StudentName = studentName;
        this.IsTransferred = isTransferred;
        this.ExcusedAbsencesCount = excusedAbsencesCount;
        this.UnexcusedAbsencesCount = unexcusedAbsencesCount;
        this.LateAbsencesCount = lateAbsencesCount;
        this.IsTotal = isTotal;
    }

    public int AbsencesByStudentsReportId { get; private set; }

    public int SchoolYear { get; private set; }

    public string ClassBookName { get; private set; }

    public string StudentName { get; private set; }

    public bool IsTransferred { get; private set; }

    public int ExcusedAbsencesCount { get; private set; }

    public int UnexcusedAbsencesCount { get; private set; }

    public int LateAbsencesCount { get; private set; }

    public bool IsTotal { get; private set; }

    public int AbsencesByStudentsReportItemId { get; private set; }
}
