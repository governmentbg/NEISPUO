namespace SB.Domain;

public class SubjectType
{
    public const int DefaultSubjectTypeId = -1;
    public static readonly int[] ProfilingSubjectIds = new int[] { 152, 153, 154, 155, 156 };

    // EF constructor
    private SubjectType()
    {
        this.Name = null!;
    }

    // only used properties should be mapped

    public int SubjectTypeId { get; private set; }

    public string Name { get; private set; }

    public bool IsValid { get; private set; }
}
