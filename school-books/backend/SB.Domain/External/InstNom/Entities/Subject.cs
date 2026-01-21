namespace SB.Domain;

public class Subject
{
    public const int SchoolLeadTeacherHour = 199;
    public const int LastInternalSubjectId = 199;

    // EF constructor
    private Subject()
    {
        this.SubjectName = null!;
    }

    // only used properties should be mapped

    public int SubjectId { get; private set; }

    public string SubjectName { get; private set; }

    public string? SubjectNameShort { get; private set; }

    public bool? IsValid { get; private set; }
}
