namespace SB.Domain;

public class AbsencesByClassesReportItem
{
    // EF constructor
    private AbsencesByClassesReportItem()
    {
        this.ClassBookName = null!;
    }

    public AbsencesByClassesReportItem(
        string classBookName,
        int studentsCount,
        int excusedAbsencesCount,
        decimal excusedAbsencesCountAverage,
        decimal unexcusedAbsencesCount,
        decimal unexcusedAbsencesCountAverage,
        bool isTotal)
    {
        this.ClassBookName = classBookName;
        this.StudentsCount = studentsCount;
        this.ExcusedAbsencesCount = excusedAbsencesCount;
        this.ExcusedAbsencesCountAverage = excusedAbsencesCountAverage;
        this.UnexcusedAbsencesCount = unexcusedAbsencesCount;
        this.UnexcusedAbsencesCountAverage = unexcusedAbsencesCountAverage;
        this.IsTotal = isTotal;
    }

    public int AbsencesByClassesReportId { get; private set; }

    public int SchoolYear { get; private set; }

    public string ClassBookName { get; private set; }

    public int StudentsCount { get; private set; }

    public int ExcusedAbsencesCount { get; private set; }

    public decimal ExcusedAbsencesCountAverage { get; private set; }

    public decimal UnexcusedAbsencesCount { get; private set; }

    public decimal UnexcusedAbsencesCountAverage { get; private set; }

    public bool IsTotal { get; private set; }

    public int AbsencesByClassesReportItemId { get; private set; }
}
