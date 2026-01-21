namespace SB.Domain;

public class FinalGradePointAverageByStudentsReportItem
{
    // EF constructor
    private FinalGradePointAverageByStudentsReportItem()
    {
        this.ClassBookName = null!;
        this.StudentNames = null!;
    }

    public FinalGradePointAverageByStudentsReportItem(
        string classBookName,
        string studentNames,
        bool isTransferred,
        decimal finalGradePointAverage)
    {
        this.ClassBookName = classBookName;
        this.StudentNames = studentNames;
        this.IsTransferred = isTransferred;
        this.FinalGradePointAverage = finalGradePointAverage;
    }

    public int FinalGradePointAverageByStudentsReportId { get; private set; }

    public int FinalGradePointAverageByStudentsReportItemId { get; private set; }

    public int SchoolYear { get; private set; }

    public string ClassBookName { get; private set; }

    public string StudentNames { get; private set; }

    public bool IsTransferred { get; private set; }

    public decimal FinalGradePointAverage { get; private set; }
}
