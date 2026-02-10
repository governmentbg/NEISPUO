namespace SB.Domain;

public class SessionStudentsReportItem
{
    // EF constructor
    public SessionStudentsReportItem()
    {
        this.StudentNames = null!;
        this.ClassBookName = null!;
    }

    public SessionStudentsReportItem(
        string studentNames,
        string classBookName,
        bool isTransferred,
        string? session1CurriculumNames,
        string? session2CurriculumNames,
        string? session3CurriculumNames)
    {
        this.StudentNames = studentNames;
        this.ClassBookName = classBookName;
        this.IsTransferred = isTransferred;
        this.Session1CurriculumNames = session1CurriculumNames;
        this.Session2CurriculumNames = session2CurriculumNames;
        this.Session3CurriculumNames = session3CurriculumNames;
    }

    public int SchoolYear { get; private set; }

    public int SessionStudentsReportId { get; private set; }

    public int SessionStudentsReportItemId { get; private set; }

    public string StudentNames { get; private set; }

    public bool IsTransferred { get; private set; }

    public string ClassBookName { get; private set; }

    public string? Session1CurriculumNames { get; set; }

    public string? Session2CurriculumNames { get; set; }

    public string? Session3CurriculumNames { get; set; }
}
