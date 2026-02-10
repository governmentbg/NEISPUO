namespace SB.Domain;

public class GradelessStudentsReportItem
{
    // EF constructor
    private GradelessStudentsReportItem()
    {
        this.ClassBookName = null!;
        this.StudentName = null!;
        this.CurriculumName = null!;
    }

    public GradelessStudentsReportItem(
        string classBookName,
        string studentName,
        string curriculumName)
    {
        this.ClassBookName = classBookName;
        this.StudentName = studentName;
        this.CurriculumName = curriculumName;
    }

    public int GradelessStudentsReportId { get; private set; }

    public int SchoolYear { get; private set; }

    public string ClassBookName { get; private set; }

    public string StudentName { get; private set; }

    public string CurriculumName { get; private set; }

    public int GradelessStudentsReportItemId { get; private set; }
}
