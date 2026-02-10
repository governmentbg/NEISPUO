namespace SB.Domain;

public class MissingTopicsReportItemTeacher
{
    // EF constructor
    private MissingTopicsReportItemTeacher()
    {
        this.PersonName = null!;
    }

    internal MissingTopicsReportItemTeacher(string personName)
    {
        this.PersonName = personName;
    }

    public int SchoolYear { get; private set; }
    public int MissingTopicsReportId { get; private set; }
    public int MissingTopicsReportItemId { get; private set; }
    public int MissingTopicsReportItemTeacherId { get; private set; }
    public string PersonName { get; private set; }
}
